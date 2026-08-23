# RQ-EFM-001 Phase C Result v0.1

Status: **COMPLETED — matched electrocaloric formulation pair executed**  
Research Question: **RQ-EFM-001 — External Energy / Physical Field–Driven Material Response**  
Date: **2026-08-23**  
Tracking: GitHub Issue #100  
Protocol: `RQ_EFM_001_Consequence_Test_Plan_v0.1.md`  
Frozen semantic baseline: `15ab144783bd3ccf1953cb7d7b2bb61998603bf6`  
Phase A freeze merge: `4114c32a04fad2b7c5d56df74c3ec72cdba5b4b2`  
Phase B merge: `5d1b93c731c1629aede3ec0ffdb22a9d06322d53`

---

## 1. Purpose

This record reports Phase C of the pre-registered RQ-EFM-001 consequence/classification evaluation.

Phase C executes the matched electrocaloric formulation family frozen in Phase A:

- `S1` — reduced/equilibrium electrocaloric formulation;
- `S2-E` — stateful polarization mechanism with mechanism-owned persistent state and generalized-work exchange; and
- `S2-T` — stateful polarization formulation in which polarization participates directly in thermodynamic closure.

The purpose is to determine whether the same mechanism family receives different admissibility outcomes because its frozen formulation/state requirements differ rather than because of mechanism naming.

This record is research-only and non-normative. It does not modify Framework Specification, production implementation, Validation, Performance, Framework Conformance, v1.0.0, or RQ-ISO-001.

---

## 2. Executed Harness

The isolated executable harness is located under:

`Research/05_Research_Questions/Execution/RQ_EFM_001_Phase_C/`

The harness contains only the Phase A equations and deterministic parameters. It does not modify or substitute for the production ThermoCore formulation.

GitHub Actions workflow:

`RQ-EFM-001 Phase C`

Successful initial workflow run:

`32599138214` — run #1 — `success`

Source head:

`f49416224e3d96740dd6506a09b6673d531c0f43`

---

## 3. S1 — Reduced Equilibrium Electrocaloric Formulation

Frozen relation:

```text
Delta T_eq = k_E * (E_(n+1) - E_n)
e_EC = c_p * Delta T_eq
h_(n+1) = h_n + e_EC
```

Observed deterministic values:

```text
Delta T_eq = 0.1 K
e_EC = 100 J/kg
h_(n+1) = 100100 J/kg
```

Witness results:

```text
Test C = NO WITNESS
Test U = U0
```

Policy record:

```text
R  = D0_REDUCED_EQUILIBRIUM
P1 = 0 promoted persistent quantities
P2 = D0_REDUCED_EQUILIBRIUM
```

Interpretation:

The selected S1 formulation contains no persistent polarization/history coordinate. The complete reduced caloric-energy exchange is sufficient for the frozen thermal update, and no policy difference is forced by the scenario.

S1 is therefore a valid reduced-formulation D0 member of the matched mechanism family.

---

## 4. S2 Common Polarization Dynamics

Frozen history rule:

```text
P_(n+1) = lambda * P_n + (1-lambda) * tanh(E_n/E_0)
```

Observed values:

```text
P_eq       = 0.761594155956
P_next,A   = -0.247681168809
P_next,B   =  0.552318831191
```

The two histories use the same thermal-side `h_n`, material information, electric-field input, and timestep but differ in persistent `P_n`.

---

## 5. S2-E — Mechanism-Owned Polarization with Generalized-Work Exchange

Frozen generalized-work relation:

```text
w_EC = beta * E_n * (P_(n+1)-P_n)
h_(n+1) = h_n + w_EC
```

Observed values:

```text
w_EC,A     = 25.231883119115 J/kg
w_EC,B     =  5.231883119115 J/kg
h_next,A   = 100025.231883119 J/kg
h_next,B   = 100005.231883119 J/kg
```

### 5.1 Minimal field-only exchange

With identical `h_n`, material information, electric-field input, and timestep but different omitted `P_n`, the physically required thermal update differs.

Result:

```text
S2-E field-only Test U = VALID WITNESS
```

Therefore the field-only exchange contract is insufficient.

### 5.2 Exchange enrichment

The contract is enriched with the physically defined interval generalized work `w_EC`.

Result:

```text
Enriched Test U = U0
Anti-smuggling audit = PASS
```

The thermal transition after enrichment depends on `h_n` and the supplied physical work exchange rather than on serialized `P` state. The producing responsibility for `P` remains external.

Policy record:

```text
R  = D0_AFTER_U1_EXCHANGE_ENRICHMENT
P1 = PROMOTE_POLARIZATION
P2 = D0_WITH_GENERALIZED_WORK_NO_FORMAL_WITNESS_TEST
```

P1 promotion record:

```text
promoted quantities = 1
false-promotion findings = 1
```

The P1 promotion is counted as false for S2-E because the frozen formulation demonstrates complete thermodynamic evolution after a semantically honest generalized-work exchange without promoting `P` into mandatory Thermodynamic State.

P2 is not assigned a missed witness in the final enriched architecture because the complete generalized-work interface is naturally present. The distinction is that R explicitly detects insufficiency of the minimal contract before accepting D0.

---

## 6. S2-T — Polarization as Thermodynamic Closure Coordinate

Frozen stored-energy relation:

```text
g(P,E) = 0.5*a_P*P^2 + 0.25*b_P*P^4 - gamma*E*P
T = T_ref + (h_total-g(P,E))/c
```

Observed values:

```text
g(P_A,E) = 103.125 J/kg
g(P_B,E) =   3.125 J/kg
T_A      = 499.79375 K
T_B      = 499.99375 K
Delta T  =   0.20000 K
```

The two admissible full states have the same claimed scalar `h_total`, material constants, and electric field but different `P` and therefore require different instantaneous temperature closure.

Result:

```text
Test C = VALID WITNESS
```

Policy record:

```text
R  = D1_FORMULATION_REVISION_REQUIRED
P1 = STATE_PROMOTION_REVISION
P2 = D0_ACCEPTED_WITH_MISSED_CLOSURE_WITNESS
```

The P1 promotion is **not** counted as false in S2-T because the selected formulation itself requires polarization in the thermodynamic closure description.

P2 misses one valid closure witness by permitting externalization without first establishing closure sufficiency.

---

## 7. Phase C Metrics

| Metric | Phase C result |
|---|---:|
| `M-F1` P1 promoted quantities | 2 |
| `M-FP` P1 false-promotion findings | 1 |
| `M-F2` R valid insufficiency witnesses | 2 |
| `M-FI` R missed witnesses | 0 |
| `M-FI` P2 missed witnesses | 1 |
| `M-F3` R required exchange enrichments | 1 |
| `M-F4` R formulation/Core revisions | 1 |
| `M-F5` same-mechanism formulation-dependent classification change | CONFIRMED |
| `M-K1` repeated rule agreement | CONFIRMED |
| `M-K2` post-hoc assumptions | 0 |
| `M-K3` hidden dependency findings | 0 |
| `M-D1` R pre-RQ-ISO admissibility decisions, Phase C | 2 |
| cumulative `M-D1` after Phase B + C | 3 |

No composite score is derived.

---

## 8. Formulation-Relative Classification Result

The same electrocaloric mechanism family produced three different architecture outcomes under the same frozen rule:

| Frozen formulation | R outcome | Cause |
|---|---|---|
| S1 reduced/equilibrium | `D0 / U0` | no persistent polarization/history coordinate; reduced energy exchange sufficient |
| S2-E mechanism-owned state | `D0 after U1` | field-only exchange insufficient; generalized-work enrichment restores update sufficiency without state promotion |
| S2-T thermodynamic closure state | `D1` | polarization changes instantaneous thermodynamic closure at the same claimed scalar energy state |

The decision input is the selected formulation/state role, not the mechanism name.

---

## 9. H-EFM-03 Verdict

Under the pre-registered H-EFM-03 rule:

```text
H-EFM-03 — Formulation-Relative Classification Stability
SUPPORTED FOR EVALUATED FORMULATIONS
```

Reasons:

- S1/S2 use the same electrocaloric mechanism family;
- the same frozen classification rule is applied to all cases;
- the classification changes are traceable to explicit differences in state/closure/evolution requirements;
- no mechanism name is used as a decision input;
- repeated rule application agrees; and
- no post-hoc assumption or hidden dependency was required.

This is bounded to the evaluated formulations and does not establish a universal electrocaloric classification.

---

## 10. Status of Other Hypotheses

Phase C provides additional positive evidence for H-EFM-01, H-EFM-02, and H-EFM-04, but their final verdicts remain deferred until S3 completes the pre-registered cross-domain governing-coupling requirement.

```text
H-EFM-01 final = DEFERRED UNTIL S3
H-EFM-02 final = DEFERRED UNTIL S3
H-EFM-04 final = DEFERRED UNTIL S3
```

S3 has not been executed by this result record.

---

## 11. Scope and Claim Boundary

This result does not establish that:

- electrocaloric mechanisms universally belong to any one of S1, S2-E, or S2-T;
- the deterministic surrogate parameters are physically validated material data;
- all internal-variable formulations require Thermodynamic Core revision;
- all mechanism-owned internal state can be externalized through generalized work;
- RQ-EFM-001 is globally novel;
- ThermoCore v1.0.0 implements electrocaloric physics; or
- Framework Specification changes are yet justified.

The supported result is narrower: the frozen formulation-relative gate distinguished reduced, exchange-enriched, and closure-revision treatments inside one matched mechanism family without changing the classification rule.

---

## 12. Phase C Decision

```text
PHASE C FORMULATION PAIR — VALID
H-EFM-03 — SUPPORTED FOR EVALUATED FORMULATIONS
PROCEED TO PHASE D S3 THERMOELECTRIC CROSS-DOMAIN COUPLING
```

Overall RQ-EFM-001 contribution disposition remains deferred until S3 and the final pre-registered hypothesis evaluation are complete.
