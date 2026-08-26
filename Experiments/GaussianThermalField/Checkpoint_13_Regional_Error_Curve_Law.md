# Checkpoint 13 — Regional Error-Curve Law

Status: **EXPLORATORY PASS — bounded 1D experiment**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Can the regional Gaussian representation-error curve be approximated by a simple analytic law of Gaussian count, and can current-state complexity begin to explain the law parameters?

The target is not merely to correlate a state metric with a final selected Gaussian count. The stronger objective is to predict the full regional error curve

```text
e_i(N) = regional relative representation error using N Gaussians
```

so that the exact global error-budget identity can later allocate Gaussian counts without exhaustively fitting every candidate count.

## Candidate Families

For every non-negligible state-region snapshot, the observed constrained sparse-fit errors for `N = 1..8` were fit in log space to:

```text
Exponential: e(N) = A exp(-lambda N)
Power:       e(N) = A N^(-p)
```

The dataset contains 17 non-negligible state-region samples across the declared A-B-C snapshots.

## Individual Curve Results

The power family was the better log-space fit for 14 of 17 samples.

```text
individual winners
power       14
exponential  3
```

Mean individual fit quality:

```text
mean exponential R2 = 0.815169
mean power R2       = 0.925321
```

Therefore the bounded dataset supports the working hypothesis that regional sparse-Gaussian approximation error is more power-law-like than exponential over `N = 1..8`.

This is an empirical result for the current candidate dictionary and bounded thermal states, not a universal approximation theorem.

## Curvature-Only Pooled Model

Using the normalized curvature score `C` from Checkpoint 11, a pooled power model was first tested:

```text
log e = beta0 + betaC log(1+C) + betaN log N
```

Result:

```text
log-space R2              = 0.716459
log-space RMSE            = 0.900522
leave-one-state-out RMSE  = 1.011938
```

The grouped predictive error is too large for this model to replace direct fitting.

## Curvature-Dependent Power Exponent

Inspection of the individually fit exponents showed that the decay exponent itself changes with state curvature. An interaction model was therefore tested:

```text
log e = beta0
      + betaC log(1+C)
      + betaN log N
      + betaCN log(1+C) log N
```

Fitted coefficients:

```text
beta0  = -4.420458
betaC  =  0.594208
betaN  = -2.082754
betaCN =  0.349910
```

Equivalent form:

```text
e(N,C) ~= 0.0120 (1+C)^0.5942
          * N^[-(2.0828 - 0.3499 log(1+C))]
```

The implied positive power exponent over the sampled state range is approximately:

```text
p(C) = 2.0828 - 0.3499 log(1+C)
range: 0.9721 .. 2.0386
```

Fit quality:

```text
log-space R2              = 0.729021
log-space RMSE            = 0.880347
leave-one-state-out RMSE  = 0.997135
```

The interaction improves predictive error only modestly relative to the curvature-only pooled power model.

## Interpretation

Two useful conclusions emerge.

First, the regional error curve is not well described by one universal decay exponent. More complex current states tend to exhibit slower Gaussian-error decay with increasing representation count.

Second, normalized curvature alone is insufficient to predict the full error curve with the accuracy required for safe direct allocation. A leave-one-state-out log RMSE near `1.0` corresponds to order-unity multiplicative uncertainty in predicted error.

Therefore the current equation is a **candidate structural form**, not a production allocation formula.

## Current Formula Stack

The exact part of the allocation problem remains:

```text
E_global^2 = sum_i [w_i e_i(N_i)]^2
```

Checkpoint 13 concerns only the still-approximate regional term `e_i(N_i)`.

Conceptually:

```text
Current State
    |
    v
State shape metrics
    |
    v
approximate regional error law e_i(N)
    |
    v
exact global L2 budget identity
    |
    v
Gaussian allocation
```

## Scope Limit

The power-law preference and fitted coefficients are specific to the current bounded 1D A-B-C dataset, `N = 1..8`, the declared constrained sparse fitter, and its Gaussian candidate dictionary. They are not ThermoCore requirements and are not claimed to generalize to arbitrary geometry or Gaussian bases.
