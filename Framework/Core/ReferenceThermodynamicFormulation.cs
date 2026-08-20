using System;
using System.Runtime.CompilerServices;
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
            var kernel = new RecoveryKernel(material);
            kernel.Recover(
                state.SpecificEnthalpy,
                out var temperature,
                out var liquidFraction);

            return new DerivedThermodynamicState(
                temperature,
                liquidFraction);
        }

        /// <summary>
        /// Recovers one transient Derived Thermodynamic State for each supplied
        /// persistent Thermodynamic State.
        ///
        /// The input states remain the owned runtime-state values. This batch
        /// operation does not mutate them and does not make the destination
        /// Derived State persistent. Material and destination-shape validation
        /// are performed at the batch boundary. A local recovery kernel caches
        /// immutable material parameters once for the batch. Per-value invariant
        /// enforcement is specialized by recovery region before the internal
        /// invariant-established Derived-State construction path is used.
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

            var kernel = new RecoveryKernel(material);

            for (var i = 0; i < states.Length; i++)
            {
                var region = kernel.Recover(
                    states[i].SpecificEnthalpy,
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EstablishBatchDerivedInvariants(
            RecoveryRegion region,
            double temperature,
            double liquidFraction)
        {
            if (region == RecoveryRegion.Latent)
            {
                DerivedThermodynamicState.RequireBoundedLiquidFraction(
                    liquidFraction);
                return;
            }

            DerivedThermodynamicState.RequireFiniteTemperature(temperature);
        }

        private static void RequireMaterial(
            CompiledThermodynamicParameters material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }
        }

        private readonly struct RecoveryKernel
        {
            private readonly double _solidTransitionEnthalpy;
            private readonly double _liquidTransitionEnthalpy;
            private readonly double _meltingTemperature;
            private readonly double _latentHeat;
            private readonly double _solidHeatCapacity;
            private readonly double _liquidHeatCapacity;

            public RecoveryKernel(CompiledThermodynamicParameters material)
            {
                _solidTransitionEnthalpy = material.SolidTransitionEnthalpy;
                _liquidTransitionEnthalpy = material.LiquidTransitionEnthalpy;
                _meltingTemperature = material.MeltingTemperature;
                _latentHeat = material.LatentHeat;
                _solidHeatCapacity = material.SolidHeatCapacity;
                _liquidHeatCapacity = material.LiquidHeatCapacity;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public RecoveryRegion Recover(
                double specificEnthalpy,
                out double temperature,
                out double liquidFraction)
            {
                if (specificEnthalpy < _solidTransitionEnthalpy)
                {
                    temperature = _meltingTemperature
                        + (specificEnthalpy - _solidTransitionEnthalpy)
                        / _solidHeatCapacity;
                    liquidFraction = 0.0;
                    return RecoveryRegion.SolidSensible;
                }

                if (specificEnthalpy <= _liquidTransitionEnthalpy)
                {
                    temperature = _meltingTemperature;
                    liquidFraction =
                        (specificEnthalpy - _solidTransitionEnthalpy)
                        / _latentHeat;
                    return RecoveryRegion.Latent;
                }

                temperature = _meltingTemperature
                    + (specificEnthalpy - _liquidTransitionEnthalpy)
                    / _liquidHeatCapacity;
                liquidFraction = 1.0;
                return RecoveryRegion.LiquidSensible;
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
