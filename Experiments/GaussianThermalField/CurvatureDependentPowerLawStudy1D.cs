using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct CurvatureDependentPowerLawResult1D
    {
        public CurvatureDependentPowerLawResult1D(
            int stateSampleCount,
            double beta0,
            double betaCurvature,
            double betaLogCount,
            double betaInteraction,
            double logR2,
            double logRmse,
            double leaveOneStateOutLogRmse,
            double baselineLeaveOneStateOutLogRmse,
            double minimumPredictedExponent,
            double maximumPredictedExponent)
        {
            StateSampleCount = stateSampleCount;
            Beta0 = beta0;
            BetaCurvature = betaCurvature;
            BetaLogCount = betaLogCount;
            BetaInteraction = betaInteraction;
            LogR2 = logR2;
            LogRmse = logRmse;
            LeaveOneStateOutLogRmse = leaveOneStateOutLogRmse;
            BaselineLeaveOneStateOutLogRmse = baselineLeaveOneStateOutLogRmse;
            MinimumPredictedExponent = minimumPredictedExponent;
            MaximumPredictedExponent = maximumPredictedExponent;
        }

        public int StateSampleCount { get; }
        public double Beta0 { get; }
        public double BetaCurvature { get; }
        public double BetaLogCount { get; }
        public double BetaInteraction { get; }
        public double LogR2 { get; }
        public double LogRmse { get; }
        public double LeaveOneStateOutLogRmse { get; }
        public double BaselineLeaveOneStateOutLogRmse { get; }
        public double MinimumPredictedExponent { get; }
        public double MaximumPredictedExponent { get; }

        public bool SatisfiesDataIntegrity()
        {
            return StateSampleCount >= 8
                && IsFinite(Beta0)
                && IsFinite(BetaCurvature)
                && IsFinite(BetaLogCount)
                && IsFinite(BetaInteraction)
                && IsFinite(LogR2)
                && IsFinite(LogRmse)
                && IsFinite(LeaveOneStateOutLogRmse)
                && IsFinite(BaselineLeaveOneStateOutLogRmse)
                && IsFinite(MinimumPredictedExponent)
                && IsFinite(MaximumPredictedExponent);
        }

        private static bool IsFinite(double value)
            => !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// Tests a curvature-dependent power-law error model:
    ///
    /// log e = beta0 + betaC log(1+C)
    ///         + betaN log N
    ///         + betaCN log(1+C) log N
    ///
    /// Equivalently:
    ///
    /// e(N,C) = exp(beta0) (1+C)^betaC
    ///          N^[betaN + betaCN log(1+C)].
    ///
    /// The interaction permits the power exponent itself to vary with current
    /// state curvature instead of forcing one universal decay exponent.
    /// </summary>
    public static class CurvatureDependentPowerLawStudy1D
    {
        private const int ModeCount = 32;
        private const int MaximumGaussianCount = 8;
        private const double L2OmissionThreshold = 1e-3;
        private const double PeakOmissionThreshold = 5e-3;
        private const int FieldSampleCount = 401;

        private static readonly double[] SnapshotTimes =
        {
            0.10, 0.20, 0.40, 0.60, 1.00, 1.50
        };

        public static CurvatureDependentPowerLawResult1D Evaluate()
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

            var records = new List<ErrorRecord>();
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

                var contribution = MeasureGlobalContributions(
                    state,
                    lengthA,
                    lengthB,
                    lengthC);

                AddRegion(records, state.StateA, lengthA, contribution.L2A, contribution.PeakA);
                AddRegion(records, state.StateB, lengthB, contribution.L2B, contribution.PeakB);
                AddRegion(records, state.StateC, lengthC, contribution.L2C, contribution.PeakC);
            }

            var interaction = FitModel(records, includeInteraction: true, excludedRecord: -1);
            var interactionQuality = EvaluateModel(
                records,
                interaction,
                includeInteraction: true,
                excludedRecord: -1,
                onlyExcluded: false);

            var interactionCv = CrossValidatedRmse(records, includeInteraction: true);
            var baselineCv = CrossValidatedRmse(records, includeInteraction: false);

            var minimumExponent = double.PositiveInfinity;
            var maximumExponent = double.NegativeInfinity;
            for (var i = 0; i < records.Count; i++)
            {
                var u = Math.Log(1.0 + records[i].Curvature);
                var exponent = -(interaction[2] + interaction[3] * u);
                minimumExponent = Math.Min(minimumExponent, exponent);
                maximumExponent = Math.Max(maximumExponent, exponent);
            }

            return new CurvatureDependentPowerLawResult1D(
                records.Count,
                interaction[0],
                interaction[1],
                interaction[2],
                interaction[3],
                interactionQuality.R2,
                interactionQuality.Rmse,
                interactionCv,
                baselineCv,
                minimumExponent,
                maximumExponent);
        }

        private static void AddRegion(
            List<ErrorRecord> records,
            in FiniteLayerReducedState1D state,
            double length,
            double l2Contribution,
            double peakContribution)
        {
            if (l2Contribution <= L2OmissionThreshold
                && peakContribution <= PeakOmissionThreshold)
            {
                return;
            }

            var fits = ConstrainedGaussianSparseFitter1D.FitSequence(
                state,
                length,
                MaximumGaussianCount);
            var errors = new double[MaximumGaussianCount];
            for (var i = 0; i < errors.Length; i++)
            {
                errors[i] = Math.Max(fits[i].RelativeError, 1e-15);
            }

            records.Add(new ErrorRecord(ComputeNormalizedCurvature(state), errors));
        }

        private static double[] FitModel(
            List<ErrorRecord> records,
            bool includeInteraction,
            int excludedRecord)
        {
            var dimension = includeInteraction ? 4 : 3;
            var normal = new double[dimension, dimension];
            var rhs = new double[dimension];

            for (var recordIndex = 0; recordIndex < records.Count; recordIndex++)
            {
                if (recordIndex == excludedRecord)
                {
                    continue;
                }

                var record = records[recordIndex];
                var u = Math.Log(1.0 + record.Curvature);
                for (var i = 0; i < record.Errors.Length; i++)
                {
                    var v = Math.Log(i + 1.0);
                    var row = includeInteraction
                        ? new[] { 1.0, u, v, u * v }
                        : new[] { 1.0, u, v };
                    var target = Math.Log(record.Errors[i]);

                    for (var r = 0; r < dimension; r++)
                    {
                        rhs[r] += row[r] * target;
                        for (var c = 0; c < dimension; c++)
                        {
                            normal[r, c] += row[r] * row[c];
                        }
                    }
                }
            }

            return SolveLinearSystem(normal, rhs);
        }

        private static ModelQuality EvaluateModel(
            List<ErrorRecord> records,
            double[] beta,
            bool includeInteraction,
            int excludedRecord,
            bool onlyExcluded)
        {
            var targets = new List<double>();
            var predictions = new List<double>();

            for (var recordIndex = 0; recordIndex < records.Count; recordIndex++)
            {
                if (onlyExcluded)
                {
                    if (recordIndex != excludedRecord)
                    {
                        continue;
                    }
                }
                else if (recordIndex == excludedRecord)
                {
                    continue;
                }

                var record = records[recordIndex];
                var u = Math.Log(1.0 + record.Curvature);
                for (var i = 0; i < record.Errors.Length; i++)
                {
                    var v = Math.Log(i + 1.0);
                    var prediction = beta[0] + beta[1] * u + beta[2] * v;
                    if (includeInteraction)
                    {
                        prediction += beta[3] * u * v;
                    }
                    targets.Add(Math.Log(record.Errors[i]));
                    predictions.Add(prediction);
                }
            }

            var mean = 0.0;
            for (var i = 0; i < targets.Count; i++)
            {
                mean += targets[i];
            }
            mean /= Math.Max(targets.Count, 1);

            var squaredError = 0.0;
            var totalSquared = 0.0;
            for (var i = 0; i < targets.Count; i++)
            {
                var residual = targets[i] - predictions[i];
                squaredError += residual * residual;
                var centered = targets[i] - mean;
                totalSquared += centered * centered;
            }

            var r2 = totalSquared > 0.0
                ? 1.0 - squaredError / totalSquared
                : 1.0;
            return new ModelQuality(
                r2,
                Math.Sqrt(squaredError / Math.Max(targets.Count, 1)),
                squaredError,
                targets.Count);
        }

        private static double CrossValidatedRmse(
            List<ErrorRecord> records,
            bool includeInteraction)
        {
            var squaredError = 0.0;
            var count = 0;
            for (var excluded = 0; excluded < records.Count; excluded++)
            {
                var beta = FitModel(records, includeInteraction, excluded);
                var fold = EvaluateModel(
                    records,
                    beta,
                    includeInteraction,
                    excluded,
                    onlyExcluded: true);
                squaredError += fold.SumSquaredError;
                count += fold.Count;
            }
            return Math.Sqrt(squaredError / Math.Max(count, 1));
        }

        private static double ComputeNormalizedCurvature(
            in FiniteLayerReducedState1D state)
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
            return Math.Sqrt(0.5 * n4Energy / Math.Max(fieldEnergy, 1e-30));
        }

        private static GlobalContribution MeasureGlobalContributions(
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

            return new GlobalContribution(
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
            var sum = 0.0;
            peak = 0.0;
            var dx = length / FieldSampleCount;
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
            for (var r = 0; r < size; r++)
            {
                for (var c = 0; c < size; c++)
                {
                    augmented[r, c] = matrix[r, c];
                }
                augmented[r, size] = rhs[r];
            }

            for (var pivot = 0; pivot < size; pivot++)
            {
                var best = pivot;
                for (var r = pivot + 1; r < size; r++)
                {
                    if (Math.Abs(augmented[r, pivot]) > Math.Abs(augmented[best, pivot]))
                    {
                        best = r;
                    }
                }

                if (Math.Abs(augmented[best, pivot]) <= 1e-18)
                {
                    throw new InvalidOperationException("Curvature power-law regression is singular.");
                }

                if (best != pivot)
                {
                    for (var c = pivot; c <= size; c++)
                    {
                        var temp = augmented[pivot, c];
                        augmented[pivot, c] = augmented[best, c];
                        augmented[best, c] = temp;
                    }
                }

                var pivotValue = augmented[pivot, pivot];
                for (var c = pivot; c <= size; c++)
                {
                    augmented[pivot, c] /= pivotValue;
                }

                for (var r = 0; r < size; r++)
                {
                    if (r == pivot)
                    {
                        continue;
                    }
                    var factor = augmented[r, pivot];
                    for (var c = pivot; c <= size; c++)
                    {
                        augmented[r, c] -= factor * augmented[pivot, c];
                    }
                }
            }

            var result = new double[size];
            for (var r = 0; r < size; r++)
            {
                result[r] = augmented[r, size];
            }
            return result;
        }

        private readonly struct ErrorRecord
        {
            public ErrorRecord(double curvature, double[] errors)
            {
                Curvature = curvature;
                Errors = errors;
            }

            public double Curvature { get; }
            public double[] Errors { get; }
        }

        private readonly struct ModelQuality
        {
            public ModelQuality(double r2, double rmse, double sumSquaredError, int count)
            {
                R2 = r2;
                Rmse = rmse;
                SumSquaredError = sumSquaredError;
                Count = count;
            }

            public double R2 { get; }
            public double Rmse { get; }
            public double SumSquaredError { get; }
            public int Count { get; }
        }

        private readonly struct GlobalContribution
        {
            public GlobalContribution(
                double l2A,
                double l2B,
                double l2C,
                double peakA,
                double peakB,
                double peakC)
            {
                L2A = l2A;
                L2B = l2B;
                L2C = l2C;
                PeakA = peakA;
                PeakB = peakB;
                PeakC = peakC;
            }

            public double L2A { get; }
            public double L2B { get; }
            public double L2C { get; }
            public double PeakA { get; }
            public double PeakB { get; }
            public double PeakC { get; }
        }
    }
}
