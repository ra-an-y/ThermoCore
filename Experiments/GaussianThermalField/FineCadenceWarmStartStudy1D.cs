using System;
using System.Diagnostics;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal readonly struct FineCadenceWarmStartPoint1D
    {
        public FineCadenceWarmStartPoint1D(
            double deltaTime,
            double reuseError,
            double amplitudeError,
            double freshError,
            bool reusePass,
            bool amplitudePass,
            bool freshPass,
            double freshMedianMilliseconds,
            double warmMedianMilliseconds)
        {
            DeltaTime = deltaTime;
            ReuseError = reuseError;
            AmplitudeError = amplitudeError;
            FreshError = freshError;
            ReusePass = reusePass;
            AmplitudePass = amplitudePass;
            FreshPass = freshPass;
            FreshMedianMilliseconds = freshMedianMilliseconds;
            WarmMedianMilliseconds = warmMedianMilliseconds;
        }

        public double DeltaTime { get; }
        public double ReuseError { get; }
        public double AmplitudeError { get; }
        public double FreshError { get; }
        public bool ReusePass { get; }
        public bool AmplitudePass { get; }
        public bool FreshPass { get; }
        public bool WarmPass => ReusePass || AmplitudePass;
        public double FreshMedianMilliseconds { get; }
        public double WarmMedianMilliseconds { get; }
        public double Speedup => FreshMedianMilliseconds / WarmMedianMilliseconds;
        public double ReductionFraction
            => 1.0 - WarmMedianMilliseconds / FreshMedianMilliseconds;
    }

    internal readonly struct FineCadenceWarmStartStudyResult1D
    {
        private readonly FineCadenceWarmStartPoint1D[] _points;

        public FineCadenceWarmStartStudyResult1D(
            FineCadenceWarmStartPoint1D[] points,
            int trialCount,
            double sink)
        {
            _points = points;
            TrialCount = trialCount;
            Sink = sink;
        }

        public int Count => _points?.Length ?? 0;
        public int TrialCount { get; }
        public double Sink { get; }

        public FineCadenceWarmStartPoint1D GetPoint(int index)
        {
            if (_points is null || index < 0 || index >= _points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _points[index];
        }

        public bool SatisfiesDataIntegrity()
        {
            if (Count == 0 || TrialCount < 3 || double.IsNaN(Sink) || double.IsInfinity(Sink))
            {
                return false;
            }

            for (var i = 0; i < Count; i++)
            {
                var point = _points[i];
                if (point.DeltaTime <= 0.0
                    || !FiniteNonNegative(point.ReuseError)
                    || !FiniteNonNegative(point.AmplitudeError)
                    || !FiniteNonNegative(point.FreshError)
                    || !PositiveFinite(point.FreshMedianMilliseconds)
                    || !PositiveFinite(point.WarmMedianMilliseconds))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool FiniteNonNegative(double value)
            => value >= 0.0 && !double.IsNaN(value) && !double.IsInfinity(value);

        private static bool PositiveFinite(double value)
            => value > 0.0 && !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// One-step temporal-coherence study around t=0.60 s using the verified
    /// 2/3/2 Gaussian allocation. It asks how far into the future the previous
    /// support remains useful before a sparse support re-search is required.
    ///
    /// Fresh baseline: rebuild the same 2/3/2 budget from scratch.
    /// Warm path: validate unchanged mixture; if needed, re-solve amplitudes on
    /// the previous support. No fresh fallback is hidden inside the warm timing.
    /// </summary>
    internal static class FineCadenceWarmStartStudy1D
    {
        private const int ModeCount = 32;
        private const int SampleCount = 401;
        private const int TrialCount = 7;
        private const double BaseTime = 0.60;
        private const double SolverDeltaTime = 0.002;
        private const double Threshold = 5e-3;

        private static readonly double[] UpdateIntervals =
        {
            0.002, 0.004, 0.010, 0.016, 0.032, 0.050, 0.100, 0.200
        };

        private static double _sink;

        public static FineCadenceWarmStartStudyResult1D Evaluate()
        {
            var baseState = BuildStateAt(BaseTime);
            var previous = BuildFreshSameBudget(baseState);
            var points = new FineCadenceWarmStartPoint1D[UpdateIntervals.Length];

            // Warmup.
            var warmState = BuildStateAt(BaseTime + UpdateIntervals[0]);
            RunWarm(warmState, previous);
            BuildFreshSameBudget(warmState);

            for (var index = 0; index < UpdateIntervals.Length; index++)
            {
                var interval = UpdateIntervals[index];
                var state = BuildStateAt(BaseTime + interval);

                var reuseError = DirectGlobalError(state, previous.A, previous.B, previous.C);
                var amplitude = RefitSameSupport(state, previous);
                var amplitudeError = amplitude.Error;
                var fresh = BuildFreshSameBudget(state);
                var freshError = fresh.Error;

                var reusePass = reuseError <= Threshold;
                var amplitudePass = amplitudeError <= Threshold;
                var freshPass = freshError <= Threshold;

                var freshSamples = new double[TrialCount];
                var warmSamples = new double[TrialCount];
                for (var trial = 0; trial < TrialCount; trial++)
                {
                    if ((trial & 1) == 0)
                    {
                        freshSamples[trial] = MeasureMilliseconds(
                            () => Consume(BuildFreshSameBudget(state)));
                        warmSamples[trial] = MeasureMilliseconds(
                            () => Consume(RunWarm(state, previous)));
                    }
                    else
                    {
                        warmSamples[trial] = MeasureMilliseconds(
                            () => Consume(RunWarm(state, previous)));
                        freshSamples[trial] = MeasureMilliseconds(
                            () => Consume(BuildFreshSameBudget(state)));
                    }
                }

                points[index] = new FineCadenceWarmStartPoint1D(
                    interval,
                    reuseError,
                    amplitudeError,
                    freshError,
                    reusePass,
                    amplitudePass,
                    freshPass,
                    Median(freshSamples),
                    Median(warmSamples));
            }

            return new FineCadenceWarmStartStudyResult1D(points, TrialCount, _sink);
        }

        private static Representation RunWarm(
            in ThreeLayerCoupledState1D state,
            in Representation previous)
        {
            var reuseError = DirectGlobalError(state, previous.A, previous.B, previous.C);
            if (reuseError <= Threshold)
            {
                return new Representation(previous.A, previous.B, previous.C, reuseError);
            }
            return RefitSameSupport(state, previous);
        }

        private static Representation RefitSameSupport(
            in ThreeLayerCoupledState1D state,
            in Representation previous)
        {
            var a = FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                state.StateA, 0.60, previous.A.Value).Mixture;
            var b = FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                state.StateB, 0.35, previous.B.Value).Mixture;
            var c = FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                state.StateC, 0.60, previous.C.Value).Mixture;
            var error = DirectGlobalError(state, a, b, c);
            return new Representation(a, b, c, error);
        }

        private static Representation BuildFreshSameBudget(
            in ThreeLayerCoupledState1D state)
        {
            var a = ConstrainedGaussianSparseFitter1D.FitSequence(state.StateA, 0.60, 2)[1].Mixture;
            var b = ConstrainedGaussianSparseFitter1D.FitSequence(state.StateB, 0.35, 3)[2].Mixture;
            var c = ConstrainedGaussianSparseFitter1D.FitSequence(state.StateC, 0.60, 2)[1].Mixture;
            var error = DirectGlobalError(state, a, b, c);
            return new Representation(a, b, c, error);
        }

        private static ThreeLayerCoupledState1D BuildStateAt(double targetTime)
        {
            var materialA = new ThermalMaterial1D(0.40, 2.0);
            var materialB = new ThermalMaterial1D(0.06, 1.2);
            var materialC = new ThermalMaterial1D(0.20, 1.6);
            const double lengthA = 0.60;
            const double lengthB = 0.35;
            const double lengthC = 0.60;

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

            var steps = (int)Math.Round(targetTime / SolverDeltaTime);
            for (var step = 0; step < steps; step++)
            {
                state = ThreeLayerCoupledEvolution1D.Advance(
                    state,
                    SolverDeltaTime,
                    lengthA,
                    lengthB,
                    lengthC,
                    materialA,
                    materialB,
                    materialC).State;
            }
            return state;
        }

        private static double DirectGlobalError(
            in ThreeLayerCoupledState1D state,
            GaussianMixture1D? a,
            GaussianMixture1D? b,
            GaussianMixture1D? c)
        {
            var errorSquared = 0.0;
            var referenceSquared = 0.0;
            Accumulate(state.StateA, a, 0.60, ref errorSquared, ref referenceSquared);
            Accumulate(state.StateB, b, 0.35, ref errorSquared, ref referenceSquared);
            Accumulate(state.StateC, c, 0.60, ref errorSquared, ref referenceSquared);
            return Math.Sqrt(errorSquared / Math.Max(referenceSquared, 1e-30));
        }

        private static void Accumulate(
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
                var represented = mixture.HasValue ? mixture.Value.Evaluate(x) : 0.0;
                var difference = represented - reference;
                errorSquared += dx * difference * difference;
                referenceSquared += dx * reference * reference;
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

        private static double MeasureMilliseconds(Action action)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var start = Stopwatch.GetTimestamp();
            action();
            var end = Stopwatch.GetTimestamp();
            return (end - start) * 1000.0 / Stopwatch.Frequency;
        }

        private static void Consume(in Representation representation)
        {
            _sink += representation.Error + representation.TotalCount;
        }

        private static double Median(double[] values)
        {
            var copy = (double[])values.Clone();
            Array.Sort(copy);
            return copy[copy.Length / 2];
        }

        private readonly struct Representation
        {
            public Representation(
                GaussianMixture1D? a,
                GaussianMixture1D? b,
                GaussianMixture1D? c,
                double error)
            {
                A = a;
                B = b;
                C = c;
                Error = error;
            }

            public GaussianMixture1D? A { get; }
            public GaussianMixture1D? B { get; }
            public GaussianMixture1D? C { get; }
            public double Error { get; }
            public int TotalCount => (A?.Count ?? 0) + (B?.Count ?? 0) + (C?.Count ?? 0);
        }
    }
}
