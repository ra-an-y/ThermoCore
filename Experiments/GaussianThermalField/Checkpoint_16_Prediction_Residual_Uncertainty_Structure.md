# Checkpoint 16 — Prediction Residual / Uncertainty Structure

Status: **EXPLORATORY PASS — count-dependent uncertainty helps, predictor still not deployable**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Does the regional error-law prediction uncertainty have enough structure to replace the single worst-case safety multiplier used in Checkpoint 15 with a less pessimistic bound?

The first uncertainty refinement is deliberately small:

```text
single safety factor s
        ->
count-dependent safety factor s(N), N = 1..8
```

No new state features are added to the mean predictor.

## Methodological Correction

An initial diagnostic computed correlations between **training residuals** and fitted regressors. Those correlations were approximately zero because ordinary least-squares residuals are orthogonal to the model design columns by construction.

Those values are therefore not used as evidence.

The corrected residual-structure diagnostic evaluates each complete time snapshot only when it is held out from fitting. Safety calibration remains training-only, while residual-correlation analysis uses held-out residuals only.

## Mean Predictor

The regional mean predictor is unchanged from Checkpoints 14–15:

```text
log e(N)
= beta0
+ betaC log(1+C_kappa)
+ betaB log(1+B)
+ betaN log N
+ betaCN log(1+C_kappa) log N
+ betaBN log(1+B) log N
```

where:

```text
C_kappa = normalized curvature
B       = normalized boundary contrast
```

## Count-Dependent Training Calibration

For each held-out fold and each Gaussian count `N`, a training-only multiplier is computed:

```text
s_train(N)
= max_training_at_same_N [ e_actual(N) / e_predicted(N) ]
```

with a lower bound of `1`.

The allocator then uses:

```text
e_upper(N) = s_train(N) * e_predicted(N)
```

inside the exact global identity:

```text
E_global^2 = sum_i [ w_i e_upper,i(N_i) ]^2
```

The held-out direct sparse-fit errors are revealed only after allocation.

## Corrected Held-Out Residual Diagnostics

Across `136` held-out regional/count residual points:

```text
corr(residual, log N)             =  0.001641
corr(residual, log(1+C_kappa))    = -0.031967
corr(residual, log(1+B))          = -0.013686
```

Therefore there is no useful **linear monotonic** residual trend with these three coordinates over the declared dataset.

This does not mean uncertainty is constant. Count-stratified dispersion is strongly non-uniform:

```text
N  mean training safety  max training safety  held-out max underprediction  held-out log-RMS
1       3.6825               4.1815                    3.4850                  0.6773
2       4.4015               5.1180                    4.9259                  1.0933
3       5.0333               5.8418                    6.3516                  1.1671
4       5.6893               7.1232                    7.6749                  1.0929
5       4.1302               4.6923                    5.0204                  0.9528
6       3.9006               4.5950                    4.8882                  0.8484
7       3.7192               4.5265                    4.6280                  0.7269
8       2.8709               3.4257                    3.5230                  0.6488
```

The uncertainty pattern is therefore **non-monotonic**. It is largest around the middle counts (`N=3..4`) and smaller at the ends of the tested range.

A single Pearson correlation with `log N` cannot represent this shape.

## Held-Out Allocation Results

Comparison with the prior single-factor allocator:

```text
time  single-factor total  count-dependent total  actual global  oracle total  count overhead
0.10          none                  14                0.0988%          6            +8
0.20           14                   12                0.2835%          8            +4
0.40           12                   11                0.3269%          8            +3
0.60           13                   11                0.1731%          7            +4
1.00           12                   11                0.4197%          6            +5
1.50           12                   11                0.1284%          5            +6
```

Summary:

```text
single-factor feasible folds      = 5 / 6
count-dependent feasible folds    = 6 / 6
count-dependent safe folds        = 6 / 6
```

For the five folds that were already feasible with the single factor, the count-dependent model reduces Gaussian count by `1..2` in every case.

It also recovers a feasible solution at `t = 0.10 s`, but that solution is highly conservative (`14` Gaussians versus oracle `6`).

## Important Safety Limitation

The training-only `s(N)` is **not** a pointwise held-out upper bound. For example:

```text
N=3: maximum training safety 5.8418, held-out underprediction reaches 6.3516
N=4: maximum training safety 7.1232, held-out underprediction reaches 7.6749
```

The six selected held-out allocations nevertheless remain below the global `0.5%` threshold because the worst regional residuals do not necessarily dominate the selected global weighted error.

Therefore this checkpoint demonstrates improved empirical allocation safety/feasibility, not a certified error bound.

## Interpretation

Checkpoint 16 separates two facts:

1. uncertainty is not well modeled as a simple monotonic function of `N`, `C_kappa`, or `B`;
2. uncertainty is still structured enough by Gaussian count that `s(N)` is materially better than one global worst-case multiplier.

The current picture is:

```text
Current State
    |
    +--> mean regional error predictor e_hat(N, C_kappa, B)
    |
    +--> count-specific uncertainty scale s(N)
    |
    v
conservative regional estimate
    |
    v
exact global L2 allocation identity
```

However, the remaining overhead (`+3..+8` Gaussians in the declared held-out cases) is still too large for the formula to replace direct sparse fitting.

## Next Research Question

The next rigorous target is not another arbitrary state feature. It is to explain the **non-monotonic count-wise residual distribution** and calibrate uncertainty without using a fold-wide maximum.

Candidate directions include:

```text
count-wise residual quantiles / conformal-style calibration
count-bin upper confidence bounds
state-regime conditioning only if justified by held-out residuals
hybrid allocator: formula proposes N, one local fit verifies/refines it
```

A hybrid formula-plus-verification allocator is particularly attractive because it could reduce exhaustive `N=1..8` fitting while retaining a direct numerical check near the final selected budget.

## Scope Limit

All results are specific to the bounded 1D A-B-C trajectory, six declared snapshots, the current constrained Gaussian dictionary, `N=0..8`, and the `0.5%` global representation-error target. They do not define ThermoCore behavior or requirements.
