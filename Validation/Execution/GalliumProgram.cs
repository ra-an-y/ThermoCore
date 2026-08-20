using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Validation.GalliumCaloric
{
    internal static class Program
    {
        private const double MolarMassKgPerMol = 0.069723;
        private const double MeltingTemperature = 302.92;
        private const double EnergyReferenceTemperature = 298.5;
        private const double ReferencePressureBar = 1.0;
        private const double DatumShift = 25_000.0;
        private const double StoredReferenceTolerance = 1e-3;
        private const double StoredEnthalpyTolerance = 1e-3;
        private const double StoredTemperatureTolerance = 1e-8;

        private static readonly ShomateCoefficients Solid = new(
            102.3394,
            -347.5134,
            603.3621,
            -360.7047,
            -1.490304,
            -24.68472,
            236.2780,
            0.0);

        private static readonly ShomateCoefficients Liquid = new(
            24.62138,
            2.701388,
            -1.272134,
            0.196526,
            0.286145,
            -0.908736,
            89.90830,
            5.577983);

        private static int Main()
        {
            try
            {
                var rows = LoadRows(
                    "Validation/Data/gallium_caloric_benchmark_v0.1.csv");

                var solidCalibration = rows
                    .Where(r => r.Role == "calibration" && r.Phase == "solid")
                    .OrderBy(r => r.ReferenceTemperature)
                    .ToArray();
                var liquidCalibration = rows
                    .Where(r => r.Role == "calibration" && r.Phase == "liquid")
                    .OrderBy(r => r.ReferenceTemperature)
                    .ToArray();
                var coexistence = rows
                    .Where(r => r.Role == "coexistence")
                    .ToArray();

                RequireCount(solidCalibration, 2, "solid calibration rows");
                RequireCount(liquidCalibration, 2, "liquid calibration rows");
                RequireCount(coexistence, 2, "coexistence rows");

                var referenceDatum = NistReferenceEnthalpy(
                    EnergyReferenceTemperature,
                    "solid");

                foreach (var row in rows)
                {
                    AssertNear(
                        ReferencePressureBar,
                        row.PressureBar,
                        1e-12,
                        "stored reference pressure");

                    var reconstructedReference = NistReferenceEnthalpy(
                        row.ReferenceTemperature,
                        row.Phase);
                    AssertNear(
                        row.NistEnthalpy,
                        reconstructedReference,
                        StoredReferenceTolerance,
                        $"stored NIST enthalpy at {row.ReferenceTemperature:R} K");

                    var normalizedReference = reconstructedReference - referenceDatum;
                    AssertNear(
                        row.NormalizedEnthalpy,
                        normalizedReference,
                        StoredReferenceTolerance,
                        $"stored normalized enthalpy at {row.ReferenceTemperature:R} K");
                }

                var solidHeatCapacity = Slope(
                    solidCalibration[0],
                    solidCalibration[1]);
                var liquidHeatCapacity = Slope(
                    liquidCalibration[0],
                    liquidCalibration[1]);

                var latentHeat = NistReferenceEnthalpy(
                        MeltingTemperature,
                        "liquid")
                    - NistReferenceEnthalpy(
                        MeltingTemperature,
                        "solid");

                var definition = new ReferenceMaterialDefinition(
                    materialId: "validation-gallium-caloric-v0.1",
                    provenance: "NIST Chemistry WebBook SRD 69 / NIST-JANAF gallium caloric benchmark v0.1",
                    referenceDensity: 1.0,
                    densityReferenceTemperature: MeltingTemperature,
                    energyReferenceTemperature: EnergyReferenceTemperature,
                    meltingTemperature: MeltingTemperature,
                    latentHeat: latentHeat,
                    solidHeatCapacity: solidHeatCapacity,
                    liquidHeatCapacity: liquidHeatCapacity);

                var material = ReferenceMaterialCompiler.Compile(definition);
                var holdouts = rows.Where(r => r.Role == "holdout").ToArray();

                if (holdouts.Length == 0)
                {
                    throw new InvalidOperationException("No holdout rows were found.");
                }

                var temperatureErrors = new List<double>();
                var enthalpyErrors = new List<double>();

                foreach (var row in rows)
                {
                    var modelEnthalpy = ForwardEnthalpy(
                        row.ReferenceTemperature,
                        row.Phase,
                        material);
                    var recoveredTemperature = ReferenceThermodynamicFormulation
                        .RecoverTemperature(
                            new ThermodynamicState(row.NormalizedEnthalpy),
                            material);

                    AssertNear(
                        row.StoredThermoCoreEnthalpy,
                        modelEnthalpy,
                        StoredEnthalpyTolerance,
                        $"stored model enthalpy at {row.ReferenceTemperature:R} K");
                    AssertNear(
                        row.StoredRecoveredTemperature,
                        recoveredTemperature,
                        StoredTemperatureTolerance,
                        $"stored recovered Temperature at {row.ReferenceTemperature:R} K");

                    if (row.Role != "holdout")
                    {
                        continue;
                    }

                    var temperatureError = recoveredTemperature
                        - row.ReferenceTemperature;
                    var enthalpyError = modelEnthalpy
                        - row.NormalizedEnthalpy;

                    temperatureErrors.Add(Math.Abs(temperatureError));
                    enthalpyErrors.Add(Math.Abs(enthalpyError));

                    VerifyDatumShiftInvariance(
                        row,
                        material,
                        temperatureError,
                        enthalpyError);
                }

                var coexistenceSolid = coexistence.Single(r => r.Phase == "solid");
                var latentWidth = material.LiquidTransitionEnthalpy
                    - material.SolidTransitionEnthalpy;
                var latentHeatError = latentWidth - latentHeat;
                var meltingTemperatureError = material.MeltingTemperature
                    - coexistenceSolid.ReferenceTemperature;

                Console.WriteLine("Gallium caloric validation execution");
                Console.WriteLine($"Evaluated holdouts: {holdouts.Length}");
                Console.WriteLine($"reference pressure [bar]: {ReferencePressureBar:R}");
                Console.WriteLine($"T_m,ref [K]: {MeltingTemperature:R}");
                Console.WriteLine($"c_s,fit [J/(kg*K)]: {solidHeatCapacity:R}");
                Console.WriteLine($"c_l,fit [J/(kg*K)]: {liquidHeatCapacity:R}");
                Console.WriteLine($"L_ref [J/kg]: {latentHeat:R}");
                Console.WriteLine($"max |T error| [K]: {temperatureErrors.Max():R}");
                Console.WriteLine($"mean |T error| [K]: {temperatureErrors.Average():R}");
                Console.WriteLine($"max |h error| [J/kg]: {enthalpyErrors.Max():R}");
                Console.WriteLine($"mean |h error| [J/kg]: {enthalpyErrors.Average():R}");
                Console.WriteLine($"latent-heat parameter error [J/kg]: {latentHeatError:R}");
                Console.WriteLine($"melting-Temperature parameter error [K]: {meltingTemperatureError:R}");
                Console.WriteLine("reference-datum shift invariance: PASS");
                Console.WriteLine("Validation comparison: COMPLETED — errors reported.");

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Validation comparison: INVALID — {ex.GetType().Name}: {ex.Message}");
                return 1;
            }
        }

        private static double NistReferenceEnthalpy(
            double temperature,
            string phase)
        {
            ShomateCoefficients coefficients;
            double phaseOffsetKJPerMol;

            switch (phase)
            {
                case "solid":
                    if (temperature < 298.0 || temperature > 302.92)
                    {
                        throw new InvalidOperationException(
                            $"Solid reference Temperature {temperature:R} K is outside the NIST Shomate validity interval.");
                    }

                    coefficients = Solid;
                    phaseOffsetKJPerMol = 0.0;
                    break;

                case "liquid":
                    if (temperature < 302.92 || temperature > 2476.57)
                    {
                        throw new InvalidOperationException(
                            $"Liquid reference Temperature {temperature:R} K is outside the NIST Shomate validity interval.");
                    }

                    coefficients = Liquid;
                    phaseOffsetKJPerMol = Liquid.H;
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unknown phase '{phase}'.");
            }

            var enthalpyKJPerMol = ShomateEnthalpyIncrement(
                    temperature,
                    coefficients)
                + phaseOffsetKJPerMol;

            return enthalpyKJPerMol * 1000.0 / MolarMassKgPerMol;
        }

        private static double ShomateEnthalpyIncrement(
            double temperature,
            ShomateCoefficients c)
        {
            var t = temperature / 1000.0;
            return c.A * t
                + c.B * t * t / 2.0
                + c.C * t * t * t / 3.0
                + c.D * t * t * t * t / 4.0
                - c.E / t
                + c.F
                - c.H;
        }

        private static double Slope(BenchmarkRow first, BenchmarkRow second)
        {
            return (second.NormalizedEnthalpy - first.NormalizedEnthalpy)
                / (second.ReferenceTemperature - first.ReferenceTemperature);
        }

        private static double ForwardEnthalpy(
            double temperature,
            string phase,
            CompiledThermodynamicParameters material)
        {
            return phase switch
            {
                "solid" => material.SolidTransitionEnthalpy
                    + material.SolidHeatCapacity
                    * (temperature - material.MeltingTemperature),
                "liquid" => material.LiquidTransitionEnthalpy
                    + material.LiquidHeatCapacity
                    * (temperature - material.MeltingTemperature),
                _ => throw new InvalidOperationException(
                    $"Unknown phase '{phase}'.")
            };
        }

        private static void VerifyDatumShiftInvariance(
            BenchmarkRow row,
            CompiledThermodynamicParameters material,
            double originalTemperatureError,
            double originalEnthalpyError)
        {
            var shiftedReferenceEnthalpy = row.NormalizedEnthalpy + DatumShift;
            var shiftedModelEnthalpy = ForwardEnthalpy(
                row.ReferenceTemperature,
                row.Phase,
                material) + DatumShift;

            var shiftedRecoveredTemperature = RecoverTemperatureWithOffset(
                shiftedReferenceEnthalpy,
                material,
                DatumShift);

            var shiftedTemperatureError = shiftedRecoveredTemperature
                - row.ReferenceTemperature;
            var shiftedEnthalpyError = shiftedModelEnthalpy
                - shiftedReferenceEnthalpy;

            AssertNear(
                originalTemperatureError,
                shiftedTemperatureError,
                1e-12,
                "reference-datum Temperature invariance");
            AssertNear(
                originalEnthalpyError,
                shiftedEnthalpyError,
                1e-9,
                "reference-datum enthalpy invariance");
        }

        private static double RecoverTemperatureWithOffset(
            double shiftedEnthalpy,
            CompiledThermodynamicParameters material,
            double offset)
        {
            var hSolid = material.SolidTransitionEnthalpy + offset;
            var hLiquid = material.LiquidTransitionEnthalpy + offset;

            if (shiftedEnthalpy < hSolid)
            {
                return material.MeltingTemperature
                    + (shiftedEnthalpy - hSolid)
                    / material.SolidHeatCapacity;
            }

            if (shiftedEnthalpy <= hLiquid)
            {
                return material.MeltingTemperature;
            }

            return material.MeltingTemperature
                + (shiftedEnthalpy - hLiquid)
                / material.LiquidHeatCapacity;
        }

        private static List<BenchmarkRow> LoadRows(string path)
        {
            var lines = File.ReadAllLines(path);
            if (lines.Length < 2)
            {
                throw new InvalidOperationException("Validation dataset is empty.");
            }

            var rows = new List<BenchmarkRow>();
            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = line.Split(',');
                if (fields.Length != 11)
                {
                    throw new InvalidOperationException(
                        $"Unexpected CSV field count: {fields.Length}.");
                }

                rows.Add(new BenchmarkRow(
                    role: fields[0],
                    phase: fields[1],
                    pressureBar: Parse(fields[2]),
                    referenceTemperature: Parse(fields[3]),
                    nistEnthalpy: Parse(fields[4]),
                    normalizedEnthalpy: Parse(fields[5]),
                    storedThermoCoreEnthalpy: Parse(fields[6]),
                    storedRecoveredTemperature: Parse(fields[7])));
            }

            return rows;
        }

        private static double Parse(string value)
        {
            return double.Parse(value, CultureInfo.InvariantCulture);
        }

        private static void RequireCount<T>(T[] values, int expected, string name)
        {
            if (values.Length != expected)
            {
                throw new InvalidOperationException(
                    $"Expected {expected} {name}, found {values.Length}.");
            }
        }

        private static void AssertNear(
            double expected,
            double actual,
            double tolerance,
            string context)
        {
            if (Math.Abs(expected - actual) > tolerance)
            {
                throw new InvalidOperationException(
                    $"{context}: expected {expected:R}, actual {actual:R}.");
            }
        }

        private sealed class BenchmarkRow
        {
            public BenchmarkRow(
                string role,
                string phase,
                double pressureBar,
                double referenceTemperature,
                double nistEnthalpy,
                double normalizedEnthalpy,
                double storedThermoCoreEnthalpy,
                double storedRecoveredTemperature)
            {
                Role = role;
                Phase = phase;
                PressureBar = pressureBar;
                ReferenceTemperature = referenceTemperature;
                NistEnthalpy = nistEnthalpy;
                NormalizedEnthalpy = normalizedEnthalpy;
                StoredThermoCoreEnthalpy = storedThermoCoreEnthalpy;
                StoredRecoveredTemperature = storedRecoveredTemperature;
            }

            public string Role { get; }
            public string Phase { get; }
            public double PressureBar { get; }
            public double ReferenceTemperature { get; }
            public double NistEnthalpy { get; }
            public double NormalizedEnthalpy { get; }
            public double StoredThermoCoreEnthalpy { get; }
            public double StoredRecoveredTemperature { get; }
        }

        private sealed class ShomateCoefficients
        {
            public ShomateCoefficients(
                double a,
                double b,
                double c,
                double d,
                double e,
                double f,
                double g,
                double h)
            {
                A = a;
                B = b;
                C = c;
                D = d;
                E = e;
                F = f;
                G = g;
                H = h;
            }

            public double A { get; }
            public double B { get; }
            public double C { get; }
            public double D { get; }
            public double E { get; }
            public double F { get; }
            public double G { get; }
            public double H { get; }
        }
    }
}
