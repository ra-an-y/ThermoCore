using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct StateComplexityMetricSample1D
    {
        public StateComplexityMetricSample1D(
            double time,
            char region,
            int requiredGaussianCount,
            double l2Contribution,
            double peakContribution,
            double rmsModeIndex,
            double quarticModeIndex,
            double highModeEnergyFraction,
            double spectralEntropy,
            double effectiveModeCount,
            double normalizedGradientScore,
            double normalizedCurvatureScore)
        {
            Time = time;
            Region = region;
            RequiredGaussianCount = requiredGaussianCount;
            L2Contribution = l2Contribution;
            PeakContribution = peakContribution;
            RmsModeIndex = rmsModeIndex;
            QuarticModeIndex = quarticModeIndex;
            HighModeEnergyFraction = highModeEnergyFraction;
            SpectralEntropy = spectralEntropy;
            EffectiveModeCount = effectiveModeCount;
            NormalizedGradientScore = normalizedGradientScore;
            NormalizedCurvatureScore = normalizedCurvatureScore;
        }

        public double Time { get; }
        public char Region { get; }
        public int RequiredGaussianCount { get; }
        public double L2Contribution { get; }
        public double PeakContribution { get; }
        public double RmsModeIndex { get; }
        public double QuarticModeIndex { get; }
        public double HighModeEnergyFraction { get; }
        public double SpectralEntropy { get; }
        public double EffectiveModeCount { get; }
        public double NormalizedGradientScore { get; }
        public double NormalizedCurvatureScore { get; }
    }

    public readonly struct StateComplexityMetricCorrelation1D
    {
        public StateComplexityMetricCorrelation1D(
            string name,
            double pearson,
            double spearman)
        {
            Name = name;
            Pearson = pearson;
            Spearman = spearman;
        }

        public string Name { get; }
        public double Pearson { get; }
        public double Spearman { get; }
    }

    public readonly struct StateComplexityMetricStudyResult1D
    {
        private readonly StateComplexityMetricSample1D[] _samples;
        private readonly StateComplexityMetricCorrelation1D[] _correlations;

        public StateComplexityMetricStudyResult1D(
            StateComplexityMetricSample1D[] samples,
            StateComplexityMetricCorrelation1D[] correlations,
            string bestPearsonMetric,
            double bestAbsolutePearson,
            string bestSpearmanMetric,
            double bestAbsoluteSpearman)
        {
            _samples = samples;
            _correlations = correlations;
            BestPearsonMetric = bestPearsonMetric;
            BestAbsolutePearson = bestAbsolutePearson;
            BestSpearmanMetric = bestSpearmanMetric;
            BestAbsoluteSpearman = bestAbsoluteSpearman;
        }

        public int SampleCount => _samples?.Length ?? 0;
        public int CorrelationCount => _correlations?.Length ?? 0;
        public string BestPearsonMetric { get; }
        public double BestAbsolutePearson { get; }
        public string BestSpearmanMetric { get; }
        public double BestAbsoluteSpearman { get; }

        public StateComplexityMetricSample1D GetSample(int index)
        {
            if (_samples is null || index < 0 || index >= _samples.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _samples[index];
        }

        public StateComplexityMetricCorrelation1D GetCorrelation(int index)
        {
            if (_correlations is null || index < 0 || index >= _correlations.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _correlations[index];
        }

        public bool SatisfiesDataIntegrity()
        {
            if (SampleCount < 8 || CorrelationCount < 5)
            {
                return false;
            }

            for (var i = 0; i < SampleCount; i++)
            {
                var sample = _samples[i];
                if (sample.RequiredGaussianCount <= 0
                    || !IsFinite(sample.L2Contribution)
                    || !IsFinite(sample.PeakContribution)
                    || !IsFinite(sample.RmsModeIndex)
                    || !IsFinite(sample.QuarticModeIndex)
                    || !IsFinite(sample.HighModeEnergyFraction)
                    || !IsFinite(sample.SpectralEntropy)
                    || !IsFinite(sample.EffectiveModeCount)
                    || !IsFinite(sample.NormalizedGradientScore)
                    || !IsFinite(sample.NormalizedCurvatureScore))
                {
                    return false;
                }
            }

            return IsFinite(BestAbsolutePearson)
                && IsFinite(BestAbsoluteSpearman);
        }

        private static bool IsFinite(double value)
            => !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// Exploratory checkpoint toward a direct budget formula.
    ///
    /// It asks which dimensionless quantities derived only from the current
    /// modal state best correlate with the observed number of Gaussians needed
    /// to reach a fixed regional representation-error threshold.
    ///
    /// This is intentionally a correlation study, not a claim that any metric
    /// already defines a universal predictor.
    /// </summary>
    public static class StateComplexityMetricStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumGaussianCount = 8;
        private const double LocalFitThreshold = 5e-3;
        private const double L2OmissionThreshold = 1e-3;
        private const double PeakOmissionThreshold = 5e-3;
        private const int FieldSampleCount = 401;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

        public static StateComplexityMetricStudyResult1D Evaluate()
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

            var samples = new List<StateComplexityMetricSample1D>();
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

                var contribution = MeasureGlobalContributions(
                    state,
                    lengthA,
                    lengthB,
                    lengthC);

                AddRegionSample(
                    samples,
                    targetTime,
                    'A',
                    state.StateA,
                    lengthA,
                    contribution.L2A,
                    contribution.PeakA);
                AddRegionSample(
                    samples,
                    targetTime,
                    'B',
                    state.StateB,
                    lengthB,
                    contribution.L2B,
                    contribution.PeakB);
                AddRegionSample(
                    samples,
                    targetTime,
                    'C',
                    state.StateC,
                    lengthC,
                    contribution.L2C,
                    contribution.PeakC);
            }

            var correlations = BuildCorrelations(samples);
            var bestPearsonMetric = string.Empty;
            var bestAbsolutePearson = -1.0;
            var bestSpearmanMetric = string.Empty;
            var bestAbsoluteSpearman = -1.0;

            for (var i = 0; i < correlations.Length; i++)
            {
                var correlation = correlations[i];
                var absPearson = Math.Abs(correlation.Pearson);
                var absSpearman = Math.Abs(correlation.Spearman);

                if (absPearson > bestAbsolutePearson)
                {
                    bestAbsolutePearson = absPearson;
                    bestPearsonMetric = correlation.Name;
                }

                if (absSpearman > bestAbsoluteSpearman)
                {
                    bestAbsoluteSpearman = absSpearman;
                    bestSpearmanMetric = correlation.Name;
                }
            }

            return new StateComplexityMetricStudyResult1D(
                samples.ToArray(),
                correlations,
                bestPearsonMetric,
                bestAbsolutePearson,
                bestSpearmanMetric,
                bestAbsoluteSpearman);
        }

        private static void AddRegionSample(
            List<StateComplexityMetricSample1D> samples,
            double time,
            char region,
            in FiniteLayerReducedState1D state,
            double length,
            double l2Contribution,
            double peakContribution)
        {
            if (l2Contribution <= L2OmissionThreshold
                && peakContribution <= PeakOmissionThreshold)
            {
                // Truly negligible regions belong to the separate zero-budget rule.
                return;
            }

            var fits = ConstrainedGaussianSparseFitter1D.FitSequence(
                state,
                length,
                MaximumGaussianCount);

            var requiredCount = FirstCountAtOrBelow(fits, LocalFitThreshold);
            if (requiredCount == 0)
            {
                // 9 means the bounded 1..8 dictionary did not reach the target.
                // Keeping this as a censored high-complexity observation is more
                // informative than silently dropping it.
                requiredCount = MaximumGaussianCount + 1;
            }

            var metric = ComputeStateMetrics(state);
            samples.Add(new StateComplexityMetricSample1D(
                time,
                region,
                requiredCount,
                l2Contribution,
                peakContribution,
                metric.RmsModeIndex,
                metric.QuarticModeIndex,
                metric.HighModeEnergyFraction,
                metric.SpectralEntropy,
                metric.EffectiveModeCount,
                metric.NormalizedGradientScore,
                metric.NormalizedCurvatureScore));
        }

        private static StateMetricValues ComputeStateMetrics(
            in FiniteLayerReducedState1D state)
        {
            var modalEnergy = 0.0;
            var n2Energy = 0.0;
            var n4Energy = 0.0;
            var highModeEnergy = 0.0;

            for (var modeIndex = 0; modeIndex < state.ModeCount; modeIndex++)
            {
                var n = modeIndex + 1;
                var coefficient = state.GetModeCoefficient(modeIndex);
                var energy = coefficient * coefficient;
                modalEnergy += energy;
                n2Energy += n * n * energy;
                n4Energy += n * n * n * n * energy;
                if (n >= 5)
                {
                    highModeEnergy += energy;
                }
            }

            var mean = state.MeanTemperaturePerturbation;
            var fieldEnergy = mean * mean + 0.5 * modalEnergy;
            var safeModalEnergy = Math.Max(modalEnergy, 1e-30);
            var safeFieldEnergy = Math.Max(fieldEnergy, 1e-30);

            var rmsModeIndex = Math.Sqrt(n2Energy / safeModalEnergy);
            var quarticModeIndex = Math.Pow(n4Energy / safeModalEnergy, 0.25);
            var highModeEnergyFraction = highModeEnergy / safeModalEnergy;

            var entropy = 0.0;
            if (modalEnergy > 1e-30)
            {
                for (var modeIndex = 0; modeIndex < state.ModeCount; modeIndex++)
                {
                    var coefficient = state.GetModeCoefficient(modeIndex);
                    var probability = coefficient * coefficient / modalEnergy;
                    if (probability > 0.0)
                    {
                        entropy -= probability * Math.Log(probability);
                    }
                }
            }

            var normalizedEntropy = state.ModeCount > 1
                ? entropy / Math.Log(state.ModeCount)
                : 0.0;
            var effectiveModeCount = Math.Exp(entropy);
            var normalizedGradientScore = Math.Sqrt(0.5 * n2Energy / safeFieldEnergy);
            var normalizedCurvatureScore = Math.Sqrt(0.5 * n4Energy / safeFieldEnergy);

            return new StateMetricValues(
                rmsModeIndex,
                quarticModeIndex,
                highModeEnergyFraction,
                normalizedEntropy,
                effectiveModeCount,
                normalizedGradientScore,
                normalizedCurvatureScore);
        }

        private static StateComplexityMetricCorrelation1D[] BuildCorrelations(
            List<StateComplexityMetricSample1D> samples)
        {
            var target = new double[samples.Count];
            var rms = new double[samples.Count];
            var quartic = new double[samples.Count];
            var high = new double[samples.Count];
            var entropy = new double[samples.Count];
            var effective = new double[samples.Count];
            var gradient = new double[samples.Count];
            var curvature = new double[samples.Count];
            var l2Contribution = new double[samples.Count];
            var peakContribution = new double[samples.Count];

            for (var i = 0; i < samples.Count; i++)
            {
                var sample = samples[i];
                target[i] = sample.RequiredGaussianCount;
                rms[i] = sample.RmsModeIndex;
                quartic[i] = sample.QuarticModeIndex;
                high[i] = sample.HighModeEnergyFraction;
                entropy[i] = sample.SpectralEntropy;
                effective[i] = sample.EffectiveModeCount;
                gradient[i] = sample.NormalizedGradientScore;
                curvature[i] = sample.NormalizedCurvatureScore;
                l2Contribution[i] = sample.L2Contribution;
                peakContribution[i] = sample.PeakContribution;
            }

            return new[]
            {
                Correlate("rms-mode-index", rms, target),
                Correlate("quartic-mode-index", quartic, target),
                Correlate("high-mode-energy-fraction", high, target),
                Correlate("spectral-entropy", entropy, target),
                Correlate("effective-mode-count", effective, target),
                Correlate("normalized-gradient-score", gradient, target),
                Correlate("normalized-curvature-score", curvature, target),
                Correlate("global-L2-contribution", l2Contribution, target),
                Correlate("global-peak-contribution", peakContribution, target)
            };
        }

        private static StateComplexityMetricCorrelation1D Correlate(
            string name,
            double[] values,
            double[] target)
        {
            return new StateComplexityMetricCorrelation1D(
                name,
                Pearson(values, target),
                Pearson(Ranks(values), Ranks(target)));
        }

        private static double Pearson(double[] x, double[] y)
        {
            if (x.Length != y.Length || x.Length < 2)
            {
                return double.NaN;
            }

            var meanX = 0.0;
            var meanY = 0.0;
            for (var i = 0; i < x.Length; i++)
            {
                meanX += x[i];
                meanY += y[i];
            }
            meanX /= x.Length;
            meanY /= y.Length;

            var covariance = 0.0;
            var varianceX = 0.0;
            var varianceY = 0.0;
            for (var i = 0; i < x.Length; i++)
            {
                var dx = x[i] - meanX;
                var dy = y[i] - meanY;
                covariance += dx * dy;
                varianceX += dx * dx;
                varianceY += dy * dy;
            }

            var denominator = Math.Sqrt(varianceX * varianceY);
            return denominator > 0.0 ? covariance / denominator : 0.0;
        }

        private static double[] Ranks(double[] values)
        {
            var indices = new int[values.Length];
            for (var i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            Array.Sort(indices, (left, right) => values[left].CompareTo(values[right]));
            var ranks = new double[values.Length];
            var position = 0;

            while (position < indices.Length)
            {
                var end = position + 1;
                while (end < indices.Length
                    && values[indices[end]].Equals(values[indices[position]]))
                {
                    end++;
                }

                var averageRank = 0.5 * (position + 1 + end);
                for (var i = position; i < end; i++)
                {
                    ranks[indices[i]] = averageRank;
                }
                position = end;
            }

            return ranks;
        }

        private static int FirstCountAtOrBelow(
            ConstrainedGaussianSparseFitResult1D[] fits,
            double threshold)
        {
            for (var i = 0; i < fits.Length; i++)
            {
                if (fits[i].RelativeError <= threshold)
                {
                    return i + 1;
                }
            }
            return 0;
        }

        private static GlobalContribution MeasureGlobalContributions(
            in ThreeLayerCoupledState1D state,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredA = 0.0;
            var squaredB = 0.0;
            var squaredC = 0.0;
            var peakA = 0.0;
            var peakB = 0.0;
            var peakC = 0.0;

            AccumulateFieldNorm(state.StateA, lengthA, ref squaredA, ref peakA);
            AccumulateFieldNorm(state.StateB, lengthB, ref squaredB, ref peakB);
            AccumulateFieldNorm(state.StateC, lengthC, ref squaredC, ref peakC);

            var globalNorm = Math.Sqrt(squaredA + squaredB + squaredC);
            var globalPeak = Math.Max(peakA, Math.Max(peakB, peakC));
            var safeNorm = Math.Max(globalNorm, 1e-30);
            var safePeak = Math.Max(globalPeak, 1e-30);

            return new GlobalContribution(
                Math.Sqrt(squaredA) / safeNorm,
                Math.Sqrt(squaredB) / safeNorm,
                Math.Sqrt(squaredC) / safeNorm,
                peakA / safePeak,
                peakB / safePeak,
                peakC / safePeak);
        }

        private static void AccumulateFieldNorm(
            in FiniteLayerReducedState1D state,
            double length,
            ref double squared,
            ref double peak)
        {
            for (var sample = 0; sample < FieldSampleCount; sample++)
            {
                var x = (sample + 0.5) * length / FieldSampleCount;
                var value = FiniteLayerFieldRepresentation1D.Evaluate(state, x, length);
                squared += value * value;
                peak = Math.Max(peak, Math.Abs(value));
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

        private readonly struct StateMetricValues
        {
            public StateMetricValues(
                double rmsModeIndex,
                double quarticModeIndex,
                double highModeEnergyFraction,
                double spectralEntropy,
                double effectiveModeCount,
                double normalizedGradientScore,
                double normalizedCurvatureScore)
            {
                RmsModeIndex = rmsModeIndex;
                QuarticModeIndex = quarticModeIndex;
                HighModeEnergyFraction = highModeEnergyFraction;
                SpectralEntropy = spectralEntropy;
                EffectiveModeCount = effectiveModeCount;
                NormalizedGradientScore = normalizedGradientScore;
                NormalizedCurvatureScore = normalizedCurvatureScore;
            }

            public double RmsModeIndex { get; }
            public double QuarticModeIndex { get; }
            public double HighModeEnergyFraction { get; }
            public double SpectralEntropy { get; }
            public double EffectiveModeCount { get; }
            public double NormalizedGradientScore { get; }
            public double NormalizedCurvatureScore { get; }
        }

        private readonly struct GlobalContribution
        {
            public GlobalContribution(
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
