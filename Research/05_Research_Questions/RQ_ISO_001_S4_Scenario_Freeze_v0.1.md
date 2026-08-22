# RQ-ISO-001 S4 Scenario Freeze v0.1

Status: Frozen Pre-execution Boundary Counterexample  
Research Question: RQ-ISO-001  
Scenario: S4 — Variable-Mass Compressible Reactive Flow  
Tracking: GitHub Issue #79  
Frozen comparison baseline: `8e3a948b0f36feefd313de1f03dd4db29b3bc465`

---

## 1. Purpose

This artifact freezes the S4 falsification-oriented boundary-validity scenario before executable classification.

S4 is deliberately **not** an ordinary extension. It introduces governing physics that violate the bounded reference formulation assumptions used by S1-S3. The test asks whether the restricted semantic/Core-state boundary recognizes that ordinary extension isolation is no longer sufficient.

A valid S4 outcome is therefore not `zero Core change`. A valid outcome must explicitly require a Core revision or classify the mechanism as outside the current Core scope.

S4 shall not be used to increase, rescue, or manufacture support for H-ISO-01, H-ISO-02, or H-ISO-03. Those ordinary-extension hypotheses were decided from S1-S3 under their pre-registered rules.

---

## 2. Frozen S4 Mechanism

S4 requires all of the following simultaneously:

1. **Variable mass** within a simulation element due to transport.
2. **Density evolution / compressibility** rather than a fixed reference density.
3. **Momentum / velocity evolution** sufficient to represent advective transport.
4. **Reactive composition / species state** whose transport and reaction affect local behavior.
5. **Pressure or an equivalent thermodynamic closure variable** coupled to density/composition/energy.
6. **Flow-dependent energy transport**, not merely an externally supplied local additive heat source.

This definition may not be weakened after execution begins.

In particular, S4 may not be converted into a fixed-mass reaction-source case; that case is already represented by S3.

---

## 3. Frozen Current-Core Assumptions Checked by S4

The S4 classifier shall check the following frozen baseline assumptions:

- the bounded reference formulation uses fixed per-cell mass;
- reference density is fixed for the bounded formulation;
- Persistent Thermodynamic State contains only `SpecificEnthalpy : double`;
- Temperature and phase fraction are recovered as Derived State;
- the reference state-evolution path applies mapped specific-enthalpy increments;
- no authoritative momentum/velocity state exists in the current Core;
- no authoritative species/composition transport state exists in the current Core;
- no pressure evolution or compressible-flow closure exists in the current Core; and
- no flow-dependent mass/species/energy transport responsibility exists in the current Core.

S4 is expected to contradict multiple assumptions in this set.

---

## 4. Frozen Information-Sufficiency Counterexample

The test includes two machine-checkable insufficiency demonstrations.

### 4.1 Variable-mass energy-content ambiguity

For a state carrying only specific enthalpy `h`, total enthalpy content depends on mass:

```text
H_total = m * h
```

The frozen numerical example is:

```text
State A: m = 1 kg, h = 100 J/kg  -> H_total = 100 J
State B: m = 2 kg, h = 100 J/kg  -> H_total = 200 J
```

`h` is identical while total energy content differs. A variable-mass transport formulation therefore cannot be represented completely by evolving only the existing `SpecificEnthalpy` coordinate using local additive energy increments; mass transport and the energy carried by that transport require governing state/evolution outside the current bounded formulation.

The example does not claim that `m*h` is a complete compressible-flow energy equation. It is a minimal demonstration that fixed-mass specific-enthalpy state is insufficient once mass changes.

### 4.2 Same-Core-State / different-flow-state ambiguity

The classifier shall construct two S4 states having identical `SpecificEnthalpy` but different density, velocity, and composition:

```text
S4-A: h = 100 J/kg, rho = 1 kg/m^3, velocity = 0 m/s, species fraction = 0.1
S4-B: h = 100 J/kg, rho = 2 kg/m^3, velocity = 10 m/s, species fraction = 0.9
```

The current Core projection maps both to the same Persistent Thermodynamic State because only `h` is retained. Yet the S4 states are not equivalent for compressible reactive-flow evolution.

Therefore the additional quantities are not merely presentation data or optional local history under this scenario; they participate in governing evolution.

---

## 5. Invalid Extension-Only Interpretation

The following interpretation is frozen as the falsification target:

> Keep all current Core semantics, state identity, governing responsibilities, and interfaces unchanged; place mass/density, momentum/velocity, species/composition, pressure/closure, and flow-transport state entirely in an ordinary extension; report zero Core impact.

This interpretation shall be classified invalid if the extension-owned quantities become authoritative governing state required to determine the next physical state while the current Core still claims complete authoritative thermodynamic state-evolution responsibility for the mechanism.

A generic container or adapter does not avoid the problem if it merely hides S4-specific governing semantics outside the declared Core authority.

---

## 6. Valid Condition R Disposition

For S4, Condition R is valid only if it produces one of the following dispositions:

```text
CORE REVISION REQUIRED
```

or

```text
OUT OF CURRENT CORE SCOPE
```

For this v0.1 experiment, the primary expected disposition is:

```text
CORE REVISION REQUIRED FOR AN IN-SCOPE S4 FORMULATION
```

because supporting S4 as part of a future in-scope formulation would require new authoritative state and governing responsibilities.

This expected disposition is frozen before execution and is not counted as support for H-ISO-01/02/03.

---

## 7. Minimum Core-Revision Categories

Without designing a CFD solver, the classifier shall require at least these categories for an in-scope future S4 formulation:

1. **State semantic revision** — the authoritative state can no longer be specific-enthalpy-only.
2. **State schema expansion or new authoritative state category** — mass/density, momentum/velocity, and reactive composition require authoritative representation.
3. **Governing formulation revision** — fixed-mass / constant-reference-density assumptions are invalid.
4. **Core responsibility revision** — transport of mass, momentum, species, and energy requires governing evolution responsibility beyond local source addition.
5. **Interface revision** — governing fluxes/state exchange cannot be represented solely as the existing bounded local energy-input contract.
6. **Verification revision** — changed state/evolution semantics require new or re-executed Core Verification.
7. **Validation expansion** — existing bounded H2O/Gallium caloric evidence does not validate compressible reactive-flow behavior.

These are categories, not a proposed implementation architecture.

---

## 8. Evidence-Impact Rule

The Phase A dependency rules remain authoritative.

For a future implementation that actually performs the required Core revision:

- Core Verification must be reconsidered and re-executed for changed state/evolution responsibilities;
- existing H2O/Gallium caloric Validation may remain historical evidence for the unchanged bounded caloric submodel only if that submodel remains intact;
- those caloric records shall **not** be generalized into Validation of S4;
- new Validation Evidence would be required for any new compressible/reactive/transport claims; and
- Framework Conformance and physical Validation remain separate questions.

The S4 research classifier itself does not alter the published Framework or historical Validation records.

---

## 9. Final Decision Rule

S4 receives:

### `BOUNDARY VALID`

if all of the following hold:

- the classifier identifies the current bounded assumptions violated by S4;
- the zero-Core-change ordinary-extension interpretation is rejected;
- Condition R explicitly requires Core revision or an out-of-scope disposition;
- governing state/responsibility is not hidden in extension infrastructure; and
- S1-S3 ordinary-extension hypothesis results are left unchanged.

### `BOUNDARY INVALID`

if Condition R accepts S4 as an ordinary extension with zero Core impact while the required governing state/responsibility is merely hidden outside Core authority.

### `RECLASSIFICATION REQUIRED`

if the frozen S4 definition is internally inconsistent or no longer constitutes the intended variable-mass compressible reactive-flow counterexample.

No other verdict is permitted in v0.1.
