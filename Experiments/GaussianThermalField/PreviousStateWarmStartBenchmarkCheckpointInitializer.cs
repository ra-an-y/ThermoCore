using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class PreviousStateWarmStartBenchmarkCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            const double threshold = 5e-3;
            var result = PreviousStateWarmStartBenchmark1D.Evaluate();
            var pass = result.Satisfies(threshold);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Previous-State Warm-Start Benchmark");
            Console.WriteLine("time | stage | counts A/B/C | fresh min | reuse error | amp-refit error | final error | overhead");
            for (var i = 0; i < result.TraceCount; i++)
            {
                var point = result.GetTracePoint(i);
                var reuse = double.IsNaN(point.ReuseError) ? "n/a" : point.ReuseError.ToString("E8");
                var amplitude = double.IsNaN(point.AmplitudeRefitError)
                    ? "n/a"
                    : double.IsInfinity(point.AmplitudeRefitError)
                        ? "inf"
                        : point.AmplitudeRefitError.ToString("E8");

                Console.WriteLine(
                    $"{point.Time,4:F2} | {point.Stage,-14} | "
                    + $"{point.CountA}/{point.CountB}/{point.CountC} | "
                    + $"{point.FreshMinimumTotal,9} | {reuse,11} | {amplitude,15} | "
                    + $"{point.FinalError:E8} | {point.CountOverhead,8}");
            }

            Console.WriteLine();
            Console.WriteLine($"Measured trials per transition: {result.TrialCount}");
            Console.WriteLine("target time | fresh median ms | warm median ms | speedup | reduction | fresh min | warm min");
            for (var i = 0; i < result.TimingCount; i++)
            {
                var point = result.GetTimingPoint(i);
                Console.WriteLine(
                    $"{point.Time,11:F2} | {point.FreshMedianMilliseconds,15:F3} | "
                    + $"{point.WarmMedianMilliseconds,14:F3} | {point.Speedup,7:F2}x | "
                    + $"{point.ReductionFraction,8:P1} | {point.FreshMinimumMilliseconds,9:F3} | "
                    + $"{point.WarmMinimumMilliseconds,8:F3}");
            }

            Console.WriteLine($"Initial fresh build median: {result.InitialFreshMedianMilliseconds:F3} ms");
            Console.WriteLine($"Steady-state fresh median-sum: {result.SteadyFreshMedianSumMilliseconds:F3} ms");
            Console.WriteLine($"Steady-state warm median-sum: {result.SteadyWarmMedianSumMilliseconds:F3} ms");
            Console.WriteLine($"Steady-state warm speedup: {result.SteadySpeedup:F2}x");
            Console.WriteLine($"Steady-state warm reduction: {result.SteadyReductionFraction:P2}");
            Console.WriteLine($"Cold-inclusive fresh total: {result.ColdFreshTotalMilliseconds:F3} ms");
            Console.WriteLine($"Cold-inclusive warm total: {result.ColdWarmTotalMilliseconds:F3} ms");
            Console.WriteLine($"Cold-inclusive speedup: {result.ColdSpeedup:F2}x");
            Console.WriteLine($"Cold-inclusive reduction: {result.ColdReductionFraction:P2}");
            Console.WriteLine("Timed warm path: direct reuse check -> fixed-support amplitude refit -> proposal-bounded fresh fallback.");
            Console.WriteLine(pass ? "WARM-START BENCHMARK PASS" : "WARM-START BENCHMARK FAIL");
            Console.WriteLine();

            if (!pass)
            {
                throw new InvalidOperationException(
                    "Previous-state warm-start benchmark failed accuracy/data checks.");
            }
        }
    }
}
