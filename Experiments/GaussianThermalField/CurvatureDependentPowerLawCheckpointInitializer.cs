using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class CurvatureDependentPowerLawCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            var result = CurvatureDependentPowerLawStudy1D.Evaluate();
            var passed = result.SatisfiesDataIntegrity();

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Curvature-Dependent Power-Law Study");
            Console.WriteLine($"state samples: {result.StateSampleCount}");
            Console.WriteLine(
                $"log e = {result.Beta0:F6} + {result.BetaCurvature:F6} log(1+C) "
                + $"+ {result.BetaLogCount:F6} log N + {result.BetaInteraction:F6} log(1+C) log N");
            Console.WriteLine($"log-space R2: {result.LogR2:F6}");
            Console.WriteLine($"log-space RMSE: {result.LogRmse:F6}");
            Console.WriteLine($"LOSO log-RMSE: {result.LeaveOneStateOutLogRmse:F6}");
            Console.WriteLine($"baseline no-interaction LOSO log-RMSE: {result.BaselineLeaveOneStateOutLogRmse:F6}");
            Console.WriteLine(
                $"predicted power exponent range over sampled states: "
                + $"{result.MinimumPredictedExponent:F4}..{result.MaximumPredictedExponent:F4}");
            Console.WriteLine(passed ? "INTERACTION DATA PASS" : "INTERACTION DATA FAIL");
            Console.WriteLine();

            if (!passed)
            {
                throw new InvalidOperationException(
                    "Curvature-dependent power-law study failed data-integrity checks.");
            }
        }
    }
}
