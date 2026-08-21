# Reference CPU Batch Attribution Plan v0.1

Status: Performance Attribution Plan — Non-Normative

---

## 1. Purpose

This plan decomposes the measured cost difference between the current scalar recovery API and the integrated semantics-preserving batch recovery API.

The objective is not merely to report another speedup ratio. It is to identify which implementation layers plausibly account for the observed difference while preserving the distinction between measured evidence and causal interpretation.

This evaluation does not modify Framework semantics, thermodynamic formulation, ownership, or conformance rules.

---

## 2. Baseline

Repository baseline:

```text
b3f9fdda82cdaf78ec6612b4af442c5f5d4775c1
```

This is `main` after PR #55, which integrated `ReferenceThermodynamicFormulation.RecoverBatch(...)` and expanded repository Verification to 21/21 PASS.

---

## 3. Attribution Principle

The benchmark uses the same prevalidated `ThermodynamicState[]`, the same compiled Material Configuration, the same deterministic mixed-phase values, the same process, and the same hosted runner for every scenario within one execution.

It then changes one implementation layer at a time.

The measured differences are differential observations, not mathematically additive costs. JIT inlining, code generation, cache behavior, branch prediction, memory traffic, and instruction scheduling can interact, so one timing difference shall not be presented as an exact universal cost assigned to a single source line.

---

## 4. Compared Scenarios

### A. `scalar_public_recovery`

For every cell:

```text
ThermodynamicState
    -> ReferenceThermodynamicFormulation.Recover(...)
    -> DerivedThermodynamicState
```

This is the current per-state public API path.

### B. `formal_batch_recovery`

For each pass:

```text
ReadOnlySpan<ThermodynamicState>
    -> ReferenceThermodynamicFormulation.RecoverBatch(...)
    -> Span<DerivedThermodynamicState>
```

This is the integrated formal batch API.

The A-to-B comparison measures the practical benefit of the formal batch abstraction as implemented, including boundary-validation amortization and any JIT/code-generation consequences of moving the loop inside the API.

### C. `local_derived_recovery`

A Performance-only local loop evaluates the same piecewise equations from the same `ThermodynamicState[]`, caches Material parameters in local variables before the timed loop, and still constructs one real `DerivedThermodynamicState` per cell.

The B-to-C comparison probes residual cost associated with the formal batch implementation structure versus a locally flattened/cached relation while keeping the semantic Derived State type and its validation.

### D. `local_raw_struct_recovery`

The same local equations write a benchmark-local two-double readonly struct with no constructor validation.

This struct has the same two-value shape as `DerivedThermodynamicState` but has no thermodynamic authority and exists only under `Performance/`.

The C-to-D comparison is designed to probe the cost associated with `DerivedThermodynamicState` constructor validation while keeping an array-of-structs output shape.

### E. `local_primitive_recovery`

The same local equations write separate primitive `double[]` Temperature and liquid-fraction arrays.

The D-to-E comparison probes array-of-structs versus split primitive-output representation after constructor validation has already been removed.

### F. `derived_output_only`

Precomputed valid Temperature and liquid-fraction values are copied into real `DerivedThermodynamicState` values. No thermodynamic recovery equation is evaluated inside this timed scenario.

### G. `raw_struct_output_only`

The same precomputed values are copied into the benchmark-local raw two-double struct.

The F-to-G comparison independently probes real Derived State validation versus an otherwise similar unvalidated struct output path.

### H. `primitive_output_only`

The same precomputed values are copied into separate primitive arrays.

The G-to-H comparison independently probes struct-output versus split primitive-output representation without thermodynamic equation cost.

---

## 5. Semantic Gate

Before timing evidence is accepted:

- formal batch output shall match scalar public recovery;
- local Derived State output shall match scalar public recovery;
- local raw-struct output shall match scalar public recovery numerically;
- local primitive output shall match scalar public recovery numerically;
- exact `h_s*` and `h_l*` values shall appear in the deterministic dataset; and
- no non-finite reference output shall be accepted.

Tolerance:

```text
1e-10
```

The raw struct and primitive arrays are measurement devices only. Numerical equivalence does not make them Framework State or Representation.

A semantic-gate failure makes the attribution run `INVALID`.

---

## 6. Working Sets

The same cell-count scale used by prior CPU evaluations is retained:

```text
1,024
16,384
262,144
1,048,576 cells
```

Each timed sample targets approximately:

```text
8,388,608 cell operations
```

---

## 7. Timing Procedure

For each scenario and working-set size:

```text
warmup samples: 3
timed samples: 7
reported timing: median / minimum / maximum
```

The timed region uses `Stopwatch.GetTimestamp()` and reports median managed allocation.

Checksums are calculated after the timed region so output consumption remains observable without adding checksum traversal to the measured interval.

Reported metrics:

- median elapsed milliseconds;
- minimum and maximum elapsed milliseconds;
- nanoseconds per cell;
- million cells per second;
- median managed bytes allocated; and
- observable checksum.

---

## 8. Attribution Ratios

For each size, the report shall calculate at least:

```text
formal batch benefit        = scalar public / formal batch
formal-vs-local-derived     = formal batch / local derived
Derived-validation effect   = local derived / local raw struct
output-layout effect        = local raw struct / local primitive
output-only validation      = derived output only / raw struct output only
output-only layout          = raw struct output only / primitive output only
```

These names describe measured scenario ratios only. They shall not be converted into exact additive percentages of total runtime without stronger evidence.

---

## 9. Environment Metadata

The runner preserves:

- .NET runtime version;
- OS description;
- architecture;
- logical processor count;
- GC mode;
- CPU model when available;
- stopwatch frequency; and
- GitHub Actions run ID.

Absolute hosted-run timing remains environment-specific.

---

## 10. Interpretation Boundary

The strongest supported conclusion should be stated in terms such as:

```text
"The measured difference is primarily associated with ... under this implementation and environment."
```

It shall not claim that a source-level operation has a universal fixed cost.

The benchmark is specifically designed to distinguish:

- formal API / batch-boundary amortization;
- Derived State constructor validation;
- struct versus split primitive output layout; and
- the thermodynamic equation path shared by the compared recovery scenarios.

SIMD is intentionally excluded because PR #54 found its incremental benefit near parity and environment-sensitive.

---

## 11. Excluded Scope

This track does not evaluate:

- multi-thread CPU execution;
- SIMD promotion;
- explicit AVX intrinsics;
- GPU execution;
- physical Validation;
- Framework Conformance;
- engine integration; or
- application frame-time suitability.

---

## 12. Result Status

Result status shall be one of:

```text
COMPLETED — attribution measurements reported
INVALID — semantic gate or execution failure
INCOMPLETE — required evidence missing
```

No post-hoc performance PASS threshold shall be introduced.

---

## 13. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
