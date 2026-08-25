using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    /// <summary>
    /// Reconstructs the current finite-layer scalar field from the reduced
    /// state. This is an experiment-local field representation and carries no
    /// ThermoCore state authority.
    /// </summary>
    public static class FiniteLayerFieldRepresentation1D
    {
        public static double Evaluate(
            in FiniteLayerReducedState1D state,
            double localPosition,
            double layerLength)
        {
            if (double.IsNaN(localPosition) || double.IsInfinity(localPosition))
            {
                throw new ArgumentOutOfRangeException(nameof(localPosition));
            }

            if (double.IsNaN(layerLength)
                || double.IsInfinity(layerLength)
                || layerLength <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(layerLength),
                    "Layer length must be finite and positive.");
            }

            if (localPosition < 0.0 || localPosition > layerLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(localPosition),
                    "Local position must lie inside the finite layer.");
            }

            var value = state.MeanTemperaturePerturbation;

            for (var modeIndex = 0; modeIndex < state.ModeCount; modeIndex++)
            {
                var n = modeIndex + 1;
                value += state.GetModeCoefficient(modeIndex)
                    * Math.Cos(n * Math.PI * localPosition / layerLength);
            }

            return value;
        }
    }
}
