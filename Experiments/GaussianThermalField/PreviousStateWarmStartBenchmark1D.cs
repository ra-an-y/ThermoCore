using System;
using System.Diagnostics;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal enum WarmStartStage1D
    {
        InitialFresh,
        DirectReuse,
        AmplitudeRefit,
        FreshFallback
    }

    internal readonly struct WarmStartTracePoint1D
    {
        public WarmStartTracePoint1D(
            double time,
            WarmStartStage1D stage,
            int countA,
            int countB,
            int countC,
            int freshMinimumTotal,
            double reuseError,
            double amplitudeRefitError,
            double finalError)
        {
            Time = time;
            Stage = stage;
            CountA = countA;
            CountB = countB;
            CountC = countC;
            FreshMinimumTotal = freshMinimumTotal;
            ReuseError = reuseError;
            AmplitudeRefitError = amplitudeRefitError;
            FinalError = finalError;
        }

        public double Time { get; }
        public WarmStartStage1D Stage { get; }
        public int CountA { get; }
        public int CountB { get; }
        public int CountC { get; }
        public int TotalCount => CountA + CountB + CountC;
        public int FreshMinimumTotal { get; }
        public int CountOverhead => TotalCount - FreshMinimumTotal;
        public double ReuseError { get; }
        public double AmplitudeRefitError { get; }
        public double FinalError { get; }
    }

    internal readonly struct WarmStartTimingPoint1D
    {
        public WarmStartTimingPoint1D(
            double time,
            double freshMedianMilliseconds,
            double warmMedianMilliseconds,
            double freshMinimumMilliseconds,
            double warmMinimumMilliseconds)
        {
            Time = time;
            FreshMedianMilliseconds = freshMedianMilliseconds;
            WarmMedianMilliseconds = warmMedianMilliseconds;
            FreshMinimumMilliseconds = freshMinimumMilliseconds;
            WarmMinimumMilliseconds = warmMinimumMilliseconds;
        }

        public double Time { get; }
        public double FreshMedianMilliseconds { get; }
        public double WarmMedianMilliseconds { get; }
        public double FreshMinimumMilliseconds { get; }
        public double WarmMinimumMilliseconds { get; }
        public double Speedup => FreshMedianMilliseconds / WarmMedianMilliseconds;
        public double ReductionFraction
            => 1.0 - WarmMedianMilliseconds / FreshMedianMilliseconds;
    }

    internal readonly struct PreviousStateWarmStartBenchmarkResult1D
    {
        private readonly WarmStartTracePoint1D[] _trace;
        private readonly WarmStartTimingPoint1D[] _timings;

        public PreviousStateWarmStartBenchmarkResult1D(
            WarmStartTracePoint1D[] trace,
            WarmStartTimingPoint1D[] timings,
            int trialCount,
            double initialFreshMedianMilliseconds,
            double steadyFreshMedianSumMilliseconds,
            double steadyWarmMedianSumMilliseconds,
            double sink)
        {
            _trace = trace;
            _timings = timings;
            TrialCount = trialCount;
            InitialFreshMedianMilliseconds = initialFreshMedianMilliseconds;
            SteadyFreshMedianSumMilliseconds = steadyFreshMedianSumMilliseconds;
            SteadyWarmMedianSumMilliseconds = steadyWarmMedianSumMilliseconds;
            Sink = sink;
        }

        public int TraceCount => _trace?.Length ?? 0;
        public int TimingCount => _timings?.Length ?? 0;
        public int TrialCount { get; }
        public double InitialFreshMedianMilliseconds { get; }
        public double SteadyFreshMedianSumMilliseconds { get; }
        public double SteadyWarmMedianSumMilliseconds { get; }
        public double SteadySpeedup
            => SteadyFreshMedianSumMilliseconds / SteadyWarmMedianSumMilliseconds;
        public double SteadyReductionFraction
            => 1.0 - SteadyWarmMedianSumMilliseconds / SteadyFreshMedianSumMilliseconds;
        public double ColdFreshTotalMilliseconds
            => InitialFreshMedianMilliseconds + SteadyFreshMedianSumMilliseconds;
        public double ColdWarmTotalMilliseconds
            => InitialFreshMedianMilliseconds + SteadyWarmMedianSumMilliseconds;
        public double ColdSpeedup => ColdFreshTotalMilliseconds / ColdWarmTotalMilliseconds;
        public double ColdReductionFraction
            => 1.0 - ColdWarmTotalMilliseconds / ColdFreshTotalMilliseconds;
        public double Sink { get; }

        public WarmStartTracePoint1D GetTracePoint(int index)
        {
            if (_trace is null || index < 0 || index >= _trace.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _trace[index];
        }

        public WarmStartTimingPoint1D GetTimingPoint(int index)
        {
            if (_timings is null || index < 0 || index >= _timings.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _timings[index];
        }

        public bool Satisfies(double globalThreshold)
        {
            if (TraceCount < 2 || TimingCount != TraceCount - 1 || TrialCount < 3
                || !PositiveFinite(InitialFreshMedianMilliseconds)
                || !PositiveFinite(SteadyFreshMedianSumMilliseconds)
                || !PositiveFinite(SteadyWarmMedianSumMilliseconds)
                || double.IsNaN(Sink) || double.IsInfinity(Sink))
            {
                return false;
            }

            for (var i = 0; i < TraceCount; i++)
            {
                var point = _trace[i];
                if (point.TotalCount <= 0
                    || point.FreshMinimumTotal <= 0
                    || point.FinalError > globalThreshold
                    || double.IsNaN(point.FinalError)
                    || double.IsInfinity(point.FinalError))
                {
                    return false;
                }
            }

            for (var i = 0; i < TimingCount; i++)
            {
                var point = _timings[i];
                if (!PositiveFinite(point.FreshMedianMilliseconds)
                    || !PositiveFinite(point.WarmMedianMilliseconds)
                    || !PositiveFinite(point.FreshMinimumMilliseconds)
                    || !PositiveFinite(point.WarmMinimumMilliseconds))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PositiveFinite(double value)
            => value > 0.0 && !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// Runtime benchmark for temporal coherence of the downstream Gaussian
    /// representation. A previous representation is first checked unchanged,
    /// then with fixed-support amplitude-only refitting, and only falls back to
    /// the validated proposal-bounded fresh sparse search if needed.
    ///
    /// Proposal inference is treated as already available; the timed comparison
    /// is the representation update itself (validation/refit/fallback).
    /// </summary>
    internal static class PreviousStateWarmStartBenchmark1D
    {
        private const int ModeCount = 32;
        private const int SampleCount = 401;
        private const int TrialCount = 7;
        private const double GlobalErrorThreshold = 5e-3;
        private const double PeakZeroGuard = 5e-3;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

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

        public static PreviousStateWarmStartBenchmarkResult1D Evaluate()
        {
            var states = BuildSnapshotStates();
            var trace = new WarmStartTracePoint1D[states.Length];
            var previousInputs = new WarmRepresentation1D[states.Length - 1];

            var initial = FreshBuild(states[0], 0);
            trace[0] = new WarmStartTracePoint1D(
                SnapshotTimes[0],
                WarmStartStage1D.InitialFresh,
                initial.CountA,
                initial.CountB,
                initial.CountC,
                initial.TotalCount,
                double.NaN,
                double.NaN,
                initial.Error);

            var previous = initial;
            for (var index = 1; index < states.Length; index++)
            {
                previousInputs[index - 1] = previous;
                var freshReference = FreshBuild(states[index], index);
                var update = WarmUpdate(states[index], index, previous);

                trace[index] = new WarmStartTracePoint1D(
                    SnapshotTimes[index],
                    update.Stage,
                    update.Representation.CountA,
                    update.Representation.CountB,
                    update.Representation.CountC,
                    freshReference.TotalCount,
                    update.ReuseError,
                    update.AmplitudeError,
                    update.Representation.Error);

                previous = update.Representation;
            }

            // Warm up both update paths before measurements.
            FreshBuild(states[0], 0);
            WarmUpdate(states[1], 1, previousInputs[0]);

            var initialFreshSamples = new double[TrialCount];
            for (var trial = 0; trial < TrialCount; trial++)
            {
                initialFreshSamples[trial] = MeasureMilliseconds(
                    () => Consume(FreshBuild(states[0], 0)));
            }
            var initialFreshMedian = Median(initialFreshSamples);

            var timings = new WarmStartTimingPoint1D[states.Length - 1];
            var steadyFreshSum = 0.0;
            var steadyWarmSum = 0.0;

            for (var index = 1; index < states.Length; index++)
            {
                var fresh = new double[TrialCount];
                var warm = new double[TrialCount];
                var previousInput = previousInputs[index - 1];
                var capturedIndex = index;

                for (var trial = 0; trial < TrialCount; trial++)
                {
                    if ((trial & 1) == 0)
                    {
                        fresh[trial] = MeasureMilliseconds(
                            () => Consume(FreshBuild(states[capturedIndex], capturedIndex)));
                        warm[trial] = MeasureMilliseconds(
                            () => Consume(WarmUpdate(
                                states[capturedIndex],
                                capturedIndex,
                                previousInput).Representation));
                    }
                    else
                    {
                        warm[trial] = MeasureMilliseconds(
                            () => Consume(WarmUpdate(
                                states[capturedIndex],
                                capturedIndex,
                                previousInput).Representation));
                        fresh[trial] = MeasureMilliseconds(
                            () => Consume(FreshBuild(states[capturedIndex], capturedIndex)));
                    }
                }

                var freshMedian = Median(fresh);
                var warmMedian = Median(warm);
                steadyFreshSum += freshMedian;
                steadyWarmSum += warmMedian;

                timings[index - 1] = new WarmStartTimingPoint1D(
                    SnapshotTimes[index],
                    freshMedian,
                    warmMedian,
                    Minimum(fresh),
                    Minimum(warm));
            }

            return new PreviousStateWarmStartBenchmarkResult1D(
                trace,
                timings,
                TrialCount,
                initialFreshMedian,
                steadyFreshSum,
                steadyWarmSum,
                _sink);
        }

        private static WarmUpdateResult1D WarmUpdate(
            in ThreeLayerCoupledState1D state,
            int snapshotIndex,
            in WarmRepresentation1D previous)
        {
            var norms = MeasureNormsAndPeaks(state);
            var reuseError = DirectGlobalError(state, previous.A, previous.B, previous.C);
            if (ZeroGuardsPass(previous, norms) && reuseError <= GlobalErrorThreshold)
            {
                return new WarmUpdateResult1D(
                    new WarmRepresentation1D(
                        previous.A,
                        previous.B,
                        previous.C,
                        reuseError),
                    WarmStartStage1D.DirectReuse,
                    reuseError,
                    double.NaN);
            }

            try
            {
                var a = previous.A.HasValue
                    ? FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                        state.StateA, 0.60, previous.A.Value).Mixture
                    : (GaussianMixture1D?)null;
                var b = previous.B.HasValue
                    ? FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                        state.StateB, 0.35, previous.B.Value).Mixture
                    : (GaussianMixture1D?)null;
                var c = previous.C.HasValue
                    ? FixedBasisGaussianWarmStart1D.RefitAmplitudes(
                        state.StateC, 0.60, previous.C.Value).Mixture
                    : (GaussianMixture1D?)null;

                var amplitudeRepresentation = new WarmRepresentation1D(a, b, c, 0.0);
                var amplitudeError = DirectGlobalError(state, a, b, c);
                amplitudeRepresentation = new WarmRepresentation1D(a, b, c, amplitudeError);

                if (ZeroGuardsPass(amplitudeRepresentation, norms)
                    && amplitudeError <= GlobalErrorThreshold)
                {
                    return new WarmUpdateResult1D(
                        amplitudeRepresentation,
                        WarmStartStage1D.AmplitudeRefit,
                        reuseError,
                        amplitudeError);
                }

                var fallback = FreshBuild(state, snapshotIndex);
                return new WarmUpdateResult1D(
                    fallback,
                    WarmStartStage1D.FreshFallback,
                    reuseError,
                    amplitudeError);
            }
            catch (InvalidOperationException)
            {
                var fallback = FreshBuild(state, snapshotIndex);
                return new WarmUpdateResult1D(
                    fallback,
                    WarmStartStage1D.FreshFallback,
                    reuseError,
                    double.PositiveInfinity);
            }
        }

        private static WarmRepresentation1D FreshBuild(
            in ThreeLayerCoupledState1D state,
            int snapshotIndex)
        {
            var maximumA = ProposalCounts[snapshotIndex, 0];
            var maximumB = ProposalCounts[snapshotIndex, 1];
            var maximumC = ProposalCounts[snapshotIndex, 2];

            var fitsA = maximumA > 0
                ? ConstrainedGaussianSparseFitter1D.FitSequence(state.StateA, 0.60, maximumA)
                : Array.Empty<ConstrainedGaussianSparseFitResult1D>();
            var fitsB = maximumB > 0
                ? ConstrainedGaussianSparseFitter1D.FitSequence(state.StateB, 0.35, maximumB)
                : Array.Empty<ConstrainedGaussianSparseFitResult1D>();
            var fitsC = maximumC > 0
                ? ConstrainedGaussianSparseFitter1D.FitSequence(state.StateC, 0.60, maximumC)
                : Array.Empty<ConstrainedGaussianSparseFitResult1D>();

            var norm = MeasureNormsAndPeaks(state);
            var bestA = -1;
            var bestB = -1;
            var bestC = -1;
            var bestTotal = int.MaxValue;
            var bestFormulaError = double.PositiveInfinity;

            for (var countA = 0; countA <= maximumA; countA++)
            {
                if (countA == 0 && norm.PeakA > PeakZeroGuard)
                {
                    continue;
                }

                for (var countB = 0; countB <= maximumB; countB++)
                {
                    if (countB == 0 && norm.PeakB > PeakZeroGuard)
                    {
                        continue;
                    }

                    for (var countC = 0; countC <= maximumC; countC++)
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

                        var eA = countA == 0 ? 1.0 : fitsA[countA - 1].RelativeError;
                        var eB = countB == 0 ? 1.0 : fitsB[countB - 1].RelativeError;
                        var eC = countC == 0 ? 1.0 : fitsC[countC - 1].RelativeError;
                        var global = Math.Sqrt(
                            Square(norm.WeightA * eA)
                            + Square(norm.WeightB * eB)
                            + Square(norm.WeightC * eC));

                        if (global > GlobalErrorThreshold)
                        {
                            continue;
                        }

                        if (total < bestTotal
                            || (total == bestTotal && global < bestFormulaError))
                        {
                            bestTotal = total;
                            bestFormulaError = global;
                            bestA = countA;
                            bestB = countB;
                            bestC = countC;
                        }
                    }
                }
            }

            if (bestTotal == int.MaxValue)
            {
                throw new InvalidOperationException(
                    $"No verified fresh allocation found at t={SnapshotTimes[snapshotIndex]:F2}.");
            }

            GaussianMixture1D? a = bestA == 0 ? null : fitsA[bestA - 1].Mixture;
            GaussianMixture1D? b = bestB == 0 ? null : fitsB[bestB - 1].Mixture;
            GaussianMixture1D? c = bestC == 0 ? null : fitsC[bestC - 1].Mixture;
            var direct = DirectGlobalError(state, a, b, c);

            if (direct > GlobalErrorThreshold)
            {
                throw new InvalidOperationException(
                    $"Fresh allocation direct error exceeded threshold at t={SnapshotTimes[snapshotIndex]:F2}.");
            }

            return new WarmRepresentation1D(a, b, c, direct);
        }

        private static bool ZeroGuardsPass(
            in WarmRepresentation1D representation,
            in NormAndPeak1D norm)
        {
            return (representation.CountA > 0 || norm.PeakA <= PeakZeroGuard)
                && (representation.CountB > 0 || norm.PeakB <= PeakZeroGuard)
                && (representation.CountC > 0 || norm.PeakC <= PeakZeroGuard);
        }

        private static NormAndPeak1D MeasureNormsAndPeaks(
            in ThreeLayerCoupledState1D state)
        {
            var squaredA = RegionSquaredNorm(state.StateA, 0.60, out var peakA);
            var squaredB = RegionSquaredNorm(state.StateB, 0.35, out var peakB);
            var squaredC = RegionSquaredNorm(state.StateC, 0.60, out var peakC);
            var totalSquared = squaredA + squaredB + squaredC;
            var globalNorm = Math.Sqrt(Math.Max(totalSquared, 1e-30));
            var globalPeak = Math.Max(peakA, Math.Max(peakB, peakC));
            var safePeak = Math.Max(globalPeak, 1e-30);

            return new NormAndPeak1D(
                Math.Sqrt(squaredA) / globalNorm,
                Math.Sqrt(squaredB) / globalNorm,
                Math.Sqrt(squaredC) / globalNorm,
                peakA / safePeak,
                peakB / safePeak,
                peakC / safePeak);
        }

        private static double RegionSquaredNorm(
            in FiniteLayerReducedState1D state,
            double length,
            out double peak)
        {
            var dx = length / SampleCount;
            var sum = 0.0;
            peak = 0.0;
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
            GaussianMixture1D? a,
            GaussianMixture1D? b,
            GaussianMixture1D? c)
        {
            var errorSquared = 0.0;
            var referenceSquared = 0.0;
            AccumulateError(state.StateA, a, 0.60, ref errorSquared, ref referenceSquared);
            AccumulateError(state.StateB, b, 0.35, ref errorSquared, ref referenceSquared);
            AccumulateError(state.StateC, c, 0.60, ref errorSquared, ref referenceSquared);
            return Math.Sqrt(errorSquared / Math.Max(referenceSquared, 1e-30));
        }

        private static void AccumulateError(
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

        private static void Consume(in WarmRepresentation1D representation)
        {
            _sink += representation.Error + representation.TotalCount;
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
            var result = double.PositiveInfinity;
            for (var i = 0; i < values.Length; i++)
            {
                result = Math.Min(result, values[i]);
            }
            return result;
        }

        private static double Square(double value) => value * value;

        private readonly struct WarmRepresentation1D
        {
            public WarmRepresentation1D(
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
            public int CountA => A?.Count ?? 0;
            public int CountB => B?.Count ?? 0;
            public int CountC => C?.Count ?? 0;
            public int TotalCount => CountA + CountB + CountC;
        }

        private readonly struct WarmUpdateResult1D
        {
            public WarmUpdateResult1D(
                WarmRepresentation1D representation,
                WarmStartStage1D stage,
                double reuseError,
                double amplitudeError)
            {
                Representation = representation;
                Stage = stage;
                ReuseError = reuseError;
                AmplitudeError = amplitudeError;
            }

            public WarmRepresentation1D Representation { get; }
            public WarmStartStage1D Stage { get; }
            public double ReuseError { get; }
            public double AmplitudeError { get; }
        }

        private readonly struct NormAndPeak1D
        {
            public NormAndPeak1D(
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
