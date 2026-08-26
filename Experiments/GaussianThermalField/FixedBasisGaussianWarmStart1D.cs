using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    /// <summary>
    /// Cheap warm-start update that preserves the previous Gaussian support
    /// (mean/variance) and only re-solves signed amplitudes for the current
    /// reduced state under the same finite-region integral constraint used by
    /// the sparse fitter.
    /// </summary>
    internal static class FixedBasisGaussianWarmStart1D
    {
        private const int SampleCount = 401;

        public static ConstrainedGaussianSparseFitResult1D RefitAmplitudes(
            in FiniteLayerReducedState1D state,
            double layerLength,
            in GaussianMixture1D previous)
        {
            if (previous.Count <= 0)
            {
                throw new ArgumentException(
                    "Warm-start amplitude refit requires at least one Gaussian.",
                    nameof(previous));
            }

            var count = previous.Count;
            var x = new double[SampleCount];
            var target = new double[SampleCount];
            var basis = new double[count][];
            var integrals = new double[count];

            for (var kernelIndex = 0; kernelIndex < count; kernelIndex++)
            {
                basis[kernelIndex] = new double[SampleCount];
            }

            for (var sample = 0; sample < SampleCount; sample++)
            {
                x[sample] = (sample + 0.5) * layerLength / SampleCount;
                target[sample] = FiniteLayerFieldRepresentation1D.Evaluate(
                    state,
                    x[sample],
                    layerLength);
            }

            for (var kernelIndex = 0; kernelIndex < count; kernelIndex++)
            {
                var prior = previous.GetKernel(kernelIndex);
                var unit = new GaussianKernel1D(prior.Mean, prior.Variance, 1.0);
                for (var sample = 0; sample < SampleCount; sample++)
                {
                    basis[kernelIndex][sample] = unit.Evaluate(x[sample]);
                }
                integrals[kernelIndex] = IntegrateUnitGaussian(
                    prior.Mean,
                    prior.Variance,
                    layerLength);
            }

            var targetIntegral = state.MeanTemperaturePerturbation * layerLength;
            var amplitudes = SolveConstrainedAmplitudes(
                basis,
                integrals,
                target,
                targetIntegral);

            var kernels = new GaussianKernel1D[count];
            var representedIntegral = 0.0;
            for (var kernelIndex = 0; kernelIndex < count; kernelIndex++)
            {
                var prior = previous.GetKernel(kernelIndex);
                kernels[kernelIndex] = new GaussianKernel1D(
                    prior.Mean,
                    prior.Variance,
                    amplitudes[kernelIndex]);
                representedIntegral += amplitudes[kernelIndex] * integrals[kernelIndex];
            }

            return new ConstrainedGaussianSparseFitResult1D(
                new GaussianMixture1D(kernels),
                RelativeError(basis, amplitudes, target),
                representedIntegral - targetIntegral);
        }

        private static double[] SolveConstrainedAmplitudes(
            double[][] basis,
            double[] integrals,
            double[] target,
            double targetIntegral)
        {
            var count = basis.Length;
            var size = count + 1;
            var matrix = new double[size, size];
            var rhs = new double[size];
            var trace = 0.0;

            for (var row = 0; row < count; row++)
            {
                for (var column = 0; column < count; column++)
                {
                    var dot = 0.0;
                    for (var sample = 0; sample < target.Length; sample++)
                    {
                        dot += basis[row][sample] * basis[column][sample];
                    }
                    matrix[row, column] = dot;
                }

                trace += matrix[row, row];

                var projection = 0.0;
                for (var sample = 0; sample < target.Length; sample++)
                {
                    projection += basis[row][sample] * target[sample];
                }
                rhs[row] = projection;

                matrix[row, count] = integrals[row];
                matrix[count, row] = integrals[row];
            }

            var ridge = 1e-12 * trace / count;
            for (var index = 0; index < count; index++)
            {
                matrix[index, index] += ridge;
            }
            rhs[count] = targetIntegral;

            var solution = SolveLinearSystem(matrix, rhs);
            var amplitudes = new double[count];
            Array.Copy(solution, amplitudes, count);
            return amplitudes;
        }

        private static double RelativeError(
            double[][] basis,
            double[] amplitudes,
            double[] target)
        {
            var errorSquared = 0.0;
            var referenceSquared = 0.0;

            for (var sample = 0; sample < target.Length; sample++)
            {
                var represented = 0.0;
                for (var kernel = 0; kernel < amplitudes.Length; kernel++)
                {
                    represented += amplitudes[kernel] * basis[kernel][sample];
                }
                var difference = represented - target[sample];
                errorSquared += difference * difference;
                referenceSquared += target[sample] * target[sample];
            }

            return referenceSquared <= 1e-30
                ? Math.Sqrt(errorSquared)
                : Math.Sqrt(errorSquared / referenceSquared);
        }

        private static double IntegrateUnitGaussian(
            double mean,
            double variance,
            double layerLength)
        {
            const int intervalCount = 4096;
            var unit = new GaussianKernel1D(mean, variance, 1.0);
            var dx = layerLength / intervalCount;
            var integral = 0.0;

            for (var sample = 0; sample <= intervalCount; sample++)
            {
                var x = sample * dx;
                var weight = sample == 0 || sample == intervalCount ? 0.5 : 1.0;
                integral += weight * unit.Evaluate(x);
            }
            return integral * dx;
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
                        "Warm-start constrained amplitude system is numerically singular.");
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
    }
}
