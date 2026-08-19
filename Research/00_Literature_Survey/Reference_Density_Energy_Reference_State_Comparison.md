# Reference Density and Energy Reference-State Normalization — Primary-Source Comparison

Status: Primary-Source Comparison  
Scope: Research only — non-normative  
Target: Reference-density convention and reference-state normalization gaps from `Fixed_Grid_Density_Mass_Volume_Phase_Change_Comparison.md`

---

## 1. Research Question

For a deliberately bounded fixed-grid thermal/phase-change reference formulation, how should a single reference density be defined, and how should thermodynamic energy reference temperature and energy offsets be normalized so that material datasets, energy evolution, temperature recovery, and latent-heat relations remain internally consistent?

This comparison does **not** modify the ThermoCore Framework Specification. It does not freeze enthalpy versus internal energy, specific versus volumetric storage, or any universal material reference state.

## 2. Source Set

This comparison uses sources that directly expose reference-density or thermodynamic reference-state semantics:

1. COMSOL Multiphysics 6.4 Heat Transfer documentation:
   - *Material Density in Features Defined in the Material Frame*;
   - *Phase Change Material*;
   - *Apparent Heat Capacity Method*;
   - *Settings for the Heat Transfer Interface*.
2. OpenFOAM official thermophysical/source documentation:
   - `solidificationMeltingSource.H`;
   - `hConstThermo.H` / `hConstThermoI.H`;
   - `eConstThermo.H`.
3. Ansys Fluent Theory/User documentation:
   - *Heat Transfer Theory*;
   - *Setting the Reference Temperature for Enthalpy*;
   - *Solidification and Melting — Energy Equation*.
4. Ansys CFX-Solver Theory documentation:
   - *Thermal Phase Change Model*.

Direct source statements are separated from dimensional or formulation implications derived from them.

## 3. Reference Density Is a Reference-State Quantity, Not Merely a Number

### 3.1 COMSOL material-frame density belongs to a reference geometry

COMSOL states that features defined in the material frame expect density defined for the **reference geometry**. It further notes that this density is constant in most cases and that a nonconstant material-frame density implies addition or removal of matter.

When a material-library density is temperature-dependent, COMSOL evaluates the reference-geometry density at a constant **volume reference temperature**.

**Evidence-supported conclusion:** In an undeformed material-frame model, reference density is tied to a declared reference configuration. A temperature-dependent physical density law cannot simply be substituted into a fixed material-frame density slot without changing the mass meaning of the model.

Source:

- COMSOL Multiphysics 6.4, *Material Density in Features Defined in the Material Frame*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_modeling.06.25.html

### 3.2 COMSOL phase-change material uses one density across phases in the solid material frame

COMSOL's phase-change documentation states that, when the phase-change model is used under a Solid node, a single density should be defined for all phases to ensure mass conservation on the material frame.

**Evidence-supported conclusion:** For the bounded no-deformation regime identified in the previous ThermoCore research step, one density across solid and liquid phases is directly supported as a conservative simplification.

Sources:

- COMSOL Multiphysics 6.4, *Phase Change Material*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_ht_features.09.036.html
- COMSOL Multiphysics 6.4, *Apparent Heat Capacity Method*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.024.html

### 3.3 OpenFOAM `rhoRef` is explicitly a reference density and is typically the solid density

OpenFOAM `solidificationMeltingSource` defines:

```text
rhoRef  Reference (solid) density
```

and the source declares the internal member as a reference density that is **typically the solid density**.

**Evidence-supported conclusion:** A practical fixed-grid phase-change source model may choose solid density as its reference-density convention.

**Boundary on interpretation:** OpenFOAM's convention does not establish that solid density is the universally correct reference density for every equal-density phase-change formulation. It is evidence for one established implementation convention.

Source:

- OpenFOAM official source, `solidificationMeltingSource.H`:  
  https://api.openfoam.com/2506/solidificationMeltingSource_8H_source.html

## 4. Candidate Reference-Density Semantics for a Minimal Bounded Formulation

The source set supports the following formulation-level candidate semantics:

```text
rho_ref = one constant material reference density [kg/m^3]
```

with all of the following declared explicitly:

```text
reference geometry: fixed cell geometry
phase scope: same rho_ref used across solid/liquid transition
mass scope: per-cell mass fixed
transport scope: no mass transport
mechanical scope: no shrinkage/expansion or deformation
reference condition: density corresponds to a declared density-reference temperature T_rho_ref
```

For a cell of fixed volume:

```text
m_cell = rho_ref * V_cell
```

then remains constant throughout the bounded phase transition.

**Analytical implication:** This convention makes the equal-density assumption operationally precise without claiming that the real solid and liquid densities are equal.

### 4.1 Which numerical value should `rho_ref` use?

The current evidence supports at least two legitimate conventions:

1. a reference-geometry density evaluated at a declared volume reference temperature, as in COMSOL's material-frame treatment;
2. a reference density chosen as the solid density, as used by OpenFOAM's `solidificationMeltingSource` convention.

The present source set does **not** establish a universal rule requiring one of these choices for ThermoCore.

**Current research position:** `rho_ref` must have an explicit provenance and reference condition, but the exact convention is not yet frozen.

## 5. Thermodynamic Energy Requires an Explicit Reference Convention

### 5.1 OpenFOAM stores reference temperature and reference sensible-energy offsets explicitly

OpenFOAM `hConstThermo` contains:

```text
Tref    reference temperature
Hsref   reference sensible enthalpy
Hf      heat of formation
```

and evaluates sensible enthalpy in the constant-`Cp` model as:

```text
Hs = Cp * (T - Tref) + Hsref + equation-of-state contribution
```

Absolute enthalpy is then formed by adding the chemical/formation contribution.

OpenFOAM `eConstThermo` similarly contains a reference temperature and a reference sensible internal-energy offset.

**Evidence-supported conclusion:** Reference temperature and reference energy offset are explicit parts of an established thermophysical representation. Enthalpy and internal-energy formulations both require a consistent energy datum.

Sources:

- OpenFOAM official source, `hConstThermo.H`:  
  https://api.openfoam.com/2212/hConstThermo_8H_source.html
- OpenFOAM official source, `hConstThermoI.H`:  
  https://api.openfoam.com/2512/hConstThermoI_8H_source.html
- OpenFOAM official source, `eConstThermo.H`:  
  https://api.openfoam.com/2512/eConstThermo_8H_source.html

### 5.2 Fluent phase-change enthalpy also uses a reference enthalpy and reference temperature

Ansys Fluent's solidification/melting theory defines material enthalpy as sensible enthalpy plus latent heat. Its sensible enthalpy relation includes:

```text
reference enthalpy
reference temperature
specific heat at constant pressure
```

The latent contribution is then added through liquid fraction and latent heat.

**Evidence-supported conclusion:** A phase-change enthalpy formulation must not treat the numerical zero of sensible enthalpy as implicit. The reference enthalpy and reference temperature belong to the formulation semantics.

Source:

- Ansys Fluent Theory Guide, *Solidification and Melting — Energy Equation*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v252/en/flu_th/flu_th_sec_melt_theory_energy.html

### 5.3 COMSOL provides an explicit zero-reference convention

COMSOL's Heat Transfer interface uses a reference temperature `T_ref` and defines reference enthalpy `H_ref` as zero at the reference pressure and reference temperature.

**Evidence-supported conclusion:** Setting the reference sensible/thermodynamic energy to zero at a declared reference condition is an established and valid convention.

**Boundary on interpretation:** This is one convention. The existence of another valid offset does not change energy differences if all material relations and state conversions are shifted consistently.

Source:

- COMSOL Multiphysics 6.4, *Settings for the Heat Transfer Interface*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_interfaces.08.26.html

## 6. Energy Offset and Physical Energy Differences

Let a selected specific thermodynamic energy coordinate be `epsilon(T)` in `[J/kg]`.

A common additive shift:

```text
epsilon'(T) = epsilon(T) + C
```

changes the numerical zero but preserves energy differences:

```text
epsilon'(T2) - epsilon'(T1)
= epsilon(T2) - epsilon(T1)
```

**Analytical consequence:** For a nonreacting bounded thermal formulation, the additive energy datum is a convention as long as all state relations, material data, initialization, and validation use the same convention.

This does not mean every offset can be mixed freely. A dataset, phase relation, or initialization using a different datum must be normalized before comparison or combination.

## 7. Canonical Dataset Normalization

Suppose an external material dataset provides a specific energy relation:

```text
epsilon_src(T)
```

with a known source datum. A formulation can normalize it to a chosen canonical reference temperature `T_ref` and canonical reference value `epsilon_ref` by:

```text
epsilon_norm(T)
= epsilon_src(T) - epsilon_src(T_ref) + epsilon_ref
```

If the bounded formulation chooses:

```text
epsilon_ref = 0 J/kg
```

then:

```text
epsilon_norm(T_ref) = 0
```

**Analytical implication:** Reference-state normalization can be performed as a configuration/data-preparation transformation before runtime state evolution. It need not change Framework-level State ownership or information flow.

### 7.1 Unknown source datum is a data-quality problem

If an external tabulation provides absolute-looking energy values but does not document its reference state, those values cannot be safely combined with another energy relation that uses a different datum.

**Research requirement:** Material-data provenance should record the source reference temperature, pressure where relevant, and energy offset convention whenever absolute enthalpy/internal-energy values are imported.

## 8. Latent Heat Requires Cross-Phase Reference Consistency

Fluent represents phase-change enthalpy as sensible enthalpy plus a latent contribution. Ansys CFX separately warns that, when latent heat is obtained from the difference between phase enthalpies, the phase enthalpy fields must contain consistently defined absolute enthalpies; custom materials therefore require correctly defined reference temperature, reference pressure, and reference specific enthalpy.

**Evidence-supported conclusion:** A common arbitrary offset is harmless only when it is common to the compared thermodynamic relations. Independent inconsistent offsets between solid and liquid phase enthalpies can corrupt the latent-heat difference.

Sources:

- Ansys Fluent Theory Guide, *Solidification and Melting — Energy Equation*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v252/en/flu_th/flu_th_sec_melt_theory_energy.html
- Ansys CFX-Solver Theory Guide, *Thermal Phase Change Model*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v251/en/cfx_mod/i1305851.html

## 9. Reference Temperature Has Numerical as Well as Semantic Consequences

Ansys Fluent documents that the sensible-enthalpy reference temperature may be moved into the expected solution-temperature range when specific heat is strongly nonlinear and the solution range is far from the default reference temperature. Fluent notes that this can reduce roundoff error in the enthalpy integral and avoid unphysical temperature oscillations.

**Evidence-supported conclusion:** The physical energy datum is conventional, but the numerical placement of the reference temperature can affect conditioning and finite-precision behavior.

Source:

- Ansys Fluent User's Guide, *Setting the Reference Temperature for Enthalpy*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v251/en/flu_ug/flu_ug_sec_hxfer.html

**Boundary on interpretation:** This is numerical guidance, not evidence that a particular `T_ref` is physically privileged.

## 10. Density Reference Temperature and Energy Reference Temperature Are Different Semantics

The source evidence exposes two distinct reference-temperature roles:

```text
T_rho_ref  -> identifies the density/reference-geometry condition
T_E_ref    -> identifies the thermodynamic energy datum
```

COMSOL uses a volume reference temperature for material-frame density and also has a Heat Transfer reference temperature for reference enthalpy and, in some couplings, reference density.

**Analytical conclusion:** A formulation may choose the same numerical temperature for convenience, but it should not silently assume that density-reference and energy-reference temperatures are semantically identical.

A later reference formulation should name them separately unless it explicitly defines one shared reference state.

## 11. Candidate Normalization Profile for the Minimal Reference Formulation

The current evidence supports, but does not yet freeze, a compact candidate profile:

```text
Density:
  rho_ref = constant [kg/m^3]
  same value across solid/liquid phase change
  tied to a declared reference geometry and T_rho_ref

Energy:
  selected specific energy coordinate epsilon [J/kg]
  T_E_ref explicitly declared
  epsilon_ref explicitly declared
  external material relations normalized to that datum

Phase change:
  latent heat L [J/kg] remains a difference/contribution
  solid/liquid energy relations share one compatible datum
```

For a zero-reference convention:

```text
epsilon_ref = 0 J/kg at T_E_ref
```

This profile is compatible with either a later enthalpy or internal-energy selection. It therefore narrows reference-state semantics without prematurely deciding the energy coordinate.

## 12. Preliminary Findings

### F-01 — Reference density must be tied to a declared reference condition

A material-frame reference density represents mass per reference volume and cannot be treated as an arbitrary phase-dependent scalar.

Status: **Supported by authoritative technical source**

### F-02 — One density across phases remains the strongest bounded candidate

For fixed geometry, fixed per-cell mass, no mass transport, and no deformation, the source evidence continues to support one density across the solid/liquid transition.

Status: **Supported bounded modeling candidate — not frozen**

### F-03 — Solid density is an established reference-density convention, not a universal rule

OpenFOAM explicitly uses a reference density that is typically the solid density.

Status: **Supported implementation precedent**

### F-04 — Thermodynamic energy requires an explicit reference temperature and offset

OpenFOAM, Fluent, and COMSOL all expose reference-state semantics rather than treating the energy zero as undefined implementation trivia.

Status: **Supported by authoritative/primary technical sources**

### F-05 — A common additive energy shift preserves nonreacting energy differences

Reference-state normalization may shift the numerical datum while preserving differences, provided every coupled material relation uses the same convention.

Status: **Analytical thermodynamic consequence**

### F-06 — Independent phase offsets can corrupt latent-heat relations

When latent heat is obtained from or coupled to phase enthalpies, solid and liquid energy relations must be reference-compatible.

Status: **Supported by authoritative technical source + analytical consequence**

### F-07 — Density and energy reference temperatures should remain semantically distinct

They may be numerically equal by convention, but they govern different mappings unless a later formulation explicitly defines one shared reference state.

Status: **Analytical classification supported by source semantics**

## 13. Implication for ThermoCore Reference-Formulation Research

The research can now move from an undefined "reference state" to explicit formulation parameters without changing Framework architecture.

A later reference formulation should be able to state, at minimum:

```text
rho_ref
T_rho_ref
energy coordinate kind
energy unit basis
T_E_ref
energy reference value
dataset normalization rule
latent-heat reference compatibility
```

These are formulation/configuration semantics, not new Framework Core components and not new Framework Interface semantics.

## 14. Remaining Evidence Gaps

After this comparison, the principal unresolved questions are:

1. whether `rho_ref` should specifically use solid density, a density evaluated at a declared nominal temperature, or another documented material-reference convention;
2. whether the candidate reference formulation should standardize `epsilon_ref = 0` at `T_E_ref` or permit an arbitrary documented offset;
3. whether `T_rho_ref` and `T_E_ref` should be unified in the minimal reference profile for simplicity;
4. whether the eventual energy coordinate is enthalpy or internal energy;
5. whether the stored coordinate is specific or volumetric after the density convention is fixed;
6. whether Temperature and Phase Fraction are Persistent or Derived State under the bounded formulation;
7. benchmark evidence demonstrating that reference-state normalization preserves energy balance and temperature/phase recovery across representative material datasets.

## 15. Current Decision

**Do not modify the Framework Specification.**

**Do not yet freeze the numerical value or universal provenance of `rho_ref`.**

**Do require any future formulation decision to make density-reference and energy-reference semantics explicit and reference-compatible.**

The strongest current candidate remains a fixed-volume, fixed-mass, equal-density reference formulation with one documented `rho_ref`, plus an explicitly normalized thermodynamic energy datum.

Proceed next to **Persistent vs Derived State classification** under this bounded candidate before authorizing `Thermodynamic_Formulation.md`.
