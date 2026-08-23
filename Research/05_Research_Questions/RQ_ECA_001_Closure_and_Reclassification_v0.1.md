# RQ-ECA-001 Closure and Reclassification

Version: 0.1  
Status: Completed Closure / Reclassification — Non-Normative  
Research Question: RQ-ECA-001 — Compositional Extension Admissibility  
Tracking Issue: #133  
Date: 2026-08-23

---

## 1. Purpose

This document closes **RQ-ECA-001 — Compositional Extension Admissibility** as an independent research-gap line and records the surviving result as an engineering / conformance property.

The closure preserves the negative research result established by the bounded prior-art review and matched-scenario stress test. It does not convert established compositionality, dependency-analysis, system-level validation, or the composition of existing ThermoCore rules into a new research contribution.

This document is non-normative. It does not modify the ThermoCore Framework Specification, production implementation, Verification, Validation, Performance results, or the frozen v1.0.0 release.

---

## 2. Research Question

RQ-ECA-001 asked:

> For a fixed declared thermodynamic scope, when multiple individually admissible ordinary extensions are composed, under what conditions does the composition remain ordinary/Core-preserving without introducing new Thermodynamic State authority, conflicting responsibility over material/property information, duplicated or omitted physical contribution, hidden closure dependence, or a requirement to revise the governing thermodynamic formulation/Core? When must the composed system be re-evaluated as an aggregate mechanism or reclassified outside ordinary-extension scope?

The key falsification path was explicit from the definition stage:

> If treating the composition as one aggregate mechanism, reapplying RQ-EFM-001 to formulation/state-space completeness and scope, and then applying RQ-ISO-001 to authority/non-promotion fully resolves the meaningful architecture cases, RQ-ECA-001 shall be closed rather than narrowed indefinitely.

---

## 3. Evidence Sequence

RQ-ECA-001 was evaluated through two bounded evidence passes.

### 3.1 Evidence Matrix v0.1

`Compositional_Extension_Admissibility_Evidence_Matrix_v0.1.md` established strong prior art for:

- local-versus-global validity distinctions;
- dependency graph analysis;
- cycle and algebraic-loop detection;
- system-level connection and data-flow validation;
- multi-participant coupling restrictions;
- aggregate treatment of tightly coupled systems; and
- explicit compositional closure under defined interconnection constraints.

The first pass found no independent architecture category beyond already-established ThermoCore boundaries and strongly supported the aggregate-mechanism null hypothesis.

### 3.2 Evidence Matrix v0.2

`Compositional_Extension_Admissibility_Evidence_Matrix_v0.2.md` stress-tested seven matched scenarios:

1. orthogonal independently admissible extensions;
2. acyclic extension-to-extension property dependency;
3. feedback requiring stronger numerical coupling but no new semantic closure;
4. composition-induced thermodynamic closure/state-space insufficiency;
5. composition-induced ownership/authority conflict;
6. duplicated or omitted physical energy contribution; and
7. true aggregate scope expansion requiring formulation/Core reconsideration.

All seven scenarios were completely classified by existing boundaries. No independent fifth architecture predicate was required.

---

## 4. Final Research-Gap Disposition

The final independent Research Gap disposition is:

```text
NOT SUPPORTED WITHIN THE BOUNDED REVIEW AND MATCHED-SCENARIO STRESS TEST
```

The research contribution claim is therefore:

```text
CLOSED
```

No RQ-ECA Research Gap Analysis shall be opened on the current evidence baseline.

This negative result is retained as valid research evidence. It is not treated as a failed attempt to be hidden or rewritten into a narrower novelty claim.

---

## 5. Why the Independent Gap Does Not Survive

The evaluated composition-induced conditions reduce to existing categories:

```text
aggregate thermodynamic closure / state-space / update insufficiency
    -> RQ-EFM-001

state authority / non-promotion / ownership conflict
    -> RQ-ISO-001 + Framework Conformance

duplicate or omitted physical energy contribution
    -> Conservative Exchange Accounting Property

iteration / ordering / synchronization / convergence / deadlock
    -> numerical or orchestration concern unless it changes one of the semantic categories above

true aggregate physical-scope expansion
    -> RQ-EFM-001 / explicit formulation-Core revision or scope narrowing
```

The matched scenarios found no remaining composition-specific semantic decision that requires an additional independent ThermoCore research rule.

The fact that local admissibility does not guarantee all global properties is already well anteceded in established modeling and co-simulation practice. The ThermoCore-specific consequence is therefore best treated as a re-evaluation obligation, not as a new general compositionality contribution.

---

## 6. Surviving Result

The surviving result is named:

> **Aggregate Re-Admissibility Property**

Classification:

```text
ENGINEERING / CONFORMANCE PROPERTY
```

It is not an independent research contribution.

### 6.1 Property statement

Individually admissible ordinary extensions do not retain permanent admissibility immunity when composed.

For the actual combined scope, the aggregate mechanism/formulation shall still satisfy the applicable existing ThermoCore boundaries concerning:

- formulation/state-space completeness;
- state and responsibility authority;
- interface semantics;
- physical contribution accounting; and
- Framework Conformance.

The property requires re-evaluation of the aggregate semantics. It does not prescribe a particular scheduler, solver, iteration scheme, dependency engine, graph representation, API, or implementation technique.

### 6.2 What the property does not mean

Aggregate Re-Admissibility does not mean:

- every composition is invalid until proven otherwise by a new research process;
- ordinary extensions may never depend on one another;
- feedback or cyclic numerical coupling automatically requires Core revision;
- every additional data dependency creates new Thermodynamic State;
- every property overlap is a new architecture category;
- pairwise local validity guarantees global validity; or
- aggregate re-evaluation establishes novelty.

---

## 7. Boundary with RQ-EFM-001

RQ-EFM-001 remains the authority for formulation-relative thermodynamic admissibility.

If composition introduces a new requirement such that the authoritative Thermodynamic State, applicable material/configuration information, and semantically honest exchanges no longer close the selected aggregate thermodynamic formulation, the case routes to RQ-EFM-001.

Typical results include:

```text
aggregate remains complete
    -> ordinary/Core-preserving participation may remain possible

aggregate becomes incomplete
    -> D1 pressure / explicit formulation-Core revision or scope narrowing
```

RQ-ECA-001 does not create a second closure-sufficiency rule.

---

## 8. Boundary with RQ-ISO-001

RQ-ISO-001 remains the authority/non-promotion rule after ordinary-extension status and information categories are accepted.

Composition does not grant an extension new authority over mandatory Core State merely because:

- another extension consumes its information;
- multiple extensions interact strongly;
- an extension participates repeatedly in state evolution; or
- aggregate coupling becomes numerically implicit.

If composition creates conflicting attempts to own, promote, or write authoritative Core State, the case is governed by RQ-ISO-001 and existing Framework Conformance semantics.

RQ-ECA-001 does not create a second state-authority rule.

---

## 9. Boundary with Conservative Exchange Accounting Property

If multiple extensions duplicate, omit, or ambiguously reinterpret the same physical energy contribution, the issue routes to the previously reclassified **Conservative Exchange Accounting Property**.

Aggregate Re-Admissibility therefore does not define a new energy-conservation interface model.

Semantic accounting remains distinct from numerical/discretization conservation.

---

## 10. Numerical and Orchestration Boundary

Composition may require:

- stronger iteration;
- Picard or Newton coupling;
- explicit or implicit exchange;
- scheduling constraints;
- synchronization;
- dependency ordering;
- algebraic-loop solution; or
- deadlock avoidance.

These are not, by themselves, evidence of a new Framework-semantic category.

They become relevant to the architectural boundary only if they expose a change in:

- thermodynamic formulation completeness;
- authoritative state/ownership;
- physical scope; or
- applicable information semantics.

---

## 11. Prior-Art Exclusions Preserved

The following shall not be claimed as novel RQ-ECA contributions on the current evidence baseline:

- recognizing that locally valid components can form a globally invalid or singular composition;
- dependency-graph analysis among composed components;
- cycle or algebraic-loop detection;
- producer/consumer ordering;
- explicit multi-participant coupling graphs;
- choosing loose, tight, explicit, implicit, or fully coupled execution based on aggregate interactions;
- detecting ambiguous system-level data flow;
- formal compositional closure under declared interconnection constraints; or
- the general requirement to evaluate properties of the composed system rather than infer them only from local components.

The reviewed external systems are not claimed to be full ThermoCore equivalents.

---

## 12. Scenario Disposition Summary

| Scenario | Aggregate result | Governing boundary | Independent ECA rule required? |
|---|---|---|---|
| ECA-S0 Orthogonal composition | Aggregate remains admissible | Existing RQ-EFM/RQ-ISO semantics | No |
| ECA-S1 Acyclic property dependency | Explicit dependency may remain admissible | Interfaces + RQ-EFM/RQ-ISO | No |
| ECA-S2 Numerical feedback cycle | Stronger coupling only | Numerical/orchestration | No |
| ECA-S3 Closure insufficiency | Aggregate D1 pressure | RQ-EFM-001 | No |
| ECA-S4 Authority conflict | Non-conforming authority assignment | RQ-ISO-001 / Conformance | No |
| ECA-S5 Duplicate contribution | Accounting ambiguity/duplication | Conservative Exchange Accounting Property | No |
| ECA-S6 True scope expansion | Revision or scope narrowing required | RQ-EFM-001 | No |

Summary:

```text
matched scenarios evaluated: 7
scenarios requiring a new independent ECA rule: 0
candidate dimensions evaluated: 8
dimensions requiring a new independent ECA category: 0
```

---

## 13. Framework Impact

```text
Framework Specification change: NONE
Production implementation change: NONE
Verification change: NONE
Validation change: NONE
Performance change: NONE
v1.0.0 release change: NONE
```

No current evidence justifies reopening the Framework Specification for RQ-ECA-001.

---

## 14. Future Use

Aggregate Re-Admissibility may be tested later as an engineering / conformance property.

A future verification profile may, for example, check that a composed set of extensions is evaluated against the actual combined:

- thermodynamic closure requirements;
- declared state/ownership relationships;
- physical exchange paths; and
- scope assumptions.

Such verification would provide engineering/conformance evidence only. It shall not reopen or imply novelty, priority, or an independent RQ-ECA contribution.

---

## 15. Final Status

```text
RQ-ECA-001 independent Research Gap:
NOT SUPPORTED WITHIN THE BOUNDED REVIEW AND MATCHED-SCENARIO STRESS TEST

Research contribution claim:
CLOSED

Surviving value:
Aggregate Re-Admissibility Property

Classification:
ENGINEERING / CONFORMANCE PROPERTY

Research Gap Analysis:
NO-GO

Novelty / priority:
NOT ESTABLISHED

Framework Specification impact:
NONE
```

RQ-ECA-001 is therefore closed as an independent research line while its bounded negative result and surviving engineering property remain part of the ThermoCore research record.
