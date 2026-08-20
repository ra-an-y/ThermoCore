# Reference CPU Batch Invariant Optimization Plan v0.1

Status: Performance Evaluation Plan — Non-Normative

---

## 1. Purpose

This evaluation measures whether the invariant-preserving batch construction introduced by the stacked implementation PR #57 reduces CPU recovery cost without weakening `DerivedThermodynamicState` invariants.

It follows the attribution result from PR #56, which identified validated per-value Derived State construction as the dominant measured cost layer relative to an otherwise similar raw two-double output.

---

## 2. Baseline and Dependency

Implementation candidate baseline:

```text
ca68063ed933faedd5cd75aede95e22a06af6eb4
```

This is the current head of stacked PR #57.

PR #57 itself depends on unmerged attribution PR #56. This evaluation is therefore also stacked and shall not be merged ahead of those dependencies.

---

## 3. Compared Paths

All paths operate on the same prevalidated `ThermodynamicState[]`, the same compiled Material Configuration, and the same deterministic mixed-phase state pattern.

1. `formal_optimized_batch` — calls the candidate `ReferenceThermodynamicFormulation.RecoverBatch(...)`.
2. `legacy_validated_batch_emulation` — evaluates the same piecewise equations locally and constructs every result through the fully validating public `DerivedThermodynamicState` constructor, emulating the pre-optimization batch cost structure.
3. `local_specialized_trusted_batch` — evaluates the same equations locally, performs the same region-specific invariant establishment as the candidate, then uses the internal invariant-established construction path.
4. `validated_output_only` — repeatedly constructs valid Derived State values through the public validating constructor using precomputed valid values.
5. `trusted_output_only` — repeatedly constructs the same precomputed valid values through the internal invariant-established path.

The local paths are measurement devices only. They do not define Framework behavior.

---

## 4. Semantic Gate

Before timing is accepted, `formal_optimized_batch`, `legacy_validated_batch_emulation`, and `local_specialized_trusted_batch` shall be compared over 1,048,576 deterministic states.

Maximum absolute disagreement shall be reported for:

- Temperature;
- liquid phase fraction.

Tolerance:

```text
1e-10
```

Any candidate exceeding the tolerance makes the run `INVALID`.

---

## 5. Timing Procedure

Working-set sizes:

```text
1,024
16,384
262,144
1,048,576 cells
```

Per scenario and working set:

```text
warmup samples: 3
timed samples: 7
target cell operations per sample: 8,388,608
reported statistic: median, minimum, maximum
```

Managed allocation inside the timed region is also recorded.

---

## 6. Interpretation Rules

The main practical comparison is:

```text
legacy_validated_batch_emulation
        versus
formal_optimized_batch
```

The local specialized path is used to determine whether remaining formal-API structure materially changes the result.

The output-only pair isolates the cost difference between generic public constructor validation and invariant-established construction for already-valid values.

Differences are differential observations, not additive source-line costs. JIT, inlining, cache behavior, branch prediction, and memory traffic may interact.

No post-hoc performance PASS threshold will be created.

---

## 7. Boundary

This evaluation does not establish:

- a universal speedup factor;
- multi-thread CPU performance;
- SIMD promotion;
- GPU performance;
- physical Validation;
- Framework Conformance.

---

## 8. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
