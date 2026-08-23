# Extension Boundary

Version: 1.1  
Status: Normative Specification

---

## 1. Normative Dependencies

Parent Specifications:

- `Framework_Principles.md`
- `Core_Architecture.md`
- `Data_Flow.md`
- `Thermodynamic_State.md`
- `Material_Representation.md`
- `Framework_Interfaces.md`
- `Specification_Governance.md`

This document conforms to the parent specifications and refines only Extension Module semantics and boundary rules.

It shall not redefine the Framework Core, architecture, ownership, Runtime State, Representation, Framework Interfaces, Communication, information flow, or governance established by the parent specifications.

## 2. Purpose

This document defines the normative semantics, responsibilities, ownership boundaries, admissibility, and constraints of Extension Modules within ThermoCore.

Extension Modules provide optional capability outside the Framework Core. The Framework Core shall remain complete, valid, and conforming without them.

This specification does not prescribe extension implementations, algorithms, APIs, programming interfaces, numerical formulations, rendering techniques, execution mechanisms, or backend-specific behavior.

## 3. Extension Overview

An Extension Module is an optional architectural responsibility that expands framework capability without becoming part of or redefining the Framework Core.

A proposed capability qualifies for treatment as an ordinary Extension Module only when it satisfies the extension-admissibility requirements of this specification. Optional packaging, implementation location, mechanism name, physical-domain label, or participation in thermodynamic evolution shall not by themselves establish Extension Module admissibility.

The conceptual relationship is:

```text
Framework Core
        │
        ▼
Framework Interfaces
        │
        ▼
Extension Module
```

This figure represents architectural boundary only. It does not define execution order, scheduling, direction of execution, implementation structure, or backend communication.

Extension Modules communicate with the Framework Core only through applicable Framework Interfaces. Communication through a Framework Interface does not transfer ownership, redefine semantics, merge responsibilities, or make an Extension Module part of the Framework Core.

## 4. Extension Responsibilities

An Extension Module may:

- Read framework information through applicable Framework Interfaces;
- Consume framework information through applicable Framework Interfaces;
- interpret framework information for an extension-specific purpose;
- Supply extension-specific information through applicable Framework Interfaces;
- Communicate extension-specific information through applicable Framework Interfaces; and
- produce and govern extension-owned information.

An Extension Module shall not:

- own the Framework Core or any Framework Core responsibility;
- redefine, replace, or perform the responsibilities assigned to the Framework Core;
- redefine Runtime State;
- redefine Representation;
- redefine Material Representation;
- redefine Framework Interfaces;
- redefine Communication semantics;
- modify Framework Core ownership;
- bypass applicable Framework Interfaces; or
- treat access to framework information as ownership of that information.

An Extension Module may interpret framework information only for its assigned extension-specific purpose. Such interpretation shall not replace Material Representation or alter the authoritative semantics of the interpreted information.

## 5. Extension Ownership

Ownership shall remain separate between the Framework Core and each Extension Module.

The ownership assignments established by the parent specifications shall remain unchanged.

In particular:

- Thermodynamic State retains ownership of Runtime State;
- Material Representation retains ownership of Representation; and
- Framework Interfaces retain their assigned communication responsibility without owning the information they communicate.

Each Extension Module owns only:

- state that exists solely for its extension-specific responsibility;
- information produced solely for its extension-specific responsibility; and
- interpretation performed solely for its extension-specific purpose.

Extension-owned information shall remain distinct from Runtime State, Representation, and Configuration governed by their applicable authoritative specifications and ownership assignments. An Extension Module shall not reclassify extension-owned information as framework-owned information or framework-owned information as extension-owned information.

Reading, supplying, communicating, consuming, or interpreting information shall not transfer, duplicate, or reassign ownership.

Communication between the Framework Core and an Extension Module shall preserve the identity, semantics, and ownership of all communicated information.

## 6. Extension Semantics

The following Extension Module semantics are normative.

### 6.1 May Extend

An Extension Module may expand framework capability within its assigned extension boundary.

Expansion shall remain optional and shall preserve every applicable parent specification.

### 6.2 Shall Not Redefine

An Extension Module shall not redefine the Framework Core, a Framework Core responsibility, or an established normative concept.

Refinement within an extension-specific responsibility shall not create an alternative authoritative definition for a framework concept.

### 6.3 Shall Not Replace

An Extension Module shall not replace a Framework Core component, responsibility, information category, ownership assignment, or Framework Interface.

Use of an Extension Module shall not become a condition for Framework Core completeness or Framework Conformance.

### 6.4 Shall Not Bypass

An Extension Module shall not bypass applicable Framework Interfaces, architectural boundaries, ownership rules, Communication semantics, or information flow.

Direct dependency shall not be used to evade an applicable normative boundary.

### 6.5 Shall Communicate Through Applicable Framework Interfaces

An Extension Module shall Read, Supply, Communicate, and Consume framework information only through applicable Framework Interfaces.

Communication shall preserve ownership, semantics, responsibility boundaries, and information flow. It shall not grant modification authority or redefine the communicated information.

### 6.6 Ordinary Extension Admissibility

Ordinary Extension Module admissibility shall be determined relative to the applicable thermodynamic formulation and the claimed thermodynamic scope.

A capability may remain an ordinary Extension Module when external, mechanism-specific, or cross-domain governing information can remain outside Thermodynamic State while the existing Thermodynamic State semantics and Thermodynamic Computation responsibility remain sufficient to represent and evolve the claimed thermodynamic condition through permitted Framework Interface communication.

The information communicated through applicable Framework Interfaces may be refined or enriched when additional semantically distinct information is required to preserve that sufficiency. Such refinement or enrichment shall preserve the information's authoritative semantics and ownership and shall not conceal a required Thermodynamic State quantity, Framework Core responsibility, or ownership transfer inside an opaque payload, extension-owned state, Configuration, Representation, or implementation-specific indirection.

A capability shall not be treated as an ordinary Extension Module when correct thermodynamic representation or evolution within the claimed scope requires a change to the authoritative Thermodynamic State semantics, a Framework Core responsibility, or the applicable thermodynamic formulation that cannot be preserved through semantically honest permitted communication. Such a case shall be governed as an explicit Framework Core or applicable specification revision, or the claimed framework scope shall be narrowed to exclude the unsupported behavior.

Mechanism name, physical-domain identity, dependency direction, bidirectional communication, repeated participation, or participation in thermodynamic evolution shall not by themselves establish Thermodynamic State membership, Framework Core membership, or a requirement for Framework Core revision.

## 7. Extension Constraints

The following constraints are normative:

1. Extension Modules shall remain optional.
2. The Framework Core shall remain complete, valid, and conforming without Extension Modules.
3. Extension Modules shall not become part of the Framework Core solely because they communicate with or depend on it.
4. Extension Modules shall not redefine, replace, duplicate, or absorb Framework Core responsibilities.
5. Extension Modules shall not modify, transfer, duplicate, or reassign Framework Core ownership.
6. Extension Modules shall not modify or reinterpret established framework semantics.
7. Extension Modules shall not redefine Runtime State, Representation, Material Representation, Framework Interfaces, Communication, or information flow.
8. Extension Modules shall not bypass applicable Framework Interfaces or architectural boundaries.
9. Extension-owned state and information shall remain owned by the applicable Extension Module.
10. Extension-owned information shall remain distinct from framework-owned information.
11. Communication shall not imply ownership, modification authority, or Framework Core membership.
12. An Extension Module shall preserve all applicable parent specifications.
13. Ordinary Extension Module admissibility shall be determined relative to the applicable thermodynamic formulation and claimed thermodynamic scope.
14. External, mechanism-specific, or cross-domain governing information shall not be promoted into Thermodynamic State solely because it participates in thermodynamic evolution.
15. Permitted Communication may be refined or enriched only when the communicated information remains semantically distinct and its ownership remains preserved.
16. Required Thermodynamic State information, Framework Core responsibility, or ownership shall not be concealed in extension-owned state, opaque Communication, Configuration, Representation, or implementation-specific indirection.
17. A capability that requires authoritative Thermodynamic State, Framework Core responsibility, or applicable thermodynamic-formulation revision for the claimed scope shall not be classified as an ordinary Extension Module under the unrevised Framework Core.

These constraints apply independently of implementation technique, numerical formulation, execution backend, or Representation Consumer.

Violation of any applicable constraint constitutes non-conformance with this specification.

## 8. Governance Rules

The following governance rules are normative.

### 8.1 Governance Rule 1 — Closed Core, Open Extension

The Framework Core shall remain closed to architectural redefinition.

Framework capability may be expanded only through conforming Extension Modules.

An Extension Module shall not be used to introduce an undeclared Framework Core change. A proposed change to the Framework Core shall be governed as a change to its authoritative specification, not as an ordinary extension.

### 8.2 Governance Rule 2 — Extend, Do Not Redefine

An Extension Module may extend framework capability within its assigned boundary. It shall not redefine, replace, duplicate, or absorb the Framework Core or its responsibilities.

### 8.3 Governance Rule 3 — Communicate, Do Not Bypass

An Extension Module shall communicate with the Framework Core only through applicable Framework Interfaces. It shall not bypass architectural boundaries, ownership, Communication semantics, or information flow.

### 8.4 Governance Rule 4 — Own Extension Information, Do Not Own Framework Information

An Extension Module shall own information that exists solely for its extension-specific responsibility. It shall not own framework information or responsibilities assigned to the Framework Core.

### 8.5 Governance Rule 5 — Preserve Parent Specifications

The Specification Dependency Rule defined by `Specification_Governance.md` applies to every Extension Module and its extension-specific refinement. Every applicable parent specification shall remain authoritative and preserved.

### 8.6 Governance Rule 6 — Preserve Implementation Independence

The Framework Core shall remain implementation-agnostic, backend-agnostic, and engine-agnostic regardless of the presence, absence, or implementation of any Extension Module.

An implementation choice made by an Extension Module shall not become a Framework Core requirement.

### 8.7 Governance Rule 7 — Admit by Formulation Completeness, Not Participation

A proposed extension shall be admitted as an ordinary Extension Module only when the applicable thermodynamic formulation and claimed scope remain complete under the existing Thermodynamic State semantics and Framework Core responsibilities with permitted Framework Interface communication.

Participation, dependency, or cross-domain coupling shall not substitute for this determination. When formulation completeness cannot be preserved without changing authoritative Thermodynamic State, a Framework Core responsibility, or the applicable thermodynamic formulation, the change shall be governed as an explicit specification revision or the unsupported scope shall be excluded.

## 9. Relationship to Conformance and Validation

Framework Conformance does not require the presence of an Extension Module. The Framework Core remains complete without Extension Modules.

When an implementation includes an Extension Module, the applicable Extension requirements, including ordinary-extension admissibility when applicable, form part of the Framework Conformance determination. The presence of an Extension Module does not by itself establish or invalidate Framework Conformance.

Future Validation documents may provide evidence relevant to Framework Conformance and stated Validation purposes. Extension-specific Validation shall not redefine the Extension semantics, ownership boundaries, admissibility requirements, Communication requirements, or governance rules established by this document.

## 10. Document Status

This document is the authoritative normative Framework Specification for Extension Module semantics, admissibility, and boundaries within ThermoCore, derived from `Framework_Principles.md`, `Core_Architecture.md`, `Data_Flow.md`, `Thermodynamic_State.md`, `Material_Representation.md`, `Framework_Interfaces.md`, and `Specification_Governance.md`.

Later specifications may refine Extension Module semantics within their assigned scope but shall not redefine the Framework Core, Extension Modules, extension ownership, extension admissibility, extension Communication, or the boundaries established by the parent specifications and this document.

Extension implementations, algorithms, APIs, programming interfaces, numerical formulations, rendering systems, execution mechanisms, backend-specific behavior, and Unity-specific concepts are intentionally outside the scope of this specification.
