using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    /// <summary>
    /// Immutable experiment-local material/configuration view for the bounded
    /// one-dimensional diffusion experiment.
    ///
    /// This type is Configuration, not evolving physical state.
    /// </summary>
    public readonly struct ThermalMaterial1D
    {
        public ThermalMaterial1D(
            double thermalConductivity,
            double volumetricHeatCapacity)
        {
            if (double.IsNaN(thermalConductivity)
                || double.IsInfinity(thermalConductivity)
                || thermalConductivity <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(thermalConductivity),
                    "Thermal conductivity must be finite and positive.");
            }

            if (double.IsNaN(volumetricHeatCapacity)
                || double.IsInfinity(volumetricHeatCapacity)
                || volumetricHeatCapacity <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(volumetricHeatCapacity),
                    "Volumetric heat capacity must be finite and positive.");
            }

            ThermalConductivity = thermalConductivity;
            VolumetricHeatCapacity = volumetricHeatCapacity;
        }

        public double ThermalConductivity { get; }

        public double VolumetricHeatCapacity { get; }

        public double ThermalDiffusivity =>
            ThermalConductivity / VolumetricHeatCapacity;

        public double ThermalEffusivity =>
            Math.Sqrt(ThermalConductivity * VolumetricHeatCapacity);
    }

    /// <summary>
    /// Piecewise Gaussian representation of the bounded perfect-contact
    /// two-half-space heat-kernel solution.
    /// </summary>
    public readonly struct PerfectInterfaceSolution1D
    {
        public PerfectInterfaceSolution1D(
            double interfacePosition,
            bool incidentFromLeft,
            GaussianKernel1D incident,
            GaussianKernel1D reflectedImage,
            GaussianKernel1D transmitted)
        {
            InterfacePosition = interfacePosition;
            IncidentFromLeft = incidentFromLeft;
            Incident = incident;
            ReflectedImage = reflectedImage;
            Transmitted = transmitted;
        }

        public double InterfacePosition { get; }

        public bool IncidentFromLeft { get; }

        public GaussianKernel1D Incident { get; }

        public GaussianKernel1D ReflectedImage { get; }

        public GaussianKernel1D Transmitted { get; }

        public double Evaluate(double x)
        {
            var isIncidentSide = IncidentFromLeft
                ? x < InterfacePosition
                : x > InterfacePosition;

            return isIncidentSide
                ? Incident.Evaluate(x) + ReflectedImage.Evaluate(x)
                : Transmitted.Evaluate(x);
        }
    }

    /// <summary>
    /// Experimental analytic propagation rule for an instantaneous source in
    /// one homogeneous half-space coupled by perfect thermal contact to a
    /// second homogeneous half-space.
    ///
    /// The implementation constructs incident, image/reflection, and
    /// transmitted Gaussian kernels. It does not mutate ThermoCore state.
    /// </summary>
    public static class PerfectInterfacePropagation1D
    {
        public static PerfectInterfaceSolution1D Create(
            double sourcePosition,
            double interfacePosition,
            double elapsedTime,
            double incidentAmplitude,
            in ThermalMaterial1D incidentMaterial,
            in ThermalMaterial1D transmittedMaterial)
        {
            if (double.IsNaN(sourcePosition) || double.IsInfinity(sourcePosition))
            {
                throw new ArgumentOutOfRangeException(nameof(sourcePosition));
            }

            if (double.IsNaN(interfacePosition) || double.IsInfinity(interfacePosition))
            {
                throw new ArgumentOutOfRangeException(nameof(interfacePosition));
            }

            if (sourcePosition == interfacePosition)
            {
                throw new ArgumentException(
                    "The bounded experiment requires the source to lie strictly inside one material half-space.",
                    nameof(sourcePosition));
            }

            if (double.IsNaN(elapsedTime)
                || double.IsInfinity(elapsedTime)
                || elapsedTime <= 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedTime),
                    "Elapsed time must be finite and positive.");
            }

            if (double.IsNaN(incidentAmplitude) || double.IsInfinity(incidentAmplitude))
            {
                throw new ArgumentOutOfRangeException(nameof(incidentAmplitude));
            }

            var alphaIncident = incidentMaterial.ThermalDiffusivity;
            var alphaTransmitted = transmittedMaterial.ThermalDiffusivity;
            var effusivityIncident = incidentMaterial.ThermalEffusivity;
            var effusivityTransmitted = transmittedMaterial.ThermalEffusivity;

            var reflectionCoefficient =
                (effusivityIncident - effusivityTransmitted)
                / (effusivityIncident + effusivityTransmitted);

            var diffusivityScale = Math.Sqrt(alphaTransmitted / alphaIncident);
            var transmissionCoefficient =
                (1.0 + reflectionCoefficient) * diffusivityScale;

            var incidentVariance = 2.0 * alphaIncident * elapsedTime;
            var transmittedVariance = 2.0 * alphaTransmitted * elapsedTime;

            var sourceOffset = sourcePosition - interfacePosition;
            var imageMean = interfacePosition - sourceOffset;
            var transmittedMean =
                interfacePosition + sourceOffset * diffusivityScale;

            var incident = new GaussianKernel1D(
                sourcePosition,
                incidentVariance,
                incidentAmplitude);

            var reflectedImage = new GaussianKernel1D(
                imageMean,
                incidentVariance,
                incidentAmplitude * reflectionCoefficient);

            var transmitted = new GaussianKernel1D(
                transmittedMean,
                transmittedVariance,
                incidentAmplitude * transmissionCoefficient);

            return new PerfectInterfaceSolution1D(
                interfacePosition,
                sourcePosition < interfacePosition,
                incident,
                reflectedImage,
                transmitted);
        }
    }
}
