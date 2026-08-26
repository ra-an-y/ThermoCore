# Checkpoint 17 — Hybrid Formula-Propose / Local-Verify Allocator

Status: **EXPLORATORY PASS — hybrid search recovers oracle total with reduced online fit levels**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Can the imperfect regional error formula still be useful if it is not trusted to produce the final Gaussian allocation directly?

The hybrid strategy tested here is:

```text
Current State
    |
    v
Checkpoint-16 count-dependent formula proposal
    |
    v
Per-region proposal upper bounds
    |
    v
Direct nested sparse fitting only up to those bounds
    |
    v
Exact global L2 allocation inside the verified box
    |
    v
Minimum verified Gaussian allocation
```

The formula therefore proposes the search region; direct fitting owns the final accuracy decision.

## Important Cost-Model Correction

The current `ConstrainedGaussianSparseFitter1D` is a nested greedy fitter. A fit with `N` retained Gaussians is constructed through the sequence

```text
1 -> 2 -> ... -> N
```

Therefore this checkpoint does **not** pretend that verifying an isolated count `N` costs one independent fit.

The online verification cost is measured as the number of nested fit levels constructed:

```text
verification levels = N_A^proposal + N_B^proposal + N_C^proposal
```

The exhaustive baseline is

```text
3 regions * 8 levels = 24 fit levels
```

per snapshot.

Proposal-generation training and the full oracle are evaluation instrumentation and are not included in this online verification-level count.

## Proposal Source

The upper bounds are the count-dependent held-out proposals from Checkpoint 16. They are not assumed to be accurate final allocations.

For each region, the fitter is evaluated only from `1` through the proposed count. All combinations inside that verified box are then evaluated with the exact identity

```text
E_global^2 = sum_i [ w_i e_i(N_i) ]^2
```

plus the existing zero-Gaussian peak guard.

The minimum verified allocation in the truncated box is selected.

## Results

```text
time  proposal A/B/C  proposal total  hybrid A/B/C  hybrid total  direct global  oracle total  fit levels  saved
0.10      6/8/0             14            3/3/0           6          0.3993%          6        14/24       10
0.20      5/6/1             12            5/2/1           8          0.3125%          8        12/24       12
0.40      4/5/2             11            4/3/1           8          0.4214%          8        11/24       13
0.60      3/5/3             11            2/3/2           7          0.3912%          7        11/24       13
1.00      4/5/2             11            2/2/2           6          0.4525%          6        11/24       13
1.50      4/4/3             11            1/2/2           5          0.4850%          5        11/24       13
```

Summary:

```text
safe snapshots                  = 6 / 6
oracle-total matches            = 6 / 6
online fit levels               = 11 .. 14
exhaustive baseline             = 24
saved fit levels                = 10 .. 13
mean fit-level reduction        = 51.39%
maximum identity discrepancy    = 1.084e-17
```

## Interpretation

This is the strongest allocator result in the current bounded trajectory.

The pure formula allocator from Checkpoints 15–16 remained too conservative and could over-allocate by several Gaussians. In contrast, the hybrid method uses that conservatism only to define an upper search box.

Within the tested snapshots, that box contains a globally minimal allocation in every case. Direct local fitting then removes the formula's excess and recovers the same minimum total Gaussian count as the full `0..8` oracle search.

The key separation is therefore:

```text
formula      -> search-space reduction
fitter       -> local numerical evidence
exact L2 law -> global allocation decision
```

The formula no longer needs to be an accurate point predictor in order to be operationally useful.

## Computational Meaning

The checkpoint measures **fit-level work**, not wall-clock runtime.

Because the current greedy fitter constructs all lower retained counts on the way to `N`, reducing the maximum verified count directly removes candidate-selection stages. The measured reduction is therefore meaningful for this fitter, but it must not be interpreted as a proven `51.39%` runtime reduction until a dedicated performance benchmark measures actual execution time.

The current result is:

```text
24 exhaustive nested levels
        ->
11..14 proposal-bounded nested levels
```

with the same global error threshold and the same oracle minimum total count in all six declared snapshots.

## Important Limitations

This is not yet a general guarantee that the formula proposal always contains the global optimum component-wise. The result is empirical for the declared trajectory.

A robust future allocator should specify a fallback when no safe solution exists inside the proposed box, for example:

```text
proposal box
    |
    +-- safe solution found -> return verified minimum
    |
    +-- no solution -> expand the most promising region by one level
                       and re-verify
```

This would turn the current bounded-box experiment into an incremental production-style hybrid search.

Also, proposal-model training and oracle generation are intentionally excluded from the online fit-level metric. A later implementation benchmark should separate offline calibration, state-metric evaluation, proposal inference, and online fitting costs explicitly.

## Research Consequence

The original goal was to derive a direct formula

```text
N_i = f(S_i, epsilon)
```

The evidence now suggests a more robust practical form:

```text
N_i^upper = f(S_i, epsilon)

verified allocation
    = argmin_N sum_i N_i
      subject to direct fitted regional errors
      and exact global error constraint,
      inside the proposal-bounded search space.
```

This preserves the mathematical global allocation law while using the learned/state-derived formula only where it is currently reliable: reducing search effort.

## Scope Limit

All conclusions are specific to the bounded 1D A-B-C trajectory, six declared snapshots, the current 32-mode reduced state, constrained Gaussian dictionary, nested greedy fitter, `N=0..8`, and the `0.5%` global representation-error target. This checkpoint does not define ThermoCore behavior or requirements.
