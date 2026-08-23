# RQ-ECA-001 — Compositional Extension Admissibility

Version: 0.1  
Status: Defined — Evidence Survey Required  
Research Question: RQ-ECA-001 — Compositional Extension Admissibility  
Tracking Issue: #127  
Date: 2026-08-23  
Definition baseline: `13c7b37c2e1df40847d9c117a19854dbe3d35905`

---

## 1. Purpose

This document defines a bounded research question concerning the **composition of ordinary ThermoCore extensions**.

The question is not whether modular composition, plugins, coupled physics, source terms, property updates, or extension-local state exist. Those are established engineering practices.

The narrower question is whether **local admissibility composes**.

An Extension A may be admissible as an ordinary/Core-preserving extension when evaluated alone. Extension B may also be admissible when evaluated alone. This does not by itself prove that the composed system A+B remains admissible under the same thermodynamic formulation, ownership boundaries, state authority, and physical-accounting assumptions.

This definition is non-normative. It does not establish novelty, priority, superiority, Framework requirements, or implementation requirements.

---

## 2. Primary Research Question

> **For a fixed declared thermodynamic scope, when multiple individually admissible ordinary extensions are composed, under what conditions does the composition remain ordinary/Core-preserving without introducing new Thermodynamic State authority, conflicting responsibility over material/property information, duplicated or omitted physical contribution, hidden closure dependence, or a requirement to revise the governing thermodynamic formulation/Core? When must the composed system be re-evaluated as an aggregate mechanism or reclassified outside ordinary-extension scope?**

The research therefore concerns a possible difference between:

```text
local admissibility
    and
compositional admissibility
```

The existence of that difference is a hypothesis to be tested, not an assumed contribution.

---

## 3. Research Boundary

RQ-ECA-001 is limited to a **fixed declared thermodynamic scope** and to composition among mechanisms that are individually admissible as ordinary extensions under their isolated assumptions.

The RQ does not investigate:

- generic plugin systems;
- arbitrary software dependency management;
- execution scheduling;
- timestep selection;
- synchronization protocols;
- solver convergence;
- numerical stability;
- API design;
- message transport;
- universal multiphysics composition;
- arbitrary mechanical, electromagnetic, chemical, or fluid formulations outside the declared thermodynamic scope.

A numerical or scheduling issue is relevant only if it reveals a **semantic dependency** that changes thermodynamic formulation completeness, information authority, or Framework responsibility.

---

## 4. Relationship to RQ-EFM-001

RQ-EFM-001 establishes the formulation-relative admissibility boundary for one selected mechanism/formulation.

Its role remains:

```text
mechanism/formulation participation
    -> thermodynamic completeness / admissibility
    -> ordinary extension or revision boundary
```

RQ-ECA-001 does not replace that gate.

Instead, it asks whether the result:

```text
A = D0
B = D0
```

implies:

```text
A + B = D0
```

or whether composition must itself be assessed as an aggregate mechanism because interaction among A and B changes closure, exchange sufficiency, or governing responsibility.

If simply applying the existing RQ-EFM-001 gate to the aggregate A+B fully resolves all relevant composition cases without any independent rule, RQ-ECA-001 shall be closed rather than narrowed indefinitely.

---

## 5. Relationship to RQ-ISO-001

RQ-ISO-001 remains the authority/non-promotion rule after information categories and ordinary-extension status are accepted.

RQ-ECA-001 may expose composition-induced authority conflicts, for example when two extensions each remain individually local but their composition causes one extension-local quantity to become a required thermodynamic closure coordinate.

Such a finding must be classified carefully:

```text
composition exposes formulation incompleteness
    -> RQ-EFM-001 boundary

composition exposes unnecessary state promotion / ownership violation
    -> RQ-ISO-001 boundary
```

RQ-ECA-001 may only remain independent if evidence supports a distinct **composition-level decision problem** that is not reducible to those existing gates.

---

## 6. Relationship to Closed RQ-FCI-001 and RQ-CEX-001

RQ-FCI-001 was closed as an independent contribution and reclassified as the **Formulation Change Containment Property**.

RQ-CEX-001 was closed as an independent contribution and reclassified as the **Conservative Exchange Accounting Property**.

RQ-ECA-001 shall not reopen either conclusion.

A composition problem that is only:

- a compatible formulation substitution issue;
- a quantity/sign/time/accounting semantic issue;
- a conservative-port / exchange-accounting issue;

shall be routed to the applicable engineering/conformance property rather than treated as a new RQ-ECA contribution.

---

## 7. Candidate Compositional Interaction Dimensions — Under Survey

The following dimensions are **Under Survey** and are not yet Framework requirements.

### ECA-D1 — Extension-to-Extension Data Dependency

One extension consumes information produced or modified by another extension.

Questions:

- Is the dependency explicit?
- Does the dependency alter thermodynamic completeness?
- Is the information still correctly classified as Configuration, exchange, extension-local state, or Thermodynamic State?

### ECA-D2 — Property / Configuration Responsibility Overlap

Two extensions influence the same effective material property or configuration-derived quantity.

Questions:

- Are their contributions composable by a declared rule?
- Does one overwrite or reinterpret the other's result?
- Is there a unique authority for the effective property meaning?

### ECA-D3 — Source / Exchange Contribution Interaction

Two extensions contribute energy/source/exchange effects that are individually valid.

Questions:

- Are contributions independent, coupled, or physically overlapping?
- Can the same physical effect be counted twice?
- Does composition require a new conservation target or accounting relation?

This dimension must not duplicate the already closed RQ-CEX research claim.

### ECA-D4 — Extension-Local State Dependency

Extension A may depend on Extension B's local state, or vice versa.

Questions:

- Does the dependency remain external to authoritative Thermodynamic State?
- Does combined closure remain sufficient without promoting either local state?
- Is an apparently local quantity now required to determine thermodynamic evolution?

### ECA-D5 — Feedback / Cyclic Dependency

Composition may create a semantic feedback loop:

```text
A output -> B response -> A response
```

The existence of a feedback loop is not itself a Framework problem.

The research question is whether the loop changes:

- required thermodynamic coordinates;
- ownership/authority;
- exchange sufficiency;
- governing formulation completeness.

### ECA-D6 — Combined Thermodynamic Closure Sufficiency

A and B may each be thermodynamically complete in isolation under frozen assumptions, while A+B may require additional thermodynamic information.

This is the strongest candidate dimension, but it is also the one most likely to reduce to aggregate reapplication of RQ-EFM-001.

### ECA-D7 — Ownership / Authority Conflict

Two individually valid extensions may attempt to govern the same semantic quantity or responsibility.

Examples include:

- competing writers to one extension-owned semantic coordinate;
- one extension treating another's local state as authoritative Core State;
- duplicated ownership of an effective property meaning.

If the issue is fully captured by existing RQ-ISO-001 non-promotion/authority rules, no independent RQ-ECA gap remains.

### ECA-D8 — Scope Identity

Composition must distinguish:

```text
same declared physical scope + interacting extensions
```

from:

```text
composition that materially enlarges the governing physical scope
```

A true scope expansion shall not be mislabeled as failure of compositionality. It is expected to route to RQ-EFM-001 / explicit formulation or Core revision.

---

## 8. Preliminary Composition Model — Under Survey

The working hypothesis is that extension composition may require assessment at two levels:

```text
Level 1 — Local admissibility
    evaluate A
    evaluate B
    ...

Level 2 — Aggregate admissibility
    evaluate interaction structure of A+B+...
```

Level 2 would be justified only if composition introduces semantics not visible in the local evaluations.

Candidate composition-preserving conditions include, but are not limited to:

- dependencies are explicit and semantically classified;
- no ownership or responsibility conflict is introduced;
- no required thermodynamic closure coordinate is hidden in extension-local state;
- physical contributions remain non-duplicated and interpretable;
- the declared thermodynamic scope remains materially unchanged;
- the aggregate selected formulation remains complete.

These are hypotheses for evidence survey, not specification requirements.

---

## 9. Candidate Scenarios — Under Survey

No consequence test is pre-registered at this stage. The following scenario families exist only to guide evidence collection and later falsifiable design.

### ECA-S0 — Independent / Orthogonal Control

Two individually admissible extensions operate on semantically distinct information with no cross-dependency.

Expected use:

- establish a control in which local admissibility should compose trivially;
- prevent the research design from treating all composition as problematic.

### ECA-S1 — Shared Property Responsibility

Two individually admissible extensions both alter or derive the same effective material property.

Candidate question:

> Is a declared combination rule sufficient, or does composition create ambiguous responsibility/meaning?

### ECA-S2 — Source + Property Feedback

Extension A modifies an effective property used by Extension B to compute an energy/source contribution; B's result may in turn alter the condition observed by A.

Candidate question:

> Does the combined system remain semantically complete under the original Thermodynamic State and exchanges, or is an additional coordinate/authority required?

### ECA-S3 — Extension-Local State Cross-Dependency

A owns local state `x`; B owns local state `y`; the composed response depends on both.

Candidate question:

> Are `x` and `y` still honestly extension-local, or does the combined thermodynamic formulation now require one or both as closure coordinates?

### ECA-S4 — Composition-Induced Formulation Boundary Control

A and B are individually admissible only under isolated/frozen assumptions, while their physically coupled aggregate introduces a governing dependency that makes the selected thermodynamic formulation incomplete.

Expected disposition:

```text
aggregate RQ-EFM reassessment
    -> formulation/Core revision or scope narrowing
```

This case must not be interpreted as evidence that all extension composition is unsafe.

---

## 10. Preliminary Falsification Conditions

RQ-ECA-001 shall be rejected as an independent Research Gap if any of the following is sufficiently established.

### ECA-F1 — Aggregate RQ-EFM Sufficiency

Treating A+B as one aggregate mechanism and reapplying the existing RQ-EFM-001 admissibility gate fully resolves all semantically meaningful composition cases.

### ECA-F2 — RQ-ISO Sufficiency

All apparent composition failures reduce to already-covered ownership/non-promotion violations without an independent composition criterion.

### ECA-F3 — Established Direct Antecedent

Prior art already provides a substantially equivalent architecture-level compositional admissibility rule for modular coupled physical mechanisms, including interaction-induced closure/authority reassessment.

### ECA-F4 — No Matched Composition Witness

No defensible same-scope scenario can be identified in which A and B are individually admissible but the composition introduces a new architecture-level semantic issue not already detected by aggregate RQ-EFM/RQ-ISO evaluation.

### ECA-F5 — Numerical/Scheduling Collapse

Apparent failures depend only on execution ordering, timestep coupling, solver convergence, or numerical discretization rather than Framework semantics.

### ECA-F6 — Scope Expansion Only

All non-compositional cases are actually physical-scope expansions and therefore belong directly to the RQ-EFM revision boundary.

A negative result under these conditions is a valid research outcome.

---

## 11. Claims Prohibited at Definition Stage

RQ-ECA-001 does not support claiming that:

- ThermoCore is the first framework to compose extensions;
- pairwise admissibility is generally insufficient in all frameworks;
- ThermoCore has discovered compositionality as a software principle;
- all extension interactions require aggregate review;
- all feedback coupling threatens Core stability;
- multiple property modifiers are inherently invalid;
- extension-local state cannot interact;
- composition can be classified by mechanism name;
- a composition rule is novel;
- the current Framework already proves compositional admissibility;
- ThermoCore provides universal multiphysics composition.

Novelty and priority remain **NOT ESTABLISHED**.

---

## 12. Prior-Art / Evidence Survey Plan

The first evidence pass should aggressively search for direct antecedents rather than generic modularity examples.

Priority evidence families include:

- Modelica multi-component / replaceable-model composition and connection semantics;
- MOOSE MultiApps, Transfers, Materials, coupled variables, and multi-physics composition responsibilities;
- OpenFOAM multi-region / fvOptions / source composition and thermophysical coupling constraints;
- preCICE multi-participant coupling and coupling-scheme composition;
- FMI / SSP / co-simulation composition semantics where interaction contracts are explicit;
- port-Hamiltonian / bond-graph compositionality as a strong physical-network antecedent;
- component-based multiphysics frameworks that explicitly distinguish local model validity from validity of coupled composition;
- software-architecture / contract-based design literature only where it directly addresses physical-model semantic composition rather than generic plugin compatibility.

The evidence matrix should ask:

1. Does the source evaluate components only locally, or also the composed interaction?
2. Can individually valid components become invalid when coupled?
3. Are ownership/responsibility conflicts represented explicitly?
4. Are state/closure dependencies recomputed at composition time?
5. Are source/exchange/property interactions classified?
6. Is there an explicit distinction between same-scope composition and scope expansion?
7. Is aggregate re-evaluation already the standard solution?
8. Does the source define a distinct compositional admissibility criterion, or only generic modular compatibility?

---

## 13. Expected Research Sequence

```text
Definition
    -> bounded prior-art / evidence survey
    -> Evidence Matrix
    -> Research Gap Analysis only if an independent candidate survives
    -> pre-registered consequence test only if justified
    -> isolated research harness
    -> final bounded disposition
    -> optional later specification consideration
```

A Research Gap Analysis shall not be opened merely because the problem is useful or intuitively plausible.

---

## 14. Current Disposition

```text
Research line: RQ-ECA-001
Definition status: COMPLETE
Evidence survey: REQUIRED
Independent Research Gap: NOT ESTABLISHED
Novelty / priority: NOT ESTABLISHED
Framework Specification impact: NONE
Implementation impact: NONE
```

The most important null hypothesis is:

> **Compositional extension admissibility may be fully reducible to treating the composition as an aggregate mechanism under RQ-EFM-001, followed by the existing RQ-ISO-001 authority rules.**

RQ-ECA-001 remains open only long enough to test that proposition against direct prior art and bounded evidence.