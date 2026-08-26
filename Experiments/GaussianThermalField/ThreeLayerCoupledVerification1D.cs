using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct ThreeLayerCoupledVerificationResult1D
    {
        public ThreeLayerCoupledVerificationResult1D(
            double relativeFieldError,
            double maximumInterfaceJump,
            double reducedEnergyDrift,
            double referenceEnergyDrift,
            double maximumAbsoluteFluxAB,
            double maximumAbsoluteFluxBC,
            int retainedStateScalars)
        {
            RelativeFieldError = relativeFieldError;
            MaximumInterfaceJump = maximumInterfaceJump;
            ReducedEnergyDrift = reducedEnergyDrift;
            ReferenceEnergyDrift = referenceEnergyDrift;
            MaximumAbsoluteFluxAB = maximumAbsoluteFluxAB;
            MaximumAbsoluteFluxBC = maximumAbsoluteFluxBC;
            RetainedStateScalars = retainedStateScalars;
        }

        public double RelativeFieldError { get; }
        public double MaximumInterfaceJump { get; }
        public double ReducedEnergyDrift { get; }
        public double ReferenceEnergyDrift { get; }
        public double MaximumAbsoluteFluxAB { get; }
        public double MaximumAbsoluteFluxBC { get; }
        public int RetainedStateScalars { get; }

        public bool Satisfies(
            double maximumRelativeFieldError,
            double maximumInterfaceJump,
            double maximumEnergyDrift)
        {
            return RelativeFieldError <= maximumRelativeFieldError
                && MaximumInterfaceJump <= maximumInterfaceJump
                && Math.Abs(ReducedEnergyDrift) <= maximumEnergyDrift
                && Math.Abs(ReferenceEnergyDrift) <= maximumEnergyDrift;
        }
    }

    /// <summary>
    /// A-B-C checkpoint with state-determined interface fluxes.
    ///
    /// The reduced path retains a fixed number of modal coefficients in each
    /// region. The independent reference is a heterogeneous cell-centered
    /// finite-volume solve using harmonic interface conductivity. The source is
    /// an initial Gaussian temperature perturbation in region A; no interface
    /// flux history or reflected-Gaussian tree is stored.
    /// </summary>
    public static class ThreeLayerCoupledVerification1D
    {
        public static ThreeLayerCoupledVerificationResult1D Evaluate()
        {
            var materialA = new ThermalMaterial1D(0.40, 2.0);
            var materialB = new ThermalMaterial1D(0.06, 1.2);
            var materialC = new ThermalMaterial1D(0.20, 1.6);

            const double lengthA = 0.60;
            const double lengthB = 0.35;
            const double lengthC = 0.60;
            const int modeCount = 32;
            const double reducedDeltaTime = 0.002;
            const double duration = 0.60;

            static double InitialGaussian(double x)
            {
                const double mean = 0.46;
                const double standardDeviation = 0.05;
                var z = (x - mean) / standardDeviation;
                return Math.Exp(-0.5 * z * z);
            }

            var state = new ThreeLayerCoupledState1D(
                ProjectToCosineState(InitialGaussian, lengthA, modeCount),
                FiniteLayerReducedState1D.Zero(modeCount),
                FiniteLayerReducedState1D.Zero(modeCount));

            var initialReducedEnergy = ReducedEnergy(
                state, lengthA, lengthB, lengthC,
                materialA, materialB, materialC);

            var maximumJump = 0.0;
            var maximumFluxAB = 0.0;
            var maximumFluxBC = 0.0;
            var reducedStepCount = (int)Math.Round(duration / reducedDeltaTime);

            for (var step = 0; step < reducedStepCount; step++)
            {
                var result = ThreeLayerCoupledEvolution1D.Advance(
                    state,
                    reducedDeltaTime,
                    lengthA,
                    lengthB,
                    lengthC,
                    materialA,
                    materialB,
                    materialC);

                state = result.State;
                maximumJump = Math.Max(
                    maximumJump,
                    Math.Max(Math.Abs(result.InterfaceJumpAB),
                        Math.Abs(result.InterfaceJumpBC)));
                maximumFluxAB = Math.Max(maximumFluxAB, Math.Abs(result.FluxAB));
                maximumFluxBC = Math.Max(maximumFluxBC, Math.Abs(result.FluxBC));
            }

            var finalReducedEnergy = ReducedEnergy(
                state, lengthA, lengthB, lengthC,
                materialA, materialB, materialC);
            var reducedEnergyDrift = finalReducedEnergy - initialReducedEnergy;

            const double cellWidth = 0.005;
            var cellCountA = (int)Math.Round(lengthA / cellWidth);
            var cellCountB = (int)Math.Round(lengthB / cellWidth);
            var cellCountC = (int)Math.Round(lengthC / cellWidth);
            var totalCellCount = cellCountA + cellCountB + cellCountC;

            var reference = new double[totalCellCount];
            var conductivity = new double[totalCellCount];
            var heatCapacity = new double[totalCellCount];

            for (var cell = 0; cell < totalCellCount; cell++)
            {
                if (cell < cellCountA)
                {
                    var x = (cell + 0.5) * cellWidth;
                    reference[cell] = InitialGaussian(x);
                    conductivity[cell] = materialA.ThermalConductivity;
                    heatCapacity[cell] = materialA.VolumetricHeatCapacity;
                }
                else if (cell < cellCountA + cellCountB)
                {
                    conductivity[cell] = materialB.ThermalConductivity;
                    heatCapacity[cell] = materialB.VolumetricHeatCapacity;
                }
                else
                {
                    conductivity[cell] = materialC.ThermalConductivity;
                    heatCapacity[cell] = materialC.VolumetricHeatCapacity;
                }
            }

            var initialReferenceEnergy = DiscreteEnergy(
                reference, heatCapacity, cellWidth);

            AdvanceHeterogeneousFiniteVolume(
                reference,
                conductivity,
                heatCapacity,
                cellWidth,
                duration);

            var finalReferenceEnergy = DiscreteEnergy(
                reference, heatCapacity, cellWidth);
            var referenceEnergyDrift = finalReferenceEnergy - initialReferenceEnergy;

            var relativeFieldError = RelativeFieldError(
                state,
                reference,
                cellWidth,
                cellCountA,
                cellCountB,
                cellCountC,
                lengthA,
                lengthB,
                lengthC);

            return new ThreeLayerCoupledVerificationResult1D(
                relativeFieldError,
                maximumJump,
                reducedEnergyDrift,
                referenceEnergyDrift,
                maximumFluxAB,
                maximumFluxBC,
                retainedStateScalars: 3 * (modeCount + 1));
        }

        private static FiniteLayerReducedState1D ProjectToCosineState(
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

        private static double ReducedEnergy(
            in ThreeLayerCoupledState1D state,
            double lengthA,
            double lengthB,
            double lengthC,
            in ThermalMaterial1D materialA,
            in ThermalMaterial1D materialB,
            in ThermalMaterial1D materialC)
        {
            return materialA.VolumetricHeatCapacity * lengthA
                    * state.StateA.MeanTemperaturePerturbation
                + materialB.VolumetricHeatCapacity * lengthB
                    * state.StateB.MeanTemperaturePerturbation
                + materialC.VolumetricHeatCapacity * lengthC
                    * state.StateC.MeanTemperaturePerturbation;
        }

        private static void AdvanceHeterogeneousFiniteVolume(
            double[] temperature,
            double[] conductivity,
            double[] heatCapacity,
            double cellWidth,
            double duration)
        {
            var maximumDiffusivity = 0.0;
            for (var cell = 0; cell < temperature.Length; cell++)
            {
                maximumDiffusivity = Math.Max(
                    maximumDiffusivity,
                    conductivity[cell] / heatCapacity[cell]);
            }

            const double stabilityFactor = 0.35;
            var maximumStep = stabilityFactor * cellWidth * cellWidth
                / maximumDiffusivity;
            var stepCount = Math.Max(1, (int)Math.Ceiling(duration / maximumStep));
            var deltaTime = duration / stepCount;

            var faceFlux = new double[temperature.Length - 1];
            var next = new double[temperature.Length];

            for (var step = 0; step < stepCount; step++)
            {
                for (var face = 0; face < faceFlux.Length; face++)
                {
                    var leftK = conductivity[face];
                    var rightK = conductivity[face + 1];
                    var harmonicK = 2.0 * leftK * rightK / (leftK + rightK);
                    faceFlux[face] = -harmonicK
                        * (temperature[face + 1] - temperature[face])
                        / cellWidth;
                }

                next[0] = temperature[0]
                    - deltaTime * faceFlux[0]
                    / (heatCapacity[0] * cellWidth);

                for (var cell = 1; cell < temperature.Length - 1; cell++)
                {
                    next[cell] = temperature[cell]
                        + deltaTime * (faceFlux[cell - 1] - faceFlux[cell])
                        / (heatCapacity[cell] * cellWidth);
                }

                var last = temperature.Length - 1;
                next[last] = temperature[last]
                    + deltaTime * faceFlux[last - 1]
                    / (heatCapacity[last] * cellWidth);

                Array.Copy(next, temperature, temperature.Length);
            }
        }

        private static double RelativeFieldError(
            in ThreeLayerCoupledState1D state,
            double[] reference,
            double cellWidth,
            int cellCountA,
            int cellCountB,
            int cellCountC,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            for (var cell = 0; cell < reference.Length; cell++)
            {
                double candidate;

                if (cell < cellCountA)
                {
                    var x = (cell + 0.5) * cellWidth;
                    candidate = FiniteLayerFieldRepresentation1D.Evaluate(
                        state.StateA, x, lengthA);
                }
                else if (cell < cellCountA + cellCountB)
                {
                    var localCell = cell - cellCountA;
                    var x = (localCell + 0.5) * cellWidth;
                    candidate = FiniteLayerFieldRepresentation1D.Evaluate(
                        state.StateB, x, lengthB);
                }
                else
                {
                    var localCell = cell - cellCountA - cellCountB;
                    var x = (localCell + 0.5) * cellWidth;
                    candidate = FiniteLayerFieldRepresentation1D.Evaluate(
                        state.StateC, x, lengthC);
                }

                var difference = candidate - reference[cell];
                squaredError += difference * difference;
                squaredReference += reference[cell] * reference[cell];
            }

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static double DiscreteEnergy(
            double[] temperature,
            double[] heatCapacity,
            double cellWidth)
        {
            var total = 0.0;
            for (var cell = 0; cell < temperature.Length; cell++)
            {
                total += heatCapacity[cell] * temperature[cell] * cellWidth;
            }

            return total;
        }
    }
}
