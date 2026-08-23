# RQ-EFM-001 Phase D Result v0.1

Status: **COMPLETED — S3 valid; pre-registered H-EFM verdicts applied**  
Research Question: **RQ-EFM-001 — External Energy / Physical Field–Driven Material Response**  
Candidate Gap: **Formulation-Relative Thermodynamic Extension Admissibility Boundary**  
Date: **2026-08-23**  
Tracking: GitHub Issue #102  
Protocol: `RQ_EFM_001_Consequence_Test_Plan_v0.1.md`  
Frozen semantic baseline: `15ab144783bd3ccf1953cb7d7b2bb61998603bf6`

---

## 1. Purpose

This record reports Phase D of the pre-registered RQ-EFM-001 consequence/classification evaluation.

Phase D executes only the frozen S3 thermoelectric cross-domain governing-coupling scenario and then, because S0-S4 are complete, applies the already pre-registered H-EFM-01 through H-EFM-04 decision rules to the full evaluated scenario set.

This record is non-normative. It does not modify:

- Framework Specification;
- production Core/Runtime implementation;
- Validation, Verification, Performance, or Framework Conformance;
- ThermoCore v1.0.0;
- the completed RQ-ISO-001 disposition; or
- novelty/priority status.

Final Research Gap / Research Contribution disposition remains a separate post-experiment task.

---

## 2. Frozen Dependencies

The executed research chain is fixed as:

```text
Protocol merge baseline:
15ab144783bd3ccf1953cb7d7b2bb61998603bf6

Phase A merge:
4114c32a04fad2b7c5d56df74c3ec72cdba5b4b2

Phase B merge:
5d1b93c731c1629aede3ec0ffdb22a9d06322d53

Phase C merge:
123e18d031f0fc15df8129dda69feb2d24d92c65
```

Phase A remains authoritative for the S3 equations, exchange semantics, deterministic parameters, Future-Exchange Rule, Test C/Test U, anti-smuggling rule, policies R/P1/P2, metrics, and decision rules.

No Phase D parameter or rule was changed after execution began.

---

## 3. Executed Harness

The research-only executable is located under:

`Research/05_Research_Questions/Execution/RQ_EFM_001_Phase_D/`

The harness does not modify or replace production ThermoCore computation.

It instantiates two distinct external electrical governing states and supplies the same complete thermal exchange packet for the frozen current update interval.

The external electrical-state values are abstract harness identifiers only. They are not calibrated device data and are not used as a physical Validation claim.

---

## 4. Frozen S3 Relation and Parameters

The thermal-side exchange packet is:

```text
X_n = { e_J, e_P, e_Th }

e_TE = e_J + e_P + e_Th
h_(n+1) = h_n + e_TE
```

Frozen values:

```text
h_n  = 100000 J/kg
e_J  = +80 J/kg
e_P  = -20 J/kg
e_Th = +5 J/kg
Delta t = 1 s
```

Therefore:

```text
e_TE = +65 J/kg
h_(n+1) = 100065 J/kg
```

Electrical potential/current remain separately governed cross-domain state in the frozen S3 formulation package.

---

## 5. Execution Evidence

GitHub Actions workflow:

`RQ-EFM-001 Phase D`

Successful source-head commit:

`53bf8d289c1d1062f8d8a2e5c1368c092014d996`

Successful workflow run:

`32618917384` — run #1 — `success`

Observed key output:

```text
S3_EXTERNAL_STATES_DIFFER=CONFIRMED
S3_E_J=80.000000000000
S3_E_P=-20.000000000000
S3_E_TH=5.000000000000
S3_E_TE=65.000000000000
S3_H_NEXT_A=100065.000000000000
S3_H_NEXT_B=100065.000000000000
S3_TEST_C=NO_WITNESS
S3_TEST_U=U0
S3_FUTURE_EXCHANGE_RULE=CONFIRMED
S3_R_POLICY=D0_CROSS_DOMAIN_GOVERNING_COUPLING
S3_R_HIDDEN_COUPLING_AUDIT=PASS
S3_P1_POLICY=PROMOTE_ELECTRICAL_GOVERNING_STATE
S3_P1_PROMOTED_QUANTITIES=2
S3_P1_FALSE_PROMOTION_FINDINGS=2
S3_P2_POLICY=D0_WITH_COMPLETE_THERMAL_EXCHANGE
S3_P2_MISSED_WITNESSES=0
S3_BIDIRECTIONAL_COUPLING_STATE_MERGER_RULE=REJECTED
M_D1_R_PRE_ISO_ADMISSIBILITY_DECISIONS_CUMULATIVE=4
H_EFM_01=SUPPORTED_FOR_EVALUATED_FORMULATIONS
H_EFM_02=SUPPORTED_FOR_EVALUATED_FORMULATIONS
H_EFM_03=SUPPORTED_FOR_EVALUATED_FORMULATIONS
H_EFM_04=SUPPORTED_FOR_EVALUATED_FORMULATIONS
PHASE_D_S3=VALID
```

No failed Phase D scenario run precedes this accepted result.

---

## 6. S3 Test C — Instantaneous Closure Sufficiency

The frozen S3 thermal formulation does not use electrical potential or current as instantaneous thermodynamic closure coordinates.

The two external electrical states therefore do not produce distinct required instantaneous thermal closure when authoritative thermal state and frozen thermal material information are held fixed.

Result:

```text
S3_TEST_C = NO_WITNESS
```

This is a formulation-specific result. It does not state that electrical variables can never be thermodynamic coordinates in another selected formulation.

---

## 7. S3 Test U — Update Sufficiency

The two external electrical states differ internally but supply the same complete current-interval exchange packet:

```text
{ +80, -20, +5 } J/kg
```

Both therefore require:

```text
h_(n+1) = 100065 J/kg
```

The external solver may evolve to different future electrical states, but those future values belong to a later exchange interval and are not part of the current `X_n` under the frozen Future-Exchange Rule.

Result:

```text
S3_TEST_U = U0
S3_FUTURE_EXCHANGE_RULE = CONFIRMED
```

No exchange enrichment is required for the frozen S3 package.

---

## 8. Policy R Result

Policy R applies the formulation-relative sufficiency gate after the witness facts are fixed.

For S3:

- instantaneous closure remains complete without electrical potential/current in Thermodynamic State;
- the complete thermal exchange packet determines the current interval thermal update;
- external electrical state remains separately governed;
- no future external-solver value is required prematurely; and
- no hidden thermodynamic dependency is identified.

R result:

```text
D0_CROSS_DOMAIN_GOVERNING_COUPLING
```

The result means only that the frozen S3 thermal formulation can preserve its current thermodynamic state identity under the declared complete exchange contract.

It does not claim that thermoelectric coupling is universally D0.

---

## 9. Policy P1 Result

P1 is the pre-registered participation-promotion control.

For the S3 controlled comparison it promotes the two explicitly named separately governed electrical quantities:

1. electrical potential;
2. electrical current.

Recorded result:

```text
S3_P1_PROMOTED_QUANTITIES = 2
```

Because S3 under the frozen formulation achieves complete thermal closure and update using the physically identified exchange packet without serializing those governing variables into Thermodynamic State, both promotions are counted as unnecessary **for this frozen thermal-state completeness question**.

Therefore:

```text
S3_P1_FALSE_PROMOTION_FINDINGS = 2
```

This does not say that potential/current are unnecessary to the electrical solver or the complete multiphysics system. They remain required electrical governing state; the finding concerns only mandatory promotion into authoritative Thermodynamic State.

---

## 10. Policy P2 Result

P2 permits cross-domain state to remain external whenever an exchange interface can be written, without R's explicit sufficiency gate.

For S3, the frozen exchange packet is genuinely complete for the current thermal interval, so P2 is not assigned a manufactured failure.

Result:

```text
S3_P2_POLICY = D0_WITH_COMPLETE_THERMAL_EXCHANGE
S3_P2_MISSED_WITNESSES = 0
```

This is an important fairness control: R is not credited merely because P2 lacks a formal witness test when the actual S3 exchange is sufficient.

---

## 11. Hidden-Coupling Audit

The R S3 result was audited for hidden relocation of governing information.

No equivalent hidden Core coupling was identified:

- no electrical potential/current value is serialized into an opaque thermal exchange payload;
- `e_J`, `e_P`, and `e_Th` are explicitly identified physical energy/heat contributions;
- no Core-side concrete electrical-solver type check is required;
- no Core branch depends on the name `thermoelectric`;
- no duplicate authoritative Thermodynamic State is created;
- the thermal responsibility does not evolve electrical potential/current;
- future electrical state is not assumed available before the external responsibility produces its later exchange;
- no scenario-specific synchronization obligation is reclassified as thermodynamic state ownership; and
- bidirectionality alone is not used as a state-merger rule.

Audit result:

```text
PASS
```

---

## 12. Phase D Metrics

| Metric | Phase D S3 result |
|---|---:|
| `M-F1` P1 promoted quantities | 2 |
| `M-FP` P1 false promotions | 2 |
| `M-F2` R valid insufficiency witnesses | 0 |
| `M-FI` R missed witnesses | 0 |
| `M-FI` P2 missed witnesses | 0 |
| `M-F3` R exchange enrichments | 0 |
| `M-F4` R formulation revisions | 0 |
| `M-K1` repeated-rule agreement | CONFIRMED |
| `M-K2` post-hoc assumptions | 0 |
| `M-K3` hidden dependency findings | 0 |
| `M-D1` pre-RQ-ISO admissibility decisions | 1 |

The S3 `M-D1` count reflects that the gate must decide whether strong separately governed electrical participation is physically admissible under the frozen thermal formulation before ordinary-extension authority/non-promotion semantics can simply be assumed.

---

## 13. Cumulative S0-S4 Metrics

Across the complete frozen evaluation:

| Metric | Cumulative result |
|---|---:|
| P1 promoted quantities recorded in discriminating/positive-boundary cases | 5 |
| P1 false promotions | 3 |
| R valid insufficiency witnesses | 3 |
| R missed insufficiency witnesses | 0 |
| P2 missed insufficiency witnesses | 2 |
| R exchange enrichments | 1 |
| R formulation/Core revisions | 2 |
| Pre-RQ-ISO admissibility decisions `M-D1` | 4 |

Interpretive mapping:

- required P1 promotions not counted false: S4 strain coordinate, S2-T polarization coordinate;
- false P1 promotions: S2-E polarization plus S3 electrical potential/current;
- R insufficiency witnesses: S4 Test C, S2-E minimal-contract Test U, S2-T Test C;
- R exchange enrichment: S2-E generalized work;
- R formulation revisions: S4 and S2-T;
- P2 missed witnesses: S4 and S2-T.

No composite score is derived.

---

## 14. Scenario Summary

| Scenario | R terminal result | Key finding |
|---|---|---|
| S0 external deposition | D0 / U0 | complete deposited-energy exchange is sufficient |
| S1 reduced electrocaloric | D0 / U0 | reduced equilibrium formulation needs no persistent polarization state |
| S2-E stateful external electrocaloric | D0 after U1 | field-only insufficient; physical generalized-work enrichment restores sufficiency |
| S2-T thermodynamic-polarization formulation | D1 | polarization is required by selected instantaneous thermodynamic closure |
| S3 thermoelectric cross-domain coupling | D0 / U0 | strong bidirectional governing coupling remains separable under complete thermal exchanges |
| S4 thermoelastic stored-energy formulation | D1 | strain-dependent stored energy makes scalar enthalpy-only closure insufficient |

This table is formulation-relative. It is not a mechanism taxonomy claiming universal D0/D1 status for entire physics domains.

---

## 15. H-EFM-01 — False-Promotion Avoidance

Pre-registered support requires:

- all valid D0 R cases pass Test C/Test U/hidden-coupling audit;
- at least one unnecessary P1 promotion is avoided; and
- R has zero missed insufficiency witnesses in those D0 cases.

Observed:

- S0, S1, S2-E after honest enrichment, and S3 remain valid D0 cases;
- R missed no insufficiency witness;
- S2-E demonstrates one unnecessary P1 polarization promotion;
- S3 demonstrates two unnecessary P1 electrical-state promotions for the frozen thermal completeness question; and
- hidden-coupling audits are clean for accepted D0 results.

Verdict:

```text
H-EFM-01
SUPPORTED FOR EVALUATED FORMULATIONS
```

This does not imply that fewer thermodynamic-state variables universally reduce total memory, total state, or system complexity.

---

## 16. H-EFM-02 — False-Isolation Detection

Pre-registered support requires the frozen insufficiency cases to produce valid witnesses and R to require enrichment, formulation/Core revision, or scope narrowing as appropriate, while not falsely rejecting D0 controls.

Observed:

- S4 produces a valid Test C witness and R requires D1 formulation revision;
- S2-E minimal field-only exchange produces a valid Test U witness and R requires U1 exchange enrichment;
- S2-T produces a valid Test C witness and R requires D1 formulation revision;
- S0 is not falsely rejected;
- S1 is not falsely rejected;
- S3 is not falsely rejected merely because future electrical state may differ; and
- R records zero missed insufficiency witnesses.

Verdict:

```text
H-EFM-02
SUPPORTED FOR EVALUATED FORMULATIONS
```

---

## 17. H-EFM-03 — Formulation-Relative Classification Stability

Phase C already satisfied the matched-formulation rule using one electrocaloric mechanism family:

- S1 reduced/equilibrium formulation -> D0/U0;
- S2-E explicit mechanism-owned polarization -> U1 then D0;
- S2-T polarization included in selected thermodynamic closure -> D1.

The classification differences are traceable to frozen state/closure/evolution facts, not mechanism naming.

S3 exposes no contradiction to that result.

Verdict retained:

```text
H-EFM-03
SUPPORTED FOR EVALUATED FORMULATIONS
```

---

## 18. H-EFM-04 — Distinctness from RQ-ISO-001

The pre-registered rule requires at least one valid physical/formulation admissibility or exchange-sufficiency decision before ordinary-extension authority/non-promotion can be meaningfully evaluated.

Observed cumulative:

```text
M-D1 = 4
```

The decisive cases include:

- S2-E: the gate first discovers that the minimal exchange contract is insufficient and requires physical exchange enrichment;
- S2-T: the gate determines that the selected thermodynamic formulation itself requires revision;
- S4: the gate determines that enthalpy-only instantaneous closure is incomplete under the selected thermoelastic formulation;
- S3: the gate establishes that strong cross-domain governing participation remains physically admissible without state merger under the complete frozen thermal exchange contract.

S2-E, S2-T, and S4 in particular change the allowed architecture outcome before a pure state-authority/non-promotion rule can simply be applied.

Verdict:

```text
H-EFM-04
SUPPORTED FOR EVALUATED FORMULATIONS
```

This result does not redefine RQ-ISO-001. It supports a bounded preceding admissibility role for RQ-EFM-001 in the evaluated formulations.

---

## 19. Final Experiment-Level Classification

The pre-registered S0-S4 experiment therefore records:

```text
H-EFM-01 = SUPPORTED FOR EVALUATED FORMULATIONS
H-EFM-02 = SUPPORTED FOR EVALUATED FORMULATIONS
H-EFM-03 = SUPPORTED FOR EVALUATED FORMULATIONS
H-EFM-04 = SUPPORTED FOR EVALUATED FORMULATIONS
```

All wording remains bounded to the executed formulations and frozen scenario scope.

The experiment supports the engineering usefulness and distinctness of the formulation-relative admissibility gate **within the evaluated set**.

It does not by itself establish a global research gap, novelty, priority, universal classification rule, or Framework normative change.

---

## 20. Claims Not Established

This result does **not** establish that:

- ThermoCore is the first framework to distinguish these coupling cases;
- the admissibility boundary is globally novel;
- all electrocaloric formulations classify like S1/S2;
- all thermoelectric formulations can remain D0;
- all thermoelastic formulations require D1;
- every external field can be reduced to an energy exchange;
- electrical potential/current are unnecessary to a coupled system;
- exchange-based architectures are universally superior;
- P1 or P2 represents any named prior framework;
- the synthetic witness parameters physically validate a material;
- current ThermoCore v1.0.0 implements these multiphysics mechanisms; or
- Framework Specification changes are automatically justified.

Novelty / priority remains:

```text
NOT ESTABLISHED
```

---

## 21. Phase D Decision

S3 reproduced the frozen expected exchange behavior, passed Test C/Test U and hidden-coupling checks, and completed the required pre-registered scenario set.

Phase D decision:

```text
S3 VALID
PRE-REGISTERED S0-S4 CONSEQUENCE TEST COMPLETE
PROCEED TO FINAL RESEARCH-GAP / CONTRIBUTION DISPOSITION
```

The next artifact shall synthesize the closed evidence survey, Research Gap Analysis, and S0-S4 experimental results without changing the completed experimental record.
