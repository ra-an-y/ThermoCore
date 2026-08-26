using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class SequentialWarmStartSchedulerCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            const double threshold = 5e-3;
            var result = SequentialWarmStartSchedulerStudy1D.Evaluate();
            var pass = result.Satisfies(threshold);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Sequential 16-ms Warm-Start Scheduler");
            Console.WriteLine($"Updates: {result.UpdateCount}");
            Console.WriteLine($"Direct reuse: {result.ReuseCount}");
            Console.WriteLine($"Amplitude-only refit: {result.AmplitudeRefitCount}");
            Console.WriteLine($"Fresh fallback: {result.FreshFallbackCount}");
            Console.WriteLine($"Maximum accepted global error: {result.MaximumFinalError:E8}");
            Console.WriteLine($"Measured trials: {result.TrialCount}");
            Console.WriteLine($"Fresh trajectory median: {result.FreshMedianMilliseconds:F3} ms");
            Console.WriteLine($"Warm trajectory median: {result.WarmMedianMilliseconds:F3} ms");
            Console.WriteLine($"Fresh mean/update: {result.FreshMeanPerUpdateMilliseconds:F3} ms");
            Console.WriteLine($"Warm mean/update: {result.WarmMeanPerUpdateMilliseconds:F3} ms");
            Console.WriteLine($"Sequential speedup: {result.Speedup:F2}x");
            Console.WriteLine($"Sequential wall-clock reduction: {result.ReductionFraction:P2}");
            Console.WriteLine("Window: t=0.600..0.792 s, 12 updates at 16 ms physical cadence, fixed 2/3/2 Gaussian budget.");
            Console.WriteLine("Accepted representation is carried into the next update; warm timing includes reuse validation, amplitude refit when needed, and fresh same-budget fallback when needed.");
            Console.WriteLine(pass ? "SEQUENTIAL WARM-START PASS" : "SEQUENTIAL WARM-START FAIL");
            Console.WriteLine();

            if (!pass)
            {
                throw new InvalidOperationException(
                    "Sequential warm-start scheduler failed accuracy/data checks.");
            }
        }
    }
}
