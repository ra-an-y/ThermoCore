using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class PredictionResidualUncertaintyCheckpointInitializer
    {
        private const double Threshold = 5e-3;

        [ModuleInitializer]
        internal static void Run()
        {
            var study = PredictionResidualUncertaintyStudy1D.Evaluate();
            var heldOutResiduals = HeldOutResidualStructureStudy1D.Evaluate();
            var dataPass = study.SatisfiesDataIntegrity()
                && heldOutResiduals.SatisfiesDataIntegrity();

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Prediction Residual / Uncertainty Structure");
            Console.WriteLine("Held-out residual diagnostics (training residual correlations are intentionally not used):");
            Console.WriteLine($"Residual corr(log N): {heldOutResiduals.CorrelationWithLogCount:F6}");
            Console.WriteLine($"Residual corr(log(1+C)): {heldOutResiduals.CorrelationWithLogCurvature:F6}");
            Console.WriteLine($"Residual corr(log(1+B)): {heldOutResiduals.CorrelationWithLogBoundaryContrast:F6}");
            Console.WriteLine($"Held-out residual points: {heldOutResiduals.ResidualPointCount}");
            Console.WriteLine("N | mean training safety | max training safety | held-out max underprediction | held-out log-RMS");
            for (var n = 1; n <= 8; n++)
            {
                Console.WriteLine(
                    $"{n} | {study.GetMeanCountSafety(n),9:F4} | {study.GetMaximumCountSafety(n),9:F4} | "
                    + $"{heldOutResiduals.GetMaxUnderpredictionByCount(n),12:F4} | "
                    + $"{heldOutResiduals.GetRmsLogResidualByCount(n),9:F4}");
            }

            Console.WriteLine();
            Console.WriteLine("time | global-total | count-total | count predicted | count actual | oracle total | overhead | safe");
            for (var i = 0; i < study.FoldCount; i++)
            {
                var fold = study.GetFold(i);
                var globalTotal = fold.GlobalFeasible ? fold.GlobalTotal.ToString() : "-";
                var countTotal = fold.CountFeasible ? fold.CountTotal.ToString() : "-";
                var predicted = fold.CountFeasible ? fold.CountPredicted.ToString("E8") : "n/a";
                var actual = fold.CountFeasible ? fold.CountActual.ToString("E8") : "n/a";
                var overhead = fold.CountFeasible ? fold.CountOverhead.ToString() : "-";
                var safe = fold.CountFeasible && fold.CountActual <= Threshold;
                Console.WriteLine(
                    $"{fold.Time,4:F2} | {globalTotal,12} | {countTotal,11} | {predicted,15} | {actual,12} | "
                    + $"{fold.OracleTotal,12} | {overhead,8} | {safe}");
            }

            Console.WriteLine($"Count-dependent feasible folds: {study.CountFeasibleFolds}/{study.FoldCount}");
            Console.WriteLine($"Count-dependent safe folds: {study.CountSafeFolds}/{study.FoldCount}");
            Console.WriteLine($"Maximum count-dependent overhead: {study.MaximumCountOverhead}");
            Console.WriteLine(dataPass ? "DATA PASS" : "DATA FAIL");
            Console.WriteLine();

            if (!dataPass)
            {
                throw new InvalidOperationException(
                    "Prediction residual uncertainty study failed data-integrity checks.");
            }
        }
    }
}
