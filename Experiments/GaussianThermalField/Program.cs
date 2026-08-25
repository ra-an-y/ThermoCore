using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    internal static class Program
    {
        private static int Main()
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

            // For the bounded impulse experiment, the incident normalized
            // temperature-kernel amplitude is E / C_A.
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

            return passed ? 0 : 1;
        }
    }
}
