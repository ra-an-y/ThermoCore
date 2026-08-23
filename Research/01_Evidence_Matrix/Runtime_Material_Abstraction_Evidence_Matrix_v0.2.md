# Runtime Material Abstraction Evidence Matrix v0.2

Version: 0.2  
Status: Completed Matched-Scenario Stress Test — Non-Normative Research Evidence  
Research Question: RQ-RMA-001 — Runtime Material Abstraction Boundary  
Tracking Issue: #139  
Date: 2026-08-23

---

## 1. Objective

This artifact performs the focused matched-scenario stress test requested after `Runtime_Material_Abstraction_Evidence_Matrix_v0.1.md`.

The purpose is not to collect another broad set of examples of lookup tables, buffers, caches, compiled kernels, or material classes. The purpose is to determine whether the surviving candidate **Configuration-Derivative Identity Property** contributes an independent architecture decision criterion after the following existing boundaries are applied:

- Runtime State / Configuration / Representation separation;
- RQ-ISO-001 state-authority and non-promotion boundary;
- RQ-EFM-001 formulation-relative thermodynamic sufficiency boundary;
- existing Framework ownership and conformance semantics;
- implementation/performance treatment for storage, layout, device, cache, and execution-only concerns.

A negative result is valid and shall not be narrowed indefinitely to preserve contribution symmetry.

This artifact is non-normative. It does not change Framework Specification, implementation, Verification, Validation, Performance, or the frozen v1.0.0 release.

---

## 2. Frozen Evidence Basis

The v0.2 stress test uses the evidence classification already established in v0.1.

v0.1 found direct or strong antecedents for all of the following broad distinctions:

- reusable medium/material/model definition versus current thermodynamic state;
- computed current material properties versus retained previous-history properties;
- constitutive parameters versus internal/history variables;
- compiled material behavior with separate material properties/parameters and state variables;
- tabular/LUT construction, persistence, runtime loading, and rebuild;
- backend or device specialization without automatic semantic reclassification;
- formulation/model selection and formulation-specific thermophysical data.

The v0.2 task therefore does **not** treat any of these broad distinctions as novel candidates.

The external evidence families remain those documented in v0.1:

- Modelica.Media;
- MOOSE Materials / Kokkos Materials;
- OpenFOAM thermophysical models;
- Cantera;
- CoolProp tabular backends;
- dolfinx_materials / MFront / JAX material behaviors;
- NEML constitutive history/internal-variable systems.

Detailed source-level references and prior-art characterizations remain in v0.1 and are not duplicated here.

---

## 3. Candidate Under Stress

The surviving v0.1 candidate was:

> **Configuration-Derivative Identity Property** — a material artifact does not leave the Configuration category solely because it is normalized, compiled, tabulated, cached, persisted, rebuilt, or backend-specialized. Reclassification requires a semantic change such as independent physical history/state, new authority, or a formulation-relative closure role.

The critical v0.2 question is:

> Does this property decide any meaningful scenario that cannot already be decided by established material/state distinctions plus existing ThermoCore ownership, RQ-ISO-001, RQ-EFM-001, and implementation/conformance rules?

If the answer is no, the candidate shall be reclassified as an engineering/conformance property rather than promoted to Research Gap Analysis.

---

## 4. Null Hypothesis

The focused null hypothesis is:

```text
Material artifact
    |
    +-- semantic content is derived from declared Configuration
    |   and has no independent physical history/authority
    |       -> remains Configuration-like for Framework classification
    |
    +-- carries independent evolving physical memory
    |       -> State classification question
    |          -> RQ-ISO-001 for authority/non-promotion
    |
    +-- required evolving coordinate for selected thermodynamic closure/update
    |       -> RQ-EFM-001
    |
    +-- differs only by cache/layout/device/compiled form
            -> implementation/performance concern
```

The null does not claim that reconstructibility alone is a universal ontological definition of Configuration.

Instead, reconstructibility is tested as one **diagnostic discriminator** among source identity, authority, physical memory, and closure role.

---

## 5. Decision Fields

Each scenario is evaluated using the same fields.

### 5.1 Authoritative Source

What information is authoritative for the claimed material semantics?

### 5.2 Reconstructibility

Can the artifact be recreated from declared Configuration/formulation inputs without knowledge of evolving physical history?

### 5.3 Independent Material Authority

Can the artifact redefine material meaning independently of the declared Material Definition / configuration source?

### 5.4 Evolving Physical Memory

Does the artifact contain information whose current value depends on prior simulated physical history and affects future physical response?

### 5.5 Closure / Update Sufficiency

Is an evolving quantity in the artifact required to make the selected thermodynamic closure or next-state update unique?

### 5.6 Existing Route

Which existing ThermoCore rule governs the scenario?

### 5.7 Independent RQ-RMA Rule Required

Does any additional material-abstraction criterion remain necessary after existing routes are applied?

---

## 6. Matched-Scenario Summary

| Scenario | Reconstructible without physical history | Independent material authority | Evolving physical memory | Closure-critical evolving coordinate | Existing route | Independent RQ-RMA rule required |
|---|---|---|---|---|---|---|
| RMA-S0 Pure normalization / unit conversion | Yes | No | No | No | Configuration semantics | No |
| RMA-S1 Derived constants / LUT | Yes | No | No | No | Configuration + implementation | No |
| RMA-S2 CPU vs GPU/SIMD packed form | Yes | No | No | No | Implementation/performance | No |
| RMA-S3 Runtime invalidation/rebuild after Configuration change | Yes, from new Configuration | No | No | No | Configuration lifecycle / conformance | No |
| RMA-S4 Current-state-dependent property, no memory | Yes from current State + Configuration | No | No | No additional coordinate | Derived evaluation | No |
| RMA-S5 Hysteretic/history-dependent response | No, not from Configuration alone | No second material authority required | Yes | Formulation-dependent | RQ-ISO; RQ-EFM if closure-critical | No |
| RMA-S6 Composition/reaction/microstructure evolution | No if evolving coordinate matters | No second authority required | Yes | Formulation-dependent | RQ-ISO / RQ-EFM | No |
| RMA-S7 Hidden formulation-specific closure coordinate | No under alleged Configuration-only model | Potentially misleading packaging | Yes or otherwise closure-required | Yes | RQ-EFM-001 | No |
| RMA-S8 Pure numerical cache/workspace | Recomputable from current authoritative inputs | No | No physical memory | No | Implementation/performance | No |

Matched scenarios evaluated: **9**  
Scenarios requiring a new independent RQ-RMA architecture predicate: **0**

---

## 7. Scenario Analyses

### 7.1 RMA-S0 — Pure Normalization / Unit Conversion

#### Setup

A reusable Material Definition contains author-facing values. A compiler normalizes units, orders values, or converts them into the numerical units expected by the selected formulation.

Example abstractly:

```text
Material Definition
    rho = author-facing density
    cp  = author-facing heat capacity
        |
        v
normalization
        |
        v
computation-ready rho, cp
```

#### Findings

- authoritative material meaning remains in the declared Material Definition and applicable transformation contract;
- transformed values are deterministically reconstructible;
- no physical history is required;
- no evolving physical coordinate is introduced;
- the transformed artifact does not acquire independent material authority merely because it is the object directly consumed by Thermodynamic Computation.

#### Existing Route

Existing Configuration semantics are sufficient.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

The candidate property is useful as a conformance reminder but does not add a new architecture category.

---

### 7.2 RMA-S1 — Precomputed Derived Constants / LUT

#### Setup

Material Definition plus formulation assumptions are used to precompute:

- transition thresholds;
- interpolation coefficients;
- tabulated `Cp(T)` values;
- tabulated equation-of-state values;
- other deterministic lookup structures.

#### Findings

A LUT may be:

- built before simulation;
- loaded at runtime;
- persisted to disk;
- held in memory for the full run;
- rebuilt when its authoritative inputs change.

None of these lifecycle facts alone establish Runtime State.

The important distinction is whether the table merely encodes a declared material/model relationship or stores independent evolving physical memory.

The direct antecedent evidence from v0.1 already shows that tabular backends can be generated and cached while current thermodynamic state remains a separate object/concept.

#### Existing Route

Configuration semantics plus implementation/performance concerns are sufficient.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

The statement `persistent LUT != physical Runtime State` is useful, but is anteceded by established state/model/cache distinctions.

---

### 7.3 RMA-S2 — CPU Object versus GPU/SIMD Packed Representation

#### Setup

The same declared material semantics are represented as:

```text
CPU object
SIMD arrays
GPU buffer
packed structure
generated evaluator
```

#### Findings

Changes in:

- memory address;
- device;
- data alignment;
- Array-of-Structures versus Structure-of-Arrays;
- packed versus object representation;
- generated code versus interpreted/object dispatch;

do not by themselves create new physical meaning.

If two backend forms are semantically equivalent encodings of the same declared material data for the same formulation, backend choice does not create a second material authority or new state category.

v0.1 already found direct antecedents for compiled/JIT/backend-specific material behavior while parameters and internal state remain separate concepts.

#### Existing Route

Implementation/performance concern.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

---

### 7.4 RMA-S3 — Runtime Invalidation and Rebuild after Explicit Configuration Change

#### Setup

A user or application changes an allowed material definition value during application operation. The old computation-ready artifact is invalidated and a new one is built.

#### Stress Condition

The rebuild occurs **during runtime**, which could tempt an implementation to classify the artifact as Runtime State solely because it changes while the program is running.

#### Findings

Runtime timing is not enough to establish physical state semantics.

The new artifact is determined by:

```text
new authoritative Configuration
+ declared transformation/formulation
```

rather than by simulated physical history.

The semantic event is a Configuration change followed by re-derivation, not State Evolution.

An implementation may need lifecycle/version/invalidation mechanisms, but the mechanism itself does not change information category.

#### Existing Route

Configuration lifecycle plus conformance/implementation concerns.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

This scenario confirms that `runtime mutation != Runtime State`.

---

### 7.5 RMA-S4 — Current-State-Dependent Property Evaluation without Material Memory

#### Setup

A property is evaluated from current authoritative thermodynamic state and immutable material parameters:

```text
Cp = f(T, Material Definition)
```

No extra persistent history variable exists.

#### Stress Condition

The output changes whenever the current state changes. This could tempt an implementation to store the output and call it material state.

#### Findings

The changing property can remain a derived value if it is fully determined from current authoritative inputs.

Its numerical cache may persist, but cached persistence does not give it independent physical authority.

The key fact is not whether the value varies, but whether an additional evolving coordinate must be retained to reproduce future behavior.

#### Existing Route

Existing Runtime State / Configuration / derived-information semantics are sufficient.

If an extension is involved, RQ-ISO prevents unnecessary promotion merely because the property participates in computation.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

---

### 7.6 RMA-S5 — Hysteretic / History-Dependent Material Response

#### Setup

Two material locations have:

- identical current temperature;
- identical reusable Material Definition;
- different prior histories;
- different future response.

A persistent internal variable `z` distinguishes the histories.

```text
same Configuration
same current T
z_A != z_B
    -> different future response
```

#### Findings

`z` cannot be reconstructed from Material Definition alone.

Its differing value records physical history.

Packing `z` into:

- a material object;
- a material buffer;
- a lookup structure;
- a compiled-material record;

does not make it Configuration.

The semantic issue is already a State question.

Whether `z` is extension-local state or must participate in authoritative thermodynamic state depends on the selected formulation and is governed by RQ-ISO and RQ-EFM respectively.

#### Existing Route

- RQ-ISO-001: state authority / non-promotion;
- RQ-EFM-001: closure/state-space sufficiency if `z` is required by the selected thermodynamic formulation.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

This is the strongest test of the candidate property and it is already classified by existing state/formulation boundaries.

---

### 7.7 RMA-S6 — Composition / Reaction / Microstructure Evolution

#### Setup

A quantity such as:

- reaction progress;
- composition;
- damage;
- porosity;
- crystallization state;
- microstructure descriptor;

evolves during simulation and changes future constitutive or thermodynamic response.

#### Findings

Mechanism name does not determine classification.

The decisive questions are:

1. Does the quantity carry independent evolving physical information?
2. Is it required to close the selected thermodynamic formulation?
3. Which responsibility owns and evolves it?

If the quantity evolves with physical history, putting it back into `MaterialDefinition` would mix reusable Configuration with runtime physical state.

If it remains extension-local and the selected thermodynamic formulation remains sufficient, RQ-ISO governs non-promotion.

If thermodynamic closure/update is incomplete without it, RQ-EFM requires formulation/Core reconsideration.

#### Existing Route

RQ-ISO-001 and RQ-EFM-001.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

---

### 7.8 RMA-S7 — Formulation-Specific Closure Coordinate Hidden in a Material Structure

#### Setup

An evolving variable is placed inside an object named or used as a `MaterialRuntimeData` structure. The implementation labels the whole structure as material Configuration.

However, holding declared Configuration and ordinary Thermodynamic State fixed while changing the hidden coordinate changes the correct thermodynamic closure or next state.

#### Findings

Packaging cannot override semantic role.

If the hidden coordinate is required for unique thermodynamic interpretation/evolution, the selected state space is incomplete under the alleged Configuration-only classification.

This is precisely RQ-EFM closure/update insufficiency, not a distinct runtime-material-abstraction problem.

If the coordinate remains extension-local while the selected thermodynamic formulation is still complete through honest exchange, RQ-ISO governs its authority.

#### Existing Route

RQ-EFM-001 first; RQ-ISO-001 after ordinary-extension admissibility where applicable.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

---

### 7.9 RMA-S8 — Pure Numerical Cache / Solver Workspace

#### Setup

The implementation stores data such as:

- interpolation index;
- branch selector;
- polynomial coefficient cache;
- temporary factorization;
- precomputed search interval;
- backend-specific execution metadata.

The values may persist across calls for performance.

#### Findings

Persistence duration alone is not enough to create physical-state semantics.

If the workspace can be discarded and recomputed from current authoritative inputs without changing the represented physical/material meaning, it carries no independent physical authority.

The distinction between Configuration derivative and numerical workspace may matter for implementation documentation, but both remain outside authoritative thermodynamic state unless additional physical semantics are introduced.

#### Existing Route

Implementation/performance.

#### RQ-RMA Necessity

**No independent RQ-RMA rule required.**

---

## 8. Reconstructibility Stress Test

The most promising narrow discriminator from the RQ definition was reconstructibility.

### 8.1 What Reconstructibility Successfully Detects

Reconstructibility helps distinguish:

```text
artifact determined by declared Configuration
        versus
artifact containing independent physical history
```

It correctly flags RMA-S5, S6, and S7 as requiring State/formulation analysis rather than simple Configuration treatment.

### 8.2 Why Reconstructibility Is Not Sufficient as a Research Criterion

Reconstructibility alone does not decide all semantic categories.

Examples:

- a numerical workspace may be reconstructible but is not necessarily Material Definition Configuration;
- a derived property may be reconstructible from current State + Configuration rather than Configuration alone;
- an artifact could be deterministically generated from two conflicting material authorities, so authority still matters;
- an evolving coordinate may be theoretically reconstructible from full simulation history but still semantically be state.

Therefore reconstructibility is best treated as a **conformance diagnostic**, not an independent architecture contribution.

### 8.3 Result

**Reconstructibility provides useful decision support but no independent RQ-RMA category.**

---

## 9. Authority Stress Test

RQ-RMA also proposed that transformed Configuration must not become a second independent material authority.

### 9.1 Valid Derived Artifact

A derived artifact is semantically replaceable by another equivalent encoding generated from the same authoritative source and applicable formulation.

### 9.2 Invalid Dual-Authority Case

If both:

```text
Material Definition A
```

and

```text
Compiled Runtime Material Data B
```

can independently redefine material meaning without a declared precedence/transformation relation, the problem is an authority inconsistency.

### 9.3 Result

This is a useful Framework conformance concern, but it follows from existing unique ownership/authority semantics rather than establishing a new research category.

**Authority preservation survives as an engineering/conformance requirement, not an independent RQ-RMA contribution.**

---

## 10. Runtime / Persistence / Backend Stress Test

The following candidate triggers were directly tested:

- runtime creation;
- runtime rebuild;
- persistent cache;
- disk persistence;
- LUT/tabulation;
- GPU residency;
- SIMD packing;
- compiled/JIT implementation;
- formulation-specific derived constants.

None of these triggers produced semantic reclassification by themselves.

The result is:

```text
runtime
persistent
compiled
tabulated
cached
GPU-resident
formulation-specific

        !=

automatically Runtime State
```

Reclassification requires a semantic fact such as evolving physical memory, authority change, or thermodynamic closure role.

This result is strongly consistent with the direct antecedent evidence already recorded in v0.1.

---

## 11. Re-Evaluation of the Eight Candidate Dimensions

| Dimension | v0.2 finding | Independent research value |
|---|---|---|
| RMA-D1 Source identity / reconstructibility | Useful discriminator; anteceded by model/parameter/state separation | No independent category |
| RMA-D2 Semantic mutability | Physical-history evolution matters; runtime rebuild alone does not | Routes to State or Configuration lifecycle |
| RMA-D3 Authority | Useful conformance rule against dual material truth | Engineering/conformance |
| RMA-D4 Thermodynamic closure role | Decisive, but already RQ-EFM-001 | Absorbed by RQ-EFM |
| RMA-D5 Lifecycle / invalidation | Implementation/conformance concern | No independent research category |
| RMA-D6 Backend specialization | Semantically neutral absent other change | Implementation/performance |
| RMA-D7 Formulation specificity | Compatible with Configuration identity; closure change routes to RQ-EFM | No independent category |
| RMA-D8 Ownership / write responsibility | Existing ownership semantics + RQ-ISO sufficient | Absorbed by existing Framework/RQ-ISO |

Candidate dimensions evaluated: **8**  
Dimensions producing an independent RQ-RMA architecture category: **0**

---

## 12. Falsification Results

### F-RMA-1 — Direct Semantic Antecedent

**Triggered substantially.**

v0.1 established strong direct antecedents for the principal distinctions among material/model data, state, internal history variables, compiled behavior, and tabular/backend forms.

### F-RMA-2 — Trivial Information-Classification Consequence

**Triggered.**

All matched scenarios are explained by the combination:

```text
derived material/configuration semantics
+ physical-history state distinction
+ authority preservation
+ RQ-ISO
+ RQ-EFM
+ implementation/performance separation
```

No additional research predicate survives.

### F-RMA-3 — RQ-ISO Absorption

**Triggered for state-authority cases.**

RMA-S5 and S6 route to RQ-ISO when the issue is extension-local state versus mandatory Core State.

### F-RMA-4 — RQ-EFM Absorption

**Triggered for closure cases.**

RMA-S5/S6 when closure-critical and RMA-S7 directly route to RQ-EFM.

### F-RMA-5 — Implementation-Only Result

**Triggered for backend/cache/layout cases.**

RMA-S1, S2, S3, and S8 contain implementation concerns but no new Framework-semantic research category.

### F-RMA-6 — Existing Framework Semantics Already Sufficient

**Triggered for all nine matched scenarios.**

The remaining useful RMA wording functions as a conformance-oriented synthesis rather than a distinct research gap.

---

## 13. Configuration-Derivative Identity Property — Final v0.2 Assessment

The candidate property remains useful in the following bounded engineering form:

> A computation-ready material artifact does not acquire Runtime State, Material Representation, or independent material-authority semantics solely because it is normalized, compiled, tabulated, cached, persisted, rebuilt, formulation-specialized, or backend-specific. Its classification follows the physical and architectural meaning of the information it carries. Independent evolving physical memory shall be treated as state under the applicable ownership rules; closure-critical evolving information shall be routed through formulation-relative admissibility; pure layout/cache/device transformations remain implementation concerns.

This property is useful because it prevents common implementation confusions such as:

```text
"stored at runtime" -> State
"GPU buffer" -> State
"LUT" -> State
"compiled data" -> new material authority
"state-dependent property" -> persistent state
```

However, the property is not supported as a new research contribution on the current evidence baseline.

It is a compact engineering/conformance synthesis of established distinctions plus already-supported ThermoCore research boundaries.

---

## 14. Research Disposition

### 14.1 Independent Research Gap

**NOT SUPPORTED WITHIN THE BOUNDED DIRECT-ANTECEDENT REVIEW AND MATCHED-SCENARIO STRESS TEST**

### 14.2 Research Gap Analysis Readiness

**NO-GO**

A separate RQ-RMA Research Gap Analysis shall not be opened on the current evidence baseline.

### 14.3 Surviving Value

**Configuration-Derivative Identity Property**

### 14.4 Recommended Classification

**ENGINEERING / CONFORMANCE PROPERTY — NOT AN INDEPENDENT RESEARCH CONTRIBUTION**

### 14.5 Novelty / Priority

**NOT ESTABLISHED**

### 14.6 Framework Specification Impact

**NONE at this research stage**

The stress test does not authorize a normative Framework Specification change.

---

## 15. Relationship to Existing Research Boundaries

The final routing is:

```text
Material Definition / transformed material artifact
        |
        +-- no independent physical memory or authority change
        |       -> Configuration / derived configuration / implementation concern
        |
        +-- independent evolving physical memory
        |       -> State classification
        |       -> RQ-ISO-001 for authority / non-promotion
        |
        +-- evolving quantity required for thermodynamic closure/update
        |       -> RQ-EFM-001
        |
        +-- only layout/cache/device/compiler difference
                -> implementation / performance
```

RQ-RMA-001 therefore does not establish a fourth independent research boundary beside the already-supported RQ-ISO and RQ-EFM lines.

---

## 16. Recommended Next Step

After review and merge of this artifact:

1. close the v0.2 tracking task as completed;
2. create a non-normative `RQ_RMA_001_Closure_and_Reclassification_v0.1.md`;
3. preserve the negative result transparently;
4. reclassify **Configuration-Derivative Identity Property** as an engineering/conformance property;
5. do not open Research Gap Analysis;
6. do not modify Framework Specification as part of closure;
7. permit future conformance/verification scenarios if useful without reopening novelty claims.

---

## 17. Guardrails

The following claims are not supported by this artifact:

- that ThermoCore invented material compilation;
- that LUTs or GPU buffers are always Configuration;
- that reconstructibility alone universally defines Configuration;
- that all history variables are Thermodynamic State;
- that all history variables may remain extension-local state;
- that backend specialization is physically irrelevant in every implementation;
- that runtime rebuild can never require synchronization or numerical safeguards;
- that numerical equivalence of two backends is guaranteed;
- that current v1.0.0 implements every scenario evaluated here;
- that RQ-RMA-001 establishes novelty or priority.

The bounded result is only that the evaluated semantic distinctions are completely routed by established prior art plus existing ThermoCore categories and research boundaries without requiring an independent RQ-RMA architecture predicate.

---

## 18. Review Checklist

- [x] All nine matched scenarios evaluated.
- [x] All eight candidate dimensions re-evaluated.
- [x] Reconstructibility tested without treating it as sufficient by itself.
- [x] Runtime lifecycle separated from physical-state evolution.
- [x] Backend specialization separated from semantic authority.
- [x] History-bearing internal variables treated as State evidence.
- [x] RQ-ISO / RQ-EFM routing explicit.
- [x] Numerical workspace kept separate.
- [x] Research Gap Analysis receives explicit NO-GO.
- [x] Negative/reclassification outcome preserved.
- [x] No Framework Specification change introduced.
- [x] No production implementation change introduced.
- [x] No Verification / Validation / Performance change introduced.
