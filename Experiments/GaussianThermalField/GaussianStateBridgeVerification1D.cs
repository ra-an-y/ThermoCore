using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct GaussianStateBridgeVerificationResult1D
    {
        public GaussianStateBridgeVerificationResult1D(
            double initialProjectionRelativeError,
            double recoveredVsStateRelativeError,
            double recoveredVsFiniteVolumeRelativeError,
            double recoveredEnergyError,
            double maximumInterfaceJump,
            int recoveredGaussianTerms)
        {
            InitialProjectionRelativeError = initialProjectionRelativeError;
            RecoveredVsStateRelativeError = recoveredVsStateRelativeError;
            RecoveredVsFiniteVolumeRelativeError = recoveredVsFiniteVolumeRelativeError;
            RecoveredEnergyError = recoveredEnergyError;
            MaximumInterfaceJump = maximumInterfaceJump;
            RecoveredGaussianTerms = recoveredGaussianTerms;
        }

        public double InitialProjectionRelativeError { get; }
        public double RecoveredVsStateRelativeError { get; }
        public double RecoveredVsFiniteVolumeRelativeError { get; }
        public double RecoveredEnergyError { get; }
        public double MaximumInterfaceJump { get; }
        public int RecoveredGaussianTerms { get; }

        public bool Satisfies(
            double maximumProjectionError,
            double maximumRecoveryError,
            double maximumFiniteVolumeError,
            double maximumEnergyError,
            double maximumInterfaceJump,
            int expectedGaussianTerms)
        {
            return InitialProjectionRelativeError <= maximumProjectionError
                && RecoveredVsStateRelativeError <= maximumRecoveryError
                && RecoveredVsFiniteVolumeRelativeError <= maximumFiniteVolumeError
                && Math.Abs(RecoveredEnergyError) <= maximumEnergyError
                && MaximumInterfaceJump <= maximumInterfaceJump
                && RecoveredGaussianTerms == expectedGaussianTerms;
        }
    }

    /// <summary>
    /// Checkpoint 6: Gaussian -> reduced current state -> state-driven A-B-C
    /// evolution -> fixed-size Gaussian-compatible field recovery.
    ///
    /// The independent comparison path is the heterogeneous finite-volume
    /// reference. Gaussian recovery remains downstream and never writes the
    /// reduced current state.
    /// </summary>
    public static class GaussianStateBridgeVerification1D
    {
        public static GaussianStateBridgeVerificationResult1D Evaluate()
        {
            var materialA = new ThermalMaterial1D(0.40, 2.0);
            var materialB = new ThermalMaterial1D(0.06, 1.2);
            var materialC = new ThermalMaterial1D(0.20, 1.6);

            const double lengthA = 0.60;
            const double lengthB = 0.35;
            const double lengthC = 0.60;
            const int modeCount = 32;
            const double reducedDeltaTime = 0.002;
            const double duration = 0.60;
            const int fittedKernelCountPerRegion = 8;
            const double recoveryWidthFactor = 1.75;

            const double gaussianMean = 0.46;
            const double gaussianStandardDeviation = 0.05;
            var gaussianVariance =
                gaussianStandardDeviation * gaussianStandardDeviation;
            var peakOneAmplitude =
                Math.Sqrt(2.0 * Math.PI * gaussianVariance);
            var initialGaussian = new GaussianKernel1D(
                gaussianMean,
                gaussianVariance,
                peakOneAmplitude);

            var projectedA = GaussianStateBridge1D.ProjectGaussianToState(
                initialGaussian,
                lengthA,
                modeCount);

            var initialProjectionError = RelativeProjectionError(
                initialGaussian,
                projectedA,
                lengthA);

            var state = new ThreeLayerCoupledState1D(
                projectedA,
                FiniteLayerReducedState1D.Zero(modeCount),
                FiniteLayerReducedState1D.Zero(modeCount));

            var maximumJump = 0.0;
            var stepCount = (int)Math.Round(duration / reducedDeltaTime);
            for (var step = 0; step < stepCount; step++)
            {
                var result = ThreeLayerCoupledEvolution1D.Advance(
                    state,
                    reducedDeltaTime,
                    lengthA,
                    lengthB,
                    lengthC,
                    materialA,
                    materialB,
                    materialC);

                state = result.State;
                maximumJump = Math.Max(
                    maximumJump,
                    Math.Max(
                        Math.Abs(result.InterfaceJumpAB),
                        Math.Abs(result.InterfaceJumpBC)));
            }

            var recoveredA = GaussianStateBridge1D.RecoverGaussianMixture(
                state.StateA,
                lengthA,
                fittedKernelCountPerRegion,
                recoveryWidthFactor);
            var recoveredB = GaussianStateBridge1D.RecoverGaussianMixture(
                state.StateB,
                lengthB,
                fittedKernelCountPerRegion,
                recoveryWidthFactor);
            var recoveredC = GaussianStateBridge1D.RecoverGaussianMixture(
                state.StateC,
                lengthC,
                fittedKernelCountPerRegion,
                recoveryWidthFactor);

            const double cellWidth = 0.005;
            var cellCountA = (int)Math.Round(lengthA / cellWidth);
            var cellCountB = (int)Math.Round(lengthB / cellWidth);
            var cellCountC = (int)Math.Round(lengthC / cellWidth);

            var recoveryError = RelativeRecoveredVsStateError(
                state,
                recoveredA,
                recoveredB,
                recoveredC,
                cellWidth,
                cellCountA,
                cellCountB,
                cellCountC,
                lengthA,
                lengthB,
                lengthC);

            var reference = CreateAndAdvanceFiniteVolumeReference(
                initialGaussian,
                materialA,
                materialB,
                materialC,
                cellWidth,
                cellCountA,
                cellCountB,
                cellCountC,
                duration);

            var recoveredVsReferenceError = RelativeRecoveredVsReferenceError(
                recoveredA,
                recoveredB,
                recoveredC,
                reference,
                cellWidth,
                cellCountA,
                cellCountB,
                cellCountC);

            var recoveredEnergy =
                materialA.VolumetricHeatCapacity
                    * GaussianStateBridge1D.IntegrateMixture(recoveredA, lengthA)
                + materialB.VolumetricHeatCapacity
                    * GaussianStateBridge1D.IntegrateMixture(recoveredB, lengthB)
                + materialC.VolumetricHeatCapacity
                    * GaussianStateBridge1D.IntegrateMixture(recoveredC, lengthC);

            var stateEnergy =
                materialA.VolumetricHeatCapacity * lengthA
                    * state.StateA.MeanTemperaturePerturbation
                + materialB.VolumetricHeatCapacity * lengthB
                    * state.StateB.MeanTemperaturePerturbation
                + materialC.VolumetricHeatCapacity * lengthC
                    * state.StateC.MeanTemperaturePerturbation;

            var recoveredTerms = recoveredA.Count + recoveredB.Count + recoveredC.Count;

            return new GaussianStateBridgeVerificationResult1D(
                initialProjectionError,
                recoveryError,
                recoveredVsReferenceError,
                recoveredEnergy - stateEnergy,
                maximumJump,
                recoveredTerms);
        }

        private static double RelativeProjectionError(
            in GaussianKernel1D gaussian,
            in FiniteLayerReducedState1D state,
            double layerLength)
        {
            const int sampleCount = 1001;
            var squaredError = 0.0;
            var squaredReference = 0.0;

            for (var sample = 0; sample < sampleCount; sample++)
            {
                var x = layerLength * sample / (sampleCount - 1.0);
                var reference = gaussian.Evaluate(x);
                var candidate = FiniteLayerFieldRepresentation1D.Evaluate(
                    state, x, layerLength);
                var difference = candidate - reference;

                squaredError += difference * difference;
                squaredReference += reference * reference;
            }

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static double RelativeRecoveredVsStateError(
            in ThreeLayerCoupledState1D state,
            in GaussianMixture1D recoveredA,
            in GaussianMixture1D recoveredB,
            in GaussianMixture1D recoveredC,
            double cellWidth,
            int cellCountA,
            int cellCountB,
            int cellCountC,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            AccumulateRegionError(
                state.StateA, recoveredA, cellWidth, cellCountA, lengthA,
                ref squaredError, ref squaredReference);
            AccumulateRegionError(
                state.StateB, recoveredB, cellWidth, cellCountB, lengthB,
                ref squaredError, ref squaredReference);
            AccumulateRegionError(
                state.StateC, recoveredC, cellWidth, cellCountC, lengthC,
                ref squaredError, ref squaredReference);

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static void AccumulateRegionError(
            in FiniteLayerReducedState1D state,
            in GaussianMixture1D recovered,
            double cellWidth,
            int cellCount,
            double layerLength,
            ref double squaredError,
            ref double squaredReference)
        {
            for (var cell = 0; cell < cellCount; cell++)
            {
                var x = (cell + 0.5) * cellWidth;
                var reference = FiniteLayerFieldRepresentation1D.Evaluate(
                    state, x, layerLength);
                var candidate = recovered.Evaluate(x);
                var difference = candidate - reference;

                squaredError += difference * difference;
                squaredReference += reference * reference;
            }
        }

        private static double[] CreateAndAdvanceFiniteVolumeReference(
            in GaussianKernel1D initialGaussian,
            in ThermalMaterial1D materialA,
            in ThermalMaterial1D materialB,
            in ThermalMaterial1D materialC,
            double cellWidth,
            int cellCountA,
            int cellCountB,
            int cellCountC,
            double duration)
        {
            var totalCellCount = cellCountA + cellCountB + cellCountC;
            var temperature = new double[totalCellCount];
            var conductivity = new double[totalCellCount];
            var heatCapacity = new double[totalCellCount];

            for (var cell = 0; cell < totalCellCount; cell++)
            {
                if (cell < cellCountA)
                {
                    var x = (cell + 0.5) * cellWidth;
                    temperature[cell] = initialGaussian.Evaluate(x);
                    conductivity[cell] = materialA.ThermalConductivity;
                    heatCapacity[cell] = materialA.VolumetricHeatCapacity;
                }
                else if (cell < cellCountA + cellCountB)
                {
                    conductivity[cell] = materialB.ThermalConductivity;
                    heatCapacity[cell] = materialB.VolumetricHeatCapacity;
                }
                else
                {
                    conductivity[cell] = materialC.ThermalConductivity;
                    heatCapacity[cell] = materialC.VolumetricHeatCapacity;
                }
            }

            AdvanceHeterogeneousFiniteVolume(
                temperature,
                conductivity,
                heatCapacity,
                cellWidth,
                duration);

            return temperature;
        }

        private static void AdvanceHeterogeneousFiniteVolume(
            double[] temperature,
            double[] conductivity,
            double[] heatCapacity,
            double cellWidth,
            double duration)
        {
            var maximumDiffusivity = 0.0;
            for (var cell = 0; cell < temperature.Length; cell++)
            {
                maximumDiffusivity = Math.Max(
                    maximumDiffusivity,
                    conductivity[cell] / heatCapacity[cell]);
            }

            const double stabilityFactor = 0.35;
            var maximumStep = stabilityFactor * cellWidth * cellWidth
                / maximumDiffusivity;
            var stepCount = Math.Max(1, (int)Math.Ceiling(duration / maximumStep));
            var deltaTime = duration / stepCount;

            var faceFlux = new double[temperature.Length - 1];
            var next = new double[temperature.Length];

            for (var step = 0; step < stepCount; step++)
            {
                for (var face = 0; face < faceFlux.Length; face++)
                {
                    var leftK = conductivity[face];
                    var rightK = conductivity[face + 1];
                    var harmonicK = 2.0 * leftK * rightK / (leftK + rightK);
                    faceFlux[face] = -harmonicK
                        * (temperature[face + 1] - temperature[face])
                        / cellWidth;
                }

                next[0] = temperature[0]
                    - deltaTime * faceFlux[0]
                    / (heatCapacity[0] * cellWidth);

                for (var cell = 1; cell < temperature.Length - 1; cell++)
                {
                    next[cell] = temperature[cell]
                        + deltaTime * (faceFlux[cell - 1] - faceFlux[cell])
                        / (heatCapacity[cell] * cellWidth);
                }

                var last = temperature.Length - 1;
                next[last] = temperature[last]
                    + deltaTime * faceFlux[last - 1]
                    / (heatCapacity[last] * cellWidth);

                Array.Copy(next, temperature, temperature.Length);
            }
        }

        private static double RelativeRecoveredVsReferenceError(
            in GaussianMixture1D recoveredA,
            in GaussianMixture1D recoveredB,
            in GaussianMixture1D recoveredC,
            double[] reference,
            double cellWidth,
            int cellCountA,
            int cellCountB,
            int cellCountC)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            for (var cell = 0; cell < reference.Length; cell++)
            {
                double candidate;
                if (cell < cellCountA)
                {
                    candidate = recoveredA.Evaluate((cell + 0.5) * cellWidth);
                }
                else if (cell < cellCountA + cellCountB)
                {
                    var localCell = cell - cellCountA;
                    candidate = recoveredB.Evaluate((localCell + 0.5) * cellWidth);
                }
                else
                {
                    var localCell = cell - cellCountA - cellCountB;
                    candidate = recoveredC.Evaluate((localCell + 0.5) * cellWidth);
                }

                var difference = candidate - reference[cell];
                squaredError += difference * difference;
                squaredReference += reference[cell] * reference[cell];
            }

            return Math.Sqrt(squaredError / squaredReference);
        }
    }
}
