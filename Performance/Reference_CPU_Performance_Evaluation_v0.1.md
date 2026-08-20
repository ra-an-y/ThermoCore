# Reference CPU Performance Evaluation v0.1

Status: Performance Result — COMPLETED — measurements reported  
Performance PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Purpose

This record preserves the first executed CPU Performance Evaluation for the bounded C# ThermoCore reference implementation.

The evaluation is limited to three scalar, single-threaded per-cell paths:

- persistent specific-enthalpy state update;
- `h -> Temperature / liquid fraction` recovery; and
- update followed by recovery.

It does not measure a conduction solver, transport, GPU execution, Unity, Unreal, rendering, mobile hardware, or an application frame pipeline.

---

## 2. Evaluated Version

The implementation baseline evaluated by this track is:

```text
Repository baseline: 11be054721ca3334932e7db20b7fbb53aed894ea
Reference formulation: Documentation/Thermodynamic_Formulation.md
Implementation profile: bounded constant-positive-Cp C# reference implementation
```

No `Framework/` or `Materials/` implementation source is modified by this Performance Evaluation branch.

The corrected benchmark harness used for the primary measurement is:

```text
Performance branch: performance/reference-cpu-scaling-v0.1
Benchmark harness commit: 8a406e0d14ee795358ae5b6bb524f0ca74148838
Workflow: Reference CPU Performance Evaluation
Primary workflow run: 32401521767
Primary job: 96530577236
Workflow conclusion: success
```

---

## 3. Benchmark Environment

The primary workflow reported:

```text
runtime: .NET 8.0.30
os: Ubuntu 24.04.4 LTS
architecture: X64
logical processors visible: 2
server GC: False
CPU model: AMD EPYC 7763 64-Core Processor
Stopwatch frequency: 1,000,000,000 Hz
```

The benchmark loops themselves are single-threaded. The two visible logical processors therefore do not mean that the measured per-cell loops used two-way parallelism.

GitHub-hosted runner measurements are environment-specific and can vary across runs even when the reported CPU model is the same.

---

## 4. Benchmark Procedure

For each scenario and cell count, the harness:

1. allocates and populates the state array before timing;
2. executes two untimed warmup samples;
3. executes five timed samples;
4. processes at least `1,048,576` cell operations per timed sample by repeating smaller arrays;
5. reports median, minimum, and maximum elapsed time;
6. derives median nanoseconds per cell and million cells per second; and
7. observes current-thread managed allocations during the timed region.

The tested cell counts are:

```text
1,024
16,384
262,144
1,048,576
```

A fixed synthetic material configuration is used only to keep the recovery branch semantics stable. It is not a physical Validation material.

---

## 5. Primary Measurements

Primary run: `32401521767`.

### 5.1 State update

| Cells | Passes | Median ms | Min ms | Max ms | ns/cell | Million cells/s | Median allocated bytes |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1,024 | 1,024 | 2.0072 | 1.9955 | 2.0252 | 1.9142 | 522.41 | 0 |
| 16,384 | 64 | 2.0046 | 1.4906 | 2.0281 | 1.9117 | 523.08 | 0 |
| 262,144 | 4 | 2.5061 | 1.9953 | 2.7046 | 2.3900 | 418.41 | 0 |
| 1,048,576 | 1 | 1.9795 | 1.9008 | 1.9964 | 1.8878 | 529.72 | 0 |

### 5.2 State recovery

| Cells | Passes | Median ms | Min ms | Max ms | ns/cell | Million cells/s | Median allocated bytes |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1,024 | 1,024 | 5.0299 | 4.9721 | 5.0822 | 4.7969 | 208.47 | 0 |
| 16,384 | 64 | 4.9393 | 4.9191 | 4.9782 | 4.7105 | 212.29 | 0 |
| 262,144 | 4 | 4.9543 | 4.8973 | 5.0634 | 4.7248 | 211.65 | 0 |
| 1,048,576 | 1 | 5.1606 | 5.0978 | 5.2941 | 4.9215 | 203.19 | 0 |

### 5.3 Update plus recovery

| Cells | Passes | Median ms | Min ms | Max ms | ns/cell | Million cells/s | Median allocated bytes |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1,024 | 1,024 | 6.1727 | 6.1297 | 6.8695 | 5.8867 | 169.87 | 0 |
| 16,384 | 64 | 7.1690 | 6.2274 | 11.4689 | 6.8369 | 146.27 | 0 |
| 262,144 | 4 | 6.7771 | 6.6892 | 6.8309 | 6.4631 | 154.72 | 0 |
| 1,048,576 | 1 | 7.5816 | 6.5740 | 15.5227 | 7.2304 | 138.31 | 0 |

---

## 6. Observations

The primary run shows that all three scalar paths remained in the low-single-digit nanoseconds-per-cell range on the identified hosted runner.

For the `1,048,576`-cell single-pass case, the observed median times were:

```text
state update:            1.9795 ms
state recovery:          5.1606 ms
update plus recovery:    7.5816 ms
```

The corresponding observed median throughputs were approximately:

```text
state update:            529.72 million cells/s
state recovery:          203.19 million cells/s
update plus recovery:    138.31 million cells/s
```

State recovery is measurably more expensive than the minimal enthalpy update, which is expected because recovery evaluates branch-dependent thermodynamic relations and constructs the derived state.

The recovery scenario is comparatively stable across the four working-set sizes in this run. The combined scenario shows substantially larger max-sample excursions at some sizes, including a `15.5227 ms` maximum for the one-million-cell case, demonstrating that hosted-runner scheduling noise can be material even when the median remains much lower.

The corrected harness observed `0` median current-thread managed bytes allocated inside every timed scenario. This does not mean the overall process allocates no memory; the state arrays, result arrays, startup, JIT, and framework activity occur outside or around the timed loop.

---

## 7. Preliminary Run and Harness Correction

The first workflow execution is preserved as historical evidence:

```text
Workflow run: 32401358312
Job: 96530036311
Result: execution success, but allocation metric contaminated by timing harness
```

That initial harness used `Stopwatch.StartNew()` inside the allocation-measurement interval. The `Stopwatch` object itself contributed `40` managed bytes to every timed sample, so those allocation values did not represent the thermodynamic loop.

The benchmark was corrected in commit:

```text
8a406e0d14ee795358ae5b6bb524f0ca74148838
```

by switching to static timestamp measurement with `Stopwatch.GetTimestamp()` / `Stopwatch.GetElapsedTime()`.

No ThermoCore Framework, Material Definition, or thermodynamic implementation source changed. The corrected run then reported zero median managed bytes in the timed region.

The preliminary and corrected runs also produced materially different absolute timing levels despite reporting the same CPU model. This reinforces the plan boundary that hosted-runner absolute timings are observations, not universal hardware guarantees.

---

## 8. Performance Conclusion

Result status:

```text
COMPLETED — measurements reported
```

The current bounded scalar C# implementation can now be characterized quantitatively on the identified GitHub-hosted runner for update, recovery, and combined update/recovery operations across four state-array sizes.

No performance acceptance requirement was defined before execution. Therefore this result does **not** convert the measurements into a performance `PASS` claim and does not establish real-time suitability for any particular application.

The result is useful as a CPU reference baseline for later comparisons such as:

- optimized CPU layouts or batch kernels;
- SIMD/vectorized implementations;
- parallel CPU execution;
- GPU backends; or
- application-specific workloads.

Any such later comparison shall preserve its own implementation commit, hardware/environment information, and benchmark procedure rather than silently replacing this baseline.

---

## 9. Known Limitations

- GitHub-hosted runners are shared virtualized/cloud execution environments.
- Only one primary corrected measurement run is used for the tables above.
- The benchmark loops are single-threaded.
- The workload evaluates local per-cell thermodynamic operations, not neighbor-coupled conduction or transport.
- State arrays are allocated before timing; allocation/setup cost is excluded.
- JIT startup cost is excluded by warmup.
- The material configuration is synthetic and exists only to stabilize code paths.
- No CPU affinity, fixed clock, thermal state, or exclusive machine reservation is available.
- No GPU or engine backend is measured.
- No application-defined frame-time or throughput requirement has been adopted.

---

## 10. Evidence Integrity

This result is tied to the identified implementation baseline, benchmark harness commit, workflow run, and hosted-runner environment.

A materially different implementation, runtime, hardware class, benchmark scenario, concurrency model, or acceptance criterion shall be preserved as a new or versioned Performance Evaluation result.
