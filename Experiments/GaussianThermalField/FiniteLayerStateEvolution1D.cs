using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    /// <summary>
    /// Experiment-local reduced state evolution for a finite homogeneous layer
    /// under piecewise-constant inward heat fluxes at its left and right
    /// boundaries during one timestep.
    ///
    /// The representation uses the cosine eigenmodes of the one-dimensional
    /// diffusion operator. Retaining a finite number of modes produces a
    /// reduced model; the per-mode timestep update is analytic for constant
    /// boundary flux over the step.
    /// </summary>
    public static class FiniteLayerStateEvolution1D
    {
        public static FiniteLayerReducedState1D Advance(
            in FiniteLayerReducedState1D state,
            double leftInwardHeatFlux,
            double rightInwardHeatFlux,
            double deltaTime,
            double layerLength,
            in ThermalMaterial1D material)
        {
            ValidateFinite(leftInwardHeatFlux, nameof(leftInwardHeatFlux));
            ValidateFinite(rightInwardHeatFlux, nameof(rightInwardHeatFlux));

            if (double.IsNaN(deltaTime)
                || double.IsInfinity(deltaTime)
                || deltaTime < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deltaTime),
                    "Delta time must be finite and non-negative.");
            }

            if (double.IsNaN(layerLength)
                || double.IsInfinity(layerLength)
                || layerLength <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(layerLength),
                    "Layer length must be finite and positive.");
            }

            var volumetricHeatCapacity = material.VolumetricHeatCapacity;
            var thermalDiffusivity = material.ThermalDiffusivity;

            var meanRate =
                (leftInwardHeatFlux + rightInwardHeatFlux)
                / (volumetricHeatCapacity * layerLength);

            var newMean =
                state.MeanTemperaturePerturbation + meanRate * deltaTime;

            var newModes = new double[state.ModeCount];

            for (var modeIndex = 0; modeIndex < state.ModeCount; modeIndex++)
            {
                var n = modeIndex + 1;
                var waveNumber = n * Math.PI / layerLength;
                var decayRate = thermalDiffusivity * waveNumber * waveNumber;
                var rightParity = (n % 2 == 0) ? 1.0 : -1.0;

                var forcing =
                    2.0
                    * (leftInwardHeatFlux + rightParity * rightInwardHeatFlux)
                    / (volumetricHeatCapacity * layerLength);

                var previous = state.GetModeCoefficient(modeIndex);

                if (deltaTime == 0.0)
                {
                    newModes[modeIndex] = previous;
                    continue;
                }

                var decay = Math.Exp(-decayRate * deltaTime);
                var forcedContribution =
                    (forcing / decayRate) * (1.0 - decay);

                newModes[modeIndex] = previous * decay + forcedContribution;
            }

            return new FiniteLayerReducedState1D(newMean, newModes);
        }

        private static void ValidateFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }
}
