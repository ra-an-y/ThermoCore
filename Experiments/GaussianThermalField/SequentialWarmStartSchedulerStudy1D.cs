using System;
using System.Diagnostics;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal readonly struct SequentialWarmStartSchedulerResult1D
    {
        public SequentialWarmStartSchedulerResult1D(
            int updateCount,
            int reuseCount,
            int amplitudeRefitCount,
            int freshFallbackCount,
            double maximumFinalError,
            int trialCount,
            double freshMedianMilliseconds,
            double warmMedianMilliseconds,
            double sink)
        {
            UpdateCount = updateCount;
            ReuseCount = reuseCount;
            AmplitudeRefitCount = amplitudeRefitCount;
            FreshFallbackCount = freshFallbackCount;
            MaximumFinalError = maximumFinalError;
            TrialCount = trialCount;
            FreshMedianMilliseconds = freshMedianMilliseconds;
            WarmMedianMilliseconds = warmMedianMilliseconds;
            Sink = sink;
        }

        public int UpdateCount { get; }
        public int ReuseCount { get; }
        public int AmplitudeRefitCount { get; }
        public int FreshFallbackCount { get; }
        public double MaximumFinalError { get; }
        public int TrialCount { get; }
        public double FreshMedianMilliseconds { get; }
        public double WarmMedianMilliseconds { get; }
        public double FreshMeanPerUpdateMilliseconds => FreshMedianMilliseconds / UpdateCount;
        public double WarmMeanPerUpdateMilliseconds => WarmMedianMilliseconds / UpdateCount;
        public double Speedup => FreshMedianMilliseconds / WarmMedianMilliseconds;
        public double ReductionFraction => 1.0 - WarmMedianMilliseconds / FreshMedianMilliseconds;
        public double Sink { get; }

        public bool Satisfies(double threshold)
        {
            return UpdateCount > 0
                && ReuseCount + AmplitudeRefitCount + FreshFallbackCount == UpdateCount
                && MaximumFinalError <= threshold
                && TrialCount >= 3
                && PositiveFinite(FreshMedianMilliseconds)
                && PositiveFinite(WarmMedianMilliseconds)
                && !double.IsNaN(Sink)
                && !double.IsInfinity(Sink);
        }

        private static bool PositiveFinite(double value)
            => value > 0.0 && !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// Stateful 16-ms scheduler benchmark over t=0.60..0.792 s.
    /// The verified 2/3/2 budget is held fixed so this experiment isolates
    /// temporal support reuse rather than budget adaptation.
    ///
    /// Each update performs:
    ///   direct reuse validation -> fixed-support amplitude refit -> fresh
    ///   same-budget sparse rebuild fallback.
    /// The accepted representation is carried into the next update.
    /// </summary>
    internal static class SequentialWarmStartSchedulerStudy1D
    {
        private const int ModeCount = 32;
        private const int SampleCount = 401;
        private const int TrialCount = 5;
        private const double StartTime = 0.60;
        private const double SolverDeltaTime = 0.002;
        private const double UpdateInterval = 0.016;
        private const int UpdateCount = 12;
        private const double Threshold = 5e-3;

        private static double _sink;

        public static SequentialWarmStartSchedulerResult1D Evaluate()
        {
            var states = BuildStates();
            var initial = BuildFreshSameBudget(states[0]);

            var traceRepresentation = initial;
            var reuseCount = 0;
            var amplitudeCount = 0;
            var fallbackCount = 0;
            var maximumError = 0.0;

            for (var index = 1; index < states.Length; index++)
            {
                var update = WarmUpdate(states[index], traceRepresentation);
                traceRepresentation = update.Representation;
                maximumError = Math.Max(maximumError, traceRepresentation.Error);
                switch (update.Stage)
                {
                    case Stage.Reuse:
                        reuseCount++;
                        break;
                    case Stage.Amplitude:
                        amplitudeCount++;
                        break;
                    case Stage.Fallback:
                        fallbackCount++;
                        break;
                }

                // The same-budget fresh reference must itself remain valid in
                // this bounded scheduler window.
                var freshReference = BuildFreshSameBudget(states[index]);
                if (freshReference.Error > Threshold)
                {
                    throw new InvalidOperationException(
                        $"Fixed 2/3/2 baseline ceased to satisfy threshold at update {index}.");
                }
            }

            // Warmup both complete trajectories.
            RunFreshTrajectory(states);
            RunWarmTrajectory(states, initial);

            var freshSamples = new double[TrialCount];
            var warmSamples = new double[TrialCount];
            for (var trial = 0; trial < TrialCount; trial++)
            {
                if ((trial & 1) == 0)
                {
                    freshSamples[trial] = MeasureMilliseconds(() => RunFreshTrajectory(states));
                    warmSamples[trial] = MeasureMilliseconds(() => RunWarmTrajectory(states, initial));
                }
                else
                {
                    warmSamples[trial] = MeasureMilliseconds(() => RunWarmTrajectory(states, initial));
                    freshSamples[trial] = MeasureMilliseconds(() => RunFreshTrajectory(states));
                }
            }

            return new SequentialWarmStartSchedulerResult1D(
                UpdateCount,
                reuseCount,
                amplitudeCount,
                fallbackCount,
                maximumError,
                TrialCount,
                Median(freshSamples),
                Median(warmSamples),
                _sink);
        }

        private static void RunFreshTrajectory(ThreeLayerCoupledState1D[] states)
        {
            for (var index = 1; index < states.Length; index++)
            {
                var representation = BuildFreshSameBudget(states[index]);
                _sink += representation.Error + representation.TotalCount;
            }
        }

        private static void RunWarmTrajectory(
            ThreeLayerCoupledState1D[] states,
            Representation initial)
        {
            var representation = initial;
            for (var index = 1; index < states.Length; index++)
            {
                representation = WarmUpdate(states[index], representation).Representation;
                _sink += representation.Error + representation.TotalCount;
            }
        }

        private static UpdateResult WarmUpdate(
            in ThreeLayerCoupledState1D state,
            in Representation previous)
        {
            var reuseError = DirectGlobalError(state, previous.A, previous.B, previous.C);
            if (reuseError <= Threshold)
            {
                return new UpdateResult(
                    new Representation(previous.A, previous.B, previous.C, reuseError),
                    Stage.Reuse);
            }

            var amplitude = RefitSameSupport(state, previous);
            if (amplitude.Error <= Threshold)
            {
                return new UpdateResult(amplitude, Stage.Amplitude);
            }

            return new UpdateResult(BuildFreshSameBudget(state), Stage.Fallback);
        }

        private static Representation RefitSameSupport(
            in ThreeLayerCoupledState1D state,
            in Representation previous)
        {
            if (!previous.A.HasValue || !previous.B.HasValue || !previous.C.HasValue)
            {
                throw new InvalidOperationException("Sequential fixed-budget representation is incomplete.");
            }

            var a = FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                state.StateA, 0.60, previous.A.Value).Mixture;
            var b = FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                state.StateB, 0.35, previous.B.Value).Mixture;
            var c = FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                state.StateC, 0.60, previous.C.Value).Mixture;
            return new Representation(a, b, c, DirectGlobalError(state, a, b, c));
        }

        private static Representation BuildFreshSameBudget(
            in ThreeLayerCoupledState1D state)
        {
            var a = ConstrainedGaussianSparseFitter1D.FitSequence(state.StateA, 0.60, 2)[1].Mixture;
            var b = ConstrainedGaussianSparseFitter1D.FitSequence(state.StateB, 0.35, 3)[2].Mixture;
            var c = ConstrainedGaussianSparseFitter1D.FitSequence(state.StateC, 0.60, 2)[1].Mixture;
            return new Representation(a, b, c, DirectGlobalError(state, a, b, c));
        }

        private static ThreeLayerCoupledState1D[] BuildStates()
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

            var startSteps = (int)Math.Round(StartTime / SolverDeltaTime);
            for (var step = 0; step < startSteps; step++)
            {
                state = ThreeLayerCoupledEvolution1D.Advance(
                    state, SolverDeltaTime,
                    lengthA, lengthB, lengthC,
                    materialA, materialB, materialC).State;
            }

            var states = new ThreeLayerCoupledState1D[UpdateCount + 1];
            states[0] = state;
            var stepsPerUpdate = (int)Math.Round(UpdateInterval / SolverDeltaTime);
            for (var update = 1; update <= UpdateCount; update++)
            {
                for (var step = 0; step < stepsPerUpdate; step++)
                {
                    state = ThreeLayerCoupledEvolution1D.Advance(
                        state, SolverDeltaTime,
                        lengthA, lengthB, lengthC,
                        materialA, materialB, materialC).State;
                }
                states[update] = state;
            }
            return states;
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

        private static double Median(double[] values)
        {
            var copy = (double[])values.Clone();
            Array.Sort(copy);
            return copy[copy.Length / 2];
        }

        private enum Stage
        {
            Reuse,
            Amplitude,
            Fallback
        }

        private readonly struct UpdateResult
        {
            public UpdateResult(Representation representation, Stage stage)
            {
                Representation = representation;
                Stage = stage;
            }

            public Representation Representation { get; }
            public Stage Stage { get; }
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
