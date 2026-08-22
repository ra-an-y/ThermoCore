# RQ-ISO-001 S2 Result v0.1

Status: COMPLETED — functional equivalence confirmed; discriminating S2 result recorded  
Research Question: RQ-ISO-001  
Scenario: S2 — Thermal Hysteresis Material-Response Extension  
Tracking: GitHub Issue #75  
Protocol: `RQ_ISO_001_Consequence_Test_Plan_v0.1.md`  
Scenario freeze: `RQ_ISO_001_S2_Scenario_Freeze_v0.1.md`  
Frozen comparison baseline: `8e3a948b0f36feefd313de1f03dd4db29b3bc465`

---

## 1. Purpose

This record reports the S2 thermal-hysteresis scenario of the pre-registered RQ-ISO-001 consequence test.

S2 is the first discriminating ordinary stateful-extension case after the valid S0/S1 control stage.

The result is bounded to the frozen S2 material-response mechanism. It does not claim that hysteresis universally belongs outside Thermodynamic State, does not modify the ThermoCore Framework Specification, and does not establish a final H-ISO-01, H-ISO-02, or H-ISO-03 verdict by itself.

---

## 2. Frozen Scenario Used

The pre-execution freeze defined exactly one persistent history quantity:

```text
HysteresisMode : byte

0 = SolidLike
1 = LiquidLike
```

The same hysteresis rule was used in both conditions:

```text
T_low  = 295 K
T_high = 305 K

if mode == SolidLike and T >= T_high:
    mode := LiquidLike
else if mode == LiquidLike and T <= T_low:
    mode := SolidLike
else:
    retain mode
```

The frozen input-temperature sequence was:

```text
294, 299, 304, 306, 302, 297, 294, 299 K
```

with expected history sequence:

```text
SolidLike,
SolidLike,
SolidLike,
LiquidLike,
LiquidLike,
LiquidLike,
SolidLike,
SolidLike
```

No threshold, payload, state-placement, or expected-sequence rule was changed after execution began.

---

## 3. Architecture Conditions Executed

### Condition R

Condition R retained the frozen Core state:

```text
Thermodynamic State:
- SpecificEnthalpy : double
```

and stored:

```text
HysteresisMode : byte
```

as extension-owned persistent state.

The extension consumed recovered Temperature and updated only its own history quantity.

### Condition P

Condition P remained modular but represented the same history quantity as part of the shared authoritative Simulation/Core State schema:

```text
Shared state:
- SpecificEnthalpy : double
- HysteresisMode   : byte
```

Hysteresis computation remained in the same separate response module. The controlled difference was state authority / membership placement, not whether the computation was modular.

---

## 4. Execution Evidence

Research-only executable harness:

`Research/05_Research_Questions/Execution/RQ_ISO_001_S2/`

GitHub Actions workflow:

`RQ-ISO-001 S2`

Successful source-head commit:

`ac1ff7db9f0206d46991c9b152dbd0006ce1d6cd`

Successful workflow run:

`32588480566` — run #1 — `success`

Observed executable output:

```text
RQ-ISO-001 S2 Thermal Hysteresis
Frozen comparison baseline: 8e3a948b0f36feefd313de1f03dd4db29b3bc465
S2_FUNCTIONAL_EQUIVALENCE=CONFIRMED
S2_HISTORY_QUANTITY=HysteresisMode(byte)
S2_EXPECTED_SEQUENCE=CONFIRMED
R_CORE_PERSISTENT_QUANTITIES=1
R_CORE_SEMANTIC_BYTES=8
R_PROMOTED_EXTENSION_QUANTITIES=0
R_EXTENSION_LOCAL_BYTES=1
R_TOTAL_PERSISTENT_BYTES=9
P_CORE_PERSISTENT_QUANTITIES=2
P_CORE_SEMANTIC_BYTES=9
P_PROMOTED_EXTENSION_QUANTITIES=1
P_EXTENSION_LOCAL_BYTES=0
P_TOTAL_PERSISTENT_BYTES=9
S2_CROSS_SCENARIO_HYPOTHESIS_VERDICT=NOT_FINAL
```

---

## 5. Functional Equivalence

Both conditions used the same reference thermodynamic recovery path and the same frozen hysteresis-response module.

For every input in the frozen sequence:

- recovered Temperature matched between R and P;
- the resulting `HysteresisMode` matched between R and P; and
- the observed mode matched the predeclared expected sequence.

Result:

```text
S2 FUNCTIONAL EQUIVALENCE — CONFIRMED
```

No persistent information was omitted from Condition R and no extra persistent quantity was added to Condition P beyond the one frozen S2 history quantity.

---

## 6. State Metrics — H-ISO-01 Contribution

Primary values use semantic payload size as pre-registered. Runtime padding, object/container overhead, and backend packing are excluded.

| Metric | Condition R | Condition P |
|---|---:|---:|
| `M-S1` mandatory persistent Core-State quantities | 1 | 2 |
| `M-S2` mandatory Core-State semantic bytes / element | 8 | 9 |
| `M-S3` extension-specific persistent quantities promoted into Core | 0 | 1 |
| `M-S4` extension-local persistent semantic bytes / element | 1 | 0 |
| `M-S5` total persistent semantic bytes / element | 9 | 9 |

Interpretation:

Condition R reduced **Core-State promotion**, not total persistent information.

The total semantic payload is identical at 9 bytes per element. Therefore S2 does **not** support a claim of total-memory reduction.

S2 satisfies the discriminating direction required by the H-ISO-01 decision rule: R has fewer extension-specific quantities promoted into mandatory Core State than P while preserving equivalent functionality and identical total persistent semantic payload.

This is only the S2 contribution; the final H-ISO-01 classification still depends on S3.

---

## 7. Core-change Metrics — H-ISO-02 Contribution

The repository implementation of both conditions is intentionally research-only. The counts below represent the architectural Core impacts required by the frozen R/P policies relative to their S0 baseline; the research harness files themselves are not counted as production ThermoCore Core changes.

### Condition R

No frozen Core semantic, implementation, or generic interface artifact needs S2-specific modification.

### Condition P

Exercising the already-frozen permissive policy does not change the policy itself, so no new normative architecture rule is counted. However, the active shared authoritative state contract must grow to include the S2-specific history quantity.

The logical P-S2 Core impacts are:

- one shared-state semantic schema change;
- one shared-state implementation schema change;
- one shared-state access/interface contract expansion exposing `HysteresisMode`; and
- one direct shared-state dependency on the S2-specific semantic quantity.

| Metric | Condition R | Condition P |
|---|---:|---:|
| `M-C1` Core normative requirements changed | 0 | 0 |
| `M-C2` Core semantic artifacts changed | 0 | 1 |
| `M-C3` Core implementation artifacts changed | 0 | 1 |
| `M-C4` Core interface contracts/signatures changed | 0 | 1 |
| `M-C5` extension-specific branches inside frozen Core implementation | 0 | 0 |
| `M-C6` direct Core-to-extension-specific dependency edges | 0 | 1 |

S2 therefore produces a strict-subset Core-impact result for Condition R relative to Condition P without making Condition P monolithic.

This is only the S2 contribution; final H-ISO-02 classification still depends on S3 and the S4 boundary-validity check.

---

## 8. Hidden-coupling / Complexity-displacement Audit

The Condition R implementation was inspected against the pre-registered hidden-coupling safeguards.

No S2-specific coupling was found in frozen ThermoCore Core artifacts:

- no S2-specific type checks in Core code;
- no hysteresis-name-specific branches or switch cases in Core code;
- no frozen Core import/dependency on the S2 response module;
- no `HysteresisMode` field hidden inside a generic Core container;
- no generic Framework Interface given hysteresis-specific semantics;
- no duplicate authoritative copy of Thermodynamic State;
- no Core adapter modified for the S2 extension; and
- no scenario-specific synchronization mechanism introduced in the bounded sequential harness.

The dependency direction remains:

```text
S2 extension -> recovered Core information
```

rather than:

```text
Core -> concrete S2 extension
```

Audit result:

```text
NO EQUIVALENT HIDDEN CORE COUPLING IDENTIFIED FOR S2
```

This does not establish that all possible stateful extensions will have the same result.

---

## 9. Evidence-impact Metrics — H-ISO-03 Contribution

The Phase A dependency rules were applied identically to both conditions.

The frozen Core evidence set used for the bounded count is:

1. Reference Verification suite;
2. H2O caloric Validation record; and
3. Gallium caloric Validation record.

### Condition R

S2 is extension-only under the frozen restricted boundary:

- no frozen Core semantic dependency changes;
- no frozen Core implementation dependency changes;
- no generic Core interface changes; and
- the H2O/Gallium thermodynamic recovery paths are unchanged.

Classification:

- Reference Verification: `Retained`
- H2O caloric Validation: `Retained`
- Gallium caloric Validation: `Retained`
- S2 hysteresis behavior: `NewExtensionEvidence`

### Condition P

The shared persistent-state schema and its access contract change relative to P-S0.

Under Phase A D2, the persistent-state schema change requires Core Verification impact review and re-execution of the applicable state/schema verification layer.

The H2O and Gallium caloric recovery/formulation paths remain numerically unchanged, so they are not classified `Revalidate`. Because the shared authoritative state membership changed, their applicability is conservatively classified `ReviewOnly` rather than silently treated as untouched semantic context.

Classification:

- Reference Verification: `Reverify`
- H2O caloric Validation: `ReviewOnly`
- Gallium caloric Validation: `ReviewOnly`
- S2 hysteresis behavior: `NewExtensionEvidence`

Primary counts:

| Metric | Condition R | Condition P |
|---|---:|---:|
| `M-E1` Core requirement/schema items requiring impact review | 0 | 1 |
| `M-E2` Core Verification records requiring re-execution | 0 | 1 |
| `M-E3` Core Validation records requiring re-execution | 0 | 0 |
| `M-E4` Core evidence records retained without re-execution | 3 | 2 |
| `M-E5` new extension-specific evidence records required | 1 | 1 |

S2 therefore produces a strict-subset Core **re-execution** impact for Condition R: the R set is empty while the P set contains the Core state/schema Verification layer.

The physical Validation re-execution sets are equal and empty for S2 because the caloric formulation/recovery dependency did not change.

This is only the S2 contribution; final H-ISO-03 classification still depends on S3.

---

## 10. Repository-change Boundary

The S2 branch changes only:

- S2 research scenario definition;
- S2 research-only executable harness;
- S2 workflow; and
- this S2 result record.

No Framework Specification or production `Framework/Core` / `Framework/Runtime` source is modified by the experiment.

This repository fact is separate from the logical Condition P Core-impact counts above. The comparator's shared-state growth is represented in the research harness so the experiment can measure the controlled architectural consequence without rewriting the published ThermoCore production implementation.

---

## 11. S2 Interpretation

S2 provides the following bounded evidence contributions:

```text
H-ISO-01 S2 contribution:
DISCRIMINATING DIRECTION OBSERVED

H-ISO-02 S2 contribution:
STRICT-SUBSET CORE IMPACT OBSERVED

H-ISO-03 S2 contribution:
STRICT-SUBSET CORE RE-EXECUTION IMPACT OBSERVED
```

These are not final cross-scenario hypothesis verdicts.

The pre-registered decision rules require S3 before a final `SUPPORTED FOR EVALUATED SCENARIOS`, `PARTIALLY SUPPORTED`, or `NOT SUPPORTED` classification can be assigned.

---

## 12. Decision

S2 preserved equivalent functional behavior, preserved the frozen history quantity and placement rules, exposed no hidden Core coupling under the bounded audit, and produced a valid discriminating comparison.

Decision:

```text
S2 VALID — PROCEED TO S3
```

S3 shall remain a separate, stronger-coupling ordinary-extension experiment. S2 results shall not be used to alter the pre-registered S3 scenario, metrics, or decision thresholds.
