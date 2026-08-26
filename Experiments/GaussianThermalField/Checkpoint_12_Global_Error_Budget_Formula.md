# Checkpoint 12 — Global Error-Budget Formula

Status: **PASS — exact sampled-L2 identity for the bounded disjoint 1D case**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Goal

Replace equal per-region relative-error thresholds with a mathematically decomposed global representation-error budget.

The reduced physical state remains unchanged. The formula concerns only allocation of downstream Gaussian representation primitives.

## Identity

For disjoint material regions, define the physical-length-weighted regional norm

```text
||T_i||_2^2 ~= dx_i * sum_j T_i(x_j)^2
```

and

```text
w_i = ||T_i||_2 / ||T_global||_2

e_i(N_i) = ||G_i(N_i) - T_i||_2 / ||T_i||_2
```

Then the global relative representation error satisfies

```text
E_global^2 = sum_i [ w_i * e_i(N_i) ]^2
```

because the regional supports are disjoint and squared L2 errors add.

This is an identity under the same sampled norm, not a fitted regression.

## Optimization form

The adaptive representation problem can therefore be written as

```text
minimize    sum_i N_i

subject to  sum_i [ w_i * e_i(N_i) ]^2 <= epsilon_global^2
```

with the current experiment using

```text
epsilon_global = 0.5%
```

A separate peak guard remains for `N_i = 0` so a region with a small L2 contribution but a locally significant hotspot cannot be hidden.

For a zero-Gaussian region,

```text
e_i(0) = 1
```

so its L2 omission cost enters the global formula automatically as `w_i`. A separate arbitrary L2 omission threshold is therefore not mathematically necessary once the global budget is enforced. The peak guard remains an independent local-safety criterion.

## Numerical identity verification

The formula-predicted global error was compared against direct field reconstruction at every tested snapshot.

```text
time   selected A/B/C   total   predicted global   direct global    identity error
0.10      3 / 3 / 0       6       0.399286%         0.399286%       8.67e-19
0.20      5 / 2 / 1       8       0.312511%         0.312511%       1.08e-17
0.40      4 / 3 / 1       8       0.421441%         0.421441%       6.07e-18
0.60      2 / 3 / 2       7       0.391240%         0.391240%       2.60e-18
1.00      1 / 2 / 3       6       0.292464%         0.292464%       0
1.50      1 / 2 / 2       5       0.485023%         0.485023%       2.60e-18
```

Maximum predicted/direct discrepancy:

```text
1.084e-17
```

which is floating-point scale for this experiment.

## Allocation improvement

The earlier equal regional-percentage strategy required more primitives because every active region was forced to satisfy the same local relative-error threshold even when its global contribution was small.

The formula-based global-budget allocation instead gives a tested total range of:

```text
5 .. 8 Gaussians
```

Examples:

```text
0.20 s
old diagnostic zero-rule allocation : 5 / 2 / 8 = 15
formula allocation                  : 5 / 2 / 1 = 8

0.40 s
old validation-aware allocation     : 5 / 4 / 4 = 13
formula allocation                  : 4 / 3 / 1 = 8

0.60 s
old validation-aware allocation     : 2 / 4 / 5 = 11
formula allocation                  : 2 / 3 / 2 = 7

1.50 s
old validation-aware allocation     : 2 / 2 / 4 = 8
formula allocation                  : 1 / 2 / 2 = 5
```

This reduction does not relax the declared global representation-error limit. It removes the unnecessarily strict requirement that every region independently achieve the same percentage error.

## Important interpretation

The formula solves the **allocation-combination** part of the problem once regional error curves `e_i(N)` are known.

It does not yet eliminate the need to estimate or measure those curves. The current experiment still obtains `e_i(N)` by constrained sparse fitting for candidate counts.

The remaining formula research target is therefore narrower and clearer:

```text
Current reduced state S_i
        |
        v
state complexity metric C_i
        |
        v
predict regional error curve e_i(N)
        |
        v
exact global budget identity
        |
        v
minimum Gaussian allocation N_i
```

Checkpoint 11 identified normalized curvature as the strongest current single shape-complexity candidate. The next research problem is whether `e_i(N)` or its marginal improvement can be predicted from such state metrics without exhaustively fitting every candidate count.

## Responsibility boundary

This formula does not alter physical state, state evolution, material definitions, interfaces, or ThermoCore requirements. It only allocates downstream Gaussian representation complexity.

## Scope limit

The exact decomposition requires disjoint regional support under the declared L2 norm. The tested allocations remain empirical with respect to the current Gaussian dictionary and fitter. The identity itself does not claim that the fitted Gaussian mixtures are globally optimal, nor does it prescribe a universal peak threshold.
