# RQ-ISO-001 S3 Result v0.1

Status: COMPLETED — functional equivalence confirmed; S3 discriminating result recorded  
Research Question: RQ-ISO-001  
Scenario: S3 — Bounded Exothermic Reaction-Heat Extension  
Tracking: GitHub Issue #77  
Protocol: `RQ_ISO_001_Consequence_Test_Plan_v0.1.md`  
Scenario freeze: `RQ_ISO_001_S3_Scenario_Freeze_v0.1.md`  
Frozen comparison baseline: `8e3a948b0f36feefd313de1f03dd4db29b3bc465`

---

## 1. Purpose

This record reports the S3 bounded exothermic reaction-heat scenario of the pre-registered RQ-ISO-001 consequence test.

S3 is the stronger ordinary stateful-extension case after the valid S0/S1 control stage and the discriminating S2 hysteresis result.

The result is bounded to the frozen fixed-mass / no-transport mechanism. It does not claim that arbitrary chemistry belongs outside Thermodynamic State, does not modify the ThermoCore Framework Specification, and does not establish novelty or universal superiority.

After recording the S3 result, this document applies the pre-registered S1-S3 decision rules for H-ISO-01, H-ISO-02, and H-ISO-03. S4 remains a separate boundary-validity counterexample.

---

## 2. Frozen Scenario Used

The pre-execution freeze defined exactly one persistent extension-specific quantity:

```text
xi : double
range: [0, 1]
initial value: 0
```

The same bounded reaction rule was used in both conditions:

```text
activation temperature T_act = 300 K
maximum delta_xi per step     = 0.25
total specific reaction heat  = 80000 J/kg

if T >= T_act and xi < 1:
    delta_xi := min(0.25, 1 - xi)
else:
    delta_xi := 0

xi_next := xi + delta_xi
delta_h_reaction := 80000 * delta_xi
```

The reaction contribution entered only through the existing `ThermodynamicComputation.ApplySpecificEnthalpyIncrement(...)` state-evolution path.

Frozen external specific-enthalpy sequence:

```text
+2000, +3000, 0, 0, 0, 0 J/kg
```

Frozen expected post-step reaction-progress sequence:

```text
0.00, 0.25, 0.50, 0.75, 1.00, 1.00
```

Frozen expected reaction-heat sequence:

```text
0, 20000, 20000, 20000, 20000, 0 J/kg
```

No state placement, reaction constant, input schedule, expected output, evidence rule, or hypothesis threshold was changed after execution began.

---

## 3. Frozen Physical Boundary

The S3 ordinary-extension classification remained valid during execution because the harness required none of the excluded governing mechanisms:

- total element mass remained fixed;
- no species mass fraction was introduced;
- no species transport was introduced;
- no pressure state or pressure evolution was introduced;
- no velocity, momentum, or flow field was introduced;
- no density evolution was introduced; and
- the reference `h -> T` / `h -> phase` closure remained unchanged.

The only thermodynamic coupling was the declared additive specific-energy contribution.

Result:

```text
S3 FIXED-MASS / NO-TRANSPORT BOUNDARY — PRESERVED
```

No S3 reclassification was required before measurement acceptance.

---

## 4. Architecture Conditions Executed

### Condition R

Condition R retained the frozen Core state:

```text
Thermodynamic State:
- SpecificEnthalpy : double
```

and stored:

```text
xi : double
```

as extension-owned persistent state.

The extension consumed recovered Temperature, updated only its own `xi`, and supplied the resulting `delta_h_reaction` to the existing Thermodynamic Computation energy-update path.

### Condition P

Condition P remained modular but represented the same `xi` quantity as part of shared authoritative Simulation/Core State:

```text
Shared state:
- SpecificEnthalpy : double
- xi               : double
```

The reaction computation itself remained the same separate module. The controlled difference remained state authority / membership placement, not functionality or modularity.

---

## 5. Execution Evidence

Research-only executable harness:

`Research/05_Research_Questions/Execution/RQ_ISO_001_S3/`

GitHub Actions workflow:

`RQ-ISO-001 S3`

Successful source-head commit:

`a51851d5d69712a778fc374c8c2b9695cccad60e`

Successful workflow run:

`32589009448` — run #1 — `success`

Observed executable output:

```text
RQ-ISO-001 S3 Bounded Exothermic Reaction Heat
Frozen comparison baseline: 8e3a948b0f36feefd313de1f03dd4db29b3bc465
S3_FUNCTIONAL_EQUIVALENCE=CONFIRMED
S3_EXPECTED_SEQUENCE=CONFIRMED
S3_REACTION_PROGRESS_QUANTITY=xi(double)
S3_FIXED_MASS_NO_TRANSPORT_BOUNDARY=CONFIRMED
R_CORE_PERSISTENT_QUANTITIES=1
R_CORE_SEMANTIC_BYTES=8
R_PROMOTED_EXTENSION_QUANTITIES=0
R_EXTENSION_LOCAL_BYTES=8
R_TOTAL_PERSISTENT_BYTES=16
P_CORE_PERSISTENT_QUANTITIES=2
P_CORE_SEMANTIC_BYTES=16
P_PROMOTED_EXTENSION_QUANTITIES=1
P_EXTENSION_LOCAL_BYTES=0
P_TOTAL_PERSISTENT_BYTES=16
S3_CROSS_SCENARIO_DECISION_RULE_INPUT=VALID
```

---

## 6. Functional Equivalence

Both conditions used:

- the same initial thermodynamic state;
- the same external energy-input sequence;
- the same reference thermodynamic recovery path;
- the same reaction-progress rule;
- the same reaction heat per `delta_xi`; and
- the same existing thermodynamic energy-update operation.

At every frozen step, Condition R and Condition P produced equivalent:

- `xi`;
- reaction specific-enthalpy contribution;
- final Specific Enthalpy;
- recovered Temperature; and
- recovered liquid phase fraction.

The observed sequence also matched the frozen expected values.

Result:

```text
S3 FUNCTIONAL EQUIVALENCE — CONFIRMED
```

No persistent information was omitted from Condition R and no extra persistent quantity was added to Condition P beyond the frozen `xi` field.

---

## 7. State Metrics — H-ISO-01 Contribution

Primary values use semantic payload size as pre-registered. Runtime padding, allocator/container overhead, and backend packing are excluded.

| Metric | Condition R | Condition P |
|---|---:|---:|
| `M-S1` mandatory persistent Core-State quantities | 1 | 2 |
| `M-S2` mandatory Core-State semantic bytes / element | 8 | 16 |
| `M-S3` extension-specific persistent quantities promoted into Core | 0 | 1 |
| `M-S4` extension-local persistent semantic bytes / element | 8 | 0 |
| `M-S5` total persistent semantic bytes / element | 16 | 16 |

Interpretation:

Condition R reduced **Core-State promotion**, not total persistent information.

Both architectures retain exactly the same 16-byte semantic information payload for this bounded scenario. Therefore S3 does **not** support a total-memory-reduction claim.

S3 satisfies the discriminating direction required by H-ISO-01 while preserving equivalent functionality.

---

## 8. Core-change Metrics — H-ISO-02 Contribution

The repository implementation of both conditions is research-only. The counts below represent the logical architectural Core impacts required by the frozen R/P policies relative to S0; research harness files are not counted as production ThermoCore Core changes.

### Condition R

No frozen Core semantic, implementation, or generic interface artifact requires S3-specific modification.

The existing generic energy-update operation accepts the reaction-specific enthalpy increment without acquiring reaction-specific semantics.

### Condition P

The already-frozen permissive policy itself is unchanged, so no new normative policy requirement is counted. However, active shared authoritative state grows to include `xi`.

Logical P-S3 Core impacts are:

- one shared-state semantic schema change;
- one shared-state implementation schema change;
- one shared-state access/interface contract expansion exposing `xi`; and
- one direct shared-state dependency on the S3-specific semantic quantity.

| Metric | Condition R | Condition P |
|---|---:|---:|
| `M-C1` Core normative requirements changed | 0 | 0 |
| `M-C2` Core semantic artifacts changed | 0 | 1 |
| `M-C3` Core implementation artifacts changed | 0 | 1 |
| `M-C4` Core interface contracts/signatures changed | 0 | 1 |
| `M-C5` extension-specific branches inside frozen Core implementation | 0 | 0 |
| `M-C6` direct Core-to-extension-specific dependency edges | 0 | 1 |

S3 therefore produces a strict-subset Core-impact result for Condition R relative to Condition P while keeping both conditions modular.

---

## 9. Hidden-coupling / Complexity-displacement Audit

Condition R was inspected against the frozen anti-displacement safeguards.

No equivalent S3-specific Core coupling was identified:

- no reaction-specific type check was added to Core code;
- no reaction-name-specific branch or switch was added to Core code;
- no frozen Core artifact imports or depends on the concrete S3 module;
- `xi` is not hidden inside a generic Core container;
- the generic specific-enthalpy increment operation retains non-reaction-specific semantics;
- no duplicate authoritative Thermodynamic State is created;
- no Core adapter changes for S3;
- no scenario-specific synchronization mechanism is introduced by the bounded sequential harness; and
- the dependency direction is extension -> existing Core information / energy-update service, not Core -> concrete S3 extension.

Audit result:

```text
NO EQUIVALENT HIDDEN CORE COUPLING IDENTIFIED FOR S3
```

This result is bounded to the frozen S3 mechanism.

---

## 10. Evidence-impact Metrics — H-ISO-03 Contribution

The Phase A D1-D6 dependency rules and the same counting method used in S2 were applied to both conditions.

Frozen Core evidence set:

1. Reference Verification suite;
2. H2O caloric Validation record; and
3. Gallium caloric Validation record.

### Condition R

The S3 extension uses the existing recovery and energy-update paths without changing their implementation or semantic contracts.

Classification:

- Reference Verification: `Retained`
- H2O caloric Validation: `Retained`
- Gallium caloric Validation: `Retained`
- S3 reaction-progress/source-coupling behavior: `NewExtensionEvidence`

The new S3 evidence is required because existing Core evidence does not establish the correctness of the scenario-specific reaction rule or its source contribution.

### Condition P

The active shared persistent-state schema and its access contract change relative to P-S0.

Under D2 and D5, this requires Core state/schema Verification impact review and re-execution of the applicable layer.

The frozen H2O/Gallium caloric executables do not exercise a changed `h -> T` / `h -> phase` formulation path, so they are not classified `Revalidate`. Their applicability to the enlarged authoritative-state contract is conservatively classified `ReviewOnly`, using the same rule applied in S2.

Classification:

- Reference Verification: `Reverify`
- H2O caloric Validation: `ReviewOnly`
- Gallium caloric Validation: `ReviewOnly`
- S3 reaction-progress/source-coupling behavior: `NewExtensionEvidence`

Primary counts:

| Metric | Condition R | Condition P |
|---|---:|---:|
| `M-E1` Core requirement/schema items requiring impact review | 0 | 1 |
| `M-E2` Core Verification records requiring re-execution | 0 | 1 |
| `M-E3` Core Validation records requiring re-execution | 0 | 0 |
| `M-E4` Core evidence records retained without re-execution | 3 | 2 |
| `M-E5` new extension-specific evidence records required | 1 | 1 |

S3 therefore produces the same type of strict-subset justified Core re-execution impact observed independently in S2.

---

## 11. Repository-change Boundary

The S3 branch changes only:

- the frozen S3 research scenario definition;
- the S3 research-only executable harness;
- the S3 research workflow; and
- this S3 result record.

No Framework Specification or production `Framework/Core` / `Framework/Runtime` source is modified by the experiment.

This repository fact is separate from the logical Condition P Core-impact counts. The P shared-state growth is represented inside the controlled research comparator so that the published ThermoCore implementation is not rewritten merely to execute the experiment.

---

## 12. S1-S3 Cross-Scenario Hypothesis Classification

The pre-registered decision rules are now applied without a composite score.

### H-ISO-01 — State-growth Isolation

Evidence:

- S1 negative control: equal zero promotion in R and P;
- S2: R promoted 0 extension-specific quantities into Core; P promoted 1;
- S3: R promoted 0 extension-specific quantities into Core; P promoted 1;
- equivalent scenario functionality was preserved; and
- total persistent semantic payload was explicitly reported and equal within both S2 and S3 comparisons.

Classification:

```text
H-ISO-01:
SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS (S1-S3)
```

This means Core-State growth isolation was observed. It does not mean total memory was reduced.

### H-ISO-02 — Core-change Isolation

Evidence:

- S1 remained neutral;
- S2 showed Condition R as a strict subset of Condition P for frozen Core semantic / implementation / interface impact;
- S3 independently showed the same strict-subset relation; and
- S2 and S3 hidden-coupling audits did not identify equivalent extension-specific Core coupling displaced behind generic infrastructure.

Classification:

```text
H-ISO-02:
SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS (S1-S3)
```

This does not yet establish that the restricted boundary correctly recognizes every case that genuinely requires Core revision. S4 tests that separate boundary-validity requirement.

### H-ISO-03 — Revalidation-scope Isolation

Evidence:

- identical frozen dependency-classification rules were used for R and P;
- S2 required no Core evidence re-execution under R and one Core Verification re-execution under P;
- S3 produced the same strict-subset re-execution relation;
- neither scenario required caloric `Revalidate` because the frozen recovery/formulation dependencies were unchanged; and
- both conditions required scenario-specific new evidence rather than treating Core evidence as proof of extension behavior.

Classification:

```text
H-ISO-03:
SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS (S1-S3)
```

The supported effect is a smaller justified **Core evidence re-execution set**, not absence of extension-specific verification/validation obligations.

---

## 13. Interpretation Boundary

The S1-S3 results support an engineering consequence of the candidate property within the evaluated bounded scenarios:

> keeping ordinary extension-specific persistent state outside mandatory Core State can isolate Core-State growth, Core contract changes, and Core evidence re-execution while preserving equivalent functionality.

The evidence does **not** establish:

- novelty;
- universal superiority over all alternative architectures;
- lower total memory use;
- zero integration complexity;
- complete physical Validation;
- automatic Framework Conformance; or
- that Core should never change.

Those stronger statements are outside the evidence.

---

## 14. Decision

S3 preserved the frozen physical boundary, functional equivalence, state-placement rules, and evidence-dependency rules, while independently reproducing the discriminating direction observed in S2.

Therefore the ordinary-extension consequence-test stage reaches:

```text
H-ISO-01: SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS (S1-S3)
H-ISO-02: SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS (S1-S3)
H-ISO-03: SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS (S1-S3)
```

Candidate-property status after S3:

```text
CONSEQUENCE SUPPORT ESTABLISHED FOR S1-S3
BOUNDARY VALIDITY NOT YET TESTED — S4 PENDING
```

Decision:

```text
S3 VALID — PROCEED TO S4 BOUNDARY-VALIDITY COUNTEREXAMPLE
```

S4 shall not be used to manufacture additional support for S1-S3. Its role is to test whether the restricted architecture correctly admits Core revision or out-of-scope classification when governing physics genuinely changes.
