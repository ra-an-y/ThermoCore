# Formulation Change Isolation Evidence Matrix

Version: 0.2  
Status: Completed Direct-Antecedent Stress Test — Non-Normative  
Research Question: RQ-FCI-001 — Thermodynamic Formulation Change Isolation  
Tracking Issue: #112

---

## 1. Objective

This second evidence pass directly stress-tests the only candidate distinction that survived `Formulation_Change_Isolation_Evidence_Matrix_v0.1.md`:

> **Architecture-Wide Formulation Change Containment under Fixed Physical Scope**.

The v0.1 survey already established substantial prior art for alternative thermodynamic state pairs, energy forms, property-package substitution, backend replacement, replaceable medium packages, and stateful/on-demand material properties. Those capabilities are therefore excluded from candidate contribution scope.

The v0.2 question is narrower:

> Do established languages/frameworks already permit formulation/media replacement behind stable component, balance-equation, connector, or interface abstractions subject to explicit compatibility constraints, such that the remaining RQ-FCI-001 idea is better treated as a ThermoCore-specific engineering/conformance property rather than an independent research gap?

This document does not modify Framework Specification, implementation, Verification, Validation, or Performance artifacts. It does not establish novelty, priority, or universal equivalence among the reviewed systems.

---

## 2. Four Evidence Layers

To avoid treating all forms of replaceability as equivalent, v0.2 separates four layers.

| Layer | Meaning | RQ-FCI relevance |
|---|---|---|
| **L1 — Property / Backend Substitution** | EOS, property implementation, interpolation table, or backend can change behind a stable property API | Already established in v0.1; not a surviving contribution |
| **L2 — Formulation / State-Coordinate Substitution** | Independent variables, energy coordinate, or closure-facing state representation can change | Already established in v0.1; not a surviving contribution |
| **L3 — Component / Balance / Interface Preservation under Compatible Substitution** | Higher-level equations/components/connectors remain stable while a lower-level formulation/media package changes, subject to an explicit compatibility contract | Direct pressure on the surviving candidate |
| **L4 — Framework-Wide Governance of Change Containment** | One framework explicitly governs which architecture, ownership, information categories, extension rules, and conformance semantics must remain invariant, and reclassifies incompatible physical-scope change | Closest remaining ThermoCore-specific distinction |

Evidence labels:

- **Established** — directly supported by reviewed source material.
- **Strong partial antecedent** — source establishes most of the structural idea but not the complete ThermoCore-specific authority/conformance framing.
- **Contextual** — relevant interface/encapsulation evidence but not a thermodynamic formulation architecture comparator.
- **Not established in reviewed public evidence** — no sufficient evidence found in this bounded pass; this is not a claim of absence.

---

## 3. Direct-Antecedent Summary Matrix

| Source | L1 property/backend | L2 formulation/state coordinate | L3 higher-level preservation + compatibility boundary | L4 framework-wide ownership/governance/conformance rule | v0.2 disposition |
|---|---|---|---|---|---|
| **Modelica Language + Modelica.Media** | Established | Established | **Established — strongest direct antecedent** | Strong partial antecedent | Collapses most of surviving candidate |
| **OpenFOAM thermophysical framework** | Established | Established | **Established for energy-form / thermo-package compatibility** | Partial | Strong direct boundary evidence |
| **MOOSE Fluid Properties / Materials** | Established | Established | Partial / established at material-property abstraction | Not established | Supporting, not decisive |
| **FMI** | Contextual | Internal formulation intentionally opaque | Established at exchange-interface contract level | Not thermodynamic architecture governance | Contextual only |

---

## 4. Modelica Language — Replaceability Has an Explicit Compatibility Boundary

### 4.1 Evidence reviewed

Primary sources:

- Modelica Language Specification, Chapter 7 — Inheritance, Modification, and Redeclaration  
  https://specification.modelica.org/master/inheritance-modification-and-redeclaration.html
- Modelica Language Specification, interface/type relationships  
  https://specification.modelica.org/maint/3.5/interface-or-type-relationships.html

### 4.2 Findings

The language specification defines `replaceable`, `redeclare`, and `constrainedby` semantics. A redeclared class or component may replace the original only when the replacement is compatible with the constraining interface.

This is not merely dynamic dispatch or a convenience API. It is an explicit **substitution boundary**:

```text
replaceable implementation
        |
        v
constraining interface
        |
        v
compatible redeclaration allowed
incompatible redeclaration rejected
```

The specification therefore already formalizes a generic version of the following principle:

> internal implementation may vary while a declared outer interface remains stable, but substitution is not universal; it is bounded by compatibility with the constraining contract.

### 4.3 RQ-FCI impact

This is strong prior art against framing RQ-FCI-001 as a novel general rule that "formulation-local changes may vary while higher-level architecture remains stable, provided the replacement satisfies a compatibility boundary."

That pattern is an established language-level concept.

What Modelica language semantics do **not** establish by themselves is ThermoCore's specific partition of:

- Thermodynamic State ownership;
- Material Representation responsibility;
- Extension admissibility;
- Framework Conformance categories.

Those are framework-specific semantics rather than evidence for a separate general research gap.

**Classification:**

```text
Generic compatible-substitution boundary: ESTABLISHED PRIOR ART
ThermoCore-specific authority partition: NOT ADDRESSED BY LANGUAGE SPECIFICATION
```

---

## 5. Modelica.Media — Direct Thermodynamic Antecedent to Formulation Change Containment

### 5.1 Evidence reviewed

Primary sources:

- Modelica.Media UsersGuide  
  https://doc.modelica.org/om/Modelica.Media.UsersGuide.html
- Modelica.Media medium usage / replaceable Medium  
  https://doc.modelica.org/Modelica%204.0.0/Resources/helpDymola/Modelica_Media_UsersGuide_MediumUsage.html
- `Modelica.Media.Interfaces.PartialMedium`  
  https://doc.modelica.org/om/Modelica.Media.Interfaces.PartialMedium.html
- `PartialMedium.BaseProperties`  
  https://doc.modelica.org/Modelica%204.0.0/Resources/helpWSM/Modelica/Modelica.Media.Interfaces.PartialMedium.BaseProperties.html
- Static State Selection  
  https://doc.modelica.org/Modelica%204.0.0/Resources/helpOM/Modelica.Media.UsersGuide.MediumDefinition.StaticStateSelection.html

### 5.2 Balance equations and medium equations are explicitly decoupled

The Modelica.Media User's Guide explicitly states that **balance equations and media model equations are decoupled**. It further explains that the same balance equations can be used for media whose independent variables differ, including pressure-temperature and pressure-specific-enthalpy formulations, and even for incompressible versus compressible medium models.

This is substantially stronger than the v0.1 finding of "multiple state constructors behind one property interface."

The architectural pattern is:

```text
Higher-level balance equations
            |
            | stable relation
            v
Medium abstraction / BaseProperties
            |
            | medium-specific equations and independent states
            v
Concrete medium formulation
```

The medium's independent variables may change without requiring the higher-level balance equations to be rewritten merely because the medium uses another state-coordinate choice.

### 5.3 Connector semantics are intentionally independent of medium state selection

The User's Guide also states that the independent variables of a medium model do not determine the definition of the fluid connector port. The medium implementation can therefore choose a different internal state representation while component connection structure remains based on the shared connector semantics.

This is a direct antecedent to **L3 — component/interface preservation under formulation substitution**.

### 5.4 Replaceable Medium packages are constrained by a common medium interface

`Modelica.Fluid` components use a replaceable `Medium` package, and custom fluid models can be used when they satisfy the interfaces defined in `Modelica.Media.Interfaces`.

Combined with the Modelica language `constrainedby` / compatibility semantics, the evidence establishes both sides of the containment idea:

1. **variation is allowed below the abstraction**, and
2. **variation is bounded by compatibility with the abstraction**.

### 5.5 Static state selection further weakens a state-schema novelty claim

Modelica.Media explicitly supports preferred medium state variables and static state selection. The physical medium may prefer different state coordinates while the component/balance formulation remains structurally reusable.

This does not mean every medium or physical scope is interchangeable. It means the distinction between:

```text
internal thermodynamic state choice
```

and

```text
higher-level balance/component definition
```

is mature prior art.

### 5.6 Direct-antecedent verdict

Modelica.Media is no longer merely "strong falsification pressure." After the v0.2 focused review, it is a **direct antecedent to most of the surviving RQ-FCI candidate**.

The evidence supports:

- formulation/state-coordinate variation;
- reusable balance equations;
- connector independence from internal medium independent variables;
- replaceable medium packages;
- compatibility-bounded substitution.

What remains unmatched is primarily ThermoCore's own framework-wide authority vocabulary and governance decomposition. That unmatched vocabulary does not by itself establish an independent research gap.

**Classification:**

```text
L1: ESTABLISHED
L2: ESTABLISHED
L3: ESTABLISHED — DIRECT THERMODYNAMIC ANTECEDENT
L4: STRONG PARTIAL ANTECEDENT; THERMOCORE-SPECIFIC AUTHORITY TERMS NOT PRESENT
```

---

## 6. OpenFOAM — Energy Substitution Is Explicitly Bounded by Solver Compatibility

### 6.1 Evidence reviewed

Primary source:

- OpenFOAM Foundation, Thermophysical Modelling / run-time selectable energy solution variable  
  https://openfoam.org/release/2-2-0/thermophysical-multiphase-energy/

### 6.2 Generalized energy variable

OpenFOAM introduced a general energy variable `he` so a solver can use either internal energy `e` or enthalpy `h` at run time with the corresponding thermodynamics package.

This is direct evidence that a solver can be written around a generalized formulation boundary rather than hard-coding one energy coordinate.

### 6.3 Compatibility is checked rather than assumed

The same documentation shows that a solver validates which energy forms it supports. The substitution therefore follows a pattern such as:

```text
generalized solver energy abstraction
            |
            v
candidate energy form / thermo package
            |
            v
solver compatibility validation
      | supported -> proceed
      | unsupported -> reject
```

This is important for RQ-FCI-001 because it demonstrates that mature prior art already distinguishes:

- a formulation substitution the surrounding solver is designed to admit; from
- an incompatible choice that is not silently treated as equivalent.

### 6.4 Limits of the antecedent

OpenFOAM does not use ThermoCore's ownership or Conformance vocabulary, and the reviewed source does not define a single framework-wide invariant set covering representation and extension governance.

Nevertheless, it strongly undermines a candidate contribution based on the mere existence of an explicit **"local formulation variation versus higher-level compatibility failure"** boundary.

**Classification:**

```text
Energy-form substitution under stable solver abstraction: ESTABLISHED
Explicit compatibility/rejection boundary: ESTABLISHED
ThermoCore-specific architecture/ownership/conformance decomposition: NOT ESTABLISHED
```

---

## 7. MOOSE — Supporting Evidence, Not the Decisive Antecedent

### 7.1 Evidence reviewed

Primary sources:

- MOOSE Fluid Properties module  
  https://mooseframework.inl.gov/releases/moose/2022-06-13/modules/fluid_properties/
- `FluidPropertiesMaterialPT`  
  https://mooseframework.inl.gov/source/materials/FluidPropertiesMaterialPT.html
- MOOSE Materials system / stateful material properties  
  https://mooseframework.inl.gov/docs/PRs/32882/site/syntax/Materials/index.html

### 7.2 Findings

MOOSE separately exposes `(p,T)` and `(v,e)` fluid-property-facing formulations and allows a fluid property user object to be sampled into material properties. Its Materials system normally computes properties on demand but supports retained old/older state where history is required.

These are strong examples of framework tolerance for different formulation-facing variables and different retention requirements.

### 7.3 RQ-FCI impact

MOOSE continues to support the conclusion that implementation/state-schema identity is not required for higher-level framework reuse.

However, the reviewed evidence does not add a stronger architecture-wide substitution boundary than the combined Modelica / OpenFOAM evidence.

**Classification:** supporting prior art; not decisive for the v0.2 survival decision.

---

## 8. FMI — Contextual Interface-Contract Evidence Only

### 8.1 Evidence reviewed

Primary source:

- Functional Mock-up Interface Specification  
  https://fmi-standard.org/docs/main/

### 8.2 Findings

FMI specifies an explicit exchange interface in which variables, types, causality, variability, and related semantics are declared in `modelDescription.xml`, while the internal mathematical implementation of an FMU is encapsulated.

This provides contextual evidence for the mature engineering principle:

> internal model realization can remain opaque while interoperability depends on an explicit external contract.

### 8.3 Scope limitation

FMI does not establish thermodynamic formulation change containment inside one framework and does not define ThermoCore-style ownership or extension admissibility.

It is therefore **contextual only** and is not used as evidence that RQ-FCI-001 is directly anticipated in full.

---

## 9. Strongest Antecedent Combination

The strongest direct-antecedent result is not one isolated API. It is the combination of two established Modelica mechanisms:

```text
Modelica language:
  replaceable / redeclare / constrainedby
  -> replacement permitted only under compatible constraining interface

Modelica.Media:
  balance equations decoupled from medium equations
  internal medium independent variables need not define connector variables
  replaceable Medium package
  -> thermodynamic formulation/media variation can remain below stable component/balance abstractions
```

This combination already captures most of the conceptual structure that v0.1 attempted to preserve as the narrow candidate.

OpenFOAM independently reinforces the same boundary from another direction:

```text
energy formulation selectable
but solver compatibility is explicitly validated
```

The surviving claim therefore cannot safely be:

> ThermoCore introduces the idea that a thermodynamic formulation may change locally while higher-level architecture stays stable, with incompatible substitutions rejected.

That claim is too broad after v0.2.

---

## 10. Reassessment of the v0.1 Candidate

### v0.1 candidate

> **Architecture-Wide Formulation Change Containment under Fixed Physical Scope**.

### v0.2 result

The candidate does **not survive as a defensible independent Research Gap in its v0.1 form**.

The evidence establishes mature antecedents for:

- compatible implementation/package substitution;
- decoupling balance equations from thermodynamic medium equations;
- changing medium independent-state choices while retaining connector/component structure;
- changing energy coordinate inside a generalized solver formulation; and
- explicitly rejecting unsupported formulation choices.

The only remaining distinction is that ThermoCore names a broader invariant set across its own architecture:

- component responsibility;
- Thermodynamic State ownership;
- information-category semantics;
- Framework Interface semantics;
- Material Representation responsibility;
- Extension admissibility; and
- Framework Conformance semantics.

That is valuable **Framework engineering and conformance structure**, but current evidence does not justify treating the act of collecting those ThermoCore-specific invariants around an established compatible-substitution pattern as a separate research contribution.

---

## 11. Relationship to RQ-EFM-001 and RQ-ISO-001

The downgrade does not weaken either completed research line.

### RQ-EFM-001

RQ-EFM-001 already provides the substantive boundary for cases where a supposed local change actually makes the selected thermodynamic formulation incomplete or enlarges governing physical scope.

Therefore the negative boundary proposed by RQ-FCI-001 is substantially governed by an existing, independently evaluated ThermoCore result:

```text
same declared physical scope + complete formulation
    -> local formulation substitution may be considered

scope/formulation completeness violated
    -> RQ-EFM-001 admissibility / Core-revision boundary applies
```

### RQ-ISO-001

If a formulation or extension introduces local information, RQ-ISO-001 already governs authority/non-promotion once the category has been accepted.

Thus much of the proposed RQ-FCI architecture rule can be composed from:

1. established compatible-substitution / medium-decoupling prior art;
2. RQ-EFM-001 formulation-admissibility semantics; and
3. RQ-ISO-001 state-authority semantics.

This further reduces the need for a third independent research-gap claim.

---

## 12. Recommended Reclassification

RQ-FCI-001 should be **downgraded from candidate independent Research Gap to a ThermoCore engineering/conformance property**.

Recommended property name:

> **Formulation Change Containment Property**

Meaning:

> For a fixed declared thermodynamic scope, a conforming implementation should be able to replace one admissible formulation with another without requiring Framework Specification changes solely because formulation-specific state coordinates, closure/recovery relations, material parameterization, or implementation structures differ; any observed Framework-level change must be traced to an actual semantic/authority/scope dependency rather than presumed from the formulation substitution itself.

This is a useful property to verify against ThermoCore, but the v0.2 evidence does not support treating the property as an independent research contribution.

---

## 13. What Should Still Be Tested

Research-line downgrade does **not** imply that ThermoCore automatically possesses the property.

A later engineering/conformance verification may still use matched formulations to test whether the current Framework abstraction is actually stable in practice.

Recommended examples:

- same-scope single-phase energy-coordinate substitution;
- specific versus volumetric energy basis with equivalent physics;
- equivalent closure/state-coordinate implementations;
- one deliberate physical-scope expansion as a negative control routed to RQ-EFM-001.

The purpose would be:

```text
verify ThermoCore's claimed abstraction property
```

not:

```text
establish a new general research gap
```

---

## 14. v0.2 Disposition

```text
Broad formulation flexibility:
ESTABLISHED PRIOR ART

Compatible formulation/package substitution behind stable abstractions:
ESTABLISHED PRIOR ART / DIRECT ANTECEDENT

Balance-equation and connector independence from internal medium state choice:
ESTABLISHED PRIOR ART IN MODELICA.MEDIA

Explicit rejection of unsupported formulation choice:
ESTABLISHED PRIOR ART IN MODELICA / OPENFOAM CONTEXTS

v0.1 independent candidate gap:
NOT SUPPORTED AS AN INDEPENDENT RESEARCH GAP WITHIN THIS BOUNDED REVIEW

Recommended status of RQ-FCI-001:
DOWNGRADE TO THERMOCORE ENGINEERING / CONFORMANCE PROPERTY

Research Gap Analysis readiness:
NO-GO — DO NOT CREATE A POSITIVE GAP ANALYSIS

Novelty / priority:
NOT ESTABLISHED

Framework Specification impact:
NONE FROM THIS EVIDENCE PASS
```

---

## 15. Research Integrity Note

This is a useful negative research result.

RQ-FCI-001 was opened because ThermoCore's formulation-neutral Framework appeared to imply a potentially distinct change-isolation claim. The first evidence pass narrowed the idea; the second focused pass found direct antecedents strong enough that preserving the idea as an independent contribution would overstate the evidence.

The appropriate outcome is therefore to close or reclassify the research line rather than continue searching only for evidence favorable to the original candidate.

This result strengthens the project evidence chain by distinguishing:

- **established external architectural practice**;
- **ThermoCore-specific engineering property**; and
- **independently supported research contributions**.

---

## 16. Current Decision

**RQ-FCI-001 should not proceed to a positive Research Gap Analysis.**

The bounded v0.2 direct-antecedent review finds that the central formulation-change containment pattern has substantial direct prior art, particularly in Modelica's compatibility-constrained replaceability combined with Modelica.Media's decoupling of balance equations, connector semantics, and medium state selection.

The remaining ThermoCore-specific invariant set is better treated as an engineering/conformance property that can be verified later, using RQ-EFM-001 and RQ-ISO-001 as already-established boundary/authority rules.

No Framework Specification change is authorized by this document.