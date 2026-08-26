using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct AdaptiveGaussianBudgetResult1D
    {
        public AdaptiveGaussianBudgetResult1D(
            int localCountA,
            int localCountB,
            int localCountC,
            double localGlobalStateError,
            double localFiniteVolumeError,
            int validatedCountA,
            int validatedCountB,
            int validatedCountC,
            double validatedGlobalStateError,
            double validatedFiniteVolumeError,
            double maximumValidatedRegionStateError,
            double maximumIntegralError)
        {
            LocalCountA = localCountA;
            LocalCountB = localCountB;
            LocalCountC = localCountC;
            LocalGlobalStateError = localGlobalStateError;
            LocalFiniteVolumeError = localFiniteVolumeError;
            ValidatedCountA = validatedCountA;
            ValidatedCountB = validatedCountB;
            ValidatedCountC = validatedCountC;
            ValidatedGlobalStateError = validatedGlobalStateError;
            ValidatedFiniteVolumeError = validatedFiniteVolumeError;
            MaximumValidatedRegionStateError = maximumValidatedRegionStateError;
            MaximumIntegralError = maximumIntegralError;
        }

        public int LocalCountA { get; }
        public int LocalCountB { get; }
        public int LocalCountC { get; }
        public int LocalTotalCount => LocalCountA + LocalCountB + LocalCountC;
        public double LocalGlobalStateError { get; }
        public double LocalFiniteVolumeError { get; }

        public int ValidatedCountA { get; }
        public int ValidatedCountB { get; }
        public int ValidatedCountC { get; }
        public int ValidatedTotalCount => ValidatedCountA + ValidatedCountB + ValidatedCountC;
        public double ValidatedGlobalStateError { get; }
        public double ValidatedFiniteVolumeError { get; }
        public double MaximumValidatedRegionStateError { get; }
        public double MaximumIntegralError { get; }

        public bool Satisfies(
            double perRegionThreshold,
            double finiteVolumeThreshold,
            int maximumValidatedTotalCount,
            double maximumIntegralError)
        {
            return LocalTotalCount > 0
                && ValidatedTotalCount > 0
                && LocalFiniteVolumeError > finiteVolumeThreshold
                && ValidatedFiniteVolumeError <= finiteVolumeThreshold
                && MaximumValidatedRegionStateError <= perRegionThreshold
                && ValidatedTotalCount <= maximumValidatedTotalCount
                && MaximumIntegralError <= maximumIntegralError;
        }
    }

    /// <summary>
    /// Checkpoint 8: adaptive per-region Gaussian budget.
    ///
    /// First, each region independently selects the smallest constrained sparse
    /// Gaussian count whose representation error is below the declared local
    /// threshold. Second, all combinations at or above those local minima are
    /// searched for the smallest total count that also satisfies an independent
    /// heterogeneous finite-volume error threshold.
    ///
    /// This distinguishes a representation-local minimum from a
    /// validation-aware minimum.
    /// </summary>
    public static class AdaptiveGaussianBudgetStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumKernelCount = 8;
        private const int StateSampleCount = 401;

        public static AdaptiveGaussianBudgetResult1D Evaluate()
        {
            var materialA = new ThermalMaterial1D(0.40, 2.0);
            var materialB = new ThermalMaterial1D(0.06, 1.2);
            var materialC = new ThermalMaterial1D(0.20, 1.6);

            const double lengthA = 0.60;
            const double lengthB = 0.35;
            const double lengthC = 0.60;
            const double reducedDeltaTime = 0.002;
            const double duration = 0.60;
            const double localThreshold = 5e-3;
            const double finiteVolumeThreshold = 5e-3;

            static double InitialField(double x)
            {
                const double mean = 0.46;
                const double standardDeviation = 0.05;
                var z = (x - mean) / standardDeviation;
                return Math.Exp(-0.5 * z * z);
            }

            var state = new ThreeLayerCoupledState1D(
                ProjectFieldToState(InitialField, lengthA, ModeCount),
                FiniteLayerReducedState1D.Zero(ModeCount),
                FiniteLayerReducedState1D.Zero(ModeCount));

            var stepCount = (int)Math.Round(duration / reducedDeltaTime);
            for (var step = 0; step < stepCount; step++)
            {
                state = ThreeLayerCoupledEvolution1D.Advance(
                    state,
                    reducedDeltaTime,
                    lengthA,
                    lengthB,
                    lengthC,
                    materialA,
                    materialB,
                    materialC).State;
            }

            var fitsA = ConstrainedGaussianSparseFitter1D.FitSequence(
                state.StateA, lengthA, MaximumKernelCount);
            var fitsB = ConstrainedGaussianSparseFitter1D.FitSequence(
                state.StateB, lengthB, MaximumKernelCount);
            var fitsC = ConstrainedGaussianSparseFitter1D.FitSequence(
                state.StateC, lengthC, MaximumKernelCount);

            var localA = FirstCountAtOrBelow(fitsA, localThreshold);
            var localB = FirstCountAtOrBelow(fitsB, localThreshold);
            var localC = FirstCountAtOrBelow(fitsC, localThreshold);

            if (localA == 0 || localB == 0 || localC == 0)
            {
                throw new InvalidOperationException(
                    "The declared local representation threshold was not reached.");
            }

            var reference = CreateFiniteVolumeReference(
                InitialField,
                duration,
                lengthA,
                lengthB,
                lengthC,
                materialA,
                materialB,
                materialC,
                out var cellWidth,
                out var cellCountA,
                out var cellCountB,
                out var cellCountC);

            var localFitA = fitsA[localA - 1];
            var localFitB = fitsB[localB - 1];
            var localFitC = fitsC[localC - 1];

            var localGlobalStateError = GlobalStateError(
                state,
                localFitA.Mixture,
                localFitB.Mixture,
                localFitC.Mixture,
                lengthA,
                lengthB,
                lengthC);
            var localFiniteVolumeError = FiniteVolumeError(
                localFitA.Mixture,
                localFitB.Mixture,
                localFitC.Mixture,
                reference,
                cellWidth,
                cellCountA,
                cellCountB,
                cellCountC);

            var bestTotal = int.MaxValue;
            var bestA = 0;
            var bestB = 0;
            var bestC = 0;
            var bestGlobalStateError = double.PositiveInfinity;
            var bestFiniteVolumeError = double.PositiveInfinity;
            var bestMaximumRegionError = double.PositiveInfinity;
            var bestMaximumIntegralError = double.PositiveInfinity;

            for (var countA = localA; countA <= MaximumKernelCount; countA++)
            {
                for (var countB = localB; countB <= MaximumKernelCount; countB++)
                {
                    for (var countC = localC; countC <= MaximumKernelCount; countC++)
                    {
                        var total = countA + countB + countC;
                        if (total > bestTotal)
                        {
                            continue;
                        }

                        var fitA = fitsA[countA - 1];
                        var fitB = fitsB[countB - 1];
                        var fitC = fitsC[countC - 1];
                        var maximumRegionError = Math.Max(
                            fitA.RelativeError,
                            Math.Max(fitB.RelativeError, fitC.RelativeError));

                        if (maximumRegionError > localThreshold)
                        {
                            continue;
                        }

                        var finiteVolumeError = FiniteVolumeError(
                            fitA.Mixture,
                            fitB.Mixture,
                            fitC.Mixture,
                            reference,
                            cellWidth,
                            cellCountA,
                            cellCountB,
                            cellCountC);

                        if (finiteVolumeError > finiteVolumeThreshold)
                        {
                            continue;
                        }

                        if (total < bestTotal
                            || (total == bestTotal
                                && finiteVolumeError < bestFiniteVolumeError))
                        {
                            bestTotal = total;
                            bestA = countA;
                            bestB = countB;
                            bestC = countC;
                            bestFiniteVolumeError = finiteVolumeError;
                            bestGlobalStateError = GlobalStateError(
                                state,
                                fitA.Mixture,
                                fitB.Mixture,
                                fitC.Mixture,
                                lengthA,
                                lengthB,
                                lengthC);
                            bestMaximumRegionError = maximumRegionError;
                            bestMaximumIntegralError = Math.Max(
                                Math.Abs(fitA.IntegralError),
                                Math.Max(
                                    Math.Abs(fitB.IntegralError),
                                    Math.Abs(fitC.IntegralError)));
                        }
                    }
                }
            }

            if (bestTotal == int.MaxValue)
            {
                throw new InvalidOperationException(
                    "No validation-aware adaptive Gaussian budget was found.");
            }

            return new AdaptiveGaussianBudgetResult1D(
                localA,
                localB,
                localC,
                localGlobalStateError,
                localFiniteVolumeError,
                bestA,
                bestB,
                bestC,
                bestGlobalStateError,
                bestFiniteVolumeError,
                bestMaximumRegionError,
                bestMaximumIntegralError);
        }

        private static int FirstCountAtOrBelow(
            ConstrainedGaussianSparseFitResult1D[] fits,
            double threshold)
        {
            for (var index = 0; index < fits.Length; index++)
            {
                if (fits[index].RelativeError <= threshold)
                {
                    return index + 1;
                }
            }
            return 0;
        }

        private static double GlobalStateError(
            in ThreeLayerCoupledState1D state,
            in GaussianMixture1D mixtureA,
            in GaussianMixture1D mixtureB,
            in GaussianMixture1D mixtureC,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            AccumulateStateError(
                state.StateA, mixtureA, lengthA,
                ref squaredError, ref squaredReference);
            AccumulateStateError(
                state.StateB, mixtureB, lengthB,
                ref squaredError, ref squaredReference);
            AccumulateStateError(
                state.StateC, mixtureC, lengthC,
                ref squaredError, ref squaredReference);

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static void AccumulateStateError(
            in FiniteLayerReducedState1D state,
            in GaussianMixture1D mixture,
            double layerLength,
            ref double squaredError,
            ref double squaredReference)
        {
            for (var sample = 0; sample < StateSampleCount; sample++)
            {
                var x = (sample + 0.5) * layerLength / StateSampleCount;
                var reference = FiniteLayerFieldRepresentation1D.Evaluate(
                    state, x, layerLength);
                var difference = mixture.Evaluate(x) - reference;
                squaredError += difference * difference;
                squaredReference += reference * reference;
            }
        }

        private static double FiniteVolumeError(
            in GaussianMixture1D mixtureA,
            in GaussianMixture1D mixtureB,
            in GaussianMixture1D mixtureC,
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
                    candidate = mixtureA.Evaluate((cell + 0.5) * cellWidth);
                }
                else if (cell < cellCountA + cellCountB)
                {
                    var localCell = cell - cellCountA;
                    candidate = mixtureB.Evaluate((localCell + 0.5) * cellWidth);
                }
                else
                {
                    var localCell = cell - cellCountA - cellCountB;
                    candidate = mixtureC.Evaluate((localCell + 0.5) * cellWidth);
                }

                var difference = candidate - reference[cell];
                squaredError += difference * difference;
                squaredReference += reference[cell] * reference[cell];
            }

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static double[] CreateFiniteVolumeReference(
            Func<double, double> initialField,
            double duration,
            double lengthA,
            double lengthB,
            double lengthC,
            in ThermalMaterial1D materialA,
            in ThermalMaterial1D materialB,
            in ThermalMaterial1D materialC,
            out double cellWidth,
            out int cellCountA,
            out int cellCountB,
            out int cellCountC)
        {
            cellWidth = 0.005;
            cellCountA = (int)Math.Round(lengthA / cellWidth);
            cellCountB = (int)Math.Round(lengthB / cellWidth);
            cellCountC = (int)Math.Round(lengthC / cellWidth);
            var totalCellCount = cellCountA + cellCountB + cellCountC;

            var temperature = new double[totalCellCount];
            var conductivity = new double[totalCellCount];
            var heatCapacity = new double[totalCellCount];

            for (var cell = 0; cell < totalCellCount; cell++)
            {
                if (cell < cellCountA)
                {
                    var x = (cell + 0.5) * cellWidth;
                    temperature[cell] = initialField(x);
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

            var maximumDiffusivity = 0.0;
            for (var cell = 0; cell < totalCellCount; cell++)
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
            var faceFlux = new double[totalCellCount - 1];
            var next = new double[totalCellCount];

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

                for (var cell = 1; cell < totalCellCount - 1; cell++)
                {
                    next[cell] = temperature[cell]
                        + deltaTime * (faceFlux[cell - 1] - faceFlux[cell])
                        / (heatCapacity[cell] * cellWidth);
                }

                var last = totalCellCount - 1;
                next[last] = temperature[last]
                    + deltaTime * faceFlux[last - 1]
                    / (heatCapacity[last] * cellWidth);

                Array.Copy(next, temperature, totalCellCount);
            }

            return temperature;
        }

        private static FiniteLayerReducedState1D ProjectFieldToState(
            Func<double, double> field,
            double length,
            int modeCount)
        {
            const int intervalCount = 8192;
            var dx = length / intervalCount;
            var meanIntegral = 0.0;
            var modeIntegrals = new double[modeCount];

            for (var sample = 0; sample <= intervalCount; sample++)
            {
                var x = sample * dx;
                var weight = sample == 0 || sample == intervalCount ? 0.5 : 1.0;
                var value = field(x);
                meanIntegral += weight * value;

                for (var modeIndex = 0; modeIndex < modeCount; modeIndex++)
                {
                    var n = modeIndex + 1;
                    modeIntegrals[modeIndex] += weight * value
                        * Math.Cos(n * Math.PI * x / length);
                }
            }

            var mean = meanIntegral * dx / length;
            for (var modeIndex = 0; modeIndex < modeCount; modeIndex++)
            {
                modeIntegrals[modeIndex] *= 2.0 * dx / length;
            }

            return new FiniteLayerReducedState1D(mean, modeIntegrals);
        }
    }
}
