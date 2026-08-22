# RQ-ISO-001 Research Gap Analysis v0.1

Status: Draft — Evidence-supported Candidate Gap Analysis  
Research Question: RQ-ISO-001  
Date: 2026-08-23  
Primary evidence dependency: [`Isolation_Capability_Matrix_v0.6.md`](../01_Evidence_Matrix/Isolation_Capability_Matrix_v0.6.md)

---

## 1. Objective

This document evaluates whether the bounded RQ-ISO-001 evidence survey supports a research-gap candidate concerning **Fixed Semantic/Core-State Boundary under Ordinary Extension**.

The purpose is not to establish novelty, priority, superiority, or universal applicability. The purpose is to determine whether the surviving distinction from the completed capability survey is sufficiently specific, evidence-bounded, falsifiable, and consequential to justify a separate research question and later evaluation.

This document is non-normative. It does not modify the ThermoCore Framework Specification, does not retroactively change the v1.0.0 publication baseline, and does not convert an evidence-supported candidate into a validated contribution.

---

## 2. Evidence Boundary

This analysis depends on the RQ-ISO-001 evidence chain consolidated through the Information Governance taxonomy and Isolation Capability Matrix revisions v0.2–v0.6.

The bounded survey explicitly examined representative systems from the following architecture families:

- multiphysics/component frameworks: SOFA, MOOSE;
- coupling and visualization frameworks: preCICE, VTK;
- model-exchange and systems-engineering standards/frameworks: FMI, OpenMDAO, Modelica;
- field-oriented simulation frameworks: OpenFOAM;
- distributed simulation standards: HLA;
- data-centric middleware: DDS;
- digital-twin/IoT property models: IoT Plug and Play / DTDL;
- digital-twin core/reference architectures: NIST/IIC Digital Twin Core;
- industrial digital-twin semantic models: Asset Administration Shell;
- safety-critical portable-component/data architectures: FACE; and
- safety-relevant platform state-management architectures: AUTOSAR Adaptive State Management.

The survey also screened ISO/IEC 30188:2026 and ISO/TS 25271:2026 as current standards-watch items where public abstract material was insufficient for detailed equivalence scoring.

The survey does not prove that no equivalent architecture exists. It establishes only that the selected bounded evidence set did not identify a direct prior-art architecture that jointly and explicitly requires the full surviving combination defined below.

---

## 3. Established Prior Art

The evidence survey removes a large set of broad architectural claims from consideration as ThermoCore-specific contributions.

The following are established prior art in one or more reviewed systems:

- modular cores and optional components;
- plugin and runtime-selection mechanisms;
- multiple representations of shared information;
- read-only or consumer-only access patterns;
- single-writer and exclusive update ownership;
- ownership-preserving communication;
- explicit ownership-transfer mechanisms;
- publish/subscribe decoupling;
- producer-versus-consumer role separation;
- request-based influence over centrally managed state;
- normative interface and connector governance;
- standardized semantic information models;
- digital-twin core/application separation;
- component-local persistent state;
- module-specific verification, validation, SQA, and conformance records; and
- extension without modification of central factory/selection logic.

Accordingly, RQ-ISO-001 shall not be framed as a claim that ThermoCore invented state ownership, read/write separation, modularity, semantic governance, extension boundaries, or central state management.

HLA provides particularly strong prior art for exclusive update authority, subscription without ownership transfer, formal object-model semantics, and modular simulation composition. AUTOSAR State Management provides strong prior art for a defined architectural state-management responsibility influenced through controlled request interfaces. FACE provides strong prior art for semantic data governance plus modular conformance. AAS provides strong prior art for standardized extensible digital-twin semantics.

These findings materially narrow the valid candidate gap.

---

## 4. Surviving Candidate Property

The bounded survey leaves one narrower architectural combination insufficiently matched by the reviewed prior art:

> **Fixed Semantic/Core-State Boundary under Ordinary Extension** — a framework defines an authoritative physical-domain State whose semantic identity and evolution responsibility remain fixed at the Core level; ordinary representations and extensions may consume that State, derive information from it, contribute through declared coupling boundaries, and own mechanism-specific local state, but they do not thereby acquire authority to redefine the State's semantic identity, owner, mandatory Core-State membership, or the completeness conditions of the Framework Core.

This candidate contains four coupled invariants.

### 4.1 Fixed State Semantic Authority

The authoritative physical-domain State is not merely a writable data object. Its semantic identity is defined by the framework responsibility that owns the physical-domain computation.

A component that consumes, renders, maps, mirrors, transports, or derives from the State does not gain authority to redefine what that State means.

### 4.2 Non-Promotion of Extension-local State

An ordinary extension may own persistent state required by its own mechanism.

The existence of that extension-local state does not automatically expand the mandatory persistent Core State. Promotion into Core State would require a separate framework-level justification rather than occurring as a side effect of adding a mechanism.

### 4.3 Core Completeness Invariant

The Framework Core remains complete when ordinary optional extensions and representation consumers are absent.

Extensions may add capability, but ordinary extension presence is not part of the definition of Core completeness.

### 4.4 Authority-preserving Participation

Communication, observation, requests, source contributions, property updates, or other declared interactions do not transfer semantic authority unless the architecture explicitly redefines the Core responsibility itself.

This distinguishes architectural authority from technical write capability in a particular implementation.

---

## 5. Why This Candidate Is Not Already Covered by the Reviewed Prior Art

The reviewed systems contain many individual pieces of the candidate combination, but their governing purpose differs at the critical boundary.

HLA preserves update ownership across communication, but attribute ownership is transferable and the federation object model is intentionally extensible. Its ownership model therefore does not establish a permanent prohibition against ordinary participants changing the shared model's semantic scope.

DDS provides exclusive DataWriter ownership and consumption without ownership transfer, but it governs communication ownership rather than physical-domain State semantics, Core-State membership, or Framework completeness.

IoT Plug and Play distinguishes reported and writable properties, but property access roles are not equivalent to a fixed simulation-state semantic authority boundary.

Modelica provides strong normative connector and equation governance, but model components intentionally contribute equations to the complete physical model. Numerical state selection is not a rule preventing optional mechanisms from altering the model's semantic state space.

OpenFOAM provides runtime-selected models and shared fields, but optional models are commonly intended to add sources, constrain equations, or correct fields. This is materially different from a rule that ordinary extensions cannot redefine Core-State semantic authority.

AAS and FACE provide strong semantic governance, but both support governed extension of domain information models. Their semantic extensibility does not establish non-promotion of extension-local physical state into a fixed Core State.

AUTOSAR State Management fixes a state-management responsibility, but the operational state model is project-specific and may be influenced by project-specific control applications. It therefore does not establish a fixed physical-domain State identity plus non-extensible Core-State membership.

The current candidate gap is therefore not the absence of any individual mechanism. It is the apparent absence, within the bounded survey, of the **joint architectural invariant** that combines fixed physical-state semantic authority, extension-local-state non-promotion, and Core completeness under optional extension absence.

---

## 6. Candidate Research Gap

The evidence supports the following bounded candidate formulation:

> Existing simulation, middleware, digital-twin, semantic-model, and safety-relevant component architectures provide mature mechanisms for modularity, state ownership, semantic governance, controlled state influence, and extension. Within the bounded RQ-ISO-001 survey, however, no reviewed architecture was found that jointly and explicitly preserves a fixed physical-domain State semantic authority and Core-State membership while allowing ordinary extensions and representations to participate without acquiring authority to redefine that State or make Core completeness depend on their presence.

This is an **evidence-supported Research Gap candidate**, not a novelty finding.

The candidate remains valid only if it is kept narrower than the established prior art and if later work demonstrates that the boundary has observable engineering consequences rather than being only a documentation convention.

---

## 7. Research Significance Test

A research gap is useful only if the missing architectural property has consequences that can be evaluated.

RQ-ISO-001 therefore should not ask only whether the boundary can be written as a specification rule. It should test whether enforcing the boundary changes system evolution in measurable ways.

Three consequence classes are currently justified for investigation.

### 7.1 Mandatory State-growth Impact

Question:

> When optional mechanisms are added, does a fixed semantic/Core-State boundary reduce unnecessary growth of mandatory persistent Core State compared with a more permissive extension model?

Candidate measurable outcomes include:

- number of mandatory Core-State fields;
- bytes of mandatory persistent Core State per simulation element;
- number of extension-specific quantities promoted into Core State; and
- number of Core semantic definitions changed by an extension.

### 7.2 Core Modification Impact

Question:

> When meaningful extensions are added or removed, does the boundary reduce changes required to Core responsibilities and Core interfaces?

Candidate measurable outcomes include:

- Core files/components changed;
- Core normative requirements changed;
- Core interfaces changed;
- Core state schema changes; and
- extension-only changes that remain outside the Core.

### 7.3 Revalidation Impact

Question:

> Does authority isolation provide a justified reduction in the set of Core verification/validation evidence that must be repeated after an extension-only change?

Candidate measurable outcomes include:

- number of Core requirements affected by a change;
- number of Core Verification cases requiring repetition;
- number of Core Validation records invalidated by an extension-only change; and
- amount of evidence that remains applicable because Core semantic authority is unchanged.

A reduction shall not be assumed. The evaluation must permit the outcome that the boundary provides no meaningful revalidation advantage.

---

## 8. Falsifiable Research Hypotheses

The following hypotheses are candidates for later evaluation. They are not findings of this document.

### H-ISO-01 — State-growth Isolation

For a controlled set of optional mechanisms, an architecture enforcing the fixed semantic/Core-State boundary will require fewer extension-specific quantities to become mandatory Core State than a permissive architecture in which extensions may enlarge the shared Core State directly.

### H-ISO-02 — Core-change Isolation

For the same controlled extensions, the enforcing architecture will require fewer changes to Core state semantics, Core responsibilities, and Core interfaces than the permissive comparator.

### H-ISO-03 — Revalidation-scope Isolation

For extension-only changes that preserve the declared Core boundary, the enforcing architecture will invalidate a smaller justified subset of Core verification/validation evidence than the permissive comparator.

These hypotheses shall be rejected, narrowed, or reclassified if controlled evaluation does not support them.

---

## 9. Candidate Evaluation Design

A later Evaluation task should compare at least two architecture conditions under the same extension scenarios.

### 9.1 Restricted condition

Use the ThermoCore-style boundary:

- one authoritative Core State semantic owner;
- representation consumers cannot redefine State semantics;
- extension-local persistent state remains extension-owned;
- ordinary extensions may couple only through declared boundaries; and
- Core completeness does not depend on optional extension presence.

### 9.2 Permissive comparator

Use a deliberately permissive but still modular architecture in which an extension may add fields directly to the shared Core State or alter the shared state schema when convenient.

The comparator need not represent a specific existing framework. Its purpose is to isolate the consequence of the authority rule while holding other engineering conditions as constant as practical.

### 9.3 Controlled extension scenarios

The evaluation should include progressively stronger cases, for example:

1. a pure derived Representation Consumer requiring no persistent local state;
2. an energy/source extension with extension-owned persistent state;
3. a stateful physical mechanism that interacts strongly with the thermodynamic computation while remaining within the declared ordinary-extension boundary; and
4. a counterexample mechanism that genuinely changes the governing Core physics and therefore should force Core revision.

The fourth case is essential. A useful boundary must distinguish legitimate extension isolation from cases where refusing Core modification would be physically or architecturally incorrect.

---

## 10. Falsification and Reclassification Conditions

The candidate Research Gap shall be rejected or narrowed if any of the following occurs:

1. a prior architecture is identified that explicitly and jointly satisfies the full fixed semantic/Core-State boundary combination before ThermoCore;
2. the apparent difference is shown to be only terminology for an already established architecture property;
3. controlled extensions produce no meaningful difference in mandatory state growth, Core modification impact, or justified revalidation scope;
4. enforcing the boundary requires hidden Core changes that merely move coupling complexity elsewhere;
5. realistic mechanisms routinely require promotion of extension-local state into Core State, making the non-promotion rule impractical; or
6. the boundary prevents correct physical coupling in cases that should remain ordinary extensions.

If prior art satisfies the architecture property but ThermoCore provides a clearer formalization, the contribution shall be reframed as integration/formalization rather than novelty.

If the architecture property is distinct but produces no measurable engineering consequence, it shall be treated as a specification/governance style rather than a research contribution.

---

## 11. Claims Not Supported

This analysis does not support claims that:

- ThermoCore invented state ownership or single-writer semantics;
- ThermoCore invented read/write separation;
- ThermoCore invented modular or optional extensions;
- ThermoCore is the first framework with a central state owner;
- ThermoCore is the first framework with semantic information governance;
- ThermoCore is the first framework to separate Core and application concerns;
- the bounded survey proves global absence of prior art;
- the candidate boundary is optimal or universally applicable;
- extension-local state should never become Core State;
- the candidate boundary always reduces state size, implementation effort, or validation cost;
- v1.0.0 already validates the RQ-ISO-001 candidate contribution; or
- the current Framework Specification shall be changed because this candidate exists.

Any stronger claim requires additional evidence.

---

## 12. Relationship to ThermoCore v1.0.0

ThermoCore v1.0.0 predates completion of this RQ-ISO-001 research line and remains a fixed published baseline.

The v1.0.0 architecture supplies an existing case in which strong ownership and extension boundaries are already documented, but the existence of those boundaries does not by itself prove the candidate research contribution.

RQ-ISO-001 is therefore a post-v1.0 research activity that asks whether a property already present in the architecture is independently supported as a meaningful research distinction and whether its consequences can be measured.

No conclusion from this document retroactively alters the claims of v1.0.0.

---

## 13. Current Classification

| Item | Classification |
|---|---|
| Broad modularity / extension mechanisms | Established prior art |
| Single-writer / update ownership | Established prior art |
| Read/consume without ownership transfer | Established prior art |
| Central state-management responsibility | Established prior art |
| Semantic information-model governance | Established prior art |
| Modular conformance / V&V separation | Established prior art |
| Fixed physical-State semantic authority under ordinary extension | Evidence-supported candidate distinction |
| Non-promotion of extension-local state into mandatory Core State | Evidence-supported candidate distinction |
| Core completeness invariant under ordinary extension absence | Evidence-supported candidate distinction |
| Reduced mandatory state growth | Unverified consequence hypothesis |
| Reduced Core modification impact | Unverified consequence hypothesis |
| Reduced revalidation impact | Unverified consequence hypothesis |
| Research Gap | Candidate supported by bounded survey; not yet validated as contribution |
| Novelty | Not established |

---

## 14. Conclusion

The RQ-ISO-001 evidence survey has sufficiently narrowed the original broad isolation question to stop generic capability-matrix expansion.

The remaining research candidate is not ownership, modularity, read-only access, state management, semantic modeling, or extension by itself. It is the joint preservation of a **fixed semantic/Core-State boundary under ordinary extension**.

The bounded evidence set did not identify a direct architectural equivalent that jointly enforces the full combination. This justifies treating the property as an evidence-supported Research Gap candidate.

The next research threshold is not another broad literature sweep. It is consequence testing.

RQ-ISO-001 should proceed only if the candidate can be evaluated against falsifiable hypotheses concerning mandatory state growth, Core modification impact, and justified revalidation scope. If those consequences are not supported, the candidate shall be narrowed or reclassified rather than promoted into a stronger contribution claim.
