using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class RegionalErrorCurveLawCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            var study = RegionalErrorCurveLawStudy1D.Evaluate();
            var passed = study.SatisfiesDataIntegrity();

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Regional Error-Curve Law Study");
            Console.WriteLine("time | region | curvature | exp A/lambda/R2/RMSE | power A/p/R2/RMSE | winner");

            for (var i = 0; i < study.SampleCount; i++)
            {
                var sample = study.GetSample(i);
                Console.WriteLine(
                    $"{sample.Time,4:F2} | {sample.Region} | {sample.Curvature,9:F4} | "
                    + $"{sample.ExponentialA:E3}/{sample.ExponentialLambda:F4}/{sample.ExponentialR2:F4}/{sample.ExponentialLogRmse:F4} | "
                    + $"{sample.PowerA:E3}/{sample.PowerExponent:F4}/{sample.PowerR2:F4}/{sample.PowerLogRmse:F4} | "
                    + (sample.ExponentialWins ? "exp" : "power"));
            }

            Console.WriteLine();
            Console.WriteLine($"Individual winners: exponential={study.ExponentialWinnerCount}, power={study.PowerWinnerCount}");
            Console.WriteLine($"Mean individual R2: exponential={study.MeanExponentialR2:F6}, power={study.MeanPowerR2:F6}");
            PrintModel(study.ExponentialCurvatureModel);
            PrintModel(study.PowerCurvatureModel);
            Console.WriteLine(passed ? "LAW DATA PASS" : "LAW DATA FAIL");
            Console.WriteLine();

            if (!passed)
            {
                throw new InvalidOperationException(
                    "Regional error-curve law study failed data-integrity checks.");
            }
        }

        private static void PrintModel(RegionalErrorCurveLawModel1D model)
        {
            Console.WriteLine(
                $"{model.Name}: beta0={model.Beta0:F6}, betaC={model.BetaCurvature:F6}, "
                + $"betaN={model.BetaCount:F6}, logR2={model.LogR2:F6}, "
                + $"logRMSE={model.LogRmse:F6}, LOSO-logRMSE={model.LeaveOneStateOutLogRmse:F6}");
        }
    }
}
