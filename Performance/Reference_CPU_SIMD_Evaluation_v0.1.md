# Reference CPU SIMD Evaluation v0.1

Status: Performance Evaluation Result — COMPLETED — measurements reported  
Performance PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Evaluation Purpose

This record preserves the first executed comparison of the current scalar ThermoCore recovery path against scalar-batch and hardware-SIMD batch candidates.

The purpose is limited to determining whether a contiguous prevalidated specific-enthalpy batch and `System.Numerics.Vector<double>` execution are promising enough to justify a later implementation proposal.

The SIMD candidate evaluated here remains under `Performance/`. It is not part of the Framework implementation represented by this result.

---

## 2. Evaluated Baseline and Candidate Snapshot

Scalar implementation baseline:

```text
a3dec25ec8e0c844d7b49dafb054c2541210161e
```

This is `main` after PR #52.

Initial executed candidate snapshot:

```text
branch: performance/reference-cpu-simd-v0.1
candidate commit: f4e861bd7683fe4a842e861efa45626ec8c701a1
workflow: Reference CPU SIMD Performance Evaluation
workflow run: 32403928666
job: 96538408686
conclusion: success
```

No `Framework/` or `Materials/` source was changed for this evaluation.

---

## 3. Environment

The primary run reported:

```text
runtime: .NET 8.0.30
os: Ubuntu 24.04.4 LTS
architecture: X64
logical processors visible: 2
GC mode: workstation
CPU: AMD EPYC 7763 64-Core Processor
Vector.IsHardwareAccelerated: True
Vector<double>.Count: 4
stopwatch frequency: 1,000,000,000 Hz
```

The reported vector width corresponds to four `double` lanes for this process and runtime environment.

These values identify the execution environment only. They are not a universal ThermoCore hardware requirement or guarantee.

---

## 4. Compared Paths

The run compared:

1. `scalar_reference_recovery` — constructs the current `ThermodynamicState` value and calls `ReferenceThermodynamicFormulation.Recover(...)` for each cell;
2. `scalar_batch_recovery` — evaluates the same piecewise equations over contiguous `double[]` buffers without SIMD; and
3. `simd_batch_recovery` — evaluates the same batch equations through `Vector<double>` lanes, with scalar tail handling.

The batch paths assume that their specific-enthalpy input buffer has already satisfied the finite-state invariant. Therefore the scalar-reference versus scalar-batch difference includes batch representation plus removal of repeated per-cell API/constructor validation overhead. It is not interpreted as a pure layout result.

The scalar-batch versus SIMD-batch ratio is the cleaner estimate of incremental SIMD benefit because those two paths share the same buffer representation and prevalidated-input assumption.

---

## 5. Semantic Equivalence Gate

Before timing evidence was accepted, the runner compared both batch candidates against the current scalar reference implementation over 1,048,576 deterministic mixed-phase values including exact transition boundaries.

Observed maximum disagreement:

```text
comparison tolerance: 1e-10

scalar batch max |Temperature error|:      0
scalar batch max |liquid-fraction error|:  0
SIMD batch max |Temperature error|:        0
SIMD batch max |liquid-fraction error|:    0

semantic equivalence gate: PASS
```

This is a performance-candidate equivalence gate, not a replacement for repository Verification. Any later promotion of a batch/SIMD implementation still requires implementation review and correctness Verification.

---

## 6. Timing Procedure

For each working-set size:

```text
warmup samples: 2
timed samples: 5
target recoveries per timed sample: 1,048,576
reported timing: median / minimum / maximum
```

The timed region uses static timestamp measurement and reported zero median managed allocation for every scenario in this primary run.

---

## 7. Primary Measurement Table

| Cells | Path | Median ms | ns/cell | Million cells/s | Median allocated bytes |
|---:|---|---:|---:|---:|---:|
| 1,024 | scalar reference | 5.841150 | 5.570555 | 179.52 | 0 |
| 1,024 | scalar batch | 2.566510 | 2.447615 | 408.56 | 0 |
| 1,024 | SIMD batch | 2.148595 | 2.049060 | 488.03 | 0 |
| 16,384 | scalar reference | 5.842062 | 5.571424 | 179.49 | 0 |
| 16,384 | scalar batch | 2.605744 | 2.485031 | 402.41 | 0 |
| 16,384 | SIMD batch | 4.230556 | 4.034573 | 247.86 | 0 |
| 262,144 | scalar reference | 5.846529 | 5.575685 | 179.35 | 0 |
| 262,144 | scalar batch | 2.809587 | 2.679431 | 373.21 | 0 |
| 262,144 | SIMD batch | 2.247761 | 2.143632 | 466.50 | 0 |
| 1,048,576 | scalar reference | 6.000889 | 5.722894 | 174.74 | 0 |
| 1,048,576 | scalar batch | 3.457432 | 3.297264 | 303.28 | 0 |
| 1,048,576 | SIMD batch | 2.535902 | 2.418425 | 413.49 | 0 |

---

## 8. Same-Run Comparative Ratios

Ratios are calculated from the median values above.

| Cells | Batch-path / reference speedup | SIMD / reference speedup | SIMD / scalar-batch speedup |
|---:|---:|---:|---:|
| 1,024 | 2.276× | 2.719× | 1.195× |
| 16,384 | 2.242× | 1.381× | 0.616× |
| 262,144 | 2.081× | 2.601× | 1.250× |
| 1,048,576 | 1.736× | 2.366× | 1.363× |

For the largest working set, the SIMD batch candidate reduced the recorded recovery median from approximately `6.001 ms` to `2.536 ms`, a same-run ratio of approximately `2.37×` relative to the current scalar reference path.

Relative to the scalar batch candidate at the same size, the incremental SIMD ratio was approximately `1.36×`.

---

## 9. 16,384-Cell Anomaly

The 16,384-cell SIMD median was slower than the scalar batch median in the primary run:

```text
scalar batch: 2.605744 ms
SIMD batch:   4.230556 ms
SIMD / batch: 0.616×
```

The SIMD sample range was also visibly wider than neighboring working-set measurements.

This result is preserved rather than filtered out. A single hosted-run anomaly is insufficient to establish a stable size-specific regression or to discard SIMD as a candidate. A final integrated-branch rerun and any later promotion work should treat this point as evidence of runtime/environment variability that requires continued measurement.

---

## 10. Interpretation

The primary run supports three bounded observations:

1. a prevalidated contiguous batch recovery path can materially reduce measured per-cell overhead relative to the current per-value scalar API under this benchmark;
2. hardware SIMD provided additional speedup over the scalar batch path for three of four measured working-set sizes, including the largest working set; and
3. one intermediate size showed a contrary result, so the evidence does not support a universal SIMD speedup guarantee.

The result therefore supports treating batch/SIMD recovery as a promising implementation candidate, not as an already adopted optimization.

---

## 11. Promotion Boundary

This result does **not** authorize silently replacing `ReferenceThermodynamicFormulation.Recover(...)`.

A production batch path would still need to define and verify:

- where the finite-state invariant is enforced for a prevalidated state buffer;
- how batch storage relates to the semantic Thermodynamic State without introducing a second owner;
- exact boundary behavior at `h_s*` and `h_l*`;
- scalar-tail behavior;
- behavior when hardware vector acceleration is unavailable; and
- equivalence with the scalar formulation across Verification cases.

Only after those questions are resolved should the candidate be promoted from `Performance/` into implementation source.

---

## 12. Excluded Claims

This result does not establish:

- multi-thread CPU performance;
- explicit AVX2 / AVX-512 performance;
- GPU performance;
- engine or mobile performance;
- application frame-time suitability;
- physical Validation;
- Framework Conformance; or
- a universal speedup factor.

---

## 13. Conclusion

Result status:

```text
COMPLETED — measurements reported
```

No performance PASS/FAIL threshold was defined before execution, so no performance `PASS` claim is created from the measured ratios.

The candidate is sufficiently promising to justify a later implementation-design step for a verified batch recovery path, while the preserved 16,384-cell anomaly and hosted-run variability remain part of the evidence.

---

## 14. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
