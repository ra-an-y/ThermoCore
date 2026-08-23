# RQ-RMA-001 — Closure and Reclassification

Version: 0.1  
Status: Closed and Reclassified — Non-Normative Research Record  
Research Question: RQ-RMA-001 — Runtime Material Abstraction Boundary  
Tracking Issue: #141  
Date: 2026-08-23

---

## 1. Purpose

This document closes **RQ-RMA-001 — Runtime Material Abstraction Boundary** as an independent research-gap line and records the surviving result as an engineering / conformance property.

This artifact is non-normative. It does not modify Framework Specification, production implementation, Verification, Validation, Performance, or the frozen v1.0.0 release.

---

## 2. Final Disposition

- **Independent Research Gap:** `NOT SUPPORTED WITHIN THE BOUNDED DIRECT-ANTECEDENT REVIEW AND MATCHED-SCENARIO STRESS TEST`
- **Research contribution claim:** `CLOSED`
- **Surviving value:** `Configuration-Derivative Identity Property`
- **Classification:** `ENGINEERING / CONFORMANCE PROPERTY — NOT AN INDEPENDENT RESEARCH CONTRIBUTION`
- **Research Gap Analysis:** `NO-GO`
- **Novelty / priority:** `NOT ESTABLISHED`
- **Framework Specification impact:** `NONE`

The negative result is retained as part of the research record rather than narrowed into a weaker novelty claim.

---

## 3. Research Question Reviewed

RQ-RMA-001 asked when reusable Material Definition Configuration that is normalized, compiled, tabulated, cached, persisted, rebuilt, or backend-specialized may remain Configuration, and when transformed material information instead crosses into Thermodynamic State, extension-local state, Material Representation, formulation-specific solver state, or Core/formulation-revision territory.

The question explicitly separated semantic identity from implementation form and did not assume that a new normative category called runtime material representation was required.

---

## 4. Evidence Basis

The bounded direct-antecedent survey reviewed evidence families including:

- Modelica.Media;
- MOOSE Materials / Kokkos Materials;
- OpenFOAM thermophysical models;
- Cantera;
- CoolProp tabular backends;
- dolfinx_materials / MFront / JAX material behavior;
- NEML history/internal-variable systems.

The review established strong antecedents for distinctions among:

- reusable material/model definition;
- current thermodynamic state;
- computed material properties;
- retained constitutive/internal history state;
- compiled or tabulated computation artifacts;
- numerical caches/workspaces;
- backend/layout specialization.

The evidence also showed that persistence, caching, compilation, tabulation, or backend specialization do not by themselves determine physical-state identity.

---

## 5. Matched-Scenario Result

The v0.2 stress test evaluated nine matched scenarios:

1. pure normalization / unit conversion;
2. precomputed derived constants / LUT;
3. CPU object versus GPU/SIMD packed representation;
4. runtime invalidation and rebuild after explicit Configuration change;
5. current-state-dependent property evaluation without retained history;
6. hysteretic/history-dependent material response with persistent internal variable;
7. composition/reaction/microstructure evolution affecting future response;
8. formulation-specific closure coordinate hidden inside a material/runtime structure;
9. pure numerical cache/workspace carrying no independent physical authority.

Result:

- matched scenarios evaluated: `9`;
- scenarios requiring a new independent RQ-RMA architecture rule: `0`;
- candidate semantic dimensions re-evaluated: `8`;
- dimensions producing a new independent architecture category: `0`.

All meaningful cases were classifiable through established distinctions plus existing ThermoCore rules.

---

## 6. Surviving Engineering Property

### 6.1 Configuration-Derivative Identity Property

For a fixed declared ThermoCore scope, a material artifact does not leave the Configuration category solely because it is:

- normalized;
- compiled;
- tabulated;
- cached;
- persisted;
- rebuilt during runtime operation;
- packed for SIMD/GPU/device use;
- specialized for a backend;
- represented as a table, object, buffer, generated evaluator, or equivalent computation-ready encoding.

A semantic reclassification requires a stronger reason than storage form or runtime lifecycle.

Relevant reasons may include:

- independent evolving physical memory/history;
- independent authority over material meaning;
- a formulation-relative evolving coordinate required for thermodynamic closure or unique state evolution;
- an explicit change in governing scope or Core responsibility.

This property is useful for implementation review and conformance reasoning. It is not established as an independent research contribution.

### 6.2 Reconstructibility

Reconstructibility from authoritative Material Definition and declared transformation inputs is a useful diagnostic.

For example, if a computation-ready artifact can be discarded and deterministically regenerated from authoritative Configuration without knowledge of evolving physical history, that supports Configuration-like classification.

However:

> **Reconstructibility is not a universal definition of Configuration.**

Classification remains semantic and responsibility-based rather than purely functional or storage-based.

---

## 7. Boundary Routing

### 7.1 RQ-ISO-001

RQ-ISO-001 remains the governing boundary for state authority and non-promotion.

If an evolving quantity is valid extension-local state but is being promoted into mandatory authoritative Core State merely because it participates in computation or material response, the case belongs to RQ-ISO-001 rather than RQ-RMA-001.

RQ-RMA-001 does not redefine state ownership.

### 7.2 RQ-EFM-001

RQ-EFM-001 remains the governing boundary for formulation-relative thermodynamic sufficiency.

If correct thermodynamic closure or unique state evolution requires an evolving material quantity that the selected formulation does not currently represent honestly, the case belongs to RQ-EFM-001 and may require formulation/Core revision or scope narrowing.

Packaging such a quantity inside a material object, LUT, cache, or backend buffer does not make the formulation complete.

### 7.3 Material Representation

Material Representation remains a distinct downstream interpretation responsibility.

A computation-ready material artifact is not Material Representation merely because it is a representation in the ordinary programming sense.

Material Representation continues to mean downstream interpretation of Thermodynamic State and applicable Material Definition for Representation Consumers.

### 7.4 Implementation / Performance

Pure differences involving:

- AoS versus SoA;
- CPU versus GPU placement;
- memory alignment;
- serialization;
- cache policy;
- table density;
- code generation;
- JIT/AOT compilation;
- workspace reuse;
- numerical acceleration structures;

remain implementation/performance concerns unless they change semantic authority, physical-state meaning, formulation completeness, or applicable Framework responsibilities.

---

## 8. Important Classification Rules

The following statements survive as engineering/conformance guidance:

1. **Runtime use does not imply Runtime State.**
2. **Persistence does not imply physical state.**
3. **Compilation does not create material authority.**
4. **Tabulation or LUT storage does not create material authority.**
5. **GPU/device residence does not create a new semantic category.**
6. **Runtime rebuild after an explicit Configuration change remains Configuration lifecycle behavior when no independent physical history is introduced.**
7. **Current-state-dependent derived property evaluation does not require retained material state when the property is fully determined by authoritative current state and Configuration.**
8. **Independent evolving physical memory/history is State even when stored inside a material object, table, cache, or device buffer.**
9. **A hidden closure-critical evolving coordinate cannot be made Configuration merely by packaging.**
10. **Storage form, class name, memory location, persistence duration, or backend alone shall not determine semantic classification.**

These are not new normative Framework clauses introduced by this research record.

---

## 9. Why the Independent Research Claim Is Closed

The bounded evidence did not reveal a distinct RQ-RMA architecture predicate beyond the combination of:

- established material-definition versus state distinctions;
- established stateful/history-variable treatment;
- established tabulation/cache/backend specialization practice;
- existing ThermoCore Runtime State / Configuration / Representation separation;
- RQ-ISO-001 state-authority/non-promotion rules;
- RQ-EFM-001 formulation-relative closure/state-space rules.

The surviving Configuration-Derivative Identity Property is useful, but the evidence supports treating it as a concise engineering/conformance consequence of those established distinctions rather than as a new independent contribution.

Accordingly, no Research Gap Analysis is opened for RQ-RMA-001 on the current evidence baseline.

---

## 10. Future Verification / Conformance Use

Future engineering work may test the surviving property without reopening the research contribution claim.

Suitable conformance/verification cases may include:

- equivalent CPU and GPU encodings of one Material Definition;
- LUT generation and deterministic rebuild from unchanged Configuration;
- cache invalidation after explicit Configuration modification;
- rejection of a history-bearing internal variable mislabeled as Configuration;
- rejection of a closure-critical evolving quantity hidden inside a material buffer;
- confirmation that state-dependent derived property evaluation introduces no independent persistent material state when current authoritative inputs are sufficient.

Such work would provide engineering evidence about a ThermoCore implementation. It would not by itself establish novelty or reopen RQ-RMA-001 as a research gap.

---

## 11. Guardrails

This closure does **not** establish that:

- every material artifact reconstructible from Configuration is universally Configuration;
- all persistent data is non-state;
- all material history belongs in Core Thermodynamic State;
- all material response can be represented without formulation revision;
- the current reference compiler proves the architecture;
- Modelica, MOOSE, OpenFOAM, Cantera, CoolProp, DOLFINx/MFront, JAX, or NEML are complete ThermoCore equivalents;
- ThermoCore is the first framework to distinguish material definition from state;
- the surviving property is universally optimal or superior.

The bounded negative result applies only to the reviewed evidence and evaluated ThermoCore scenarios.

---

## 12. Relationship to the Original RQ-001 Gap

The original RQ-001 analysis identified Runtime Material Abstraction as unresolved while already recognizing runtime material abstraction itself as established practice.

RQ-RMA-001 has now tested the narrower candidate boundary and found no independent research gap within the bounded direct-antecedent review and matched-scenario evaluation.

The practical value of the original concern is retained through explicit Configuration/State/Representation semantics, RQ-ISO-001, RQ-EFM-001, and the surviving Configuration-Derivative Identity Property.

This closes the Runtime Material Abstraction research line without requiring a new Framework information category.

---

## 13. Final Status

RQ-RMA-001 is closed as an independent research-contribution line.

Final status:

```text
Independent Research Gap:
NOT SUPPORTED WITHIN THE BOUNDED DIRECT-ANTECEDENT REVIEW
AND MATCHED-SCENARIO STRESS TEST

Research Contribution Claim:
CLOSED

Surviving Value:
Configuration-Derivative Identity Property

Classification:
ENGINEERING / CONFORMANCE PROPERTY
— NOT AN INDEPENDENT RESEARCH CONTRIBUTION

Research Gap Analysis:
NO-GO

Framework Specification Impact:
NONE
```

Future evidence may motivate a new, independently defined research question, but it shall not retroactively convert this bounded negative result into a novelty claim.