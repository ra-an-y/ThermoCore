# Checkpoint 19 — Temporal Warm-Start Runtime

Status: **PERFORMANCE PASS — temporal reuse reduces sequential representation-update cost with bounded fallback**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Can a previous Gaussian field representation be reused across nearby physical states so that every representation update does not require a fresh sparse support search?

The tested runtime path is:

```text
previous representation
  -> direct reuse validation
  -> fixed-support amplitude refit if needed
  -> fresh sparse rebuild fallback if still above threshold
```

The Gaussian representation remains downstream and non-authoritative. Reduced physical state and state evolution are unchanged.

## Coarse-snapshot negative result

Reusing between the widely spaced research snapshots (`0.10, 0.20, 0.40, 0.60, 1.00, 1.50 s`) failed on every transition. Direct reuse errors were about `7–26%`, and amplitude-only errors about `3.6–8.8%`, far above the `0.5%` threshold. Because every transition ultimately rebuilt fresh, this warm path was about `5%` slower.

This establishes that research snapshot spacing is too coarse to represent runtime temporal coherence.

## Fine-cadence one-step result

Using the verified `2/3/2` Gaussian representation at `t=0.60 s`, the unchanged representation stayed below `0.5%` through a `16 ms` physical interval:

```text
delta t   reuse error   amplitude error   fresh error
0.002      0.3768%        0.3817%          0.3802%
0.004      0.3695%        0.3747%          0.4213%
0.010      0.3909%        0.3693%          0.3200%
0.016      0.4648%        0.3865%          0.3411%
0.032      0.7725%        0.5082%          0.4035%
```

At `16 ms`, a representative hosted-runner measurement was approximately:

```text
fresh same-budget rebuild ~68.3 ms
direct reuse validation   ~1.18 ms
one-step speedup           ~58x
```

This one-step number is not a long-run speed claim because reuse error can accumulate.

## Sequential 16-ms scheduler

The final test carried each accepted representation into the next update.

```text
physical window: 0.600 -> 0.792 s
cadence:         16 ms
updates:         12
fixed budget:    2/3/2 Gaussians
global threshold: 0.5%
```

At every update, direct reuse was tried first, then amplitude-only refit, then a fresh same-budget sparse rebuild if required.

Results:

```text
direct reuse accepted      3 / 12
amplitude-only refit       4 / 12
fresh sparse fallback      5 / 12
fresh search avoided       7 / 12 = 58.3%
maximum accepted error     0.46480059%
```

Five repeated whole-trajectory timing trials produced:

```text
fresh rebuild every update  823.128 ms
stateful warm scheduler      377.986 ms

fresh mean/update             68.594 ms
warm mean/update              31.499 ms

speedup                         2.18x
wall-clock reduction           54.08%
```

The initial representation at `t=0.60 s` is excluded from both trajectory timings.

## Interpretation

The strongest bounded statement is:

> In the declared 1D A-B-C sequential window, the 16-ms stateful warm-start scheduler avoided fresh sparse support search on 7 of 12 updates, maintained the declared global representation-error threshold, and reduced measured representation-update wall-clock by about 54% relative to rebuilding the same `2/3/2` Gaussian budget at every update.

The runtime pattern is therefore non-uniform:

```text
cheap reuse -> amplitude correction -> cheap reuse -> fresh rebuild -> ...
```

Checkpoint 17–18 reduce the cost when a fresh sparse search is required; this checkpoint reduces how often fresh search is required. These mechanisms are complementary, but the current sequential benchmark intentionally fixes the Gaussian count to isolate temporal support reuse.

## Limit

The average warm representation-update cost is still about `31.5 ms` on this hosted CPU, so this is not evidence of synchronous 60-Hz (`16.7 ms`) representation rebuilding or 60-FPS end-to-end simulation. State evolution, proposal inference, rendering/application work, and other system costs are outside this timing.

The remaining dominant opportunity is reducing the cost/frequency of the `5/12` fresh support fallbacks, for example by allowing incremental support relocation rather than immediately returning to a fresh sparse search. This is not demonstrated here.

All conclusions are limited to the current C#/.NET 8 implementation, hosted runner, bounded 1D trajectory, fixed `2/3/2` sequential budget, current Gaussian dictionary/fitter, and `0.5%` global L2 representation-error target. This checkpoint does not define ThermoCore behavior or performance.
