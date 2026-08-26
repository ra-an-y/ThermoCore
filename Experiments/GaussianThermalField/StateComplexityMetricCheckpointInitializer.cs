using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class StateComplexityMetricCheckpointInitializer
    {
        [ModuleInitializer]
        internal static void Run()
        {
            var study = StateComplexityMetricStudy1D.Evaluate();
            var passed = study.SatisfiesDataIntegrity();

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — State Complexity Metric Study");
            Console.WriteLine("time | region | required N | L2 contrib | peak contrib | rms-n | quartic-n | high-mode | entropy | Neff | grad | curvature");

            for (var i = 0; i < study.SampleCount; i++)
            {
                var sample = study.GetSample(i);
                Console.WriteLine(
                    $"{sample.Time,4:F2} | {sample.Region} | {sample.RequiredGaussianCount,10} | "
                    + $"{sample.L2Contribution:E3} | {sample.PeakContribution:E3} | "
                    + $"{sample.RmsModeIndex:F4} | {sample.QuarticModeIndex:F4} | "
                    + $"{sample.HighModeEnergyFraction:F4} | {sample.SpectralEntropy:F4} | "
                    + $"{sample.EffectiveModeCount:F4} | {sample.NormalizedGradientScore:F4} | "
                    + $"{sample.NormalizedCurvatureScore:F4}");
            }

            Console.WriteLine();
            Console.WriteLine("metric | Pearson(required N) | Spearman(required N)");
            for (var i = 0; i < study.CorrelationCount; i++)
            {
                var correlation = study.GetCorrelation(i);
                Console.WriteLine(
                    $"{correlation.Name} | {correlation.Pearson:F6} | {correlation.Spearman:F6}");
            }

            Console.WriteLine(
                $"Best |Pearson|: {study.BestPearsonMetric} = {study.BestAbsolutePearson:F6}");
            Console.WriteLine(
                $"Best |Spearman|: {study.BestSpearmanMetric} = {study.BestAbsoluteSpearman:F6}");
            Console.WriteLine(passed ? "DATA PASS" : "DATA FAIL");
            Console.WriteLine();

            if (!passed)
            {
                throw new InvalidOperationException(
                    "State complexity metric study failed data-integrity checks.");
            }
        }
    }
}
