# Data Flow

Version: 1.0  
Status: Normative Specification

---

## 1. Normative Dependencies

Parent Specifications:

- `Framework_Principles.md`
- `Core_Architecture.md`

This document conforms to the parent specifications and refines only the normative movement, supply, consumption, and ownership semantics of information. It shall not redefine architectural components, responsibilities, ownership, boundaries, or invariants established by the parent specifications.

## 2. Purpose

This document defines the normative movement of information through ThermoCore. It specifies runtime information flow, configuration information flow, information ownership, and constraints on those flows.

This document specifies information flow only. It does not define implementation details, execution order, execution scheduling, algorithms, synchronization mechanisms, APIs, storage structures, data layouts, or backend-specific pipelines.

The relationships Read, Write, Consume, Supply, and Own describe information semantics only. They shall not be interpreted as API operations or implementation mechanisms.

## 3. Information Flow Overview

ThermoCore separates runtime information flow from configuration information flow.

### 3.1 Runtime Information Flow

```text
Energy Input
      │
      ▼
Thermodynamic Computation
      │
      ▼
Thermodynamic State
      │
      ▼
Material Representation
      │
      ▼
Representation Consumer
```

This figure expresses conceptual information dependency only. It does not define execution order, scheduling, synchronization, or a backend pipeline.

Communication across these dependencies shall occur through applicable Framework Interfaces. The arrows do not imply that information bypasses Framework Interfaces.

### 3.2 Configuration Information Flow

```text
Material Definition
      │
      ▼
Framework Interfaces
      ├──► Thermodynamic Computation
      └──► Material Representation
```

This figure expresses conceptual configuration supply only. It does not define execution order, scheduling, transformation, storage, or delivery mechanisms.

Runtime information and configuration information shall retain distinct meanings even when both are communicated through Framework Interfaces.

## 4. Runtime Information Flow

Runtime information flow concerns information that participates in the evolution or downstream interpretation of Thermodynamic State.

An Energy Source is an external provider of Energy Input. It is not a Framework Core component.

The normative runtime relationships are:

1. Energy Input is supplied to and consumed by Thermodynamic Computation.
2. Thermodynamic Computation reads applicable input and configuration information.
3. Thermodynamic Computation writes Thermodynamic State.
4. Material Representation reads Thermodynamic State and applicable material information.
5. Material Representation supplies representation to a Representation Consumer.
6. A Representation Consumer consumes representation only and remains outside the framework core.

Thermodynamic State shall evolve only through writes performed by Thermodynamic Computation. Reading Thermodynamic State shall not grant permission to write or own it.

Material Representation shall not write Thermodynamic State. A Representation Consumer shall not write Thermodynamic State, perform Thermodynamic Computation, or acquire ownership of a framework-core responsibility by consuming representation.

These relationships define information dependencies. They do not require that the relationships occur in a single sequence, at a particular frequency, or through a particular execution model.

## 5. Configuration Information Flow

Configuration information flow shall remain separate from runtime information flow.

Material Definition supplies reusable material information through applicable Framework Interfaces to:

- Thermodynamic Computation, where material information is required for thermodynamic evolution; and
- Material Representation, where material information is required for interpretation.

Material Definition is configuration. It is not Energy Input, Thermodynamic State, or representation.

Material Definition shall never own evolving Thermodynamic State. Supplying material information does not transfer ownership of Material Definition, Thermodynamic State, Thermodynamic Computation, or Material Representation.

Framework Interfaces supply configuration information across defined architectural boundaries. They shall not reinterpret configuration as runtime state, own evolving Thermodynamic State, or absorb the responsibilities of the components that receive configuration.

This specification does not define how Material Definition is authored, stored, transformed, transported, or made available to a conforming implementation.

## 6. Information Semantics

The following relationships define normative information semantics only:

- **Read** means obtaining information without modifying it. Read does not imply Write or Own.
- **Write** means modifying information within an explicitly assigned architectural responsibility. Write does not imply ownership beyond that assigned responsibility.
- **Consume** means receiving information for downstream use. Consume does not imply modification or ownership of the information source.
- **Supply** means making information available across an applicable architectural boundary. Supply does not imply ownership of the receiving component or responsibility.
- **Own** means holding the architectural responsibility that governs information. Own is not established by Read, Write, Consume, or Supply alone.

Accordingly:

- Read does not imply Write.
- Supply does not imply Ownership.
- Consume does not imply Modification.

These relationships shall remain distinct and shall not be mapped by this specification to APIs, storage operations, synchronization mechanisms, or backend behavior.

## 7. Information Ownership

Information ownership identifies the architectural responsibility that governs information. Reading, writing, consuming, or supplying information does not by itself transfer ownership.

| Information | Owner | Permitted relationship |
|---|---|---|
| Energy Input | Energy Source | Supply to Thermodynamic Computation |
| Thermodynamic State | Thermodynamic State | Written by Thermodynamic Computation; read by permitted framework responsibilities |
| Material Definition | Material Definition | Supply configuration through Framework Interfaces |
| Representation | Material Representation | Supply to a Representation Consumer |

The owner of Thermodynamic State is the Thermodynamic State component defined by `Core_Architecture.md`; responsibility for evolving and writing that state belongs exclusively to Thermodynamic Computation.

Framework Interfaces communicate information without owning the information or the responsibilities that produce, govern, or consume it.

A Representation Consumer consumes representation without owning Material Representation or any framework-core responsibility.

Supplying information does not imply ownership of the receiving component. Reading information does not imply ownership of the information read. Consuming information does not imply permission to modify its source.

## 8. Flow Constraints

The following constraints are normative.

### 8.1 Thermodynamic Computation

Thermodynamic Computation:

- may read Energy Input;
- may read applicable information supplied from Material Definition through Framework Interfaces;
- may write Thermodynamic State;
- shall not transfer ownership of Thermodynamic State; and
- shall not own Material Definition or representation.

### 8.2 Thermodynamic State

Thermodynamic State:

- may be written only by Thermodynamic Computation;
- may be read by Material Representation and other responsibilities permitted by subsequent conforming specifications;
- shall not evolve itself; and
- shall not be interpreted as configuration.

### 8.3 Material Representation

Material Representation:

- may read Thermodynamic State;
- may read applicable information supplied from Material Definition through Framework Interfaces;
- may supply representation to a Representation Consumer;
- shall not modify Thermodynamic State; and
- shall not perform Thermodynamic Computation.

### 8.4 Representation Consumer

A Representation Consumer:

- may consume representation;
- shall remain outside the framework core;
- shall not modify Thermodynamic State;
- shall not modify or redefine the Framework Core; and
- shall not acquire ownership of Material Representation by consuming representation.

### 8.5 Material Definition and Framework Interfaces

Material Definition and Framework Interfaces:

- may supply applicable configuration information;
- shall not be interpreted as runtime Thermodynamic State;
- shall not own evolving Thermodynamic State;
- shall not convert configuration supply into state-evolution responsibility; and
- shall preserve the architectural ownership defined by `Core_Architecture.md`.

### 8.6 General Constraints

A conforming information flow shall preserve all of the following:

1. Runtime and configuration information remain semantically distinct.
2. Every write to Thermodynamic State belongs to Thermodynamic Computation.
3. Read, Write, Consume, Supply, and Own remain distinct relationships.
4. Communication through Framework Interfaces does not transfer ownership.
5. No information relationship merges or reassigns core architectural responsibilities.
6. No flow relationship implies an execution order, synchronization mechanism, API, storage structure, or backend pipeline.

Violation of these constraints constitutes non-conformance with this specification.

## 9. Relationship to Subsequent Specifications

This document provides normative information-flow constraints for:

- `Thermodynamic_State.md`;
- `Material_Representation.md`;
- `Framework_Interfaces.md`; and
- `Extension_Boundary.md`.

`Thermodynamic_State.md` may refine the semantics and permitted access of Thermodynamic State.

`Material_Representation.md` may refine how Material Representation reads thermodynamic and material information and supplies representation.

`Framework_Interfaces.md` may refine the information boundaries through which runtime and configuration information are communicated.

`Extension_Boundary.md` may refine how Extension Modules read, write, consume, supply, or own extension-related information without violating core ownership.

Later specifications may refine information semantics within their assigned responsibilities. They shall not redefine the normative runtime flow, configuration flow, ownership relationships, or flow constraints established by this document.

## 10. Document Status

This document is a normative Framework Specification derived from `Framework_Principles.md` and `Core_Architecture.md`.

It defines the information flow to which later Framework Specification documents and conforming implementations shall adhere. Later specifications may refine information semantics only within the architectural ownership and boundaries established by the parent specifications and this document.

Implementation details remain outside the scope of this document. Conformance depends on preserving the specified information relationships and constraints, not on adopting a particular implementation, execution schedule, numerical method, API, storage layout, synchronization mechanism, or backend.

This specification defines normative information movement only. Execution scheduling, synchronization, implementation ordering, and backend pipelines are intentionally outside its scope.
