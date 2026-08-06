# Isolation Capability Matrix v0.2

Status: Under Survey  
Research Question: RQ-ISO-001  
Date: 2026-08-07

---

## 1. Objective

Determine whether ThermoCore's isolation model provides architectural guarantees not jointly provided by existing modular, multi-model, visualization-pipeline, or partitioned-coupling frameworks.

This matrix evaluates capabilities rather than surface-level features. A framework is not considered equivalent merely because it supports plugins, mappings, multiple representations, or coupled solvers.

The comparison focuses on whether authority, ownership, modification rights, extension state, communication boundaries, and core completeness are explicitly governed.

---

## 2. Scope

Initial comparison set:

- SOFA;
- MOOSE;
- preCICE;
- VTK; and
- ThermoCore.

This version uses official project documentation and ThermoCore normative specifications as the initial evidence base. It does not establish novelty or priority.

---

## 3. Assessment Scale

| Mark | Meaning |
|---|---|
| Y | Explicitly supported by available evidence |
| P | Partially supported, supported only for some component types, or materially different in scope |
| N | Available evidence indicates the capability is not part of the architecture being evaluated |
| U | Unknown; insufficient evidence |
| N/A | Not applicable to the framework's stated purpose |

A `Y` does not mean two frameworks implement the capability with equivalent semantics.

---

## 4. Capability Definitions

| ID | Capability | Evaluation Question |
|---|---|---|
| C1 | Authoritative State | Does the architecture identify one authoritative runtime state for the evaluated physical domain? |
| C2 | Explicit State Owner | Is responsibility for evolving that state assigned to a defined architectural owner? |
| C3 | Read Does Not Confer Ownership | Is consuming or reading information explicitly separated from owning or modifying it? |
| C4 | Representation Cannot Redefine State | Are representation components prohibited from redefining authoritative state semantics or evolution? |
| C5 | Extension-owned State | Can an extension own persistent state that exists only for its mechanism without expanding core state? |
| C6 | Core Complete Without Extensions | Is the core specified as complete and valid when every optional extension is absent? |
| C7 | Interface-governed Communication | Must extension-to-core communication use declared interfaces or coupling mechanisms? |
| C8 | Ownership Preservation Across Communication | Does communication preserve the original ownership and semantics of information? |
| C9 | State Growth Control | Does the architecture prevent optional mechanisms from automatically becoming mandatory core state? |
| C10 | Validation Boundary | Is there an explicit basis for separating core validation from extension- or module-specific validation? |
| C11 | Core Change Isolation | Can a new optional capability be added without redefining or modifying core responsibilities? |
| C12 | Normative Governance Constraints | Are restrictions such as shall-not-redefine, shall-not-replace, shall-not-own, or shall-not-bypass stated normatively? |

---

## 5. Isolation Capability Matrix

| Capability | SOFA | MOOSE | preCICE | VTK | ThermoCore | Current Interpretation |
|---|---:|---:|---:|---:|---:|---|
| C1 Authoritative State | P | P | N | N/A | Y | SOFA has a parent mechanical model driving mapped representations, but mapped models can contribute forces back. MOOSE has solver variables and FE systems, but no single universal state authority across all pluggable systems. preCICE deliberately preserves participant-owned states. VTK is a data-processing pipeline rather than a physical-state authority model. |
| C2 Explicit State Owner | P | P | Y | N/A | Y | preCICE participants own their solver states. SOFA assigns strong parent-child roles in mappings but supports bidirectional physical contribution. MOOSE assigns storage and assembly responsibilities but permits multiple physics objects to define the solved system. |
| C3 Read Does Not Confer Ownership | U | Y | P | P | Y | MOOSE Material consumers receive const references and cannot modify consumed properties. This protection applies to the Material-property consumer relationship, not necessarily every plugin relationship. |
| C4 Representation Cannot Redefine State | P | N/A | N/A | P | Y | SOFA visual and collision models are separate representations, but mapped force fields may contribute to the parent mechanical system. MOOSE physics objects are intended to define equation terms, so this restriction is not its goal. |
| C5 Extension-owned State | P | Y | Y | P | Y | MOOSE supports stateful Material properties; preCICE participants retain their own states. The unresolved distinction is whether such state is normatively prevented from being reclassified as core state. |
| C6 Core Complete Without Extensions | P | Y | Y | Y | Y | MOOSE's framework core is separate from optional physics modules. preCICE and VTK libraries remain usable without any one participant adapter or filter. SOFA uses plugins, but the exact conformance meaning of a plugin-free core remains under survey. |
| C7 Interface-governed Communication | Y | Y | Y | Y | Y | All evaluated systems use declared mappings, APIs, pipelines, or coupling interfaces. This capability alone is not a research gap. |
| C8 Ownership Preservation Across Communication | U | P | Y | P | Y | preCICE exchanges data while each participant retains solver ownership. MOOSE provides const property consumption but not a general normative ownership doctrine. VTK pipeline ownership semantics are implementation-level rather than physical-state governance. |
| C9 State Growth Control | U | P | Y | P | Y | MOOSE can store stateful properties only when requested, but they are stored over relevant quadrature points and can be memory intensive. ThermoCore explicitly permits extension-owned state while preserving minimal core state. |
| C10 Validation Boundary | U | Y | P | P | Y | MOOSE maintains framework and module-specific V&V structures. ThermoCore explicitly separates Framework Conformance from extension-specific validation. SOFA and preCICE require further evidence on formal validation-scope isolation. |
| C11 Core Change Isolation | P | Y | Y | Y | Y | Plugin, module, adapter, and filter architectures support adding capability without editing central framework code. ThermoCore adds the stronger condition that extension capability must not redefine core responsibilities. |
| C12 Normative Governance Constraints | U | U | U | N | Y | ThermoCore explicitly states shall-not-redefine, shall-not-replace, shall-not-own, and shall-not-bypass constraints. Equivalent normative rules have not yet been found in the surveyed frameworks. Absence has not been proven. |

---

## 6. Evidence Records

### ISO-E01 — SOFA Multi-model Representation and Mapping

**Evidence status:** Verified for multi-model mapping; Under Survey for ownership governance.

SOFA models physical, visual, and collision representations separately. Mappings maintain correspondence among these representations. A parent mechanical model can drive mapped child positions and velocities, while forces from a mapped child can be propagated back to the parent through the transpose Jacobian.

This establishes strong representation and resolution isolation, but it is not equivalent to a read-only representation boundary. Mapped force fields may affect the governing mechanical system.

**Supported capabilities:** C1 partial, C2 partial, C4 partial, C7 yes, C11 partial.

**Primary sources:**

- SOFA Documentation, `Mappings`: https://sofa-framework.github.io/doc/simulation-principles/multi-model-representation/mapping/
- SOFA, `Features`: https://www.sofa-framework.org/about/features/
- SOFA, `About SOFA`: https://www.sofa-framework.org/about-sofa/

**Open checks:**

1. Does SOFA define an authoritative-state owner beyond parent/child mapping semantics?
2. Does SOFA prohibit visual or collision components from redefining state semantics?
3. Does SOFA publish normative extension or plugin governance equivalent to ThermoCore's shall-not rules?
4. Is core validation scope formally preserved when plugins are added?

---

### ISO-E02 — MOOSE Pluggable Systems, Kernels, and Materials

**Evidence status:** Verified for pluggable systems, read-only Material consumption, optional modules, and stateful Material properties.

MOOSE uses a core plus pluggable systems. Developers specialize framework objects through defined polymorphic interfaces. Material-property consumers obtain const references and cannot modify consumed properties. MOOSE also supports old and older stateful Material properties, which are stored when requested and may substantially increase memory use.

However, MOOSE Kernels are intentionally pieces of physics. They contribute residual and Jacobian terms and therefore participate directly in defining the governing PDE system. MOOSE isolates equation terms as composable components; it does not impose ThermoCore's rule that an ordinary extension must not redefine the core thermodynamic responsibility.

**Supported capabilities:** C3 yes for Material consumers, C5 yes, C6 yes, C7 yes, C9 partial, C10 yes, C11 yes.

**Primary sources:**

- MOOSE, `System Design Description`: https://mooseframework.inl.gov/sqa/moose_sdd.html
- MOOSE, `Framework System Design Description`: https://mooseframework.inl.gov/moose/sqa/framework_sdd.html
- MOOSE, `Materials System`: https://mooseframework.inl.gov/moose/syntax/Materials/
- MOOSE, `Kernel`: https://mooseframework.inl.gov/source/kernels/Kernel.html

**Open checks:**

1. Is there a general ownership model beyond the Material producer-consumer API?
2. Are modules prohibited from redefining framework concepts, or are boundaries maintained primarily through API design and review practice?
3. Is revalidation scope formally derived from module boundaries?
4. Is there an explicit rule that optional module state shall never become framework state?

---

### ISO-E03 — preCICE Partitioned Coupling

**Evidence status:** Verified for participant isolation, coupling interfaces, data mapping, and participant-owned state.

preCICE couples separate solver participants through configured read/write data, coupling schemes, and mappings between nonmatching meshes. Each participant retains its own solver and state. The architecture therefore provides strong solver isolation and ownership preservation across participant boundaries.

This differs from ThermoCore because preCICE has no single central Thermodynamic State that all representations interpret. Its purpose is coordination among independently authoritative solvers.

**Supported capabilities:** C2 yes at participant level, C5 yes, C6 yes, C7 yes, C8 yes, C9 yes, C11 yes.

**Primary sources:**

- preCICE, `Configuration overview`: https://precice.org/configuration-overview
- preCICE, `Coupling scheme configuration`: https://precice.org/configuration-coupling
- preCICE, `Mapping configuration`: https://precice.org/configuration-mapping

**Open checks:**

1. Does preCICE define normative prohibitions against participant responsibility reclassification?
2. Is validation-scope isolation documented as an architectural guarantee?
3. How are data ownership and checkpoint rollback described in the participant API?

---

### ISO-E04 — VTK Data-processing and Rendering Pipeline

**Evidence status:** Preliminary; direct official pipeline sources require further extraction.

VTK separates data objects, algorithms or filters, mappers, and rendering consumers. This is a strong precedent for data-flow and representation separation. It does not ordinarily define physical-state evolution, conservation-law ownership, extension-owned physical history, or framework conformance.

VTK therefore serves as a control case: it demonstrates that pipeline modularity and replaceable consumers do not by themselves establish governed physical-state isolation.

**Supported capabilities:** C7 yes, C11 yes; other physical-state capabilities are not applicable or remain under survey.

**Open checks:**

1. Identify the current official architecture source for executive/data objects and pipeline update semantics.
2. Determine whether VTK documents data ownership in a way relevant to C3 or C8.
3. Confirm whether any simulation-oriented VTK subsystem adds physical-state governance.

---

### ISO-E05 — ThermoCore Normative Isolation Model

**Evidence status:** Verified at specification level; implementation benefits remain unverified.

ThermoCore specifies that Thermodynamic Computation evolves Thermodynamic State, while Material Representation interprets state and material information without owning state evolution. Extension Modules remain optional, communicate through applicable Framework Interfaces, own only extension-specific information, and shall not redefine, replace, own, or bypass Framework Core responsibilities and information boundaries.

The Framework Core must remain complete, valid, and conforming without any Extension Module.

**Supported capabilities:** C1–C12 at normative specification level.

**Primary sources:**

- `Documentation/Framework_Specification/Framework_Principles.md`
- `Documentation/Framework_Specification/Thermodynamic_State.md`
- `Documentation/Framework_Specification/Material_Representation.md`
- `Documentation/Framework_Specification/Framework_Interfaces.md`
- `Documentation/Framework_Specification/Extension_Boundary.md`
- `Documentation/Framework_Specification/Framework_Conformance.md`

**Unverified benefits:**

1. Zero Core files changed when adding meaningful extensions.
2. Reduced revalidation scope.
3. Reduced mandatory per-cell state growth.
4. Prevention of inconsistent duplicate thermodynamic state across consumers.
5. Lower maintenance or integration effort.

---

## 7. Preliminary Findings

### F-ISO-01 — Interface use is not distinctive

All surveyed frameworks use declared APIs, mappings, pipelines, or coupling mechanisms. C7 alone cannot support a Research Gap.

### F-ISO-02 — Multiple representations are not distinctive

SOFA already supports physical, visual, collision, and other representations connected through mappings. ThermoCore shall not claim novelty merely for allowing multiple Representation Consumers.

### F-ISO-03 — Read-only consumption exists elsewhere

MOOSE Material consumers receive const references and cannot modify consumed properties. ThermoCore shall not claim that `Read does not imply Write` is unique by itself.

### F-ISO-04 — Extension-owned history exists elsewhere

MOOSE supports stateful Material properties, and preCICE participants own independent solver states. Extension-owned history is not unique by itself.

### F-ISO-05 — Core-plus-module architecture exists elsewhere

MOOSE explicitly separates a framework core from optional physics modules. ThermoCore shall not claim novelty merely for having optional extensions.

### F-ISO-06 — The strongest unresolved distinction is normative authority governance

The current evidence has not found an equivalent framework that jointly and normatively requires all of the following:

1. one authoritative Thermodynamic State;
2. an explicit owner of state evolution;
3. representation as interpretation rather than state ownership;
4. extension-owned state that remains outside Runtime State;
5. core completeness without extensions;
6. communication that preserves ownership and semantics; and
7. explicit shall-not-redefine, shall-not-replace, shall-not-own, and shall-not-bypass constraints.

This is a candidate distinction, not a verified Research Gap.

---

## 8. Falsification Conditions

The Governed Isolation hypothesis shall be rejected or narrowed if an existing framework is found that explicitly provides equivalent rules for:

- authoritative physical-state ownership;
- representation non-ownership;
- extension-specific state ownership;
- ownership preservation through communication;
- prohibition of extension redefinition or replacement of the core;
- complete core conformance without extensions; and
- independently bounded validation responsibility.

Functional similarity is insufficient. The evidence must show equivalent semantics or governance.

---

## 9. Next Evidence Tasks

Priority order:

1. **SOFA governance deep review** — plugin model, mechanical state ownership, mapping responsibility, and validation boundaries.
2. **MOOSE ownership deep review** — Variable/System ownership, Material property ownership, module governance, and SQA dependency structure.
3. **preCICE participant API review** — data ownership, checkpointing, rollback, and validation responsibility.
4. **VTK pipeline primary-source review** — executive model, data ownership, and mutation semantics.
5. **Additional strongest counterexamples** — OpenMDAO, FMI/FMU co-simulation, OpenFOAM runtime selection, and simulation digital-twin frameworks.
6. **ThermoCore implementation experiment design** — three extension types: stateless representation, derived field, and history-dependent extension.

---

## 10. Current Classification

| Item | Classification |
|---|---|
| Multi-representation support | Verified prior art |
| Plugin or module extensibility | Verified prior art |
| Read-only property consumption | Verified prior art |
| Extension-owned historical state | Verified prior art |
| Solver isolation | Verified prior art |
| ThermoCore governed state-authority combination | Under Survey |
| Reduced Core changes | Unverified hypothesis |
| Reduced revalidation scope | Unverified hypothesis |
| Reduced mandatory state growth | Unverified hypothesis |
| Research Gap | Not yet established |

---

## 11. Interim Conclusion

ThermoCore's candidate distinction is not ordinary modularity, multiple representation, plugin support, or interface-based communication. Existing frameworks already provide these capabilities.

The remaining hypothesis is narrower:

> ThermoCore may provide governed state-authority isolation: optional representations and extensions can interact with an authoritative Thermodynamic State while explicit ownership and normative constraints prevent interaction from silently redefining the Framework Core, Runtime State, or Conformance boundary.

This hypothesis remains Under Survey and shall not enter the normative Framework Specification as a claimed research contribution until the falsification tasks and implementation evidence are complete.
