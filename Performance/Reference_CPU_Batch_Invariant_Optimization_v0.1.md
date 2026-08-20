# Reference CPU Batch Invariant Optimization v0.1

Status: Performance Evaluation Result — ACTIVE REFINEMENT — current candidate improvement observed  
Performance PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Purpose

This record measures implementation candidates for preserving `DerivedThermodynamicState` invariants while reducing repeated generic per-value validation inside `RecoverBatch(...)`.

The track follows PR #56, which attributed a dominant measured recovery/output cost layer to validated Derived State construction. It intentionally preserves failed or neutral intermediate candidates rather than rewriting history after later refinements.

---

## 2. Method

All executions use the same five comparison paths:

1. `formal_optimized_batch` — the current candidate `ReferenceThermodynamicFormulation.RecoverBatch(...)`;
2. `legacy_validated_batch_emulation` — local equivalent thermodynamic equations followed by the fully validating public `DerivedThermodynamicState` constructor for every result;
3. `local_specialized_trusted_batch` — local equivalent equations followed by specialized invariant establishment and internal invariant-established construction;
4. `validated_output_only` — public validating Derived State construction from precomputed valid values;
5. `trusted_output_only` — invariant-established construction from the same precomputed valid values.

For each execution:

```text
working sets: 1,024; 16,384; 262,144; 1,048,576 cells
warmup samples: 3
timed samples: 7
target cell operations/sample: 8,388,608
managed allocation: recorded
semantic tolerance: 1e-10
```

No post-hoc performance threshold is introduced.

---

## 3. Semantic Equivalence

Every accepted execution reported zero disagreement between the formal candidate and both comparison implementations for Temperature and liquid fraction over the 1,048,576-state deterministic mixed-phase gate.

```text
semantic equivalence gate: PASS
```

Repository Verification on the latest implementation head `d15785cbbe218b82da7a3a9d086d33f5197fe4a2` also completed:

```text
Reference verification: 21/21 passed
Batch invariant verification: 5/5 passed
```

The targeted cases include rejection of non-finite sensible-branch recovery under extreme finite inputs. The optimization therefore does not rely on silently accepting invalid Derived State.

---

## 4. Candidate A — Specialized Construction Without Hot-Path Refinement

Implementation snapshot:

```text
ca68063ed933faedd5cd75aede95e22a06af6eb4
```

Workflow run `32410024193`, job `96558030402`  
Environment: AMD EPYC 9V74, .NET 8.0.30, Ubuntu 24.04.4 LTS.

At 1,048,576 cells:

| Path | Median ms |
|---|---:|
| formal optimized batch | 40.800213 |
| legacy validated batch emulation | 24.938777 |
| local specialized trusted batch | 29.301431 |
| validated output only | 17.442052 |
| trusted output only | 8.993347 |

The isolated output pair showed that invariant-established construction itself was substantially cheaper (`~1.94x` for this run), but the formal batch hot path regressed badly. PR #57 was therefore returned to Draft.

Disposition at this stage:

```text
trusted construction mechanism: PROMISING
formal candidate: NOT PERFORMANCE-JUSTIFIED
```

---

## 5. Candidate B — Inlined Hot Helpers

The next refinement used inlining-oriented hot helpers and cold exception paths while preserving the same invariant checks.

Workflow run `32410447524`, job `96559370790`  
Environment: AMD EPYC 7763, .NET 8.0.30, Ubuntu 24.04.4 LTS.

At 1,048,576 cells:

| Path | Median ms |
|---|---:|
| formal optimized batch | 19.950984 |
| legacy validated batch emulation | 19.502506 |
| local specialized trusted batch | 21.479610 |
| validated output only | 13.722648 |
| trusted output only | 7.537442 |

At smaller working sets the formal candidate was faster than the legacy emulation, but at the largest working set it was approximately at parity (`legacy/formal ~= 0.978x`).

This removed the severe regression but did not yet justify a stable large-set speedup claim.

---

## 6. Candidate C — Cached Recovery Kernel

The latest implementation refinement caches immutable material recovery parameters once per batch in a local `RecoveryKernel` while retaining:

- the same piecewise thermodynamic relation;
- the same specialized invariant establishment;
- the same internal invariant-established Derived State construction;
- the fully validating public constructor for scalar recovery.

Implementation head:

```text
d15785cbbe218b82da7a3a9d086d33f5197fe4a2
```

Performance integration head:

```text
fc97a6e48541b6a4900c85acc48f339ec53d7872
```

Workflow run `32410784963`, job `96560451483`  
Environment: Intel Xeon Platinum 8370C, .NET 8.0.30, Ubuntu 24.04.4 LTS.

### 6.1 Measurement table

| Cells | Path | Median ms | ns/cell | Million cells/s | Median allocated bytes |
|---:|---|---:|---:|---:|---:|
| 1,024 | formal optimized batch | 27.728264 | 3.305467 | 302.53 | 0 |
| 1,024 | legacy validated batch emulation | 49.892326 | 5.947629 | 168.13 | 0 |
| 1,024 | local specialized trusted batch | 40.418473 | 4.818257 | 207.54 | 0 |
| 1,024 | validated output only | 17.004650 | 2.027112 | 493.31 | 0 |
| 1,024 | trusted output only | 4.943998 | 0.589370 | 1696.73 | 0 |
| 16,384 | formal optimized batch | 12.411993 | 1.479625 | 675.85 | 0 |
| 16,384 | legacy validated batch emulation | 24.538789 | 2.925252 | 341.85 | 0 |
| 16,384 | local specialized trusted batch | 17.220737 | 2.052872 | 487.12 | 0 |
| 16,384 | validated output only | 16.854899 | 2.009261 | 497.70 | 0 |
| 16,384 | trusted output only | 4.912727 | 0.585643 | 1707.53 | 0 |
| 262,144 | formal optimized batch | 17.189770 | 2.049180 | 488.00 | 0 |
| 262,144 | legacy validated batch emulation | 25.101044 | 2.992278 | 334.19 | 0 |
| 262,144 | local specialized trusted batch | 20.767742 | 2.475708 | 403.92 | 0 |
| 262,144 | validated output only | 16.977602 | 2.023888 | 494.10 | 0 |
| 262,144 | trusted output only | 8.265807 | 0.985361 | 1014.86 | 0 |
| 1,048,576 | formal optimized batch | 18.743900 | 2.234447 | 447.54 | 0 |
| 1,048,576 | legacy validated batch emulation | 22.433557 | 2.674288 | 373.93 | 0 |
| 1,048,576 | local specialized trusted batch | 21.028287 | 2.506767 | 398.92 | 0 |
| 1,048,576 | validated output only | 17.759964 | 2.117153 | 472.33 | 0 |
| 1,048,576 | trusted output only | 8.795623 | 1.048520 | 953.73 | 0 |

### 6.2 Current interpretation

At the largest working set:

```text
legacy validated batch: 22.433557 ms
formal cached-kernel:    18.743900 ms
same-run ratio:          ~1.197x
```

The formal candidate was also faster than the legacy emulation at all four measured sizes in this execution.

The output-only comparison again showed a substantial isolated validated/trusted difference (`~2.02x` at the largest set).

This is the first execution in this track where the formal invariant-preserving implementation produced a clear large-set same-run improvement over the legacy validated emulation. Because hosted-run absolute timing and JIT behavior have already varied materially across environments, one execution is not treated as sufficient for a stable speedup claim.

---

## 7. Evidence Evolution

The sequence is informative in its own right:

```text
Candidate A
trusted construction introduced
→ mechanism cheaper in isolation
→ formal hot path regressed

Candidate B
hot helpers made inlining-friendly
→ severe regression removed
→ large-set result near parity

Candidate C
immutable material parameters cached per batch
→ first clear same-run formal improvement at all sizes
```

This supports the interpretation that the original performance difference was not caused by a single validation instruction. Constructor validation, helper/JIT structure, and repeated material-property access/code shape all interact in the hot loop.

---

## 8. Current Disposition

```text
Invariant-established Derived construction:
SUPPORTED AS A PERFORMANCE MECHANISM

Cached invariant-preserving formal RecoverBatch candidate:
PROMISING — independent current-head confirmation required before promotion

Derived State invariants:
PRESERVED — no relaxation justified
```

This is an engineering evidence disposition, not a Framework Conformance or physical Validation category.

---

## 9. Boundary

This result does not justify:

- deleting Derived State invariants;
- exposing an unchecked public constructor;
- claiming a universal speedup factor;
- SIMD adoption;
- multi-thread CPU performance;
- GPU performance;
- physical Validation or Framework Conformance claims.

---

## 10. Result Status

```text
ACTIVE — cached-kernel improvement observed; confirmation pending
```

No performance PASS/FAIL threshold was defined before execution.

---

## 11. Specification Impact

```text
Framework Specification change: None
Reference Formulation change: None
Framework Freeze reopen: No
Performance acceptance threshold: None
```
