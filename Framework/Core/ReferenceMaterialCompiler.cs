using System;
using ThermoCore.Framework.Runtime;
using ThermoCore.Materials.Definitions;

namespace ThermoCore.Framework.Core
{
    /// <summary>
    /// Converts reusable Material Definition Configuration into the normalized,
    /// computation-ready Configuration consumed by the bounded reference core.
    /// </summary>
    public static class ReferenceMaterialCompiler
    {
        public static CompiledThermodynamicParameters Compile(
            ReferenceMaterialDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            // The current constant-Cp implementation profile places the zero
            // enthalpy datum on the solid sensible branch. This keeps h_s* fully
            // determined by T_E_ref and avoids the isothermal latent interval,
            // where Temperature alone would not identify one enthalpy value.
            if (definition.EnergyReferenceTemperature
                >= definition.MeltingTemperature)
            {
                throw new NotSupportedException(
                    "The current reference material compiler requires "
                    + "T_E_ref < T_m so the zero enthalpy datum lies on the "
                    + "solid sensible branch.");
            }

            var solidTransitionEnthalpy = RequireFiniteResult(
                definition.SolidHeatCapacity
                * (definition.MeltingTemperature
                    - definition.EnergyReferenceTemperature),
                "Solid-transition enthalpy normalization produced a non-finite value.");

            return new CompiledThermodynamicParameters(
                definition.ReferenceDensity,
                definition.DensityReferenceTemperature,
                definition.EnergyReferenceTemperature,
                definition.MeltingTemperature,
                definition.LatentHeat,
                definition.SolidHeatCapacity,
                definition.LiquidHeatCapacity,
                solidTransitionEnthalpy);
        }

        private static double RequireFiniteResult(double value, string message)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new OverflowException(message);
            }

            return value;
        }
    }
}
