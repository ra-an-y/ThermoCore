using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct NegligibleRegionGaussianRulePoint1D
    {
        public NegligibleRegionGaussianRulePoint1D(
            double time,
            double l2ContributionA,
            double l2ContributionB,
            double l2ContributionC,
            double peakContributionA,
            double peakContributionB,
            double peakContributionC,
            int countA,
            int countB,
            int countC,
            double globalRepresentationError)
        {
            Time = time;
            L2ContributionA = l2ContributionA;
            L2ContributionB = l2ContributionB;
            L2ContributionC = l2ContributionC;
            PeakContributionA = peakContributionA;
            PeakContributionB = peakContributionB;
            PeakContributionC = peakContributionC;
            CountA = countA;
            CountB = countB;
            CountC = countC;
            GlobalRepresentationError = globalRepresentationError;
        }

        public double Time { get; }
        public double L2ContributionA { get; }
        public double L2ContributionB { get; }
        public double L2ContributionC { get; }
        public double PeakContributionA { get; }
        public double PeakContributionB { get; }
        public double PeakContributionC { get; }
        public int CountA { get; }
        public int CountB { get; }
        public int CountC { get; }
        public int TotalCount => CountA + CountB + CountC;
        public double GlobalRepresentationError { get; }
        public bool HasZeroBudgetRegion => CountA == 0 || CountB == 0 || CountC == 0;
    }

    public readonly struct NegligibleRegionGaussianRuleStudyResult1D
    {
        private readonly NegligibleRegionGaussianRulePoint1D[] _points;

        public NegligibleRegionGaussianRuleStudyResult1D(
            NegligibleRegionGaussianRulePoint1D[] points,
            double l2OmissionThreshold,
            double peakOmissionThreshold)
        {
            _points = points;
            L2OmissionThreshold = l2OmissionThreshold;
            PeakOmissionThreshold = peakOmissionThreshold;
        }

        public int Count => _points?.Length ?? 0;
        public double L2OmissionThreshold { get; }
        public double PeakOmissionThreshold { get; }

        public NegligibleRegionGaussianRulePoint1D GetPoint(int index)
        {
            if (_points is null || index < 0 || index >= _points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _points[index];
        }

        public bool Satisfies(double maximumGlobalRepresentationError)
        {
            if (Count != 3)
            {
                return false;
            }

            var earlyZeroObserved = false;
            for (var index = 0; index < Count; index++)
            {
                var point = _points[index];
                if (point.GlobalRepresentationError > maximumGlobalRepresentationError)
                {
                    return false;
                }

                if (index < 2 && point.HasZeroBudgetRegion)
                {
                    earlyZeroObserved = true;
                }
            }

            // The final 0.40 s control should no longer classify a region as
            // negligible under the declared omission guards.
            return earlyZeroObserved && !_points[Count - 1].HasZeroBudgetRegion;
        }
    }

    /// <summary>
    /// Checkpoint 10: a downstream region may receive zero Gaussian primitives
    /// only when omitting that region is small in both global L2 contribution
    /// and peak-magnitude contribution. Physical state is never removed.
    /// </summary>
    public static class NegligibleRegionGaussianRuleStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumKernelCount = 8;
        private const int SampleCount = 401;
        private const double LocalFitThreshold = 5e-3;
        private const double L2OmissionThreshold = 1e-3;
        private const double PeakOmissionThreshold = 5e-3;

        private static readonly double[] SnapshotTimes = { 0.10, 0.20, 0.40 };

        public static NegligibleRegionGaussianRuleStudyResult1D Evaluate()
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
                const double standardDeviation = 0.05;
                var z = (x - mean) / standardDeviation;
                return Math.Exp(-0.5 * z * z);
            }

            var state = new ThreeLayerCoupledState1D(
                ProjectFieldToState(InitialField, lengthA, ModeCount),
                FiniteLayerReducedState1D.Zero(ModeCount),
                FiniteLayerReducedState1D.Zero(ModeCount));

            var points = new NegligibleRegionGaussianRulePoint1D[SnapshotTimes.Length];
            var currentTime = 0.0;

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

                var measures = MeasureContributions(
                    state,
                    lengthA,
                    lengthB,
                    lengthC);

                var fitsA = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateA, lengthA, MaximumKernelCount);
                var fitsB = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateB, lengthB, MaximumKernelCount);
                var fitsC = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateC, lengthC, MaximumKernelCount);

                var zeroA = IsNegligible(measures.L2A, measures.PeakA);
                var zeroB = IsNegligible(measures.L2B, measures.PeakB);
                var zeroC = IsNegligible(measures.L2C, measures.PeakC);

                var firstA = FirstCountAtOrBelow(fitsA, LocalFitThreshold);
                var firstB = FirstCountAtOrBelow(fitsB, LocalFitThreshold);
                var firstC = FirstCountAtOrBelow(fitsC, LocalFitThreshold);

                // Diagnostic fallback: when a non-negligible near-zero region
                // does not meet a relative regional threshold within the bounded
                // dictionary, retain the maximum budget instead of aborting.
                // This lets the checkpoint expose whether the omission guards or
                // the relative regional metric are responsible.
                var countA = zeroA ? 0 : (firstA > 0 ? firstA : MaximumKernelCount);
                var countB = zeroB ? 0 : (firstB > 0 ? firstB : MaximumKernelCount);
                var countC = zeroC ? 0 : (firstC > 0 ? firstC : MaximumKernelCount);

                var representationError = GlobalRepresentationError(
                    state,
                    countA == 0 ? null : fitsA[countA - 1].Mixture,
                    countB == 0 ? null : fitsB[countB - 1].Mixture,
                    countC == 0 ? null : fitsC[countC - 1].Mixture,
                    lengthA,
                    lengthB,
                    lengthC);

                points[snapshotIndex] = new NegligibleRegionGaussianRulePoint1D(
                    targetTime,
                    measures.L2A,
                    measures.L2B,
                    measures.L2C,
                    measures.PeakA,
                    measures.PeakB,
                    measures.PeakC,
                    countA,
                    countB,
                    countC,
                    representationError);
            }

            return new NegligibleRegionGaussianRuleStudyResult1D(
                points,
                L2OmissionThreshold,
                PeakOmissionThreshold);
        }

        private static bool IsNegligible(double l2Contribution, double peakContribution)
        {
            return l2Contribution <= L2OmissionThreshold
                && peakContribution <= PeakOmissionThreshold;
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

        private static ContributionMeasures MeasureContributions(
            in ThreeLayerCoupledState1D state,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var sumA = 0.0;
            var sumB = 0.0;
            var sumC = 0.0;
            var peakA = 0.0;
            var peakB = 0.0;
            var peakC = 0.0;

            MeasureRegion(state.StateA, lengthA, ref sumA, ref peakA);
            MeasureRegion(state.StateB, lengthB, ref sumB, ref peakB);
            MeasureRegion(state.StateC, lengthC, ref sumC, ref peakC);

            var globalNorm = Math.Sqrt(sumA + sumB + sumC);
            var globalPeak = Math.Max(peakA, Math.Max(peakB, peakC));

            return new ContributionMeasures(
                globalNorm > 0.0 ? Math.Sqrt(sumA) / globalNorm : 0.0,
                globalNorm > 0.0 ? Math.Sqrt(sumB) / globalNorm : 0.0,
                globalNorm > 0.0 ? Math.Sqrt(sumC) / globalNorm : 0.0,
                globalPeak > 0.0 ? peakA / globalPeak : 0.0,
                globalPeak > 0.0 ? peakB / globalPeak : 0.0,
                globalPeak > 0.0 ? peakC / globalPeak : 0.0);
        }

        private static void MeasureRegion(
            in FiniteLayerReducedState1D state,
            double length,
            ref double squaredNorm,
            ref double peak)
        {
            for (var sample = 0; sample < SampleCount; sample++)
            {
                var x = (sample + 0.5) * length / SampleCount;
                var value = FiniteLayerFieldRepresentation1D.Evaluate(state, x, length);
                squaredNorm += value * value;
                peak = Math.Max(peak, Math.Abs(value));
            }
        }

        private static double GlobalRepresentationError(
            in ThreeLayerCoupledState1D state,
            GaussianMixture1D? mixtureA,
            GaussianMixture1D? mixtureB,
            GaussianMixture1D? mixtureC,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            AccumulateError(state.StateA, mixtureA, lengthA, ref squaredError, ref squaredReference);
            AccumulateError(state.StateB, mixtureB, lengthB, ref squaredError, ref squaredReference);
            AccumulateError(state.StateC, mixtureC, lengthC, ref squaredError, ref squaredReference);

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static void AccumulateError(
            in FiniteLayerReducedState1D state,
            GaussianMixture1D? mixture,
            double length,
            ref double squaredError,
            ref double squaredReference)
        {
            for (var sample = 0; sample < SampleCount; sample++)
            {
                var x = (sample + 0.5) * length / SampleCount;
                var reference = FiniteLayerFieldRepresentation1D.Evaluate(state, x, length);
                var represented = mixture.HasValue ? mixture.Value.Evaluate(x) : 0.0;
                var difference = represented - reference;
                squaredError += difference * difference;
                squaredReference += reference * reference;
            }
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

        private readonly struct ContributionMeasures
        {
            public ContributionMeasures(
                double l2A,
                double l2B,
                double l2C,
                double peakA,
                double peakB,
                double peakC)
            {
                L2A = l2A;
                L2B = l2B;
                L2C = l2C;
                PeakA = peakA;
                PeakB = peakB;
                PeakC = peakC;
            }

            public double L2A { get; }
            public double L2B { get; }
            public double L2C { get; }
            public double PeakA { get; }
            public double PeakB { get; }
            public double PeakC { get; }
        }
    }
}
