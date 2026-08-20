using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Performance.ReferenceCpuBatchAttribution
{
    internal static class BatchAttributionProgram
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
            DerivedThermodynamicState[] derived,
            double[] temperatures,
            double[] liquidFractions,
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
                        "scalar_public_recovery",
                        cellCount,
                        material,
                        RunScalarPublicRecovery);
                    Measure(
                        "formal_batch_recovery",
                        cellCount,
                        material,
                        RunFormalBatchRecovery);
                    Measure(
                        "local_derived_recovery",
                        cellCount,
                        material,
                        RunLocalDerivedRecovery);
                    Measure(
                        "local_primitive_recovery",
                        cellCount,
                        material,
                        RunLocalPrimitiveRecovery);
                    Measure(
                        "derived_output_only",
                        cellCount,
                        material,
                        RunDerivedOutputOnly);
                    Measure(
                        "primitive_output_only",
                        cellCount,
                        material,
                        RunPrimitiveOutputOnly);
                }

                Console.WriteLine(
                    "Batch attribution evaluation: COMPLETED — attribution measurements reported.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Batch attribution evaluation: INVALID — {ex.GetType().Name}: {ex.Message}");
                return 1;
            }
        }

        private static CompiledThermodynamicParameters BuildMaterial()
        {
            var definition = new ReferenceMaterialDefinition(
                materialId: "performance-attribution-synthetic-reference-v0.1",
                provenance: "Synthetic fixed configuration for CPU batch attribution only",
                referenceDensity: 1000.0,
                densityReferenceTemperature: 300.0,
                energyReferenceTemperature: 250.0,
                meltingTemperature: 300.0,
                latentHeat: 250_000.0,
                solidHeatCapacity: 2_000.0,
                liquidHeatCapacity: 4_000.0);

            return ReferenceMaterialCompiler.Compile(definition);
        }

        private static void RunSemanticGate(
            CompiledThermodynamicParameters material)
        {
            const int count = 1_048_576;
            var states = CreateStates(count, material);

            var scalar = new DerivedThermodynamicState[count];
            var formalBatch = new DerivedThermodynamicState[count];
            var localDerived = new DerivedThermodynamicState[count];
            var localTemperatures = new double[count];
            var localFractions = new double[count];
            var unusedTemperatures = new double[count];
            var unusedFractions = new double[count];

            RunScalarPublicRecovery(
                states,
                scalar,
                unusedTemperatures,
                unusedFractions,
                unusedTemperatures,
                unusedFractions,
                1,
                material);
            RunFormalBatchRecovery(
                states,
                formalBatch,
                unusedTemperatures,
                unusedFractions,
                unusedTemperatures,
                unusedFractions,
                1,
                material);
            RunLocalDerivedRecovery(
                states,
                localDerived,
                unusedTemperatures,
                unusedFractions,
                unusedTemperatures,
                unusedFractions,
                1,
                material);
            RunLocalPrimitiveRecovery(
                states,
                localDerived,
                localTemperatures,
                localFractions,
                unusedTemperatures,
                unusedFractions,
                1,
                material);

            var formalTemperatureError = 0.0;
            var formalFractionError = 0.0;
            var localDerivedTemperatureError = 0.0;
            var localDerivedFractionError = 0.0;
            var localPrimitiveTemperatureError = 0.0;
            var localPrimitiveFractionError = 0.0;

            for (var i = 0; i < count; i++)
            {
                var reference = scalar[i];
                EnsureFinite(reference.Temperature, "scalar Temperature");
                EnsureFinite(reference.LiquidPhaseFraction, "scalar liquid fraction");

                formalTemperatureError = Math.Max(
                    formalTemperatureError,
                    Math.Abs(reference.Temperature - formalBatch[i].Temperature));
                formalFractionError = Math.Max(
                    formalFractionError,
                    Math.Abs(reference.LiquidPhaseFraction - formalBatch[i].LiquidPhaseFraction));

                localDerivedTemperatureError = Math.Max(
                    localDerivedTemperatureError,
                    Math.Abs(reference.Temperature - localDerived[i].Temperature));
                localDerivedFractionError = Math.Max(
                    localDerivedFractionError,
                    Math.Abs(reference.LiquidPhaseFraction - localDerived[i].LiquidPhaseFraction));

                localPrimitiveTemperatureError = Math.Max(
                    localPrimitiveTemperatureError,
                    Math.Abs(reference.Temperature - localTemperatures[i]));
                localPrimitiveFractionError = Math.Max(
                    localPrimitiveFractionError,
                    Math.Abs(reference.LiquidPhaseFraction - localFractions[i]));
            }

            Console.WriteLine($"equivalence_tolerance: {EquivalenceTolerance:R}");
            Console.WriteLine(
                $"formal_batch_max_temperature_error: {formalTemperatureError:R}");
            Console.WriteLine(
                $"formal_batch_max_liquid_fraction_error: {formalFractionError:R}");
            Console.WriteLine(
                $"local_derived_max_temperature_error: {localDerivedTemperatureError:R}");
            Console.WriteLine(
                $"local_derived_max_liquid_fraction_error: {localDerivedFractionError:R}");
            Console.WriteLine(
                $"local_primitive_max_temperature_error: {localPrimitiveTemperatureError:R}");
            Console.WriteLine(
                $"local_primitive_max_liquid_fraction_error: {localPrimitiveFractionError:R}");

            if (formalTemperatureError > EquivalenceTolerance
                || formalFractionError > EquivalenceTolerance
                || localDerivedTemperatureError > EquivalenceTolerance
                || localDerivedFractionError > EquivalenceTolerance
                || localPrimitiveTemperatureError > EquivalenceTolerance
                || localPrimitiveFractionError > EquivalenceTolerance)
            {
                throw new InvalidOperationException(
                    "One or more attribution candidates exceeded the semantic-equivalence tolerance.");
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
            var derived = new DerivedThermodynamicState[cellCount];
            var temperatures = new double[cellCount];
            var liquidFractions = new double[cellCount];
            var sourceTemperatures = new double[cellCount];
            var sourceLiquidFractions = new double[cellCount];

            ReferenceThermodynamicFormulation.RecoverBatch(
                states,
                derived,
                material);
            for (var i = 0; i < cellCount; i++)
            {
                sourceTemperatures[i] = derived[i].Temperature;
                sourceLiquidFractions[i] = derived[i].LiquidPhaseFraction;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (var i = 0; i < WarmupSamples; i++)
            {
                scenario(
                    states,
                    derived,
                    temperatures,
                    liquidFractions,
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
                    derived,
                    temperatures,
                    liquidFractions,
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

            var checksum = scenarioName switch
            {
                "local_primitive_recovery" or "primitive_output_only"
                    => ComputePrimitiveChecksum(temperatures, liquidFractions),
                _ => ComputeDerivedChecksum(derived)
            };

            Array.Sort(elapsedMilliseconds);
            Array.Sort(allocatedBytes);

            var medianMilliseconds = elapsedMilliseconds[TimedSamples / 2];
            var minMilliseconds = elapsedMilliseconds[0];
            var maxMilliseconds = elapsedMilliseconds[TimedSamples - 1];
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
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;
            var latent = material.LatentHeat;

            for (var i = 0; i < states.Length; i++)
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

        private static void RunScalarPublicRecovery(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] derived,
            double[] temperatures,
            double[] liquidFractions,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    derived[i] = ReferenceThermodynamicFormulation.Recover(
                        states[i],
                        material);
                }
            }
        }

        private static void RunFormalBatchRecovery(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] derived,
            double[] temperatures,
            double[] liquidFractions,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                ReferenceThermodynamicFormulation.RecoverBatch(
                    states,
                    derived,
                    material);
            }
        }

        private static void RunLocalDerivedRecovery(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] derived,
            double[] temperatures,
            double[] liquidFractions,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;
            var meltingTemperature = material.MeltingTemperature;
            var solidHeatCapacity = material.SolidHeatCapacity;
            var liquidHeatCapacity = material.LiquidHeatCapacity;
            var latentHeat = material.LatentHeat;

            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    RecoverOne(
                        states[i].SpecificEnthalpy,
                        hSolid,
                        hLiquid,
                        meltingTemperature,
                        solidHeatCapacity,
                        liquidHeatCapacity,
                        latentHeat,
                        out var temperature,
                        out var liquidFraction);

                    derived[i] = new DerivedThermodynamicState(
                        temperature,
                        liquidFraction);
                }
            }
        }

        private static void RunLocalPrimitiveRecovery(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] derived,
            double[] temperatures,
            double[] liquidFractions,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;
            var meltingTemperature = material.MeltingTemperature;
            var solidHeatCapacity = material.SolidHeatCapacity;
            var liquidHeatCapacity = material.LiquidHeatCapacity;
            var latentHeat = material.LatentHeat;

            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    RecoverOne(
                        states[i].SpecificEnthalpy,
                        hSolid,
                        hLiquid,
                        meltingTemperature,
                        solidHeatCapacity,
                        liquidHeatCapacity,
                        latentHeat,
                        out temperatures[i],
                        out liquidFractions[i]);
                }
            }
        }

        private static void RunDerivedOutputOnly(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] derived,
            double[] temperatures,
            double[] liquidFractions,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < derived.Length; i++)
                {
                    derived[i] = new DerivedThermodynamicState(
                        sourceTemperatures[i],
                        sourceLiquidFractions[i]);
                }
            }
        }

        private static void RunPrimitiveOutputOnly(
            ThermodynamicState[] states,
            DerivedThermodynamicState[] derived,
            double[] temperatures,
            double[] liquidFractions,
            double[] sourceTemperatures,
            double[] sourceLiquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < temperatures.Length; i++)
                {
                    temperatures[i] = sourceTemperatures[i];
                    liquidFractions[i] = sourceLiquidFractions[i];
                }
            }
        }

        private static void RecoverOne(
            double specificEnthalpy,
            double hSolid,
            double hLiquid,
            double meltingTemperature,
            double solidHeatCapacity,
            double liquidHeatCapacity,
            double latentHeat,
            out double temperature,
            out double liquidFraction)
        {
            if (specificEnthalpy < hSolid)
            {
                temperature = meltingTemperature
                    + (specificEnthalpy - hSolid) / solidHeatCapacity;
                liquidFraction = 0.0;
            }
            else if (specificEnthalpy <= hLiquid)
            {
                temperature = meltingTemperature;
                liquidFraction = (specificEnthalpy - hSolid) / latentHeat;
            }
            else
            {
                temperature = meltingTemperature
                    + (specificEnthalpy - hLiquid) / liquidHeatCapacity;
                liquidFraction = 1.0;
            }
        }

        private static double ComputeDerivedChecksum(
            DerivedThermodynamicState[] values)
        {
            var checksum = 0.0;
            var stride = Math.Max(1, values.Length / 16);
            for (var i = 0; i < values.Length; i += stride)
            {
                checksum += values[i].Temperature;
                checksum += values[i].LiquidPhaseFraction;
            }

            return checksum;
        }

        private static double ComputePrimitiveChecksum(
            double[] temperatures,
            double[] liquidFractions)
        {
            var checksum = 0.0;
            var stride = Math.Max(1, temperatures.Length / 16);
            for (var i = 0; i < temperatures.Length; i += stride)
            {
                checksum += temperatures[i];
                checksum += liquidFractions[i];
            }

            return checksum;
        }

        private static void EnsureFinite(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new InvalidOperationException($"{name} must be finite.");
            }
        }

        private static void PrintEnvironment()
        {
            Console.WriteLine("Reference CPU batch attribution evaluation v0.1");
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
                if (line.StartsWith("model name", StringComparison.OrdinalIgnoreCase))
                {
                    var separator = line.IndexOf(':');
                    return separator >= 0
                        ? line[(separator + 1)..].Trim()
                        : line.Trim();
                }
            }

            return "unavailable";
        }
    }
}
