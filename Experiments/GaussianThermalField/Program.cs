using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class Program
    {
        private static int Main()
        {
            var perfectInterfacePassed = RunPerfectInterfaceCheckpoint();
            var finiteLayerPassed = RunFiniteLayerReducedStateCheckpoint();
            var convergencePassed = RunFiniteLayerModeConvergenceCheckpoint();
            var independentReferencePassed = RunFiniteLayerIndependentReferenceCheckpoint();
            var threeLayerCoupledPassed = RunThreeLayerCoupledCheckpoint();
            var gaussianStateBridgePassed = RunGaussianStateBridgeCheckpoint();
            var minimumRepresentationPassed = RunMinimumGaussianRepresentationCheckpoint();
            var adaptiveBudgetPassed = RunAdaptiveGaussianBudgetCheckpoint();
            var timeAdaptiveBudgetPassed = RunTimeAdaptiveGaussianBudgetCheckpoint();
            var negligibleRegionRulePassed = RunNegligibleRegionGaussianRuleCheckpoint();

            var passed = perfectInterfacePassed
                && finiteLayerPassed
                && convergencePassed
                && independentReferencePassed
                && threeLayerCoupledPassed
                && gaussianStateBridgePassed
                && minimumRepresentationPassed
                && adaptiveBudgetPassed
                && timeAdaptiveBudgetPassed
                && negligibleRegionRulePassed;

            Console.WriteLine();
            Console.WriteLine(passed ? "OVERALL PASS" : "OVERALL FAIL");

            return passed ? 0 : 1;
        }

        private static bool RunPerfectInterfaceCheckpoint()
        {
            var materialA = new ThermalMaterial1D(0.40, 2.0);
            var materialB = new ThermalMaterial1D(0.06, 1.2);

            const double sourcePosition = -0.55;
            const double interfacePosition = 0.0;
            const double elapsedTime = 0.9;
            const double sourceEnergy = 1.0;

            var incidentAmplitude = sourceEnergy / materialA.VolumetricHeatCapacity;

            var solution = PerfectInterfacePropagation1D.Create(
                sourcePosition,
                interfacePosition,
                elapsedTime,
                incidentAmplitude,
                materialA,
                materialB);

            var verification = PerfectInterfaceVerification1D.Evaluate(
                solution,
                materialA,
                materialB);

            const double tolerance = 1e-12;
            var passed = verification.Satisfies(tolerance);

            Console.WriteLine("Gaussian Thermal Field — Perfect Interface Checkpoint");
            Console.WriteLine($"Temperature jump: {verification.TemperatureJump:E16}");
            Console.WriteLine($"Conductive-gradient jump: {verification.ConductiveGradientJump:E16}");
            Console.WriteLine($"Tolerance: {tolerance:E2}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunFiniteLayerReducedStateCheckpoint()
        {
            var verification = FiniteLayerReducedStateVerification1D.Evaluate();

            const double tolerance = 1e-12;
            var passed = verification.Satisfies(tolerance);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Finite Layer Reduced-State Checkpoint");
            Console.WriteLine($"Energy-balance error: {verification.EnergyBalanceError:E16}");
            Console.WriteLine($"Maximum no-flux decay error: {verification.MaximumNoFluxDecayError:E16}");
            Console.WriteLine($"Maximum symmetric odd-mode magnitude: {verification.MaximumSymmetricOddModeMagnitude:E16}");
            Console.WriteLine($"Symmetric field mirror error: {verification.SymmetricFieldMirrorError:E16}");
            Console.WriteLine($"Tolerance: {tolerance:E2}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunFiniteLayerModeConvergenceCheckpoint()
        {
            var convergence = FiniteLayerModeConvergence1D.Evaluate();
            const double maximum32ModeRelativeError = 5e-3;
            var passed = convergence.Satisfies(maximum32ModeRelativeError);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Finite Layer Mode-Convergence Checkpoint");
            Console.WriteLine($"4 modes relative error: {convergence.Error4Modes:E8}");
            Console.WriteLine($"8 modes relative error: {convergence.Error8Modes:E8}");
            Console.WriteLine($"16 modes relative error: {convergence.Error16Modes:E8}");
            Console.WriteLine($"32 modes relative error: {convergence.Error32Modes:E8}");
            Console.WriteLine($"32-mode limit: {maximum32ModeRelativeError:E2}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunFiniteLayerIndependentReferenceCheckpoint()
        {
            var verification = FiniteLayerIndependentReference1D.Evaluate();

            const double maximumConstantFluxRelativeError = 5e-3;
            const double maximumPulseRelativeError = 1e-3;
            const double maximumEnergyError = 1e-10;

            var passed = verification.Satisfies(
                maximumConstantFluxRelativeError,
                maximumPulseRelativeError,
                maximumEnergyError);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Independent Finite-Volume Checkpoint");
            Console.WriteLine($"32-mode constant-flux relative error: {verification.ConstantFluxRelativeError32Modes:E8}");
            Console.WriteLine($"4-mode pulse-history relative error: {verification.PulseHistoryRelativeError4Modes:E8}");
            Console.WriteLine($"Constant-flux energy error: {verification.ConstantFluxEnergyError:E16}");
            Console.WriteLine($"Pulse energy error: {verification.PulseEnergyError:E16}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunThreeLayerCoupledCheckpoint()
        {
            var verification = ThreeLayerCoupledVerification1D.Evaluate();

            const double maximumRelativeFieldError = 5e-3;
            const double maximumInterfaceJump = 1e-10;
            const double maximumEnergyDrift = 1e-10;

            var passed = verification.Satisfies(
                maximumRelativeFieldError,
                maximumInterfaceJump,
                maximumEnergyDrift);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — State-Driven A-B-C Coupling Checkpoint");
            Console.WriteLine($"Relative field error vs heterogeneous FV: {verification.RelativeFieldError:E8}");
            Console.WriteLine($"Maximum interface temperature jump: {verification.MaximumInterfaceJump:E16}");
            Console.WriteLine($"Reduced-state energy drift: {verification.ReducedEnergyDrift:E16}");
            Console.WriteLine($"Reference energy drift: {verification.ReferenceEnergyDrift:E16}");
            Console.WriteLine($"Maximum |q_AB|: {verification.MaximumAbsoluteFluxAB:E8}");
            Console.WriteLine($"Maximum |q_BC|: {verification.MaximumAbsoluteFluxBC:E8}");
            Console.WriteLine($"Fixed retained state scalars: {verification.RetainedStateScalars}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunGaussianStateBridgeCheckpoint()
        {
            var verification = GaussianStateBridgeVerification1D.Evaluate();

            const double maximumProjectionError = 1e-3;
            const double maximumRecoveryError = 3.5e-3;
            const double maximumFiniteVolumeError = 5e-3;
            const double maximumEnergyError = 1e-8;
            const double maximumInterfaceJump = 1e-10;
            const int expectedGaussianTerms = 27;

            var passed = verification.Satisfies(
                maximumProjectionError,
                maximumRecoveryError,
                maximumFiniteVolumeError,
                maximumEnergyError,
                maximumInterfaceJump,
                expectedGaussianTerms);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Gaussian/State Bridge Checkpoint");
            Console.WriteLine($"Initial Gaussian -> state relative error: {verification.InitialProjectionRelativeError:E8}");
            Console.WriteLine($"Recovered Gaussian vs state relative error: {verification.RecoveredVsStateRelativeError:E8}");
            Console.WriteLine($"Recovered Gaussian vs heterogeneous FV: {verification.RecoveredVsFiniteVolumeRelativeError:E8}");
            Console.WriteLine($"Recovered representation energy error: {verification.RecoveredEnergyError:E16}");
            Console.WriteLine($"Maximum interface temperature jump: {verification.MaximumInterfaceJump:E16}");
            Console.WriteLine($"Fixed recovered Gaussian terms: {verification.RecoveredGaussianTerms}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunMinimumGaussianRepresentationCheckpoint()
        {
            var study = MinimumGaussianRepresentationStudy1D.Evaluate();

            const double maximumEightKernelGlobalError = 1e-3;
            const double maximumEightKernelFiniteVolumeError = 4e-3;
            const double maximumIntegralError = 1e-9;

            var passed = study.Satisfies(
                maximumEightKernelGlobalError,
                maximumEightKernelFiniteVolumeError,
                maximumIntegralError);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Minimum Gaussian Representation Study");
            Console.WriteLine("N/region | total N | global vs state | max region vs state | vs heterogeneous FV | max integral error");

            for (var index = 0; index < study.Count; index++)
            {
                var point = study.GetPoint(index);
                Console.WriteLine(
                    $"{point.KernelsPerRegion,8} | {point.TotalKernelCount,7} | "
                    + $"{point.GlobalRelativeErrorVsState:E8} | "
                    + $"{point.MaximumRegionRelativeErrorVsState:E8} | "
                    + $"{point.RelativeErrorVsFiniteVolume:E8} | "
                    + $"{point.MaximumRegionIntegralError:E8}");
            }

            Console.WriteLine(
                $"First N/region with global state error <= 0.5%: {study.FirstGlobalCountBelowHalfPercent}");
            Console.WriteLine(
                $"First N/region with every-region state error <= 0.5%: {study.FirstEveryRegionCountBelowHalfPercent}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunAdaptiveGaussianBudgetCheckpoint()
        {
            var study = AdaptiveGaussianBudgetStudy1D.Evaluate();

            const double perRegionThreshold = 5e-3;
            const double finiteVolumeThreshold = 5e-3;
            const int maximumValidatedTotalCount = 11;
            const double maximumIntegralError = 1e-9;

            var passed = study.Satisfies(
                perRegionThreshold,
                finiteVolumeThreshold,
                maximumValidatedTotalCount,
                maximumIntegralError);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Adaptive Per-Region Gaussian Budget");
            Console.WriteLine(
                $"Representation-local counts A/B/C: "
                + $"{study.LocalCountA}/{study.LocalCountB}/{study.LocalCountC} "
                + $"(total {study.LocalTotalCount})");
            Console.WriteLine($"Local global vs state: {study.LocalGlobalStateError:E8}");
            Console.WriteLine($"Local vs heterogeneous FV: {study.LocalFiniteVolumeError:E8}");
            Console.WriteLine(
                $"Validation-aware counts A/B/C: "
                + $"{study.ValidatedCountA}/{study.ValidatedCountB}/{study.ValidatedCountC} "
                + $"(total {study.ValidatedTotalCount})");
            Console.WriteLine($"Validated global vs state: {study.ValidatedGlobalStateError:E8}");
            Console.WriteLine($"Validated max region vs state: {study.MaximumValidatedRegionStateError:E8}");
            Console.WriteLine($"Validated vs heterogeneous FV: {study.ValidatedFiniteVolumeError:E8}");
            Console.WriteLine($"Validated max integral error: {study.MaximumIntegralError:E8}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunTimeAdaptiveGaussianBudgetCheckpoint()
        {
            var study = TimeAdaptiveGaussianBudgetStudy1D.Evaluate();

            const double perRegionThreshold = 5e-3;
            const double finiteVolumeThreshold = 5e-3;
            const double maximumIntegralError = 1e-9;

            var passed = study.Satisfies(
                perRegionThreshold,
                finiteVolumeThreshold,
                maximumIntegralError);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Time-Adaptive Gaussian Budget");
            Console.WriteLine("time | state-vs-FV | local A/B/C | validated A/B/C | total | validated-vs-FV | max region");

            for (var index = 0; index < study.Count; index++)
            {
                var point = study.GetPoint(index);
                var validatedCounts = point.HasValidatedBudget
                    ? $"{point.ValidatedCountA}/{point.ValidatedCountB}/{point.ValidatedCountC}"
                    : "none";
                var validatedTotal = point.HasValidatedBudget
                    ? point.ValidatedTotalCount.ToString()
                    : "-";
                var validatedFv = point.HasValidatedBudget
                    ? point.ValidatedFiniteVolumeError.ToString("E8")
                    : "n/a";
                var maxRegion = point.HasValidatedBudget
                    ? point.ValidatedMaximumRegionError.ToString("E8")
                    : "n/a";

                Console.WriteLine(
                    $"{point.Time,4:F2} | {point.ReducedStateVsFiniteVolumeError:E8} | "
                    + $"{point.LocalCountA}/{point.LocalCountB}/{point.LocalCountC} | "
                    + $"{validatedCounts} | {validatedTotal} | {validatedFv} | {maxRegion}");
            }

            Console.WriteLine($"Validated total range: {study.MinimumValidatedTotal}..{study.MaximumValidatedTotal}");
            Console.WriteLine($"Allocation changed over time: {study.AllocationChanged}");
            Console.WriteLine(passed ? "PASS" : "FAIL");

            return passed;
        }

        private static bool RunNegligibleRegionGaussianRuleCheckpoint()
        {
            var study = NegligibleRegionGaussianRuleStudy1D.Evaluate();
            const double maximumGlobalRepresentationError = 5e-3;
            var passed = study.Satisfies(maximumGlobalRepresentationError);

            Console.WriteLine();
            Console.WriteLine("Gaussian Thermal Field — Negligible Region / Zero-Gaussian Rule");
            Console.WriteLine(
                $"Omission guards: global L2 <= {study.L2OmissionThreshold:E2}, "
                + $"peak/global peak <= {study.PeakOmissionThreshold:E2}");
            Console.WriteLine("time | L2 A/B/C | peak A/B/C | counts A/B/C | total | global vs state");

            for (var index = 0; index < study.Count; index++)
            {
                var point = study.GetPoint(index);
                Console.WriteLine(
                    $"{point.Time,4:F2} | "
                    + $"{point.L2ContributionA:E3}/{point.L2ContributionB:E3}/{point.L2ContributionC:E3} | "
                    + $"{point.PeakContributionA:E3}/{point.PeakContributionB:E3}/{point.PeakContributionC:E3} | "
                    + $"{point.CountA}/{point.CountB}/{point.CountC} | "
                    + $"{point.TotalCount} | {point.GlobalRepresentationError:E8}");
            }

            Console.WriteLine(passed ? "PASS" : "FAIL");
            return passed;
        }
    }
}
