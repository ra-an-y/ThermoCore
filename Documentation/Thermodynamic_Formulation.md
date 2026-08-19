# Thermodynamic Formulation

Version: 0.1
Status: Reference Formulation Specification — Non-Framework

---

## 1. Authority and Scope

This document defines one bounded thermodynamic reference formulation
for a ThermoCore implementation.

It is not part of the ThermoCore Framework Specification hierarchy.

It shall not redefine Framework architecture, ownership,
Thermodynamic State semantics, Material Representation semantics,
Framework Interface semantics, Extension boundaries, or Framework
Conformance.

An implementation may conform to the ThermoCore Framework without
using this reference formulation.

An implementation that declares use of this reference formulation
shall preserve the formulation semantics and constraints defined by
this document.

---

## 2. Framework Compatibility

This reference formulation is interpreted subject to the existing
ThermoCore Framework Specification, including:

- `Framework_Principles.md`
- `Core_Architecture.md`
- `Data_Flow.md`
- `Thermodynamic_State.md`
- `Material_Representation.md`
- `Framework_Interfaces.md`
- `Extension_Boundary.md`
- `Framework_Conformance.md`

Where this document is silent, it shall not be interpreted as creating
a new Framework requirement.

Where an apparent conflict exists, the Framework Specification remains
authoritative for Framework semantics.

---

## 3. Bounded Physical Scope

This reference formulation assumes:

- fixed cell geometry;
- fixed per-cell material mass;
- one constant material reference density across the modeled
  solid/liquid transition;
- no mass transport between cells;
- no shrinkage or expansion caused by phase-density differences;
- no moving mesh or free-surface evolution;
- no evolving runtime pressure state;
- no pressure-volume-work evolution;
- no mechanical-work state;
- nonreacting solid/liquid thermodynamics;
- an equilibrium-like, history-independent phase transition.

Variable-density phase change, compressible thermodynamics,
mass transport, deformation, physical mushy-zone formulations,
hysteresis, metastability, kinetic phase evolution, and chemical
reaction are outside this reference formulation.

---

## 4. Material Configuration

Material Definition shall provide the formulation parameters required
to interpret the persistent thermodynamic coordinate.

The applicable material parameters are:

- `rho_ref` — constant material reference density `[kg/m^3]`;
- `T_rho_ref` — reference condition associated with `rho_ref`;
- `T_E_ref` — thermodynamic energy reference temperature `[K]`;
- `T_m` — isothermal solid/liquid phase-change temperature `[K]`;
- `L` — latent heat of fusion `[J/kg]`;
- `c_s(T)` — solid sensible heat capacity `[J/(kg*K)]`;
- `c_l(T)` — liquid sensible heat capacity `[J/(kg*K)]`.

The formulation requires:

`rho_ref > 0`

`L > 0`

`c_s(T) > 0`

`c_l(T) > 0`

throughout the supported material-temperature range.

`T_rho_ref` and `T_E_ref` are semantically distinct reference
conditions. They may have the same numerical value when explicitly
selected by Material Definition.

Material Configuration shall not become Runtime State merely because
these quantities participate in thermodynamic computation.

---

## 5. Reference Density and Cell Mass

The same `rho_ref` shall apply throughout the modeled solid/liquid
transition.

For a fixed cell volume `V_cell`, the reference cell mass is:

`m_cell = rho_ref * V_cell`

`m_cell` remains constant under this reference formulation.

The use of one `rho_ref` does not assert that real solid and liquid
phases universally have equal physical density.

It is a bounded modeling assumption that excludes density-jump-induced
shrinkage, expansion, and material redistribution.

---

## 6. Persistent Thermodynamic State

The primary thermodynamic energy coordinate is specific enthalpy:

`h [J/kg]`

Specific enthalpy `h` is Persistent Thermodynamic State under this
reference formulation.

It is the maintained thermodynamic quantity used to preserve the
accumulated energy condition across state evolution.

Temperature and liquid Phase Fraction shall not be required as
independent Persistent State when they are recoverable according to
Sections 9 and 10.

An implementation may cache or buffer derived quantities without
changing their semantic classification.

---

## 7. Energy Reference Convention

The reference enthalpy convention is:

`h = 0 J/kg at T_E_ref`

All solid and liquid enthalpy relations shall use one compatible energy
datum.

External material datasets that use another enthalpy datum shall be
normalized before being combined with this formulation.

Independent solid-phase and liquid-phase reference offsets shall not
be introduced if they alter the latent-enthalpy difference.

The bounded formulation contains no evolving runtime pressure state.

Any constant pressure-volume contribution included by the selected
enthalpy definition shall be handled consistently within the common
enthalpy reference datum.

This rule shall not be interpreted as applying to variable-pressure
or compressible formulations.

---

## 8. Sensible Enthalpy

Let the solid sensible enthalpy relation be `h_s(T)`.

For any supported solid temperatures `T1` and `T2`:

`h_s(T2) - h_s(T1) = integral(T1 -> T2) c_s(theta) dtheta`

Define:

`h_s_star = h_s(T_m)`

The fully liquid enthalpy at completion of the isothermal phase
transition is:

`h_l_star = h_s_star + L`

For the liquid branch above `T_m`:

`h_l(T) = h_l_star + integral(T_m -> T) c_l(theta) dtheta`

Because the supported sensible heat-capacity functions shall remain
positive, both sensible enthalpy branches shall remain monotonically
increasing over their declared validity ranges.

---

## 9. Temperature Recovery

Temperature is Derived Thermodynamic State under this reference
formulation.

Temperature shall be recovered from persistent enthalpy according to:

if `h < h_s_star`:

`T = inverse_h_s(h)`

if `h_s_star <= h <= h_l_star`:

`T = T_m`

if `h > h_l_star`:

`T = inverse_h_l(h)`

where `inverse_h_s` and `inverse_h_l` are inverses of the applicable
monotonic sensible-enthalpy relations.

For every supported value of `h`, the formulation shall define one
thermodynamic Temperature.

The numerical algorithm used to perform sensible-branch inversion is
an implementation concern and shall not redefine this formulation.

---

## 10. Phase Fraction Recovery

Let `phi` denote liquid Phase Fraction.

`phi` is Derived Thermodynamic State under this reference formulation.

It shall be recovered from persistent enthalpy as follows:

if `h < h_s_star`:

`phi = 0`

if `h_s_star <= h <= h_l_star`:

`phi = (h - h_s_star) / L`

if `h > h_l_star`:

`phi = 1`

The recovered Phase Fraction shall satisfy:

`0 <= phi <= 1`

and:

`phi(h_s_star) = 0`

`phi(h_l_star) = 1`

This relation is history-independent.

A formulation requiring hysteresis, metastability, kinetic phase
evolution, or another history-dependent transition shall not use this
Derived-State rule without defining the additional Persistent State
required by that formulation.

---

## 11. Energy Input Mapping

Framework-level Energy Input does not acquire universal physical units
from this reference formulation.

Before an Energy Input modifies persistent specific enthalpy, the
implementation of this formulation shall map the supplied physical
quantity into an applicable cell energy increment.

For total cell energy `Delta E [J]`:

`Delta h = Delta E / m_cell`

For total heat rate `P [W]` applied during timestep `Delta t`:

`Delta h = P * Delta t / m_cell`

For boundary heat flux `q_A [W/m^2]` applied over affected face area
`A`:

`Delta h = q_A * A * Delta t / m_cell`

For volumetric source `Q_v [W/m^3]`:

`Delta h = Q_v * V_cell * Delta t / m_cell`

and therefore, under constant `rho_ref`:

`Delta h = Q_v * Delta t / rho_ref`

If one Energy Input is distributed across multiple cells, the
distribution rule shall be explicit.

Equal-per-cell, equal-per-volume, equal-per-mass, and spatially
weighted distributions shall not be treated as equivalent unless they
produce the same declared physical input.

---

## 12. State Evolution

Thermodynamic Computation remains the only Framework Core
responsibility permitted to evolve or write Thermodynamic State.

For this reference formulation, accepted energy input modifies the
persistent specific enthalpy coordinate.

Conceptually:

`h_(n+1) = h_n + Delta h`

when no other applicable energy-transfer contribution is present.

A later implementation may combine multiple valid energy contributions
within one update, provided the resulting update preserves the same
specific-enthalpy semantics and applicable conservation requirements.

This document does not define execution scheduling, synchronization,
time-integration algorithm, numerical discretization, or backend
pipeline.

---

## 13. Numerical Regularization

The physical reference phase transition defined by this document is
isothermal at `T_m`.

No finite numerical transition-temperature width is part of the
physical reference formulation.

An implementation may introduce smoothing or numerical regularization
when necessary for numerical behavior.

Such regularization:

- shall remain an implementation approximation;
- shall not redefine `T_m`;
- shall not redefine `L`;
- shall not be presented as a measured physical solidus/liquidus
  interval without independent physical evidence;
- shall be evaluated through Verification.

A material with a physically supported finite solidus/liquidus interval
belongs to a different formulation profile.

---

## 14. Formulation Invariants

An implementation declaring use of this reference formulation shall
preserve the following formulation invariants within its declared
supported material range:

`rho_ref > 0`

`L > 0`

`c_s(T) > 0`

`c_l(T) > 0`

`h_l_star - h_s_star = L`

`0 <= phi <= 1`

For latent states:

`h_s_star <= h <= h_l_star`

`T = T_m`

`phi = (h - h_s_star) / L`

For phase boundaries:

`phi(h_s_star) = 0`

`phi(h_l_star) = 1`

A common enthalpy-reference shift shall not alter energy differences
or latent interval width.

---

## 15. Verification and Validation Boundary

This specification defines formulation semantics and invariants.

It does not claim that an implementation has verified or validated
them.

Later Verification shall evaluate at least:

- specific-enthalpy update consistency;
- `h -> T` recovery;
- `h -> phi` recovery;
- Phase Fraction bounds;
- latent interval width;
- sensible-branch monotonicity over supported material ranges;
- Energy Input dimensional mapping;
- enthalpy-reference normalization invariance;
- latent-energy conservation;
- effects of any numerical regularization.

Later Validation shall compare the implemented formulation against
representative physical or benchmark cases appropriate to the bounded
scope.

Performance evaluation remains separate from both formulation
semantics and physical Validation.

---

## 16. Relationship to Other Formulations

This document defines one ThermoCore reference formulation.

It shall not be interpreted as requiring all conforming ThermoCore
implementations to use:

- enthalpy rather than internal energy;
- specific rather than volumetric energy;
- equal-density phase change;
- an isothermal phase transition;
- Temperature as Derived State;
- Phase Fraction as Derived State.

Other formulations remain permissible when the implementation
continues to satisfy applicable ThermoCore Framework requirements.

Such formulations shall define their own physical quantity semantics,
state classification, conservation assumptions, and applicable
reference conditions without redefining Framework architecture or
ownership.

---

## 17. Document Status

This document is an authoritative specification for the bounded
reference formulation defined herein.

It is not a ThermoCore Framework Specification.

It does not create a new Framework Core component, Framework Interface
semantic, Framework Conformance category, or universal Thermodynamic
State variable.

Its authority is limited to implementations that explicitly declare
use of this reference formulation.

The research basis for this document is recorded in:

- `Research/00_Literature_Survey/`
- `Research/01_Evidence_Matrix/Thermodynamic_Formulation_Evidence_Matrix.md`
- `Research/04_Research_Gap/Thermodynamic_Formulation_Research_Gap_Analysis.md`

Implementation, Verification, Validation, and Performance Evaluation
remain subsequent stages.
