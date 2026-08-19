# Specific vs Volumetric Energy and Source Mapping — Primary-Source Comparison

Status: Primary-Source Comparison  
Scope: Research only — non-normative  
Target: Remaining formulation evidence gaps from `Thermodynamic_Formulation_Survey.md` and `Enthalpy_Internal_Energy_Primary_Source_Comparison.md`

---

## 1. Research Question

For a fixed-grid, cell-based ThermoCore implementation, what do primary and authoritative technical sources support about representing thermodynamic energy as a specific quantity `[J/kg]` versus a volumetric quantity `[J/m^3]`, and how should total energy, heat rate, boundary heat flux, and volumetric heat sources be dimensionally mapped into the selected formulation?

This comparison does **not** modify the ThermoCore Framework Specification. It does not assign universal units to Framework-level `Energy Input`, and it does not designate a universal stored Thermodynamic State variable.

## 2. Source Set

This comparison uses sources that directly expose thermodynamic variable units, conservative energy-equation structure, or source-term units:

1. OpenFOAM official thermophysical and solver source documentation:
   - `heThermo.H` — thermodynamic energy field `he` in `[J/kg]`;
   - `chtMultiRegionFoam/fluid/EEqn.H` — conservative accumulation `ddt(rho, he)` and finite-volume source terms.
2. COMSOL Multiphysics 6.4 Heat Transfer documentation:
   - *Solid* / *Theory for Heat Transfer in Solids* — density, specific heat capacity, effective volumetric heat capacity, and domain heat source units;
   - *Heat Source* — distributed domain heat-source semantics;
   - *Boundary Heat Sources* / *Heat Flux* — boundary source and flux semantics.
3. ANSYS Fluent User's Guide and Theory Guide:
   - *Defining Mass, Momentum, Energy, and Other Sources* — cell-zone sources are specified per unit volume and require cell-volume awareness;
   - *Heat Transfer Theory* — energy equation includes volumetric heat sources;
   - *Solidification and Melting — Energy Equation* — phase-change enthalpy is coupled with density and source terms.

The comparison distinguishes direct source statements from dimensional or architectural implications derived from them.

## 3. Specific Energy and Volumetric Energy Are Different Quantities

### 3.1 OpenFOAM stores the thermodynamic energy coordinate as a specific quantity

OpenFOAM `heThermo` exposes `he` as:

```text
Enthalpy/Internal energy [J/kg]
```

The thermophysical energy coordinate is therefore mass-specific.

Source:

- OpenFOAM official source, `heThermo.H`:  
  https://api.openfoam.com/2606/heThermo_8H_source.html

### 3.2 The conservative finite-volume equation uses density times the specific energy

OpenFOAM's transient fluid energy equation contains:

```text
fvm::ddt(rho, he)
```

where `he` is `[J/kg]` and `rho` is density. Their product has volumetric-energy dimensions.

**Evidence-supported conclusion:** A solver may represent the thermodynamic energy coordinate as a specific quantity while evolving a conservative volumetric accumulation term.

Source:

- OpenFOAM official source, `chtMultiRegionFoam/fluid/EEqn.H`:  
  https://api.openfoam.com/2512/heatTransfer_2chtMultiRegionFoam_2fluid_2EEqn_8H_source.html

### 3.3 COMSOL separates specific material properties from volumetric balance coefficients

COMSOL's solid heat-transfer formulation uses:

- density `rho` in `[kg/m^3]`;
- specific heat capacity `Cp` in `[J/(kg*K)]`;
- the product `rho Cp` as volumetric heat capacity in `[J/(m^3*K)]`;
- domain heat source `Q` in `[W/m^3]`.

**Analytical implication:** Specific material properties and volumetric balance quantities can coexist in one formulation. The source equations do not imply that an implementation must store a volumetric energy state merely because its balance contains volumetric coefficients or sources.

Sources:

- COMSOL 6.4, *Theory for Heat Transfer in Solids*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.007.html
- COMSOL 6.4, *Solid*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_ht_features.09.044.html

## 4. Dimensional Identity for a Fixed Cell

Let:

```text
e_s    = specific thermodynamic energy [J/kg]
rho    = density [kg/m^3]
e_v    = volumetric thermodynamic energy [J/m^3]
V      = cell volume [m^3]
m      = cell mass [kg]
E_cell = total thermodynamic energy represented by the cell [J]
```

The dimensional relations are:

```text
e_v = rho * e_s
m = rho * V
E_cell = e_v * V = rho * e_s * V = m * e_s
```

These equations are dimensional identities, not a Framework requirement and not a storage prescription.

### 4.1 Fixed volume does not imply fixed mass

For a geometrically fixed cell, `V` may remain constant while `rho` changes. Then:

```text
m = rho * V
```

also changes unless mass conservation, phase treatment, or another formulation constraint prevents it.

**Analytical implication:** A fixed-grid geometry alone does not justify treating specific and volumetric energy as interchangeable through one constant conversion factor. Density behavior must be declared.

## 5. Volumetric Domain Sources

### 5.1 Authoritative solver interfaces use `[W/m^3]` domain energy sources

COMSOL defines domain heat source `Q` in `[W/m^3]`. ANSYS Fluent likewise defines cell-zone energy sources per unit volume and explicitly notes that users often need the cell-zone volume to determine an appropriate source value.

OpenFOAM energy equations combine `rho*he` accumulation with source terms on the right-hand side.

**Evidence-supported conclusion:** Volumetric source terms are an established continuum and finite-volume representation even when the thermodynamic energy field itself is specific.

Sources:

- COMSOL 6.4, *Heat Source*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_ht_features.09.016.html
- ANSYS Fluent 2025 R1, *Defining Mass, Momentum, Energy, and Other Sources*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v251/en/flu_ug/flu_ug_bcs_sec_cell_zones.html
- OpenFOAM official source, `chtMultiRegionFoam/fluid/EEqn.H`:  
  https://api.openfoam.com/2512/heatTransfer_2chtMultiRegionFoam_2fluid_2EEqn_8H_source.html

### 5.2 Mapping a volumetric source into cell energy

Let:

```text
Q_v = volumetric source [W/m^3]
dt  = timestep [s]
```

For a cell of volume `V`:

```text
P_cell = Q_v * V                  [W]
Delta E_cell = Q_v * V * dt      [J]
```

If the stored thermodynamic coordinate is volumetric energy:

```text
Delta e_v = Q_v * dt             [J/m^3]
```

If the stored coordinate is specific energy and density is treated consistently over the update:

```text
Delta e_s = (Q_v / rho) * dt     [J/kg]
```

The last relation is a dimensional mapping. Its numerical use requires an explicit density-update convention when `rho` varies during the timestep.

## 6. Boundary Heat Flux

COMSOL distinguishes boundary heat sources from domain heat sources. Its boundary-source quantity is expressed per unit area `[W/m^2]`, whereas domain heat sources are expressed per unit volume `[W/m^3]`.

Sources:

- COMSOL 6.4, *Boundary Heat Sources*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_modeling.06.11.html
- COMSOL 6.4, *Heat Flux*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_ht_features.09.065.html

Let:

```text
q_A = inward boundary heat flux [W/m^2]
A   = affected face area [m^2]
```

For a uniformly applied face flux:

```text
P_face = q_A * A                  [W]
Delta E_cell = q_A * A * dt      [J]
```

An equivalent cell-averaged volumetric source would be:

```text
Q_v,eq = q_A * A / V             [W/m^3]
```

and the corresponding specific source rate would be:

```text
q_m,eq = q_A * A / (rho * V)     [W/kg]
```

These are dimensional mappings for a selected cell and face. They do not define how a solver distributes a boundary contribution among multiple adjacent cells or handles nonuniform face integration.

## 7. Total Heat Rate and Total Energy

Framework-level `Energy Input` currently does not define whether an input is total energy, heat rate, surface flux, volumetric source, or another normalized quantity. A formulation layer must therefore make the physical and dimensional mapping explicit before state evolution.

### 7.1 Total heat rate `[W]`

For a total heat rate `P` assigned uniformly to one selected cell:

```text
Q_v = P / V                      [W/m^3]
q_m = P / (rho * V)              [W/kg]
```

For a selected multi-cell region, the conversion requires a declared distribution rule. Equal-per-cell, equal-per-volume, equal-per-mass, or spatially weighted distributions are not equivalent.

### 7.2 Total energy `[J]`

If a total energy increment `Delta E` is assigned to one cell:

```text
Delta e_v = Delta E / V          [J/m^3]
Delta e_s = Delta E / (rho * V)  [J/kg]
```

If that energy is delivered over a timestep and represented as a rate:

```text
P = Delta E / dt                 [W]
```

This temporal conversion is a formulation choice. An impulse-like deposit, a continuously applied rate, and a timestep-integrated source may have the same net joules while differing numerically and physically.

## 8. Specific Storage vs Volumetric Storage

| Question | Specific energy `[J/kg]` | Volumetric energy `[J/m^3]` |
|---|---|---|
| Direct compatibility with OpenFOAM `he` | Direct | Requires multiplication by density |
| Compatibility with the present source-set material quantities | Direct for OpenFOAM `he`; COMSOL also exposes specific `Cp` | Requires density conversion where the source quantity is specific |
| Direct compatibility with volumetric source `Q [W/m^3]` | Requires density normalization for a specific source rate | Direct source-rate increment |
| Fixed-cell total energy | Multiply by `rho V` | Multiply by `V` |
| Density variation | Stored value is mass-specific; conservative cell energy still depends on `rho` | Volumetric value changes with `rho` even when the specific thermodynamic value is unchanged |
| Cell-volume variation | Specific coordinate itself does not depend on volume | Volumetric coordinate changes with volume for fixed total energy |
| Conservative finite-volume accumulation | Can appear as `rho * e_s`, as in OpenFOAM | Already volume-normalized, subject to formulation details |

**Analytical implication:** The storage-coordinate choice and the source-unit choice are separate decisions. A solver can store specific energy while accepting volumetric sources, or use a volumetric energy representation while consuming specific material data, provided density and geometry mappings are explicit and conservative.

## 9. Density Change Is the Critical Coupling Variable

OpenFOAM's `ddt(rho, he)` directly demonstrates that conservative accumulation can depend on both density and a mass-specific energy coordinate.

COMSOL documentation similarly uses `rho Cp` as the volumetric heat-capacity factor and notes that moving/deforming-frame treatment can account for volume-change effects on density.

ANSYS Fluent's phase-change energy equation couples enthalpy with density and includes source terms.

**Analytical implication:** Density is the conversion bridge between mass-specific and volume-specific energy representations. Any bounded formulation that permits density change must state how density evolution, cell volume, mass conservation, and phase change are related.

Sources:

- OpenFOAM official source, `heThermo.H`:  
  https://api.openfoam.com/2606/heThermo_8H_source.html
- OpenFOAM official source, `chtMultiRegionFoam/fluid/EEqn.H`:  
  https://api.openfoam.com/2512/heatTransfer_2chtMultiRegionFoam_2fluid_2EEqn_8H_source.html
- COMSOL 6.4, *Heat Source*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_ht_features.09.016.html
- ANSYS Fluent Theory Guide, *Solidification and Melting — Energy Equation*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v252/en/flu_th/flu_th_sec_melt_theory_energy.html

## 10. Preliminary Findings

### F-01 — Specific energy and volumetric energy are distinct representations

They are related by density:

```text
e_v = rho * e_s
```

Status: **Dimensional identity supported by source-defined units**

### F-02 — Specific state plus volumetric conservative accumulation is established practice

OpenFOAM stores `he` in `[J/kg]` while evolving terms containing `rho*he`.

Status: **Supported by primary source**

### F-03 — Volumetric heat-source semantics are established

COMSOL and Fluent define domain energy sources per unit volume; boundary heat contributions are separately represented per unit area.

Status: **Supported by authoritative technical sources**

### F-04 — Source unit does not determine storage unit

A volumetric source can update a specific state through density normalization, and specific material quantities can coexist with volumetric balance equations.

Status: **Analytical conclusion supported by primary-source equation structure**

### F-05 — Geometry is required to map surface or total inputs into a cell balance

Area converts surface flux to power; volume converts volumetric source to cell power; mass or `rho V` converts total cell energy to a specific increment.

Status: **Dimensional consequence**

### F-06 — Density change prevents a universal constant conversion between specific and volumetric energy

When density varies, `e_v = rho e_s` varies even if `e_s` does not, and cell mass changes for fixed `V` unless additional mass/phase constraints are imposed.

Status: **Derived analytical consequence; density-change formulation remains an evidence gap**

### F-07 — Current evidence does not require formulation-specific units at Framework level

The source set supports multiple physically meaningful source representations whose concrete conversion belongs to a thermodynamic formulation used by an implementation. No evidence in this comparison requires changing the current Framework-level `Energy Input` semantics.

Status: **Research conclusion consistent with current Framework architecture**

## 11. Implication for a ThermoCore Reference Formulation

The present comparison narrows the formulation design space but does not yet justify selecting one stored coordinate.

A reference formulation must explicitly declare:

1. whether the primary stored energy coordinate is specific or volumetric;
2. whether density is constant, prescribed, phase-dependent, or evolved;
3. whether cell volume is fixed;
4. whether mass is conserved per cell;
5. how a volumetric source `[W/m^3]` updates the stored coordinate;
6. how a boundary flux `[W/m^2]` is integrated over cell faces;
7. how a total rate `[W]` is distributed across one or more cells;
8. how a total energy increment `[J]` is distributed and time-integrated;
9. how latent heat and phase fraction modify the selected energy relation;
10. how conservation is checked after every mapping.

## 12. Remaining Evidence Gaps

The following remain unresolved after this comparison:

1. density change across solid/liquid phase change in a fixed-grid cell;
2. whether per-cell mass is fixed, redistributed, or represented only through effective properties;
3. reference-state normalization for specific and volumetric enthalpy/internal energy;
4. benchmark comparison of specific-state and volumetric-state implementations under the same physical assumptions;
5. numerical treatment when `rho` changes during the same timestep as energy deposition;
6. classification of Temperature and Phase Fraction as Persistent or Derived State under the selected bounded formulation.

## 13. Current Decision

**Do not modify the Framework Specification.**

**Do not yet freeze specific or volumetric energy as the reference storage coordinate.**

The strongest current result is:

- specific thermodynamic energy `[J/kg]` is directly supported as a thermophysical energy coordinate by OpenFOAM;
- conservative finite-volume accumulation naturally introduces density and therefore a volumetric energy quantity;
- domain sources are commonly volumetric `[W/m^3]`, while boundary source/flux quantities are area-based `[W/m^2]`;
- total `[J]` and rate `[W]` inputs require explicit geometry, mass, distribution, and timestep mapping;
- density evolution is the central unresolved issue linking specific and volumetric representations.

Proceed next to **density change across fixed-grid solid/liquid phase change** before authorizing a reference thermodynamic formulation.
