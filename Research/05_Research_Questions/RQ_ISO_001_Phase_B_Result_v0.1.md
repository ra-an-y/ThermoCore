# RQ-ISO-001 Phase B Result v0.1

Status: COMPLETED — S0 equivalent; S1 negative control neutral  
Research Question: RQ-ISO-001  
Date: 2026-08-23  
Tracking: GitHub Issue #73  
Protocol: `RQ_ISO_001_Consequence_Test_Plan_v0.1.md`  
Frozen Phase A baseline: `8e3a948b0f36feefd313de1f03dd4db29b3bc465`

---

## 1. Purpose

This record reports Phase B of the pre-registered RQ-ISO-001 consequence test.

Phase B evaluates only:

- S0 — equivalent baseline capability under Condition R and Condition P; and
- S1 — the Derived Representation Consumer negative control.

It does not test S2, S3, or S4 and does not produce support for H-ISO-01, H-ISO-02, or H-ISO-03.

This record is non-normative and does not modify Framework Specifications, Framework Conformance, existing Verification or Validation conclusions, or the ThermoCore v1.0.0 publication baseline.

---

## 2. Executed Harness

The research-only executable harness is located under:

`Research/05_Research_Questions/Execution/RQ_ISO_001_Phase_B/`

It compiles against the existing Framework Core, Runtime, and Material Definition implementation without modifying those production artifacts.

Condition R and Condition P use the same bounded thermodynamic behavior for S0. For S1, both conditions pass recovered Derived Thermodynamic State to the same stateless representation consumer.

The consumer derives only transient presentation-oriented information:

- recovered Temperature;
- recovered liquid phase fraction; and
- a three-level derived phase-band representation.

No persistent consumer-local state is introduced.

---

## 3. Execution Evidence

GitHub Actions workflow:

`RQ-ISO-001 Phase B`

Successful source-head commit:

`f099919da5679b084baeca0230ed41104e5ec0e4`

Successful workflow run:

`32587548646` — run #2 — `success`

Observed executable output:

```text
RQ-ISO-001 Phase B S0-S1
Frozen comparison baseline: 8e3a948b0f36feefd313de1f03dd4db29b3bc465
S0_EQUIVALENCE=CONFIRMED
S1_NEGATIVE_CONTROL=NEUTRAL
CORE_PERSISTENT_QUANTITIES_R=1
CORE_PERSISTENT_QUANTITIES_P=1
CORE_SEMANTIC_BYTES_R=8
CORE_SEMANTIC_BYTES_P=8
S1_CONSUMER_PERSISTENT_FIELDS_R=0
S1_CONSUMER_PERSISTENT_FIELDS_P=0
HYPOTHESIS_SUPPORT_FROM_S0_S1=NONE
```

An earlier run (`32587480945`, run #1) failed during build because the research harness project initially omitted the existing `Materials/Definitions/*.cs` source dependency required by `ReferenceMaterialCompiler.cs`. The project include was corrected without changing Framework, scenario, metric, or decision-rule semantics. That failed build is retained as execution history and is not treated as a scenario result.

---

## 4. S0 Baseline Equivalence

Three persistent-state samples spanning solid, latent, and liquid recovery regions were evaluated under both conditions.

For every sample:

- recovered Temperature was identical;
- recovered liquid phase fraction was identical; and
- the same frozen Persistent Thermodynamic State schema was used.

S0 conclusion:

```text
EQUIVALENT BASELINE CAPABILITY — CONFIRMED
```

No hypothesis receives support from S0 alone.

---

## 5. S1 Negative Control

S1 adds only a pure Derived Representation Consumer.

The consumer has zero instance fields and therefore introduces no consumer-local persistent state in the executed harness.

For every evaluated state sample, Condition R and Condition P produced identical representation output.

Neither condition required promotion of presentation information into mandatory Core State.

S1 conclusion:

```text
NEGATIVE CONTROL — NEUTRAL
```

This is the expected valid negative-control outcome. A forced difference would have invalidated the comparison design.

---

## 6. State Metrics

Semantic payload counts use the Phase A definitions and exclude object headers, allocator overhead, alignment, and container capacity.

| Metric | S0 R | S0 P | S1 R | S1 P |
|---|---:|---:|---:|---:|
| `M-S1` mandatory persistent Core-State quantities | 1 | 1 | 1 | 1 |
| `M-S2` mandatory Core-State payload bytes / element | 8 | 8 | 8 | 8 |
| `M-S3` extension/consumer-specific persistent quantities promoted into Core | 0 | 0 | 0 | 0 |
| `M-S4` extension/consumer-local persistent payload bytes / element | 0 | 0 | 0 | 0 |
| `M-S5` total persistent semantic payload bytes / element | 8 | 8 | 8 | 8 |

Interpretation:

S1 creates no state-growth distinction. This is expected and provides no support for H-ISO-01.

---

## 7. Core-change Metrics

The Phase B branch adds only research harness, workflow, and result artifacts. No frozen Core semantic, implementation, or interface artifact is modified.

| Metric | S0 R | S0 P | S1 R | S1 P |
|---|---:|---:|---:|---:|
| `M-C1` Core normative requirements changed | 0 | 0 | 0 | 0 |
| `M-C2` Core semantic artifacts changed | 0 | 0 | 0 | 0 |
| `M-C3` Core implementation artifacts changed | 0 | 0 | 0 | 0 |
| `M-C4` Core interface contracts/signatures changed | 0 | 0 | 0 | 0 |
| `M-C5` extension-specific branches added inside frozen Core | 0 | 0 | 0 | 0 |
| `M-C6` undeclared direct Core-to-consumer dependency edges added | 0 | 0 | 0 | 0 |

Interpretation:

S1 creates no Core-change distinction. This is expected and provides no support for H-ISO-02.

---

## 8. Hidden-coupling / Complexity-displacement Audit

The S1 audit found no hidden Core coupling.

Specifically:

- no consumer-specific type checks were added to frozen Core code;
- no consumer-name-specific branches were added to frozen Core code;
- no frozen Core artifact imports or depends on the research consumer;
- no consumer field is hidden inside a generic Core container;
- no generic Core interface acquires consumer-specific semantics;
- no duplicate authoritative Thermodynamic State is created;
- no synchronization obligation is introduced; and
- no Core adapter requires modification for the S1 consumer.

The harness depends on the existing Framework implementation. The dependency direction is research consumer -> Framework, not Framework -> research consumer.

Audit conclusion:

```text
NO HIDDEN CORE COUPLING IDENTIFIED FOR S1
```

---

## 9. Evidence-impact Metrics

For Phase B counting, the frozen evidence units are treated as three named Core evidence records from Phase A:

1. bounded reference-formulation Verification suite;
2. H2O caloric Validation record; and
3. Gallium caloric Validation record.

Because S0/S1 modify no frozen semantic or executable dependency, all three remain applicable without re-execution for the existing Core claims. The Phase B research harness is separate research evidence, not a replacement for those records.

| Metric | S0 R | S0 P | S1 R | S1 P |
|---|---:|---:|---:|---:|
| `M-E1` Core requirements requiring impact review | 0 | 0 | 0 | 0 |
| `M-E2` Core Verification cases requiring re-execution | 0 | 0 | 0 | 0 |
| `M-E3` Core Validation records requiring re-execution | 0 | 0 | 0 | 0 |
| `M-E4` frozen Core evidence records retained without re-execution | 3 | 3 | 3 | 3 |
| `M-E5` new extension-specific evidence records required | 0 | 0 | 0 | 0 |

Interpretation:

S1 creates no revalidation-scope distinction and provides no support for H-ISO-03.

---

## 10. Hypothesis Status After Phase B

| Hypothesis | Status after S0-S1 | Reason |
|---|---|---|
| H-ISO-01 State-growth Isolation | UNTESTED BY DISCRIMINATING SCENARIO | S1 correctly creates no persistent-state difference |
| H-ISO-02 Core-change Isolation | UNTESTED BY DISCRIMINATING SCENARIO | S1 correctly creates no Core-change difference |
| H-ISO-03 Revalidation-scope Isolation | UNTESTED BY DISCRIMINATING SCENARIO | S1 correctly creates no evidence-impact difference |

No hypothesis-support claim is made from Phase B.

---

## 11. Phase B Validity Decision

The pre-registered Phase B validity conditions are satisfied:

- S0 begins from equivalent thermodynamic behavior;
- both conditions retain one mandatory persistent `SpecificEnthalpy` quantity / 8 semantic bytes per element;
- S1 adds no persistent state;
- S1 does not change frozen Core semantics, implementation, or interfaces;
- S1 has no identified hidden Core coupling; and
- S1 does not manufacture a difference between the two architecture conditions.

Decision:

```text
PHASE B VALID — PROCEED TO DISCRIMINATING S2/S3 SCENARIOS
```

This decision validates the experimental control structure only. It does not validate the RQ-ISO-001 candidate contribution.

---

## 12. Current Classification

| Item | Classification |
|---|---|
| S0 baseline equivalence | Confirmed |
| S1 negative-control neutrality | Confirmed |
| Hidden coupling in S1 | Not identified |
| H-ISO-01 | No support claim; awaiting S2/S3 |
| H-ISO-02 | No support claim; awaiting S2/S3 |
| H-ISO-03 | No support claim; awaiting S2/S3 |
| Phase B experimental validity | Supported |
| Novelty | Not established |
| Framework Specification change | None |
| Existing Validation / Conformance status | Unchanged |

---

## 13. Conclusion

RQ-ISO-001 Phase B produced the intended neutral control result.

Condition R and Condition P remain equivalent at S0, and a stateless Derived Representation Consumer in S1 does not create mandatory Core-State growth, Core modification, hidden coupling, or different Core evidence impact in either condition.

The absence of a difference is the correct negative-control result and shall not be converted into hypothesis support.

The experiment may now proceed to S2 and S3, where the pre-registered architecture policies intentionally diverge in treatment of extension-specific persistent state.
