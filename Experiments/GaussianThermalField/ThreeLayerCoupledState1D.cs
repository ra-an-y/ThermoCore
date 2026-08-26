using System;

namespace ThermoCore.Experiments.GaussianThermalField
{
    public readonly struct ThreeLayerCoupledState1D
    {
        public ThreeLayerCoupledState1D(
            FiniteLayerReducedState1D stateA,
            FiniteLayerReducedState1D stateB,
            FiniteLayerReducedState1D stateC)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
        }

        public FiniteLayerReducedState1D StateA { get; }
        public FiniteLayerReducedState1D StateB { get; }
        public FiniteLayerReducedState1D StateC { get; }
    }

    public readonly struct ThreeLayerCoupledStepResult1D
    {
        public ThreeLayerCoupledStepResult1D(
            ThreeLayerCoupledState1D state,
            double fluxAB,
            double fluxBC,
            double interfaceJumpAB,
            double interfaceJumpBC)
        {
            State = state;
            FluxAB = fluxAB;
            FluxBC = fluxBC;
            InterfaceJumpAB = interfaceJumpAB;
            InterfaceJumpBC = interfaceJumpBC;
        }

        public ThreeLayerCoupledState1D State { get; }

        /// <summary>Positive means energy transfer A -> B.</summary>
        public double FluxAB { get; }

        /// <summary>Positive means energy transfer B -> C.</summary>
        public double FluxBC { get; }

        public double InterfaceJumpAB { get; }
        public double InterfaceJumpBC { get; }
    }

    /// <summary>
    /// Experiment-local A-B-C coupling for three finite homogeneous layers.
    ///
    /// Interface heat fluxes are not prescribed. For each timestep, the two
    /// unknown piecewise-constant interface fluxes are solved from end-of-step
    /// temperature continuity using the affine boundary response of each
    /// reduced layer state. Equal and opposite fluxes are then applied to the
    /// adjacent regions, preserving energy accounting without storing a
    /// reflection/transmission event history.
    /// </summary>
    public static class ThreeLayerCoupledEvolution1D
    {
        public static ThreeLayerCoupledStepResult1D Advance(
            in ThreeLayerCoupledState1D state,
            double deltaTime,
            double lengthA,
            double lengthB,
            double lengthC,
            in ThermalMaterial1D materialA,
            in ThermalMaterial1D materialB,
            in ThermalMaterial1D materialC)
        {
            if (double.IsNaN(deltaTime)
                || double.IsInfinity(deltaTime)
                || deltaTime <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            var responseA = BoundaryResponse.Create(
                state.StateA, deltaTime, lengthA, materialA);
            var responseB = BoundaryResponse.Create(
                state.StateB, deltaTime, lengthB, materialB);
            var responseC = BoundaryResponse.Create(
                state.StateC, deltaTime, lengthC, materialC);

            // qAB > 0 transfers energy A -> B.
            // qBC > 0 transfers energy B -> C.
            // External boundaries are insulated.
            //
            // T_A,R(qAB) - T_B,L(qAB,qBC) = 0
            // T_B,R(qAB,qBC) - T_C,L(qBC) = 0
            var c1 = responseA.FreeRight - responseB.FreeLeft;
            var c2 = responseB.FreeRight - responseC.FreeLeft;

            var a11 = -responseA.RightFluxToRight
                - responseB.LeftFluxToLeft;
            var a12 = responseB.RightFluxToLeft;
            var a21 = responseB.LeftFluxToRight;
            var a22 = -responseB.RightFluxToRight
                - responseC.LeftFluxToLeft;

            var determinant = a11 * a22 - a12 * a21;
            if (Math.Abs(determinant) <= 1e-18)
            {
                throw new InvalidOperationException(
                    "The bounded interface-response system is singular.");
            }

            var rhs1 = -c1;
            var rhs2 = -c2;

            var fluxAB = (rhs1 * a22 - a12 * rhs2) / determinant;
            var fluxBC = (a11 * rhs2 - rhs1 * a21) / determinant;

            var nextA = FiniteLayerStateEvolution1D.Advance(
                state.StateA,
                leftInwardHeatFlux: 0.0,
                rightInwardHeatFlux: -fluxAB,
                deltaTime,
                lengthA,
                materialA);

            var nextB = FiniteLayerStateEvolution1D.Advance(
                state.StateB,
                leftInwardHeatFlux: fluxAB,
                rightInwardHeatFlux: -fluxBC,
                deltaTime,
                lengthB,
                materialB);

            var nextC = FiniteLayerStateEvolution1D.Advance(
                state.StateC,
                leftInwardHeatFlux: fluxBC,
                rightInwardHeatFlux: 0.0,
                deltaTime,
                lengthC,
                materialC);

            var temperatureARight = FiniteLayerFieldRepresentation1D.Evaluate(
                nextA, lengthA, lengthA);
            var temperatureBLeft = FiniteLayerFieldRepresentation1D.Evaluate(
                nextB, 0.0, lengthB);
            var temperatureBRight = FiniteLayerFieldRepresentation1D.Evaluate(
                nextB, lengthB, lengthB);
            var temperatureCLeft = FiniteLayerFieldRepresentation1D.Evaluate(
                nextC, 0.0, lengthC);

            return new ThreeLayerCoupledStepResult1D(
                new ThreeLayerCoupledState1D(nextA, nextB, nextC),
                fluxAB,
                fluxBC,
                temperatureARight - temperatureBLeft,
                temperatureBRight - temperatureCLeft);
        }

        private readonly struct BoundaryResponse
        {
            private BoundaryResponse(
                double freeLeft,
                double freeRight,
                double leftFluxToLeft,
                double leftFluxToRight,
                double rightFluxToLeft,
                double rightFluxToRight)
            {
                FreeLeft = freeLeft;
                FreeRight = freeRight;
                LeftFluxToLeft = leftFluxToLeft;
                LeftFluxToRight = leftFluxToRight;
                RightFluxToLeft = rightFluxToLeft;
                RightFluxToRight = rightFluxToRight;
            }

            public double FreeLeft { get; }
            public double FreeRight { get; }
            public double LeftFluxToLeft { get; }
            public double LeftFluxToRight { get; }
            public double RightFluxToLeft { get; }
            public double RightFluxToRight { get; }

            public static BoundaryResponse Create(
                in FiniteLayerReducedState1D state,
                double deltaTime,
                double length,
                in ThermalMaterial1D material)
            {
                var free = FiniteLayerStateEvolution1D.Advance(
                    state, 0.0, 0.0, deltaTime, length, material);

                var zero = FiniteLayerReducedState1D.Zero(state.ModeCount);
                var unitLeft = FiniteLayerStateEvolution1D.Advance(
                    zero, 1.0, 0.0, deltaTime, length, material);
                var unitRight = FiniteLayerStateEvolution1D.Advance(
                    zero, 0.0, 1.0, deltaTime, length, material);

                return new BoundaryResponse(
                    FiniteLayerFieldRepresentation1D.Evaluate(free, 0.0, length),
                    FiniteLayerFieldRepresentation1D.Evaluate(free, length, length),
                    FiniteLayerFieldRepresentation1D.Evaluate(unitLeft, 0.0, length),
                    FiniteLayerFieldRepresentation1D.Evaluate(unitLeft, length, length),
                    FiniteLayerFieldRepresentation1D.Evaluate(unitRight, 0.0, length),
                    FiniteLayerFieldRepresentation1D.Evaluate(unitRight, length, length));
            }
        }
    }
}
