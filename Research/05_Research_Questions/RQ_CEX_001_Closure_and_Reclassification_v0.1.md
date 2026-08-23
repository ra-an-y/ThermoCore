# RQ-CEX-001 Closure and Reclassification

Version: 0.1  
Status: Closed and Reclassified — Non-Normative Research Record  
Research Question: RQ-CEX-001 — Conservative Cross-Boundary Energy Exchange

---

## 1. Purpose

This document closes RQ-CEX-001 as an independent research-gap line and records the surviving result as an engineering / conformance property rather than an independent research contribution.

It preserves the negative result produced by the bounded direct-antecedent review and does not reinterpret established conservation-interface semantics, their composition with existing ThermoCore rules, or the surviving engineering value as novelty.

## 2. Evidence Basis

This closure depends on:

- `Research/05_Research_Questions/RQ_CEX_001_Definition_v0.1.md`;
- `Research/01_Evidence_Matrix/Conservative_Energy_Exchange_Evidence_Matrix_v0.1.md`;
- `Research/01_Evidence_Matrix/Conservative_Energy_Exchange_Evidence_Matrix_v0.2.md`;
- the completed RQ-ISO-001 final disposition;
- the completed RQ-EFM-001 final disposition; and
- the current Framework Specification baseline, including `Data_Flow.md`, `Framework_Interfaces.md`, and `Extension_Boundary.md` v1.1.

The v0.1 evidence pass established substantial prior art for:

- heat-flow-rate and other energy-bearing interface quantities;
- quantity basis and support, including volumetric, boundary, sum, surface-integral, and volume-integral forms;
- sign and orientation conventions;
- communication-point and communication-interval semantics;
- zero-sum flow relations;
- power-bond and residual-energy accounting;
- source-density versus boundary-flux distinctions; and
- conservative / integral-preserving mapping semantics.

The v0.2 direct-antecedent stress test then found stronger prior art for the remaining candidate, particularly through:

- Modelica flow and stream connection equations, including connection-set mass / energy conservation with transported specific enthalpy;
- bond-graph 0 / 1 junction power-continuity semantics and separation of source, storage, dissipation, and lossless interconnection;
- port-Hamiltonian / Dirac-structure power-conserving interconnection with open / boundary power ports; and
- Simscape conserving-port Through / Across balance as an independent engineering precedent.

## 3. Independent Research-Gap Disposition

```text
RQ-CEX-001 independent Research Gap:
NOT SUPPORTED WITHIN THE BOUNDED REVIEW
```

The evidence does not support continuing to narrow the RQ-CEX-001 candidate indefinitely in order to preserve a separate research contribution.

The following claims are treated as established prior art, directly anteceded, or too compositionally dependent on already-established concepts to support an independent RQ-CEX contribution claim:

- energy or power conservation across physical connections;
- heat-flow-rate interface semantics;
- sign / orientation conventions;
- zero-sum flow balance;
- power-bond semantics;
- residual power and residual energy over communication intervals;
- stream-carried enthalpy with connection-level energy balance;
- conservative / integral-preserving mapping;
- source-density versus boundary-flux semantics;
- separation of subsystem state while using conserving interconnections; and
- distinction between internal energy transfer and external / boundary power through established port or bond structures.

No Research Gap Analysis shall be opened for RQ-CEX-001 on this evidence baseline.

## 4. Reclassified Result

The surviving useful project property is named:

> **Conservative Exchange Accounting Property**

For an admitted energy-bearing interaction that crosses a ThermoCore architectural boundary, the communicated contribution should have enough declared physical meaning that its thermodynamic accounting role and applicable conservation target are unambiguous for the claimed scope.

Depending on the physical interaction, this may require the applicable semantics to identify distinctions such as:

- energy versus power / rate;
- total, specific, volumetric, areal, flux, work, or other declared quantity basis;
- sign or orientation convention;
- temporal support where a value is interval-dependent;
- external source / sink versus internal redistribution versus cross-domain conversion versus boundary exchange; and
- the conservation relation or accounting target applicable to that interaction.

The property does not require every exchange to use one universal payload schema or one universal set of metadata. It requires only that the information needed to avoid semantic ambiguity in the applicable physical accounting be present at the level where that distinction matters.

This property is an engineering / conformance expectation. It is not claimed as novel, first, universally applicable, or sufficient by itself to guarantee numerical conservation.

## 5. Single-Authority Boundary

Within ThermoCore, Thermodynamic Computation remains the exclusive Framework Core responsibility that writes Thermodynamic State.

Therefore an extension, external domain, boundary mechanism, mapper, or other communicating responsibility may provide a physically meaningful contribution through applicable Framework Interfaces without acquiring Thermodynamic State write authority.

The surviving ThermoCore-specific accounting rule is consequently bounded as follows:

> one physical contribution shall not be semantically treated simultaneously as both an already-accounted internal transfer and an additional external thermodynamic source under the same claimed accounting scope.

This is an engineering consistency rule derived from:

1. established conserving-port / bond / connection semantics; and
2. the already-supported RQ-ISO-001 state-authority / non-promotion rule.

The composition of those established ideas is not treated as a new independent research contribution.

This rule is also not a transport-level `exactly once` delivery guarantee. It does not prescribe message IDs, transaction protocols, queues, retries, or duplicate-delivery handling.

## 6. Semantic Conservation versus Numerical Conservation

RQ-CEX-001 retains an important analytical distinction:

```text
semantic conservation meaning
    !=
numerical / discretization conservation achieved by an implementation
```

A contribution can be semantically well-defined while a numerical method still introduces conservation error through:

- timestep approximation;
- interpolation or extrapolation;
- nonmatching-grid mapping;
- operator splitting;
- partitioned iteration choices;
- solver tolerances; or
- other discretization and coupling effects.

Conversely, a numerical result that appears globally balanced does not by itself establish that the communicated quantities had correct or unambiguous physical meaning.

Accordingly, the Conservative Exchange Accounting Property may support Framework Conformance or implementation verification, but it does not replace numerical conservation verification or physical Validation.

## 7. Relationship to RQ-ISO-001

RQ-ISO-001 remains the authority / non-promotion rule after information categories and ordinary-extension status are accepted.

Energy-bearing communication does not transfer ownership of Thermodynamic State, grant write authority, or make external / extension / cross-domain state part of mandatory Core State merely because it contributes to thermodynamic evolution.

The Conservative Exchange Accounting Property therefore does not replace RQ-ISO-001. Its ThermoCore-specific single-authority aspect is subordinate to and derived from the existing state/write authority rule.

## 8. Relationship to RQ-EFM-001

RQ-EFM-001 remains the admissibility boundary when correct thermodynamic representation or evolution cannot be achieved through semantically honest exchange under the existing thermodynamic formulation and state-space.

Therefore:

```text
exchange semantics sufficient for the claimed formulation
    -> Conservative Exchange Accounting Property may apply

required governing thermodynamic information cannot remain outside Core State
or the existing formulation / closure is incomplete
    -> RQ-EFM-001 / explicit revision boundary applies
```

RQ-CEX-001 does not authorize hiding required governing thermodynamic state inside an exchange payload, mapper, extension-owned state, or implementation-specific indirection.

## 9. Framework and Implementation Impact

```text
Framework Specification change: NONE
Production implementation change: NONE
Validation change: NONE
Verification change in this closure task: NONE
Performance change: NONE
```

The closure does not add a new Framework Interface semantic, prescribe a communication format, or modify `Data_Flow.md`, `Framework_Interfaces.md`, or `Extension_Boundary.md`.

The evidence is sufficient to close the independent research claim, but not to justify a new normative specification merely to restate established conservation-interface practice.

## 10. Optional Downstream Verification

The surviving property remains worth testing as engineering / conformance evidence.

A later verification task may use bounded cases such as:

- external energy injection with an explicit quantity basis and sign convention;
- internal pairwise redistribution whose closed-pair net energy change is zero apart from declared external contributions;
- cross-domain dissipative conversion where electrical, mechanical, or other external-domain state remains separately owned while the thermal contribution is supplied to Thermodynamic Computation;
- boundary heat-flux transfer with a declared orientation and integrated conservation target; and
- a negative case where the same physical contribution is intentionally classified twice to confirm that duplicate thermodynamic accounting is detectable.

Such verification should distinguish semantic-accounting conformance from numerical conservation error.

It may also include a boundary case in which exchange semantics are insufficient because the governing thermodynamic formulation itself is incomplete; that case should route to RQ-EFM-001 rather than be reported as a failure of the Conservative Exchange Accounting Property.

These activities would verify a ThermoCore engineering property. They would not reopen the independent Research Gap claim unless materially new external evidence changes the prior-art position.

## 11. Final Disposition

```text
Research line: RQ-CEX-001
Independent Research Gap: NOT SUPPORTED WITHIN THE BOUNDED REVIEW
Research contribution claim: CLOSED
Surviving value: Conservative Exchange Accounting Property
Classification: ENGINEERING / CONFORMANCE PROPERTY
Research Gap Analysis: NO-GO
Novelty / priority: NOT ESTABLISHED
Framework Specification impact: NONE
```

RQ-CEX-001 is therefore closed as an independent research question.

Its negative result is retained as part of the ThermoCore evidence history. The surviving Conservative Exchange Accounting Property may be used later for implementation-neutral conformance or verification work without being represented as a separate research contribution.
