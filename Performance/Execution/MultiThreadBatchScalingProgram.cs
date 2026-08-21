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

        private sealed class ScalingScenario : IDisposable
        {
            private readonly ThermodynamicState[] _states;
            private readonly CompiledThermodynamicParameters _material;
            private readonly int _passes;
            private readonly PersistentWorkerPool? _pool;

            public ScalingScenario(
                string name,
                int workers,
                bool direct,
                ThermodynamicState[] states,
                CompiledThermodynamicParameters material,
                int passes)
            {
                Name = name;
                Workers = workers;
                IsDirect = direct;
                _states = states;
                _material = material;
                _passes = passes;
                Destination = new DerivedThermodynamicState[states.Length];

                if (!direct)
                {
                    _pool = new PersistentWorkerPool(
                        states,
                        Destination,
                        material,
                        passes,
                        workers);
                }
            }

            public string Name { get; }
            public int Workers { get; }
            public bool IsDirect { get; }
            public DerivedThermodynamicState[] Destination { get; }

            public void RunOnce()
            {
                if (_pool != null)
                {
                    _pool.RunOnce();
                    return;
                }

                RunDirect(_states, Destination, _material, _passes);
            }

            public void Dispose() => _pool?.Dispose();
        }

        private static int Main()
        {
            try
            {
                var material = BuildMaterial();
                PrintEnvironment();
                RunSemanticGate(material);

                Console.WriteLine(
                    "RESULT_HEADER|scenario|cells|workers|logical_processors|oversubscribed|passes|median_ms|min_ms|max_ms|ns_per_cell|million_cells_per_second|speedup_vs_direct|speedup_vs_worker1|worker_scaling_efficiency|checksum");

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
            var scenarios = new[]
            {
                new ScalingScenario("direct_single_thread", 1, true, states, material, passes),
                new ScalingScenario("worker_pool_1", 1, false, states, material, passes),
                new ScalingScenario("worker_pool_2", 2, false, states, material, passes),
                new ScalingScenario("worker_pool_4", 4, false, states, material, passes),
                new ScalingScenario("worker_pool_8", 8, false, states, material, passes)
            };

            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                for (var warmup = 0; warmup < WarmupSamples; warmup++)
                {
                    RunRotatedRound(scenarios, warmup, samples: null, sampleIndex: -1);
                }

                var samples = new double[scenarios.Length][];
                for (var i = 0; i < samples.Length; i++)
                {
                    samples[i] = new double[TimedSamples];
                }

                for (var sample = 0; sample < TimedSamples; sample++)
                {
                    RunRotatedRound(scenarios, sample, samples, sample);
                }

                var results = new TimingResult[scenarios.Length];
                for (var i = 0; i < scenarios.Length; i++)
                {
                    var ordered = samples[i];
                    Array.Sort(ordered);
                    results[i] = new TimingResult(
                        ordered[TimedSamples / 2],
                        ordered[0],
                        ordered[TimedSamples - 1],
                        ComputeChecksum(scenarios[i].Destination));
                }

                var directMedian = results[0].MedianMs;
                var worker1Median = results[1].MedianMs;
                for (var i = 0; i < scenarios.Length; i++)
                {
                    PrintResult(
                        scenarios[i],
                        cellCount,
                        passes,
                        results[i],
                        directMedian,
                        worker1Median);
                }
            }
            finally
            {
                for (var i = scenarios.Length - 1; i >= 0; i--)
                {
                    scenarios[i].Dispose();
                }
            }
        }

        private static void RunRotatedRound(
            ScalingScenario[] scenarios,
            int rotation,
            double[][]? samples,
            int sampleIndex)
        {
            var startIndex = rotation % scenarios.Length;
            for (var step = 0; step < scenarios.Length; step++)
            {
                var scenarioIndex = (startIndex + step) % scenarios.Length;
                var scenario = scenarios[scenarioIndex];

                if (samples == null)
                {
                    scenario.RunOnce();
                    continue;
                }

                var start = Stopwatch.GetTimestamp();
                scenario.RunOnce();
                var end = Stopwatch.GetTimestamp();
                samples[scenarioIndex][sampleIndex] =
                    (end - start) * 1000.0 / Stopwatch.Frequency;
            }
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
            ScalingScenario scenario,
            int cellCount,
            int passes,
            TimingResult result,
            double directMedianMs,
            double worker1MedianMs)
        {
            var operations = (long)cellCount * passes;
            var nsPerCell = result.MedianMs * 1_000_000.0 / operations;
            var throughput = operations / (result.MedianMs / 1000.0) / 1_000_000.0;
            var speedupVsDirect = directMedianMs / result.MedianMs;
            var speedupVsWorker1 = scenario.IsDirect
                ? 1.0
                : worker1MedianMs / result.MedianMs;
            var workerEfficiency = scenario.IsDirect
                ? 1.0
                : speedupVsWorker1 / scenario.Workers;
            var oversubscribed = scenario.Workers > Environment.ProcessorCount;

            Console.WriteLine(string.Join('|', new[]
            {
                "RESULT",
                scenario.Name,
                cellCount.ToString(CultureInfo.InvariantCulture),
                scenario.Workers.ToString(CultureInfo.InvariantCulture),
                Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture),
                oversubscribed ? "true" : "false",
                passes.ToString(CultureInfo.InvariantCulture),
                result.MedianMs.ToString("R", CultureInfo.InvariantCulture),
                result.MinMs.ToString("R", CultureInfo.InvariantCulture),
                result.MaxMs.ToString("R", CultureInfo.InvariantCulture),
                nsPerCell.ToString("R", CultureInfo.InvariantCulture),
                throughput.ToString("R", CultureInfo.InvariantCulture),
                speedupVsDirect.ToString("R", CultureInfo.InvariantCulture),
                speedupVsWorker1.ToString("R", CultureInfo.InvariantCulture),
                workerEfficiency.ToString("R", CultureInfo.InvariantCulture),
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
            Console.WriteLine("timing_order: interleaved_rotating_start");
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
