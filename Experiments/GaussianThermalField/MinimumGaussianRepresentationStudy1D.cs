using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct MinimumGaussianRepresentationPoint1D
    {
        public MinimumGaussianRepresentationPoint1D(
            int kernelsPerRegion,
            double globalRelativeErrorVsState,
            double maximumRegionRelativeErrorVsState,
            double relativeErrorVsFiniteVolume,
            double maximumRegionIntegralError)
        {
            KernelsPerRegion = kernelsPerRegion;
            GlobalRelativeErrorVsState = globalRelativeErrorVsState;
            MaximumRegionRelativeErrorVsState = maximumRegionRelativeErrorVsState;
            RelativeErrorVsFiniteVolume = relativeErrorVsFiniteVolume;
            MaximumRegionIntegralError = maximumRegionIntegralError;
        }

        public int KernelsPerRegion { get; }
        public int TotalKernelCount => 3 * KernelsPerRegion;
        public double GlobalRelativeErrorVsState { get; }
        public double MaximumRegionRelativeErrorVsState { get; }
        public double RelativeErrorVsFiniteVolume { get; }
        public double MaximumRegionIntegralError { get; }
    }

    public readonly struct MinimumGaussianRepresentationStudyResult1D
    {
        private readonly MinimumGaussianRepresentationPoint1D[] _points;

        public MinimumGaussianRepresentationStudyResult1D(
            MinimumGaussianRepresentationPoint1D[] points,
            int firstGlobalCountBelowHalfPercent,
            int firstEveryRegionCountBelowHalfPercent)
        {
            _points = points;
            FirstGlobalCountBelowHalfPercent = firstGlobalCountBelowHalfPercent;
            FirstEveryRegionCountBelowHalfPercent = firstEveryRegionCountBelowHalfPercent;
        }

        public int Count => _points?.Length ?? 0;
        public int FirstGlobalCountBelowHalfPercent { get; }
        public int FirstEveryRegionCountBelowHalfPercent { get; }

        public MinimumGaussianRepresentationPoint1D GetPoint(int index)
        {
            if (_points is null || index < 0 || index >= _points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _points[index];
        }

        public bool Satisfies(
            double maximumEightKernelGlobalError,
            double maximumEightKernelFiniteVolumeError,
            double maximumIntegralError)
        {
            if (Count != 8)
            {
                return false;
            }

            for (var index = 1; index < Count; index++)
            {
                if (_points[index].GlobalRelativeErrorVsState
                    > _points[index - 1].GlobalRelativeErrorVsState + 1e-12)
                {
                    return false;
                }
            }

            var last = _points[Count - 1];
            return last.GlobalRelativeErrorVsState <= maximumEightKernelGlobalError
                && last.RelativeErrorVsFiniteVolume <= maximumEightKernelFiniteVolumeError
                && last.MaximumRegionIntegralError <= maximumIntegralError
                && FirstGlobalCountBelowHalfPercent > 0
                && FirstEveryRegionCountBelowHalfPercent > 0;
        }
    }

    /// <summary>
    /// Checkpoint 7: starts at the non-trivial theoretical count floor of one
    /// signed Gaussian per non-zero region and increases the retained count one
    /// term at a time.
    ///
    /// The fitter uses a bounded candidate dictionary and greedy sparse
    /// selection. For every candidate count, all selected amplitudes are
    /// re-solved with an equality constraint on the region integral. No extra
    /// energy-correction Gaussian is appended.
    ///
    /// This is an empirical sparse-approximation study inside the declared
    /// dictionary; it is not a proof of the globally optimal Gaussian mixture.
    /// </summary>
    public static class MinimumGaussianRepresentationStudy1D
    {
        private const int ModeCount = 32;
        private const int FitSampleCount = 401;
        private const int MaximumKernelCount = 8;

        private static readonly double[] CenterScales = CreateCenterScales();
        private static readonly double[] SigmaScales =
        {
            0.06, 0.08, 0.10, 0.14, 0.20, 0.28,
            0.40, 0.60, 0.90, 1.30, 2.00
        };

        public static MinimumGaussianRepresentationStudyResult1D Evaluate()
        {
            var materialA = new ThermalMaterial1D(0.40, 2.0);
            var materialB = new ThermalMaterial1D(0.06, 1.2);
            var materialC = new ThermalMaterial1D(0.20, 1.6);

            const double lengthA = 0.60;
            const double lengthB = 0.35;
            const double lengthC = 0.60;
            const double reducedDeltaTime = 0.002;
            const double duration = 0.60;

            static double InitialField(double x)
            {
                const double mean = 0.46;
                const double standardDeviation = 0.05;
                var z = (x - mean) / standardDeviation;
                return Math.Exp(-0.5 * z * z);
            }

            var state = new ThreeLayerCoupledState1D(
                ProjectFieldToState(InitialField, lengthA, ModeCount),
                FiniteLayerReducedState1D.Zero(ModeCount),
                FiniteLayerReducedState1D.Zero(ModeCount));

            var reducedStepCount = (int)Math.Round(duration / reducedDeltaTime);
            for (var step = 0; step < reducedStepCount; step++)
            {
                state = ThreeLayerCoupledEvolution1D.Advance(
                    state,
                    reducedDeltaTime,
                    lengthA,
                    lengthB,
                    lengthC,
                    materialA,
                    materialB,
                    materialC).State;
            }

            var fitsA = FitSequence(state.StateA, lengthA, MaximumKernelCount);
            var fitsB = FitSequence(state.StateB, lengthB, MaximumKernelCount);
            var fitsC = FitSequence(state.StateC, lengthC, MaximumKernelCount);

            var finiteVolume = CreateFiniteVolumeReference(
                InitialField,
                duration,
                lengthA,
                lengthB,
                lengthC,
                materialA,
                materialB,
                materialC,
                out var cellWidth,
                out var cellCountA,
                out var cellCountB,
                out var cellCountC);

            var points = new MinimumGaussianRepresentationPoint1D[MaximumKernelCount];
            var firstGlobal = 0;
            var firstEveryRegion = 0;
            const double halfPercent = 5e-3;

            for (var count = 1; count <= MaximumKernelCount; count++)
            {
                var fitA = fitsA[count - 1];
                var fitB = fitsB[count - 1];
                var fitC = fitsC[count - 1];

                var globalStateError = GlobalRelativeStateError(
                    state,
                    fitA.Mixture,
                    fitB.Mixture,
                    fitC.Mixture,
                    lengthA,
                    lengthB,
                    lengthC);

                var maximumRegionError = Math.Max(
                    fitA.RelativeError,
                    Math.Max(fitB.RelativeError, fitC.RelativeError));

                var finiteVolumeError = RelativeFiniteVolumeError(
                    fitA.Mixture,
                    fitB.Mixture,
                    fitC.Mixture,
                    finiteVolume,
                    cellWidth,
                    cellCountA,
                    cellCountB,
                    cellCountC);

                var maximumIntegralError = Math.Max(
                    fitA.IntegralError,
                    Math.Max(fitB.IntegralError, fitC.IntegralError));

                points[count - 1] = new MinimumGaussianRepresentationPoint1D(
                    count,
                    globalStateError,
                    maximumRegionError,
                    finiteVolumeError,
                    maximumIntegralError);

                if (firstGlobal == 0 && globalStateError <= halfPercent)
                {
                    firstGlobal = count;
                }

                if (firstEveryRegion == 0 && maximumRegionError <= halfPercent)
                {
                    firstEveryRegion = count;
                }
            }

            return new MinimumGaussianRepresentationStudyResult1D(
                points,
                firstGlobal,
                firstEveryRegion);
        }

        private static SparseFit[] FitSequence(
            in FiniteLayerReducedState1D state,
            double layerLength,
            int maximumCount)
        {
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
            var results = new SparseFit[maximumCount];
            var targetIntegral = state.MeanTemperaturePerturbation * layerLength;

            for (var count = 1; count <= maximumCount; count++)
            {
                var bestError = double.PositiveInfinity;
                var bestCandidate = -1;
                double[]? bestAmplitudes = null;

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

                    var amplitudes = SolveConstrainedAmplitudes(
                        candidates,
                        trial,
                        target,
                        targetIntegral);
                    var error = RelativeSampleError(
                        candidates,
                        trial,
                        amplitudes,
                        target);

                    if (error < bestError)
                    {
                        bestError = error;
                        bestCandidate = candidateIndex;
                        bestAmplitudes = amplitudes;
                    }
                }

                if (bestCandidate < 0 || bestAmplitudes is null)
                {
                    throw new InvalidOperationException(
                        "No valid constrained Gaussian sparse fit was found.");
                }

                selected.Add(bestCandidate);

                // Re-solve after the greedy selection so every retained count
                // satisfies the integral equality as one coupled fit.
                var selectedArray = selected.ToArray();
                var amplitudesFinal = SolveConstrainedAmplitudes(
                    candidates,
                    selectedArray,
                    target,
                    targetIntegral);

                var kernels = new GaussianKernel1D[selectedArray.Length];
                for (var index = 0; index < selectedArray.Length; index++)
                {
                    var candidate = candidates[selectedArray[index]];
                    kernels[index] = new GaussianKernel1D(
                        candidate.Mean,
                        candidate.Variance,
                        amplitudesFinal[index]);
                }

                var mixture = new GaussianMixture1D(kernels);
                var relativeError = RelativeSampleError(
                    candidates,
                    selectedArray,
                    amplitudesFinal,
                    target);
                var representedIntegral = 0.0;
                for (var index = 0; index < selectedArray.Length; index++)
                {
                    representedIntegral += amplitudesFinal[index]
                        * candidates[selectedArray[index]].UnitIntegral;
                }

                results[count - 1] = new SparseFit(
                    mixture,
                    relativeError,
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

        private static double GlobalRelativeStateError(
            in ThreeLayerCoupledState1D state,
            in GaussianMixture1D mixtureA,
            in GaussianMixture1D mixtureB,
            in GaussianMixture1D mixtureC,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            AccumulateStateError(
                state.StateA, mixtureA, lengthA,
                ref squaredError, ref squaredReference);
            AccumulateStateError(
                state.StateB, mixtureB, lengthB,
                ref squaredError, ref squaredReference);
            AccumulateStateError(
                state.StateC, mixtureC, lengthC,
                ref squaredError, ref squaredReference);

            return Math.Sqrt(squaredError / squaredReference);
        }

        private static void AccumulateStateError(
            in FiniteLayerReducedState1D state,
            in GaussianMixture1D mixture,
            double layerLength,
            ref double squaredError,
            ref double squaredReference)
        {
            for (var sample = 0; sample < FitSampleCount; sample++)
            {
                var x = (sample + 0.5) * layerLength / FitSampleCount;
                var reference = FiniteLayerFieldRepresentation1D.Evaluate(
                    state, x, layerLength);
                var difference = mixture.Evaluate(x) - reference;
                squaredError += difference * difference;
                squaredReference += reference * reference;
            }
        }

        private static double[] CreateFiniteVolumeReference(
            Func<double, double> initialField,
            double duration,
            double lengthA,
            double lengthB,
            double lengthC,
            in ThermalMaterial1D materialA,
            in ThermalMaterial1D materialB,
            in ThermalMaterial1D materialC,
            out double cellWidth,
            out int cellCountA,
            out int cellCountB,
            out int cellCountC)
        {
            cellWidth = 0.005;
            cellCountA = (int)Math.Round(lengthA / cellWidth);
            cellCountB = (int)Math.Round(lengthB / cellWidth);
            cellCountC = (int)Math.Round(lengthC / cellWidth);
            var totalCellCount = cellCountA + cellCountB + cellCountC;

            var temperature = new double[totalCellCount];
            var conductivity = new double[totalCellCount];
            var heatCapacity = new double[totalCellCount];

            for (var cell = 0; cell < totalCellCount; cell++)
            {
                if (cell < cellCountA)
                {
                    var x = (cell + 0.5) * cellWidth;
                    temperature[cell] = initialField(x);
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

            var maximumDiffusivity = 0.0;
            for (var cell = 0; cell < totalCellCount; cell++)
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
            var faceFlux = new double[totalCellCount - 1];
            var next = new double[totalCellCount];

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

                for (var cell = 1; cell < totalCellCount - 1; cell++)
                {
                    next[cell] = temperature[cell]
                        + deltaTime * (faceFlux[cell - 1] - faceFlux[cell])
                        / (heatCapacity[cell] * cellWidth);
                }

                var last = totalCellCount - 1;
                next[last] = temperature[last]
                    + deltaTime * faceFlux[last - 1]
                    / (heatCapacity[last] * cellWidth);

                Array.Copy(next, temperature, totalCellCount);
            }

            return temperature;
        }

        private static double RelativeFiniteVolumeError(
            in GaussianMixture1D mixtureA,
            in GaussianMixture1D mixtureB,
            in GaussianMixture1D mixtureC,
            double[] reference,
            double cellWidth,
            int cellCountA,
            int cellCountB,
            int cellCountC)
        {
            var squaredError = 0.0;
            var squaredReference = 0.0;

            for (var cell = 0; cell < reference.Length; cell++)
            {
                double candidate;
                if (cell < cellCountA)
                {
                    candidate = mixtureA.Evaluate((cell + 0.5) * cellWidth);
                }
                else if (cell < cellCountA + cellCountB)
                {
                    var localCell = cell - cellCountA;
                    candidate = mixtureB.Evaluate((localCell + 0.5) * cellWidth);
                }
                else
                {
                    var localCell = cell - cellCountA - cellCountB;
                    candidate = mixtureC.Evaluate((localCell + 0.5) * cellWidth);
                }

                var difference = candidate - reference[cell];
                squaredError += difference * difference;
                squaredReference += reference[cell] * reference[cell];
            }

            return Math.Sqrt(squaredError / squaredReference);
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

            var solution = new double[size];
            for (var row = 0; row < size; row++)
            {
                solution[row] = augmented[row, size];
            }
            return solution;
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

        private readonly struct SparseFit
        {
            public SparseFit(
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
    }
}
