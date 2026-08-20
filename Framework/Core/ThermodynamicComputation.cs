using System;
using ThermoCore.Framework.Runtime;

namespace ThermoCore.Framework.Core
{
    /// <summary>
    /// State-evolution operations for the bounded reference implementation.
    ///
    /// This is the implementation responsibility that modifies Thermodynamic
    /// State. Energy Input is converted to a specific-enthalpy increment before
    /// being applied here.
    /// </summary>
    public static class ThermodynamicComputation
    {
        public static ThermodynamicState ApplySpecificEnthalpyIncrement(
            in ThermodynamicState state,
            double deltaSpecificEnthalpy)
        {
            if (double.IsNaN(deltaSpecificEnthalpy)
                || double.IsInfinity(deltaSpecificEnthalpy))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deltaSpecificEnthalpy),
                    "Specific-enthalpy increment must be finite.");
            }

            return new ThermodynamicState(
                state.SpecificEnthalpy + deltaSpecificEnthalpy);
        }

        public static ThermodynamicState ApplyCellEnergy(
            in ThermodynamicState state,
            double deltaEnergy,
            double cellMass)
        {
            var deltaH = EnergyInputMapping.FromCellEnergy(deltaEnergy, cellMass);
            return ApplySpecificEnthalpyIncrement(state, deltaH);
        }

        public static ThermodynamicState ApplyPower(
            in ThermodynamicState state,
            double power,
            double deltaTime,
            double cellMass)
        {
            var deltaH = EnergyInputMapping.FromPower(power, deltaTime, cellMass);
            return ApplySpecificEnthalpyIncrement(state, deltaH);
        }

        public static ThermodynamicState ApplyBoundaryHeatFlux(
            in ThermodynamicState state,
            double heatFlux,
            double affectedArea,
            double deltaTime,
            double cellMass)
        {
            var deltaH = EnergyInputMapping.FromBoundaryHeatFlux(
                heatFlux,
                affectedArea,
                deltaTime,
                cellMass);

            return ApplySpecificEnthalpyIncrement(state, deltaH);
        }

        public static ThermodynamicState ApplyVolumetricHeatSource(
            in ThermodynamicState state,
            double volumetricHeatSource,
            double deltaTime,
            CompiledThermodynamicParameters material)
        {
            var deltaH = EnergyInputMapping.FromVolumetricHeatSource(
                volumetricHeatSource,
                deltaTime,
                material);

            return ApplySpecificEnthalpyIncrement(state, deltaH);
        }
    }
}
