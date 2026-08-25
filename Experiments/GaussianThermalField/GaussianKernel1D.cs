using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    /// <summary>
    /// Experimental one-dimensional normalized Gaussian basis term.
    ///
    /// This is a numerical field representation owned by the experiment.
    /// It is not ThermoCore Thermodynamic State and carries no Framework authority.
    /// </summary>
    public readonly struct GaussianKernel1D
    {
        public GaussianKernel1D(double mean, double variance, double amplitude)
        {
            if (double.IsNaN(mean) || double.IsInfinity(mean))
            {
                throw new ArgumentOutOfRangeException(nameof(mean));
            }

            if (double.IsNaN(variance)
                || double.IsInfinity(variance)
                || variance <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(variance),
                    "Variance must be finite and positive.");
            }

            if (double.IsNaN(amplitude) || double.IsInfinity(amplitude))
            {
                throw new ArgumentOutOfRangeException(nameof(amplitude));
            }

            Mean = mean;
            Variance = variance;
            Amplitude = amplitude;
        }

        public double Mean { get; }

        public double Variance { get; }

        /// <summary>
        /// Signed scalar-field coefficient multiplying the normalized Gaussian.
        /// This is not rendering opacity.
        /// </summary>
        public double Amplitude { get; }

        public double StandardDeviation => Math.Sqrt(Variance);

        public double Evaluate(double x)
        {
            ValidatePosition(x);

            var displacement = x - Mean;
            var normalization = 1.0 / Math.Sqrt(2.0 * Math.PI * Variance);
            var exponent = -(displacement * displacement) / (2.0 * Variance);

            return Amplitude * normalization * Math.Exp(exponent);
        }

        /// <summary>
        /// Evaluates dG/dx for the signed normalized Gaussian basis term.
        /// </summary>
        public double EvaluateDerivative(double x)
        {
            ValidatePosition(x);

            var value = Evaluate(x);
            return -((x - Mean) / Variance) * value;
        }

        private static void ValidatePosition(double x)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
        }
    }
}
