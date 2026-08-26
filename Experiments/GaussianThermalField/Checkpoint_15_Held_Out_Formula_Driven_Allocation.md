# Checkpoint 15 — Held-Out Formula-Driven Gaussian Allocation

Status: **EXPLORATORY PASS — predictor not yet deployable**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Can the compact state-based regional error-law predictor drive Gaussian allocation on an unseen time snapshot while preserving the declared global representation-error threshold?

This checkpoint evaluates the predictor as an allocator rather than as a regression model.

## Held-Out Protocol

Each fold removes one complete time snapshot from model fitting. The model uses only the remaining snapshots and the compact state coordinates identified previously:

```text
C_kappa = normalized curvature
B       = normalized boundary contrast
```

The regional predictor has the power-law interaction form

```text
log e(N)
= beta0
+ betaC log(1+C_kappa)
+ betaB log(1+B)
+ betaN log N
+ betaCN log(1+C_kappa) log N
+ betaBN log(1+B) log N
```

For each fold, a multiplicative safety factor is calibrated **only from training data**:

```text
s_train = max_training [ e_actual / e_predicted ]
```

with a lower bound of `1`. The allocator therefore uses

```text
e_conservative(N) = s_train * e_predicted(N)
```

for every nonzero Gaussian count.

The held-out snapshot contributes only current-state metrics, global L2 weights, and the existing peak zero-budget guard. Direct sparse-fit errors are withheld until after allocation and used only for validation and oracle comparison.

## Global Allocation Constraint

The formula-driven allocator minimizes total Gaussian count subject to

```text
sum_i [ w_i e_conservative,i(N_i) ]^2 <= (0.005)^2
```

with the existing zero-Gaussian peak guard.

An oracle allocation is separately computed from the true held-out sparse-fit curves. The oracle is not visible to the formula-driven allocator.

## Results

```text
time  safety  formula A/B/C  total  predicted global  actual global  oracle A/B/C  oracle total  overhead
0.10   6.047       none        -         n/a              n/a          3/3/0           6          -
0.20   6.828       6/7/1      14       0.4725%          0.1814%        5/2/1           8         +6
0.40   5.257       4/6/2      12       0.4319%          0.3224%        4/3/1           8         +4
0.60   7.123       4/6/3      13       0.4688%          0.1174%        2/3/2           7         +6
1.00   4.589       4/5/3      12       0.4457%          0.0456%        1/2/3           6         +6
1.50   4.582       4/5/3      12       0.4492%          0.1219%        1/2/2           5         +7
```

Summary:

```text
formula-feasible folds       = 5 / 6
held-out safe folds          = 5 / 6 overall
safe among feasible folds    = 5 / 5
exact oracle-total matches   = 0 / 6
maximum safety factor        = 7.123153
maximum Gaussian overhead    = 7
```

## Interpretation

The result separates **safety** from **efficiency**.

The training-maximum calibration is conservative enough that every held-out fold for which it finds an allocation remains below the `0.5%` global representation-error threshold after direct sparse-fit validation.

However, the calibration is too pessimistic to serve as a useful replacement for direct fitting:

- the required safety multiplier is roughly `4.58x` to `7.12x`;
- the formula-selected budgets exceed the oracle minimum by `4` to `7` Gaussians;
- the `t = 0.10 s` fold becomes infeasible within the declared `0..8` Gaussian-per-region search space even though the oracle requires only `6` total Gaussians;
- none of the six folds matches the oracle total count.

Therefore the current compact predictor cannot yet be promoted to a direct allocator.

## Research Consequence

The failure is not in the exact global-budget identity:

```text
E_global^2 = sum_i [ w_i e_i(N_i) ]^2
```

That identity remains exact for the declared disjoint-region L2 definition.

The remaining limitation is uncertainty in the regional predictor `e_i(N)`. A single worst-case multiplicative envelope over all training points is too coarse because it treats all state regions and all Gaussian counts as if they shared the same prediction uncertainty.

The next rigorous direction is therefore **uncertainty structure**, not additional arbitrary state features. Candidate questions include whether prediction residuals depend systematically on Gaussian count, curvature regime, or boundary-placement regime, and whether a count-dependent or locally calibrated upper bound can remain safe without the `4.6x–7.1x` global penalty.

## Scope Limit

This checkpoint is specific to the bounded 1D A-B-C trajectory, six declared snapshots, `N = 0..8`, the current constrained Gaussian dictionary, and the `0.5%` global representation-error target. It is an experimental result and does not define ThermoCore behavior or requirements.
