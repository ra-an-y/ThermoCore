# RQ-EFM-001 Definition v0.1

Status: **UNDER SURVEY — Research Question Definition**  
Research Question: **RQ-EFM-001**  
Title: **External Energy / Physical Field–Driven Material Response**  
Tracking: GitHub Issue #84  

---

## 1. Purpose

This document defines the next ThermoCore research question after completion of RQ-ISO-001.

RQ-EFM-001 investigates how externally driven physical mechanisms should couple to a thermodynamic framework when the mechanism may contribute energy, alter material response, maintain mechanism-specific history, or require governing state beyond the current thermodynamic Core.

This document is non-normative. It does not modify the ThermoCore Framework Specification, the reference thermodynamic formulation, production implementation, Validation, Performance conclusions, Framework Conformance, or the frozen ThermoCore v1.0.0 publication baseline.

No Research Gap or novelty claim is established by this document.

---

## 2. Background

ThermoCore currently separates:

- authoritative Thermodynamic State;
- Thermodynamic Computation / State Evolution;
- Material Definition and Material Representation;
- Framework Interfaces; and
- ordinary Extension Modules.

The current bounded reference formulation uses fixed per-cell material mass and specific enthalpy as Persistent Thermodynamic State. It explicitly excludes variable-mass, compressible, mechanical-work, reactive-transport, and other governing mechanisms outside that formulation's scope.

RQ-ISO-001 subsequently evaluated a separate architecture question: whether ordinary extension-specific state can remain outside mandatory Core State while preserving a fixed semantic/Core-state boundary, and whether the boundary still requires Core revision when governing physics changes.

RQ-ISO-001 does not establish how a particular external energy or physical field mechanism should be classified physically. RQ-EFM-001 addresses that missing classification problem.

---

## 3. Primary Research Question

> **Under a bounded thermodynamic-framework model, when may an externally driven material-response mechanism be represented through declared energy/source contributions, material-property updates, or extension-owned local state without changing authoritative Thermodynamic State semantics or the governing thermodynamic conservation formulation, and what conditions instead require explicit Core revision or out-of-scope treatment?**

The question is intentionally two-sided.

It does not assume that every external field mechanism is an ordinary extension, and it does not assume that every coupled field requires a new Core state.

---

## 4. Research Scope

RQ-EFM-001 is initially limited to **architecture, information semantics, and coupling classification**.

It asks:

- what information a mechanism requires;
- what information it owns;
- whether the mechanism changes only thermodynamic source terms or material response;
- whether persistent mechanism-local state is sufficient;
- whether new authoritative cross-domain state is required;
- whether the governing thermodynamic formulation remains semantically intact; and
- when a proposed mechanism stops being an ordinary extension and becomes a Core/formulation change.

The first phase does not derive complete constitutive models, implement full multiphysics solvers, or establish physical accuracy for any named mechanism.

---

## 5. Candidate Coupling Classes — Under Survey

The following classes are **research hypotheses / taxonomy candidates**, not established ThermoCore rules.

### C-EFM-1 — Energy / Source Contribution

A mechanism may be representable as an externally supplied energy contribution when its effect on the thermodynamic system can be expressed through the existing energy-evolution responsibility without adding new authoritative governing state.

Candidate examples for evidence testing include:

- photothermal or optical absorption heating;
- resistive / Joule heating under a supplied electrical solution; and
- bounded reaction or dissipation heat where the non-thermal governing variables remain external or extension-owned.

This class shall not be accepted merely because a mechanism eventually produces heat.

### C-EFM-2 — Material-Property Update

A mechanism may alter effective material parameters used by the thermodynamic formulation while leaving authoritative Thermodynamic State and the conservation formulation unchanged.

Candidate examples may include field-dependent:

- heat capacity;
- conductivity or related transport coefficients;
- transition temperature;
- latent-response parameters; or
- other constitutive properties.

Evidence must distinguish true configuration/property change from hidden governing state.

### C-EFM-3 — Extension-Owned Local Persistent State

A mechanism may require persistent history that exists solely for its own responsibility while contributing to thermodynamic evolution through declared boundaries.

Candidate examples may include:

- hysteresis state;
- bounded internal order/history variables;
- kinetic progress variables; or
- field-response memory.

The existence of such state does not by itself establish that the mechanism is an ordinary extension. The state must not be authoritative governing state that the Core actually requires for complete evolution.

### C-EFM-4 — Core / Governing-Formulation Revision

A mechanism requires Core revision or explicit out-of-scope treatment when correct governing evolution requires new authoritative state, conservation responsibilities, or coupled equations that cannot be represented honestly by source/property/local-state coupling alone.

Likely boundary families for later evidence testing include:

- coupled charge/energy transport where electrical state must co-evolve with thermal state;
- thermoelectric transport when current, electrochemical potential, or coupled flux conservation becomes governing rather than externally supplied;
- magneto-/electro-/mechanocaloric formulations requiring additional authoritative order parameters, work terms, or coupled field evolution;
- variable-mass or species transport;
- pressure-volume work / compressible formulations; and
- deformation or mechanical-work formulations requiring authoritative mechanical state.

These are candidate boundary cases, not pre-classified conclusions.

---

## 6. Information Categories to Preserve During Survey

The survey shall explicitly distinguish the following information categories:

### 6.1 Thermodynamic State

Authoritative runtime thermodynamic information required by the selected thermodynamic formulation.

RQ-EFM-001 shall not silently redefine electromagnetic, mechanical, chemical, or field state as Thermodynamic State merely because it influences temperature or enthalpy.

### 6.2 External Field / Cross-Domain State

State belonging to another governing physical domain, such as electric, magnetic, mechanical, chemical, optical, or flow state.

Such state may be supplied by another solver, owned by an extension, or require a future coupled Core architecture depending on evidence.

### 6.3 Material Configuration / Properties

Parameters or constitutive relationships used to interpret or evolve thermodynamic state.

A field-dependent property shall not be classified as mere Configuration if its value actually depends on unresolved authoritative runtime history or cross-domain state.

### 6.4 Extension-Owned State

Persistent information required solely by an optional mechanism and not required to redefine the authoritative Thermodynamic State semantics or Core completeness.

### 6.5 Derived / Representation Information

Information calculated from authoritative state for interpretation, display, mapping, or downstream use. Derived information shall not be used to hide governing state required for evolution.

---

## 7. Representative Mechanism Families for Evidence Survey

The first bounded evidence survey should prioritize mechanism families that place different pressure on the candidate taxonomy.

### Group A — Predominantly Energy-Deposition Candidates

- optical / photothermal absorption;
- prescribed radiative deposition;
- Joule heating when electrical power density is externally solved/supplied;
- viscous or frictional dissipation when mechanical work is externally resolved.

Purpose: test whether mature multiphysics frameworks treat these as source coupling without transferring state authority.

### Group B — Field-Dependent Material-Response Candidates

- magnetocaloric response;
- electrocaloric response;
- mechanocaloric / elastocaloric response;
- field-dependent phase or constitutive response.

Purpose: test whether property updates and extension-local internal variables are sufficient, or whether thermodynamic potentials / additional state variables are normally part of the governing formulation.

### Group C — Strongly Coupled Boundary Candidates

- thermoelectric coupling;
- electrochemical / reactive transport with thermal feedback;
- compressible thermo-fluid coupling;
- thermo-mechanical work coupling.

Purpose: attempt to falsify any overly broad claim that externally driven mechanisms can always enter as sources or property updates.

---

## 8. Initial Prior-Art Search Boundary

The evidence phase should prioritize systems and literature that expose **how coupling responsibilities and state are architected**, not only final governing equations.

Strong initial targets include:

- MOOSE multiphysics kernels/materials and coupled variables;
- OpenFOAM source terms, thermophysical models, electromagnetics/thermoelectric extensions;
- Modelica multi-domain energy coupling and state selection;
- COMSOL Multiphysics coupling interfaces as documented architecture examples;
- MFEM / multiphysics finite-element coupling patterns where relevant;
- preCICE partitioned multiphysics coupling and data ownership;
- specialized literature for Joule heating, photothermal, thermoelectric, magnetocaloric, electrocaloric, and mechanocaloric formulations;
- relevant standards or reference architectures only where they specify physical information ownership or coupling semantics.

The search should stop broadening when additional sources repeat already-established coupling properties without testing the remaining classification boundary.

---

## 9. Evidence Questions

For each reviewed mechanism / architecture, record at minimum:

1. What is the authoritative persistent state?
2. Which solver/responsibility evolves that state?
3. Is the external field prescribed, solved elsewhere, or co-evolved?
4. Does the thermal equation receive a source term, modified property, work term, or coupled flux?
5. Is mechanism-specific history persistent?
6. Who owns that history?
7. Does the mechanism require new conservation equations?
8. Does the mechanism change the thermodynamic state-space definition?
9. Can the mechanism be absent while the thermal Core remains complete?
10. Does implementation modularity correspond to semantic authority separation, or only code modularity?
11. What Verification / Validation evidence becomes mechanism-specific?
12. What fact would force reclassification from ordinary extension to Core/formulation change?

---

## 10. Falsification / Reclassification Conditions

The initial RQ framing shall be rejected, narrowed, or reclassified if evidence shows any of the following.

### F-EFM-1 — No Stable Coupling Taxonomy

If mature formulations do not support a meaningful distinction among source contribution, property update, extension-local state, and governing-state revision, the proposed taxonomy shall be replaced rather than forced onto the evidence.

### F-EFM-2 — Source-Term Framing Is Physically Misleading

If mechanisms commonly described as external heating actually require coupled authoritative state or work terms for correct evolution, they shall not be retained in the source-only class.

### F-EFM-3 — Property Update Hides Governing State

If a property depends on runtime field/history variables that must be co-evolved as part of the governing physical system, treating it as a simple material-property update shall be rejected.

### F-EFM-4 — Extension-Local State Is Actually Core-Governing State

If removing or hiding the mechanism-local state makes the authoritative thermodynamic evolution incomplete, the mechanism cannot be classified as an ordinary extension solely because the code is modular.

### F-EFM-5 — Prior Art Already Formalizes the Full Boundary

If a reviewed framework or literature line already explicitly formalizes the same state/coupling classification and boundary, any later contribution shall be reframed as adoption, specialization, integration, or evaluation rather than novelty.

### F-EFM-6 — Current Framework Abstraction Is Too Narrow

If representative ordinary mechanisms consistently require architectural capabilities not expressible through current Framework Interfaces or Extension semantics without redefining Core responsibilities, the research result may justify a future Framework change — but only after Evidence → Gap → Specification.

---

## 11. Relationship to RQ-ISO-001

RQ-ISO-001 established a bounded research contribution around **Fixed Semantic/Core-State Boundary under Ordinary Extension** and demonstrated that the boundary can preserve Core isolation for evaluated ordinary extensions while still requiring Core revision for a governing-physics counterexample.

RQ-EFM-001 uses that result only as methodological and architectural background.

RQ-ISO-001 does **not** prove:

- that a named field-driven mechanism is an ordinary extension;
- that source-term coupling is sufficient;
- that extension-local state is physically complete;
- that thermoelectric or caloric effects belong outside Core; or
- that external physical fields never require changes to Thermodynamic State semantics.

Those classifications require new mechanism-specific evidence under RQ-EFM-001.

---

## 12. Claims Not Supported at Definition Stage

RQ-EFM-001 v0.1 does not support claims that:

- all external energy mechanisms are source terms;
- all physical fields can be handled by Extension Modules;
- thermodynamic conservation equations never need revision;
- ThermoCore already supports coupled electromagnetics, mechanics, chemistry, or flow;
- external field variables are Thermodynamic State;
- a Research Gap exists;
- the proposed coupling taxonomy is novel;
- ThermoCore is superior to existing multiphysics frameworks; or
- any current Validation evidence validates the listed field-driven mechanisms.

---

## 13. Initial Research Classification

| Item | Classification |
|---|---|
| RQ-EFM-001 primary question | Defined |
| Source contribution class | Under Survey |
| Material-property update class | Under Survey |
| Extension-owned persistent-state class | Under Survey |
| Core / governing-formulation revision boundary | Under Survey |
| Representative mechanism classification | Under Survey |
| Research Gap | Not established |
| Research Contribution | Not established |
| Novelty / priority | Not established |
| Framework Specification change | Not authorized |

---

## 14. Next Stage

The next stage is a bounded prior-art and mechanism-evidence survey.

The first evidence task should build an **External Energy / Field Coupling Evidence Matrix** that compares representative mechanism families against the information and authority questions in Section 9.

The survey should begin with mechanisms that are deliberately different in coupling strength:

1. externally supplied Joule heating or photothermal energy deposition;
2. one caloric effect with field-dependent material response;
3. thermoelectric or another co-evolved transport mechanism as a strong boundary case.

The goal is not to confirm the candidate taxonomy. The goal is to try to break it early.

Only after evidence convergence may RQ-EFM-001 proceed to a Research Gap analysis, consequence-test design, or any Framework Specification proposal.
