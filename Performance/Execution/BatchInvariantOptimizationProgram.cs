using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Performance.ReferenceCpuBatchInvariantOptimization
{
    internal static class BatchInvariantOptimizationProgram
    {
        private const int WarmupSamples = 3;
        private const int TimedSamples = 7;
        private const int TargetCellOperations = 8_388_608;
        private const double EquivalenceTolerance = 1e-10;

        private static readonly int[] CellCounts =
        {
            1_024,
            16_384,
            262_144,
            1_048_576
        };

        private delegate void RecoveryScenario(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material);

        private static int Main()
        {
            try
            {
                var material = BuildMaterial();
                PrintEnvironment();
                RunSemanticGate(material);

                Console.WriteLine(
                    "RESULT_HEADER|scenario|cells|passes|median_ms|min_ms|max_ms|ns_per_cell|million_cells_per_second|median_allocated_bytes|checksum");

                foreach (var cellCount in CellCounts)
                {
                    Measure(
                        "formal_optimized_batch",
                        cellCount,
                        material,
                        RunFormalOptimizedBatch);
                    Measure(
                        "legacy_validated_batch_emulation",
                        cellCount,
                        material,
                        RunLegacyValidatedBatch);
                    Measure(
                        "local_specialized_trusted_batch",
                        cellCount,
                        material,
                        RunLocalSpecializedTrustedBatch);
                    Measure(
                        "validated_output_only",
                        cellCount,
                        material,
                        RunValidatedOutputOnly);
                    Measure(
                        "trusted_output_only",
                        cellCount,
                        material,
                        RunTrustedOutputOnly);
                }

                Console.WriteLine(
                    "Batch invariant optimization evaluation: COMPLETED — measurements reported.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Batch invariant optimization evaluation: INVALID — {ex.GetType().Name}: {ex.Message}");
                return 1;
            }
        }

        private static CompiledThermodynamicParameters BuildMaterial()
        {
            return ReferenceMaterialCompiler.Compile(
                new ReferenceMaterialDefinition(
                    materialId: "batch-invariant-performance-v0.1",
                    provenance: "Synthetic fixed configuration for invariant-preserving batch performance evaluation",
                    referenceDensity: 1000.0,
                    densityReferenceTemperature: 300.0,
                    energyReferenceTemperature: 250.0,
                    meltingTemperature: 300.0,
                    latentHeat: 250_000.0,
                    solidHeatCapacity: 2_000.0,
                    liquidHeatCapacity: 4_000.0));
        }

        private static void RunSemanticGate(
            CompiledThermodynamicParameters material)
        {
            const int count = 1_048_576;
            var states = CreateStates(count, material);
            var formal = new DerivedThermodynamicState[count];
            var legacy = new DerivedThermodynamicState[count];
            var localTrusted = new DerivedThermodynamicState[count];
            var sourceTemperatures = new double[count];
            var sourceFractions = new double[count];

            RunFormalOptimizedBatch(
                states,
                formal,
                sourceTemperatures,
                sourceFractions,
                1,
                material);
            RunLegacyValidatedBatch(
                states,
                legacy,
                sourceTemperatures,
                sourceFractions,
                1,
                material);
            RunLocalSpecializedTrustedBatch(
                states,
                localTrusted,
                sourceTemperatures,
                sourceFractions,
                1,
                material);

            var legacyTemperatureError = 0.0;
            var legacyFractionError = 0.0;
            var localTemperatureError = 0.0;
            var localFractionError = 0.0;

            for (var i = 0; i < count; i++)
            {
                legacyTemperatureError = Math.Max(
                    legacyTemperatureError,
                    Math.Abs(formal[i].Temperature - legacy[i].Temperature));
                legacyFractionError = Math.Max(
                    legacyFractionError,
                    Math.Abs(formal[i].LiquidPhaseFraction - legacy[i].LiquidPhaseFraction));
                localTemperatureError = Math.Max(
                    localTemperatureError,
                    Math.Abs(formal[i].Temperature - localTrusted[i].Temperature));
                localFractionError = Math.Max(
                    localFractionError,
                    Math.Abs(formal[i].LiquidPhaseFraction - localTrusted[i].LiquidPhaseFraction));
            }

            Console.WriteLine($"equivalence_tolerance: {EquivalenceTolerance:R}");
            Console.WriteLine(
                $"legacy_max_temperature_error: {legacyTemperatureError:R}");
            Console.WriteLine(
                $"legacy_max_liquid_fraction_error: {legacyFractionError:R}");
            Console.WriteLine(
                $"local_trusted_max_temperature_error: {localTemperatureError:R}");
            Console.WriteLine(
                $"local_trusted_max_liquid_fraction_error: {localFractionError:R}");

            if (legacyTemperatureError > EquivalenceTolerance
                || legacyFractionError > EquivalenceTolerance
                || localTemperatureError > EquivalenceTolerance
                || localFractionError > EquivalenceTolerance)
            {
                throw new InvalidOperationException(
                    "One or more optimization comparison paths exceeded the semantic-equivalence tolerance.");
            }

            Console.WriteLine("semantic_equivalence_gate: PASS");
        }

        private static void Measure(
            string scenarioName,
            int cellCount,
            CompiledThermodynamicParameters material,
            RecoveryScenario scenario)
        {
            var passes = Math.Max(1, TargetCellOperations / cellCount);
            var states = CreateStates(cellCount, material);
            var destination = new DerivedThermodynamicState[cellCount];
            var sourceTemperatures = new double[cellCount];
            var sourceLiquidFractions = new double[cellCount];

            ReferenceThermodynamicFormulation.RecoverBatch(
                states,
                destination,
                material);
            for (var i = 0; i < cellCount; i++)
            {
                sourceTemperatures[i] = destination[i].Temperature;
                sourceLiquidFractions[i] = destination[i].LiquidPhaseFraction;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (var i = 0; i < WarmupSamples; i++)
            {
                scenario(
                    states,
                    destination,
                    sourceTemperatures,
                    sourceLiquidFractions,
                    passes,
                    material);
            }

            var elapsedMilliseconds = new double[TimedSamples];
            var allocatedBytes = new long[TimedSamples];

            for (var sample = 0; sample < TimedSamples; sample++)
            {
                var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
                var startTimestamp = Stopwatch.GetTimestamp();

                scenario(
                    states,
                    destination,
                    sourceTemperatures,
                    sourceLiquidFractions,
                    passes,
                    material);

                var endTimestamp = Stopwatch.GetTimestamp();
                var allocatedAfter = GC.GetAllocatedBytesForCurrentThread();

                elapsedMilliseconds[sample] =
                    (endTimestamp - startTimestamp)
                    * 1000.0
                    / Stopwatch.Frequency;
                allocatedBytes[sample] = allocatedAfter - allocatedBefore;
            }

            var checksum = ComputeChecksum(destination);

            Array.Sort(elapsedMilliseconds);
            Array.Sort(allocatedBytes);

            var medianMilliseconds = elapsedMilliseconds[TimedSamples / 2];
            var minimumMilliseconds = elapsedMilliseconds[0];
            var maximumMilliseconds = elapsedMilliseconds[TimedSamples - 1];
            var medianAllocatedBytes = allocatedBytes[TimedSamples / 2];

            var cellOperations = (long)cellCount * passes;
            var nanosecondsPerCell =
                medianMilliseconds * 1_000_000.0 / cellOperations;
            var millionCellsPerSecond =
                cellOperations / (medianMilliseconds / 1000.0) / 1_000_000.0;

            Console.WriteLine(string.Join('|', new[]
            {
                "RESULT",
                scenarioName,
                cellCount.ToString(CultureInfo.InvariantCulture),
                passes.ToString(CultureInfo.InvariantCulture),
                medianMilliseconds.ToString("R", CultureInfo.InvariantCulture),
                minimumMilliseconds.ToString("R", CultureInfo.InvariantCulture),
                maximumMilliseconds.ToString("R", CultureInfo.InvariantCulture),
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
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;
            var latent = material.LatentHeat;

            for (var i = 0; i < count; i++)
            {
                var enthalpy = i % 7 switch
                {
                    0 => hSolid - 50_000.0,
                    1 => hSolid,
                    2 => hSolid + 0.25 * latent,
                    3 => hSolid + 0.50 * latent,
                    4 => hLiquid,
                    5 => hLiquid + 50_000.0,
                    _ => hLiquid + 200_000.0
                };

                states[i] = new ThermodynamicState(enthalpy);
            }

            return states;
        }

        private static void RunFormalOptimizedBatch(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                ReferenceThermodynamicFormulation.RecoverBatch(
                    states,
                    destination,
                    material);
            }
        }

        private static void RunLegacyValidatedBatch(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    RecoverRaw(
                        states[i].SpecificEnthalpy,
                        material,
                        out var temperature,
                        out var liquidFraction,
                        out _);

                    destination[i] = new DerivedThermodynamicState(
                        temperature,
                        liquidFraction);
                }
            }
        }

        private static void RunLocalSpecializedTrustedBatch(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    RecoverRaw(
                        states[i].SpecificEnthalpy,
                        material,
                        out var temperature,
                        out var liquidFraction,
                        out var region);

                    EstablishSpecializedInvariants(
                        region,
                        temperature,
                        liquidFraction);

                    destination[i] = DerivedThermodynamicState.FromEstablishedInvariants(
                        temperature,
                        liquidFraction);
                }
            }
        }

        private static void RunValidatedOutputOnly(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < destination.Length; i++)
                {
                    destination[i] = new DerivedThermodynamicState(
                        sourceTemperatures[i],
                        sourceLiquidFractions[i]);
                }
            }
        }

        private static void RunTrustedOutputOnly(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] destination,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < destination.Length; i++)
                {
                    destination[i] = DerivedThermodynamicState.FromEstablishedInvariants(
                        sourceTemperatures[i],
                        sourceLiquidFractions[i]);
                }
            }
        }

        private static void RecoverRaw(
            double specificEnthalpy,
            CompiledThermodynamicParameters material,
            out double temperature,
            out double liquidFraction,
            out RecoveryRegion region)
        {
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;

            if (specificEnthalpy < hSolid)
            {
                temperature = material.MeltingTemperature
                    + (specificEnthalpy - hSolid) / material.SolidHeatCapacity;
                liquidFraction = 0.0;
                region = RecoveryRegion.SolidSensible;
                return;
            }

            if (specificEnthalpy <= hLiquid)
            {
                temperature = material.MeltingTemperature;
                liquidFraction = (specificEnthalpy - hSolid) / material.LatentHeat;
                region = RecoveryRegion.Latent;
                return;
            }

            temperature = material.MeltingTemperature
                + (specificEnthalpy - hLiquid) / material.LiquidHeatCapacity;
            liquidFraction = 1.0;
            region = RecoveryRegion.LiquidSensible;
        }

        private static void EstablishSpecializedInvariants(
            RecoveryRegion region,
            double temperature,
            double liquidFraction)
        {
            if (region == RecoveryRegion.Latent)
            {
                DerivedThermodynamicState.RequireBoundedLiquidFraction(
                    liquidFraction);
                return;
            }

            DerivedThermodynamicState.RequireFiniteTemperature(temperature);
        }

        private static double ComputeChecksum(
            DerivedThermodynamicState[] destination)
        {
            if (destination.Length == 0)
            {
                return 0.0;
            }

            var first = destination[0];
            var middle = destination[destination.Length / 2];
            var last = destination[destination.Length - 1];

            return first.Temperature
                + first.LiquidPhaseFraction
                + middle.Temperature
                + middle.LiquidPhaseFraction
                + last.Temperature
                + last.LiquidPhaseFraction;
        }

        private static void PrintEnvironment()
        {
            Console.WriteLine("Reference CPU batch invariant optimization evaluation v0.1");
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
                const string prefix = "model name";
                if (line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var separator = line.IndexOf(':');
                    if (separator >= 0 && separator + 1 < line.Length)
                    {
                        return line[(separator + 1)..].Trim();
                    }
                }
            }

            return "unavailable";
        }

        private enum RecoveryRegion
        {
            SolidSensible,
            Latent,
            LiquidSensible
        }
    }
}
