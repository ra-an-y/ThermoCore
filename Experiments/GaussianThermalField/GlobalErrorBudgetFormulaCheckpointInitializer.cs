using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class GlobalErrorBudgetFormulaCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            var study = GlobalErrorBudgetFormulaStudy1D.Evaluate();
            const double globalThreshold = 5e-3;
            const double identityTolerance = 1e-12;
            var passed = study.Satisfies(globalThreshold, identityTolerance);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Global Error-Budget Formula Study");
            Console.WriteLine("time | counts A/B/C | total | weights A/B/C | regional e A/B/C | predicted global | direct global | identity error");

            for (var i = 0; i < study.Count; i++)
            {
                var point = study.GetPoint(i);
                Console.WriteLine(
                    $"{point.Time,4:F2} | {point.CountA}/{point.CountB}/{point.CountC} | "
                    + $"{point.TotalCount,5} | "
                    + $"{point.WeightA:F4}/{point.WeightB:F4}/{point.WeightC:F4} | "
                    + $"{point.RegionalErrorA:F4}/{point.RegionalErrorB:F4}/{point.RegionalErrorC:F4} | "
                    + $"{point.PredictedGlobalError:E8} | {point.DirectGlobalError:E8} | "
                    + $"{point.IdentityError:E3}");
            }

            Console.WriteLine(
                $"Formula-selected total range: {study.MinimumTotalCount}..{study.MaximumTotalCount}");
            Console.WriteLine(
                $"Maximum predicted/direct identity error: {study.MaximumIdentityError:E3}");
            Console.WriteLine(passed ? "FORMULA PASS" : "FORMULA FAIL");
            Console.WriteLine();

            if (!passed)
            {
                throw new InvalidOperationException(
                    "Global error-budget formula checkpoint failed.");
            }
        }
    }
}
