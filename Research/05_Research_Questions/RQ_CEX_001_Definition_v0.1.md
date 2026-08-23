# RQ-CEX-001 — Conservative Cross-Boundary Energy Exchange

Version: 0.1  
Status: UNDER SURVEY — Research Question Definition  
Tracking Issue: #119  
Scope: Thermodynamic-framework architecture / exchange semantics  
Normative Effect: None

---

## 1. Purpose

This document defines **RQ-CEX-001 — Conservative Cross-Boundary Energy Exchange**.

The research asks what semantic information and conservation constraints are minimally required when an energy-related contribution crosses a ThermoCore architectural boundary so that Thermodynamic Computation can interpret and account for that contribution unambiguously while preserving applicable energy conservation and the existing ownership model.

The question is deliberately narrower than generic energy conservation, multiphysics coupling, conservative numerical methods, co-simulation accuracy, or interface design.

This document does not establish a Research Gap, novelty, priority, Framework requirement, implementation requirement, or Validation result.

---

## 2. Background

ThermoCore currently establishes that:

1. Energy Input is supplied to and consumed by Thermodynamic Computation.
2. Thermodynamic Computation exclusively evolves and writes Thermodynamic State.
3. Framework Interfaces communicate information without acquiring or transferring ownership.
4. Extension-owned and cross-domain information may remain outside Thermodynamic State when the applicable thermodynamic formulation remains complete.
5. Permitted Framework Interface communication may be refined or enriched when additional semantically distinct information is required.
6. Framework semantics intentionally do not prescribe execution scheduling, synchronization, API signatures, protocols, message formats, storage structures, or backend behavior.

The existing specification therefore answers **who owns state**, **who may evolve Thermodynamic State**, and **when external/cross-domain information may remain outside the Core**.

It does not yet determine whether every energy-bearing communication is semantically equivalent.

For example, the numeric value `10` may represent:

- 10 J of interval-integrated energy;
- 10 W of instantaneous power;
- 10 W/m² of interface heat flux;
- 10 W/m³ of volumetric source density;
- 10 J/kg of specific energy;
- generalized work already integrated over a path or interval; or
- a value whose sign depends on an interface orientation convention.

Those meanings are physically different even when the numeric payload is identical.

A second distinction concerns **what kind of energy event is represented**. Energy supplied from outside the modeled system changes the system total. Energy transferred between two locations inside a closed modeled system should redistribute energy without changing the closed-system total. Cross-domain conversion may require equal and opposite accounting across two physical-domain responsibilities. A boundary flux may require an orientation and an integration measure.

RQ-CEX-001 investigates whether these distinctions require a stable framework-level semantic contract and, if so, what the minimal contract is.

---

## 3. Relationship to Completed Research

### 3.1 RQ-EFM-001

RQ-EFM-001 asks whether a mechanism/formulation may participate without changing authoritative Thermodynamic State semantics or the governing thermodynamic formulation.

RQ-CEX-001 begins **after** a candidate exchange is considered admissible in principle and asks whether the exchange carries enough semantics to be conservatively and unambiguously accounted for.

If an exchange cannot remain correct without introducing additional governing thermodynamic state, changing thermodynamic closure, or revising governing thermodynamic responsibility, the case is not an RQ-CEX exchange-semantics problem. It shall be routed back to the RQ-EFM-001 boundary or to explicit specification/Core revision.

### 3.2 RQ-ISO-001

RQ-ISO-001 governs authority and non-promotion after information categories are accepted.

RQ-CEX-001 shall not solve an accounting problem by moving extension-local or cross-domain governing state into Thermodynamic State solely for convenience.

### 3.3 RQ-FCI-001

RQ-FCI-001 was closed as an independent Research Gap and reclassified as an engineering/conformance property.

RQ-CEX-001 shall not rely on formulation substitution itself as a contribution claim. The research concerns the semantics of energy-related communication across architectural boundaries.

---

## 4. Primary Research Question

> **Within a bounded ThermoCore thermodynamic scope, what semantic contract is minimally sufficient for an energy-related contribution crossing a Framework Interface to be interpreted and accounted for unambiguously by Thermodynamic Computation while preserving applicable energy conservation and existing ownership boundaries, and under what conditions is semantic enrichment of the exchange sufficient versus requiring stronger coupling, formulation revision, or explicit Core/specification revision?**

---

## 5. Bounded Scope

RQ-CEX-001 is restricted to:

- energy-related communication relevant to thermodynamic evolution;
- communication across declared ThermoCore architectural boundaries;
- preservation of state ownership and State Evolution authority;
- conservation/accounting semantics that can plausibly remain independent of implementation mechanism;
- bounded thermodynamic source, transfer, conversion, work, and flux cases.

The research does not attempt to define:

- a universal multiphysics coupling framework;
- a universal co-simulation algorithm;
- a numerical discretization scheme;
- a synchronization or timestep policy;
- a message transport protocol;
- a transaction system;
- distributed exactly-once delivery;
- a universal energy-variable ontology for all physics;
- or a replacement for existing power-bond, bond-graph, FMI, preCICE, OpenFOAM, MOOSE, or other established coupling concepts.

---

## 6. Candidate Semantic Dimensions — Under Survey

The following dimensions are research candidates. They are not Framework requirements.

### CEX-D1 — Quantity Form

The exchange shall be investigated according to what physical quantity it represents, for example:

- total energy;
- specific energy;
- volumetric energy;
- power / energy rate;
- heat flux;
- volumetric source density;
- generalized work or work rate;
- another energy-equivalent quantity with an explicit physical definition.

The research shall test whether a generic label such as `Energy Input` is sufficient without a more specific declared quantity form.

### CEX-D2 — Measure / Basis

The same quantity form may require a declared basis or support, such as:

- total system;
- cell / discrete element;
- unit mass;
- unit volume;
- unit area / interface;
- another declared integration measure.

The research shall distinguish a physical measure/basis from an implementation data layout.

### CEX-D3 — Temporal Support

The research shall distinguish at least:

- instantaneous value at a declared time;
- interval-integrated energy contribution;
- average rate over a declared interval;
- another explicitly defined temporal support.

A semantic temporal support is not the same as prescribing timestep size, scheduling, interpolation, extrapolation, or synchronization algorithms.

### CEX-D4 — Sign / Orientation

The research shall determine when an exchange requires an explicit convention such as:

- positive into Thermodynamic Computation / receiving thermodynamic control volume;
- positive out of a source domain;
- positive relative to a declared interface normal;
- pairwise antisymmetric transfer convention.

The objective is unambiguous accounting, not one universal sign convention.

### CEX-D5 — Exchange Role

Candidate roles include:

- external source or sink;
- internal redistribution / transfer;
- cross-domain conversion;
- boundary/interface flux;
- generalized thermodynamic work contribution.

The research shall test whether role is independent from quantity form.

### CEX-D6 — Accounting Responsibility

RQ-CEX-001 shall investigate whether one physical contribution requires one unambiguous thermodynamic accounting responsibility.

This is **not** a transport-level exactly-once delivery guarantee and shall not require message identifiers, transactional queues, or protocol semantics.

The question is architectural: which responsibility is authoritative for applying a physical contribution to Thermodynamic State, and how is duplicate application or omission made semantically detectable or avoidable?

Thermodynamic Computation remains the only Framework Core responsibility permitted to write Thermodynamic State.

### CEX-D7 — Conservation Relation

The applicable conservation statement may differ by role and scope, including:

- external source: system-energy change equals supplied contribution plus other declared exchanges;
- closed internal transfer: algebraic sum over the closed transfer set equals zero;
- pairwise transfer: equal-and-opposite extensive transfer for the pair where the formulation justifies pairwise accounting;
- cross-domain conversion: energy decrease in one declared domain corresponds to energy increase, storage, or declared dissipation in another according to the governing model;
- interface flux: integrated transfer across the interface is consistent with the declared orientation and measure.

The research shall not assume that every valid conservation test is pairwise or local.

### CEX-D8 — Provenance / Contribution Distinction

The research may require enough provenance to distinguish physically separate contributions or to identify whether two representations refer to the same physical transfer.

Any such requirement shall remain conceptual and shall not prescribe UUIDs, packet IDs, database keys, queues, or transport protocols.

---

## 7. Semantic Conservation versus Numerical Conservation

RQ-CEX-001 shall explicitly separate two levels.

### 7.1 Semantic Conservation Sufficiency

The communicated information has enough physical meaning to state what should be conserved and how the contribution should be accounted for.

Examples include knowing that a value is an interval-integrated energy transfer into a receiving control volume, or knowing that an interface value is an outward heat flux with a declared area measure and orientation.

### 7.2 Numerical / Discretization Conservation

A numerical method actually satisfies the conservation statement within its declared numerical properties, tolerances, and discretization.

This may depend on:

- mesh mapping;
- time integration;
- interpolation or extrapolation;
- iterative coupling;
- solver tolerances;
- discrete balance equations;
- parallel reduction behavior;
- floating-point behavior.

RQ-CEX-001 shall not convert numerical-method requirements into Framework semantics unless evidence shows that a stable semantic requirement is necessary at the architectural boundary.

---

## 8. Intensive Consistency versus Extensive Conservation

RQ-CEX-001 shall distinguish:

- mapping or communicating an **intensive** quantity such as temperature, pressure, or normalized field value consistently; and
- conserving an **extensive** quantity or integral such as total force, total energy transfer, mass flow, or interface-integrated contribution.

A mapping operation described as `conservative` does not by itself establish complete thermodynamic conservation of the coupled simulation.

Likewise, a consistent intensive mapping does not imply conservation of its area- or volume-integrated effect unless the mapping semantics and governing equations justify that inference.

---

## 9. Candidate Scenario Families — Under Survey

These scenarios define future evidence/evaluation families only. They are not pre-registered experiments.

### CEX-S0 — External Energy Injection Control

A one-way external provider supplies an energy-related contribution to Thermodynamic Computation.

Purpose:

- establish the simplest source accounting control;
- distinguish interval energy from rate/power;
- test whether an external source requires a conservation partner inside the modeled system.

### CEX-S1 — Internal Pairwise Transfer

Two thermodynamic locations exchange energy within a closed modeled pair or bounded set.

Representative mechanism:

- conduction-style redistribution.

Purpose:

- test zero-net closed-system transfer;
- test whether an extension can compute a transfer contribution while Thermodynamic Computation remains the sole writer of both receiving states;
- detect double accounting when both sides independently submit the same physical transfer.

### CEX-S2 — Cross-Domain Dissipative Conversion

Another physical-domain responsibility resolves a quantity such as dissipative mechanical or electrical work and supplies a thermal contribution.

Purpose:

- test domain-state separation;
- test whether the thermal contribution must carry enough meaning to relate domain loss and thermal gain;
- distinguish externally resolved Joule/mechanical dissipation from hidden governing thermodynamic state.

### CEX-S3 — Interface Heat Flux / Nonmatching Discretization

An interface transfer is represented on different spatial discretizations.

Purpose:

- separate interface-flux semantics from mesh-mapping implementation;
- distinguish extensive conservation from intensive consistency;
- pressure-test whether framework semantics can stop before numerical mapping rules.

### CEX-S4 — Temporal-Semantics Stress Case

Two exchanges contain the same numeric value but have different temporal meanings, for example:

```text
10 W instantaneous power
10 J interval energy
10 W average power over 0.5 s
```

Purpose:

- determine whether temporal support is necessary to recover a unique physical contribution;
- distinguish semantic time support from prescribed scheduling.

### CEX-S5 — Formulation-Boundary Counterexample

A proposed `exchange` appears syntactically representable, but the governing physical model requires additional authoritative thermodynamic state, work terms, closure, or conservation responsibility.

Purpose:

- verify that exchange enrichment is not used to hide a formulation/Core revision;
- route the case to RQ-EFM-001 or explicit Framework/specification revision.

---

## 10. Initial Prior-Art Search Plan

The first Evidence Matrix shall prioritize direct antecedents rather than generic examples of heat sources.

### T-CEX-1 — Power-Bond / Bond-Graph Semantics

Investigate:

- effort-flow power pairs;
- sign conventions and power direction;
- energy storage, dissipation, supply, and conversion;
- whether power-port semantics already provide a direct antecedent to the candidate semantic contract.

### T-CEX-2 — Energy-Conserving Co-Simulation

Investigate ECCO, NEPCE, and related work for:

- power/energy residuals;
- energy exchanged between independently solved simulators;
- coupling error as conservation residual;
- temporal integration of exchanged power;
- limits between semantic energy balance and coupling algorithm.

### T-CEX-3 — FMI

Investigate official FMI Co-Simulation semantics for:

- variable causality;
- units and quantity metadata;
- communication points and step intervals;
- whether FMI itself guarantees, exposes, or deliberately leaves energy conservation to models/importers.

### T-CEX-4 — preCICE

Investigate:

- conservative mappings;
- consistent mappings;
- scaled-consistent surface/volume mappings;
- interface heat-transfer coupling;
- distinction between conserving mapped sums/integrals and full governing-system conservation.

### T-CEX-5 — OpenFOAM / MOOSE / Similar PDE Frameworks

Investigate:

- energy source terms;
- boundary heat fluxes;
- work terms;
- internal conservative flux/divergence formulations;
- responsibility for applying source/flux contributions to the governing energy equation.

### T-CEX-6 — Direct Architecture / Contract Antecedents

Search specifically for frameworks or standards that formalize all or most of the following together:

- physical quantity form;
- measure/basis;
- temporal support;
- sign/orientation;
- source versus transfer versus conversion role;
- unique accounting responsibility;
- conservation relation;
- state-ownership separation.

If such an antecedent is found in substantially equivalent form, the RQ shall be narrowed or closed.

---

## 11. Initial Falsification and Reclassification Conditions

### F-CEX-1 — Direct Semantic-Contract Antecedent

If reviewed prior art already provides a substantially equivalent architecture-level semantic contract for energy source/transfer/conversion accounting under separated state ownership, the independent RQ candidate shall be narrowed or closed.

### F-CEX-2 — Pure Implementation Detail

If quantity form, measure, sign, role, temporal support, and accounting responsibility are shown to have no stable architecture-level meaning and are necessarily implementation-specific, RQ-CEX-001 shall be reclassified as implementation/Verification work.

### F-CEX-3 — Numerical Coupling Dominance

If conservation can be defined meaningfully only after prescribing a numerical coupling algorithm, the research shall separate semantic sufficiency from numerical conservation and shall not claim an architecture-level conservation solution.

### F-CEX-4 — Hidden Governing State

If a proposed energy exchange requires hidden thermodynamic state or changes thermodynamic closure, it shall not be accepted as an exchange-only case. The mechanism shall be routed to RQ-EFM-001 / formulation revision.

### F-CEX-5 — Write-Authority Leakage

If an internal transfer seems possible only by giving an Extension Module write authority over Thermodynamic State, that architecture shall be rejected as inconsistent with the existing Framework baseline. The research shall test whether supplying contributions to Thermodynamic Computation preserves the required behavior instead.

### F-CEX-6 — Composition-Only Result

If the surviving concept is only a recombination of established power-bond/co-simulation/conservative-mapping prior art with already completed RQ-EFM-001 and RQ-ISO-001 results, RQ-CEX-001 shall be downgraded to an engineering/conformance property rather than retained as an independent research contribution.

---

## 12. Guardrails

RQ-CEX-001 shall not claim novelty for:

- conservation of energy;
- energy balance equations;
- effort-flow power variables;
- bond graphs or power bonds;
- interface fluxes;
- source terms;
- conservative finite-volume fluxes;
- conservative mesh mapping;
- energy residuals in co-simulation;
- power integration over time;
- or generic multiphysics coupling.

RQ-CEX-001 shall not prescribe:

- API signatures;
- packet or message schemas;
- UUIDs or message identifiers;
- queues or transactional delivery;
- thread/process scheduling;
- synchronization protocols;
- timestep or subcycling algorithms;
- interpolation/extrapolation algorithms;
- numerical tolerances;
- mesh mapping algorithms;
- or hardware/backend mechanisms.

The research shall remain bounded to ThermoCore thermodynamic-framework architecture.

---

## 13. Candidate Research Outputs

If the first evidence pass justifies continuation, the next artifacts should be:

1. `Research/01_Evidence_Matrix/Conservative_Energy_Exchange_Evidence_Matrix_v0.1.md`
2. a refined semantic-dimension matrix;
3. direct-antecedent exclusions;
4. a bounded candidate-gap statement, if any;
5. only then, if evidence supports it, a Research Gap Analysis and pre-registered consequence test.

No normative Framework refinement is authorized by this definition.

---

## 14. Current Disposition

| Question | Status |
|---|---|
| Is generic energy conservation a candidate ThermoCore contribution? | **NO — established physics / prior art** |
| Are power-bond / conservative-coupling concepts assumed novel? | **NO** |
| Is a ThermoCore-specific cross-boundary semantic contract established? | **UNDER SURVEY** |
| Is a Research Gap established? | **NO** |
| Is novelty / first-ever priority established? | **NO** |
| Is a Framework Specification change authorized? | **NO** |
| Next step | **Bounded direct-antecedent Evidence Matrix** |

---

## 15. Review Checklist

- [x] The question is about exchange semantics/accounting, not generic conservation novelty.
- [x] Quantity form, measure basis, temporal support, sign/orientation, exchange role, and accounting responsibility are separated.
- [x] Semantic conservation is separated from numerical conservation.
- [x] External sources are distinguished from internal transfer and cross-domain conversion.
- [x] Thermodynamic Computation remains the exclusive Framework Core writer of Thermodynamic State.
- [x] Framework Interfaces remain communication responsibilities without information ownership.
- [x] RQ-EFM-001 remains the governing boundary for formulation incompleteness.
- [x] RQ-ISO-001 remains the authority/non-promotion rule.
- [x] Strong prior art is explicitly anticipated.
- [x] Negative and downgrade outcomes remain valid.
- [x] No Framework Specification or production implementation change is introduced.
- [x] The next stage is evidence review rather than consequence testing.
