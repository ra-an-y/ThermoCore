using System;

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
        public DerivedThermodynamicState(double temperature, double liquidPhaseFraction)
        {
            if (double.IsNaN(temperature) || double.IsInfinity(temperature))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(temperature),
                    "Recovered temperature must be finite.");
            }

            if (double.IsNaN(liquidPhaseFraction)
                || double.IsInfinity(liquidPhaseFraction)
                || liquidPhaseFraction < 0.0
                || liquidPhaseFraction > 1.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(liquidPhaseFraction),
                    "Liquid phase fraction must be finite and within [0, 1].");
            }

            Temperature = temperature;
            LiquidPhaseFraction = liquidPhaseFraction;
        }

        /// <summary>Recovered Temperature in K.</summary>
        public double Temperature { get; }

        /// <summary>Recovered liquid phase fraction in [0, 1].</summary>
        public double LiquidPhaseFraction { get; }
    }
}
