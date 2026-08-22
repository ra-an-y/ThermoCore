# External Field Coupling Evidence Matrix v0.3

Status: **UNDER SURVEY — final bounded evidence closure pass**  
Research Question: **RQ-EFM-001**  
Date: **2026-08-23**  
Tracking: GitHub Issue #90  
Dependencies:

- `Research/05_Research_Questions/RQ_EFM_001_Definition_v0.1.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.1.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.2.md`

---

## 1. Objective

This document performs the final bounded evidence pass before any dedicated Research Gap analysis for RQ-EFM-001 — **External Energy / Physical Field–Driven Material Response**.

The v0.1 evidence pass established that source-term coupling is mature prior art and that strongly coupled physical domains need not be collapsed into one shared thermodynamic state. The v0.2 evidence pass then showed that the provisional five-label vocabulary is not a mutually exclusive taxonomy: interaction form, persistent state, coupling direction, and authority impact are orthogonal dimensions that can coexist within one mechanism.

The remaining question is therefore narrower:

> **Is the distinction between Core-preserving external/cross-domain coupling and genuine thermodynamic state/formulation revision already established by thermodynamic state-space theory and multi-domain modeling, or does a narrower framework-level authority/completeness boundary remain unresolved?**

This is a falsification-oriented evidence task. It is not intended to preserve an RQ-EFM-001 contribution if prior art already covers the distinction.

This document is non-normative. It does not modify the ThermoCore Framework Specification, reference thermodynamic formulation, production implementation, Verification, Validation, Performance, Framework Conformance, the frozen v1.0.0 release baseline, or the completed RQ-ISO-001 disposition.

No Research Gap, novelty, priority, or research-contribution claim is established by this document.

---

## 2. Cumulative Findings Entering v0.3

### 2.1 v0.1 findings retained

v0.1 established the following prior-art facts:

- externally computed power or loss may enter a thermal equation as a source contribution;
- Joule heating and optical absorption are established source-coupling patterns;
- temperature-dependent electrical or optical properties can make a source-coupled problem bidirectional;
- thermoelectric coupling can require co-evolving electrical and thermal governing equations without implying that electrical state becomes Thermodynamic State;
- partitioned multiphysics can preserve separate solver/state responsibilities while exchanging governing quantities; and
- caloric effects cannot be classified from mechanism name alone as simple heat sources or coefficient updates.

The original four-class hierarchy therefore failed as a complete classification.

### 2.2 v0.2 findings retained

v0.2 established that the refined five labels are also not a mutually exclusive taxonomy.

In particular:

- constitutive/property coupling and persistent internal state can coexist;
- a mechanism can have a thermal-side source or flux while also participating in cross-domain governing coupling;
- solver partitioning versus monolithic solution is not equivalent to semantic state authority;
- reduced, equilibrium, dynamic, and hysteretic formulations of the same named physical mechanism may legitimately require different state variables; and
- generic multi-axis coupling classification is itself prior art and shall not be claimed as a ThermoCore contribution.

The evidence therefore favored four orthogonal research axes:

1. **Thermal / thermodynamic interaction form** — source, flux/work exchange, constitutive/property dependency, or state-space/formulation change.
2. **Driving / internal-state role** — prescribed input, algebraic/equilibrium variable, mechanism-local persistent variable, cross-domain governing state, or thermodynamic governing state.
3. **Coupling relation** — one-way/bidirectional and explicit/implicit, with partitioned/monolithic implementation treated separately from authority.
4. **Thermodynamic authority impact** — existing selected Thermodynamic State/formulation remains complete, or the selected thermodynamic state-space/formulation must be revised.

### 2.3 Remaining v0.3 falsification target

The provisional boundary entering v0.3 is:

> **After another physical domain or mechanism is abstracted to declared inputs/exchanges, does the selected thermodynamic state-space and governing formulation remain semantically complete for the claimed model scope?**

If yes, strong external or cross-domain coupling does not by itself require Thermodynamic Core revision.

If no, the selected thermodynamic state-space/formulation must be enlarged, revised, or declared out of scope.

v0.3 tests how much of this statement is already established prior art.

---

## 3. Evidence Questions

Each new evidence record is evaluated against the following questions where the source exposes enough information:

1. Does the source define generalized thermodynamic coordinates, conjugate forces, or work pairs?
2. Does the source treat state-variable selection as formulation-dependent?
3. Does the source enlarge the state space when current variables are insufficient to determine material response without history?
4. Does the source distinguish external driving variables from internal persistent variables?
5. Does the source support separate physical-domain states coupled through explicit energy/power/data exchange?
6. Does the source require all coupled physical-domain variables to become one thermodynamic state set?
7. Does the source give an operational criterion comparable to ThermoCore's proposed Core-preserving-versus-Core-revision decision?
8. Is the evidence physics-theory prior art, software-architecture prior art, or both?
9. Which later novelty candidates must be excluded?
10. What remains unresolved after the evidence is accounted for?

Unknown or unexposed semantic ownership is recorded as unknown rather than inferred from implementation structure.

---

## 4. New Evidence Records

### E-EFM-17 — Generalized Thermodynamics of Multicaloric Effects

**Source family:** peer-reviewed multicaloric thermodynamics  
**Mechanism:** generalized magnetic/electric/mechanical field-driven caloric response  
**Primary sources:**

- https://pmc.ncbi.nlm.nih.gov/articles/PMC4938063/
- https://cpb.iphy.ac.cn/article/2020/2027/cpb_29_4_047504.html
- https://www.sciencedirect.com/science/article/pii/S0020768316302281

**Observed physics:**

The reviewed multicaloric literature formulates caloric response using generalized thermodynamic displacements/properties and their conjugate generalized fields. Representative pairs include:

- magnetization `M` and magnetic field `H`;
- polarization `P` and electric field `E`;
- strain `epsilon` and stress `sigma`; and
- volume-related coordinates and pressure.

The product of generalized field and conjugate displacement has energy-density character, and Maxwell-type relations connect field changes to entropy and temperature responses.

The generalized framework explicitly permits cross-coupling among multiple ferroic coordinates and fields.

**Evidence result:**

`GENERALIZED WORK-PAIR REPRESENTATION — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

This eliminates any later claim that RQ-EFM-001 contributes the general idea of treating magnetic, electric, and mechanical influences as generalized thermodynamic work pairs or conjugate field-coordinate pairs.

It also shows that a field-driven material response may be represented within a thermodynamic potential rather than as an externally deposited heat source.

**Limit of evidence:**

The sources are thermodynamic formulations. They do not establish ThermoCore-style software ownership, extension governance, or a fixed Core/Extension authority rule.

---

### E-EFM-18 — Electrocaloric Gibbs / Landau Thermodynamics

**Source family:** electrocaloric reference and review literature  
**Mechanism:** electric-field-driven entropy / temperature response  
**Primary sources:**

- https://doi.org/10.1002/047134608X.W8244
- https://www.sciencedirect.com/science/article/pii/B9780128216477000025
- https://pmc.ncbi.nlm.nih.gov/articles/PMC12430785/

**Observed physics:**

Electrocaloric formulations commonly express free energy using temperature, electric field and polarization, and where relevant mechanical stress/strain. Maxwell formulations can treat equilibrium relationships algebraically, while Landau or Landau-Ginzburg-Devonshire formulations make polarization/order response explicit.

Time-dependent formulations may evolve polarization using a Landau-Khalatnikov-type equation while solving a heat equation for temperature. Gradient terms may be required when spatially inhomogeneous domain structures are modeled.

**Evidence result:**

`SAME MECHANISM — MULTIPLE LEGITIMATE STATE-SPACE CHOICES`

**Taxonomy pressure:**

Electrocaloric behavior cannot be assigned a permanent mechanism-level label such as `property update`, `local state`, or `Core revision` without specifying the selected formulation and model scope.

A reduced equilibrium model may absorb field dependence into an algebraic constitutive relation. A dynamic/hysteretic model may require polarization/order variables and their evolution.

**Prior-art consequence:**

Formulation-dependent state choice is not a ThermoCore novelty candidate.

---

### E-EFM-19 — Coleman–Gurtin Internal State Variable Thermodynamics

**Source family:** foundational nonequilibrium thermodynamics  
**Source:**

- Coleman, B. D. and Gurtin, M. E., *Thermodynamics with Internal State Variables*, Journal of Chemical Physics 47, 597–613 (1967), DOI `10.1063/1.1711937`.
- Bibliographic/abstract access: https://cir.nii.ac.jp/crid/1363951795581250560

**Observed theory:**

The Coleman–Gurtin framework treats nonlinear materials with internal state variables whose temporal evolution is governed by evolution equations. Internal variables extend the ordinary state description to represent material behavior that cannot be adequately captured by the conventional observable variables alone.

Later internal-state-variable literature uses this approach for plasticity, damage, viscoelasticity, phase transformation, multiphysics materials, and other history-dependent behavior.

**Evidence result:**

`STATE-SPACE ENLARGEMENT FOR INSUFFICIENT STATE DESCRIPTION — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

This strongly eliminates any claim that RQ-EFM-001 originates the principle:

> when current variables are insufficient to determine the material response, additional persistent state/internal variables are required.

That principle is mature thermodynamic prior art.

**Limit of evidence:**

Internal-variable theory does not by itself determine whether a given software framework should classify the additional variable as Thermodynamic Core State, extension-owned mechanism state, or another physical-domain state. That is an architectural/semantic allocation question layered on top of the physical state-space requirement.

---

### E-EFM-20 — Historical / Modern Internal-Variable State-Space Theory

**Source family:** continuum thermomechanics / internal-variable reviews  
**Primary sources:**

- https://www.sciencedirect.com/science/article/pii/S0093641315001056
- https://www.sciencedirect.com/science/article/pii/S0749641910000847
- https://pmc.ncbi.nlm.nih.gov/articles/PMC10217598/

**Observed theory:**

Internal variables are widely used to represent unresolved material structure and dissipative history in viscoelasticity, plasticity, damage, phase transformations, rheology, and coupled-field problems.

The reviewed literature distinguishes ordinary observable state variables from additional internal variables and derives evolution relations subject to thermodynamic restrictions.

**Evidence result:**

`INTERNAL-VARIABLE / HISTORY-STATE CLASSIFICATION — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

`C-EFM-3` cannot be treated as a new conceptual invention. Stateful mechanism-local or constitutive-history variables have an extensive existing thermodynamic foundation.

The surviving research question, if any, must concern how such variables are governed relative to a reusable framework's authoritative Core State — not the existence of internal variables themselves.

---

### E-EFM-21 — Constitutive State Space and Equipresence

**Source family:** continuum thermodynamics / constitutive theory  
**Primary sources:**

- https://www.sciencedirect.com/science/article/pii/S0196890412003780
- https://pmc.ncbi.nlm.nih.gov/articles/PMC10528319/
- https://www.sciencedirect.com/science/article/pii/S0022509623000492

**Observed theory:**

Continuum-thermodynamics formulations explicitly define a state or constitutive space containing variables on which constitutive quantities depend. The principle of equipresence requires constitutive quantities initially to depend on the same selected set unless restrictions eliminate dependencies.

Modern constitutive-modeling work states directly that selecting an adequate material state space is essential. Internal variables are introduced when a local-in-time description needs additional information to represent irreversible or complex mechanisms.

**Evidence result:**

`STATE-SPACE SELECTION / SUFFICIENCY AS A MODELING PROBLEM — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

The abstract notion of asking whether a selected state space is sufficient/adequate is not new.

**Important distinction:**

The reviewed continuum theory generally asks what variables a constitutive material description requires. It does not automatically answer the software-architecture question of which subsystem owns those variables, whether an optional capability may remain outside a framework Core, or whether Core completeness may depend on an extension.

---

### E-EFM-22 — Minimal State / Response-Equivalent Histories

**Source family:** thermodynamics of materials with memory  
**Source:**

- https://arpi.unipi.it/handle/11568/150079

**Observed theory:**

The minimal-state approach defines state through material response: different histories are equivalent if every admissible continuation produces the same response. The state can therefore be represented by the equivalence class needed to predict future response rather than by the full raw history.

**Evidence result:**

`RESPONSE-COMPLETENESS / MINIMAL-STATE THINKING — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

This is particularly strong prior-art pressure against claiming a generic "completeness criterion" as novel. Thermodynamic and constitutive theories already use response sufficiency/minimality to reason about state.

**Limit of evidence:**

The source does not define optional software-extension authority, Framework Core completeness, or ownership non-promotion.

---

### E-EFM-23 — Magnetocaloric Hysteresis and Kinetic State

**Source family:** magnetocaloric nonequilibrium / hysteresis literature  
**Primary sources:**

- https://onlinelibrary.wiley.com/doi/full/10.1002/pssb.201700278
- https://www.sciencedirect.com/science/article/abs/pii/S0304885304015549
- https://arxiv.org/abs/1702.08347

**Observed physics:**

Magnetocaloric first-order transitions can exhibit hysteresis, kinetic effects, energy barriers, domain-boundary motion, thermal relaxation, and other out-of-equilibrium behavior.

Thermodynamic models with internal variables have been explicitly used to represent magnetic hysteresis and to derive entropy/entropy-production behavior along hysteretic paths.

**Evidence result:**

`HYSTERETIC CALORIC RESPONSE MAY REQUIRE INTERNAL / NONEQUILIBRIUM STATE — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

The equilibrium-versus-hysteretic distinction is physically meaningful, but not new. A mechanism's state role depends on formulation fidelity and timescale.

This supports the v0.2 conclusion that mechanism names cannot be mapped one-to-one to architecture classes.

---

### E-EFM-24 — Elastocaloric Thermodynamics and Martensitic Transformation

**Source family:** elastocaloric review literature  
**Primary sources:**

- https://link.springer.com/article/10.1007/s40830-024-00477-x
- https://www.sciencedirect.com/science/article/pii/S235249282100698X

**Observed physics:**

Elastocaloric effects in shape-memory alloys arise through stress-induced thermoelastic/martensitic transformations. The thermodynamic response is tied to stress–strain work, latent heat, phase transformation, and frequently hysteresis.

Modeling choices range from equilibrium thermodynamic descriptions to transformation-kinetic and internal-variable models.

**Evidence result:**

`MECHANICAL WORK-PAIR + FORMULATION-DEPENDENT TRANSFORMATION STATE — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

Elastocaloric response further demonstrates that "external field" does not imply "external heat source". The stress field can perform thermodynamic work and may require transformation state depending on model scope.

---

### E-EFM-25 — Bond Graph Multi-Domain Energy Ports

**Source family:** bond-graph multiphysics modeling  
**Primary sources:**

- https://pmc.ncbi.nlm.nih.gov/articles/PMC9497721/
- https://www.mdpi.com/2073-8994/15/12/2170
- https://www.sciencedirect.com/science/article/abs/pii/S1569190X0600027X

**Observed architecture:**

Bond graphs represent power transfer using generalized effort and flow variables whose product is power. Different physical domains use domain-specific effort/flow pairs, including electrical, mechanical, hydraulic, thermal/thermodynamic, and chemical domains.

Bond-graph models can connect different energy domains through power-conserving elements while retaining domain-appropriate energy storage variables. The methodology supports systematic state-variable determination and multi-domain coupling.

**Evidence result:**

`SEPARATE ENERGY-DOMAIN STATE + EXPLICIT POWER COUPLING — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

This eliminates any claim that RQ-EFM-001 invents the idea that separate physical domains may exchange energy without merging their domain-specific states.

It also eliminates any broad claim that an explicit energy-coupling boundary is itself novel.

**Limit of evidence:**

Bond-graph causality and energy-port structure are not the same concept as ThermoCore's normative ownership, Extension Module optionality, or Core completeness governance.

---

### E-EFM-26 — Port-Hamiltonian Multi-Physics Interconnection

**Source family:** port-Hamiltonian systems  
**Primary sources:**

- https://academic.oup.com/imamci/article/37/4/1400/5877069
- https://www.mdpi.com/1996-1073/19/2/324
- https://arxiv.org/abs/2104.13459

**Observed architecture/theory:**

Port-Hamiltonian formulations represent open physical systems in terms of energy storage, dissipation, and interconnection through ports. Distributed and lumped physical subsystems can exchange power or energy while preserving explicit energy-balance structure.

The literature treats energy/power as a common language across physical domains and supports coupling subsystems without requiring all subsystem variables to become one common state category.

**Evidence result:**

`ENERGY-BASED CROSS-DOMAIN INTERCONNECTION — ESTABLISHED PRIOR ART`

**Taxonomy pressure:**

Multi-domain compositional energy coupling cannot be claimed as a new ThermoCore principle.

The possible surviving RQ-EFM-001 issue must be more specific than energy-consistent interconnection.

---

### E-EFM-27 — Modelica Multi-Domain Physical Connectors

**Source family:** Modelica Language Specification / Standard Library  
**Primary sources:**

- https://specification.modelica.org/master/connectors-and-connections.html
- https://specification.modelica.org/master/stream-connectors.html
- https://doc.modelica.org/om/Modelica.UsersGuide.Connectors.html

**Observed architecture:**

Modelica defines physical connectors using potential/across and flow/through variables and extends this with stream variables for convective transport of quantities such as specific enthalpy and composition.

The Modelica Standard Library supplies domain-specific connector definitions for electrical, thermal, mechanical, fluid, and other physical domains. Connection equations enforce domain-relevant conservation relations.

**Evidence result:**

`MULTI-DOMAIN PHYSICAL INTERFACE / CONSERVATION CONNECTORS — ESTABLISHED SOFTWARE PRIOR ART`

**Taxonomy pressure:**

A software architecture that preserves different physical-domain variables while coupling them through physically meaningful interfaces is established prior art.

**Limit of evidence:**

Connector semantics do not by themselves define a fixed authoritative Thermodynamic State owner, optional-extension non-promotion rule, or framework-level decision that Core completeness is invariant under ordinary extension.

---

### E-EFM-28 — Modelica.Media ThermodynamicState

**Source family:** Modelica Standard Library, `Modelica.Media`  
**Primary sources:**

- https://doc.modelica.org/Modelica%204.0.0/Resources/helpWSM/Modelica/Modelica.Media.Interfaces.TemplateMedium.ThermodynamicState.html
- https://doc.modelica.org/Modelica%204.0.0/Resources/helpWSM/Modelica/Modelica.Media.Interfaces.PartialMedium.ThermodynamicState.html
- https://doc.modelica.org/Modelica%203.2.3/Resources/helpWSM/Modelica/Modelica.Media.UsersGuide.MediumUsage.BasicUsage.html
- https://doc.modelica.org/Modelica%204.0.0/Resources/helpWSM/Modelica/Modelica.Media.UsersGuide.MediumDefinition.BasicStructure.html

**Observed architecture:**

`Modelica.Media` explicitly defines `ThermodynamicState` as a selection/minimal set of variables that uniquely defines a medium's thermodynamic state and serves as input to property functions.

Different media may use different independent variable sets. The library supports state construction from alternative pairs such as density/temperature, pressure/enthalpy, pressure/entropy, or pressure/temperature, plus composition where applicable.

The base medium interface intentionally does not impose one universal standard thermodynamic variable set; actual media define the record appropriate to their formulation.

**Evidence result:**

`FORMULATION-RELATIVE THERMODYNAMIC STATE ABSTRACTION — ESTABLISHED SOFTWARE PRIOR ART`

**Taxonomy pressure:**

This is strong direct prior art against claiming that RQ-EFM-001 originates:

- formulation-relative thermodynamic state selection;
- a minimal variable set sufficient to define thermodynamic state; or
- abstraction of property evaluation behind a thermodynamic-state record.

**Important distinction:**

The reviewed `Modelica.Media` sources do not establish the exact ThermoCore rule that an ordinary optional extension may own persistent mechanism state but may not thereby redefine the authoritative framework Thermodynamic State, its owner, mandatory Core membership, or Framework Core completeness.

That distinction was separately investigated under RQ-ISO-001 and remains a narrower governance concept than Modelica's medium-state abstraction.

---

### E-EFM-29 — Modelica Fluid Stream Connectors as a Boundary Counterexample

**Source family:** Modelica Language Specification  
**Source:**

- https://specification.modelica.org/master/stream-connectors.html

**Observed architecture:**

Modelica's ordinary across/through connector variables are explicitly documented as insufficient for numerically sound bidirectional material flow carrying specific enthalpy and composition. The language therefore introduces `stream` variables to represent convected quantities whose values depend on flow direction.

**Evidence result:**

`INTERFACE ABSTRACTION MUST EXPAND WHEN GOVERNING TRANSPORT SEMANTICS REQUIRE IT — ESTABLISHED SOFTWARE PRIOR ART`

**Taxonomy pressure:**

This is a useful falsification-oriented example. A generic interface abstraction is not preserved dogmatically when the physical transport semantics require additional information.

It supports the general principle that architecture must admit boundary/interface revision when the modeled physics exceeds the assumptions of the simpler interface.

**Limit of evidence:**

This is not a direct statement about Thermodynamic State authority or optional extensions.

---

## 5. Prior-Art Determination Table

| Candidate statement | v0.3 determination | Evidence basis | Later novelty status |
|---|---|---|---|
| Generalized conjugate field/work-pair representation across magnetic, electric, mechanical, pressure domains | **ESTABLISHED PRIOR ART** | E-EFM-17, E-EFM-18, E-EFM-24 | Excluded |
| External field/material interaction may be source, work/flux, constitutive dependence, or order/state response | **ESTABLISHED / DISTRIBUTED PRIOR ART** | v0.1-v0.3 caloric and multiphysics evidence | Excluded as generic classification |
| State space must be enlarged when current variables are insufficient for memory/history-dependent response | **ESTABLISHED PRIOR ART** | E-EFM-19 to E-EFM-23 | Excluded |
| Internal/history variables may coexist with constitutive/property relations | **ESTABLISHED PRIOR ART** | v0.2 + E-EFM-19 to E-EFM-23 | Excluded |
| Strongly coupled domains can retain separate state while exchanging energy/power/data | **ESTABLISHED PRIOR ART** | preCICE v0.1, E-EFM-25 to E-EFM-27 | Excluded |
| Energy/power ports or physically meaningful multi-domain connectors | **ESTABLISHED PRIOR ART** | E-EFM-25 to E-EFM-27 | Excluded |
| Thermodynamic state variable selection can be formulation/medium dependent | **ESTABLISHED PRIOR ART** | E-EFM-18, E-EFM-21, E-EFM-28 | Excluded |
| A minimal/sufficient state description can be judged by ability to determine present/future response | **ESTABLISHED PRIOR ART** | E-EFM-19, E-EFM-21, E-EFM-22 | Excluded as generic completeness principle |
| Exact software-framework rule tying thermodynamic state-space sufficiency to ordinary-extension authority, non-promotion, and Core-completeness preservation | **NOT FOUND IN REVIEWED BOUNDED EVIDENCE** | Cross-source comparison | Survives only as bounded Research Gap candidate |

The final row is not a novelty finding. It only identifies a combination not found explicitly in the reviewed evidence set.

---

## 6. Final Evidence-Supported Representation

The evidence does **not** support a single mutually exclusive coupling taxonomy.

The stable representation after v0.3 is an **orthogonal classification plus an authority decision**.

### Axis A — Thermodynamic interaction form

A mechanism may contribute through one or more of:

- `A1 — Source / deposition`: externally or separately computed energy appears as a volumetric/surface energy source.
- `A2 — Flux / generalized work exchange`: coupled flux or generalized work enters the thermodynamic balance or potential.
- `A3 — Constitutive / property dependence`: material response coefficients or equilibrium relations depend on an external or internal variable.
- `A4 — State-space / closure change`: the selected thermodynamic formulation requires additional thermodynamic coordinates/state or revised closure.

These tags are not mutually exclusive.

### Axis B — State role

A participating quantity may be:

- `B1 — Prescribed / configuration-like input`;
- `B2 — Algebraic / equilibrium derived variable`;
- `B3 — Mechanism-local persistent internal/history state`;
- `B4 — Cross-domain governing state` evolved by another physical responsibility; or
- `B5 — Thermodynamic governing state` required by the selected thermodynamic formulation.

A named quantity such as polarization, magnetization, strain, phase fraction, reaction progress, or electric potential cannot be permanently assigned to one role without specifying formulation and scope.

### Axis C — Coupling relation

Coupling may be:

- one-way or bidirectional;
- explicit, staggered, iterative, or implicit;
- partitioned or monolithic at the numerical implementation level.

These numerical/organizational choices do not determine semantic state ownership by themselves.

### Axis D — Thermodynamic authority impact

The architecture-level terminal decision is:

- `D0 — Core-preserving for the selected thermodynamic formulation`: after external/cross-domain state is abstracted to declared inputs/exchanges, the selected Thermodynamic State and thermodynamic governing formulation remain sufficient and semantically complete for the claimed scope.
- `D1 — Thermodynamic formulation/Core revision required`: correct thermodynamic evolution for the claimed scope requires additional authoritative thermodynamic coordinates/state, new thermodynamic work/conservation responsibilities, or revised closure that cannot honestly be represented as external exchange plus existing thermodynamic state.

`D0` does not mean the complete multiphysics system is simple or optional. A cross-domain solver may still be governing and mandatory for the larger coupled-system problem.

`D1` does not mean all external-domain state must be merged into Thermodynamic State. It means the selected thermodynamic formulation itself is no longer complete as previously defined.

---

## 7. Provisional Operational Decision Rule

The evidence supports using the following rule as a **research analysis procedure**, not as a new Framework Specification rule:

### Step 1 — Freeze the selected thermodynamic formulation and claimed scope

Identify the authoritative Thermodynamic State, governing thermodynamic balance/closure, material assumptions, and excluded physics for the formulation being evaluated.

A classification cannot be made in the abstract without this scope.

### Step 2 — Identify external/cross-domain governing quantities

Determine whether the field/material mechanism depends on prescribed values, algebraic equilibrium variables, persistent internal variables, or separately evolved physical-domain state.

### Step 3 — Abstract external-domain state to its declared thermodynamic exchanges

Represent what the thermodynamic formulation actually receives: energy source, flux, generalized work, boundary condition, constitutive input, or other declared exchange.

### Step 4 — Test thermodynamic sufficiency

Ask whether two physically distinct coupled states that have identical declared thermodynamic inputs/exchanges and identical current Thermodynamic State can nevertheless require different future thermodynamic evolution within the claimed scope.

- If **no**, the external/cross-domain state need not automatically become Thermodynamic State; classify authority impact as `D0` for that formulation.
- If **yes**, the current thermodynamic state/exchange abstraction is insufficient. Additional thermodynamic state, work/conservation responsibility, or closure is required; classify as `D1` or narrow the model scope.

### Step 5 — Check history dependence

If current observable variables do not determine the future response, introduce or expose the required internal/history variables somewhere in the physical model. Then separately determine whether those variables are mechanism-local, cross-domain governing, or thermodynamic governing state.

### Step 6 — Do not infer authority from software placement

A variable being stored in an extension object, material object, solver module, FMU, participant, or connector does not establish its physical/semantic role.

This procedure synthesizes established prior-art concepts. Its status at v0.3 is an evidence-based RQ-EFM analysis method, not a novelty claim.

---

## 8. What v0.3 Eliminates from Later Novelty Claims

The following shall not be presented as RQ-EFM-001 novelty or research contribution without substantially different evidence:

1. treating external physical fields through generalized conjugate variables/work pairs;
2. distinguishing source, flux/work, constitutive/property, and state effects in physical modeling;
3. using internal variables to represent history, hysteresis, order, or unresolved material structure;
4. enlarging state space when an existing variable set is insufficient;
5. selecting thermodynamic state variables relative to a particular formulation or medium;
6. coupling separate physical-domain states through energy/power/data interfaces;
7. using partitioned or monolithic multiphysics coupling;
8. using an energy-based interface between physical domains;
9. defining minimal/sufficient state in terms of predictive response; or
10. using a multi-axis classification of multiphysics coupling.

These exclusions materially narrow any later Research Gap Analysis.

---

## 9. Surviving Bounded Research Gap Candidate

After removing established physics and software-architecture prior art, the following narrower combination was **not found explicitly** in the bounded v0.1-v0.3 evidence set:

> **Formulation-Relative Thermodynamic Authority Boundary for Field-Driven Extensions** — a reusable thermodynamic software framework operationally preserves one authoritative Thermodynamic State and a complete Core for the selected thermodynamic formulation while allowing externally driven mechanisms to participate through source/work/constitutive exchange, mechanism-local state, or semantically separate cross-domain governing state; ordinary participation does not promote those quantities into mandatory Thermodynamic Core State, and explicit Core/formulation revision is required only when the existing thermodynamic state-space and governing closure are no longer sufficient for the claimed scope.

This is a **Research Gap candidate only**.

The candidate is deliberately narrower than:

- generalized thermodynamics;
- state-space sufficiency;
- internal-variable theory;
- energy-port modeling;
- multi-domain connector design;
- multiphysics solver coupling; and
- the already completed RQ-ISO-001 fixed semantic/Core-state boundary contribution.

The possible new question is whether these established ingredients have been explicitly operationalized together as a **field-driven mechanism classification/governance rule in a reusable thermodynamic framework**, and whether doing so produces measurable engineering consequences beyond RQ-ISO-001.

---

## 10. Relationship to RQ-ISO-001

RQ-ISO-001 already established a bounded contribution around **Fixed Semantic/Core-State Boundary under Ordinary Extension** and evaluated state-growth, Core-change, and revalidation-scope consequences for selected ordinary extensions.

RQ-EFM-001 must not simply rename that result.

The surviving RQ-EFM-001 candidate differs only if it can establish additional value in the **physical classification decision itself**, for example:

- correctly distinguishing source-only, constitutive, internal-state, cross-domain governing, and actual formulation-revision cases before implementation;
- preventing physically invalid reduction of work-coupled/caloric mechanisms to heat sources;
- preventing unnecessary promotion of separately governed cross-domain state into Thermodynamic State; and
- producing a reproducible decision procedure whose outcomes can be falsified against mechanism-specific formulations.

If later Gap Analysis shows that these are merely direct applications of RQ-ISO-001 plus established thermodynamic state-space theory, RQ-EFM-001 shall be reclassified as an application/specialization study rather than a distinct research contribution.

---

## 11. Falsification / Reclassification Conditions for the Surviving Candidate

The candidate in Section 9 shall be rejected, narrowed, or reclassified if any of the following is found.

### F-EFM-07 — Direct Framework Prior Art

A reviewed software framework, reference architecture, or formal method already explicitly combines:

- formulation-relative authoritative thermodynamic state;
- optional mechanism/cross-domain participation;
- non-promotion of external/mechanism state into mandatory thermodynamic Core state;
- a state-space/closure sufficiency criterion for Core revision; and
- an operational classification process comparable to Section 7.

If found, later work shall be framed as adoption, specialization, comparison, or evaluation.

### F-EFM-08 — Collapse into RQ-ISO-001

If the Section 7 decision procedure produces no research distinction beyond the already established RQ-ISO-001 authority boundary, RQ-EFM-001 shall not be claimed as an independent architecture contribution.

### F-EFM-09 — No Reproducible Physical Classification

If competent reviewers using the same frozen formulation and mechanism evidence cannot consistently classify `D0` versus `D1`, the procedure is too subjective and must be revised or rejected.

### F-EFM-10 — Hidden Governing-State Failure

If the procedure systematically permits required thermodynamic governing information to remain hidden as external or extension-local state, it fails its physical-boundary purpose.

### F-EFM-11 — Over-Promotion Failure

If the procedure systematically promotes separately governed cross-domain variables into Thermodynamic State even though the frozen thermodynamic formulation remains complete under declared exchanges, it fails its isolation purpose.

### F-EFM-12 — Mechanism-Name Dependence

If classification depends primarily on mechanism names rather than formulation-relative state/closure evidence, the method has failed and must be narrowed.

---

## 12. Candidate Evidence Test Families for a Later Gap Task

If a dedicated Gap Analysis is opened, it should not begin by adding more general literature. It should use a small set of mechanism formulations that force distinct decisions.

Recommended families:

1. **Externally supplied Joule/optical loss** — expected source-coupled `D0` control when field solution is outside the thermodynamic formulation.
2. **Reduced equilibrium electrocaloric or magnetocaloric formulation** — test whether generalized field dependence can remain constitutive/equilibrium without persistent added thermodynamic state.
3. **Hysteretic electrocaloric/magnetocaloric formulation with polarization/magnetization/internal order evolution** — test mechanism-local versus thermodynamic-state classification.
4. **Thermoelectric coupled transport** — test cross-domain governing electrical state with bidirectional thermal/electrical fluxes while preserving semantic state separation where formulation permits.
5. **A formulation requiring additional thermodynamic work/state coordinates** — positive `D1` boundary case.

The same mechanism may appear in more than one family if its reduced and high-fidelity formulations deliberately differ.

---

## 13. Research Gap Readiness Decision

### Decision

**GO — OPEN A DEDICATED, BOUNDED RQ-EFM-001 RESEARCH GAP ANALYSIS**

### Basis

The evidence survey is now sufficiently converged because:

- the original taxonomy has already been falsified and refined;
- generic coupling classification is excluded as novelty;
- generalized work-pair thermodynamics is established prior art;
- state-space sufficiency and internal-variable enlargement are established prior art;
- separate multi-domain state with explicit energy/power interfaces is established prior art;
- formulation-relative thermodynamic-state abstraction is established software prior art; and
- one narrow framework-level combination remains unlocated in the bounded survey.

Another broad evidence pass would likely repeat established ingredients rather than test the surviving combination.

A Gap Analysis is therefore justified, but it must begin from the exclusions in Section 8 rather than from the original RQ definition.

---

## 14. Current Research Classification

| Item | Classification after v0.3 |
|---|---|
| Source/energy-deposition coupling | Established prior art |
| Generalized field/work-pair thermodynamics | Established prior art |
| Constitutive/property coupling | Established prior art |
| Internal/history variables | Established prior art |
| Cross-domain governing coupling | Established prior art |
| Multi-domain energy/power interfaces | Established prior art |
| Formulation-relative thermodynamic state selection | Established prior art |
| Generic state-space sufficiency/completeness principle | Established prior art |
| Four-axis RQ-EFM analysis representation | Research synthesis; not a novelty claim |
| Formulation-relative authority decision `D0`/`D1` | Evidence-supported analysis procedure; novelty not established |
| Framework-level combination in Section 9 | **Research Gap candidate — bounded survey only** |
| Research Gap | Not yet established; dedicated Gap Analysis authorized by evidence readiness |
| Research Contribution | Not established |
| Novelty / priority | Not established |
| Framework Specification change | Not authorized |

---

## 15. Survey Closure Rule

The broad RQ-EFM-001 evidence survey shall stop at v0.3.

Additional sources should be added before Gap Analysis only if they are likely to satisfy the full surviving combination in Section 9 or directly falsify the D0/D1 operational distinction.

Sources that merely repeat any of the following are no longer sufficient reason to broaden the survey:

- another source-term implementation;
- another generic multiphysics coupling scheme;
- another generalized field/work pair;
- another statement that internal variables describe history;
- another partitioned versus monolithic comparison; or
- another medium with a formulation-dependent state variable set.

The next stage is a dedicated **RQ-EFM-001 Research Gap Analysis**, not Specification or implementation.

---

## 16. References Added / Emphasized in v0.3

1. Coleman, B. D.; Gurtin, M. E. *Thermodynamics with Internal State Variables*. Journal of Chemical Physics 47, 597–613 (1967). DOI: `10.1063/1.1711937`.
2. Maugin, G. A.; Muschik, W. historical/internal-variable literature and later reviews represented by: https://www.sciencedirect.com/science/article/pii/S0093641315001056
3. Internal-state-variable historical review: https://www.sciencedirect.com/science/article/pii/S0749641910000847
4. Modern constitutive state-space discussion: https://www.sciencedirect.com/science/article/pii/S0022509623000492
5. Minimal-state / materials-with-memory reference: https://arpi.unipi.it/handle/11568/150079
6. Multicaloric thermodynamics: https://pmc.ncbi.nlm.nih.gov/articles/PMC4938063/
7. Multicaloric/coupled-caloric review: https://cpb.iphy.ac.cn/article/2020/2027/cpb_29_4_047504.html
8. Generalized multicaloric theory: https://www.sciencedirect.com/science/article/pii/S0020768316302281
9. Electrocaloric thermodynamics reference: https://doi.org/10.1002/047134608X.W8244
10. Electrocaloric dynamic/equilibrium formulation review: https://www.sciencedirect.com/science/article/pii/B9780128216477000025
11. Magnetocaloric hysteresis/kinetics: https://onlinelibrary.wiley.com/doi/full/10.1002/pssb.201700278
12. Magnetocaloric internal-variable hysteresis model: https://www.sciencedirect.com/science/article/abs/pii/S0304885304015549
13. Elastocaloric review: https://link.springer.com/article/10.1007/s40830-024-00477-x
14. Bond-graph multi-domain power modeling: https://pmc.ncbi.nlm.nih.gov/articles/PMC9497721/
15. Bond-graph CFD/state-variable formulation: https://www.sciencedirect.com/science/article/abs/pii/S1569190X0600027X
16. Distributed port-Hamiltonian review: https://academic.oup.com/imamci/article/37/4/1400/5877069
17. Modelica connectors: https://specification.modelica.org/master/connectors-and-connections.html
18. Modelica stream connectors: https://specification.modelica.org/master/stream-connectors.html
19. Modelica.Media `ThermodynamicState`: https://doc.modelica.org/Modelica%204.0.0/Resources/helpWSM/Modelica/Modelica.Media.Interfaces.PartialMedium.ThermodynamicState.html
20. Modelica.Media medium usage/state construction: https://doc.modelica.org/Modelica%203.2.3/Resources/helpWSM/Modelica/Modelica.Media.UsersGuide.MediumUsage.BasicUsage.html

---

## 17. Conclusion

v0.3 substantially reduces the space of defensible RQ-EFM-001 contribution claims.

The literature already provides mature foundations for generalized field/work pairs, internal-variable state-space enlargement, predictive/minimal-state concepts, formulation-relative thermodynamic state selection, multi-domain power/energy interconnection, and physical connector abstractions.

That is beneficial to the research because the remaining question is no longer an attempt to invent a universal coupling taxonomy.

The only surviving bounded candidate is a narrower framework-governance combination: whether a reusable thermodynamic framework can operationalize formulation-relative state-space sufficiency as the decision boundary between ordinary field-driven participation and explicit Thermodynamic Core/formulation revision, while preserving the authority/non-promotion guarantees evaluated by RQ-ISO-001.

The bounded evidence survey does not establish that this combination is novel or absent globally.

It is, however, now narrow enough to justify a dedicated falsifiable Research Gap Analysis.
