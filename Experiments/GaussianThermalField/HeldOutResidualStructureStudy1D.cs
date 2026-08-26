using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct HeldOutResidualStructureResult1D
    {
        private readonly double[] _maxUnderpredictionByCount;
        private readonly double[] _rmsLogResidualByCount;

        public HeldOutResidualStructureResult1D(
            double correlationWithLogCount,
            double correlationWithLogCurvature,
            double correlationWithLogBoundaryContrast,
            double[] maxUnderpredictionByCount,
            double[] rmsLogResidualByCount,
            int residualPointCount)
        {
            CorrelationWithLogCount = correlationWithLogCount;
            CorrelationWithLogCurvature = correlationWithLogCurvature;
            CorrelationWithLogBoundaryContrast = correlationWithLogBoundaryContrast;
            _maxUnderpredictionByCount = maxUnderpredictionByCount;
            _rmsLogResidualByCount = rmsLogResidualByCount;
            ResidualPointCount = residualPointCount;
        }

        public double CorrelationWithLogCount { get; }
        public double CorrelationWithLogCurvature { get; }
        public double CorrelationWithLogBoundaryContrast { get; }
        public int ResidualPointCount { get; }

        public double GetMaxUnderpredictionByCount(int count)
        {
            if (_maxUnderpredictionByCount is null || count < 1 || count > _maxUnderpredictionByCount.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            return _maxUnderpredictionByCount[count - 1];
        }

        public double GetRmsLogResidualByCount(int count)
        {
            if (_rmsLogResidualByCount is null || count < 1 || count > _rmsLogResidualByCount.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            return _rmsLogResidualByCount[count - 1];
        }

        public bool SatisfiesDataIntegrity()
        {
            if (ResidualPointCount <= 0
                || _maxUnderpredictionByCount is null
                || _rmsLogResidualByCount is null
                || _maxUnderpredictionByCount.Length != 8
                || _rmsLogResidualByCount.Length != 8)
            {
                return false;
            }

            if (!IsFinite(CorrelationWithLogCount)
                || !IsFinite(CorrelationWithLogCurvature)
                || !IsFinite(CorrelationWithLogBoundaryContrast))
            {
                return false;
            }

            for (var i = 0; i < 8; i++)
            {
                if (!IsFinite(_maxUnderpredictionByCount[i])
                    || !IsFinite(_rmsLogResidualByCount[i])
                    || _maxUnderpredictionByCount[i] < 1.0
                    || _rmsLogResidualByCount[i] < 0.0)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsFinite(double value)
            => !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// Correct residual-structure diagnostic. Each residual is evaluated only
    /// on a complete time snapshot excluded from fitting. This avoids the OLS
    /// orthogonality artifact that makes correlations of training residuals with
    /// fitted regressors uninformative.
    /// </summary>
    public static class HeldOutResidualStructureStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumGaussianCount = 8;
        private const int FieldSampleCount = 401;
        private const double PeakZeroGuard = 5e-3;
        private const double ModelL2OmissionThreshold = 1e-3;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

        public static HeldOutResidualStructureResult1D Evaluate()
        {
            var snapshots = BuildSnapshots();
            var residuals = new List<double>();
            var logCounts = new List<double>();
            var logCurvatures = new List<double>();
            var logBoundaryContrasts = new List<double>();
            var maxUnderprediction = new double[MaximumGaussianCount];
            var sumSquaredResidual = new double[MaximumGaussianCount];
            var countByN = new int[MaximumGaussianCount];
            for (var i = 0; i < MaximumGaussianCount; i++)
            {
                maxUnderprediction[i] = 1.0;
            }

            for (var heldOut = 0; heldOut < snapshots.Length; heldOut++)
            {
                var beta = FitModel(snapshots, heldOut);
                AddHeldOutRegion(
                    snapshots[heldOut].A,
                    beta,
                    residuals,
                    logCounts,
                    logCurvatures,
                    logBoundaryContrasts,
                    maxUnderprediction,
                    sumSquaredResidual,
                    countByN);
                AddHeldOutRegion(
                    snapshots[heldOut].B,
                    beta,
                    residuals,
                    logCounts,
                    logCurvatures,
                    logBoundaryContrasts,
                    maxUnderprediction,
                    sumSquaredResidual,
                    countByN);
                AddHeldOutRegion(
                    snapshots[heldOut].C,
                    beta,
                    residuals,
                    logCounts,
                    logCurvatures,
                    logBoundaryContrasts,
                    maxUnderprediction,
                    sumSquaredResidual,
                    countByN);
            }

            var rms = new double[MaximumGaussianCount];
            for (var i = 0; i < MaximumGaussianCount; i++)
            {
                rms[i] = Math.Sqrt(sumSquaredResidual[i] / Math.Max(countByN[i], 1));
            }

            return new HeldOutResidualStructureResult1D(
                Pearson(residuals, logCounts),
                Pearson(residuals, logCurvatures),
                Pearson(residuals, logBoundaryContrasts),
                maxUnderprediction,
                rms,
                residuals.Count);
        }

        private static void AddHeldOutRegion(
            Region region,
            double[] beta,
            List<double> residuals,
            List<double> logCounts,
            List<double> logCurvatures,
            List<double> logBoundaryContrasts,
            double[] maxUnderprediction,
            double[] sumSquaredResidual,
            int[] countByN)
        {
            if (!region.ModelEligible)
            {
                return;
            }

            var logC = Math.Log(1.0 + region.Curvature);
            var logB = Math.Log(1.0 + region.BoundaryContrast);
            for (var i = 0; i < region.Errors.Length; i++)
            {
                var predicted = Math.Exp(PredictLogError(beta, region.Curvature, region.BoundaryContrast, i + 1));
                var ratio = region.Errors[i] / Math.Max(predicted, 1e-30);
                var residual = Math.Log(Math.Max(ratio, 1e-30));
                residuals.Add(residual);
                logCounts.Add(Math.Log(i + 1.0));
                logCurvatures.Add(logC);
                logBoundaryContrasts.Add(logB);
                maxUnderprediction[i] = Math.Max(maxUnderprediction[i], ratio);
                sumSquaredResidual[i] += residual * residual;
                countByN[i]++;
            }
        }

        private static Snapshot[] BuildSnapshots()
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
            var result = new Snapshot[SnapshotTimes.Length];
            var currentTime = 0.0;

            for (var snapshotIndex = 0; snapshotIndex < SnapshotTimes.Length; snapshotIndex++)
            {
                var targetTime = SnapshotTimes[snapshotIndex];
                var stepCount = (int)Math.Round((targetTime - currentTime) / deltaTime);
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

                var norms = MeasureNormsAndPeaks(state, lengthA, lengthB, lengthC);
                result[snapshotIndex] = new Snapshot(
                    BuildRegion(state.StateA, lengthA, norms.WeightA, norms.PeakA),
                    BuildRegion(state.StateB, lengthB, norms.WeightB, norms.PeakB),
                    BuildRegion(state.StateC, lengthC, norms.WeightC, norms.PeakC));
            }
            return result;
        }

        private static Region BuildRegion(
            in FiniteLayerReducedState1D state,
            double length,
            double weight,
            double peak)
        {
            var modalEnergy = 0.0;
            var n4Energy = 0.0;
            for (var modeIndex = 0; modeIndex < state.ModeCount; modeIndex++)
            {
                var n = modeIndex + 1.0;
                var coefficient = state.GetModeCoefficient(modeIndex);
                var energy = coefficient * coefficient;
                modalEnergy += energy;
                n4Energy += n * n * n * n * energy;
            }
            var mean = state.MeanTemperaturePerturbation;
            var fieldEnergy = Math.Max(mean * mean + 0.5 * modalEnergy, 1e-30);
            var curvature = Math.Sqrt(0.5 * n4Energy / fieldEnergy);
            var left = FiniteLayerFieldRepresentation1D.Evaluate(state, 0.0, length);
            var right = FiniteLayerFieldRepresentation1D.Evaluate(state, length, length);
            var boundaryContrast = Math.Abs(left - right) / Math.Sqrt(fieldEnergy);

            var fits = ConstrainedGaussianSparseFitter1D.FitSequence(
                state,
                length,
                MaximumGaussianCount);
            var errors = new double[MaximumGaussianCount];
            for (var i = 0; i < errors.Length; i++)
            {
                errors[i] = Math.Max(fits[i].RelativeError, 1e-15);
            }
            return new Region(curvature, boundaryContrast, weight, peak, errors);
        }

        private static double[] FitModel(Snapshot[] snapshots, int heldOut)
        {
            const int dimension = 6;
            var normal = new double[dimension, dimension];
            var rhs = new double[dimension];
            for (var snapshotIndex = 0; snapshotIndex < snapshots.Length; snapshotIndex++)
            {
                if (snapshotIndex == heldOut)
                {
                    continue;
                }
                AddRegionToNormal(snapshots[snapshotIndex].A, normal, rhs);
                AddRegionToNormal(snapshots[snapshotIndex].B, normal, rhs);
                AddRegionToNormal(snapshots[snapshotIndex].C, normal, rhs);
            }
            return SolveLinearSystem(normal, rhs);
        }

        private static void AddRegionToNormal(Region region, double[,] normal, double[] rhs)
        {
            if (!region.ModelEligible)
            {
                return;
            }
            var u = Math.Log(1.0 + region.Curvature);
            var b = Math.Log(1.0 + region.BoundaryContrast);
            for (var i = 0; i < region.Errors.Length; i++)
            {
                var v = Math.Log(i + 1.0);
                var row = new[] { 1.0, u, b, v, u * v, b * v };
                var target = Math.Log(region.Errors[i]);
                for (var r = 0; r < row.Length; r++)
                {
                    rhs[r] += row[r] * target;
                    for (var c = 0; c < row.Length; c++)
                    {
                        normal[r, c] += row[r] * row[c];
                    }
                }
            }
        }

        private static double PredictLogError(
            double[] beta,
            double curvature,
            double boundaryContrast,
            int count)
        {
            var u = Math.Log(1.0 + curvature);
            var b = Math.Log(1.0 + boundaryContrast);
            var v = Math.Log(count);
            return beta[0] + beta[1] * u + beta[2] * b + beta[3] * v + beta[4] * u * v + beta[5] * b * v;
        }

        private static NormPeak MeasureNormsAndPeaks(
            in ThreeLayerCoupledState1D state,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredA = PhysicalSquaredNorm(state.StateA, lengthA, out var peakA);
            var squaredB = PhysicalSquaredNorm(state.StateB, lengthB, out var peakB);
            var squaredC = PhysicalSquaredNorm(state.StateC, lengthC, out var peakC);
            var globalNorm = Math.Sqrt(Math.Max(squaredA + squaredB + squaredC, 1e-30));
            var globalPeak = Math.Max(Math.Max(peakA, peakB), Math.Max(peakC, 1e-30));
            return new NormPeak(
                Math.Sqrt(squaredA) / globalNorm,
                Math.Sqrt(squaredB) / globalNorm,
                Math.Sqrt(squaredC) / globalNorm,
                peakA / globalPeak,
                peakB / globalPeak,
                peakC / globalPeak);
        }

        private static double PhysicalSquaredNorm(
            in FiniteLayerReducedState1D state,
            double length,
            out double peak)
        {
            var dx = length / FieldSampleCount;
            var sum = 0.0;
            peak = 0.0;
            for (var i = 0; i < FieldSampleCount; i++)
            {
                var x = (i + 0.5) * dx;
                var value = FiniteLayerFieldRepresentation1D.Evaluate(state, x, length);
                sum += value * value * dx;
                peak = Math.Max(peak, Math.Abs(value));
            }
            return sum;
        }

        private static FiniteLayerReducedState1D ProjectFieldToState(
            Func<double, double> field,
            double length,
            int modeCount)
        {
            const int intervals = 8192;
            var dx = length / intervals;
            var meanIntegral = 0.0;
            var modeIntegrals = new double[modeCount];
            for (var i = 0; i <= intervals; i++)
            {
                var x = i * dx;
                var weight = i == 0 || i == intervals ? 0.5 : 1.0;
                var value = field(x);
                meanIntegral += weight * value;
                for (var modeIndex = 0; modeIndex < modeCount; modeIndex++)
                {
                    var n = modeIndex + 1.0;
                    modeIntegrals[modeIndex] += weight * value * Math.Cos(n * Math.PI * x / length);
                }
            }
            meanIntegral *= dx;
            var coefficients = new double[modeCount];
            for (var i = 0; i < modeCount; i++)
            {
                coefficients[i] = 2.0 * modeIntegrals[i] * dx / length;
            }
            return new FiniteLayerReducedState1D(meanIntegral / length, coefficients);
        }

        private static double[] SolveLinearSystem(double[,] matrix, double[] rhs)
        {
            var n = rhs.Length;
            var augmented = new double[n, n + 1];
            for (var r = 0; r < n; r++)
            {
                for (var c = 0; c < n; c++)
                {
                    augmented[r, c] = matrix[r, c];
                }
                augmented[r, n] = rhs[r];
            }

            for (var pivot = 0; pivot < n; pivot++)
            {
                var best = pivot;
                var bestMagnitude = Math.Abs(augmented[pivot, pivot]);
                for (var r = pivot + 1; r < n; r++)
                {
                    var magnitude = Math.Abs(augmented[r, pivot]);
                    if (magnitude > bestMagnitude)
                    {
                        best = r;
                        bestMagnitude = magnitude;
                    }
                }
                if (bestMagnitude < 1e-14)
                {
                    throw new InvalidOperationException("Held-out residual normal equation is singular.");
                }
                if (best != pivot)
                {
                    for (var c = pivot; c <= n; c++)
                    {
                        (augmented[pivot, c], augmented[best, c]) = (augmented[best, c], augmented[pivot, c]);
                    }
                }
                var scale = augmented[pivot, pivot];
                for (var c = pivot; c <= n; c++)
                {
                    augmented[pivot, c] /= scale;
                }
                for (var r = 0; r < n; r++)
                {
                    if (r == pivot)
                    {
                        continue;
                    }
                    var factor = augmented[r, pivot];
                    for (var c = pivot; c <= n; c++)
                    {
                        augmented[r, c] -= factor * augmented[pivot, c];
                    }
                }
            }
            var solution = new double[n];
            for (var i = 0; i < n; i++)
            {
                solution[i] = augmented[i, n];
            }
            return solution;
        }

        private static double Pearson(List<double> x, List<double> y)
        {
            if (x.Count != y.Count || x.Count == 0)
            {
                return 0.0;
            }
            var meanX = 0.0;
            var meanY = 0.0;
            for (var i = 0; i < x.Count; i++)
            {
                meanX += x[i];
                meanY += y[i];
            }
            meanX /= x.Count;
            meanY /= y.Count;
            var numerator = 0.0;
            var sx = 0.0;
            var sy = 0.0;
            for (var i = 0; i < x.Count; i++)
            {
                var dx = x[i] - meanX;
                var dy = y[i] - meanY;
                numerator += dx * dy;
                sx += dx * dx;
                sy += dy * dy;
            }
            var denominator = Math.Sqrt(sx * sy);
            return denominator > 0.0 ? numerator / denominator : 0.0;
        }

        private readonly struct Snapshot
        {
            public Snapshot(Region a, Region b, Region c)
            {
                A = a;
                B = b;
                C = c;
            }
            public Region A { get; }
            public Region B { get; }
            public Region C { get; }
        }

        private readonly struct Region
        {
            public Region(double curvature, double boundaryContrast, double weight, double peak, double[] errors)
            {
                Curvature = curvature;
                BoundaryContrast = boundaryContrast;
                Weight = weight;
                Peak = peak;
                Errors = errors;
            }
            public double Curvature { get; }
            public double BoundaryContrast { get; }
            public double Weight { get; }
            public double Peak { get; }
            public double[] Errors { get; }
            public bool ModelEligible => !(Weight <= ModelL2OmissionThreshold && Peak <= PeakZeroGuard);
        }

        private readonly struct NormPeak
        {
            public NormPeak(double weightA, double weightB, double weightC, double peakA, double peakB, double peakC)
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
