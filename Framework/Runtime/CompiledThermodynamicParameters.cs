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
    /// heat capacities and places the zero-enthalpy datum on the solid sensible
    /// branch with T_E_ref < T_m.
    /// </summary>
    public sealed class CompiledThermodynamicParameters
    {
        public CompiledThermodynamicParameters(
            double referenceDensity,
            double densityReferenceTemperature,
            double energyReferenceTemperature,
            double meltingTemperature,
            double latentHeat,
            double solidHeatCapacity,
            double liquidHeatCapacity)
        {
            RequirePositiveFinite(referenceDensity, nameof(referenceDensity));
            RequireFinite(
                densityReferenceTemperature,
                nameof(densityReferenceTemperature));
            RequireFinite(
                energyReferenceTemperature,
                nameof(energyReferenceTemperature));
            RequireFinite(meltingTemperature, nameof(meltingTemperature));
            RequirePositiveFinite(latentHeat, nameof(latentHeat));
            RequirePositiveFinite(solidHeatCapacity, nameof(solidHeatCapacity));
            RequirePositiveFinite(liquidHeatCapacity, nameof(liquidHeatCapacity));

            if (energyReferenceTemperature >= meltingTemperature)
            {
                throw new NotSupportedException(
                    "The current bounded implementation requires T_E_ref < T_m "
                    + "so the zero enthalpy datum lies on the solid sensible branch.");
            }

            var solidTransitionEnthalpy = RequireFiniteResult(
                solidHeatCapacity
                * (meltingTemperature - energyReferenceTemperature),
                "Solid-transition enthalpy normalization produced a non-finite value.");

            var liquidTransitionEnthalpy = RequireFiniteResult(
                solidTransitionEnthalpy + latentHeat,
                "Liquid-transition enthalpy normalization produced a non-finite value.");

            ReferenceDensity = referenceDensity;
            DensityReferenceTemperature = densityReferenceTemperature;
            EnergyReferenceTemperature = energyReferenceTemperature;
            MeltingTemperature = meltingTemperature;
            LatentHeat = latentHeat;
            SolidHeatCapacity = solidHeatCapacity;
            LiquidHeatCapacity = liquidHeatCapacity;
            SolidTransitionEnthalpy = solidTransitionEnthalpy;
            LiquidTransitionEnthalpy = liquidTransitionEnthalpy;
        }

        /// <summary>Constant reference density rho_ref in kg/m^3.</summary>
        public double ReferenceDensity { get; }

        /// <summary>Density-reference temperature T_rho_ref in K.</summary>
        public double DensityReferenceTemperature { get; }

        /// <summary>Energy-reference temperature T_E_ref in K.</summary>
        public double EnergyReferenceTemperature { get; }

        /// <summary>Isothermal phase-change temperature T_m in K.</summary>
        public double MeltingTemperature { get; }

        /// <summary>Latent heat L in J/kg.</summary>
        public double LatentHeat { get; }

        /// <summary>Constant solid sensible heat capacity in J/(kg*K).</summary>
        public double SolidHeatCapacity { get; }

        /// <summary>Constant liquid sensible heat capacity in J/(kg*K).</summary>
        public double LiquidHeatCapacity { get; }

        /// <summary>
        /// Normalized enthalpy threshold h_s* in J/kg at T_m under the datum
        /// h = 0 J/kg at T_E_ref.
        /// </summary>
        public double SolidTransitionEnthalpy { get; }

        /// <summary>Fully liquid threshold h_l* = h_s* + L in J/kg.</summary>
        public double LiquidTransitionEnthalpy { get; }

        private static double RequireFiniteResult(double value, string message)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new OverflowException(message);
            }

            return value;
        }

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
