using ThermoCore.Framework.Runtime;

namespace ThermoCore.Framework.Core
{
    /// <summary>
    /// Pure thermodynamic relations for the bounded constant-heat-capacity
    /// reference implementation profile.
    /// </summary>
    public static class ReferenceThermodynamicFormulation
    {
        public static DerivedThermodynamicState Recover(
            in ThermodynamicState state,
            in CompiledThermodynamicParameters material)
        {
            var h = state.SpecificEnthalpy;
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;

            double temperature;
            double liquidFraction;

            if (h < hSolid)
            {
                temperature = material.MeltingTemperature
                    + (h - hSolid) / material.SolidHeatCapacity;
                liquidFraction = 0.0;
            }
            else if (h <= hLiquid)
            {
                temperature = material.MeltingTemperature;
                liquidFraction = (h - hSolid) / material.LatentHeat;
            }
            else
            {
                temperature = material.MeltingTemperature
                    + (h - hLiquid) / material.LiquidHeatCapacity;
                liquidFraction = 1.0;
            }

            return new DerivedThermodynamicState(temperature, liquidFraction);
        }

        public static double RecoverTemperature(
            in ThermodynamicState state,
            in CompiledThermodynamicParameters material)
        {
            return Recover(state, material).Temperature;
        }

        public static double RecoverLiquidPhaseFraction(
            in ThermodynamicState state,
            in CompiledThermodynamicParameters material)
        {
            return Recover(state, material).LiquidPhaseFraction;
        }
    }
}
