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

            return new CompiledThermodynamicParameters(
                definition.ReferenceDensity,
                definition.DensityReferenceTemperature,
                definition.EnergyReferenceTemperature,
                definition.MeltingTemperature,
                definition.LatentHeat,
                definition.SolidHeatCapacity,
                definition.LiquidHeatCapacity);
        }
    }
}
