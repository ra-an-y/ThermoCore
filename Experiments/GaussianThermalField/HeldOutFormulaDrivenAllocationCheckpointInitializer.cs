using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class HeldOutFormulaDrivenAllocationCheckpointInitializer
    {
        private const double Threshold = 5e-3;

        [ModuleInitializer]
        internal static void Run()
        {
            var study = HeldOutFormulaDrivenAllocationStudy1D.Evaluate();
            var dataPass = study.SatisfiesDataIntegrity();

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Held-Out Formula-Driven Allocation");
            Console.WriteLine("time | safety | formula A/B/C | total | predicted | actual | oracle A/B/C | oracle total | overhead | safe");

            for (var i = 0; i < study.Count; i++)
            {
                var point = study.GetPoint(i);
                var formulaCounts = point.FormulaFeasible
                    ? $"{point.CountA}/{point.CountB}/{point.CountC}"
                    : "none";
                var formulaTotal = point.FormulaFeasible
                    ? point.FormulaTotal.ToString()
                    : "-";
                var predicted = point.FormulaFeasible
                    ? point.PredictedGlobalError.ToString("E8")
                    : "n/a";
                var actual = point.FormulaFeasible
                    ? point.ActualGlobalError.ToString("E8")
                    : "n/a";
                var overhead = point.FormulaFeasible
                    ? point.BudgetOverhead.ToString()
                    : "-";
                var safe = point.FormulaFeasible
                    ? point.IsSafe(Threshold).ToString()
                    : "False";

                Console.WriteLine(
                    $"{point.Time,4:F2} | {point.SafetyFactor,6:F3} | "
                    + $"{formulaCounts,11} | {formulaTotal,5} | {predicted,12} | {actual,12} | "
                    + $"{point.OracleA}/{point.OracleB}/{point.OracleC} | {point.OracleTotal,6} | "
                    + $"{overhead,8} | {safe}");
            }

            Console.WriteLine($"Formula-feasible folds: {study.FeasibleCount}/{study.Count}");
            Console.WriteLine($"Held-out safe folds: {study.SafeCount}/{study.Count}");
            Console.WriteLine($"Exact oracle-total matches: {study.ExactOracleMatchCount}/{study.Count}");
            Console.WriteLine($"Maximum training-only safety factor: {study.MaximumSafetyFactor:F6}");
            Console.WriteLine($"Maximum Gaussian-count overhead: {study.MaximumBudgetOverhead}");
            Console.WriteLine(dataPass ? "DATA PASS" : "DATA FAIL");
            Console.WriteLine();

            if (!dataPass)
            {
                throw new InvalidOperationException(
                    "Held-out formula allocation study failed data-integrity checks.");
            }
        }
    }
}
