# Material Representation

Version: 1.0  
Status: Normative Specification

---

## 1. Normative Dependencies

Parent Specifications:

- `Framework_Principles.md`
- `Core_Architecture.md`
- `Data_Flow.md`
- `Thermodynamic_State.md`

This document conforms to the parent specifications and refines only the normative semantics, responsibilities, ownership, lifecycle, classification, and interpretation principles of Material Representation. It shall not redefine the architecture, information flow, Thermodynamic State, responsibilities, ownership boundaries, or invariants established by the parent specifications.

## 2. Purpose

This document defines what Material Representation means within ThermoCore. Material Representation is the architectural responsibility that interprets Thermodynamic State and applicable Material Definition to produce Representation for downstream use.

Material Representation is not Runtime State or Configuration. Representation is the information produced and owned by Material Representation.

This document defines the Material Representation responsibility and Representation semantics only. It shall not define rendering techniques, shaders, numerical variables, physical fields, material asset formats, storage structures, APIs, backend-specific representations, or implementation procedures.

## 3. Material Representation Overview

Material Representation is the framework responsibility that interprets Thermodynamic State for downstream consumption. Interpretation may incorporate applicable Material Definition when material information is required to establish the meaning supplied to a Representation Consumer.

The conceptual interpretation relationship is:

```text
Thermodynamic State
        │
        ▼
Material Representation
        │
        ▼
Representation Consumer
```

This figure represents interpretation only. It does not define rendering order, execution scheduling, synchronization, an implementation pipeline, or a backend mechanism. Communication across these relationships shall occur through applicable Framework Interfaces.

Material Representation reads Thermodynamic State and applicable material information without owning or modifying Thermodynamic State. It supplies Representation for downstream consumption without making the Representation Consumer part of the framework core.

Representation preserves an interpretive relationship to the Thermodynamic State and applicable Material Definition from which it is produced. It shall not be treated as a substitute for either source.

### 3.1 Representation Semantics

The following concepts are fundamentally different:

- **Runtime State** represents the current thermodynamic condition governed by the framework.
- **Configuration** represents reusable framework information, including applicable Material Definition, supplied without becoming evolving state.
- **Representation** represents interpretation of Thermodynamic State and applicable Material Definition for downstream consumption.

Configuration shall not be classified as Representation merely because it participates in interpretation. Runtime State shall not be classified as Representation merely because Material Representation reads it. Representation shall not be classified as Runtime State merely because it is informed by the current thermodynamic condition.

The identity of Representation is independent of any rendering technique, storage format, API, backend, or implementation.

## 4. Representation Ownership

Ownership of Representation shall remain unique.

The normative ownership relationships are:

- Material Representation owns Representation.
- Thermodynamic State owns Runtime State.
- Thermodynamic Computation owns State Evolution.
- Framework Interfaces communicate Representation without owning it.

Ownership of Representation and ownership of its source information are distinct. Material Representation owns the responsibility for interpretation and the resulting Representation; it does not acquire ownership of Thermodynamic State or Material Definition through that interpretation.

Reading, communicating, supplying, or consuming Representation does not transfer ownership. A Representation Consumer consumes Representation without owning Material Representation or any framework-core responsibility.

No Framework Interface, Representation Consumer, Configuration, or Extension Module shall acquire ownership of Representation through access alone.

## 5. Representation Classification

Representation has two normative classifications: Persistent Representation and Derived Representation.

### 5.1 Persistent Representation

Persistent Representation is Representation maintained across framework operation when required by applicable specifications.

Information shall be classified as Persistent Representation only when maintaining the Representation is required for conforming downstream interpretation or consumption. This classification defines semantic continuity and does not prescribe storage duration, storage location, data structure, asset format, or backend representation.

Persistent Representation remains Representation. Its maintenance shall not convert it into Runtime State or Configuration.

### 5.2 Derived Representation

Derived Representation is Representation produced from Runtime State and applicable Material Definition when required.

Derived Representation remains semantically dependent on the information it interprets. It shall not be treated as an independently owned substitute for Thermodynamic State or Material Definition and shall not create a second owner of either source.

The distinction between Persistent Representation and Derived Representation is normative. The specific Representation assigned to either classification depends on applicable later specifications; this document does not enumerate rendering assets, physical variables, fields, or implementation forms.

## 6. Representation Lifecycle

The conceptual lifecycle of Representation is:

```text
Creation
    │
    ▼
Interpretation
    │
    ▼
Consumption
    │
    ▼
Termination
```

This diagram represents conceptual lifecycle only. It does not define implementation order, execution scheduling, synchronization, rendering stages, APIs, or backend behavior.

### 6.1 Creation

Creation establishes the framework-governed representation context within the ownership and constraints of the Framework Specification. It does not imply that interpreted Representation already exists.

Creation does not prescribe allocation, initialization, authoring, loading, conversion, or storage procedures.

### 6.2 Interpretation

Interpretation establishes downstream meaning from Thermodynamic State and applicable Material Definition.

Interpretation belongs to Material Representation. It does not perform State Evolution, modify Thermodynamic State, or convert Configuration into Runtime State.

### 6.3 Consumption

Consumption makes Representation available to a Representation Consumer through applicable Framework Interfaces.

Consumption does not imply modification, ownership transfer, State Evolution, or membership in the framework core. A Representation Consumer remains outside the framework core.

### 6.4 Termination

Termination ends the framework-governed lifecycle of Representation.

Termination does not prescribe deallocation, destruction, persistence, serialization, asset disposal, or any other implementation mechanism.

The lifecycle stages describe semantic conditions. They do not require a single sequential execution path or define a rendering or execution pipeline.

## 7. Representation Constraints

The following constraints are normative:

1. Material Representation shall remain the architectural responsibility for interpretation.
2. Representation shall remain distinct from Runtime State and Configuration.
3. Material Representation shall own Representation.
4. Material Representation shall not own Runtime State.
5. Material Representation shall not modify Thermodynamic State.
6. Material Representation shall not perform State Evolution.
7. Representation shall not become or replace Thermodynamic State.
8. Representation shall not become or replace Material Definition.
9. Configuration shall not be classified as Representation.
10. Framework Interfaces shall communicate Representation without owning it.
11. Communication through Framework Interfaces shall not transfer ownership.
12. A Representation Consumer may consume Representation without becoming part of the framework core.
13. A Representation Consumer shall not modify or redefine the Framework Core.
14. Consumption of Representation shall not grant ownership of Material Representation.
15. Persistent Representation shall remain distinct from Runtime State and Configuration.
16. Derived Representation shall not replace the source information from which it is produced.
17. An Extension Module shall not redefine, duplicate, or reassign Representation ownership.
18. An Extension Module shall not use Representation to bypass the ownership or flow constraints of the parent specifications.

These constraints are conceptual and apply independently of implementation technique, rendering system, numerical formulation, storage model, execution backend, or Representation Consumer.

Violation of any applicable constraint constitutes non-conformance with this specification.

## 8. Relationship to Subsequent Specifications

This document provides normative Material Representation semantics for:

- `Framework_Interfaces.md`;
- `Extension_Boundary.md`; and
- `Framework_Conformance.md`.

`Framework_Interfaces.md` may refine how Representation and its applicable source information are communicated while preserving their ownership and semantics.

`Extension_Boundary.md` may refine how Extension Modules interact with or provide extension-specific Representation without redefining Material Representation or reassigning its ownership.

`Framework_Conformance.md` defines how Conformance with Material Representation semantics, ownership, classification, lifecycle, and constraints is determined. Future Validation documents may define how evidence of that Conformance is provided.

Later specifications may refine communication, extension behavior, Conformance, and Validation within their assigned responsibilities. They shall not redefine Material Representation, transfer its ownership, merge it with Runtime State or Configuration, or contradict the semantics and constraints established by this document.

## 9. Document Status

This document is a normative Framework Specification derived from `Framework_Principles.md`, `Core_Architecture.md`, `Data_Flow.md`, and `Thermodynamic_State.md`.

It is the authoritative specification for the semantics of Material Representation. Later specifications may reference these semantics but shall not redefine them.

Conforming implementations and subsequent Framework Specification documents shall preserve the responsibilities, ownership, classification, lifecycle, interpretation principles, and constraints defined here.

This specification defines Representation semantics only. Rendering techniques, shaders, material asset formats, numerical variables, physical fields, storage structures, APIs, backend-specific representations, and implementation details are intentionally outside its scope.
