# Reference CPU SIMD Evaluation Plan v0.2

Status: Corrective Performance Evaluation Plan — Non-Normative

---

## 1. Purpose

This plan corrects a fairness limitation in `Reference_CPU_SIMD_Evaluation_v0.1.md` before any batch/SIMD implementation decision is made.

The v0.1 scalar-reference timing reconstructed a new `ThermodynamicState` from raw specific enthalpy inside every timed cell recovery. The established scalar CPU baseline in PR #52 instead recovers from an already-existing `ThermodynamicState[]` buffer. Therefore the v0.1 scalar-reference versus batch ratios include repeated state-constructor validation that is not representative of the existing stored-state recovery path.

v0.2 preserves v0.1 as historical evidence and performs a corrected same-run comparison.

---

## 2. Evaluated Baseline

Repository baseline:

```text
6beaba024626a4c5c3625dd75d12d9123a3e79d3
```

This is `main` after PR #53.

No Framework or Material implementation source shall be changed by this evaluation.

---

## 3. Corrected Compared Paths

### 3.1 Scalar reference recovery

A `ThermodynamicState[]` buffer is constructed before warmup and timing.

Timed recovery performs only:

```text
ReferenceThermodynamicFormulation.Recover(states[i], material)
```

No per-cell `ThermodynamicState` reconstruction occurs inside the timed region.

### 3.2 Scalar batch recovery

A contiguous `double[]` specific-enthalpy buffer, derived from the same prevalidated state values before timing, is recovered by the scalar batch equations.

### 3.3 SIMD batch recovery

The same prevalidated specific-enthalpy buffer is recovered with `System.Numerics.Vector<double>` and a scalar tail.

The scalar-batch versus SIMD-batch comparison continues to isolate incremental SIMD behavior within the batch representation.

---

## 4. Semantic Equivalence Gate

Both batch candidates shall match the scalar reference recovery over the deterministic mixed-phase dataset, including exact `h_s*` and `h_l*` boundaries.

Tolerance:

```text
1e-10
```

Non-finite output or disagreement beyond tolerance makes the execution `INVALID`.

---

## 5. Measurement Stabilization

To reduce hosted-run noise relative to v0.1:

```text
warmup samples: 3
timed samples: 7
target recoveries per timed sample: 8,388,608
```

The same working-set sizes are retained:

```text
1,024
16,384
262,144
1,048,576 cells
```

The timed region uses allocation-free timestamp measurement.

---

## 6. Required Output

For each path and size, record:

- median / minimum / maximum elapsed time;
- ns per cell;
- million cells per second;
- median managed allocation inside the timed region;
- checksum;
- CPU / runtime / OS metadata;
- `Vector.IsHardwareAccelerated`;
- `Vector<double>.Count`.

Same-run ratios:

```text
batch / reference = scalar_reference_time / scalar_batch_time
SIMD / reference  = scalar_reference_time / simd_batch_time
SIMD / batch      = scalar_batch_time / simd_batch_time
```

---

## 7. Interpretation Boundary

v0.1 remains useful for semantic equivalence and hosted-run variability observations, but its scalar-reference speedup ratios shall not be used as the primary evidence for promoting a batch implementation.

v0.2 is the corrective performance comparison for that decision.

No post-hoc performance PASS threshold is introduced.

Result status:

```text
COMPLETED — corrected measurements reported
INVALID — equivalence or execution failure
INCOMPLETE — required evidence missing
```

---

## 8. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
