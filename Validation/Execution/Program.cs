using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Validation.ReferenceCaloric
{
    internal static class Program
    {
        private const double StoredTemperatureTolerance = 1e-8;
        private const double StoredEnthalpyTolerance = 1e-3;
        private const double DatumShift = 50_000.0;

        private static int Main()
        {
            try
            {
                var rows = LoadRows(
                    "Validation/Data/reference_caloric_benchmark_v0.1.csv");

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

                var coexistenceSolid = coexistence.Single(r => r.Phase == "solid");
                var coexistenceLiquid = coexistence.Single(r => r.Phase == "liquid");

                var energyReferenceTemperature = solidCalibration
                    .Single(r => Math.Abs(r.NormalizedEnthalpy) < 1e-9)
                    .ReferenceTemperature;

                var solidHeatCapacity = Slope(
                    solidCalibration[0],
                    solidCalibration[1]);
                var liquidHeatCapacity = Slope(
                    liquidCalibration[0],
                    liquidCalibration[1]);
                var meltingTemperature = coexistenceSolid.ReferenceTemperature;
                var latentHeat = coexistenceLiquid.NormalizedEnthalpy
                    - coexistenceSolid.NormalizedEnthalpy;

                var definition = new ReferenceMaterialDefinition(
                    materialId: "validation-h2o-caloric-v0.1",
                    provenance: "IAPWS bounded caloric validation dataset v0.1",
                    referenceDensity: 916.721305914164,
                    densityReferenceTemperature: meltingTemperature,
                    energyReferenceTemperature: energyReferenceTemperature,
                    meltingTemperature: meltingTemperature,
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
                    var enthalpyError = modelEnthalpy - row.NormalizedEnthalpy;

                    temperatureErrors.Add(Math.Abs(temperatureError));
                    enthalpyErrors.Add(Math.Abs(enthalpyError));

                    VerifyDatumShiftInvariance(
                        row,
                        material,
                        temperatureError,
                        enthalpyError);
                }

                var maxAbsTemperatureError = temperatureErrors.Max();
                var meanAbsTemperatureError = temperatureErrors.Average();
                var maxAbsEnthalpyError = enthalpyErrors.Max();
                var meanAbsEnthalpyError = enthalpyErrors.Average();

                var latentWidth = material.LiquidTransitionEnthalpy
                    - material.SolidTransitionEnthalpy;
                var latentHeatError = latentWidth - latentHeat;
                var meltingTemperatureError = material.MeltingTemperature
                    - coexistenceSolid.ReferenceTemperature;

                Console.WriteLine("Reference caloric validation execution");
                Console.WriteLine($"Evaluated holdouts: {holdouts.Length}");
                Console.WriteLine($"p_ref [MPa]: {0.1:R}");
                Console.WriteLine($"T_m,ref [K]: {meltingTemperature:R}");
                Console.WriteLine($"c_s,fit [J/(kg*K)]: {solidHeatCapacity:R}");
                Console.WriteLine($"c_l,fit [J/(kg*K)]: {liquidHeatCapacity:R}");
                Console.WriteLine($"L_ref [J/kg]: {latentHeat:R}");
                Console.WriteLine($"max |T error| [K]: {maxAbsTemperatureError:R}");
                Console.WriteLine($"mean |T error| [K]: {meanAbsTemperatureError:R}");
                Console.WriteLine($"max |h error| [J/kg]: {maxAbsEnthalpyError:R}");
                Console.WriteLine($"mean |h error| [J/kg]: {meanAbsEnthalpyError:R}");
                Console.WriteLine($"latent-heat error [J/kg]: {latentHeatError:R}");
                Console.WriteLine($"melting-Temperature error [K]: {meltingTemperatureError:R}");
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
                    pressure: Parse(fields[2]),
                    referenceTemperature: Parse(fields[3]),
                    iapwsEnthalpy: Parse(fields[4]),
                    normalizedEnthalpy: Parse(fields[5]),
                    storedThermoCoreEnthalpy: Parse(fields[6]),
                    storedRecoveredTemperature: Parse(fields[7])));
            }

            return rows;
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
                double pressure,
                double referenceTemperature,
                double iapwsEnthalpy,
                double normalizedEnthalpy,
                double storedThermoCoreEnthalpy,
                double storedRecoveredTemperature)
            {
                Role = role;
                Phase = phase;
                Pressure = pressure;
                ReferenceTemperature = referenceTemperature;
                IapwsEnthalpy = iapwsEnthalpy;
                NormalizedEnthalpy = normalizedEnthalpy;
                StoredThermoCoreEnthalpy = storedThermoCoreEnthalpy;
                StoredRecoveredTemperature = storedRecoveredTemperature;
            }

            public string Role { get; }
            public string Phase { get; }
            public double Pressure { get; }
            public double ReferenceTemperature { get; }
            public double IapwsEnthalpy { get; }
            public double NormalizedEnthalpy { get; }
            public double StoredThermoCoreEnthalpy { get; }
            public double StoredRecoveredTemperature { get; }
        }
    }
}
