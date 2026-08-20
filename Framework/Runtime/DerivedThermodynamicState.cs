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
            if (!double.IsFinite(temperature))
            {
                ThrowNonFiniteTemperature();
            }

            if (!double.IsFinite(liquidPhaseFraction)
                || liquidPhaseFraction < 0.0
                || liquidPhaseFraction > 1.0)
            {
                ThrowInvalidLiquidFraction();
            }

            Temperature = temperature;
            LiquidPhaseFraction = liquidPhaseFraction;
        }

        /// <summary>Recovered Temperature in K.</summary>
        public double Temperature { get; }

        /// <summary>Recovered liquid phase fraction in [0, 1].</summary>
        public double LiquidPhaseFraction { get; }

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
    }
}
