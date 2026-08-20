using System;
using ThermoCore.Framework.Runtime;

namespace ThermoCore.Framework.Core
{
    /// <summary>
    /// Formulation-level dimensional mappings from signed physical energy input
    /// quantities into specific-enthalpy increments.
    ///
    /// Positive mapped energy adds enthalpy and negative mapped energy removes
    /// enthalpy. Distribution across multiple cells remains an upstream caller
    /// responsibility and must be explicit.
    /// </summary>
    public static class EnergyInputMapping
    {
        public static double FromCellEnergy(double deltaEnergy, double cellMass)
        {
            RequireFinite(deltaEnergy, nameof(deltaEnergy));
            RequirePositiveFinite(cellMass, nameof(cellMass));
            return RequireFiniteResult(deltaEnergy / cellMass);
        }

        public static double FromPower(double power, double deltaTime, double cellMass)
        {
            RequireFinite(power, nameof(power));
            RequireNonNegativeFinite(deltaTime, nameof(deltaTime));
            RequirePositiveFinite(cellMass, nameof(cellMass));
            return RequireFiniteResult(power * deltaTime / cellMass);
        }

        public static double FromBoundaryHeatFlux(
            double heatFlux,
            double affectedArea,
            double deltaTime,
            double cellMass)
        {
            RequireFinite(heatFlux, nameof(heatFlux));
            RequireNonNegativeFinite(affectedArea, nameof(affectedArea));
            RequireNonNegativeFinite(deltaTime, nameof(deltaTime));
            RequirePositiveFinite(cellMass, nameof(cellMass));
            return RequireFiniteResult(
                heatFlux * affectedArea * deltaTime / cellMass);
        }

        public static double FromVolumetricHeatSource(
            double volumetricHeatSource,
            double deltaTime,
            CompiledThermodynamicParameters material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            RequireFinite(volumetricHeatSource, nameof(volumetricHeatSource));
            RequireNonNegativeFinite(deltaTime, nameof(deltaTime));
            return RequireFiniteResult(
                volumetricHeatSource * deltaTime / material.ReferenceDensity);
        }

        public static double CellMass(
            double cellVolume,
            CompiledThermodynamicParameters material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            RequirePositiveFinite(cellVolume, nameof(cellVolume));
            return RequireFiniteResult(material.ReferenceDensity * cellVolume);
        }

        private static double RequireFiniteResult(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new OverflowException(
                    "Energy mapping produced a non-finite result.");
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

        private static void RequireNonNegativeFinite(double value, string name)
        {
            RequireFinite(value, name);
            if (value < 0.0)
            {
                throw new ArgumentOutOfRangeException(name, "Value must be non-negative.");
            }
        }
    }
}
