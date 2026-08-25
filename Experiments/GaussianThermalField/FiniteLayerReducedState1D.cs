using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    /// <summary>
    /// Experiment-local reduced current state for a finite one-dimensional
    /// homogeneous thermal layer.
    ///
    /// The state stores the present mean temperature perturbation and a bounded
    /// set of cosine diffusion-mode coefficients. It is neither material
    /// definition nor historical event storage, and it is not ThermoCore
    /// Thermodynamic State.
    /// </summary>
    public readonly struct FiniteLayerReducedState1D
    {
        private readonly double[] _modes;

        public FiniteLayerReducedState1D(
            double meanTemperaturePerturbation,
            double[] modeCoefficients)
        {
            if (double.IsNaN(meanTemperaturePerturbation)
                || double.IsInfinity(meanTemperaturePerturbation))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(meanTemperaturePerturbation));
            }

            if (modeCoefficients is null)
            {
                throw new ArgumentNullException(nameof(modeCoefficients));
            }

            _modes = new double[modeCoefficients.Length];

            for (var i = 0; i < modeCoefficients.Length; i++)
            {
                var coefficient = modeCoefficients[i];
                if (double.IsNaN(coefficient) || double.IsInfinity(coefficient))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(modeCoefficients),
                        "Mode coefficients must be finite.");
                }

                _modes[i] = coefficient;
            }

            MeanTemperaturePerturbation = meanTemperaturePerturbation;
        }

        public double MeanTemperaturePerturbation { get; }

        public int ModeCount => _modes?.Length ?? 0;

        public double GetModeCoefficient(int modeIndex)
        {
            if (_modes is null)
            {
                throw new InvalidOperationException(
                    "The default state has no initialized mode storage.");
            }

            if (modeIndex < 0 || modeIndex >= _modes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(modeIndex));
            }

            return _modes[modeIndex];
        }

        public double[] CopyModeCoefficients()
        {
            if (_modes is null)
            {
                return Array.Empty<double>();
            }

            return (double[])_modes.Clone();
        }

        public static FiniteLayerReducedState1D Zero(int modeCount)
        {
            if (modeCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(modeCount));
            }

            return new FiniteLayerReducedState1D(
                meanTemperaturePerturbation: 0.0,
                modeCoefficients: new double[modeCount]);
        }
    }
}
