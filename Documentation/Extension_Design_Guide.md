# Extension Design Guide

Status: **Non-Normative User Guide**  
Audience: Extension authors, integrators, and third-party implementers  
Authority: The documents under `Documentation/Framework_Specification/` remain normative and authoritative.

---

## 1. Purpose

This guide explains how to extend ThermoCore without requiring users to read the full research history behind the Framework Specification.

It translates the completed RQ-001 research results and existing Framework rules into a practical decision process for questions such as:

- Can a new physical mechanism remain an ordinary Extension Module?
- Where should mechanism-specific information live?
- When is a new quantity Thermodynamic State rather than extension-local state, Configuration, or Representation?
- How should energy-bearing interactions cross a Framework boundary?
- When must the thermodynamic formulation or Framework Core be revised instead of adding another extension?

This document does **not** create new Framework requirements. If this guide conflicts with an applicable Framework Specification, the Framework Specification is authoritative.

---

## 2. The Two-Stage Extension Decision

Use the following order whenever a new mechanism or coupled domain is proposed.

```text
New physical mechanism / coupled domain
              │
              ▼
Stage 1 — Formulation-relative admissibility
Can the selected thermodynamic formulation remain complete
for the claimed scope through semantically honest communication?
              │
        ┌─────┴─────┐
       Yes           No
        │             │
        ▼             ▼
Ordinary Extension   Revise the applicable
candidate             thermodynamic formulation/Core
        │             or narrow the claimed scope
        ▼
Stage 2 — Core-State authority / non-promotion
Can the extension participate without redefining
Thermodynamic State identity, ownership, mandatory membership,
or Core completeness?
        │
        ├─ Yes → ordinary Extension participation
        └─ No  → revise the architecture/specification;
                 do not hide the change inside an Extension
```

The order matters:

> **Admissibility first; authority/non-promotion second.**

Do not use State isolation rules to keep information outside the Core when the selected thermodynamic formulation is itself incomplete.

---

## 3. Stage 1 — Can This Remain an Ordinary Extension?

A mechanism may remain an ordinary Extension when the existing Thermodynamic State semantics and Thermodynamic Computation responsibility remain sufficient for the claimed thermodynamic scope, while the mechanism communicates semantically distinct information through applicable Framework Interfaces.

Ask these questions:

1. With the current authoritative Thermodynamic State and applicable material/configuration information, can Thermodynamic Computation still determine the thermodynamic condition correctly for the claimed scope?
2. Can any additional influence be represented as an honest physical input, source, exchange, property contribution, boundary contribution, or other semantically distinct communication?
3. Does the mechanism-specific state remain meaningful only to the external mechanism or Extension rather than becoming a thermodynamic coordinate required for closure or next-state determination?
4. Can the Framework Core remain complete without redefining its responsibilities?

If all applicable answers remain yes, ordinary Extension treatment may be appropriate.

### 3.1 When communication enrichment is enough

An interface may carry richer physical meaning when necessary. For example, a simple scalar input may need to become a declared energy rate, boundary flux, generalized work contribution, or another semantically distinct exchange.

Enrichment is acceptable when it preserves the meaning and ownership of the communicated information.

It is **not** acceptable to use an opaque payload merely to serialize a hidden thermodynamic coordinate that the Core actually requires for closure or state evolution.

### 3.2 When Core/formulation revision is required

Do not classify a capability as an ordinary Extension if correct thermodynamic representation or evolution requires any of the following:

- changing the authoritative meaning of Thermodynamic State;
- adding a new mandatory thermodynamic coordinate because the current formulation is incomplete;
- changing a Framework Core responsibility;
- changing the governing thermodynamic formulation in a way that cannot be preserved through semantically honest communication.

In these cases, revise the applicable Framework/specification/formulation or narrow the claimed scope.

Mechanism name, physical domain, coupling strength, bidirectionality, or repeated participation do **not** decide this boundary by themselves.

---

## 4. Stage 2 — Preserve Core-State Authority

Once ordinary Extension status is accepted, keep the authoritative Core-State boundary fixed.

An ordinary Extension may:

- read or consume permitted framework information;
- supply or communicate extension-specific information;
- own extension-specific local state;
- derive extension-specific results;
- participate in thermodynamic evolution through declared contributions.

An ordinary Extension must not gain authority to:

- redefine Thermodynamic State identity;
- write Thermodynamic State directly;
- reassign Thermodynamic State ownership;
- make extension-local quantities mandatory Core State merely because the extension uses them;
- redefine Core completeness;
- bypass applicable Framework Interfaces.

Thermodynamic Computation remains the Framework Core responsibility that evolves and writes Thermodynamic State.

---

## 5. Where Should Information Live?

Use semantic role rather than storage location, class name, device placement, or persistence duration.

| Information category | Use it when | Do not classify it by |
|---|---|---|
| **Thermodynamic State** | Information is part of the authoritative evolving thermodynamic condition for the selected formulation | buffer location, persistence alone, GPU/CPU placement |
| **Extension-local state** | Persistent information exists solely for an Extension's mechanism-specific responsibility and is not required as authoritative Core thermodynamic state | the fact that it affects an exchanged contribution |
| **Configuration / Material Definition-derived Configuration** | Reusable information defines material/model behavior without becoming evolving physical state | compilation, LUT generation, caching, runtime rebuild, packing, serialization, GPU residence |
| **Representation** | Information interprets authoritative State and applicable material information for downstream consumption | display status alone, caching, persistence, rendering, output files |
| **Input / exchange / contribution** | Information crosses a boundary with a declared physical or control role while retaining its source ownership and semantics | transport mechanism, packet/message structure, API shape |

### 5.1 Configuration does not become State because it is optimized

The following transformations do not by themselves convert Configuration into Runtime State:

- normalization;
- unit conversion;
- compilation;
- derived constants;
- lookup tables;
- CPU/SIMD/GPU packing;
- device buffers;
- caching;
- serialization;
- runtime invalidation and rebuild after an explicit configuration change.

Reclassification requires a semantic reason such as independent evolving physical history, new governing authority, or a formulation-relative closure role.

### 5.2 Representation does not become State because it persists

The following do not by themselves convert Representation into Thermodynamic State:

- frame-to-frame caching;
- persistence for consumer continuity;
- serialization;
- storage in a GPU resource;
- rendering;
- consumer-side transformation;
- output-file retention.

If a value becomes required to determine future thermodynamic evolution or closure, reconsider its role through the Stage 1 and Stage 2 decisions rather than continuing to call it Representation.

---

## 6. Energy-Bearing Interactions

An energy-bearing interaction should have enough declared physical meaning that its accounting role and applicable conservation target are unambiguous for the claimed scope.

Useful distinctions include:

- external source or sink;
- internal redistribution;
- cross-domain conversion;
- boundary or interface exchange.

Avoid representing the same physical contribution simultaneously as both an already-accounted internal transfer and a new external source.

This is an accounting rule, not a transport guarantee. ThermoCore does not require packet identifiers, exactly-once messaging, queues, transactions, or a particular synchronization protocol.

Also preserve this distinction:

```text
semantic conservation meaning
        !=
numerical/discretization conservation achieved by an implementation
```

A semantically correct exchange still requires appropriate numerical methods, mapping, discretization, and Verification to demonstrate numerical conservation under declared conditions.

---

## 7. Feedback from Representation or Downstream Consumers

Representation is downstream interpretation and does not gain governing authority merely because an application later wants to use it as feedback.

If a downstream quantity is fed back into thermodynamic evolution, classify the re-entry explicitly according to its actual role, for example:

- external input;
- energy/source contribution;
- control input;
- property/configuration update;
- another declared coupling role.

Do not let Representation write Thermodynamic State or bypass Framework Interfaces.

The fact that feedback originated from a displayed, cached, or derived quantity does not determine the semantics of its later re-entry.

---

## 8. Combining Multiple Extensions

Individual admissibility is not permanent immunity under composition.

If Extension A and Extension B are each ordinary Extensions in isolation, evaluate the actual combination `A + B` again.

Check whether composition introduces:

- a new thermodynamic closure dependency;
- a new required state coordinate;
- an ownership or authority conflict;
- duplicated or omitted energy contribution;
- a true expansion of the claimed physical scope.

Route the combined result through the same two-stage decision:

```text
A + B aggregate mechanism
        ↓
Stage 1 — formulation-relative admissibility
        ↓
Stage 2 — Core-State authority / non-promotion
        ↓
energy/accounting and other conformance checks as applicable
```

Pure execution-order, iteration, synchronization, or convergence difficulty is not automatically a new Framework-semantic problem unless it exposes a change in thermodynamic completeness, authority, ownership, or claimed scope.

---

## 9. Practical Examples

### 9.1 External heating

An external heater provides a declared energy contribution to Thermodynamic Computation.

Typical classification:

```text
heater state/control      → external responsibility
energy contribution       → declared input/exchange
Thermodynamic State       → written only by Thermodynamic Computation
```

No Core-State promotion is required merely because the heater acts repeatedly.

### 9.2 Joule-heating coupling

An electrical model may retain voltage, current, charge, and other electrical state outside Thermodynamic State while supplying the resulting thermal energy contribution to Thermodynamic Computation, provided the selected thermodynamic formulation remains complete under that exchange.

Strong or bidirectional coupling does not by itself require electrical state to become Thermodynamic State.

If the chosen thermodynamic formulation cannot evolve correctly without an additional governing thermodynamic coordinate, return to Stage 1 rather than hiding that coordinate in the electrical Extension.

### 9.3 Hysteresis or mechanism-local memory

A mechanism may own a hysteresis variable as extension-local state when that memory exists solely for the mechanism-specific responsibility and the Core thermodynamic formulation remains sufficient through declared exchanges.

If the variable is actually required as an authoritative thermodynamic coordinate for closure or next-state determination, it is not merely extension-local memory for the claimed formulation. Reconsider the formulation/Core boundary.

### 9.4 A new closure-critical coordinate

Suppose two systems have identical current Thermodynamic State, applicable material information, external exchanges, and complete extension-local information available under the current contract, yet correct thermodynamic next-state evolution differs because an omitted coordinate is required.

That is pressure for formulation/Core revision, not a reason to serialize the coordinate through an opaque extension payload.

### 9.5 GPU material table

A Material Definition is normalized and compiled into a GPU-ready lookup table.

Typical classification:

```text
Material Definition       → Configuration
compiled GPU table        → computation-ready Configuration
GPU residence             → implementation detail
Thermodynamic State       → unchanged
```

Runtime compilation, caching, or device placement does not create a new physical state category.

### 9.6 Temperature-to-color visualization

A renderer maps current temperature to color.

Typical classification:

```text
Thermodynamic State       → source information
color / visual property   → Representation
renderer/application      → Representation Consumer
```

Caching or persisting the color does not grant it authority over Thermodynamic State.

### 9.7 Representation-derived control feedback

An application derives a control decision from a visual or interpreted quantity and later uses that decision to affect heating.

The control signal must re-enter as an explicit input/control/source role. The Representation path does not gain permission to write Thermodynamic State.

### 9.8 Two individually valid Extensions interact

A radiation Extension and a chemical-reaction Extension may each be admissible alone. If their composition introduces a new closure dependency, duplicated heat contribution, or new governing state requirement, re-evaluate the aggregate mechanism. Do not assume pairwise admissibility implies global admissibility.

---

## 10. What This Guide Does Not Specify

This guide intentionally does not prescribe:

- APIs or function signatures;
- packet/message formats;
- message identifiers;
- queues or middleware;
- synchronization protocols;
- execution scheduling;
- timestep selection;
- explicit or implicit solver choice;
- mesh/data mapping algorithms;
- storage layout;
- CPU/GPU execution design;
- engine-specific integration.

Those implementation choices are permitted when they preserve the applicable normative Framework semantics.

---

## 11. Pre-Integration Checklist

Before calling a new capability an ordinary ThermoCore Extension, confirm:

- [ ] The claimed thermodynamic scope is explicit.
- [ ] The applicable thermodynamic formulation is identified.
- [ ] The formulation remains complete under semantically honest declared communication.
- [ ] No required thermodynamic coordinate is hidden in opaque exchange, Configuration, Representation, or extension-local state.
- [ ] Thermodynamic State identity and ownership remain unchanged.
- [ ] Thermodynamic Computation remains the Framework Core writer of Thermodynamic State.
- [ ] Extension-local state is owned only by the Extension and is not silently promoted to mandatory Core State.
- [ ] Configuration remains Configuration despite compilation/cache/LUT/backend transformation unless a real semantic reclassification occurs.
- [ ] Representation remains downstream and non-authoritative unless a later feedback role is explicitly reclassified on re-entry.
- [ ] Energy-bearing interactions have an unambiguous accounting role and applicable conservation target.
- [ ] Semantic conservation is not being used as proof of numerical conservation.
- [ ] Composed Extensions have been re-evaluated as the actual aggregate mechanism.
- [ ] Applicable Framework Interfaces are used without ownership transfer or write-authority leakage.
- [ ] No implementation detail has been elevated into a Framework requirement without specification authority.

If any applicable item cannot be satisfied, do not hide the conflict inside an Extension. Revisit the formulation, scope, architecture, or applicable Framework Specification.

---

## 12. Authoritative References and Research Traceability

Normative references:

- [`Framework_Principles.md`](Framework_Specification/Framework_Principles.md)
- [`Core_Architecture.md`](Framework_Specification/Core_Architecture.md)
- [`Data_Flow.md`](Framework_Specification/Data_Flow.md)
- [`Thermodynamic_State.md`](Framework_Specification/Thermodynamic_State.md)
- [`Material_Representation.md`](Framework_Specification/Material_Representation.md)
- [`Framework_Interfaces.md`](Framework_Specification/Framework_Interfaces.md)
- [`Extension_Boundary.md`](Framework_Specification/Extension_Boundary.md)
- [`Framework_Conformance.md`](Framework_Specification/Framework_Conformance.md)

Research traceability:

- [`RQ_001_Research_Synthesis_and_Final_Closure_v0.1.md`](../Research/04_Research_Gap/RQ_001_Research_Synthesis_and_Final_Closure_v0.1.md)
- [`RQ_EFM_001_Final_Research_Gap_Disposition_v0.1.md`](../Research/04_Research_Gap/RQ_EFM_001_Final_Research_Gap_Disposition_v0.1.md)
- [`RQ_ISO_001_Final_Research_Gap_Disposition_v0.1.md`](../Research/04_Research_Gap/RQ_ISO_001_Final_Research_Gap_Disposition_v0.1.md)

The research artifacts explain why the boundaries were adopted and how competing candidate claims were supported, narrowed, or reclassified. Ordinary users do not need to read them to apply this guide; the normative Framework Specifications remain the source of requirements.