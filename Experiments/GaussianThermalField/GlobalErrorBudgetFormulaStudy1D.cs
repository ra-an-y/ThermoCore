using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct GlobalErrorBudgetFormulaPoint1D
    {
        public GlobalErrorBudgetFormulaPoint1D(
            double time,
            int countA,
            int countB,
            int countC,
            double weightA,
            double weightB,
            double weightC,
            double regionalErrorA,
            double regionalErrorB,
            double regionalErrorC,
            double predictedGlobalError,
            double directGlobalError,
            double identityError,
            double peakContributionA,
            double peakContributionB,
            double peakContributionC)
        {
            Time = time;
            CountA = countA;
            CountB = countB;
            CountC = countC;
            WeightA = weightA;
            WeightB = weightB;
            WeightC = weightC;
            RegionalErrorA = regionalErrorA;
            RegionalErrorB = regionalErrorB;
            RegionalErrorC = regionalErrorC;
            PredictedGlobalError = predictedGlobalError;
            DirectGlobalError = directGlobalError;
            IdentityError = identityError;
            PeakContributionA = peakContributionA;
            PeakContributionB = peakContributionB;
            PeakContributionC = peakContributionC;
        }

        public double Time { get; }
        public int CountA { get; }
        public int CountB { get; }
        public int CountC { get; }
        public int TotalCount => CountA + CountB + CountC;
        public double WeightA { get; }
        public double WeightB { get; }
        public double WeightC { get; }
        public double RegionalErrorA { get; }
        public double RegionalErrorB { get; }
        public double RegionalErrorC { get; }
        public double PredictedGlobalError { get; }
        public double DirectGlobalError { get; }
        public double IdentityError { get; }
        public double PeakContributionA { get; }
        public double PeakContributionB { get; }
        public double PeakContributionC { get; }
    }

    public readonly struct GlobalErrorBudgetFormulaStudyResult1D
    {
        private readonly GlobalErrorBudgetFormulaPoint1D[] _points;

        public GlobalErrorBudgetFormulaStudyResult1D(
            GlobalErrorBudgetFormulaPoint1D[] points,
            double maximumIdentityError,
            int minimumTotalCount,
            int maximumTotalCount)
        {
            _points = points;
            MaximumIdentityError = maximumIdentityError;
            MinimumTotalCount = minimumTotalCount;
            MaximumTotalCount = maximumTotalCount;
        }

        public int Count => _points?.Length ?? 0;
        public double MaximumIdentityError { get; }
        public int MinimumTotalCount { get; }
        public int MaximumTotalCount { get; }

        public GlobalErrorBudgetFormulaPoint1D GetPoint(int index)
        {
            if (_points is null || index < 0 || index >= _points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _points[index];
        }

        public bool Satisfies(
            double globalErrorThreshold,
            double maximumIdentityError)
        {
            if (Count == 0 || MaximumIdentityError > maximumIdentityError)
            {
                return false;
            }

            for (var i = 0; i < Count; i++)
            {
                var point = _points[i];
                if (point.TotalCount <= 0
                    || point.PredictedGlobalError > globalErrorThreshold
                    || point.DirectGlobalError > globalErrorThreshold
                    || point.IdentityError > maximumIdentityError)
                {
                    return false;
                }
            }

            return MinimumTotalCount > 0
                && MaximumTotalCount >= MinimumTotalCount;
        }
    }

    /// <summary>
    /// Verifies and uses the disjoint-region L2 identity
    ///
    /// E_global^2 = sum_i [ w_i * e_i(N_i) ]^2
    ///
    /// where w_i is the physical-length-weighted regional field norm divided by
    /// the global norm and e_i is the regional relative representation error.
    ///
    /// The identity turns Gaussian allocation into a constrained budget problem
    /// instead of requiring the same relative percentage in every region.
    /// </summary>
    public static class GlobalErrorBudgetFormulaStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumGaussianCount = 8;
        private const int SampleCount = 401;
        private const double GlobalErrorThreshold = 5e-3;
        private const double PeakZeroGuard = 5e-3;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

        public static GlobalErrorBudgetFormulaStudyResult1D Evaluate()
        {
            var materialA = new ThermalMaterial1D(0.40, 2.0);
            var materialB = new ThermalMaterial1D(0.06, 1.2);
            var materialC = new ThermalMaterial1D(0.20, 1.6);

            const double lengthA = 0.60;
            const double lengthB = 0.35;
            const double lengthC = 0.60;
            const double deltaTime = 0.002;

            static double InitialField(double x)
            {
                const double mean = 0.46;
                const double sigma = 0.05;
                var z = (x - mean) / sigma;
                return Math.Exp(-0.5 * z * z);
            }

            var state = new ThreeLayerCoupledState1D(
                ProjectFieldToState(InitialField, lengthA, ModeCount),
                FiniteLayerReducedState1D.Zero(ModeCount),
                FiniteLayerReducedState1D.Zero(ModeCount));

            var points = new GlobalErrorBudgetFormulaPoint1D[SnapshotTimes.Length];
            var currentTime = 0.0;
            var maximumIdentityError = 0.0;
            var minimumTotal = int.MaxValue;
            var maximumTotal = 0;

            for (var snapshotIndex = 0; snapshotIndex < SnapshotTimes.Length; snapshotIndex++)
            {
                var targetTime = SnapshotTimes[snapshotIndex];
                var interval = targetTime - currentTime;
                var stepCount = (int)Math.Round(interval / deltaTime);

                for (var step = 0; step < stepCount; step++)
                {
                    state = ThreeLayerCoupledEvolution1D.Advance(
                        state,
                        deltaTime,
                        lengthA,
                        lengthB,
                        lengthC,
                        materialA,
                        materialB,
                        materialC).State;
                }
                currentTime = targetTime;

                var fitsA = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateA, lengthA, MaximumGaussianCount);
                var fitsB = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateB, lengthB, MaximumGaussianCount);
                var fitsC = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateC, lengthC, MaximumGaussianCount);

                var norm = MeasureNormsAndPeaks(
                    state,
                    lengthA,
                    lengthB,
                    lengthC);

                var bestTotal = int.MaxValue;
                var bestA = 0;
                var bestB = 0;
                var bestC = 0;
                var bestPredicted = double.PositiveInfinity;
                var bestDirect = double.PositiveInfinity;
                var bestIdentity = double.PositiveInfinity;
                var bestErrorA = double.PositiveInfinity;
                var bestErrorB = double.PositiveInfinity;
                var bestErrorC = double.PositiveInfinity;

                for (var countA = 0; countA <= MaximumGaussianCount; countA++)
                {
                    if (countA == 0 && norm.PeakA > PeakZeroGuard)
                    {
                        continue;
                    }

                    for (var countB = 0; countB <= MaximumGaussianCount; countB++)
                    {
                        if (countB == 0 && norm.PeakB > PeakZeroGuard)
                        {
                            continue;
                        }

                        for (var countC = 0; countC <= MaximumGaussianCount; countC++)
                        {
                            if (countC == 0 && norm.PeakC > PeakZeroGuard)
                            {
                                continue;
                            }

                            var total = countA + countB + countC;
                            if (total == 0 || total > bestTotal)
                            {
                                continue;
                            }

                            var errorA = countA == 0
                                ? 1.0
                                : fitsA[countA - 1].RelativeError;
                            var errorB = countB == 0
                                ? 1.0
                                : fitsB[countB - 1].RelativeError;
                            var errorC = countC == 0
                                ? 1.0
                                : fitsC[countC - 1].RelativeError;

                            var predicted = Math.Sqrt(
                                Square(norm.WeightA * errorA)
                                + Square(norm.WeightB * errorB)
                                + Square(norm.WeightC * errorC));

                            if (predicted > GlobalErrorThreshold)
                            {
                                continue;
                            }

                            var direct = DirectGlobalError(
                                state,
                                countA == 0 ? null : fitsA[countA - 1].Mixture,
                                countB == 0 ? null : fitsB[countB - 1].Mixture,
                                countC == 0 ? null : fitsC[countC - 1].Mixture,
                                lengthA,
                                lengthB,
                                lengthC);

                            var identity = Math.Abs(predicted - direct);

                            if (total < bestTotal
                                || (total == bestTotal && direct < bestDirect))
                            {
                                bestTotal = total;
                                bestA = countA;
                                bestB = countB;
                                bestC = countC;
                                bestPredicted = predicted;
                                bestDirect = direct;
                                bestIdentity = identity;
                                bestErrorA = errorA;
                                bestErrorB = errorB;
                                bestErrorC = errorC;
                            }
                        }
                    }
                }

                if (bestTotal == int.MaxValue)
                {
                    throw new InvalidOperationException(
                        "No global-error-budget allocation was found.");
                }

                maximumIdentityError = Math.Max(maximumIdentityError, bestIdentity);
                minimumTotal = Math.Min(minimumTotal, bestTotal);
                maximumTotal = Math.Max(maximumTotal, bestTotal);

                points[snapshotIndex] = new GlobalErrorBudgetFormulaPoint1D(
                    targetTime,
                    bestA,
                    bestB,
                    bestC,
                    norm.WeightA,
                    norm.WeightB,
                    norm.WeightC,
                    bestErrorA,
                    bestErrorB,
                    bestErrorC,
                    bestPredicted,
                    bestDirect,
                    bestIdentity,
                    norm.PeakA,
                    norm.PeakB,
                    norm.PeakC);
            }

            return new GlobalErrorBudgetFormulaStudyResult1D(
                points,
                maximumIdentityError,
                minimumTotal,
                maximumTotal);
        }

        private static NormAndPeak MeasureNormsAndPeaks(
            in ThreeLayerCoupledState1D state,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredA = PhysicalSquaredNorm(state.StateA, lengthA, out var peakA);
            var squaredB = PhysicalSquaredNorm(state.StateB, lengthB, out var peakB);
            var squaredC = PhysicalSquaredNorm(state.StateC, lengthC, out var peakC);
            var totalSquared = squaredA + squaredB + squaredC;
            var globalNorm = Math.Sqrt(Math.Max(totalSquared, 1e-30));
            var globalPeak = Math.Max(peakA, Math.Max(peakB, peakC));
            var safePeak = Math.Max(globalPeak, 1e-30);

            return new NormAndPeak(
                Math.Sqrt(squaredA) / globalNorm,
                Math.Sqrt(squaredB) / globalNorm,
                Math.Sqrt(squaredC) / globalNorm,
                peakA / safePeak,
                peakB / safePeak,
                peakC / safePeak);
        }

        private static double PhysicalSquaredNorm(
            in FiniteLayerReducedState1D state,
            double length,
            out double peak)
        {
            var sum = 0.0;
            peak = 0.0;
            var dx = length / SampleCount;

            for (var sample = 0; sample < SampleCount; sample++)
            {
                var x = (sample + 0.5) * dx;
                var value = FiniteLayerFieldRepresentation1D.Evaluate(state, x, length);
                sum += value * value;
                peak = Math.Max(peak, Math.Abs(value));
            }

            return dx * sum;
        }

        private static double DirectGlobalError(
            in ThreeLayerCoupledState1D state,
            GaussianMixture1D? mixtureA,
            GaussianMixture1D? mixtureB,
            GaussianMixture1D? mixtureC,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var errorSquared = 0.0;
            var referenceSquared = 0.0;

            AccumulateDirectError(
                state.StateA,
                mixtureA,
                lengthA,
                ref errorSquared,
                ref referenceSquared);
            AccumulateDirectError(
                state.StateB,
                mixtureB,
                lengthB,
                ref errorSquared,
                ref referenceSquared);
            AccumulateDirectError(
                state.StateC,
                mixtureC,
                lengthC,
                ref errorSquared,
                ref referenceSquared);

            return Math.Sqrt(errorSquared / referenceSquared);
        }

        private static void AccumulateDirectError(
            in FiniteLayerReducedState1D state,
            GaussianMixture1D? mixture,
            double length,
            ref double errorSquared,
            ref double referenceSquared)
        {
            var dx = length / SampleCount;
            for (var sample = 0; sample < SampleCount; sample++)
            {
                var x = (sample + 0.5) * dx;
                var reference = FiniteLayerFieldRepresentation1D.Evaluate(state, x, length);
                var candidate = mixture.HasValue ? mixture.Value.Evaluate(x) : 0.0;
                var difference = candidate - reference;
                errorSquared += dx * difference * difference;
                referenceSquared += dx * reference * reference;
            }
        }

        private static double Square(double value) => value * value;

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

        private readonly struct NormAndPeak
        {
            public NormAndPeak(
                double weightA,
                double weightB,
                double weightC,
                double peakA,
                double peakB,
                double peakC)
            {
                WeightA = weightA;
                WeightB = weightB;
                WeightC = weightC;
                PeakA = peakA;
                PeakB = peakB;
                PeakC = peakC;
            }

            public double WeightA { get; }
            public double WeightB { get; }
            public double WeightC { get; }
            public double PeakA { get; }
            public double PeakB { get; }
            public double PeakC { get; }
        }
    }
}
