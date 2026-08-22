# RQ-ISO-001 Phase A Frozen Baseline v0.1

Status: Frozen Pre-execution Baseline  
Research Question: RQ-ISO-001  
Date: 2026-08-23  
Tracking: GitHub Issue #70  
Protocol dependency: `RQ_ISO_001_Consequence_Test_Plan_v0.1.md`

---

## 1. Purpose

This artifact freezes the baseline used for the RQ-ISO-001 consequence test before scenario implementation or measurement begins.

It fixes:

- the repository baseline commit;
- the scored Core semantic and implementation artifacts;
- the baseline persistent-state schema;
- the restricted and permissive architecture conditions;
- the scenario classifications already pre-registered in the test plan; and
- the dependency rules used later to classify Core evidence as Retained, ReviewOnly, Reverify, Revalidate, or NewExtensionEvidence.

No consequence hypothesis is tested by this document.

This artifact is non-normative and does not modify the ThermoCore Framework Specification, Framework Conformance, published Validation Evidence, or the ThermoCore v1.0.0 release baseline.

---

## 2. Frozen Repository Baseline

The experimental baseline is:

```text
ThermoCore main commit:
8e3a948b0f36feefd313de1f03dd4db29b3bc465
```

This commit is the merge result of PR #71, which pre-registered the consequence-test protocol.

All S0-S4 comparisons shall be interpreted relative to this commit unless a later versioned correction explicitly replaces this baseline before any scenario measurement is accepted.

Changes to `main` after this commit do not silently alter the experiment baseline.

---

## 3. Frozen Core Semantic Manifest

The following Framework Specification artifacts define the semantic Core boundary for Condition R and are frozen for the experiment:

| Artifact | Baseline blob SHA | Experimental role |
|---|---|---|
| `Documentation/Framework_Specification/Framework_Principles.md` | `1d9a9c97570303a4e830fdc334da4f6eff370a64` | Root Framework authority |
| `Documentation/Framework_Specification/Core_Architecture.md` | `fe8f318db814e10d81a743fa126db47c5c2fe654` | Core responsibilities and ownership |
| `Documentation/Framework_Specification/Data_Flow.md` | `35a9850ebbefe572337718e79d7322e81e558a8e` | Information-flow semantics |
| `Documentation/Framework_Specification/Thermodynamic_State.md` | `6144b9f4ba2b601b9d38456485e1a8567a5b4c77` | Authoritative Runtime State semantics |
| `Documentation/Framework_Specification/Material_Representation.md` | `8156e9df7d8f223aa952aad33971709db2c60c57` | Representation boundary |
| `Documentation/Framework_Specification/Framework_Interfaces.md` | `2aaffe42117bb2b4871a9fb1ca99c39ea66e0d01` | Communication boundary |
| `Documentation/Framework_Specification/Extension_Boundary.md` | `e36346b91bdc912ac2ff03ddd888a31fd592477d` | Ordinary Extension authority boundary |
| `Documentation/Framework_Specification/Framework_Conformance.md` | `1c506c8c067085653c966ec0dd726ccf4ff10507` | Conformance/evidence relationship |
| `Documentation/Framework_Specification/Specification_Governance.md` | `73b5e78b211e609e119b61bc2ab632fdee7b1bd3` | Specification dependency/governance |

The bounded physical reference formulation is also frozen as experimental support:

| Artifact | Baseline blob SHA | Authority |
|---|---|---|
| `Documentation/Thermodynamic_Formulation.md` | `3d0e0ab9294a50927e8a337b13fcb34f324485c1` | Non-Framework reference-formulation specification |

A scenario is not permitted to rewrite these artifacts and then count the rewrite as an ordinary extension implementation.

If a scenario genuinely requires a semantic change to one of these artifacts, that change is scored as Core impact. For S4, such a result may be the correct architectural outcome.

---

## 4. Frozen Core Implementation Manifest

The following existing implementation artifacts are classified as frozen Core implementation for M-C3, M-C5, and hidden-coupling analysis:

| Artifact | Baseline blob SHA |
|---|---|
| `Framework/Core/EnergyInputMapping.cs` | `ab04de138445efe3ca0b6acfa2a4a0c025c8c3b7` |
| `Framework/Core/ReferenceMaterialCompiler.cs` | `38fc5de7562a9c07e049b92c072af0dde92b85e7` |
| `Framework/Core/ReferenceThermodynamicFormulation.cs` | `cf07c7b960f11e5f68c008d3cd9c6e7d7920ee28` |
| `Framework/Core/ThermodynamicComputation.cs` | `617fd39d70fdb5ec07a65cd140f12c9f1d12d047` |
| `Framework/Runtime/CompiledThermodynamicParameters.cs` | `ad3f265a06104116bc88070a08588b3b5bb3213e` |
| `Framework/Runtime/DerivedThermodynamicState.cs` | `263278c72566fc3ee23f9608f5046f8cca8a4180` |
| `Framework/Runtime/ThermodynamicState.cs` | `cc5edc3b6525b9fb35df97726b108395b4f78386` |

Repository guides, comments, generated artifacts, research documents, and formatting-only edits are not Core implementation artifacts for the primary structural metrics.

Material Definition artifacts may participate as Configuration, but they are not reclassified as Core Runtime State merely because a scenario uses them.

---

## 5. Frozen Baseline State Schema

For the bounded reference formulation at the frozen baseline:

```text
Persistent Thermodynamic State:
- SpecificEnthalpy : double [J/kg]
```

Temperature and liquid phase fraction are Derived State and are not mandatory Persistent State.

Therefore the semantic baseline for state-growth metrics is:

| Metric | S0 baseline value |
|---|---:|
| `M-S1` mandatory persistent Core-State semantic quantities | 1 |
| `M-S2` idealized mandatory Core-State payload bytes per element | 8 |
| `M-S3` extension-specific persistent quantities promoted into Core State | 0 |
| `M-S4` extension-local persistent payload bytes per element | 0 |
| `M-S5` total persistent semantic payload bytes per element | 8 |

`M-S2` and `M-S5` use semantic payload size only. Runtime padding, object headers, allocator overhead, alignment, container capacity, and backend-specific packing are excluded from these two primary values and may be reported separately.

---

## 6. Frozen Architecture Conditions

### 6.1 Condition R — Restricted Semantic/Core-State Boundary

Condition R is fixed as follows for the experiment:

- Thermodynamic State remains the authoritative Runtime State category.
- Thermodynamic Computation retains Core state-evolution responsibility.
- Representation Consumers may derive from Thermodynamic State but do not own or modify it.
- Ordinary extensions may own mechanism-specific persistent state.
- Extension-owned persistent state remains outside mandatory Core State unless the scenario is reclassified as requiring a genuine Core revision.
- Ordinary extension communication uses declared generic boundaries.
- Ordinary extension presence does not redefine Core State semantics, ownership, or Core completeness.

Condition R shall not be strengthened after results are observed.

### 6.2 Condition P — Permissive Shared-State Modular Comparator

Condition P is fixed as follows:

- implementation remains modular;
- extension computation remains in separable modules;
- an active ordinary extension may add persistent per-element fields to the shared authoritative Simulation/Core State schema;
- central state/update structures may carry or dispatch those extension-specific fields;
- shared state interfaces may expand to expose those fields; and
- Core completeness may be evaluated against the active shared-state schema.

Condition P is not a monolithic or intentionally defective architecture. It must provide equivalent target capability to Condition R for S1-S3.

For S2 and S3, Condition P shall actually exercise the permissive policy by promoting the declared extension-specific persistent quantity into the shared state schema. Otherwise the independent variable would not differ between conditions.

---

## 7. Frozen Scenario Classification

The scenario roles are frozen before implementation:

| Scenario | Frozen classification | State expectation |
|---|---|---|
| S0 Baseline | Core only | No extension state |
| S1 Derived Representation Consumer | Ordinary consumer / negative control | No persistent consumer state |
| S2 Thermal hysteresis | Ordinary stateful extension | Persistent history variable required |
| S3 Bounded exothermic reaction heat | Strongly interacting ordinary extension | Persistent reaction progress `xi` required |
| S4 Variable-mass compressible reactive flow | Core-change counterexample | Ordinary extension isolation expected to be insufficient |

S1 shall not be forced to create a difference merely to support a hypothesis.

S2 and S3 shall preserve equivalent declared functionality between R and P.

S4 is not an efficiency contest. It tests whether Condition R recognizes when Core change is semantically necessary.

---

## 8. Frozen Baseline Evidence Manifest

### 8.1 Verification

The baseline Core Verification executable is represented by:

| Artifact | Baseline blob SHA |
|---|---|
| `Tests/Verification/Program.cs` | `ca1d18d9194f1576039824a1c5cca7163b211186` |
| `Tests/ThermoCore.ReferenceVerification.csproj` | `fdf1d3e548c2cd9eadb4cd32e509eacbecd49b64` |

### 8.2 H2O caloric Validation

Frozen published evidence set includes:

- `Validation/Reference_Formulation_Caloric_Validation_Plan.md`
- `Validation/Reference_Formulation_Caloric_Validation_v0.1.md`
- `Validation/Execution/Program.cs` — blob `31c2f468de3cb34f5e82401b82ce2416e21c816c`
- frozen benchmark data under `Validation/Data/`

### 8.3 Gallium caloric Validation

Frozen published evidence set includes:

- `Validation/Reference_Formulation_Gallium_Caloric_Validation_Plan.md`
- `Validation/Reference_Formulation_Gallium_Caloric_Validation_v0.1.md`
- `Validation/Execution/GalliumProgram.cs` — blob `254d451b3ce9e3fa4ffca7449b1c0692107517fb`
- frozen benchmark data under `Validation/Data/`

These Validation records remain descriptive physical-comparison evidence. The experiment shall not reinterpret their historical conclusions.

---

## 9. Frozen Evidence-Impact Dependency Rules

The following rules are used later for `M-E1`-`M-E5`. They are impact-classification rules, not claims that every possible change has the same consequence.

### D1 — Normative semantic dependency

A change to a frozen Framework Specification that changes State identity, ownership, Extension authority, interface semantics, or Core completeness is a Core semantic change.

For S1-S3, such a change counts against Core-change isolation and cannot be hidden as extension-only work.

If the change alters the semantics against which existing Verification or Validation evidence was interpreted, the affected evidence is at least `ReviewOnly`; re-execution is required when the executable/formulation dependency also changes.

For S4, a justified D1 change may be the correct outcome.

### D2 — Reference state/recovery dependency

Changes to the persistent state schema or the reference recovery/material-parameter path, including relevant changes to:

- `Framework/Runtime/ThermodynamicState.cs`;
- `Framework/Runtime/DerivedThermodynamicState.cs`;
- `Framework/Core/ReferenceThermodynamicFormulation.cs`;
- `Framework/Core/ReferenceMaterialCompiler.cs`; or
- `Framework/Runtime/CompiledThermodynamicParameters.cs`

require Core Verification impact review.

If the changed behavior lies on the executed `h -> T`, `h -> phase`, latent-interval, material-parameter, or caloric-comparison path used by the H2O/Gallium benchmarks, the applicable caloric Validation is classified `Revalidate` rather than retained solely because the change was introduced for an extension.

### D3 — Core energy-update dependency

Changes to:

- `Framework/Core/EnergyInputMapping.cs`; or
- `Framework/Core/ThermodynamicComputation.cs`

require Core Verification impact review and normally `Reverify` when executable behavior changes.

Existing H2O/Gallium caloric Validation is not automatically classified `Revalidate` for an energy-update-only change if its frozen executable dependency does not exercise the changed path. It remains `Retained` or `ReviewOnly` according to the actual frozen dependency.

### D4 — Extension-only dependency

A change confined to a scenario-specific extension artifact may be classified `NewExtensionEvidence` when:

- no frozen Core semantic artifact changes;
- no frozen Core implementation artifact changes;
- no frozen generic Core interface acquires extension-specific semantics;
- no existing Validation executable/formulation dependency changes; and
- the hidden-coupling audit finds no undeclared Core dependency.

Under those conditions, existing Core evidence may remain `Retained` while new extension-specific Verification/Validation is added for the extension claim.

### D5 — Generic interface dependency

A change to a generic shared interface is a Core impact when the interface's semantic contract, required fields, signature, or mandatory state exposure changes.

Adding an extension-specific field under a neutral generic name still counts as Core/interface change if the field is mandatory for the shared contract.

### D6 — No label-based exemption

Neither the filesystem location nor the word `Extension` determines evidence impact.

Evidence classification follows semantic and executable dependencies. A file under an extension directory may invalidate Core evidence if it changes a dependency; a Core-adjacent documentation-only edit may leave executable evidence retained if semantics are unchanged.

---

## 10. Baseline Fairness Rules

The following are frozen before S1-S4 work:

1. R and P begin from equivalent S0 capability.
2. R and P use the same scenario-level physical simplification and observable target for each ordinary-extension case.
3. P remains modular and must not be deliberately degraded.
4. R may not omit required persistent information to achieve a smaller Core State.
5. Total persistent semantic payload `M-S5` is always reported with Core-only payload.
6. Hidden coupling is scored even when hidden behind generic wrappers or adapters.
7. Existing evidence is never retained or invalidated merely by naming convention.
8. S4 must permit explicit Core revision or out-of-scope classification when governing physics requires it.
9. No metric or decision threshold may be changed after scenario results are observed without creating a new protocol version and invalidating the affected pre-registration claim.

---

## 11. Phase A Completion Criteria

Phase A is complete when the following are fixed before scenario measurement:

- [x] repository baseline commit;
- [x] Core semantic artifact manifest;
- [x] Core implementation artifact manifest;
- [x] baseline persistent-state schema;
- [x] Condition R policy;
- [x] Condition P policy;
- [x] S0-S4 scenario classifications;
- [x] baseline Verification/Validation evidence manifest;
- [x] evidence-impact dependency rules; and
- [x] fairness/anti-displacement rules.

No H-ISO-01, H-ISO-02, or H-ISO-03 result is produced by Phase A.

---

## 12. Next Stage

After this freeze artifact is reviewed and merged, Phase B may instantiate S0 under both conditions and execute the S1 negative control.

Scenario implementation shall use separate versioned research/evaluation artifacts and shall not silently alter this frozen baseline.