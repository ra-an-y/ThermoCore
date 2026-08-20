# Reference CPU SIMD Evaluation v0.2

Status: Corrective Performance Evaluation Result — COMPLETED — corrected measurements reported  
Performance PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Purpose

This record corrects the scalar-reference timing method used by `Reference_CPU_SIMD_Evaluation_v0.1.md` before any batch or SIMD optimization is promoted into ThermoCore implementation source.

In v0.1, the timed scalar-reference path reconstructed `ThermodynamicState` from raw specific enthalpy for every cell. The established scalar CPU baseline instead operates on already-existing Thermodynamic State values. v0.2 therefore constructs and validates the state buffer before timing and performs scalar recovery directly from that buffer.

The v0.1 record remains preserved as historical evidence, especially for semantic-equivalence and hosted-run variability observations, but its scalar-reference speedup ratios are superseded by this corrected comparison for implementation decisions.

---

## 2. Baseline and Executed Snapshot

Repository baseline:

```text
6beaba024626a4c5c3625dd75d12d9123a3e79d3
```

This is `main` after PR #53.

Executed corrective snapshot:

```text
branch: performance/reference-cpu-simd-v0.2
candidate commit: 156529b88947eda0dc7fe408046b9e42243eefef
workflow: Reference CPU SIMD Performance Evaluation v0.2
workflow run: 32404819259
job: 96541299421
conclusion: success
```

No `Framework/` or `Materials/` implementation source was changed.

---

## 3. Execution Environment

The run reported:

```text
runtime: .NET 8.0.30
os: Ubuntu 24.04.4 LTS
architecture: X64
logical processors visible: 2
GC mode: workstation
CPU: AMD EPYC 9V74 80-Core Processor
Vector.IsHardwareAccelerated: True
Vector<double>.Count: 4
stopwatch frequency: 1,000,000,000 Hz
```

These values identify this hosted execution only.

---

## 4. Corrected Compared Paths

### 4.1 Scalar reference recovery

A `ThermodynamicState[]` buffer is created before warmup and timing. Each timed cell performs:

```text
ReferenceThermodynamicFormulation.Recover(states[i], material)
```

No `ThermodynamicState` reconstruction occurs inside the timed region.

### 4.2 Scalar batch recovery

A contiguous `double[]` enthalpy buffer is derived from the same prevalidated Thermodynamic State values before timing. The batch path evaluates the same piecewise formulation directly into Temperature and liquid-fraction output arrays.

### 4.3 SIMD batch recovery

The same prevalidated enthalpy buffer is evaluated with `System.Numerics.Vector<double>`, with scalar tail handling.

---

## 5. Remaining Batch-Path Methodological Difference

The v0.2 correction removes the unfair per-cell **input-state construction** cost from the scalar reference path.

A remaining intentional difference still exists between the current scalar API and the batch candidates:

- `ReferenceThermodynamicFormulation.Recover(...)` performs the current per-call material null guard and returns `DerivedThermodynamicState` through its constructor, which validates recovered Temperature and liquid fraction;
- the experimental batch paths write primitive output arrays directly after operating on a prevalidated state buffer and validated material configuration.

Therefore the scalar-reference versus scalar-batch ratio should be interpreted as the effect of a **batch recovery abstraction with amortized invariant enforcement and reduced per-cell API/derived-value validation overhead**, not as a pure memory-layout speedup.

Any production batch implementation must preserve the same semantic invariants at an appropriate batch boundary.

---

## 6. Semantic Equivalence Gate

The corrected run compared the scalar batch and SIMD batch outputs against the current scalar reference recovery over 1,048,576 deterministic mixed-phase states including exact transition boundaries.

Observed maximum disagreement:

```text
equivalence tolerance: 1e-10

scalar batch max |Temperature error|:      0
scalar batch max |liquid-fraction error|:  0
SIMD batch max |Temperature error|:        0
SIMD batch max |liquid-fraction error|:    0

semantic equivalence gate: PASS
```

This is a performance-candidate equivalence gate, not repository Verification.

---

## 7. Timing Procedure

The corrective measurement increased timed work relative to v0.1:

```text
warmup samples: 3
timed samples: 7
target recoveries per timed sample: 8,388,608
```

Working-set sizes remained:

```text
1,024
16,384
262,144
1,048,576 cells
```

All timed scenarios reported median managed allocation of `0` bytes.

---

## 8. Corrected Measurement Table

| Cells | Path | Median ms | ns/cell | Million cells/s | Median allocated bytes |
|---:|---|---:|---:|---:|---:|
| 1,024 | scalar reference | 34.309530 | 4.090015 | 244.50 | 0 |
| 1,024 | scalar batch | 13.013937 | 1.551382 | 644.59 | 0 |
| 1,024 | SIMD batch | 13.502914 | 1.609673 | 621.24 | 0 |
| 16,384 | scalar reference | 32.710394 | 3.899383 | 256.45 | 0 |
| 16,384 | scalar batch | 13.704734 | 1.633732 | 612.10 | 0 |
| 16,384 | SIMD batch | 12.897203 | 1.537466 | 650.42 | 0 |
| 262,144 | scalar reference | 32.335707 | 3.854717 | 259.42 | 0 |
| 262,144 | scalar batch | 10.689043 | 1.274233 | 784.79 | 0 |
| 262,144 | SIMD batch | 10.723284 | 1.278315 | 782.28 | 0 |
| 1,048,576 | scalar reference | 18.712374 | 2.230689 | 448.29 | 0 |
| 1,048,576 | scalar batch | 10.816823 | 1.289466 | 775.51 | 0 |
| 1,048,576 | SIMD batch | 10.558469 | 1.258668 | 794.49 | 0 |

---

## 9. Corrected Same-Run Ratios

| Cells | Batch / reference | SIMD / reference | SIMD / scalar batch |
|---:|---:|---:|---:|
| 1,024 | 2.636× | 2.541× | 0.964× |
| 16,384 | 2.387× | 2.536× | 1.063× |
| 262,144 | 3.025× | 3.015× | 0.997× |
| 1,048,576 | 1.730× | 1.772× | 1.024× |

The corrected comparison therefore changes the interpretation of v0.1 materially.

The batch abstraction still shows a substantial same-run reduction relative to the existing scalar recovery path, even after per-cell Thermodynamic State construction is removed from the baseline.

However, the incremental SIMD ratio relative to scalar batch is close to `1.0×` across all four sizes in this corrected run.

---

## 10. Evidence Interpretation

### 10.1 Batch recovery abstraction

The corrected result strengthens the case for evaluating a real batch recovery API.

The largest working set recorded:

```text
scalar reference: 18.712374 ms
scalar batch:     10.816823 ms
same-run ratio:   1.730×
```

At the other measured sizes, the batch/reference ratio ranged from approximately `2.39×` to `3.03×`.

Because the batch path amortizes or removes repeated per-cell API guards and derived-value constructor validation, these values are evidence for a batch **execution abstraction**, not merely a different array layout.

### 10.2 Incremental SIMD

The corrected run does not show a meaningful or consistent additional benefit from `Vector<double>` over the scalar batch path:

```text
SIMD / scalar-batch ratio range: approximately 0.964× to 1.063×
```

At the largest size the SIMD path was only about `1.024×` faster than scalar batch.

This is too close to parity, especially given the hosted-run variability already observed in v0.1, to justify adding SIMD complexity to the implementation at this stage.

---

## 11. Corrected Disposition

The evidence disposition after the fair-baseline correction is:

```text
Batch recovery abstraction:
PROMISING — implementation design and Verification justified

System.Numerics.Vector<double> SIMD:
NOT JUSTIFIED FOR PROMOTION YET — corrected incremental benefit is near parity
```

`NOT JUSTIFIED FOR PROMOTION YET` is an engineering evidence disposition, not a Framework conformance status or physical PASS/FAIL category.

SIMD may be revisited later on controlled hardware, with different data layouts, explicit ISA intrinsics, or after a verified batch API exists.

---

## 12. Next Implementation Question

Before a batch recovery path enters `Framework/`, implementation work must decide how to preserve existing invariants without reintroducing per-cell overhead.

The key design requirement is:

```text
validate state/configuration at the batch boundary
        ↓
operate over already-valid Thermodynamic State information
        ↓
produce Derived State without changing its semantic classification
```

The batch path must not become a second owner of Thermodynamic State and must not make Temperature or phase fraction Persistent State.

---

## 13. Excluded Claims

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

## 14. Result Status

```text
COMPLETED — corrected measurements reported
```

No performance PASS/FAIL threshold was defined before execution.

---

## 15. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
