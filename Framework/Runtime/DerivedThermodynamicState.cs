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
            Temperature = temperature;
            LiquidPhaseFraction = liquidPhaseFraction;
        }

        /// <summary>Recovered Temperature in K.</summary>
        public double Temperature { get; }

        /// <summary>Recovered liquid phase fraction in [0, 1].</summary>
        public double LiquidPhaseFraction { get; }
    }
}
