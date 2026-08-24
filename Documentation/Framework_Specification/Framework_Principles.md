# Framework Principles

Version: 1.1  
Status: Normative Specification

---

## 1. Purpose

This document establishes the highest-level normative architectural principles for ThermoCore and the responsibilities, boundaries, and relationships that all subsequent Framework Specifications shall preserve.

ThermoCore is engine-agnostic, implementation-agnostic, GPU-oriented, and reusable. GPU-oriented describes an architectural objective and does not require any particular GPU API, execution model, or backend.

This specification defines architectural principles without prescribing a particular implementation or numerical method.

## 2. Scope

ThermoCore specifies architectural responsibilities for:

- thermodynamic computation;
- Thermodynamic State;
- Material Representation;
- Framework Interfaces; and
- extension boundaries.

The Framework Specification defines how these responsibilities are separated and related. It does not define rendering engines, numerical optimization techniques, engine-specific implementations, API signatures, data layouts, or solver mathematics.

## 3. Design Goals

ThermoCore has the following architectural goals:

1. Separate thermodynamic computation from Material Representation.
2. Keep Thermodynamic Computation independent of material-specific simulation logic.
3. Preserve engine-agnostic and implementation-agnostic architecture.
4. Maintain a stable framework core with explicit responsibilities.
5. Support extensions through explicitly defined coupling boundaries.
6. Enable reuse across conforming implementations and Representation Consumers.

These goals define architectural direction. They do not establish performance, accuracy, or universal-applicability claims.

## 4. Non-goals

ThermoCore is not:

- a rendering engine;
- a computational fluid dynamics framework;
- a finite element method framework;
- a general multiphysics framework;
- a game engine; or
- a prescription for one numerical algorithm or implementation backend.

Possible future extensions are not part of the current framework scope merely because they may interact with thermodynamic phenomena. A future mechanism shall remain outside the normative core until it has completed the applicable research and framework-decision process.

## 5. Core Principles

### 5.1 Separation of Computation and Representation

Thermodynamic Computation and Material Representation shall remain separate architectural responsibilities.

Thermodynamic Computation shall evolve Thermodynamic State. Material Representation shall interpret thermodynamic and material information for downstream use without assuming ownership of thermodynamic state evolution.

### 5.2 Minimal Persistent State

An implementation shall persist only the Thermodynamic State required by its conforming thermodynamic formulation and declared Framework Interfaces.

Quantities that can be derived without violating the specification should not be treated as independently owned persistent state. This principle does not prescribe one universal set of stored variables or prohibit extension-owned state.

### 5.3 Explicit State Ownership

Every evolving state category shall have an explicit architectural owner.

Material Definition shall not own per-location evolving Thermodynamic State. Extensions shall own state that exists solely for their mechanisms unless a later normative specification assigns that state elsewhere.

### 5.4 Material-independent Computation

The thermodynamic computational core shall not embed material-specific simulation procedures. Material-dependent information required by computation shall be supplied through specified Framework Interfaces.

Material independence does not mean that thermodynamic results are independent of material properties.

### 5.5 Stable Core Architecture

Optional capabilities shall not alter the core architectural responsibilities solely for implementation convenience. A change to the core requires a normative specification change supported by the framework governance process.

## 6. Conceptual Architecture

ThermoCore distinguishes runtime flow from configuration flow.

### 6.1 Runtime Flow

```text
Energy Input
      ↓
Thermodynamic Computation
      ↓
Thermodynamic State
      ↓
Material Representation
      ↓
Representation Consumer
```

Energy Input is runtime input to Thermodynamic Computation. Thermodynamic Computation evolves Thermodynamic State. Material Representation uses Thermodynamic State and applicable material information to provide representation for a Representation Consumer.

### 6.2 Configuration Flow

```text
Material Definition
      ↓
Framework Interfaces
      ├──→ Thermodynamic Computation
      └──→ Material Representation
```

Material Definition is reusable configuration. It supplies material-referenced information through Framework Interfaces and shall remain distinct from Energy Input and evolving Thermodynamic State.

The configuration flow does not prescribe how a Material Definition is authored, stored, transformed, or delivered at runtime.

## 7. Framework Output and Representation

The primary Framework Output is Thermodynamic State.

Material Representation is not a substitute for Thermodynamic State. It is the architectural responsibility that interprets state and material information for downstream consumption.

A Representation Consumer uses framework-provided state or representation without becoming part of thermodynamic computation. Representation Consumers may include:

- a heatmap;
- CSV output;
- a graphical user interface;
- a renderer; or
- a Reference Application.

Graphical output is therefore not the primary output of ThermoCore, and a Representation Consumer is not part of the framework core merely because it displays or exports framework results.

## 8. Extension Philosophy

The core contains broadly applicable thermodynamic computation, Thermodynamic State responsibilities, Material Representation responsibilities, and the interfaces that connect them.

An Extension Module shall couple to the core only through one or more declared mechanisms:

- property updates;
- source terms; or
- extension-owned state.

An extension shall not modify the core thermodynamic architecture unless the governing conservation equations fundamentally change. If such a change is required, it shall be evaluated as a framework change rather than treated as an ordinary extension.

The identification of a possible Extension Module does not place that mechanism within the current Framework Specification.

Extension Modules are optional and are not required for Framework Conformance.

## 9. Verification and Validation Philosophy

Verification and Validation activities provide evidence relevant to Framework Conformance and to their explicitly stated Verification or Validation purposes.

Such activities may evaluate, within their declared scopes:

- architectural Conformance concerns;
- Thermodynamic State evolution;
- phase behavior;
- energy consistency; and
- other implementation or physical questions appropriate to the stated evidence purpose.

The Framework Specification does not require one fixed repository naming scheme or a permanent numbered Validation sequence for these evidence activities.

These activities may evaluate framework-defined behavior, formulation behavior, or stated physical Validation purposes within their declared scope. They do not verify or validate:

- Unity scenes;
- Unity Physics;
- rendering quality; or
- engine-specific behavior.

An engine-specific test environment may be used as an implementation backend, but its behavior is not the subject of Framework Conformance.

Verification determines whether an implementation satisfies specified requirements. Validation evaluates whether the implemented thermodynamic behavior is adequate for the stated Validation purpose. Validation Evidence produced by these activities may support a Framework Conformance determination; Verification, Validation, and Validation Evidence do not define or replace Framework Conformance. Neither activity converts a backend-specific feature into a framework requirement.

## 10. Conformance

An implementation conforms to ThermoCore only when it satisfies all applicable normative requirements in the Framework Specification.

Conformance shall not depend on Unity, Unreal, Vulkan, CUDA, DirectX, Compute Shader, or any other specific engine, API, language, hardware vendor, or implementation technique.

Use of a particular backend does not by itself establish Conformance. Likewise, an implementation is not non-conforming merely because it uses a backend different from a reference implementation.

Reference Applications and Representation Consumers may demonstrate or consume a conforming implementation, but they do not define Framework Conformance.

## 11. Document Status

This document is the root normative specification for ThermoCore. All subsequent Framework Specification documents shall conform to it.

Subsequent documents shall define the Core Architecture, Data Flow, Thermodynamic State, Material Representation, Framework Interfaces, and Extension Boundary at progressively greater specificity.

Implementation details are intentionally excluded from this document. They may be defined in implementation documentation only when they do not replace or contradict normative Framework Specification requirements.

Research findings and candidate ideas are non-normative. They shall remain in Research until they complete the required evidence and framework-decision process and are formally adopted into the Framework Specification.
