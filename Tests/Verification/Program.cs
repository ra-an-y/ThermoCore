using System;
using System.Collections.Generic;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Tests.Verification
{
    internal static class Program
    {
        private const double Tolerance = 1e-9;

        private static int Main()
        {
            var tests = new List<(string Name, Action Body)>
            {
                ("compile_preserves_reference_semantics", CompilePreservesReferenceSemantics),
                ("energy_zero_recovers_T_E_ref", EnergyZeroRecoversReferenceTemperature),
                ("solid_branch_recovery", SolidBranchRecovery),
                ("latent_interval_recovery", LatentIntervalRecovery),
                ("liquid_branch_recovery", LiquidBranchRecovery),
                ("latent_width_equals_L", LatentWidthEqualsLatentHeat),
                ("recovery_is_monotonic_and_phi_bounded", RecoveryIsMonotonicAndPhaseFractionBounded),
                ("reference_datum_shift_preserves_physical_recovery", ReferenceDatumShiftPreservesPhysicalRecovery),
                ("cell_mass_uses_rho_ref", CellMassUsesReferenceDensity),
                ("energy_input_dimensional_mapping", EnergyInputDimensionalMapping),
                ("signed_energy_removal", SignedEnergyRemoval),
                ("latent_energy_conservation", LatentEnergyConservation),
                ("state_update_is_immutable", StateUpdateIsImmutable),
                ("compiler_rejects_ambiguous_reference_datum", CompilerRejectsAmbiguousReferenceDatum),
                ("material_definition_rejects_invalid_density", MaterialDefinitionRejectsInvalidDensity),
                ("non_finite_state_is_rejected", NonFiniteStateIsRejected)
            };

            var failed = 0;
            foreach (var test in tests)
            {
                try
                {
                    test.Body();
                    Console.WriteLine($"PASS {test.Name}");
                }
                catch (Exception ex)
                {
                    failed++;
                    Console.Error.WriteLine($"FAIL {test.Name}: {ex.GetType().Name}: {ex.Message}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Reference verification: {tests.Count - failed}/{tests.Count} passed.");
            return failed == 0 ? 0 : 1;
        }

        private static ReferenceMaterialDefinition CreateMaterial(
            double energyReferenceTemperature = 250.0,
            double meltingTemperature = 300.0)
        {
            return new ReferenceMaterialDefinition(
                materialId: "verification-material",
                provenance: "Deterministic verification fixture",
                referenceDensity: 800.0,
                densityReferenceTemperature: 293.15,
                energyReferenceTemperature: energyReferenceTemperature,
                meltingTemperature: meltingTemperature,
                latentHeat: 100_000.0,
                solidHeatCapacity: 2_000.0,
                liquidHeatCapacity: 2_500.0);
        }

        private static CompiledThermodynamicParameters CompileMaterial()
        {
            return ReferenceMaterialCompiler.Compile(CreateMaterial());
        }

        private static void CompilePreservesReferenceSemantics()
        {
            var material = CompileMaterial();

            AssertNear(800.0, material.ReferenceDensity);
            AssertNear(293.15, material.DensityReferenceTemperature);
            AssertNear(250.0, material.EnergyReferenceTemperature);
            AssertNear(300.0, material.MeltingTemperature);
            AssertNear(100_000.0, material.SolidTransitionEnthalpy);
            AssertNear(200_000.0, material.LiquidTransitionEnthalpy);
        }

        private static void EnergyZeroRecoversReferenceTemperature()
        {
            var material = CompileMaterial();
            var derived = ReferenceThermodynamicFormulation.Recover(
                new ThermodynamicState(0.0),
                material);

            AssertNear(material.EnergyReferenceTemperature, derived.Temperature);
            AssertNear(0.0, derived.LiquidPhaseFraction);
        }

        private static void SolidBranchRecovery()
        {
            var material = CompileMaterial();
            var derived = ReferenceThermodynamicFormulation.Recover(
                new ThermodynamicState(50_000.0),
                material);

            AssertNear(275.0, derived.Temperature);
            AssertNear(0.0, derived.LiquidPhaseFraction);
        }

        private static void LatentIntervalRecovery()
        {
            var material = CompileMaterial();

            var atSolidBoundary = ReferenceThermodynamicFormulation.Recover(
                new ThermodynamicState(material.SolidTransitionEnthalpy),
                material);
            AssertNear(300.0, atSolidBoundary.Temperature);
            AssertNear(0.0, atSolidBoundary.LiquidPhaseFraction);

            var midpoint = ReferenceThermodynamicFormulation.Recover(
                new ThermodynamicState(150_000.0),
                material);
            AssertNear(300.0, midpoint.Temperature);
            AssertNear(0.5, midpoint.LiquidPhaseFraction);

            var atLiquidBoundary = ReferenceThermodynamicFormulation.Recover(
                new ThermodynamicState(material.LiquidTransitionEnthalpy),
                material);
            AssertNear(300.0, atLiquidBoundary.Temperature);
            AssertNear(1.0, atLiquidBoundary.LiquidPhaseFraction);
        }

        private static void LiquidBranchRecovery()
        {
            var material = CompileMaterial();
            var derived = ReferenceThermodynamicFormulation.Recover(
                new ThermodynamicState(225_000.0),
                material);

            AssertNear(310.0, derived.Temperature);
            AssertNear(1.0, derived.LiquidPhaseFraction);
        }

        private static void LatentWidthEqualsLatentHeat()
        {
            var material = CompileMaterial();
            AssertNear(
                material.LatentHeat,
                material.LiquidTransitionEnthalpy - material.SolidTransitionEnthalpy);
        }

        private static void RecoveryIsMonotonicAndPhaseFractionBounded()
        {
            var material = CompileMaterial();
            var previousTemperature = double.NegativeInfinity;
            var previousPhaseFraction = double.NegativeInfinity;

            const int samples = 1_000;
            const double startEnthalpy = 0.0;
            const double endEnthalpy = 250_000.0;

            for (var i = 0; i <= samples; i++)
            {
                var h = startEnthalpy
                    + (endEnthalpy - startEnthalpy) * i / samples;
                var derived = ReferenceThermodynamicFormulation.Recover(
                    new ThermodynamicState(h),
                    material);

                if (derived.Temperature + Tolerance < previousTemperature)
                {
                    throw new InvalidOperationException(
                        "Recovered Temperature must be monotonic non-decreasing in h.");
                }

                if (derived.LiquidPhaseFraction + Tolerance < previousPhaseFraction)
                {
                    throw new InvalidOperationException(
                        "Recovered liquid Phase Fraction must be monotonic non-decreasing in h.");
                }

                if (derived.LiquidPhaseFraction < -Tolerance
                    || derived.LiquidPhaseFraction > 1.0 + Tolerance)
                {
                    throw new InvalidOperationException(
                        "Recovered liquid Phase Fraction must remain in [0, 1].");
                }

                previousTemperature = derived.Temperature;
                previousPhaseFraction = derived.LiquidPhaseFraction;
            }
        }

        private static void ReferenceDatumShiftPreservesPhysicalRecovery()
        {
            var first = ReferenceMaterialCompiler.Compile(CreateMaterial(250.0));
            var shifted = ReferenceMaterialCompiler.Compile(CreateMaterial(260.0));

            const double solidTemperature = 275.0;
            var firstSolidEnthalpy = first.SolidHeatCapacity
                * (solidTemperature - first.EnergyReferenceTemperature);
            var shiftedSolidEnthalpy = shifted.SolidHeatCapacity
                * (solidTemperature - shifted.EnergyReferenceTemperature);

            AssertNear(
                solidTemperature,
                ReferenceThermodynamicFormulation.RecoverTemperature(
                    new ThermodynamicState(firstSolidEnthalpy),
                    first));
            AssertNear(
                solidTemperature,
                ReferenceThermodynamicFormulation.RecoverTemperature(
                    new ThermodynamicState(shiftedSolidEnthalpy),
                    shifted));

            const double secondSolidTemperature = 290.0;
            var firstSecondEnthalpy = first.SolidHeatCapacity
                * (secondSolidTemperature - first.EnergyReferenceTemperature);
            var shiftedSecondEnthalpy = shifted.SolidHeatCapacity
                * (secondSolidTemperature - shifted.EnergyReferenceTemperature);

            AssertNear(
                firstSecondEnthalpy - firstSolidEnthalpy,
                shiftedSecondEnthalpy - shiftedSolidEnthalpy);

            const double liquidTemperature = 310.0;
            var firstLiquidEnthalpy = first.LiquidTransitionEnthalpy
                + first.LiquidHeatCapacity
                * (liquidTemperature - first.MeltingTemperature);
            var shiftedLiquidEnthalpy = shifted.LiquidTransitionEnthalpy
                + shifted.LiquidHeatCapacity
                * (liquidTemperature - shifted.MeltingTemperature);

            AssertNear(
                liquidTemperature,
                ReferenceThermodynamicFormulation.RecoverTemperature(
                    new ThermodynamicState(firstLiquidEnthalpy),
                    first));
            AssertNear(
                liquidTemperature,
                ReferenceThermodynamicFormulation.RecoverTemperature(
                    new ThermodynamicState(shiftedLiquidEnthalpy),
                    shifted));
            AssertNear(first.LatentHeat, shifted.LatentHeat);
            AssertNear(
                first.LiquidTransitionEnthalpy - first.SolidTransitionEnthalpy,
                shifted.LiquidTransitionEnthalpy - shifted.SolidTransitionEnthalpy);
        }

        private static void CellMassUsesReferenceDensity()
        {
            var material = CompileMaterial();
            AssertNear(8.0, EnergyInputMapping.CellMass(0.01, material));
        }

        private static void EnergyInputDimensionalMapping()
        {
            var material = CompileMaterial();
            var cellMass = EnergyInputMapping.CellMass(0.01, material);

            AssertNear(1_000.0, EnergyInputMapping.FromCellEnergy(8_000.0, cellMass));
            AssertNear(1_000.0, EnergyInputMapping.FromPower(4_000.0, 2.0, cellMass));
            AssertNear(
                1.0,
                EnergyInputMapping.FromBoundaryHeatFlux(
                    heatFlux: 2_000.0,
                    affectedArea: 0.002,
                    deltaTime: 2.0,
                    cellMass: cellMass));
            AssertNear(
                1_000.0,
                EnergyInputMapping.FromVolumetricHeatSource(
                    volumetricHeatSource: 400_000.0,
                    deltaTime: 2.0,
                    material: material));
        }

        private static void SignedEnergyRemoval()
        {
            var material = CompileMaterial();
            var cellMass = EnergyInputMapping.CellMass(0.01, material);
            AssertNear(-1_000.0, EnergyInputMapping.FromCellEnergy(-8_000.0, cellMass));
        }

        private static void LatentEnergyConservation()
        {
            var material = CompileMaterial();
            var cellMass = EnergyInputMapping.CellMass(0.01, material);
            var initial = new ThermodynamicState(material.SolidTransitionEnthalpy);
            var requiredLatentEnergy = cellMass * material.LatentHeat;

            var final = ThermodynamicComputation.ApplyCellEnergy(
                initial,
                requiredLatentEnergy,
                cellMass);

            AssertNear(material.LiquidTransitionEnthalpy, final.SpecificEnthalpy);
            var derived = ReferenceThermodynamicFormulation.Recover(final, material);
            AssertNear(material.MeltingTemperature, derived.Temperature);
            AssertNear(1.0, derived.LiquidPhaseFraction);
        }

        private static void StateUpdateIsImmutable()
        {
            var initial = new ThermodynamicState(10.0);
            var updated = ThermodynamicComputation.ApplySpecificEnthalpyIncrement(initial, 5.0);

            AssertNear(10.0, initial.SpecificEnthalpy);
            AssertNear(15.0, updated.SpecificEnthalpy);
        }

        private static void CompilerRejectsAmbiguousReferenceDatum()
        {
            AssertThrows<NotSupportedException>(() =>
                ReferenceMaterialCompiler.Compile(
                    CreateMaterial(
                        energyReferenceTemperature: 300.0,
                        meltingTemperature: 300.0)));
        }

        private static void MaterialDefinitionRejectsInvalidDensity()
        {
            AssertThrows<ArgumentOutOfRangeException>(() =>
                new ReferenceMaterialDefinition(
                    materialId: "invalid-density",
                    provenance: "Deterministic verification fixture",
                    referenceDensity: 0.0,
                    densityReferenceTemperature: 293.15,
                    energyReferenceTemperature: 250.0,
                    meltingTemperature: 300.0,
                    latentHeat: 100_000.0,
                    solidHeatCapacity: 2_000.0,
                    liquidHeatCapacity: 2_500.0));
        }

        private static void NonFiniteStateIsRejected()
        {
            AssertThrows<ArgumentOutOfRangeException>(() =>
                new ThermodynamicState(double.NaN));
        }

        private static void AssertNear(double expected, double actual)
        {
            if (Math.Abs(expected - actual) > Tolerance)
            {
                throw new InvalidOperationException(
                    $"Expected {expected:R}, actual {actual:R}.");
            }
        }

        private static void AssertThrows<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Expected exception {typeof(TException).Name} was not thrown.");
        }
    }
}
