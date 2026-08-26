# Checkpoint 11 — State Complexity Metric Study

Status: **PASS — exploratory bounded 1D study**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Goal

Move from empirical Gaussian-count scans toward a formula that can estimate downstream representation complexity directly from the current reduced state.

This checkpoint does **not** claim a final predictor. It asks which state-derived dimensionless shape metrics are most associated with the observed Gaussian count required to reach the current regional `0.5%` representation-error target.

Negligible regions that satisfy the separate zero-Gaussian rule are excluded. A required count of `9` denotes a censored observation where the bounded one-to-eight Gaussian dictionary did not reach the regional target.

## Candidate state metrics

For cosine-mode coefficients `a_n`, the study evaluates:

```text
RMS mode index
quartic mode index
high-mode energy fraction (n >= 5)
spectral entropy
effective mode count
normalized gradient score
normalized curvature score
```

The normalized curvature-like score is

```text
C_kappa = sqrt( 0.5 * sum(n^4 * a_n^2)
                / (mean^2 + 0.5 * sum(a_n^2)) )
```

The corresponding normalized gradient score replaces `n^4` with `n^2`.

These scores are dimensionless in the current normalized modal coordinate. They measure shape roughness relative to total represented field energy rather than simply counting high modes.

Global L2 contribution and global peak contribution are also recorded as importance variables, but they are conceptually distinct from shape complexity.

## Correlation results

Across the non-negligible region/snapshot observations:

```text
metric                       Pearson(required N)   Spearman(required N)
rms-mode-index                     0.470704             -0.105088
quartic-mode-index                 0.405312             -0.086322
high-mode-energy-fraction          0.635998             -0.068808
spectral-entropy                   0.354648             -0.070059
effective-mode-count               0.455192             -0.070059
normalized-gradient-score          0.757881              0.534198
normalized-curvature-score         0.779371              0.564223
global-L2-contribution            -0.559926             -0.399084
global-peak-contribution          -0.604729             -0.452046
```

The strongest single candidate in this bounded dataset is the normalized curvature score:

```text
|Pearson|  = 0.779371
|Spearman| = 0.564223
```

## Interpretation

The result supports a useful distinction:

```text
shape complexity
    !=
global importance
```

A region can have a high modal index while still being easy to approximate if those rough components are very small compared with the field mean or total field energy. This is why raw RMS/quartic modal index is weaker than the normalized gradient/curvature scores.

The negative correlation of global contribution with the old regional required-count target is also diagnostic rather than a desired law. Small-but-non-negligible regions can receive artificially large required counts because a regional relative percentage divides by a small regional norm. This reinforces the need to replace equal per-region relative thresholds with a global error budget.

Therefore the current candidate direction is not simply:

```text
N_i = f(curvature_i)
```

but rather a split model:

```text
State shape metric C_i
        -> representation difficulty

Global weight w_i / peak guard
        -> representation importance
```

A future budget formula should combine these two roles explicitly.

## Next mathematical step

For disjoint material regions, the global L2 representation error can be decomposed exactly from regional relative errors and regional global weights. If

```text
w_i = ||T_i||_2 / ||T_global||_2

e_i(n) = ||G_i(n) - T_i||_2 / ||T_i||_2
```

then

```text
E_global^2 = sum_i [ w_i * e_i(N_i) ]^2
```

under the same disjoint sampled-domain norm.

This identity is a stronger basis for allocation than imposing the same percentage error on every region. The next checkpoint should verify this decomposition numerically and use it to formulate the minimum-budget optimization directly.

## Scope limit

The correlations come from one bounded 1D A-B-C experiment, six snapshot times, a 32-mode reduced state, the current constrained greedy Gaussian dictionary, and the declared `0.5%` regional target. They are evidence for candidate variables, not a universal Gaussian-complexity law.
