# Conservative Energy Exchange Evidence Matrix

Version: 0.1  
Status: UNDER SURVEY — Bounded Direct-Antecedent Evidence Pass  
Research Question: RQ-CEX-001 — Conservative Cross-Boundary Energy Exchange  
Tracking Issue: #121  
Date: 2026-08-23

---

## 1. Objective

This matrix evaluates direct prior art relevant to **cross-boundary energy-exchange semantics and accounting**.

The question is not whether energy conservation, source terms, power bonds, heat fluxes, conservative mappings, or co-simulation timing already exist. Those are expected to be mature concepts.

The narrower question is whether ThermoCore requires a framework-level semantic contract for energy-bearing communication such that Thermodynamic Computation can interpret and account for a physical contribution unambiguously while:

- preserving applicable conservation meaning;
- preserving current Thermodynamic State ownership;
- preserving Thermodynamic Computation as the only Framework Core writer of Thermodynamic State;
- avoiding duplicate or omitted physical accounting;
- remaining independent of API, transport, synchronization, timestep algorithm, and backend.

This document is non-normative. It does not establish novelty, priority, superiority, Framework Specification requirements, or implementation requirements.

---

## 2. Evidence Questions

The RQ-CEX-001 definition proposed eight candidate semantic dimensions. v0.1 evaluates each independently.

| ID | Dimension | Evidence question |
|---|---|---|
| CEX-E1 | Quantity form | Does prior art distinguish energy, power/rate, flux, source density, generalized work, or other energy-equivalent quantities? |
| CEX-E2 | Measure / support basis | Is the quantity explicitly tied to system, mass, volume, area/interface, mesh sum, or integral support? |
| CEX-E3 | Temporal support | Is the communicated quantity tied to an instant, communication point, interval, average rate, or interval-integrated energy? |
| CEX-E4 | Sign / orientation | Is positive/negative direction or interface orientation explicitly defined? |
| CEX-E5 | Exchange role | Is external source/sink distinguished from internal transfer, boundary flux, or cross-domain conversion? |
| CEX-E6 | Accounting responsibility | Is there an explicit rule preventing one physical contribution from being counted twice or omitted? |
| CEX-E7 | Conservation relation | Is a zero-sum, equal-integral, power-balance, energy-residual, or other conservation relation defined? |
| CEX-E8 | Provenance / identity | Is physical contribution identity required independently of transport-level message identity? |

Evidence labels:

- **Established** — directly supported by reviewed primary/official evidence.
- **Strong Partial** — the underlying semantic dimension is explicit but does not establish the full ThermoCore-style architecture meaning.
- **Partial** — relevant but incomplete evidence.
- **Not established in reviewed evidence** — no sufficient evidence in this bounded pass; not a claim of absence.
- **Not required as a general semantic dimension** — evidence suggests the concept should not be mandatory without a mechanism-specific reason.

---

## 3. Cross-Source Summary

| Source family | E1 quantity form | E2 basis/support | E3 temporal | E4 sign/orientation | E5 role | E6 accounting responsibility | E7 conservation relation | E8 provenance | RQ-CEX pressure |
|---|---|---|---|---|---|---|---|---|---|
| Modelica flow connectors + HeatPort | **Established** | **Established at port level** | Continuous-time / not discrete-exchange focused | **Established** | **Strong Partial** | **Strong Partial / Established inside connection semantics** | **Established** | Not generally required | Very strong direct semantic antecedent |
| ECCO / power-bond co-simulation | **Established** | **Established at power-bond interface** | **Established** | **Established through bond-side power balance** | **Established for inter-simulator energy transfer** | **Strong Partial** | **Established** | Not a message-identity concept | Strongest energy-accounting / temporal antecedent |
| FMI 3.0.2 Co-Simulation | Variable semantics only | Units/types available; physical basis model-defined | **Established** | Model-defined | Generic input/output rather than energy roles | Co-simulation algorithm responsibility, not energy-accounting rule | **Not defined as energy conservation** | Interface-variable identity exists, physical contribution identity not defined | Strong temporal/causality context; not conservation antecedent |
| preCICE mapping | Data-field dependent | **Established: sum/surface/volume integral distinctions** | Coupling-scheme dependent | Field/model dependent | Generic interface data | Mapping responsibility only | **Established for sum/integral preservation according to mapping constraint** | Not physical contribution identity | Strong extensive-vs-intensive mapping antecedent |
| MOOSE heat source / external heat flux | **Established** | **Established: W/m³, W/m², integrated heat rate** | Solver/model dependent | **Established** | **Established: volumetric source vs boundary flux** | **Strong Partial** | **Established / verification-oriented** | Not generally required | Strong PDE/source/flux semantic antecedent |

---

## 4. Evidence Record CEX-01 — Modelica Flow Connectors and HeatPort

### 4.1 Primary sources

- Modelica Language Specification, Connectors and Connections:  
  https://specification.modelica.org/master/connectors-and-connections.html
- Modelica Standard Library 4.0.0, Thermal HeatTransfer Interfaces:  
  https://doc.modelica.org/Modelica%204.0.0/Resources/helpDymola/Modelica_Thermal_HeatTransfer_Interfaces.html
- Modelica Thermal HeatTransfer UsersGuide:  
  https://doc.modelica.org/Modelica%204.0.0/Resources/helpOM/Modelica.Thermal.HeatTransfer.UsersGuide.html

### 4.2 Findings

Modelica flow variables have connection semantics that generate a zero-sum equation over a connection set. This is explicit architecture/language-level balance semantics rather than an incidental numerical convention.

For thermal connections, `HeatPort` carries:

- temperature `T` in K; and
- heat flow rate `Q_flow` in W.

The Modelica thermal sign convention is explicit: positive `Q_flow` means heat flowing **into** the component.

This provides direct prior art for all of the following:

- declaring the physical quantity form as heat flow rate rather than total energy;
- declaring the quantity unit/basis at a component port;
- declaring a receiver-oriented sign convention;
- generating a conservation relation from connection semantics;
- keeping connected component internals separate while enforcing interface-level flow balance.

### 4.3 RQ-CEX significance

Modelica strongly falsifies any broad claim that a framework contribution consists merely of:

> attaching units, direction, and zero-sum conservation semantics to an energy-flow interface.

Those semantics are established prior art.

It also provides strong pressure against claiming that ownership-separated components cannot conserve an exchanged flow without sharing internal state.

### 4.4 Limitation relative to RQ-CEX

The reviewed Modelica evidence is primarily continuous-equation connection semantics. It does not by itself answer the discrete communication question of what one numeric payload means over a finite communication interval when independently stepped participants exchange energy-related information.

It also does not establish the exact ThermoCore responsibility rule that only Thermodynamic Computation may write Thermodynamic State.

**Preliminary classification:** `DIRECT SEMANTIC ANTECEDENT — VERY STRONG`.

---

## 5. Evidence Record CEX-02 — ECCO and Power-Bond Co-Simulation

### 5.1 Primary / author sources

- Sadjina et al., *Energy conservation and power bonds in co-simulations: non-iterative adaptive step size control and error estimation*, Engineering with Computers, 2017:  
  https://www.sintef.no/en/publications/publication/0198cc88d6f9-e165dba7-9e72-4d1a-b51b-d3f21422620f/
- SINTEF overview, *Energy conservation and coupling error reduction in non-iterative co-simulations*:  
  https://www.sintef.no/en/publications/publication/1713183/
- SINTEF 2025 background chapter, *Energy conservation and co-simulation: Background and challenges*:  
  https://www.sintef.no/en/publications/publication/2365962/

### 5.2 Findings

ECCO treats an energetic connection between independently simulated subsystems as a **power bond**. A power bond uses paired power variables whose product gives physical power.

For the two sides of an energetic coupling, ideal conservation requires opposite-side powers to balance. In non-iterative co-simulation, independently extrapolated coupling variables generally violate that balance.

ECCO defines a residual power from the imbalance and integrates it over a communication interval to obtain a residual energy. This provides a direct link among:

- physically meaningful coupling variables;
- power at each side of a boundary;
- communication interval;
- conservation mismatch; and
- accumulated energy error.

NEPCE is described in the same literature family as using input corrections to approximately restore energy conservation.

### 5.3 RQ-CEX significance

ECCO is a very strong direct antecedent to the idea that:

> energy-bearing communication across separated solvers must be interpreted with enough semantics to determine the power exchanged and assess the resulting energy balance over time.

It strongly pressures any claim that temporal support, power-versus-energy distinction, or cross-boundary energy residual accounting is novel.

### 5.4 Semantic versus numerical separation

ECCO also provides important evidence that **semantic energy meaning and numerical coupling error are distinct**.

The power bond gives physical meaning to the exchanged variables. The residual arises because the non-iterative numerical coupling uses approximated inputs between communication points.

Therefore:

```text
physical exchange semantics
        !=
numerical conservation achieved by a particular coupling algorithm
```

A framework can require unambiguous physical meaning without prescribing ECCO, NEPCE, iterative coupling, rollback, or adaptive timesteps.

### 5.5 Limitation relative to RQ-CEX

ECCO does not directly define ThermoCore-style state ownership, extension admissibility, or a single Framework Core writer of thermodynamic state.

Its accounting objective is error estimation/control between simulators, not a general framework governance rule for classifying external source, internal transfer, boundary flux, and cross-domain conversion under one thermodynamic state authority.

**Preliminary classification:** `DIRECT ENERGY-ACCOUNTING + TEMPORAL ANTECEDENT — VERY STRONG`.

---

## 6. Evidence Record CEX-03 — FMI 3.0.2 Co-Simulation

### 6.1 Primary source

- FMI Specification 3.0.2:  
  https://fmi-standard.org/docs/3.0.2/

### 6.2 Findings

FMI Co-Simulation defines explicit temporal communication semantics:

- communication point `t_i`;
- communication step size `h_i = t_(i+1) - t_i`;
- input variables whose values are set externally;
- output variables computed by an FMU and exposed externally;
- `fmi3DoStep` using a current communication point and requested communication step size;
- optional intermediate updates and early return.

The co-simulation algorithm controls data exchange and synchronization. FMI explicitly states that the co-simulation algorithm itself is outside the FMI standard.

### 6.3 RQ-CEX significance

FMI provides strong prior art for separating:

- interface data meaning;
- input/output causality;
- communication points and intervals; and
- the external numerical co-simulation algorithm.

This strongly supports the RQ-CEX distinction that an architecture-level semantic contract may declare **what a communicated value means in time** without prescribing the algorithm that performs synchronization or integration.

### 6.4 Important negative finding

FMI does **not** by itself define a generic energy-conservation contract for arbitrary FMU variables.

A variable may carry physical units and causality, but FMI does not infer from that whether the variable is:

- external energy injection;
- internal redistribution;
- heat flux;
- generalized work;
- or one side of a conservative power bond.

Therefore FMI is **not** a complete antecedent to RQ-CEX accounting semantics.

**Preliminary classification:** `TEMPORAL / CAUSALITY ANTECEDENT — STRONG PARTIAL`.

---

## 7. Evidence Record CEX-04 — preCICE Mapping Constraints

### 7.1 Primary sources

- preCICE Mapping configuration:  
  https://precice.org/configuration-mapping
- preCICE XML configuration reference:  
  https://precice.org/configuration-xml-reference
- preCICE Mapping class reference:  
  https://precice.org/doxygen/main/classprecice_1_1mapping_1_1Mapping.html

### 7.2 Findings

preCICE explicitly distinguishes mapping constraints by the semantic nature of exchanged quantities:

- **consistent** mapping retains mean/continuous-field behavior;
- **conservative** mapping retains the sum of mapped values;
- **scaled-consistent-surface** first maps consistently and then rescales so surface integrals match;
- **scaled-consistent-volume** analogously preserves volume integrals.

The documentation distinguishes examples such as:

- displacement / temperature style continuous fields;
- force-like quantities where sum preservation matters;
- pressure / heat-flux-like intensive fields where integral preservation may matter.

### 7.3 RQ-CEX significance

This is strong prior art for the distinction:

> **conservation of an extensive transferred quantity is not the same operation as consistency of an intensive field.**

It also shows that the support measure — nodal sum, surface integral, or volume integral — is part of conservation meaning.

### 7.4 Important negative finding

A conservative or scaled-consistent spatial mapping is **not a complete thermodynamic accounting contract**.

It does not by itself determine:

- whether the quantity represents a source or an internal transfer;
- whether the same contribution has already been applied elsewhere;
- whether a power value applies instantaneously or over a communication interval;
- whether a heat flux is receiver-positive or source-positive;
- whether governing state or closure is missing.

Therefore:

```text
conservative mapping
        !=
complete physical energy accounting
```

**Preliminary classification:** `MEASURE / EXTENSIVE-CONSERVATION ANTECEDENT — STRONG`.

---

## 8. Evidence Record CEX-05 — MOOSE Heat Source and External Heat Flux

### 8.1 Primary sources

- MOOSE `HeatSource`:  
  https://mooseframework.inl.gov/docs/PRs/33142/site/source/kernels/HeatSource.html
- MOOSE `INSADEnergySource`:  
  https://mooseframework.inl.gov/source/kernels/INSADEnergySource.html
- MOOSE HeatConductionCG:  
  https://mooseframework.inl.gov/syntax/Physics/HeatConduction/FiniteElement/index.html
- MOOSE `HSBoundaryExternalAppHeatFlux`:  
  https://mooseframework.inl.gov/moose/source/components/HSBoundaryExternalAppHeatFlux.html
- MOOSE `HSBoundaryHeatFlux`:  
  https://mooseframework.inl.gov/docs/site/source/components/HSBoundaryHeatFlux.html

### 8.2 Findings — volumetric source

MOOSE documents a volumetric `HeatSource` with example units `W/m^3`.

`INSADEnergySource` explicitly defines positive source values as sources and negative values as sinks.

This directly establishes prior art for:

- source-density quantity form;
- volumetric support basis;
- source/sink sign semantics; and
- insertion into an energy equation as a source term.

### 8.3 Findings — boundary flux transferred from another application

`HSBoundaryExternalAppHeatFlux` is particularly relevant because it receives a heat-flux variable transferred from another application and applies it as a Neumann boundary condition.

The documentation includes:

- a transferred heat-flux quantity;
- an explicit `heat_flux_is_inward` convention;
- positive-value meaning relative to the heat structure boundary;
- discrete-perimeter normalization used to achieve energy conservation;
- an integrated heat-rate postprocessor recommended for verifying conservation.

This is a strong direct antecedent to several RQ-CEX candidate semantic dimensions appearing together in one engineering construct.

### 8.4 RQ-CEX significance

MOOSE strongly falsifies broad claims that it is novel to require:

- source density versus boundary heat flux distinction;
- explicit spatial basis;
- explicit inward/outward sign convention;
- integration of flux to heat rate; or
- conservation checking of a transferred heat flux.

### 8.5 Limitation relative to RQ-CEX

The reviewed MOOSE artifacts are implementation/module-specific and do not establish a general architecture-wide energy-contribution taxonomy tied to ThermoCore-style state ownership and extension admissibility.

**Preliminary classification:** `SOURCE / FLUX / BASIS / SIGN ANTECEDENT — VERY STRONG`.

---

## 9. Semantic Dimensions After v0.1

### 9.1 Quantity form

**Status: ESTABLISHED PRIOR ART.**

Reviewed evidence explicitly distinguishes:

- heat flow rate / power `[W]`;
- volumetric heat source `[W/m^3]`;
- boundary heat flux `[W/m^2]`;
- power from paired coupling variables;
- integrated energy residual over a communication interval.

No RQ-CEX contribution may be based merely on distinguishing these forms.

### 9.2 Measure / support basis

**Status: ESTABLISHED PRIOR ART.**

Reviewed evidence distinguishes:

- component-port heat flow;
- volume source density;
- area heat flux;
- nodal sum conservation;
- surface-integral conservation;
- volume-integral conservation.

Basis/support is physically necessary metadata for many exchange quantities, but the idea is not novel.

### 9.3 Temporal support

**Status: ESTABLISHED PRIOR ART AS A REQUIRED COUPLING CONCEPT.**

FMI establishes communication points and communication intervals; ECCO explicitly integrates power residual over the interval to obtain energy residual.

The important RQ-CEX consequence is not novelty but a boundary:

> ThermoCore may need enough temporal semantics to distinguish an instantaneous rate from an interval-integrated contribution, while the integration/coupling algorithm remains outside Framework semantics.

### 9.4 Sign / orientation

**Status: ESTABLISHED PRIOR ART.**

Modelica HeatPort and MOOSE external heat-flux interfaces both define explicit directional conventions.

### 9.5 Exchange role

**Status: PARTIALLY ESTABLISHED; COMPOSITE TAXONOMY UNDER SURVEY.**

Reviewed systems clearly distinguish important cases:

- prescribed source/sink;
- connection flow;
- transferred boundary flux;
- inter-simulator energetic transfer.

However, no reviewed source in this pass was found to define the exact ThermoCore candidate role set:

```text
external source/sink
internal redistribution
cross-domain conversion
boundary/interface flux
```

as a framework-level accounting taxonomy under a single thermodynamic state authority.

This is not evidence of novelty; it is only a surviving classification question.

### 9.6 Accounting responsibility

**Status: STRONG PRIOR ART FOR BALANCE; THERMOCORE-SPECIFIC SINGLE-AUTHORITY QUESTION SURVIVES.**

Modelica automatically generates zero-sum flow equations for a connection set.

ECCO evaluates both sides of an energetic coupling and measures power/energy residual when the two sides do not balance.

MOOSE provides an integrated heat-rate quantity to verify conservation of a transferred heat flux.

These are strong direct antecedents.

What is not yet established in this bounded pass is a substantially equivalent general rule of the form:

> one physical energy contribution crossing into the thermodynamic domain shall have one unambiguous thermodynamic accounting responsibility, while the producer may remain the owner of its external/mechanism state and only Thermodynamic Computation writes Thermodynamic State.

This surviving statement may still collapse into a composition of existing balance semantics plus ThermoCore's already-established ownership rules. It therefore requires direct falsification in v0.2.

### 9.7 Conservation relation

**Status: ESTABLISHED PRIOR ART.**

Examples include:

- zero-sum connection flows;
- opposite-side power balance;
- residual power / residual energy;
- conserved mapped sums;
- preserved surface or volume integrals;
- integrated heat rate for boundary-flux conservation checks.

### 9.8 Provenance / identity

**Status: NOT REQUIRED AS A GENERAL PHYSICAL SEMANTIC DIMENSION.**

None of the strongest reviewed physical formalisms requires a transport-style UUID to define conservation.

A physical model may need to distinguish separate contributions when multiple mechanisms act simultaneously, but this is different from packet/message identity.

Accordingly, RQ-CEX shall not make transport-level identity a Framework requirement.

---

## 10. Major Findings

### F-CEX-v0.1-01 — Most candidate semantic dimensions are established prior art

Quantity type, basis, time support, sign/orientation, and conservation relation are not defensible research contributions by themselves.

### F-CEX-v0.1-02 — Modelica is the strongest continuous semantic antecedent

Modelica demonstrates that a physical interface can carry explicit flow semantics, direction, and a generated zero-sum conservation equation without merging component internals.

### F-CEX-v0.1-03 — ECCO is the strongest discrete temporal/accounting antecedent

ECCO demonstrates that independently simulated participants can retain separated internals while power-bond semantics make cross-boundary energy exchange and residual energy measurable using coupling data.

### F-CEX-v0.1-04 — preCICE falsifies any collapse of conservation into one mapping concept

Conservative, consistent, and scaled-consistent mappings preserve different mathematical properties. Intensive field consistency and extensive/integral conservation are not interchangeable.

### F-CEX-v0.1-05 — MOOSE demonstrates that quantity/basis/sign/integral semantics already coexist in transferred heat-flux engineering

Transferred heat flux can carry inward/outward meaning, normalization data, and an integrated heat-rate conservation check.

### F-CEX-v0.1-06 — Semantic and numerical conservation can be separated, but not completely disconnected

A framework-level contract can declare:

- what quantity is communicated;
- what support/basis it uses;
- what its sign/orientation means;
- what role it plays in the modeled energy balance;
- what temporal interpretation applies.

It need not prescribe:

- integration method;
- interpolation/extrapolation;
- mapping algorithm;
- coupling iteration;
- rollback;
- scheduler;
- timestep controller.

However, whether numerical conservation is actually achieved remains dependent on those numerical choices.

### F-CEX-v0.1-07 — `exactly once` must remain a physical accounting concept, not a transport guarantee

The relevant architecture question is whether one physical contribution has one thermodynamic accounting responsibility.

The Framework should not infer or prescribe reliable-message-delivery semantics, transaction IDs, queues, or packet de-duplication from this phrase.

---

## 11. Prior-Art Exclusions

After v0.1, the following shall **not** be presented as RQ-CEX novelty:

1. Defining a heat-flow-rate interface.
2. Declaring a positive-inward sign convention.
3. Requiring connected physical flows to sum to zero.
4. Representing a power bond using paired variables whose product is power.
5. Integrating power mismatch over a communication interval to obtain energy residual.
6. Distinguishing communication points and communication step sizes.
7. Distinguishing consistent from conservative data mapping.
8. Preserving mapped sums or interface integrals.
9. Distinguishing volumetric heat source from boundary heat flux.
10. Expressing source density in `W/m^3` or heat flux in `W/m^2`.
11. Integrating heat flux over a boundary to obtain heat rate.
12. Checking transferred heat-flux conservation using an integrated quantity.
13. Keeping coupled subsystems internally separate while exchanging energetic variables.

---

## 12. Surviving Candidate Distinction

The original broad RQ must be narrowed.

A possible surviving distinction is:

> **Ownership-Preserving Single-Authority Energy Accounting Boundary** — within a framework where external/mechanism/cross-domain state remains owned outside Thermodynamic State and only Thermodynamic Computation may write Thermodynamic State, determine whether every admitted energy-bearing exchange requires one explicit thermodynamic accounting role and conservation target such that the same physical contribution cannot be semantically interpreted simultaneously as both an already-accounted internal transfer and a new external source.

This candidate is narrower than:

- generic energy conservation;
- power bonds;
- source-term semantics;
- heat-flux sign conventions;
- conservative mapping;
- co-simulation timing;
- state ownership alone.

### Important caution

The surviving distinction may still be only a **composition** of:

- established Modelica/bond-graph balance semantics;
- established ECCO power/energy accounting;
- established source/flux conventions;
- current ThermoCore RQ-ISO ownership rules; and
- current RQ-EFM admissibility rules.

If so, RQ-CEX-001 should be downgraded to an engineering/conformance property rather than preserved as an independent research contribution.

---

## 13. Candidate Minimal Semantic Tuple — Under Survey

v0.1 evidence supports using the following as a research-analysis tuple, not as a normative contract:

```text
EnergyExchangeSemantic = {
    quantity_form,
    measure_or_support_basis,
    temporal_support,
    sign_or_orientation,
    exchange_role,
    accounting_responsibility,
    conservation_target
}
```

`provenance_or_identity` is mechanism-specific and is not retained as a universally required element.

The tuple is a synthesis device only. Each of its major elements has prior art. The tuple itself is not claimed as novel.

---

## 14. Scenario Reassessment

### CEX-S0 — External energy injection

Retain as a control. Prior art is strong. Expected research value is low except for testing duplicate-accounting semantics.

### CEX-S1 — Internal pairwise transfer

Retain and prioritize. This is the cleanest way to test whether an extension can supply transfer contributions while Thermodynamic Computation remains the exclusive Thermodynamic State writer and total energy of the closed pair remains unchanged.

### CEX-S2 — Cross-domain dissipative conversion

Retain. Must distinguish energy newly entering the thermodynamic accounting domain from energy that is already represented in the modeled thermodynamic total.

### CEX-S3 — Interface heat flux / nonmatching discretization

Retain as a mapping stress case, but do not mistake conservative mapping for complete accounting semantics.

### CEX-S4 — Temporal-semantics stress case

Retain and prioritize. Same numeric value interpreted as `100 W` versus `100 J over the interval` is a direct semantic ambiguity even before numerical integration choices are considered.

### CEX-S5 — Formulation-boundary counterexample

Retain as a routing control to RQ-EFM-001. It shall not count as a failure of energy-exchange semantics when the real problem is an incomplete governing thermodynamic formulation.

---

## 15. Research Gap Readiness

**Research Gap Analysis readiness: NO-GO.**

Reason:

The first direct-antecedent pass shows that almost every proposed semantic dimension has strong prior art, and several sources combine multiple dimensions. The only surviving candidate is the narrow ThermoCore-specific relationship among:

- ownership-separated external/mechanism state;
- one Thermodynamic State writer;
- explicit exchange role;
- one thermodynamic accounting responsibility; and
- conservation meaning independent of transport implementation.

This remaining candidate has not yet been sufficiently pressure-tested against:

- bond-graph junction/accounting semantics;
- port-Hamiltonian / power-port interface formulations;
- Modelica stream/transported-property balance semantics;
- co-simulation interface-energy accounting beyond ECCO;
- existing conservation-accounting contracts in partitioned multiphysics.

Proceeding directly to Research Gap Analysis would therefore be premature.

---

## 16. Required v0.2 Direct-Antecedent Stress Test

v0.2 should be narrow and falsification-oriented.

### T1 — Single accounting authority antecedents

Search for formalisms that explicitly assign one side / one connection object / one balance equation responsibility for physical flow accounting while preserving component state separation.

Targets:

- bond-graph junction semantics;
- port-Hamiltonian power ports;
- Modelica stream and transported-energy connection semantics.

### T2 — Source versus transfer versus conversion classification

Determine whether prior frameworks explicitly distinguish:

- energy entering modeled-system scope;
- energy redistributed internally;
- energy converted from another modeled physical domain;
- boundary exchange with an external environment.

If mature antecedents already provide an equivalent classification, narrow RQ-CEX further.

### T3 — Temporal semantic minimum

Test whether time support can remain a semantic declaration without specifying numerical integration.

A candidate criterion is:

> the receiver must be able to determine the energy contribution over the declared accounting interval from the communicated semantics, but the Framework need not prescribe how the producer/consumer numerically approximates that quantity.

This criterion must be compared against established co-simulation theory rather than assumed novel.

### T4 — Duplicate-accounting counterexamples

Search for documented multi-domain/source coupling failures or double-counting rules where the same physical dissipation/transfer can appear as both an internal energy change and an externally re-applied source.

This is the highest-value surviving ThermoCore question.

### T5 — Reclassification test

Explicitly evaluate F-CEX-6:

> If the surviving rule is only established conservation-interface semantics composed with RQ-EFM-001 and RQ-ISO-001, close RQ-CEX-001 as an independent research line and retain the rule as engineering/conformance guidance.

---

## 17. Bounded Disposition

Current v0.1 disposition:

- Generic energy conservation: **ESTABLISHED PRIOR ART**.
- Power-bond energy accounting: **ESTABLISHED PRIOR ART**.
- Quantity/basis/sign semantics: **ESTABLISHED PRIOR ART**.
- Communication-time semantics: **ESTABLISHED PRIOR ART**.
- Conservative/integral mapping semantics: **ESTABLISHED PRIOR ART**.
- Source-density / boundary-flux semantics: **ESTABLISHED PRIOR ART**.
- Semantic-versus-numerical conservation distinction: **SUPPORTED AS A NECESSARY ANALYTICAL DISTINCTION; NOT NOVELTY**.
- Ownership-preserving single-authority thermodynamic accounting candidate: **UNDER SURVEY**.
- Independent Research Gap: **NOT YET ESTABLISHED**.
- Novelty / priority: **NOT ESTABLISHED**.
- Framework Specification change: **NOT AUTHORIZED**.
- Recommended next step: **v0.2 direct-antecedent stress test**.

---

## 18. Guardrails

The following remain in force:

- no Framework Specification change from this evidence pass;
- no production implementation change;
- no Verification, Validation, or Performance change;
- no claim that reviewed systems are direct ThermoCore equivalents;
- no claim that conservative mesh mapping implies thermodynamic conservation;
- no claim that semantic sufficiency guarantees numerical conservation;
- no transport-level exactly-once requirement;
- no API/message/timestep/synchronization prescription;
- RQ-EFM-001 remains authoritative for governing-formulation incompleteness;
- RQ-ISO-001 remains authoritative for state authority and non-promotion after information categories are accepted;
- negative/reclassification outcome remains valid.
