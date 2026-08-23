# RQ-FCI-001 Definition v0.1

Status: **Defined — Evidence Survey Required**  
Research Question: **RQ-FCI-001 — Thermodynamic Formulation Change Isolation**  
Date: **2026-08-23**  
Tracking: GitHub Issue #108  
Post-specification baseline: `8ebe1cf8dedc6b2c7d9d755ad200159ad0297202`

---

## 1. Purpose

This document defines the third focused ThermoCore research question after completion of RQ-ISO-001 and RQ-EFM-001.

RQ-FCI-001 investigates whether a change in the thermodynamic formulation used by a conforming implementation can remain **architecturally contained**: formulation-specific state coordinates, closure/recovery relations, material parameters, and Thermodynamic Computation implementation may change, while Framework-level architecture, ownership, information-category semantics, Framework Interface semantics, Material Representation responsibility, Extension governance, and Framework Conformance semantics remain unchanged.

The question is intentionally narrower than generic thermodynamic state-variable selection or solver modularity. Those topics already have substantial prior art and shall not be treated as novel by this research line.

This document is non-normative. It does not modify the Framework Specification, production implementation, Verification, Validation, Performance, or ThermoCore v1.0.0.

---

## 2. Primary Research Question

> **For a fixed declared thermodynamic scope, when one valid thermodynamic formulation is replaced by another valid formulation with different solved/represented quantities, state schema, basis, or closure, can the resulting changes remain contained within formulation-specific implementation/specification artifacts without changing ThermoCore Framework architecture, ownership, information-category semantics, Framework Interface semantics, Material Representation responsibility, Extension governance, or Conformance semantics? What changes falsify that containment and require Framework-level revision or scope reclassification?**

The question therefore concerns **change isolation**, not whether one formulation is physically or numerically superior.

---

## 3. Background and Current Baseline

### 3.1 Framework neutrality already specified

The current Framework Specification does not prescribe which numerical variables constitute Thermodynamic State. `Thermodynamic_State.md` defines Persistent and Derived State semantically and leaves the specific assignment formulation-relative.

The current bounded reference formulation is one implementation/specification profile using specific enthalpy as Persistent Thermodynamic State. It is not the Framework definition of thermodynamic state.

### 3.2 Existing formulation research

The completed thermodynamic-formulation research line already established bounded decisions for the current reference branch, including:

- enthalpy versus internal-energy choice as a formulation-level decision;
- specific versus volumetric energy basis;
- reference-density and energy-datum conventions;
- temperature and phase recovery closure;
- explicit separation between reference-formulation decisions and Framework architecture.

Those results support the plausibility of formulation locality but did not evaluate architectural change containment as a focused research question.

### 3.3 Relationship to RQ-EFM-001

RQ-EFM-001 asks whether a selected formulation is thermodynamically complete enough for a mechanism or external physical-domain participation to remain Core-preserving.

RQ-FCI-001 assumes a formulation has already passed the applicable admissibility boundary and asks whether replacing that formulation with another formulation of the **same declared thermodynamic scope** can remain a formulation-local change.

If a proposed “formulation change” actually enlarges governing physical scope, RQ-EFM-001 remains the applicable boundary and RQ-FCI-001 shall not relabel the change as ordinary substitution.

### 3.4 Relationship to RQ-ISO-001

RQ-ISO-001 governs state authority and non-promotion after architectural categories are accepted.

RQ-FCI-001 does not change those ownership rules. It tests whether a formulation substitution can preserve them even when the concrete Persistent/Derived State schema changes.

---

## 4. Research Scope

RQ-FCI-001 is limited to thermodynamic-framework architecture.

It evaluates whether formulation changes can remain local with respect to:

1. Framework architecture and component responsibilities;
2. Thermodynamic State semantics and ownership;
3. Persistent versus Derived State assignment;
4. Framework Interface semantics;
5. Material Representation responsibility;
6. Extension admissibility and extension governance;
7. Framework Conformance semantics;
8. formulation-specific implementation, parameters, and recovery/closure artifacts.

The research does not require implementation files to remain identical. A formulation change is expected to modify formulation-specific code and data.

The architectural question is whether those changes propagate into **Framework concepts and responsibilities**.

---

## 5. Candidate Change Dimensions — Under Survey

The following dimensions are research targets and are not yet classified as safely local in all cases.

### FCI-D1 — Solved / represented thermodynamic coordinate

Examples include:

- specific enthalpy;
- specific internal energy;
- temperature in a scope where temperature is a sufficient primary coordinate;
- another valid thermodynamic coordinate selected by a bounded formulation.

### FCI-D2 — Persistent / Derived State assignment

A formulation may change which quantities must persist across updates while preserving the Framework-level identity of Persistent versus Derived State.

### FCI-D3 — Energy basis

Examples include:

- specific energy;
- volumetric energy;
- total per-cell energy.

A basis change may require normalization or geometry/material information without necessarily changing Framework semantics.

### FCI-D4 — Reference convention

Examples include:

- reference-energy datum;
- reference temperature;
- density/reference-condition convention.

### FCI-D5 — Recovery / closure relation

Examples include:

- direct algebraic recovery;
- piecewise phase recovery;
- monotonic inversion;
- equivalent bounded constitutive closure.

### FCI-D6 — Material parameterization

Different formulations may require different material parameters or compiled runtime material data while preserving Material Definition as Configuration and Thermodynamic Computation as the State Evolution responsibility.

---

## 6. Preliminary Change-Containment Model — Under Survey

RQ-FCI-001 will distinguish two levels of change.

### 6.1 Formulation-local change

A change is a candidate **formulation-local change** when all of the following remain unchanged:

- Framework Core component set;
- component responsibilities;
- ownership assignments;
- Runtime State / Configuration / Representation information-category semantics;
- Framework Interface relationship semantics;
- Material Representation responsibility;
- ordinary-extension admissibility semantics;
- Framework Conformance categories and meaning.

The following may change without automatically violating locality:

- concrete Persistent State schema;
- specific solved coordinate;
- Derived State recovery;
- numerical formulae;
- material parameter set;
- source normalization;
- implementation classes/functions;
- reference-formulation specification and Verification/Validation obligations.

### 6.2 Framework-level change

A change is a candidate **Framework-level change** when correct interpretation requires altering one or more authoritative Framework concepts, such as:

- adding/removing/reassigning a Framework Core responsibility;
- changing ownership of Thermodynamic State or Representation;
- redefining Runtime State / Configuration / Representation;
- changing Framework Interface semantics rather than only communicated information;
- changing Material Representation responsibility;
- weakening or changing ordinary-extension admissibility;
- changing Framework Conformance semantics.

A change in concrete state quantity count alone is not sufficient to establish Framework-level change.

---

## 7. Initial Formulation-Pair Families — Under Survey

These families are evidence-search and later evaluation candidates. None is pre-classified as a research result.

### FCI-P0 — Single-phase coordinate substitution control

Use the same bounded caloric physical scope with two bijectively related state-coordinate formulations, for example:

- energy-coordinate formulation; and
- temperature-coordinate formulation under conditions where temperature uniquely determines the thermodynamic condition.

Purpose:

- provide a clean equivalent-scope substitution control;
- test whether implementation change can occur without Framework change.

### FCI-P1 — Energy-basis substitution

Compare the same bounded formulation under specific, volumetric, or total-cell energy bookkeeping where geometry/mass relations provide exact conversion.

Purpose:

- separate state semantics from unit/basis representation;
- test source-normalization and material-data impact.

### FCI-P2 — Equivalent phase-change state-coordinate formulations

Investigate whether two valid bounded formulations can represent the same equilibrium phase-change scope with different primary-state choices while preserving equivalent thermodynamic observables and Framework-level responsibilities.

Potential candidates may include an enthalpy-primary profile and an alternative sufficient state-coordinate profile.

This family requires careful evidence review before selection because an alternative profile must not introduce hidden history dependence or enlarged physical scope.

### FCI-P3 — Closure substitution

Compare alternative valid recovery/closure relations within a deliberately fixed physical scope.

Purpose:

- determine whether closure replacement remains formulation-local when Framework State/ownership semantics are preserved.

### FCI-B1 — Scope-expansion boundary control

Use a case in which the “new formulation” actually introduces governing physics absent from the original scope, such as pressure/compressibility, variable mass, reactive transport, or another additional governing responsibility.

Purpose:

- ensure RQ-FCI-001 does not falsely classify physical-scope expansion as harmless formulation substitution;
- preserve the RQ-EFM-001 admissibility boundary.

---

## 8. Evidence Search Plan

The first evidence pass shall search for architectures and thermodynamic software that explicitly support interchangeable or selectable formulations/state variables while preserving higher-level component responsibilities.

Priority evidence families include:

- Modelica.Media state selection and medium formulations;
- OpenFOAM thermophysical / energy-variable selection and thermo packages;
- MOOSE variable/material/constitutive organization where alternative primary variables are used;
- finite-element / constitutive frameworks that separate state-variable choice from outer solver architecture;
- multiphysics frameworks that expose thermodynamic package substitution without redefining global architecture;
- thermodynamic software or standards distinguishing independent-state choice from system architecture.

The survey shall record both positive and negative evidence:

- what actually remains stable across formulation choices;
- which interfaces/contracts change;
- whether state-coordinate choice leaks into consumers or coupled modules;
- whether alternative formulations require architecture-specific adapters;
- whether a true scope change is explicitly separated from state-variable substitution.

The survey shall not infer architecture from class names alone.

---

## 9. Preliminary Falsification Conditions

RQ-FCI-001 shall be narrowed, reclassified, or rejected if evidence or later testing shows any of the following.

### FCI-F1 — Framework neutrality is only documentary

If every meaningful formulation substitution requires changing Framework-level responsibilities or semantic contracts, the claimed change-isolation boundary is not supported.

### FCI-F2 — Equivalent-scope substitution cannot be defined

If candidate formulation pairs cannot be shown to represent the same declared physical scope, they cannot support a change-isolation comparison.

### FCI-F3 — Material Representation responsibility must change

If equivalent formulation substitution necessarily transfers or changes the architectural responsibility of Material Representation rather than only changing its formulation-specific interpretation path, Framework invariance is falsified for that case.

### FCI-F4 — Interface semantics must change

If an equivalent formulation substitution requires new Framework Interface semantics rather than merely different permitted communicated information, containment is falsified for that case.

### FCI-F5 — Scope expansion is required for the apparent result

If a positive or negative result depends on adding pressure, transport, reaction, mechanics, or another governing domain not present in the baseline scope, the result must be reclassified under scope/admissibility analysis rather than treated as formulation substitution.

### FCI-F6 — Contribution collapses to established software abstraction

If prior art already directly operationalizes the complete architecture-level change-containment rule with equivalent scope, ownership preservation, interface-semantic stability, and explicit scope-change boundaries, RQ-FCI-001 shall not claim a distinct Research Gap.

---

## 10. Claims Not Supported at Definition Stage

RQ-FCI-001 does not currently support claims that:

- ThermoCore is the first framework to support alternative thermodynamic state variables;
- formulation substitution is novel;
- all thermodynamic formulations are interchangeable;
- state-coordinate choice never affects interfaces or representation code;
- enthalpy is universally preferable;
- Framework architecture is already proven invariant across formulations;
- a change in state schema is always harmless;
- a formulation change never requires new Verification or Validation;
- ThermoCore is universally solver-independent or formulation-independent.

These remain prohibited until bounded evidence supports a narrower statement.

---

## 11. Expected Research Sequence

RQ-FCI-001 shall follow:

```text
Definition
  -> bounded prior-art / evidence survey
  -> Evidence Matrix
  -> Research Gap Analysis
  -> pre-registered consequence / substitution test if justified
  -> implementation-neutral or isolated research harness
  -> final bounded disposition
  -> optional later Framework Specification consideration
```

No Framework Specification change is authorized by this definition.

---

## 12. Definition-Stage Decision

**RQ-FCI-001 is defined and ready for a bounded evidence survey.**

The first evidence task shall focus on whether existing thermodynamic and simulation frameworks already provide explicit architecture-level change isolation across alternative thermodynamic formulations, and which parts of formulation substitution are established prior art versus still unresolved at the reusable-framework level.
