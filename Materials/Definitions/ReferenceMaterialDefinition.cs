using System;

namespace ThermoCore.Materials.Definitions
{
    /// <summary>
    /// Reusable Material Definition for the bounded constant-heat-capacity
    /// reference implementation profile.
    ///
    /// This type is Configuration. It is not Thermodynamic State and does not
    /// own evolving per-cell runtime information.
    /// </summary>
    public sealed class ReferenceMaterialDefinition
    {
        public ReferenceMaterialDefinition(
            string materialId,
            string provenance,
            double referenceDensity,
            double densityReferenceTemperature,
            double energyReferenceTemperature,
            double meltingTemperature,
            double latentHeat,
            double solidHeatCapacity,
            double liquidHeatCapacity)
        {
            if (string.IsNullOrWhiteSpace(materialId))
            {
                throw new ArgumentException(
                    "Material identifier must be non-empty.",
                    nameof(materialId));
            }

            if (string.IsNullOrWhiteSpace(provenance))
            {
                throw new ArgumentException(
                    "Material provenance must be non-empty.",
                    nameof(provenance));
            }

            RequirePositiveFinite(referenceDensity, nameof(referenceDensity));
            RequireFinite(densityReferenceTemperature, nameof(densityReferenceTemperature));
            RequireFinite(energyReferenceTemperature, nameof(energyReferenceTemperature));
            RequireFinite(meltingTemperature, nameof(meltingTemperature));
            RequirePositiveFinite(latentHeat, nameof(latentHeat));
            RequirePositiveFinite(solidHeatCapacity, nameof(solidHeatCapacity));
            RequirePositiveFinite(liquidHeatCapacity, nameof(liquidHeatCapacity));

            MaterialId = materialId;
            Provenance = provenance;
            ReferenceDensity = referenceDensity;
            DensityReferenceTemperature = densityReferenceTemperature;
            EnergyReferenceTemperature = energyReferenceTemperature;
            MeltingTemperature = meltingTemperature;
            LatentHeat = latentHeat;
            SolidHeatCapacity = solidHeatCapacity;
            LiquidHeatCapacity = liquidHeatCapacity;
        }

        /// <summary>Stable material identifier.</summary>
        public string MaterialId { get; }

        /// <summary>
        /// Human-readable provenance for the material values and reference
        /// conditions used to construct this definition.
        /// </summary>
        public string Provenance { get; }

        /// <summary>Constant reference density rho_ref in kg/m^3.</summary>
        public double ReferenceDensity { get; }

        /// <summary>Density-reference temperature T_rho_ref in K.</summary>
        public double DensityReferenceTemperature { get; }

        /// <summary>
        /// Energy-reference temperature T_E_ref in K for the formulation datum
        /// h = 0 J/kg at T_E_ref.
        /// </summary>
        public double EnergyReferenceTemperature { get; }

        /// <summary>Isothermal solid/liquid transition temperature T_m in K.</summary>
        public double MeltingTemperature { get; }

        /// <summary>Latent heat L in J/kg.</summary>
        public double LatentHeat { get; }

        /// <summary>Constant solid sensible heat capacity in J/(kg*K).</summary>
        public double SolidHeatCapacity { get; }

        /// <summary>Constant liquid sensible heat capacity in J/(kg*K).</summary>
        public double LiquidHeatCapacity { get; }

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
                throw new ArgumentOutOfRangeException(
                    name,
                    "Value must be greater than zero.");
            }
        }
    }
}
