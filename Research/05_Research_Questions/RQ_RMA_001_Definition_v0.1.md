# RQ-RMA-001 — Runtime Material Abstraction Boundary

Version: 0.1  
Status: DEFINED — Evidence Survey Required  
Research Question: RQ-RMA-001 — Runtime Material Abstraction Boundary  
Tracking Issue: #135  
Date: 2026-08-23

---

## 1. Objective

Define a bounded research question concerning the semantic identity and authority boundary of **computation-ready material data** produced from reusable Material Definition Configuration.

The research shall determine whether transformed material data can remain Configuration across normalization, compilation, caching, lookup-table generation, backend specialization, or equivalent implementation transformations, and identify the conditions under which such data instead becomes evolving physical state, extension-local state, formulation-specific solver state, or evidence that the selected thermodynamic formulation/Core is incomplete.

This document is non-normative. It does not modify Framework Specification, implementation, Verification, Validation, Performance, or the frozen v1.0.0 release.

---

## 2. Motivation

The initial RQ-001 research-gap analysis identified **Runtime Material Abstraction** as an unresolved architectural boundary while also recognizing the broader use of runtime material abstractions as established practice.

The unresolved question was not whether material data may be transformed for runtime use. That is already common. The narrower question was whether a stable semantic boundary can exist between reusable Material Definition and material-independent Thermodynamic Computation without prescribing one storage layout, backend, object model, table format, or numerical formulation.

ThermoCore's current normative baseline establishes that:

- Material Definition is Configuration;
- Configuration is distinct from Runtime State and Representation;
- Thermodynamic Computation may read applicable material information;
- Material Definition does not own evolving Thermodynamic State;
- Framework Interfaces communicate configuration without reclassifying it;
- `Data_Flow.md` intentionally does not define how Material Definition is authored, stored, transformed, transported, or made available.

The current bounded reference implementation contains a concrete but non-normative example:

```text
ReferenceMaterialDefinition
        |
        v
ReferenceMaterialCompiler
        |
        v
CompiledThermodynamicParameters
```

`ReferenceMaterialCompiler` describes the output as normalized, computation-ready Configuration, and `CompiledThermodynamicParameters` explicitly states that it is not Thermodynamic State and not Material Representation.

This implementation choice shall not be used as novelty evidence. It is one candidate realization of the boundary being investigated.

---

## 3. Primary Research Question

> **Within a fixed declared ThermoCore thermodynamic scope, when reusable Material Definition Configuration is compiled, normalized, cached, specialized, or otherwise transformed into computation-ready material data, what semantic and authority conditions are sufficient for the transformed data to remain Configuration rather than becoming Thermodynamic State, Material Representation, extension-owned state, or formulation-specific solver state; and under what conditions does evolving, history-dependent, backend-specialized, or closure-critical material data cross those boundaries and require different ownership, state classification, formulation treatment, or explicit Framework/Core revision?**

The question is intentionally formulation-relative and implementation-agnostic.

It does not assume that a new normative information category called "runtime material representation" is required.

---

## 4. Relationship to Existing ThermoCore Results

### 4.1 RQ-ISO-001

RQ-ISO-001 governs **state authority and non-promotion** after an information category and ordinary-extension status are accepted.

RQ-RMA-001 shall route a case to RQ-ISO-001 when the key question becomes whether extension-local or transformed information is being promoted into mandatory authoritative Core State merely because it participates in computation.

RQ-RMA-001 shall not duplicate RQ-ISO-001.

### 4.2 RQ-EFM-001

RQ-EFM-001 governs **formulation-relative thermodynamic sufficiency** and ordinary/Core-preserving extension admissibility.

RQ-RMA-001 shall route a case to RQ-EFM-001 when a transformed or evolving material quantity becomes necessary to close the selected thermodynamic formulation or determine Thermodynamic State evolution uniquely.

RQ-RMA-001 shall not create a new category merely because a selected formulation is incomplete.

### 4.3 RQ-FCI-001, RQ-CEX-001, and RQ-ECA-001

These later research lines demonstrated that useful architectural properties may fail as independent contribution claims after direct-antecedent review or matched-scenario stress testing.

RQ-RMA-001 adopts the same falsification discipline.

If the surviving distinction is only a composition of established material-compilation practice plus existing ThermoCore Configuration/State ownership rules, the independent research claim shall be closed or reclassified rather than narrowed indefinitely.

---

## 5. Baseline Information Classes

RQ-RMA-001 begins from the existing ThermoCore information distinctions.

### 5.1 Material Definition Configuration

Reusable material information supplied for thermodynamic computation or material interpretation.

Material Definition is not Runtime State and does not own evolving Thermodynamic State.

### 5.2 Computation-Ready Transformed Material Data

A provisional research term for data derived from Material Definition and prepared for direct computation.

Examples may include:

- normalized constants;
- precomputed thresholds;
- interpolation coefficients;
- lookup tables;
- packed records;
- backend-specific buffers;
- generated evaluators;
- equivalent computation-ready encodings.

This term is **not** a new normative information category.

Its semantic classification is the subject of the research.

### 5.3 Thermodynamic State

Authoritative Runtime State governed by the Framework and written through Thermodynamic Computation.

### 5.4 Extension-Local State

Persistent evolving information owned by an admitted Extension Module and kept distinct from authoritative Thermodynamic State unless a Core/formulation revision is justified.

### 5.5 Material Representation

Downstream interpretation of Thermodynamic State and applicable Material Definition for Representation Consumers.

Computation-ready material data shall not be called Material Representation merely because it is a representation in the ordinary programming sense.

### 5.6 Numerical Cache / Solver Workspace

Transient or persistent implementation data that accelerates or supports numerical execution without acquiring independent physical authority.

A cache may persist across calls without becoming Runtime State if its semantic content is entirely reconstructible from authoritative inputs and its value does not represent independent physical memory.

This proposition remains Under Survey and must be tested against direct prior art.

---

## 6. Candidate Semantic Dimensions

All dimensions in this section are **Under Survey**.

### RMA-D1 — Source Identity

Question:

> Is the transformed data wholly determined by a declared Material Definition plus declared transformation rules, or does it contain information not present in that source relationship?

Candidate distinction:

```text
reconstructible from Configuration
    versus
contains independent evolving information
```

Reconstructibility alone may not be sufficient to establish Configuration status, but it is a candidate discriminator.

### RMA-D2 — Semantic Mutability

Question:

> Does a value change only because Configuration was changed/recompiled, or can it evolve because simulated physical history evolved?

Candidate distinction:

- configuration mutation / recompilation;
- physical state evolution.

Runtime mutation by itself is not enough to classify data as Runtime State. Cache invalidation and backend rebuilding may occur at runtime without representing physical evolution.

### RMA-D3 — Authority

Question:

> Does transformed material data merely encode authoritative Material Definition semantics, or can it redefine material meaning independently?

A second independent source of material truth would create an authority conflict even if both objects are called Configuration.

### RMA-D4 — Thermodynamic Closure Role

Question:

> Is the transformed quantity a parameter/evaluator used with already sufficient Thermodynamic State, or is it an evolving coordinate required to close the selected thermodynamic formulation?

If an evolving quantity is necessary for closure or unique update, RQ-EFM-001 becomes the governing boundary.

### RMA-D5 — Lifecycle and Invalidation

Question:

> Can computation-ready data be built, cached, invalidated, and rebuilt while preserving Configuration semantics?

Candidate factors include:

- explicit source version/configuration identity;
- deterministic rebuild from authoritative inputs;
- invalidation after material-definition change;
- no independent physical history stored only in the cache.

This dimension shall not prescribe cache architecture or invalidation mechanisms.

### RMA-D6 — Backend Specialization

Question:

> Can CPU/GPU/SIMD/table/function/backend-specific encodings remain semantically equivalent Configuration even when their memory layouts and access mechanisms differ?

The candidate premise is that semantic identity shall not depend on storage layout, API, memory location, device, or code-generation technique.

### RMA-D7 — Formulation Specificity

Question:

> Can a computation-ready material artifact be specific to one thermodynamic formulation while still remaining Configuration, provided that it is explicitly derived from Material Definition for that formulation?

A formulation-specific artifact is not automatically Framework-level material truth.

RQ-RMA-001 shall distinguish:

```text
Framework-level Material Definition
        versus
formulation-specific compiled Configuration
```

without assuming either must own the other.

### RMA-D8 — Ownership and Write Responsibility

Question:

> Which responsibility may create, rebuild, or replace computation-ready material data, and what semantic meaning does that write carry?

Candidate possibilities include:

- compiler/adapter responsibility derives Configuration;
- backend cache rebuilds an equivalent encoding;
- extension owns extension-specific configuration transformation;
- Thermodynamic Computation consumes but does not acquire Material Definition authority.

This dimension shall remain conceptual and shall not prescribe an API or class hierarchy.

---

## 7. Candidate Boundary Tests

The following tests are provisional and shall be evaluated by evidence survey before any pre-registered consequence testing.

### Test R — Reconstructibility

Given the same authoritative Material Definition, declared formulation/configuration inputs, and declared transformation semantics, can the computation-ready artifact be reconstructed without knowledge of evolving physical history?

If no, the artifact may contain Runtime State or extension-local state.

A positive result does not by itself prove Configuration status.

### Test A — Authority Preservation

Can the transformed artifact be replaced by another semantically equivalent encoding derived from the same authoritative Material Definition without changing material meaning?

If no because the transformed artifact has become the only authority for material meaning, an authority boundary may have been crossed.

### Test C — Closure Dependence

Holding authoritative Thermodynamic State, applicable Material Definition, declared exchanges, and all admitted local state fixed, would omission of an allegedly "configuration-only" evolving quantity make the selected thermodynamic closure ambiguous or the required next state non-unique?

If yes, route to RQ-EFM-001 rather than treating the issue solely as material compilation.

### Test H — Physical History

Can two physically different histories yield different values of the artifact while all authoritative Configuration inputs are identical?

If yes, the differing information is a candidate physical memory/state quantity rather than pure transformed Configuration.

### Test B — Backend Equivalence

Can two backend-specific encodings differ in layout/precision organization/access path while representing the same declared material semantics and producing equivalent formulation inputs within the applicable implementation contract?

If yes, backend specialization alone should not imply semantic reclassification.

Numerical accuracy/performance remains a separate concern.

---

## 8. Candidate Matched Scenarios

These scenarios are survey guides, not pre-registered experiments.

### RMA-S0 — Pure normalization control

Material Definition stores author-facing values; a compiler performs unit normalization and packs them into a computation-ready record.

Expected pressure:

```text
same material meaning
no physical history
reconstructible from Configuration
```

Candidate classification: transformed Configuration.

### RMA-S1 — Precomputed derived constants

The compiler derives transition enthalpy thresholds or interpolation coefficients from Material Definition and the selected formulation.

The values are not independently authored and can be rebuilt deterministically.

Purpose: test whether formulation-specific derived constants remain Configuration.

### RMA-S2 — Backend-specific packed representation

The same Material Definition is transformed into:

- CPU objects;
- SIMD-aligned arrays;
- GPU buffers;
- lookup tables.

Purpose: test whether backend-specific structure is semantically irrelevant to information classification.

### RMA-S3 — Runtime cache invalidation after Configuration change

A user or application changes an allowed Material Definition value; the compiled artifact is invalidated and rebuilt during runtime operation.

Purpose: distinguish runtime lifecycle from Runtime State.

### RMA-S4 — State-dependent property evaluation without material memory

A property such as heat capacity is evaluated as a function of current authoritative temperature/state and immutable material parameters.

No additional persistent physical memory exists.

Purpose: determine whether state-dependent output is merely derived computation rather than new material state.

### RMA-S5 — Hysteretic/history-dependent material response

Two locations with identical current temperature and identical Material Definition produce different future response because a persistent internal variable records different past histories.

Purpose: test the transition from transformed Configuration to extension-local or governing state.

Expected routing pressure: RQ-ISO-001 and possibly RQ-EFM-001 depending on formulation role.

### RMA-S6 — Composition / reaction / microstructure evolution

Composition, damage, reaction progress, phase-history, porosity, crystallization, or microstructure evolves and changes future constitutive response.

Purpose: test whether evolving material identity can remain in Material Definition or must become state owned by an appropriate runtime responsibility.

Mechanism names shall not determine classification.

### RMA-S7 — Formulation-specific closure coordinate

A quantity is initially packaged in a computation-ready material structure, but correct thermodynamic evolution under the selected formulation requires its current evolving value as a closure coordinate.

Purpose: test whether packaging can hide thermodynamic state.

Expected routing: RQ-EFM-001 if closure/state-space insufficiency is established.

### RMA-S8 — Pure numerical acceleration cache

A solver caches interpolation indices, polynomial coefficients, branch metadata, or other accelerators that can be regenerated from authoritative current inputs and do not represent independent material meaning.

Purpose: distinguish numerical workspace from physical state and Configuration authority.

---

## 9. Preliminary Falsification / Reclassification Conditions

RQ-RMA-001 must remain falsifiable.

### F-RMA-1 — Direct semantic antecedent

If established frameworks/literature already define an equivalent boundary among reusable material definition, compiled/computation-ready material data, evolving state, and backend-specific representation, the independent candidate must be narrowed or closed.

### F-RMA-2 — Trivial information-classification consequence

If all meaningful cases reduce to the ordinary rule:

> deterministic transformed material data remains Configuration; independent evolving physical memory is State,

and no additional ThermoCore-specific decision boundary survives beyond existing ownership semantics, classify the result as engineering/conformance rather than a research contribution.

### F-RMA-3 — RQ-ISO absorption

If the only unresolved question is whether extension-local information may become mandatory Core State, route to RQ-ISO-001 and close the duplicate RMA claim.

### F-RMA-4 — RQ-EFM absorption

If the only unresolved question is whether a quantity is required for thermodynamic closure or state evolution, route to RQ-EFM-001 and close the duplicate RMA claim.

### F-RMA-5 — Implementation-only result

If surviving distinctions concern only:

- object versus table;
- CPU versus GPU;
- AoS versus SoA;
- cache strategy;
- code generation;
- serialization;
- memory placement;
- performance;

then classify them as implementation/performance concerns, not Framework research.

### F-RMA-6 — Existing Framework semantics already sufficient

If the current Runtime State / Configuration / Representation separation plus existing ownership and conformance rules completely classify all matched cases without any additional criterion, RQ-RMA-001 shall be closed or reclassified.

---

## 10. Prior-Art Survey Plan

The first evidence pass shall be aggressive and falsification-oriented.

Priority source families:

1. **Modelica.Media / Modelica material/medium architecture**
   - replaceable medium packages;
   - thermodynamic state records;
   - cached/base properties;
   - distinction between independent state and derived properties.

2. **MOOSE Materials**
   - material properties;
   - stateful versus non-stateful material properties;
   - old/older property storage;
   - dependency and recomputation semantics.

3. **OpenFOAM thermophysical models**
   - dictionaries/configuration;
   - runtime-selected thermophysical packages;
   - fields versus derived thermophysical properties;
   - cache/update behavior where documented.

4. **DOLFINx / constitutive-law interfaces and material kernels**
   - parameter/state separation;
   - quadrature-point history/state;
   - compiled kernels and backend forms.

5. **Cantera / CoolProp**
   - reusable phase/fluid definitions;
   - state objects versus parameter databases/backends;
   - derived property evaluation.

6. **GPU / HPC material-data systems**
   - host-side material definitions versus device-side packed data;
   - immutable parameter buffers versus evolving per-point state;
   - only where sources provide architectural semantics rather than performance anecdotes.

7. **Constitutive modeling literature**
   - material parameters versus internal variables/history variables;
   - stateful constitutive models;
   - sufficient-state concepts.

The survey shall not treat ordinary serialization, LUT construction, shader material parameters, or GPU buffers alone as direct evidence of the targeted semantic boundary.

---

## 11. Evidence Questions for v0.1

The first evidence matrix shall answer:

1. Is the general Material Definition -> computation-ready transformation already directly formalized in prior art?
2. Do reviewed systems explicitly distinguish immutable/material parameters from evolving internal/history variables?
3. Is there direct prior art for state-dependent properties that remain derived rather than persistent state?
4. Is backend-specific compilation/packing commonly treated as semantically equivalent material configuration?
5. Do any systems define a formal authority relationship between source material definition and compiled/runtime material artifacts?
6. Does cache persistence create any recognized semantic distinction beyond reconstructibility and physical history?
7. Are formulation-specific material artifacts treated as configuration, state, or solver-local structures in mature frameworks?
8. After routing closure to RQ-EFM and authority to RQ-ISO, does an independent RMA predicate remain?

---

## 12. Prohibited Claims

RQ-RMA-001 shall not claim that:

- ThermoCore invented runtime material abstraction;
- ThermoCore invented material compilation or material parameter packing;
- compiled material data is universally Configuration;
- every evolving material property must be Thermodynamic State;
- every state-dependent property must be stored;
- cache persistence implies physical state;
- GPU/device-side material data forms a new semantic category;
- one data layout is superior or normative;
- one material-definition format is universal;
- Material Representation and computation-ready material data are the same thing;
- current reference implementation proves architectural novelty;
- the Framework is a universal constitutive or multiphysics framework.

Novelty, priority, universal applicability, and superiority remain `NOT ESTABLISHED` unless later evidence supports a narrower statement.

---

## 13. Decision Gate

No RQ-RMA Research Gap Analysis shall be opened from this definition alone.

Required sequence:

```text
Definition
    |
    v
Bounded direct-antecedent Evidence Matrix v0.1
    |
    +--> strong prior-art / existing-boundary explanation
    |       -> narrow / close / reclassify
    |
    +--> defensible unresolved semantic distinction
            -> focused v0.2 stress test
            -> only then consider Research Gap Analysis
```

A negative result is a successful research outcome if it prevents unsupported contribution claims.

---

## 14. Current Disposition

```text
Original RQ-001 runtime-material-abstraction motivation:
VALID HISTORICAL RESEARCH MOTIVATION

General runtime material abstraction:
KNOWN TO BE ESTABLISHED PRACTICE — DIRECT SURVEY STILL REQUIRED

Independent RQ-RMA-001 Research Gap:
NOT ESTABLISHED

Candidate semantic dimensions:
UNDER SURVEY

Research Gap Analysis readiness:
NO-GO

Framework Specification impact:
NONE

Implementation impact:
NONE

Novelty / priority:
NOT ESTABLISHED

Next action:
BUILD RQ-RMA-001 DIRECT-ANTECEDENT EVIDENCE MATRIX v0.1
```

RQ-RMA-001 remains open only as a bounded evidence-driven research question. Its purpose is to determine whether a defensible semantic material-abstraction boundary survives direct prior-art pressure after RQ-ISO-001 and RQ-EFM-001 are treated as already-established ThermoCore boundaries.