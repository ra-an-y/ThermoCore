using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct RegionalErrorCurveLawSample1D
    {
        public RegionalErrorCurveLawSample1D(
            double time,
            char region,
            double curvature,
            double exponentialA,
            double exponentialLambda,
            double exponentialR2,
            double exponentialLogRmse,
            double powerA,
            double powerExponent,
            double powerR2,
            double powerLogRmse)
        {
            Time = time;
            Region = region;
            Curvature = curvature;
            ExponentialA = exponentialA;
            ExponentialLambda = exponentialLambda;
            ExponentialR2 = exponentialR2;
            ExponentialLogRmse = exponentialLogRmse;
            PowerA = powerA;
            PowerExponent = powerExponent;
            PowerR2 = powerR2;
            PowerLogRmse = powerLogRmse;
        }

        public double Time { get; }
        public char Region { get; }
        public double Curvature { get; }
        public double ExponentialA { get; }
        public double ExponentialLambda { get; }
        public double ExponentialR2 { get; }
        public double ExponentialLogRmse { get; }
        public double PowerA { get; }
        public double PowerExponent { get; }
        public double PowerR2 { get; }
        public double PowerLogRmse { get; }
        public bool ExponentialWins => ExponentialLogRmse < PowerLogRmse;
    }

    public readonly struct RegionalErrorCurveLawModel1D
    {
        public RegionalErrorCurveLawModel1D(
            string name,
            double beta0,
            double betaCurvature,
            double betaCount,
            double logR2,
            double logRmse,
            double leaveOneStateOutLogRmse)
        {
            Name = name;
            Beta0 = beta0;
            BetaCurvature = betaCurvature;
            BetaCount = betaCount;
            LogR2 = logR2;
            LogRmse = logRmse;
            LeaveOneStateOutLogRmse = leaveOneStateOutLogRmse;
        }

        public string Name { get; }
        public double Beta0 { get; }
        public double BetaCurvature { get; }
        public double BetaCount { get; }
        public double LogR2 { get; }
        public double LogRmse { get; }
        public double LeaveOneStateOutLogRmse { get; }
    }

    public readonly struct RegionalErrorCurveLawStudyResult1D
    {
        private readonly RegionalErrorCurveLawSample1D[] _samples;

        public RegionalErrorCurveLawStudyResult1D(
            RegionalErrorCurveLawSample1D[] samples,
            RegionalErrorCurveLawModel1D exponentialCurvatureModel,
            RegionalErrorCurveLawModel1D powerCurvatureModel,
            int exponentialWinnerCount,
            int powerWinnerCount,
            double meanExponentialR2,
            double meanPowerR2)
        {
            _samples = samples;
            ExponentialCurvatureModel = exponentialCurvatureModel;
            PowerCurvatureModel = powerCurvatureModel;
            ExponentialWinnerCount = exponentialWinnerCount;
            PowerWinnerCount = powerWinnerCount;
            MeanExponentialR2 = meanExponentialR2;
            MeanPowerR2 = meanPowerR2;
        }

        public int SampleCount => _samples?.Length ?? 0;
        public RegionalErrorCurveLawModel1D ExponentialCurvatureModel { get; }
        public RegionalErrorCurveLawModel1D PowerCurvatureModel { get; }
        public int ExponentialWinnerCount { get; }
        public int PowerWinnerCount { get; }
        public double MeanExponentialR2 { get; }
        public double MeanPowerR2 { get; }

        public RegionalErrorCurveLawSample1D GetSample(int index)
        {
            if (_samples is null || index < 0 || index >= _samples.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _samples[index];
        }

        public bool SatisfiesDataIntegrity()
        {
            if (SampleCount < 8
                || ExponentialWinnerCount + PowerWinnerCount != SampleCount)
            {
                return false;
            }

            for (var i = 0; i < SampleCount; i++)
            {
                var sample = _samples[i];
                if (!IsFinite(sample.Curvature)
                    || !IsFinite(sample.ExponentialA)
                    || !IsFinite(sample.ExponentialLambda)
                    || !IsFinite(sample.ExponentialR2)
                    || !IsFinite(sample.ExponentialLogRmse)
                    || !IsFinite(sample.PowerA)
                    || !IsFinite(sample.PowerExponent)
                    || !IsFinite(sample.PowerR2)
                    || !IsFinite(sample.PowerLogRmse))
                {
                    return false;
                }
            }

            return ModelIsFinite(ExponentialCurvatureModel)
                && ModelIsFinite(PowerCurvatureModel)
                && IsFinite(MeanExponentialR2)
                && IsFinite(MeanPowerR2);
        }

        private static bool ModelIsFinite(RegionalErrorCurveLawModel1D model)
            => IsFinite(model.Beta0)
                && IsFinite(model.BetaCurvature)
                && IsFinite(model.BetaCount)
                && IsFinite(model.LogR2)
                && IsFinite(model.LogRmse)
                && IsFinite(model.LeaveOneStateOutLogRmse);

        private static bool IsFinite(double value)
            => !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// Exploratory study toward a direct regional Gaussian-error law.
    ///
    /// For each non-negligible A/B/C state snapshot, the observed sparse-fit
    /// error curve e(N), N=1..8, is compared against:
    ///
    ///   e(N) = A exp(-lambda N)
    ///   e(N) = A N^(-p)
    ///
    /// A second stage then asks whether one pooled formula using only the
    /// current state's normalized curvature score and Gaussian count can
    /// predict log error across states.
    /// </summary>
    public static class RegionalErrorCurveLawStudy1D
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

        public static RegionalErrorCurveLawStudyResult1D Evaluate()
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

            var records = new List<CurveRecord>();
            var samples = new List<RegionalErrorCurveLawSample1D>();
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

                AddRegion(
                    records,
                    samples,
                    targetTime,
                    'A',
                    state.StateA,
                    lengthA,
                    contribution.L2A,
                    contribution.PeakA);
                AddRegion(
                    records,
                    samples,
                    targetTime,
                    'B',
                    state.StateB,
                    lengthB,
                    contribution.L2B,
                    contribution.PeakB);
                AddRegion(
                    records,
                    samples,
                    targetTime,
                    'C',
                    state.StateC,
                    lengthC,
                    contribution.L2C,
                    contribution.PeakC);
            }

            var exponentialWinners = 0;
            var powerWinners = 0;
            var exponentialR2Sum = 0.0;
            var powerR2Sum = 0.0;

            for (var i = 0; i < samples.Count; i++)
            {
                if (samples[i].ExponentialWins)
                {
                    exponentialWinners++;
                }
                else
                {
                    powerWinners++;
                }
                exponentialR2Sum += samples[i].ExponentialR2;
                powerR2Sum += samples[i].PowerR2;
            }

            var exponentialModel = FitPooledModel(records, useLogCount: false, "curvature-exponential");
            var powerModel = FitPooledModel(records, useLogCount: true, "curvature-power");

            return new RegionalErrorCurveLawStudyResult1D(
                samples.ToArray(),
                exponentialModel,
                powerModel,
                exponentialWinners,
                powerWinners,
                exponentialR2Sum / samples.Count,
                powerR2Sum / samples.Count);
        }

        private static void AddRegion(
            List<CurveRecord> records,
            List<RegionalErrorCurveLawSample1D> samples,
            double time,
            char region,
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
            var curvature = ComputeNormalizedCurvature(state);
            var errors = new double[MaximumGaussianCount];

            for (var i = 0; i < MaximumGaussianCount; i++)
            {
                errors[i] = Math.Max(fits[i].RelativeError, 1e-15);
            }

            var exponential = FitCurve(errors, useLogCount: false);
            var power = FitCurve(errors, useLogCount: true);

            samples.Add(new RegionalErrorCurveLawSample1D(
                time,
                region,
                curvature,
                Math.Exp(exponential.Intercept),
                -exponential.Slope,
                exponential.R2,
                exponential.Rmse,
                Math.Exp(power.Intercept),
                -power.Slope,
                power.R2,
                power.Rmse));

            records.Add(new CurveRecord(time, region, curvature, errors));
        }

        private static LineFit FitCurve(double[] errors, bool useLogCount)
        {
            var x = new double[errors.Length];
            var y = new double[errors.Length];
            for (var i = 0; i < errors.Length; i++)
            {
                var count = i + 1.0;
                x[i] = useLogCount ? Math.Log(count) : count;
                y[i] = Math.Log(errors[i]);
            }
            return FitLine(x, y);
        }

        private static RegionalErrorCurveLawModel1D FitPooledModel(
            List<CurveRecord> records,
            bool useLogCount,
            string name)
        {
            var coefficients = FitPooledCoefficients(records, useLogCount, excludedRecord: -1);
            var fitQuality = EvaluatePooled(records, coefficients, useLogCount, excludedRecord: -1);

            var cvSquaredError = 0.0;
            var cvCount = 0;
            for (var excluded = 0; excluded < records.Count; excluded++)
            {
                var fold = FitPooledCoefficients(records, useLogCount, excluded);
                var quality = EvaluatePooled(records, fold, useLogCount, excludedRecord: excluded, onlyExcluded: true);
                cvSquaredError += quality.SumSquaredError;
                cvCount += quality.Count;
            }

            var cvRmse = Math.Sqrt(cvSquaredError / Math.Max(cvCount, 1));

            return new RegionalErrorCurveLawModel1D(
                name,
                coefficients[0],
                coefficients[1],
                coefficients[2],
                fitQuality.R2,
                fitQuality.Rmse,
                cvRmse);
        }

        private static double[] FitPooledCoefficients(
            List<CurveRecord> records,
            bool useLogCount,
            int excludedRecord)
        {
            var normal = new double[3, 3];
            var rhs = new double[3];

            for (var recordIndex = 0; recordIndex < records.Count; recordIndex++)
            {
                if (recordIndex == excludedRecord)
                {
                    continue;
                }

                var record = records[recordIndex];
                var curvatureFeature = Math.Log(1.0 + record.Curvature);

                for (var i = 0; i < record.Errors.Length; i++)
                {
                    var count = i + 1.0;
                    var countFeature = useLogCount ? Math.Log(count) : count;
                    var row = new[] { 1.0, curvatureFeature, countFeature };
                    var target = Math.Log(Math.Max(record.Errors[i], 1e-15));

                    for (var r = 0; r < 3; r++)
                    {
                        rhs[r] += row[r] * target;
                        for (var c = 0; c < 3; c++)
                        {
                            normal[r, c] += row[r] * row[c];
                        }
                    }
                }
            }

            return Solve3x3(normal, rhs);
        }

        private static PooledQuality EvaluatePooled(
            List<CurveRecord> records,
            double[] coefficients,
            bool useLogCount,
            int excludedRecord,
            bool onlyExcluded = false)
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
                var curvatureFeature = Math.Log(1.0 + record.Curvature);
                for (var i = 0; i < record.Errors.Length; i++)
                {
                    var count = i + 1.0;
                    var countFeature = useLogCount ? Math.Log(count) : count;
                    var prediction = coefficients[0]
                        + coefficients[1] * curvatureFeature
                        + coefficients[2] * countFeature;
                    targets.Add(Math.Log(Math.Max(record.Errors[i], 1e-15)));
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
            var rmse = Math.Sqrt(squaredError / Math.Max(targets.Count, 1));
            return new PooledQuality(r2, rmse, squaredError, targets.Count);
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

        private static LineFit FitLine(double[] x, double[] y)
        {
            var meanX = 0.0;
            var meanY = 0.0;
            for (var i = 0; i < x.Length; i++)
            {
                meanX += x[i];
                meanY += y[i];
            }
            meanX /= x.Length;
            meanY /= y.Length;

            var covariance = 0.0;
            var varianceX = 0.0;
            for (var i = 0; i < x.Length; i++)
            {
                var dx = x[i] - meanX;
                covariance += dx * (y[i] - meanY);
                varianceX += dx * dx;
            }

            var slope = covariance / Math.Max(varianceX, 1e-30);
            var intercept = meanY - slope * meanX;

            var squaredError = 0.0;
            var totalSquared = 0.0;
            for (var i = 0; i < x.Length; i++)
            {
                var prediction = intercept + slope * x[i];
                var residual = y[i] - prediction;
                squaredError += residual * residual;
                var centered = y[i] - meanY;
                totalSquared += centered * centered;
            }

            var r2 = totalSquared > 0.0
                ? 1.0 - squaredError / totalSquared
                : 1.0;
            return new LineFit(
                intercept,
                slope,
                r2,
                Math.Sqrt(squaredError / x.Length));
        }

        private static double[] Solve3x3(double[,] matrix, double[] rhs)
        {
            var augmented = new double[3, 4];
            for (var r = 0; r < 3; r++)
            {
                for (var c = 0; c < 3; c++)
                {
                    augmented[r, c] = matrix[r, c];
                }
                augmented[r, 3] = rhs[r];
            }

            for (var pivot = 0; pivot < 3; pivot++)
            {
                var best = pivot;
                for (var r = pivot + 1; r < 3; r++)
                {
                    if (Math.Abs(augmented[r, pivot]) > Math.Abs(augmented[best, pivot]))
                    {
                        best = r;
                    }
                }

                if (Math.Abs(augmented[best, pivot]) <= 1e-18)
                {
                    throw new InvalidOperationException("Pooled regression is singular.");
                }

                if (best != pivot)
                {
                    for (var c = pivot; c < 4; c++)
                    {
                        var temp = augmented[pivot, c];
                        augmented[pivot, c] = augmented[best, c];
                        augmented[best, c] = temp;
                    }
                }

                var pivotValue = augmented[pivot, pivot];
                for (var c = pivot; c < 4; c++)
                {
                    augmented[pivot, c] /= pivotValue;
                }

                for (var r = 0; r < 3; r++)
                {
                    if (r == pivot)
                    {
                        continue;
                    }
                    var factor = augmented[r, pivot];
                    for (var c = pivot; c < 4; c++)
                    {
                        augmented[r, c] -= factor * augmented[pivot, c];
                    }
                }
            }

            return new[] { augmented[0, 3], augmented[1, 3], augmented[2, 3] };
        }

        private readonly struct LineFit
        {
            public LineFit(double intercept, double slope, double r2, double rmse)
            {
                Intercept = intercept;
                Slope = slope;
                R2 = r2;
                Rmse = rmse;
            }

            public double Intercept { get; }
            public double Slope { get; }
            public double R2 { get; }
            public double Rmse { get; }
        }

        private readonly struct CurveRecord
        {
            public CurveRecord(double time, char region, double curvature, double[] errors)
            {
                Time = time;
                Region = region;
                Curvature = curvature;
                Errors = errors;
            }

            public double Time { get; }
            public char Region { get; }
            public double Curvature { get; }
            public double[] Errors { get; }
        }

        private readonly struct PooledQuality
        {
            public PooledQuality(double r2, double rmse, double sumSquaredError, int count)
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
