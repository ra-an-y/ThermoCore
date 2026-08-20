# Reference CPU Batch Invariant Optimization v0.1

Status: Performance Evaluation Result — INITIAL CANDIDATE REGRESSION OBSERVED  
Performance PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Purpose

This record measures the first implementation candidate for preserving `DerivedThermodynamicState` invariants while reducing repeated generic per-value validation inside `RecoverBatch(...)`.

The candidate was introduced by stacked PR #57 after PR #56 attributed a dominant measured cost layer to validated Derived State construction.

---

## 2. Executed Snapshot

Candidate implementation head at first execution:

```text
ca68063ed933faedd5cd75aede95e22a06af6eb4
```

Performance branch snapshot:

```text
6c93b4dad2ced349e308237483d95966c8c8262d
```

Workflow:

```text
Reference CPU Batch Invariant Optimization
run: 32410024193
job: 96558030402
conclusion: success
```

---

## 3. Environment

```text
runtime: .NET 8.0.30
os: Ubuntu 24.04.4 LTS
architecture: X64
logical processors visible: 2
GC mode: workstation
CPU: AMD EPYC 9V74 80-Core Processor
stopwatch frequency: 1,000,000,000 Hz
```

These values identify this hosted execution only.

---

## 4. Semantic Equivalence Gate

The 1,048,576-state gate reported:

```text
legacy max Temperature error:          0
legacy max liquid-fraction error:      0
local trusted max Temperature error:   0
local trusted max liquid-fraction error: 0

tolerance: 1e-10
semantic equivalence gate: PASS
```

The observed performance problem is therefore not a numerical-equivalence failure.

---

## 5. First Measurement Table

| Cells | Path | Median ms | ns/cell | Million cells/s | Median allocated bytes |
|---:|---|---:|---:|---:|---:|
| 1,024 | formal optimized batch | 35.731731 | 4.259554 | 234.77 | 0 |
| 1,024 | legacy validated batch emulation | 22.633070 | 2.698072 | 370.64 | 0 |
| 1,024 | local specialized trusted batch | 17.852304 | 2.128160 | 469.89 | 0 |
| 1,024 | validated output only | 17.224289 | 2.053295 | 487.02 | 0 |
| 1,024 | trusted output only | 5.996470 | 0.714835 | 1398.92 | 0 |
| 16,384 | formal optimized batch | 32.677370 | 3.895446 | 256.71 | 0 |
| 16,384 | legacy validated batch emulation | 22.258957 | 2.653474 | 376.86 | 0 |
| 16,384 | local specialized trusted batch | 18.081341 | 2.155464 | 463.94 | 0 |
| 16,384 | validated output only | 17.099140 | 2.038376 | 490.59 | 0 |
| 16,384 | trusted output only | 5.925223 | 0.706342 | 1415.75 | 0 |
| 262,144 | formal optimized batch | 41.765261 | 4.978807 | 200.85 | 0 |
| 262,144 | legacy validated batch emulation | 24.523298 | 2.923405 | 342.07 | 0 |
| 262,144 | local specialized trusted batch | 19.806336 | 2.361099 | 423.53 | 0 |
| 262,144 | validated output only | 17.259129 | 2.057449 | 486.04 | 0 |
| 262,144 | trusted output only | 6.082407 | 0.725079 | 1379.16 | 0 |
| 1,048,576 | formal optimized batch | 40.800213 | 4.863764 | 205.60 | 0 |
| 1,048,576 | legacy validated batch emulation | 24.938777 | 2.972934 | 336.37 | 0 |
| 1,048,576 | local specialized trusted batch | 29.301431 | 3.493003 | 286.29 | 0 |
| 1,048,576 | validated output only | 17.442052 | 2.079255 | 480.94 | 0 |
| 1,048,576 | trusted output only | 8.993347 | 1.072091 | 932.76 | 0 |

---

## 6. Interpretation

The initial candidate produces two distinct observations.

First, the invariant-established construction mechanism itself is measurably cheaper for already-valid outputs. At the largest working set:

```text
validated output only: 17.442052 ms
trusted output only:     8.993347 ms
ratio: approximately 1.94x
```

Second, the formal `RecoverBatch(...)` hot path is substantially slower than the legacy validated batch emulation in this candidate:

```text
formal optimized batch:           40.800213 ms
legacy validated batch emulation: 24.938777 ms
```

Therefore the constructor-level idea is not disproven, but the current formal implementation shape does not realize the expected gain.

The local specialized-trusted path being materially faster than the formal path suggests that hot-loop structure, helper-call/inlining behavior, switch/region handling, Span access, or related JIT/code-generation effects are plausible contributors. This record does not assign a single causal percentage to any one of those factors.

---

## 7. Disposition

```text
Invariant-established Derived construction:
PROMISING MECHANISM — measurable isolated cost reduction

Current formal RecoverBatch candidate:
NOT PERFORMANCE-JUSTIFIED — regression observed

Next action:
REFINE HOT PATH WITHOUT REMOVING INVARIANTS
```

This is an engineering evidence disposition, not a Framework conformance or physical Validation category.

PR #57 was returned to Draft after this result.

---

## 8. Boundary

This result does not justify:

- deleting Derived State validation invariants;
- promoting an unvalidated public construction API;
- SIMD adoption;
- a universal speedup factor;
- multi-thread or GPU claims.

---

## 9. Result Status

```text
COMPLETED — initial candidate regression observed and preserved
```

No performance PASS/FAIL threshold was defined before execution.

---

## 10. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
