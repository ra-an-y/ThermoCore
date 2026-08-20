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

The scalar CPU baseline is defined by:

- `Reference_CPU_Performance_Evaluation_Plan.md`

Its executed result is preserved as:

- `Reference_CPU_Performance_Evaluation_v0.1.md`

The first batch/SIMD candidate track is defined by:

- `Reference_CPU_SIMD_Evaluation_Plan.md`

Its executed result is preserved as:

- `Reference_CPU_SIMD_Evaluation_v0.1.md`

Latest recorded result status:

```text
COMPLETED — measurements reported
```

The SIMD candidate remains performance evidence only and has not been promoted into `Framework/` implementation source by this track.

No performance PASS/FAIL threshold has been adopted. Absolute GitHub-hosted runner timings remain environment-specific observations rather than universal ThermoCore guarantees.

No GPU, Unity, engine, mobile, or production-hardware performance claim is implied by these initial CPU tracks.

---

# Relationship

The repository workflow currently reaches Performance Evaluation after the first bounded implementation has completed initial Verification and independent caloric Validation benchmarks:

```text
Research -> Evidence -> Specification -> Implementation -> Verification -> Validation -> Performance Evaluation
```

Performance measurements do not redefine Framework semantics or ownership.

A performance candidate that appears promising still requires explicit implementation review and correctness Verification before it becomes part of an implementation profile.

---

# Document Status

This document is a repository directory guide.

It is not a Framework Specification and does not define thermodynamic semantics or performance requirements.
