using System;
using System.Linq;
using System.Reflection;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;

internal static class Program
{
    private const int ExpectedCorePersistentQuantities = 1;
    private const int ExpectedCoreSemanticBytesPerElement = 8;

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

            var states = new[]
            {
                new ThermodynamicState(0.0),
                new ThermodynamicState(
                    0.5 * (material.SolidTransitionEnthalpy + material.LiquidTransitionEnthalpy)),
                new ThermodynamicState(material.LiquidTransitionEnthalpy + 40000.0)
            };

            Require(
                CountPersistentCoreQuantities() == ExpectedCorePersistentQuantities,
                "Frozen S0 Core-State schema no longer contains exactly one persistent semantic quantity.");

            Require(
                sizeof(double) == ExpectedCoreSemanticBytesPerElement,
                "The semantic payload assumption for SpecificEnthalpy is no longer eight bytes.");

            Require(
                CountRepresentationConsumerInstanceFields() == 0,
                "S1 representation consumer unexpectedly contains persistent instance state.");

            for (var i = 0; i < states.Length; i++)
            {
                var state = states[i];

                var restrictedDerived = RestrictedCondition.Recover(state, material);
                var permissiveDerived = PermissiveCondition.Recover(state, material);

                Require(
                    restrictedDerived.Temperature == permissiveDerived.Temperature,
                    $"S0 temperature mismatch at sample {i}.");
                Require(
                    restrictedDerived.LiquidPhaseFraction == permissiveDerived.LiquidPhaseFraction,
                    $"S0 phase-fraction mismatch at sample {i}.");

                var restrictedRepresentation = RestrictedCondition.Represent(state, material);
                var permissiveRepresentation = PermissiveCondition.Represent(state, material);

                Require(
                    restrictedRepresentation.Equals(permissiveRepresentation),
                    $"S1 representation mismatch at sample {i}.");
            }

            Console.WriteLine("RQ-ISO-001 Phase B S0-S1");
            Console.WriteLine("Frozen comparison baseline: 8e3a948b0f36feefd313de1f03dd4db29b3bc465");
            Console.WriteLine("S0_EQUIVALENCE=CONFIRMED");
            Console.WriteLine("S1_NEGATIVE_CONTROL=NEUTRAL");
            Console.WriteLine($"CORE_PERSISTENT_QUANTITIES_R={ExpectedCorePersistentQuantities}");
            Console.WriteLine($"CORE_PERSISTENT_QUANTITIES_P={ExpectedCorePersistentQuantities}");
            Console.WriteLine($"CORE_SEMANTIC_BYTES_R={ExpectedCoreSemanticBytesPerElement}");
            Console.WriteLine($"CORE_SEMANTIC_BYTES_P={ExpectedCoreSemanticBytesPerElement}");
            Console.WriteLine("S1_CONSUMER_PERSISTENT_FIELDS_R=0");
            Console.WriteLine("S1_CONSUMER_PERSISTENT_FIELDS_P=0");
            Console.WriteLine("HYPOTHESIS_SUPPORT_FROM_S0_S1=NONE");
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

    private static int CountRepresentationConsumerInstanceFields()
    {
        return typeof(DerivedRepresentationConsumer)
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Length;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}

internal static class RestrictedCondition
{
    public static DerivedThermodynamicState Recover(
        in ThermodynamicState state,
        CompiledThermodynamicParameters material)
    {
        return ReferenceThermodynamicFormulation.Recover(state, material);
    }

    public static RepresentationSample Represent(
        in ThermodynamicState state,
        CompiledThermodynamicParameters material)
    {
        var derived = Recover(state, material);
        return DerivedRepresentationConsumer.Consume(derived);
    }
}

internal static class PermissiveCondition
{
    public static DerivedThermodynamicState Recover(
        in ThermodynamicState state,
        CompiledThermodynamicParameters material)
    {
        return ReferenceThermodynamicFormulation.Recover(state, material);
    }

    public static RepresentationSample Represent(
        in ThermodynamicState state,
        CompiledThermodynamicParameters material)
    {
        var derived = Recover(state, material);
        return DerivedRepresentationConsumer.Consume(derived);
    }
}

internal static class DerivedRepresentationConsumer
{
    public static RepresentationSample Consume(in DerivedThermodynamicState state)
    {
        var phaseBand = state.LiquidPhaseFraction switch
        {
            <= 0.0 => 0,
            >= 1.0 => 2,
            _ => 1
        };

        return new RepresentationSample(
            state.Temperature,
            state.LiquidPhaseFraction,
            phaseBand);
    }
}

internal readonly record struct RepresentationSample(
    double Temperature,
    double LiquidPhaseFraction,
    int PhaseBand);
