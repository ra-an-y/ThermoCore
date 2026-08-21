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

        private readonly struct RawOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public RawOutput(double temperature, double liquidFraction)
            {
                Temperature = temperature;
                LiquidFraction = liquidFraction;
            }

            public double Temperature { get; }
            public double LiquidFraction { get; }
        }

        private readonly struct TemperatureFiniteOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TemperatureFiniteOutput(double temperature, double liquidFraction)
            {
                if (!double.IsFinite(temperature))
                {
                    ThrowTemperature();
                }

                Temperature = temperature;
                LiquidFraction = liquidFraction;
            }

            public double Temperature { get; }
            public double LiquidFraction { get; }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void ThrowTemperature() =>
                throw new ArgumentOutOfRangeException(nameof(temperature));
        }

        private readonly struct BothFiniteOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BothFiniteOutput(double temperature, double liquidFraction)
            {
                if (!double.IsFinite(temperature))
                {
                    ThrowTemperature();
                }

                if (!double.IsFinite(liquidFraction))
                {
                    ThrowLiquidFraction();
                }

                Temperature = temperature;
                LiquidFraction = liquidFraction;
            }

            public double Temperature { get; }
            public double LiquidFraction { get; }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void ThrowTemperature() =>
                throw new ArgumentOutOfRangeException(nameof(temperature));

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void ThrowLiquidFraction() =>
                throw new ArgumentOutOfRangeException(nameof(liquidFraction));
        }

        private readonly struct FiniteLowerBoundOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FiniteLowerBoundOutput(double temperature, double liquidFraction)
            {
                if (!double.IsFinite(temperature))
                {
                    ThrowTemperature();
                }

                if (!double.IsFinite(liquidFraction) || liquidFraction < 0.0)
                {
                    ThrowLiquidFraction();
                }

                Temperature = temperature;
                LiquidFraction = liquidFraction;
            }

            public double Temperature { get; }
            public double LiquidFraction { get; }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void ThrowTemperature() =>
                throw new ArgumentOutOfRangeException(nameof(temperature));

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static void ThrowLiquidFraction() =>
                throw new ArgumentOutOfRangeException(nameof(liquidFraction));
        }

        private readonly struct LocalFullValidationOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LocalFullValidationOutput(double temperature, double liquidFraction)
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

                Temperature = temperature;
                LiquidFraction = liquidFraction;
            }

            public double Temperature { get; }
            public double LiquidFraction { get; }

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

        private sealed class Buffers
        {
            public Buffers(int count)
            {
                Public = new DerivedThermodynamicState[count];
                Raw = new RawOutput[count];
                TemperatureFinite = new TemperatureFiniteOutput[count];
                BothFinite = new BothFiniteOutput[count];
                FiniteLowerBound = new FiniteLowerBoundOutput[count];
                LocalFull = new LocalFullValidationOutput[count];
            }

            public DerivedThermodynamicState[] Public { get; }
            public RawOutput[] Raw { get; }
            public TemperatureFiniteOutput[] TemperatureFinite { get; }
            public BothFiniteOutput[] BothFinite { get; }
            public FiniteLowerBoundOutput[] FiniteLowerBound { get; }
            public LocalFullValidationOutput[] LocalFull { get; }
        }

        private delegate void Scenario(
            double[] temperatures,
            double[] liquidFractions,
            Buffers buffers,
            int passes);

        private delegate double Checksum(Buffers buffers);

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
                    Measure("raw_output", count, RunRaw, ChecksumRaw);
                    Measure("temperature_finite_output", count, RunTemperatureFinite, ChecksumTemperatureFinite);
                    Measure("both_finite_output", count, RunBothFinite, ChecksumBothFinite);
                    Measure("finite_lower_bound_output", count, RunFiniteLowerBound, ChecksumFiniteLowerBound);
                    Measure("local_full_validation_output", count, RunLocalFull, ChecksumLocalFull);
                    Measure("public_derived_output", count, RunPublic, ChecksumPublic);
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

        private static void RunInvariantSanityGate()
        {
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(double.NaN, 0.5));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(double.PositiveInfinity, 0.5));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(300.0, double.NaN));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(300.0, double.PositiveInfinity));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(300.0, -0.01));
            ExpectArgumentOutOfRange(() => new DerivedThermodynamicState(300.0, 1.01));

            ExpectArgumentOutOfRange(() => new LocalFullValidationOutput(double.NaN, 0.5));
            ExpectArgumentOutOfRange(() => new LocalFullValidationOutput(double.PositiveInfinity, 0.5));
            ExpectArgumentOutOfRange(() => new LocalFullValidationOutput(300.0, double.NaN));
            ExpectArgumentOutOfRange(() => new LocalFullValidationOutput(300.0, double.PositiveInfinity));
            ExpectArgumentOutOfRange(() => new LocalFullValidationOutput(300.0, -0.01));
            ExpectArgumentOutOfRange(() => new LocalFullValidationOutput(300.0, 1.01));

            Console.WriteLine("public_and_local_full_invariant_sanity_gate: PASS");
        }

        private static void RunSemanticGate()
        {
            const int count = 1_048_576;
            CreateSourceValues(count, out var temperatures, out var liquidFractions);
            var buffers = new Buffers(count);

            RunRaw(temperatures, liquidFractions, buffers, 1);
            RunTemperatureFinite(temperatures, liquidFractions, buffers, 1);
            RunBothFinite(temperatures, liquidFractions, buffers, 1);
            RunFiniteLowerBound(temperatures, liquidFractions, buffers, 1);
            RunLocalFull(temperatures, liquidFractions, buffers, 1);
            RunPublic(temperatures, liquidFractions, buffers, 1);

            var maxTemperatureError = 0.0;
            var maxFractionError = 0.0;

            for (var i = 0; i < count; i++)
            {
                var referenceTemperature = buffers.Public[i].Temperature;
                var referenceFraction = buffers.Public[i].LiquidPhaseFraction;

                AccumulateError(referenceTemperature, referenceFraction,
                    buffers.Raw[i].Temperature, buffers.Raw[i].LiquidFraction,
                    ref maxTemperatureError, ref maxFractionError);
                AccumulateError(referenceTemperature, referenceFraction,
                    buffers.TemperatureFinite[i].Temperature, buffers.TemperatureFinite[i].LiquidFraction,
                    ref maxTemperatureError, ref maxFractionError);
                AccumulateError(referenceTemperature, referenceFraction,
                    buffers.BothFinite[i].Temperature, buffers.BothFinite[i].LiquidFraction,
                    ref maxTemperatureError, ref maxFractionError);
                AccumulateError(referenceTemperature, referenceFraction,
                    buffers.FiniteLowerBound[i].Temperature, buffers.FiniteLowerBound[i].LiquidFraction,
                    ref maxTemperatureError, ref maxFractionError);
                AccumulateError(referenceTemperature, referenceFraction,
                    buffers.LocalFull[i].Temperature, buffers.LocalFull[i].LiquidFraction,
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

        private static void AccumulateError(
            double referenceTemperature,
            double referenceFraction,
            double temperature,
            double fraction,
            ref double maxTemperatureError,
            ref double maxFractionError)
        {
            maxTemperatureError = Math.Max(
                maxTemperatureError,
                Math.Abs(referenceTemperature - temperature));
            maxFractionError = Math.Max(
                maxFractionError,
                Math.Abs(referenceFraction - fraction));
        }

        private static void Measure(
            string scenarioName,
            int count,
            Scenario scenario,
            Checksum checksum)
        {
            var passes = Math.Max(1, TargetValueOperations / count);
            CreateSourceValues(count, out var temperatures, out var liquidFractions);
            var buffers = new Buffers(count);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (var i = 0; i < WarmupSamples; i++)
            {
                scenario(temperatures, liquidFractions, buffers, passes);
            }

            var elapsedMilliseconds = new double[TimedSamples];
            var allocatedBytes = new long[TimedSamples];

            for (var sample = 0; sample < TimedSamples; sample++)
            {
                var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
                var start = Stopwatch.GetTimestamp();

                scenario(temperatures, liquidFractions, buffers, passes);

                var end = Stopwatch.GetTimestamp();
                var allocatedAfter = GC.GetAllocatedBytesForCurrentThread();

                elapsedMilliseconds[sample] =
                    (end - start) * 1000.0 / Stopwatch.Frequency;
                allocatedBytes[sample] = allocatedAfter - allocatedBefore;
            }

            var resultChecksum = checksum(buffers);

            Array.Sort(elapsedMilliseconds);
            Array.Sort(allocatedBytes);

            var medianMs = elapsedMilliseconds[TimedSamples / 2];
            var minMs = elapsedMilliseconds[0];
            var maxMs = elapsedMilliseconds[TimedSamples - 1];
            var medianAllocated = allocatedBytes[TimedSamples / 2];
            var operations = (long)count * passes;
            var nsPerValue = medianMs * 1_000_000.0 / operations;
            var millionValuesPerSecond =
                operations / (medianMs / 1000.0) / 1_000_000.0;

            Console.WriteLine(string.Join('|', new[]
            {
                "RESULT",
                scenarioName,
                count.ToString(CultureInfo.InvariantCulture),
                passes.ToString(CultureInfo.InvariantCulture),
                medianMs.ToString("R", CultureInfo.InvariantCulture),
                minMs.ToString("R", CultureInfo.InvariantCulture),
                maxMs.ToString("R", CultureInfo.InvariantCulture),
                nsPerValue.ToString("R", CultureInfo.InvariantCulture),
                millionValuesPerSecond.ToString("R", CultureInfo.InvariantCulture),
                medianAllocated.ToString(CultureInfo.InvariantCulture),
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

        private static void RunRaw(
            double[] temperatures, double[] liquidFractions, Buffers buffers, int passes)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < temperatures.Length; i++)
                {
                    buffers.Raw[i] = new RawOutput(temperatures[i], liquidFractions[i]);
                }
            }
        }

        private static void RunTemperatureFinite(
            double[] temperatures, double[] liquidFractions, Buffers buffers, int passes)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < temperatures.Length; i++)
                {
                    buffers.TemperatureFinite[i] =
                        new TemperatureFiniteOutput(temperatures[i], liquidFractions[i]);
                }
            }
        }

        private static void RunBothFinite(
            double[] temperatures, double[] liquidFractions, Buffers buffers, int passes)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < temperatures.Length; i++)
                {
                    buffers.BothFinite[i] =
                        new BothFiniteOutput(temperatures[i], liquidFractions[i]);
                }
            }
        }

        private static void RunFiniteLowerBound(
            double[] temperatures, double[] liquidFractions, Buffers buffers, int passes)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < temperatures.Length; i++)
                {
                    buffers.FiniteLowerBound[i] =
                        new FiniteLowerBoundOutput(temperatures[i], liquidFractions[i]);
                }
            }
        }

        private static void RunLocalFull(
            double[] temperatures, double[] liquidFractions, Buffers buffers, int passes)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < temperatures.Length; i++)
                {
                    buffers.LocalFull[i] =
                        new LocalFullValidationOutput(temperatures[i], liquidFractions[i]);
                }
            }
        }

        private static void RunPublic(
            double[] temperatures, double[] liquidFractions, Buffers buffers, int passes)
        {
            for (var pass = 0; pass < passes; pass++)
            {
                for (var i = 0; i < temperatures.Length; i++)
                {
                    buffers.Public[i] =
                        new DerivedThermodynamicState(temperatures[i], liquidFractions[i]);
                }
            }
        }

        private static double ChecksumRaw(Buffers buffers) =>
            ComputeChecksum(buffers.Raw.Length, i => buffers.Raw[i].Temperature, i => buffers.Raw[i].LiquidFraction);

        private static double ChecksumTemperatureFinite(Buffers buffers) =>
            ComputeChecksum(buffers.TemperatureFinite.Length,
                i => buffers.TemperatureFinite[i].Temperature,
                i => buffers.TemperatureFinite[i].LiquidFraction);

        private static double ChecksumBothFinite(Buffers buffers) =>
            ComputeChecksum(buffers.BothFinite.Length,
                i => buffers.BothFinite[i].Temperature,
                i => buffers.BothFinite[i].LiquidFraction);

        private static double ChecksumFiniteLowerBound(Buffers buffers) =>
            ComputeChecksum(buffers.FiniteLowerBound.Length,
                i => buffers.FiniteLowerBound[i].Temperature,
                i => buffers.FiniteLowerBound[i].LiquidFraction);

        private static double ChecksumLocalFull(Buffers buffers) =>
            ComputeChecksum(buffers.LocalFull.Length,
                i => buffers.LocalFull[i].Temperature,
                i => buffers.LocalFull[i].LiquidFraction);

        private static double ChecksumPublic(Buffers buffers) =>
            ComputeChecksum(buffers.Public.Length,
                i => buffers.Public[i].Temperature,
                i => buffers.Public[i].LiquidPhaseFraction);

        private static double ComputeChecksum(
            int count,
            Func<int, double> temperature,
            Func<int, double> liquidFraction)
        {
            var checksum = 0.0;
            var stride = Math.Max(1, count / 16);
            for (var i = 0; i < count; i += stride)
            {
                checksum += temperature(i);
                checksum += liquidFraction(i);
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

            throw new InvalidOperationException(
                "Expected ArgumentOutOfRangeException was not thrown.");
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
