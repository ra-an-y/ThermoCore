# RQ-EFM-001 Final Research Gap Disposition v0.1

Status: **Completed Research Gap / Contribution Disposition — Non-Normative**  
Research Question: **RQ-EFM-001 — External Energy / Physical Field–Driven Material Response**  
Surviving Gap: **Formulation-Relative Thermodynamic Extension Admissibility Boundary**  
Date: **2026-08-23**  
Tracking: GitHub Issue #104  
Final experiment merge: `9567cb64bdcce4f76646b1680463969ab09c2411`

---

## 1. Purpose

This document closes RQ-EFM-001 v0.1 by disposing the bounded Research Gap candidate after completion of:

```text
Research Question Definition
→ Evidence Survey v0.1-v0.3
→ Research Gap Analysis
→ Pre-registered Consequence Test
→ Phase A Frozen Baseline
→ Phase B Controls
→ Phase C Matched Formulation Pair
→ Phase D Cross-Domain Governing Coupling
→ Final Gap / Contribution Disposition
```

The purpose is to state, with explicit limits:

- what is established prior art and therefore excluded from contribution claims;
- whether the surviving Research Gap candidate is supported within the bounded survey and evaluated thermodynamic-framework scope;
- what the pre-registered consequence test supports;
- whether RQ-EFM-001 remains distinct from RQ-ISO-001;
- what bounded Research Contribution can be claimed;
- what novelty / priority claims remain unsupported; and
- whether the result is mature enough to enter a future normative Framework Specification task.

This document is non-normative. It does not modify the ThermoCore Framework Specification, production implementation, Verification, Validation, Performance, Framework Conformance, the frozen v1.0.0 publication baseline, or the completed RQ-ISO-001 disposition.

---

## 2. Final Disposition Summary

### Research Gap

**SUPPORTED within the bounded survey and evaluated thermodynamic-framework scope.**

The bounded survey established strong prior art for the underlying thermodynamic and multiphysics concepts, but did not identify a reviewed reusable thermodynamic software-framework architecture that operationalized the same complete extension-admissibility rule evaluated here: freeze a selected thermodynamic formulation and scope; preserve semantically separate mechanism/cross-domain state where possible; test instantaneous closure and update sufficiency under declared exchanges; permit physically meaningful exchange enrichment; and require explicit formulation/Core revision or scope narrowing when the existing thermodynamic state-space remains insufficient.

This is a bounded Research Gap disposition, not a novelty or first-ever priority finding.

### Experiment-level hypotheses

The pre-registered result is:

- **H-EFM-01 — False-Promotion Avoidance:** `SUPPORTED FOR EVALUATED FORMULATIONS`
- **H-EFM-02 — False-Isolation Detection:** `SUPPORTED FOR EVALUATED FORMULATIONS`
- **H-EFM-03 — Formulation-Relative Classification Stability:** `SUPPORTED FOR EVALUATED FORMULATIONS`
- **H-EFM-04 — Distinctness from RQ-ISO-001:** `SUPPORTED FOR EVALUATED FORMULATIONS`

No hypothesis is promoted to universal support.

### Research Contribution classification

**Bounded Architectural Operationalization + Pre-registered Consequence Evaluation.**

More specifically:

> **RQ-EFM-001 operationalizes a formulation-relative thermodynamic extension-admissibility boundary for reusable thermodynamic-framework architecture: external, mechanism-local, and cross-domain governing state may remain outside authoritative Thermodynamic State when the frozen thermodynamic formulation remains complete under semantically honest declared exchanges; exchange enrichment is permitted when it restores update sufficiency without serializing hidden governing state; and explicit thermodynamic formulation/Core revision or scope narrowing is required when instantaneous closure or update sufficiency cannot otherwise be preserved. The pre-registered S0-S4 evaluation supports this decision procedure for the evaluated formulations and shows that it produces admissibility decisions before the RQ-ISO-001 ordinary-extension authority rule is applicable.**

### Novelty / priority

**NOT ESTABLISHED.**

The repository shall not claim that ThermoCore is the first framework to use state-space sufficiency, generalized work exchange, internal variables, multi-domain coupling, formulation-relative state, or any globally novel extension-admissibility principle.

---

## 3. Evidence Chain

The final disposition depends on the following cumulative chain:

### Research definition and evidence

- `Research/05_Research_Questions/RQ_EFM_001_Definition_v0.1.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.1.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.2.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.3.md`
- `Research/04_Research_Gap/RQ_EFM_001_Research_Gap_Analysis_v0.1.md`

### Pre-registration and execution

- `Research/05_Research_Questions/RQ_EFM_001_Consequence_Test_Plan_v0.1.md`
- `Research/05_Research_Questions/RQ_EFM_001_Phase_A_Frozen_Baseline_v0.1.md`
- `Research/05_Research_Questions/RQ_EFM_001_Phase_B_Result_v0.1.md`
- `Research/05_Research_Questions/RQ_EFM_001_Phase_C_Result_v0.1.md`
- `Research/05_Research_Questions/RQ_EFM_001_Phase_D_Result_v0.1.md`

### Distinctness comparator

- `Research/04_Research_Gap/RQ_ISO_001_Final_Research_Gap_Disposition_v0.1.md`

### Frozen execution lineage

```text
Pre-registered semantic baseline:
15ab144783bd3ccf1953cb7d7b2bb61998603bf6

Phase A merge:
4114c32a04fad2b7c5d56df74c3ec72cdba5b4b2

Phase B merge:
5d1b93c731c1629aede3ec0ffdb22a9d06322d53

Phase C merge:
123e18d031f0fc15df8129dda69feb2d24d92c65

Phase D merge:
9567cb64bdcce4f76646b1680463969ab09c2411
```

The experiment did not change its scenario equations, thresholds, exchange definitions, or hypothesis decision rules after execution began.

---

## 4. Established Prior Art — Explicitly Excluded from Contribution Claims

RQ-EFM-001 does **not** contribute the following concepts by themselves.

### 4.1 Generalized thermodynamic work pairs

Field-coordinate and conjugate-variable treatments such as electric field/polarization, magnetic field/magnetization, stress/strain, and pressure/volume are established thermodynamic prior art.

ThermoCore shall not claim invention of generalized field work or conjugate thermodynamic coordinates.

### 4.2 Internal variables, order parameters, and history state

Internal-state-variable thermodynamics and materials-with-memory theory already establish that additional variables may be required when conventional state variables are insufficient for history-dependent response.

ThermoCore shall not claim invention of persistent order/history variables or state-space enlargement.

### 4.3 State-space sufficiency and minimal-state reasoning

The general principle that a constitutive or thermodynamic state must be sufficient to determine the required response is established prior art.

The contribution is not the mathematical idea of sufficiency itself.

### 4.4 Formulation-relative thermodynamic state

Thermodynamic formulations and software such as Modelica.Media already permit formulation-/medium-relative selection of independent thermodynamic state variables.

ThermoCore shall not claim formulation-relative state selection as new.

### 4.5 Source, flux/work, constitutive, and state coupling forms

Multiphysics literature and software already distinguish heat/source terms, fluxes, generalized work, constitutive dependencies, and state/order effects.

The contribution is not a new list of coupling forms.

### 4.6 Separate domain state under multiphysics coupling

Existing multi-domain and co-simulation approaches already show that distinct physical-domain state can exchange energy or other coupling data without all state being merged into one global thermodynamic state.

Strong or bidirectional coupling alone is therefore not a novel state-separation principle.

### 4.7 Generic multi-axis coupling taxonomy

The RQ-EFM evidence survey's interaction/state/coupling/authority axes are an analytical synthesis for this research question, not a novelty claim.

---

## 5. Supported Bounded Research Gap

After prior-art exclusion, the surviving gap is not a new physical law, constitutive model, or general multiphysics taxonomy.

The supported bounded gap is:

> **Formulation-Relative Thermodynamic Extension Admissibility Boundary** — within the evaluated thermodynamic-framework problem, the literature reviewed in the bounded survey supplied the component theories needed to reason about thermodynamic state-space, internal variables, generalized work, multi-domain coupling, and formulation-relative state selection, but did not identify a reviewed reusable software-framework architecture that operationalized them together as a pre-extension admissibility gate deciding when external/mechanism/cross-domain state can remain outside authoritative Thermodynamic State through declared exchanges and when thermodynamic formulation/Core revision or scope narrowing is required.

This support is bounded in three ways.

First, it is bounded by the reviewed evidence set. Absence from the survey is not proof of global absence.

Second, it is bounded by thermodynamic-framework architecture. Generalization to arbitrary physical frameworks or all multiphysics software would require separate evidence.

Third, it is bounded by the evaluated formulations. The experiment does not establish a universal classifier for every Joule, optical, caloric, thermoelectric, thermoelastic, reactive, compressible, electromagnetic, or mechanical formulation.

---

## 6. Final Experimental Evidence

### 6.1 S0 — external energy deposition control

S0 demonstrated that distinct external field states can remain outside Thermodynamic State when the frozen current-interval thermal exchange is already complete.

The result was a valid `D0 / U0` control and supported the Future-Exchange Rule: differences that an external solver will resolve for a later interval are not automatically missing current Thermodynamic State.

### 6.2 S1 — reduced equilibrium electrocaloric formulation

S1 had no persistent polarization/history coordinate in the selected reduced formulation.

The supplied generalized caloric energy exchange was sufficient, giving `D0 / U0`.

This demonstrated that the name of a field-driven mechanism does not by itself require persistent field/order state in Thermodynamic State.

### 6.3 S2-E — mechanism-owned polarization with generalized-work exchange

S2-E first exposed an insufficient field-only exchange contract: different polarization histories with the same thermal state and same field-only input required different thermal updates.

The procedure therefore produced a valid Test U witness.

A physically interpretable generalized-work exchange then restored sufficiency without serializing polarization state itself. The anti-smuggling audit passed.

The final result was:

`D0 AFTER U1 EXCHANGE ENRICHMENT`.

This is a central result because it distinguishes:

- a genuinely incomplete exchange contract; from
- a genuinely incomplete thermodynamic state-space.

Not every insufficiency requires state promotion or Core revision.

### 6.4 S2-T — polarization as thermodynamic closure coordinate

S2-T used the same electrocaloric mechanism family but selected a formulation in which polarization contributes directly to thermodynamic stored-energy closure.

The frozen witness produced different instantaneous temperature closure under the same scalar total enthalpy when polarization differed.

The result was a valid Test C witness and:

`D1 — FORMULATION REVISION REQUIRED`.

This supports formulation-relative classification rather than mechanism-name classification.

### 6.5 S3 — thermoelectric cross-domain governing coupling

S3 kept electrical potential/current as semantically electrical governing state while supplying a complete thermal exchange packet containing Joule, Peltier, and Thomson contributions.

Two distinct electrical governing states produced the same current-interval thermal evolution when the complete frozen thermal exchange packet was identical.

The result was:

- Test C: no witness;
- Test U: `U0`;
- hidden-coupling audit: PASS;
- R classification: `D0 — CROSS-DOMAIN GOVERNING COUPLING`.

This supports the rule that bidirectional governing coupling alone does not imply Thermodynamic State merger.

### 6.6 S4 — thermoelastic closure counterexample

S4 intentionally selected a thermoelastic stored-energy formulation where strain contributes directly to instantaneous thermodynamic closure.

Equal scalar total enthalpy with different strain produced different temperature closure.

The procedure correctly produced a D1 formulation-revision requirement instead of preserving Core isolation at the expense of thermodynamic completeness.

---

## 7. Final Hypothesis Disposition

### H-EFM-01 — False-Promotion Avoidance

**SUPPORTED FOR EVALUATED FORMULATIONS.**

The evaluated D0 cases retained external/mechanism/cross-domain state outside mandatory Thermodynamic State when declared exchanges were sufficient.

The controlled P1 policy produced five promotions across the experiment, of which three were classified as false promotions under the frozen formulation facts.

This does not prove that state promotion is generally wrong. S2-T demonstrates the opposite: when the selected thermodynamic closure actually requires the additional coordinate, promotion/revision is not a false promotion.

### H-EFM-02 — False-Isolation Detection

**SUPPORTED FOR EVALUATED FORMULATIONS.**

The R policy identified valid insufficiency witnesses where the frozen abstraction was incomplete and recorded zero missed witnesses in the evaluated scenario set.

The result includes both:

- insufficiency repairable through semantically honest exchange enrichment; and
- insufficiency requiring thermodynamic formulation/Core revision.

The controlled permissive P2 policy missed two closure insufficiency witnesses in the evaluated set.

### H-EFM-03 — Formulation-Relative Classification Stability

**SUPPORTED FOR EVALUATED FORMULATIONS.**

The matched electrocaloric family produced different outcomes because the frozen formulations had different state/closure requirements:

```text
S1 reduced equilibrium      → D0 / U0
S2-E external P/history     → D0 after U1 exchange enrichment
S2-T P in thermo closure    → D1 formulation revision
```

The classification change therefore followed formulation facts rather than the mechanism name `electrocaloric`.

### H-EFM-04 — Distinctness from RQ-ISO-001

**SUPPORTED FOR EVALUATED FORMULATIONS.**

RQ-EFM-001 produced four cumulative pre-RQ-ISO admissibility decisions (`M-D1 = 4`). These decisions answer a logically prior question:

> Is this formulation admissible as a Core-preserving / ordinary participation case at all under the declared thermodynamic state and exchange contract?

RQ-ISO-001 begins after ordinary-extension status is already accepted and then evaluates authority/non-promotion consequences.

The S2-E and S2-T pair is especially discriminating:

- S2-E first requires exchange-contract repair, after which ordinary/Core-preserving participation is admissible;
- S2-T requires formulation revision before ordinary-extension authority rules can be meaningfully applied.

Therefore the evaluated RQ-EFM-001 procedure does not collapse into merely applying RQ-ISO-001 to all participating state.

---

## 8. Relationship to RQ-ISO-001

The two research contributions are complementary but not interchangeable.

### RQ-EFM-001

Primary question:

> **Given a frozen thermodynamic formulation and scope, is a field-driven mechanism admissible as Core-preserving participation under the declared exchanges, or does thermodynamic completeness require exchange enrichment, formulation/Core revision, or scope narrowing?**

Primary concern:

- physical/formulation admissibility;
- closure sufficiency;
- update sufficiency;
- honest exchange boundaries;
- distinction between external/mechanism state and required thermodynamic governing state.

### RQ-ISO-001

Primary question:

> **Once a mechanism is accepted as an ordinary extension, what happens when its persistent state remains extension-owned rather than being promoted into authoritative Core State?**

Primary concern:

- fixed semantic/Core-state authority;
- non-promotion of ordinary extension-local state;
- Core completeness under ordinary extension;
- Core modification and evidence re-execution consequences.

### Combined ordering

The supported research ordering is therefore:

```text
Physical mechanism / selected formulation
        ↓
RQ-EFM-001 admissibility gate
        ↓
D0 / ordinary Core-preserving participation?
        ↓ yes
RQ-ISO-001 authority / non-promotion boundary
        ↓
Extension implementation and evidence obligations
```

If RQ-EFM-001 returns D1, the correct path is formulation/Core revision or scope narrowing, not ordinary-extension treatment.

---

## 9. Research Contribution Statement

### Full bounded statement

> **Within the evaluated thermodynamic-framework scope, ThermoCore formalizes and evaluates a formulation-relative extension-admissibility decision procedure that freezes thermodynamic state, formulation, scope, and declared exchanges; tests instantaneous closure and update sufficiency; permits physically meaningful exchange enrichment without hidden state serialization; preserves semantically separate mechanism/cross-domain governing state when the thermodynamic formulation remains complete; and requires explicit thermodynamic formulation/Core revision or scope narrowing when sufficiency cannot be preserved. A pre-registered S0-S4 evaluation supports false-promotion avoidance, false-isolation detection, formulation-relative classification, and distinct pre-RQ-ISO admissibility decisions for the evaluated formulations.**

### Concise paper/thesis wording

> **ThermoCore contributes a bounded formulation-relative admissibility gate for field-driven thermodynamic extensions, distinguishing Core-preserving exchange coupling from cases that require thermodynamic formulation revision, and evaluates the gate through pre-registered source, caloric, thermoelectric, and thermoelastic formulation cases.**

### Contribution type

**Architectural operationalization and evaluation**, not new thermodynamic physics.

---

## 10. Safe Claims and Prohibited Claims

| Claim | Status | Reason |
|---|---|---|
| The bounded survey supports a reusable-framework Research Gap candidate around formulation-relative extension admissibility | **Allowed with bounded-survey qualifier** | Evidence v0.1-v0.3 + final consequence test |
| H-EFM-01..04 are supported for the evaluated formulations | **Allowed** | Pre-registered S0-S4 results |
| Same mechanism family may receive different admissibility outcomes under different formulations | **Allowed for evaluated electrocaloric formulations** | S1 / S2-E / S2-T |
| Strong bidirectional coupling does not by itself require Thermodynamic State merger | **Allowed as an evaluated architectural result, not universal theorem** | S3 + prior-art support |
| Some insufficient exchange contracts can be repaired by physical exchange enrichment without state promotion | **Allowed for evaluated S2-E case** | Valid U1 enrichment + anti-smuggling PASS |
| Some selected formulations require explicit thermodynamic formulation revision | **Allowed for evaluated S2-T/S4 formulations** | Test C witnesses |
| ThermoCore invented generalized work pairs | **Prohibited** | Prior art |
| ThermoCore invented internal variables / history state | **Prohibited** | Prior art |
| ThermoCore invented state-space sufficiency or minimal-state reasoning | **Prohibited** | Prior art |
| ThermoCore invented formulation-relative thermodynamic state | **Prohibited** | Prior art |
| ThermoCore is the first framework ever to implement this exact rule | **Not established / prohibited** | No exhaustive novelty or priority proof |
| All electrocaloric mechanisms are D0 or D1 | **Prohibited** | Classification is formulation-relative |
| All thermoelectric coupling is D0 | **Prohibited** | Only the frozen S3 formulation was evaluated |
| All thermoelastic formulations require D1 | **Prohibited** | Only the frozen S4 formulation was evaluated |
| The gate is universally correct across arbitrary physics domains | **Prohibited** | Out of evidence scope |
| RQ-EFM-001 proves numerical accuracy, physical validation, or performance superiority | **Prohibited** | Evaluation is architectural/classification evidence |
| RQ-EFM-001 eliminates the need for extension-specific evidence | **Prohibited** | Admissibility does not replace extension verification/validation obligations |

---

## 11. What the Result Does Not Establish

The experiment does not establish:

- an exhaustive taxonomy of all field-driven material-response mechanisms;
- universal minimal Thermodynamic State for all formulations;
- universal sufficiency of specific enthalpy;
- automatic D0/D1 classification from a mechanism name;
- numerical stability or convergence of coupled solvers;
- physical validation of the synthetic witness parameters;
- runtime performance benefits;
- correctness of arbitrary external solver exchange data;
- global novelty or historical priority; or
- third-party reproduction / peer-review endorsement.

The S0-S4 harnesses are deterministic architectural witness constructions, not calibrated physical-validation models.

---

## 12. Normative Readiness

### Research-governance decision

**GO FOR FUTURE NORMATIVE CONSIDERATION.**

RQ-EFM-001 has now completed the ThermoCore evidence path required before a new normative decision may be considered:

```text
Research
→ Evidence
→ Research Gap
→ Pre-registration
→ Evaluation
→ Final Disposition
```

The evidence is therefore sufficient to open a **separate future Framework Specification task** asking whether the Framework should normatively define a formulation-relative extension-admissibility rule or related extension-boundary criterion.

### What this does not mean

This disposition does **not** itself modify the Framework Specification.

A future normative task must still:

- identify the exact parent specification authority;
- determine whether the rule belongs in `Framework_Principles`, `Thermodynamic_State`, `Extension_Boundary`, `Framework_Interfaces`, `Framework_Conformance`, or a new refinement document;
- avoid duplicating the already normative RQ-ISO-style extension ownership rules;
- preserve formulation relativity rather than hard-code specific mechanisms;
- specify only evidence-supported semantics, not experimental harness details;
- define normative terms such as declared exchange, thermodynamic closure sufficiency, formulation revision, and scope narrowing with care;
- assess backward compatibility with v1.0.0 before any future software/specification release.

No retroactive claim is made that v1.0.0 already normatively specifies the complete RQ-EFM-001 procedure.

---

## 13. Publication and Versioning Implication

RQ-EFM-001 is now a completed research result on `main`, but by itself it does not require an immediate ThermoCore software release.

A future Framework version becomes justified only if a later normative task actually changes:

- Framework Specification semantics;
- required conformance behavior;
- public Framework interfaces;
- production reference implementation; or
- another versioned Framework-level contract.

Until such a normative change is accepted, the current result remains a research contribution and evidence baseline rather than a new software-version feature.

The frozen v1.0.0 DOI/release remains unchanged.

---

## 14. Final RQ-EFM-001 v0.1 Closure

RQ-EFM-001 v0.1 is **CLOSED AS A COMPLETED BOUNDED RESEARCH QUESTION** after merge of this final disposition.

Final classifications:

```text
Research Gap:
SUPPORTED WITHIN BOUNDED SURVEY AND EVALUATED THERMODYNAMIC-FRAMEWORK SCOPE

Research Contribution:
BOUNDED ARCHITECTURAL OPERATIONALIZATION + PRE-REGISTERED CONSEQUENCE EVALUATION

H-EFM-01:
SUPPORTED FOR EVALUATED FORMULATIONS

H-EFM-02:
SUPPORTED FOR EVALUATED FORMULATIONS

H-EFM-03:
SUPPORTED FOR EVALUATED FORMULATIONS

H-EFM-04:
SUPPORTED FOR EVALUATED FORMULATIONS

Novelty / First-Ever Priority:
NOT ESTABLISHED

Normative Readiness:
GO FOR SEPARATE FUTURE FRAMEWORK-SPECIFICATION CONSIDERATION

Framework Specification Change in This Artifact:
NONE

ThermoCore v1.0.0 Retroactive Change:
NONE
```

Further S5/S6-style scenario accumulation is not required for the v0.1 claim. Additional scenarios should be opened only if they test a stronger claim, challenge one of the current boundaries, support external replication, or provide evidence for a future normative specification decision.

---

## 15. Closure Statement

The RQ-EFM-001 evidence does not establish that ThermoCore discovered new thermodynamic physics or a globally unprecedented state-sufficiency principle.

It does support a narrower architectural research result: **a reusable thermodynamic framework can make extension admissibility formulation-relative and evidence-testable rather than classifying physical mechanisms by name or by participation alone.** In the evaluated formulations, this separates valid source/work/cross-domain exchange from hidden thermodynamic incompleteness, permits exchange enrichment where sufficient, and forces formulation/Core revision where the existing thermodynamic closure is genuinely incomplete.

That bounded result is sufficiently supported to close RQ-EFM-001 v0.1 and to permit, but not require, a future normative ThermoCore specification task.