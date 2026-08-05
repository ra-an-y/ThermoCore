# Core Architecture

Version: 1.0  
Status: Normative Specification

---

## Normative Dependencies

Parent Specification:

- `Framework_Principles.md`

This document conforms to the parent specification and refines only the normative decomposition, responsibilities, ownership, and boundaries of the Core Architecture.

## 1. Purpose

This document defines the normative architectural decomposition of ThermoCore. It assigns responsibilities, ownership, boundaries, and relationships to the framework's core architectural components.

This document defines architecture only. It does not prescribe implementation details, numerical algorithms, APIs, storage layouts, execution backends, or backend-specific behavior.

## 2. Architectural Overview

ThermoCore consists of four normative architectural components:

```text
                 ThermoCore
                      │
 ┌────────────────────┼────────────────────┐
 │                    │                    │
 │          ┌─────────┴─────────┐          │
 │          │                   │          │
 ▼          ▼                   ▼          ▼
Thermodynamic   Thermodynamic   Material   Framework
Computation     State           Representation
                                           Interfaces
```

This figure is conceptual. It identifies architectural membership and does not represent runtime execution, data transfer, precedence, or configuration flow.

The four components are:

- Thermodynamic Computation;
- Thermodynamic State;
- Material Representation; and
- Framework Interfaces.

Together, these components define the framework core. A Representation Consumer is outside the framework core.

## 3. Core Components

### 3.1 Thermodynamic Computation

Thermodynamic Computation is responsible for thermodynamic evolution and state update.

Thermodynamic Computation shall:

- determine the evolution of Thermodynamic State; and
- perform state updates in accordance with the applicable Framework Specification.

Thermodynamic Computation shall not:

- perform rendering;
- own representation responsibilities; or
- own Material Definition or other reusable configuration.

### 3.2 Thermodynamic State

Thermodynamic State is responsible for representing the evolving thermodynamic condition governed by the framework.

Thermodynamic State shall:

- represent evolving thermodynamic state; and
- remain available to the framework responsibilities that are permitted to consume it.

Thermodynamic State shall not:

- contain responsibility for its own evolution;
- define state-update logic; or
- perform rendering.

### 3.3 Material Representation

Material Representation is responsible for interpreting Thermodynamic State and applicable material information for downstream representation.

Material Representation shall:

- interpret thermodynamic and material information; and
- provide representation for a Representation Consumer through applicable Framework Interfaces.

Material Representation shall not:

- perform Thermodynamic Computation;
- own evolving Thermodynamic State; or
- modify Thermodynamic State.

### 3.4 Framework Interfaces

Framework Interfaces are responsible for communication between framework responsibilities and for supplying configuration information across defined architectural boundaries.

Framework Interfaces shall:

- provide the communication boundaries among core components; and
- supply Material Definition and other applicable configuration information to the responsibilities that require it.

Framework Interfaces shall not:

- perform numerical computation;
- perform Material Representation logic; or
- own evolving Thermodynamic State.

## 4. Component Responsibilities

Each architectural responsibility shall have exactly one owner. No responsibility shall belong simultaneously to multiple core components.

| Core component | Owned architectural responsibility | Responsibilities explicitly not owned |
|---|---|---|
| Thermodynamic Computation | Thermodynamic evolution and state update | Rendering, Material Representation, configuration ownership |
| Thermodynamic State | Representation of evolving thermodynamic state | State-evolution logic, rendering |
| Material Representation | Interpretation of thermodynamic and material information | Thermodynamic Computation, ownership or modification of evolving Thermodynamic State |
| Framework Interfaces | Communication across framework boundaries and supply of configuration information | Numerical computation, Material Representation logic, ownership of evolving Thermodynamic State |

Communication through Framework Interfaces does not transfer ownership. Access to information does not imply ownership of the responsibility that produces or governs that information.

## 5. Component Relationships

The components interact through explicitly separated runtime and configuration relationships.

### 5.1 Runtime Relationship

The conceptual runtime dependency is:

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
```

The arrows express conceptual dependency. Communication across these dependencies shall occur through applicable Framework Interfaces.

Energy Input is consumed by Thermodynamic Computation. Thermodynamic Computation evolves Thermodynamic State. Material Representation interprets Thermodynamic State and applicable material information.

This relationship identifies architectural dependency only. The detailed runtime flow is specified by `Data_Flow.md`.

### 5.2 Configuration Relationship

Configuration is supplied separately from runtime evolution:

```text
Material Definition
      │
      ▼
Framework Interfaces
      ├── Thermodynamic Computation
      └── Material Representation
```

Material Definition is configuration. It is not Energy Input and does not own evolving Thermodynamic State.

Framework Interfaces mediate both runtime communication and configuration supply while preserving their distinct meanings. Configuration flow shall not be treated as runtime state evolution.

## 6. Architectural Boundaries

The following boundaries are normative:

- Thermodynamic State shall not evolve itself.
- Thermodynamic State shall be evolved only by Thermodynamic Computation.
- Material Representation shall not modify Thermodynamic State.
- Material Representation shall not perform thermodynamic state evolution.
- Framework Interfaces shall not own evolving Thermodynamic State.
- Framework Interfaces shall not absorb the computation or representation responsibilities that they connect.
- Material Definition shall remain reusable configuration and shall not own per-location evolving Thermodynamic State.
- A Representation Consumer shall remain outside the framework core.
- Communication across a boundary shall not merge the responsibilities on either side of that boundary.

These boundaries apply independently of implementation technique, numerical method, execution backend, or Representation Consumer.

## 7. Architectural Invariants

A conforming architecture shall preserve all of the following invariants:

1. Thermodynamic State evolves only through Thermodynamic Computation.
2. Thermodynamic Computation remains separate from Material Representation.
3. Material Definition never owns evolving runtime Thermodynamic State.
4. Material Representation never owns or modifies evolving Thermodynamic State.
5. Framework Interfaces never own the responsibilities or evolving state that they connect.
6. Representation Consumers never belong to the framework core.
7. Each core architectural responsibility has exactly one owner.
8. Core responsibilities remain architecturally independent even when they communicate through Framework Interfaces.
9. Extension Modules shall not violate, duplicate, or reassign core architectural ownership.
10. No implementation choice shall redefine the four normative core components.

Violation of any invariant constitutes architectural non-conformance.

## 8. Relationship to Subsequent Specifications

This document is the parent architectural specification for:

- `Data_Flow.md`;
- `Thermodynamic_State.md`;
- `Material_Representation.md`;
- `Framework_Interfaces.md`; and
- `Extension_Boundary.md`.

Those documents further specify the behavior, information boundaries, and constraints of individual architectural components and relationships. They shall not redefine the four core components, transfer their ownership, or contradict the boundaries and invariants established here.

## 9. Document Status

This document is a normative Framework Specification derived from `Framework_Principles.md`.

It defines the Core Architecture to which later Framework Specification documents and conforming implementations shall adhere. Later specifications shall conform to this document and may refine its components only within the ownership and boundaries defined here.
