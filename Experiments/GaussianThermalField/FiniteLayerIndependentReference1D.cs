using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct FiniteLayerIndependentReferenceResult1D
    {
        public FiniteLayerIndependentReferenceResult1D(
            double constantFluxRelativeError32Modes,
            double pulseHistoryRelativeError4Modes,
            double constantFluxEnergyError,
            double pulseEnergyError)
        {
            ConstantFluxRelativeError32Modes = constantFluxRelativeError32Modes;
            PulseHistoryRelativeError4Modes = pulseHistoryRelativeError4Modes;
            ConstantFluxEnergyError = constantFluxEnergyError;
            PulseEnergyError = pulseEnergyError;
        }

        public double ConstantFluxRelativeError32Modes { get; }

        public double PulseHistoryRelativeError4Modes { get; }

        public double ConstantFluxEnergyError { get; }

        public double PulseEnergyError { get; }

        public bool Satisfies(
            double maximumConstantFluxRelativeError,
            double maximumPulseRelativeError,
            double maximumEnergyError)
        {
            return ConstantFluxRelativeError32Modes <= maximumConstantFluxRelativeError
                && PulseHistoryRelativeError4Modes <= maximumPulseRelativeError
                && Math.Abs(ConstantFluxEnergyError) <= maximumEnergyError
                && Math.Abs(PulseEnergyError) <= maximumEnergyError;
        }
    }

    /// <summary>
    /// Independent numerical checkpoint for the finite-layer reduced state.
    ///
    /// The reference path is a cell-centered finite-volume discretization with
    /// explicit time integration. It does not use the cosine modal evolution
    /// equations, so agreement is not a same-formulation self-comparison.
    /// </summary>
    public static class FiniteLayerIndependentReference1D
    {
        public static FiniteLayerIndependentReferenceResult1D Evaluate()
        {
            var material = new ThermalMaterial1D(
                thermalConductivity: 0.12,
                volumetricHeatCapacity: 1.4);

            const double layerLength = 0.35;
            const int cellCount = 200;

            var constantReference = CreateZeroReference(cellCount);
            AdvanceFiniteVolume(
                constantReference,
                leftInwardHeatFlux: 0.7,
                rightInwardHeatFlux: 0.0,
                duration: 0.2,
                layerLength,
                material);

            var constantReduced = FiniteLayerStateEvolution1D.Advance(
                FiniteLayerReducedState1D.Zero(modeCount: 32),
                leftInwardHeatFlux: 0.7,
                rightInwardHeatFlux: 0.0,
                deltaTime: 0.2,
                layerLength,
                material);

            var constantError = RelativeFieldError(
                constantReduced,
                constantReference,
                layerLength);

            var cellWidth = layerLength / cellCount;
            var constantReferenceEnergy =
                material.VolumetricHeatCapacity
                * cellWidth
                * Sum(constantReference);
            var constantExpectedEnergy = 0.7 * 0.2;
            var constantEnergyError =
                constantReferenceEnergy - constantExpectedEnergy;

            var pulseReference = CreateZeroReference(cellCount);
            AdvanceFiniteVolume(
                pulseReference,
                leftInwardHeatFlux: 0.7,
                rightInwardHeatFlux: 0.0,
                duration: 0.1,
                layerLength,
                material);
            AdvanceFiniteVolume(
                pulseReference,
                leftInwardHeatFlux: 0.0,
                rightInwardHeatFlux: 0.0,
                duration: 0.3,
                layerLength,
                material);

            var pulseReduced = FiniteLayerStateEvolution1D.Advance(
                FiniteLayerReducedState1D.Zero(modeCount: 4),
                leftInwardHeatFlux: 0.7,
                rightInwardHeatFlux: 0.0,
                deltaTime: 0.1,
                layerLength,
                material);
            pulseReduced = FiniteLayerStateEvolution1D.Advance(
                pulseReduced,
                leftInwardHeatFlux: 0.0,
                rightInwardHeatFlux: 0.0,
                deltaTime: 0.3,
                layerLength,
                material);

            var pulseError = RelativeFieldError(
                pulseReduced,
                pulseReference,
                layerLength);

            var pulseReferenceEnergy =
                material.VolumetricHeatCapacity
                * cellWidth
                * Sum(pulseReference);
            var pulseExpectedEnergy = 0.7 * 0.1;
            var pulseEnergyError = pulseReferenceEnergy - pulseExpectedEnergy;

            return new FiniteLayerIndependentReferenceResult1D(
                constantError,
                pulseError,
                constantEnergyError,
                pulseEnergyError);
        }

        private static double[] CreateZeroReference(int cellCount)
        {
            return new double[cellCount];
        }

        private static void AdvanceFiniteVolume(
            double[] temperature,
            double leftInwardHeatFlux,
            double rightInwardHeatFlux,
            double duration,
            double layerLength,
            in ThermalMaterial1D material)
        {
            if (duration == 0.0)
            {
                return;
            }

            var cellCount = temperature.Length;
            var cellWidth = layerLength / cellCount;
            var diffusivity = material.ThermalDiffusivity;

            const double stabilityFactor = 0.40;
            var maximumStep =
                stabilityFactor * cellWidth * cellWidth / diffusivity;
            var stepCount = Math.Max(1, (int)Math.Ceiling(duration / maximumStep));
            var deltaTime = duration / stepCount;
            var diffusionRatio =
                diffusivity * deltaTime / (cellWidth * cellWidth);
            var sourceScale =
                deltaTime / (material.VolumetricHeatCapacity * cellWidth);

            var next = new double[cellCount];

            for (var step = 0; step < stepCount; step++)
            {
                next[0] = temperature[0]
                    + diffusionRatio * (temperature[1] - temperature[0])
                    + sourceScale * leftInwardHeatFlux;

                for (var cell = 1; cell < cellCount - 1; cell++)
                {
                    next[cell] = temperature[cell]
                        + diffusionRatio
                        * (temperature[cell + 1]
                            - 2.0 * temperature[cell]
                            + temperature[cell - 1]);
                }

                next[cellCount - 1] = temperature[cellCount - 1]
                    + diffusionRatio
                    * (temperature[cellCount - 2] - temperature[cellCount - 1])
                    + sourceScale * rightInwardHeatFlux;

                Array.Copy(next, temperature, cellCount);
            }
        }

        private static double RelativeFieldError(
            in FiniteLayerReducedState1D reduced,
            double[] reference,
            double layerLength)
        {
            var cellWidth = layerLength / reference.Length;
            var squaredError = 0.0;
            var squaredReference = 0.0;

            for (var cell = 0; cell < reference.Length; cell++)
            {
                var x = (cell + 0.5) * cellWidth;
                var candidate = FiniteLayerFieldRepresentation1D.Evaluate(
                    reduced,
                    x,
                    layerLength);
                var difference = candidate - reference[cell];

                squaredError += difference * difference;
                squaredReference += reference[cell] * reference[cell];
            }

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static double Sum(double[] values)
        {
            var total = 0.0;
            for (var i = 0; i < values.Length; i++)
            {
                total += values[i];
            }

            return total;
        }
    }
}
