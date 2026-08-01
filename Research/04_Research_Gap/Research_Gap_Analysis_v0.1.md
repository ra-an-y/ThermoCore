# Research Gap Analysis

Version: 0.1  
Status: Research Draft  
Research Question: RQ-001

---

## 1. Objective

This document identifies architectural research gaps revealed by the completed Literature Survey, Evidence Matrix, Architecture Comparison Matrix, Architecture Pattern Matrix, and Framework Evidence Summary.

The analysis distinguishes among:

- practices consistently established in the reviewed evidence;
- practices implemented inconsistently across existing frameworks;
- architectural boundaries that remain insufficiently resolved; and
- candidate architectural contributions to be investigated by the Decoupled Thermodynamic Simulation Framework (DTS).

The purpose of this analysis is not to establish novelty, superiority, or universal applicability. Its purpose is to identify unresolved architectural problems that provide the motivation and evaluation scope for RQ-001.

This document is a research analysis. It does not define framework conformance requirements and does not promote candidate principles into Framework Specification.

---

## 2. Established Practices

The reviewed evidence supports several recurring architectural practices. These practices form the existing engineering baseline and shall not be presented as original DTS contributions.

### 2.1 Material–Solver Separation

Existing frameworks commonly distinguish material information from the numerical procedures that evolve a simulated system. Material properties are supplied to computational components without requiring each solver to be defined as a material-specific implementation.

The evidence therefore supports material–solver separation as an established architectural practice. The existence of this separation alone does not constitute a DTS contribution.

### 2.2 Runtime Material Abstraction

Existing frameworks commonly transform program-side material descriptions into forms that can be accessed during simulation. The specific forms differ, but the broader use of a runtime material abstraction is established.

DTS may investigate a particular boundary for this abstraction, but it cannot claim the general concept of runtime material abstraction as novel.

### 2.3 Core / Extension Architecture

The reviewed frameworks commonly separate broadly reusable capabilities from optional or domain-specific mechanisms. Extension mechanisms may introduce additional properties, source terms, state, or coupled equations while relying on a comparatively stable computational foundation.

Core / Extension separation is therefore an established architectural pattern. The unresolved issue is not whether extension is useful, but how extension responsibilities and coupling boundaries should be defined.

### 2.4 Interface-based Coupling

Existing architectures commonly use defined interfaces, data contracts, or equivalent abstraction boundaries to connect solvers, material models, and additional physical mechanisms.

Interface-based coupling is an established engineering practice. A DTS contribution, if supported, would need to concern the specific responsibilities and constraints assigned to its interfaces rather than the use of interfaces itself.

### 2.5 Verification and Validation

The evidence supports verification and validation as necessary but distinct activities in simulation framework development. Architectural conformance, numerical correctness, physical consistency, and performance require explicit evaluation rather than assumption.

DTS adopts this established practice through staged verification and validation. The use of verification and validation is not itself a DTS contribution.

---

## 3. Inconsistent Practices

The reviewed evidence does not indicate one consistently adopted solution for several architectural concerns. These concerns are characterized by variation among frameworks rather than by a demonstrated hierarchy of better and worse solutions.

### 3.1 Material Representation Boundary

Frameworks differ in where material responsibility ends and solver responsibility begins. In some architectures, material models provide properties only. In others, material-related components also resolve state-dependent behavior, phase behavior, constitutive response, or solver-facing runtime data.

The evidence establishes variation in responsibility placement. It does not establish one universally applicable material representation boundary.

### 3.2 State Ownership

Frameworks differ in where evolving simulation state is stored and managed. State may be associated with cells, fields, material points, solver-owned structures, constitutive models, or coupled modules.

These alternatives reflect different numerical methods and framework scopes. The current evidence does not support treating any single ownership model as universally superior.

### 3.3 Minimal Primary State

The reviewed architectures do not use a uniform definition of the minimum state that must persist between updates. Some retain a compact set of primary variables, while others retain additional quantities for constitutive history, coupling, numerical convenience, or performance.

The evidence supports minimizing unnecessary persistent state as a general design concern, but it does not establish one universal primary-state set for all thermodynamic frameworks.

### 3.4 Derived State

Frameworks differ in which quantities are stored and which are reconstructed from primary state and material information. Temperature, phase fractions, response variables, and other material-dependent quantities may be primary in one architecture and derived in another.

The choice is affected by governing equations, numerical formulation, physical mechanisms, and runtime requirements. The evidence does not support a universal stored-versus-derived classification.

### 3.5 Runtime Representation

Existing frameworks employ different runtime forms for material and state information, including direct object access, tables, indexed data, fields, and solver-specific structures.

The evidence supports the need for a runtime representation but does not establish one representation as optimal or universally applicable.

---

## 4. Unresolved Architectural Boundaries

The inconsistencies above expose architectural questions that remain insufficiently standardized across the reviewed frameworks. These questions define the motivation for RQ-001.

### 4.1 Ownership of Evolving Simulation State

It remains necessary to determine whether a thermodynamic framework can define explicit ownership rules that separate reusable material definition from per-location evolving state without constraining the framework to one implementation technique or numerical method.

The unresolved question is:

> Which framework component should own each category of evolving state, and which state may be shared only as immutable or reusable configuration?

### 4.2 Responsibility of Material Representation

Material representation may refer to static material properties, state-dependent thermodynamic interpretation, application-facing material response, or combinations of these responsibilities.

The unresolved question is:

> Which responsibilities belong to Material Representation, and which must remain within Thermodynamic Computation or an Extension Module?

### 4.3 Runtime Material Abstraction

Although runtime material abstraction is established, the relationship between program-side material definition and computation-ready runtime data remains variable.

The unresolved question is:

> Can a compiled runtime representation provide a stable boundary between reusable material definition and material-independent thermodynamic computation without prescribing a specific storage format or backend?

### 4.4 Extension Coupling Boundary

Core / Extension architectures are common, but reviewed frameworks do not provide one uniform rule for when an additional physical mechanism should update properties, contribute source terms, own additional state, or modify governing equations.

The unresolved question is:

> Which coupling mechanisms preserve core stability, and under what conditions does an extension require a change to the governing thermodynamic computation?

### 4.5 Relationship to RQ-001

RQ-001 is motivated by the combined absence of a consistently defined boundary across state ownership, Material Representation, runtime material abstraction, and extension coupling.

The research gap is therefore not the absence of material–solver separation, modularity, interfaces, or validation. Those practices are already established. The gap concerns whether these established practices can be organized into an explicit and internally consistent thermodynamic framework architecture, and whether that architecture can be evaluated without claiming universal applicability.

---

## 5. Candidate DTS Contribution

The following items are candidate architectural contributions derived from the unresolved boundaries. They remain under investigation and shall not be treated as validated contributions or formalized by this document as Framework Specification.

### 5.1 Explicit State Ownership

DTS proposes explicit ownership categories in which reusable material definitions do not own evolving per-cell thermodynamic state, thermodynamic cells or equivalent state locations own core thermodynamic state, and extensions own extension-specific state.

RQ-001 shall evaluate whether this ownership model produces clear responsibilities while remaining compatible with the engine-agnostic and implementation-agnostic scope of the framework.

### 5.2 Independent Material Representation

DTS investigates Material Representation as an architectural responsibility independent from the evolution of thermodynamic state.

The investigation shall determine whether material-dependent interpretation and application-facing response can remain separate from the computational ownership of evolving thermodynamic state.

### 5.3 Compiled Runtime Representation

DTS explores a separation between program-side Material Definition and a computation-ready runtime representation.

The candidate contribution is the explicit architectural role of this transformation and boundary, not a particular buffer, table, object model, data layout, or compilation technique.

### 5.4 Strict Core / Extension Boundary

DTS proposes that optional physical mechanisms couple through defined categories such as property updates, source terms, and extension-owned state. A change to core thermodynamic computation would require evidence that the governing conservation equations have fundamentally changed.

RQ-001 shall evaluate whether these categories are sufficient to preserve a stable core without preventing necessary physical coupling.

### 5.5 Material-independent Thermodynamic Computation

DTS investigates whether thermodynamic computation can operate through standardized material-referenced data without embedding material-specific simulation logic in the computational core.

This candidate contribution concerns the explicit combination of responsibilities and boundaries within DTS. It does not claim that material–solver separation or material-independent solver design is absent from prior work.

### 5.6 Candidate Status

The candidate contributions above require subsequent specification, implementation, verification, and validation. Their inclusion in this analysis indicates research relevance only.

Failure to verify a candidate shall result in revision or rejection of that candidate rather than reinterpretation of the evidence as confirmation.

---

## 6. Claims Not Supported by Current Evidence

The current evidence does not support the following claims:

- DTS is the first framework to separate thermodynamic computation from material representation.
- DTS is the first framework to use runtime material abstraction, explicit interfaces, or a Core / Extension architecture.
- DTS is a universal thermodynamic framework.
- DTS is a general multiphysics framework.
- The proposed DTS architecture is optimal.
- The proposed state ownership model is universally applicable.
- A minimal persistent state can be defined identically for all physical mechanisms, numerical formulations, and applications.
- The proposed runtime material representation is universally applicable or superior to all alternatives.
- The proposed extension boundary is sufficient for every additional physical mechanism.
- Material-independent thermodynamic computation eliminates all material-dependent behavior from simulation.
- The current analysis provides complete proof of architectural superiority.
- Successful verification or validation of one implementation would prove universal validity of the architecture.

These exclusions apply unless future evidence directly supports a narrower, testable claim. Unsupported claims shall not be introduced into Framework Specification, implementation documentation, validation reports, or publication materials.

---

## 7. Implications for Future Research

The present analysis establishes a bounded architectural problem for RQ-001. It does not establish the outcome of that research question.

### 7.1 RQ-001 Evaluation

The immediate research task is to convert the candidate boundaries into testable architectural requirements and evaluate whether they provide:

- unambiguous responsibility allocation;
- separation of evolving state from reusable material definition;
- material-independent core computation;
- stable and explicit extension coupling; and
- compatibility with engine-agnostic implementation.

These criteria require later verification and validation. They are not confirmed by this analysis alone.

### 7.2 RQ-002

RQ-002 may investigate physical-field-driven material response after the RQ-001 architecture has been specified and evaluated. RQ-002 shall remain separate from the present architectural conclusions and shall not be used retroactively to expand the DTS core.

### 7.3 Additional Physical Mechanisms

Thermal hysteresis, moisture, chemical reaction, mechanical behavior, electromagnetic coupling, and other mechanisms may be examined as future extensions. Each mechanism shall first be analyzed to determine whether it can couple through properties, source terms, or extension-owned state, or whether it changes the governing conservation equations.

Their identification as future research directions does not establish their compatibility with the current candidate boundary.

### 7.4 Mechanical and Electromagnetic State

Mechanical State and Electromagnetic State may require ownership models, governing equations, and representations distinct from Thermodynamic State. They shall not be added to the DTS core solely because an abstract relationship among energy, state, and representation can be described.

Any future integration shall require an independent evidence review and research-gap analysis.

### 7.5 Generalized Energy Framework

A generalized framework spanning multiple energy domains may be explored only as a future research hypothesis. The current evidence and DTS scope do not support presenting ThermoCore as such a framework.

When uncertainty exists, preservation of the stable thermodynamic core takes precedence. New architectural ideas shall enter Research documents first and may become Framework Specification only after sufficient evidence, specification review, and evaluation support them.

---

## Evidence Boundary

This analysis is limited to the findings consolidated in the following project artifacts:

- Literature Survey
- Evidence Matrix
- Architecture Comparison Matrix
- Architecture Pattern Matrix
- Framework Evidence Summary
- Framework Principles

The document does not independently extend the literature set, infer absence from unreviewed frameworks, or convert recurring patterns into novelty claims.

---

## Document Status

This document completes the initial research-gap analysis step of RQ-001 and provides input to subsequent Framework Specification work.

It is non-normative. Where this document and the normative Framework Principles differ in authority, the Framework Principles govern the current framework baseline. Candidate findings in this document require explicit review before they may modify or extend that baseline.
