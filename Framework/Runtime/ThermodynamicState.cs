using System;

namespace ThermoCore.Framework.Runtime
{
    /// <summary>
    /// Persistent Thermodynamic State for the bounded reference formulation.
    ///
    /// This implementation stores only specific enthalpy. Temperature and
    /// liquid phase fraction remain derived quantities.
    /// </summary>
    public readonly struct ThermodynamicState
    {
        public ThermodynamicState(double specificEnthalpy)
        {
            if (double.IsNaN(specificEnthalpy) || double.IsInfinity(specificEnthalpy))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(specificEnthalpy),
                    "Specific enthalpy must be finite.");
            }

            SpecificEnthalpy = specificEnthalpy;
        }

        /// <summary>
        /// Persistent specific enthalpy h in J/kg.
        /// </summary>
        public double SpecificEnthalpy { get; }
    }
}
