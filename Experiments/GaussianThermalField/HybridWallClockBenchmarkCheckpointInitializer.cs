using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class HybridWallClockBenchmarkCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            var result = HybridWallClockBenchmark1D.Evaluate();
            var dataPass = result.SatisfiesDataIntegrity();

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Hybrid Wall-Clock Benchmark");
            Console.WriteLine($"Measured trials per snapshot: {result.TrialCount}");
            Console.WriteLine("time | proposal A/B/C | fit levels | exhaustive median ms | hybrid median ms | speedup | reduction | exhaustive min | hybrid min");
            for (var i = 0; i < result.Count; i++)
            {
                var point = result.GetPoint(i);
                Console.WriteLine(
                    $"{point.Time,4:F2} | {point.ProposalA}/{point.ProposalB}/{point.ProposalC,1} | "
                    + $"{point.ProposalFitLevels,10}/24 | {point.ExhaustiveMedianMilliseconds,20:F3} | "
                    + $"{point.HybridMedianMilliseconds,16:F3} | {point.MedianSpeedup,7:F3}x | "
                    + $"{point.MedianReductionFraction,8:P1} | {point.ExhaustiveMinimumMilliseconds,14:F3} | "
                    + $"{point.HybridMinimumMilliseconds,10:F3}");
            }

            Console.WriteLine($"Aggregate median-sum exhaustive: {result.ExhaustiveMedianSumMilliseconds:F3} ms");
            Console.WriteLine($"Aggregate median-sum hybrid: {result.HybridMedianSumMilliseconds:F3} ms");
            Console.WriteLine($"Aggregate speedup: {result.AggregateSpeedup:F3}x");
            Console.WriteLine($"Aggregate wall-clock reduction: {result.AggregateReductionFraction:P2}");
            Console.WriteLine("Timed scope: online nested Gaussian fitting only; snapshot construction/offline calibration/oracle excluded.");
            Console.WriteLine(dataPass ? "BENCHMARK DATA PASS" : "BENCHMARK DATA FAIL");
            Console.WriteLine();

            if (!dataPass)
            {
                throw new InvalidOperationException(
                    "Hybrid wall-clock benchmark failed data-integrity checks.");
            }
        }
    }
}
