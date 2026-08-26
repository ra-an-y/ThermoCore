using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class HybridFormulaLocalVerifyCheckpointInitializer
    {
        private const double Threshold = 5e-3;
        private const double IdentityTolerance = 1e-12;

        [ModuleInitializer]
        internal static void Run()
        {
            var study = HybridFormulaLocalVerifyStudy1D.Evaluate();
            var pass = study.SatisfiesDataIntegrity(Threshold, IdentityTolerance);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Hybrid Formula-Propose / Local-Verify Allocator");
            Console.WriteLine("time | proposal A/B/C | proposal total | hybrid A/B/C | hybrid total | direct error | oracle total | fit levels | saved | reduction | oracle-total match");

            for (var i = 0; i < study.Count; i++)
            {
                var point = study.GetPoint(i);
                Console.WriteLine(
                    $"{point.Time,4:F2} | "
                    + $"{point.ProposalA}/{point.ProposalB}/{point.ProposalC,1} | {point.ProposalTotal,14} | "
                    + $"{point.HybridA}/{point.HybridB}/{point.HybridC,1} | {point.HybridTotal,12} | "
                    + $"{point.HybridDirectGlobal:E8} | {point.OracleTotal,12} | "
                    + $"{point.EvaluatedFitLevels,10}/{point.ExhaustiveFitLevels,-2} | "
                    + $"{point.SavedFitLevels,5} | {point.FitLevelReductionFraction,8:P1} | "
                    + $"{point.MatchesOracleTotal}");
            }

            Console.WriteLine($"Safe snapshots: {study.SafeCount}/{study.Count}");
            Console.WriteLine($"Oracle-total matches: {study.OracleTotalMatchCount}/{study.Count}");
            Console.WriteLine($"Saved fit levels per snapshot: {study.MinimumSavedFitLevels}..{study.MaximumSavedFitLevels}");
            Console.WriteLine($"Mean fit-level reduction: {study.MeanReductionFraction:P2}");
            Console.WriteLine($"Maximum predicted/direct identity error: {study.MaximumIdentityError:E3}");
            Console.WriteLine(pass ? "HYBRID PASS" : "HYBRID FAIL");
            Console.WriteLine();

            if (!pass)
            {
                throw new InvalidOperationException(
                    "Hybrid formula/local verification study failed its declared checks.");
            }
        }
    }
}
