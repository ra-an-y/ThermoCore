using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Framework.Runtime
{
    /// <summary>
    /// Transient derived thermodynamic values recovered from Persistent State
    /// and computation-ready Configuration.
    ///
    /// This type does not make Temperature or liquid phase fraction Persistent
    /// State. Implementations may cache it without changing that semantic
    /// classification.
    /// </summary>
    public readonly struct DerivedThermodynamicState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DerivedThermodynamicState(double temperature, double liquidPhaseFraction)
        {
            RequireFiniteTemperature(temperature);
            RequireBoundedLiquidFraction(liquidPhaseFraction);

            Temperature = temperature;
            LiquidPhaseFraction = liquidPhaseFraction;
        }

        private DerivedThermodynamicState(
            double temperature,
            double liquidPhaseFraction,
            InvariantEstablishedMarker _)
        {
            Temperature = temperature;
            LiquidPhaseFraction = liquidPhaseFraction;
        }

        /// <summary>Recovered Temperature in K.</summary>
        public double Temperature { get; }

        /// <summary>Recovered liquid phase fraction in [0, 1].</summary>
        public double LiquidPhaseFraction { get; }

        /// <summary>
        /// Constructs Derived State only after the caller has established the
        /// same invariants enforced by the public constructor.
        ///
        /// This internal path exists so a batch recovery implementation can
        /// avoid repeating generic validation that is already guaranteed by its
        /// branch semantics and explicit specialized checks. It must never be
        /// used to weaken the finite-Temperature or [0,1] phase-fraction
        /// invariants.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static DerivedThermodynamicState FromEstablishedInvariants(
            double temperature,
            double liquidPhaseFraction)
        {
            return new DerivedThermodynamicState(
                temperature,
                liquidPhaseFraction,
                default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void RequireFiniteTemperature(double temperature)
        {
            if (!double.IsFinite(temperature))
            {
                ThrowNonFiniteTemperature();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void RequireBoundedLiquidFraction(double liquidPhaseFraction)
        {
            if (!double.IsFinite(liquidPhaseFraction)
                || liquidPhaseFraction < 0.0
                || liquidPhaseFraction > 1.0)
            {
                ThrowInvalidLiquidFraction();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowNonFiniteTemperature()
        {
            throw new ArgumentOutOfRangeException(
                "temperature",
                "Recovered temperature must be finite.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowInvalidLiquidFraction()
        {
            throw new ArgumentOutOfRangeException(
                "liquidPhaseFraction",
                "Liquid phase fraction must be finite and within [0, 1].");
        }

        private readonly struct InvariantEstablishedMarker
        {
        }
    }
}
