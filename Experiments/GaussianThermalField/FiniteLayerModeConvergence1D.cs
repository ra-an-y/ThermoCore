using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct FiniteLayerModeConvergenceResult1D
    {
        public FiniteLayerModeConvergenceResult1D(
            double error4Modes,
            double error8Modes,
            double error16Modes,
            double error32Modes)
        {
            Error4Modes = error4Modes;
            Error8Modes = error8Modes;
            Error16Modes = error16Modes;
            Error32Modes = error32Modes;
        }

        public double Error4Modes { get; }

        public double Error8Modes { get; }

        public double Error16Modes { get; }

        public double Error32Modes { get; }

        public bool IsMonotonicallyConvergent =>
            Error8Modes < Error4Modes
            && Error16Modes < Error8Modes
            && Error32Modes < Error16Modes;

        public bool Satisfies(double maximum32ModeRelativeError)
        {
            if (double.IsNaN(maximum32ModeRelativeError)
                || double.IsInfinity(maximum32ModeRelativeError)
                || maximum32ModeRelativeError < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximum32ModeRelativeError));
            }

            return IsMonotonicallyConvergent
                && Error32Modes <= maximum32ModeRelativeError;
        }
    }

    /// <summary>
    /// Convergence checkpoint for the bounded finite-layer reduced state.
    /// A high-mode cosine solution is used only as an internal numerical
    /// reference for this experiment. The checkpoint tests whether increasing
    /// the retained current-state dimension systematically reduces field error.
    /// </summary>
    public static class FiniteLayerModeConvergence1D
    {
        public static FiniteLayerModeConvergenceResult1D Evaluate()
        {
            var material = new ThermalMaterial1D(
                thermalConductivity: 0.12,
                volumetricHeatCapacity: 1.4);

            const double layerLength = 0.35;
            const double leftInwardHeatFlux = 0.7;
            const double rightInwardHeatFlux = 0.0;
            const double elapsedTime = 0.2;
            const int referenceModeCount = 256;
            const int sampleCount = 101;

            var reference = EvolveZeroState(
                referenceModeCount,
                leftInwardHeatFlux,
                rightInwardHeatFlux,
                elapsedTime,
                layerLength,
                material);

            var error4 = RelativeFieldError(
                EvolveZeroState(4, leftInwardHeatFlux, rightInwardHeatFlux,
                    elapsedTime, layerLength, material),
                reference,
                layerLength,
                sampleCount);

            var error8 = RelativeFieldError(
                EvolveZeroState(8, leftInwardHeatFlux, rightInwardHeatFlux,
                    elapsedTime, layerLength, material),
                reference,
                layerLength,
                sampleCount);

            var error16 = RelativeFieldError(
                EvolveZeroState(16, leftInwardHeatFlux, rightInwardHeatFlux,
                    elapsedTime, layerLength, material),
                reference,
                layerLength,
                sampleCount);

            var error32 = RelativeFieldError(
                EvolveZeroState(32, leftInwardHeatFlux, rightInwardHeatFlux,
                    elapsedTime, layerLength, material),
                reference,
                layerLength,
                sampleCount);

            return new FiniteLayerModeConvergenceResult1D(
                error4,
                error8,
                error16,
                error32);
        }

        private static FiniteLayerReducedState1D EvolveZeroState(
            int modeCount,
            double leftInwardHeatFlux,
            double rightInwardHeatFlux,
            double elapsedTime,
            double layerLength,
            in ThermalMaterial1D material)
        {
            return FiniteLayerStateEvolution1D.Advance(
                FiniteLayerReducedState1D.Zero(modeCount),
                leftInwardHeatFlux,
                rightInwardHeatFlux,
                elapsedTime,
                layerLength,
                material);
        }

        private static double RelativeFieldError(
            in FiniteLayerReducedState1D candidate,
            in FiniteLayerReducedState1D reference,
            double layerLength,
            int sampleCount)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            for (var sample = 0; sample < sampleCount; sample++)
            {
                var x = layerLength * sample / (sampleCount - 1.0);
                var candidateValue = FiniteLayerFieldRepresentation1D.Evaluate(
                    candidate,
                    x,
                    layerLength);
                var referenceValue = FiniteLayerFieldRepresentation1D.Evaluate(
                    reference,
                    x,
                    layerLength);

                var difference = candidateValue - referenceValue;
                squaredError += difference * difference;
                squaredReference += referenceValue * referenceValue;
            }

            return Math.Sqrt(squaredError / squaredReference);
        }
    }
}
