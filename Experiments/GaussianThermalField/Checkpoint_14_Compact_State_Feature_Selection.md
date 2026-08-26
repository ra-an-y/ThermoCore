# Checkpoint 14 — Compact State Feature Selection for the Error Law

Status: **EXPLORATORY PASS — bounded 1D experiment**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Can one additional quantity derived from the current reduced state materially improve prediction of the regional Gaussian error curve beyond normalized curvature alone?

The goal is to keep a future formula compact. Instead of adding many state descriptors simultaneously, this checkpoint keeps normalized curvature as the primary shape coordinate and tests exactly one additional feature at a time.

Selection is based on grouped leave-one-state-out log RMSE, not training R2.

## Candidate Additional Features

The tested second features were:

```text
normalized gradient score
modal energy fraction
spectral entropy
mean dominance
boundary contrast
```

Each two-feature model allows both curvature and the additional feature to affect the power-law amplitude and the effective exponent through interaction with `log N`.

## Results

```text
feature                 params  train logR2  train logRMSE  LOSO logRMSE
curvature only             4      0.729021      0.880347      0.997135
normalized gradient        6      0.745161      0.853727      1.017096
modal energy fraction      6      0.731582      0.876177      1.013759
spectral entropy           6      0.762490      0.824190      1.186210
mean dominance             6      0.740104      0.862156      0.993023
boundary contrast          6      0.775374      0.801524      0.947618
```

The best grouped predictive result is obtained by **boundary contrast**.

## Interpretation

Boundary contrast improves the leave-one-state-out error from:

```text
0.997135 -> 0.947618
```

This suggests that normalized curvature and boundary contrast capture different aspects of Gaussian representability:

```text
curvature
-> local spatial roughness / high-order shape complexity

boundary contrast
-> large-scale one-sidedness / placement of the field within a finite region
```

This is consistent with the experiment's finite-layer geometry. Two fields may have similar spectral roughness while differing in how strongly their temperature profile is biased from one interface toward the other; a finite Gaussian dictionary can represent those profiles with different efficiency.

## Overfitting Counterexample

Spectral entropy gives a better training fit than curvature alone:

```text
training R2: 0.762490
```

but its leave-one-state-out RMSE degrades to:

```text
1.186210
```

Therefore higher training R2 is explicitly rejected as a model-selection criterion for this research line.

## Current Candidate State Coordinates

The minimal currently supported pair is therefore:

```text
C_kappa = normalized curvature score
B       = normalized boundary contrast
```

A future regional error law may take the structural form:

```text
log e(N)
= beta0
+ betaC f(C_kappa)
+ betaB g(B)
+ betaN log N
+ betaCN f(C_kappa) log N
+ betaBN g(B) log N
```

The exact coefficients are not promoted here because the current cross-validated predictive spread remains too large for safe replacement of direct sparse fitting.

`LOSO log-RMSE = 0.947618` corresponds to a multiplicative RMS scale of approximately `exp(0.9476) ~= 2.58`, which is still too uncertain near a strict global error threshold.

## Research Consequence

Checkpoint 14 narrows the formula target without declaring completion:

```text
Current Reduced State
    |
    +--> normalized curvature
    +--> boundary contrast
    |
    v
regional error-law predictor
    |
    v
exact global error-budget formula
```

The next rigorous question is not whether more features can improve training fit. It is whether a compact predictor can safely drive allocation on held-out state snapshots, potentially using a calibrated conservative uncertainty margin.

## Scope Limit

All feature rankings are specific to the current bounded 1D A-B-C trajectory, constrained Gaussian dictionary, and snapshot set. Boundary contrast is a candidate predictor, not a universal physical invariant or ThermoCore requirement.
