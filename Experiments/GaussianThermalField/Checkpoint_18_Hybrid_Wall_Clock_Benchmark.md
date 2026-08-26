# Checkpoint 18 — Hybrid Wall-Clock Benchmark

Status: **PERFORMANCE PASS — hybrid verification nearly halves measured fitter wall-clock**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Does the fit-level reduction demonstrated by Checkpoint 17 produce a comparable reduction in actual elapsed CPU time for the current nested greedy Gaussian fitter?

Checkpoint 17 established a work-count reduction from an exhaustive `24` nested fit levels per snapshot to `11..14` proposal-bounded levels, with the same minimum total Gaussian allocation on all six declared snapshots.

This checkpoint measures the wall-clock consequence directly.

## Timed Scope

The timed operation is the **online nested Gaussian fitting phase only**.

Included:

```text
ConstrainedGaussianSparseFitter1D.FitSequence(...)
for the three material regions
```

Excluded from timing:

```text
physical snapshot construction / state evolution
offline model calibration
held-out training instrumentation
full oracle generation
CI process startup and .NET build
```

The hybrid benchmark uses the already validated Checkpoint-17 proposal upper bounds:

```text
0.10 s -> 6/8/0
0.20 s -> 5/6/1
0.40 s -> 4/5/2
0.60 s -> 3/5/3
1.00 s -> 4/5/2
1.50 s -> 4/4/3
```

The exhaustive baseline always constructs `8/8/8 = 24` nested fit levels.

## Measurement Method

The benchmark runs in Release mode on the same GitHub Actions runner process.

To reduce measurement bias:

- both paths are warmed up before measurement;
- each snapshot is measured `7` times;
- exhaustive and hybrid order alternates between trials;
- garbage from the preceding measurement is collected outside the timed interval;
- the median is reported rather than a single run;
- minimum timings are also recorded as a noise reference.

This is still a hosted-VM benchmark rather than dedicated bare-metal benchmarking, so the exact millisecond values should be interpreted as environment-specific. The within-process ratio is the stronger result.

## Results

```text
time  proposal  levels   exhaustive median   hybrid median   speedup   reduction
0.10  6/8/0     14/24       232.496 ms        133.305 ms      1.744x     42.7%
0.20  5/6/1     12/24       232.168 ms        126.923 ms      1.829x     45.3%
0.40  4/5/2     11/24       232.234 ms        115.060 ms      2.018x     50.5%
0.60  3/5/3     11/24       232.235 ms        114.397 ms      2.030x     50.7%
1.00  4/5/2     11/24       232.544 ms        115.269 ms      2.017x     50.4%
1.50  4/4/3     11/24       232.441 ms        113.252 ms      2.052x     51.3%
```

Aggregate median-sum across the six snapshots:

```text
exhaustive = 1394.118 ms
hybrid     =  718.205 ms
speedup    =    1.941x
reduction  =   48.48%
```

Minimum observed timings were close to the corresponding medians, indicating low within-run timing dispersion for this hosted runner.

## Interpretation

The measured wall-clock reduction closely follows the prior fit-level reduction:

```text
mean fit-level reduction    = 51.39%
measured wall-clock reduction = 48.48%
```

Therefore the removed nested fit levels correspond to real computational work in the present fitter rather than merely reducing a bookkeeping counter.

The strongest bounded statement is:

> For the declared 1D A-B-C snapshots and the current constrained nested greedy fitter, proposal-bounded hybrid verification reduced measured online fitting wall-clock by about 48.5%, corresponding to about 1.94x speedup, while Checkpoint 17 retained the same oracle-minimum Gaussian total on all six snapshots.

## Important Runtime Meaning

This benchmark does **not** show that the complete thermal simulation is 1.94x faster.

It shows that the **Gaussian representation rebuilding / allocation fitting stage** is 1.94x faster than exhaustive fitting.

The measured hybrid fitting time remains approximately:

```text
113..133 ms per snapshot
```

on this GitHub-hosted CPU environment.

That is not a 60-FPS per-frame budget (`16.7 ms`). Therefore the present constrained dictionary fitter should be treated as an experimental representation-construction algorithm, not yet as a proven per-frame production implementation.

A practical runtime system could still benefit if Gaussian allocation/re-fitting occurs less frequently than physical state evolution, or if the fitter is later optimized / parallelized / moved to GPU. Those possibilities are not demonstrated by this checkpoint.

## Relation to the Formula Goal

The performance evidence strengthens the hybrid formulation:

```text
State-derived formula
        -> proposal upper bounds
        -> approximately half-sized fitting search
        -> direct local verification
        -> exact global error allocation
```

The formula does not need to replace numerical verification to provide a measurable runtime benefit.

## Remaining Performance Question

A true end-to-end runtime study should separately measure:

```text
state evolution
state-complexity metric evaluation
proposal inference
Gaussian fitting / verification
final representation queries
```

Only then can a total simulation/update speedup be reported.

## Scope Limit

All timing numbers are specific to the current C#/.NET 8 implementation, GitHub-hosted Ubuntu runner, constrained Gaussian candidate dictionary, nested greedy fitting algorithm, six bounded 1D A-B-C snapshots, and proposal counts validated by Checkpoint 17. They do not define ThermoCore performance or general hardware performance.
