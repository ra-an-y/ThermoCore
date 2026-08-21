# Multi-thread CPU Batch-Recovery Scaling v0.1

## Status

`COMPLETED — environment-sensitive scaling measurements reported`

No performance PASS/FAIL threshold is defined.

Framework Conformance: Not claimed.  
Physical Validation: Not claimed.  
Scheduler requirement: Not introduced.

## Evaluated baseline

Performance harness source commit:

`943acd14b5f7dcccdc17e3b7e4c144448f9dbb01`

Dependency baseline:

`1470e8ec2c7246433db323cc9a630c8d529b1a29`

This study is stacked on PR #60 and modifies no `Framework/**` or `Materials/**` implementation source.

## Purpose

Measure scaling of the current formal, semantics-preserving `ReferenceThermodynamicFormulation.RecoverBatch(...)` path when one large independent batch is partitioned across persistent dedicated CPU worker threads.

The study distinguishes:

- direct caller performance;
- one-worker pool performance;
- worker-count scaling relative to the one-worker pool; and
- oversubscription behavior when requested workers exceed the environment-reported logical processor count.

## Final controlled method

Working sets:

- 262,144 cells
- 1,048,576 cells
- 4,194,304 cells

Requested worker counts:

- 1
- 2
- 4
- 8

Every timed sample targets at least 16,777,216 recovered cells.

Each working set uses five persistent scenarios:

- `direct_single_thread`
- `worker_pool_1`
- `worker_pool_2`
- `worker_pool_4`
- `worker_pool_8`

Worker pools are created before warmup and retained through the timed rounds. Worker thread creation is outside timing. Release/completion synchronization is inside timing.

To reduce run-order, CPU-frequency, and thermal bias, the final method uses:

```text
3 interleaved warmup rounds
7 interleaved timed rounds
rotating starting scenario per round
```

The primary worker-scaling metric is:

`speedup_vs_worker1(W) = median(worker_pool_1) / median(worker_pool_W)`

The direct caller remains a separate end-to-end baseline because direct-main-thread placement and worker-pool execution can differ.

## Semantic gates

Both final controlled executions reported:

```text
semantic_gate_workers_1_max_temperature_error: 0
semantic_gate_workers_1_max_liquid_fraction_error: 0
semantic_gate_workers_2_max_temperature_error: 0
semantic_gate_workers_2_max_liquid_fraction_error: 0
semantic_gate_workers_4_max_temperature_error: 0
semantic_gate_workers_4_max_liquid_fraction_error: 0
semantic_gate_workers_8_max_temperature_error: 0
semantic_gate_workers_8_max_liquid_fraction_error: 0
multithread_semantic_equivalence_gate: PASS
persistent_state_immutability_gate: PASS
partition_coverage_gate: PASS
```

Therefore the worker-count measurements compare scheduling/partitioning of the same formal thermodynamic recovery semantics.

---

## Controlled execution A — AMD EPYC 9V74

GitHub Actions run:

`32512564202`

Job:

`96866838366`

Environment:

```text
CPU: AMD EPYC 9V74 80-Core Processor
Environment.ProcessorCount: 2
Runtime: .NET 8.0.30
OS: Ubuntu 24.04.4 LTS
Architecture: X64
Server GC: false
```

The hosted job exposed two logical processors. Requested 4- and 8-worker cases are therefore oversubscribed observations, not 4-core or 8-core scaling tests.

### 262,144 cells

| Path | Median ms | Throughput M cells/s | Speedup vs worker 1 |
| --- | ---: | ---: | ---: |
| direct single thread | 40.919114 | 410.009 | — |
| worker 1 | 40.810973 | 411.096 | 1.000x |
| worker 2 | 41.584104 | 403.453 | 0.981x |
| worker 4 | 41.485517 | 404.411 | 0.984x |
| worker 8 | 41.384777 | 405.396 | 0.986x |

### 1,048,576 cells

| Path | Median ms | Throughput M cells/s | Speedup vs worker 1 |
| --- | ---: | ---: | ---: |
| direct single thread | 42.319944 | 396.438 | — |
| worker 1 | 42.299684 | 396.627 | 1.000x |
| worker 2 | 42.594518 | 393.882 | 0.993x |
| worker 4 | 42.717696 | 392.746 | 0.990x |
| worker 8 | 42.895184 | 391.121 | 0.986x |

### 4,194,304 cells

| Path | Median ms | Throughput M cells/s | Speedup vs worker 1 |
| --- | ---: | ---: | ---: |
| direct single thread | 42.605082 | 393.784 | — |
| worker 1 | 42.744179 | 392.503 | 1.000x |
| worker 2 | 43.168169 | 388.648 | 0.990x |
| worker 4 | 43.403320 | 386.542 | 0.985x |
| worker 8 | 43.462017 | 386.020 | 0.983x |

### AMD observation

Across all three working sets, increasing the worker count did not improve throughput relative to the one-worker pool. Two workers were near parity but slightly slower in the median, and oversubscribed 4/8-worker execution also remained near parity or regressed slightly.

For this hosted environment, useful scaling saturated before any measured multi-worker gain appeared.

---

## Controlled execution B — Intel Xeon 6973P-C

Same workflow re-run without source changes.

GitHub Actions run:

`32512564202`

Job:

`96867044459`

Environment:

```text
CPU: Intel Xeon 6973P-C
Environment.ProcessorCount: 2
Runtime: .NET 8.0.30
OS: Ubuntu 24.04.4 LTS
Architecture: X64
Server GC: false
```

This job also exposed two logical processors. Requested 4- and 8-worker cases are oversubscribed observations.

### 262,144 cells

| Path | Median ms | Throughput M cells/s | Speedup vs worker 1 |
| --- | ---: | ---: | ---: |
| direct single thread | 33.197506 | 505.376 | — |
| worker 1 | 32.944537 | 509.256 | 1.000x |
| worker 2 | 32.673331 | 513.483 | 1.008x |
| worker 4 | 33.167461 | 505.834 | 0.993x |
| worker 8 | 33.030570 | 507.930 | 0.997x |

### 1,048,576 cells

| Path | Median ms | Throughput M cells/s | Speedup vs worker 1 |
| --- | ---: | ---: | ---: |
| direct single thread | 35.644512 | 470.682 | — |
| worker 1 | 35.256037 | 475.868 | 1.000x |
| worker 2 | 33.659199 | 498.444 | 1.047x |
| worker 4 | 33.719030 | 497.559 | 1.046x |
| worker 8 | 33.620940 | 499.011 | 1.049x |

### 4,194,304 cells

| Path | Median ms | Throughput M cells/s | Speedup vs worker 1 |
| --- | ---: | ---: | ---: |
| direct single thread | 51.443770 | 326.127 | — |
| worker 1 | 52.068784 | 322.213 | 1.000x |
| worker 2 | 36.373266 | 461.251 | 1.432x |
| worker 4 | 37.567603 | 446.587 | 1.386x |
| worker 8 | 36.947713 | 454.080 | 1.409x |

### Intel observation

The smallest working set showed essentially no meaningful worker-count scaling. At 1,048,576 cells, two workers improved median throughput by about 4.7% relative to one worker. At 4,194,304 cells, two workers improved median throughput by about 43.2%.

The 4- and 8-worker cases did not improve beyond the two-worker result for the large working set. Because the environment exposed only two logical processors, this is consistent with saturation at the available reported processor count; it is not evidence about 4-core or 8-core scaling.

---

## Cross-environment interpretation

The final controlled evidence does **not** support a universal multi-thread speedup factor.

```text
AMD EPYC 9V74, 2 logical processors:
2-worker scaling: near parity / slight regression

Intel Xeon 6973P-C, 2 logical processors:
262k: near parity
1M:   ~1.05x vs worker 1
4M:   ~1.43x vs worker 1

4/8 requested workers on both jobs:
oversubscribed; no evidence of additional-core scaling
```

The strongest supported conclusion is therefore:

> Multi-thread batch recovery is semantically safe under disjoint partitioning, but its performance benefit is environment- and working-set-sensitive. The tested Intel hosted runner benefited materially from two workers only at the largest working set, while the tested AMD hosted runner showed no multi-worker throughput gain.

This result argues against embedding a universal default worker count into the Framework or reference formulation API.

A runtime/backend that chooses to parallelize large batches should benchmark or otherwise select an execution policy appropriate to its environment rather than assuming that more workers are faster.

## Direct caller versus one-worker pool

In both final interleaved runs, direct caller and one-worker pool timings were close for the reviewed working sets. This validates the use of `worker_pool_1` as the primary scaling bridge without treating thread placement itself as parallel speedup.

## Methodological history

Before the final interleaved method, predecessor executions used scenario-by-scenario timing. On the AMD runner, the 262,144-cell direct caller appeared almost 2x slower than the later one-worker scenario while the larger working sets were near parity.

That inconsistent first-scenario behavior was treated as a run-order/frequency confound rather than a performance result. The harness was revised so all scenarios are warmed and timed in interleaved rotating order.

Final controlled runs no longer showed the anomalous direct-versus-worker-1 separation at 262,144 cells. The predecessor measurements remain historical methodological evidence and are not used for the final scaling conclusion.

## Limitations

- Both reviewed hosted jobs exposed only two logical processors.
- The physical relationship between those exposed logical processors is not established by this benchmark.
- CPU quota, topology, cache hierarchy, SMT placement, memory bandwidth, frequency policy, and other virtualization details may differ between hosted-runner environments.
- 4/8-worker measurements are oversubscription tests, not 4-core/8-core scaling.
- The benchmark evaluates one synthetic mixed-phase material profile and independent cell recovery only.
- It does not evaluate conduction, transport, synchronization between physical subsystems, GPU execution, Unity, engine integration, mobile hardware, NUMA, or production deployment.

## Engineering disposition

```text
Semantic correctness under disjoint multi-thread partitioning:
SUPPORTED

Universal multi-thread speedup:
NOT SUPPORTED

Two-worker benefit:
ENVIRONMENT / WORKING-SET DEPENDENT

4/8-worker additional-core scaling:
NOT TESTED — hosted environments exposed only 2 logical processors

Automatic Framework-level thread policy:
NOT JUSTIFIED
```

No implementation change is promoted by this result.

## Specification impact

- Framework Specification change: None
- Reference Formulation change: None
- Framework Freeze reopen: No
- Runtime State ownership change: None
- Derived State invariant relaxation: None
- Scheduler requirement introduced: None
- New Framework component / owner: None
