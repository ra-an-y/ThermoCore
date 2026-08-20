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
        /// Derived State persistent. Material and destination-shape validation
        /// are performed at the batch boundary. Per-value invariant enforcement
        /// is specialized by recovery region before the internal trusted
        /// Derived-State construction path is used.
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
                var region = RecoverRaw(
                    states[i].SpecificEnthalpy,
                    material,
                    out var temperature,
                    out var liquidFraction);

                EstablishBatchDerivedInvariants(
                    region,
                    temperature,
                    liquidFraction);

                destination[i] = DerivedThermodynamicState.FromEstablishedInvariants(
                    temperature,
                    liquidFraction);
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
            RecoverRaw(
                specificEnthalpy,
                material,
                out var temperature,
                out var liquidFraction);

            return new DerivedThermodynamicState(temperature, liquidFraction);
        }

        private static RecoveryRegion RecoverRaw(
            double specificEnthalpy,
            CompiledThermodynamicParameters material,
            out double temperature,
            out double liquidFraction)
        {
            var hSolid = material.SolidTransitionEnthalpy;
            var hLiquid = material.LiquidTransitionEnthalpy;

            if (specificEnthalpy < hSolid)
            {
                temperature = material.MeltingTemperature
                    + (specificEnthalpy - hSolid) / material.SolidHeatCapacity;
                liquidFraction = 0.0;
                return RecoveryRegion.SolidSensible;
            }

            if (specificEnthalpy <= hLiquid)
            {
                temperature = material.MeltingTemperature;
                liquidFraction = (specificEnthalpy - hSolid) / material.LatentHeat;
                return RecoveryRegion.Latent;
            }

            temperature = material.MeltingTemperature
                + (specificEnthalpy - hLiquid) / material.LiquidHeatCapacity;
            liquidFraction = 1.0;
            return RecoveryRegion.LiquidSensible;
        }

        private static void EstablishBatchDerivedInvariants(
            RecoveryRegion region,
            double temperature,
            double liquidFraction)
        {
            switch (region)
            {
                case RecoveryRegion.SolidSensible:
                case RecoveryRegion.LiquidSensible:
                    // These branches assign phase fractions exactly to 0 or 1.
                    // Only finite recovered Temperature remains to be established.
                    DerivedThermodynamicState.RequireFiniteTemperature(temperature);
                    return;

                case RecoveryRegion.Latent:
                    // The latent branch assigns Temperature directly from the
                    // already-finite material melting Temperature. Floating-point
                    // endpoint arithmetic can still perturb the phase fraction,
                    // so its finite [0,1] invariant remains explicitly checked.
                    DerivedThermodynamicState.RequireBoundedLiquidFraction(
                        liquidFraction);
                    return;

                default:
                    throw new InvalidOperationException(
                        "Unknown thermodynamic recovery region.");
            }
        }

        private static void RequireMaterial(
            CompiledThermodynamicParameters material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }
        }

        private enum RecoveryRegion
        {
            SolidSensible,
            Latent,
            LiquidSensible
        }
    }
}
