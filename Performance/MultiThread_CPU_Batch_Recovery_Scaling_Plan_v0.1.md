# Multi-thread CPU Batch-Recovery Scaling Plan v0.1

## Status

Performance Scaling Plan — no implementation promotion decision yet.

## Purpose

The preceding CPU attribution work narrowed the dominant single-thread recovery/output costs and supported keeping the semantics-preserving public Derived State construction path. The next question is whether the current formal `RecoverBatch(...)` workload scales usefully across multiple CPU worker threads before oversubscription or orchestration overhead dominates.

This is a Performance Evaluation artifact. It is not Verification, physical Validation, Framework Conformance, or a Framework Specification.

## Dependency and evaluated baseline

This study is stacked on PR #60 (`Performance: attribute residual Derived validation cost v0.1`).

Evaluated code baseline before this study:

`1470e8ec2c7246433db323cc9a630c8d529b1a29`

The scaling harness does not modify `Framework/**` or `Materials/**` implementation source. It calls the current formal `ReferenceThermodynamicFormulation.RecoverBatch(...)` over disjoint contiguous slices.

## Question

For large independent Thermodynamic State batches, how does the current semantics-preserving CPU reference recovery path scale as requested worker count increases, and where does useful scaling saturate on the tested hosted-runner environment?

## Compared execution paths

For each working set the harness measures:

1. `direct_single_thread`
   - one caller invokes formal `RecoverBatch(...)` over the entire array;
   - no worker-pool synchronization.

2. `worker_pool_1`
   - one persistent dedicated worker owns the entire array slice;
   - includes the benchmark worker-pool start/completion synchronization cost.

3. `worker_pool_2`
   - two persistent dedicated workers process two disjoint contiguous slices.

4. `worker_pool_4`
   - four persistent dedicated workers process four disjoint contiguous slices.

5. `worker_pool_8`
   - eight persistent dedicated workers process eight disjoint contiguous slices.

Worker threads are created before warmup and reused across all timed samples. Thread creation is outside the timed interval. The timed interval includes synchronization needed to release workers and wait for completion because coordination is part of observable multi-thread batch cost.

The requested worker count is not treated as a CPU-core count. Each result records `Environment.ProcessorCount`; requested counts above that value are explicitly oversubscribed observations rather than additional-core scaling evidence.

## Work partitioning

For worker count `W` and `N` cells:

- cells are partitioned into `W` disjoint contiguous slices;
- slice sizes differ by at most one cell;
- every input cell is read exactly once per pass;
- every destination cell is written by exactly one worker;
- workers share the same immutable compiled Material Configuration;
- no worker mutates persistent Thermodynamic State.

The study changes execution scheduling only. It does not change thermodynamic equations, Runtime State ownership, Derived State semantics, or Material Configuration semantics.

## Working sets

Primary working sets:

- 262,144 cells
- 1,048,576 cells
- 4,194,304 cells

For each timed sample, the harness repeats enough full-batch passes to target at least 16,777,216 cell recoveries.

## Semantic and integrity gates

Before timing is accepted:

- a deterministic 1,048,576-cell mixed-phase input is recovered by the direct formal batch path;
- worker-pool results at 1, 2, 4, and 8 requested workers must exactly reproduce direct Temperature and liquid-fraction outputs;
- source Thermodynamic State values remain unchanged;
- worker partitions must cover the destination exactly once without overlap;
- any worker exception invalidates the run;
- checksums are computed outside timed intervals;
- no Framework or Material implementation source is modified by the benchmark.

## Timing procedure

For each working set:

- all five execution paths are created before timing;
- 3 warmup rounds execute every path;
- 7 timed rounds execute every path;
- the starting scenario rotates each round so no path always occupies the coldest or hottest CPU/JIT position;
- samples are therefore interleaved rather than completing all samples for one scenario before moving to the next;
- median, minimum, maximum, nanoseconds/cell, and throughput are reported;
- absolute hosted-runner timing is treated as environment-specific.

The direct caller and one-worker pool are deliberately retained as separate baselines because thread placement and worker-pool orchestration can differ from direct caller execution.

Two relative metrics are reported:

`speedup_vs_direct(W) = median_time(direct_single_thread) / median_time(worker_pool_W)`

and, for the worker-pool scaling question:

`speedup_vs_worker1(W) = median_time(worker_pool_1) / median_time(worker_pool_W)`

The second metric is the primary worker-scaling comparison because it keeps the persistent-worker execution model constant while changing only requested worker count.

Descriptive worker scaling efficiency is:

`worker_scaling_efficiency(W) = speedup_vs_worker1(W) / W`

Efficiency is descriptive only, especially when `W > Environment.ProcessorCount`.

## Multi-environment confirmation

At least two successful hosted-runner executions should be reviewed before a scaling conclusion is promoted. Different CPU models are preferred when the hosted environment supplies them naturally.

If all reviewed runners expose only two logical processors, the study may support 1-to-2-worker scaling and oversubscription behavior at 4/8 workers, but it shall not claim 4-core or 8-core scaling.

## Interpretation boundaries

The study may support statements such as:

- two-worker recovery improves or does not improve throughput relative to the one-worker pool on the tested environment;
- scaling saturates before, at, or after the environment-reported processor count;
- oversubscribed 4/8-worker execution improves, plateaus, or regresses;
- coordination overhead is negligible or material for the tested large batches;
- direct caller and one-worker timings differ, without mislabeling that difference as parallel scaling.

The study shall not claim:

- universal thread scaling across CPUs;
- 4-core or 8-core scaling unless the tested environment actually exposes that many processors;
- NUMA behavior from a two-processor hosted runner;
- GPU, Unity, engine, mobile, or production-hardware performance;
- that the Framework requires a particular scheduler or thread count;
- Framework Conformance or physical Validation.

No post-hoc performance PASS/FAIL threshold is adopted.

## Evidence disposition

A result record may be added only after successful execution logs and semantic gates are reviewed. If scaling differs materially across runner CPU models, the result shall preserve that environment sensitivity rather than collapse it into one universal speedup number.

Historical predecessor runs using non-interleaved scenario ordering may be retained as methodological history but shall not be used as the final scaling basis if run-order bias is observed.

## Specification impact

- Framework Specification change: None
- Reference Formulation change: None
- Framework Freeze reopen: No
- Runtime State ownership change: None
- Derived State invariant relaxation: None
- Scheduler requirement introduced: None
- New Framework component / owner: None
