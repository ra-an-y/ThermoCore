# Checkpoint 09 — Time-Adaptive Gaussian Budget

Status: **PASS — bounded 1D experiment**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**

## Question

Does the minimum useful downstream Gaussian representation remain fixed during diffusion, or should the Gaussian budget depend on the current physical state?

This checkpoint repeats the constrained sparse representation and independent finite-volume validation at multiple times during the same heterogeneous A-B-C diffusion process.

The reduced current state remains unchanged and authoritative. Only the downstream Gaussian representation budget is allowed to change.

## Method

The experiment uses the same A-B-C materials, geometry, initial Gaussian field, 32-mode regional reduced state, perfect-contact coupling, and independent heterogeneous finite-volume reference used by Checkpoints 5–8.

Snapshots are evaluated at:

```text
0.10 s
0.20 s
0.40 s
0.60 s
1.00 s
1.50 s
```

At each snapshot:

1. the current reduced state is compared directly against the finite-volume reference;
2. each region receives a constrained sparse Gaussian fit from one through eight retained terms;
3. the first per-region representation count satisfying the declared `0.5%` regional relative-error threshold is recorded;
4. bounded A/B/C count combinations are searched for the smallest total allocation satisfying both:

```text
every represented region error vs current state <= 0.5%
global Gaussian representation error vs heterogeneous FV <= 0.5%
```

The reduced-state-vs-FV error is explicitly recorded as a validation floor. If the reduced state itself exceeds the `0.5%` independent-reference threshold, failure to find a Gaussian allocation is not interpreted as a Gaussian-compression failure.

A regional local count of `0` in the table means the declared relative-error criterion was not reached within the bounded one-to-eight-term search. In the earliest snapshots this occurs in the almost-unexcited downstream region, where a relative regional metric is also poorly conditioned. This checkpoint does not yet introduce a separate absolute-negligibility rule or zero-Gaussian representation rule.

## Results

```text
time   state vs FV   local A/B/C   validated A/B/C   total   validated vs FV   max region
0.10   0.913965%     3/3/0         none              -       n/a               n/a
0.20   0.644726%     5/2/0         none              -       n/a               n/a
0.40   0.418593%     4/4/4         5/4/4             13      0.442957%         0.493759%
0.60   0.341684%     1/4/5         2/4/5             11      0.438940%         0.449468%
1.00   0.334180%     1/2/3         1/2/3              6      0.438413%         0.386118%
1.50   0.371017%     1/2/4         2/2/4              8      0.397941%         0.420149%
```

For snapshots where the declared independent validation threshold is reachable, the validation-aware total Gaussian budget spans:

```text
minimum : 6 terms
maximum : 13 terms
```

The allocation changes over time.

## Interpretation

The experiment rejects a simple fixed-budget interpretation for this bounded case.

The useful representation complexity is state-dependent:

```text
0.40 s : 5/4/4 -> 13 terms
0.60 s : 2/4/5 -> 11 terms
1.00 s : 1/2/3 ->  6 terms
1.50 s : 2/2/4 ->  8 terms
```

The budget decreases strongly as early high-frequency structure diffuses, but it does not decrease monotonically. Between 1.00 s and 1.50 s the minimum validated count rises from 6 to 8 as heat redistribution across heterogeneous regions changes which regional field shapes must be represented.

Therefore the supported bounded conclusion is:

> Gaussian representation budget should be treated as a function of the current field/state and validation criterion, not as a universal constant and not as a quantity guaranteed to decrease monotonically with elapsed diffusion time.

The early 0.10 s and 0.20 s snapshots also expose a separate limitation. Their reduced-state-vs-FV errors are approximately `0.914%` and `0.645%`, already above the declared `0.5%` validation threshold. Increasing Gaussian count cannot remove an error floor that originates upstream in the reduced physical model.

This reinforces the responsibility split:

```text
Reduced current state accuracy
        -> physical-model / state-resolution question

Gaussian budget
        -> downstream representation-compression question
```

A representation study must not claim failure or success against an independent PDE reference without first checking the upstream state-model error floor.

## Responsibility Boundary

```text
Current Reduced State(t)
        |
        v
State-dependent Gaussian budget selection
        |
        v
Downstream Gaussian Representation(t)
```

Changing the Gaussian count does not alter material definitions, state ownership, state evolution, or ThermoCore Framework requirements.

## Scope Limit

These results remain empirical for the declared 1D A-B-C case, bounded Gaussian candidate dictionary, greedy constrained sparse selection, 32-mode reduced state, selected snapshot times, and `0.5%` thresholds.

They do not establish a universal budget schedule, monotonic complexity law, globally optimal Gaussian mixture, zero-Gaussian rule for negligible regions, or arbitrary multilayer behavior.
