# Reference CPU SIMD Evaluation Plan v0.1

Status: Performance Evaluation Plan — Non-Normative

---

## 1. Purpose

This plan evaluates whether a contiguous batch representation and hardware-accelerated SIMD execution can reduce the cost of the bounded ThermoCore reference-formulation recovery path without changing thermodynamic semantics.

The evaluation is comparative. It does not create a new Framework component, change Thermodynamic State ownership, redefine the reference formulation, or establish a performance requirement.

---

## 2. Evaluated Baseline

Baseline repository commit:

```text
a3dec25ec8e0c844d7b49dafb054c2541210161e
```

This is `main` after the scalar CPU Performance Evaluation in PR #52.

The authoritative scalar recovery semantics remain those implemented by:

- `Framework/Core/ReferenceThermodynamicFormulation.cs`
- `Framework/Runtime/ThermodynamicState.cs`
- `Framework/Runtime/DerivedThermodynamicState.cs`

The SIMD candidate is evaluated only inside the `Performance/` area in this track. It is not promoted into `Framework/` by this evaluation.

---

## 3. Motivation

The scalar CPU baseline showed that state recovery costs more per cell than the simple specific-enthalpy update path. The first optimization experiment therefore targets recovery before adding threading or GPU execution.

The candidate combines two implementation techniques that are common prerequisites for vector execution:

1. a structure-of-arrays style batch representation for specific enthalpy and derived outputs; and
2. `System.Numerics.Vector<double>` SIMD operations when hardware acceleration is available.

Because layout and SIMD can each affect performance, this evaluation separates them into distinct scenarios.

---

## 4. Compared Scenarios

Each working set is evaluated through three recovery paths.

### 4.1 Scalar reference path

For each specific-enthalpy value:

1. construct the current `ThermodynamicState` value;
2. call `ReferenceThermodynamicFormulation.Recover(...)`; and
3. write Temperature and liquid fraction into output arrays.

This preserves the current public reference implementation path and acts as the semantic and performance baseline for the comparison.

### 4.2 Scalar batch path

The same piecewise equations are evaluated over contiguous `double[]` arrays without `Vector<double>`.

This isolates the effect of a batch / structure-of-arrays layout from SIMD execution.

### 4.3 SIMD batch path

The batch equations are evaluated in `Vector<double>.Count` lanes using `System.Numerics.Vector<double>`, with a scalar tail for lengths that are not exact multiples of the vector width.

The vector path preserves the scalar branch boundaries:

```text
h < h_s*                 -> solid
h_s* <= h <= h_l*        -> latent interval
h > h_l*                 -> liquid
```

---

## 5. Semantic Equivalence Gate

Performance numbers are not accepted unless the batch candidates first agree with the scalar reference implementation over a deterministic equivalence dataset.

The equivalence set shall include:

- solid sensible states;
- exact `h_s*`;
- latent-interval states;
- exact `h_l*`;
- liquid sensible states; and
- a large repeated mixed-phase working set.

The runner shall report maximum absolute Temperature and liquid-fraction disagreement for both candidate paths relative to the scalar reference path.

Tolerance:

```text
1e-10
```

If either candidate exceeds the tolerance or produces a non-finite result, the evaluation is `INVALID` and no speedup claim shall be made.

This gate is an implementation-equivalence check for the performance candidate. It is not a replacement for repository Verification.

---

## 6. Working-Set Sizes

The same cell-count scale used by the scalar baseline is retained:

```text
1,024
16,384
262,144
1,048,576 cells
```

For each size, the runner chooses enough passes to target approximately:

```text
1,048,576 cell recoveries per timed sample
```

This keeps small-array timings from being dominated by a single extremely short traversal while preserving a single pass for the largest working set.

---

## 7. Timing Procedure

For each scenario and working-set size:

```text
warmup samples: 2
timed samples: 5
reported timing: median, minimum, maximum
```

The runner shall use allocation-free timestamp measurement inside the timed region.

Reported metrics include:

- median elapsed milliseconds;
- minimum and maximum elapsed milliseconds;
- nanoseconds per cell recovery;
- million cells per second;
- median managed bytes allocated inside the timed region; and
- a checksum to make output consumption observable.

---

## 8. SIMD Environment Metadata

In addition to the environment fields already used by the scalar baseline, the runner shall record:

```text
Vector.IsHardwareAccelerated
Vector<double>.Count
```

A result obtained with hardware acceleration disabled shall remain valid as an execution record but shall not be interpreted as evidence of hardware SIMD acceleration.

---

## 9. Comparison Metrics

For each working-set size, the result shall report:

```text
batch-layout speedup = scalar_reference_time / scalar_batch_time
SIMD speedup         = scalar_reference_time / simd_batch_time
SIMD-over-batch      = scalar_batch_time / simd_batch_time
```

Speedup values are descriptive ratios for the recorded environment only.

No universal performance factor is inferred from one hosted runner.

---

## 10. Environment Boundary

GitHub-hosted runner timings are environment-specific observations.

This evaluation does not emulate a particular consumer CPU and does not claim reproducible absolute timing across hosted runs. CPU model, runtime, OS, visible processor count, GC mode, vector width, and hardware-acceleration status shall be preserved with the result.

The most meaningful result inside one execution is the same-run ratio between the three candidate paths because they share the same runner allocation.

---

## 11. Excluded Scope

This track does not evaluate:

- multi-threaded execution;
- explicit ISA-specific intrinsics such as AVX2 or AVX-512 APIs;
- GPU execution;
- Unity, Unreal, mobile, or WebGL backends;
- conduction, transport, spatial stencils, or source-distribution kernels;
- application frame-time requirements;
- physical Validation; or
- Framework Conformance.

---

## 12. Interpretation Rule

The SIMD candidate remains an experimental performance implementation until separately promoted through implementation review and correctness Verification.

A favorable performance result does not by itself authorize replacing the scalar Framework implementation.

A neutral or unfavorable result is also useful evidence: it indicates that the chosen batch/SIMD strategy does not justify promotion under the measured conditions.

No post-hoc performance PASS threshold shall be introduced.

Result status shall be one of:

```text
COMPLETED — measurements reported
INVALID — equivalence or execution failure
INCOMPLETE — required evidence missing
```

---

## 13. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
