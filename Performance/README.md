# Purpose

Performance Evaluation measures implementation cost and scaling for explicitly identified ThermoCore versions and execution environments.

Performance evidence is separate from Verification, Validation, and Framework Conformance. A fast implementation is not thereby physically valid or Framework-conforming, and a physically validated implementation is not thereby fast.

---

# Scope

This directory stores performance plans, executable benchmark harnesses, preserved result reports, and environment-specific measurements.

Framework Specifications, physical Validation evidence, and routine correctness Verification do not belong here.

---

# Evidence Requirements

Each Performance Evaluation record shall identify at least:

- the evaluated repository commit;
- the implementation profile under test;
- the benchmark scenarios and cell counts;
- warmup and repetition procedure;
- runtime / operating-system information available to the benchmark;
- reported timing and throughput metrics;
- whether values are absolute measurements or only comparative observations; and
- known environmental and methodological limitations.

Historical measurements shall remain tied to their recorded environment and commit. They shall not be silently rewritten as if they applied to a later implementation or machine.

---

# Current Status

Active — bounded CPU reference-implementation Performance Evaluation.

The first track is defined by:

- `Reference_CPU_Performance_Evaluation_Plan.md`

No GPU, Unity, engine, mobile, or production-hardware performance claim is implied by this initial CPU track.

---

# Relationship

The repository workflow currently reaches Performance Evaluation after the first bounded implementation has completed initial Verification and independent caloric Validation benchmarks:

```text
Research -> Evidence -> Specification -> Implementation -> Verification -> Validation -> Performance Evaluation
```

Performance measurements do not redefine Framework semantics or ownership.

---

# Document Status

This document is a repository directory guide.

It is not a Framework Specification and does not define thermodynamic semantics or performance requirements.
