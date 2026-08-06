# Information Governance Taxonomy v0.1

Status: Under Survey  
Research Line: IG-001 — Information Governance in Simulation Frameworks  
Date: 2026-08-07

---

## 1. Purpose

Define a conservative taxonomy for evaluating whether simulation frameworks govern information beyond ordinary data structures, interfaces, and numerical coupling.

This document does not claim that ThermoCore introduces information governance. It establishes common evaluation language for later evidence comparison.

---

## 2. Working Definition

For this research line, **information governance** means explicit architectural rules that determine:

- what an information category means;
- who is authoritative for its semantics;
- who is responsible for maintaining or evolving it;
- who may read, supply, modify, transform, or consume it;
- how it may legally flow between architectural responsibilities;
- how ownership and semantics are preserved through communication;
- what evidence is required to establish conformance; and
- how changes affect verification and validation scope.

This working definition adapts concepts from data governance, software architecture, systems engineering, and simulation-framework practice. It is narrower than enterprise data governance and broader than an API access-control model.

---

## 3. Governance Dimensions

| ID | Dimension | Evaluation Question |
|---|---|---|
| IG-1 | Information Identity | Is the information category explicitly defined and distinguishable from related categories? |
| IG-2 | Semantic Authority | Is there a defined authority that determines the information's meaning and valid interpretation? |
| IG-3 | Operational Ownership | Is a responsibility assigned to maintain or evolve the information at runtime? |
| IG-4 | Access Permission | Are read, write, supply, consume, transform, and communicate permissions distinguished? |
| IG-5 | Responsibility Boundary | Are components prohibited from silently absorbing another component's information responsibility? |
| IG-6 | Information Flow | Are legal sources, destinations, and communication paths defined? |
| IG-7 | Ownership Preservation | Does communication preserve original ownership and semantics unless an explicit transfer is defined? |
| IG-8 | Extension Governance | Are optional mechanisms constrained from redefining core information categories or authority? |
| IG-9 | Information Lifecycle | Are creation, update, persistence, history, replacement, and disposal responsibilities addressed? |
| IG-10 | Provenance and Traceability | Can information and governance decisions be traced to requirements, sources, transformations, or evidence? |
| IG-11 | Conformance Boundary | Are violations of information rules identifiable as architectural non-conformance? |
| IG-12 | Change and Revalidation Scope | Is there a defensible basis for deciding which changes require renewed verification or validation? |

---

## 4. Distinctions Required for Assessment

### 4.1 Data Source Is Not Semantic Authority

A component may produce a value without owning the authoritative definition of the information category represented by that value.

### 4.2 Read Permission Is Not Ownership

Receiving or consuming information shall not be interpreted as owning its runtime evolution or semantics.

### 4.3 Operational Ownership Is Not Governance Authority

A runtime component may update information under rules defined by a specification without having authority to redefine those rules.

### 4.4 Architectural State Is Not Numerical State

A framework-defined state category may differ from the numerical variables selected by a solver for integration, linearization, caching, or optimization.

### 4.5 Extension State Is Not Automatically Core State

Persistent information required only by one optional mechanism does not become core information merely because it participates in a coupled calculation.

### 4.6 Information Governance Is Not Scheduling

Governance may define who may communicate and what information means without prescribing call order, synchronization, execution backend, or scheduling policy.

---

## 5. Evidence Basis

### IG-T01 — Data Governance

NIST defines data governance as processes that formally manage data assets and establish authority, management, and decision-making parameters. NIST also defines an information owner as an official with operational or statutory authority and responsibility for controls over information generation, processing, dissemination, and disposal.

These concepts support IG-2, IG-3, IG-4, and IG-9, but they originate in organizational governance rather than simulation architecture.

Primary sources:

- NIST CSRC Glossary, `data governance`: https://csrc.nist.gov/glossary/term/data_governance
- NIST CSRC Glossary, `information owner`: https://csrc.nist.gov/glossary/term/information_owner

### IG-T02 — ISO Governance of Data and Information

ISO/IEC 38505-1 treats governance of data as a governance domain concerned with effective, efficient, acceptable use and protection of data. ISO work on information governance further emphasizes accountable creation, use, maintenance, preservation, and disposition of data and information.

These sources support lifecycle, accountability, policy, and evidence concepts, but do not directly establish simulation-framework semantics.

Primary sources:

- ISO/IEC 38505-1:2017 and 2026 revision page: https://www.iso.org/standard/56639.html and https://www.iso.org/standard/87195.html
- ISO/CD TS 17955.2: https://www.iso.org/standard/85109.html

### IG-T03 — Software Architecture Responsibility and Constraints

SEI architecture methods treat architecture as allocation of function and responsibility and use templates that constrain how components interact with shared services. Design by Contract formalizes component conditions of use and behavioral obligations.

These concepts support IG-4, IG-5, IG-6, and IG-11, but do not by themselves define information ownership or physical-state authority.

Primary sources:

- SEI, `The Architecture Based Design Method`: https://www.sei.cmu.edu/library/the-architecture-based-design-method/
- SEI, `PSP-VDC`: https://www.sei.cmu.edu/library/psp-vdc-an-adaptation-of-the-psp-that-incorporates-verified-design-by-contract/

### IG-T04 — Traceability and Revalidation

MOOSE SQA links change requests, requirements, design, tests, verification, and validation. Module-specific SQA structures demonstrate that traceability and bounded quality evidence are established practice.

These sources support IG-10 and IG-12 but do not prove that information-governance boundaries determine revalidation scope.

Primary sources:

- MOOSE SQA Extension: https://mooseframework.inl.gov/releases/moose/2024-03-08/python/MooseDocs/extensions/sqa.html
- MOOSE NQA-1 Traceability Training: https://mooseframework.inl.gov/sqa/training/ccb/index.html

---

## 6. Initial Research Hypothesis

> **IG-H1:** Existing simulation frameworks explicitly govern computation, interfaces, and extensibility, but may only partially integrate information identity, semantic authority, operational ownership, permissions, lifecycle, provenance, and conformance into one architectural model.

This hypothesis is Under Survey and shall be rejected or narrowed if an existing framework provides equivalent integrated governance.

---

## 7. Falsification Conditions

IG-H1 shall be rejected or narrowed if a simulation framework is found that explicitly and jointly defines:

1. authoritative information categories for the physical domain;
2. semantic authority distinct from data production;
3. operational ownership of runtime evolution;
4. distinct read, write, supply, consume, and transformation permissions;
5. ownership preservation across communication;
6. extension-specific state that remains outside core state unless formally adopted;
7. information lifecycle and provenance;
8. conformance violations for governance breaches; and
9. traceable change and revalidation boundaries.

---

## 8. Current Classification

| Item | Classification |
|---|---|
| Enterprise data governance concepts | Verified prior art |
| Component responsibility and contract constraints | Verified prior art |
| Traceability and module-specific SQA | Verified prior art |
| Integrated information governance for simulation-state semantics | Under Survey |
| ThermoCore compliance with IG-1–IG-12 | Specification review required |
| Research Gap | Not established |

---

## 9. Next Tasks

1. Apply IG-1–IG-12 to ThermoCore normative specifications without assuming full compliance.
2. Apply the same taxonomy to MOOSE, SOFA, Modelica, FMI, OpenFOAM, preCICE, and OpenMDAO.
3. Distinguish evidence of implementation mechanism from evidence of governance semantics.
4. Record design implications separately as Change Request candidates.
5. Produce an Information Governance Evidence Matrix and falsification summary.
