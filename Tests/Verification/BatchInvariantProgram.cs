using System;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Tests.Verification.BatchInvariant
{
    internal static class BatchInvariantProgram
    {
        private const double Tolerance = 1e-10;

        private static int Main()
        {
            var passed = 0;
            var failed = 0;

            Run("batch_matches_scalar_across_regions", BatchMatchesScalarAcrossRegions, ref passed, ref failed);
            Run("batch_preserves_exact_normal_boundaries", BatchPreservesExactNormalBoundaries, ref passed, ref failed);
            Run("batch_rejects_nonfinite_solid_recovery", BatchRejectsNonFiniteSolidRecovery, ref passed, ref failed);
            Run("batch_rejects_nonfinite_liquid_recovery", BatchRejectsNonFiniteLiquidRecovery, ref passed, ref failed);
            Run("public_derived_constructor_still_enforces_invariants", PublicDerivedConstructorStillEnforcesInvariants, ref passed, ref failed);

            Console.WriteLine();
            Console.WriteLine($"Batch invariant verification: {passed}/{passed + failed} passed.");
            return failed == 0 ? 0 : 1;
        }

        private static CompiledThermodynamicParameters CreateNormalMaterial()
        {
            return ReferenceMaterialCompiler.Compile(
                new ReferenceMaterialDefinition(
                    materialId: "batch-invariant-verification",
                    provenance: "Deterministic targeted verification fixture",
                    referenceDensity: 800.0,
                    densityReferenceTemperature: 293.15,
                    energyReferenceTemperature: 250.0,
                    meltingTemperature: 300.0,
                    latentHeat: 100_000.0,
                    solidHeatCapacity: 2_000.0,
                    liquidHeatCapacity: 2_500.0));
        }

        private static void BatchMatchesScalarAcrossRegions()
        {
            var material = CreateNormalMaterial();
            var states = new[]
            {
                new ThermodynamicState(0.0),
                new ThermodynamicState(50_000.0),
                new ThermodynamicState(material.SolidTransitionEnthalpy),
                new ThermodynamicState(150_000.0),
                new ThermodynamicState(material.LiquidTransitionEnthalpy),
                new ThermodynamicState(225_000.0),
                new ThermodynamicState(500_000.0)
            };
            var batch = new DerivedThermodynamicState[states.Length];

            ReferenceThermodynamicFormulation.RecoverBatch(states, batch, material);

            for (var i = 0; i < states.Length; i++)
            {
                var scalar = ReferenceThermodynamicFormulation.Recover(states[i], material);
                AssertNear(scalar.Temperature, batch[i].Temperature);
                AssertNear(scalar.LiquidPhaseFraction, batch[i].LiquidPhaseFraction);
            }
        }

        private static void BatchPreservesExactNormalBoundaries()
        {
            var material = CreateNormalMaterial();
            var states = new[]
            {
                new ThermodynamicState(material.SolidTransitionEnthalpy),
                new ThermodynamicState(material.LiquidTransitionEnthalpy)
            };
            var batch = new DerivedThermodynamicState[2];

            ReferenceThermodynamicFormulation.RecoverBatch(states, batch, material);

            AssertNear(material.MeltingTemperature, batch[0].Temperature);
            AssertNear(0.0, batch[0].LiquidPhaseFraction);
            AssertNear(material.MeltingTemperature, batch[1].Temperature);
            AssertNear(1.0, batch[1].LiquidPhaseFraction);
        }

        private static void BatchRejectsNonFiniteSolidRecovery()
        {
            var material = new CompiledThermodynamicParameters(
                referenceDensity: 1.0,
                densityReferenceTemperature: 293.15,
                energyReferenceTemperature: 250.0,
                meltingTemperature: 300.0,
                latentHeat: 1.0,
                solidHeatCapacity: 1e-308,
                liquidHeatCapacity: 1.0);
            var state = new ThermodynamicState(-double.MaxValue);

            AssertThrows<ArgumentOutOfRangeException>(() =>
                ReferenceThermodynamicFormulation.Recover(state, material));

            AssertThrows<ArgumentOutOfRangeException>(() =>
            {
                var destination = new DerivedThermodynamicState[1];
                ReferenceThermodynamicFormulation.RecoverBatch(
                    new[] { state },
                    destination,
                    material);
            });
        }

        private static void BatchRejectsNonFiniteLiquidRecovery()
        {
            var material = new CompiledThermodynamicParameters(
                referenceDensity: 1.0,
                densityReferenceTemperature: 293.15,
                energyReferenceTemperature: 250.0,
                meltingTemperature: 300.0,
                latentHeat: 1.0,
                solidHeatCapacity: 1.0,
                liquidHeatCapacity: 1e-308);
            var state = new ThermodynamicState(double.MaxValue);

            AssertThrows<ArgumentOutOfRangeException>(() =>
                ReferenceThermodynamicFormulation.Recover(state, material));

            AssertThrows<ArgumentOutOfRangeException>(() =>
            {
                var destination = new DerivedThermodynamicState[1];
                ReferenceThermodynamicFormulation.RecoverBatch(
                    new[] { state },
                    destination,
                    material);
            });
        }

        private static void PublicDerivedConstructorStillEnforcesInvariants()
        {
            AssertThrows<ArgumentOutOfRangeException>(() =>
                new DerivedThermodynamicState(double.PositiveInfinity, 0.0));
            AssertThrows<ArgumentOutOfRangeException>(() =>
                new DerivedThermodynamicState(300.0, -0.01));
            AssertThrows<ArgumentOutOfRangeException>(() =>
                new DerivedThermodynamicState(300.0, 1.01));
        }

        private static void Run(
            string name,
            Action body,
            ref int passed,
            ref int failed)
        {
            try
            {
                body();
                passed++;
                Console.WriteLine($"PASS {name}");
            }
            catch (Exception ex)
            {
                failed++;
                Console.Error.WriteLine(
                    $"FAIL {name}: {ex.GetType().Name}: {ex.Message}");
            }
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
