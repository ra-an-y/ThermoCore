using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Performance.ReferenceCpu
{
    internal static class Program
    {
        private const int WarmupSamples = 2;
        private const int TimedSamples = 5;
        private const int TargetCellOperations = 1_048_576;
        private const double DeltaSpecificEnthalpy = 0.25;

        private static readonly int[] CellCounts =
        {
            1_024,
            16_384,
            262_144,
            1_048_576
        };

        private delegate double Scenario(
            ThermodynamicState[] states,
            int passes,
            CompiledThermodynamicParameters material);

        private static int Main()
        {
            try
            {
                var material = BuildMaterial();

                PrintEnvironment();
                Console.WriteLine(
                    "RESULT_HEADER|scenario|cells|passes|median_ms|min_ms|max_ms|ns_per_cell|million_cells_per_second|median_allocated_bytes|checksum");

                foreach (var cellCount in CellCounts)
                {
                    Measure("state_update", cellCount, material, RunStateUpdate);
                    Measure("state_recovery", cellCount, material, RunStateRecovery);
                    Measure("update_plus_recovery", cellCount, material, RunCombined);
                }

                Console.WriteLine("Performance evaluation: COMPLETED — measurements reported.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Performance evaluation: INVALID — {ex.GetType().Name}: {ex.Message}");
                return 1;
            }
        }

        private static CompiledThermodynamicParameters BuildMaterial()
        {
            var definition = new ReferenceMaterialDefinition(
                materialId: "performance-synthetic-reference-v0.1",
                provenance: "Synthetic fixed configuration for CPU performance evaluation only",
                referenceDensity: 1000.0,
                densityReferenceTemperature: 300.0,
                energyReferenceTemperature: 250.0,
                meltingTemperature: 300.0,
                latentHeat: 250_000.0,
                solidHeatCapacity: 2_000.0,
                liquidHeatCapacity: 4_000.0);

            return ReferenceMaterialCompiler.Compile(definition);
        }

        private static void Measure(
            string scenarioName,
            int cellCount,
            CompiledThermodynamicParameters material,
            Scenario scenario)
        {
            var passes = Math.Max(1, TargetCellOperations / cellCount);
            var states = CreateStates(cellCount, material);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (var i = 0; i < WarmupSamples; i++)
            {
                _ = scenario(states, passes, material);
            }

            var elapsedMilliseconds = new double[TimedSamples];
            var allocatedBytes = new long[TimedSamples];
            double checksum = 0.0;

            for (var sample = 0; sample < TimedSamples; sample++)
            {
                var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
                var startTimestamp = Stopwatch.GetTimestamp();

                checksum = scenario(states, passes, material);

                var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
                var allocatedAfter = GC.GetAllocatedBytesForCurrentThread();

                elapsedMilliseconds[sample] = elapsed.TotalMilliseconds;
                allocatedBytes[sample] = allocatedAfter - allocatedBefore;
            }

            Array.Sort(elapsedMilliseconds);
            Array.Sort(allocatedBytes);

            var medianMilliseconds = elapsedMilliseconds[TimedSamples / 2];
            var minMilliseconds = elapsedMilliseconds[0];
            var maxMilliseconds = elapsedMilliseconds[TimedSamples - 1];
            var medianAllocatedBytes = allocatedBytes[TimedSamples / 2];

            var cellOperations = (long)cellCount * passes;
            var nanosecondsPerCell = medianMilliseconds * 1_000_000.0 / cellOperations;
            var millionCellsPerSecond = cellOperations / (medianMilliseconds / 1000.0) / 1_000_000.0;

            Console.WriteLine(string.Join('|', new[]
            {
                "RESULT",
                scenarioName,
                cellCount.ToString(CultureInfo.InvariantCulture),
                passes.ToString(CultureInfo.InvariantCulture),
                medianMilliseconds.ToString("R", CultureInfo.InvariantCulture),
                minMilliseconds.ToString("R", CultureInfo.InvariantCulture),
                maxMilliseconds.ToString("R", CultureInfo.InvariantCulture),
                nanosecondsPerCell.ToString("R", CultureInfo.InvariantCulture),
                millionCellsPerSecond.ToString("R", CultureInfo.InvariantCulture),
                medianAllocatedBytes.ToString(CultureInfo.InvariantCulture),
                checksum.ToString("R", CultureInfo.InvariantCulture)
            }));
        }

        private static ThermodynamicState[] CreateStates(
            int count,
            CompiledThermodynamicParameters material)
        {
            var states = new ThermodynamicState[count];

            var solidEnthalpy = material.SolidTransitionEnthalpy - 50_000.0;
            var latentEnthalpy = material.SolidTransitionEnthalpy
                + 0.5 * material.LatentHeat;
            var liquidEnthalpy = material.LiquidTransitionEnthalpy + 200_000.0;

            for (var i = 0; i < states.Length; i++)
            {
                var h = i % 3 switch
                {
                    0 => solidEnthalpy,
                    1 => latentEnthalpy,
                    _ => liquidEnthalpy
                };

                states[i] = new ThermodynamicState(h);
            }

            return states;
        }

        private static double RunStateUpdate(
            ThermodynamicState[] states,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    states[i] = ThermodynamicComputation.ApplySpecificEnthalpyIncrement(
                        states[i],
                        DeltaSpecificEnthalpy);
                }
            }

            return states[0].SpecificEnthalpy
                + states[states.Length / 2].SpecificEnthalpy
                + states[^1].SpecificEnthalpy;
        }

        private static double RunStateRecovery(
            ThermodynamicState[] states,
            int passes,
            CompiledThermodynamicParameters material)
        {
            double checksum = 0.0;

            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    var derived = ReferenceThermodynamicFormulation.Recover(
                        states[i],
                        material);

                    checksum = derived.Temperature + derived.LiquidPhaseFraction;
                }
            }

            return checksum;
        }

        private static double RunCombined(
            ThermodynamicState[] states,
            int passes,
            CompiledThermodynamicParameters material)
        {
            double checksum = 0.0;

            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    var updated = ThermodynamicComputation.ApplySpecificEnthalpyIncrement(
                        states[i],
                        DeltaSpecificEnthalpy);
                    states[i] = updated;

                    var derived = ReferenceThermodynamicFormulation.Recover(
                        updated,
                        material);

                    checksum = derived.Temperature + derived.LiquidPhaseFraction;
                }
            }

            return checksum;
        }

        private static void PrintEnvironment()
        {
            Console.WriteLine("Reference CPU performance evaluation");
            Console.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
            Console.WriteLine($"os: {RuntimeInformation.OSDescription}");
            Console.WriteLine($"architecture: {RuntimeInformation.ProcessArchitecture}");
            Console.WriteLine($"logical_processors: {Environment.ProcessorCount}");
            Console.WriteLine($"server_gc: {GCSettings.IsServerGC}");
            Console.WriteLine($"stopwatch_frequency_hz: {Stopwatch.Frequency}");
            Console.WriteLine($"cpu_model: {ReadCpuModel()}");
            Console.WriteLine($"github_run_id: {Environment.GetEnvironmentVariable("GITHUB_RUN_ID") ?? "not-github-actions"}");
            Console.WriteLine($"warmup_samples: {WarmupSamples}");
            Console.WriteLine($"timed_samples: {TimedSamples}");
            Console.WriteLine($"target_cell_operations_per_sample: {TargetCellOperations}");
        }

        private static string ReadCpuModel()
        {
            const string cpuInfoPath = "/proc/cpuinfo";
            if (!File.Exists(cpuInfoPath))
            {
                return "unavailable";
            }

            foreach (var line in File.ReadLines(cpuInfoPath))
            {
                if (!line.StartsWith("model name", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var separator = line.IndexOf(':');
                if (separator >= 0 && separator + 1 < line.Length)
                {
                    return line[(separator + 1)..].Trim();
                }
            }

            return "unavailable";
        }
    }
}
