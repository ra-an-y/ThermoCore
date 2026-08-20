# Reference CPU Batch Attribution Plan v0.1

Status: Performance Attribution Plan — Non-Normative

---

## 1. Purpose

This plan decomposes the measured cost difference between the current scalar recovery API and the newly integrated semantics-preserving batch recovery API.

The objective is not merely to report another speedup ratio. It is to identify which implementation layers plausibly account for the observed difference while preserving the distinction between measured evidence and causal interpretation.

This evaluation does not modify Framework semantics, thermodynamic formulation, ownership, or conformance rules.

---

## 2. Baseline

Repository baseline:

```text
b3f9fdda82cdaf78ec6612b4af442c5f5d4775c1
```

This is `main` after PR #55, which integrated `ReferenceThermodynamicFormulation.RecoverBatch(...)` and expanded repository Verification to 21/21 PASS.

The actual integrated scalar and batch APIs remain authoritative implementation paths for this comparison.

---

## 3. Attribution Principle

The benchmark uses the same prevalidated `ThermodynamicState[]`, the same compiled Material Configuration, the same deterministic mixed-phase values, the same process, and the same hosted runner for every scenario within one execution.

It then removes or changes one implementation layer at a time.

The measured differences are **differential observations**, not mathematically additive costs. JIT inlining, code generation, cache behavior, branch prediction, memory traffic, and instruction scheduling can interact, so one timing difference shall not be presented as an exact universal cost assigned to a single source line.

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

This is the newly integrated formal batch API and is the primary implementation measurement.

The A-to-B comparison measures the practical benefit of the formal batch abstraction as implemented. It includes boundary-validation amortization and any JIT/code-generation consequences of moving the loop inside the API.

### C. `local_derived_recovery`

A Performance-only local loop evaluates the same bounded piecewise equations from the same `ThermodynamicState[]`, caches Material parameters in local variables before the timed loop, and still constructs one `DerivedThermodynamicState` per cell.

The B-to-C comparison probes residual cost associated with the formal batch implementation structure versus a locally flattened/cached relation while keeping the semantic Derived State output type.

### D. `local_primitive_output_recovery`

The same local equations are evaluated from the same state buffer, but outputs are written directly to separate `double[]` Temperature and liquid-fraction arrays.

The C-to-D comparison probes the effect of `DerivedThermodynamicState` construction/validation and struct-output representation versus primitive output buffers.

This is an implementation experiment only. It does not change the semantic classification of Temperature or phase fraction.

### E. `local_compute_only_recovery`

The same equations are evaluated, but results are consumed through an accumulated checksum instead of being stored into output arrays.

The D-to-E comparison probes output-buffer traffic and storage effects.

### F. `state_traversal_only`

The benchmark reads `SpecificEnthalpy` from the same state buffer and accumulates a checksum without evaluating the recovery equations.

The E-to-F comparison provides a lower-level traversal reference for the arithmetic/branching work of the local recovery relation.

---

## 5. Semantic Gate

Before timing evidence is accepted:

- formal batch output shall match scalar public recovery;
- local Derived State output shall match scalar public recovery;
- local primitive output shall match scalar public recovery;
- local compute-only checksum shall match the checksum of scalar public recovery within the defined tolerance;
- exact `h_s*` and `h_l*` values shall appear in the deterministic dataset;
- no non-finite Temperature or phase-fraction value shall be accepted.

Tolerance:

```text
1e-10
```

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

This reduces sensitivity to very short individual traversals.

---

## 7. Timing Procedure

For each scenario and working-set size:

```text
warmup samples: 3
timed samples: 7
reported timing: median / minimum / maximum
```

The timed region uses `Stopwatch.GetTimestamp()` and shall report median managed allocation.

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
formal batch benefit      = scalar public / formal batch
formal-vs-local-derived   = formal batch / local derived
Derived-vs-primitive      = local derived / local primitive
primitive-vs-compute-only = local primitive / local compute-only
compute-vs-traversal      = local compute-only / traversal only
```

The names describe measured scenario ratios only. They shall not be converted into exact additive percentages of total runtime without stronger evidence.

---

## 9. Environment Metadata

The runner shall preserve:

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

The strongest supported conclusion from this experiment should be stated in terms such as:

```text
"The measured difference is primarily associated with ... under this implementation and environment."
```

It shall not claim that a source-level operation has a universal fixed cost.

In particular, the benchmark shall distinguish:

- formal API/boundary amortization;
- Derived State construction/validation and output representation;
- output-buffer memory traffic; and
- underlying piecewise arithmetic/traversal.

SIMD is intentionally excluded from this attribution track because PR #54 found its incremental benefit near parity and environment-sensitive.

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
