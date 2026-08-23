# Compositional Extension Admissibility Evidence Matrix

Version: 0.1  
Status: UNDER SURVEY — Bounded Prior-Art / Aggregate-Null Evidence Pass  
Research Question: RQ-ECA-001 — Compositional Extension Admissibility  
Tracking Issue: #129  
Date: 2026-08-23

---

## 1. Objective

This matrix evaluates direct prior art relevant to **composition-level admissibility** of independently defined simulation components / mechanisms and stress-tests whether RQ-ECA-001 requires an independent architecture criterion beyond the already-completed ThermoCore boundaries.

The question is not whether component composition, dependency analysis, algebraic-loop detection, multiphysics coupling, power-conserving interconnection, or system-level connection validation already exist. Those are mature areas.

The narrower ThermoCore question is:

> If Extension A and Extension B are each individually admissible as ordinary/Core-preserving extensions, can their composition create a new semantic condition that is not already resolved by (1) re-evaluating the aggregate mechanism/formulation through RQ-EFM-001, (2) applying RQ-ISO-001 to authority/non-promotion, and (3) applying existing Framework/conformance properties to communication and conservative accounting?

This document is non-normative. It does not establish novelty, priority, superiority, Framework Specification requirements, or implementation requirements.

---

## 2. Research Baseline Being Tested

RQ-ECA-001 sits after two completed research boundaries.

### 2.1 RQ-EFM-001

RQ-EFM-001 evaluates whether a mechanism/formulation remains ordinary/Core-preserving by testing formulation-relative thermodynamic sufficiency.

If the selected thermodynamic formulation becomes incomplete unless additional information is promoted into authoritative Thermodynamic State or the governing formulation is revised, the case is not an ordinary extension under the unrevised Core.

### 2.2 RQ-ISO-001

RQ-ISO-001 governs state authority after ordinary-extension status and information categories are accepted.

Extension-local persistent quantities do not become mandatory Core State merely because an extension participates in state evolution or interacts strongly with the Core.

### 2.3 Reclassified engineering properties

Two later research lines were closed as independent contribution claims:

- RQ-FCI-001 -> **Formulation Change Containment Property**;
- RQ-CEX-001 -> **Conservative Exchange Accounting Property**.

The latter is especially relevant when composition creates several energy-bearing contribution paths, but it remains an engineering/conformance property rather than a separate research contribution.

---

## 3. Strong Null Hypothesis

The strongest null explanation for RQ-ECA-001 is:

```text
Extension A individually D0
Extension B individually D0
        |
        v
compose A + B
        |
        v
re-evaluate A+B as one aggregate mechanism/formulation
        |
        +--> RQ-EFM-001: is combined thermodynamic formulation complete?
        |
        +--> RQ-ISO-001: are state authority and non-promotion preserved?
        |
        +--> existing interface/accounting/conformance rules
```

If this sequence resolves all meaningful architecture cases, then RQ-ECA-001 does not survive as an independent Research Gap.

The evidence pass therefore seeks both:

- positive evidence that composition can create new system-level conditions not visible locally; and
- evidence that established system-level re-evaluation already handles that fact as a normal property of composition.

---

## 4. Candidate Interaction Dimensions

| ID | Candidate dimension | RQ-ECA question |
|---|---|---|
| ECA-D1 | Extension-to-extension data dependency | Can B depend on A even though both are individually admissible? |
| ECA-D2 | Property / configuration responsibility overlap | Can two extensions create ambiguous or conflicting responsibility over the same property/configuration information? |
| ECA-D3 | Source / exchange contribution interaction | Can individually valid contributions duplicate, cancel, omit, or reinterpret the same physical contribution when combined? |
| ECA-D4 | Extension-local state dependency | Can one extension's local state become necessary to another extension or to thermodynamic closure? |
| ECA-D5 | Feedback / cyclic dependency | Can composition create a feedback or cycle absent from either extension alone? |
| ECA-D6 | Combined thermodynamic closure sufficiency | Can A+B make the selected Thermodynamic State / exchange set insufficient although A and B are each sufficient separately? |
| ECA-D7 | Ownership / authority conflict | Can composition create conflicting claims over authoritative state or responsibilities? |
| ECA-D8 | Scope identity | Does A+B remain the same declared thermodynamic scope, or has the aggregate governing scope materially expanded? |

Evidence labels:

- **Established direct antecedent** — the composition phenomenon is explicit in reviewed primary/official evidence.
- **Strong partial antecedent** — the phenomenon is explicit, but not with ThermoCore's exact thermodynamic/authority semantics.
- **Already routed by prior ThermoCore result** — the dimension is real but is already governed by RQ-EFM-001, RQ-ISO-001, or a closed/reclassified property.
- **Not established in reviewed evidence** — no sufficient evidence in this bounded pass; not a claim of absence.

---

## 5. Cross-Source Summary

| Source family | Local vs composed validity | Dependencies / cycles | Connection / responsibility ambiguity | Explicit compositional closure | Main RQ-ECA pressure |
|---|---|---|---|---|---|
| Modelica Language Specification | **Established direct antecedent** | Structural singularity can emerge from combinations | Connection semantics explicit | Local balance gives global equation-count guarantee, but not global nonsingularity | Very strong evidence that local validity does not imply all global validity properties |
| MOOSE Materials + MultiApps | **Established direct antecedent** | Material cycles detected; MultiApp coupling iterated when needed | Producer/consumer and Transfers explicit | No universal closure theorem; coupling mode selected at composed-system level | Strong dependency / feedback / responsibility antecedent |
| preCICE multi-coupling | **Established direct antecedent** | Circular orchestration dependencies documented; multiple strong interactions require multi-coupling | Participant/data exchange graph explicit | Fully implicit multi-coupling provides aggregate treatment | Strong evidence that pairwise-valid coupling configurations may need system-level treatment |
| FMI 3.0.2 + SSP 2.0.1 | **Established direct antecedent** | Cross-FMU algebraic loops detected from dependencies | SSP requires unambiguous system-level data flow | No thermodynamic closure theorem | Strong system-composition validation antecedent |
| Port-Hamiltonian / Dirac structures | **Established direct antecedent** | Algebraic constraints arise in composition | Port variables define interconnection | **Explicit closure:** power-conserving interconnection of PHS yields PHS under formal conditions | Strongest formal compositionality antecedent |

---

## 6. Evidence Record ECA-01 — Modelica Balanced Models and Connection Composition

### 6.1 Primary source

- Modelica Language Specification, current master / 3.8-dev, sections 4.8, 9, and 15:  
  https://specification.modelica.org/master/MLS.pdf

### 6.2 Relevant findings

Modelica defines **local balance** for a model/block in terms of local unknown count versus local equation size and requires non-partial model/block classes to be locally balanced.

The specification gives a strong composition-level guarantee that simulation models and blocks are globally balanced when the used non-partial classes satisfy the local-balance restrictions.

However, the same section explicitly warns that local structural properties do not imply every global property. In the `Circuit` example, Modelica states that particular combinations of locally non-singular models may still produce a globally singular model.

This is a direct antecedent to the general proposition:

```text
local component validity
    !=
complete composed-system validity
```

### 6.3 Stream / flow composition

Modelica stream connectors further demonstrate that connection semantics may generate system-level conservation equations.

The stream-connection set is treated as an infinitesimal control volume and its generated equations correspond to mass and energy conservation. Thus a component can remain internally separate while composition generates physically meaningful global constraints.

### 6.4 RQ-ECA significance

Modelica strongly excludes any broad RQ-ECA claim such as:

> It is novel to recognize that individually valid components can become invalid or constrained when composed.

That is established prior art.

Modelica also shows that **some properties are compositional under explicit local restrictions while others still require aggregate analysis**.

This is conceptually very close to the RQ-ECA distinction between:

- an ordinary-extension status established locally; and
- a new condition produced by composition.

### 6.5 Limitation relative to ThermoCore

Modelica's balanced-model result concerns equation count / structural modeling semantics, not ThermoCore's specific distinctions among:

- authoritative Thermodynamic State;
- extension-local state;
- Material Definition / property responsibility;
- ordinary-extension admissibility;
- formulation-relative thermodynamic completeness.

Therefore Modelica is not a full antecedent to the ThermoCore decision procedure.

**Classification:** `DIRECT COMPOSITIONALITY / NON-COMPOSITIONALITY ANTECEDENT — VERY STRONG`.

---

## 7. Evidence Record ECA-02 — MOOSE Materials Dependency Graph

### 7.1 Primary / official sources

- MOOSE Materials System:  
  https://mooseframework.inl.gov/releases/moose/2024-03-08/syntax/Materials/
- MOOSE MultiApps tutorial / coupling model:  
  https://mooseframework.inl.gov/moose/getting_started/examples_and_tutorials/tutorial02_multiapps/presentation/index.html
- MOOSE fixed-point iteration algorithms:  
  https://mooseframework.inl.gov/syntax/Executioner/FixedPointAlgorithms/

### 7.2 Material producer / consumer semantics

MOOSE Materials use an explicit producer/consumer relationship:

- one Material produces a property;
- kernels, boundary conditions, or other Materials may consume it;
- Materials are sorted so a producer executes before a consumer;
- a cyclic dependency among Materials is detected and reported as an error.

This is a direct antecedent for ECA-D1 and ECA-D5.

Acyclic dependency can be made explicit and ordered; cyclic dependency is a property of the composed dependency graph, not of either Material in isolation.

### 7.3 MultiApp composition

MOOSE distinguishes multiple composition/coupling regimes:

- loosely coupled;
- tightly coupled / Picard fixed point;
- fully coupled.

MultiApps and Transfers exchange information between separately solved applications. Tight coupling iterates the applications and transfers until convergence.

This shows that once individually defined physics are composed, the aggregate system may need a different coupling treatment even though the standalone models remain individually valid.

### 7.4 RQ-ECA significance

MOOSE strongly pressures the following broad candidates:

- extension-to-extension dependency discovery;
- dependency ordering;
- cycle detection;
- explicit cross-component data transfer;
- aggregate feedback iteration.

None can be presented as novel ThermoCore concepts.

### 7.5 Important boundary

MOOSE also helps preserve the distinction between **semantic composition** and **numerical coupling**.

A fixed-point iteration requirement does not by itself imply a ThermoCore Core/State semantic change. If all required state and exchanges are already explicit and the selected thermodynamic formulation remains complete, the need for iterative coupling is numerical/orchestration behavior rather than evidence of a new Framework authority rule.

**Classification:** `DIRECT DEPENDENCY / CYCLE / MULTIPHYSICS-COMPOSITION ANTECEDENT — VERY STRONG`.

---

## 8. Evidence Record ECA-03 — preCICE Multi-Participant Composition

### 8.1 Primary source

- preCICE multi-coupling configuration:  
  https://precice.org/configuration-coupling-multi

Supporting official pages:

- coupling scheme configuration:  
  https://precice.org/configuration-coupling
- multi-participant tutorials:  
  https://precice.org/tutorials-multiple-perpendicular-flaps

### 8.2 Findings

preCICE supports coupling more than two participants through either:

1. composition of multiple two-participant coupling schemes; or
2. a fully implicit multi-coupling scheme.

The documentation gives explicit restrictions and failure modes at the composed-system level.

Examples include:

- combining more than one implicit bi-coupling scheme is not generally allowed;
- multiple strong interactions should use fully implicit multi-coupling;
- circular serial dependencies can produce deadlock even though each pairwise coupling specification is meaningful in isolation.

### 8.3 RQ-ECA significance

This is direct prior art for the proposition that:

```text
pairwise-valid or pairwise-meaningful couplings
    do not automatically imply
valid aggregate multi-participant coupling
```

It also shows that an aggregate coupling graph may require a distinct global treatment.

### 8.4 Important limitation

The documented failure modes are mainly orchestration / numerical coupling conditions:

- serial versus parallel execution;
- implicit versus explicit coupling;
- convergence;
- deadlock.

RQ-ECA-001 shall not convert these into Framework semantic novelty unless the interaction changes thermodynamic formulation completeness or authority.

Thus preCICE creates strong prior-art pressure while simultaneously reinforcing the need to keep numerical composition distinct from architecture admissibility.

**Classification:** `DIRECT MULTI-COUPLING COMPOSITION ANTECEDENT — VERY STRONG`.

---

## 9. Evidence Record ECA-04 — FMI 3.0.2 Dependency / Algebraic-Loop Semantics

### 9.1 Primary source

- Functional Mock-up Interface Specification 3.0.2:  
  https://fmi-standard.org/docs/3.0.2/

### 9.2 Findings

FMI allows FMUs to declare variable dependencies in `ModelStructure`.

The specification explicitly states that these dependencies support detection and classification of **algebraic loops across inputs and outputs when connecting FMUs**.

When FMUs are connected, loop structures may create linear or nonlinear algebraic equation systems even though the individual FMUs are independently defined artifacts.

This is direct system-composition evidence for ECA-D1 and ECA-D5.

### 9.3 RQ-ECA significance

FMI strongly falsifies a broad claim that cross-component dependencies emerging only after composition are an unrecognized problem.

It also provides an important architecture analogy:

- local components expose dependency metadata;
- the importer / composed environment evaluates cross-component dependency structure;
- aggregate loops are properties of the connected system.

### 9.4 Limitation

FMI dependency semantics do not classify whether a dependency is:

- Thermodynamic State;
- extension-local state;
- Material Definition;
- source contribution;
- thermodynamic closure coordinate.

Therefore FMI does not directly resolve ThermoCore's formulation-relative classification.

**Classification:** `DIRECT CROSS-COMPONENT DEPENDENCY / LOOP ANTECEDENT — STRONG`.

---

## 10. Evidence Record ECA-05 — SSP 2.0.1 System-Level Connection Constraints

### 10.1 Primary source

- System Structure and Parameterization Standard 2.0.1:  
  https://ssp-standard.org/docs/main/

### 10.2 Findings

SSP defines systems containing components / nested systems plus connections among connectors.

Its system-level connection rules require implementations to preserve allowed connection forms and unambiguous data flow.

In particular, SSP requires that multiple inbound connections do not ambiguously drive connector kinds whose semantics imply one inbound source.

### 10.3 RQ-ECA significance

SSP provides a direct engineering antecedent for ECA-D2:

> independently valid component interfaces do not guarantee that an arbitrary aggregate connection graph has unambiguous responsibility or data flow.

This is not identical to ThermoCore property/configuration ownership, but it strongly pressures any generic claim about composition-time responsibility conflict detection.

### 10.4 Limitation

SSP intentionally delegates many detailed semantics to the underlying component/modeling technology.

It does not establish ThermoCore's specific State/Configuration/Representation ownership model.

**Classification:** `SYSTEM-LEVEL CONNECTION-RESPONSIBILITY ANTECEDENT — STRONG PARTIAL`.

---

## 11. Evidence Record ECA-06 — Port-Hamiltonian / Dirac-Structure Compositional Closure

### 11.1 Author-hosted academic source

- Cervera, van der Schaft, Baños, *Interconnection of port-Hamiltonian systems and composition of Dirac structures*, Automatica 43(2), 2007, University of Twente record:  
  https://research.utwente.nl/en/publications/interconnection-of-port-hamiltonian-systems-and-composition-of-di/

Supporting author-hosted material:

- *A Port-Hamiltonian Approach...*, chapter on power-conserving interconnection:  
  https://ris.utwente.nl/ws/portalfiles/portal/6041264/thesis_Villegas.pdf

### 11.2 Findings

Port-Hamiltonian systems provide a formal compositionality result:

- subsystems possess their own state/energy functions;
- interconnection is expressed through power variables / Dirac structures;
- power-conserving interconnection of port-Hamiltonian systems yields another port-Hamiltonian system;
- the composed Dirac structure determines aggregate algebraic constraints.

This is a strong formal antecedent showing that a model class can define **conditions under which a property is preserved by composition**.

### 11.3 RQ-ECA significance

This eliminates any broad claim that defining a composition-preservation condition is novel in physical-system architecture.

It also demonstrates the correct structure of a compositional theorem:

```text
local members belong to class C
+ interconnection satisfies relation R
------------------------------------
composed system remains in class C
```

RQ-ECA-001 would need an independently justified ThermoCore-specific semantic predicate if it were to survive as a research contribution.

### 11.4 Limitation

Port-Hamiltonian compositional closure is about a formal physical model class and power-conserving interconnection.

It does not directly encode ThermoCore's ordinary-extension status, Material Definition responsibility, or state non-promotion rule.

**Classification:** `FORMAL COMPOSITIONAL-CLOSURE ANTECEDENT — VERY STRONG`.

---

## 12. Dimension-by-Dimension Assessment

### 12.1 ECA-D1 — Extension-to-extension data dependency

**External evidence:** Established directly.

- MOOSE Materials explicitly form producer/consumer dependency graphs.
- FMI exposes cross-component variable dependencies.
- SSP defines component connection structure.

**ThermoCore routing:**

A declared dependency does not by itself create a Core change.

If the dependency is only communication between extension-owned/configuration quantities, ordinary extension semantics may remain intact.

If the dependency makes the selected thermodynamic closure incomplete without the depended-on quantity, RQ-EFM-001 applies.

**Disposition:** `ESTABLISHED PHENOMENON; NO INDEPENDENT ECA RULE YET`.

---

### 12.2 ECA-D2 — Property / configuration responsibility overlap

**External evidence:** Strong partial antecedent.

- MOOSE distinguishes property producers and consumers and manages dependency order.
- SSP rejects ambiguous multiple inbound dataflow to connector categories requiring unambiguous input semantics.

**ThermoCore routing:**

If two extensions attempt to redefine ownership of Material Definition or another Framework-governed category, this is a specification/conformance conflict rather than evidence of a new thermodynamic formulation criterion.

If they provide distinct contributions from which a property is computed by an explicitly owned responsibility, composition may be valid.

**Disposition:** `REAL COMPOSITION HAZARD; CURRENTLY LOOKS LIKE EXISTING OWNERSHIP / CONFORMANCE GOVERNANCE`.

---

### 12.3 ECA-D3 — Source / exchange contribution interaction

**External evidence:** Established broadly through Modelica conserving connections, preCICE exchange graphs, MOOSE forcing/property transfer, and prior RQ-CEX evidence.

**ThermoCore routing:**

Duplicate / omitted physical accounting belongs to the already-reclassified **Conservative Exchange Accounting Property** unless it exposes formulation incompleteness.

If the combined contributions change what the governing thermodynamic formulation must represent, RQ-EFM-001 applies.

**Disposition:** `ALREADY ROUTED; DOES NOT CURRENTLY SUPPORT AN INDEPENDENT ECA GAP`.

---

### 12.4 ECA-D4 — Extension-local state dependency

**External evidence:** Strong partial antecedent through FMI dependencies and MOOSE coupled fields / transferred variables.

**ThermoCore routing:**

A depends-on relationship does not transfer ownership.

If Extension B can consume information derived from Extension A's local state without making that state an authoritative thermodynamic coordinate, RQ-ISO-001 ownership can remain intact.

If the selected thermodynamic formulation cannot be closed without that state, RQ-EFM-001 reclassifies the aggregate case.

**Disposition:** `CURRENTLY FULLY ROUTABLE TO RQ-ISO + RQ-EFM`.

---

### 12.5 ECA-D5 — Feedback / cyclic dependency

**External evidence:** Established directly.

- MOOSE detects cyclic Material dependencies.
- FMI supports algebraic-loop detection across connected FMUs.
- preCICE documents circular multi-participant dependencies and multi-coupling requirements.

**ThermoCore routing:**

A cycle may be:

1. purely numerical/orchestration — requires iteration, scheduling, or coupled solve but no Framework semantic change;
2. semantic — exposes a previously hidden state/closure dependency, which routes to RQ-EFM-001 or RQ-ISO-001.

**Disposition:** `ESTABLISHED PHENOMENON; SEMANTIC CASES ALREADY ROUTABLE`.

---

### 12.6 ECA-D6 — Combined thermodynamic closure sufficiency

**External evidence:** Strong conceptual antecedent.

Modelica explicitly demonstrates that local structural properties can fail to imply global nonsingularity, while port-Hamiltonian theory demonstrates that other properties can be preserved by composition under defined interconnection conditions.

This establishes the general need to evaluate an aggregate system property rather than infer it only from local components.

**ThermoCore routing:**

This dimension is almost exactly the RQ-EFM-001 aggregate-null path:

```text
A individually complete
B individually complete
A+B composed
        |
        v
Does authoritative Thermodynamic State + material/configuration + honest exchanges
remain sufficient to close the selected aggregate thermodynamic formulation?
```

If yes -> D0 remains possible.

If no -> D1 / formulation-Core revision pressure under RQ-EFM-001.

**Disposition:** `STRONGEST NULL-HYPOTHESIS SUPPORT — CURRENTLY NO DISTINCT ECA PREDICATE IDENTIFIED`.

---

### 12.7 ECA-D7 — Ownership / authority conflict

**External evidence:** Partial analogues exist through producer/consumer and connection-direction semantics.

**ThermoCore-specific evidence:** Already directly governed by RQ-ISO-001 and normative ownership rules.

If two extensions both attempt to acquire authority over mandatory Core State or Core state evolution, the composition is non-conforming under existing architecture; no new RQ-ECA principle is needed to discover the conflict.

**Disposition:** `ALREADY ROUTED TO RQ-ISO / FRAMEWORK CONFORMANCE`.

---

### 12.8 ECA-D8 — Scope identity

**External evidence:** No reviewed source provides the exact ThermoCore distinction between ordinary-extension composition under a fixed declared thermodynamic scope and a true thermodynamic scope expansion.

However, RQ-EFM-001 already treats formulation completeness as scope-relative.

Therefore if A+B introduces mass transport, pressure/compressibility, new governing conservation responsibilities, or another physical requirement beyond the declared formulation, the aggregate is re-evaluated as a changed formulation/scope rather than protected by the fact that A and B were individually D0.

**Disposition:** `THERMOCORE-SPECIFIC SEMANTIC DISTINCTION, BUT ALREADY PROVIDED BY RQ-EFM-001`.

---

## 13. Aggregate-Mechanism Null Test

### 13.1 Null procedure

For any composition `A + B + ...`:

1. preserve each mechanism's declared physical meaning;
2. construct the aggregate physical dependency / exchange / state relation;
3. evaluate the aggregate thermodynamic formulation through RQ-EFM-001;
4. if aggregate D0, evaluate ownership/non-promotion through RQ-ISO-001;
5. apply applicable Framework Interface / Conformance / Conservative Exchange Accounting constraints;
6. treat solver iteration, timestep synchronization, execution ordering, deadlock, and convergence separately unless they expose a semantic dependency that changes steps 3 or 4.

### 13.2 First-pass result

Every candidate ECA dimension currently maps to one of four categories:

```text
A. formulation / closure sufficiency
   -> RQ-EFM-001

B. state / authority / non-promotion
   -> RQ-ISO-001 + Framework Conformance

C. contribution accounting
   -> Conservative Exchange Accounting Property

D. scheduling / convergence / algebraic-loop solution / deadlock
   -> numerical or orchestration concern unless it changes A or B
```

No fifth independently necessary architecture category has yet been identified.

### 13.3 Preliminary null disposition

```text
Aggregate-mechanism null hypothesis:
STRONGLY SUPPORTED BY FIRST EVIDENCE PASS
```

This is not yet the final RQ-ECA closure decision because the aggregate-null routing should be stress-tested on matched ThermoCore scenarios before closure.

---

## 14. Candidate Scenario Routing for v0.2 Stress Test

The following scenarios are recommended for a focused v0.2 direct stress test.

### ECA-S0 — Orthogonal composition control

Extension A and B have disjoint local state, properties, and exchanges.

Expected routing:

```text
A = D0
B = D0
A+B aggregate = D0
no authority conflict
```

Purpose: verify that composition is not rejected merely because more than one extension is present.

### ECA-S1 — Declared acyclic property dependency

A supplies a derived extension property consumed by B.

The property dependency is explicit and does not create new thermodynamic closure requirements.

Expected routing:

```text
aggregate closure sufficient -> RQ-EFM D0
ownership preserved -> RQ-ISO compliant
```

Purpose: separate valid extension composition from forbidden property-owner duplication.

### ECA-S2 — Feedback cycle requiring iteration but not new state semantics

A depends on a B output and B depends on an A output; all required quantities remain explicit and the selected thermodynamic state-space is complete.

Expected routing:

```text
architecture may remain D0
numerical / orchestration coupling becomes stronger
```

Purpose: ensure ECA does not misclassify solver iteration as Core revision.

### ECA-S3 — Composition-induced closure dependency

A and B are individually D0, but their combined physical law makes an extension-local quantity necessary to determine thermodynamic evolution uniquely.

Expected routing:

```text
aggregate Test C / Test U fails
-> RQ-EFM D1 pressure
```

Purpose: direct test of whether aggregate RQ-EFM is sufficient.

### ECA-S4 — State-authority conflict

A and B each attempt to define/write the same mandatory Core thermodynamic quantity or to promote their own local state as authoritative Core State.

Expected routing:

```text
RQ-ISO / Framework Conformance violation
```

Purpose: verify no new ECA authority rule is necessary.

### ECA-S5 — Duplicate physical contribution

A and B each account for the same physical energy transfer through separate source/exchange paths.

Expected routing:

```text
Conservative Exchange Accounting Property violation
```

Purpose: verify accounting conflict is not relabeled as independent compositional research.

### ECA-S6 — True scope-expansion boundary control

A+B jointly introduce a governing physical responsibility outside the declared thermodynamic formulation, e.g. variable-mass reactive transport or another mechanism requiring revised governing state/equations.

Expected routing:

```text
aggregate RQ-EFM -> D1 / explicit revision or scope narrowing
```

Purpose: prevent pairwise D0 labels from hiding aggregate scope expansion.

---

## 15. Prior-Art Exclusions

The following shall not be claimed as novel RQ-ECA contributions on this evidence baseline:

- recognizing that locally valid components can form a globally invalid/singular composition;
- dependency-graph analysis among composed components;
- cycle detection;
- algebraic-loop detection across component boundaries;
- producer/consumer ordering;
- explicit multi-participant coupling graphs;
- choosing loose, tight, explicit, implicit, or fully coupled execution based on composed interactions;
- detecting ambiguous multi-source data flow at system connections;
- power-conserving compositional closure;
- the general proposition that a composed system may need re-evaluation at the aggregate level.

These are established prior art or directly anteceded engineering concepts.

---

## 16. Surviving Candidate, If Any

No independently defensible RQ-ECA architecture rule is established by v0.1.

The only surviving project-specific proposition is presently:

> **Aggregate Re-Admissibility Property** — individually ordinary extensions do not receive permanent admissibility immunity when composed; the aggregate mechanism/formulation must still satisfy the existing formulation-relative admissibility and authority rules for the actual combined scope.

This proposition is useful, but current evidence suggests it may be an **engineering/conformance consequence of RQ-EFM-001 + RQ-ISO-001** rather than a new research contribution.

Its independent status remains `UNDER STRESS TEST` pending v0.2 scenarios.

---

## 17. Research-Gap Readiness

```text
Generic composition/dependency problem:
ESTABLISHED PRIOR ART

Local-validity != all global-validity properties:
DIRECT ANTECEDENT ESTABLISHED

Explicit compositional closure under interconnection constraints:
DIRECT ANTECEDENT ESTABLISHED

ThermoCore aggregate-mechanism re-admissibility:
STRONGLY EXPLAINED BY RQ-EFM-001 + RQ-ISO-001

Independent RQ-ECA Research Gap:
NOT ESTABLISHED

Research Gap Analysis readiness:
NO-GO

Recommended next action:
FOCUSED v0.2 AGGREGATE-NULL / MATCHED-SCENARIO STRESS TEST

Novelty / priority:
NOT ESTABLISHED

Framework Specification impact:
NONE
```

---

## 18. Decision for Next Stage

Do **not** open an RQ-ECA Research Gap Analysis on v0.1.

The next evidence step should be narrow and falsification-oriented:

1. instantiate ECA-S0 through ECA-S6 as semantic scenario descriptions;
2. apply aggregate RQ-EFM Test C / Test U where relevant;
3. apply RQ-ISO authority rules where relevant;
4. apply Conservative Exchange Accounting only to duplicated/omitted physical contribution;
5. record whether any scenario requires a new decision criterion after those existing rules are exhausted.

If no scenario requires such a fifth criterion, RQ-ECA-001 should be closed and reclassified rather than narrowed indefinitely.

If a scenario survives only because it is numerical, scheduling, orchestration, or convergence-related, that does not establish an architecture research gap.

---

## 19. Current Disposition

**RQ-ECA-001 remains open only for a focused aggregate-null stress test.**

The first evidence pass finds strong prior art for both sides of the composition problem:

- composition can create system-level conditions absent locally; and
- mature formalisms already handle such conditions through aggregate dependency analysis, connection constraints, or explicit compositional closure rules.

Within ThermoCore, the evaluated semantic dimensions currently reduce to already-completed boundaries:

```text
combined thermodynamic completeness -> RQ-EFM-001
state authority / non-promotion     -> RQ-ISO-001
energy contribution accounting      -> Conservative Exchange Accounting Property
numerical / orchestration coupling   -> outside independent Framework-semantic claim
```

Therefore the present evidence places **strong negative pressure** on RQ-ECA-001 as an independent contribution while preserving a valid final falsification step.