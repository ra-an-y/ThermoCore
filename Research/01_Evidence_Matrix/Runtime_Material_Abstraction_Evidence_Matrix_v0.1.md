# Runtime Material Abstraction Evidence Matrix

Version: 0.1  
Status: UNDER SURVEY — Bounded Direct-Antecedent Evidence Pass  
Research Question: RQ-RMA-001 — Runtime Material Abstraction Boundary  
Tracking Issue: #137  
Date: 2026-08-23

---

## 1. Objective

This matrix evaluates direct prior art relevant to the semantic boundary between reusable material/model definition, computation-ready transformed material data, runtime thermodynamic state, constitutive/internal state, numerical caches, and backend-specific material encodings.

The survey tests whether RQ-RMA-001 requires an independent ThermoCore research criterion or whether the meaningful cases are already explained by established material-model/state separation plus existing ThermoCore boundaries.

The principal null explanation is:

```text
reusable material/model definition
        |
        v
normalization / compilation / tabulation / caching / backend specialization
        |
        v
computation-ready encoding
```

may remain configuration-like when it only represents or accelerates already-declared material semantics, while:

```text
independent physical memory / history
        -> state

closure-critical evolving coordinate
        -> RQ-EFM-001

state authority / promotion question
        -> RQ-ISO-001

layout / device / cache / code-generation only
        -> implementation / performance
```

This document is non-normative. It does not establish novelty, priority, superiority, Framework Specification requirements, implementation requirements, Verification results, Validation results, or Performance conclusions.

---

## 2. ThermoCore Baseline Under Test

RQ-RMA-001 begins from existing Framework distinctions rather than inventing a new information class.

The current normative baseline already provides:

- Material Definition is Configuration;
- Runtime State is distinct from Configuration;
- Material Representation is distinct from both Runtime State and Configuration;
- Thermodynamic Computation may read applicable material information;
- configuration supply does not transfer ownership;
- `Data_Flow.md` intentionally does not define how Material Definition is authored, stored, transformed, transported, or made available.

The bounded reference implementation already contains one non-normative realization:

```text
ReferenceMaterialDefinition
        |
        v
ReferenceMaterialCompiler
        |
        v
CompiledThermodynamicParameters
```

The implementation describes the result as computation-ready Configuration, but this project-local design choice is not evidence of novelty.

---

## 3. Candidate Semantic Dimensions

The eight RQ-RMA candidate dimensions are evaluated below.

| ID | Candidate dimension | Question |
|---|---|---|
| RMA-D1 | Source identity / reconstructibility | Is transformed data wholly derivable from declared material/configuration semantics? |
| RMA-D2 | Semantic mutability | Does change reflect configuration rebuild or independent physical history? |
| RMA-D3 | Authority | Does the transformed artifact merely encode material meaning, or become a second source of truth? |
| RMA-D4 | Thermodynamic closure role | Is the quantity a parameter/evaluator, or an evolving coordinate needed for closure/update uniqueness? |
| RMA-D5 | Lifecycle / invalidation | Can build/cache/rebuild occur without creating physical state? |
| RMA-D6 | Backend specialization | Do CPU/GPU/table/JIT/compiled forms preserve semantic identity? |
| RMA-D7 | Formulation specificity | Can one formulation-specific encoding remain configuration-like without becoming Framework-level material truth? |
| RMA-D8 | Ownership / write responsibility | Who creates or updates transformed material data, and what does that update mean? |

Evidence labels:

- **DIRECT ANTECEDENT** — the reviewed source explicitly distinguishes the relevant semantic roles.
- **STRONG PARTIAL ANTECEDENT** — the phenomenon is explicit, but not with ThermoCore's exact ownership vocabulary.
- **ALREADY ROUTED BY THERMOCORE** — the issue is real but already belongs to RQ-ISO-001, RQ-EFM-001, or existing Conformance.
- **IMPLEMENTATION / PERFORMANCE** — the distinction concerns storage, layout, execution, or acceleration rather than semantic authority.
- **UNRESOLVED IN THIS PASS** — no sufficient direct evidence found; not a claim of absence.

---

## 4. Cross-Source Summary

| Source family | Material/model definition | Runtime thermodynamic state | History/internal state | Compiled/tabulated/cache form | Backend specialization | RQ-RMA pressure |
|---|---|---|---|---|---|---|
| Modelica.Media | medium constants, package, functions | explicit `ThermodynamicState` record | medium-dependent; not the main focus | translated/model-specific functions and records | replaceable medium implementation | Strong model-definition vs state antecedent |
| MOOSE Materials | input/material property definitions | coupled field variables plus material evaluation | explicit stateful material properties with old/older values | most material properties computed on demand | Kokkos Materials preserve stateful-property semantics | Very strong history-state discriminator |
| OpenFOAM thermophysical models | `thermophysicalProperties` + `thermoType` model selection | solver fields / thermodynamic variables | mechanism-dependent | constant, polynomial, tabulated, runtime-selected models | implementation package choice | Strong configuration/model-selection antecedent |
| Cantera | species/phase model data and coefficients | explicit phase thermodynamic state | model-dependent | parameterized polynomial model data | interchangeable thermo model classes | Strong model-data vs state separation |
| CoolProp | fluid/EOS backend model | `AbstractState` update with state-variable pairs | not general constitutive history | TTSE/BICUBIC tables generated, stored, loaded, rebuilt | multiple backends behind common state interface | Very strong cache/LUT/backend antecedent |
| dolfinx_materials / MFront / JAX | material properties and parameters | external fields / gradients | explicit internal state variables | compiled MFront behavior; JAX JIT behavior | Python, compiled library, JAX/GPU paths | Very strong compiled-behavior vs internal-state antecedent |
| NEML | constitutive model definitions/parameters | strain, stress, temperature, time inputs | explicit `History` internal variables | history may use flat arrays or wrapped objects | memory representation may vary | Very strong physical-history state antecedent |

---

## 5. Evidence Record RMA-01 — Modelica.Media

### 5.1 Sources

- Modelica Standard Library, `Modelica.Media.UsersGuide.MediumDefinition.BasicStructure`:  
  https://doc.modelica.org/om/Modelica.Media.UsersGuide.MediumDefinition.BasicStructure.html
- Modelica Standard Library, `Modelica.Media.UsersGuide.MediumUsage.BasicUsage`:  
  https://doc.modelica.org/om/Modelica.Media.UsersGuide.MediumUsage.BasicUsage.html
- `Modelica.Media.Interfaces.PartialMedium.ThermodynamicState`:  
  https://doc.modelica.org/Modelica%204.0.0/Resources/helpWSM/Modelica/Modelica.Media.Interfaces.PartialMedium.ThermodynamicState.html

### 5.2 Findings

Modelica.Media explicitly separates reusable medium-model information from the thermodynamic state used by property functions.

A medium package defines items such as:

- medium constants;
- a `BaseProperties` model;
- `setState_XXX` functions;
- additional property functions such as viscosity or conductivity.

`ThermodynamicState` is separately defined as the minimal variable set available as input to medium functions.

The `BasicStructure` documentation shows optional material/medium property functions receiving a `ThermodynamicState` record, while the medium package supplies equations and material-specific definitions.

This is a direct antecedent to the broad separation:

```text
material / medium model semantics
        !=
current thermodynamic state
```

and to the idea that current-state-dependent material properties can be **computed from state** without becoming additional independent state merely because they are used at runtime.

### 5.3 RQ-RMA significance

Modelica.Media strongly pressures any claim that ThermoCore is novel merely for separating:

- reusable material definition;
- state variables;
- functions/evaluators used to obtain state-dependent properties.

It also shows that formulation-specific material functions and state records can coexist without requiring every derived property to be persisted.

### 5.4 Limitation

Modelica.Media does not directly formalize ThermoCore's specific question of whether a serialized LUT, GPU buffer, or compiled parameter block remains Configuration after transformation.

**Classification:** `DIRECT MODEL-DEFINITION / THERMODYNAMIC-STATE ANTECEDENT — VERY STRONG`.

---

## 6. Evidence Record RMA-02 — MOOSE Materials

### 6.1 Sources

- MOOSE Materials System:  
  https://mooseframework.inl.gov/releases/moose/2024-03-08/syntax/Materials/
- Kokkos Materials System:  
  https://mooseframework.inl.gov/docs/PRs/32820/site/syntax/KokkosMaterials/

### 6.2 Computed-on-demand versus stateful properties

MOOSE provides one of the strongest direct antecedents for RQ-RMA-D2.

The Materials documentation states that material properties are generally computed on demand and not stored. When a value from a previous timestep is required, the framework exposes old/older accessors and treats these as **stateful material properties**.

Therefore MOOSE explicitly distinguishes:

```text
current property computed from current inputs
        !=
property/history value intentionally retained across timesteps
```

The latter has a memory cost because stateful values are stored at quadrature points.

### 6.3 Backend significance

The Kokkos Materials system retains explicit current/old/older state semantics under a different accelerated implementation path.

This is important for RMA-D6: moving material evaluation to an accelerator-oriented backend does not erase the conceptual distinction between ordinary current properties and stateful historical properties.

### 6.4 RQ-RMA significance

MOOSE directly pressures the candidate rule:

> a material-related value is not state merely because it is evaluated at runtime; it becomes stateful when previous physical values must be retained as part of the model semantics.

This does not exactly equal ThermoCore's Configuration vocabulary, but it strongly antecedents the **physical-history discriminator**.

**Classification:** `DIRECT CURRENT-PROPERTY / HISTORY-STATE ANTECEDENT — VERY STRONG`.

---

## 7. Evidence Record RMA-03 — OpenFOAM Thermophysical Models

### 7.1 Sources

- OpenFOAM Thermophysical documentation:  
  https://doc.openfoam.com/2212/tools/processing/models/thermophysical/
- OpenFOAM Transport models:  
  https://doc.openfoam.com/2312/tools/processing/models/thermophysical/transport/
- OpenFOAM Equation of State models:  
  https://doc.openfoam.com/2212/tools/processing/models/thermophysical/equation-of-state/

### 7.2 Findings

OpenFOAM thermophysical behavior is assembled from user-selectable packages declared in `constant/thermophysicalProperties`.

The `thermoType` selection specifies model components such as thermodynamics, transport, and equation of state. Transport and equation-of-state documentation explicitly include constant, polynomial, and tabulated forms among available model choices.

Thus the runtime solver can consume thermophysical behavior from different parameterizations and representation forms without treating the representation format itself as the governing field state.

### 7.3 RQ-RMA significance

OpenFOAM directly establishes that:

- material/thermophysical semantics may be configured through reusable model definitions;
- multiple model encodings, including tabulated forms, can supply computation;
- solver requirements determine which model package is compatible.

This is a strong antecedent for RMA-D7: a formulation/solver-specific thermophysical representation can remain a model/configuration choice rather than becoming a universal material truth.

### 7.4 Limitation

The documentation does not provide a ThermoCore-like semantic theorem that deterministic transformed data necessarily remains Configuration.

**Classification:** `DIRECT THERMOPHYSICAL-CONFIGURATION / MODEL-SELECTION ANTECEDENT — STRONG`.

---

## 8. Evidence Record RMA-04 — Cantera

### 8.1 Sources

- Cantera Thermodynamic Properties:  
  https://www.cantera.org/3.1/reference/thermo/index.html
- Cantera Species Thermodynamic Models:  
  https://www.cantera.org/3.1/reference/thermo/species-thermo.html
- Cantera Phase Definitions:  
  https://cantera.org/3.1/yaml/phases.html

### 8.2 Findings

Cantera separates species/phase thermodynamic model information from the current thermodynamic state of a phase.

Species thermodynamic models provide parameterizations such as heat-capacity/enthalpy/entropy polynomial data. Phase models then use those definitions to calculate properties at a selected state.

Cantera separately documents phase-state setting through thermodynamic variable pairs such as `T,P`, `H,P`, `U,V`, and composition.

Thus:

```text
thermodynamic model + coefficient data
        !=
current thermodynamic state
```

although both may be held by one software object.

### 8.3 RQ-RMA significance

This is a strong direct antecedent to the proposition that **semantic classification does not follow object co-location**. A software object may contain both model definitions and current state, but their physical roles remain distinguishable.

It also pressures any RQ-RMA claim that parameterized material functions are a new class merely because they are computation-ready.

**Classification:** `DIRECT MODEL-DATA / CURRENT-STATE ANTECEDENT — VERY STRONG`.

---

## 9. Evidence Record RMA-05 — CoolProp Tabular Backends

### 9.1 Sources

- CoolProp Tabular Interpolation:  
  https://coolprop.org/coolprop/Tabular.html
- CoolProp Backends:  
  https://coolprop.org/develop/backends.html
- CoolProp `AbstractState`:  
  https://coolprop.org/_static/doxygen/html/class_cool_prop_1_1_abstract_state.html

### 9.2 Tabulation and persistence

CoolProp provides a particularly direct antecedent for the LUT/cache question.

TTSE and BICUBIC backends precompute gridded thermodynamic data from a lower-level equation-of-state backend. The tables are written to disk, loaded into `AbstractState` instances, and reused for faster property evaluation.

The documentation also states that tables are cached per fluid and rebuilt when relevant table-resolution configuration changes.

Therefore the same physical fluid model may be accessed through:

```text
full EOS backend
        or
precomputed tabular backend
```

while state updates still use thermodynamic input pairs.

### 9.3 RQ-RMA significance

This is direct evidence that all of the following may occur without the table itself becoming independent physical history:

- precomputation;
- persistent on-disk storage;
- loading at runtime;
- cache reuse;
- rebuild after configuration change;
- alternative backend representation;
- interpolation coefficients and derivatives stored for acceleration.

This strongly pressures any claim that persistence, runtime rebuild, or LUT form is sufficient to classify an artifact as Runtime State.

It is also a direct antecedent to RMA-D5 and RMA-D6.

### 9.4 Important distinction

The tabular backend still has a current `AbstractState`; the cached table is not a substitute for the current thermodynamic state variables supplied to update the state object.

**Classification:** `DIRECT TABULATION / CACHE / BACKEND-SPECIALIZATION ANTECEDENT — VERY STRONG`.

---

## 10. Evidence Record RMA-06 — dolfinx_materials, MFront, and JAX

### 10.1 Sources

- Material interfaces:  
  https://bleyerj.github.io/dolfinx_materials/api/material.html
- QuadratureMap API:  
  https://bleyerj.github.io/dolfinx_materials/api/quadrature_map.html
- MFront behavior use:  
  https://bleyerj.github.io/dolfinx_materials/mfront.html
- JAX material implementation:  
  https://bleyerj.github.io/dolfinx_materials/jax.html

### 10.2 Compiled behavior versus material properties

`MFrontMaterial` loads a material behavior from a compiled shared library while accepting material properties and parameters separately.

Thus executable/compiled form and material parameter data are already explicitly separable in a production constitutive interface.

### 10.3 Internal state variables

`QuadratureMap` separately exposes dictionaries for:

- gradients;
- fluxes;
- internal state variables;
- external state variables.

Its time-advance operation copies current state to old state after convergence.

The JAX material documentation is even more direct: a constitutive update receives current strain, previous `state`, and time step, and returns stress plus `new_state`.

### 10.4 Backend significance

The same conceptual constitutive role may be supplied by:

- pure Python behavior;
- MFront compiled behavior;
- JAX/JIT material behavior;
- accelerated JAX execution.

This is a strong direct antecedent against the claim that compilation, JIT, or accelerated backend changes semantic state authority by itself.

### 10.5 RQ-RMA significance

This family directly antecedents the boundary:

```text
material parameters / compiled behavior
        !=
internal constitutive state
```

and shows that backend specialization can remain orthogonal to the distinction.

**Classification:** `DIRECT COMPILED-BEHAVIOR / PARAMETER / INTERNAL-STATE ANTECEDENT — VERY STRONG`.

---

## 11. Evidence Record RMA-07 — NEML Internal Variables / History

### 11.1 Sources

- NEML Interfaces:  
  https://neml.readthedocs.io/en/latest/interfaces.html
- NEML History Object System:  
  https://neml.readthedocs.io/en/latest/advanced/history.html
- NEML Integrating Models:  
  https://neml.readthedocs.io/en/stable/integration.html

### 11.2 Findings

NEML explicitly describes constitutive response as depending on strain, temperature, time, and a set of internal variables maintained by the model.

The calling code must preserve the time series of relevant previous quantities and history variables. The model returns updated history along with updated response.

NEML's `History` system can store the same internal variables in different memory organizations, including wrapping externally managed flat arrays without copying.

### 11.3 RQ-RMA significance

This provides direct evidence for two critical RQ-RMA distinctions.

First:

```text
history-dependent constitutive memory
        is not merely reusable material definition
```

Second:

```text
memory layout / wrapper / storage organization
        does not define the physical identity of the history variable
```

Therefore physical history and implementation storage form are already clearly separated in mature constitutive-model infrastructure.

**Classification:** `DIRECT PHYSICAL-HISTORY / STORAGE-FORM ANTECEDENT — VERY STRONG`.

---

## 12. Dimension-by-Dimension Evaluation

### 12.1 RMA-D1 — Source identity / reconstructibility

Direct evidence from CoolProp shows cached/tabulated data regenerated from lower-level EOS semantics and rebuild-triggering configuration. OpenFOAM and Cantera similarly distinguish model parameterization from current state.

However, the exact ThermoCore use of **reconstructibility from authoritative Material Definition** as an explicit conformance discriminator is not directly formalized in the reviewed sources.

**Disposition:** `STRONG PARTIAL ANTECEDENT; POSSIBLE PROJECT-SPECIFIC CONFORMANCE FORMULATION`.

### 12.2 RMA-D2 — Semantic mutability

MOOSE and NEML directly establish the difference between values computed from current inputs and values intentionally retained because physical history affects future response.

**Disposition:** `DIRECT ANTECEDENT ESTABLISHED`.

### 12.3 RMA-D3 — Authority

The reviewed frameworks strongly distinguish model parameters from state, but they do not generally use ThermoCore's explicit single-authority terminology for transformed material definitions.

Nevertheless, nothing in the reviewed evidence supports treating a cache, table, compiled library, or device buffer as a new independent material truth merely because it exists.

**Disposition:** `STRONG PARTIAL ANTECEDENT; EXACT THERMOCORE AUTHORITY LANGUAGE NOT DIRECTLY ANTECEDED`.

### 12.4 RMA-D4 — Thermodynamic closure role

Modelica.Media and Cantera make state-space requirements explicit through thermodynamic state variables. NEML and dolfinx_materials similarly make constitutive internal state explicit when response depends on history.

Within ThermoCore, if an evolving material quantity is required to make thermodynamic closure or the state update unique, this is already an RQ-EFM-001 question.

**Disposition:** `REAL PHENOMENON; THERMOCORE DECISION ALREADY ROUTED TO RQ-EFM-001`.

### 12.5 RMA-D5 — Lifecycle / invalidation

CoolProp directly establishes persistent table generation, on-disk storage, load, reuse, and automatic rebuild after table-resolution configuration changes.

This is especially strong evidence that runtime build/rebuild and persistence do not by themselves define physical state.

**Disposition:** `DIRECT ANTECEDENT ESTABLISHED`.

### 12.6 RMA-D6 — Backend specialization

CoolProp uses multiple backends behind a common state interface. dolfinx_materials supports Python, compiled MFront, JAX/JIT, and accelerated paths. MOOSE retains stateful-property semantics in Kokkos Materials.

**Disposition:** `DIRECT BACKEND-ORTHOGONALITY ANTECEDENT ESTABLISHED`.

### 12.7 RMA-D7 — Formulation specificity

Modelica medium packages, OpenFOAM `thermoType`, Cantera phase/species model selections, and CoolProp backends all show that material/property parameterizations may be tied to selected model families while remaining separate from current state.

The existence of a formulation-specific compiled representation is therefore not itself novel.

**Disposition:** `DIRECT / STRONG ANTECEDENT ESTABLISHED`.

### 12.8 RMA-D8 — Ownership / write responsibility

The reviewed systems distinguish who manages current/history state from who provides material models or properties, but exact Framework-level ownership semantics differ.

Within ThermoCore, state write authority is already governed by existing architecture and RQ-ISO-001.

**Disposition:** `STRONG PARTIAL ANTECEDENT; STATE-AUTHORITY CASES ALREADY ROUTED TO RQ-ISO-001`.

---

## 13. Direct Test of the Strong Null Explanation

### 13.1 Null procedure

For a material-related runtime artifact `X`:

1. Identify the authoritative material/model definition from which `X` is produced.
2. Determine whether `X` can be rebuilt from declared configuration/model semantics without independent physical history.
3. Determine whether changes to `X` reflect configuration change/rebuild or physical state evolution.
4. Determine whether `X` contains history/internal variables required by constitutive or thermodynamic evolution.
5. If an evolving quantity is required for thermodynamic closure or unique update, route to RQ-EFM-001.
6. If the question is whether local/transformed state becomes mandatory Core State or acquires state authority, route to RQ-ISO-001.
7. If only representation form changes — table/object/buffer/JIT/GPU/CPU/cache — treat that distinction as implementation/performance unless semantic authority changes.

### 13.2 First-pass result

All reviewed evidence supports the following broad classification pattern:

```text
A. reusable model data / coefficients / parameters
   -> configuration-like model definition

B. deterministic tabulation / interpolation coefficients / compiled behavior
   -> transformed computation representation; not state merely because runtime-used

C. current thermodynamic variables
   -> thermodynamic state

D. history/internal variables that affect future response
   -> evolving state

E. device/layout/JIT/cache form
   -> implementation detail unless it changes A-D semantics
```

The reviewed sources use different terminology, but the semantic distinction is mature.

### 13.3 Null disposition

```text
Strong RQ-RMA null explanation:
STRONGLY SUPPORTED BY FIRST DIRECT-ANTECEDENT PASS
```

No independent new information category has been identified.

---

## 14. Matched Scenario Routing — Preliminary v0.1

The RQ definition listed nine candidate scenarios. This pass performs a preliminary route only; a later v0.2 should make the matched comparison explicit.

| Scenario | First-pass route | New RQ-RMA predicate required? |
|---|---|---|
| RMA-S0 normalization/unit conversion | transformed Configuration | No |
| RMA-S1 precomputed constants / LUT coefficients | transformed Configuration if rebuildable and non-historical | No |
| RMA-S2 CPU/SIMD/GPU/table packed representation | backend/layout | No |
| RMA-S3 cache invalidation after allowed configuration change | configuration lifecycle | No |
| RMA-S4 state-dependent property from current authoritative state | derived property evaluation | No |
| RMA-S5 hysteretic/history-dependent response | state / RQ-ISO; possibly RQ-EFM depending closure role | No independent predicate yet |
| RMA-S6 composition/reaction/microstructure evolution | runtime state classification; RQ-ISO / RQ-EFM depending role | No independent predicate yet |
| RMA-S7 formulation-specific closure coordinate | RQ-EFM-001 | No |
| RMA-S8 pure numerical acceleration cache | implementation/performance | No |

This routing strongly suggests that the useful project-level result may be a conformance property rather than an independent research contribution.

---

## 15. Prior-Art Exclusions

On this evidence baseline, RQ-RMA-001 shall not claim novelty in:

- separating material/model definitions from current thermodynamic state;
- using parameterized material or medium functions;
- computing state-dependent material properties on demand;
- retaining old/older values for stateful material behavior;
- using internal/history variables for hysteretic or constitutive response;
- compiling material behavior into executable libraries;
- JIT-compiling constitutive/material functions;
- using CPU, GPU, Kokkos, JAX, or other backend-specific material execution;
- generating LUTs or interpolation tables;
- persisting tabular data on disk;
- loading cached property tables at runtime;
- rebuilding tables after configuration changes;
- selecting among different thermophysical model packages;
- using polynomial or tabulated thermophysical models;
- distinguishing material parameters from internal constitutive variables;
- storing the same state variables in different memory layouts.

These are established prior art or mature engineering practices.

---

## 16. Surviving Candidate

The first-pass evidence does not establish an independent RQ-RMA research gap.

The only project-specific proposition that remains potentially useful is:

> **Configuration-Derivative Identity Property** — a material artifact does not leave the Configuration category solely because it is normalized, compiled, tabulated, cached, persisted, rebuilt, or specialized for a backend. Reclassification requires a semantic change such as independent physical history/state, new authority, or a formulation-relative closure role that is not reducible to the authoritative Material Definition and declared transformation.

This proposition is currently better characterized as a candidate **ENGINEERING / CONFORMANCE PROPERTY** than as a new research contribution.

Its exact necessity should be stress-tested before closure because RMA-D1 and RMA-D3 retain project-specific wording around reconstructibility and authority preservation.

---

## 17. Research-Gap Readiness

```text
Material/model definition versus current thermodynamic state:
DIRECT PRIOR ART ESTABLISHED

Computed-on-demand property versus stored history/state:
DIRECT PRIOR ART ESTABLISHED

Internal/history variables for path-dependent response:
DIRECT PRIOR ART ESTABLISHED

Compiled material behavior versus internal state:
DIRECT PRIOR ART ESTABLISHED

LUT / tabular cache generation and runtime loading:
DIRECT PRIOR ART ESTABLISHED

Cache rebuild after configuration change:
DIRECT PRIOR ART ESTABLISHED

Backend specialization without erasing state distinction:
DIRECT / STRONG PRIOR ART ESTABLISHED

Exact ThermoCore reconstructibility + authority formulation:
NOT DIRECTLY ESTABLISHED AS A DISTINCT RESEARCH CONTRIBUTION

Independent RQ-RMA Research Gap:
NOT ESTABLISHED

Research Gap Analysis readiness:
NO-GO

Recommended next action:
FOCUSED v0.2 MATCHED-SCENARIO / DIRECT-ANTECEDENT STRESS TEST

Novelty / priority:
NOT ESTABLISHED

Framework Specification impact:
NONE
```

---

## 18. Recommended v0.2 Stress Test

Do not open a Research Gap Analysis yet.

The next step should test whether the proposed **Configuration-Derivative Identity Property** contributes any decision power beyond prior art and existing ThermoCore rules.

The matched cases should include:

1. same Material Definition -> normalized CPU record;
2. same definition -> LUT/table generated and persisted;
3. same definition -> GPU/device-packed representation;
4. same definition -> cache rebuilt after configuration change;
5. current-temperature-dependent property computed on demand;
6. hysteretic variable retained across timesteps;
7. constitutive internal state represented in several storage layouts;
8. compiled/JIT material behavior with separate parameters and state;
9. closure-critical evolving material quantity routed through RQ-EFM-001.

For each case ask:

- does reconstructibility from authoritative material/configuration semantics add a decision not already obvious from prior art?
- does authority-preservation add a decision not already governed by ThermoCore ownership/conformance?
- does any case survive after RQ-ISO and RQ-EFM routing?

If no, close/reclassify RQ-RMA-001 rather than narrowing it indefinitely.

---

## 19. Current Disposition

**RQ-RMA-001 remains open only for a focused v0.2 stress test.**

The first direct-antecedent pass finds strong prior art across thermodynamic libraries, multiphysics frameworks, constitutive interfaces, and tabular property backends for nearly every broad semantic distinction in the RQ.

The most direct pressure comes from three independent evidence families:

```text
MOOSE / NEML / dolfinx_materials
    -> history/internal variables are explicit evolving state

CoolProp
    -> persistent generated LUT/cache/backend data is an acceleration representation,
       not physical history merely because it persists or is loaded at runtime

Modelica.Media / Cantera / OpenFOAM
    -> reusable material/model definitions and current thermodynamic state are
       semantically distinguishable even when tightly integrated in software
```

Within ThermoCore, the remaining candidate appears to reduce to:

```text
transformed material encoding
    + existing Configuration semantics
    + RQ-ISO authority rules
    + RQ-EFM closure rules
```

rather than a new independently necessary architecture category.

The correct next stage is therefore falsification-oriented v0.2 stress testing, not Research Gap Analysis.

---

## 20. References

1. Modelica Standard Library — Medium Definition / Basic Structure.  
   https://doc.modelica.org/om/Modelica.Media.UsersGuide.MediumDefinition.BasicStructure.html
2. Modelica Standard Library — Medium Usage / Basic Usage.  
   https://doc.modelica.org/om/Modelica.Media.UsersGuide.MediumUsage.BasicUsage.html
3. MOOSE Materials System.  
   https://mooseframework.inl.gov/releases/moose/2024-03-08/syntax/Materials/
4. MOOSE Kokkos Materials System.  
   https://mooseframework.inl.gov/docs/PRs/32820/site/syntax/KokkosMaterials/
5. OpenFOAM Thermophysical Models.  
   https://doc.openfoam.com/2212/tools/processing/models/thermophysical/
6. OpenFOAM Transport Models.  
   https://doc.openfoam.com/2312/tools/processing/models/thermophysical/transport/
7. Cantera Thermodynamic Properties.  
   https://www.cantera.org/3.1/reference/thermo/index.html
8. Cantera Species Thermodynamic Models.  
   https://www.cantera.org/3.1/reference/thermo/species-thermo.html
9. CoolProp Tabular Interpolation.  
   https://coolprop.org/coolprop/Tabular.html
10. CoolProp Backends.  
    https://coolprop.org/develop/backends.html
11. dolfinx_materials Material Interfaces.  
    https://bleyerj.github.io/dolfinx_materials/api/material.html
12. dolfinx_materials QuadratureMap.  
    https://bleyerj.github.io/dolfinx_materials/api/quadrature_map.html
13. dolfinx_materials MFront Behaviors.  
    https://bleyerj.github.io/dolfinx_materials/mfront.html
14. dolfinx_materials JAX Material Behaviors.  
    https://bleyerj.github.io/dolfinx_materials/jax.html
15. NEML Interfaces.  
    https://neml.readthedocs.io/en/latest/interfaces.html
16. NEML History Object System.  
    https://neml.readthedocs.io/en/latest/advanced/history.html
17. NEML Integrating Models.  
    https://neml.readthedocs.io/en/stable/integration.html
