using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct TimeAdaptiveGaussianBudgetPoint1D
    {
        public TimeAdaptiveGaussianBudgetPoint1D(
            double time,
            double reducedStateVsFiniteVolumeError,
            int localCountA,
            int localCountB,
            int localCountC,
            int validatedCountA,
            int validatedCountB,
            int validatedCountC,
            double validatedGlobalStateError,
            double validatedMaximumRegionError,
            double validatedFiniteVolumeError,
            double maximumIntegralError)
        {
            Time = time;
            ReducedStateVsFiniteVolumeError = reducedStateVsFiniteVolumeError;
            LocalCountA = localCountA;
            LocalCountB = localCountB;
            LocalCountC = localCountC;
            ValidatedCountA = validatedCountA;
            ValidatedCountB = validatedCountB;
            ValidatedCountC = validatedCountC;
            ValidatedGlobalStateError = validatedGlobalStateError;
            ValidatedMaximumRegionError = validatedMaximumRegionError;
            ValidatedFiniteVolumeError = validatedFiniteVolumeError;
            MaximumIntegralError = maximumIntegralError;
        }

        public double Time { get; }
        public double ReducedStateVsFiniteVolumeError { get; }
        public int LocalCountA { get; }
        public int LocalCountB { get; }
        public int LocalCountC { get; }
        public int LocalTotalCount => LocalCountA + LocalCountB + LocalCountC;
        public int ValidatedCountA { get; }
        public int ValidatedCountB { get; }
        public int ValidatedCountC { get; }
        public int ValidatedTotalCount => ValidatedCountA + ValidatedCountB + ValidatedCountC;
        public bool HasValidatedBudget => ValidatedTotalCount > 0;
        public double ValidatedGlobalStateError { get; }
        public double ValidatedMaximumRegionError { get; }
        public double ValidatedFiniteVolumeError { get; }
        public double MaximumIntegralError { get; }
    }

    public readonly struct TimeAdaptiveGaussianBudgetStudyResult1D
    {
        private readonly TimeAdaptiveGaussianBudgetPoint1D[] _points;

        public TimeAdaptiveGaussianBudgetStudyResult1D(
            TimeAdaptiveGaussianBudgetPoint1D[] points,
            int minimumValidatedTotal,
            int maximumValidatedTotal,
            bool allocationChanged)
        {
            _points = points;
            MinimumValidatedTotal = minimumValidatedTotal;
            MaximumValidatedTotal = maximumValidatedTotal;
            AllocationChanged = allocationChanged;
        }

        public int Count => _points?.Length ?? 0;
        public int MinimumValidatedTotal { get; }
        public int MaximumValidatedTotal { get; }
        public bool AllocationChanged { get; }

        public TimeAdaptiveGaussianBudgetPoint1D GetPoint(int index)
        {
            if (_points is null || index < 0 || index >= _points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _points[index];
        }

        public bool Satisfies(
            double perRegionThreshold,
            double finiteVolumeThreshold,
            double maximumIntegralError)
        {
            if (Count < 2 || !AllocationChanged)
            {
                return false;
            }

            var validatedCount = 0;
            for (var i = 0; i < Count; i++)
            {
                var point = _points[i];
                if (!point.HasValidatedBudget)
                {
                    continue;
                }

                validatedCount++;
                if (point.ValidatedMaximumRegionError > perRegionThreshold
                    || point.ValidatedFiniteVolumeError > finiteVolumeThreshold
                    || point.MaximumIntegralError > maximumIntegralError)
                {
                    return false;
                }
            }

            return validatedCount >= 2
                && MinimumValidatedTotal > 0
                && MaximumValidatedTotal >= MinimumValidatedTotal;
        }
    }

    /// <summary>
    /// Checkpoint 9: repeats the validation-aware adaptive Gaussian-budget
    /// search at multiple times during the same heterogeneous A-B-C diffusion
    /// process. The reduced current state remains authoritative; Gaussian counts
    /// are recomputed only for downstream representation.
    ///
    /// The study also records the reduced-state-vs-finite-volume error floor at
    /// each snapshot. If that floor already exceeds the declared independent
    /// validation threshold, absence of a validated Gaussian allocation is not
    /// attributed to representation compression.
    /// </summary>
    public static class TimeAdaptiveGaussianBudgetStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumKernelCount = 8;
        private const int StateSampleCount = 401;
        private const double LocalThreshold = 5e-3;
        private const double FiniteVolumeThreshold = 5e-3;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

        public static TimeAdaptiveGaussianBudgetStudyResult1D Evaluate()
        {
            var materialA = new ThermalMaterial1D(0.40, 2.0);
            var materialB = new ThermalMaterial1D(0.06, 1.2);
            var materialC = new ThermalMaterial1D(0.20, 1.6);

            const double lengthA = 0.60;
            const double lengthB = 0.35;
            const double lengthC = 0.60;
            const double reducedDeltaTime = 0.002;

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

            var finiteVolume = new IncrementalFiniteVolumeReference(
                InitialField,
                lengthA,
                lengthB,
                lengthC,
                materialA,
                materialB,
                materialC);

            var points = new TimeAdaptiveGaussianBudgetPoint1D[SnapshotTimes.Length];
            var currentTime = 0.0;
            var minimumValidatedTotal = int.MaxValue;
            var maximumValidatedTotal = 0;
            var previousA = -1;
            var previousB = -1;
            var previousC = -1;
            var allocationChanged = false;

            for (var snapshotIndex = 0;
                snapshotIndex < SnapshotTimes.Length;
                snapshotIndex++)
            {
                var targetTime = SnapshotTimes[snapshotIndex];
                var interval = targetTime - currentTime;
                var stepCount = (int)Math.Round(interval / reducedDeltaTime);

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

                finiteVolume.Advance(interval);
                currentTime = targetTime;

                var fitsA = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateA, lengthA, MaximumKernelCount);
                var fitsB = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateB, lengthB, MaximumKernelCount);
                var fitsC = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateC, lengthC, MaximumKernelCount);

                var localA = FirstCountAtOrBelow(fitsA, LocalThreshold);
                var localB = FirstCountAtOrBelow(fitsB, LocalThreshold);
                var localC = FirstCountAtOrBelow(fitsC, LocalThreshold);

                var stateVsFiniteVolume = ReducedStateVsFiniteVolumeError(
                    state,
                    finiteVolume.Temperature,
                    finiteVolume.CellWidth,
                    finiteVolume.CellCountA,
                    finiteVolume.CellCountB,
                    lengthA,
                    lengthB,
                    lengthC);

                var bestA = 0;
                var bestB = 0;
                var bestC = 0;
                var bestTotal = int.MaxValue;
                var bestGlobalStateError = double.PositiveInfinity;
                var bestMaximumRegionError = double.PositiveInfinity;
                var bestFiniteVolumeError = double.PositiveInfinity;
                var bestMaximumIntegralError = double.PositiveInfinity;

                if (localA > 0 && localB > 0 && localC > 0)
                {
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

                                if (maximumRegionError > LocalThreshold)
                                {
                                    continue;
                                }

                                var finiteVolumeError = GaussianVsFiniteVolumeError(
                                    fitA.Mixture,
                                    fitB.Mixture,
                                    fitC.Mixture,
                                    finiteVolume.Temperature,
                                    finiteVolume.CellWidth,
                                    finiteVolume.CellCountA,
                                    finiteVolume.CellCountB);

                                if (finiteVolumeError > FiniteVolumeThreshold)
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
                                    bestMaximumRegionError = maximumRegionError;
                                    bestGlobalStateError = GlobalStateError(
                                        state,
                                        fitA.Mixture,
                                        fitB.Mixture,
                                        fitC.Mixture,
                                        lengthA,
                                        lengthB,
                                        lengthC);
                                    bestMaximumIntegralError = Math.Max(
                                        Math.Abs(fitA.IntegralError),
                                        Math.Max(
                                            Math.Abs(fitB.IntegralError),
                                            Math.Abs(fitC.IntegralError)));
                                }
                            }
                        }
                    }
                }

                if (bestTotal == int.MaxValue)
                {
                    bestTotal = 0;
                    bestGlobalStateError = double.NaN;
                    bestMaximumRegionError = double.NaN;
                    bestFiniteVolumeError = double.NaN;
                    bestMaximumIntegralError = double.NaN;
                }
                else
                {
                    minimumValidatedTotal = Math.Min(minimumValidatedTotal, bestTotal);
                    maximumValidatedTotal = Math.Max(maximumValidatedTotal, bestTotal);

                    if (previousA >= 0
                        && (bestA != previousA || bestB != previousB || bestC != previousC))
                    {
                        allocationChanged = true;
                    }
                    previousA = bestA;
                    previousB = bestB;
                    previousC = bestC;
                }

                points[snapshotIndex] = new TimeAdaptiveGaussianBudgetPoint1D(
                    targetTime,
                    stateVsFiniteVolume,
                    localA,
                    localB,
                    localC,
                    bestA,
                    bestB,
                    bestC,
                    bestGlobalStateError,
                    bestMaximumRegionError,
                    bestFiniteVolumeError,
                    bestMaximumIntegralError);
            }

            if (minimumValidatedTotal == int.MaxValue)
            {
                minimumValidatedTotal = 0;
            }

            return new TimeAdaptiveGaussianBudgetStudyResult1D(
                points,
                minimumValidatedTotal,
                maximumValidatedTotal,
                allocationChanged);
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
            AccumulateStateError(state.StateA, mixtureA, lengthA,
                ref squaredError, ref squaredReference);
            AccumulateStateError(state.StateB, mixtureB, lengthB,
                ref squaredError, ref squaredReference);
            AccumulateStateError(state.StateC, mixtureC, lengthC,
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

        private static double ReducedStateVsFiniteVolumeError(
            in ThreeLayerCoupledState1D state,
            double[] reference,
            double cellWidth,
            int cellCountA,
            int cellCountB,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            for (var cell = 0; cell < reference.Length; cell++)
            {
                double candidate;
                if (cell < cellCountA)
                {
                    candidate = FiniteLayerFieldRepresentation1D.Evaluate(
                        state.StateA, (cell + 0.5) * cellWidth, lengthA);
                }
                else if (cell < cellCountA + cellCountB)
                {
                    var local = cell - cellCountA;
                    candidate = FiniteLayerFieldRepresentation1D.Evaluate(
                        state.StateB, (local + 0.5) * cellWidth, lengthB);
                }
                else
                {
                    var local = cell - cellCountA - cellCountB;
                    candidate = FiniteLayerFieldRepresentation1D.Evaluate(
                        state.StateC, (local + 0.5) * cellWidth, lengthC);
                }

                var difference = candidate - reference[cell];
                squaredError += difference * difference;
                squaredReference += reference[cell] * reference[cell];
            }

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static double GaussianVsFiniteVolumeError(
            in GaussianMixture1D mixtureA,
            in GaussianMixture1D mixtureB,
            in GaussianMixture1D mixtureC,
            double[] reference,
            double cellWidth,
            int cellCountA,
            int cellCountB)
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
                    var local = cell - cellCountA;
                    candidate = mixtureB.Evaluate((local + 0.5) * cellWidth);
                }
                else
                {
                    var local = cell - cellCountA - cellCountB;
                    candidate = mixtureC.Evaluate((local + 0.5) * cellWidth);
                }

                var difference = candidate - reference[cell];
                squaredError += difference * difference;
                squaredReference += reference[cell] * reference[cell];
            }

            return Math.Sqrt(squaredError / squaredReference);
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

        private sealed class IncrementalFiniteVolumeReference
        {
            private readonly double[] _conductivity;
            private readonly double[] _heatCapacity;
            private readonly double[] _faceFlux;
            private readonly double[] _next;
            private readonly double _maximumDiffusivity;

            public IncrementalFiniteVolumeReference(
                Func<double, double> initialField,
                double lengthA,
                double lengthB,
                double lengthC,
                in ThermalMaterial1D materialA,
                in ThermalMaterial1D materialB,
                in ThermalMaterial1D materialC)
            {
                CellWidth = 0.005;
                CellCountA = (int)Math.Round(lengthA / CellWidth);
                CellCountB = (int)Math.Round(lengthB / CellWidth);
                var cellCountC = (int)Math.Round(lengthC / CellWidth);
                var total = CellCountA + CellCountB + cellCountC;

                Temperature = new double[total];
                _conductivity = new double[total];
                _heatCapacity = new double[total];
                _faceFlux = new double[total - 1];
                _next = new double[total];

                var maximumDiffusivity = 0.0;
                for (var cell = 0; cell < total; cell++)
                {
                    if (cell < CellCountA)
                    {
                        var x = (cell + 0.5) * CellWidth;
                        Temperature[cell] = initialField(x);
                        _conductivity[cell] = materialA.ThermalConductivity;
                        _heatCapacity[cell] = materialA.VolumetricHeatCapacity;
                    }
                    else if (cell < CellCountA + CellCountB)
                    {
                        _conductivity[cell] = materialB.ThermalConductivity;
                        _heatCapacity[cell] = materialB.VolumetricHeatCapacity;
                    }
                    else
                    {
                        _conductivity[cell] = materialC.ThermalConductivity;
                        _heatCapacity[cell] = materialC.VolumetricHeatCapacity;
                    }

                    maximumDiffusivity = Math.Max(
                        maximumDiffusivity,
                        _conductivity[cell] / _heatCapacity[cell]);
                }

                _maximumDiffusivity = maximumDiffusivity;
            }

            public double CellWidth { get; }
            public int CellCountA { get; }
            public int CellCountB { get; }
            public double[] Temperature { get; }

            public void Advance(double duration)
            {
                const double stabilityFactor = 0.35;
                var maximumStep = stabilityFactor * CellWidth * CellWidth
                    / _maximumDiffusivity;
                var stepCount = Math.Max(1, (int)Math.Ceiling(duration / maximumStep));
                var deltaTime = duration / stepCount;

                for (var step = 0; step < stepCount; step++)
                {
                    for (var face = 0; face < _faceFlux.Length; face++)
                    {
                        var leftK = _conductivity[face];
                        var rightK = _conductivity[face + 1];
                        var harmonicK = 2.0 * leftK * rightK / (leftK + rightK);
                        _faceFlux[face] = -harmonicK
                            * (Temperature[face + 1] - Temperature[face])
                            / CellWidth;
                    }

                    _next[0] = Temperature[0]
                        - deltaTime * _faceFlux[0]
                        / (_heatCapacity[0] * CellWidth);

                    for (var cell = 1; cell < Temperature.Length - 1; cell++)
                    {
                        _next[cell] = Temperature[cell]
                            + deltaTime * (_faceFlux[cell - 1] - _faceFlux[cell])
                            / (_heatCapacity[cell] * CellWidth);
                    }

                    var last = Temperature.Length - 1;
                    _next[last] = Temperature[last]
                        + deltaTime * _faceFlux[last - 1]
                        / (_heatCapacity[last] * CellWidth);

                    Array.Copy(_next, Temperature, Temperature.Length);
                }
            }
        }
    }
}
