using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class RegionalErrorLawFeatureSelectionCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            var result = RegionalErrorLawFeatureSelectionStudy1D.Evaluate();
            var passed = result.SatisfiesDataIntegrity();

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Regional Error-Law Feature Selection");
            Console.WriteLine("feature | params | train logR2 | train logRMSE | LOSO logRMSE");
            for (var i = 0; i < result.ModelCount; i++)
            {
                var model = result.GetModel(i);
                Console.WriteLine(
                    $"{model.FeatureName} | {model.ParameterCount} | "
                    + $"{model.TrainingLogR2:F6} | {model.TrainingLogRmse:F6} | "
                    + $"{model.LeaveOneStateOutLogRmse:F6}");
            }
            Console.WriteLine(
                $"Best LOSO feature: {result.BestFeatureName} = "
                + $"{result.BestLeaveOneStateOutLogRmse:F6}");
            Console.WriteLine(passed ? "FEATURE DATA PASS" : "FEATURE DATA FAIL");
            Console.WriteLine();

            if (!passed)
            {
                throw new InvalidOperationException(
                    "Regional error-law feature-selection study failed data-integrity checks.");
            }
        }
    }
}
