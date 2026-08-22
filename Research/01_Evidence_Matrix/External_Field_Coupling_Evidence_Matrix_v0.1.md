# External Field Coupling Evidence Matrix v0.1

Status: **UNDER SURVEY — initial evidence falsification pass**  
Research Question: **RQ-EFM-001**  
Date: **2026-08-23**  
Tracking: GitHub Issue #86  
Dependency: `Research/05_Research_Questions/RQ_EFM_001_Definition_v0.1.md`

---

## 1. Objective

This matrix performs the first bounded evidence test for RQ-EFM-001 — **External Energy / Physical Field–Driven Material Response**.

The purpose is not to confirm the candidate taxonomy defined in `RQ_EFM_001_Definition_v0.1.md`. The purpose is to expose where mature multiphysics formulations treat externally driven material response as:

- a heat / energy source;
- a constitutive or material-property dependency;
- mechanism-local persistent state;
- a bidirectionally coupled cross-domain governing problem; or
- a genuine change to the thermodynamic governing formulation.

This document is non-normative. It does not modify the ThermoCore Framework Specification, production implementation, reference thermodynamic formulation, Validation, Performance, Framework Conformance, or the frozen v1.0.0 release baseline.

No Research Gap, novelty, or research-contribution claim is made here.

---

## 2. Candidate Taxonomy Entering the Survey

RQ-EFM-001 v0.1 entered the survey with four candidate classes:

| Candidate | Definition-stage meaning | Status entering v0.1 evidence |
|---|---|---|
| C-EFM-1 | Energy / source contribution | Under Survey |
| C-EFM-2 | Material-property update | Under Survey |
| C-EFM-3 | Extension-owned local persistent state | Under Survey |
| C-EFM-4 | Core / governing-formulation revision | Under Survey |

A principal falsification target is whether these four classes are sufficient.

The first evidence pass specifically tests an alternative possibility:

> a physical mechanism can require two or more governing physical-domain states and equations to co-evolve, while those states remain semantically separate and neither domain's state must automatically be reclassified as the other's Core State.

If this pattern is established, the original four-class taxonomy is incomplete and requires a distinct cross-domain governing-coupling class.

---

## 3. Evidence Questions

Each record is assessed against the following questions where the public source exposes enough information:

1. What state or dependent variables are solved or supplied?
2. Which physical responsibility evolves them?
3. Is the external field prescribed, solved separately, or co-evolved?
4. Does the thermal equation receive a source term, flux term, constitutive/property dependency, or governing-equation change?
5. Is coupling one-way or bidirectional?
6. Does the mechanism introduce a separate conservation equation or governing field equation?
7. Does the evidence require the cross-domain state to become Thermodynamic State?
8. Does code modularity correspond to state/physics separation, or is that not stated?
9. Which original candidate class is supported, challenged, or insufficient?
10. What evidence would force later reclassification?

Unknown or unexposed items are recorded as such rather than inferred.

---

## 4. Evidence Records

### E-EFM-01 — COMSOL Joule Heating / Electromagnetic Heating

**Source family:** COMSOL Multiphysics official documentation  
**Mechanism:** resistive / electromagnetic heating  
**Primary sources:**

- https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_multiphysics_interfaces.11.56.html
- https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_multiphysics_features.12.06.html
- https://www.comsol.com/support/learning-center/course/introduction-to-defining-multiphysics-models-122/viewing-and-accessing-the-equations-and-variables-for-physics-feature-nodes-30761

**Observed architecture / physics:**

- The predefined Joule Heating interface combines an `Electric Currents` physics interface with `Heat Transfer in Solids`.
- Electric current / electric potential is solved by the electrical physics interface.
- Electromagnetic power dissipation is inserted into the heat equation as a heat source.
- Electromagnetic material properties may depend on temperature.
- Therefore the coupling may be one-way when the electrical solution is unaffected by temperature, or bidirectional when electrical properties depend on temperature.

**State / variable interpretation:**

The public documentation identifies electric potential / current and temperature as dependent variables belonging to different constituent physics interfaces. It does not state that electric state becomes thermal state merely because electrical losses heat the material.

**Taxonomy pressure:**

- **Supports C-EFM-1** for the thermal-side representation of externally computed resistive loss as a source contribution.
- **Supports a constitutive-feedback pattern related to C-EFM-2**, because temperature can alter electromagnetic material properties.
- **Challenges a purely one-way interpretation of C-EFM-1**: the same Joule-heating mechanism may become bidirectionally coupled without ceasing to use a heat-source contribution on the thermal equation.

**Preliminary classification:**

`SOURCE CONTRIBUTION + OPTIONAL BIDIRECTIONAL PROPERTY FEEDBACK`

This does not establish that all Joule-heating problems are ordinary ThermoCore extensions. If electric current must be co-evolved as a governing physical domain, additional coupling semantics are required beyond a prescribed scalar heat source.

---

### E-EFM-02 — COMSOL Laser / Optical Heating

**Source family:** COMSOL Wave Optics / Heat Transfer official documentation  
**Mechanism:** optical / photothermal energy deposition  
**Primary sources:**

- https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_multiphysics_interfaces.11.59.html
- https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.092.html
- https://doc.comsol.com/6.4/doc/com.comsol.help.comsol/comsol_ref_solver.36.092.html

**Observed architecture / physics:**

- The Laser Heating interface combines an electromagnetic-wave physics interface with Heat Transfer in Solids.
- Electromagnetic losses are supplied to heat transfer as a heat source.
- One-way study modes solve the electromagnetic field first and then apply its losses to the subsequent thermal solve.
- Fully coupled modes are also supported when material electromagnetic properties depend on temperature.

**State / variable interpretation:**

Optical / electromagnetic field variables remain solved by the electromagnetic interface; temperature remains solved by the thermal interface.

**Taxonomy pressure:**

- **Strong evidence for C-EFM-1** when optical absorption / electromagnetic loss is externally resolved and supplied as thermal power deposition.
- Also demonstrates that a source-coupled mechanism can transition from one-way to bidirectional coupling through constitutive feedback without necessarily changing the thermal state definition.

**Preliminary classification:**

`ENERGY DEPOSITION SOURCE — STRONGLY SUPPORTED AS PRIOR ART PATTERN`

This is one of the cleanest source-coupling examples in the v0.1 survey.

---

### E-EFM-03 — MOOSE Joule-Heating Coupling

**Source family:** MOOSE official documentation  
**Mechanism:** Joule heating coupled between electromagnetics / electrostatics and heat transfer  
**Primary sources:**

- https://mooseframework.inl.gov/modules/electromagnetics/index.html
- https://mooseframework.inl.gov/modules/combined/examples/current_heating_of_wire.html
- https://mooseframework.inl.gov/docs/doxygen/modules/classADJouleHeatingSource.html

**Observed architecture / physics:**

- MOOSE explicitly documents a multiphysics coupling case between electromagnetics and heat transfer for wire heating.
- The Joule-heating source is calculated from electrical / electromagnetic variables and electrical conductivity.
- `ADJouleHeatingSource` supplies the thermal source term while obtaining the field information from an electromagnetic solution or an electrostatic-potential approximation.
- The source term is applied to the heat-transfer equation rather than redefining the thermal unknown as an electromagnetic quantity.

**Taxonomy pressure:**

- Independently **supports C-EFM-1** as an established multiphysics architecture pattern.
- Establishes that source coupling is not specific to one commercial solver.
- Also demonstrates that the source may depend on another solver's field variable while remaining a heat-equation source term.

**Preliminary classification:**

`CROSS-MODULE FIELD -> THERMAL SOURCE CONTRIBUTION`

The MOOSE evidence does not by itself establish the semantic ownership rules required by ThermoCore; it establishes the technical and architectural feasibility of cross-domain source coupling.

---

### E-EFM-04 — OpenFOAM Heat-Source / Energy-Equation Source Pattern

**Source family:** OpenFOAM official source-code / equation documentation  
**Mechanism:** generic externally defined thermal source  
**Primary sources:**

- https://cpp.openfoam.org/v14/classFoam_1_1fv_1_1heatSource.html
- https://doc.openfoam.com/2212/tools/processing/solvers/transport-eqns/energy-transport/

**Observed architecture / physics:**

- `heatSource` is an `fvModel` that adds a source term to the energy equation.
- The model accepts power or power per unit volume.
- The OpenFOAM energy-transport equation explicitly includes source terms alongside enthalpy/internal-energy transport, kinetic energy, pressure, diffusion, gravity, and radiation terms.
- Separate source methods exist for ordinary and compressible energy equations.

**Taxonomy pressure:**

- **Supports C-EFM-1** as mature prior art for injecting energy without making the source itself a new thermodynamic-state variable.
- Also provides a useful boundary reminder: compressible energy equations already include additional governing quantities such as density, velocity, kinetic energy, and pressure. A generic source mechanism does not eliminate the need for those governing states when the formulation requires them.

**Preliminary classification:**

`GENERIC ENERGY-SOURCE EXTENSIBILITY — ESTABLISHED PRIOR ART`

---

### E-EFM-05 — COMSOL Thermoelectric Effect

**Source family:** COMSOL Heat Transfer / Electric Currents official documentation  
**Mechanism:** Seebeck / Peltier / Thomson thermoelectric coupling  
**Primary sources:**

- https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_multiphysics_features.12.14.html
- https://doc.comsol.com/6.3/doc/com.comsol.help.heat/heat_ug_theory.07.083.html
- https://doc.comsol.com/6.4/doc/com.comsol.help.comsol/physics_builder_manual_examples.44.03.html

**Observed architecture / physics:**

- The thermoelectric interface combines Electric Currents and Heat Transfer in Solids.
- Peltier contributions alter the heat flux.
- Seebeck contributions alter current density.
- Joule heating remains present as an additional dissipative contribution.
- Heat-energy conservation and electric-current conservation are both part of the coupled formulation.
- Temperature and electric potential/current are therefore mutually coupled rather than reducible to a single externally prescribed heat-load signal.

**State / variable interpretation:**

The public architecture keeps thermal and electrical physics interfaces distinct while coupling their governing fluxes/equations.

Nothing in the cited material requires electric potential/current to be renamed or absorbed into `Thermodynamic State` simply because the coupled solution is strong.

**Taxonomy pressure:**

This is the strongest v0.1 falsification pressure against the original four-class taxonomy.

A thermoelectric problem can be:

- stronger than C-EFM-1 source-only coupling;
- not adequately described as C-EFM-2 property update;
- not merely C-EFM-3 mechanism-local memory; and
- still architected as **separate physical-domain governing equations connected through an explicit multiphysics coupling**, rather than necessarily requiring one domain to absorb the other's state.

Therefore the original C-EFM-4 wording (`Core / governing-formulation revision`) conflates at least two distinct cases:

1. **cross-domain governing coupling with separate domain-state authority**, and
2. **actual revision of the thermodynamic Core's own authoritative state/formulation**.

**Preliminary classification:**

`BIDIRECTIONAL CROSS-DOMAIN GOVERNING COUPLING`

**Result:** original four-class taxonomy is **insufficient as written**.

---

### E-EFM-06 — preCICE Partitioned Multiphysics Coupling

**Source family:** preCICE official documentation  
**Mechanism:** general partitioned equation coupling  
**Primary sources:**

- https://precice.org/docs
- https://precice.org/configuration-overview
- https://precice.org/tutorials-partitioned-heat-conduction

**Observed architecture / physics:**

- preCICE explicitly defines partitioned multiphysics as coupling existing programs / solvers that each simulate a subpart of the complete physics.
- The coupling layer exchanges selected data and provides communication, mapping, transient coupling schemes, and acceleration.
- Its partitioned heat-conduction tutorial demonstrates one participant supplying temperature and another supplying flux through an iterative coupling arrangement.

**Taxonomy pressure:**

This establishes broad prior art for the architectural pattern:

> governing physical responsibilities can remain in separate solvers while participating in one coupled simulation through explicit data exchange.

The evidence does not prove ThermoCore conformance or semantic ownership equivalence. It does, however, falsify any assumption that strong or iterative coupling necessarily requires all participating physical state to be merged into one shared Core State.

**Preliminary classification:**

`PARTITIONED GOVERNING-COUPLING PRIOR ART`

This strongly supports adding a distinct candidate class between simple source/property coupling and Thermodynamic Core revision.

---

### E-EFM-07 — Multicaloric / Magnetocaloric Material Response

**Source family:** peer-reviewed review literature  
**Mechanism:** field-driven caloric response  
**Primary sources:**

- https://www.nature.com/articles/s41578-022-00428-x
- https://www.annualreviews.org/content/journals/10.1146/annurev-matsci-062910-100356

**Observed physics:**

- Caloric effects are described as reversible changes in material temperature and entropic state under applied fields.
- Magnetocaloric literature treats magnetic entropy, phase transition behavior, and field-dependent thermodynamic response as central quantities.
- The applied field is therefore not merely an energy packet deposited irreversibly into the thermal system.

**Taxonomy pressure:**

- **Challenges any broad classification of caloric effects as C-EFM-1 heat sources.**
- Also **challenges a naive C-EFM-2 property-update interpretation**: an externally driven caloric effect may involve state-dependent entropy / order response rather than only replacing a coefficient in an unchanged thermal equation.
- Public review evidence at this stage is not sufficient to decide whether a particular bounded magnetocaloric model belongs in C-EFM-2, C-EFM-3, the new cross-domain governing class, or Core/formulation revision.

**Preliminary classification:**

`FIELD-DRIVEN THERMODYNAMIC/ORDER RESPONSE — CLASSIFICATION UNRESOLVED`

The correct architecture depends on the selected constitutive/formulation model, not merely on the mechanism name `magnetocaloric`.

---

### E-EFM-08 — Electrocaloric Thermodynamics / Polarization Response

**Source family:** peer-reviewed review / reference literature  
**Mechanism:** electrocaloric effect in dielectric / ferroelectric materials  
**Primary sources:**

- https://journal.hep.com.cn/fie/EN/10.1007/s11708-023-0884-6
- https://doi.org/10.1002/047134608X.W8244
- https://cpb.iphy.ac.cn/en/article/doi/10.1088/1674-1056/ae1f80

**Observed physics:**

- Electrocaloric response is characterized through field-induced entropy change and adiabatic temperature change.
- Phenomenological descriptions use a free energy depending on polarization and external electric field.
- In several modeling approaches, polarization / order evolution is not reducible to a constant property selected before runtime.

**Taxonomy pressure:**

- Strongly falsifies the shortcut `external electric field -> heat source` as a universal electrocaloric model.
- **C-EFM-2 is only potentially valid for reduced constitutive models where field dependence can be represented without hidden governing state.**
- When polarization / order variables evolve and determine entropy response, those quantities must be accounted for explicitly somewhere in the governing model. Whether they are extension-owned, an external field-domain state, or part of a revised thermodynamic formulation remains formulation-dependent.

**Preliminary classification:**

`FIELD-DEPENDENT FREE-ENERGY / ORDER-PARAMETER RESPONSE — NOT A GENERIC PROPERTY UPDATE`

---

## 5. Cross-Evidence Matrix

Legend:

- `YES` — explicitly supported by source.
- `NO` — source explicitly indicates otherwise for the reviewed formulation.
- `COND.` — depends on model / study configuration.
- `U` — unresolved or not exposed strongly enough for semantic classification.

| Evidence | Mechanism | Separate cross-domain variable/state solved | Thermal source term | Property / constitutive feedback | Bidirectional governing coupling | New nonthermal governing equation | Source-only classification sufficient? | Original taxonomy pressure |
|---|---|---:|---:|---:|---:|---:|---:|---|
| E-EFM-01 | Joule heating | YES | YES | COND. | COND. | YES | COND. | C1 valid but not exhaustive |
| E-EFM-02 | Laser / optical heating | YES | YES | COND. | COND. | YES | COND. | C1 strongly supported for one-way deposition |
| E-EFM-03 | MOOSE Joule heating | YES | YES | U / model-dependent | COND. | YES | COND. | Independent support for C1 |
| E-EFM-04 | OpenFOAM generic heat source | not required by heatSource itself | YES | NO for generic source object | NO for source object | NO for source object | YES for the bounded source object | C1 established prior art |
| E-EFM-05 | Thermoelectric | YES | YES, but not only source | YES | YES | YES | NO | Four-class taxonomy incomplete |
| E-EFM-06 | Partitioned multiphysics | YES | coupling-data dependent | coupling-data dependent | YES / scheme-dependent | YES in participants | NO as general architecture | Strong support for separate coupled-domain class |
| E-EFM-07 | Magneto-/multicaloric | field/order response involved | not generically | YES / thermodynamic response | formulation-dependent | formulation-dependent | NO as universal rule | C2/C3/Cross-domain boundary unresolved |
| E-EFM-08 | Electrocaloric | polarization / field response involved | not generically | YES | formulation-dependent | often yes in detailed models | NO | C2 too broad if it hides order state |

---

## 6. Finding F-EFM-01 — Energy-Source Coupling Is Established Prior Art

The first evidence pass establishes that the architectural idea of applying externally computed physical losses as a thermal energy source is mature prior art.

COMSOL, MOOSE, and OpenFOAM independently demonstrate this pattern.

Therefore RQ-EFM-001 shall not later claim novelty for:

- receiving externally computed power density;
- adding a volumetric heat source to an energy equation;
- mapping electromagnetic loss into thermal energy deposition; or
- implementing Joule / optical heating as a thermal source term when the external-domain solution is already available.

Classification:

```text
C-EFM-1 ENERGY / SOURCE CONTRIBUTION:
SUPPORTED AS AN ESTABLISHED PRIOR-ART COUPLING PATTERN
```

This does not establish that every mechanism listed in RQ-EFM-001 belongs in C-EFM-1.

---

## 7. Finding F-EFM-02 — Source Coupling and Bidirectional Feedback Are Orthogonal

COMSOL Joule and Laser Heating show that electromagnetic loss may still enter the thermal equation as a heat source even when temperature feeds back into electromagnetic material properties.

Therefore `source contribution` and `coupling direction` are not mutually exclusive taxonomy dimensions.

A mechanism may be:

```text
thermal-side coupling form = source contribution
coupling direction         = bidirectional
cross-domain state         = separately solved
```

This means the taxonomy should not force every bidirectional problem out of C-EFM-1 merely because feedback exists.

Instead, later RQ-EFM-001 work should distinguish at least:

- **what term enters the thermodynamic equation**, and
- **what governing state must be solved / exchanged to determine that term**.

These are separate classification axes.

---

## 8. Finding F-EFM-03 — The Original Four-Class Taxonomy Is Incomplete

Thermoelectric coupling and partitioned multiphysics provide direct falsification pressure against the original taxonomy.

A coupled problem may require:

- thermal governing state;
- electrical or another physical-domain governing state;
- separate conservation equations;
- bidirectional flux / constitutive coupling; and
- iterative or monolithic numerical coupling,

without logically requiring all cross-domain state to be reclassified as Thermodynamic State.

Therefore the original C-EFM-4 class conflates two distinct architectural outcomes:

1. **separate authoritative physical domains participating in a coupled governing problem**, and
2. **a revision of the thermodynamic Core's own authoritative state / governing formulation**.

The four-class taxonomy shall not survive unchanged.

---

## 9. Taxonomy Refinement Candidate v0.1 — Under Survey

The evidence supports replacing the single classification ladder with a two-axis interpretation plus a refined set of coupling classes.

### Axis A — Thermodynamic-side coupling form

A1. `Energy / Source Contribution`  
A2. `Flux / Work Contribution`  
A3. `Constitutive / Material-Property Dependency`  
A4. `Thermodynamic Governing-Formulation Change`

### Axis B — Governing-state relationship

B1. `Externally Prescribed Input`  
B2. `Extension-Owned Local State`  
B3. `Separate Cross-Domain Governing State`  
B4. `Thermodynamic Core State Revision`

For compatibility with the original RQ definition, a five-class shorthand is proposed for the next evidence revision:

| Refined candidate | Meaning | Status |
|---|---|---|
| C-EFM-1 | Energy / source contribution | Prior-art pattern established; mechanism-specific eligibility still Under Survey |
| C-EFM-2 | Constitutive / material-property coupling without hidden governing state | Under Survey |
| C-EFM-3 | Extension-owned local persistent state | Under Survey |
| **C-EFM-4R** | **Cross-domain governing coupling with semantically separate physical-domain state** | **New refinement candidate — evidence supported as an architectural pattern** |
| **C-EFM-5R** | **Thermodynamic Core / governing-formulation revision** | Under Survey boundary |

The suffix `R` indicates a research taxonomy refinement, not a normative ThermoCore concept.

---

## 10. Why C-EFM-4R Matters

C-EFM-4R is not merely `more complicated source coupling`.

The defining distinction is:

> another physical domain has authoritative governing state/equations that must participate in the coupled solution, while semantic ownership of those variables need not be transferred into Thermodynamic State.

Examples in this evidence pass include:

- electric potential / current coupled with thermal state in thermoelectric simulation;
- separate solver participants coupled through preCICE data exchange.

This distinction prevents two opposite classification errors:

### Error A — Everything becomes a heat source

This loses governing feedback when the source depends on a state that must itself be solved consistently with temperature or thermal flux.

### Error B — Every strong coupling expands Thermodynamic State

This collapses physical-domain boundaries even when a mature multiphysics architecture can keep domain states separate and couple them through explicit governing interfaces.

RQ-EFM-001 must determine when C-EFM-4R is architecturally compatible with ThermoCore's Framework Interfaces / Extension Boundary and when a mechanism instead crosses into C-EFM-5R.

That question is not answered by this v0.1 matrix.

---

## 11. Finding F-EFM-04 — Caloric Effects Cannot Be Preclassified as Property Updates

The magnetocaloric / multicaloric and electrocaloric literature directly weakens the simple hypothesis that field-driven caloric effects can generally be represented as `material property updates`.

The reviewed literature treats the field as changing entropy / temperature through magnetic, dipolar, polarization, or phase/order response.

In reduced models, some of this behavior may be represented through field-dependent constitutive functions. However, such a representation is acceptable for C-EFM-2 only if it does not hide a required runtime governing variable or history state.

Therefore:

```text
C-EFM-2 MATERIAL-PROPERTY UPDATE:
NOT YET VERIFIED AS A STABLE MECHANISM CLASS
```

It remains a valid candidate for bounded reduced formulations, but it needs stronger evidence separating:

- algebraic constitutive dependence on an externally supplied field;
- path/history dependence;
- explicit order-parameter dynamics; and
- thermodynamic-potential/state-space revision.

---

## 12. Finding F-EFM-05 — Thermoelectric Coupling Is a Strong Boundary Case, Not a Joule-Heating Variant

The Thermoelectric Effect evidence shows why mechanism names are insufficient for classification.

Joule heating alone can often appear on the thermal side as dissipated electrical power.

Thermoelectric behavior additionally couples:

- temperature gradients into electrical current through Seebeck response;
- electrical current into heat flux through Peltier response; and
- current and temperature-gradient interaction through Thomson response.

Therefore the mechanism cannot be faithfully reduced to:

```text
external field -> heat source -> thermodynamic update
```

when the electrical state is part of the governing solution.

This is the first clear RQ-EFM-001 example where classification depends on **whether the external-domain state is prescribed or co-evolved**.

---

## 13. Initial Classification After v0.1

| Item | Classification after initial evidence |
|---|---|
| Externally computed power -> thermal source | **Verified prior-art pattern** |
| Joule heating as bounded source contribution | **Supported where electrical losses are externally resolved / supplied** |
| Optical / laser heating as bounded source contribution | **Supported where optical losses are externally resolved / supplied** |
| Bidirectional source + constitutive feedback | **Verified prior-art pattern** |
| Generic material-property-update class | **Under Survey — too broad as currently stated** |
| Extension-local persistent state | **Under Survey — insufficient new mechanism evidence in v0.1** |
| Cross-domain governing coupling with separate domain states | **Evidence-supported taxonomy refinement candidate** |
| Thermodynamic Core/formulation revision boundary | **Under Survey** |
| Caloric-effect classification | **Unresolved / formulation-dependent** |
| Thermoelectric source-only classification | **Falsified for co-evolved governing formulation** |
| Four-class taxonomy unchanged | **FALSIFIED** |
| Research Gap | **Not established** |
| Novelty / contribution | **Not established** |
| Framework Specification change | **Not authorized** |

---

## 14. What the Evidence Does Not Establish

This matrix does not establish that:

- C-EFM-4R is novel;
- C-EFM-4R is already conforming to current ThermoCore Extension semantics;
- every separately solved physical domain can remain an ordinary Extension Module;
- every thermoelectric implementation requires Framework Core revision;
- every caloric mechanism requires a new order parameter;
- every field-dependent material property requires runtime state;
- partitioned coupling is superior to monolithic coupling;
- source coupling is physically accurate for arbitrary field strengths or time scales;
- the current ThermoCore implementation supports any of the reviewed external-field mechanisms; or
- the current H2O / Gallium Validation Evidence applies to these mechanisms.

---

## 15. Next Bounded Evidence Target

The next evidence revision should not broaden randomly across more heating examples. Source coupling is already sufficiently established as prior art.

The next pass should target the unresolved boundaries created by this matrix:

### Target T1 — Constitutive-only vs hidden governing state

Find formulations where an externally supplied field changes a thermal / phase constitutive relation algebraically without requiring a co-evolved order parameter or path history.

Goal: determine whether C-EFM-2 can be made a stable class with explicit admissibility criteria.

### Target T2 — Stateful field response

Inspect hysteretic magnetocaloric / electrocaloric / ferroic formulations and identify whether persistent order/history state is:

- purely mechanism-local;
- another physical-domain governing state; or
- part of the thermodynamic state-space of the selected formulation.

Goal: test C-EFM-3 against realistic field-driven statefulness.

### Target T3 — Cross-domain authority boundary

Inspect Modelica multi-domain energy coupling, preCICE coupling semantics, and one additional multiphysics framework to distinguish:

- numerical solver separation;
- information ownership;
- governing physical-domain responsibility; and
- thermodynamic state authority.

Goal: determine whether C-EFM-4R is only a generic multiphysics pattern or whether RQ-EFM-001 has a narrower ThermoCore-specific classification problem.

### Target T4 — Core-revision trigger

Identify a mechanism where correct thermodynamic evolution requires a new work term, thermodynamic potential, or persistent coordinate that cannot remain external without making the thermodynamic formulation incomplete.

Goal: define a falsifiable boundary between C-EFM-4R and C-EFM-5R.

---

## 16. Decision

The first RQ-EFM-001 evidence pass produces one immediate research correction:

```text
ORIGINAL FOUR-CLASS TAXONOMY:
DOES NOT SURVIVE UNCHANGED
```

The strongest reason is not implementation complexity. It is the existence of **bidirectionally coupled governing physical domains whose state can remain semantically separate**.

The working research taxonomy shall therefore carry C-EFM-4R (`Cross-Domain Governing Coupling`) as a distinct candidate before C-EFM-5R (`Thermodynamic Core / Governing-Formulation Revision`).

At the same time:

```text
C-EFM-1 source coupling:
ESTABLISHED PRIOR-ART PATTERN

C-EFM-2 constitutive/property coupling:
UNDER SURVEY

C-EFM-3 extension-local persistent state:
UNDER SURVEY

C-EFM-4R cross-domain governing coupling:
EVIDENCE-SUPPORTED TAXONOMY REFINEMENT CANDIDATE

C-EFM-5R Core/formulation revision:
UNDER SURVEY BOUNDARY
```

RQ-EFM-001 should proceed to a v0.2 evidence pass focused on **constitutive-only vs hidden governing state, stateful caloric response, and the C-EFM-4R / C-EFM-5R boundary**.

No Research Gap analysis or Framework Specification proposal is justified yet.
