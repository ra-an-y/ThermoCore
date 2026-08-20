# Purpose

Validation exists to evaluate implemented ThermoCore behavior for explicitly declared validation purposes and to preserve the evidence required to understand those evaluations.

Validation is separate from Verification and Framework Conformance. Verification checks implementation correctness against specified behavior; Validation evaluates the implemented result against an external physical, analytical, experimental, or reference-model basis appropriate to the stated validation purpose.

---

# Scope

This directory is responsible for Validation plans, benchmark definitions, preserved run evidence, result summaries, and conclusions tied to identified repository versions.

Framework Specifications, research investigation, implementation source, routine Verification tests, and Reference Applications do not belong here.

---

# Evidence Requirements

Each Validation record shall identify at least:

- the evaluated repository version or commit;
- the applicable Framework Specification baseline and, when relevant, reference-formulation specification;
- the validation purpose;
- the external reference or benchmark basis;
- the procedure and relevant configuration;
- the observed result;
- the evaluation conclusion; and
- known limitations or deviations.

Historical evidence shall remain traceable and shall not be silently rewritten to represent a later implementation or run.

---

# Current Status

Active — bounded reference-formulation Validation.

Two independent caloric Validation tracks are now preserved.

## H2O caloric benchmark

Plan and result:

- `Reference_Formulation_Caloric_Validation_Plan.md`
- `Reference_Formulation_Caloric_Validation_v0.1.md`
- `Data/reference_caloric_benchmark_v0.1.csv`

External basis: IAPWS reference formulations.

Recorded status:

```text
COMPLETED — errors reported
```

## Gallium caloric benchmark

Plan and result:

- `Reference_Formulation_Gallium_Caloric_Validation_Plan.md`
- `Reference_Formulation_Gallium_Caloric_Validation_v0.1.md`
- `Data/gallium_caloric_benchmark_v0.1.csv`

External basis: NIST Chemistry WebBook SRD 69 / NIST-JANAF condensed-phase thermochemistry.

Recorded status:

```text
COMPLETED — errors reported
```

These descriptive statuses record successful completion of the declared comparison procedures and preservation of measured errors. No physical PASS/FAIL threshold has been adopted for either track, and no Framework Conformance conclusion is implied.

---

# Relationship

Validation follows the repository workflow:

```text
Research -> Evidence -> Specification -> Implementation -> Verification -> Validation
```

Passing `Tests/` does not by itself establish Validation success. Validation Evidence may later support Framework Conformance assessment, but it does not define Framework Conformance.

---

# Document Status

This document is a repository directory guide.

It provides repository navigation and Validation evidence-handling guidance consistent with Repository Governance.

It is not a Framework Specification and does not define thermodynamic semantics.
