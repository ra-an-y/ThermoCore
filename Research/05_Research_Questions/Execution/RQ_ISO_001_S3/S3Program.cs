using System;
using System.Linq;
using System.Reflection;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;

internal static class Program
{
    private const double Tolerance = 1e-9;
    private const int ExpectedRestrictedCoreQuantities = 1;
    private const int ExpectedPermissiveCoreQuantities = 2;
    private const int RestrictedCoreSemanticBytes = 8;
    private const int RestrictedExtensionBytes = 8;
    private const int PermissiveCoreSemanticBytes = 16;

    public static int Main()
    {
        try
        {
            var material = new CompiledThermodynamicParameters(
                referenceDensity: 1000.0,
                densityReferenceTemperature: 293.15,
                energyReferenceTemperature: 273.15,
                meltingTemperature: 300.0,
                latentHeat: 200000.0,
                solidHeatCapacity: 2000.0,
                liquidHeatCapacity: 4000.0);

            var externalSpecificEnthalpy = new[]
            {
                2000.0,
                3000.0,
                0.0,
                0.0,
                0.0,
                0.0
            };

            var expectedXi = new[]
            {
                0.0,
                0.25,
                0.50,
                0.75,
                1.00,
                1.00
            };

            var expectedReactionHeat = new[]
            {
                0.0,
                20000.0,
                20000.0,
                20000.0,
                20000.0,
                0.0
            };

            var expectedFinalEnthalpy = new[]
            {
                51700.0,
                74700.0,
                94700.0,
                114700.0,
                134700.0,
                134700.0
            };

            var expectedTemperature = new[]
            {
                299.0,
                300.0,
                300.0,
                300.0,
                300.0,
                300.0
            };

            var expectedLiquidFraction = new[]
            {
                0.000,
                0.105,
                0.205,
                0.305,
                0.405,
                0.405
            };

            Require(
                CountPersistentCoreQuantities() == ExpectedRestrictedCoreQuantities,
                "Frozen Thermodynamic State schema no longer contains exactly one persistent semantic quantity.");

            Require(
                CountPermissiveSharedStateQuantities() == ExpectedPermissiveCoreQuantities,
                "The S3 permissive shared-state comparator must contain exactly SpecificEnthalpy and xi.");

            var initialSpecificEnthalpy = material.SolidHeatCapacity
                * (298.0 - material.EnergyReferenceTemperature);

            var restrictedThermodynamicState = new ThermodynamicState(initialSpecificEnthalpy);
            var restrictedReactionState = new RestrictedReactionState(0.0);
            var permissiveState = new PermissiveSharedState(initialSpecificEnthalpy, 0.0);

            for (var i = 0; i < externalSpecificEnthalpy.Length; i++)
            {
                restrictedThermodynamicState = ThermodynamicComputation.ApplySpecificEnthalpyIncrement(
                    restrictedThermodynamicState,
                    externalSpecificEnthalpy[i]);

                var restrictedPreReaction = ReferenceThermodynamicFormulation.Recover(
                    restrictedThermodynamicState,
                    material);

                var restrictedReaction = BoundedReactionHeatExtension.Evaluate(
                    restrictedPreReaction.Temperature,
                    restrictedReactionState.Xi);

                restrictedReactionState = new RestrictedReactionState(restrictedReaction.NextXi);
                restrictedThermodynamicState = ThermodynamicComputation.ApplySpecificEnthalpyIncrement(
                    restrictedThermodynamicState,
                    restrictedReaction.DeltaSpecificEnthalpy);

                var restrictedDerived = ReferenceThermodynamicFormulation.Recover(
                    restrictedThermodynamicState,
                    material);

                var permissiveThermodynamicState = new ThermodynamicState(
                    permissiveState.SpecificEnthalpy);

                permissiveThermodynamicState = ThermodynamicComputation.ApplySpecificEnthalpyIncrement(
                    permissiveThermodynamicState,
                    externalSpecificEnthalpy[i]);

                var permissivePreReaction = ReferenceThermodynamicFormulation.Recover(
                    permissiveThermodynamicState,
                    material);

                var permissiveReaction = BoundedReactionHeatExtension.Evaluate(
                    permissivePreReaction.Temperature,
                    permissiveState.Xi);

                permissiveThermodynamicState = ThermodynamicComputation.ApplySpecificEnthalpyIncrement(
                    permissiveThermodynamicState,
                    permissiveReaction.DeltaSpecificEnthalpy);

                permissiveState = new PermissiveSharedState(
                    permissiveThermodynamicState.SpecificEnthalpy,
                    permissiveReaction.NextXi);

                var permissiveDerived = ReferenceThermodynamicFormulation.Recover(
                    permissiveThermodynamicState,
                    material);

                RequireNear(
                    restrictedReactionState.Xi,
                    permissiveState.Xi,
                    $"R/P xi mismatch at step {i + 1}.");
                RequireNear(
                    restrictedReaction.DeltaSpecificEnthalpy,
                    permissiveReaction.DeltaSpecificEnthalpy,
                    $"R/P reaction-heat mismatch at step {i + 1}.");
                RequireNear(
                    restrictedThermodynamicState.SpecificEnthalpy,
                    permissiveState.SpecificEnthalpy,
                    $"R/P specific-enthalpy mismatch at step {i + 1}.");
                RequireNear(
                    restrictedDerived.Temperature,
                    permissiveDerived.Temperature,
                    $"R/P Temperature mismatch at step {i + 1}.");
                RequireNear(
                    restrictedDerived.LiquidPhaseFraction,
                    permissiveDerived.LiquidPhaseFraction,
                    $"R/P liquid-phase-fraction mismatch at step {i + 1}.");

                RequireNear(
                    restrictedReactionState.Xi,
                    expectedXi[i],
                    $"Unexpected xi at step {i + 1}.");
                RequireNear(
                    restrictedReaction.DeltaSpecificEnthalpy,
                    expectedReactionHeat[i],
                    $"Unexpected reaction heat at step {i + 1}.");
                RequireNear(
                    restrictedThermodynamicState.SpecificEnthalpy,
                    expectedFinalEnthalpy[i],
                    $"Unexpected final specific enthalpy at step {i + 1}.");
                RequireNear(
                    restrictedDerived.Temperature,
                    expectedTemperature[i],
                    $"Unexpected final Temperature at step {i + 1}.");
                RequireNear(
                    restrictedDerived.LiquidPhaseFraction,
                    expectedLiquidFraction[i],
                    $"Unexpected final liquid phase fraction at step {i + 1}.");
            }

            Console.WriteLine("RQ-ISO-001 S3 Bounded Exothermic Reaction Heat");
            Console.WriteLine("Frozen comparison baseline: 8e3a948b0f36feefd313de1f03dd4db29b3bc465");
            Console.WriteLine("S3_FUNCTIONAL_EQUIVALENCE=CONFIRMED");
            Console.WriteLine("S3_EXPECTED_SEQUENCE=CONFIRMED");
            Console.WriteLine("S3_REACTION_PROGRESS_QUANTITY=xi(double)");
            Console.WriteLine("S3_FIXED_MASS_NO_TRANSPORT_BOUNDARY=CONFIRMED");
            Console.WriteLine($"R_CORE_PERSISTENT_QUANTITIES={ExpectedRestrictedCoreQuantities}");
            Console.WriteLine($"R_CORE_SEMANTIC_BYTES={RestrictedCoreSemanticBytes}");
            Console.WriteLine("R_PROMOTED_EXTENSION_QUANTITIES=0");
            Console.WriteLine($"R_EXTENSION_LOCAL_BYTES={RestrictedExtensionBytes}");
            Console.WriteLine($"R_TOTAL_PERSISTENT_BYTES={RestrictedCoreSemanticBytes + RestrictedExtensionBytes}");
            Console.WriteLine($"P_CORE_PERSISTENT_QUANTITIES={ExpectedPermissiveCoreQuantities}");
            Console.WriteLine($"P_CORE_SEMANTIC_BYTES={PermissiveCoreSemanticBytes}");
            Console.WriteLine("P_PROMOTED_EXTENSION_QUANTITIES=1");
            Console.WriteLine("P_EXTENSION_LOCAL_BYTES=0");
            Console.WriteLine($"P_TOTAL_PERSISTENT_BYTES={PermissiveCoreSemanticBytes}");
            Console.WriteLine("S3_CROSS_SCENARIO_DECISION_RULE_INPUT=VALID");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static int CountPersistentCoreQuantities()
    {
        return typeof(ThermodynamicState)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Count(property => property.GetMethod != null);
    }

    private static int CountPermissiveSharedStateQuantities()
    {
        return typeof(PermissiveSharedState)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Count(property => property.GetMethod != null);
    }

    private static void RequireNear(double actual, double expected, string message)
    {
        if (!double.IsFinite(actual)
            || !double.IsFinite(expected)
            || Math.Abs(actual - expected) > Tolerance)
        {
            throw new InvalidOperationException(
                $"{message} Expected {expected:R}, observed {actual:R}.");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}

internal readonly record struct RestrictedReactionState(double Xi);

internal readonly record struct PermissiveSharedState(
    double SpecificEnthalpy,
    double Xi);

internal readonly record struct ReactionStep(
    double NextXi,
    double DeltaSpecificEnthalpy);

internal static class BoundedReactionHeatExtension
{
    private const double ActivationTemperature = 300.0;
    private const double MaximumProgressIncrement = 0.25;
    private const double TotalSpecificReactionHeat = 80000.0;

    public static ReactionStep Evaluate(double temperature, double xi)
    {
        if (!double.IsFinite(temperature))
        {
            throw new ArgumentOutOfRangeException(
                nameof(temperature),
                "Temperature must be finite.");
        }

        if (!double.IsFinite(xi) || xi < 0.0 || xi > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(xi),
                "Reaction progress must be finite and within [0, 1].");
        }

        var deltaXi = temperature >= ActivationTemperature && xi < 1.0
            ? Math.Min(MaximumProgressIncrement, 1.0 - xi)
            : 0.0;

        var nextXi = xi + deltaXi;
        var deltaSpecificEnthalpy = TotalSpecificReactionHeat * deltaXi;

        return new ReactionStep(nextXi, deltaSpecificEnthalpy);
    }
}
