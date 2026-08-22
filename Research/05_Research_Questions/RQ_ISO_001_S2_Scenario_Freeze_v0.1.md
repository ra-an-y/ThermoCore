# RQ-ISO-001 S2 Scenario Freeze v0.1

Status: Frozen Pre-execution Scenario Definition  
Research Question: RQ-ISO-001  
Scenario: S2 — Thermal Hysteresis Material-Response Extension  
Tracking: GitHub Issue #75  
Frozen comparison baseline: `8e3a948b0f36feefd313de1f03dd4db29b3bc465`

---

## 1. Purpose

This artifact freezes the S2 scenario before executable measurement.

S2 is intentionally bounded as a material-response extension. It does not change the reference formulation's persistent specific-enthalpy coordinate, energy evolution, mass model, pressure model, or equilibrium `h -> T` / `h -> phi` recovery.

The scenario tests only whether a path-dependent response history quantity remains extension-owned under Condition R or becomes part of shared authoritative state under Condition P.

No hypothesis result is produced by this document.

---

## 2. Frozen History Quantity

S2 requires exactly one persistent history quantity:

```text
HysteresisMode : byte

0 = SolidLike
1 = LiquidLike
```

`HysteresisMode` is a material-response latch used only by the S2 extension.

It is not the reference formulation's equilibrium liquid Phase Fraction and shall not replace or redefine Derived Thermodynamic State.

Semantic payload size for the primary state metrics is fixed at **1 byte per element**.

Implementation padding, alignment, container overhead, and backend packing are excluded from the primary semantic-payload metric and may be reported separately.

---

## 3. Frozen Hysteresis Rule

The same rule shall be executed in both architecture conditions.

Let:

- `T_low = 295 K`
- `T_high = 305 K`
- `T_low < T_high`

The update rule is:

```text
if mode == SolidLike and T >= T_high:
    mode := LiquidLike
else if mode == LiquidLike and T <= T_low:
    mode := SolidLike
else:
    mode := mode
```

The initial state is:

```text
HysteresisMode = SolidLike
```

This rule creates a dead band in which the response depends on prior history.

The rule is a bounded research mechanism. It is not presented as a universal physical hysteresis law or as a measured property of a particular material.

---

## 4. Frozen Input Sequence

Both conditions shall execute the same recovered-temperature sequence:

```text
294 K
299 K
304 K
306 K
302 K
297 K
294 K
299 K
```

The corresponding expected mode sequence, starting from `SolidLike`, is:

```text
SolidLike
SolidLike
SolidLike
LiquidLike
LiquidLike
LiquidLike
SolidLike
SolidLike
```

The test harness shall construct thermodynamic states using the existing reference formulation/material parameters and shall confirm that both conditions recover equivalent temperatures before applying the hysteresis rule.

---

## 5. Condition R Placement

Condition R shall retain:

```text
Core Persistent Thermodynamic State:
- SpecificEnthalpy : double  [8 semantic bytes]

S2 extension-owned persistent state:
- HysteresisMode : byte      [1 semantic byte]
```

Therefore the predeclared S2 state metrics for Condition R are:

- `M-S1 = 1`
- `M-S2 = 8`
- `M-S3 = 0`
- `M-S4 = 1`
- `M-S5 = 9`

No frozen Core semantic artifact, Core implementation artifact, or generic Core interface is permitted to acquire S2-specific semantics in Condition R.

---

## 6. Condition P Placement

Condition P shall remain modular but promote the same quantity into shared authoritative state:

```text
Shared authoritative Simulation/Core State:
- SpecificEnthalpy : double  [8 semantic bytes]
- HysteresisMode   : byte    [1 semantic byte]

Extension-local persistent state:
- none
```

Therefore the predeclared S2 state metrics for Condition P are:

- `M-S1 = 2`
- `M-S2 = 9`
- `M-S3 = 1`
- `M-S4 = 0`
- `M-S5 = 9`

Condition P shall not be made monolithic. Hysteresis computation remains a separate module. The controlled difference is only the authority/membership placement of `HysteresisMode`.

---

## 7. Predeclared Architectural Impact Interpretation

The repository harness itself remains research-only and shall not be counted as production ThermoCore Core modification.

For the controlled architecture comparison, adding S2 under Condition P necessarily changes the active shared authoritative state schema relative to P-S0. The experiment shall therefore inspect whether the following logical Core impacts occur:

- shared state semantic schema growth;
- shared state implementation schema growth;
- shared state access/interface exposure of `HysteresisMode`; and
- a direct shared-state dependency on the S2-specific semantic quantity.

Condition P's general policy allowing schema growth is already frozen, so merely exercising that policy shall not be counted as changing the policy itself.

Condition R shall be audited for hidden displacement into generic wrappers, adapters, containers, type checks, synchronization obligations, or duplicated authoritative state.

---

## 8. Evidence-Impact Rule for S2

The Phase A dependency rules remain authoritative for this experiment.

Expected classifications are not assumed as results, but the following decision logic is frozen:

- extension-only S2 changes under Condition R may retain existing Core evidence if no frozen semantic/executable dependency changes;
- a Condition P persistent shared-state schema change requires Core Verification impact review under D2;
- H2O/Gallium caloric Validation shall be re-executed only if the executed thermodynamic recovery/formulation dependency changes, not merely because `HysteresisMode` exists;
- both conditions require new S2-specific evidence for the hysteresis claim.

---

## 9. Reclassification Stop Rule

Execution shall stop before accepting S2 measurements if review shows that `HysteresisMode` is semantically required to be authoritative Thermodynamic State for this bounded scenario rather than extension-owned material-response history.

In that case S2 must be reclassified rather than forced to support the candidate property.

No post-measurement redefinition of the history quantity, payload size, thresholds, state placement, or expected sequence is permitted within v0.1.
