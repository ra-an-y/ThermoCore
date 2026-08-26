using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct ResidualUncertaintyFold1D
    {
        public ResidualUncertaintyFold1D(
            double time,
            double globalSafety,
            bool globalFeasible,
            int globalA,
            int globalB,
            int globalC,
            double globalActual,
            bool countFeasible,
            int countA,
            int countB,
            int countC,
            double countPredicted,
            double countActual,
            int oracleA,
            int oracleB,
            int oracleC)
        {
            Time = time;
            GlobalSafety = globalSafety;
            GlobalFeasible = globalFeasible;
            GlobalA = globalA;
            GlobalB = globalB;
            GlobalC = globalC;
            GlobalActual = globalActual;
            CountFeasible = countFeasible;
            CountA = countA;
            CountB = countB;
            CountC = countC;
            CountPredicted = countPredicted;
            CountActual = countActual;
            OracleA = oracleA;
            OracleB = oracleB;
            OracleC = oracleC;
        }

        public double Time { get; }
        public double GlobalSafety { get; }
        public bool GlobalFeasible { get; }
        public int GlobalA { get; }
        public int GlobalB { get; }
        public int GlobalC { get; }
        public int GlobalTotal => GlobalFeasible ? GlobalA + GlobalB + GlobalC : -1;
        public double GlobalActual { get; }
        public bool CountFeasible { get; }
        public int CountA { get; }
        public int CountB { get; }
        public int CountC { get; }
        public int CountTotal => CountFeasible ? CountA + CountB + CountC : -1;
        public double CountPredicted { get; }
        public double CountActual { get; }
        public int OracleA { get; }
        public int OracleB { get; }
        public int OracleC { get; }
        public int OracleTotal => OracleA + OracleB + OracleC;
        public int CountOverhead => CountFeasible ? CountTotal - OracleTotal : -1;
    }

    public readonly struct PredictionResidualUncertaintyResult1D
    {
        private readonly ResidualUncertaintyFold1D[] _folds;
        private readonly double[] _meanCountSafety;
        private readonly double[] _maxCountSafety;

        public PredictionResidualUncertaintyResult1D(
            ResidualUncertaintyFold1D[] folds,
            double residualCorrelationWithLogCount,
            double residualCorrelationWithLogCurvature,
            double residualCorrelationWithLogBoundaryContrast,
            double[] meanCountSafety,
            double[] maxCountSafety,
            int countFeasibleFolds,
            int countSafeFolds,
            int maximumCountOverhead)
        {
            _folds = folds;
            ResidualCorrelationWithLogCount = residualCorrelationWithLogCount;
            ResidualCorrelationWithLogCurvature = residualCorrelationWithLogCurvature;
            ResidualCorrelationWithLogBoundaryContrast = residualCorrelationWithLogBoundaryContrast;
            _meanCountSafety = meanCountSafety;
            _maxCountSafety = maxCountSafety;
            CountFeasibleFolds = countFeasibleFolds;
            CountSafeFolds = countSafeFolds;
            MaximumCountOverhead = maximumCountOverhead;
        }

        public int FoldCount => _folds?.Length ?? 0;
        public double ResidualCorrelationWithLogCount { get; }
        public double ResidualCorrelationWithLogCurvature { get; }
        public double ResidualCorrelationWithLogBoundaryContrast { get; }
        public int CountFeasibleFolds { get; }
        public int CountSafeFolds { get; }
        public int MaximumCountOverhead { get; }

        public ResidualUncertaintyFold1D GetFold(int index)
        {
            if (_folds is null || index < 0 || index >= _folds.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _folds[index];
        }

        public double GetMeanCountSafety(int count)
        {
            if (_meanCountSafety is null || count < 1 || count > _meanCountSafety.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            return _meanCountSafety[count - 1];
        }

        public double GetMaximumCountSafety(int count)
        {
            if (_maxCountSafety is null || count < 1 || count > _maxCountSafety.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            return _maxCountSafety[count - 1];
        }

        public bool SatisfiesDataIntegrity()
        {
            if (FoldCount < 4
                || _meanCountSafety is null
                || _maxCountSafety is null
                || _meanCountSafety.Length != _maxCountSafety.Length
                || _meanCountSafety.Length < 4)
            {
                return false;
            }

            if (!IsFinite(ResidualCorrelationWithLogCount)
                || !IsFinite(ResidualCorrelationWithLogCurvature)
                || !IsFinite(ResidualCorrelationWithLogBoundaryContrast))
            {
                return false;
            }

            for (var i = 0; i < _meanCountSafety.Length; i++)
            {
                if (!IsFinite(_meanCountSafety[i])
                    || !IsFinite(_maxCountSafety[i])
                    || _meanCountSafety[i] < 1.0
                    || _maxCountSafety[i] < 1.0)
                {
                    return false;
                }
            }

            for (var i = 0; i < FoldCount; i++)
            {
                var fold = _folds[i];
                if (!IsFinite(fold.GlobalSafety) || fold.GlobalSafety < 1.0 || fold.OracleTotal <= 0)
                {
                    return false;
                }
                if (fold.CountFeasible
                    && (fold.CountTotal <= 0
                        || !IsFinite(fold.CountPredicted)
                        || !IsFinite(fold.CountActual)))
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
    /// Examines the residual structure of the compact regional error predictor.
    /// The first uncertainty model is deliberately minimal: a training-only
    /// multiplicative safety factor is calibrated separately for each Gaussian
    /// count N=1..8. This is compared against the prior single worst-case factor.
    /// </summary>
    public static class PredictionResidualUncertaintyStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumGaussianCount = 8;
        private const int FieldSampleCount = 401;
        private const double GlobalErrorThreshold = 5e-3;
        private const double PeakZeroGuard = 5e-3;
        private const double ModelL2OmissionThreshold = 1e-3;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

        public static PredictionResidualUncertaintyResult1D Evaluate()
        {
            var snapshots = BuildSnapshots();
            var folds = new ResidualUncertaintyFold1D[snapshots.Length];
            var residuals = new List<double>();
            var logCounts = new List<double>();
            var logCurvatures = new List<double>();
            var logBoundaryContrasts = new List<double>();
            var countSafetySum = new double[MaximumGaussianCount];
            var countSafetyMax = new double[MaximumGaussianCount];
            var countFeasible = 0;
            var countSafe = 0;
            var maximumCountOverhead = 0;

            for (var i = 0; i < countSafetyMax.Length; i++)
            {
                countSafetyMax[i] = 1.0;
            }

            for (var heldOut = 0; heldOut < snapshots.Length; heldOut++)
            {
                var beta = FitModel(snapshots, heldOut);
                var globalSafety = 1.0;
                var countSafety = new double[MaximumGaussianCount];
                for (var i = 0; i < countSafety.Length; i++)
                {
                    countSafety[i] = 1.0;
                }

                CollectTrainingResiduals(
                    snapshots,
                    heldOut,
                    beta,
                    countSafety,
                    ref globalSafety,
                    residuals,
                    logCounts,
                    logCurvatures,
                    logBoundaryContrasts);

                for (var i = 0; i < countSafety.Length; i++)
                {
                    countSafetySum[i] += countSafety[i];
                    countSafetyMax[i] = Math.Max(countSafetyMax[i], countSafety[i]);
                }

                var snapshot = snapshots[heldOut];
                var globalAllocation = SelectAllocation(
                    snapshot,
                    beta,
                    count => globalSafety);
                var countAllocation = SelectAllocation(
                    snapshot,
                    beta,
                    count => countSafety[count - 1]);
                var oracle = SelectOracleAllocation(snapshot);

                var globalActual = globalAllocation.Feasible
                    ? ActualGlobalError(snapshot, globalAllocation.A, globalAllocation.B, globalAllocation.C)
                    : double.NaN;
                var countActual = countAllocation.Feasible
                    ? ActualGlobalError(snapshot, countAllocation.A, countAllocation.B, countAllocation.C)
                    : double.NaN;

                if (countAllocation.Feasible)
                {
                    countFeasible++;
                    if (countActual <= GlobalErrorThreshold)
                    {
                        countSafe++;
                    }
                    maximumCountOverhead = Math.Max(
                        maximumCountOverhead,
                        countAllocation.Total - oracle.Total);
                }

                folds[heldOut] = new ResidualUncertaintyFold1D(
                    snapshot.Time,
                    globalSafety,
                    globalAllocation.Feasible,
                    globalAllocation.A,
                    globalAllocation.B,
                    globalAllocation.C,
                    globalActual,
                    countAllocation.Feasible,
                    countAllocation.A,
                    countAllocation.B,
                    countAllocation.C,
                    countAllocation.PredictedGlobalError,
                    countActual,
                    oracle.A,
                    oracle.B,
                    oracle.C);
            }

            var meanSafety = new double[MaximumGaussianCount];
            for (var i = 0; i < meanSafety.Length; i++)
            {
                meanSafety[i] = countSafetySum[i] / snapshots.Length;
            }

            return new PredictionResidualUncertaintyResult1D(
                folds,
                Pearson(residuals, logCounts),
                Pearson(residuals, logCurvatures),
                Pearson(residuals, logBoundaryContrasts),
                meanSafety,
                countSafetyMax,
                countFeasible,
                countSafe,
                maximumCountOverhead);
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
            var snapshots = new Snapshot[SnapshotTimes.Length];
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
                snapshots[snapshotIndex] = new Snapshot(
                    targetTime,
                    BuildRegion(state.StateA, lengthA, norms.WeightA, norms.PeakA),
                    BuildRegion(state.StateB, lengthB, norms.WeightB, norms.PeakB),
                    BuildRegion(state.StateC, lengthC, norms.WeightC, norms.PeakC));
            }
            return snapshots;
        }

        private static Region BuildRegion(
            in FiniteLayerReducedState1D state,
            double length,
            double weight,
            double peak)
        {
            var metrics = ComputeMetrics(state, length);
            var fits = ConstrainedGaussianSparseFitter1D.FitSequence(
                state,
                length,
                MaximumGaussianCount);
            var errors = new double[MaximumGaussianCount];
            for (var i = 0; i < errors.Length; i++)
            {
                errors[i] = Math.Max(fits[i].RelativeError, 1e-15);
            }
            return new Region(metrics, weight, peak, errors);
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

            var u = Math.Log(1.0 + region.Metrics.Curvature);
            var b = Math.Log(1.0 + region.Metrics.BoundaryContrast);
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

        private static void CollectTrainingResiduals(
            Snapshot[] snapshots,
            int heldOut,
            double[] beta,
            double[] countSafety,
            ref double globalSafety,
            List<double> residuals,
            List<double> logCounts,
            List<double> logCurvatures,
            List<double> logBoundaryContrasts)
        {
            for (var snapshotIndex = 0; snapshotIndex < snapshots.Length; snapshotIndex++)
            {
                if (snapshotIndex == heldOut)
                {
                    continue;
                }

                CollectRegionResiduals(
                    snapshots[snapshotIndex].A,
                    beta,
                    countSafety,
                    ref globalSafety,
                    residuals,
                    logCounts,
                    logCurvatures,
                    logBoundaryContrasts);
                CollectRegionResiduals(
                    snapshots[snapshotIndex].B,
                    beta,
                    countSafety,
                    ref globalSafety,
                    residuals,
                    logCounts,
                    logCurvatures,
                    logBoundaryContrasts);
                CollectRegionResiduals(
                    snapshots[snapshotIndex].C,
                    beta,
                    countSafety,
                    ref globalSafety,
                    residuals,
                    logCounts,
                    logCurvatures,
                    logBoundaryContrasts);
            }
        }

        private static void CollectRegionResiduals(
            Region region,
            double[] beta,
            double[] countSafety,
            ref double globalSafety,
            List<double> residuals,
            List<double> logCounts,
            List<double> logCurvatures,
            List<double> logBoundaryContrasts)
        {
            if (!region.ModelEligible)
            {
                return;
            }

            var logC = Math.Log(1.0 + region.Metrics.Curvature);
            var logB = Math.Log(1.0 + region.Metrics.BoundaryContrast);
            for (var i = 0; i < region.Errors.Length; i++)
            {
                var predicted = Math.Exp(PredictLogError(beta, region.Metrics, i + 1));
                var ratio = predicted > 0.0 ? region.Errors[i] / predicted : 1.0;
                var safeRatio = Math.Max(1.0, ratio);
                countSafety[i] = Math.Max(countSafety[i], safeRatio);
                globalSafety = Math.Max(globalSafety, safeRatio);

                residuals.Add(Math.Log(Math.Max(ratio, 1e-15)));
                logCounts.Add(Math.Log(i + 1.0));
                logCurvatures.Add(logC);
                logBoundaryContrasts.Add(logB);
            }
        }

        private static Allocation SelectAllocation(
            Snapshot snapshot,
            double[] beta,
            Func<int, double> safetyForCount)
        {
            var best = Allocation.Infeasible;
            for (var countA = 0; countA <= MaximumGaussianCount; countA++)
            {
                if (countA == 0 && snapshot.A.Peak > PeakZeroGuard)
                {
                    continue;
                }
                for (var countB = 0; countB <= MaximumGaussianCount; countB++)
                {
                    if (countB == 0 && snapshot.B.Peak > PeakZeroGuard)
                    {
                        continue;
                    }
                    for (var countC = 0; countC <= MaximumGaussianCount; countC++)
                    {
                        if (countC == 0 && snapshot.C.Peak > PeakZeroGuard)
                        {
                            continue;
                        }

                        var total = countA + countB + countC;
                        if (total == 0 || (best.Feasible && total > best.Total))
                        {
                            continue;
                        }

                        var errorA = FormulaRegionalError(snapshot.A, countA, beta, safetyForCount);
                        var errorB = FormulaRegionalError(snapshot.B, countB, beta, safetyForCount);
                        var errorC = FormulaRegionalError(snapshot.C, countC, beta, safetyForCount);
                        var global = Math.Sqrt(
                            Square(snapshot.A.Weight * errorA)
                            + Square(snapshot.B.Weight * errorB)
                            + Square(snapshot.C.Weight * errorC));

                        if (global > GlobalErrorThreshold)
                        {
                            continue;
                        }

                        if (!best.Feasible
                            || total < best.Total
                            || (total == best.Total && global < best.PredictedGlobalError))
                        {
                            best = new Allocation(countA, countB, countC, global, true);
                        }
                    }
                }
            }
            return best;
        }

        private static Allocation SelectOracleAllocation(Snapshot snapshot)
        {
            var best = Allocation.Infeasible;
            for (var countA = 0; countA <= MaximumGaussianCount; countA++)
            {
                if (countA == 0 && snapshot.A.Peak > PeakZeroGuard)
                {
                    continue;
                }
                for (var countB = 0; countB <= MaximumGaussianCount; countB++)
                {
                    if (countB == 0 && snapshot.B.Peak > PeakZeroGuard)
                    {
                        continue;
                    }
                    for (var countC = 0; countC <= MaximumGaussianCount; countC++)
                    {
                        if (countC == 0 && snapshot.C.Peak > PeakZeroGuard)
                        {
                            continue;
                        }

                        var total = countA + countB + countC;
                        if (total == 0 || (best.Feasible && total > best.Total))
                        {
                            continue;
                        }

                        var global = ActualGlobalError(snapshot, countA, countB, countC);
                        if (global > GlobalErrorThreshold)
                        {
                            continue;
                        }

                        if (!best.Feasible
                            || total < best.Total
                            || (total == best.Total && global < best.PredictedGlobalError))
                        {
                            best = new Allocation(countA, countB, countC, global, true);
                        }
                    }
                }
            }
            return best;
        }

        private static double FormulaRegionalError(
            Region region,
            int count,
            double[] beta,
            Func<int, double> safetyForCount)
        {
            if (count == 0)
            {
                return 1.0;
            }
            var predicted = Math.Exp(PredictLogError(beta, region.Metrics, count));
            return safetyForCount(count) * predicted;
        }

        private static double ActualGlobalError(Snapshot snapshot, int a, int b, int c)
        {
            var errorA = a == 0 ? 1.0 : snapshot.A.Errors[a - 1];
            var errorB = b == 0 ? 1.0 : snapshot.B.Errors[b - 1];
            var errorC = c == 0 ? 1.0 : snapshot.C.Errors[c - 1];
            return Math.Sqrt(
                Square(snapshot.A.Weight * errorA)
                + Square(snapshot.B.Weight * errorB)
                + Square(snapshot.C.Weight * errorC));
        }

        private static double PredictLogError(double[] beta, MetricValues metrics, int count)
        {
            var u = Math.Log(1.0 + metrics.Curvature);
            var b = Math.Log(1.0 + metrics.BoundaryContrast);
            var v = Math.Log(count);
            return beta[0]
                + beta[1] * u
                + beta[2] * b
                + beta[3] * v
                + beta[4] * u * v
                + beta[5] * b * v;
        }

        private static MetricValues ComputeMetrics(in FiniteLayerReducedState1D state, double length)
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
            return new MetricValues(curvature, boundaryContrast);
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
            var globalSquared = Math.Max(squaredA + squaredB + squaredC, 1e-30);
            var globalNorm = Math.Sqrt(globalSquared);
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
            var sum = 0.0;
            peak = 0.0;
            var dx = length / FieldSampleCount;
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
                    throw new InvalidOperationException("Residual-study normal equation is singular.");
                }
                if (best != pivot)
                {
                    for (var c = pivot; c <= n; c++)
                    {
                        (augmented[pivot, c], augmented[best, c])
                            = (augmented[best, c], augmented[pivot, c]);
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
            var sumX = 0.0;
            var sumY = 0.0;
            for (var i = 0; i < x.Count; i++)
            {
                var dx = x[i] - meanX;
                var dy = y[i] - meanY;
                numerator += dx * dy;
                sumX += dx * dx;
                sumY += dy * dy;
            }
            var denominator = Math.Sqrt(sumX * sumY);
            return denominator > 0.0 ? numerator / denominator : 0.0;
        }

        private static double Square(double value) => value * value;

        private readonly struct Snapshot
        {
            public Snapshot(double time, Region a, Region b, Region c)
            {
                Time = time;
                A = a;
                B = b;
                C = c;
            }
            public double Time { get; }
            public Region A { get; }
            public Region B { get; }
            public Region C { get; }
        }

        private readonly struct Region
        {
            public Region(MetricValues metrics, double weight, double peak, double[] errors)
            {
                Metrics = metrics;
                Weight = weight;
                Peak = peak;
                Errors = errors;
            }
            public MetricValues Metrics { get; }
            public double Weight { get; }
            public double Peak { get; }
            public double[] Errors { get; }
            public bool ModelEligible => !(Weight <= ModelL2OmissionThreshold && Peak <= PeakZeroGuard);
        }

        private readonly struct MetricValues
        {
            public MetricValues(double curvature, double boundaryContrast)
            {
                Curvature = curvature;
                BoundaryContrast = boundaryContrast;
            }
            public double Curvature { get; }
            public double BoundaryContrast { get; }
        }

        private readonly struct NormPeak
        {
            public NormPeak(
                double weightA,
                double weightB,
                double weightC,
                double peakA,
                double peakB,
                double peakC)
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

        private readonly struct Allocation
        {
            public static readonly Allocation Infeasible = new(0, 0, 0, double.NaN, false);
            public Allocation(int a, int b, int c, double predictedGlobalError, bool feasible)
            {
                A = a;
                B = b;
                C = c;
                PredictedGlobalError = predictedGlobalError;
                Feasible = feasible;
            }
            public int A { get; }
            public int B { get; }
            public int C { get; }
            public int Total => A + B + C;
            public double PredictedGlobalError { get; }
            public bool Feasible { get; }
        }
    }
}
