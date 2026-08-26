# Checkpoint 08 — Adaptive Per-Region Gaussian Budget

Status: **PASS — bounded 1D experiment**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Can the downstream Gaussian representation use a different retained Gaussian count in each material region, instead of forcing one uniform count, while preserving the declared `0.5%` representation threshold and independent finite-volume validation threshold?

The reduced current state remains unchanged. Gaussian count adaptation affects only downstream field representation.

## Method

The final state is the same 0.6 s heterogeneous A-B-C case used by Checkpoints 5–7.

For each region independently, constrained sparse Gaussian fitting starts at one term and increases through at most eight terms. At every count, selected amplitudes are re-solved with the regional integral equality constraint:

```text
integral(Gaussian mixture)
=
integral(current reduced-state field)
```

Two minima are distinguished.

### Representation-local minimum

Choose the first count in each region whose own relative L2 error against that region's reduced-state field is `<= 0.5%`.

### Validation-aware minimum

Starting from those regional minima, search all bounded count combinations through eight terms per region and choose the smallest total Gaussian count that satisfies both:

```text
every-region error vs current state <= 0.5%
global error vs heterogeneous finite-volume reference <= 0.5%
```

This prevents local representation criteria from being mistaken for independent PDE-level validation.

## Results

### Representation-local allocation

```text
Region A : 1 Gaussian
Region B : 4 Gaussians
Region C : 5 Gaussians
Total    : 10 Gaussians
```

Measured result:

```text
global relative error vs reduced state : 4.12514828e-3
relative error vs heterogeneous FV     : 5.53772527e-3
```

All three regions individually satisfy the declared `0.5%` representation threshold, but the combined finite-volume comparison is approximately `0.554%` and therefore does **not** satisfy the independent `0.5%` threshold.

### Validation-aware allocation

The smallest bounded allocation satisfying both criteria is:

```text
Region A : 2 Gaussians
Region B : 4 Gaussians
Region C : 5 Gaussians
Total    : 11 Gaussians
```

Measured result:

```text
global relative error vs reduced state : 2.71525901e-3
maximum region error vs reduced state  : 4.49467842e-3
relative error vs heterogeneous FV     : 4.38939609e-3
maximum regional integral error        : 1.38777878e-17
```

The validation-aware representation therefore satisfies the declared `0.5%` per-region state threshold and the independent `0.5%` finite-volume threshold.

## Interpretation

Uniform five-Gaussian representation from Checkpoint 7 requires:

```text
5 + 5 + 5 = 15 Gaussians
```

The validation-aware adaptive allocation requires:

```text
2 + 4 + 5 = 11 Gaussians
```

For this declared case, adaptive regional allocation reduces the retained representation count from 15 to 11, a reduction of approximately `26.7%`, while keeping the declared validation thresholds.

The representation-local result is also important: satisfying every region's representation error independently is **not sufficient** to guarantee the same threshold against an independent PDE reference. The independent validation criterion changes the minimum from 10 to 11 terms.

These counts are empirical minima for this bounded state, candidate dictionary, greedy sparse selection procedure, and declared thresholds. They are not proofs of globally optimal Gaussian mixtures or general multilayer requirements.

## Responsibility Boundary

This checkpoint does not alter physical-state ownership:

```text
Current Reduced State
        |
        v
Adaptive Gaussian budget selection
        |
        v
Downstream Gaussian field representation
```

The Gaussian budget may adapt to representation complexity. It does not write the reduced current state, alter material definitions, or modify ThermoCore Framework requirements.
