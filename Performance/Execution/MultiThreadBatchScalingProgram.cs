using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Performance.MultiThreadBatchScaling
{
    internal static class MultiThreadBatchScalingProgram
    {
        private const int WarmupSamples = 3;
        private const int TimedSamples = 7;
        private const int TargetCellOperations = 16_777_216;

        private static readonly int[] CellCounts =
        {
            262_144,
            1_048_576,
            4_194_304
        };

        private static readonly int[] WorkerCounts =
        {
            1,
            2,
            4,
            8
        };

        private readonly struct TimingResult
        {
            public TimingResult(double medianMs, double minMs, double maxMs, double checksum)
            {
                MedianMs = medianMs;
                MinMs = minMs;
                MaxMs = maxMs;
                Checksum = checksum;
            }

            public double MedianMs { get; }
            public double MinMs { get; }
            public double MaxMs { get; }
            public double Checksum { get; }
        }

        private sealed class PersistentWorkerPool : IDisposable
        {
            private readonly ThermodynamicState[] _states;
            private readonly DerivedThermodynamicState[] _destination;
            private readonly CompiledThermodynamicParameters _material;
            private readonly int _passes;
            private readonly Thread[] _threads;
            private readonly Barrier _barrier;
            private readonly object _failureGate = new object();
            private Exception? _failure;
            private volatile bool _stopping;

            public PersistentWorkerPool(
                ThermodynamicState[] states,
                DerivedThermodynamicState[] destination,
                CompiledThermodynamicParameters material,
                int passes,
                int workerCount)
            {
                if (workerCount <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(workerCount));
                }

                _states = states;
                _destination = destination;
                _material = material;
                _passes = passes;
                _threads = new Thread[workerCount];
                _barrier = new Barrier(workerCount + 1);

                ValidatePartitionCoverage(states.Length, workerCount);

                for (var workerIndex = 0; workerIndex < workerCount; workerIndex++)
                {
                    var capturedIndex = workerIndex;
                    var thread = new Thread(() => WorkerLoop(capturedIndex))
                    {
                        IsBackground = true,
                        Name = $"thermocore-perf-worker-{workerIndex}"
                    };
                    _threads[workerIndex] = thread;
                    thread.Start();
                }
            }

            public void RunOnce()
            {
                _barrier.SignalAndWait();
                _barrier.SignalAndWait();

                Exception? failure;
                lock (_failureGate)
                {
                    failure = _failure;
                    _failure = null;
                }

                if (failure != null)
                {
                    throw new InvalidOperationException(
                        "A benchmark worker failed during batch recovery.",
                        failure);
                }
            }

            private void WorkerLoop(int workerIndex)
            {
                var (start, length) = GetPartition(
                    _states.Length,
                    _threads.Length,
                    workerIndex);

                while (true)
                {
                    _barrier.SignalAndWait();

                    if (_stopping)
                    {
                        _barrier.SignalAndWait();
                        return;
                    }

                    try
                    {
                        for (var pass = 0; pass < _passes; pass++)
                        {
                            ReferenceThermodynamicFormulation.RecoverBatch(
                                _states.AsSpan(start, length),
                                _destination.AsSpan(start, length),
                                _material);
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (_failureGate)
                        {
                            _failure ??= ex;
                        }
                    }

                    _barrier.SignalAndWait();
                }
            }

            public void Dispose()
            {
                _stopping = true;
                _barrier.SignalAndWait();
                _barrier.SignalAndWait();

                foreach (var thread in _threads)
                {
                    thread.Join();
                }

                _barrier.Dispose();
            }
        }

        private static int Main()
        {
            try
            {
                var material = BuildMaterial();
                PrintEnvironment();
                RunSemanticGate(material);

                Console.WriteLine(
                    "RESULT_HEADER|scenario|cells|workers|logical_processors|oversubscribed|passes|median_ms|min_ms|max_ms|ns_per_cell|million_cells_per_second|speedup_vs_direct|parallel_efficiency|checksum");

                foreach (var cellCount in CellCounts)
                {
                    RunScalingSet(cellCount, material);
                }

                Console.WriteLine(
                    "Multi-thread batch scaling: COMPLETED — scaling measurements reported.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Multi-thread batch scaling: INVALID — {ex.GetType().Name}: {ex.Message}");
                return 1;
            }
        }

        private static CompiledThermodynamicParameters BuildMaterial()
        {
            var definition = new ReferenceMaterialDefinition(
                materialId: "multithread-scaling-synthetic-reference-v0.1",
                provenance: "Synthetic fixed configuration for CPU multi-thread scaling only",
                referenceDensity: 1000.0,
                densityReferenceTemperature: 300.0,
                energyReferenceTemperature: 250.0,
                meltingTemperature: 300.0,
                latentHeat: 250_000.0,
                solidHeatCapacity: 2_000.0,
                liquidHeatCapacity: 4_000.0);

            return ReferenceMaterialCompiler.Compile(definition);
        }

        private static void RunSemanticGate(CompiledThermodynamicParameters material)
        {
            const int count = 1_048_576;
            var states = CreateStates(count, material);
            var originalEnthalpies = new double[count];
            for (var i = 0; i < states.Length; i++)
            {
                originalEnthalpies[i] = states[i].SpecificEnthalpy;
            }

            var direct = new DerivedThermodynamicState[count];
            ReferenceThermodynamicFormulation.RecoverBatch(states, direct, material);

            foreach (var workers in WorkerCounts)
            {
                ValidatePartitionCoverage(count, workers);
                var parallel = new DerivedThermodynamicState[count];
                using (var pool = new PersistentWorkerPool(
                    states,
                    parallel,
                    material,
                    passes: 1,
                    workerCount: workers))
                {
                    pool.RunOnce();
                }

                var maxTemperatureError = 0.0;
                var maxFractionError = 0.0;
                for (var i = 0; i < count; i++)
                {
                    maxTemperatureError = Math.Max(
                        maxTemperatureError,
                        Math.Abs(direct[i].Temperature - parallel[i].Temperature));
                    maxFractionError = Math.Max(
                        maxFractionError,
                        Math.Abs(direct[i].LiquidPhaseFraction - parallel[i].LiquidPhaseFraction));

                    if (states[i].SpecificEnthalpy != originalEnthalpies[i])
                    {
                        throw new InvalidOperationException(
                            "Persistent Thermodynamic State was modified by a worker path.");
                    }
                }

                Console.WriteLine(
                    $"semantic_gate_workers_{workers}_max_temperature_error: {maxTemperatureError:R}");
                Console.WriteLine(
                    $"semantic_gate_workers_{workers}_max_liquid_fraction_error: {maxFractionError:R}");

                if (maxTemperatureError != 0.0 || maxFractionError != 0.0)
                {
                    throw new InvalidOperationException(
                        $"Worker-count {workers} differed from direct formal RecoverBatch output.");
                }
            }

            Console.WriteLine("multithread_semantic_equivalence_gate: PASS");
            Console.WriteLine("persistent_state_immutability_gate: PASS");
            Console.WriteLine("partition_coverage_gate: PASS");
        }

        private static void RunScalingSet(
            int cellCount,
            CompiledThermodynamicParameters material)
        {
            var passes = Math.Max(1, TargetCellOperations / cellCount);
            var states = CreateStates(cellCount, material);
            var destination = new DerivedThermodynamicState[cellCount];

            var direct = MeasureDirect(states, destination, material, passes);
            PrintResult(
                scenario: "direct_single_thread",
                cellCount,
                workers: 1,
                passes,
                direct,
                direct.MedianMs);

            foreach (var workers in WorkerCounts)
            {
                Array.Clear(destination, 0, destination.Length);
                var parallel = MeasureWorkerPool(
                    states,
                    destination,
                    material,
                    passes,
                    workers);

                PrintResult(
                    scenario: $"worker_pool_{workers}",
                    cellCount,
                    workers,
                    passes,
                    parallel,
                    direct.MedianMs);
            }
        }

        private static TimingResult MeasureDirect(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            CompiledThermodynamicParameters material,
            int passes)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (var i = 0; i < WarmupSamples; i++)
            {
                RunDirect(states, destination, material, passes);
            }

            var samples = new double[TimedSamples];
            for (var sample = 0; sample < TimedSamples; sample++)
            {
                var start = Stopwatch.GetTimestamp();
                RunDirect(states, destination, material, passes);
                var end = Stopwatch.GetTimestamp();
                samples[sample] = (end - start) * 1000.0 / Stopwatch.Frequency;
            }

            var checksum = ComputeChecksum(destination);
            Array.Sort(samples);
            return new TimingResult(
                samples[TimedSamples / 2],
                samples[0],
                samples[TimedSamples - 1],
                checksum);
        }

        private static TimingResult MeasureWorkerPool(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            CompiledThermodynamicParameters material,
            int passes,
            int workers)
        {
            using var pool = new PersistentWorkerPool(
                states,
                destination,
                material,
                passes,
                workers);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (var i = 0; i < WarmupSamples; i++)
            {
                pool.RunOnce();
            }

            var samples = new double[TimedSamples];
            for (var sample = 0; sample < TimedSamples; sample++)
            {
                var start = Stopwatch.GetTimestamp();
                pool.RunOnce();
                var end = Stopwatch.GetTimestamp();
                samples[sample] = (end - start) * 1000.0 / Stopwatch.Frequency;
            }

            var checksum = ComputeChecksum(destination);
            Array.Sort(samples);
            return new TimingResult(
                samples[TimedSamples / 2],
                samples[0],
                samples[TimedSamples - 1],
                checksum);
        }

        private static void RunDirect(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            CompiledThermodynamicParameters material,
            int passes)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                ReferenceThermodynamicFormulation.RecoverBatch(
                    states,
                    destination,
                    material);
            }
        }

        private static void PrintResult(
            string scenario,
            int cellCount,
            int workers,
            int passes,
            TimingResult result,
            double directMedianMs)
        {
            var operations = (long)cellCount * passes;
            var nsPerCell = result.MedianMs * 1_000_000.0 / operations;
            var throughput = operations / (result.MedianMs / 1000.0) / 1_000_000.0;
            var speedup = directMedianMs / result.MedianMs;
            var efficiency = speedup / workers;
            var oversubscribed = workers > Environment.ProcessorCount;

            Console.WriteLine(string.Join('|', new[]
            {
                "RESULT",
                scenario,
                cellCount.ToString(CultureInfo.InvariantCulture),
                workers.ToString(CultureInfo.InvariantCulture),
                Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture),
                oversubscribed ? "true" : "false",
                passes.ToString(CultureInfo.InvariantCulture),
                result.MedianMs.ToString("R", CultureInfo.InvariantCulture),
                result.MinMs.ToString("R", CultureInfo.InvariantCulture),
                result.MaxMs.ToString("R", CultureInfo.InvariantCulture),
                nsPerCell.ToString("R", CultureInfo.InvariantCulture),
                throughput.ToString("R", CultureInfo.InvariantCulture),
                speedup.ToString("R", CultureInfo.InvariantCulture),
                efficiency.ToString("R", CultureInfo.InvariantCulture),
                result.Checksum.ToString("R", CultureInfo.InvariantCulture)
            }));
        }

        private static ThermodynamicState[] CreateStates(
            int count,
            CompiledThermodynamicParameters material)
        {
            var states = new ThermodynamicState[count];
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;
            var latent = material.LatentHeat;

            for (var i = 0; i < states.Length; i++)
            {
                var enthalpy = i % 9 switch
                {
                    0 => hSolid - 100_000.0,
                    1 => hSolid - 1.0,
                    2 => hSolid,
                    3 => hSolid + 0.10 * latent,
                    4 => hSolid + 0.50 * latent,
                    5 => hSolid + 0.90 * latent,
                    6 => hLiquid,
                    7 => hLiquid + 1.0,
                    _ => hLiquid + 100_000.0
                };

                states[i] = new ThermodynamicState(enthalpy);
            }

            return states;
        }

        private static (int Start, int Length) GetPartition(
            int totalLength,
            int workerCount,
            int workerIndex)
        {
            var baseLength = totalLength / workerCount;
            var remainder = totalLength % workerCount;
            var length = baseLength + (workerIndex < remainder ? 1 : 0);
            var start = workerIndex * baseLength + Math.Min(workerIndex, remainder);
            return (start, length);
        }

        private static void ValidatePartitionCoverage(int totalLength, int workerCount)
        {
            var expectedStart = 0;
            var covered = 0;

            for (var workerIndex = 0; workerIndex < workerCount; workerIndex++)
            {
                var (start, length) = GetPartition(totalLength, workerCount, workerIndex);
                if (start != expectedStart || length < 0)
                {
                    throw new InvalidOperationException(
                        "Worker partitioning contains a gap, overlap, or invalid length.");
                }

                expectedStart = start + length;
                covered += length;
            }

            if (covered != totalLength || expectedStart != totalLength)
            {
                throw new InvalidOperationException(
                    "Worker partitioning does not cover the full batch exactly once.");
            }
        }

        private static double ComputeChecksum(DerivedThermodynamicState[] values)
        {
            var checksum = 0.0;
            var stride = Math.Max(1, values.Length / 32);
            for (var i = 0; i < values.Length; i += stride)
            {
                checksum += values[i].Temperature;
                checksum += values[i].LiquidPhaseFraction;
            }

            return checksum;
        }

        private static void PrintEnvironment()
        {
            Console.WriteLine("Multi-thread CPU batch-recovery scaling v0.1");
            Console.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
            Console.WriteLine($"os: {RuntimeInformation.OSDescription}");
            Console.WriteLine($"architecture: {RuntimeInformation.ProcessArchitecture}");
            Console.WriteLine($"logical_processors: {Environment.ProcessorCount}");
            Console.WriteLine($"server_gc: {GCSettings.IsServerGC}");
            Console.WriteLine($"stopwatch_frequency_hz: {Stopwatch.Frequency}");
            Console.WriteLine($"cpu_model: {ReadCpuModel()}");
            Console.WriteLine($"github_run_id: {Environment.GetEnvironmentVariable("GITHUB_RUN_ID") ?? "local"}");
            Console.WriteLine($"warmup_samples: {WarmupSamples}");
            Console.WriteLine($"timed_samples: {TimedSamples}");
            Console.WriteLine($"target_cell_operations_per_sample: {TargetCellOperations}");
            Console.WriteLine("requested_worker_counts: 1,2,4,8");
        }

        private static string ReadCpuModel()
        {
            try
            {
                if (!File.Exists("/proc/cpuinfo"))
                {
                    return "unavailable";
                }

                foreach (var line in File.ReadLines("/proc/cpuinfo"))
                {
                    if (line.StartsWith("model name", StringComparison.OrdinalIgnoreCase))
                    {
                        var separator = line.IndexOf(':');
                        return separator >= 0 ? line[(separator + 1)..].Trim() : line.Trim();
                    }
                }
            }
            catch
            {
                // Environment metadata is informative and must not invalidate the benchmark.
            }

            return "unavailable";
        }
    }
}
