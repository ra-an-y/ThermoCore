using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    /// <summary>
    /// Checkpoint 21: test whether a state-change vector is more informative than
    /// a single curvature trend for predicting the direction of Gaussian-count change.
    ///
    /// The predictor is deliberately simple and leakage-controlled: for each held-out
    /// transition, feature weights are estimated only from the other transitions using
    /// the Pearson association between feature change and count change. The held-out
    /// score is the weighted standardized feature-change sum. This is a directional
    /// predictor only; representation fitting remains the safety authority.
    /// </summary>
    internal static class Checkpoint21MultifeatureCountTrendStudy1D
    {
        private const double Epsilon = 1e-12;
        private static readonly double[] Times = { 0.10, 0.20, 0.40, 0.60, 1.00, 1.50 };
        private static readonly string[] FeatureNames =
        {
            "d-curvature",
            "d-high-mode",
            "d-effective-mode-count",
            "d-spectral-entropy",
            "d-gradient",
            "d-L2",
            "d-peak"
        };

        [ModuleInitializer]
        internal static void Run()
        {
            var study = StateComplexityMetricStudy1D.Evaluate();
            var samples = CollectSamples(study);

            var transitionCount = (Times.Length - 1) * 3;
            var correct = 0;
            var nonZeroTruth = 0;
            var nonZeroCorrect = 0;
            var abstain = 0;

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Checkpoint 21 Multi-Feature Count Trend Study");
            Console.WriteLine("LOTO directional predictor; fitting authority remains exact representation verification.");
            Console.WriteLine("transition | region | dN | predicted | score | top training feature association");

            for (var transition = 1; transition < Times.Length; transition++)
            {
                for (var region = 0; region < 3; region++)
                {
                    var current = samples[transition, region];
                    var previous = samples[transition - 1, region];
                    var dN = Sign(current.RequiredGaussianCount - previous.RequiredGaussianCount);
                    var featureDelta = BuildFeatureDelta(previous, current);

                    var training = new List<int>();
                    for (var otherTransition = 1; otherTransition < Times.Length; otherTransition++)
                    {
                        if (otherTransition == transition)
                        {
                            continue;
                        }
                        training.Add(otherTransition);
                    }

                    var weights = EstimateFeatureAssociations(samples, region, training);
                    var score = Score(featureDelta, weights);
                    var prediction = Sign(score);
                    if (Math.Abs(score) < 1e-9)
                    {
                        prediction = 0;
                        abstain++;
                    }

                    if (dN != 0)
                    {
                        nonZeroTruth++;
                        if (prediction == dN)
                        {
                            nonZeroCorrect++;
                        }
                    }

                    if (prediction == dN)
                    {
                        correct++;
                    }

                    var topFeature = TopFeature(weights);
                    Console.WriteLine(
                        $"{Times[transition],8:F2} | {(char)('A' + region),6} | {dN,2} | {prediction,9} | {score,7:F3} | {topFeature}");
                }
            }

            Console.WriteLine($"All-direction accuracy: {correct}/{transitionCount} = {(double)correct / transitionCount:P2}");
            Console.WriteLine($"Non-zero trend accuracy: {nonZeroCorrect}/{nonZeroTruth} = {(double)nonZeroCorrect / Math.Max(1, nonZeroTruth):P2}");
            Console.WriteLine($"Abstentions: {abstain}/{transitionCount}");
            Console.WriteLine("This checkpoint is exploratory; no allocator safety claim is made.");
            Console.WriteLine("MULTIFEATURE TREND STUDY COMPLETE");
            Console.WriteLine();
        }

        private static StateComplexityMetricSample1D[,] CollectSamples(
            StateComplexityMetricStudyResult1D study)
        {
            var result = new StateComplexityMetricSample1D[Times.Length, 3];
            var found = new bool[Times.Length, 3];
            for (var i = 0; i < study.SampleCount; i++)
            {
                var sample = study.GetSample(i);
                var transition = Array.IndexOf(Times, sample.Time);
                var region = sample.Region == 'A' ? 0 : sample.Region == 'B' ? 1 : 2;
                if (transition >= 0)
                {
                    result[transition, region] = sample;
                    found[transition, region] = true;
                }
            }

            for (var t = 0; t < Times.Length; t++)
            {
                for (var r = 0; r < 3; r++)
                {
                    if (!found[t, r])
                    {
                        throw new InvalidOperationException($"Missing state-complexity sample at t={Times[t]:F2}, region={(char)('A' + r)}.");
                    }
                }
            }

            return result;
        }

        private static double[] BuildFeatureDelta(
            StateComplexityMetricSample1D previous,
            StateComplexityMetricSample1D current)
        {
            return new[]
            {
                current.NormalizedCurvatureScore - previous.NormalizedCurvatureScore,
                current.HighModeEnergyFraction - previous.HighModeEnergyFraction,
                current.EffectiveModeCount - previous.EffectiveModeCount,
                current.SpectralEntropy - previous.SpectralEntropy,
                current.NormalizedGradientScore - previous.NormalizedGradientScore,
                current.L2Contribution - previous.L2Contribution,
                current.PeakContribution - previous.PeakContribution
            };
        }

        private static double[] EstimateFeatureAssociations(
            StateComplexityMetricSample1D[,] samples,
            int region,
            List<int> trainingTransitions)
        {
            var weights = new double[FeatureNames.Length];
            for (var feature = 0; feature < FeatureNames.Length; feature++)
            {
                var x = new double[trainingTransitions.Count];
                var y = new double[trainingTransitions.Count];
                for (var i = 0; i < trainingTransitions.Count; i++)
                {
                    var transition = trainingTransitions[i];
                    var previous = samples[transition - 1, region];
                    var current = samples[transition, region];
                    x[i] = BuildFeatureDelta(previous, current)[feature];
                    y[i] = current.RequiredGaussianCount - previous.RequiredGaussianCount;
                }

                weights[feature] = Pearson(x, y);
            }

            return weights;
        }

        private static double Score(double[] delta, double[] weights)
        {
            var score = 0.0;
            for (var i = 0; i < delta.Length; i++)
            {
                var scale = Math.Sqrt(Math.Abs(weights[i]) + Epsilon);
                score += Math.Sign(weights[i]) * delta[i] * scale;
            }
            return score;
        }

        private static string TopFeature(double[] weights)
        {
            var index = 0;
            var best = Math.Abs(weights[0]);
            for (var i = 1; i < weights.Length; i++)
            {
                var candidate = Math.Abs(weights[i]);
                if (candidate > best)
                {
                    best = candidate;
                    index = i;
                }
            }
            return $"{FeatureNames[index]} ({weights[index]:F3})";
        }

        private static int Sign(double value)
        {
            if (value > Epsilon) return 1;
            if (value < -Epsilon) return -1;
            return 0;
        }

        private static double Pearson(double[] x, double[] y)
        {
            if (x.Length != y.Length || x.Length < 2)
            {
                return 0.0;
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

            var numerator = 0.0;
            var sumX2 = 0.0;
            var sumY2 = 0.0;
            for (var i = 0; i < x.Length; i++)
            {
                var dx = x[i] - meanX;
                var dy = y[i] - meanY;
                numerator += dx * dy;
                sumX2 += dx * dx;
                sumY2 += dy * dy;
            }

            var denominator = Math.Sqrt(sumX2 * sumY2);
            return denominator > Epsilon ? numerator / denominator : 0.0;
        }
    }
}
