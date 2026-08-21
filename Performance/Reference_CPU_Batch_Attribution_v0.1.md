# Reference CPU Batch Attribution v0.1

Status: Performance Attribution Result — COMPLETED — attribution measurements reported  
Performance PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Purpose

This record identifies which implementation layers are most strongly associated with the CPU recovery-cost difference observed between the scalar reference API and simplified batch experiments.

The evaluated baseline already contains the semantics-preserving `ReferenceThermodynamicFormulation.RecoverBatch(...)` API. The attribution harness does not modify Framework or Material implementation source.

---

## 2. Baseline and Executed Snapshot

Repository baseline:

```text
b3f9fdda82cdaf78ec6612b4af442c5f5d4775c1
```

This is `main` after PR #55.

Primary refined attribution execution:

```text
branch: performance/reference-batch-attribution-v0.1
candidate head: af918decb08dbc8e828b585e9b5cb37620c3b3a9
workflow: Reference CPU Batch Attribution
workflow run: 32408554810
job: 96553339347
conclusion: success
```

---

## 3. Environment

The primary refined run reported:

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

The scalar public path, formal batch path, local validated-Derived path, local raw-struct path, and local primitive-output path were compared over 1,048,576 deterministic mixed-phase states containing exact transition boundaries.

Observed maximum disagreement:

```text
equivalence tolerance: 1e-10

formal batch Temperature error:       0
formal batch liquid-fraction error:   0
local Derived Temperature error:      0
local Derived liquid-fraction error:  0
local raw-struct Temperature error:   0
local raw-struct fraction error:      0
local primitive Temperature error:    0
local primitive fraction error:       0

semantic equivalence gate: PASS
```

The raw struct and primitive arrays are Performance-only measurement devices. Numerical equivalence does not give them Framework authority or change the classification of Derived State.

---

## 5. Timing Procedure

For each working-set size:

```text
warmup samples: 3
timed samples: 7
target cell operations per sample: 8,388,608
reported timing: median / minimum / maximum
```

All timed scenarios reported median managed allocation of `0` bytes.

Checksums were calculated after the timed region.

---

## 6. Primary Measurements

| Cells | Scenario | Median ms | ns/cell | Million cells/s |
|---:|---|---:|---:|---:|
| 1,024 | scalar public recovery | 42.776125 | 5.099311 | 196.10 |
| 1,024 | formal batch recovery | 40.070675 | 4.776797 | 209.35 |
| 1,024 | local Derived recovery | 44.005220 | 5.245831 | 190.63 |
| 1,024 | local raw-struct recovery | 22.672376 | 2.702758 | 369.99 |
| 1,024 | local primitive recovery | 31.496948 | 3.754729 | 266.33 |
| 1,024 | Derived output only | 17.662022 | 2.105477 | 474.95 |
| 1,024 | raw-struct output only | 5.988060 | 0.713832 | 1400.89 |
| 1,024 | primitive output only | 6.010204 | 0.716472 | 1395.73 |
| 16,384 | scalar public recovery | 23.688328 | 2.823869 | 354.12 |
| 16,384 | formal batch recovery | 21.715943 | 2.588742 | 386.29 |
| 16,384 | local Derived recovery | 25.164705 | 2.999867 | 333.35 |
| 16,384 | local raw-struct recovery | 15.318654 | 1.826126 | 547.61 |
| 16,384 | local primitive recovery | 15.103945 | 1.800531 | 555.39 |
| 16,384 | Derived output only | 17.062910 | 2.034057 | 491.63 |
| 16,384 | raw-struct output only | 5.921962 | 0.705953 | 1416.53 |
| 16,384 | primitive output only | 6.120998 | 0.729680 | 1370.46 |
| 262,144 | scalar public recovery | 24.168553 | 2.881116 | 347.09 |
| 262,144 | formal batch recovery | 22.986755 | 2.740235 | 364.93 |
| 262,144 | local Derived recovery | 25.369210 | 3.024246 | 330.66 |
| 262,144 | local raw-struct recovery | 14.656759 | 1.747222 | 572.34 |
| 262,144 | local primitive recovery | 14.896666 | 1.775821 | 563.12 |
| 262,144 | Derived output only | 17.252982 | 2.056716 | 486.21 |
| 262,144 | raw-struct output only | 5.962633 | 0.710801 | 1406.86 |
| 262,144 | primitive output only | 6.186095 | 0.737440 | 1356.04 |
| 1,048,576 | scalar public recovery | 24.960131 | 2.975479 | 336.08 |
| 1,048,576 | formal batch recovery | 23.525226 | 2.804425 | 356.58 |
| 1,048,576 | local Derived recovery | 25.553184 | 3.046177 | 328.28 |
| 1,048,576 | local raw-struct recovery | 14.696268 | 1.751932 | 570.80 |
| 1,048,576 | local primitive recovery | 14.988573 | 1.786777 | 559.67 |
| 1,048,576 | Derived output only | 17.552681 | 2.092443 | 477.91 |
| 1,048,576 | raw-struct output only | 7.052167 | 0.840684 | 1189.51 |
| 1,048,576 | primitive output only | 6.639943 | 0.791543 | 1263.36 |

---

## 7. Differential Ratios

| Cells | Scalar / formal batch | Local Derived / raw struct | Raw struct / primitive | Derived-output / raw-output | Raw-output / primitive-output |
|---:|---:|---:|---:|---:|---:|
| 1,024 | 1.068× | 1.941× | 0.720× | 2.950× | 0.996× |
| 16,384 | 1.091× | 1.643× | 1.014× | 2.881× | 0.967× |
| 262,144 | 1.051× | 1.731× | 0.984× | 2.894× | 0.964× |
| 1,048,576 | 1.061× | 1.739× | 0.980× | 2.489× | 1.062× |

The smallest full-recovery raw-struct versus primitive result is anomalous relative to the other three working sets and the output-only pair. It is preserved rather than filtered.

---

## 8. What the Formal Batch API Actually Changes

At 1,048,576 cells:

```text
scalar public recovery: 24.960131 ms
formal batch recovery:  23.525226 ms
same-run ratio:         1.061×
```

Across all four sizes the scalar/formal-batch ratio ranged from approximately `1.05×` to `1.09×`.

Therefore the integrated batch boundary itself provides a measurable but modest improvement in this run. The previously observed approximately `1.7×` to `3×` experimental batch ratios cannot be attributed primarily to moving the material guard or loop boundary into `RecoverBatch(...)`.

---

## 9. Dominant Measured Difference: Derived State Validation

The strongest and most repeatable separation appears when the same local thermodynamic equations and the same array-of-structs output shape are used, but the real `DerivedThermodynamicState` constructor is replaced by a benchmark-local two-double struct without validation.

At 1,048,576 cells:

```text
local Derived recovery:    25.553184 ms
local raw-struct recovery: 14.696268 ms
ratio:                     1.739×
```

The same comparison ranged from approximately `1.64×` to `1.94×` across all sizes.

The output-only micro-kernel makes the signal even clearer. It removes the thermodynamic equations entirely and copies the same precomputed valid values:

```text
1,048,576 cells
Derived output only:   17.552681 ms
raw-struct output only: 7.052167 ms
ratio:                  2.489×
```

Across all sizes the Derived-output / raw-output ratio ranged from approximately `2.49×` to `2.95×`.

The primary measured cost difference is therefore strongly associated with the per-value validation path executed by `DerivedThermodynamicState` construction under this implementation and runtime.

This experiment does **not** isolate which individual condition inside that constructor contributes how much. It identifies the validated constructor path as the dominant layer relative to an otherwise similar raw two-double struct.

---

## 10. Output Layout Is Not the Main Difference

When validation is removed, the benchmark-local raw struct and the two primitive output arrays are near parity for the three larger working sets.

At 1,048,576 cells:

```text
local raw-struct recovery: 14.696268 ms
local primitive recovery:  14.988573 ms
ratio raw/primitive:       0.980×
```

The independent output-only comparison is also close:

```text
raw-struct output only:   7.052167 ms
primitive output only:    6.639943 ms
ratio raw/primitive:      1.062×
```

For 16,384 and 262,144 cells both full-recovery and output-only layout ratios were likewise close to `1.0×`.

Therefore the evidence does not support array-of-structs versus split primitive arrays as the primary source of the large experimental speed difference.

---

## 11. Attribution Conclusion

The refined evidence supports the following bounded explanation:

```text
Formal batch-boundary amortization:
REAL BUT MODEST in this run (~1.05× to ~1.09×)

DerivedThermodynamicState validated construction:
DOMINANT MEASURED COST LAYER in the tested recovery/output path

Struct versus split primitive output layout:
NEAR PARITY for the larger working sets; not the dominant explanation
```

In practical terms, the earlier primitive batch experiment was much faster mainly because it bypassed per-cell construction of the validated `DerivedThermodynamicState`, not because the thermodynamic equations themselves became dramatically cheaper and not primarily because it changed from struct output to two primitive arrays.

This is an implementation-specific performance attribution, not a Framework semantic conclusion.

---

## 12. Engineering Consequence

The next optimization question is now narrower:

```text
Can a batch recovery path preserve the existing Derived State invariants
while amortizing validation safely at the batch boundary?
```

Any such implementation must continue to guarantee finite Temperature and liquid fraction within `[0,1]`. Performance evidence does not justify simply deleting those invariants.

A candidate optimization should therefore be designed as invariant-preserving batch validation / trusted construction, followed by repository Verification and a fresh Performance Evaluation.

---

## 13. Excluded Claims

This result does not establish:

- a universal cost for `double.IsNaN`, `double.IsInfinity`, or range checks individually;
- multi-thread CPU performance;
- SIMD performance;
- GPU performance;
- physical Validation;
- Framework Conformance; or
- application frame-time suitability.

---

## 14. Result Status

```text
COMPLETED — attribution measurements reported
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
