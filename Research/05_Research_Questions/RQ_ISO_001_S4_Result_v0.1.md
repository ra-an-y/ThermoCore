# RQ-ISO-001 S4 Result v0.1

Status: COMPLETED — boundary-validity counterexample passed  
Research Question: RQ-ISO-001  
Scenario: S4 — Variable-Mass Compressible Reactive Flow  
Tracking: GitHub Issue #79  
Protocol: `RQ_ISO_001_Consequence_Test_Plan_v0.1.md`  
Scenario freeze: `RQ_ISO_001_S4_Scenario_Freeze_v0.1.md`  
Frozen comparison baseline: `8e3a948b0f36feefd313de1f03dd4db29b3bc465`

---

## 1. Purpose

S4 is the falsification-oriented boundary-validity stage of RQ-ISO-001.

Unlike S2 and S3, S4 is intentionally not an ordinary extension. It introduces variable mass, compressibility, reactive composition, momentum-dependent transport, pressure/closure requirements, and flow-dependent energy transport.

The test asks whether the restricted semantic/Core-state boundary correctly recognizes that these governing changes cannot be hidden behind extension-local state while reporting zero Core impact.

S4 is not used to increase the support level of H-ISO-01, H-ISO-02, or H-ISO-03. Those hypotheses were already classified from S1-S3 using the pre-registered ordinary-extension decision rules.

---

## 2. Frozen S4 Definition Used

The pre-execution freeze required all of the following simultaneously:

- variable element mass;
- density evolution / compressibility;
- momentum or velocity evolution;
- reactive composition / species state;
- pressure or equivalent thermodynamic closure coupled to density/composition/energy; and
- flow-dependent energy transport.

The scenario was not weakened into a fixed-mass local reaction-source problem after execution began.

That distinction is essential because the bounded fixed-mass reaction-source case was already evaluated as S3.

---

## 3. Current-Core Assumptions Tested

The classifier tested the S4 requirements against the frozen baseline and recorded eight contradictions:

1. fixed per-cell mass;
2. constant reference density;
3. specific-enthalpy-only Persistent Thermodynamic State;
4. no authoritative momentum state;
5. no authoritative species-transport state;
6. no pressure evolution;
7. local specific-enthalpy increment evolution as the bounded energy-update mechanism; and
8. no Core responsibility for flow-dependent mass/species/energy transport.

Observed result:

```text
S4_ASSUMPTION_CONTRADICTIONS=8
```

This count is descriptive of the frozen S4 classifier. It is not a general count of all changes required by every possible compressible-flow formulation.

---

## 4. Information-Sufficiency Tests

### 4.1 Variable-mass energy-content ambiguity

The frozen example compared:

```text
A: m = 1 kg, h = 100 J/kg -> m*h = 100 J
B: m = 2 kg, h = 100 J/kg -> m*h = 200 J
```

Both states have the same `SpecificEnthalpy`, but the enthalpy content associated with the different masses is not the same.

Observed result:

```text
S4_VARIABLE_MASS_ENERGY_AMBIGUITY=CONFIRMED
```

This is not presented as a complete compressible-flow energy equation. Its role is narrower: once mass is allowed to change, a persistent coordinate containing only `h` is insufficient to describe the governing mass-energy evolution by itself.

### 4.2 Same Core state, different flow state

The classifier also constructed two S4 states with identical `SpecificEnthalpy = 100 J/kg` but different density, velocity, composition, and mass.

The current frozen Core projects both to the same one-quantity Persistent Thermodynamic State even though they are not equivalent states for compressible reactive-flow evolution.

Observed result:

```text
S4_SAME_CORE_STATE_DIFFERENT_FLOW_STATE=CONFIRMED
```

Therefore the additional S4 quantities cannot be treated merely as presentation data or bounded mechanism-local history while still claiming that the unchanged current Core completely represents the governing S4 state.

---

## 5. Extension-only Zero-Core-change Falsification Test

The falsification target was explicitly frozen as:

> Keep all current Core semantics, state identity, governing responsibilities, and interfaces unchanged; move mass/density, momentum/velocity, species/composition, pressure/closure, and flow-transport state into an ordinary extension; report zero Core impact.

The classifier rejected this interpretation.

Observed result:

```text
S4_EXTENSION_ONLY_ZERO_CORE_CHANGE=REJECTED
```

Reason:

The S4 quantities participate directly in governing evolution. If they were made authoritative only inside an ordinary extension while the Core retained the existing fixed-mass specific-enthalpy-only governing contract, the architecture would no longer be preserving the declared boundary; it would be hiding new governing authority outside the Core merely to avoid admitting a Core change.

This is exactly the behavior S4 was designed to detect.

---

## 6. Required Disposition

For an in-scope future S4 formulation, the classifier required:

```text
S4_REQUIRED_DISPOSITION=CORE_REVISION_REQUIRED
```

This does **not** mean the current ThermoCore release supports variable-mass compressible reactive flow.

It means that if such a mechanism were brought into the Framework's in-scope governing physics, the current bounded formulation/Core contract could not remain unchanged and still represent the mechanism correctly.

The alternative valid project-level disposition would be to keep the mechanism explicitly outside the current Core scope.

---

## 7. Minimum Core-revision Categories

The frozen classifier required seven categories of change for an in-scope S4 formulation:

1. `STATE_SEMANTIC_REVISION`
2. `STATE_SCHEMA_OR_AUTHORITY_EXPANSION`
3. `GOVERNING_FORMULATION_REVISION`
4. `CORE_RESPONSIBILITY_REVISION`
5. `INTERFACE_REVISION`
6. `VERIFICATION_REVISION`
7. `VALIDATION_EXPANSION`

Observed result:

```text
S4_REQUIRED_REVISION_CATEGORIES=7
```

These categories intentionally stop short of proposing a CFD architecture. S4 does not determine whether a future implementation should use conservative variables, primitive variables, separate mechanical state, coupled solvers, or another formulation.

Its conclusion is only that the current ordinary-extension boundary is insufficient for this counterexample.

---

## 8. Evidence Impact

S4 is not scored as another H-ISO-03 ordinary-extension comparison.

The frozen Phase A dependency rules imply the following if an S4-capable Core were actually implemented:

- changed authoritative state/evolution semantics would require new and/or re-executed Core Verification;
- a changed governing formulation could require new Verification beyond the current reference suite;
- H2O and Gallium caloric Validation remain historical evidence for the bounded reference formulation to which they apply;
- those caloric records do not become Validation of compressible reactive flow merely because an S4 implementation reuses some thermodynamic submodel; and
- new Validation Evidence would be required for any new mass/momentum/species/pressure/transport claims.

Therefore the correct evidence disposition is neither `all retained as proof of S4` nor `all historical evidence invalidated`.

The bounded caloric evidence remains bounded to its original claim, while S4 requires a new evidence scope for new governing physics.

---

## 9. Execution Evidence

Research-only executable classifier:

`Research/05_Research_Questions/Execution/RQ_ISO_001_S4/`

GitHub Actions workflow:

`RQ-ISO-001 S4`

### Initial failed execution

Run:

`32589524339` — run #1 — `failure`

The failure was a C# harness compilation defect caused by incorrect named-argument casing for `AssumptionCheck` record-constructor parameters.

This failed run is retained as part of the execution record. The correction changed only the research harness source syntax. It did not modify:

- the frozen S4 scenario;
- any S4 physical assumption;
- the decision rule;
- the expected disposition;
- the set of required revision categories; or
- any S1-S3 result.

### Successful execution

Source-head commit:

`905800672b284fea4024dd442c46a4e588d14267`

Run:

`32589577444` — run #2 — `success`

Observed output:

```text
RQ-ISO-001 S4 Variable-Mass Compressible Reactive-Flow Boundary Test
Frozen comparison baseline: 8e3a948b0f36feefd313de1f03dd4db29b3bc465
S4_CURRENT_CORE_STATE_QUANTITIES=1
S4_VARIABLE_MASS_ENERGY_AMBIGUITY=CONFIRMED
S4_SAME_CORE_STATE_DIFFERENT_FLOW_STATE=CONFIRMED
S4_ASSUMPTION_CONTRADICTIONS=8
S4_EXTENSION_ONLY_ZERO_CORE_CHANGE=REJECTED
S4_REQUIRED_DISPOSITION=CORE_REVISION_REQUIRED
S4_REQUIRED_REVISION_CATEGORIES=7
S4_BOUNDARY_VERDICT=BOUNDARY_VALID
S1_S3_HYPOTHESIS_RESULTS=UNCHANGED
```

---

## 10. Boundary-validity Verdict

The pre-registered S4 decision rule is satisfied:

- the current bounded assumptions violated by S4 were identified;
- the extension-only zero-Core-change interpretation was rejected;
- Core revision is explicitly required for an in-scope S4 formulation;
- governing state/responsibility is not hidden in an ordinary extension merely to preserve Core immutability; and
- S1-S3 hypothesis results remain unchanged.

Final S4 verdict:

```text
BOUNDARY VALID
```

This verdict is bounded to the evaluated counterexample. It does not establish that every future physical mechanism will always be classified correctly without further analysis.

---

## 11. Post-S4 RQ-ISO-001 Evaluation Disposition

The complete pre-registered consequence test now yields:

```text
S1 negative control:
NEUTRAL

S2 thermal hysteresis:
VALID DISCRIMINATING ORDINARY-EXTENSION RESULT

S3 bounded reaction heat:
VALID DISCRIMINATING ORDINARY-EXTENSION RESULT

H-ISO-01 State-growth Isolation:
SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS

H-ISO-02 Core-change Isolation:
SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS

H-ISO-03 Revalidation-scope Isolation:
SUPPORTED FOR EVALUATED ORDINARY-EXTENSION SCENARIOS

S4 boundary-validity counterexample:
BOUNDARY VALID
```

The supported research statement is therefore bounded to the evaluated scenarios:

> In the evaluated ThermoCore scenarios, enforcing a fixed semantic/Core-state boundary for ordinary extensions prevented extension-specific persistent quantities from becoming mandatory Core State, reduced the required Core semantic/interface change set, and reduced justified Core evidence re-execution scope relative to the permissive shared-state comparator, while the S4 counterexample correctly required Core revision when the governing physics exceeded the ordinary-extension boundary.

The following claims remain **not established** by this evaluation:

- novelty relative to all prior art;
- universal superiority over other architectures;
- reduced total persistent memory;
- universal reduction in implementation complexity;
- complete Framework Validation;
- Framework Conformance of arbitrary implementations;
- support for compressible reactive flow in the current ThermoCore Core; or
- a rule that the Core should never change.

RQ-ISO-001 may now proceed from consequence testing to a bounded research-contribution synthesis / final research-gap disposition without changing the frozen v1.0.0 Framework baseline.
