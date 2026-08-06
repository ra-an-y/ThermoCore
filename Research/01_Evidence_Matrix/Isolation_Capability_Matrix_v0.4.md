# Isolation Capability Matrix v0.4

Status: Under Survey  
Research Question: RQ-ISO-001  
Date: 2026-08-07

---

## 1. Purpose of This Revision

This revision extends the falsification-oriented survey with:

- Modelica as a strong equation-based and connector-governed counterexample;
- OpenFOAM as a field-oriented, runtime-selectable simulation framework;
- additional MOOSE evidence on module-specific Software Quality Assurance and Verification/Validation boundaries; and
- a separate list of non-normative change-request candidates derived from external frameworks.

The document continues to test whether ThermoCore provides a distinct combination of state authority, representation non-ownership, extension-owned state, core completeness, and validation isolation.

This revision does not establish novelty or a Research Gap.

---

## 2. Assessment Scale

| Mark | Meaning |
|---|---|
| Y | Explicitly supported by available evidence |
| P | Partially supported, supported only for some component types, or materially different in scope |
| N | Available evidence indicates the capability is not part of the evaluated architecture |
| U | Unknown; insufficient evidence |
| N/A | Not applicable to the framework's stated purpose |

A `Y` does not imply semantic equivalence with ThermoCore.

---

## 3. Capability Definitions

| ID | Capability | Evaluation Question |
|---|---|---|
| C1 | Authoritative State | Does the architecture identify one authoritative runtime state for the evaluated physical domain? |
| C2 | Explicit State Owner | Is responsibility for evolving that state assigned to a defined architectural owner? |
| C3 | Read Does Not Confer Ownership | Is consuming information explicitly separated from owning or modifying it? |
| C4 | Representation Cannot Redefine State | Are representation components prohibited from redefining authoritative state semantics or evolution? |
| C5 | Extension-owned State | Can an extension own persistent state that exists only for its mechanism without expanding core state? |
| C6 | Core Complete Without Extensions | Is the core complete and valid when every optional extension is absent? |
| C7 | Interface-governed Communication | Must communication use declared interfaces, connectors, mappings, or coupling mechanisms? |
| C8 | Ownership Preservation Across Communication | Does communication preserve the original ownership and semantics of information? |
| C9 | State Growth Control | Are optional mechanisms prevented from automatically becoming mandatory core state? |
| C10 | Validation Boundary | Is there an explicit basis for separating core validation from extension- or module-specific validation? |
| C11 | Core Change Isolation | Can optional capability be added without modifying central framework responsibilities? |
| C12 | Normative Governance Constraints | Are restrictions expressed normatively through language-level or framework-level rules? |

---

## 4. Expanded Capability Matrix

| Capability | SOFA | MOOSE | preCICE | VTK | FMI | OpenMDAO | Modelica | OpenFOAM | ThermoCore | Current Interpretation |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|
| C1 Authoritative State | P | P | N | N/A | P | P | N | P | Y | Modelica intentionally defines coupled equation systems rather than one central state owner. OpenFOAM solvers commonly create and evolve named fields, but authority is solver- and application-specific rather than framework-wide. |
| C2 Explicit State Owner | P | P | Y | N/A | Y | P | N | P | Y | FMI assigns state responsibility according to FMU mode. Modelica equations jointly constrain variables without requiring one procedural owner. OpenFOAM solver code and models share field evolution responsibilities. |
| C3 Read Does Not Confer Ownership | U | Y | P | P | Y | Y | P | P | Y | Modelica connector semantics constrain compatibility and equation generation, but do not establish a general representation-consumer ownership doctrine. OpenFOAM uses const references in many model APIs, but mutable field correction is also a normal extension mechanism. |
| C4 Representation Cannot Redefine State | P | N/A | N/A | P | N/A | N/A | N/A | N | Y | OpenFOAM runtime-selected models and fvModels may add source terms, constrain equations, or correct fields. Such modification is intentional and not prohibited as representation redefinition. |
| C5 Extension-owned State | P | Y | Y | P | Y | Y | Y | Y | Y | Modelica components, FMUs, MOOSE Materials, preCICE participants, and OpenFOAM models can all maintain component-local state. This capability is established prior art. |
| C6 Core Complete Without Extensions | P | Y | Y | Y | Y | Y | Y | Y | Y | Language, library, or framework cores remain usable without any particular optional model. This capability is not distinctive. |
| C7 Interface-governed Communication | Y | Y | Y | Y | Y | Y | Y | Y | Y | Modelica has particularly strong connector restrictions and generated connection equations. This capability is firmly prior art. |
| C8 Ownership Preservation Across Communication | U | P | Y | P | Y | Y | P | P | Y | Modelica preserves connector semantics and equation compatibility but not a single source-owner model. OpenFOAM object registries and field references enable controlled access, while models may intentionally alter equations or fields. |
| C9 State Growth Control | U | P | Y | P | Y | Y | P | P | Y | Modelica tools select states from equations, including explicit StateSelect guidance, but this solves numerical state selection rather than preventing optional domain mechanisms from becoming core state. |
| C10 Validation Boundary | U | Y | P | P | P | P | P | P | Y | MOOSE has direct evidence of module-specific SQA, requirements traceability, verification, and validation records. This substantially weakens any claim that validation-scope isolation is distinctive. |
| C11 Core Change Isolation | P | Y | Y | Y | Y | Y | Y | Y | Y | Modelica replaceable/redeclare mechanisms and OpenFOAM runtime selection enable extension without editing central selection logic. This capability is established prior art. |
| C12 Normative Governance Constraints | U | P | P | N | Y | P | Y | P | Y | Modelica and FMI have strong normative language-level constraints. ThermoCore's remaining distinction cannot be the mere presence of shall-not rules. |

---

## 5. New Evidence Records

### ISO-E08 — Modelica Equation-based Components, Connectors, and State Selection

**Evidence status:** Verified for formal connector governance, connection-equation generation, equation-based component composition, and language-level state selection.

Modelica is an object-oriented language for large heterogeneous physical systems. Components are connected through declared connectors, and `connect` statements generate connection equations. The language specification imposes restrictions on connector compatibility, flow variables, stream variables, dimensions, types, and connection sets.

Modelica equations do not assign procedural ownership to one component. Equations constrain variables simultaneously, and no particular variable must be manually selected as the solved output. Tools may select numerical states from candidate variables, while `StateSelect` attributes provide guidance such as `never`, `avoid`, and preferred state selection.

This creates a strong counterexample to broad claims about governed communication and state discipline. Modelica provides formal, normative, multi-domain composition and state-selection mechanisms.

However, it is not equivalent to ThermoCore's candidate restriction. Modelica components are intended to contribute equations to the complete physical model. A component is not generally restricted to interpreting an externally authoritative physical state while being prohibited from redefining its semantics.

**Supported capabilities:** C5 yes, C6 yes, C7 yes, C9 partial, C11 yes, C12 yes.

**Primary sources:**

- Modelica Language Specification 3.6, Preface: https://specification.modelica.org/maint/3.6/preface.html
- Modelica Language Specification 3.6, Equations: https://specification.modelica.org/maint/3.6/equations.html
- Modelica Language Specification 3.6, Connectors and Connections: https://specification.modelica.org/maint/3.6/connectors-and-connections.html
- Modelica Language Specification 3.6, StateSelect: https://specification.modelica.org/maint/3.6/class-predefined-types-and-declarations.html

**Interpretation:**

Modelica strongly falsifies any ThermoCore claim based on formal connector governance, equation composition, or state selection alone. The remaining candidate distinction must depend on restricting ordinary representations and extensions from acquiring authority over a pre-existing Thermodynamic State.

---

### ISO-E09 — OpenFOAM Fields, Runtime Selection, and Equation Modification

**Evidence status:** Verified for runtime-selected model construction and field-oriented solver composition; Under Survey for formal ownership governance and validation boundaries.

OpenFOAM provides runtime selection tables that allow derived models to be selected and constructed from configuration dictionaries without editing central factory logic. Solvers create named volume and surface fields and pass references to physical models. Runtime-selected models may add source terms, constrain equations, or correct fields.

Examples in the source documentation show:

- runtime selection tables holding constructors for derived classes;
- `fvModel` implementations adding source terms to finite-volume matrices;
- optional models correcting fields such as enthalpy or temperature; and
- solvers creating and registering fields such as pressure, velocity, density, energy, and phase fraction.

This is a strong counterexample to broad claims about field-oriented extensibility and core-change isolation. OpenFOAM already permits substantial new physical capability through runtime-selected models and shared fields.

It also shows the boundary difference clearly: OpenFOAM extensions are often intended to modify equations or fields. It therefore does not provide ThermoCore's proposed restriction that an ordinary representation or extension shall not redefine the authoritative Thermodynamic State or core thermodynamic responsibility.

**Supported capabilities:** C5 yes, C6 yes, C7 yes, C9 partial, C11 yes, C12 partial.

**Primary sources:**

- OpenFOAM Source Code Guide, Runtime Selection Tables: https://cpp.openfoam.org/dev/runTimeSelectionTables_8H.html
- OpenFOAM Source Code Guide, fvModel forcing example: https://cpp.openfoam.org/v13/classFoam_1_1fv_1_1forcing-members.html
- OpenFOAM Source Code Guide, field creation example: https://cpp.openfoam.org/v8/solvers_2lagrangian_2sprayFoam_2createFields_8H_source.html
- OpenFOAM Source Code Guide, temperature-limiting model: https://cpp.openfoam.org/v7/classFoam_1_1fv_1_1limitTemperature-members.html

**Open checks:**

1. Whether OpenFOAM formally defines an authoritative owner for a field beyond solver implementation responsibility.
2. Whether function objects are strictly consumers or may modify registered fields.
3. Whether module-level verification and validation boundaries are formally documented.
4. Whether object-registry access has a general ownership doctrine relevant to C3 and C8.

---

### ISO-E10 — MOOSE Module-specific SQA and Verification/Validation Traceability

**Evidence status:** Verified for module-specific SQA structure and requirement-linked verification/validation records.

MOOSE documentation provides SQA extensions that link requirements, design, issues, test results, verification files, and validation files. Individual physics modules publish their own Software Quality Assurance areas and may include Software Requirements Specifications, Software Design Descriptions, Requirements Traceability Matrices, Verification Validation Plans, and Verification Validation Reports.

Examples include Navier-Stokes, Porous Flow, and Electromagnetics module SQA pages.

This provides direct prior art for bounded module-level quality records and validation responsibility. ThermoCore shall not claim that separating framework validation from extension-specific validation is unique.

The remaining question is narrower: whether ThermoCore's explicit state-authority boundary permits a formally smaller revalidation impact after an extension-only change, and whether this effect can be measured experimentally.

**Supported capabilities:** C10 yes; strengthens the existing C6 and C11 findings.

**Primary sources:**

- MOOSE SQA Extension: https://mooseframework.inl.gov/releases/moose/2024-03-08/python/MooseDocs/extensions/sqa.html
- Navier-Stokes SQA: https://mooseframework.inl.gov/releases/moose/2024-03-08/modules/navier_stokes/sqa/
- Porous Flow SQA: https://mooseframework.inl.gov/modules/porous_flow/sqa/
- Electromagnetics SQA: https://mooseframework.inl.gov/modules/electromagnetics/sqa/

---

## 6. Revised Findings

### F-ISO-10 — Normative connector governance is established prior art

Modelica formally specifies connector compatibility, connection-set construction, equation generation, and connection restrictions. ThermoCore cannot claim distinctiveness merely because communication is constrained by declared semantics.

### F-ISO-11 — Numerical state selection is not the same as semantic state authority

Modelica's state selection determines which variables serve as numerical states in a compiled equation system. ThermoCore's candidate contribution concerns which architectural responsibility is permitted to define and evolve the meaning of Thermodynamic State.

These concepts shall not be conflated.

### F-ISO-12 — Field-oriented extensibility is established prior art

OpenFOAM already supports named physical fields, runtime-selected models, source-term injection, equation constraints, and field correction. ThermoCore cannot claim novelty for field-based extensibility or model selection.

### F-ISO-13 — Module-specific V&V isolation is established prior art

MOOSE documents module-specific SQA, requirements traceability, verification, and validation. ThermoCore's potential value must be measured as change-impact isolation under its specific state-authority rules, not merely separate validation folders.

### F-ISO-14 — Remaining distinction is now domain- and role-specific

The surviving hypothesis is no longer broad enough to be called generic governed isolation.

A more precise candidate is:

> In a real-time thermodynamic framework, one authoritative Thermodynamic State may be evolved by the thermodynamic computational responsibility, while optional Representation Consumers and ordinary Extension Modules may interpret, consume, or contribute through declared boundaries without acquiring authority to redefine state semantics, ownership, or core completeness.

This remains Under Survey.

---

## 7. Change-request Candidates Derived from Prior Art

The following are non-normative design inspirations. They shall not enter the Framework Specification without the normal Research → Evidence → Specification process.

### CR-CAND-01 — Separate architectural authority from execution lifecycle

**Source inspiration:** FMI state machines and Modelica execution semantics.

ThermoCore should preserve a clear distinction among:

- who owns a responsibility;
- what communication is legal; and
- when or in what order implementation operations occur.

Potential request: add an informative clarification that ownership and communication semantics do not prescribe execution lifecycle, scheduling, or call order.

### CR-CAND-02 — Machine-readable ownership and communication inspection

**Source inspiration:** OpenMDAO connection listing, FMI model descriptions, and Modelica connector metadata.

Potential validation tooling could report:

- information category;
- authoritative owner;
- permitted suppliers;
- permitted consumers;
- extension-owned state;
- communication path; and
- undeclared bypasses.

This is currently a Validation-tool candidate, not a normative API requirement.

### CR-CAND-03 — Explicit distinction between numerical state and architectural state

**Source inspiration:** Modelica `StateSelect` and equation-based state selection.

Potential request: add terminology clarifying that Thermodynamic State is an architectural information category and shall not be confused with backend-selected numerical integration variables.

This clarification may be important for implementations that transform, compress, reconstruct, or choose alternative numerical state vectors.

### CR-CAND-04 — Evidence-linked validation scope

**Source inspiration:** MOOSE SQA requirement, verification, and validation linkage.

Potential request: define an informative validation-impact record that links each changed requirement or component to the validation evidence that must be repeated.

This shall remain outside normative conformance until supported by project evidence.

### CR-CAND-05 — Runtime-selected implementation mechanisms remain non-normative

**Source inspiration:** OpenFOAM runtime selection tables.

Potential request: provide an implementation example showing that Material Representation or Extension implementations may be selected at runtime without making a factory, registry, or plugin mechanism part of Framework Conformance.

---

## 8. Updated Falsification Conditions

The remaining hypothesis shall be rejected or narrowed if an existing framework is found that jointly and explicitly requires:

1. one authoritative physical-domain state;
2. one assigned architectural responsibility for evolving that state;
3. representation components that may interpret but may not redefine that state;
4. ordinary extensions that may own mechanism-specific state but may not promote it into core state;
5. communication that does not transfer physical-semantic authority;
6. core completeness and conformance without optional extensions; and
7. a bounded validation impact derived from these responsibilities.

Language-level connector restrictions, plugin APIs, field registries, source-term models, numerical state selection, and module-specific V&V are individually insufficient to establish equivalence.

---

## 9. Current Classification

| Item | Classification |
|---|---|
| Formal connector governance | Verified prior art |
| Equation-based multi-domain composition | Verified prior art |
| Numerical state selection | Verified prior art |
| Field-oriented runtime model selection | Verified prior art |
| Source-term and field-correction extensions | Verified prior art |
| Module-specific SQA and V&V records | Verified prior art |
| Representation non-authority over shared Thermodynamic State | Under Survey |
| Reduced Core modification under meaningful extensions | Unverified hypothesis |
| Reduced revalidation impact due to state-authority boundaries | Unverified hypothesis |
| Reduced mandatory state growth | Unverified hypothesis |
| Research Gap | Not established |

---

## 10. Interim Conclusion

Modelica, OpenFOAM, and MOOSE remove three additional broad claims from consideration:

- formal connector and equation governance is not distinctive;
- field-based runtime extensibility is not distinctive; and
- module-specific validation separation is not distinctive.

The surviving hypothesis is now deliberately narrow and application-specific. ThermoCore may provide value by restricting which roles can acquire semantic authority over one thermodynamic state, rather than by inventing modularity, connectors, fields, plugins, or separate validation.

The next phase should combine two activities:

1. continue searching for direct equivalents in digital-twin and simulation-state architectures; and
2. design implementation experiments that test whether these restrictions produce measurable change isolation, state-growth control, and revalidation reduction.
