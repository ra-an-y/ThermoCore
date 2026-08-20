using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime;
using System.Runtime.InteropServices;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Performance.ReferenceCpuSimdV02
{
    internal static class SimdProgramV02
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

        private delegate double RecoveryScenario(
            ThermodynamicState[] states,
            double[] enthalpies,
            double[] temperatures,
            double[] liquidFractions,
            int passes,
            CompiledThermodynamicParameters material);

        private static int Main()
        {
            try
            {
                var material = BuildMaterial();
                PrintEnvironment();
                RunEquivalenceGate(material);

                Console.WriteLine(
                    "RESULT_HEADER|scenario|cells|passes|median_ms|min_ms|max_ms|ns_per_cell|million_cells_per_second|median_allocated_bytes|checksum");

                foreach (var cellCount in CellCounts)
                {
                    Measure(
                        "scalar_reference_recovery",
                        cellCount,
                        material,
                        RunScalarReferenceRecovery);
                    Measure(
                        "scalar_batch_recovery",
                        cellCount,
                        material,
                        RunScalarBatchRecovery);
                    Measure(
                        "simd_batch_recovery",
                        cellCount,
                        material,
                        RunSimdBatchRecovery);
                }

                Console.WriteLine(
                    "SIMD performance evaluation v0.2: COMPLETED — corrected measurements reported.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"SIMD performance evaluation v0.2: INVALID — {ex.GetType().Name}: {ex.Message}");
                return 1;
            }
        }

        private static CompiledThermodynamicParameters BuildMaterial()
        {
            var definition = new ReferenceMaterialDefinition(
                materialId: "performance-simd-synthetic-reference-v0.2",
                provenance: "Synthetic fixed configuration for corrected SIMD performance evaluation only",
                referenceDensity: 1000.0,
                densityReferenceTemperature: 300.0,
                energyReferenceTemperature: 250.0,
                meltingTemperature: 300.0,
                latentHeat: 250_000.0,
                solidHeatCapacity: 2_000.0,
                liquidHeatCapacity: 4_000.0);

            return ReferenceMaterialCompiler.Compile(definition);
        }

        private static void RunEquivalenceGate(
            CompiledThermodynamicParameters material)
        {
            const int count = 1_048_576;
            CreateInputs(
                count,
                material,
                out var states,
                out var enthalpies);

            var referenceTemperatures = new double[count];
            var referenceFractions = new double[count];
            var scalarTemperatures = new double[count];
            var scalarFractions = new double[count];
            var simdTemperatures = new double[count];
            var simdFractions = new double[count];

            _ = RunScalarReferenceRecovery(
                states,
                enthalpies,
                referenceTemperatures,
                referenceFractions,
                passes: 1,
                material);
            _ = RunScalarBatchRecovery(
                states,
                enthalpies,
                scalarTemperatures,
                scalarFractions,
                passes: 1,
                material);
            _ = RunSimdBatchRecovery(
                states,
                enthalpies,
                simdTemperatures,
                simdFractions,
                passes: 1,
                material);

            var scalarMaxTemperatureError = 0.0;
            var scalarMaxFractionError = 0.0;
            var simdMaxTemperatureError = 0.0;
            var simdMaxFractionError = 0.0;

            for (var i = 0; i < count; i++)
            {
                EnsureFinite(referenceTemperatures[i], "reference Temperature");
                EnsureFinite(referenceFractions[i], "reference liquid fraction");
                EnsureFinite(scalarTemperatures[i], "scalar batch Temperature");
                EnsureFinite(scalarFractions[i], "scalar batch liquid fraction");
                EnsureFinite(simdTemperatures[i], "SIMD Temperature");
                EnsureFinite(simdFractions[i], "SIMD liquid fraction");

                scalarMaxTemperatureError = Math.Max(
                    scalarMaxTemperatureError,
                    Math.Abs(referenceTemperatures[i] - scalarTemperatures[i]));
                scalarMaxFractionError = Math.Max(
                    scalarMaxFractionError,
                    Math.Abs(referenceFractions[i] - scalarFractions[i]));
                simdMaxTemperatureError = Math.Max(
                    simdMaxTemperatureError,
                    Math.Abs(referenceTemperatures[i] - simdTemperatures[i]));
                simdMaxFractionError = Math.Max(
                    simdMaxFractionError,
                    Math.Abs(referenceFractions[i] - simdFractions[i]));
            }

            Console.WriteLine($"equivalence_tolerance: {EquivalenceTolerance:R}");
            Console.WriteLine(
                $"scalar_batch_max_temperature_error: {scalarMaxTemperatureError:R}");
            Console.WriteLine(
                $"scalar_batch_max_liquid_fraction_error: {scalarMaxFractionError:R}");
            Console.WriteLine(
                $"simd_batch_max_temperature_error: {simdMaxTemperatureError:R}");
            Console.WriteLine(
                $"simd_batch_max_liquid_fraction_error: {simdMaxFractionError:R}");

            if (scalarMaxTemperatureError > EquivalenceTolerance
                || scalarMaxFractionError > EquivalenceTolerance
                || simdMaxTemperatureError > EquivalenceTolerance
                || simdMaxFractionError > EquivalenceTolerance)
            {
                throw new InvalidOperationException(
                    "Batch recovery exceeded the semantic-equivalence tolerance.");
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
            CreateInputs(
                cellCount,
                material,
                out var states,
                out var enthalpies);
            var temperatures = new double[cellCount];
            var liquidFractions = new double[cellCount];

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (var i = 0; i < WarmupSamples; i++)
            {
                _ = scenario(
                    states,
                    enthalpies,
                    temperatures,
                    liquidFractions,
                    passes,
                    material);
            }

            var elapsedMilliseconds = new double[TimedSamples];
            var allocatedBytes = new long[TimedSamples];
            double checksum = 0.0;

            for (var sample = 0; sample < TimedSamples; sample++)
            {
                var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
                var startTimestamp = Stopwatch.GetTimestamp();

                checksum = scenario(
                    states,
                    enthalpies,
                    temperatures,
                    liquidFractions,
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

        private static void CreateInputs(
            int count,
            CompiledThermodynamicParameters material,
            out ThermodynamicState[] states,
            out double[] enthalpies)
        {
            states = new ThermodynamicState[count];
            enthalpies = new double[count];

            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;
            var latent = material.LatentHeat;

            for (var i = 0; i < count; i++)
            {
                var h = i % 7 switch
                {
                    0 => hSolid - 50_000.0,
                    1 => hSolid,
                    2 => hSolid + 0.25 * latent,
                    3 => hSolid + 0.50 * latent,
                    4 => hLiquid,
                    5 => hLiquid + 50_000.0,
                    _ => hLiquid + 200_000.0
                };

                states[i] = new ThermodynamicState(h);
                enthalpies[i] = states[i].SpecificEnthalpy;
            }
        }

        private static double RunScalarReferenceRecovery(
            ThermodynamicState[] states,
            double[] enthalpies,
            double[] temperatures,
            double[] liquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < states.Length; i++)
                {
                    var derived = ReferenceThermodynamicFormulation.Recover(
                        states[i],
                        material);
                    temperatures[i] = derived.Temperature;
                    liquidFractions[i] = derived.LiquidPhaseFraction;
                }
            }

            return ComputeChecksum(temperatures, liquidFractions);
        }

        private static double RunScalarBatchRecovery(
            ThermodynamicState[] states,
            double[] enthalpies,
            double[] temperatures,
            double[] liquidFractions,
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
                for (var i = 0; i < enthalpies.Length; i++)
                {
                    RecoverOne(
                        enthalpies[i],
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

            return ComputeChecksum(temperatures, liquidFractions);
        }

        private static double RunSimdBatchRecovery(
            ThermodynamicState[] states,
            double[] enthalpies,
            double[] temperatures,
            double[] liquidFractions,
            int passes,
            CompiledThermodynamicParameters material)
        {
            var vectorWidth = Vector<double>.Count;
            var hSolidVector = new Vector<double>(material.SolidTransitionEnthalpy);
            var hLiquidVector = new Vector<double>(material.LiquidTransitionEnthalpy);
            var meltingVector = new Vector<double>(material.MeltingTemperature);
            var solidHeatCapacityVector = new Vector<double>(material.SolidHeatCapacity);
            var liquidHeatCapacityVector = new Vector<double>(material.LiquidHeatCapacity);
            var latentHeatVector = new Vector<double>(material.LatentHeat);
            var zeroVector = Vector<double>.Zero;
            var oneVector = new Vector<double>(1.0);

            for (var pass = 0; pass < passes; pass++)
            {
                var i = 0;
                var vectorizedLength = enthalpies.Length - enthalpies.Length % vectorWidth;

                for (; i < vectorizedLength; i += vectorWidth)
                {
                    var h = new Vector<double>(enthalpies, i);

                    var solidMask = Vector.LessThan(h, hSolidVector);
                    var throughLatentMask = Vector.LessThanOrEqual(h, hLiquidVector);

                    var solidTemperature = meltingVector
                        + (h - hSolidVector) / solidHeatCapacityVector;
                    var liquidTemperature = meltingVector
                        + (h - hLiquidVector) / liquidHeatCapacityVector;
                    var latentFraction = (h - hSolidVector) / latentHeatVector;

                    var temperature = Vector.ConditionalSelect(
                        solidMask,
                        solidTemperature,
                        Vector.ConditionalSelect(
                            throughLatentMask,
                            meltingVector,
                            liquidTemperature));

                    var liquidFraction = Vector.ConditionalSelect(
                        solidMask,
                        zeroVector,
                        Vector.ConditionalSelect(
                            throughLatentMask,
                            latentFraction,
                            oneVector));

                    temperature.CopyTo(temperatures, i);
                    liquidFraction.CopyTo(liquidFractions, i);
                }

                for (; i < enthalpies.Length; i++)
                {
                    RecoverOne(
                        enthalpies[i],
                        material.SolidTransitionEnthalpy,
                        material.LiquidTransitionEnthalpy,
                        material.MeltingTemperature,
                        material.SolidHeatCapacity,
                        material.LiquidHeatCapacity,
                        material.LatentHeat,
                        out temperatures[i],
                        out liquidFractions[i]);
                }
            }

            return ComputeChecksum(temperatures, liquidFractions);
        }

        private static void RecoverOne(
            double h,
            double hSolid,
            double hLiquid,
            double meltingTemperature,
            double solidHeatCapacity,
            double liquidHeatCapacity,
            double latentHeat,
            out double temperature,
            out double liquidFraction)
        {
            if (h < hSolid)
            {
                temperature = meltingTemperature
                    + (h - hSolid) / solidHeatCapacity;
                liquidFraction = 0.0;
            }
            else if (h <= hLiquid)
            {
                temperature = meltingTemperature;
                liquidFraction = (h - hSolid) / latentHeat;
            }
            else
            {
                temperature = meltingTemperature
                    + (h - hLiquid) / liquidHeatCapacity;
                liquidFraction = 1.0;
            }
        }

        private static double ComputeChecksum(
            double[] temperatures,
            double[] liquidFractions)
        {
            var middle = temperatures.Length / 2;
            var last = temperatures.Length - 1;

            return temperatures[0]
                + temperatures[middle]
                + temperatures[last]
                + liquidFractions[0]
                + liquidFractions[middle]
                + liquidFractions[last];
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
            Console.WriteLine("Reference CPU SIMD performance evaluation v0.2");
            Console.WriteLine("scalar_reference_input: prevalidated ThermodynamicState[]");
            Console.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
            Console.WriteLine($"os: {RuntimeInformation.OSDescription}");
            Console.WriteLine($"architecture: {RuntimeInformation.ProcessArchitecture}");
            Console.WriteLine($"logical_processors: {Environment.ProcessorCount}");
            Console.WriteLine($"server_gc: {GCSettings.IsServerGC}");
            Console.WriteLine($"stopwatch_frequency_hz: {Stopwatch.Frequency}");
            Console.WriteLine($"cpu_model: {ReadCpuModel()}");
            Console.WriteLine($"vector_hardware_accelerated: {Vector.IsHardwareAccelerated}");
            Console.WriteLine($"vector_double_count: {Vector<double>.Count}");
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
