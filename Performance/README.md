# Purpose

Performance Evaluation measures implementation cost and scaling for explicitly identified ThermoCore versions and execution environments.

Performance evidence is separate from Verification, Validation, and Framework Conformance. A fast implementation is not thereby physically valid or Framework-conforming, and a physically validated implementation is not thereby fast.

---

# Scope

This directory stores performance plans, executable benchmark harnesses, preserved result reports, environment-specific measurements, and bounded performance-attribution studies.

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

Active — bounded CPU reference-implementation Performance Evaluation and attribution.

The scalar CPU baseline is defined by:

- `Reference_CPU_Performance_Evaluation_Plan.md`

Its executed result is preserved as:

- `Reference_CPU_Performance_Evaluation_v0.1.md`

The first batch/SIMD exploratory track is preserved as:

- `Reference_CPU_SIMD_Evaluation_Plan.md`
- `Reference_CPU_SIMD_Evaluation_v0.1.md`
- `Reference_CPU_SIMD_Evaluation_v0.1_Erratum.md`

A fairness correction to the scalar-reference timing method is defined and executed by:

- `Reference_CPU_SIMD_Evaluation_Plan_v0.2.md`
- `Reference_CPU_SIMD_Evaluation_v0.2.md`

For implementation decisions, the v0.2 corrected comparison supersedes the v0.1 scalar-reference versus batch speedup ratios. The v0.1 record remains historical evidence and is not silently rewritten.

After the formal semantics-preserving batch API was integrated, the first performance-attribution track was defined and executed by:

- `Reference_CPU_Batch_Attribution_Plan_v0.1.md`
- `Reference_CPU_Batch_Attribution_v0.1.md`

The attribution study separates formal batch-boundary effects from `DerivedThermodynamicState` validated construction and from struct-versus-primitive output representation.

Latest attribution result status:

```text
COMPLETED — attribution measurements reported
```

Current bounded evidence disposition:

```text
Formal batch-boundary amortization:
REAL BUT MODEST in the primary attribution run

DerivedThermodynamicState validated construction:
DOMINANT MEASURED COST LAYER in the tested recovery/output path

Struct versus split primitive output layout:
NEAR PARITY for the larger working sets; not the dominant explanation

System.Numerics.Vector<double> SIMD:
NOT JUSTIFIED FOR PROMOTION YET — corrected incremental benefit is near parity / environment-sensitive
```

These are engineering evidence dispositions, not Framework conformance categories or physical PASS/FAIL results.

No performance PASS/FAIL threshold has been adopted. Absolute GitHub-hosted runner timings remain environment-specific observations rather than universal ThermoCore guarantees.

No GPU, Unity, engine, mobile, or production-hardware performance claim is implied by these CPU tracks.

---

# Relationship

The repository workflow currently reaches Performance Evaluation after the first bounded implementation has completed initial Verification and independent caloric Validation benchmarks:

```text
Research -> Evidence -> Specification -> Implementation -> Verification -> Validation -> Performance Evaluation
```

Performance measurements do not redefine Framework semantics or ownership.

A performance candidate that appears promising still requires explicit implementation review and correctness Verification before it becomes part of an implementation profile.

Performance attribution may narrow a later optimization question, but it does not itself authorize removing semantic invariants for speed.

---

# Document Status

This document is a repository directory guide.

It is not a Framework Specification and does not define thermodynamic semantics or performance requirements.
