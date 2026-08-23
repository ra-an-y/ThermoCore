# Compositional Extension Admissibility Evidence Matrix

Version: 0.2  
Status: COMPLETED — Aggregate-Null / Matched-Scenario Stress Test  
Research Question: RQ-ECA-001 — Compositional Extension Admissibility  
Tracking Issue: #131  
Date: 2026-08-23

---

## 1. Objective

This v0.2 matrix performs the final bounded falsification-oriented stress test for **RQ-ECA-001 — Compositional Extension Admissibility**.

The test asks whether any composition-specific ThermoCore architecture criterion remains necessary after independently admissible extensions are re-evaluated as one aggregate mechanism/formulation through the already-established project boundaries.

The strong null procedure is:

```text
individually admissible extensions
        |
        v
compose actual physical dependency / state / exchange relation
        |
        v
re-evaluate aggregate mechanism/formulation
        |
        +--> RQ-EFM-001: formulation / state-space / exchange sufficiency
        |
        +--> RQ-ISO-001: state authority / non-promotion
        |
        +--> existing Framework ownership / interface / conformance rules
        |
        +--> Conservative Exchange Accounting Property when physical contribution accounting is involved
        |
        +--> numerical / orchestration handling when only iteration, ordering, synchronization, or convergence changes
```

If every meaningful matched scenario is completely classified by this sequence, RQ-ECA-001 does not survive as an independent Research Gap.

This document is non-normative. It does not modify the Framework Specification, implementation, Verification, Validation, or Performance baseline.

---

## 2. Evidence Context Retained from v0.1

The v0.1 survey already established strong prior-art pressure on broad composition claims.

### 2.1 Modelica

The Modelica Language Specification distinguishes local and global structural properties. It provides local-balance rules that support global equation-count balance, while explicitly noting that specific combinations of locally non-singular models may still produce a globally singular model.

Primary source:

- Modelica Language Specification, current master / 3.8-dev  
  https://specification.modelica.org/master/MLS.pdf

Relevant implication:

```text
local validity does not imply every global validity property
```

This directly excludes novelty claims based only on recognizing composition-level failure.

### 2.2 MOOSE

MOOSE Materials are sorted through producer/consumer dependencies; cyclic Material dependencies are rejected. MultiApps distinguish loose, Picard/tight, and fully coupled treatment.

Official sources:

- https://mooseframework.inl.gov/releases/moose/2024-03-08/syntax/Materials/
- https://mooseframework.inl.gov/moose/getting_started/examples_and_tutorials/tutorial02_multiapps/presentation/index.html

Relevant implication:

```text
composition may create dependency or coupling requirements not visible in a standalone component
```

Dependency ordering and stronger numerical coupling are established engineering concerns.

### 2.3 preCICE

preCICE permits composition of multiple bi-coupling schemes, but documents aggregate restrictions:

- a second implicit coupling scheme for the same participant is forbidden;
- more than one strong interaction requires fully implicit multi-coupling;
- circular serial compositions can deadlock.

Official source:

- https://precice.org/configuration-coupling-multi

Relevant implication:

```text
pairwise-meaningful coupling does not imply valid aggregate coupling configuration
```

These examples are primarily numerical/orchestration antecedents, not ThermoCore state-authority rules.

### 2.4 FMI 3.0.2 and SSP

FMI variable dependencies support detection/classification of algebraic loops across connected FMUs. SSP requires system-level connection validity and unambiguous data flow, including restrictions on ambiguous multiple inbound connections.

Official sources:

- https://fmi-standard.org/docs/3.0.2/
- https://ssp-standard.org/docs/main/

Relevant implication:

```text
system composition is routinely re-evaluated at the aggregate dependency / connection level
```

### 2.5 Port-Hamiltonian / Dirac structures

Port-Hamiltonian systems provide a formal positive compositionality result: under power-conserving interconnection, the interconnected system remains port-Hamiltonian and its Dirac structure is obtained from subsystem composition.

Author / institutional source:

- Cervera, van der Schaft, Baños (2007), *Interconnection of port-Hamiltonian systems and composition of Dirac structures*  
  https://research.utwente.nl/en/publications/interconnection-of-port-hamiltonian-systems-and-composition-of-di/

Relevant implication:

```text
compositional closure can be established by applying explicit aggregate interconnection conditions
```

The general idea of a composition-preservation criterion is therefore established prior art.

---

## 3. Decision Categories Used in v0.2

The stress test does not introduce a new RQ-ECA decision layer in advance.

Each scenario must first route through the following existing categories.

| Category | Governing project rule |
|---|---|
| Aggregate thermodynamic closure / state-space / update sufficiency | RQ-EFM-001 |
| State authority, mandatory Core-State promotion, exclusive Core write responsibility | RQ-ISO-001 + Framework Conformance |
| Configuration / Representation / Interface ownership preservation | Existing Framework Specification / Conformance |
| Duplicate or omitted physical energy contribution | Conservative Exchange Accounting Property |
| Iteration, solver convergence, ordering, scheduling, synchronization, deadlock | Numerical / orchestration concern unless it changes semantic completeness or authority |
| True governing physical scope expansion | RQ-EFM-001 / explicit formulation-Core revision or scope narrowing |

The test asks whether a fifth independent architecture category is necessary after these are exhausted.

---

## 4. Matched-Scenario Matrix

| Scenario | Local A | Local B | Aggregate interaction | Same declared scope? | RQ-EFM | RQ-ISO / Conformance | CEX property | Numerical-only issue? | Independent ECA rule required? |
|---|---|---|---|---|---|---|---|---|---|
| ECA-S0 Orthogonal composition | D0 | D0 | No semantic cross-dependency | Yes | D0 | PASS | N/A | No | **NO** |
| ECA-S1 Declared acyclic property dependency | D0 | D0 | B consumes A-derived extension property | Yes | D0 if aggregate closure complete | PASS if ownership preserved | N/A | Ordering may exist | **NO** |
| ECA-S2 Feedback requiring stronger iteration | D0 | D0 | Explicit A↔B feedback, no hidden thermodynamic coordinate | Yes | D0 if C/U remain satisfied | PASS | N/A | Yes | **NO** |
| ECA-S3 Composition-induced closure insufficiency | D0 in isolated assumptions | D0 in isolated assumptions | Hidden/local quantity becomes necessary for aggregate closure/update | Yes initially, but unrevised formulation incomplete | **D1 pressure** | Apply after reclassification | N/A | Possibly, but not decisive | **NO** |
| ECA-S4 Ownership / responsibility conflict | D0 individually | D0 individually | Two extensions claim incompatible authority over same semantic information | Yes | D0 only if closure otherwise complete | **FAIL / non-conforming** | N/A | No | **NO** |
| ECA-S5 Duplicate physical contribution | D0 | D0 | Same physical transfer counted twice or omitted between paths | Yes | D0 may still hold structurally | PASS if state authority preserved | **FAIL** | Numerical error may follow | **NO** |
| ECA-S6 True aggregate scope expansion | D0 under bounded isolated scopes | D0 under bounded isolated scopes | Combined physics introduces new governing responsibility/state/equation requirement | **No** | **D1 / revision / scope narrowing** | Apply to revised scope | As applicable | May also require stronger solver | **NO** |

### Aggregate-null score

```text
Evaluated matched scenarios: 7
Scenarios requiring a new independent ECA architecture predicate: 0
Scenarios completely routable through existing ThermoCore boundaries: 7
```

Within this bounded semantic stress test, the aggregate-null survives all scenarios.

---

## 5. ECA-S0 — Orthogonal Composition Control

### 5.1 Construction

Extension A and Extension B are each individually D0 and have:

- distinct extension-local information;
- distinct physical contributions;
- no extension-to-extension dependency;
- no shared property authority;
- no hidden closure coordinate;
- no duplicated physical exchange.

### 5.2 Aggregate test

The aggregate introduces no new physical relation beyond the union of the two declared ordinary extensions.

RQ-EFM Test C / Test U remain satisfied because authoritative Thermodynamic State plus applicable configuration and honest exchanges remain sufficient.

RQ-ISO remains satisfied because neither extension acquires mandatory Core-State authority.

### 5.3 Result

```text
A = D0
B = D0
A+B = D0
```

No rule stating "multiple extensions require special admission" is justified.

**ECA-specific predicate required:** `NO`.

This is the false-positive control: composition alone must not cause rejection.

---

## 6. ECA-S1 — Declared Acyclic Extension-to-Extension Property Dependency

### 6.1 Construction

Extension A owns extension-local state or mechanism-local information `x_A` and derives an explicit property-like output `p_A`.

Extension B consumes `p_A` together with already-authorized Thermodynamic State / material information to compute its own contribution.

The dependency is:

```text
A.x_A -> A.p_A -> B
```

There is no reverse dependency and `p_A` is not silently reclassified as authoritative Thermodynamic State.

### 6.2 Aggregate closure

If `p_A` is supplied as a semantically explicit extension output and the aggregate thermodynamic formulation remains complete under the existing state/exchange set, RQ-EFM remains D0.

The fact that B depends on A does not itself require Core-State promotion.

### 6.3 Property / configuration responsibility test

This scenario directly tests the strongest possible surviving ECA-D2 concern.

Three cases are distinguishable without adding a new composition category:

1. `p_A` is an extension-owned derived/mechanism output: existing interface and ownership semantics govern it.
2. `p_A` is reusable Material Definition / Configuration: its ownership remains with the applicable configuration responsibility; communication does not transfer ownership.
3. `p_A` is actually an evolving physical quantity required for thermodynamic closure: it is not merely Configuration and must be routed to RQ-EFM / RQ-ISO classification.

Therefore a composition-time ambiguity over `p_A` is either:

- an existing ownership / interface conformance issue; or
- evidence that the semantic classification was wrong and the aggregate must be re-evaluated under RQ-EFM / RQ-ISO.

No independent "composition property owner" category is required.

### 6.4 External antecedent pressure

MOOSE explicitly sorts Material producers before consumers and rejects dependency cycles. SSP separately demonstrates system-level connection responsibility / unambiguous data-flow constraints.

The producer-consumer graph itself is established practice.

### 6.5 Result

```text
explicit acyclic dependency + preserved semantics + complete aggregate formulation
    -> ordinary composition remains possible
```

**ECA-specific predicate required:** `NO`.

---

## 7. ECA-S2 — Feedback Cycle Requiring Iteration but No New Semantic State

### 7.1 Construction

Extension A consumes an explicit B output; Extension B consumes an explicit A output.

The combined system contains a feedback relation:

```text
A <-> B
```

All quantities required by the selected thermodynamic formulation remain explicit. No hidden extension-local coordinate becomes necessary to recover current Thermodynamic State or determine its next update.

### 7.2 Semantic test

RQ-EFM Test C remains satisfied: the current thermodynamic condition is still determined by the declared authoritative state, applicable configuration, and explicit exchange set.

RQ-EFM Test U remains satisfied at the semantic formulation level: for a fully specified current aggregate condition and declared physical interaction, the governing update is defined.

The implementation may nevertheless require:

- Picard iteration;
- Newton / monolithic solve;
- implicit coupling;
- checkpoint / rollback;
- smaller timestep;
- convergence control.

These are not new Framework authority categories.

### 7.3 External antecedent pressure

MOOSE distinguishes loose, tight/Picard, and fully coupled treatment. preCICE similarly distinguishes explicit/implicit coupling and requires fully implicit multi-coupling for multiple strong interactions.

Thus stronger numerical coupling after composition is established prior art.

### 7.4 Result

```text
stronger numerical coupling != automatic Core / State semantic revision
```

**ECA-specific predicate required:** `NO`.

---

## 8. ECA-S3 — Composition-Induced Thermodynamic Closure / Update Insufficiency

### 8.1 Construction

A and B are each individually D0 under their isolated bounded assumptions.

When composed, however, an extension-local quantity `z` becomes necessary to determine either:

- the current thermodynamic closure; or
- the next authoritative Thermodynamic State update.

The aggregate may exhibit one of the RQ-EFM witnesses:

```text
same authoritative S + same declared material/configuration
but different hidden z
    -> different thermodynamic closure
```

or:

```text
same S_n + same declared material/configuration + same declared exchange
but different hidden z
    -> different required S_(n+1)
```

### 8.2 Aggregate test

This is exactly why individual D0 labels cannot be treated as permanent immunity.

The correct procedure is to evaluate **A+B as the actual aggregate mechanism/formulation** through RQ-EFM Test C / Test U.

If the selected thermodynamic formulation is incomplete unless `z` becomes authoritative thermodynamic information or the governing formulation changes, the aggregate is D1 under the unrevised Core.

### 8.3 Result

```text
composition creates new closure/update dependency
    -> aggregate RQ-EFM fails
    -> D1 / explicit formulation-Core revision or scope narrowing
```

No separate ECA criterion is needed to reach the decision.

**ECA-specific predicate required:** `NO`.

This scenario provides the strongest direct falsification of an independent ECA gap: the most composition-specific semantic failure is already a direct RQ-EFM case once the aggregate is evaluated honestly.

---

## 9. ECA-S4 — Ownership / Authority / Responsibility Conflict

### 9.1 Construction

A and B are individually ordinary extensions, but their composition contains one of the following conflicts:

- both claim authority over the same mandatory Core thermodynamic quantity;
- both attempt to write/evolve Thermodynamic State outside Thermodynamic Computation;
- one extension promotes its local state as mandatory Core State because another extension consumes it;
- two components reinterpret the same information category with incompatible ownership semantics;
- two property/configuration paths ambiguously claim ownership rather than merely reading/supplying information.

### 9.2 Existing-rule test

ThermoCore already distinguishes:

- ownership from Read / Write / Supply / Consume;
- Thermodynamic State authority from extension-local state;
- Configuration from Runtime State;
- communication from ownership transfer.

RQ-ISO-001 further establishes the non-promotion / fixed semantic Core-state boundary for ordinary extensions.

Therefore the composition does not create a new kind of authority. It creates a conflict among already-defined authorities.

If the disputed quantity is actually required for aggregate thermodynamic completeness, the case also routes to RQ-EFM.

### 9.3 Result

```text
composition-induced authority conflict
    -> existing Framework Conformance / RQ-ISO violation
```

**ECA-specific predicate required:** `NO`.

---

## 10. ECA-S5 — Duplicate or Omitted Physical Energy Contribution

### 10.1 Construction

A and B are individually D0, but in the aggregate both account for the same physical transfer/conversion, for example:

```text
physical transfer X
   -> A reports thermal contribution q_X
   -> B independently reports the same q_X
```

or each component assumes the other is responsible, so the transfer is omitted.

### 10.2 Aggregate semantic test

The thermodynamic state-space may remain formally sufficient and ownership may remain valid, yet the physical accounting is wrong.

This case does not require a new composition-specific architecture category because RQ-CEX-001 already survived only as the **Conservative Exchange Accounting Property**:

> an admitted energy-bearing interaction should have an unambiguous thermodynamic accounting role and applicable conservation target.

The duplicate/omitted contribution is precisely a violation of that surviving engineering/conformance property.

### 10.3 Result

```text
duplicate / omitted physical contribution
    -> Conservative Exchange Accounting Property failure
```

**ECA-specific predicate required:** `NO`.

The error may cause numerical energy drift, but the semantic defect precedes the numerical symptom.

---

## 11. ECA-S6 — True Aggregate Scope Expansion Boundary Control

### 11.1 Construction

A and B are each D0 only under bounded isolated assumptions, but their actual joint physics introduces a new governing responsibility beyond the declared thermodynamic scope.

Representative pattern:

```text
A alone: external/cross-domain contribution compatible with fixed thermodynamic formulation
B alone: another bounded external/cross-domain contribution compatible with same formulation
A+B: interaction introduces variable mass, pressure/compressibility, deformation work,
     reaction-transport coupling, or another governing coordinate/equation requirement
```

The mechanism names are not decisive; the governing aggregate physics is.

### 11.2 Aggregate test

The correct classification is not "two ordinary extensions interacting strongly".

The correct question is whether the selected thermodynamic formulation remains complete for the **actual aggregate scope**.

If not:

```text
aggregate RQ-EFM -> D1
```

and the project must choose:

- explicit thermodynamic formulation/Core revision; or
- scope narrowing.

### 11.3 Result

```text
true aggregate physical-scope expansion
    -> not protected by local D0 labels
    -> RQ-EFM / explicit revision boundary
```

**ECA-specific predicate required:** `NO`.

---

## 12. Re-Evaluation of the Eight RQ-ECA Candidate Dimensions

| Dimension | v0.2 matched-scenario evidence | Final routing | Independent ECA status |
|---|---|---|---|
| ECA-D1 Extension-to-extension data dependency | S1 shows explicit acyclic dependency is admissible; S2 shows feedback may require stronger numerical coupling | Interfaces / dependency declaration; RQ-EFM only if semantic completeness changes | **No independent predicate** |
| ECA-D2 Property/configuration responsibility overlap | S1/S4 distinguish producer-consumer access from actual ownership conflict or misclassified evolving information | Existing ownership/interface/conformance; RQ-EFM/RQ-ISO if information category changes | **No independent predicate** |
| ECA-D3 Source/exchange contribution interaction | S5 directly exercises double-count/omission | Conservative Exchange Accounting Property | **No independent predicate** |
| ECA-D4 Extension-local state dependency | S1 valid when explicit and non-authoritative; S3 fails when local quantity becomes necessary to thermodynamic closure/update | RQ-EFM; RQ-ISO for promotion/authority | **No independent predicate** |
| ECA-D5 Feedback/cyclic dependency | S2 may stay architecture-D0 with stronger iteration; S3 becomes semantic only when closure/update sufficiency fails | Numerical/orchestration unless RQ-EFM semantic failure | **No independent predicate** |
| ECA-D6 Combined thermodynamic closure sufficiency | S3 is direct aggregate closure/update witness | RQ-EFM-001 | **No independent predicate** |
| ECA-D7 Ownership/authority conflict | S4 is directly non-conforming under existing authority rules | RQ-ISO-001 + Framework Conformance | **No independent predicate** |
| ECA-D8 Scope identity | S6 distinguishes same-scope composition from true aggregate physical-scope expansion | RQ-EFM-001 / revision or scope narrowing | **No independent predicate** |

### Dimension result

```text
Candidate dimensions evaluated: 8
Dimensions requiring a new independent ECA architecture category: 0
```

---

## 13. Aggregate-Mechanism Null Decision

### 13.1 Null statement

> Individually ordinary extensions do not retain permanent admissibility immunity under composition. The actual composed mechanism/formulation must be re-evaluated at the aggregate level, but that re-evaluation does not require a new architecture category if formulation completeness, state authority, ownership/interface semantics, physical accounting, and numerical coupling are already classified by existing rules.

### 13.2 Stress-test outcome

All seven matched scenarios are resolved without adding a new semantic predicate.

The aggregate procedure is sufficient within the evaluated scope:

```text
Step 1 — compose the actual physical dependency/state/exchange relation

Step 2 — RQ-EFM-001
         test aggregate thermodynamic closure/update sufficiency and scope

Step 3 — RQ-ISO-001 / Framework Conformance
         test state authority, non-promotion, information ownership

Step 4 — Conservative Exchange Accounting Property
         test duplicated/omitted energy-bearing contribution where applicable

Step 5 — treat iteration/order/synchronization/convergence as numerical/orchestration
         unless they expose a failure in Steps 2–4
```

### 13.3 Decision

```text
Aggregate-mechanism null hypothesis:
SUPPORTED FOR ALL EVALUATED MATCHED SCENARIOS
```

No fifth independent architecture decision category is required by the evaluated cases.

---

## 14. Prior-Art Pressure After v0.2

The following remain clearly excluded from any RQ-ECA novelty claim:

- recognizing that local validity may fail after composition;
- system-level dependency analysis;
- producer/consumer ordering;
- cycle detection;
- algebraic-loop detection;
- aggregate connection validation;
- selecting loose / tight / implicit / fully coupled numerical treatment;
- preventing ambiguous multiple inbound data-flow responsibility;
- defining power-conserving compositional closure;
- requiring aggregate re-evaluation of a connected system.

The matched-scenario test also fails to reveal a narrower ThermoCore-specific criterion beyond the already-completed RQ-EFM / RQ-ISO boundaries and reclassified Conservative Exchange Accounting Property.

---

## 15. Surviving Project Value

The useful project result is retained as:

> **Aggregate Re-Admissibility Property**

Definition:

> Individually admissible ordinary extensions shall not be assumed to remain jointly admissible solely because each extension passed a local admissibility assessment. The actual aggregate physical mechanism/formulation must still satisfy the applicable formulation-relative admissibility, authority, ownership/interface, and physical-accounting rules for the combined declared scope.

This is useful for engineering and conformance because it prevents stale local D0 labels from masking a composition-induced closure failure, authority conflict, accounting defect, or scope expansion.

However, v0.2 provides no evidence that this property is an independent research contribution.

Its status should therefore be:

```text
Aggregate Re-Admissibility Property:
ENGINEERING / CONFORMANCE PROPERTY
```

It is not claimed as novel, first, universally applicable, or superior.

---

## 16. Research-Gap Disposition

```text
Generic composition/dependency problem:
ESTABLISHED PRIOR ART

Local validity not implying every global validity property:
DIRECT ANTECEDENT ESTABLISHED

System-level dependency / connection validation:
ESTABLISHED PRIOR ART

Explicit compositional closure under interconnection constraints:
DIRECT ANTECEDENT ESTABLISHED

Aggregate-null matched-scenario result:
SUPPORTED IN 7 / 7 EVALUATED SCENARIOS

Independent RQ-ECA Research Gap:
NOT SUPPORTED WITHIN THE BOUNDED REVIEW AND MATCHED-SCENARIO STRESS TEST

Research Gap Analysis readiness:
NO-GO

Surviving value:
AGGREGATE RE-ADMISSIBILITY PROPERTY

Recommended classification:
ENGINEERING / CONFORMANCE PROPERTY — NOT AN INDEPENDENT RESEARCH CONTRIBUTION

Novelty / priority:
NOT ESTABLISHED

Framework Specification impact:
NONE
```

---

## 17. Falsification / Boundary Notes

This negative result is bounded.

It does **not** prove that no future composition-specific thermodynamic research problem can exist.

RQ-ECA-001 could only be reopened if new evidence demonstrates a composition-induced semantic condition that:

1. is not a formulation/state-space completeness issue under RQ-EFM-001;
2. is not a state/ownership/non-promotion issue under RQ-ISO-001 or current Framework Conformance;
3. is not an exchange-accounting issue under the Conservative Exchange Accounting Property;
4. is not merely a numerical/orchestration issue; and
5. cannot be resolved by honest aggregate re-evaluation of the actual combined physical scope.

No such condition is established in the current bounded review.

---

## 18. Framework and Implementation Impact

```text
Framework Specification change: NONE
Production implementation change: NONE
Verification change in this research task: NONE
Validation change: NONE
Performance change: NONE
Frozen v1.0.0 release change: NONE
```

The evidence supports research closure/reclassification only.

---

## 19. Decision for Next Stage

Do **not** open an RQ-ECA-001 Research Gap Analysis.

The evidence sequence is sufficient to close the independent contribution line:

```text
Definition
    -> v0.1 prior-art / aggregate-null survey
    -> v0.2 matched-scenario falsification test
    -> independent gap not supported
```

The next task should be a non-normative closure/reclassification record preserving:

- the negative result;
- the prior-art exclusions;
- the Aggregate Re-Admissibility Property;
- the routing relationship to RQ-EFM-001, RQ-ISO-001, Framework Conformance, and Conservative Exchange Accounting;
- the fact that no Framework Specification change follows from this research line.

---

## 20. Current Disposition

**RQ-ECA-001 does not survive as an independent Research Gap within the bounded review and matched-scenario stress test.**

The strongest composition-specific semantic failures evaluated are already classified by existing ThermoCore boundaries once the actual combined mechanism is evaluated honestly at aggregate level.

The surviving engineering lesson is important but narrower:

> local ordinary-extension admissibility is not compositional immunity; aggregate re-admissibility must be checked against the existing semantic boundaries.

That lesson should be retained as an engineering / conformance property rather than represented as a third independent architectural research contribution.