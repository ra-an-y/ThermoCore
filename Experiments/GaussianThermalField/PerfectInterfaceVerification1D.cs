using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct PerfectInterfaceVerificationResult
    {
        public PerfectInterfaceVerificationResult(
            double incidentSideTemperature,
            double transmittedSideTemperature,
            double incidentSideConductiveGradient,
            double transmittedSideConductiveGradient)
        {
            IncidentSideTemperature = incidentSideTemperature;
            TransmittedSideTemperature = transmittedSideTemperature;
            IncidentSideConductiveGradient = incidentSideConductiveGradient;
            TransmittedSideConductiveGradient = transmittedSideConductiveGradient;
        }

        public double IncidentSideTemperature { get; }

        public double TransmittedSideTemperature { get; }

        public double IncidentSideConductiveGradient { get; }

        public double TransmittedSideConductiveGradient { get; }

        public double TemperatureJump =>
            IncidentSideTemperature - TransmittedSideTemperature;

        public double ConductiveGradientJump =>
            IncidentSideConductiveGradient - TransmittedSideConductiveGradient;

        public bool Satisfies(double absoluteTolerance)
        {
            if (double.IsNaN(absoluteTolerance)
                || double.IsInfinity(absoluteTolerance)
                || absoluteTolerance < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(absoluteTolerance));
            }

            return Math.Abs(TemperatureJump) <= absoluteTolerance
                && Math.Abs(ConductiveGradientJump) <= absoluteTolerance;
        }
    }

    /// <summary>
    /// Branch-local diagnostics for the bounded analytic interface experiment.
    /// These checks verify mathematical interface continuity only; they are not
    /// ThermoCore Framework conformance claims.
    /// </summary>
    public static class PerfectInterfaceVerification1D
    {
        public static PerfectInterfaceVerificationResult Evaluate(
            in PerfectInterfaceSolution1D solution,
            in ThermalMaterial1D incidentMaterial,
            in ThermalMaterial1D transmittedMaterial)
        {
            var x = solution.InterfacePosition;

            var incidentTemperature =
                solution.Incident.Evaluate(x)
                + solution.ReflectedImage.Evaluate(x);

            var transmittedTemperature = solution.Transmitted.Evaluate(x);

            var incidentGradient =
                solution.Incident.EvaluateDerivative(x)
                + solution.ReflectedImage.EvaluateDerivative(x);

            var transmittedGradient = solution.Transmitted.EvaluateDerivative(x);

            var incidentConductiveGradient =
                incidentMaterial.ThermalConductivity * incidentGradient;

            var transmittedConductiveGradient =
                transmittedMaterial.ThermalConductivity * transmittedGradient;

            return new PerfectInterfaceVerificationResult(
                incidentTemperature,
                transmittedTemperature,
                incidentConductiveGradient,
                transmittedConductiveGradient);
        }
    }
}
