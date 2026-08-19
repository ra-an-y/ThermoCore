# Thermodynamic Formulation Research Gap Analysis

Version: 0.1  
Status: Completed Initial Formulation-Gap Analysis — Non-Normative  
Research line: Bounded fixed-grid thermodynamic reference formulation

---

## 1. Objective

This document analyzes the unresolved formulation gaps carried forward by `Thermodynamic_Formulation_Evidence_Matrix.md`.

The purpose is to determine, for each gap, whether it:

- can be closed by an explicit bounded modeling decision supported by the existing evidence;
- requires additional research before a reference-formulation specification can be authorized; or
- belongs to downstream verification / validation rather than pre-specification research.

This document does **not** modify the ThermoCore Framework Specification and does not itself define a thermodynamic formulation.

## 2. Evidence Baseline

The analysis depends on:

- `Research/01_Evidence_Matrix/Thermodynamic_Formulation_Evidence_Matrix.md`;
- the six source survey/comparison artifacts traced by that matrix;
- the frozen Framework semantics in `Documentation/Framework_Specification/Thermodynamic_State.md` and related Framework documents.

The governing research constraint remains:

> formulation decisions may narrow implementation physics without redefining Framework architecture, ownership, State semantics, Representation, or Interface semantics.

## 3. Gap Classification

| Classification | Meaning |
|---|---|
| **Close by bounded decision** | Existing evidence is sufficient to select a reference-formulation convention without claiming universal superiority |
| **Blocking research gap** | Additional formulation research is required before the reference-formulation specification can be authorized |
| **Downstream verification / validation** | The issue should be tested after the formulation is specified; it is not a reason to delay specification authorization once the formulation is complete |
| **Deferred broader formulation** | Valid but outside the minimal reference formulation |

## 4. TF-G01 — Enthalpy vs Internal Energy

### 4.1 Evidence position

The evidence establishes that:

- enthalpy and internal energy are physically distinct;
- internal energy aligns directly with a general first-law accumulation description;
- enthalpy has strong fixed-grid phase-change precedent in Ansys Fluent and the Voller phase-change literature;
- established thermophysical software can support either coordinate below a stable higher-level architecture;
- the current bounded candidate excludes mechanical work, mass transport, and variable-density volume-change physics.

The evidence does not establish a universal winner.

### 4.2 Bounded decision criterion

The reference formulation is intended primarily to provide a minimal fixed-grid solid/liquid phase-change implementation path, not a universal continuum-energy formulation.

For that scope, enthalpy has the stronger direct phase-change precedent because sensible and latent energy can be represented in one thermodynamic coordinate.

### 4.3 Gap disposition

**Disposition: Close by bounded decision.**

Candidate decision for the later reference formulation:

```text
primary energy-coordinate family = enthalpy
```

This is a reference-formulation choice only. It is not a Framework requirement and does not prohibit internal-energy formulations in other conforming implementations.

## 5. TF-G02 — Specific vs Volumetric Energy Coordinate

### 5.1 Evidence position

The evidence establishes that:

- OpenFOAM uses thermodynamic `he` as a specific quantity `[J/kg]`;
- conservative finite-volume accumulation may still use `rho * he`;
- heat capacity and latent heat data are commonly expressed on a mass-specific basis;
- volumetric source terms can be mapped to a specific energy update through density;
- under the current constant-`rho_ref` candidate, specific and volumetric energy are related by one fixed conversion factor.

### 5.2 Bounded decision criterion

A specific coordinate keeps the thermodynamic state aligned with common material-property and latent-heat units while leaving geometric source conversion at the formulation boundary.

Because `rho_ref` is constant in the bounded candidate, no additional dynamic density state is introduced by this choice.

### 5.3 Gap disposition

**Disposition: Close by bounded decision.**

Candidate decision:

```text
primary persistent energy basis = specific enthalpy [J/kg]
```

Volumetric conservative bookkeeping remains permitted as an implementation or equation-level representation derived through `rho_ref`.

## 6. TF-G03 — Exact Reference-Density Convention

### 6.1 Evidence position

The evidence establishes that:

- the bounded fixed-volume, fixed-mass candidate should use one density across solid/liquid phase change;
- a reference density must have explicit provenance and a declared reference condition;
- OpenFOAM provides precedent for using solid density as a reference density;
- COMSOL ties material-frame density to a reference geometry and reference condition.

### 6.2 What must be standardized

The formulation does not need one universal numerical density value. Density remains material configuration.

What must be standardized is the semantic contract:

```text
rho_ref = one constant material reference density [kg/m^3]
```

with:

```text
same rho_ref across the modeled solid/liquid transition
explicit provenance
explicit density-reference condition T_rho_ref
fixed cell mass = rho_ref * cell volume
```

### 6.3 Gap disposition

**Disposition: Close by bounded decision.**

The later formulation may recommend solid-phase density at a declared reference condition as the default material-data convention, but it should not claim that this is the only physically valid reference-density convention.

## 7. TF-G04 — Energy Datum Convention

### 7.1 Evidence position

The evidence establishes that:

- reference temperature and reference-energy offset are explicit thermophysical semantics in OpenFOAM, Fluent, and COMSOL;
- a common additive offset preserves nonreacting energy differences if all relations use the same datum;
- inconsistent phase offsets can corrupt latent-heat or phase-energy relationships;
- COMSOL provides precedent for a zero reference enthalpy at a declared reference condition.

### 7.2 Bounded decision criterion

For interoperability and dataset normalization, a zero reference is simpler than permitting arbitrary offsets in the reference formulation.

External datasets may still use different source datums, but they must be normalized before entering the formulation.

### 7.3 Gap disposition

**Disposition: Close by bounded decision.**

Candidate decision:

```text
h_ref = 0 J/kg at declared T_E_ref
```

with pressure reference included if required by the selected enthalpy definition, even though pressure evolution remains outside the bounded runtime model.

## 8. TF-G05 — Density and Energy Reference Temperatures

### 8.1 Evidence position

The evidence shows that:

```text
T_rho_ref
```

and

```text
T_E_ref
```

have distinct semantics.

They may be numerically equal for convenience, but the source semantics do not require them to be identical.

### 8.2 Gap disposition

**Disposition: Close by bounded decision.**

Candidate decision:

```text
retain T_rho_ref and T_E_ref as distinct formulation/configuration semantics
permit them to have the same numerical value
```

This avoids hiding density-provenance assumptions inside the thermodynamic energy datum.

## 9. TF-G06 — Temperature-Recovery Uniqueness and Stability

### 9.1 Evidence position

The evidence establishes that temperature recovery from enthalpy/internal energy is an established thermophysical operation, but does not establish that every candidate ThermoCore material relation will provide a unique and numerically stable inverse over all intended phase-change states.

This question depends on:

- the exact enthalpy-temperature relation;
- heat-capacity functions;
- latent-heat treatment;
- transition regularization;
- numerical inversion method.

### 9.2 Gap disposition

**Disposition: Blocking research gap.**

Before a concrete reference-formulation specification is authorized, the research must define a bounded enthalpy-to-temperature closure with an explicit uniqueness condition.

The research artifact should distinguish:

```text
physical relation
numerical regularization
inversion algorithm
```

and should identify conditions under which the inversion is single-valued.

A later implementation must then verify numerical stability; that implementation-level stability test is downstream Verification.

## 10. TF-G07 — Phase-Fraction Recovery Uniqueness

### 10.1 Evidence position

COMSOL and Fluent provide established phase-fraction/phase-transition relations, but the present ThermoCore research has not yet frozen one exact transition law.

The current Derived-State candidate for Phase Fraction is only valid when the phase relation is single-valued and history-independent.

### 10.2 Required research decision

The reference formulation needs an explicit bounded phase relation that specifies:

```text
transition temperature or interval
phase-fraction mapping
latent-heat coupling
behavior at interval boundaries
whether the transition width is physical, numerical, or explicitly modeled as a chosen regularization
```

The research must not silently treat a numerical smoothing width as measured physical solidus/liquidus evidence.

### 10.3 Gap disposition

**Disposition: Blocking research gap.**

A short focused comparison/selection of phase-transition closure is required before authorizing the reference-formulation specification.

Once a single-valued law is selected, Phase Fraction can remain a Derived-State candidate for the bounded reference formulation.

## 11. TF-G08 — Benchmark Energy and Recovery Consistency

### 11.1 Evidence position

The evidence matrix correctly identifies the need to demonstrate energy conservation and Temperature/Phase recovery consistency.

However these results cannot exist before the formulation and implementation are defined.

### 11.2 Gap disposition

**Disposition: Downstream verification / validation.**

TF-G08 is **not** a pre-specification blocker.

It should become a traceable requirement for later:

```text
formulation verification
implementation verification
physical / benchmark validation
```

The reference-formulation specification may define the invariants and expected relationships that later tests must evaluate, but it should not claim validation before those tests exist.

## 12. Gap Disposition Summary

| Gap | Disposition | Result |
|---|---|---|
| TF-G01 | Close by bounded decision | Select enthalpy family for the minimal reference formulation |
| TF-G02 | Close by bounded decision | Select specific enthalpy `[J/kg]` as persistent energy basis |
| TF-G03 | Close by bounded decision | Use one constant material `rho_ref` with explicit provenance/reference condition |
| TF-G04 | Close by bounded decision | Normalize reference enthalpy to zero at `T_E_ref` |
| TF-G05 | Close by bounded decision | Keep density and energy reference temperatures semantically distinct |
| TF-G06 | **Blocking research gap** | Need explicit unique enthalpy-to-temperature closure |
| TF-G07 | **Blocking research gap** | Need explicit single-valued phase-transition closure |
| TF-G08 | Downstream verification / validation | Carry forward as test/validation obligation |

## 13. Candidate Reference-Formulation Profile After Gap Closure

The evidence plus bounded decisions now support the following candidate profile:

```text
Geometry / mass:
  fixed cell volume
  fixed per-cell mass
  no mass transport
  no shrinkage / expansion

Density:
  rho_ref constant per material
  one rho_ref across solid/liquid phase change
  explicit T_rho_ref and provenance

Persistent Thermodynamic State candidate:
  specific enthalpy h [J/kg]

Energy datum:
  h_ref = 0 J/kg at T_E_ref

Derived Thermodynamic State candidates:
  Temperature T
  Phase Fraction phi

Still unresolved before specification authorization:
  exact h -> T closure
  exact T/h -> phi phase-transition closure
```

This remains a **research candidate** until TF-G06 and TF-G07 are resolved.

## 14. Framework Impact

The gap analysis finds no reason to modify the Framework Specification.

The candidate profile is compatible with the frozen Framework because:

- the Framework does not prescribe concrete physical state variables;
- Persistent/Derived classification remains formulation-relative;
- Material Definition remains Configuration;
- Energy Input remains Framework-level runtime information without universal physical units;
- source-unit conversion remains formulation-level;
- Thermodynamic Computation remains the only Core responsibility that evolves/writes Thermodynamic State.

```text
Framework Specification change: None
Framework Freeze reopen: No
```

## 15. Specification Authorization Assessment

### Current status

```text
Thermodynamic_Formulation.md authorization: NOT YET READY
```

Only two pre-specification blockers remain:

```text
TF-G06 — enthalpy-to-temperature closure
TF-G07 — phase-fraction / transition closure
```

All other gaps from the Evidence Matrix are either closed by bounded modeling decisions or correctly moved to downstream Verification / Validation.

## 16. Next Research Step

Perform one focused closure study covering TF-G06 and TF-G07 together:

> Define and compare bounded enthalpy–temperature–phase closure options for a fixed-density, fixed-grid solid/liquid phase-change reference formulation, and determine which option provides single-valued Temperature and Phase Fraction recovery without conflating physical transition width with numerical regularization.

That study should be the final research artifact required before deciding whether to authorize `Thermodynamic_Formulation.md`.

## 17. Current Decision

**The thermodynamic-formulation research gap has been reduced from eight open items to two pre-specification blockers.**

**Do not modify the Framework Specification.**

**Do not yet authorize `Thermodynamic_Formulation.md`.**

Proceed to focused closure of TF-G06 and TF-G07.