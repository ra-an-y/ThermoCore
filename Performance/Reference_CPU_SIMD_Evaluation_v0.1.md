# Reference CPU SIMD Evaluation v0.1

Status: Performance Evaluation Result — COMPLETED — measurements reported  
Performance PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Evaluation Purpose

This record preserves the first executed comparison of the current scalar ThermoCore recovery path against scalar-batch and hardware-SIMD batch candidates.

The purpose is limited to determining whether a contiguous prevalidated specific-enthalpy batch and `System.Numerics.Vector<double>` execution are promising enough to justify later implementation work.

The SIMD candidate evaluated here remains under `Performance/`. It is not part of the Framework implementation represented by this result.

---

## 2. Evaluated Baseline and Candidate Snapshots

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

Final integrated-branch confirmation snapshot:

```text
candidate head: 6fe1a5482977674860da40f20f93c7fc34803757
workflow run: 32404174657
job: 96539199315
conclusion: success
```

The existing scalar CPU Performance workflow also completed successfully on the final integrated head.

No `Framework/` or `Materials/` source was changed for this evaluation.

---

## 3. Environment

Both SIMD executions reported the same high-level process environment:

```text
runtime: .NET 8.0.30
os: Ubuntu 24.04.4 LTS
architecture: X64
logical processors visible: 2
GC mode: workstation
CPU model: AMD EPYC 7763 64-Core Processor
Vector.IsHardwareAccelerated: True
Vector<double>.Count: 4
stopwatch frequency: 1,000,000,000 Hz
```

The reported vector width corresponds to four `double` lanes for this process and runtime environment.

The two runs were separate GitHub-hosted allocations even though the reported CPU model matched. Their materially different absolute timings demonstrate that matching model metadata is not enough to make hosted-run microbenchmark timings directly interchangeable.

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

Both the initial run and final integrated run reported:

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

The timed region uses static timestamp measurement. Both preserved executions reported zero median managed allocation for every scenario.

---

## 7. Initial Execution Measurements

Workflow run `32403928666`:

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

Initial same-run median ratios:

| Cells | Batch-path / reference | SIMD / reference | SIMD / scalar batch |
|---:|---:|---:|---:|
| 1,024 | 2.276× | 2.719× | 1.195× |
| 16,384 | 2.242× | 1.381× | 0.616× |
| 262,144 | 2.081× | 2.601× | 1.250× |
| 1,048,576 | 1.736× | 2.366× | 1.363× |

---

## 8. Final Integrated-Branch Measurements

Workflow run `32404174657`:

| Cells | Path | Median ms | ns/cell | Million cells/s | Median allocated bytes |
|---:|---|---:|---:|---:|---:|
| 1,024 | scalar reference | 3.122188 | 2.977551 | 335.85 | 0 |
| 1,024 | scalar batch | 1.490801 | 1.421739 | 703.36 | 0 |
| 1,024 | SIMD batch | 1.268819 | 1.210040 | 826.42 | 0 |
| 16,384 | scalar reference | 4.709042 | 4.490892 | 222.67 | 0 |
| 16,384 | scalar batch | 1.503154 | 1.433519 | 697.58 | 0 |
| 16,384 | SIMD batch | 2.090759 | 1.993903 | 501.53 | 0 |
| 262,144 | scalar reference | 3.411769 | 3.253716 | 307.34 | 0 |
| 262,144 | scalar batch | 1.521128 | 1.450661 | 689.34 | 0 |
| 262,144 | SIMD batch | 1.385141 | 1.320973 | 757.02 | 0 |
| 1,048,576 | scalar reference | 3.244401 | 3.094102 | 323.20 | 0 |
| 1,048,576 | scalar batch | 1.607182 | 1.532728 | 652.43 | 0 |
| 1,048,576 | SIMD batch | 1.933242 | 1.843683 | 542.39 | 0 |

Final same-run median ratios:

| Cells | Batch-path / reference | SIMD / reference | SIMD / scalar batch |
|---:|---:|---:|---:|
| 1,024 | 2.094× | 2.461× | 1.175× |
| 16,384 | 3.133× | 2.252× | 0.719× |
| 262,144 | 2.243× | 2.463× | 1.098× |
| 1,048,576 | 2.019× | 1.678× | 0.831× |

---

## 9. Cross-Run Observation

The strongest repeatable observation across both hosted executions is the scalar batch path relative to the current scalar reference path:

- it was faster at every measured working-set size in both runs;
- the largest working set showed approximately `1.74×` and `2.02×` same-run ratios in the two executions; and
- semantic-equivalence disagreement remained zero under the deterministic gate.

Incremental SIMD benefit was not repeatable across all sizes or both runs:

- SIMD beat scalar batch for three of four sizes in the initial run;
- SIMD beat scalar batch for two of four sizes in the final run;
- the 16,384-cell case was slower under SIMD in both runs; and
- the 1,048,576-cell case changed from approximately `1.36×` faster than scalar batch in the initial run to approximately `0.83×` of scalar-batch speed in the final run.

Because both runs reported the same CPU model, vector width, runtime, and logical processor count, the change cannot be attributed solely to those recorded metadata fields. Hosted-run noise, runtime/JIT state, scheduling, cache behavior, or other uncontrolled execution conditions remain plausible contributors.

The evidence therefore does **not** support a universal or stable incremental SIMD speedup claim from this v0.1 methodology.

---

## 10. Interpretation

The completed result supports two different conclusions that should not be merged into one claim.

### 10.1 Batch-path conclusion

A prevalidated contiguous batch recovery path is a credible optimization candidate. It consistently reduced measured recovery cost relative to the current per-value scalar API across both executions while remaining numerically equivalent in the performance gate.

However, the measured ratio includes removal of repeated per-cell state-constructor validation overhead. A future implementation design must preserve the finite Thermodynamic State invariant at a defined batch boundary rather than simply omitting validation.

### 10.2 SIMD conclusion

`System.Numerics.Vector<double>` is technically viable and produced exact evaluated outputs, but its incremental speed benefit over the scalar batch path is inconclusive under this hosted-run methodology.

The correct next step is not to declare SIMD adopted. It is either to stabilize the microbenchmark methodology before another SIMD decision or to first promote and verify the consistently favorable scalar batch abstraction, then evaluate SIMD on top of that verified implementation.

---

## 11. Promotion Boundary

This result does **not** authorize silently replacing `ReferenceThermodynamicFormulation.Recover(...)`.

A production batch path would still need to define and verify:

- where the finite-state invariant is enforced for a prevalidated state buffer;
- how batch storage relates to semantic Thermodynamic State without introducing a second owner;
- exact boundary behavior at `h_s*` and `h_l*`;
- scalar-tail behavior for any vectorized implementation;
- behavior when hardware vector acceleration is unavailable; and
- equivalence with the scalar formulation across repository Verification cases.

The most defensible candidate for the next implementation-design step is the batch recovery abstraction itself. SIMD should remain an optional optimization technique until repeatable incremental benefit is demonstrated.

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

Evidence disposition:

```text
Batch recovery abstraction: PROMISING — implementation design justified
Incremental SIMD speedup: INCONCLUSIVE — no promotion claim
```

These are evidence interpretations, not new Framework requirements or conformance categories.

---

## 14. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
