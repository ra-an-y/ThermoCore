using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct HybridFormulaVerifyPoint1D
    {
        public HybridFormulaVerifyPoint1D(
            double time,
            int proposalA,
            int proposalB,
            int proposalC,
            int hybridA,
            int hybridB,
            int hybridC,
            double hybridPredictedGlobal,
            double hybridDirectGlobal,
            int oracleA,
            int oracleB,
            int oracleC,
            int evaluatedFitLevels,
            int exhaustiveFitLevels)
        {
            Time = time;
            ProposalA = proposalA;
            ProposalB = proposalB;
            ProposalC = proposalC;
            HybridA = hybridA;
            HybridB = hybridB;
            HybridC = hybridC;
            HybridPredictedGlobal = hybridPredictedGlobal;
            HybridDirectGlobal = hybridDirectGlobal;
            OracleA = oracleA;
            OracleB = oracleB;
            OracleC = oracleC;
            EvaluatedFitLevels = evaluatedFitLevels;
            ExhaustiveFitLevels = exhaustiveFitLevels;
        }

        public double Time { get; }
        public int ProposalA { get; }
        public int ProposalB { get; }
        public int ProposalC { get; }
        public int ProposalTotal => ProposalA + ProposalB + ProposalC;
        public int HybridA { get; }
        public int HybridB { get; }
        public int HybridC { get; }
        public int HybridTotal => HybridA + HybridB + HybridC;
        public double HybridPredictedGlobal { get; }
        public double HybridDirectGlobal { get; }
        public int OracleA { get; }
        public int OracleB { get; }
        public int OracleC { get; }
        public int OracleTotal => OracleA + OracleB + OracleC;
        public int EvaluatedFitLevels { get; }
        public int ExhaustiveFitLevels { get; }
        public int SavedFitLevels => ExhaustiveFitLevels - EvaluatedFitLevels;
        public double FitLevelReductionFraction
            => ExhaustiveFitLevels <= 0
                ? 0.0
                : 1.0 - (double)EvaluatedFitLevels / ExhaustiveFitLevels;
        public bool MatchesOracleTotal => HybridTotal == OracleTotal;
    }

    public readonly struct HybridFormulaVerifyStudyResult1D
    {
        private readonly HybridFormulaVerifyPoint1D[] _points;

        public HybridFormulaVerifyStudyResult1D(
            HybridFormulaVerifyPoint1D[] points,
            int safeCount,
            int oracleTotalMatchCount,
            int minimumSavedFitLevels,
            int maximumSavedFitLevels,
            double meanReductionFraction,
            double maximumIdentityError)
        {
            _points = points;
            SafeCount = safeCount;
            OracleTotalMatchCount = oracleTotalMatchCount;
            MinimumSavedFitLevels = minimumSavedFitLevels;
            MaximumSavedFitLevels = maximumSavedFitLevels;
            MeanReductionFraction = meanReductionFraction;
            MaximumIdentityError = maximumIdentityError;
        }

        public int Count => _points?.Length ?? 0;
        public int SafeCount { get; }
        public int OracleTotalMatchCount { get; }
        public int MinimumSavedFitLevels { get; }
        public int MaximumSavedFitLevels { get; }
        public double MeanReductionFraction { get; }
        public double MaximumIdentityError { get; }

        public HybridFormulaVerifyPoint1D GetPoint(int index)
        {
            if (_points is null || index < 0 || index >= _points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _points[index];
        }

        public bool SatisfiesDataIntegrity(double threshold, double identityTolerance)
        {
            if (Count == 0 || SafeCount < 0 || OracleTotalMatchCount < 0)
            {
                return false;
            }

            if (double.IsNaN(MeanReductionFraction)
                || double.IsInfinity(MeanReductionFraction)
                || MeanReductionFraction < 0.0
                || MaximumIdentityError > identityTolerance)
            {
                return false;
            }

            for (var i = 0; i < Count; i++)
            {
                var point = _points[i];
                if (point.ProposalTotal <= 0
                    || point.HybridTotal <= 0
                    || point.OracleTotal <= 0
                    || point.EvaluatedFitLevels <= 0
                    || point.EvaluatedFitLevels > point.ExhaustiveFitLevels
                    || double.IsNaN(point.HybridPredictedGlobal)
                    || double.IsInfinity(point.HybridPredictedGlobal)
                    || double.IsNaN(point.HybridDirectGlobal)
                    || double.IsInfinity(point.HybridDirectGlobal)
                    || point.HybridPredictedGlobal > threshold
                    || point.HybridDirectGlobal > threshold)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Hybrid allocation study.
    ///
    /// The count-dependent formula proposal from Checkpoint 16 is treated as an
    /// online upper budget for each region. The current nested greedy fitter is
    /// then evaluated only up to those proposed counts, rather than always to
    /// N=8 in all three regions. Direct fitted error curves inside that truncated
    /// box are combined with the exact disjoint-region global L2 identity to find
    /// the smallest verified allocation available inside the proposal box.
    ///
    /// Proposal generation and the full oracle are evaluation instrumentation;
    /// EvaluatedFitLevels counts only the online verification sequence levels.
    /// </summary>
    public static class HybridFormulaLocalVerifyStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumGaussianCount = 8;
        private const int SampleCount = 401;
        private const double GlobalErrorThreshold = 5e-3;
        private const double PeakZeroGuard = 5e-3;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

        public static HybridFormulaVerifyStudyResult1D Evaluate()
        {
            var proposalStudy = PredictionResidualUncertaintyStudy1D.Evaluate();
            var oracleStudy = GlobalErrorBudgetFormulaStudy1D.Evaluate();
            var states = BuildSnapshotStates();
            var points = new HybridFormulaVerifyPoint1D[SnapshotTimes.Length];

            var safeCount = 0;
            var oracleTotalMatchCount = 0;
            var minimumSaved = int.MaxValue;
            var maximumSaved = 0;
            var reductionSum = 0.0;
            var maximumIdentityError = 0.0;

            for (var index = 0; index < SnapshotTimes.Length; index++)
            {
                var proposal = proposalStudy.GetFold(index);
                var oracle = oracleStudy.GetPoint(index);
                var state = states[index];

                if (!proposal.CountFeasible)
                {
                    throw new InvalidOperationException(
                        $"Checkpoint 16 proposal is infeasible at t={SnapshotTimes[index]:F2}.");
                }

                var fitsA = proposal.CountA > 0
                    ? ConstrainedGaussianSparseFitter1D.FitSequence(
                        state.StateA, 0.60, proposal.CountA)
                    : Array.Empty<ConstrainedGaussianSparseFitResult1D>();
                var fitsB = proposal.CountB > 0
                    ? ConstrainedGaussianSparseFitter1D.FitSequence(
                        state.StateB, 0.35, proposal.CountB)
                    : Array.Empty<ConstrainedGaussianSparseFitResult1D>();
                var fitsC = proposal.CountC > 0
                    ? ConstrainedGaussianSparseFitter1D.FitSequence(
                        state.StateC, 0.60, proposal.CountC)
                    : Array.Empty<ConstrainedGaussianSparseFitResult1D>();

                var best = FindVerifiedMinimum(
                    state,
                    oracle,
                    proposal.CountA,
                    proposal.CountB,
                    proposal.CountC,
                    fitsA,
                    fitsB,
                    fitsC);

                if (!best.Feasible)
                {
                    throw new InvalidOperationException(
                        $"No verified allocation exists inside proposal box at t={SnapshotTimes[index]:F2}.");
                }

                var directGlobal = DirectGlobalError(
                    state,
                    best.A == 0 ? null : fitsA[best.A - 1].Mixture,
                    best.B == 0 ? null : fitsB[best.B - 1].Mixture,
                    best.C == 0 ? null : fitsC[best.C - 1].Mixture,
                    0.60,
                    0.35,
                    0.60);

                var identityError = Math.Abs(best.GlobalError - directGlobal);
                maximumIdentityError = Math.Max(maximumIdentityError, identityError);

                var evaluatedFitLevels = proposal.CountA + proposal.CountB + proposal.CountC;
                const int exhaustiveFitLevels = 3 * MaximumGaussianCount;

                points[index] = new HybridFormulaVerifyPoint1D(
                    SnapshotTimes[index],
                    proposal.CountA,
                    proposal.CountB,
                    proposal.CountC,
                    best.A,
                    best.B,
                    best.C,
                    best.GlobalError,
                    directGlobal,
                    oracle.CountA,
                    oracle.CountB,
                    oracle.CountC,
                    evaluatedFitLevels,
                    exhaustiveFitLevels);

                if (directGlobal <= GlobalErrorThreshold)
                {
                    safeCount++;
                }
                if (best.A + best.B + best.C == oracle.TotalCount)
                {
                    oracleTotalMatchCount++;
                }

                var saved = exhaustiveFitLevels - evaluatedFitLevels;
                minimumSaved = Math.Min(minimumSaved, saved);
                maximumSaved = Math.Max(maximumSaved, saved);
                reductionSum += 1.0 - (double)evaluatedFitLevels / exhaustiveFitLevels;
            }

            return new HybridFormulaVerifyStudyResult1D(
                points,
                safeCount,
                oracleTotalMatchCount,
                minimumSaved == int.MaxValue ? 0 : minimumSaved,
                maximumSaved,
                reductionSum / SnapshotTimes.Length,
                maximumIdentityError);
        }

        private static VerifiedAllocation FindVerifiedMinimum(
            in ThreeLayerCoupledState1D state,
            in GlobalErrorBudgetFormulaPoint1D oracle,
            int maximumA,
            int maximumB,
            int maximumC,
            ConstrainedGaussianSparseFitResult1D[] fitsA,
            ConstrainedGaussianSparseFitResult1D[] fitsB,
            ConstrainedGaussianSparseFitResult1D[] fitsC)
        {
            var best = VerifiedAllocation.Infeasible;

            for (var countA = 0; countA <= maximumA; countA++)
            {
                if (countA == 0 && oracle.PeakContributionA > PeakZeroGuard)
                {
                    continue;
                }

                for (var countB = 0; countB <= maximumB; countB++)
                {
                    if (countB == 0 && oracle.PeakContributionB > PeakZeroGuard)
                    {
                        continue;
                    }

                    for (var countC = 0; countC <= maximumC; countC++)
                    {
                        if (countC == 0 && oracle.PeakContributionC > PeakZeroGuard)
                        {
                            continue;
                        }

                        var total = countA + countB + countC;
                        if (total == 0 || (best.Feasible && total > best.Total))
                        {
                            continue;
                        }

                        var errorA = countA == 0 ? 1.0 : fitsA[countA - 1].RelativeError;
                        var errorB = countB == 0 ? 1.0 : fitsB[countB - 1].RelativeError;
                        var errorC = countC == 0 ? 1.0 : fitsC[countC - 1].RelativeError;

                        var global = Math.Sqrt(
                            Square(oracle.WeightA * errorA)
                            + Square(oracle.WeightB * errorB)
                            + Square(oracle.WeightC * errorC));

                        if (global > GlobalErrorThreshold)
                        {
                            continue;
                        }

                        if (!best.Feasible
                            || total < best.Total
                            || (total == best.Total && global < best.GlobalError))
                        {
                            best = new VerifiedAllocation(
                                countA, countB, countC, global, true);
                        }
                    }
                }
            }

            return best;
        }

        private static ThreeLayerCoupledState1D[] BuildSnapshotStates()
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

            var states = new ThreeLayerCoupledState1D[SnapshotTimes.Length];
            var currentTime = 0.0;

            for (var index = 0; index < SnapshotTimes.Length; index++)
            {
                var target = SnapshotTimes[index];
                var steps = (int)Math.Round((target - currentTime) / deltaTime);
                for (var step = 0; step < steps; step++)
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
                currentTime = target;
                states[index] = state;
            }

            return states;
        }

        private static double DirectGlobalError(
            in ThreeLayerCoupledState1D state,
            GaussianMixture1D? mixtureA,
            GaussianMixture1D? mixtureB,
            GaussianMixture1D? mixtureC,
            double lengthA,
            double lengthB,
            double lengthC)
        {
            var errorSquared = 0.0;
            var referenceSquared = 0.0;

            AccumulateDirectError(
                state.StateA, mixtureA, lengthA,
                ref errorSquared, ref referenceSquared);
            AccumulateDirectError(
                state.StateB, mixtureB, lengthB,
                ref errorSquared, ref referenceSquared);
            AccumulateDirectError(
                state.StateC, mixtureC, lengthC,
                ref errorSquared, ref referenceSquared);

            return Math.Sqrt(errorSquared / referenceSquared);
        }

        private static void AccumulateDirectError(
            in FiniteLayerReducedState1D state,
            GaussianMixture1D? mixture,
            double length,
            ref double errorSquared,
            ref double referenceSquared)
        {
            var dx = length / SampleCount;
            for (var sample = 0; sample < SampleCount; sample++)
            {
                var x = (sample + 0.5) * dx;
                var reference = FiniteLayerFieldRepresentation1D.Evaluate(state, x, length);
                var candidate = mixture.HasValue ? mixture.Value.Evaluate(x) : 0.0;
                var difference = candidate - reference;
                errorSquared += dx * difference * difference;
                referenceSquared += dx * reference * reference;
            }
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

        private static double Square(double value) => value * value;

        private readonly struct VerifiedAllocation
        {
            public VerifiedAllocation(
                int a,
                int b,
                int c,
                double globalError,
                bool feasible)
            {
                A = a;
                B = b;
                C = c;
                GlobalError = globalError;
                Feasible = feasible;
            }

            public int A { get; }
            public int B { get; }
            public int C { get; }
            public int Total => A + B + C;
            public double GlobalError { get; }
            public bool Feasible { get; }

            public static VerifiedAllocation Infeasible
                => new VerifiedAllocation(0, 0, 0, double.PositiveInfinity, false);
        }
    }
}
