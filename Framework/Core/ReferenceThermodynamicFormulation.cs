using System;
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
            CompiledThermodynamicParameters material)
        {
            RequireMaterial(material);
            return RecoverValidatedState(state.SpecificEnthalpy, material);
        }

        /// <summary>
        /// Recovers one transient Derived Thermodynamic State for each supplied
        /// persistent Thermodynamic State.
        ///
        /// The input states remain the owned runtime-state values. This batch
        /// operation does not mutate them and does not make the destination
        /// Derived State persistent. Material validation and destination-shape
        /// validation are performed once at the batch boundary; each recovered
        /// value retains the same numerical semantics and derived-value checks as
        /// <see cref="Recover"/>.
        /// </summary>
        public static void RecoverBatch(
            ReadOnlySpan<ThermodynamicState> states,
            Span<DerivedThermodynamicState> destination,
            CompiledThermodynamicParameters material)
        {
            RequireMaterial(material);

            if (destination.Length != states.Length)
            {
                throw new ArgumentException(
                    "Destination length must match the number of supplied Thermodynamic State values.",
                    nameof(destination));
            }

            for (var i = 0; i < states.Length; i++)
            {
                destination[i] = RecoverValidatedState(
                    states[i].SpecificEnthalpy,
                    material);
            }
        }

        public static double RecoverTemperature(
            in ThermodynamicState state,
            CompiledThermodynamicParameters material)
        {
            return Recover(state, material).Temperature;
        }

        public static double RecoverLiquidPhaseFraction(
            in ThermodynamicState state,
            CompiledThermodynamicParameters material)
        {
            return Recover(state, material).LiquidPhaseFraction;
        }

        private static DerivedThermodynamicState RecoverValidatedState(
            double specificEnthalpy,
            CompiledThermodynamicParameters material)
        {
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;

            double temperature;
            double liquidFraction;

            if (specificEnthalpy < hSolid)
            {
                temperature = material.MeltingTemperature
                    + (specificEnthalpy - hSolid) / material.SolidHeatCapacity;
                liquidFraction = 0.0;
            }
            else if (specificEnthalpy <= hLiquid)
            {
                temperature = material.MeltingTemperature;
                liquidFraction = (specificEnthalpy - hSolid) / material.LatentHeat;
            }
            else
            {
                temperature = material.MeltingTemperature
                    + (specificEnthalpy - hLiquid) / material.LiquidHeatCapacity;
                liquidFraction = 1.0;
            }

            return new DerivedThermodynamicState(temperature, liquidFraction);
        }

        private static void RequireMaterial(
            CompiledThermodynamicParameters material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }
        }
    }
}
