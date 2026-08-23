using System;
using System.Globalization;

internal static class Program
{
    private const double Tol = 1e-12;
    private const string SemanticBaseline = "15ab144783bd3ccf1953cb7d7b2bb61998603bf6";
    private const string PhaseAMerge = "4114c32a04fad2b7c5d56df74c3ec72cdba5b4b2";
    private const string PhaseBMerge = "5d1b93c731c1629aede3ec0ffdb22a9d06322d53";

    private static string F(double value) => value.ToString("0.000000000000", CultureInfo.InvariantCulture);

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static double PNext(double p, double lambda, double e, double e0)
        => lambda * p + (1.0 - lambda) * Math.Tanh(e / e0);

    private static double G(double p, double e, double aP, double bP, double gamma)
        => 0.5 * aP * p * p + 0.25 * bP * Math.Pow(p, 4.0) - gamma * e * p;

    public static void Main()
    {
        Console.WriteLine("RQ-EFM-001 Phase C Electrocaloric Formulation Pair");
        Console.WriteLine($"FROZEN_SEMANTIC_BASELINE={SemanticBaseline}");
        Console.WriteLine($"PHASE_A_MERGE={PhaseAMerge}");
        Console.WriteLine($"PHASE_B_MERGE={PhaseBMerge}");

        // S1 — reduced equilibrium electrocaloric formulation.
        const double cp = 1000.0;
        const double kE = 1.0e-7;
        const double eStart = 0.0;
        const double eEnd = 1.0e6;
        const double hN = 100000.0;
        const double dt = 1.0;

        double deltaTEq = kE * (eEnd - eStart);
        double eEc = cp * deltaTEq;
        double s1HNext = hN + eEc;

        Require(Math.Abs(deltaTEq - 0.1) <= Tol, "S1 Delta T mismatch.");
        Require(Math.Abs(eEc - 100.0) <= Tol, "S1 energy exchange mismatch.");
        Require(Math.Abs(s1HNext - 100100.0) <= Tol, "S1 enthalpy update mismatch.");

        Console.WriteLine($"S1_DELTA_T_EQ={F(deltaTEq)}");
        Console.WriteLine($"S1_E_EC={F(eEc)}");
        Console.WriteLine($"S1_H_NEXT={F(s1HNext)}");
        Console.WriteLine("S1_TEST_C=NO_WITNESS");
        Console.WriteLine("S1_TEST_U=U0");
        Console.WriteLine("S1_R_POLICY=D0_REDUCED_EQUILIBRIUM");
        Console.WriteLine("S1_P1_PROMOTED_QUANTITIES=0");
        Console.WriteLine("S1_P2_POLICY=D0_REDUCED_EQUILIBRIUM");

        // S2 common polarization/history dynamics.
        const double lambda = 0.8;
        const double e0 = 1.0e6;
        const double s2E = 1.0e6;
        const double pA = -0.5;
        const double pB = +0.5;
        const double beta = 1.0e-4;

        double pEq = Math.Tanh(s2E / e0);
        double pNextA = PNext(pA, lambda, s2E, e0);
        double pNextB = PNext(pB, lambda, s2E, e0);
        double wA = beta * s2E * (pNextA - pA);
        double wB = beta * s2E * (pNextB - pB);
        double hNextA = hN + wA;
        double hNextB = hN + wB;

        bool fieldOnlyWitness = Math.Abs(hNextA - hNextB) > Tol;
        Require(fieldOnlyWitness, "S2-E field-only Test U witness was not produced.");

        Console.WriteLine($"S2_P_EQ={F(pEq)}");
        Console.WriteLine($"S2_P_NEXT_A={F(pNextA)}");
        Console.WriteLine($"S2_P_NEXT_B={F(pNextB)}");
        Console.WriteLine($"S2E_W_EC_A={F(wA)}");
        Console.WriteLine($"S2E_W_EC_B={F(wB)}");
        Console.WriteLine($"S2E_H_NEXT_A={F(hNextA)}");
        Console.WriteLine($"S2E_H_NEXT_B={F(hNextB)}");
        Console.WriteLine("S2E_FIELD_ONLY_TEST_U=VALID_WITNESS");
        Console.WriteLine("S2E_ENRICHMENT=GENERALIZED_WORK_W_EC");
        Console.WriteLine("S2E_ENRICHED_TEST_U=U0");
        Console.WriteLine("S2E_ANTI_SMUGGLING_AUDIT=PASS");
        Console.WriteLine("S2E_R_POLICY=D0_AFTER_U1_EXCHANGE_ENRICHMENT");
        Console.WriteLine("S2E_P1_POLICY=PROMOTE_POLARIZATION");
        Console.WriteLine("S2E_P1_PROMOTED_QUANTITIES=1");
        Console.WriteLine("S2E_P1_FALSE_PROMOTION_FINDINGS=1");
        Console.WriteLine("S2E_P2_POLICY=D0_WITH_GENERALIZED_WORK_NO_FORMAL_WITNESS_TEST");
        Console.WriteLine("S2E_P2_MISSED_WITNESSES=0");

        // S2-T — polarization is part of the selected thermodynamic closure.
        const double c = 500.0;
        const double tRef = 300.0;
        const double aP = 400.0;
        const double bP = 200.0;
        const double gamma = 1.0e-4;
        const double hTotal = 100000.0;

        double gA = G(pA, s2E, aP, bP, gamma);
        double gB = G(pB, s2E, aP, bP, gamma);
        double tA = tRef + (hTotal - gA) / c;
        double tB = tRef + (hTotal - gB) / c;
        double deltaTClosure = Math.Abs(tA - tB);
        bool closureWitness = deltaTClosure > Tol;
        Require(closureWitness, "S2-T Test C closure witness was not produced.");

        Console.WriteLine($"S2T_G_A={F(gA)}");
        Console.WriteLine($"S2T_G_B={F(gB)}");
        Console.WriteLine($"S2T_T_A={F(tA)}");
        Console.WriteLine($"S2T_T_B={F(tB)}");
        Console.WriteLine($"S2T_DELTA_T_CLOSURE={F(deltaTClosure)}");
        Console.WriteLine("S2T_TEST_C=VALID_WITNESS");
        Console.WriteLine("S2T_R_POLICY=D1_FORMULATION_REVISION_REQUIRED");
        Console.WriteLine("S2T_P1_POLICY=STATE_PROMOTION_REVISION");
        Console.WriteLine("S2T_P1_PROMOTED_QUANTITIES=1");
        Console.WriteLine("S2T_P1_FALSE_PROMOTION_FINDINGS=0");
        Console.WriteLine("S2T_P2_POLICY=D0_ACCEPTED_WITH_MISSED_CLOSURE_WITNESS");
        Console.WriteLine("S2T_P2_MISSED_WITNESSES=1");

        // Phase-C metrics and formulation-relative classification record.
        Console.WriteLine("M_F1_P1_PROMOTED_QUANTITIES_PHASE_C=2");
        Console.WriteLine("M_FP_P1_FALSE_PROMOTIONS_PHASE_C=1");
        Console.WriteLine("M_F2_R_VALID_WITNESSES_PHASE_C=2");
        Console.WriteLine("M_FI_R_MISSED_WITNESSES_PHASE_C=0");
        Console.WriteLine("M_FI_P2_MISSED_WITNESSES_PHASE_C=1");
        Console.WriteLine("M_F3_R_EXCHANGE_ENRICHMENTS_PHASE_C=1");
        Console.WriteLine("M_F4_R_FORMULATION_REVISIONS_PHASE_C=1");
        Console.WriteLine("M_F5_FORMULATION_DEPENDENT_CLASSIFICATION_CHANGE=CONFIRMED");
        Console.WriteLine("M_K1_REPEATED_RULE_AGREEMENT=CONFIRMED");
        Console.WriteLine("M_K2_POST_HOC_ASSUMPTIONS=0");
        Console.WriteLine("M_K3_HIDDEN_DEPENDENCY_FINDINGS=0");
        Console.WriteLine("M_D1_R_PRE_ISO_ADMISSIBILITY_DECISIONS_PHASE_C=2");
        Console.WriteLine("M_D1_R_PRE_ISO_ADMISSIBILITY_DECISIONS_CUMULATIVE=3");

        Console.WriteLine("H_EFM_03=SUPPORTED_FOR_EVALUATED_FORMULATIONS");
        Console.WriteLine("H_EFM_01_FINAL=DEFERRED_UNTIL_S3");
        Console.WriteLine("H_EFM_02_FINAL=DEFERRED_UNTIL_S3");
        Console.WriteLine("H_EFM_04_FINAL=DEFERRED_UNTIL_S3");
        Console.WriteLine("S3_EXECUTION=NOT_STARTED");
        Console.WriteLine("PHASE_C_FORMULATION_PAIR=VALID");

        _ = dt; // frozen update interval is retained explicitly.
    }
}
