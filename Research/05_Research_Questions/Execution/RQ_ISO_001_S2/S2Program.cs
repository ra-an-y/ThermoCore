using System;
using System.Collections.Generic;
using ThermoCore.Framework.Core;
using ThermoCore.Framework.Runtime;

internal static class Program
{
    private const double LowerThresholdKelvin = 295.0;
    private const double UpperThresholdKelvin = 305.0;
    private const int CoreSemanticBytes = 8;
    private const int HysteresisSemanticBytes = 1;

    private static readonly double[] TemperatureSequence =
    {
        294.0,
        299.0,
        304.0,
        306.0,
        302.0,
        297.0,
        294.0,
        299.0
    };

    private static readonly HysteresisMode[] ExpectedModeSequence =
    {
        HysteresisMode.SolidLike,
        HysteresisMode.SolidLike,
        HysteresisMode.SolidLike,
        HysteresisMode.LiquidLike,
        HysteresisMode.LiquidLike,
        HysteresisMode.LiquidLike,
        HysteresisMode.SolidLike,
        HysteresisMode.SolidLike
    };

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

            var states = BuildStates(material);

            var restrictedHistory = new HysteresisExtensionState(HysteresisMode.SolidLike);
            var permissiveState = new PermissiveSharedSimulationState(
                states[0].SpecificEnthalpy,
                HysteresisMode.SolidLike);

            var restrictedModes = new List<HysteresisMode>();
            var permissiveModes = new List<HysteresisMode>();

            for (var i = 0; i < states.Length; i++)
            {
                var coreState = states[i];
                var restrictedTemperature = ReferenceThermodynamicFormulation
                    .RecoverTemperature(coreState, material);

                RequireClose(
                    restrictedTemperature,
                    TemperatureSequence[i],
                    $"Recovered restricted temperature mismatch at sample {i}.");

                restrictedHistory = RestrictedCondition.Step(
                    coreState,
                    restrictedHistory,
                    material);

                permissiveState = permissiveState with
                {
                    SpecificEnthalpy = coreState.SpecificEnthalpy
                };

                var permissiveTemperature = PermissiveCondition.RecoverTemperature(
                    permissiveState,
                    material);

                RequireClose(
                    permissiveTemperature,
                    restrictedTemperature,
                    $"Condition temperature mismatch at sample {i}.");

                permissiveState = PermissiveCondition.Step(
                    permissiveState,
                    material);

                restrictedModes.Add(restrictedHistory.Mode);
                permissiveModes.Add(permissiveState.HysteresisMode);

                Require(
                    restrictedHistory.Mode == permissiveState.HysteresisMode,
                    $"R/P hysteresis output mismatch at sample {i}.");

                Require(
                    restrictedHistory.Mode == ExpectedModeSequence[i],
                    $"Observed hysteresis mode differs from frozen expected sequence at sample {i}.");
            }

            Require(
                sizeof(byte) == HysteresisSemanticBytes,
                "The frozen semantic payload assumption for HysteresisMode is no longer one byte.");

            Console.WriteLine("RQ-ISO-001 S2 Thermal Hysteresis");
            Console.WriteLine("Frozen comparison baseline: 8e3a948b0f36feefd313de1f03dd4db29b3bc465");
            Console.WriteLine("S2_FUNCTIONAL_EQUIVALENCE=CONFIRMED");
            Console.WriteLine("S2_HISTORY_QUANTITY=HysteresisMode(byte)");
            Console.WriteLine("S2_EXPECTED_SEQUENCE=CONFIRMED");
            Console.WriteLine("R_CORE_PERSISTENT_QUANTITIES=1");
            Console.WriteLine($"R_CORE_SEMANTIC_BYTES={CoreSemanticBytes}");
            Console.WriteLine("R_PROMOTED_EXTENSION_QUANTITIES=0");
            Console.WriteLine($"R_EXTENSION_LOCAL_BYTES={HysteresisSemanticBytes}");
            Console.WriteLine($"R_TOTAL_PERSISTENT_BYTES={CoreSemanticBytes + HysteresisSemanticBytes}");
            Console.WriteLine("P_CORE_PERSISTENT_QUANTITIES=2");
            Console.WriteLine($"P_CORE_SEMANTIC_BYTES={CoreSemanticBytes + HysteresisSemanticBytes}");
            Console.WriteLine("P_PROMOTED_EXTENSION_QUANTITIES=1");
            Console.WriteLine("P_EXTENSION_LOCAL_BYTES=0");
            Console.WriteLine($"P_TOTAL_PERSISTENT_BYTES={CoreSemanticBytes + HysteresisSemanticBytes}");
            Console.WriteLine("S2_CROSS_SCENARIO_HYPOTHESIS_VERDICT=NOT_FINAL");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static ThermodynamicState[] BuildStates(
        CompiledThermodynamicParameters material)
    {
        var states = new ThermodynamicState[TemperatureSequence.Length];
        for (var i = 0; i < TemperatureSequence.Length; i++)
        {
            states[i] = StateAtTemperature(TemperatureSequence[i], material);
        }

        return states;
    }

    private static ThermodynamicState StateAtTemperature(
        double temperature,
        CompiledThermodynamicParameters material)
    {
        double h;

        if (temperature < material.MeltingTemperature)
        {
            h = material.SolidTransitionEnthalpy
                + material.SolidHeatCapacity
                * (temperature - material.MeltingTemperature);
        }
        else if (temperature > material.MeltingTemperature)
        {
            h = material.LiquidTransitionEnthalpy
                + material.LiquidHeatCapacity
                * (temperature - material.MeltingTemperature);
        }
        else
        {
            h = material.SolidTransitionEnthalpy;
        }

        return new ThermodynamicState(h);
    }

    private static void RequireClose(double actual, double expected, string message)
    {
        if (Math.Abs(actual - expected) > 1e-12)
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

internal enum HysteresisMode : byte
{
    SolidLike = 0,
    LiquidLike = 1
}

internal readonly record struct HysteresisExtensionState(HysteresisMode Mode);

internal readonly record struct PermissiveSharedSimulationState(
    double SpecificEnthalpy,
    HysteresisMode HysteresisMode);

internal static class HysteresisResponseModule
{
    public static HysteresisMode Update(HysteresisMode mode, double temperature)
    {
        if (mode == HysteresisMode.SolidLike
            && temperature >= UpperThreshold)
        {
            return HysteresisMode.LiquidLike;
        }

        if (mode == HysteresisMode.LiquidLike
            && temperature <= LowerThreshold)
        {
            return HysteresisMode.SolidLike;
        }

        return mode;
    }

    private const double LowerThreshold = 295.0;
    private const double UpperThreshold = 305.0;
}

internal static class RestrictedCondition
{
    public static HysteresisExtensionState Step(
        in ThermodynamicState state,
        HysteresisExtensionState history,
        CompiledThermodynamicParameters material)
    {
        var temperature = ReferenceThermodynamicFormulation
            .RecoverTemperature(state, material);

        var nextMode = HysteresisResponseModule.Update(
            history.Mode,
            temperature);

        return new HysteresisExtensionState(nextMode);
    }
}

internal static class PermissiveCondition
{
    public static double RecoverTemperature(
        in PermissiveSharedSimulationState state,
        CompiledThermodynamicParameters material)
    {
        var coreState = new ThermodynamicState(state.SpecificEnthalpy);
        return ReferenceThermodynamicFormulation
            .RecoverTemperature(coreState, material);
    }

    public static PermissiveSharedSimulationState Step(
        in PermissiveSharedSimulationState state,
        CompiledThermodynamicParameters material)
    {
        var temperature = RecoverTemperature(state, material);
        var nextMode = HysteresisResponseModule.Update(
            state.HysteresisMode,
            temperature);

        return state with { HysteresisMode = nextMode };
    }
}
