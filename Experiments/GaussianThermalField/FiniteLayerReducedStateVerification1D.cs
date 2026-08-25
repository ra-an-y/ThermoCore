using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct FiniteLayerReducedStateVerificationResult1D
    {
        public FiniteLayerReducedStateVerificationResult1D(
            double energyBalanceError,
            double maximumNoFluxDecayError,
            double maximumSymmetricOddModeMagnitude,
            double symmetricFieldMirrorError)
        {
            EnergyBalanceError = energyBalanceError;
            MaximumNoFluxDecayError = maximumNoFluxDecayError;
            MaximumSymmetricOddModeMagnitude = maximumSymmetricOddModeMagnitude;
            SymmetricFieldMirrorError = symmetricFieldMirrorError;
        }

        public double EnergyBalanceError { get; }

        public double MaximumNoFluxDecayError { get; }

        public double MaximumSymmetricOddModeMagnitude { get; }

        public double SymmetricFieldMirrorError { get; }

        public bool Satisfies(double tolerance)
        {
            if (double.IsNaN(tolerance)
                || double.IsInfinity(tolerance)
                || tolerance < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            }

            return Math.Abs(EnergyBalanceError) <= tolerance
                && MaximumNoFluxDecayError <= tolerance
                && MaximumSymmetricOddModeMagnitude <= tolerance
                && SymmetricFieldMirrorError <= tolerance;
        }
    }

    /// <summary>
    /// Mathematical checkpoint for the experiment-local finite-layer reduced
    /// state. The checks exercise conservation accounting, autonomous modal
    /// decay, and the expected symmetry under equal inward boundary fluxes.
    /// </summary>
    public static class FiniteLayerReducedStateVerification1D
    {
        public static FiniteLayerReducedStateVerificationResult1D Evaluate()
        {
            var material = new ThermalMaterial1D(
                thermalConductivity: 0.12,
                volumetricHeatCapacity: 1.4);

            const double layerLength = 0.35;
            const double deltaTime = 0.2;

            var initial = new FiniteLayerReducedState1D(
                meanTemperaturePerturbation: 0.25,
                modeCoefficients: new[] { 0.40, -0.20, 0.10, -0.05 });

            var noFlux = FiniteLayerStateEvolution1D.Advance(
                initial,
                leftInwardHeatFlux: 0.0,
                rightInwardHeatFlux: 0.0,
                deltaTime,
                layerLength,
                material);

            var maximumDecayError = 0.0;
            for (var modeIndex = 0; modeIndex < initial.ModeCount; modeIndex++)
            {
                var n = modeIndex + 1;
                var waveNumber = n * Math.PI / layerLength;
                var decayRate =
                    material.ThermalDiffusivity * waveNumber * waveNumber;
                var expected = initial.GetModeCoefficient(modeIndex)
                    * Math.Exp(-decayRate * deltaTime);

                maximumDecayError = Math.Max(
                    maximumDecayError,
                    Math.Abs(noFlux.GetModeCoefficient(modeIndex) - expected));
            }

            const double symmetricFlux = 0.7;
            var symmetric = FiniteLayerStateEvolution1D.Advance(
                FiniteLayerReducedState1D.Zero(modeCount: 4),
                leftInwardHeatFlux: symmetricFlux,
                rightInwardHeatFlux: symmetricFlux,
                deltaTime,
                layerLength,
                material);

            var expectedEnergyChangePerArea =
                2.0 * symmetricFlux * deltaTime;
            var representedEnergyChangePerArea =
                material.VolumetricHeatCapacity
                * layerLength
                * symmetric.MeanTemperaturePerturbation;
            var energyBalanceError =
                representedEnergyChangePerArea - expectedEnergyChangePerArea;

            var maximumOddModeMagnitude = Math.Max(
                Math.Abs(symmetric.GetModeCoefficient(0)),
                Math.Abs(symmetric.GetModeCoefficient(2)));

            var leftProbe = FiniteLayerFieldRepresentation1D.Evaluate(
                symmetric,
                localPosition: 0.20 * layerLength,
                layerLength);
            var rightProbe = FiniteLayerFieldRepresentation1D.Evaluate(
                symmetric,
                localPosition: 0.80 * layerLength,
                layerLength);
            var mirrorError = Math.Abs(leftProbe - rightProbe);

            return new FiniteLayerReducedStateVerificationResult1D(
                energyBalanceError,
                maximumDecayError,
                maximumOddModeMagnitude,
                mirrorError);
        }
    }
}
