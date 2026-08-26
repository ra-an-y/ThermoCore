using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct HeldOutFormulaAllocationPoint1D
    {
        public HeldOutFormulaAllocationPoint1D(
            double time,
            double safetyFactor,
            bool formulaFeasible,
            int countA,
            int countB,
            int countC,
            double predictedGlobalError,
            double actualGlobalError,
            int oracleA,
            int oracleB,
            int oracleC,
            double oracleGlobalError)
        {
            Time = time;
            SafetyFactor = safetyFactor;
            FormulaFeasible = formulaFeasible;
            CountA = countA;
            CountB = countB;
            CountC = countC;
            PredictedGlobalError = predictedGlobalError;
            ActualGlobalError = actualGlobalError;
            OracleA = oracleA;
            OracleB = oracleB;
            OracleC = oracleC;
            OracleGlobalError = oracleGlobalError;
        }

        public double Time { get; }
        public double SafetyFactor { get; }
        public bool FormulaFeasible { get; }
        public int CountA { get; }
        public int CountB { get; }
        public int CountC { get; }
        public int FormulaTotal => FormulaFeasible ? CountA + CountB + CountC : -1;
        public double PredictedGlobalError { get; }
        public double ActualGlobalError { get; }
        public int OracleA { get; }
        public int OracleB { get; }
        public int OracleC { get; }
        public int OracleTotal => OracleA + OracleB + OracleC;
        public double OracleGlobalError { get; }
        public int BudgetOverhead => FormulaFeasible ? FormulaTotal - OracleTotal : -1;
        public bool IsSafe(double threshold)
            => FormulaFeasible && ActualGlobalError <= threshold;
    }

    public readonly struct HeldOutFormulaAllocationStudyResult1D
    {
        private readonly HeldOutFormulaAllocationPoint1D[] _points;

        public HeldOutFormulaAllocationStudyResult1D(
            HeldOutFormulaAllocationPoint1D[] points,
            int feasibleCount,
            int safeCount,
            int exactOracleMatchCount,
            double maximumSafetyFactor,
            int maximumBudgetOverhead)
        {
            _points = points;
            FeasibleCount = feasibleCount;
            SafeCount = safeCount;
            ExactOracleMatchCount = exactOracleMatchCount;
            MaximumSafetyFactor = maximumSafetyFactor;
            MaximumBudgetOverhead = maximumBudgetOverhead;
        }

        public int Count => _points?.Length ?? 0;
        public int FeasibleCount { get; }
        public int SafeCount { get; }
        public int ExactOracleMatchCount { get; }
        public double MaximumSafetyFactor { get; }
        public int MaximumBudgetOverhead { get; }

        public HeldOutFormulaAllocationPoint1D GetPoint(int index)
        {
            if (_points is null || index < 0 || index >= _points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _points[index];
        }

        public bool SatisfiesDataIntegrity()
        {
            if (Count < 4 || FeasibleCount < 0 || SafeCount < 0)
            {
                return false;
            }

            if (double.IsNaN(MaximumSafetyFactor)
                || double.IsInfinity(MaximumSafetyFactor)
                || MaximumSafetyFactor < 1.0)
            {
                return false;
            }

            for (var i = 0; i < Count; i++)
            {
                var point = _points[i];
                if (double.IsNaN(point.SafetyFactor)
                    || double.IsInfinity(point.SafetyFactor)
                    || point.SafetyFactor < 1.0
                    || point.OracleTotal <= 0
                    || double.IsNaN(point.OracleGlobalError)
                    || double.IsInfinity(point.OracleGlobalError))
                {
                    return false;
                }

                if (point.FormulaFeasible
                    && (point.FormulaTotal <= 0
                        || double.IsNaN(point.PredictedGlobalError)
                        || double.IsInfinity(point.PredictedGlobalError)
                        || double.IsNaN(point.ActualGlobalError)
                        || double.IsInfinity(point.ActualGlobalError)))
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Held-out end-to-end test of formula-driven Gaussian allocation.
    ///
    /// Each fold removes one complete time snapshot from model fitting.
    /// Training-only residuals calibrate a multiplicative safety factor using
    /// the maximum observed under-prediction ratio. The held-out state then
    /// supplies only current-state shape metrics, global L2 weights, and peak
    /// guards to the allocator. Direct sparse-fit errors are consulted only
    /// after allocation, for validation and oracle comparison.
    /// </summary>
    public static class HeldOutFormulaDrivenAllocationStudy1D
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

        public static HeldOutFormulaAllocationStudyResult1D Evaluate()
        {
            var snapshots = BuildSnapshots();
            var points = new HeldOutFormulaAllocationPoint1D[snapshots.Length];
            var feasibleCount = 0;
            var safeCount = 0;
            var exactOracleMatchCount = 0;
            var maximumSafetyFactor = 1.0;
            var maximumBudgetOverhead = 0;

            for (var heldOut = 0; heldOut < snapshots.Length; heldOut++)
            {
                var beta = FitModel(snapshots, heldOut);
                var safetyFactor = CalibrateSafetyFactor(snapshots, heldOut, beta);
                maximumSafetyFactor = Math.Max(maximumSafetyFactor, safetyFactor);

                var heldOutSnapshot = snapshots[heldOut];
                var formula = SelectFormulaAllocation(heldOutSnapshot, beta, safetyFactor);
                var oracle = SelectOracleAllocation(heldOutSnapshot);

                var actualGlobal = formula.Feasible
                    ? ActualGlobalError(heldOutSnapshot, formula.A, formula.B, formula.C)
                    : double.NaN;

                points[heldOut] = new HeldOutFormulaAllocationPoint1D(
                    heldOutSnapshot.Time,
                    safetyFactor,
                    formula.Feasible,
                    formula.A,
                    formula.B,
                    formula.C,
                    formula.PredictedGlobalError,
                    actualGlobal,
                    oracle.A,
                    oracle.B,
                    oracle.C,
                    oracle.PredictedGlobalError);

                if (formula.Feasible)
                {
                    feasibleCount++;
                    if (actualGlobal <= GlobalErrorThreshold)
                    {
                        safeCount++;
                    }

                    var overhead = formula.A + formula.B + formula.C
                        - (oracle.A + oracle.B + oracle.C);
                    maximumBudgetOverhead = Math.Max(maximumBudgetOverhead, overhead);

                    if (formula.A + formula.B + formula.C
                        == oracle.A + oracle.B + oracle.C)
                    {
                        exactOracleMatchCount++;
                    }
                }
            }

            return new HeldOutFormulaAllocationStudyResult1D(
                points,
                feasibleCount,
                safeCount,
                exactOracleMatchCount,
                maximumSafetyFactor,
                maximumBudgetOverhead);
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

                var norm = MeasureNormsAndPeaks(state, lengthA, lengthB, lengthC);
                result[snapshotIndex] = new Snapshot(
                    targetTime,
                    BuildRegion(state.StateA, lengthA, norm.WeightA, norm.PeakA),
                    BuildRegion(state.StateB, lengthB, norm.WeightB, norm.PeakB),
                    BuildRegion(state.StateC, lengthC, norm.WeightC, norm.PeakC));
            }

            return result;
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

        private static double[] FitModel(Snapshot[] snapshots, int heldOutSnapshot)
        {
            const int dimension = 6;
            var normal = new double[dimension, dimension];
            var rhs = new double[dimension];

            for (var snapshotIndex = 0; snapshotIndex < snapshots.Length; snapshotIndex++)
            {
                if (snapshotIndex == heldOutSnapshot)
                {
                    continue;
                }

                AddRegionToNormal(snapshots[snapshotIndex].A, normal, rhs);
                AddRegionToNormal(snapshots[snapshotIndex].B, normal, rhs);
                AddRegionToNormal(snapshots[snapshotIndex].C, normal, rhs);
            }

            return SolveLinearSystem(normal, rhs);
        }

        private static void AddRegionToNormal(
            Region region,
            double[,] normal,
            double[] rhs)
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

        private static double CalibrateSafetyFactor(
            Snapshot[] snapshots,
            int heldOutSnapshot,
            double[] beta)
        {
            var safetyFactor = 1.0;
            for (var snapshotIndex = 0; snapshotIndex < snapshots.Length; snapshotIndex++)
            {
                if (snapshotIndex == heldOutSnapshot)
                {
                    continue;
                }

                safetyFactor = Math.Max(
                    safetyFactor,
                    RegionSafetyFactor(snapshots[snapshotIndex].A, beta));
                safetyFactor = Math.Max(
                    safetyFactor,
                    RegionSafetyFactor(snapshots[snapshotIndex].B, beta));
                safetyFactor = Math.Max(
                    safetyFactor,
                    RegionSafetyFactor(snapshots[snapshotIndex].C, beta));
            }
            return safetyFactor;
        }

        private static double RegionSafetyFactor(Region region, double[] beta)
        {
            if (!region.ModelEligible)
            {
                return 1.0;
            }

            var factor = 1.0;
            for (var i = 0; i < region.Errors.Length; i++)
            {
                var predicted = Math.Exp(PredictLogError(beta, region.Metrics, i + 1));
                if (predicted > 0.0)
                {
                    factor = Math.Max(factor, region.Errors[i] / predicted);
                }
            }
            return factor;
        }

        private static Allocation SelectFormulaAllocation(
            Snapshot snapshot,
            double[] beta,
            double safetyFactor)
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

                        var errorA = FormulaRegionalError(snapshot.A, countA, beta, safetyFactor);
                        var errorB = FormulaRegionalError(snapshot.B, countB, beta, safetyFactor);
                        var errorC = FormulaRegionalError(snapshot.C, countC, beta, safetyFactor);
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

            if (!best.Feasible)
            {
                throw new InvalidOperationException("No oracle allocation was found.");
            }
            return best;
        }

        private static double FormulaRegionalError(
            Region region,
            int count,
            double[] beta,
            double safetyFactor)
        {
            if (count == 0)
            {
                return 1.0;
            }
            return safetyFactor * Math.Exp(PredictLogError(beta, region.Metrics, count));
        }

        private static double ActualGlobalError(
            Snapshot snapshot,
            int countA,
            int countB,
            int countC)
        {
            var errorA = countA == 0 ? 1.0 : snapshot.A.Errors[countA - 1];
            var errorB = countB == 0 ? 1.0 : snapshot.B.Errors[countB - 1];
            var errorC = countC == 0 ? 1.0 : snapshot.C.Errors[countC - 1];
            return Math.Sqrt(
                Square(snapshot.A.Weight * errorA)
                + Square(snapshot.B.Weight * errorB)
                + Square(snapshot.C.Weight * errorC));
        }

        private static double PredictLogError(
            double[] beta,
            MetricValues metrics,
            int count)
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

        private static MetricValues ComputeMetrics(
            in FiniteLayerReducedState1D state,
            double length)
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
            var fieldEnergy = mean * mean + 0.5 * modalEnergy;
            var safeEnergy = Math.Max(fieldEnergy, 1e-30);
            var curvature = Math.Sqrt(0.5 * n4Energy / safeEnergy);
            var left = FiniteLayerFieldRepresentation1D.Evaluate(state, 0.0, length);
            var right = FiniteLayerFieldRepresentation1D.Evaluate(state, length, length);
            var boundaryContrast = Math.Abs(left - right) / Math.Sqrt(safeEnergy);
            return new MetricValues(curvature, boundaryContrast);
        }

        private static NormAndPeak MeasureNormsAndPeaks(
            in ThreeLayerCoupledState1D state,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var squaredA = PhysicalSquaredNorm(state.StateA, lengthA, out var peakA);
            var squaredB = PhysicalSquaredNorm(state.StateB, lengthB, out var peakB);
            var squaredC = PhysicalSquaredNorm(state.StateC, lengthC, out var peakC);
            var globalSquared = squaredA + squaredB + squaredC;
            var globalNorm = Math.Sqrt(Math.Max(globalSquared, 1e-30));
            var globalPeak = Math.Max(peakA, Math.Max(peakB, peakC));
            var safePeak = Math.Max(globalPeak, 1e-30);
            return new NormAndPeak(
                Math.Sqrt(squaredA) / globalNorm,
                Math.Sqrt(squaredB) / globalNorm,
                Math.Sqrt(squaredC) / globalNorm,
                peakA / safePeak,
                peakB / safePeak,
                peakC / safePeak);
        }

        private static double PhysicalSquaredNorm(
            in FiniteLayerReducedState1D state,
            double length,
            out double peak)
        {
            var dx = length / FieldSampleCount;
            var sum = 0.0;
            peak = 0.0;
            for (var sample = 0; sample < FieldSampleCount; sample++)
            {
                var x = (sample + 0.5) * dx;
                var value = FiniteLayerFieldRepresentation1D.Evaluate(state, x, length);
                sum += value * value;
                peak = Math.Max(peak, Math.Abs(value));
            }
            return dx * sum;
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
                    throw new InvalidOperationException("Held-out formula fit is numerically singular.");
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

        private static double Square(double value) => value * value;

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

        private readonly struct Region
        {
            public Region(
                MetricValues metrics,
                double weight,
                double peak,
                double[] errors)
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
            public bool ModelEligible
                => !(Weight <= ModelL2OmissionThreshold && Peak <= PeakZeroGuard);
        }

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

        private readonly struct NormAndPeak
        {
            public NormAndPeak(
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
            public Allocation(
                int a,
                int b,
                int c,
                double predictedGlobalError,
                bool feasible)
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

            public static Allocation Infeasible
                => new Allocation(0, 0, 0, double.PositiveInfinity, false);
        }
    }
}
