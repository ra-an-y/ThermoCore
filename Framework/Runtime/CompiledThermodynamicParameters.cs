using System;

namespace ThermoCore.Framework.Runtime
{
    /// <summary>
    /// Computation-ready Configuration for the bounded reference formulation.
    ///
    /// This type is not Thermodynamic State and is not Material Representation.
    /// It carries normalized material parameters required by Thermodynamic
    /// Computation after Material Definition has been converted into a
    /// computation-ready form.
    ///
    /// The initial reference implementation supports constant positive sensible
    /// heat capacities. This is a bounded implementation profile of the more
    /// general c_s(T) and c_l(T) relations permitted by the formulation.
    /// </summary>
    public readonly struct CompiledThermodynamicParameters
    {
        public CompiledThermodynamicParameters(
            double referenceDensity,
            double meltingTemperature,
            double latentHeat,
            double solidHeatCapacity,
            double liquidHeatCapacity,
            double solidTransitionEnthalpy)
        {
            RequirePositiveFinite(referenceDensity, nameof(referenceDensity));
            RequireFinite(meltingTemperature, nameof(meltingTemperature));
            RequirePositiveFinite(latentHeat, nameof(latentHeat));
            RequirePositiveFinite(solidHeatCapacity, nameof(solidHeatCapacity));
            RequirePositiveFinite(liquidHeatCapacity, nameof(liquidHeatCapacity));
            RequireFinite(solidTransitionEnthalpy, nameof(solidTransitionEnthalpy));

            var liquidTransitionEnthalpy = solidTransitionEnthalpy + latentHeat;
            RequireFinite(liquidTransitionEnthalpy, nameof(liquidTransitionEnthalpy));

            ReferenceDensity = referenceDensity;
            MeltingTemperature = meltingTemperature;
            LatentHeat = latentHeat;
            SolidHeatCapacity = solidHeatCapacity;
            LiquidHeatCapacity = liquidHeatCapacity;
            SolidTransitionEnthalpy = solidTransitionEnthalpy;
            LiquidTransitionEnthalpy = liquidTransitionEnthalpy;
        }

        /// <summary>Constant reference density rho_ref in kg/m^3.</summary>
        public double ReferenceDensity { get; }

        /// <summary>Isothermal phase-change temperature T_m in K.</summary>
        public double MeltingTemperature { get; }

        /// <summary>Latent heat L in J/kg.</summary>
        public double LatentHeat { get; }

        /// <summary>Constant solid sensible heat capacity in J/(kg*K).</summary>
        public double SolidHeatCapacity { get; }

        /// <summary>Constant liquid sensible heat capacity in J/(kg*K).</summary>
        public double LiquidHeatCapacity { get; }

        /// <summary>
        /// Normalized enthalpy threshold h_s* in J/kg at T_m.
        /// This is compiled Configuration derived from the common energy datum;
        /// it is not an independently persistent thermodynamic variable.
        /// </summary>
        public double SolidTransitionEnthalpy { get; }

        /// <summary>Fully liquid threshold h_l* = h_s* + L in J/kg.</summary>
        public double LiquidTransitionEnthalpy { get; }

        private static void RequireFinite(double value, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(name, "Value must be finite.");
            }
        }

        private static void RequirePositiveFinite(double value, string name)
        {
            RequireFinite(value, name);
            if (value <= 0.0)
            {
                throw new ArgumentOutOfRangeException(name, "Value must be greater than zero.");
            }
        }
    }
}
