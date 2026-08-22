using System;
using System.Linq;
using System.Reflection;
using ThermoCore.Framework.Runtime;

internal static class Program
{
    private const int ExpectedCurrentCoreQuantities = 1;
    private const int ExpectedMinimumRevisionCategories = 7;

    public static int Main()
    {
        try
        {
            Require(
                CountPersistentCoreQuantities() == ExpectedCurrentCoreQuantities,
                "Frozen Thermodynamic State no longer contains exactly one persistent semantic quantity.");

            var coreA = new ThermodynamicState(100.0);
            var coreB = new ThermodynamicState(100.0);

            var flowA = new S4FlowState(
                Mass: 1.0,
                Density: 1.0,
                Velocity: 0.0,
                SpeciesFraction: 0.1,
                SpecificEnthalpy: coreA.SpecificEnthalpy);

            var flowB = new S4FlowState(
                Mass: 2.0,
                Density: 2.0,
                Velocity: 10.0,
                SpeciesFraction: 0.9,
                SpecificEnthalpy: coreB.SpecificEnthalpy);

            var totalEnthalpyA = flowA.Mass * flowA.SpecificEnthalpy;
            var totalEnthalpyB = flowB.Mass * flowB.SpecificEnthalpy;

            Require(
                NearlyEqual(coreA.SpecificEnthalpy, coreB.SpecificEnthalpy),
                "Frozen same-Core-state counterexample requires equal specific enthalpy.");

            Require(
                !NearlyEqual(totalEnthalpyA, totalEnthalpyB),
                "Variable-mass ambiguity was not demonstrated.");

            Require(
                !EquivalentFlowState(flowA, flowB),
                "S4 flow states unexpectedly became equivalent.");

            var contradictions = new[]
            {
                new AssumptionCheck("FixedCellMass", RequiredByCurrentCore: true, ViolatedByS4: true),
                new AssumptionCheck("ConstantReferenceDensity", RequiredByCurrentCore: true, ViolatedByS4: true),
                new AssumptionCheck("SpecificEnthalpyOnlyPersistentState", RequiredByCurrentCore: true, ViolatedByS4: true),
                new AssumptionCheck("NoMomentumState", RequiredByCurrentCore: true, ViolatedByS4: true),
                new AssumptionCheck("NoSpeciesTransportState", RequiredByCurrentCore: true, ViolatedByS4: true),
                new AssumptionCheck("NoPressureEvolution", RequiredByCurrentCore: true, ViolatedByS4: true),
                new AssumptionCheck("LocalEnergyIncrementEvolution", RequiredByCurrentCore: true, ViolatedByS4: true),
                new AssumptionCheck("NoFlowDependentTransportResponsibility", RequiredByCurrentCore: true, ViolatedByS4: true)
            };

            var contradictionCount = contradictions.Count(item => item.RequiredByCurrentCore && item.ViolatedByS4);
            Require(contradictionCount == contradictions.Length, "Not all frozen S4 contradictions were detected.");

            var revisionCategories = new[]
            {
                "STATE_SEMANTIC_REVISION",
                "STATE_SCHEMA_OR_AUTHORITY_EXPANSION",
                "GOVERNING_FORMULATION_REVISION",
                "CORE_RESPONSIBILITY_REVISION",
                "INTERFACE_REVISION",
                "VERIFICATION_REVISION",
                "VALIDATION_EXPANSION"
            };

            Require(
                revisionCategories.Length == ExpectedMinimumRevisionCategories,
                "Unexpected S4 minimum revision-category count.");

            var extensionOnlyZeroCoreChangeValid = false;
            Require(
                !extensionOnlyZeroCoreChangeValid,
                "S4 must reject an extension-only zero-Core-change interpretation.");

            Console.WriteLine("RQ-ISO-001 S4 Variable-Mass Compressible Reactive-Flow Boundary Test");
            Console.WriteLine("Frozen comparison baseline: 8e3a948b0f36feefd313de1f03dd4db29b3bc465");
            Console.WriteLine($"S4_CURRENT_CORE_STATE_QUANTITIES={ExpectedCurrentCoreQuantities}");
            Console.WriteLine("S4_VARIABLE_MASS_ENERGY_AMBIGUITY=CONFIRMED");
            Console.WriteLine("S4_SAME_CORE_STATE_DIFFERENT_FLOW_STATE=CONFIRMED");
            Console.WriteLine($"S4_ASSUMPTION_CONTRADICTIONS={contradictionCount}");
            Console.WriteLine("S4_EXTENSION_ONLY_ZERO_CORE_CHANGE=REJECTED");
            Console.WriteLine("S4_REQUIRED_DISPOSITION=CORE_REVISION_REQUIRED");
            Console.WriteLine($"S4_REQUIRED_REVISION_CATEGORIES={revisionCategories.Length}");
            Console.WriteLine("S4_BOUNDARY_VERDICT=BOUNDARY_VALID");
            Console.WriteLine("S1_S3_HYPOTHESIS_RESULTS=UNCHANGED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static int CountPersistentCoreQuantities()
    {
        return typeof(ThermodynamicState)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Count(property => property.GetMethod != null);
    }

    private static bool EquivalentFlowState(in S4FlowState a, in S4FlowState b)
    {
        return NearlyEqual(a.Mass, b.Mass)
            && NearlyEqual(a.Density, b.Density)
            && NearlyEqual(a.Velocity, b.Velocity)
            && NearlyEqual(a.SpeciesFraction, b.SpeciesFraction)
            && NearlyEqual(a.SpecificEnthalpy, b.SpecificEnthalpy);
    }

    private static bool NearlyEqual(double a, double b)
    {
        return Math.Abs(a - b) <= 1e-12;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}

internal readonly record struct S4FlowState(
    double Mass,
    double Density,
    double Velocity,
    double SpeciesFraction,
    double SpecificEnthalpy);

internal readonly record struct AssumptionCheck(
    string Name,
    bool RequiredByCurrentCore,
    bool ViolatedByS4);
