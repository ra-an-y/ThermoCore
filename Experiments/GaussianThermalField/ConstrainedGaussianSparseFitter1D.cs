using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal readonly struct ConstrainedGaussianSparseFitResult1D
    {
        public ConstrainedGaussianSparseFitResult1D(
            GaussianMixture1D mixture,
            double relativeError,
            double integralError)
        {
            Mixture = mixture;
            RelativeError = relativeError;
            IntegralError = integralError;
        }

        public GaussianMixture1D Mixture { get; }
        public double RelativeError { get; }
        public double IntegralError { get; }
    }

    /// <summary>
    /// Experiment-local constrained sparse Gaussian fitter used by adaptive
    /// representation studies. Amplitudes are re-solved at every retained
    /// count with an equality constraint on the finite-region integral.
    /// </summary>
    internal static class ConstrainedGaussianSparseFitter1D
    {
        private const int FitSampleCount = 401;

        private static readonly double[] CenterScales = CreateCenterScales();
        private static readonly double[] SigmaScales =
        {
            0.06, 0.08, 0.10, 0.14, 0.20, 0.28,
            0.40, 0.60, 0.90, 1.30, 2.00
        };

        public static ConstrainedGaussianSparseFitResult1D[] FitSequence(
            in FiniteLayerReducedState1D state,
            double layerLength,
            int maximumCount)
        {
            if (maximumCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCount));
            }

            var sampleX = new double[FitSampleCount];
            var target = new double[FitSampleCount];
            for (var sample = 0; sample < FitSampleCount; sample++)
            {
                sampleX[sample] = (sample + 0.5) * layerLength / FitSampleCount;
                target[sample] = FiniteLayerFieldRepresentation1D.Evaluate(
                    state,
                    sampleX[sample],
                    layerLength);
            }

            var candidates = CreateCandidates(sampleX, layerLength);
            var selected = new List<int>();
            var results = new ConstrainedGaussianSparseFitResult1D[maximumCount];
            var targetIntegral = state.MeanTemperaturePerturbation * layerLength;

            for (var count = 1; count <= maximumCount; count++)
            {
                var bestError = double.PositiveInfinity;
                var bestCandidate = -1;

                for (var candidateIndex = 0;
                    candidateIndex < candidates.Length;
                    candidateIndex++)
                {
                    if (selected.Contains(candidateIndex))
                    {
                        continue;
                    }

                    var trial = new int[selected.Count + 1];
                    for (var index = 0; index < selected.Count; index++)
                    {
                        trial[index] = selected[index];
                    }
                    trial[trial.Length - 1] = candidateIndex;

                    double[] amplitudes;
                    try
                    {
                        amplitudes = SolveConstrainedAmplitudes(
                            candidates,
                            trial,
                            target,
                            targetIntegral);
                    }
                    catch (InvalidOperationException)
                    {
                        continue;
                    }

                    var error = RelativeSampleError(
                        candidates,
                        trial,
                        amplitudes,
                        target);

                    if (error < bestError)
                    {
                        bestError = error;
                        bestCandidate = candidateIndex;
                    }
                }

                if (bestCandidate < 0)
                {
                    throw new InvalidOperationException(
                        "No valid constrained Gaussian sparse fit was found.");
                }

                selected.Add(bestCandidate);
                var selectedArray = selected.ToArray();
                var amplitudesFinal = SolveConstrainedAmplitudes(
                    candidates,
                    selectedArray,
                    target,
                    targetIntegral);

                var kernels = new GaussianKernel1D[selectedArray.Length];
                var representedIntegral = 0.0;
                for (var index = 0; index < selectedArray.Length; index++)
                {
                    var candidate = candidates[selectedArray[index]];
                    kernels[index] = new GaussianKernel1D(
                        candidate.Mean,
                        candidate.Variance,
                        amplitudesFinal[index]);
                    representedIntegral += amplitudesFinal[index]
                        * candidate.UnitIntegral;
                }

                results[count - 1] = new ConstrainedGaussianSparseFitResult1D(
                    new GaussianMixture1D(kernels),
                    RelativeSampleError(
                        candidates,
                        selectedArray,
                        amplitudesFinal,
                        target),
                    representedIntegral - targetIntegral);
            }

            return results;
        }

        private static Candidate[] CreateCandidates(
            double[] sampleX,
            double layerLength)
        {
            var candidates = new Candidate[CenterScales.Length * SigmaScales.Length];
            var index = 0;

            for (var centerIndex = 0; centerIndex < CenterScales.Length; centerIndex++)
            {
                for (var sigmaIndex = 0; sigmaIndex < SigmaScales.Length; sigmaIndex++)
                {
                    var mean = CenterScales[centerIndex] * layerLength;
                    var standardDeviation = SigmaScales[sigmaIndex] * layerLength;
                    var variance = standardDeviation * standardDeviation;
                    var values = new double[sampleX.Length];

                    for (var sample = 0; sample < sampleX.Length; sample++)
                    {
                        values[sample] = NormalizedGaussian(
                            sampleX[sample],
                            mean,
                            variance);
                    }

                    candidates[index++] = new Candidate(
                        mean,
                        variance,
                        values,
                        IntegrateUnitGaussian(mean, variance, layerLength));
                }
            }

            return candidates;
        }

        private static double[] SolveConstrainedAmplitudes(
            Candidate[] candidates,
            int[] selected,
            double[] target,
            double targetIntegral)
        {
            var count = selected.Length;
            var size = count + 1;
            var system = new double[size, size];
            var rhs = new double[size];
            var trace = 0.0;

            for (var row = 0; row < count; row++)
            {
                var rowCandidate = candidates[selected[row]];
                for (var column = 0; column < count; column++)
                {
                    var columnCandidate = candidates[selected[column]];
                    var value = 0.0;
                    for (var sample = 0; sample < target.Length; sample++)
                    {
                        value += rowCandidate.Values[sample]
                            * columnCandidate.Values[sample];
                    }
                    system[row, column] = value;
                }

                trace += system[row, row];

                var projection = 0.0;
                for (var sample = 0; sample < target.Length; sample++)
                {
                    projection += rowCandidate.Values[sample] * target[sample];
                }
                rhs[row] = projection;

                system[row, count] = rowCandidate.UnitIntegral;
                system[count, row] = rowCandidate.UnitIntegral;
            }

            var ridge = 1e-12 * trace / count;
            for (var index = 0; index < count; index++)
            {
                system[index, index] += ridge;
            }

            rhs[count] = targetIntegral;
            var solution = SolveLinearSystem(system, rhs);
            var amplitudes = new double[count];
            Array.Copy(solution, amplitudes, count);
            return amplitudes;
        }

        private static double RelativeSampleError(
            Candidate[] candidates,
            int[] selected,
            double[] amplitudes,
            double[] target)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            for (var sample = 0; sample < target.Length; sample++)
            {
                var represented = 0.0;
                for (var index = 0; index < selected.Length; index++)
                {
                    represented += amplitudes[index]
                        * candidates[selected[index]].Values[sample];
                }

                var difference = represented - target[sample];
                squaredError += difference * difference;
                squaredReference += target[sample] * target[sample];
            }

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static double IntegrateUnitGaussian(
            double mean,
            double variance,
            double layerLength)
        {
            const int intervalCount = 8192;
            var dx = layerLength / intervalCount;
            var integral = 0.0;

            for (var sample = 0; sample <= intervalCount; sample++)
            {
                var x = sample * dx;
                var weight = sample == 0 || sample == intervalCount ? 0.5 : 1.0;
                integral += weight * NormalizedGaussian(x, mean, variance);
            }

            return integral * dx;
        }

        private static double NormalizedGaussian(
            double x,
            double mean,
            double variance)
        {
            var displacement = x - mean;
            return Math.Exp(-(displacement * displacement) / (2.0 * variance))
                / Math.Sqrt(2.0 * Math.PI * variance);
        }

        private static double[] SolveLinearSystem(double[,] matrix, double[] rhs)
        {
            var size = rhs.Length;
            var augmented = new double[size, size + 1];

            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    augmented[row, column] = matrix[row, column];
                }
                augmented[row, size] = rhs[row];
            }

            for (var pivot = 0; pivot < size; pivot++)
            {
                var bestRow = pivot;
                var bestMagnitude = Math.Abs(augmented[pivot, pivot]);
                for (var row = pivot + 1; row < size; row++)
                {
                    var magnitude = Math.Abs(augmented[row, pivot]);
                    if (magnitude > bestMagnitude)
                    {
                        bestMagnitude = magnitude;
                        bestRow = row;
                    }
                }

                if (bestMagnitude <= 1e-18)
                {
                    throw new InvalidOperationException(
                        "Constrained Gaussian fit system is numerically singular.");
                }

                if (bestRow != pivot)
                {
                    for (var column = pivot; column <= size; column++)
                    {
                        var temporary = augmented[pivot, column];
                        augmented[pivot, column] = augmented[bestRow, column];
                        augmented[bestRow, column] = temporary;
                    }
                }

                var pivotValue = augmented[pivot, pivot];
                for (var column = pivot; column <= size; column++)
                {
                    augmented[pivot, column] /= pivotValue;
                }

                for (var row = 0; row < size; row++)
                {
                    if (row == pivot)
                    {
                        continue;
                    }

                    var factor = augmented[row, pivot];
                    for (var column = pivot; column <= size; column++)
                    {
                        augmented[row, column] -= factor * augmented[pivot, column];
                    }
                }
            }

            var result = new double[size];
            for (var row = 0; row < size; row++)
            {
                result[row] = augmented[row, size];
            }
            return result;
        }

        private static double[] CreateCenterScales()
        {
            var values = new double[21];
            for (var index = 0; index < values.Length; index++)
            {
                values[index] = -0.5 + 0.1 * index;
            }
            return values;
        }

        private readonly struct Candidate
        {
            public Candidate(
                double mean,
                double variance,
                double[] values,
                double unitIntegral)
            {
                Mean = mean;
                Variance = variance;
                Values = values;
                UnitIntegral = unitIntegral;
            }

            public double Mean { get; }
            public double Variance { get; }
            public double[] Values { get; }
            public double UnitIntegral { get; }
        }
    }
}
