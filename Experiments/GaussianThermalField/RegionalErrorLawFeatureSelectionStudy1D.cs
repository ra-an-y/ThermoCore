using System;
using System.Collections.Generic;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct RegionalErrorLawFeatureModel1D
    {
        public RegionalErrorLawFeatureModel1D(
            string featureName,
            int parameterCount,
            double trainingLogR2,
            double trainingLogRmse,
            double leaveOneStateOutLogRmse)
        {
            FeatureName = featureName;
            ParameterCount = parameterCount;
            TrainingLogR2 = trainingLogR2;
            TrainingLogRmse = trainingLogRmse;
            LeaveOneStateOutLogRmse = leaveOneStateOutLogRmse;
        }

        public string FeatureName { get; }
        public int ParameterCount { get; }
        public double TrainingLogR2 { get; }
        public double TrainingLogRmse { get; }
        public double LeaveOneStateOutLogRmse { get; }
    }

    public readonly struct RegionalErrorLawFeatureSelectionResult1D
    {
        private readonly RegionalErrorLawFeatureModel1D[] _models;

        public RegionalErrorLawFeatureSelectionResult1D(
            RegionalErrorLawFeatureModel1D[] models,
            string bestFeatureName,
            double bestLeaveOneStateOutLogRmse)
        {
            _models = models;
            BestFeatureName = bestFeatureName;
            BestLeaveOneStateOutLogRmse = bestLeaveOneStateOutLogRmse;
        }

        public int ModelCount => _models?.Length ?? 0;
        public string BestFeatureName { get; }
        public double BestLeaveOneStateOutLogRmse { get; }

        public RegionalErrorLawFeatureModel1D GetModel(int index)
        {
            if (_models is null || index < 0 || index >= _models.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _models[index];
        }

        public bool SatisfiesDataIntegrity()
        {
            if (ModelCount < 4 || string.IsNullOrWhiteSpace(BestFeatureName))
            {
                return false;
            }

            for (var i = 0; i < ModelCount; i++)
            {
                var model = _models[i];
                if (model.ParameterCount < 4
                    || double.IsNaN(model.TrainingLogR2)
                    || double.IsInfinity(model.TrainingLogR2)
                    || double.IsNaN(model.TrainingLogRmse)
                    || double.IsInfinity(model.TrainingLogRmse)
                    || double.IsNaN(model.LeaveOneStateOutLogRmse)
                    || double.IsInfinity(model.LeaveOneStateOutLogRmse))
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Small feature-selection study for the regional power-law error model.
    /// Curvature remains the mandatory first shape coordinate. Exactly one
    /// additional current-state feature is added at a time, together with its
    /// interaction with log Gaussian count. Selection is based on grouped
    /// leave-one-state-out log RMSE, not training R2.
    /// </summary>
    public static class RegionalErrorLawFeatureSelectionStudy1D
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

        public static RegionalErrorLawFeatureSelectionResult1D Evaluate()
        {
            var records = BuildRecords();
            var definitions = new[]
            {
                new FeatureDefinition("curvature-only", FeatureKind.None),
                new FeatureDefinition("normalized-gradient", FeatureKind.Gradient),
                new FeatureDefinition("modal-energy-fraction", FeatureKind.ModalFraction),
                new FeatureDefinition("spectral-entropy", FeatureKind.Entropy),
                new FeatureDefinition("mean-dominance", FeatureKind.MeanDominance),
                new FeatureDefinition("boundary-contrast", FeatureKind.BoundaryContrast)
            };

            var models = new RegionalErrorLawFeatureModel1D[definitions.Length];
            var bestName = string.Empty;
            var bestCv = double.PositiveInfinity;

            for (var i = 0; i < definitions.Length; i++)
            {
                var definition = definitions[i];
                var beta = Fit(records, definition.Kind, excludedRecord: -1);
                var quality = Evaluate(records, beta, definition.Kind, -1, onlyExcluded: false);
                var cv = CrossValidatedRmse(records, definition.Kind);
                models[i] = new RegionalErrorLawFeatureModel1D(
                    definition.Name,
                    beta.Length,
                    quality.R2,
                    quality.Rmse,
                    cv);

                if (cv < bestCv)
                {
                    bestCv = cv;
                    bestName = definition.Name;
                }
            }

            return new RegionalErrorLawFeatureSelectionResult1D(
                models,
                bestName,
                bestCv);
        }

        private static List<Record> BuildRecords()
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
            var records = new List<Record>();
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

                var contribution = MeasureGlobalContributions(state, lengthA, lengthB, lengthC);
                AddRecord(records, state.StateA, lengthA, contribution.L2A, contribution.PeakA);
                AddRecord(records, state.StateB, lengthB, contribution.L2B, contribution.PeakB);
                AddRecord(records, state.StateC, lengthC, contribution.L2C, contribution.PeakC);
            }
            return records;
        }

        private static void AddRecord(
            List<Record> records,
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
            records.Add(new Record(metrics, errors));
        }

        private static MetricValues ComputeMetrics(
            in FiniteLayerReducedState1D state,
            double length)
        {
            var modalEnergy = 0.0;
            var n2Energy = 0.0;
            var n4Energy = 0.0;
            var entropy = 0.0;

            for (var modeIndex = 0; modeIndex < state.ModeCount; modeIndex++)
            {
                var n = modeIndex + 1.0;
                var coefficient = state.GetModeCoefficient(modeIndex);
                var energy = coefficient * coefficient;
                modalEnergy += energy;
                n2Energy += n * n * energy;
                n4Energy += n * n * n * n * energy;
            }

            if (modalEnergy > 1e-30)
            {
                for (var modeIndex = 0; modeIndex < state.ModeCount; modeIndex++)
                {
                    var coefficient = state.GetModeCoefficient(modeIndex);
                    var probability = coefficient * coefficient / modalEnergy;
                    if (probability > 0.0)
                    {
                        entropy -= probability * Math.Log(probability);
                    }
                }
            }

            var mean = state.MeanTemperaturePerturbation;
            var fieldEnergy = mean * mean + 0.5 * modalEnergy;
            var safeEnergy = Math.Max(fieldEnergy, 1e-30);
            var gradient = Math.Sqrt(0.5 * n2Energy / safeEnergy);
            var curvature = Math.Sqrt(0.5 * n4Energy / safeEnergy);
            var modalFraction = 0.5 * modalEnergy / safeEnergy;
            var normalizedEntropy = state.ModeCount > 1
                ? entropy / Math.Log(state.ModeCount)
                : 0.0;
            var meanDominance = Math.Abs(mean) / Math.Sqrt(safeEnergy);

            var left = FiniteLayerFieldRepresentation1D.Evaluate(state, 0.0, length);
            var right = FiniteLayerFieldRepresentation1D.Evaluate(state, length, length);
            var boundaryContrast = Math.Abs(left - right) / Math.Sqrt(safeEnergy);

            return new MetricValues(
                curvature,
                gradient,
                modalFraction,
                normalizedEntropy,
                meanDominance,
                boundaryContrast);
        }

        private static double[] Fit(
            List<Record> records,
            FeatureKind featureKind,
            int excludedRecord)
        {
            var dimension = featureKind == FeatureKind.None ? 4 : 6;
            var normal = new double[dimension, dimension];
            var rhs = new double[dimension];

            for (var recordIndex = 0; recordIndex < records.Count; recordIndex++)
            {
                if (recordIndex == excludedRecord)
                {
                    continue;
                }

                var record = records[recordIndex];
                var u = Math.Log(1.0 + record.Metrics.Curvature);
                var f = FeatureValue(record.Metrics, featureKind);

                for (var i = 0; i < record.Errors.Length; i++)
                {
                    var v = Math.Log(i + 1.0);
                    var row = featureKind == FeatureKind.None
                        ? new[] { 1.0, u, v, u * v }
                        : new[] { 1.0, u, f, v, u * v, f * v };
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

        private static Quality Evaluate(
            List<Record> records,
            double[] beta,
            FeatureKind featureKind,
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
                var u = Math.Log(1.0 + record.Metrics.Curvature);
                var f = FeatureValue(record.Metrics, featureKind);
                for (var i = 0; i < record.Errors.Length; i++)
                {
                    var v = Math.Log(i + 1.0);
                    var prediction = beta[0] + beta[1] * u;
                    if (featureKind == FeatureKind.None)
                    {
                        prediction += beta[2] * v + beta[3] * u * v;
                    }
                    else
                    {
                        prediction += beta[2] * f
                            + beta[3] * v
                            + beta[4] * u * v
                            + beta[5] * f * v;
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

            return new Quality(
                totalSquared > 0.0 ? 1.0 - squaredError / totalSquared : 1.0,
                Math.Sqrt(squaredError / Math.Max(targets.Count, 1)),
                squaredError,
                targets.Count);
        }

        private static double CrossValidatedRmse(
            List<Record> records,
            FeatureKind featureKind)
        {
            var squaredError = 0.0;
            var count = 0;
            for (var excluded = 0; excluded < records.Count; excluded++)
            {
                var beta = Fit(records, featureKind, excluded);
                var fold = Evaluate(records, beta, featureKind, excluded, onlyExcluded: true);
                squaredError += fold.SumSquaredError;
                count += fold.Count;
            }
            return Math.Sqrt(squaredError / Math.Max(count, 1));
        }

        private static double FeatureValue(MetricValues metrics, FeatureKind kind)
        {
            return kind switch
            {
                FeatureKind.Gradient => Math.Log(1.0 + metrics.Gradient),
                FeatureKind.ModalFraction => metrics.ModalFraction,
                FeatureKind.Entropy => metrics.Entropy,
                FeatureKind.MeanDominance => metrics.MeanDominance,
                FeatureKind.BoundaryContrast => Math.Log(1.0 + metrics.BoundaryContrast),
                _ => 0.0
            };
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
                    throw new InvalidOperationException("Feature-selection regression is singular.");
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

        private enum FeatureKind
        {
            None,
            Gradient,
            ModalFraction,
            Entropy,
            MeanDominance,
            BoundaryContrast
        }

        private readonly struct FeatureDefinition
        {
            public FeatureDefinition(string name, FeatureKind kind)
            {
                Name = name;
                Kind = kind;
            }
            public string Name { get; }
            public FeatureKind Kind { get; }
        }

        private readonly struct MetricValues
        {
            public MetricValues(
                double curvature,
                double gradient,
                double modalFraction,
                double entropy,
                double meanDominance,
                double boundaryContrast)
            {
                Curvature = curvature;
                Gradient = gradient;
                ModalFraction = modalFraction;
                Entropy = entropy;
                MeanDominance = meanDominance;
                BoundaryContrast = boundaryContrast;
            }
            public double Curvature { get; }
            public double Gradient { get; }
            public double ModalFraction { get; }
            public double Entropy { get; }
            public double MeanDominance { get; }
            public double BoundaryContrast { get; }
        }

        private readonly struct Record
        {
            public Record(MetricValues metrics, double[] errors)
            {
                Metrics = metrics;
                Errors = errors;
            }
            public MetricValues Metrics { get; }
            public double[] Errors { get; }
        }

        private readonly struct Quality
        {
            public Quality(double r2, double rmse, double sumSquaredError, int count)
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
