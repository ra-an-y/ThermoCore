# RQ-EFM-001 Research Gap Analysis v0.1

Status: **Completed Candidate Gap Analysis — Non-Normative**  
Research Question: **RQ-EFM-001 — External Energy / Physical Field–Driven Material Response**  
Date: **2026-08-23**  
Tracking: GitHub Issue #92

---

## 1. Purpose

This document performs the bounded Research Gap Analysis for RQ-EFM-001 after closure of the v0.1-v0.3 evidence survey.

The task is not to preserve the original research framing. The evidence survey removed multiple broad claims from consideration and showed that many apparent distinctions are established prior art.

The remaining question is whether a narrower framework-level architectural gap survives:

> **Can a reusable thermodynamic framework operationally decide whether a field-driven mechanism is Core-preserving or requires thermodynamic formulation/Core revision by testing formulation-relative thermodynamic sufficiency, while preserving separate authority for mechanism-local and cross-domain governing state?**

This document is non-normative. It does not modify the ThermoCore Framework Specification, reference formulation, implementation, Verification, Validation, Performance, Framework Conformance, v1.0.0 release claims, or the completed RQ-ISO-001 disposition.

No novelty or first-ever priority claim is made.

---

## 2. Evidence Baseline

This analysis depends on:

- `Research/05_Research_Questions/RQ_EFM_001_Definition_v0.1.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.1.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.2.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.3.md`
- `Research/04_Research_Gap/RQ_ISO_001_Final_Research_Gap_Disposition_v0.1.md`
- relevant Framework Specification documents as background authority only.

The v0.3 evidence survey is treated as closed for broad discovery. New sources should be added only if they are likely to directly satisfy or falsify the surviving candidate in Section 7.

---

## 3. Established Prior Art — Excluded from the Gap

The following are not RQ-EFM-001 Research Gaps.

### 3.1 Generalized thermodynamic work pairs

The literature already provides generalized conjugate field-coordinate descriptions, including:

- magnetic field / magnetization;
- electric field / polarization;
- stress / strain; and
- pressure / volume-related coordinates.

RQ-EFM-001 shall not claim contribution for introducing generalized work-pair treatment of field-driven thermodynamics.

### 3.2 Source, flux/work, constitutive, and state effects

Existing multiphysics and thermodynamic formulations already distinguish among:

- externally supplied heat/energy sources;
- coupled fluxes and generalized work;
- constitutive or material-property dependence; and
- state/order/history dependence.

The existence of these interaction forms is prior art.

### 3.3 Internal variables and history state

Coleman-Gurtin and later internal-variable thermodynamics establish that additional internal variables may be required when conventional state variables are insufficient for history-dependent or dissipative response.

RQ-EFM-001 shall not claim the invention of internal/state variables, hysteresis variables, order parameters, or state-space enlargement.

### 3.4 State-space sufficiency and minimal-state reasoning

Continuum thermodynamics and materials-with-memory theory already treat state-space selection, constitutive state, response sufficiency, and minimal state as established concepts.

The generic question "is this state description sufficient to determine response?" is not novel.

### 3.5 Separate physical-domain state under coupling

preCICE, bond graphs, port-Hamiltonian systems, Modelica, and other multiphysics approaches establish that separate physical-domain variables/state can participate in one coupled system through explicit data, energy, power, or connector relationships.

Strong coupling does not imply that all domain state must be merged into one thermodynamic state set.

### 3.6 Formulation-relative thermodynamic state

`Modelica.Media` and thermodynamic theory already support formulation/medium-relative selection of independent thermodynamic variables and minimal state representations.

RQ-EFM-001 shall not claim formulation-relative state selection itself as new.

### 3.7 Generic multi-axis coupling taxonomies

The v0.2-v0.3 four-axis research representation is a synthesis for ThermoCore analysis, not a novelty claim. Generic multi-dimensional distinctions among coupling form, variables, direction, and numerical organization are established prior art.

---

## 4. What RQ-ISO-001 Already Established

RQ-ISO-001 established a bounded architectural contribution around **Fixed Semantic/Core-State Boundary under Ordinary Extension**.

Its evaluated result showed that, for selected ordinary-extension scenarios:

- extension-specific persistent state need not be promoted into mandatory Core State;
- Core semantic/implementation/interface change can be reduced relative to a shared-state comparator;
- justified Core evidence re-execution scope can be smaller; and
- a genuine governing-physics counterexample must still trigger Core revision rather than be hidden in extension state.

Therefore RQ-EFM-001 cannot claim as new:

- fixed Core-State authority;
- non-promotion of ordinary extension state;
- Core completeness under ordinary extension; or
- the principle that governing-physics changes can require Core revision.

If RQ-EFM-001 contributes anything distinct, it must concern **how a physical field-driven formulation is classified before the RQ-ISO-001 authority rule is applied**.

---

## 5. Remaining Problem after Prior-Art Exclusion

The evidence leaves a practical classification problem.

For a named mechanism such as electrocaloric, magnetocaloric, elastocaloric, Joule heating, or thermoelectric coupling, the mechanism name alone does not determine whether it should be treated as:

- an external source;
- a constitutive dependency;
- mechanism-local persistent state;
- separate cross-domain governing state; or
- additional thermodynamic governing state requiring formulation revision.

Different legitimate formulations of the same named mechanism can make different choices.

The missing architectural decision is therefore not "which class does this mechanism name belong to?"

It is:

> **Given a frozen thermodynamic formulation and claimed scope, what evidence-based rule determines whether external/mechanism/cross-domain state can remain outside authoritative Thermodynamic State without making thermodynamic evolution incomplete?**

The v0.3 survey provides prior art for the ingredients of such a decision, but did not find the full combination operationalized as a reusable thermodynamic-framework extension-admissibility rule.

---

## 6. Stable Analytical Model

RQ-EFM-001 shall use the v0.3 orthogonal model rather than a mutually exclusive taxonomy.

### Axis A — Interaction form

A mechanism may involve one or more of:

- `A1` source/deposition;
- `A2` flux/generalized-work exchange;
- `A3` constitutive/property dependence; or
- `A4` thermodynamic state-space/closure change.

### Axis B — State role

A participating quantity may be:

- `B1` prescribed/configuration-like input;
- `B2` equilibrium/algebraically derived quantity;
- `B3` mechanism-local persistent internal/history state;
- `B4` cross-domain governing state; or
- `B5` thermodynamic governing state.

### Axis C — Coupling relation

Coupling may be one-way or bidirectional and explicit, staggered, iterative, implicit, partitioned, or monolithic.

These implementation/numerical dimensions do not determine authority.

### Axis D — Thermodynamic authority impact

- `D0 — Core-preserving`: the frozen Thermodynamic State and selected thermodynamic governing formulation remain sufficient for the claimed scope when external/mechanism state is represented through declared exchanges.
- `D1 — Core/formulation revision`: correct thermodynamic evolution for the claimed scope requires additional authoritative thermodynamic state, generalized-work/conservation responsibility, or closure that cannot be represented honestly by the existing state plus declared exchanges.

The Research Gap candidate is primarily about the reproducible `D0`/`D1` decision, not Axes A-C themselves.

---

## 7. Bounded Research Gap Candidate

### Candidate name

**Formulation-Relative Thermodynamic Extension Admissibility Boundary**

### Candidate formulation

> **Within the evaluated thermodynamic-framework domain, existing thermodynamic and multiphysics literature provides state-space theory, internal variables, generalized work pairs, multi-domain coupling, and formulation-relative thermodynamic states, but the bounded survey did not identify a reviewed reusable software-framework architecture that jointly operationalizes these concepts as an extension-admissibility rule: a field-driven mechanism may remain outside mandatory Thermodynamic Core State only when, for a frozen thermodynamic formulation and claimed scope, its external/mechanism/cross-domain state can be abstracted to declared exchanges without loss of thermodynamic state-space sufficiency; otherwise explicit thermodynamic formulation/Core revision or scope narrowing is required.**

### Classification

**Evidence-supported Research Gap candidate — bounded survey only.**

This is not a novelty finding.

---

## 8. Why the Candidate Does Not Automatically Collapse into RQ-ISO-001

RQ-ISO-001 answers:

> Once a mechanism is accepted as an ordinary extension, what happens if its state remains extension-owned versus promoted into shared Core State?

RQ-EFM-001 asks:

> What evidence determines whether a field-driven mechanism is admissible as an ordinary/core-preserving participation case at all?

The distinction is therefore:

`RQ-EFM-001: physical/formulation classification gate`

followed by:

`RQ-ISO-001: authority/non-promotion behavior after ordinary-extension classification`

However, this distinction is only meaningful if RQ-EFM-001 can demonstrate a reproducible decision procedure and consequences not already implied trivially by RQ-ISO-001 plus standard state-space theory.

If it cannot, RQ-EFM-001 shall be reclassified as an application/specialization of RQ-ISO-001 rather than a distinct architecture contribution.

---

## 9. Candidate Operational Criterion

The v0.3 evidence supports evaluating the candidate with a formulation-relative sufficiency test.

### 9.1 Freeze formulation and scope

Before classification, freeze:

- authoritative Thermodynamic State;
- thermodynamic governing balance/evolution relation;
- material/closure assumptions;
- allowed external inputs/exchanges; and
- explicitly excluded physics.

### 9.2 Identify omitted/non-Core quantities

Record all mechanism-local and cross-domain state that is proposed to remain outside Thermodynamic State.

### 9.3 Define declared thermodynamic exchanges

Specify exactly what the thermodynamic responsibility receives from the mechanism/domain:

- source/deposition;
- flux/work;
- boundary condition;
- constitutive input;
- property update; or
- another declared exchange.

### 9.4 Sufficiency witness test

Search for a pair of physically admissible coupled states satisfying:

- same frozen Thermodynamic State;
- same declared thermodynamic inputs/exchanges under the candidate abstraction;
- different omitted mechanism/cross-domain state; and
- different required future thermodynamic evolution within the claimed scope.

If such a pair exists, the abstraction is insufficient.

The result must then be one of:

- enrich the declared exchange;
- add required thermodynamic governing state/closure;
- revise the formulation/Core; or
- narrow the claimed scope.

If no such pair can be constructed under the frozen formulation and the external state affects thermodynamics only through declared exchanges, `D0` remains a viable classification.

This witness logic is derived from established state-sufficiency/minimal-state concepts; its possible research role is the architectural application to extension admissibility, not the underlying mathematical idea.

---

## 10. Candidate Engineering Failure Modes

A useful RQ-EFM-001 contribution would need to address two opposite errors.

### 10.1 False Promotion

A separately governed external/mechanism quantity is promoted into mandatory Thermodynamic Core State merely because it participates in thermal evolution, even though the frozen thermodynamic formulation remains complete through declared exchanges.

Potential consequences:

- unnecessary Core-State growth;
- unnecessary semantic coupling;
- unnecessary Core interface changes; and
- unnecessary Core evidence impact.

These consequences overlap with RQ-ISO-001, so they cannot alone establish a new RQ-EFM-001 contribution.

### 10.2 False Isolation

A quantity required for complete thermodynamic evolution is left outside authoritative Thermodynamic State/closure and represented as if source/property/local-state abstraction were sufficient.

Potential consequences:

- non-unique thermodynamic future under identical declared state/exchange;
- hidden governing physics;
- invalid conservation/work representation;
- formulation incompleteness; or
- misleading Core completeness claims.

**False-isolation detection is the more distinct RQ-EFM-001 target.**

RQ-ISO-001 tested one explicit governing-physics counterexample, but RQ-EFM-001 would generalize the admissibility decision across different field-driven formulation families.

---

## 11. Candidate Hypotheses

The following hypotheses are proposed for later pre-registration. They are untested.

### H-EFM-01 — False-Promotion Avoidance

For pre-classified `D0` field-driven formulations, a formulation-relative sufficiency gate will keep separately governed mechanism/cross-domain state outside mandatory Thermodynamic Core State when declared exchanges are sufficient, without omitting information required for correct thermodynamic evolution.

**Status:** Untested.

### H-EFM-02 — False-Isolation Detection

For pre-classified `D1` formulations, the sufficiency witness test will identify at least one pair of states for which identical current Thermodynamic State and declared exchanges produce different required future thermodynamic evolution, forcing exchange enrichment, formulation/Core revision, or scope narrowing.

**Status:** Untested.

### H-EFM-03 — Formulation-Relative Classification Stability

When the same mechanism is evaluated under two frozen formulations with different state/closure requirements, the decision procedure will change `D0`/`D1` classification only when the thermodynamic sufficiency condition changes, rather than because the mechanism name changes.

**Status:** Untested.

### H-EFM-04 — Distinctness from RQ-ISO-001

The RQ-EFM-001 decision procedure will produce at least one non-trivial admissibility/reclassification result that cannot be obtained merely by assuming an ordinary extension and then applying the RQ-ISO-001 non-promotion rule.

If H-EFM-04 fails, RQ-EFM-001 shall be reclassified as an application/specialization of RQ-ISO-001.

**Status:** Untested.

---

## 12. Evaluation Scenario Families

The future consequence test should use formulation pairs rather than mechanism names alone.

### S0 — Externally supplied energy deposition control

Example family:

- prescribed Joule loss or optical absorption power supplied as an energy source;
- external field solution is outside the thermodynamic formulation;
- no hidden state influences thermodynamics except through the supplied power trajectory.

Purpose:

- establish a clean `D0` control;
- verify that field participation alone does not cause state promotion.

### S1 — Reduced equilibrium caloric formulation

Example family:

- electrocaloric or magnetocaloric response represented by a frozen equilibrium free-energy/constitutive relation;
- field is prescribed or separately supplied;
- no kinetic/history variable is required by the selected scope.

Purpose:

- test whether generalized field dependence can remain a Core-preserving constitutive/work interaction when thermodynamic sufficiency is retained.

No result is pre-ordained; the scenario must be reviewed for actual state requirements before measurement.

### S2 — Hysteretic/stateful caloric formulation

Example family:

- same named caloric mechanism as S1 where possible;
- polarization, magnetization, phase/order fraction, or another history variable follows an evolution law and affects thermodynamic response.

Purpose:

- test `B3` versus `B5` classification;
- attempt to construct a sufficiency witness pair;
- determine whether exchange enrichment or thermodynamic formulation revision is required.

### S3 — Cross-domain governing thermoelectric formulation

Example family:

- electrical potential/current and thermal state co-evolve;
- Peltier/Seebeck/Thomson/Joule contributions couple electrical and thermal balances;
- electrical state remains semantically electrical unless the thermodynamic formulation itself requires otherwise.

Purpose:

- test whether strong bidirectional governing coupling can remain `D0` with respect to Thermodynamic State while still being mandatory for the larger coupled-system problem.

### S4 — True thermodynamic-formulation revision case

Select a formulation where the frozen thermodynamic state/closure is demonstrably insufficient unless an additional thermodynamic coordinate, generalized-work term, conservation responsibility, or closure is introduced.

Purpose:

- provide a positive `D1` case;
- verify that the procedure does not protect Core isolation at the expense of physical completeness.

---

## 13. Comparator Logic

The future test shall not compare ThermoCore against an intentionally defective monolithic architecture.

Instead, use decision policies as controls:

### Policy P1 — Participation-Promotion Control

Persistent/cross-domain quantities that directly participate in thermodynamic evolution are promoted into shared thermodynamic state.

Purpose: expose false-promotion pressure.

This is a controlled policy, not a claim about a named framework.

### Policy P2 — Exchange-Only Permissive Control

A quantity remains outside Thermodynamic State whenever some source/work/property interface can be written, without an explicit sufficiency witness test.

Purpose: expose false-isolation pressure.

This is also a controlled policy, not a claim about a named framework.

### Policy R — Formulation-Relative Sufficiency Gate

Use the Section 9 procedure before deciding `D0` or `D1`.

The evaluation shall not assign an overall superiority score. It shall report false promotion, false isolation, classification stability, and hidden-coupling findings separately.

---

## 14. Candidate Metrics

### 14.1 Classification metrics

- `M-F1` number of quantities promoted into mandatory Thermodynamic State;
- `M-F2` number of omitted quantities shown by a witness pair to affect future thermodynamic evolution;
- `M-F3` number of required exchange/interface enrichments;
- `M-F4` number of explicit formulation/Core revisions;
- `M-F5` classification changes caused only by formulation change rather than mechanism-name change.

### 14.2 Consistency metrics

- `M-K1` deterministic agreement of repeated classification from the same frozen evidence package;
- `M-K2` number of classification decisions requiring post-hoc unstated assumptions;
- `M-K3` number of hidden dependencies discovered after initial classification.

### 14.3 Distinctness metric

- `M-D1` count of scenarios where RQ-EFM-001 must first decide whether ordinary-extension treatment is admissible before the RQ-ISO-001 authority rule can be applied.

If `M-D1 = 0` across all valid scenarios, H-EFM-04 is not supported and distinct RQ-EFM-001 contribution is doubtful.

---

## 15. Pre-Registered Result Categories for a Future Test

Each hypothesis should later be classified independently as:

- `SUPPORTED FOR EVALUATED FORMULATIONS`;
- `PARTIALLY SUPPORTED`;
- `NOT SUPPORTED`; or
- `FALSIFIED / RECLASSIFICATION REQUIRED`.

No composite `ThermoCore wins` result shall be used.

---

## 16. Negative / Null Outcomes

RQ-EFM-001 must preserve the following outcomes if observed.

### Outcome N1 — Every valid case is already obvious from RQ-ISO-001

Then RQ-EFM-001 is not a distinct architecture contribution.

### Outcome N2 — D0/D1 depends on subjective modeling preference rather than frozen scope/evidence

Then the proposed admissibility procedure is not reproducible enough and the Research Gap candidate must be narrowed or rejected.

### Outcome N3 — The witness test always reduces to standard state-space theory with no framework-level consequence

Then RQ-EFM-001 is best treated as an application of established thermodynamic theory.

### Outcome N4 — Cross-domain governing state is routinely over-promoted

Then the criterion fails isolation usefulness.

### Outcome N5 — Required thermodynamic state is routinely hidden outside Core

Then the criterion fails physical soundness and must be rejected.

### Outcome N6 — Direct prior art is found

If an existing framework explicitly implements the full Section 7 combination, the candidate shall be reframed as adoption/specialization/comparison rather than a gap.

---

## 17. Research Gap Classification

### Established by the bounded evidence

The following are established prior art:

- generalized work-pair thermodynamics;
- internal variables;
- state-space sufficiency/minimal-state reasoning;
- formulation-relative thermodynamic state selection;
- multi-domain energy/power coupling;
- physical connectors;
- partitioned/monolithic multiphysics coupling; and
- generic coupling taxonomies.

### Surviving candidate

**Formulation-Relative Thermodynamic Extension Admissibility Boundary**

### Current status

**Evidence-supported Research Gap candidate — not established as novelty.**

The candidate is narrower than RQ-EFM-001's original framing and remains dependent on demonstrating H-EFM-04 distinctness from RQ-ISO-001.

---

## 18. Consequence-Test Readiness

### Decision

**GO — a bounded pre-registered consequence/classification test is justified.**

### Reason

The candidate is now:

- sufficiently narrow;
- explicitly separated from established prior art;
- falsifiable through witness-pair and formulation-pair scenarios;
- capable of negative/null outcomes; and
- at risk of collapsing into RQ-ISO-001, which can itself be tested.

No Framework Specification change is justified at this stage.

The next step should be a dedicated Work Task Prompt for an RQ-EFM-001 consequence-test plan, followed by pre-registration before any implementation measurements.

---

## 19. Claims Not Supported by This Analysis

This document does not establish that:

- the candidate is globally novel;
- no prior framework implements a similar admissibility boundary;
- ThermoCore is superior to Modelica, MOOSE, COMSOL, OpenFOAM, preCICE, bond graphs, port-Hamiltonian systems, or other multiphysics frameworks;
- all field-driven mechanisms should remain extensions;
- all internal variables should remain outside Thermodynamic State;
- all cross-domain governing state can remain outside Core;
- the current ThermoCore reference formulation supports electrocaloric, magnetocaloric, elastocaloric, thermoelectric, or other reviewed mechanisms;
- the proposed decision procedure is validated;
- RQ-EFM-001 is distinct from RQ-ISO-001; or
- Framework Specification changes are authorized.

---

## 20. Conclusion

RQ-EFM-001 survives the evidence survey only in a narrowed form.

The broad physical and software concepts needed to describe external field coupling are established prior art. The potentially unresolved issue is not how to enumerate coupling types, but how a reusable thermodynamic framework decides **admissibility**: whether a mechanism's non-Core state can remain outside authoritative Thermodynamic State without making the frozen thermodynamic formulation incomplete.

That candidate is distinct from RQ-ISO-001 in principle because it occurs before ordinary-extension authority rules are applied. It remains distinct in research terms only if a pre-registered evaluation shows non-trivial classification consequences that RQ-ISO-001 alone cannot provide.

The correct next step is therefore consequence-test design, not Specification.
