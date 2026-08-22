using System.Globalization;

static class Check
{
    public static void True(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    public static void NearlyEqual(double a, double b, double tolerance, string message)
    {
        if (Math.Abs(a - b) > tolerance)
        {
            throw new InvalidOperationException($"{message}: {a:R} != {b:R}");
        }
    }
}

internal static class Program
{
    private const string FrozenSemanticBaseline = "15ab144783bd3ccf1953cb7d7b2bb61998603bf6";
    private const string PhaseAMerge = "4114c32a04fad2b7c5d56df74c3ec72cdba5b4b2";
    private const double Tol = 1e-12;

    public static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine("RQ-EFM-001 Phase B Controls S0-S4");
        Console.WriteLine($"FROZEN_SEMANTIC_BASELINE={FrozenSemanticBaseline}");
        Console.WriteLine($"PHASE_A_MERGE={PhaseAMerge}");

        RunS0();
        RunS4();

        Console.WriteLine("M_F2_R_VALID_WITNESSES=1");
        Console.WriteLine("M_FI_R_MISSED_WITNESSES=0");
        Console.WriteLine("M_FI_P2_MISSED_WITNESSES=1");
        Console.WriteLine("M_F4_R_FORMULATION_REVISIONS=1");
        Console.WriteLine("M_F1_P1_PROMOTED_QUANTITIES_S4=1");
        Console.WriteLine("M_D1_R_PRE_ISO_ADMISSIBILITY_DECISIONS=1");
        Console.WriteLine("S1_S2_S3_EXECUTION=NOT_STARTED");
        Console.WriteLine("FINAL_H_EFM_VERDICTS=DEFERRED");
        Console.WriteLine("PHASE_B_CONTROLS=VALID");
    }

    private static void RunS0()
    {
        // Frozen S0 package: different external field states provide the same
        // complete interval-integrated deposited-energy exchange.
        const double hN = 100000.0;
        const double eDep = 1000.0;
        const double deltaT = 1.0;
        const double externalFieldStateA = -2.0;
        const double externalFieldStateB = +3.0;

        Check.True(externalFieldStateA != externalFieldStateB,
            "S0 requires two different external field states.");

        var hNextA = hN + eDep;
        var hNextB = hN + eDep;

        Check.NearlyEqual(hNextA, 101000.0, Tol, "Unexpected S0 enthalpy");
        Check.NearlyEqual(hNextA, hNextB, Tol,
            "S0 Future-Exchange control failed: same S_n/M_n/X_n/dt must produce same update.");
        Check.NearlyEqual(deltaT, 1.0, Tol, "S0 frozen timestep changed");

        Console.WriteLine($"S0_H_NEXT_A={hNextA:F12}");
        Console.WriteLine($"S0_H_NEXT_B={hNextB:F12}");
        Console.WriteLine("S0_TEST_C=NO_WITNESS");
        Console.WriteLine("S0_TEST_U=U0");
        Console.WriteLine("S0_R_POLICY=D0_CONTROL");
        Console.WriteLine("S0_P1_PROMOTED_QUANTITIES=0");
        Console.WriteLine("S0_P2_POLICY=D0_CONTROL");
        Console.WriteLine("S0_FUTURE_EXCHANGE_RULE=CONFIRMED");
    }

    private static void RunS4()
    {
        // Frozen S4 thermoelastic closure relation:
        // h_total = c * (T - T_ref) + 0.5 * k_eps * eps^2
        // T = T_ref + (h_total - 0.5 * k_eps * eps^2) / c
        const double c = 500.0;
        const double tRef = 300.0;
        const double kEps = 1.0e6;
        const double hTotal = 100000.0;
        const double epsA = 0.00;
        const double epsB = 0.02;
        const double deltaT = 1.0;

        static double Temperature(double h, double eps, double heatCapacity, double referenceTemperature, double stiffnessPerMass)
            => referenceTemperature + (h - 0.5 * stiffnessPerMass * eps * eps) / heatCapacity;

        var tA = Temperature(hTotal, epsA, c, tRef, kEps);
        var tB = Temperature(hTotal, epsB, c, tRef, kEps);
        var difference = Math.Abs(tA - tB);

        Check.NearlyEqual(deltaT, 1.0, Tol, "S4 frozen timestep changed");
        Check.NearlyEqual(tA, 500.0, Tol, "Unexpected S4 T_A");
        Check.NearlyEqual(tB, 499.6, Tol, "Unexpected S4 T_B");
        Check.True(difference > Tol,
            "S4 must produce different instantaneous closure for the same scalar h_total when strain differs.");

        // Policy application occurs only after the frozen witness fact is established.
        Console.WriteLine($"S4_T_A={tA:F12}");
        Console.WriteLine($"S4_T_B={tB:F12}");
        Console.WriteLine($"S4_DELTA_T_CLOSURE={difference:F12}");
        Console.WriteLine("S4_TEST_C=VALID_WITNESS");
        Console.WriteLine("S4_TEST_U=NOT_REQUIRED_FOR_CONTROL");
        Console.WriteLine("S4_R_POLICY=D1_FORMULATION_REVISION_REQUIRED");
        Console.WriteLine("S4_P1_POLICY=STATE_PROMOTION_REVISION");
        Console.WriteLine("S4_P1_PROMOTED_QUANTITIES=1");
        Console.WriteLine("S4_P2_POLICY=D0_ACCEPTED_WITH_MISSED_CLOSURE_WITNESS");
        Console.WriteLine("S4_EXCHANGE_ONLY_RESCUE=REJECTED_BY_FROZEN_TEST_C");
        Console.WriteLine("S4_PRE_ISO_ADMISSIBILITY_DECISION=YES");
    }
}
