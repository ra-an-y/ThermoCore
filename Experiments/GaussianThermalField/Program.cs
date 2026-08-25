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

            var passed = perfectInterfacePassed
                && finiteLayerPassed
                && convergencePassed
                && independentReferencePassed;

            Console.WriteLine();
            Console.WriteLine(passed ? "OVERALL PASS" : "OVERALL FAIL");

            return passed ? 0 : 1;
        }

        private static bool RunPerfectInterfaceCheckpoint()
        {
            var materialA = new ThermalMaterial1D(
                thermalConductivity: 0.40,
                volumetricHeatCapacity: 2.0);

            var materialB = new ThermalMaterial1D(
                thermalConductivity: 0.06,
                volumetricHeatCapacity: 1.2);

            const double sourcePosition = -0.55;
            const double interfacePosition = 0.0;
            const double elapsedTime = 0.9;
            const double sourceEnergy = 1.0;

            var incidentAmplitude =
                sourceEnergy / materialA.VolumetricHeatCapacity;

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
    }
}
