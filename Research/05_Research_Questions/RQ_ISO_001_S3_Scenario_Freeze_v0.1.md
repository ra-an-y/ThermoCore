# RQ-ISO-001 S3 Scenario Freeze v0.1

Status: Frozen Pre-execution Scenario Definition  
Research Question: RQ-ISO-001  
Scenario: S3 — Bounded Exothermic Reaction-Heat Extension  
Tracking: GitHub Issue #77  
Frozen comparison baseline: `8e3a948b0f36feefd313de1f03dd4db29b3bc465`

---

## 1. Purpose

This artifact freezes the S3 scenario before executable measurement.

S3 is intentionally bounded as an ordinary stateful extension with stronger thermodynamic coupling than S2. A reaction-progress quantity is persistent, but total element mass remains fixed and the mechanism introduces no species transport, pressure evolution, flow, variable-mass state, or revised equilibrium thermodynamic closure.

The only coupling into Thermodynamic Computation is an additive specific-enthalpy source contribution generated from the reaction-progress update and applied through the existing energy-update responsibility.

No hypothesis result is produced by this document.

---

## 2. Frozen Persistent Quantity

S3 requires exactly one extension-specific persistent quantity:

```text
xi : double
range: [0, 1]
initial value: 0
```

`xi` is a bounded reaction-progress coordinate for this research scenario only.

It does not represent species concentrations, mass fractions, density, pressure, velocity, or a universal chemical-state model.

Semantic payload size for the primary state metrics is fixed at **8 bytes per element**.

---

## 3. Frozen Material and Thermodynamic Baseline

Both conditions use the same existing bounded reference formulation and the same test material parameters:

```text
reference density             = 1000 kg/m^3
density reference temperature = 293.15 K
energy reference temperature  = 273.15 K
melting temperature            = 300 K
latent heat                    = 200000 J/kg
solid heat capacity            = 2000 J/(kg*K)
liquid heat capacity           = 4000 J/(kg*K)
```

Initial thermodynamic state corresponds to:

```text
T_initial = 298 K
```

on the solid sensible branch.

The reference formulation's authoritative persistent thermodynamic coordinate remains:

```text
SpecificEnthalpy : double
```

---

## 4. Frozen Reaction Rule

The same reaction module shall be executed in both architecture conditions.

Frozen constants:

```text
activation temperature T_act = 300 K
maximum progress increment    = 0.25 per step
total specific reaction heat  = 80000 J/kg for xi: 0 -> 1
```

For each step, after the external specific-enthalpy input is applied, recover Temperature using the existing reference formulation and evaluate:

```text
if T >= T_act and xi < 1:
    delta_xi := min(0.25, 1 - xi)
else:
    delta_xi := 0

xi_next := xi + delta_xi
delta_h_reaction := 80000 * delta_xi   [J/kg]
```

The reaction contribution is then applied as a positive additive specific-enthalpy increment using the existing Thermodynamic Computation energy-update responsibility.

The rule is deterministic and deliberately simplified. It is not claimed as a kinetic model for a particular chemical system.

---

## 5. Frozen External-Energy Sequence

Both conditions shall execute the same external specific-enthalpy input sequence:

```text
step 1: +2000 J/kg
step 2: +3000 J/kg
step 3:     0 J/kg
step 4:     0 J/kg
step 5:     0 J/kg
step 6:     0 J/kg
```

This schedule raises the material from 298 K to 299 K on step 1 and into the 300 K phase-change plateau on step 2, after which the bounded reaction may proceed.

The frozen expected reaction-progress sequence after each full step is:

```text
0.00
0.25
0.50
0.75
1.00
1.00
```

The frozen expected reaction specific-heat contributions are:

```text
0
20000
20000
20000
20000
0 J/kg
```

The expected final specific-enthalpy sequence is:

```text
51700
74700
94700
114700
134700
134700 J/kg
```

The expected final recovered Temperature sequence is:

```text
299
300
300
300
300
300 K
```

The expected final liquid-phase-fraction sequence is:

```text
0.000
0.105
0.205
0.305
0.405
0.405
```

Floating-point comparisons may use a fixed numerical tolerance; the physical rule, state placement, and expected values shall not be changed after execution begins.

---

## 6. Condition R Placement

Condition R shall retain:

```text
Core Persistent Thermodynamic State:
- SpecificEnthalpy : double  [8 semantic bytes]

S3 extension-owned persistent state:
- xi               : double  [8 semantic bytes]
```

Therefore the predeclared S3 state metrics for Condition R are:

- `M-S1 = 1`
- `M-S2 = 8`
- `M-S3 = 0`
- `M-S4 = 8`
- `M-S5 = 16`

The S3 extension may read recovered Temperature, update only its own `xi`, and supply `delta_h_reaction` to the existing energy-update path.

No frozen Core semantic artifact, Core implementation artifact, or generic Core interface is permitted to acquire S3-specific state semantics in Condition R.

---

## 7. Condition P Placement

Condition P shall remain modular but promote the same reaction-progress quantity into shared authoritative state:

```text
Shared authoritative Simulation/Core State:
- SpecificEnthalpy : double  [8 semantic bytes]
- xi               : double  [8 semantic bytes]

Extension-local persistent state:
- none
```

Therefore the predeclared S3 state metrics for Condition P are:

- `M-S1 = 2`
- `M-S2 = 16`
- `M-S3 = 1`
- `M-S4 = 0`
- `M-S5 = 16`

Reaction computation remains a separate module. The controlled difference is the authority/membership placement of `xi`, not modularity or reaction functionality.

---

## 8. Fixed-Mass / No-Transport Boundary

S3 remains an ordinary extension only under the following frozen restrictions:

- total element mass is fixed;
- no species mass fractions are represented;
- no species transport is represented;
- no pressure state or pressure evolution is introduced;
- no velocity, momentum, or flow field is introduced;
- no density evolution is introduced;
- no chemical-equilibrium closure is added to the Core formulation; and
- reaction heat enters only as the declared additive specific-energy contribution.

If correct execution is found to require any of those excluded governing quantities, S3 must be reclassified before its measurements are accepted.

---

## 9. Predeclared Architectural Impact Interpretation

The repository harness itself remains research-only and shall not be counted as production ThermoCore Core modification.

For the controlled architecture comparison, Condition P necessarily grows the active shared authoritative state schema to include `xi`. The experiment shall inspect the same logical impact categories used for S2:

- shared state semantic schema growth;
- shared state implementation schema growth;
- shared state access/interface expansion exposing `xi`; and
- a direct shared-state dependency on the S3-specific semantic quantity.

Condition P's already-frozen policy allowing active shared-state growth is not itself counted as a newly changed normative policy.

Condition R shall be audited for hidden displacement into generic wrappers, adapters, containers, type checks, duplicated authoritative state, scenario-specific synchronization obligations, or Core-to-S3 dependencies.

---

## 10. Evidence-Impact Rule for S3

The Phase A dependency rules and the S2 counting method remain fixed.

The following classification logic is frozen before S3 execution:

- if Condition R uses the unchanged thermodynamic recovery and energy-update paths and adds only extension-owned `xi` plus a supplied energy increment, existing Core Verification and caloric Validation may remain retained;
- both conditions require new S3-specific evidence for reaction-progress and source-coupling behavior;
- a Condition P shared persistent-state schema change requires Core Verification impact review and re-execution of the applicable state/schema layer;
- H2O/Gallium caloric Validation shall be re-executed only if their formulation/recovery dependency changes;
- if shared authoritative state membership changes without changing caloric recovery numerics, H2O/Gallium applicability may be classified `ReviewOnly`, as in S2, rather than `Revalidate`.

No evidence category shall be changed merely to obtain a preferred H-ISO-03 result.

---

## 11. Cross-Scenario Decision Rule After S3

After valid S3 execution, the pre-registered S1-S3 rules shall be applied directly:

- H-ISO-01 may be `SUPPORTED FOR EVALUATED SCENARIOS` only if both S2 and S3 show fewer extension-specific quantities promoted into mandatory Core State under R than P while equivalent functionality is preserved and total persistent information is reported.
- H-ISO-02 may be `SUPPORTED FOR EVALUATED SCENARIOS` only if both S2 and S3 show a valid strict-subset Core impact under R and the hidden-coupling audits remain clean.
- H-ISO-03 may be `SUPPORTED FOR EVALUATED SCENARIOS` only if both S2 and S3 show a strict-subset justified Core Verification and/or Validation re-execution set under the same dependency rules.

S4 remains a separate boundary-validity counterexample. S4 may force reclassification of the candidate architecture property if the restricted boundary hides genuinely governing Core changes, but it shall not be used to manufacture support for the ordinary-extension S1-S3 results.

---

## 12. Reclassification Stop Rule

Execution shall stop before accepting S3 measurements if review shows that the frozen bounded mechanism cannot be represented correctly without adding species, mass, pressure, density, momentum, or another governing quantity to authoritative Core State.

In that case S3 must be reclassified rather than forced to fit the ordinary-extension hypothesis.

No post-measurement redefinition of `xi`, payload size, reaction constants, input schedule, state placement, expected sequence, or evidence rules is permitted within v0.1.
