# RQ-MRR-001 — Material Representation Responsibility Boundary

Version: 0.1  
Status: DEFINED — Evidence Survey Required  
Research Question: RQ-MRR-001 — Material Representation Responsibility Boundary  
Tracking Issue: #143  
Date: 2026-08-23

---

## 1. Objective

Define the final unresolved architectural research line inherited from the original RQ-001 gap analysis: the responsibility boundary of **Material Representation**.

The research shall determine whether ThermoCore requires an independent architectural criterion for distinguishing legitimate downstream material interpretation from responsibilities that belong instead to Thermodynamic Computation, Material Definition Configuration, Extension Modules, or external/application consumers.

This document is non-normative. It does not modify Framework Specification, implementation, Verification, Validation, Performance, or the frozen v1.0.0 release.

---

## 2. Historical Research Context

The initial `Research_Gap_Analysis_v0.1.md` identified four unresolved architectural boundaries:

1. ownership of evolving simulation state;
2. responsibility of Material Representation;
3. runtime material abstraction;
4. extension coupling boundary.

Subsequent research has independently processed the other three lines:

- **RQ-ISO-001** established a bounded result for fixed semantic/Core-state authority and non-promotion under ordinary extension;
- **RQ-EFM-001** established a bounded formulation-relative admissibility gate for externally driven and cross-domain material response;
- **RQ-RMA-001** found that the runtime material abstraction candidate did not survive as an independent research gap and was reclassified as the **Configuration-Derivative Identity Property**.

RQ-MRR-001 therefore addresses the remaining original RQ-001 boundary.

The original RQ-001 analysis described the candidate as **Independent Material Representation** and asked whether material-dependent interpretation and application-facing response could remain separate from the computational ownership of evolving thermodynamic state.

The present research must not assume that this historical candidate is novel or independently contributory.

---

## 3. Current Normative Baseline

The current Framework Specification already provides a strong normative answer.

`Material_Representation.md` states that:

- Material Representation interprets Thermodynamic State and applicable Material Definition;
- Material Representation produces and owns Representation;
- Representation remains distinct from Runtime State and Configuration;
- Material Representation does not own Runtime State;
- Material Representation does not modify Thermodynamic State;
- Material Representation does not perform State Evolution;
- Representation does not replace Thermodynamic State or Material Definition;
- Representation Consumers remain outside the Framework Core;
- persistent Representation remains Representation rather than becoming Runtime State merely because it is maintained.

This existing normative design is a **baseline to be evaluated**, not novelty evidence.

RQ-MRR-001 asks whether this separation adds independent architectural decision power beyond established prior art and existing ThermoCore boundaries.

---

## 4. Primary Research Question

> **Within a fixed declared ThermoCore thermodynamic scope, what semantic boundary distinguishes legitimate Material Representation — downstream interpretation of authoritative Thermodynamic State and applicable Material Definition for a Representation Consumer — from responsibilities that belong instead to Thermodynamic Computation, Material Definition Configuration, an Extension Module, or another external/application responsibility; and does this boundary provide any independent research decision power beyond established state/output separation and existing ThermoCore ownership, admissibility, and conformance rules?**

The question is intentionally responsibility-centered rather than rendering-centered.

It does not assume that visualization, rendering, post-processing, constitutive output, derived variables, or application-facing data are identical concepts.

---

## 5. Relationship to Existing ThermoCore Results

### 5.1 RQ-ISO-001

RQ-ISO-001 governs authoritative state ownership and non-promotion.

RQ-MRR-001 shall route a case to RQ-ISO-001 when the decisive question is whether representation-local or extension-local information is being promoted into mandatory authoritative Core State merely because it participates in a downstream or coupled workflow.

RQ-MRR-001 shall not duplicate state-authority logic.

### 5.2 RQ-EFM-001

RQ-EFM-001 governs formulation-relative thermodynamic sufficiency and ordinary/Core-preserving extension admissibility.

RQ-MRR-001 shall route a case to RQ-EFM-001 when a quantity called "representation" is actually required to close the selected thermodynamic formulation, determine future Thermodynamic State uniquely, or otherwise carry governing thermodynamic authority.

Packaging does not override physical role.

### 5.3 RQ-RMA-001

RQ-RMA-001 established that normalized, compiled, tabulated, cached, persistent, runtime-rebuilt, or backend-specialized material data does not become Runtime State solely because of implementation form or lifecycle.

RQ-MRR-001 shall preserve that distinction.

A cached visualization output, persistent derived field, or GPU-resident representation shall not be classified by storage form alone.

### 5.4 Conservative Exchange Accounting Property

If downstream interpretation is converted into an energy-bearing contribution and fed back into Thermodynamic Computation, the resulting physical contribution must be classified and accounted through applicable source/exchange semantics.

The fact that the value originated in a Representation path does not exempt it from energy-accounting requirements.

### 5.5 RQ-ECA-001

If several representation-producing Extensions interact, composition alone does not establish a new boundary. The aggregate mechanism shall be reevaluated through the existing admissibility, ownership, and accounting rules when necessary.

---

## 6. Baseline Information and Responsibility Classes

### 6.1 Thermodynamic State

Authoritative Runtime State governed by the Framework and evolved by Thermodynamic Computation.

### 6.2 Material Definition

Reusable Configuration describing applicable material semantics or parameters without becoming evolving thermodynamic state.

### 6.3 Thermodynamic Computation

The Core responsibility that owns State Evolution and remains the exclusive Core writer of Thermodynamic State.

### 6.4 Material Representation

The Framework responsibility that interprets Thermodynamic State and applicable Material Definition to produce Representation for downstream consumption.

### 6.5 Representation

Information produced by Material Representation for downstream interpretation or consumption.

Representation may be persistent or derived under the current normative baseline, but neither persistence nor derivation alone determines Runtime State status.

### 6.6 Representation Consumer

An external/downstream consumer of Representation.

A consumer may render, display, transform, record, classify, or otherwise use Representation without becoming part of the Framework Core merely through consumption.

### 6.7 Extension-Specific Representation

A provisional research term for downstream output produced from extension-owned state or extension-specific material interpretation.

It remains subject to extension ownership and admissibility rules and shall not redefine Core Representation ownership or Thermodynamic State.

---

## 7. Candidate Semantic Dimensions

All dimensions in this section are **UNDER SURVEY**.

### MRR-D1 — Interpretation versus State Evolution

Question:

> Does the operation interpret already-authoritative information for downstream use, or does it participate in determining/evolving authoritative thermodynamic state?

Candidate distinction:

```text
interpret existing state
    versus
change governing state evolution
```

A state-dependent function is not automatically Representation; the decisive issue is whether the result is downstream interpretation or governing input/closure.

### MRR-D2 — Source Dependence versus Independent Physical Authority

Question:

> Is Representation semantically dependent on authoritative Thermodynamic State and applicable Material Definition, or can it carry independent physical meaning required to determine future thermodynamic behavior?

Candidate expectation:

Representation should not become a second authoritative source of thermodynamic truth.

### MRR-D3 — Representation Ownership versus Source Ownership

Question:

> Can Material Representation own the produced Representation while preserving ownership of Thermodynamic State and Material Definition in their respective responsibilities?

The current normative answer is yes.

The research question is whether this ownership distinction has independent contribution value or merely formalizes established producer/output ownership practice.

### MRR-D4 — State-Dependent Interpretation versus Closure-Critical Computation

Question:

> If an output depends strongly on current Thermodynamic State, does that dependence remain downstream interpretation, or is the output required for closure/update sufficiency?

Candidate discriminator:

- downstream derived interpretation -> Representation candidate;
- closure-critical evolving quantity -> RQ-EFM-001 / State or governing formulation pressure.

### MRR-D5 — Downstream Consumption versus Feedback / Control

Question:

> Does a Representation Consumer merely consume output, or does transformed Representation feed back into Thermodynamic Computation as a control, source, material update, or coupled-domain contribution?

Feedback does not necessarily make Representation invalid, but the feedback path must be reclassified according to its new physical role.

A downstream value cannot bypass Energy Input, extension admissibility, ownership, or accounting boundaries by retaining the label "Representation".

### MRR-D6 — Persistent Representation versus Runtime State

Question:

> Can Representation persist across framework operation without becoming Runtime State?

The current normative baseline says yes when persistence serves downstream continuity rather than carrying governing physical memory.

This proposition must be tested against direct antecedents.

### MRR-D7 — Consumer-Specific Output versus Reusable Material Definition

Question:

> Does an application-facing response encode downstream interpretation, or does it redefine reusable material meaning?

Consumer-specific visual or application mappings should not silently become a second Material Definition authority.

### MRR-D8 — Extension-Specific Representation versus Extension Governing Responsibility

Question:

> When an Extension produces representation from extension-local state, can the output remain downstream Representation without transferring extension-state ownership or governing authority into Material Representation?

Mechanism name or consumer type shall not determine classification.

---

## 8. Candidate Boundary Tests

These tests are provisional research tools and are not normative requirements.

### Test I — Interpretation Sufficiency

Holding authoritative Thermodynamic State and applicable Configuration fixed, does the candidate operation merely produce a downstream interpretation without affecting required thermodynamic evolution?

If yes, it is compatible with Representation semantics.

If no, further routing is required.

### Test G — Governing Necessity

Would omission of the candidate output make the selected thermodynamic closure incomplete or the required next Thermodynamic State non-unique?

If yes, the quantity or operation is not merely downstream Representation for the claimed formulation and shall be routed to RQ-EFM-001.

### Test A — Authority Preservation

Can the Representation be regenerated or replaced by a semantically equivalent downstream encoding without changing authoritative Thermodynamic State or Material Definition meaning?

If no because the Representation has become the only source of governing material or state truth, an authority boundary has been crossed.

### Test F — Feedback Reclassification

If downstream Representation is transformed and fed back into the simulation, is the feedback semantically declared according to its actual physical role?

Possible roles include:

- Energy Input;
- property/configuration update;
- extension-local input;
- cross-domain exchange;
- external control.

The original Representation label does not govern the feedback semantics.

### Test P — Persistence Independence

Can the Representation be retained across frames/steps solely for downstream continuity or performance while authoritative simulation behavior remains determined by State, Configuration, declared Extension state, and governing exchanges?

If yes, persistence alone does not imply Runtime State.

---

## 9. Candidate Matched Scenarios

These scenarios guide evidence collection. They are not pre-registered experiments.

### MRR-S0 — Temperature-to-color control

Current temperature and applicable material information are mapped to a display color.

Expected pressure:

- downstream interpretation;
- no State Evolution;
- no independent physical authority.

Candidate classification: Representation.

### MRR-S1 — Phase/application interpretation

A phase label, opacity, visual fraction, or consumer-facing phase category is derived from current authoritative thermodynamic information and material rules.

Purpose: test whether strongly state-dependent interpretation remains Representation when it does not determine state evolution.

### MRR-S2 — Expensive derived Representation cache

A representation field is expensive to calculate and therefore cached across frames or steps and invalidated when its authoritative sources change.

Purpose: test persistent/derived Representation versus Runtime State.

### MRR-S3 — Persistent consumer continuity

A downstream consumer requires representation continuity across updates, for example interpolation state, display history, or visual smoothing.

Purpose: determine whether consumer continuity data remains Representation/application state or acquires thermodynamic authority.

### MRR-S4 — Hidden governing quantity mislabeled as Representation

A quantity is stored in a representation structure but is required to determine the next thermodynamic state uniquely.

Expected routing: RQ-EFM-001 and/or RQ-ISO-001 depending on authority.

Purpose: test whether packaging can hide governing state.

### MRR-S5 — Representation-derived feedback

A consumer computes a control action or energy contribution from Representation and sends it back into the thermodynamic system.

Purpose: test whether the feedback must enter through declared input/coupling/accounting semantics rather than allowing Representation to acquire write authority.

### MRR-S6 — Extension-specific Representation

An extension owns local state such as hysteresis, moisture, reaction progress, or another admitted mechanism and produces a downstream visual/application representation from that state.

Purpose: test whether extension-specific Representation can remain downstream output without reassigning extension-state ownership.

### MRR-S7 — Constitutive response required by governing computation

A material response quantity is generated from current state and material information but is required inside the governing state-update calculation.

Purpose: distinguish solver-facing constitutive response from application-facing Representation.

Expected pressure: Thermodynamic Computation / RQ-EFM rather than downstream Material Representation.

### MRR-S8 — Consumer-side transformation

A Representation Consumer stores, converts, renders, compresses, resamples, or derives additional display/application data from Representation.

Purpose: test whether downstream transformation transfers any Framework Core ownership.

Candidate expectation: no ownership transfer through consumption alone.

### MRR-S9 — Persistent physically meaningful memory

A structure historically treated as Representation stores a physically meaningful memory variable whose value affects future constitutive or thermodynamic response.

Purpose: falsify classification-by-location and route genuine physical memory to State / extension-state analysis.

---

## 10. Preliminary Falsification / Reclassification Conditions

RQ-MRR-001 must remain strongly falsifiable.

### F-MRR-1 — Direct antecedent for state/output separation

If established simulation frameworks, constitutive systems, visualization/post-processing architectures, or scientific-computing frameworks already define equivalent separation among authoritative state/model data, derived outputs, post-processing, and downstream consumers, the independent contribution candidate must be narrowed or closed.

### F-MRR-2 — Existing ThermoCore rules fully classify the cases

If all meaningful matched scenarios are completely classified by:

- Runtime State / Configuration / Representation separation;
- RQ-ISO-001 state authority;
- RQ-EFM-001 closure/admissibility;
- Framework Interfaces ownership-preserving communication;
- Conservative Exchange Accounting where energy feedback exists;
- ordinary implementation/application separation;

then RQ-MRR-001 shall be reclassified as an architecture/conformance property rather than preserved as an independent research contribution.

### F-MRR-3 — RQ-ISO absorption

If the only surviving issue is whether representation-local information may become mandatory Core State, route to RQ-ISO-001 and close the duplicate claim.

### F-MRR-4 — RQ-EFM absorption

If the only surviving issue is whether a supposedly representational quantity is required for closure or state evolution, route to RQ-EFM-001 and close the duplicate claim.

### F-MRR-5 — Consumer / rendering implementation only

If the distinction concerns only:

- shader design;
- rendering pipeline;
- display mapping;
- asset format;
- serialization;
- texture/buffer organization;
- frontend/UI state;
- consumer-side smoothing;
- backend/device layout;

then it is outside the independent Framework research claim.

### F-MRR-6 — Mature derived-output / post-processing antecedent

If the surviving rule reduces to the mature engineering principle that governing state/model information is distinct from derived/post-processed outputs and consumer artifacts, the result shall be preserved only as a ThermoCore architecture/conformance formalization unless an additional independently testable boundary is demonstrated.

---

## 11. Prior-Art Survey Plan

The first evidence pass shall be aggressive and falsification-oriented.

Priority source families:

1. **Modelica / Modelica.Media**
   - thermodynamic state records versus property functions;
   - sensor/output variables;
   - derived quantities versus governing states;
   - component outputs without state ownership transfer.

2. **MOOSE**
   - Materials versus AuxVariables / AuxKernels / postprocessors / reporters;
   - computed material properties versus stateful properties;
   - visualization/output systems versus nonlinear governing variables.

3. **OpenFOAM**
   - primary fields versus derived functionObjects/post-processing fields;
   - thermophysical model properties versus output/visualization data;
   - solver-facing versus post-processing responsibilities.

4. **DOLFINx / finite-element ecosystems**
   - solution Functions versus projected/derived fields;
   - constitutive update outputs versus post-processing;
   - history variables versus visualization fields.

5. **Cantera / CoolProp**
   - current state and property evaluation versus user-facing derived outputs;
   - whether property-query results carry state authority.

6. **Constitutive-material frameworks**
   - state/internal variables versus observable/output quantities;
   - stress or response outputs that participate in governing equations versus pure post-processing outputs.

7. **Visualization / scientific data models**
   - derived fields and filters versus authoritative simulation state;
   - pipeline data products versus source data ownership;
   - only where architecture semantics are explicit enough to be relevant.

8. **Simulation coupling / control systems**
   - output feedback re-entering as declared input rather than direct state ownership;
   - causality boundaries in FMI/Modelica or equivalent systems where relevant.

The survey shall not infer novelty from generic MVC, rendering, shader, or UI examples unless they provide a directly relevant scientific/simulation responsibility antecedent.

---

## 12. Evidence Questions for v0.1

The first Evidence Matrix shall answer:

1. Is authoritative simulation state versus derived/post-processed output separation already directly formalized in mature simulation frameworks?
2. Is state-dependent output commonly distinguished from state evolution or solver governing responsibility?
3. Do mature frameworks distinguish stateful constitutive/internal variables from ordinary derived/output quantities?
4. Is persistence of an output/derived field already treated independently from physical state authority?
5. Do output or visualization consumers commonly remain outside solver/state ownership?
6. Are solver-facing constitutive responses treated differently from downstream application/visualization representations?
7. Are feedback paths from outputs back to simulation generally reintroduced as explicit inputs/couplings rather than output-side state writes?
8. Does ThermoCore's exact ownership formalization leave any independent architecture category not already explained by these antecedents plus RQ-ISO/RQ-EFM?

---

## 13. Preliminary Candidate Outcome Space

The evidence survey may produce one of four dispositions.

### Outcome A — Independent bounded research candidate survives

Proceed only if a distinct semantic boundary remains that:

- is not directly anteceded;
- is not merely RQ-ISO state ownership;
- is not merely RQ-EFM closure/admissibility;
- is not standard derived-output/post-processing separation;
- produces independent decision consequences.

### Outcome B — Narrow architecture/conformance property survives

If the ThermoCore-specific formalization remains useful but does not constitute an independent research gap, reclassify it as an engineering/conformance property.

A provisional name, if needed after evidence, is:

**Downstream Representation Non-Authority Property**

This name is provisional and shall not be promoted before evidence review.

### Outcome C — Existing Framework rules fully absorb the issue

Close RQ-MRR-001 without creating a new named property if current State/Configuration/Representation ownership semantics are already sufficient.

### Outcome D — Problem belongs elsewhere

Route specific cases to:

- RQ-ISO-001;
- RQ-EFM-001;
- Conservative Exchange Accounting Property;
- implementation/performance;
- application/consumer architecture.

---

## 14. Prohibited Claims

Until future evidence supports a narrower statement, RQ-MRR-001 shall not claim that:

- ThermoCore is the first framework to separate simulation state from representation/output;
- Material Representation is a novel general software pattern;
- all state-dependent material response belongs in Material Representation;
- Representation is equivalent to rendering;
- persistent Representation is always non-state;
- all consumer-side feedback is invalid;
- all constitutive response belongs outside Thermodynamic Computation;
- the current Material Representation boundary is universally optimal;
- the current boundary applies unchanged to every physical domain or numerical formulation;
- a clear normative specification proves research novelty.

---

## 15. Decision Gate

RQ-MRR-001 shall not proceed to Research Gap Analysis merely because it is the final unresolved historical RQ-001 line.

A later Research Gap Analysis is **GO** only if the direct-antecedent survey identifies a surviving, independently testable boundary that is not already explained by mature output/state separation or existing ThermoCore results.

Otherwise the correct outcome is closure/reclassification.

Current disposition:

- Independent RQ-MRR Research Gap: `NOT ESTABLISHED`
- Candidate dimensions: `UNDER SURVEY`
- Research Gap Analysis readiness: `NO-GO`
- Novelty / priority: `NOT ESTABLISHED`
- Framework Specification impact: `NONE`

---

## 16. Next Research Step

After merge of this definition, create:

`Research/01_Evidence_Matrix/Material_Representation_Responsibility_Evidence_Matrix_v0.1.md`

The first pass shall prioritize direct antecedents that can falsify the independent claim before collecting broad examples of rendering or visualization systems.

---

## Document Status

This is a non-normative research definition artifact.

It defines the research problem, candidate dimensions, falsification paths, and evidence gate for RQ-MRR-001. It does not authorize Framework Specification change and does not establish a research contribution.