# RQ-FCI-001 Closure and Reclassification

Version: 0.1  
Status: Closed and Reclassified — Non-Normative Research Record  
Research Question: RQ-FCI-001 — Thermodynamic Formulation Change Isolation

---

## 1. Purpose

This document closes RQ-FCI-001 as an independent research-gap line and records the surviving result as an engineering / conformance property rather than an independent research contribution.

It preserves the negative result produced by the bounded prior-art review and does not reinterpret that result as novelty.

## 2. Evidence Basis

This closure depends on:

- `Research/05_Research_Questions/RQ_FCI_001_Definition_v0.1.md`;
- `Research/01_Evidence_Matrix/Formulation_Change_Isolation_Evidence_Matrix_v0.1.md`;
- `Research/01_Evidence_Matrix/Formulation_Change_Isolation_Evidence_Matrix_v0.2.md`;
- completed RQ-EFM-001 and RQ-ISO-001 dispositions;
- the current Framework Specification baseline, including `Extension_Boundary.md` v1.1.

The v0.1 survey established substantial prior art for alternative thermodynamic coordinates, energy-form selection, replaceable thermodynamic/property packages, stable property interfaces, and stored-versus-derived/stateful flexibility.

The v0.2 direct-antecedent stress test found stronger prior art for compatible formulation substitution behind stable abstractions, particularly through Modelica replaceability / constraining-interface semantics, Modelica.Media balance/media decoupling, and OpenFOAM solver-to-energy-form compatibility checks.

## 3. Independent Research-Gap Disposition

```text
RQ-FCI-001 independent Research Gap:
NOT SUPPORTED WITHIN THE BOUNDED REVIEW
```

The evidence does not support continuing to narrow the original candidate indefinitely in order to preserve a separate research contribution.

The following broad or intermediate claims are treated as established prior art or too directly anteceded to support an independent RQ-FCI contribution claim:

- thermodynamic state-coordinate substitution;
- enthalpy/internal-energy or related energy-form selection;
- replaceable medium/property/backend packages;
- stable thermodynamic property interfaces across implementation changes;
- compatible substitution constrained by interface or solver requirements;
- separation of balance/component equations from internal thermodynamic-property formulation;
- stored-versus-derived or stateful/on-demand material-property flexibility.

No Research Gap Analysis shall be opened for RQ-FCI-001 on this evidence baseline.

## 4. Reclassified Result

The surviving useful project property is named:

> **Formulation Change Containment Property**

For a fixed declared physical scope, a compatible change of thermodynamic formulation may change formulation-specific artifacts such as:

- Persistent / Derived quantity assignment;
- state coordinate or schema;
- energy basis;
- closure / recovery relations;
- material parameterization;
- implementation or reference-formulation code.

Such changes do not, by themselves, imply that ThermoCore Framework architecture, ownership, information-category semantics, Framework Interface semantics, Material Representation responsibility, Extension governance, or Conformance semantics must change.

This property is an engineering / conformance expectation. It is not claimed as novel, first, universally applicable, or evidence that all thermodynamic formulations are interchangeable.

## 5. Containment Boundary

A formulation change counts as a candidate containment case only when the declared physical scope remains materially the same and the replacement formulation is itself complete for that scope.

A change shall not be counted as a containment failure merely because formulation-specific implementation artifacts differ.

Conversely, a change that introduces a new governing physical responsibility, enlarges the claimed thermodynamic scope, or makes the previous state-space / closure incomplete is not an ordinary same-scope formulation substitution.

Those cases route to the RQ-EFM-001 boundary and may require explicit Core / formulation / specification revision or scope narrowing.

## 6. Relationship to RQ-EFM-001

RQ-EFM-001 remains the admissibility boundary for deciding whether the selected thermodynamic formulation is complete for the claimed physical scope while mechanism-local or cross-domain state remains external.

Therefore:

```text
same-scope compatible formulation substitution
    -> Formulation Change Containment Property may apply

scope expansion or formulation incompleteness
    -> RQ-EFM-001 / explicit revision boundary applies
```

RQ-FCI-001 does not supersede or weaken Ordinary Extension Admissibility.

## 7. Relationship to RQ-ISO-001

RQ-ISO-001 remains the authority / non-promotion rule after information categories and ordinary-extension status are accepted.

Changing a formulation does not grant extension-local, material, representation, or communicated information authority to redefine mandatory Core State.

The reclassified Formulation Change Containment Property therefore does not replace RQ-ISO-001.

## 8. Framework and Implementation Impact

```text
Framework Specification change: NONE
Production implementation change: NONE
Validation change: NONE
Verification change in this closure task: NONE
Performance change: NONE
```

Implementation identity is explicitly not required. Different valid formulations may legitimately use different code, state layouts, recovery functions, material parameters, or reference-formulation documents while still preserving the same higher-level Framework semantics.

## 9. Optional Downstream Verification

The property remains worth testing as engineering / conformance evidence.

A later verification task may use matched same-scope formulation pairs and check separately whether changes are confined to formulation-local artifacts while the applicable Framework-level authority and interface semantics remain unchanged.

Suitable matched pairs may include:

- two valid single-phase state-coordinate choices for the same bounded material law;
- specific versus volumetric energy representation under a fixed conversion convention;
- two same-scope closure implementations with equivalent declared thermodynamic responsibility.

A boundary control should include a true scope-expansion case that is expected to be reclassified rather than treated as containment failure.

Such testing would verify a ThermoCore engineering property. It would not reopen the independent Research Gap claim unless new external evidence changes the prior-art position.

## 10. Final Disposition

```text
Research line: RQ-FCI-001
Independent Research Gap: NOT SUPPORTED WITHIN THE BOUNDED REVIEW
Research contribution claim: CLOSED
Surviving value: Formulation Change Containment Property
Classification: ENGINEERING / CONFORMANCE PROPERTY
Research Gap Analysis: NO-GO
Novelty / priority: NOT ESTABLISHED
Framework Specification impact: NONE
```

RQ-FCI-001 is therefore closed as an independent research question.

Its negative result is retained as part of the ThermoCore evidence history, and its surviving engineering property may be verified later without being represented as a separate research contribution.
