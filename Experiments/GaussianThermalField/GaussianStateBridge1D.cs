using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    /// <summary>
    /// Fixed-size experiment-local mixture of signed Gaussian field terms.
    /// This is a downstream field representation, not authoritative physical state.
    /// </summary>
    public readonly struct GaussianMixture1D
    {
        private readonly GaussianKernel1D[] _kernels;

        public GaussianMixture1D(GaussianKernel1D[] kernels)
        {
            if (kernels is null)
            {
                throw new ArgumentNullException(nameof(kernels));
            }

            _kernels = (GaussianKernel1D[])kernels.Clone();
        }

        public int Count => _kernels?.Length ?? 0;

        public GaussianKernel1D GetKernel(int index)
        {
            if (_kernels is null || index < 0 || index >= _kernels.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _kernels[index];
        }

        public double Evaluate(double x)
        {
            var value = 0.0;
            for (var i = 0; i < Count; i++)
            {
                value += _kernels[i].Evaluate(x);
            }

            return value;
        }
    }

    /// <summary>
    /// Experiment-local bridge between Gaussian field representation and the
    /// finite-layer reduced current state.
    ///
    /// Projection converts a Gaussian field into bounded cosine state.
    /// Recovery fits a fixed Gaussian dictionary to the current field and adds
    /// one broad signed correction kernel so the represented domain integral
    /// matches the reduced state's mean term.
    /// </summary>
    public static class GaussianStateBridge1D
    {
        public static FiniteLayerReducedState1D ProjectGaussianToState(
            in GaussianKernel1D gaussian,
            double layerLength,
            int modeCount)
        {
            ValidateLayerLength(layerLength);
            if (modeCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(modeCount));
            }

            const int intervalCount = 8192;
            var dx = layerLength / intervalCount;
            var meanIntegral = 0.0;
            var modeIntegrals = new double[modeCount];

            for (var sample = 0; sample <= intervalCount; sample++)
            {
                var x = sample * dx;
                var weight = sample == 0 || sample == intervalCount ? 0.5 : 1.0;
                var value = gaussian.Evaluate(x);
                meanIntegral += weight * value;

                for (var modeIndex = 0; modeIndex < modeCount; modeIndex++)
                {
                    var n = modeIndex + 1;
                    modeIntegrals[modeIndex] += weight * value
                        * Math.Cos(n * Math.PI * x / layerLength);
                }
            }

            var mean = meanIntegral * dx / layerLength;
            for (var modeIndex = 0; modeIndex < modeCount; modeIndex++)
            {
                modeIntegrals[modeIndex] *= 2.0 * dx / layerLength;
            }

            return new FiniteLayerReducedState1D(mean, modeIntegrals);
        }

        public static GaussianMixture1D RecoverGaussianMixture(
            in FiniteLayerReducedState1D state,
            double layerLength,
            int fittedKernelCount,
            double widthFactor)
        {
            ValidateLayerLength(layerLength);
            if (fittedKernelCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fittedKernelCount));
            }

            if (double.IsNaN(widthFactor)
                || double.IsInfinity(widthFactor)
                || widthFactor <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(widthFactor));
            }

            const int sampleCount = 401;
            var spacing = layerLength / fittedKernelCount;
            var standardDeviation = widthFactor * spacing;
            var variance = standardDeviation * standardDeviation;

            var normal = new double[fittedKernelCount, fittedKernelCount];
            var rhs = new double[fittedKernelCount];
            var centers = new double[fittedKernelCount];

            for (var kernel = 0; kernel < fittedKernelCount; kernel++)
            {
                centers[kernel] = (kernel + 0.5) * spacing;
            }

            for (var sample = 0; sample < sampleCount; sample++)
            {
                var x = (sample + 0.5) * layerLength / sampleCount;
                var target = FiniteLayerFieldRepresentation1D.Evaluate(
                    state, x, layerLength);
                var basis = new double[fittedKernelCount];

                for (var kernel = 0; kernel < fittedKernelCount; kernel++)
                {
                    basis[kernel] = NormalizedGaussian(
                        x, centers[kernel], variance);
                    rhs[kernel] += basis[kernel] * target;
                }

                for (var row = 0; row < fittedKernelCount; row++)
                {
                    for (var column = 0; column < fittedKernelCount; column++)
                    {
                        normal[row, column] += basis[row] * basis[column];
                    }
                }
            }

            var trace = 0.0;
            for (var i = 0; i < fittedKernelCount; i++)
            {
                trace += normal[i, i];
            }

            var ridge = 1e-10 * trace / fittedKernelCount;
            for (var i = 0; i < fittedKernelCount; i++)
            {
                normal[i, i] += ridge;
            }

            var amplitudes = SolveLinearSystem(normal, rhs);
            var kernels = new GaussianKernel1D[fittedKernelCount + 1];
            var representedIntegral = 0.0;

            for (var kernel = 0; kernel < fittedKernelCount; kernel++)
            {
                kernels[kernel] = new GaussianKernel1D(
                    centers[kernel],
                    variance,
                    amplitudes[kernel]);

                representedIntegral += amplitudes[kernel]
                    * IntegrateUnitGaussian(centers[kernel], variance, layerLength);
            }

            var targetIntegral =
                state.MeanTemperaturePerturbation * layerLength;

            // Broad correction preserves the region-integrated scalar quantity
            // without changing the authoritative reduced current state.
            var correctionMean = 0.5 * layerLength;
            var correctionVariance = layerLength * layerLength;
            var correctionUnitIntegral = IntegrateUnitGaussian(
                correctionMean,
                correctionVariance,
                layerLength);
            var correctionAmplitude =
                (targetIntegral - representedIntegral) / correctionUnitIntegral;

            kernels[fittedKernelCount] = new GaussianKernel1D(
                correctionMean,
                correctionVariance,
                correctionAmplitude);

            return new GaussianMixture1D(kernels);
        }

        public static double IntegrateMixture(
            in GaussianMixture1D mixture,
            double layerLength,
            int intervalCount = 16384)
        {
            ValidateLayerLength(layerLength);
            if (intervalCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(intervalCount));
            }

            var dx = layerLength / intervalCount;
            var integral = 0.0;
            for (var sample = 0; sample <= intervalCount; sample++)
            {
                var x = sample * dx;
                var weight = sample == 0 || sample == intervalCount ? 0.5 : 1.0;
                integral += weight * mixture.Evaluate(x);
            }

            return integral * dx;
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
                        "Gaussian recovery system is numerically singular.");
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

            var solution = new double[size];
            for (var row = 0; row < size; row++)
            {
                solution[row] = augmented[row, size];
            }

            return solution;
        }

        private static void ValidateLayerLength(double layerLength)
        {
            if (double.IsNaN(layerLength)
                || double.IsInfinity(layerLength)
                || layerLength <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(layerLength));
            }
        }
    }
}
