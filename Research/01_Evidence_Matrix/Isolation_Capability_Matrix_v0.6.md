# Isolation Capability Matrix v0.6 — Semantic/Core-Boundary Stress Test

Status: Evidence-supported candidate gap (bounded survey)  
Research Question: RQ-ISO-001  
Date: 2026-08-23  
Dependency: `Isolation_Capability_Matrix_v0.5.md`

---

## 1. Purpose of This Revision

This revision performs the bounded follow-up search identified by v0.5. The search no longer targets generic modularity, single-writer ownership, publish/subscribe isolation, plugin systems, or interface governance because those capabilities have already been established as prior art.

The search instead targets architectures that are more likely to falsify the remaining ThermoCore candidate by combining strong semantic governance with fixed architectural roles.

The evaluated classes are:

1. digital-twin core/reference architectures;
2. standardized industrial digital-twin information models;
3. safety-critical portable-component architectures with governed shared semantics; and
4. safety-relevant platform state-management architectures with explicit state authority.

The specific systems examined in this revision are:

- NIST/IIC Digital Twin Core;
- Asset Administration Shell (AAS / IDTA);
- FACE Technical Standard / FACE Data Architecture; and
- AUTOSAR Adaptive Platform State Management.

Newly published ISO/IEC 30188:2026 and ISO/TS 25271:2026 were also screened as current digital-twin reference-architecture standards. Their publicly available abstracts were sufficient to establish relevance, but not sufficient to support detailed capability scoring on semantic authority or extension-state promotion. They are therefore recorded as watch items rather than used as positive evidence for equivalence.

This revision does **not** establish novelty. It records that a bounded falsification search has not identified a direct prior-art architecture that jointly requires the full surviving semantic/Core-boundary combination.

---

## 2. Focused Falsification Criteria

The remaining candidate is evaluated against the following stricter conditions derived from v0.5.

| ID | Condition |
|---|---|
| S1 | A physical-domain state has normatively defined semantics independent of optional representations/extensions. |
| S2 | One architectural responsibility is assigned authority to evolve that state. |
| S3 | Consumers/representations may observe or derive from the state without obtaining semantic or evolution authority. |
| S4 | Optional mechanisms may own persistent mechanism-specific state. |
| S5 | Ordinary optional mechanisms are prohibited from promoting their local state into mandatory Core State. |
| S6 | Ordinary optional mechanisms are prohibited from redefining the semantic identity or owner of the authoritative state. |
| S7 | Core completeness/conformance remains valid when all ordinary optional mechanisms are absent. |
| S8 | A bounded change/validation impact follows from these authority boundaries rather than only from module separation. |

A direct falsification candidate should satisfy these conditions jointly and explicitly. Partial matches remain prior art for the matched capability but do not establish equivalence.

---

## 3. Focused Comparison

| Condition | NIST/IIC Digital Twin Core | AAS / IDTA | FACE | AUTOSAR Adaptive State Management | ThermoCore | Current interpretation |
|---|---:|---:|---:|---:|---:|---|
| S1 Fixed physical-domain state semantics | P | P | P | P | Y | All four comparators provide governed semantics or models, but none establishes the same fixed thermodynamic-state category independent of optional mechanisms. |
| S2 One architectural evolution authority | N/P | N | N | Y | Y | AUTOSAR is the strongest match: State Management owns platform operational-state management. The other three primarily govern information/model interoperability rather than one physical-state evolution owner. |
| S3 Consumer use without semantic authority | P | P | P | Y/P | Y | FACE, AAS, and Digital Twin Core support consumer/application separation, but not a general prohibition on acquiring domain-semantic authority. AUTOSAR applications request transitions rather than directly becoming State Management, but project-specific applications may influence the state machine. |
| S4 Extension-local persistent state | Y | Y | Y | Y | Y | Component/submodel/application-local state is established prior art. |
| S5 No promotion of extension-local state into mandatory Core State | U/P | N/P | U/P | N/P | Y | AAS and FACE intentionally permit domain/model extension. AUTOSAR state machines and configuration are project-specific. No searched source states ThermoCore's non-promotion rule as a general architectural invariant. |
| S6 No extension redefinition of state semantic identity/owner | U/P | N/P | U/P | P | Y | AUTOSAR fixes the State Management role but permits project-specific state models and control applications. AAS/FACE are designed to extend semantic models rather than freeze one physical-state identity. |
| S7 Core complete without ordinary extensions | Y/P | Y | Y | Y/P | Y | Core/platform availability without a particular optional component is common prior art, but the semantic meaning of `Core completeness` differs across architectures. |
| S8 Authority-derived bounded revalidation impact | U | U | P | P | Candidate | FACE has strong conformance isolation and AUTOSAR has safety-oriented functional-cluster boundaries, but no searched evidence derives revalidation scope specifically from a non-transferable physical-state semantic authority rule. |

`P` means partial or materially different in scope. `U` means the public evidence inspected was insufficient for a positive or negative determination.

---

## 4. New Evidence Records

### ISO-E14 — NIST/IIC Digital Twin Core

**Evidence status:** Verified for a digital-twin core positioned between supporting infrastructure and business applications, common core functionality, metamodel/information-model support, and plug-in interoperability. Under Survey for direct semantic-authority equivalence.

The NIST-hosted IIC report `Digital Twin Core Conceptual Models and Services` proposes a digital-twin core as middleware between underlying IT infrastructure and business applications. It argues that digital twins of equipment, subsystems, and processes can be predefined with common core functionality and integrated through standard interfaces to support different applications.

The report is important prior art for several broad ThermoCore-adjacent ideas:

- a reusable core separated from business/application concerns;
- explicit metamodel and information-model support;
- standard interfaces for interoperability; and
- independently supplied digital twins being integrated into a larger system.

This substantially weakens any claim that ThermoCore is distinctive merely because a reusable core is isolated from application-specific representation or because information models are separated from business applications.

However, the public report summary does not establish the surviving ThermoCore combination. In particular, it does not identify one authoritative thermodynamic or physical-domain state whose semantic identity is fixed independently of optional models, nor a rule preventing application- or twin-specific state from becoming part of the shared model.

**Relevant conditions:** S4 yes; S7 partial/yes; S1/S3/S5/S6/S8 not established as equivalent.

**Primary source:**

- NIST publication page, `Digital Twin Core Conceptual Models and Services`: https://www.nist.gov/publications/digital-twin-core-conceptual-models-and-services

**Interpretation:**

Digital Twin Core is strong prior art for reusable core middleware and application separation, but it does not presently falsify the fixed semantic/Core-State boundary candidate.

---

### ISO-E15 — Asset Administration Shell Semantic Extensibility

**Evidence status:** Verified for a normative metamodel, semantic identifiers, submodels/submodel elements, standardized APIs, and extensible asset information modeling.

The Asset Administration Shell (AAS) specifications define the software structure, interfaces, and semantics of a standardized industrial digital twin. The normative metamodel provides submodels and submodel elements for describing assets, including properties, relationships, operations, events, files, and other typed elements. Elements may carry semantic identifiers and data specifications.

AAS is therefore strong prior art for:

- machine-readable industrial semantics;
- standardized digital-twin information models;
- separation between metamodel structure and runtime APIs; and
- modular domain-specific submodels.

AAS does **not** support a broad novelty claim based on semantic governance or standardized extensible state descriptions.

At the same time, the AAS model is intentionally extensible through submodels and submodel elements. Its purpose is to describe and differentiate assets across many domains. The inspected specifications do not impose a fixed single physical-domain state owner or prohibit a new submodel from introducing additional domain-relevant state semantics.

**Relevant conditions:** S4 yes; S7 yes; S1/S3 partial; S5/S6 not equivalent; S2/S8 not established.

**Primary sources:**

- IDTA AAS specification index: https://industrialdigitaltwin.org/en/content-hub/aasspecifications
- IDTA AAS Part 1 Metamodel: https://industrialdigitaltwin.org/en/?specificationpapers=specification-of-the-asset-administration-shell-part-1-metamodel-idta-number-01001
- IDTA AAS Part 2 APIs: https://industrialdigitaltwin.org/en/content-hub/aasspecifications/specification-of-the-asset-administration-shell-part-2-application-programming-interfaces-idta-number-01002

**Interpretation:**

AAS shows that strong semantic governance and modular digital-twin information models are established prior art. It does not establish the surviving non-promotion/non-redefinition rule because semantic extension is a normal capability rather than a prohibited one.

---

### ISO-E16 — FACE Data Architecture, Portable Components, and Conformance

**Evidence status:** Verified for standardized architectural segments, governed interfaces, machine-readable data semantics, portable components, shared/domain-specific data models, and formal conformance verification.

The FACE Technical Standard defines a common software computing environment for portable avionics components. Its Data Architecture requires sufficiently precise data models to capture data type, units, precision, reference frames, entities, associations, and semantic meaning for information exchanged between software components. The FACE ecosystem also publishes Shared Data Models, governance material, conformance verification matrices, and certification processes.

FACE is therefore a strong safety-critical counterexample to broad ThermoCore claims about:

- normative component boundaries;
- governed interfaces;
- semantic interoperability;
- reusable/replaceable components;
- machine-readable data definitions; and
- independently verifiable conformance scopes.

FACE materially narrows the remaining hypothesis because it shows that a framework can enforce both architecture and data semantics while allowing capability components to remain portable.

However, FACE Data Architecture is centered on interoperable exchanged data and portable Units of Portability, not on one fixed authoritative physical-domain state whose semantic owner cannot be altered by domain-specific capability insertion. The FACE Shared Data Model and Domain Specific Data Models are governed mechanisms for defining and extending semantic content; they do not establish the ThermoCore prohibition against ordinary extensions promoting local physical state into mandatory Core State.

**Relevant conditions:** S3 partial; S4 yes; S7 yes; S8 partial; S1/S5/S6 not equivalent; S2 not established.

**Primary sources:**

- FACE Approach: https://www.opengroup.org/face/approach
- FACE Data Modelers / Data Architecture: https://www.opengroup.org/face/datamodelers
- FACE Documents & Tools / Shared Data Model / Conformance artifacts: https://www.opengroup.org/face/docsandtools
- FACE Conformance FAQs: https://www.opengroup.org/face/conformance-FAQs

**Interpretation:**

FACE eliminates any remaining claim based on semantic-model governance plus modular conformance. The surviving ThermoCore candidate must remain specific to fixed physical-state semantic authority and Core-State membership.

---

### ISO-E17 — AUTOSAR Adaptive Platform State Management

**Evidence status:** Verified for one functional cluster responsible for platform Operational State Management, controlled state-transition requests, application influence through declared interfaces, and safety-relevant functional separation.

AUTOSAR Adaptive Platform State Management is the strongest counterexample found in this revision.

AUTOSAR specifies State Management as a functional cluster responsible for all aspects of Operational State Management, including handling and prioritizing incoming events/requests and setting corresponding internal states. Current specifications expose a `StateMachineService` through which an `SMControlApplication` requests state transitions using `RequestTransition` rather than directly becoming State Management.

This directly demonstrates mature prior art for:

- one architectural state-management responsibility;
- other applications influencing state through a declared request interface;
- request/observation being distinct from the state-management role; and
- safety-relevant separation of platform responsibilities.

ThermoCore therefore cannot claim novelty merely because one component owns state evolution while other components request effects through interfaces.

The remaining difference is scope and semantic invariance. AUTOSAR explicitly describes State Management as highly project-specific. State Management may contain project-specific state machines, and project-specific control applications decide which transitions should be requested. The architecture fixes a state-management responsibility, but it does not freeze one domain-independent physical-state semantic identity or prohibit project-specific capability from extending the state model.

**Relevant conditions:** S2 yes; S3 yes/partial; S4 yes; S7 partial/yes; S1/S5/S6 only partial; S8 partial.

**Primary sources:**

- AUTOSAR Adaptive Platform overview: https://www.autosar.org/standards/adaptive-platform/
- AUTOSAR `Specification of State Management` R25-11: https://www.autosar.org/fileadmin/standards/R25-11/AP/AUTOSAR_AP_SWS_StateManagement.pdf
- AUTOSAR `Requirements of State Management`: https://autosar.org/fileadmin/standards/R22-11/AP/AUTOSAR_RS_StateManagement.pdf

**Interpretation:**

AUTOSAR falsifies a broad `central state owner + request-only clients` contribution. It does not currently falsify the stricter claim that a fixed physical-domain State semantic identity and Core membership remain non-transferable and non-extensible by ordinary extensions.

---

## 5. Current Standards Watch

### ISO/IEC 30188:2026 — Digital twin — Reference architecture

The standard was published in July 2026 and specifies a general digital-twin reference architecture through architecture views.

Public abstract evidence establishes relevance but does not expose enough normative detail to score S1–S8 without access to the standard text. No positive equivalence claim is therefore made.

Source: https://www.iso.org/standard/53308.html

### ISO/TS 25271:2026 — Industrial digital twin interface architecture

The Technical Specification was published in August 2026. Its public abstract structures an industrial digital-twin system around the physical twin, digital twin, and interface between them.

The public abstract does not establish a fixed physical-state owner, extension-state promotion rule, or semantic/Core completeness invariant. The document remains a priority source for future full-text review if accessible.

Source: https://www.iso.org/standard/89689.html

---

## 6. Revised Findings

### F-ISO-19 — Reusable digital-twin cores are established prior art

The NIST/IIC Digital Twin Core provides prior art for common reusable twin functionality located between infrastructure and business applications.

ThermoCore shall not claim distinctiveness for core/application separation by itself.

### F-ISO-20 — Standardized extensible industrial semantics are established prior art

AAS provides a normative metamodel, semantic identifiers, submodels, typed submodel elements, and APIs for industrial digital twins.

ThermoCore shall not claim novelty for machine-readable semantic governance or modular domain-model extension.

### F-ISO-21 — Safety-critical semantic/data governance and modular conformance are established prior art

FACE combines architectural segmentation, governed interfaces, semantic data models, portable components, and formal conformance verification.

ThermoCore shall not claim novelty for semantic governance plus independently conformant modular components.

### F-ISO-22 — Central state authority with request-based external influence is established prior art

AUTOSAR State Management assigns Operational State Management to a defined functional cluster while applications interact through controlled interfaces and may request transitions.

ThermoCore shall not claim novelty for `one state-management authority + request-only clients` by itself.

### F-ISO-23 — Surviving candidate reduced to fixed semantic/Core-State invariance

Across the bounded survey, no inspected architecture was found to jointly and explicitly require all of the following:

> one authoritative physical-domain State with fixed semantics and one evolution responsibility; ordinary representations/extensions may consume it or contribute through declared boundaries, but may not redefine its semantic identity or owner, may not promote extension-local state into mandatory Core State, and may not make Core completeness depend on their presence.

This is narrower than modularity, ownership, access control, state management, digital-twin core separation, or semantic interoperability individually.

---

## 7. Bounded Survey Coverage

The RQ-ISO-001 survey has now explicitly tested the surviving hypothesis against the following architecture families and representative systems:

- multiphysics/component frameworks: SOFA, MOOSE;
- coupling and visualization frameworks: preCICE, VTK;
- model-exchange and systems-engineering standards: FMI, OpenMDAO, Modelica;
- field-oriented simulation frameworks: OpenFOAM;
- distributed simulation standards: HLA;
- data-centric middleware: DDS;
- digital-twin/IoT property models: IoT Plug and Play / DTDL;
- digital-twin core/reference architectures: NIST/IIC Digital Twin Core;
- industrial digital-twin semantic models: AAS;
- safety-critical portable-component/data architectures: FACE; and
- safety-relevant platform state management: AUTOSAR Adaptive State Management.

The survey also screened the newly published ISO/IEC 30188:2026 and ISO/TS 25271:2026 as current standards-watch items.

This breadth does not prove novelty. It is sufficient to stop unbounded expansion of generic comparisons because new systems increasingly reproduce capabilities already classified as prior art.

---

## 8. Current Classification

| Item | Classification |
|---|---|
| Modularity / plugins / optional components | Verified prior art |
| Multiple representations / consumers | Verified prior art |
| Single-writer / exclusive update ownership | Verified prior art |
| Read/subscribe without ownership transfer | Verified prior art |
| Request-based influence over centrally managed state | Verified prior art |
| Normative interfaces and connector governance | Verified prior art |
| Machine-readable semantic information models | Verified prior art |
| Digital-twin core / application separation | Verified prior art |
| Modular semantic-model extension | Verified prior art |
| Module/component-level conformance and V&V separation | Verified prior art |
| Fixed physical-domain semantic authority | Evidence-supported candidate distinction |
| Non-promotion of extension-local state into mandatory Core State | Evidence-supported candidate distinction |
| Core completeness invariant under ordinary extension absence | Evidence-supported candidate distinction |
| Reduced mandatory state growth caused by these rules | Unverified consequence hypothesis |
| Reduced revalidation impact caused by these rules | Unverified consequence hypothesis |
| Research Gap | **Candidate supported by bounded evidence; not yet established** |
| Novelty | **Not established** |

---

## 9. Survey Closure Decision

The evidence survey should stop adding broad comparison targets unless a new source is specifically likely to satisfy the full S1–S8 combination.

The appropriate next research step is no longer another general capability matrix revision. It is a bounded Research Gap Analysis that asks:

1. whether the surviving fixed semantic/Core-State boundary is sufficiently distinct and useful to constitute an architectural research gap;
2. whether the distinction can be expressed without claiming general novelty over ownership, modularity, or semantic governance;
3. whether the proposed consequences — reduced mandatory state growth, reduced Core modification, and reduced revalidation impact — are testable; and
4. what evidence would falsify those consequence claims.

No Framework Specification change is justified by this document alone.

---

## 10. Interim Conclusion

RQ-ISO-001 has survived a bounded falsification search, but only after substantial narrowing.

The surviving candidate is not `state ownership`, `decoupling`, `modularity`, `read-only access`, `semantic interoperability`, `digital-twin core separation`, or `central state management`. Each of those has strong prior art.

The remaining candidate is a stricter architectural invariant:

> **Fixed Semantic/Core-State Boundary under Ordinary Extension** — optional representations and ordinary extensions may participate in a physical-domain framework without acquiring authority to redefine the semantic identity, owner, mandatory membership, or completeness conditions of the authoritative Core State.

The bounded evidence collected so far supports treating this as a **Research Gap candidate**, not as an established novel contribution.
