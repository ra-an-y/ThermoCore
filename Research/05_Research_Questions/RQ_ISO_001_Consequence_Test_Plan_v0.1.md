# RQ-ISO-001 Consequence Test Plan v0.1

Status: Draft — Pre-registered Research Evaluation Design  
Research Question: RQ-ISO-001  
Date: 2026-08-23  
Task: GitHub Issue #70  
Primary dependency: [`RQ_ISO_001_Research_Gap_Analysis_v0.1.md`](../04_Research_Gap/RQ_ISO_001_Research_Gap_Analysis_v0.1.md)

---

## 1. Objective

This document defines a bounded consequence-testing protocol for the RQ-ISO-001 candidate property:

> **Fixed Semantic/Core-State Boundary under Ordinary Extension**

The evaluation asks whether enforcing that boundary produces observable engineering consequences in the evaluated thermodynamic-framework scope.

The plan is intentionally pre-registered before implementation measurements. Metrics, scenario classifications, and decision rules are defined here so that later results are not judged using post-hoc criteria selected after observing the outcome.

This document is non-normative. It does not modify the ThermoCore Framework Specification, Framework Conformance, the v1.0.0 publication baseline, existing Validation Evidence, or the current reference formulation.

---

## 2. Research Dependencies

The plan depends on the following evidence and specification boundaries:

- `Research/01_Evidence_Matrix/Isolation_Capability_Matrix_v0.6.md`
- `Research/04_Research_Gap/RQ_ISO_001_Research_Gap_Analysis_v0.1.md`
- `Documentation/Framework_Specification/Thermodynamic_State.md`
- `Documentation/Framework_Specification/Extension_Boundary.md`
- `Documentation/Framework_Specification/Framework_Conformance.md`
- `Tests/README.md`
- `Validation/README.md`

The normative Framework Specifications are used only to define the already-published restricted boundary. They are not treated as research evidence proving that the boundary is beneficial.

---

## 3. Hypotheses Under Test

### H-ISO-01 — State-growth Isolation

For controlled ordinary extensions, the restricted architecture will require fewer extension-specific quantities to become mandatory Core State than the permissive comparator.

### H-ISO-02 — Core-change Isolation

For the same controlled ordinary extensions, the restricted architecture will require fewer changes to Core state semantics, Core responsibilities, and Core interfaces than the permissive comparator.

### H-ISO-03 — Revalidation-scope Isolation

For extension-only changes that preserve the declared restricted boundary, the restricted architecture will require a smaller justified set of Core evidence to be reviewed or re-executed than the permissive comparator.

These hypotheses are independent. Support for one shall not be treated as support for another.

---

## 4. Experimental Principle

The independent variable is the **state-authority / Core-membership policy**, not whether the architecture is modular.

Both evaluated conditions shall remain modular and shall provide equivalent target capability for each ordinary-extension scenario.

The comparison shall not be constructed as:

```text
well-designed ThermoCore
versus
intentionally monolithic bad design
```

Instead, both conditions shall use separable extension modules. They differ only in where persistent extension-specific information is permitted to become authoritative shared state and how extension changes are allowed to alter the Core semantic contract.

---

## 5. Architecture Conditions

### 5.1 Condition R — Restricted Semantic/Core-State Boundary

Condition R follows the ThermoCore-style boundary already present in the v1.0.0 architecture:

- Thermodynamic State remains the authoritative Runtime State category.
- Thermodynamic Computation retains state-evolution responsibility.
- Representation Consumers may derive from Thermodynamic State but do not own or modify it.
- Ordinary extensions may own mechanism-specific persistent state.
- Extension-owned state remains distinct from mandatory Core State.
- Ordinary extensions communicate through declared boundaries.
- Adding an ordinary extension does not redefine Core State semantics, ownership, or Core completeness.

Condition R is an experimental use of an existing architecture property. Its inclusion does not assume that the property is superior.

### 5.2 Condition P — Permissive Shared-State Modular Comparator

Condition P remains modular but allows the shared simulation-state schema to grow when an active extension needs persistent per-element information.

Under Condition P:

- extension computation remains implemented in separate modules;
- an extension may add persistent fields directly to the shared Core/Simulation State schema;
- the central state update may carry or dispatch extension-specific state fields;
- shared interfaces may expand to expose those new state fields; and
- Core completeness may be defined against the active shared state schema.

Condition P is not claimed to represent any specific prior framework. It is a controlled comparator designed to isolate the consequence of allowing ordinary extensions to enlarge shared authoritative state while preserving modular implementation.

### 5.3 Fairness Requirement

For each ordinary-extension scenario, Conditions R and P shall implement the same declared functional behavior, use the same physical simplifications, and expose equivalent observable outputs wherever practical.

A metric difference is not interpretable if one condition implements less functionality.

---

## 6. Frozen Baseline Before Scenario Work

Before implementing scenario-specific changes, the experiment shall record a baseline manifest for both conditions.

The manifest shall identify:

- Core semantic artifacts;
- Core implementation artifacts;
- Core state schema;
- Core public/internal interface contracts relevant to the experiment;
- extension boundaries;
- Verification cases applicable to the baseline;
- Validation records applicable to the baseline; and
- dependency links used later for evidence-impact analysis.

The Core manifest shall be frozen before scenario implementation so that artifacts cannot be reclassified as `extension` or `Core` after observing the result.

Formatting-only changes, comments, generated files, and unrelated repository edits shall be excluded from structural change counts and reported separately when they occur.

---

## 7. Controlled Scenario Matrix

The scenarios deliberately include one negative control, two ordinary extensions of increasing coupling strength, and one Core-change counterexample.

| ID | Scenario | Expected classification | Persistent extension-local state | Intended purpose |
|---|---|---|---:|---|
| S0 | Baseline, no optional extension | Core only | No | Establish identical starting point |
| S1 | Derived Representation Consumer | Ordinary consumer | No | Negative control: neither condition should need Core-State growth |
| S2 | Thermal hysteresis material-response extension | Ordinary stateful extension | Yes | Test extension-local history without Core promotion |
| S3 | Bounded exothermic reaction-heat extension | Strongly interacting ordinary extension | Yes | Test stateful mechanism contributing energy/property effects |
| S4 | Variable-mass compressible reactive-flow mechanism | Core-change counterexample | Yes, but insufficient as extension-only state | Verify that the boundary permits/requires Core revision when governing physics genuinely changes |

### 7.1 S0 — Baseline

S0 uses the same bounded thermodynamic capability in both conditions with no optional extension.

Required result for experimental validity:

- equivalent observable baseline behavior;
- no scenario-specific state; and
- a frozen Core/evidence manifest for later comparison.

S0 is not used to support any hypothesis by itself.

### 7.2 S1 — Derived Representation Consumer

S1 adds a consumer that derives presentation-oriented information from Thermodynamic State, such as a temperature/phase visualization mapping, without persistent consumer-local history.

This is a negative control.

A sensible restricted or permissive modular architecture should not require mandatory Core-State growth for this case. If Condition P is artificially forced to add presentation state to Core solely to create a difference, the comparison is invalid.

Expected interpretation:

- equal zero promotion is acceptable and desirable;
- H-ISO-01 is not supported by S1 merely because Condition R remains unchanged; and
- any unexpected Core changes in either condition require investigation.

### 7.3 S2 — Thermal Hysteresis Extension

S2 introduces a bounded material-response mechanism that requires a persistent history variable to distinguish heating and cooling response.

The experiment shall use the same hysteresis rule in both conditions.

Condition R shall place the history variable in extension-owned persistent state and communicate only the information required by declared coupling boundaries.

Condition P may place the same history variable into the shared state schema and expose it through shared state access while keeping hysteresis computation modular.

The experiment is not intended to claim that hysteresis universally belongs outside Thermodynamic State. It tests the consequence of one explicitly classified ordinary-extension case under a frozen scenario definition.

### 7.4 S3 — Bounded Exothermic Reaction-Heat Extension

S3 introduces a bounded reaction-progress variable `xi` owned by the extension and a reaction-heat contribution to thermodynamic computation.

The scenario shall be deliberately limited so that:

- total cell mass is fixed for the experiment;
- no species transport is modeled;
- no pressure evolution is introduced;
- reaction progress is extension-specific persistent information;
- reaction heat enters through a declared energy/source contribution; and
- any effective-property update is explicitly declared.

These restrictions keep S3 within the intended ordinary-extension boundary while providing stronger two-way coupling than S2.

Condition R shall retain `xi` as extension-owned state.

Condition P may promote `xi` into the shared state schema and permit central state structures/interfaces to grow accordingly.

### 7.5 S4 — Variable-Mass Compressible Reactive Flow Counterexample

S4 intentionally violates the ordinary-extension assumptions by requiring a mechanism whose correct formulation introduces coupled mass/species transport, pressure/density evolution, and flow-dependent energy transport.

The purpose is not to implement a full CFD solver. The purpose is to establish that the restricted boundary is not interpreted as `Core never changes`.

For S4, a valid architecture must identify that ordinary extension isolation is insufficient. The outcome may require:

- a new or revised Core responsibility;
- a new authoritative state category;
- a revised governing formulation; or
- rejection of the mechanism as outside ThermoCore Core scope.

If Condition R hides S4 behind extension-local state while leaving an architecturally incorrect Core unchanged, the candidate property fails its boundary-validity test.

---

## 8. Metric Set

Metrics are grouped by hypothesis and by safeguards against misleading interpretation.

### 8.1 State Metrics — H-ISO-01

For each scenario and condition record:

- `M-S1` — number of mandatory persistent Core-State semantic quantities;
- `M-S2` — idealized payload bytes per simulation element required by mandatory Core State;
- `M-S3` — number of extension-specific persistent quantities promoted into mandatory Core State;
- `M-S4` — persistent bytes per element owned locally by extensions; and
- `M-S5` — total persistent bytes per element across Core plus extensions.

`M-S5` is mandatory because a reduction in Core-State bytes must not be misreported as a reduction in total memory when the same information merely resides in extension-owned state.

Implementation padding, allocator overhead, cache layout, and backend-specific packing shall be reported separately from semantic payload size.

### 8.2 Core-change Metrics — H-ISO-02

For each scenario and condition record:

- `M-C1` — set and count of Core normative requirements changed;
- `M-C2` — set and count of Core semantic artifacts changed;
- `M-C3` — set and count of Core implementation artifacts changed;
- `M-C4` — set and count of Core interface contracts/signatures changed;
- `M-C5` — number of extension-specific conditions/branches introduced inside frozen Core implementation artifacts; and
- `M-C6` — number of direct Core-to-extension dependency edges introduced outside the declared generic boundary.

Raw line count may be recorded as descriptive secondary evidence but shall not be a primary support criterion because formatting, language syntax, and code organization can dominate LOC.

### 8.3 Evidence-impact Metrics — H-ISO-03

Each changed artifact shall be mapped through a predeclared dependency record to applicable requirements and evidence.

For each scenario and condition record the Core evidence sets classified as:

- `Retained` — no review or re-execution required by the declared dependency;
- `ReviewOnly` — semantic applicability must be reviewed but execution is not required;
- `Reverify` — applicable Core Verification must be re-executed;
- `Revalidate` — applicable physical Validation must be re-executed because its implementation/formulation dependency changed; and
- `NewExtensionEvidence` — new evidence needed only for the extension-specific claim.

Primary counts:

- `M-E1` — Core requirements requiring impact review;
- `M-E2` — Core Verification cases requiring re-execution;
- `M-E3` — Core Validation records requiring re-execution;
- `M-E4` — Core evidence records retained without re-execution; and
- `M-E5` — extension-specific new evidence records required.

Validation shall not be rerun merely because an extension exists. Conversely, Validation shall not be exempted merely because the change is labeled an extension. Revalidation classification must follow the frozen dependency mapping.

---

## 9. Hidden-Coupling / Complexity-Displacement Audit

The experiment shall explicitly test whether Condition R obtains smaller Core-change numbers only by moving equivalent Core-specific coupling into hidden infrastructure.

For every scenario inspect:

- extension-specific type checks inside Core code;
- extension-name-specific branches or switch cases;
- direct Core imports/dependencies on a concrete extension;
- extension-specific fields hidden inside generic Core containers;
- generic interfaces that acquire extension-specific semantics despite neutral names;
- duplicate authoritative copies of the same state;
- synchronization obligations created solely because state was moved out of Core; and
- adapter layers that must change for every new extension.

If a reduction in `M-C1`–`M-C4` is offset by equivalent extension-specific changes inside supposedly generic Core infrastructure, H-ISO-02 shall be narrowed or marked not supported for that scenario.

---

## 10. Pre-registered Decision Rules

No composite `ThermoCore wins` score shall be produced.

Each hypothesis is classified independently as `SUPPORTED FOR EVALUATED SCENARIOS`, `PARTIALLY SUPPORTED`, `NOT SUPPORTED`, or `FALSIFIED / RECLASSIFICATION REQUIRED`.

### 10.1 H-ISO-01 Decision Rule

H-ISO-01 is `SUPPORTED FOR EVALUATED SCENARIOS` only if:

1. S1 remains a negative control with no artificial Core-State promotion in either condition;
2. at least S2 and S3 preserve equivalent functionality;
3. Condition R has fewer extension-specific quantities promoted into mandatory Core State than Condition P for S2 and S3;
4. the difference is not created by omitting required persistent information; and
5. total persistent state (`M-S5`) is reported so that Core isolation is not misrepresented as total memory reduction.

If Condition R and Condition P require the same mandatory Core-State growth, H-ISO-01 is `NOT SUPPORTED` for the evaluated scenarios.

### 10.2 H-ISO-02 Decision Rule

H-ISO-02 is `SUPPORTED FOR EVALUATED SCENARIOS` only if, for S2 and S3:

- Condition R changes a strict subset of the frozen Core semantic requirements/artifacts/interfaces changed by Condition P; and
- the hidden-coupling audit does not reveal equivalent extension-specific Core coupling displaced behind generic wrappers.

S4 must trigger explicit Core-revision consideration in Condition R. If Condition R reports zero Core impact for S4 solely by hiding necessary governing changes in an extension, the candidate boundary is `FALSIFIED / RECLASSIFICATION REQUIRED`.

### 10.3 H-ISO-03 Decision Rule

H-ISO-03 is `SUPPORTED FOR EVALUATED SCENARIOS` only if, for at least S2 and S3:

- the same dependency-classification rules are used for both conditions;
- Condition R produces a strict subset of Core Verification and/or Validation evidence requiring re-execution compared with Condition P; and
- no retained evidence depends on a changed semantic or implementation dependency.

If both conditions require the same justified Core evidence re-execution, H-ISO-03 is `NOT SUPPORTED` even if Condition R has cleaner source-code separation.

---

## 11. Boundary-validity Rule for S4

S4 is not scored as an ordinary-extension efficiency scenario.

Its role is adversarial.

The candidate property remains credible only if the architecture recognizes when extension isolation is no longer semantically valid.

Acceptable S4 outcomes include:

- explicit Core revision;
- a separately specified new Core responsibility;
- a new research/specification cycle before integration; or
- explicit classification of S4 as outside current ThermoCore Core scope.

An unacceptable outcome is preserving unchanged Core semantics by silently moving required Core physics into an `Extension` label.

---

## 12. Execution Sequence

### Phase A — Freeze

1. Freeze repository baseline commit for the experiment.
2. Freeze Core artifact manifest and evidence dependency map.
3. Freeze Condition R and Condition P architecture rules.
4. Freeze scenario requirements and expected observable outputs.

### Phase B — Baseline and Negative Control

1. Implement/instantiate S0 for both conditions.
2. Confirm equivalent baseline capability.
3. Execute S1.
4. Confirm S1 does not create an artificial difference.

### Phase C — Ordinary Stateful Extensions

1. Implement S2 under both conditions.
2. Record state, Core-change, hidden-coupling, and evidence-impact metrics.
3. Implement S3 under both conditions.
4. Record the same metric set.

### Phase D — Adversarial Core-change Case

1. Analyze S4 against both architecture policies.
2. Record which assumptions fail.
3. Record whether Core revision is recognized rather than hidden.

Full CFD implementation is not required to establish the architectural classification if the governing-state/conservation dependency is explicit and traceable.

### Phase E — Analysis

1. Apply only the pre-registered decision rules in Section 10.
2. Preserve all negative and null results.
3. Report scenario-bounded conclusions.
4. Do not convert support into a universal or novelty claim.

---

## 13. Required Result Artifacts

Later execution should preserve at minimum:

- frozen baseline manifest;
- Condition R architecture record;
- Condition P architecture record;
- per-scenario requirement definitions;
- per-scenario implementation diffs or equivalent structural records;
- state metric tables;
- Core-change metric tables;
- hidden-coupling audit records;
- evidence dependency/impact matrix;
- H-ISO-01 result;
- H-ISO-02 result;
- H-ISO-03 result; and
- S4 boundary-validity conclusion.

Historical results shall remain immutable once reported for a fixed experimental baseline. Corrections shall be versioned rather than silently replacing prior measurements.

---

## 14. Claims This Plan Cannot Establish

Even a positive result under this plan cannot by itself establish that:

- ThermoCore is the first architecture to enforce the candidate property;
- the property is universally superior;
- all extensions should remain outside Core State;
- total application memory is always reduced;
- implementation effort is always reduced;
- physical accuracy is improved;
- every future extension will reduce revalidation scope;
- other frameworks are architecturally inferior; or
- the current Framework Specification should be modified.

A positive result would support only the narrower claim that the fixed semantic/Core-State boundary produced the measured consequences for the evaluated thermodynamic scenarios and frozen comparator conditions.

---

## 15. Stop / Reclassification Conditions

The experiment shall stop or reclassify the candidate if:

1. Condition P cannot provide equivalent target functionality without becoming intentionally defective;
2. Condition R requires hidden extension-specific Core coupling equivalent to the changes it claims to avoid;
3. the distinction collapses to file placement or naming rather than semantic authority;
4. S2 or S3 cannot be physically or architecturally justified as an ordinary extension;
5. the evidence dependency mapping cannot justify different re-execution scope without subjective exceptions; or
6. S4 demonstrates that the boundary systematically blocks necessary Core evolution.

Under those outcomes, the research shall preserve the negative result and narrow the contribution claim rather than redesigning the metrics after the fact.

---

## 16. Current Status

This plan pre-registers the consequence-testing design only.

No consequence hypothesis has yet been tested.

Current classification:

| Item | Status |
|---|---|
| RQ-ISO-001 candidate gap | Evidence-supported by bounded survey |
| Consequence-test design | Draft / pre-registered |
| H-ISO-01 | Untested |
| H-ISO-02 | Untested |
| H-ISO-03 | Untested |
| S4 boundary-validity test | Untested |
| Novelty | Not established |
| Framework Specification change | Not authorized by this plan |
