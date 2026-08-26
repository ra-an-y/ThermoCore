# Checkpoint 10 — Negligible Region / Zero-Gaussian Rule

Status: **PASS — bounded 1D experiment**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Can a material region temporarily receive zero downstream Gaussian primitives when its current field contribution is negligible, without deleting or disabling its physical state?

The physical reduced state remains present and continues to evolve. A zero Gaussian budget means only that the downstream representation omits the region for the current snapshot.

## Rule

A region is eligible for a zero Gaussian representation only if both omission guards are satisfied:

```text
region L2 contribution / global L2 norm <= 0.1%
region absolute peak / global absolute peak <= 0.5%
```

The L2 guard bounds the global field contribution of omitting the region. The peak guard prevents a region with small total contribution but a locally significant hotspot from being discarded.

This rule is deliberately stricter than a regional-energy-only criterion.

## Results

The rule was evaluated at early snapshots where Checkpoint 9 exposed poor conditioning of purely regional relative error in the almost-unexcited downstream region.

```text
time   L2 contribution A/B/C            peak contribution A/B/C          selected A/B/C   total   global error vs state
0.10   87.99% / 47.51% / 0.00954%       100% / 96.93% / 0.0274%           3 / 3 / 0        6       0.3951%
0.20   85.06% / 52.58% / 0.2994%        100% / 98.82% / 1.031%            5 / 2 / 8       15       0.3138%
0.40   84.62% / 53.23% / 2.442%         100% / 98.86% / 7.583%            4 / 4 / 4       12       0.3285%
```

The regional L2 contribution values are component norms normalized by the global norm; because they are norm components rather than scalar shares, their arithmetic sum is not expected to equal 100%.

## Interpretation

At `0.10 s`, Region C satisfies both omission guards:

```text
L2 contribution   = 0.00954%  <= 0.1%
peak contribution = 0.0274%   <= 0.5%
```

It is therefore represented with zero Gaussians while its physical reduced state remains active. The total downstream representation uses six Gaussians (`3/3/0`) and remains below the declared `0.5%` global representation-error threshold.

At `0.20 s`, Region C is no longer eligible for omission:

```text
L2 contribution   = 0.2994%  > 0.1%
peak contribution = 1.031%   > 0.5%
```

The peak guard is especially important here: Region C is still small globally, but its local maximum is no longer negligible relative to the global field maximum. A zero-Gaussian representation would therefore be rejected rather than hiding that emerging local structure.

At `0.40 s`, Region C is clearly active and both contribution measures are well above the omission thresholds. No region receives a zero budget.

## Important Limitation Exposed

The `0.20 s` result also exposes a separate issue. Region C is too important to omit under the declared guards, but its own regional relative-error metric remains poorly conditioned and does not reach the local `0.5%` target within the bounded one-to-eight Gaussian dictionary. The diagnostic fallback therefore assigns eight Gaussians.

This should **not** be interpreted as evidence that eight Gaussians are physically necessary for Region C. It shows that a purely regional relative-error rule is a poor allocator when a region is small-but-non-negligible.

The next allocation problem should therefore use a global error budget rather than requiring every represented region to satisfy the same relative regional percentage.

Conceptually:

```text
Current Reduced State
        |
        +--> negligible under global + peak guards --> 0 Gaussian
        |
        +--> represented region
                 |
                 v
          global-error-budget allocation
```

## Responsibility Boundary

`0 Gaussian` does **not** mean:

- zero physical state;
- zero material;
- skipped state evolution;
- deleted energy;
- inactive interfaces.

It means only:

> no downstream Gaussian primitive is allocated to this region at the current representation snapshot.

State evolution remains independent of representation budget.

## Scope Limit

The `0.1%` L2 omission guard and `0.5%` peak guard are experiment-level thresholds chosen for this bounded study. They are not ThermoCore requirements and are not claimed to be universally optimal.
