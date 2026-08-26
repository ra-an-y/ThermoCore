using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class FineCadenceWarmStartCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            var result = FineCadenceWarmStartStudy1D.Evaluate();
            var pass = result.SatisfiesDataIntegrity();

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Fine-Cadence Warm-Start Study");
            Console.WriteLine($"Measured trials per interval: {result.TrialCount}");
            Console.WriteLine("delta t | reuse err | amp err | fresh err | reuse pass | amp pass | fresh pass | fresh ms | warm ms | speedup | reduction");
            for (var i = 0; i < result.Count; i++)
            {
                var p = result.GetPoint(i);
                Console.WriteLine(
                    $"{p.DeltaTime,7:F3} | {p.ReuseError:E3} | {p.AmplitudeError:E3} | "
                    + $"{p.FreshError:E3} | {p.ReusePass,10} | {p.AmplitudePass,8} | {p.FreshPass,10} | "
                    + $"{p.FreshMedianMilliseconds,8:F3} | {p.WarmMedianMilliseconds,7:F3} | "
                    + $"{p.Speedup,7:F2}x | {p.ReductionFraction,8:P1}");
            }
            Console.WriteLine("Base state: t=0.60 s, verified 2/3/2 Gaussian allocation.");
            Console.WriteLine("Warm path: direct reuse, otherwise fixed-support amplitude refit; no fresh fallback hidden in timing.");
            Console.WriteLine(pass ? "FINE-CADENCE DATA PASS" : "FINE-CADENCE DATA FAIL");
            Console.WriteLine();

            if (!pass)
            {
                throw new InvalidOperationException(
                    "Fine-cadence warm-start study failed data-integrity checks.");
            }
        }
    }
}
