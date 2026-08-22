# Information Governance Evidence Matrix v0.1

Status: Under Survey  
Research Line: IG-001 — Information Governance in Simulation Frameworks  
Date: 2026-08-07

---

## 1. Research Question

> **RQ-IG-001:** To what extent do existing simulation frameworks explicitly govern information identity, semantic authority, operational ownership, permissions, responsibilities, flow, lifecycle, provenance, extension boundaries, conformance, and revalidation scope?

The purpose is to test whether simulation frameworks already integrate these concerns, not to assume that ThermoCore is unique.

---

## 2. Assessment Scale

| Mark | Meaning |
|---|---|
| Y | Explicit evidence supports the dimension |
| P | Partial, local, or scope-limited support |
| N | Evidence indicates the dimension is outside the architecture's purpose |
| U | Insufficient evidence |
| N/A | Not applicable |

A `Y` indicates presence, not semantic equivalence with ThermoCore.

---

## 3. Initial Matrix

| Governance Dimension | Data Governance / NIST–ISO | Software Architecture / SEI | MOOSE | Modelica | ThermoCore | Initial Interpretation |
|---|---:|---:|---:|---:|---:|---|
| IG-1 Information Identity | Y | P | Y | Y | Y | Data governance identifies governed assets; simulation frameworks define variables, properties, connectors, or state categories. |
| IG-2 Semantic Authority | Y | P | P | P | Y | Organizational governance defines authority clearly. Simulation frameworks define semantics through APIs and language rules, but often distribute physical meaning among models or equations. |
| IG-3 Operational Ownership | Y | Y | Y | P | Y | Runtime responsibility is usually assignable, though Modelica equation systems do not map neatly to one procedural owner. |
| IG-4 Access Permission | Y | Y | P | Y | Y | MOOSE has local const-consumer protections; Modelica defines connector causality and connection restrictions. General information permissions remain uneven. |
| IG-5 Responsibility Boundary | P | Y | Y | P | Y | Software architecture strongly supports responsibility allocation. The open question is whether simulation frameworks prevent silent semantic responsibility transfer. |
| IG-6 Information Flow | Y | Y | Y | Y | Y | All domains provide some flow, connection, or interface rules. This is not a distinctive capability by itself. |
| IG-7 Ownership Preservation | P | P | P | U | Y | Explicit preservation of ownership through communication is clearer in ThermoCore than in currently reviewed external sources, but absence is not proven. |
| IG-8 Extension Governance | P | Y | Y | P | Y | Existing frameworks constrain extension mechanisms. The unresolved issue is whether extensions are prevented from acquiring authority over core physical-state semantics. |
| IG-9 Information Lifecycle | Y | P | Y | P | P | Data governance covers creation through disposal. MOOSE supports historical properties. ThermoCore addresses persistence and extension state but not a complete lifecycle doctrine. |
| IG-10 Provenance and Traceability | Y | Y | Y | U | P | MOOSE provides strong requirement–design–test–V&V traceability. ThermoCore has research-to-specification traceability but machine-readable runtime provenance is not established. |
| IG-11 Conformance Boundary | Y | Y | Y | Y | Y | Standards, contracts, SQA requirements, language validity, and ThermoCore specifications all define some conformance boundary. |
| IG-12 Change and Revalidation Scope | P | Y | Y | U | P | MOOSE provides the strongest current evidence. ThermoCore defines separated validation responsibilities, but the causal reduction of revalidation scope remains unverified. |

---

## 4. Evidence Records

### IG-E01 — NIST Data Governance and Information Owner

**Status:** Verified prior art.

NIST defines data governance as formal management of data assets with authority and decision-making parameters. It defines an information owner as an authority responsible for controls over generation, collection, processing, dissemination, and disposal.

**Supports:** IG-2, IG-3, IG-4, IG-9, IG-11.

**Limitation:** Organizational governance roles are not automatically equivalent to software-component or simulation-state ownership.

Sources:

- https://csrc.nist.gov/glossary/term/data_governance
- https://csrc.nist.gov/glossary/term/information_owner

### IG-E02 — ISO Governance of Data and Information

**Status:** Verified at abstract and scope level.

ISO/IEC 38505-1 applies governance principles to the effective, efficient, acceptable use and protection of data. ISO/CD TS 17955.2 emphasizes accountable creation, use, maintenance, preservation, and disposition of data and information.

**Supports:** IG-2, IG-3, IG-9, IG-10, IG-11.

**Limitation:** The standards govern organizational use of information and do not directly prescribe simulation architecture.

Sources:

- https://www.iso.org/standard/56639.html
- https://www.iso.org/standard/87195.html
- https://www.iso.org/standard/85109.html

### IG-E03 — SEI Architecture Responsibility and Contracts

**Status:** Verified prior art.

SEI architecture methods allocate function and responsibility and define component interaction constraints. Design by Contract establishes formal conditions of use and behavioral obligations.

**Supports:** IG-3, IG-4, IG-5, IG-6, IG-11.

**Limitation:** These methods do not by themselves define ownership of physical information semantics.

Sources:

- https://www.sei.cmu.edu/library/the-architecture-based-design-method/
- https://www.sei.cmu.edu/library/psp-vdc-an-adaptation-of-the-psp-that-incorporates-verified-design-by-contract/

### IG-E04 — MOOSE SQA and Module Traceability

**Status:** Verified for requirements, design, tests, verification, validation, and module-specific quality records.

MOOSE provides application and module templates for requirements, design, traceability, verification, and validation. Its NQA-1 training explicitly links Change Request, Requirement, Design, and Test.

**Supports:** IG-10, IG-11, IG-12.

**Limitation:** Evidence does not yet show that information ownership boundaries themselves determine revalidation scope.

Sources:

- https://mooseframework.inl.gov/releases/moose/2024-03-08/python/MooseDocs/extensions/sqa.html
- https://mooseframework.inl.gov/sqa/training/ccb/index.html

### IG-E05 — Modelica Connector and Connection Governance

**Status:** Verified for connection semantics and restrictions.

Modelica defines connector identities, connection equations, type and dimension compatibility, flow and stream categories, and input/output restrictions. These are formal information-flow rules embedded in the language.

**Supports:** IG-1, IG-4, IG-6, IG-11.

**Limitation:** Modelica components jointly contribute equations to the complete model. This is not equivalent to preserving one external authoritative physical state against component redefinition.

Sources:

- https://specification.modelica.org/master/connectors-and-connections.html
- https://specification.modelica.org/maint/3.6/class-predefined-types-and-declarations.html

### IG-E06 — ThermoCore Normative Information Governance

**Status:** Verified at specification level; benefits unverified.

ThermoCore distinguishes Runtime State, Material Representation, Configuration, Framework Interfaces, Extension-owned Information, and Representation Consumers. It explicitly separates read, supply, communicate, consume, ownership, state evolution, and extension responsibility.

**Supports:** IG-1 through IG-8 and IG-11 at specification level. IG-9, IG-10, and IG-12 are partial and require stronger evidence or implementation mechanisms.

Primary repository sources:

- `Documentation/Framework_Specification/Framework_Principles.md`
- `Documentation/Framework_Specification/Data_Flow.md`
- `Documentation/Framework_Specification/Thermodynamic_State.md`
- `Documentation/Framework_Specification/Material_Representation.md`
- `Documentation/Framework_Specification/Framework_Interfaces.md`
- `Documentation/Framework_Specification/Extension_Boundary.md`
- `Documentation/Framework_Specification/Framework_Conformance.md`

---

## 5. Preliminary Findings

### IG-F01 — Information governance is established prior art

Authority, ownership, lifecycle, accountability, controls, provenance, and traceability are mature concepts in data governance and software quality systems. ThermoCore shall not claim to invent information governance.

### IG-F02 — Formal information-flow governance is established prior art

Modelica, FMI, OpenMDAO, MOOSE, and other frameworks already define formal connection, causality, interface, or producer–consumer rules. ThermoCore shall not claim novelty for explicit information flow alone.

### IG-F03 — Responsibility and contract governance are established prior art

Software architecture and Design by Contract already constrain component behavior and interaction. ThermoCore shall not claim novelty merely for shall-not rules or interface contracts.

### IG-F04 — The candidate contribution is application and integration

The remaining candidate is not a new governance primitive. It is the integration of governance concepts into an engine-agnostic thermodynamic simulation architecture where physical-state identity, semantic authority, runtime ownership, representation permissions, extension-owned information, and conformance boundaries are jointly specified.

### IG-F05 — ThermoCore is not yet complete against the taxonomy

ThermoCore appears strong in identity, authority, ownership, permissions, flow, extension governance, and conformance. It is weaker or incomplete in full lifecycle governance, runtime provenance, machine-readable ownership inspection, and evidence-based revalidation scope.

This finding supports future Change Request candidates but does not justify immediate normative modification.

---

## 6. Change Request Candidates

The following are non-normative candidates derived from evidence:

1. Define the distinction between semantic authority, operational ownership, and data source explicitly.
2. Add an informational ownership-and-permission table for each information category.
3. Define lifecycle expectations for extension-owned persistent information without prescribing storage layout.
4. Explore machine-readable reporting of owners, suppliers, consumers, and bypass violations.
5. Link specification changes to affected verification and validation evidence.
6. Keep execution scheduling and numerical-state selection outside information-governance semantics unless separately specified.

These candidates require the established Research → Evidence → Framework Decision process.

---

## 7. Falsification Priorities

1. Find simulation frameworks that explicitly distinguish semantic authority from operational ownership.
2. Find formal ownership-preservation rules across component communication.
3. Find frameworks that prevent optional model state from becoming mandatory core state without governance approval.
4. Find explicit information-lifecycle rules for physical simulation state.
5. Find architecture-driven revalidation-scope methods tied to information boundaries.

Priority targets:

- FMI state ownership and importer/FMUs;
- OpenMDAO vector ownership and promoted-variable authority;
- SOFA mechanical-object and mapping semantics;
- OpenFOAM field ownership and runtime-selected model authority;
- digital-twin authoritative-state architectures;
- systems-engineering digital thread and authoritative source-of-truth models.

---

## 8. Current Conclusion

The new research direction remains viable, but its scope is narrower than initially proposed.

> ThermoCore does not appear to introduce information governance as a general concept. Its possible contribution is a domain-specific integration of existing governance principles into a thermodynamic framework that explicitly separates physical-state semantic authority, runtime evolution, representation, extension-owned information, communication permissions, and conformance.

Whether this integration is absent from prior simulation frameworks, and whether it produces measurable engineering benefits, remains Under Survey.
