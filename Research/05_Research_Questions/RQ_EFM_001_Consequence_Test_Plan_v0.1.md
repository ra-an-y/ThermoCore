# RQ-EFM-001 Consequence Test Plan v0.1

Status: **Draft — Pre-registered Research Evaluation Design**  
Research Question: **RQ-EFM-001 — External Energy / Physical Field–Driven Material Response**  
Candidate Gap: **Formulation-Relative Thermodynamic Extension Admissibility Boundary**  
Date: **2026-08-23**  
Tracking: GitHub Issue #94

---

## 1. Purpose

This document pre-registers the bounded consequence/classification evaluation for RQ-EFM-001 before scenario implementation or measurement.

The evaluated candidate is not a universal coupling taxonomy. Prior evidence already established generalized work-pair thermodynamics, internal variables, formulation-relative thermodynamic-state selection, state-space sufficiency, multi-domain energy interfaces, and generic multiphysics coupling as prior art.

The remaining candidate concerns an architectural decision gate:

> **For a frozen thermodynamic formulation and claimed scope, may field-driven mechanism/cross-domain state remain outside authoritative Thermodynamic State while participating through declared exchanges, or is the selected thermodynamic state-space/formulation itself insufficient and therefore in need of explicit revision or scope narrowing?**

The evaluation is designed to test that gate, including the possibility that RQ-EFM-001 collapses into an application of RQ-ISO-001 plus established thermodynamic state-space theory.

This document is non-normative. It does not modify Framework Specification, production implementation, Validation, Performance, Framework Conformance, v1.0.0, or the completed RQ-ISO-001 disposition.

---

## 2. Evidence and Research Dependencies

This plan depends on:

- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.1.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.2.md`
- `Research/01_Evidence_Matrix/External_Field_Coupling_Evidence_Matrix_v0.3.md`
- `Research/04_Research_Gap/RQ_EFM_001_Research_Gap_Analysis_v0.1.md`
- `Research/04_Research_Gap/RQ_ISO_001_Final_Research_Gap_Disposition_v0.1.md`
- `Documentation/Thermodynamic_Formulation.md`
- `Documentation/Framework_Specification/Thermodynamic_State.md`
- `Documentation/Framework_Specification/Extension_Boundary.md`

The broad evidence survey is considered closed. New prior-art sources may be introduced only if they directly falsify or satisfy the surviving candidate; they shall not be used to change decision rules after scenario results are known.

---

## 3. Research Hypotheses

The hypotheses are inherited from the bounded Gap Analysis and remain untested at pre-registration.

### H-EFM-01 — False-Promotion Avoidance

For evaluated `D0` formulations, the formulation-relative sufficiency gate will allow separately governed mechanism/cross-domain state to remain outside mandatory Thermodynamic State when declared exchanges are sufficient, without omitting information required for correct thermodynamic evolution.

### H-EFM-02 — False-Isolation Detection

For evaluated cases in which the proposed state/exchange abstraction is insufficient, the sufficiency gate will identify a closure or update witness and require exchange enrichment, thermodynamic formulation/Core revision, or scope narrowing rather than accepting an incomplete `D0` classification.

### H-EFM-03 — Formulation-Relative Classification Stability

For the same physical mechanism family represented by different frozen formulations, the gate will change classification only when formulation-relative state/closure requirements change, rather than because of the mechanism name itself.

### H-EFM-04 — Distinctness from RQ-ISO-001

The RQ-EFM-001 gate will produce at least one non-trivial admissibility/reclassification decision that must occur **before** the RQ-ISO-001 ordinary-extension authority rule can be applied.

If H-EFM-04 is not supported, RQ-EFM-001 shall be reclassified as an application/specialization of RQ-ISO-001 and established thermodynamic state-space theory rather than a distinct architecture contribution.

---

## 4. Experimental Independent Variable

The independent variable is the **admissibility/classification policy** applied to the same frozen physical formulation package.

It is not:

- code modularity;
- programming language;
- solver speed;
- monolithic versus partitioned implementation by itself;
- amount of source code;
- number of files;
- memory footprint; or
- numerical accuracy of a production multiphysics solver.

All policies shall receive the same frozen formulation facts, state definitions, allowed exchanges, required outputs, and scope constraints.

---

## 5. Controlled Classification Policies

### 5.1 Policy R — Formulation-Relative Sufficiency Gate

Policy R applies the following sequence:

1. freeze thermodynamic formulation and scope;
2. identify authoritative Thermodynamic State;
3. identify external/mechanism/cross-domain variables proposed to remain outside Thermodynamic State;
4. define the declared exchange contract for one thermodynamic update;
5. test instantaneous thermodynamic closure sufficiency;
6. test update sufficiency under the declared exchange contract;
7. enrich the exchange contract if a semantically honest external exchange can resolve insufficiency without changing thermodynamic state identity/closure;
8. classify `D0` only after closure and update sufficiency hold;
9. classify `D1`, revise formulation/Core, or narrow scope if thermodynamic state-space/closure itself remains insufficient.

Policy R does not assume that fewer Core quantities are better. It prioritizes physical completeness and semantic consistency.

### 5.2 Policy P1 — Participation-Promotion Control

Policy P1 is a conservative shared-state control:

> persistent mechanism/cross-domain quantities that directly participate in thermodynamic evolution are promoted into the shared mandatory thermodynamic state schema.

P1 is allowed to remain modular in code and may retain separate solver responsibilities. Its distinguishing policy is state promotion, not intentional monolithic design.

P1 exists to expose **false-promotion pressure**. It is not presented as a named prior framework or as an intentionally defective architecture.

### 5.3 Policy P2 — Exchange-Only Permissive Control

Policy P2 applies the opposite permissive rule:

> a mechanism/cross-domain quantity may remain outside Thermodynamic State whenever some source, flux/work, property, boundary, or input interface can be written; P2 does not require an explicit state/transition sufficiency witness test before accepting that abstraction.

P2 may enrich interfaces where an implementation naturally requires it, but it does not apply Policy R's formal closure/update witness criterion.

P2 exists to expose **false-isolation pressure**. It is a controlled decision policy, not a claim about a named prior framework.

### 5.4 Control Interpretation Rule

P1 and P2 are stress controls around the candidate decision boundary. They shall not be used to claim universal framework superiority.

A positive RQ-EFM result requires correct and reproducible classification behavior, not merely a lower state count than P1 or a stricter boundary than P2.

---

## 6. Frozen Meaning of `D0` and `D1`

### D0 — Core-Preserving for the Selected Thermodynamic Formulation

A scenario may be classified `D0` when:

- the authoritative Thermodynamic State remains sufficient to identify the instantaneous thermodynamic condition required by the frozen formulation;
- the frozen thermodynamic evolution relation remains complete when all external/mechanism effects are supplied through semantically valid declared exchanges;
- omitted non-Core variables are evolved/owned elsewhere or are derived/configurational as declared;
- no omitted variable must be reinterpreted inside the thermodynamic responsibility as an undeclared thermodynamic coordinate; and
- the larger coupled system may still require other governing solvers.

`D0` does **not** mean the external mechanism is optional to the larger application or that coupling is weak.

### D1 — Thermodynamic Formulation/Core Revision Required

A scenario shall be classified `D1` when, within the claimed scope:

- the current authoritative Thermodynamic State does not uniquely determine required instantaneous thermodynamic closure; or
- the current thermodynamic governing balance/evolution relation lacks a thermodynamic work/conservation/closure responsibility required by the selected formulation; and
- that insufficiency cannot be resolved by a semantically honest declared exchange from an external/mechanism responsibility without changing the thermodynamic state identity or governing formulation.

`D1` does **not** require merging every external-domain variable into Thermodynamic State. The revision may instead add the required thermodynamic coordinate/closure/responsibility while other domain state remains separately governed.

---

## 7. Declared Exchange Contract

A central pre-registration requirement is to avoid a false witness caused merely because another solver will supply a different value on the next update.

For one thermodynamic update interval `[t_n, t_{n+1}]`, define:

- `S_n` — complete authoritative Thermodynamic State at the start of the update;
- `M_n` — applicable frozen material/configuration information;
- `X_n` — the **complete declared thermodynamic exchange packet for that update interval**;
- `Delta t` — update interval;
- `S_{n+1}` — thermodynamic state after the update.

The candidate Core-preserving transition has the abstract form:

`S_{n+1} = F(S_n, M_n, X_n, Delta t)`

`X_n` may contain, where semantically appropriate:

- integrated or averaged power deposition;
- boundary heat flux;
- generalized work increment;
- externally solved field value required as a constitutive input for the interval;
- externally supplied effective material parameter;
- cross-domain flux contribution; or
- another explicitly defined exchange required by the frozen formulation.

The contract shall state sampling/integration timing and whether a quantity is supplied at the beginning, end, midpoint, or as an interval-integrated value.

### Future-Exchange Rule

The Thermodynamic Core is **not** required to predict `X_{n+1}` if another governing responsibility is explicitly assigned to compute and supply `X_{n+1}` before the next thermodynamic update.

Therefore:

> two coupled states that differ only because they will produce different `X_{n+1}` do not constitute a witness against the current `X_n` contract.

A valid update witness must show different physically required `S_{n+1}` under the **same complete `S_n`, `M_n`, `X_n`, and `Delta t`**.

---

## 8. Two Sufficiency Tests

Policy R separates instantaneous closure from interval evolution.

### 8.1 Test C — Instantaneous Closure Sufficiency

Construct or identify two physically admissible full states `Z_a` and `Z_b` such that:

- their claimed Thermodynamic State `S_n` is identical;
- their applicable material/configuration `M_n` is identical; and
- omitted mechanism/cross-domain quantities differ.

Ask whether the frozen thermodynamic formulation requires different **instantaneous thermodynamic closure** for the two states, such as different:

- temperature recovered from the same claimed state coordinate;
- equilibrium phase condition;
- thermodynamic potential required by the selected formulation;
- state equation value;
- caloric thermodynamic property required for state identity; or
- other instantaneous thermodynamic quantity that the frozen formulation defines as part of its state closure.

If yes, the claimed Thermodynamic State is not sufficient for that formulation. The case cannot be accepted as `D0` merely by renaming the omitted coordinate a property input.

### 8.2 Test U — Update Sufficiency

Construct or identify two physically admissible full states satisfying:

- same `S_n`;
- same `M_n`;
- same complete declared `X_n` under the frozen interval contract;
- same `Delta t`; and
- different omitted mechanism/cross-domain state.

If the physically required `S_{n+1}` differs, the declared exchange abstraction is insufficient.

The next action is not automatically `D1`. Policy R shall first determine whether a **semantically honest exchange enrichment** can make the transition unique while preserving thermodynamic state identity and governing responsibility.

Possible outcomes:

- `U0`: existing exchange sufficient;
- `U1`: exchange enrichment required but formulation remains `D0`;
- `U2`: thermodynamic formulation/Core revision or scope narrowing required (`D1`).

### 8.3 Anti-Smuggling Rule

An exchange enrichment shall not qualify as `D0` if it merely serializes hidden persistent state into an opaque payload that the thermodynamic responsibility must internally evolve or interpret as a thermodynamic coordinate.

A valid exchange must have an explicit physical/interface meaning such as source, flux, work, boundary value, constitutive input, or another declared cross-domain transfer whose producing responsibility remains external.

---

## 9. Scenario Freeze Requirements

Before executing any scenario, Phase A shall freeze for that scenario:

- physical/model scope;
- governing equations or abstract update relations;
- state variables and their physical roles;
- material assumptions;
- allowed declared exchanges;
- update timing semantics;
- required observable outputs;
- excluded physics;
- expected evidence source/derivation for each classification fact; and
- any parameter values needed for a deterministic witness.

Scenario facts shall be frozen **before** applying R/P1/P2 or collecting metric results.

If pre-execution review shows that a scenario does not represent its intended research role, it may be replaced or reclassified before measurement. The replacement and reason shall be versioned. It shall not be silently changed after results are available.

---

## 10. Scenario Matrix

### S0 — Externally Supplied Joule / Optical Deposition Control

**Research role:** clean `D0` source/deposition control.

Frozen family requirements:

- the electrical/optical field solution is outside the selected thermodynamic formulation;
- the thermodynamic side receives an interval-defined deposited energy or power `X_n`;
- no omitted electrical/optical state alters instantaneous thermodynamic closure under the selected scope;
- any later difference in electrical/optical state is the responsibility of the external solver and affects thermodynamics only through later supplied exchanges.

Required control check:

For equal `S_n`, `M_n`, deposited-energy `X_n`, and `Delta t`, different external field states shall not require a different thermodynamic update within the frozen scope.

Purpose:

- verify the future-exchange rule;
- expose unnecessary state promotion under P1;
- prevent the sufficiency gate from treating every external field as Core state.

### S1 — Reduced Equilibrium Caloric Formulation

**Research role:** reduced/equilibrium field-dependent thermodynamic response.

Preferred mechanism family: electrocaloric or magnetocaloric.

Frozen family requirements:

- external field trajectory or generalized work exchange is prescribed/supplied;
- response is represented by a frozen equilibrium or reduced constitutive/free-energy relation;
- no kinetic/hysteretic internal variable is required within the selected model scope;
- the allowed exchange packet shall state whether it contains field value, field increment, generalized work, or an equivalent physically interpretable quantity.

Purpose:

- test whether generalized field dependence can remain `D0` under a reduced formulation;
- establish one half of the formulation-relative S1/S2 comparison.

No `D0` result is guaranteed. If instantaneous thermodynamic closure actually requires an additional coordinate under the chosen formulation, the scenario shall be reclassified before measurement or shall become a `D1` result.

### S2 — Stateful / Hysteretic Caloric Formulation

**Research role:** same or closely matched mechanism family as S1 with explicit history/order dynamics.

Preferred mechanism family: the same electrocaloric or magnetocaloric family selected for S1.

Frozen family requirements:

- at least one polarization, magnetization, phase/order fraction, or related internal variable follows an explicit history/evolution rule;
- that variable affects entropy, energy, temperature response, or generalized work under the selected formulation;
- at least two admissible internal states can share relevant macroscopic thermal conditions.

S2 shall test two possible architectural treatments where physically meaningful:

- `S2-E`: the order/history state is evolved by a mechanism-specific responsibility which supplies a complete energy/work/constitutive exchange to the thermodynamic update;
- `S2-T`: the selected thermodynamic formulation itself treats the order/history coordinate as necessary for thermodynamic state/closure.

`S2-E` may remain `D0` if the enriched exchange is physically sufficient and anti-smuggling rules are satisfied.

`S2-T` is expected to create `D1` pressure relative to an enthalpy-only thermodynamic state, but the final classification shall follow the frozen formulation evidence rather than this expectation.

Purpose:

- test hidden-state detection;
- test exchange enrichment versus true state-space revision;
- test H-EFM-03 using the same mechanism family as S1.

### S3 — Thermoelectric Cross-Domain Governing Coupling

**Research role:** strong bidirectional governing coupling without automatic thermodynamic-state merger.

Frozen family requirements:

- electrical potential/current or equivalent electrical governing state is evolved by an electrical responsibility;
- thermal state is evolved by the thermodynamic/thermal responsibility;
- Joule, Peltier, Seebeck, and/or Thomson terms are represented according to the selected bounded model;
- the exchange packet identifies the source/flux/work information required by the thermal update;
- electrical state remains semantically electrical unless Test C shows the selected thermodynamic formulation itself requires otherwise.

Purpose:

- test `B4` cross-domain governing state;
- test that bidirectional/implicit coupling alone does not trigger D1;
- test exchange enrichment under strong coupling;
- expose over-promotion under P1 where applicable.

The larger coupled-system problem may require both solvers even if the thermal authority classification is `D0`.

### S4 — Mechanical / Pressure-Dependent True Formulation-Revision Case

**Research role:** positive `D1` boundary control.

Preferred family:

- pressure-dependent/compressible thermodynamics;
- thermoelastic/elastocaloric formulation with state-dependent mechanical work and deformation; or
- another mechanical-field formulation incompatible with the current fixed-mass, fixed-density, no-mechanical-work reference assumptions.

Frozen family requirements:

- the selected physical scope requires at least one thermodynamic closure/state relation not uniquely determined by the current reference `SpecificEnthalpy` state alone;
- the missing quantity cannot be represented merely as an interval energy source without losing instantaneous thermodynamic closure or required work/conservation semantics;
- the case is supported by an explicit formulation, not by mechanism-name assertion.

Purpose:

- verify that Policy R does not preserve Core isolation when the selected thermodynamic formulation is genuinely incomplete;
- provide a positive Test C and/or `U2` case;
- test false-isolation pressure under P2.

If no physically defensible S4 formulation satisfying these requirements can be frozen, execution shall stop and H-EFM-02/H-EFM-04 shall not receive a positive verdict from an invented counterexample.

---

## 11. Functional Equivalence and Fairness

For each scenario, all policies shall receive the same:

- frozen physical equations/relations;
- initial full physical state;
- material parameters;
- boundary/driving history;
- timestep/update contract;
- required observable outputs; and
- numerical tolerance where a numerical witness is used.

P1 may carry more state centrally. P2 may carry less. That difference is the intended policy variable.

No policy may omit required functionality or alter the physical scenario to make its classification easier.

A policy result that produces a different physical model is not a valid comparison and shall be marked `INCOMPARABLE / RECLASSIFICATION REQUIRED` rather than scored as better or worse.

---

## 12. Baseline and Manifest Freeze

After this plan is merged and before any scenario execution, Phase A shall record:

- exact repository baseline commit;
- relevant Framework normative artifacts;
- current reference formulation artifact;
- relevant implementation artifacts only where used by a research harness;
- current authoritative Thermodynamic State schema;
- current allowed exchange/interface semantics used by the research model;
- RQ-ISO-001 boundary baseline; and
- all scenario formulation/evidence packages.

The frozen baseline for RQ-EFM-001 shall be the post-plan `main` commit, not the historical v1.0.0 release commit.

The v1.0.0 release remains an archival publication reference and is not retroactively changed by this research.

---

## 13. Metrics

### 13.1 False-Promotion Metrics

`M-F1 — Promoted mandatory Thermodynamic-State quantities`

Count quantities that a policy classifies as mandatory authoritative Thermodynamic State beyond the frozen baseline.

`M-F1` is semantic quantity count, not byte size.

`M-FP — False-promotion findings`

For each promoted quantity, record whether the frozen formulation demonstrates that instantaneous closure and update sufficiency hold without promoting that quantity when a semantically valid declared exchange is used.

A promotion is counted as false only when this sufficiency is demonstrated; smaller state is not assumed better by default.

### 13.2 False-Isolation Metrics

`M-F2 — Valid insufficiency witnesses`

Count valid Test C or Test U witnesses under the frozen contract.

`M-FI — Missed insufficiency witnesses`

Count valid witnesses that a policy fails to detect before accepting `D0`.

A policy that later fails during implementation does not erase the earlier missed classification.

### 13.3 Exchange-Enrichment Metrics

`M-F3 — Required exchange enrichments`

Count semantically distinct additions required to make the thermodynamic update sufficient while preserving state identity.

Examples include adding:

- interval work;
- boundary flux;
- externally solved constitutive field value; or
- another physically defined exchange.

Opaque serialization of hidden persistent state does not qualify.

### 13.4 Formulation-Revision Metrics

`M-F4 — Explicit formulation/Core revisions`

Count cases requiring changes to:

- authoritative thermodynamic state-space;
- thermodynamic work/conservation responsibility;
- state equation/closure; or
- claimed formulation scope.

Scope narrowing shall be recorded separately from an implemented Core revision.

### 13.5 Formulation-Relative Classification Metric

`M-F5 — Same-mechanism formulation-dependent classification changes`

Record whether the S1/S2 mechanism family receives different authority outcomes when and only when the frozen formulation/state requirements differ.

The result shall identify which formulation fact caused the change.

### 13.6 Consistency Metrics

`M-K1 — Repeated rule agreement`

Applying the frozen rule table to the same frozen scenario record shall yield the same classification and witness status.

`M-K2 — Post-hoc assumption count`

Count classification decisions requiring an assumption not present in the frozen scenario package.

`M-K3 — Hidden dependency findings`

Count dependencies discovered after initial classification that affect closure, update sufficiency, or state role.

Any `M-K2 > 0` or material `M-K3 > 0` requires explicit review before hypothesis verdicts.

### 13.7 Distinctness Metric

`M-D1 — Pre-ISO admissibility decisions`

Count valid scenarios where the classification gate must decide whether ordinary/Core-preserving participation is physically admissible **before** the RQ-ISO-001 non-promotion rule can be meaningfully applied.

A case contributes to `M-D1` only if:

- the decision concerns physical/formulation sufficiency rather than state placement alone; and
- applying RQ-ISO-001 first would presuppose the disputed ordinary-extension classification.

If `M-D1 = 0` across all valid scenarios, H-EFM-04 cannot be supported.

---

## 14. Hidden-Coupling Audit

Every `D0` classification shall be audited for hidden relocation of governing information.

Check for:

- persistent mechanism state serialized into an opaque exchange packet;
- Core-side type checks for specific mechanisms;
- direct Core dependency on concrete external solver state;
- duplicated authoritative state;
- property values that actually require Core-side evolution of hidden variables;
- an interface whose neutral name masks mechanism-specific governing semantics;
- future external exchange values implicitly assumed available before they are produced;
- synchronization or ordering requirements that effectively transfer governing responsibility; and
- a thermodynamic closure that cannot be evaluated without undeclared non-Core state.

A `D0` result fails the audit if Core preservation is achieved only by hiding a required governing dependency.

---

## 15. Hypothesis-to-Metric Mapping

| Hypothesis | Primary metrics | Required scenarios |
|---|---|---|
| H-EFM-01 False-Promotion Avoidance | `M-F1`, `M-FP`, `M-FI` | S0, S1, S3, valid D0 portions of S2 |
| H-EFM-02 False-Isolation Detection | `M-F2`, `M-FI`, `M-F3`, `M-F4` | S2, S4; S3 if witness pressure appears |
| H-EFM-03 Formulation-Relative Stability | `M-F5`, `M-K1`, `M-K2`, `M-K3` | matched S1/S2 mechanism family |
| H-EFM-04 Distinctness from RQ-ISO-001 | `M-D1`, witness/reclassification records | S2, S4 and any other valid pre-ISO gate case |

No composite score shall be derived from these metrics.

---

## 16. Pre-Registered Decision Rules

### 16.1 H-EFM-01 — False-Promotion Avoidance

`SUPPORTED FOR EVALUATED FORMULATIONS` if:

- all valid D0 scenarios classified by R pass Test C, Test U, and hidden-coupling audit;
- R avoids at least one P1 promotion shown unnecessary by the frozen formulation/exchange contract; and
- R has zero missed insufficiency witnesses in those D0 cases.

`PARTIALLY SUPPORTED` if:

- R avoids unnecessary promotion in at least one valid D0 scenario;
- no false-isolation contradiction occurs; but
- at least one other valid D0 scenario shows no distinction from P1 or remains unresolved.

`NOT SUPPORTED` if:

- no valid scenario demonstrates unnecessary promotion avoided by R.

`FALSIFIED / RECLASSIFICATION REQUIRED` if:

- R keeps state outside Core but a valid closure/update witness shows that information is required and no valid exchange resolves it.

### 16.2 H-EFM-02 — False-Isolation Detection

`SUPPORTED FOR EVALUATED FORMULATIONS` if:

- every frozen insufficiency case produces a valid Test C or Test U witness under R;
- R requires the appropriate exchange enrichment, formulation/Core revision, or scope narrowing;
- S4 produces a defensible `D1`/scope-narrowing outcome; and
- no D0 control is falsely rejected merely because future external exchanges can differ.

`PARTIALLY SUPPORTED` if:

- R detects at least one but not all valid insufficiency cases;
- no detected witness is invalid; and
- unresolved cases remain explicitly unresolved.

`NOT SUPPORTED` if:

- R adds no useful detection beyond the frozen controls or all discriminating cases remain unresolved.

`FALSIFIED / RECLASSIFICATION REQUIRED` if:

- R accepts `D0` despite a valid unhandled witness; or
- R's witness rule falsely rejects a D0 control because it requires prediction of a future external exchange outside the current update contract.

### 16.3 H-EFM-03 — Formulation-Relative Classification Stability

`SUPPORTED FOR EVALUATED FORMULATIONS` if:

- the matched S1/S2 mechanism family is classified using the same frozen rule;
- any different D0/D1/exchange-enrichment result is traceable to a specific difference in state/closure/evolution requirements;
- repeated application yields the same result; and
- mechanism name is not used as a decision input.

`PARTIALLY SUPPORTED` if:

- the formulation distinction changes an intermediate state-role/exchange decision but not the terminal authority classification, while remaining internally consistent.

`NOT SUPPORTED` if:

- matched formulations produce no meaningful classification distinction and no contradiction.

`FALSIFIED / RECLASSIFICATION REQUIRED` if:

- classification depends on naming, undocumented modeling preference, or post-hoc assumptions rather than frozen formulation facts.

### 16.4 H-EFM-04 — Distinctness from RQ-ISO-001

`SUPPORTED FOR EVALUATED FORMULATIONS` if:

- `M-D1 >= 1`;
- at least one valid scenario requires a physical/formulation admissibility or exchange-sufficiency decision before ordinary-extension authority can be evaluated; and
- that decision changes the allowed architecture outcome by requiring exchange enrichment, D1/Core revision, or scope narrowing rather than merely choosing where already-admissible state is stored.

`PARTIALLY SUPPORTED` if:

- `M-D1 >= 1` but the distinct consequence is limited to interface enrichment and does not produce a D1/scope-revision case beyond what RQ-ISO-001 could plausibly infer.

`NOT SUPPORTED` if:

- `M-D1 = 0`; or
- every valid decision can be reproduced by assuming ordinary extension admissibility and applying RQ-ISO-001 plus standard state-space reasoning with no additional architectural gate.

`FALSIFIED / RECLASSIFICATION REQUIRED` if:

- the only claimed distinction is terminology; or
- direct prior art is found that already operationalizes the full candidate boundary and eliminates the proposed gap.

---

## 17. Scenario Validity before Hypothesis Scoring

A scenario shall be marked `VALID FOR SCORING` only if:

- its physical/model equations are frozen;
- required state variables and exchanges are explicit;
- intended control/discriminating role is supported by evidence;
- no policy receives less required physical functionality;
- witness construction is deterministic where used; and
- any numerical tolerance is fixed before result collection.

Otherwise it shall be marked:

- `RECLASSIFIED BEFORE MEASUREMENT`;
- `INCOMPARABLE`;
- `OUT OF SCOPE`; or
- `INVALID SCENARIO`.

Invalid scenarios shall not be replaced after seeing hypothesis results unless the replacement is versioned as a new evaluation revision.

---

## 18. Required Result Records

Execution shall produce versioned records containing at least:

1. frozen repository and specification/formulation baseline;
2. policy definitions exactly as executed;
3. scenario evidence/facts package;
4. declared exchange contract for each scenario;
5. Test C records;
6. Test U records;
7. exchange-enrichment decisions;
8. state-role and authority classification table;
9. P1 promotion records;
10. P2 isolation decisions;
11. hidden-coupling audit;
12. `M-F1` through `M-F5` results;
13. `M-K1` through `M-K3` results;
14. `M-D1` distinctness results;
15. H-EFM-01 through H-EFM-04 verdicts; and
16. negative/null/reclassification findings.

Historical result files for a frozen baseline shall not be silently rewritten to improve outcomes. Corrections shall be versioned and explained.

---

## 19. Execution Sequence

### Phase A — Freeze

1. freeze the post-plan `main` baseline commit;
2. freeze relevant Framework/reference-formulation artifacts;
3. freeze R/P1/P2 policy definitions;
4. freeze the scenario formulation packages;
5. freeze state-role and exchange definitions;
6. freeze update timing semantics and numerical tolerances;
7. perform scenario-validity review before metric collection.

### Phase B — Controls

Execute:

- S0 source/deposition D0 control;
- S4 positive formulation-revision boundary control if its formulation package is already valid.

Purpose: verify Test C/Test U mechanics against obvious boundary cases before discriminating cases.

### Phase C — Formulation Pair

Execute matched:

- S1 reduced/equilibrium caloric formulation;
- S2 stateful/hysteretic formulation.

Purpose: test formulation-relative classification and exchange enrichment.

### Phase D — Cross-Domain Governing Coupling

Execute S3 thermoelectric case.

Purpose: test strong bidirectional coupling, separate domain state, and P1 false-promotion pressure.

### Phase E — Verdict

Apply the pre-registered decision rules without changing thresholds or scenario definitions.

If H-EFM-04 is not supported, preserve that result and reclassify RQ-EFM-001 accordingly rather than adding post-hoc scenarios solely to obtain distinctness.

---

## 20. Stop / Reclassification Conditions

Execution shall stop, narrow, or version a revised plan if:

- no defensible S4 true formulation-revision scenario can be defined;
- S1/S2 cannot be matched closely enough to test formulation-relative rather than mechanism-name classification;
- the exchange contract cannot be made temporally precise;
- Policy R requires undefined subjective judgments not represented in the frozen rule;
- P1 or P2 cannot be implemented/interpreted with the same physical functionality;
- the anti-smuggling rule cannot distinguish genuine exchange enrichment from hidden state relocation;
- direct prior art is found satisfying the full candidate gap;
- a scenario's intended D0/D1 status was implicitly decided by naming rather than formulation evidence; or
- implementation results appear before the required scenario freeze.

Negative results are valid research outcomes and shall not trigger threshold redesign.

---

## 21. Claims This Plan Cannot Establish

Even a fully positive evaluation cannot establish by itself that:

- the admissibility boundary is globally novel;
- ThermoCore is the first framework to use such a boundary;
- all field-driven mechanisms can be classified universally by these scenarios;
- the four-axis analysis representation is novel;
- generalized work-pair thermodynamics, internal variables, state-space sufficiency, or energy-port coupling are ThermoCore contributions;
- R is universally superior to other frameworks or decision policies;
- fewer Core variables always imply lower total memory or complexity;
- current ThermoCore v1.0.0 implements electrocaloric, magnetocaloric, thermoelectric, or thermo-mechanical multiphysics;
- the reviewed mechanism models are physically validated by existing ThermoCore Validation evidence; or
- Framework Specification changes are justified automatically.

A positive result can support only a bounded, formulation/scenario-specific engineering consequence and possible research contribution subject to later final disposition.

---

## 22. Pre-Registered Current Status

| Item | Status before execution |
|---|---|
| Broad RQ-EFM evidence survey | Closed at v0.3 |
| Research Gap | Evidence-supported candidate — bounded survey only |
| Consequence-test design | Draft / pre-registered |
| H-EFM-01 | Untested |
| H-EFM-02 | Untested |
| H-EFM-03 | Untested |
| H-EFM-04 | Untested |
| S0-S4 | Not executed |
| Novelty / priority | Not established |
| Framework Specification change | Not authorized |
| Production implementation change | Not authorized |

---

## 23. Conclusion

RQ-EFM-001 testing shall evaluate a classification gate rather than invent another coupling taxonomy.

The decisive distinction is not whether a mechanism has a source term, a property dependency, internal state, or strong bidirectional coupling. Those patterns are established and may coexist.

The candidate research question is whether a reusable thermodynamic framework can make a **reproducible formulation-relative admissibility decision**:

- preserve external/mechanism/cross-domain state outside mandatory Thermodynamic State when closure and interval evolution remain complete through declared exchanges;
- enrich the exchange contract when more information must cross the boundary but state identity remains valid; and
- require thermodynamic formulation/Core revision or scope narrowing when the selected state-space/closure is genuinely insufficient.

The experiment must also determine whether this gate adds anything beyond RQ-ISO-001. Failure to demonstrate that distinctness is an accepted outcome.
