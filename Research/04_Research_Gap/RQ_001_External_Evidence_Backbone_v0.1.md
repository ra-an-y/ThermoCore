# RQ-001 External Evidence Backbone v0.1

Status: **COMPLETED — representative citation backbone for final RQ-001 synthesis**  
Classification: **Non-Normative Research Reference / Citation-Coverage Supplement**  
Date: **2026-08-24**  
Companion synthesis: `RQ_001_Research_Synthesis_and_Final_Closure_v0.1.md`

---

## 1. Purpose

This document provides a compact external-evidence backbone for the completed RQ-001 research synthesis.

The final RQ-001 synthesis intentionally prioritizes bounded conclusions and internal traceability. The detailed downstream Evidence Matrices contain substantially more source-level records than would be practical to duplicate in the final synthesis. This companion therefore exposes a representative set of primary, standards, official-framework, and peer-reviewed sources so that a reader can see the external evidence underlying the major prior-art exclusions and contribution boundaries without traversing every intermediate research artifact.

This document does **not** replace the Evidence Matrices and is **not** an exhaustive bibliography or systematic-review claim.

It introduces no new Research Gap, novelty, priority, Framework Specification, implementation, Verification, Validation, or Performance conclusion.

---

## 2. Citation-Coverage Audit Result

The completed RQ-001 closure package was reviewed for:

- consistency between the two supported bounded contributions;
- consistency of the five closed / reclassified research lines;
- preservation of prior-art exclusions;
- novelty and priority guardrails;
- internal research-to-evidence traceability;
- separation of Research, Framework Specification, Conformance, Verification, Validation, and Performance claims; and
- external citation visibility at the final-synthesis layer.

Audit disposition:

```text
Research conclusion consistency: PASS
Contribution-boundary consistency: PASS
Negative-result preservation: PASS
Novelty / priority restraint: PASS
Internal traceability: PASS
Framework impact discipline: PASS
External evidence availability in underlying matrices: PASS
External citation visibility in final synthesis layer: STRENGTHENED BY THIS DOCUMENT
```

No substantive RQ-001 conclusion required reopening during this audit.

---

## 3. Claim-to-Evidence Map

| RQ-001 claim family | Representative external evidence | Role in the final disposition |
|---|---|---|
| Single-writer / update ownership, read-versus-write separation, ownership-preserving communication | IEEE HLA 1516 / 1516.1; OMG DDS | Establishes strong prior art; excludes broad RQ-ISO claims based only on exclusive update authority or consumer non-ownership |
| Central state-management responsibility influenced through declared requests | AUTOSAR Adaptive Platform State Management | Establishes prior art for a central state-management role plus request-based external influence |
| Reusable core, governed semantic models, modular conformance, extensible digital-twin structures | NIST/IIC Digital Twin Core; IDTA AAS; FACE | Excludes broad claims based on core/application separation, semantic governance, extensible information models, or modular conformance alone |
| Internal variables, history state, state-space enlargement | Coleman & Gurtin (1967) and later internal-variable literature | Establishes that adding persistent internal variables when the selected state description is insufficient is thermodynamic prior art |
| Generalized field/work pairs and formulation-dependent caloric state descriptions | Planes, Castán & Saxena (2016) and related multicaloric literature | Excludes novelty claims for generalized thermodynamic work pairs and mechanism-name-based classification |
| Replaceable medium / thermodynamic formulation behind stable component equations | Modelica.Fluid / Modelica.Media; OpenFOAM thermophysical models | Supports the RQ-FCI negative result and the formulation-relative interpretation of persistent / derived quantities |
| Conserving connector / energy-port semantics | Modelica connection / stream semantics; port-Hamiltonian literature; Simscape conserving ports | Supports the RQ-CEX negative result; energy/power-conserving interconnection is established prior art |
| Material-property producer/consumer systems and explicit stateful material properties | MOOSE Materials / Kokkos Materials | Supports the RQ-RMA and RQ-MRR exclusions: material-property systems, stateful material properties, and backend-specific representations are established practice |
| Current digital-twin standards close to the surviving RQ-ISO boundary | ISO/IEC 30188:2026; ISO/TS 25271:2026 | Standards-watch evidence only; public abstracts were screened for relevance but not used as proof of absence or detailed equivalence |

The map is intentionally representative. Full source-by-source scoring and limitations remain authoritative in the applicable Evidence Matrices.

---

## 4. Representative External References

### 4.1 State Authority, Ownership, Semantic Governance, and Core Separation

**[EXT-ISO-01] IEEE 1516-2025 — High Level Architecture Framework and Rules.**  
IEEE Standards Association. *IEEE Standard for Modeling and Simulation (M&S) High Level Architecture (HLA) — Framework and Rules.*  
https://standards.ieee.org/ieee/1516/6687

**Evidence role:** establishes a standardized simulation architecture with federate/federation rules and mature ownership / interaction governance. It is prior art against broad claims based on governed distributed simulation architecture.

**[EXT-ISO-02] IEEE 1516.1-2025 — HLA Federate Interface Specification.**  
IEEE Standards Association. *High Level Architecture — Federate Interface Specification.*  
https://standards.ieee.org/ieee/1516.1/6688/

**Evidence role:** establishes standardized services for federate interaction, including ownership-related services and governed data exchange. Together with HLA object-model semantics, it materially narrows any claim based only on update ownership versus observation.

**[EXT-ISO-03] Object Management Group — Data Distribution Service 1.4.**  
OMG. *Data Distribution Service (DDS), Version 1.4.*  
https://www.omg.org/spec/DDS/

**Evidence role:** DDS exclusive ownership and DataWriter/DataReader separation establish prior art for selected-writer authority and consumption without ownership transfer.

**[EXT-ISO-04] AUTOSAR Adaptive Platform — State Management.**  
AUTOSAR. *Specification of State Management*, R25-11.  
https://www.autosar.org/fileadmin/standards/R25-11/AP/AUTOSAR_AP_SWS_StateManagement.pdf

Adaptive Platform overview:  
https://www.autosar.org/standards/adaptive-platform/

**Evidence role:** establishes a defined State Management responsibility that receives and arbitrates requests from other applications. The specification is also explicitly project-specific, which is important to the bounded non-equivalence finding.

**[EXT-ISO-05] Industrial Digital Twin Association — Asset Administration Shell Metamodel.**  
IDTA. *Specification of the Asset Administration Shell — Part 1: Metamodel*, IDTA-01001, v3.1.1, July 2025. DOI: `10.62628/IDTA.01001-3-1-1`.  
https://industrialdigitaltwin.io/aas-specifications/IDTA-01001/v3.1.1/index.html

**Evidence role:** establishes strong normative semantic governance, typed submodels, semantic identifiers, and extensible industrial digital-twin information models. This excludes novelty based on semantic modeling or modular domain-model extension alone.

**[EXT-ISO-06] Lin et al. — Digital Twin Core Conceptual Models and Services.**  
Lin, S.-W.; Watson, K.; Shao, G.; Stojanovic, L.; Zarkout, B. (2023). *Digital Twin Core Conceptual Models and Services*. An IIC Technical Report, hosted by NIST.  
https://www.nist.gov/publications/digital-twin-core-conceptual-models-and-services

**Evidence role:** establishes prior art for reusable digital-twin core middleware, common core functionality, metamodel / information-model support, standard interfaces, and separation from business applications.

**[EXT-ISO-07] FACE Technical Standard ecosystem — Data Architecture and Conformance.**  
The Open Group. FACE Data Architecture / Data Modelers:  
https://www.opengroup.org/face/datamodelers

FACE Documents and Tools:  
https://www.opengroup.org/face/docsandtools

FACE Conformance FAQs:  
https://www.opengroup.org/face/conformance-FAQs

**Evidence role:** establishes mature safety-critical prior art for governed architecture segments, machine-readable shared semantics, portable components, and formal conformance verification. It narrows RQ-ISO away from semantic governance plus modular conformance as a broad contribution claim.

---

### 4.2 Thermodynamic State Space, Internal Variables, and Formulation-Relative Modeling

**[EXT-EFM-01] Coleman & Gurtin — Thermodynamics with Internal State Variables.**  
Coleman, B. D.; Gurtin, M. E. (1967). *Thermodynamics with Internal State Variables*. *The Journal of Chemical Physics*, 47(2), 597–613. DOI: `10.1063/1.1711937`.

**Evidence role:** foundational prior art for nonlinear thermodynamics with additional internal state variables and their evolution. This excludes any claim that RQ-EFM invents state-space enlargement for history-dependent response.

**[EXT-EFM-02] Planes, Castán & Saxena — Multicaloric Thermodynamics.**  
Planes, A.; Castán, T.; Saxena, A. (2016). *Thermodynamics of multicaloric effects in multiferroic materials: application to metamagnetic shape-memory alloys and ferrotoroidics*. *Philosophical Transactions of the Royal Society A*, 374:20150304. DOI: `10.1098/rsta.2015.0304`.

**Evidence role:** establishes generalized thermodynamic treatment of multiple ferroic order parameters and conjugate fields. It is prior art for generalized work-pair reasoning and cross-coupled caloric formulations.

**[EXT-EFM-03] Casella et al. — Modelica.Fluid and Modelica.Media.**  
Casella, F.; Otter, M.; Proelss, K.; Richter, C.; Tummescheit, H. (2006). *The Modelica Fluid and Media Library for Modeling of Incompressible and Compressible Thermo-Fluid Pipe Networks*. Proceedings of the 5th International Modelica Conference.  
https://modelica.org/events/modelica2006/Proceedings/sessions/Session6b1.pdf

**Evidence role:** documents decoupling of component balance equations from replaceable medium-property formulations, standard fluid connectors, and medium-relative independent thermodynamic state variables. This is important prior art for RQ-FCI and for formulation-relative state selection.

**[EXT-EFM-04] OpenFOAM — Thermophysical Models.**  
OpenCFD Ltd. *OpenFOAM User Guide — Thermophysical Models*.  
https://www.openfoam.com/documentation/user-guide/5-models-and-physical-properties/5.2-thermophysical-models

**Evidence role:** OpenFOAM exposes selectable thermophysical packages and allows thermal energy formulations based on enthalpy or internal energy. This supports the conclusion that energy-form / formulation selection behind a stable higher-level thermophysical architecture is established practice.

---

### 4.3 Conserving Exchange and Multi-Domain Interconnection

**[EXT-CEX-01] Modelica Language Specification — Connectors and Stream Semantics.**  
Modelica Association. *Modelica Language Specification*, connector / connection and stream-connector semantics.  
https://specification.modelica.org/master/connectors-and-connections.html  
https://specification.modelica.org/master/stream-connectors.html

**Evidence role:** establishes zero-sum flow connection equations and stream-carried thermofluid quantities, including connection-level conservation semantics. This is direct prior art against an independent novelty claim for semantic energy-conserving ports.

**[EXT-CEX-02] van der Schaft — Port-Hamiltonian Systems.**  
van der Schaft, A. (2007). *Port-Hamiltonian systems: an introductory survey*. DOI: `10.4171/022-3/65`.  
https://ems.press/books/standalone/24/557

**Evidence role:** establishes energy-based, power-conserving interconnection through ports and Dirac structures, including composition of subsystems across physical domains.

**[EXT-CEX-03] Distributed Port-Hamiltonian Literature Review.**  
*Twenty years of distributed port-Hamiltonian systems: a literature review*. *IMA Journal of Mathematical Control and Information*, 37(4), 1400–1422.  
https://academic.oup.com/imamci/article/37/4/1400/5877069

**Evidence role:** provides broad evidence that power-flow-centered multi-physical interconnection and separation of interconnection structure from constitutive relations are mature prior art.

**[EXT-CEX-04] Simscape Conserving Ports.**  
MathWorks. *Simscape Language Guide — Across and Through Variables / Conserving Connections*.  
https://www.mathworks.com/help/simscape/lang/declare-through-and-across-variables-for-a-domain.html

**Evidence role:** independent engineering precedent for conserving ports with Across/Through variable semantics and balance equations.

---

### 4.4 Material Property Systems, Stateful Material Data, and Backend Separation

**[EXT-RMA-01] MOOSE Materials System.**  
Idaho National Laboratory. *MOOSE Materials System*.  
https://mooseframework.inl.gov/syntax/Materials/

**Evidence role:** establishes producer/consumer material-property architecture, on-demand properties coupled to solution variables, and explicitly stateful material properties retaining old/older values.

**[EXT-RMA-02] MOOSE Kokkos Materials System.**  
Idaho National Laboratory. *Kokkos Materials System*.  
https://mooseframework.inl.gov/syntax/KokkosMaterials/index.html

**Evidence role:** provides explicit backend-specific material-property representation and stateful-property handling in a Kokkos execution model. This is prior art against treating GPU/device representation alone as a new material information category.

These references are representative of the wider RQ-RMA evidence set, which also reviewed Modelica.Media, OpenFOAM, Cantera, CoolProp, DOLFINx/MFront/JAX material behavior, and NEML history/internal-variable systems.

---

### 4.5 Standards Watch — Relevance Screening Only

**[WATCH-01] ISO/IEC 30188:2026 — Digital twin — Reference architecture.**  
https://www.iso.org/standard/53308.html

**[WATCH-02] ISO/TS 25271:2026 — Industrial digital twin interface architecture.**  
https://www.iso.org/standard/89689.html

These sources were screened because they are current and close to the surviving architecture question. Publicly accessible abstract-level material was **not** treated as sufficient evidence for detailed S1–S8 capability scoring, proof of non-equivalence, or proof of novelty.

---

## 5. Relationship to the Detailed Evidence Matrices

This citation backbone shall be read together with the detailed evidence artifacts rather than as a replacement for them.

Primary detailed sources include:

- `Research/01_Evidence_Matrix/Isolation_Capability_Matrix_v0.5.md`
- `Research/01_Evidence_Matrix/Isolation_Capability_Matrix_v0.6.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.3.md`
- `Research/01_Evidence_Matrix/Formulation_Change_Isolation_Evidence_Matrix_v0.2.md`
- `Research/01_Evidence_Matrix/Conservative_Energy_Exchange_Evidence_Matrix_v0.2.md`
- `Research/01_Evidence_Matrix/Compositional_Extension_Admissibility_Evidence_Matrix_v0.2.md`
- `Research/01_Evidence_Matrix/Runtime_Material_Abstraction_Evidence_Matrix_v0.2.md`
- `Research/01_Evidence_Matrix/Material_Representation_Responsibility_Evidence_Matrix_v0.2.md`

Those artifacts preserve source-specific observations, capability scoring, limitations, falsification pressure, and negative findings. The present document only makes the external evidence backbone visible at closure level.

---

## 6. Citation Use Rules

The RQ-001 closure package should preserve the following citation discipline:

1. A source may establish prior art for one capability without being a full ThermoCore equivalent.
2. `Not identified in the bounded survey` shall not be rewritten as `does not exist`.
3. Standards-watch sources with inaccessible full normative text shall not be scored beyond accessible evidence.
4. Official documentation is appropriate evidence for a framework's documented architecture or behavior; it is not automatically evidence of scientific novelty.
5. Peer-reviewed thermodynamics literature supports physical/formulation antecedents; it does not automatically establish software-architecture equivalence.
6. Engineering/conformance properties retained after negative research results shall not be promoted to independent contributions merely because they are useful.
7. The two supported contributions remain bounded by the actual surveyed/evaluated scope and do not establish global novelty, first-ever priority, or universal superiority.

---

## 7. Final Audit Disposition

The final RQ-001 synthesis remains substantively valid after citation-coverage review.

The audit found no reason to change:

- the number of supported bounded research contributions (`2`);
- the number of closed / reclassified engineering-conformance lines (`5`);
- the RQ-EFM -> RQ-ISO ordering;
- the prohibited-claim list;
- the no-global-novelty conclusion;
- the Framework Specification impact (`NONE`); or
- the frozen v1.0.0 release boundary.

The identified weakness was presentation-level citation visibility: the closure synthesis exposed internal traceability much more strongly than the external evidence base.

This companion corrects that imbalance without changing the research result.

Final classification:

```text
RQ-001 substantive review: PASS
External evidence backbone: PASS after supplement
Research conclusion change: NONE
Novelty / priority change: NONE
Normative semantic change: NONE
Framework Freeze reopen: NO
Frozen v1.0.0 impact: NONE
```
