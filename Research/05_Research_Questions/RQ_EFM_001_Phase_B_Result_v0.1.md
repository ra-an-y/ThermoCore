# RQ-EFM-001 Phase B Result v0.1

Status: **COMPLETED — S0 Future-Exchange control valid; S4 closure witness detected**  
Research Question: **RQ-EFM-001 — External Energy / Physical Field–Driven Material Response**  
Date: **2026-08-23**  
Tracking: GitHub Issue #98  
Protocol: `RQ_EFM_001_Consequence_Test_Plan_v0.1.md`  
Frozen semantic baseline: `15ab144783bd3ccf1953cb7d7b2bb61998603bf6`  
Frozen Phase A merge: `4114c32a04fad2b7c5d56df74c3ec72cdba5b4b2`

---

## 1. Purpose

This record reports Phase B of the pre-registered RQ-EFM-001 consequence/classification test.

Phase B executes only the two frozen controls:

- S0 — externally supplied Joule/optical deposition control; and
- S4 — thermoelastic strain-dependent instantaneous-closure control.

S1, S2, and S3 remain unexecuted. Therefore this record does not issue final verdicts for H-EFM-01 through H-EFM-04.

This record is non-normative and does not modify Framework Specification, production implementation, Validation, Performance, Framework Conformance, the v1.0.0 publication baseline, or the completed RQ-ISO-001 disposition.

---

## 2. Executed Harness

The research-only executable harness is located under:

`Research/05_Research_Questions/Execution/RQ_EFM_001_Phase_B/`

It is self-contained and does not modify or replace production ThermoCore code.

The harness uses exactly the deterministic S0 and S4 relations and parameters frozen in Phase A. Policy R/P1/P2 outputs are produced only after the frozen witness facts are computed.

---

## 3. Execution Evidence

GitHub Actions workflow:

`RQ-EFM-001 Phase B`

Successful source-head commit:

`ccc98e9e65b38379e2700b2438478ced3bc61639`

Successful workflow run:

`32598499285` — run #1 — `success`

Observed executable output:

```text
RQ-EFM-001 Phase B Controls S0-S4
FROZEN_SEMANTIC_BASELINE=15ab144783bd3ccf1953cb7d7b2bb61998603bf6
PHASE_A_MERGE=4114c32a04fad2b7c5d56df74c3ec72cdba5b4b2
S0_H_NEXT_A=101000.000000000000
S0_H_NEXT_B=101000.000000000000
S0_TEST_C=NO_WITNESS
S0_TEST_U=U0
S0_R_POLICY=D0_CONTROL
S0_P1_PROMOTED_QUANTITIES=0
S0_P2_POLICY=D0_CONTROL
S0_FUTURE_EXCHANGE_RULE=CONFIRMED
S4_T_A=500.000000000000
S4_T_B=499.600000000000
S4_DELTA_T_CLOSURE=0.400000000000
S4_TEST_C=VALID_WITNESS
S4_TEST_U=NOT_REQUIRED_FOR_CONTROL
S4_R_POLICY=D1_FORMULATION_REVISION_REQUIRED
S4_P1_POLICY=STATE_PROMOTION_REVISION
S4_P1_PROMOTED_QUANTITIES=1
S4_P2_POLICY=D0_ACCEPTED_WITH_MISSED_CLOSURE_WITNESS
S4_EXCHANGE_ONLY_RESCUE=REJECTED_BY_FROZEN_TEST_C
S4_PRE_ISO_ADMISSIBILITY_DECISION=YES
M_F2_R_VALID_WITNESSES=1
M_FI_R_MISSED_WITNESSES=0
M_FI_P2_MISSED_WITNESSES=1
M_F4_R_FORMULATION_REVISIONS=1
M_F1_P1_PROMOTED_QUANTITIES_S4=1
M_D1_R_PRE_ISO_ADMISSIBILITY_DECISIONS=1
S1_S2_S3_EXECUTION=NOT_STARTED
FINAL_H_EFM_VERDICTS=DEFERRED
PHASE_B_CONTROLS=VALID
```

The workflow completed without scenario, parameter, protocol, or production-code correction.

---

## 4. S0 — Future-Exchange / Source-Deposition Control

The frozen S0 update is:

```text
h_(n+1) = h_n + e_dep
```

with:

```text
h_n = 100000 J/kg
e_dep = 1000 J/kg
Delta t = 1 s
```

Two different external field states were assigned the same complete interval exchange packet. Both produced:

```text
h_(n+1) = 101000 J/kg
```

No instantaneous closure witness exists in the frozen reduced S0 formulation, and Test U returned `U0`.

The important control result is that R did **not** reject S0 merely because the external solver may have different future state. The Future-Exchange Rule therefore behaved as pre-registered.

S0 control conclusion:

```text
VALID D0 CONTROL — FUTURE-EXCHANGE RULE CONFIRMED
```

This is a control result, not support for a full hypothesis by itself.

---

## 5. S4 — Thermoelastic Instantaneous-Closure Control

The frozen S4 relation is:

```text
h_total = c * (T - T_ref) + 0.5 * k_eps * eps^2
T = T_ref + (h_total - 0.5 * k_eps * eps^2) / c
```

with:

```text
c = 500 J/(kg K)
T_ref = 300 K
k_eps = 1.0e6 J/kg
h_total = 100000 J/kg
eps_A = 0.00
eps_B = 0.02
```

The two frozen full states share the same scalar `h_total` and material parameters but yield:

```text
T_A = 500.0 K
T_B = 499.6 K
Delta T = 0.4 K
```

Therefore the frozen scalar enthalpy-like state does not uniquely determine instantaneous thermodynamic closure for this selected thermoelastic formulation.

This is a valid Test C witness under the pre-registered rule.

S4 witness conclusion:

```text
VALID INSTANTANEOUS-CLOSURE INSUFFICIENCY WITNESS
```

This conclusion is bounded to the explicitly frozen S4 formulation. It does not imply that every thermoelastic model must use the same thermodynamic-state choice.

---

## 6. Policy Results after Witness Construction

The policies were applied to the same frozen S4 witness facts only after Test C was established.

| Policy | S0 | S4 |
|---|---|---|
| R — formulation-relative sufficiency gate | `D0 / U0` | `D1 — formulation revision required` |
| P1 — participation-promotion control | no promotion required in frozen S0 thermal contract | promotes `eps`-dependent closure information into revised shared state |
| P2 — exchange-only permissive control | accepts D0 control | accepts externalization but misses the frozen Test C closure insufficiency |

For S4, an interval mechanical-work exchange cannot rescue the existing scalar state from Test C because the insufficiency is instantaneous closure, not merely missing interval energy transfer.

P1 does not count as a false promotion in S4: the frozen selected formulation actually requires strain-dependent closure information. P1's response is a state/formulation revision rather than the omission detected under P2.

---

## 7. Phase-B Metrics

Only metrics meaningful for the executed controls are recorded.

| Metric | R | P1 | P2 | Interpretation |
|---|---:|---:|---:|---|
| `M-F2` valid insufficiency witnesses detected | 1 | n/a | 0 before acceptance | S4 Test C is the one frozen witness |
| `M-FI` missed insufficiency witnesses | 0 | 0 | 1 | P2 accepts S4 without the required closure test |
| `M-F4` explicit formulation/Core revisions | 1 | 1 response path | 0 | R and P1 both acknowledge S4 cannot remain the original scalar-state formulation |
| `M-F1` promoted mandatory quantities in S4 | 0 as a placement metric; R records D1 | 1 | 0 | P1 explicitly promotes the missing coordinate; R does not reduce D1 to a placement decision |
| `M-D1` pre-RQ-ISO admissibility decisions | 1 | n/a | n/a | S4 requires a formulation admissibility decision before ordinary-extension state-placement rules are meaningful |

`M-FP`, `M-F3`, `M-F5`, and the full consistency/hidden-dependency metrics require later S1-S3 execution for their intended discriminating use and are not given final values here.

---

## 8. Hidden-Coupling Review for Controls

### S0

No hidden dependency was required:

- external field state is not serialized into the thermal exchange;
- only the frozen interval deposited energy crosses the boundary;
- future external values are not assumed available in the current update; and
- no Core-side mechanism type check is present.

### S4

The research harness does not hide strain inside a neutral energy packet. The Test C calculation uses the frozen thermoelastic closure relation directly and exposes the strain dependence before any policy decision.

Therefore the positive S4 witness is not produced by opaque state relocation.

---

## 9. Distinctness Signal Relative to RQ-ISO-001

S4 produces one recorded pre-RQ-ISO admissibility decision:

```text
M-D1 = 1
```

The reason is physical/formulation sufficiency rather than state placement alone. Before asking whether `eps` may remain extension-owned under the RQ-ISO-001 non-promotion rule, the selected S4 formulation must first decide whether the original enthalpy-only thermodynamic state/closure is admissible at all.

This is **positive control evidence** for the distinctness hypothesis, but H-EFM-04 remains formally deferred until the full pre-registered scenario set is executed.

---

## 10. Hypothesis Status after Phase B

| Hypothesis | Phase-B status | Reason |
|---|---|---|
| H-EFM-01 — False-Promotion Avoidance | **DEFERRED** | S0 is neutral for P1; discriminating D0 cases S1/S3 remain unexecuted |
| H-EFM-02 — False-Isolation Detection | **POSITIVE CONTROL EVIDENCE; FINAL VERDICT DEFERRED** | R detects the frozen S4 Test C witness and does not falsely reject S0 |
| H-EFM-03 — Formulation-Relative Classification Stability | **UNTESTED** | matched S1/S2 pair not executed |
| H-EFM-04 — Distinctness from RQ-ISO-001 | **POSITIVE CONTROL EVIDENCE; FINAL VERDICT DEFERRED** | S4 records `M-D1 = 1`, but S2/S3 remain unexecuted |

No full `SUPPORTED FOR EVALUATED FORMULATIONS` verdict is issued at Phase B.

---

## 11. Phase B Validity Decision

The two control cases behaved consistently with their frozen research roles:

- S0 verifies that the update-relative exchange rule does not demand future external-state prediction;
- S4 verifies that Test C can detect an instantaneous thermodynamic closure insufficiency before state-placement policy is applied.

Phase B decision:

```text
PHASE B CONTROLS VALID — PROCEED TO PHASE C S1/S2 FORMULATION PAIR
```

No scenario redesign is required from the control results.

---

## 12. Claims Not Supported by Phase B

Phase B does not establish that:

- the RQ-EFM admissibility boundary is novel;
- H-EFM-01 through H-EFM-04 have final positive verdicts;
- every thermoelastic formulation requires D1;
- every external source case is D0;
- P1 or P2 represents an existing named framework;
- current ThermoCore production code supports thermoelastic, electrocaloric, or thermoelectric multiphysics; or
- the deterministic witness parameters constitute physical Validation.

---

## 13. Next Stage

Phase C shall execute the matched electrocaloric formulation pair already frozen in Phase A:

- S1 — reduced/equilibrium electrocaloric formulation;
- S2-E — stateful electrocaloric model with external mechanism-owned polarization and generalized-work exchange;
- S2-T — stateful electrocaloric model in which polarization participates directly in selected thermodynamic closure.

This is the first phase that can directly test H-EFM-03 and exchange-enrichment behavior for the same mechanism family.
