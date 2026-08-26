using System;
using System.Diagnostics;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct HybridWallClockPoint1D
    {
        public HybridWallClockPoint1D(
            double time,
            int proposalA,
            int proposalB,
            int proposalC,
            double exhaustiveMedianMilliseconds,
            double hybridMedianMilliseconds,
            double exhaustiveMinimumMilliseconds,
            double hybridMinimumMilliseconds)
        {
            Time = time;
            ProposalA = proposalA;
            ProposalB = proposalB;
            ProposalC = proposalC;
            ExhaustiveMedianMilliseconds = exhaustiveMedianMilliseconds;
            HybridMedianMilliseconds = hybridMedianMilliseconds;
            ExhaustiveMinimumMilliseconds = exhaustiveMinimumMilliseconds;
            HybridMinimumMilliseconds = hybridMinimumMilliseconds;
        }

        public double Time { get; }
        public int ProposalA { get; }
        public int ProposalB { get; }
        public int ProposalC { get; }
        public int ProposalFitLevels => ProposalA + ProposalB + ProposalC;
        public double ExhaustiveMedianMilliseconds { get; }
        public double HybridMedianMilliseconds { get; }
        public double ExhaustiveMinimumMilliseconds { get; }
        public double HybridMinimumMilliseconds { get; }
        public double MedianSpeedup => ExhaustiveMedianMilliseconds / HybridMedianMilliseconds;
        public double MedianReductionFraction
            => 1.0 - HybridMedianMilliseconds / ExhaustiveMedianMilliseconds;
    }

    public readonly struct HybridWallClockBenchmarkResult1D
    {
        private readonly HybridWallClockPoint1D[] _points;

        public HybridWallClockBenchmarkResult1D(
            HybridWallClockPoint1D[] points,
            int trialCount,
            double exhaustiveMedianSumMilliseconds,
            double hybridMedianSumMilliseconds,
            double sink)
        {
            _points = points;
            TrialCount = trialCount;
            ExhaustiveMedianSumMilliseconds = exhaustiveMedianSumMilliseconds;
            HybridMedianSumMilliseconds = hybridMedianSumMilliseconds;
            Sink = sink;
        }

        public int Count => _points?.Length ?? 0;
        public int TrialCount { get; }
        public double ExhaustiveMedianSumMilliseconds { get; }
        public double HybridMedianSumMilliseconds { get; }
        public double AggregateSpeedup
            => ExhaustiveMedianSumMilliseconds / HybridMedianSumMilliseconds;
        public double AggregateReductionFraction
            => 1.0 - HybridMedianSumMilliseconds / ExhaustiveMedianSumMilliseconds;
        public double Sink { get; }

        public HybridWallClockPoint1D GetPoint(int index)
        {
            if (_points is null || index < 0 || index >= _points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _points[index];
        }

        public bool SatisfiesDataIntegrity()
        {
            if (Count == 0 || TrialCount < 3
                || !IsPositiveFinite(ExhaustiveMedianSumMilliseconds)
                || !IsPositiveFinite(HybridMedianSumMilliseconds)
                || double.IsNaN(Sink)
                || double.IsInfinity(Sink))
            {
                return false;
            }

            for (var i = 0; i < Count; i++)
            {
                var point = _points[i];
                if (point.ProposalFitLevels <= 0
                    || point.ProposalFitLevels > 24
                    || !IsPositiveFinite(point.ExhaustiveMedianMilliseconds)
                    || !IsPositiveFinite(point.HybridMedianMilliseconds)
                    || !IsPositiveFinite(point.ExhaustiveMinimumMilliseconds)
                    || !IsPositiveFinite(point.HybridMinimumMilliseconds))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsPositiveFinite(double value)
            => value > 0.0 && !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// Wall-clock benchmark for the online nested Gaussian fitting phase.
    ///
    /// Snapshot construction, offline calibration, oracle construction, and
    /// checkpoint-16 training instrumentation are deliberately outside the
    /// timed section. The hybrid path uses the already validated checkpoint-17
    /// proposal bounds and measures the actual current fitter work needed to
    /// construct levels 1..N in each region.
    ///
    /// Measurements are warmed up, repeated, and interleaved in alternating
    /// order. Medians are reported to reduce sensitivity to shared CI noise.
    /// </summary>
    public static class HybridWallClockBenchmark1D
    {
        private const int ModeCount = 32;
        private const int MaximumGaussianCount = 8;
        private const int TrialCount = 7;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

        // Checkpoint-17 count-dependent proposals. These are treated as online
        // upper bounds; their offline derivation is not part of fitter timing.
        private static readonly int[,] ProposalCounts =
        {
            { 6, 8, 0 },
            { 5, 6, 1 },
            { 4, 5, 2 },
            { 3, 5, 3 },
            { 4, 5, 2 },
            { 4, 4, 3 }
        };

        private static double _sink;

        public static HybridWallClockBenchmarkResult1D Evaluate()
        {
            var states = BuildSnapshotStates();
            var points = new HybridWallClockPoint1D[states.Length];

            // JIT and candidate-path warmup, intentionally excluded from timing.
            RunExhaustive(states[0]);
            RunHybrid(states[0], ProposalCounts[0, 0], ProposalCounts[0, 1], ProposalCounts[0, 2]);

            var exhaustiveMedianSum = 0.0;
            var hybridMedianSum = 0.0;

            for (var snapshot = 0; snapshot < states.Length; snapshot++)
            {
                var exhaustive = new double[TrialCount];
                var hybrid = new double[TrialCount];
                var proposalA = ProposalCounts[snapshot, 0];
                var proposalB = ProposalCounts[snapshot, 1];
                var proposalC = ProposalCounts[snapshot, 2];

                for (var trial = 0; trial < TrialCount; trial++)
                {
                    // Alternate order so one strategy is not always favored by
                    // thermal/frequency drift or allocator state on the runner.
                    if ((trial & 1) == 0)
                    {
                        exhaustive[trial] = MeasureMilliseconds(
                            () => RunExhaustive(states[snapshot]));
                        hybrid[trial] = MeasureMilliseconds(
                            () => RunHybrid(states[snapshot], proposalA, proposalB, proposalC));
                    }
                    else
                    {
                        hybrid[trial] = MeasureMilliseconds(
                            () => RunHybrid(states[snapshot], proposalA, proposalB, proposalC));
                        exhaustive[trial] = MeasureMilliseconds(
                            () => RunExhaustive(states[snapshot]));
                    }
                }

                var exhaustiveMedian = Median(exhaustive);
                var hybridMedian = Median(hybrid);
                exhaustiveMedianSum += exhaustiveMedian;
                hybridMedianSum += hybridMedian;

                points[snapshot] = new HybridWallClockPoint1D(
                    SnapshotTimes[snapshot],
                    proposalA,
                    proposalB,
                    proposalC,
                    exhaustiveMedian,
                    hybridMedian,
                    Minimum(exhaustive),
                    Minimum(hybrid));
            }

            return new HybridWallClockBenchmarkResult1D(
                points,
                TrialCount,
                exhaustiveMedianSum,
                hybridMedianSum,
                _sink);
        }

        private static double MeasureMilliseconds(Action action)
        {
            // Clear prior benchmark garbage outside the timed interval. This does
            // not prevent collections inside the operation; repeated medians are
            // therefore still required.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var start = Stopwatch.GetTimestamp();
            action();
            var end = Stopwatch.GetTimestamp();
            return (end - start) * 1000.0 / Stopwatch.Frequency;
        }

        private static void RunExhaustive(in ThreeLayerCoupledState1D state)
        {
            var a = ConstrainedGaussianSparseFitter1D.FitSequence(
                state.StateA, 0.60, MaximumGaussianCount);
            var b = ConstrainedGaussianSparseFitter1D.FitSequence(
                state.StateB, 0.35, MaximumGaussianCount);
            var c = ConstrainedGaussianSparseFitter1D.FitSequence(
                state.StateC, 0.60, MaximumGaussianCount);
            _sink += a[^1].RelativeError + b[^1].RelativeError + c[^1].RelativeError;
        }

        private static void RunHybrid(
            in ThreeLayerCoupledState1D state,
            int countA,
            int countB,
            int countC)
        {
            if (countA > 0)
            {
                var a = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateA, 0.60, countA);
                _sink += a[^1].RelativeError;
            }
            if (countB > 0)
            {
                var b = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateB, 0.35, countB);
                _sink += b[^1].RelativeError;
            }
            if (countC > 0)
            {
                var c = ConstrainedGaussianSparseFitter1D.FitSequence(
                    state.StateC, 0.60, countC);
                _sink += c[^1].RelativeError;
            }
        }

        private static ThreeLayerCoupledState1D[] BuildSnapshotStates()
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

            var states = new ThreeLayerCoupledState1D[SnapshotTimes.Length];
            var currentTime = 0.0;

            for (var index = 0; index < SnapshotTimes.Length; index++)
            {
                var target = SnapshotTimes[index];
                var steps = (int)Math.Round((target - currentTime) / deltaTime);
                for (var step = 0; step < steps; step++)
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
                currentTime = target;
                states[index] = state;
            }

            return states;
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

        private static double Median(double[] values)
        {
            var copy = (double[])values.Clone();
            Array.Sort(copy);
            return copy[copy.Length / 2];
        }

        private static double Minimum(double[] values)
        {
            var minimum = double.PositiveInfinity;
            for (var i = 0; i < values.Length; i++)
            {
                minimum = Math.Min(minimum, values[i]);
            }
            return minimum;
        }
    }
}
