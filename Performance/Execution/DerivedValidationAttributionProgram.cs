using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Performance.DerivedValidationAttribution
{
    internal static class DerivedValidationAttributionProgram
    {
        private const int WarmupSamples = 3;
        private const int TimedSamples = 7;
        private const int TargetValueOperations = 8_388_608;

        private static readonly int[] ValueCounts =
        {
            1_024,
            16_384,
            262_144,
            1_048_576
        };

        /// <summary>
        /// Benchmark-local two-double output used by every partial-validation layer.
        /// Static factories vary validation only; storage type and layout stay fixed.
        /// This type has no Framework authority and is not an implementation candidate.
        /// </summary>
        private readonly struct LayeredOutput
        {
            private LayeredOutput(double temperature, double liquidFraction)
            {
                Temperature = temperature;
                LiquidFraction = liquidFraction;
            }

            public double Temperature { get; }
            public double LiquidFraction { get; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static LayeredOutput Raw(double temperature, double liquidFraction) =>
                new LayeredOutput(temperature, liquidFraction);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static LayeredOutput TemperatureFinite(double temperature, double liquidFraction)
            {
                if (!double.IsFinite(temperature))
                {
                    ThrowTemperature();
                }

                return new LayeredOutput(temperature, liquidFraction);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static LayeredOutput BothFinite(double temperature, double liquidFraction)
            {
                if (!double.IsFinite(temperature))
                {
                    ThrowTemperature();
                }

                if (!double.IsFinite(liquidFraction))
                {
                    ThrowLiquidFraction();
                }

                return new LayeredOutput(temperature, liquidFraction);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static LayeredOutput FiniteLowerBound(double temperature, double liquidFraction)
            {
                if (!double.IsFinite(temperature))
                {
                    ThrowTemperature();
                }

                if (!double.IsFinite(liquidFraction) || liquidFraction < 0.0)
                {
                    ThrowLiquidFraction();
                }

                return new LayeredOutput(temperature, liquidFraction);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static LayeredOutput FullValidation(double temperature, double liquidFraction)
            {
                if (!double.IsFinite(temperature))
                {
                    ThrowTemperature();
                }

                if (!double.IsFinite(liquidFraction)
                    || liquidFraction < 0.0
                    || liquidFraction > 1.0)
                {
                    ThrowLiquidFraction();
                }

                return new LayeredOutput(temperature, liquidFraction);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void ThrowTemperature() =>
                throw new ArgumentOutOfRangeException(
                    "temperature",
                    "Recovered temperature must be finite.");

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void ThrowLiquidFraction() =>
                throw new ArgumentOutOfRangeException(
                    "liquidPhaseFraction",
                    "Liquid phase fraction must be finite and within [0, 1].");
        }

        private static int Main()
        {
            try
            {
                PrintEnvironment();
                RunInvariantSanityGate();
                RunSemanticGate();

                Console.WriteLine(
                    "RESULT_HEADER|scenario|values|passes|median_ms|min_ms|max_ms|ns_per_value|million_values_per_second|median_allocated_bytes|checksum");

                foreach (var count in ValueCounts)
                {
                    MeasureLayered("raw_output", count, LayeredMode.Raw);
                    MeasureLayered("temperature_finite_output", count, LayeredMode.TemperatureFinite);
                    MeasureLayered("both_finite_output", count, LayeredMode.BothFinite);
                    MeasureLayered("finite_lower_bound_output", count, LayeredMode.FiniteLowerBound);
                    MeasureLayered("local_full_validation_output", count, LayeredMode.FullValidation);
                    MeasurePublic(count);
                }

                Console.WriteLine(
                    "Derived validation attribution: COMPLETED — fine-grained measurements reported.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Derived validation attribution: INVALID — {ex.GetType().Name}: {ex.Message}");
                return 1;
            }
        }

        private enum LayeredMode
        {
            Raw,
            TemperatureFinite,
            BothFinite,
            FiniteLowerBound,
            FullValidation
        }

        private static void RunInvariantSanityGate()
        {
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(double.NaN, 0.5));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(double.PositiveInfinity, 0.5));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(300.0, double.NaN));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(300.0, double.PositiveInfinity));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(300.0, -0.01));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(300.0, 1.01));

            ExpectArgumentOutOfRange(() => LayeredOutput.FullValidation(double.NaN, 0.5));
            ExpectArgumentOutOfRange(() => LayeredOutput.FullValidation(double.PositiveInfinity, 0.5));
            ExpectArgumentOutOfRange(() => LayeredOutput.FullValidation(300.0, double.NaN));
            ExpectArgumentOutOfRange(() => LayeredOutput.FullValidation(300.0, double.PositiveInfinity));
            ExpectArgumentOutOfRange(() => LayeredOutput.FullValidation(300.0, -0.01));
            ExpectArgumentOutOfRange(() => LayeredOutput.FullValidation(300.0, 1.01));

            Console.WriteLine("public_and_local_full_invariant_sanity_gate: PASS");
        }

        private static void RunSemanticGate()
        {
            const int count = 1_048_576;
            CreateSourceValues(count, out var temperatures, out var liquidFractions);

            var maxTemperatureError = 0.0;
            var maxFractionError = 0.0;

            for (var i = 0; i < count; i++)
            {
                var temperature = temperatures[i];
                var fraction = liquidFractions[i];
                var reference = new DerivedThermodynamicState(temperature, fraction);

                Accumulate(reference, LayeredOutput.Raw(temperature, fraction),
                    ref maxTemperatureError, ref maxFractionError);
                Accumulate(reference, LayeredOutput.TemperatureFinite(temperature, fraction),
                    ref maxTemperatureError, ref maxFractionError);
                Accumulate(reference, LayeredOutput.BothFinite(temperature, fraction),
                    ref maxTemperatureError, ref maxFractionError);
                Accumulate(reference, LayeredOutput.FiniteLowerBound(temperature, fraction),
                    ref maxTemperatureError, ref maxFractionError);
                Accumulate(reference, LayeredOutput.FullValidation(temperature, fraction),
                    ref maxTemperatureError, ref maxFractionError);
            }

            Console.WriteLine($"semantic_gate_max_temperature_error: {maxTemperatureError:R}");
            Console.WriteLine($"semantic_gate_max_liquid_fraction_error: {maxFractionError:R}");

            if (maxTemperatureError != 0.0 || maxFractionError != 0.0)
            {
                throw new InvalidOperationException(
                    "One or more valid-domain attribution paths differ from the public Derived State values.");
            }

            Console.WriteLine("valid_domain_semantic_equivalence_gate: PASS");
        }

        private static void Accumulate(
            DerivedThermodynamicState reference,
            LayeredOutput candidate,
            ref double maxTemperatureError,
            ref double maxFractionError)
        {
            maxTemperatureError = Math.Max(
                maxTemperatureError,
                Math.Abs(reference.Temperature - candidate.Temperature));
            maxFractionError = Math.Max(
                maxFractionError,
                Math.Abs(reference.LiquidPhaseFraction - candidate.LiquidFraction));
        }

        private static void MeasureLayered(string scenario, int count, LayeredMode mode)
        {
            CreateSourceValues(count, out var temperatures, out var fractions);
            var output = new LayeredOutput[count];

            Action<int> run = mode switch
            {
                LayeredMode.Raw => passes =>
                {
                    for (var pass = 0; pass < passes; pass++)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            output[i] = LayeredOutput.Raw(temperatures[i], fractions[i]);
                        }
                    }
                },
                LayeredMode.TemperatureFinite => passes =>
                {
                    for (var pass = 0; pass < passes; pass++)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            output[i] = LayeredOutput.TemperatureFinite(temperatures[i], fractions[i]);
                        }
                    }
                },
                LayeredMode.BothFinite => passes =>
                {
                    for (var pass = 0; pass < passes; pass++)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            output[i] = LayeredOutput.BothFinite(temperatures[i], fractions[i]);
                        }
                    }
                },
                LayeredMode.FiniteLowerBound => passes =>
                {
                    for (var pass = 0; pass < passes; pass++)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            output[i] = LayeredOutput.FiniteLowerBound(temperatures[i], fractions[i]);
                        }
                    }
                },
                LayeredMode.FullValidation => passes =>
                {
                    for (var pass = 0; pass < passes; pass++)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            output[i] = LayeredOutput.FullValidation(temperatures[i], fractions[i]);
                        }
                    }
                },
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };

            MeasureCore(
                scenario,
                count,
                run,
                () => ComputeChecksum(output.Length, i => output[i].Temperature, i => output[i].LiquidFraction));
        }

        private static void MeasurePublic(int count)
        {
            CreateSourceValues(count, out var temperatures, out var fractions);
            var output = new DerivedThermodynamicState[count];

            MeasureCore(
                "public_derived_output",
                count,
                passes =>
                {
                    for (var pass = 0; pass < passes; pass++)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            output[i] = new DerivedThermodynamicState(temperatures[i], fractions[i]);
                        }
                    }
                },
                () => ComputeChecksum(output.Length, i => output[i].Temperature, i => output[i].LiquidPhaseFraction));
        }

        private static void MeasureCore(
            string scenario,
            int count,
            Action<int> run,
            Func<double> checksum)
        {
            var passes = Math.Max(1, TargetValueOperations / count);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (var i = 0; i < WarmupSamples; i++)
            {
                run(passes);
            }

            var elapsed = new double[TimedSamples];
            var allocated = new long[TimedSamples];

            for (var sample = 0; sample < TimedSamples; sample++)
            {
                var beforeAllocation = GC.GetAllocatedBytesForCurrentThread();
                var start = Stopwatch.GetTimestamp();
                run(passes);
                var end = Stopwatch.GetTimestamp();
                var afterAllocation = GC.GetAllocatedBytesForCurrentThread();

                elapsed[sample] = (end - start) * 1000.0 / Stopwatch.Frequency;
                allocated[sample] = afterAllocation - beforeAllocation;
            }

            var resultChecksum = checksum();
            Array.Sort(elapsed);
            Array.Sort(allocated);

            var medianMs = elapsed[TimedSamples / 2];
            var operations = (long)count * passes;
            var nsPerValue = medianMs * 1_000_000.0 / operations;
            var millionValuesPerSecond = operations / (medianMs / 1000.0) / 1_000_000.0;

            Console.WriteLine(string.Join('|', new[]
            {
                "RESULT",
                scenario,
                count.ToString(CultureInfo.InvariantCulture),
                passes.ToString(CultureInfo.InvariantCulture),
                medianMs.ToString("R", CultureInfo.InvariantCulture),
                elapsed[0].ToString("R", CultureInfo.InvariantCulture),
                elapsed[^1].ToString("R", CultureInfo.InvariantCulture),
                nsPerValue.ToString("R", CultureInfo.InvariantCulture),
                millionValuesPerSecond.ToString("R", CultureInfo.InvariantCulture),
                allocated[TimedSamples / 2].ToString(CultureInfo.InvariantCulture),
                resultChecksum.ToString("R", CultureInfo.InvariantCulture)
            }));
        }

        private static void CreateSourceValues(
            int count,
            out double[] temperatures,
            out double[] liquidFractions)
        {
            var definition = new ReferenceMaterialDefinition(
                materialId: "derived-validation-attribution-synthetic-reference-v0.1",
                provenance: "Synthetic fixed configuration for output-construction attribution only",
                referenceDensity: 1000.0,
                densityReferenceTemperature: 300.0,
                energyReferenceTemperature: 250.0,
                meltingTemperature: 300.0,
                latentHeat: 250_000.0,
                solidHeatCapacity: 2_000.0,
                liquidHeatCapacity: 4_000.0);

            var material = ReferenceMaterialCompiler.Compile(definition);
            var states = new ThermodynamicState[count];
            var derived = new DerivedThermodynamicState[count];
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;
            var latent = material.LatentHeat;

            for (var i = 0; i < count; i++)
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

            ReferenceThermodynamicFormulation.RecoverBatch(states, derived, material);

            temperatures = new double[count];
            liquidFractions = new double[count];
            for (var i = 0; i < count; i++)
            {
                temperatures[i] = derived[i].Temperature;
                liquidFractions[i] = derived[i].LiquidPhaseFraction;
            }
        }

        private static double ComputeChecksum(
            int count,
            Func<int, double> temperature,
            Func<int, double> fraction)
        {
            var checksum = 0.0;
            var stride = Math.Max(1, count / 16);
            for (var i = 0; i < count; i += stride)
            {
                checksum += temperature(i);
                checksum += fraction(i);
            }
            return checksum;
        }

        private static void ExpectArgumentOutOfRange(Action action)
        {
            try
            {
                action();
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            throw new InvalidOperationException("Expected ArgumentOutOfRangeException was not thrown.");
        }

        private static void PrintEnvironment()
        {
            Console.WriteLine("Derived State validation attribution v0.1");
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
            Console.WriteLine($"target_value_operations_per_sample: {TargetValueOperations}");
        }

        private static string ReadCpuModel()
        {
            try
            {
                if (File.Exists("/proc/cpuinfo"))
                {
                    foreach (var line in File.ReadLines("/proc/cpuinfo"))
                    {
                        if (line.StartsWith("model name", StringComparison.OrdinalIgnoreCase))
                        {
                            var separator = line.IndexOf(':');
                            return separator >= 0 ? line[(separator + 1)..].Trim() : line.Trim();
                        }
                    }
                }
            }
            catch
            {
                // Environment metadata is informative only.
            }

            return "unavailable";
        }
    }
}
