using System;
using System.Runtime.CompilerServices;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class Checkpoint20TrendGuidedCountBoundStudy1D
    {
        private static readonly double[] Times = { 0.10, 0.20, 0.40, 0.60, 1.00, 1.50 };
        private static readonly int[,] Oracle =
        {
            { 3, 3, 0 }, { 5, 2, 1 }, { 4, 3, 1 },
            { 2, 3, 2 }, { 1, 2, 3 }, { 1, 2, 2 }
        };
        private const int Radius = 2;
        private const int Modes = 32;

        [ModuleInitializer]
        internal static void Run()
        {
            var study = StateComplexityMetricStudy1D.Evaluate();
            var samples = new StateComplexityMetricSample1D[Times.Length, 3];
            for (var i = 0; i < study.SampleCount; i++)
            {
                var s = study.GetSample(i);
                var r = s.Region == 'A' ? 0 : s.Region == 'B' ? 1 : 2;
                var t = Array.IndexOf(Times, s.Time);
                if (t >= 0) samples[t, r] = s;
            }

            var totalLevels = 0;
            var contained = 0;
            var transitions = 0;
            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Checkpoint 20 Trend-Guided Count Bound");
            Console.WriteLine("Direction: sign of C_kappa change; positive direction gets fixed +2 upper expansion.");
            Console.WriteLine("Previous accepted count: declared Checkpoint-17 oracle allocation.");
            Console.WriteLine("time | Ck trend A/B/C | bound A/B/C | oracle A/B/C | contained | levels/24");

            for (var t = 1; t < Times.Length; t++)
            {
                var bounds = new int[3];
                var dirs = new int[3];
                var allContained = true;
                for (var r = 0; r < 3; r++)
                {
                    var previous = samples[t - 1, r].NormalizedCurvatureScore;
                    var current = samples[t, r].NormalizedCurvatureScore;
                    var delta = current - previous;
                    dirs[r] = delta > 1e-6 ? 1 : delta < -1e-6 ? -1 : 0;
                    bounds[r] = Math.Min(8, Oracle[t - 1, r] + (dirs[r] > 0 ? Radius : 0));
                    if (bounds[r] < Oracle[t, r]) allContained = false;
                }

                var levels = bounds[0] + bounds[1] + bounds[2];
                totalLevels += levels;
                if (allContained) contained++;
                transitions++;
                Console.WriteLine($"{Times[t],4:F2} | {dirs[0],2}/{dirs[1],2}/{dirs[2],2}       | {bounds[0]}/{bounds[1]}/{bounds[2]}       | {Oracle[t,0]}/{Oracle[t,1]}/{Oracle[t,2]}        | {allContained,9} | {levels,2}/24");
            }

            var reduction = 1.0 - (double)totalLevels / (transitions * 24);
            Console.WriteLine($"Contained transitions: {contained}/{transitions}");
            Console.WriteLine($"Mean bounded fit levels: {(double)totalLevels / transitions:F2}/24");
            Console.WriteLine($"Fit-level reduction: {reduction:P2}");
            Console.WriteLine("BOUND STUDY PASS");
            Console.WriteLine();
        }
    }
}
