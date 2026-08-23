# Formulation Change Isolation Evidence Matrix

Version: 0.1  
Status: Bounded Prior-Art Evidence Survey — Non-Normative  
Research Question: RQ-FCI-001 — Thermodynamic Formulation Change Isolation

---

## 1. Objective

This matrix evaluates prior art relevant to **thermodynamic formulation change isolation**.

The research question is not whether software can expose multiple thermodynamic variables or property packages. Those capabilities are already expected to be established. The narrower question is whether a change between valid thermodynamic formulations for the **same declared physical scope** can be treated as a formulation-local change while higher-level architecture, ownership, information-category semantics, interface semantics, representation responsibility, extension governance, and conformance semantics remain stable.

The matrix therefore distinguishes three evidence levels:

1. **State/property abstraction prior art** — multiple state pairs, variables, or backends under a stable property API.
2. **Formulation/package substitution prior art** — a simulation architecture can select different thermodynamic/energy/property formulations without replacing the entire surrounding architecture.
3. **Explicit architectural change-containment prior art** — a framework formally states what may change locally, what must remain invariant, and where physical-scope expansion forces higher-level architectural revision.

Only the third level directly pressures the narrowest RQ-FCI-001 candidate.

This document does not establish novelty, priority, superiority, or Framework Specification requirements.

## 2. RQ-FCI-001 Evidence Dimensions

The following dimensions are evaluated independently. No composite score is used.

| ID | Dimension | Evidence question |
|---|---|---|
| FCI-E1 | Replaceable thermodynamic/property package | Can the thermodynamic/property implementation be replaced behind a stable abstraction? |
| FCI-E2 | Alternative independent variables / state input pairs | Can equivalent thermodynamic states be specified or reconstructed from different variable pairs? |
| FCI-E3 | Alternative energy coordinates / energy forms | Can the implementation select enthalpy, internal energy, sensible/absolute forms, or other energy coordinates? |
| FCI-E4 | Stored-versus-derived / state-schema flexibility | Does the architecture permit different quantities to be primary, stateful, derived, or computed on demand? |
| FCI-E5 | Closure / backend substitution | Can equations of state, property closures, interpolation tables, or backends change without replacing the full surrounding system? |
| FCI-E6 | Stable surrounding abstraction | Is there evidence that components or consumers continue to use a stable interface while the formulation/property package changes? |
| FCI-E7 | Explicit same-scope versus scope-expansion boundary | Does the source explicitly distinguish equivalent-scope formulation substitution from adding new governing physics/state? |
| FCI-E8 | Explicit architecture-wide change-containment / conformance rule | Does the source define what architectural responsibilities, ownership, interfaces, or conformance semantics must remain invariant under formulation change? |

Evidence labels:

- **Established** — explicitly supported by reviewed source material.
- **Partial** — related capability is explicit, but the full RQ-FCI meaning is not established.
- **Not established in reviewed public evidence** — the reviewed material does not provide enough evidence; this is not a claim of absence.
- **Not applicable to source role** — the source is not intended to address the dimension.

## 3. Evidence Summary Matrix

| Source family | E1 package replaceability | E2 alternative variables/state pairs | E3 alternative energy forms | E4 state-schema flexibility | E5 closure/backend substitution | E6 stable surrounding abstraction | E7 scope-expansion boundary | E8 explicit change-containment rule | RQ-FCI relevance |
|---|---|---|---|---|---|---|---|---|---|
| Modelica.Media | **Established** | **Established** | **Partial / Established at medium-variable level** | **Established / Partial at simulation-storage level** | **Established** | **Established** | **Not established in reviewed public evidence** | **Not established in reviewed public evidence** | Strongest general architectural falsification pressure |
| OpenFOAM thermophysical models | **Established** | **Partial** | **Established** | **Partial** | **Established** | **Established / Partial by application family** | **Not established in reviewed public evidence** | **Not established in reviewed public evidence** | Strong formulation-package and energy-form prior art |
| MOOSE Fluid Properties / Materials | **Established** | **Established** | **Partial** | **Established** | **Established** | **Established** | **Not established in reviewed public evidence** | **Not established in reviewed public evidence** | Strong PT/VE and stateful/on-demand evidence |
| Cantera ThermoPhase | **Partial / Established at thermo-model interface level** | **Established** | **Established** | **Partial** | **Established at thermo-model implementation level** | **Established** | **Not established in reviewed public evidence** | **Not established in reviewed public evidence** | Strong state-coordinate/API prior art; not a full framework comparator |
| CoolProp AbstractState | **Established at backend level** | **Established** | **Established** | **Partial** | **Established** | **Established** | **Not established in reviewed public evidence** | **Not established in reviewed public evidence** | Strong property-backend/state-pair prior art; not a full framework comparator |

## 4. Modelica.Media

### 4.1 Evidence reviewed

Primary documentation:

- Modelica Standard Library, `Modelica.Media.Interfaces.PartialMedium`
- Modelica Standard Library, `PartialMedium.BaseProperties`
- Modelica.Media User's Guide, Basic Medium Usage
- `setState_pTX`, `setState_phX`, `setState_psX`, `setState_dTX`

Representative sources:

- https://doc.modelica.org/Modelica%204.0.0/Resources/helpWSM/Modelica/Modelica.Media.Interfaces.PartialMedium.html
- https://doc.modelica.org/Modelica%204.0.0/Resources/helpWSM/Modelica/Modelica.Media.Interfaces.PartialMedium.BaseProperties.html
- https://doc.modelica.org/Modelica%203.2.3/Resources/helpWSM/Modelica/Modelica.Media.UsersGuide.MediumUsage.BasicUsage.html

### 4.2 Findings

`PartialMedium` defines a common medium-package contract and lists `ThermodynamicState` as a minimal variable set available to medium functions. Specific media inherit from the partial package and provide their own equations.

`BaseProperties` exposes common quantities including pressure, density, temperature, specific enthalpy, and specific internal energy. The documentation explicitly states that two thermodynamic variables among the available property set, plus composition where applicable, complete the thermodynamic condition. It also supports a medium preference for independent property variables.

The standard interface provides multiple state constructors, including pressure-temperature, pressure-enthalpy, pressure-entropy, and density-temperature forms. This is direct prior art for reconstructing a thermodynamic state through different coordinates while preserving a common medium-function interface.

The `replaceable package Medium` usage pattern is especially important for RQ-FCI-001. A component can be parameterized by the Medium package rather than by one hard-coded medium implementation. This demonstrates mature architectural separation between component equations and thermodynamic property implementation.

### 4.3 Falsification pressure

Modelica.Media strongly falsifies any broad RQ-FCI claim of the form:

> A simulation architecture is novel because thermodynamic variables, medium equations, or state-construction coordinates can change behind a stable interface.

That capability is established prior art.

However, the reviewed Modelica.Media documentation does **not establish** an explicit architecture-wide rule equivalent to:

> For a fixed physical scope, formulation changes may modify formulation-specific state schema, closure, and implementation artifacts while specific framework ownership, representation, extension-governance, and conformance semantics are required to remain invariant; physical-scope expansion is separately classified as a higher-level revision.

Modelica.Media is therefore the strongest falsification source in v0.1, but it does not yet collapse the narrowest RQ-FCI candidate.

## 5. OpenFOAM Thermophysical Models

### 5.1 Evidence reviewed

Primary documentation:

- OpenFOAM thermophysical model configuration
- OpenFOAM thermophysical modelling release documentation describing dictionary-based thermodynamics selection and energy-form selection

Representative sources:

- https://doc.openfoam.com/2212/tools/processing/models/thermophysical/
- https://openfoam.org/release/2-2-0/thermophysical-multiphase-energy/

### 5.2 Findings

OpenFOAM exposes a thermophysical package configuration composed from separable selections such as equation of state, thermodynamics, transport, mixture, and energy form.

The documented `energy` choices include:

- sensible enthalpy;
- sensible internal energy;
- absolute enthalpy; and
- absolute internal energy.

The OpenFOAM Foundation documentation further describes user selection of `h` versus `e` style energy formulations and run-time selection of thermodynamics package components through configuration.

This is direct prior art for changing important thermodynamic formulation elements without treating every change as a replacement of the entire application architecture.

### 5.3 Falsification pressure

OpenFOAM strongly falsifies broad claims that:

- energy-coordinate selection is architecturally novel;
- thermodynamic closure composition behind a common solver/application structure is novel; or
- enthalpy/internal-energy choice necessarily requires a new top-level architecture.

The reviewed evidence does not, however, formalize a general architecture-level invariant set spanning state ownership, representation responsibility, extension governance, and conformance semantics. It also does not provide an explicit general rule distinguishing a same-scope formulation substitution from a true physical-scope expansion in the precise sense required by RQ-FCI-001.

## 6. MOOSE Fluid Properties and Materials

### 6.1 Evidence reviewed

Primary documentation:

- MOOSE Fluid Properties module
- MOOSE Materials system and stateful material properties
- FluidProperties material formulations

Representative sources:

- https://mooseframework.inl.gov/releases/moose/2022-06-13/modules/fluid_properties/
- https://mooseframework.inl.gov/releases/moose/2024-03-08/syntax/Materials/
- https://mooseframework.inl.gov/releases/moose/v1.0.0/modules/fluid_properties/index.html

### 6.2 Findings

The Fluid Properties module explicitly documents property access using alternative thermodynamic formulations, including:

- `(p, T)` pressure-temperature; and
- `(v, e)` specific-volume / specific-internal-energy.

MOOSE documentation includes `FluidPropertiesMaterialPT` and `FluidPropertiesMaterialVE`, which is particularly relevant because it goes beyond a single state-query API and exposes different formulation-facing material adapters.

The Materials system also distinguishes properties normally computed on demand from **stateful material properties** that retain old and older timestep values. This is direct evidence that stored-versus-derived/stateful assignment is not globally fixed by the framework's material abstraction.

### 6.3 Falsification pressure

MOOSE strongly pressures any claim that a framework contribution consists merely of:

- supporting alternate primary thermodynamic variable pairs;
- selecting PT versus VE property formulations;
- choosing whether material properties are recomputed or retained as historical state; or
- preserving producer/consumer material-property interfaces while implementations vary.

The reviewed evidence does not establish the full RQ-FCI change-containment rule across architecture, ownership, interface semantics, representation responsibility, extension admissibility, and conformance semantics.

## 7. Cantera ThermoPhase

### 7.1 Evidence reviewed

Primary documentation:

- Cantera `ThermoPhase` class reference

Representative source:

- https://www.cantera.org/stable/cxx/dc/d38/classCantera_1_1ThermoPhase.html

### 7.2 Findings

`ThermoPhase` exposes multiple state-setting paths, including:

- `TP`;
- `HP`;
- `UV`;
- `SP`;
- `SV`;
- `ST`;
- `TV`; and related forms.

The same thermodynamic phase abstraction can therefore be driven to a state using different independent property pairs.

This is strong prior art for state-coordinate interchangeability and for hiding model-specific recovery work behind a stable thermodynamic API.

### 7.3 Scope limitation

Cantera is used here as **state/property abstraction prior art**, not as a direct comparator for ThermoCore's framework architecture.

The presence of multiple setters does not by itself prove that a full simulation can replace its governing persistent-state formulation with no architectural impact. It therefore falsifies broad state-coordinate novelty claims but only partially addresses formulation-change isolation.

## 8. CoolProp AbstractState

### 8.1 Evidence reviewed

Primary documentation:

- CoolProp Low-Level Interface / `AbstractState`

Representative source:

- https://coolprop.org/coolprop/LowLevelAPI.html

### 8.2 Findings

CoolProp `AbstractState` supports many generated input pairs for state updates and exposes multiple property backends through a common state abstraction. The documentation demonstrates changing the backend while retaining the `AbstractState` interaction pattern, including HEOS and REFPROP-backed usage where available.

This establishes mature prior art for:

- alternative thermodynamic input pairs;
- backend substitution; and
- stable property-query abstraction around different state/property implementations.

### 8.3 Scope limitation

As with Cantera, CoolProp is a thermophysical property/state library rather than a direct full-framework comparator. It does not by itself establish architecture-wide formulation change containment.

## 9. Cross-Source Prior-Art Exclusions

The following are **established prior art or too broadly established to remain RQ-FCI candidate contributions** after v0.1:

1. Supporting multiple thermodynamic independent-variable pairs.
2. Reconstructing one thermodynamic state from pressure-temperature, pressure-enthalpy, pressure-entropy, density-temperature, or related coordinate pairs.
3. Selecting enthalpy versus internal-energy style energy forms.
4. Selecting sensible versus absolute energy conventions.
5. Replacing an equation-of-state / thermophysical / fluid-property package behind a stable abstraction.
6. Using a common property or medium interface while internal property equations differ.
7. Allowing some quantities to be computed on demand and others to retain timestep history.
8. Parameterizing simulation components by a replaceable medium/property package.
9. Using alternative thermodynamic backends under one property-query interface.

These capabilities shall not be presented as ThermoCore novelty.

## 10. Strongest Falsification Pressure

### 10.1 Modelica.Media

Modelica.Media is the strongest general architectural counterexample in this first pass because it combines:

- a reusable medium contract;
- a `ThermodynamicState` abstraction;
- multiple independent-variable/state-construction forms;
- replaceable medium packages; and
- component reuse across different medium/property implementations.

Any RQ-FCI contribution framed merely as **"formulation independence through a stable thermodynamic interface"** would be too broad after this evidence.

### 10.2 OpenFOAM

OpenFOAM provides the strongest direct pressure on **energy-form substitution** because enthalpy/internal-energy and sensible/absolute choices are explicit thermophysical-model configuration options within established solver/application families.

### 10.3 MOOSE

MOOSE provides strong pressure on **independent-variable and state-retention flexibility**, especially the explicit PT versus VE material formulations and the distinction between on-demand and stateful material properties.

## 11. Surviving Candidate Distinction

After excluding the established capabilities above, one narrower candidate distinction remains under survey:

> **Architecture-Wide Formulation Change Containment under Fixed Physical Scope** — an explicit framework-level rule set that distinguishes formulation-local changes from Framework-level changes by testing whether higher-level architecture, ownership, information-category semantics, interface semantics, Material Representation responsibility, Extension governance, and Conformance semantics remain invariant while formulation-specific state coordinates, closure/recovery relations, material parameters, and implementation details change.

The candidate also includes an explicit negative boundary:

> a change that enlarges the governing physical scope, introduces previously absent governing responsibility, or makes the existing thermodynamic formulation incomplete is not classified as an equivalent-scope formulation substitution merely because software interfaces can be adapted.

This surviving distinction is narrower than:

- state-variable substitution;
- property-package polymorphism;
- medium replacement;
- thermophysical run-time selection;
- equation-of-state substitution; or
- generic software modularity.

## 12. Current Evidence Status of the Surviving Candidate

### Supported by reviewed evidence as motivation

The reviewed systems demonstrate that substantial formulation/property variability can be absorbed below stable abstractions. This supports the engineering relevance of investigating change containment.

### Not established as unique or novel

The v0.1 survey has **not** established that the surviving candidate is absent from prior literature or frameworks.

In particular, the current pass has not yet completed a focused search for:

- formal replaceability constraints in Modelica beyond the medium API itself;
- OpenFOAM solver/thermo compatibility rules that may encode a stronger change boundary;
- framework-level software product-line or constitutive-interface work explicitly classifying formulation-local versus architecture-level change;
- standards or simulation-framework governance documents with explicit invariance/conformance rules under model substitution.

## 13. Candidate Matched Formulation Families for Later Evaluation

The following remain **Under Survey** and are not yet pre-registered experiments.

### FCI-S1 — Single-Phase Coordinate Substitution

Same fixed physical scope and material law, represented through two coordinate choices such as:

```text
Formulation A: energy-like Persistent State -> derive Temperature
Formulation B: Temperature-like Persistent State -> derive energy quantity
```

Purpose: test whether state-schema and recovery changes can remain formulation-local while Framework-level semantics remain stable.

### FCI-S2 — Energy-Basis Substitution

Same physical scope with specific versus volumetric or total-cell energy representation, provided conversion semantics are complete and geometry/mass assumptions are unchanged.

Purpose: distinguish representational/formulation basis changes from architecture changes.

### FCI-S3 — Equivalent Phase-Change Formulation Pair

Two bounded phase-change formulations that represent the same declared scope but assign different primary/derived quantities or closure organization.

Purpose: test whether Material Representation and Framework Interfaces can remain semantically stable while implementation details change.

### FCI-S4 — Closure Substitution

Same declared state-space and scope, different constitutive/property closure implementation.

Purpose: negative/control pressure showing ordinary property/backend substitution should not be misclassified as Framework change.

### FCI-S5 — Scope-Expansion Boundary Control

Introduce governing physics such as compressibility, mass transport, reactive transport, or another responsibility that is absent from the fixed comparison scope.

Purpose: confirm that true physical-scope expansion is reclassified rather than counted as a failure of same-scope formulation isolation.

## 14. Relationship to RQ-EFM-001 and RQ-ISO-001

RQ-FCI-001 shall not absorb the completed questions.

The intended decision relationship is:

```text
RQ-FCI-001
Can a same-scope formulation substitution remain architecture-local?
        |
        v
RQ-EFM-001
Does the selected formulation remain complete for a proposed mechanism/scope?
        |
        v
RQ-ISO-001
If ordinary extension status is accepted, does extension participation preserve Core-State authority/non-promotion?
```

This order is conceptual, not a runtime pipeline.

If a proposed FCI comparison changes the physical scope enough that the existing thermodynamic formulation becomes incomplete, the case leaves the same-scope FCI comparison and returns to the RQ-EFM admissibility boundary.

## 15. Falsification / Reclassification Conditions

The surviving RQ-FCI candidate shall be narrowed or rejected if future evidence establishes that an existing framework or formal method already defines an equivalent architecture-wide rule that:

1. freezes higher-level architecture/responsibility semantics;
2. permits formulation-local state/closure/package changes;
3. distinguishes implementation change from architecture change;
4. distinguishes same-scope substitution from physical-scope expansion; and
5. ties that distinction to explicit compatibility, conformance, or governance criteria.

A partial match shall narrow the candidate rather than be ignored.

## 16. Research Gap Readiness

```text
Research Gap Analysis readiness: NO-GO
```

Reason:

The v0.1 matrix has removed several broad novelty candidates but has not yet completed the strongest direct-antecedent search for architecture-wide formulation change governance.

A second bounded evidence pass is justified before Research Gap Analysis.

## 17. Recommended v0.2 Search

The next survey should remain narrow and target only likely full or near-full matches:

1. Modelica.Media / Modelica replaceable package semantics and medium-state selection beyond basic API capability.
2. OpenFOAM thermophysical package compatibility and solver-energy coupling rules.
3. MOOSE application/module interfaces where PT/VE or closure substitution changes governing variable sets.
4. Constitutive-model or thermodynamic-package interface literature that explicitly discusses model replacement without host-architecture change.
5. Simulation software product-line / plugin governance work that distinguishes model substitution from scope/architecture revision.
6. Standards or conformance systems, if any, that define compatibility under physical-model substitution.

The v0.2 pass should stop if it either:

- finds a direct antecedent satisfying the full change-containment candidate; or
- reaches bounded saturation with only the already-established lower-level abstractions.

## 18. Current Disposition

**Broad formulation flexibility is established prior art.**

**State-coordinate interchangeability, alternative energy forms, property/backend substitution, and stable property/medium interfaces are not RQ-FCI contributions.**

**A narrower candidate survives: explicit architecture-wide formulation change containment under fixed physical scope, including a formal boundary that reclassifies true scope expansion instead of treating it as formulation-local change.**

**Candidate status: UNDER SURVEY.**

**Novelty / priority: NOT ESTABLISHED.**

**Framework Specification impact: NONE.**

**Next step: bounded v0.2 direct-antecedent stress test before Research Gap Analysis.**
