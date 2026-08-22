# Isolation Capability Matrix v0.3

Status: Under Survey  
Research Question: RQ-ISO-001  
Date: 2026-08-07

---

## 1. Purpose of this revision

This revision extends the falsification search beyond SOFA, MOOSE, preCICE, and VTK by adding two stronger counterexamples:

- Functional Mock-up Interface (FMI/FMU); and
- OpenMDAO.

The purpose is not to accumulate frameworks. It is to test whether ThermoCore's remaining candidate distinction—governed state-authority isolation—has already been provided by model-exchange or multidisciplinary-analysis architectures.

This document supersedes the capability assessment in v0.2 for the frameworks covered here. It does not establish novelty or a Research Gap.

---

## 2. Assessment scale

| Mark | Meaning |
|---|---|
| Y | Explicitly supported by direct evidence |
| P | Partially supported, differently scoped, or supported only for certain component types |
| N | Evidence indicates the capability is not an architectural objective |
| U | Insufficient evidence |
| N/A | Not applicable to the framework purpose |

A `Y` indicates capability presence, not semantic equivalence with ThermoCore.

---

## 3. Capabilities

| ID | Capability | Evaluation question |
|---|---|---|
| C1 | Authoritative State | Is one authoritative runtime state identified for the evaluated physical domain? |
| C2 | Explicit State Evolver | Is responsibility for advancing that state assigned to a defined owner? |
| C3 | Read Does Not Confer Ownership | Is information consumption separated from ownership or modification authority? |
| C4 | Representation Non-redefinition | Is a representation prohibited from redefining the authoritative physical state? |
| C5 | Extension-owned State | Can optional mechanisms own state without expanding mandatory core state? |
| C6 | Core Complete Without Extensions | Is the core complete and valid when optional extensions are absent? |
| C7 | Interface-governed Communication | Must communication use declared interfaces, connections, mappings, or calls? |
| C8 | Ownership/Semantics Preserved in Communication | Does communication preserve source authority and information semantics? |
| C9 | Optional-state Containment | Is optional state prevented from automatically becoming universal framework state? |
| C10 | Validation-scope Boundary | Is core validation separable from extension or component validation? |
| C11 | Core-change Isolation | Can optional capability be added without redefining core responsibilities? |
| C12 | Normative Non-redefinition Governance | Are explicit rules stated against redefining, replacing, owning, or bypassing the core? |

---

## 4. Expanded capability matrix

| Capability | SOFA | MOOSE | preCICE | VTK | FMI | OpenMDAO | ThermoCore | Interpretation |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| C1 Authoritative State | P | P | N | N/A | P | P | Y | FMI defines FMU states and variable semantics, but authority may reside in an FMU or importer depending on interface type. OpenMDAO has source outputs and model vectors, not one physical-domain state. |
| C2 Explicit State Evolver | P | P | Y | N/A | Y | P | Y | FMI explicitly assigns integration/time advancement to the importer in Model Exchange and to the FMU in Co-Simulation. This is strong execution authority, but it is interface-type dependent. |
| C3 Read Does Not Confer Ownership | U | Y | P | P | Y | Y | FMI input/output causality and permitted API calls constrain access. OpenMDAO components receive inputs and compute their own outputs. Neither alone proves physical-state ownership governance. |
| C4 Representation Non-redefinition | P | N/A | N/A | P | N/A | N/A | Y | FMI and OpenMDAO integrate executable models or computational components, not representation-only consumers. Their components are expected to define behavior or outputs. |
| C5 Extension-owned State | P | Y | Y | P | Y | Y | Y | FMUs encapsulate internal state; OpenMDAO components own outputs and may maintain implementation-local data. This capability is established prior art. |
| C6 Core Complete Without Extensions | P | Y | Y | Y | Y | Y | Y | Standards/frameworks remain meaningful without any particular optional model, component, plugin, or layered standard. |
| C7 Interface-governed Communication | Y | Y | Y | Y | Y | Y | Y | Universal prior art. FMI uses API/state-machine rules; OpenMDAO uses declared inputs, outputs, and connections. |
| C8 Ownership/Semantics Preserved | U | P | Y | P | Y | P | Y | FMI variable causality, variability, and state-machine permissions preserve declared variable semantics across the API. OpenMDAO preserves source/target roles, units, and shape metadata, but does not present a general physical ownership doctrine. |
| C9 Optional-state Containment | U | P | Y | P | Y | Y | Y | FMU internal state is encapsulated rather than inserted into an importer-wide universal state. OpenMDAO component variables remain scoped to components/model connections. |
| C10 Validation-scope Boundary | U | Y | P | P | P | P | Y | FMI supplies validated reference FMUs and conformance-oriented artifacts, but the standard does not by itself prove application-model validation independence. OpenMDAO supports component-level derivative checks, not an equivalent normative conformance boundary. |
| C11 Core-change Isolation | P | Y | Y | Y | Y | Y | Y | FMI and OpenMDAO are strong counterexamples for adding models/components without changing framework internals. This capability is not distinctive. |
| C12 Normative Non-redefinition Governance | U | U | U | N | P | U | Y | FMI has extensive normative state-machine and API restrictions, but they govern legal interaction sequences and model exchange—not a rule that optional representations cannot redefine an authoritative physical core. ThermoCore's exact combination remains unmatched in this survey. |

---

## 5. New evidence records

### ISO-E06 — FMI/FMU interface and state-machine governance

**Evidence status:** Verified for interface-state governance, variable causality, encapsulated FMU state, and separation of model algorithm from solver responsibility.

FMI defines standardized APIs and state machines for three interface types:

- Model Exchange: the FMU exposes model equations while the importer performs numerical integration and advances continuous states;
- Co-Simulation: the FMU includes the model algorithm and required solution method, advancing internally between communication points; and
- Scheduled Execution: an external scheduler triggers model partitions.

Variables exchanged between importer and FMU carry declared causality, variability, type, and other semantics in `modelDescription.xml`. The standard constrains which calls are legal in each state and when variable classes may change.

FMI is therefore a strong example of **normatively governed interaction**. It falsifies any claim that ThermoCore is unusual merely because it uses strict interfaces, state machines, encapsulated component state, or legal-call constraints.

However, FMI governs integration and interoperability among executable dynamic models. It does not establish a common physical state that optional representation layers may interpret but must not redefine. An FMU can itself define subsystem behavior and state equations; that is its intended purpose.

**Supported capabilities:** C2 yes, C3 yes, C5 yes, C6 yes, C7 yes, C8 yes, C9 yes, C11 yes, C12 partial.

**Primary sources:**

- FMI 3.0.2 Specification: https://fmi-standard.org/docs/3.0.2/
- FMI current development specification: https://fmi-standard.org/docs/main/
- FMI project overview and Layered Standards: https://fmi-standard.org/

**Falsification result:**

FMI significantly narrows the candidate distinction. `Normative governance` alone is not distinctive. The remaining distinction must include **what is governed**: ThermoCore governs the authority boundary between thermodynamic-state evolution and optional interpretation/extension, whereas FMI governs execution, state-machine transitions, variable exchange, and solver/model responsibility.

---

### ISO-E07 — OpenMDAO source-output and component-connection architecture

**Evidence status:** Verified for component-scoped inputs/outputs, declared connections, ultimate variable sources, and independent-variable components.

OpenMDAO requires components to declare inputs and outputs. Data flows from a source output to one or more target inputs through explicit or promoted connections. Every input ultimately has a source; unconnected model inputs receive an automatically created independent-variable source. Components compute their own outputs from provided inputs, and the framework manages connected vectors, unit conversion, shapes, and solver coordination.

This architecture provides strong **data-source authority** at the variable level:

```text
Source component output
          ↓ declared connection
Target component input
          ↓
Target-owned output
```

OpenMDAO therefore falsifies any claim that source/consumer direction, component-local outputs, explicit connections, or replaceable computational components are unique to ThermoCore.

However, OpenMDAO is intended to compose multidisciplinary analyses and optimization models. Components may define arbitrary discipline equations and outputs. It does not distinguish one framework-owned thermodynamic state from representation-only consumers, nor does it prohibit a newly added component from introducing a new discipline state or governing relation.

**Supported capabilities:** C3 yes at variable-flow level, C5 yes, C6 yes, C7 yes, C9 yes, C11 yes; C1, C2, C8, C10 partial; C4 not applicable.

**Primary sources:**

- OpenMDAO, Declaring Continuous Variables: https://openmdao.org/newdocs/versions/latest/features/core_features/working_with_components/continuous_variables.html
- OpenMDAO, Connecting Variables: https://openmdao.org/newdocs/versions/latest/features/core_features/working_with_groups/connect.html
- OpenMDAO, IndepVarComp: https://openmdao.org/newdocs/versions/latest/features/core_features/working_with_components/indepvarcomp.html
- OpenMDAO, Setting and Getting Component Variables: https://openmdao.org/newdocs/versions/latest/features/core_features/running_your_models/set_get.html
- OpenMDAO, Scaling Variables: https://openmdao.org/newdocs/versions/latest/theory_manual/scaling.html

**Falsification result:**

OpenMDAO shows that explicit source ownership and component-scoped outputs are established architecture patterns. ThermoCore's candidate contribution cannot be framed as generic source-to-consumer flow. It must be framed around a domain-specific restriction: optional representations and ordinary extensions do not gain authority to redefine Thermodynamic State or its evolution.

---

## 6. Revised findings

### F-ISO-07 — Normative interface governance is established prior art

FMI contains extensive normative restrictions on state transitions, legal calls, variable changes, and solver/model responsibilities. ThermoCore shall not claim novelty merely because its interfaces use normative `shall` or `shall not` language.

### F-ISO-08 — Source-output authority is established prior art

OpenMDAO ensures that every connected input has a source output and that components declare their own inputs and outputs. ThermoCore shall not claim novelty merely for producer-consumer direction or source authority.

### F-ISO-09 — Encapsulated optional state is established prior art

FMUs encapsulate internal model state; MOOSE supports stateful Material properties; preCICE participants retain solver state; OpenMDAO components scope variables and computation. Extension-owned state is not distinctive by itself.

### F-ISO-10 — The remaining hypothesis is a restricted authority model

After adding FMI and OpenMDAO, the remaining candidate distinction is narrower than `Governed Isolation` in general:

> ThermoCore may define a restricted authority model in which Thermodynamic State has one framework-level semantic authority, while optional Representation and ordinary Extension responsibilities may consume, interpret, supply permitted coupling information, or own mechanism-specific state without acquiring authority to redefine Thermodynamic State, its owner, or Framework Core completeness.

This is currently named **Restricted State-Authority Isolation** as a working term. It remains Under Survey.

---

## 7. What FMI and OpenMDAO do that ThermoCore should learn from

### 7.1 FMI lesson: specify legal lifecycle transitions

ThermoCore currently governs responsibility and ownership but intentionally avoids execution scheduling. FMI demonstrates the value of separately specifying legal lifecycle states and call sequences.

This does not mean ThermoCore should adopt an execution state machine into the current core specification. It means future implementation guidance may need to distinguish:

- architectural authority;
- legal communication operations; and
- backend execution lifecycle.

These shall not be conflated.

### 7.2 OpenMDAO lesson: make the source graph inspectable

OpenMDAO can list inputs, outputs, and connections. ThermoCore's ownership rules would be stronger in practice if a conforming implementation could expose an inspectable graph showing:

- authoritative source of each Framework information category;
- consumers;
- permitted suppliers;
- extension-owned state; and
- prohibited or undeclared bypasses.

This is a candidate validation mechanism, not a current normative requirement.

---

## 8. Stronger falsification test

The Restricted State-Authority Isolation hypothesis shall be rejected if an existing framework is found that jointly and explicitly provides:

1. one framework-level authoritative state for a physical domain;
2. a defined owner with exclusive authority over state evolution semantics;
3. representation components restricted to interpretation/consumption;
4. optional extension-owned persistent state kept outside authoritative runtime state;
5. permitted feedback mechanisms that do not transfer state ownership;
6. core completeness and conformance without all optional extensions;
7. explicit prohibition of ordinary extensions redefining the state owner, state semantics, or core responsibility; and
8. a validation/conformance boundary derived from those authority rules.

FMI meets several governance conditions but not conditions 1–4 in this representation-specific form. OpenMDAO meets strong source/connection conditions but not conditions 1–3 or 7.

---

## 9. Next evidence tasks

1. Search scientific-workflow and digital-twin frameworks for a common physical state plus read-only views or representations.
2. Review Modelica connector semantics and state selection as a possible stronger counterexample.
3. Review OpenFOAM runtime-selection architecture to test whether physical fields remain authoritative while models are replaceable.
4. Deepen SOFA evidence on mechanical-state authority and mapped visual/collision models.
5. Deepen MOOSE SQA evidence on requirement dependencies and revalidation boundaries.
6. Design an implementation falsification experiment with:
   - a stateless representation;
   - a history-dependent extension;
   - a feedback-producing extension; and
   - an intentionally non-conforming extension that attempts to redefine Runtime State.

---

## 10. Current classification

| Candidate claim | Classification after v0.3 |
|---|---|
| Multiple representations | Verified prior art |
| Plugin/module extensibility | Verified prior art |
| Read-only consumption | Verified prior art |
| Explicit source-to-consumer connections | Verified prior art |
| Encapsulated component/extension state | Verified prior art |
| Normative interface and lifecycle governance | Verified prior art |
| Core-change isolation | Verified prior art |
| Restricted State-Authority Isolation combination | Under Survey |
| Reduced revalidation scope | Unverified hypothesis |
| Reduced mandatory core-state growth | Unverified hypothesis |
| Research Gap | Not established |

---

## 11. Interim conclusion

FMI and OpenMDAO substantially reduce the breadth of any defensible ThermoCore contribution.

ThermoCore cannot claim novelty for strict interfaces, legal call constraints, component state encapsulation, declared data sources, or modular model composition. Those capabilities are well established.

The remaining candidate is specifically the restriction placed on optional interpretation and extension responsibilities around one authoritative thermodynamic state:

> Interaction may add capability and mechanism-specific information, but shall not silently transfer authority over Thermodynamic State semantics, ownership, or Framework Core completeness.

The next survey must actively search for frameworks that already combine a shared physical state with this representation-specific non-redefinition rule.