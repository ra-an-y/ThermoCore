# Enthalpy vs Internal Energy — Primary-Source Comparison

Status: Primary-Source Comparison  
Scope: Research only — non-normative  
Target: Evidence Gap 1 from `Thermodynamic_Formulation_Survey.md`

---

## 1. Research Question

Under incompressible, isochoric, fixed-grid, and solid/liquid phase-change assumptions, what do primary and authoritative technical sources support about choosing enthalpy versus internal energy as the evolving thermodynamic energy quantity in a ThermoCore implementation?

This comparison does **not** modify the ThermoCore Framework Specification. It does not designate either enthalpy or internal energy as a universal Thermodynamic State variable.

## 2. Source Set

This comparison uses sources that directly define or implement the relevant thermodynamic relations or phase-change formulation:

1. OpenFOAM official thermophysical source documentation:
   - `heThermo.H` — selectable Enthalpy/Internal energy field and explicit incompressible/isochoric distinctions;
   - `HtoEthermo.H` / `EtoHthermo.H` — direct conversion between sensible enthalpy and sensible internal energy.
2. COMSOL Multiphysics 6.4 Heat Transfer theory:
   - *Thermodynamic Description of Heat Transfer*;
   - *The Heat Balance Equation*;
   - *Material and Spatial Frames*.
3. ANSYS Fluent Theory Guide:
   - *Solidification and Melting — Energy Equation*.
4. Voller, Cross, and Markatos (1987), *An enthalpy method for convection/diffusion phase change*.
5. Voller and Prakash (1987), *A fixed grid numerical modelling methodology for convection-diffusion mushy region phase-change problems*.

The comparison is limited to what these sources directly support. Conclusions that combine multiple source observations are marked as analytical implications rather than as source quotations.

## 3. Direct Thermodynamic Relationship

### 3.1 Enthalpy and internal energy are distinct but directly related

OpenFOAM's thermophysical conversion source exposes the sensible internal energy relation as:

```text
Es = Hs - p / rho
```

and the inverse mapping as:

```text
Hs = Es + p / rho
```

The same relationship is used for absolute internal energy and absolute enthalpy.

COMSOL likewise states that, for a fluid, enthalpy includes the pressure-volume contribution `p / rho` in addition to internal energy.

**Evidence-supported conclusion:** Enthalpy and internal energy are not interchangeable names for the same physical quantity. Their difference contains a pressure-volume term.

Sources:

- OpenFOAM official source, `HtoEthermo.H`:  
  https://api.openfoam.com/2212/HtoEthermo_8H_source.html
- OpenFOAM official source, `EtoHthermo.H`:  
  https://api.openfoam.com/2506/EtoHthermo_8H.html
- COMSOL Multiphysics 6.4, *Thermodynamic Description of Heat Transfer*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.003.html

### 3.2 Energy-coordinate choice can be abstracted from higher-level architecture

OpenFOAM's `heThermo` exposes one transported thermodynamic energy field described as **Enthalpy/Internal energy [J/kg]**. The same class also reports separately whether its equation of state is:

```text
incompressible: rho != f(p)
isochoric:      rho = const
```

The thermophysical layer therefore distinguishes equation-of-state assumptions while allowing the energy coordinate to be enthalpy or internal energy.

**Evidence-supported conclusion:** A thermodynamic software architecture can remain stable while a lower thermophysical formulation selects enthalpy or internal energy.

Source:

- OpenFOAM official source, `heThermo.H`:  
  https://api.openfoam.com/2606/heThermo_8H_source.html

## 4. Incompressible and Isochoric Assumptions

### 4.1 OpenFOAM distinguishes incompressible from isochoric

OpenFOAM explicitly classifies:

- incompressible: density is not a function of pressure (`rho != f(p)`);
- isochoric: density is constant (`rho = const`).

Under this OpenFOAM taxonomy, an incompressible equation of state may still permit density dependence on variables other than pressure, while an isochoric equation of state constrains density to remain constant.

**Evidence-supported conclusion:** The two flags represent distinct assumptions in the source implementation and should not be collapsed into one category in this comparison.

Source:

- OpenFOAM official source, `heThermo.H`:  
  https://api.openfoam.com/2606/heThermo_8H_source.html

### 4.2 Isochoric alone does not make enthalpy and internal energy identical

From the source-defined relation:

```text
h - u = p / rho
```

constant density alone removes density variation but does not remove pressure variation. Therefore, for an isochoric formulation:

```text
Delta h = Delta u + Delta(p / rho)
```

and with constant `rho`:

```text
Delta h = Delta u + Delta p / rho
```

**Analytical implication:** Isochoric behavior by itself is insufficient to conclude `Delta h = Delta u`. That equality additionally requires the pressure contribution to remain constant or to be explicitly neglected within the selected formulation.

This implication is derived directly from the primary-source conversion relation and the source definition of `isochoric`; it is not a separate Framework requirement.

### 4.3 OpenFOAM-style incompressibility alone does not remove the pressure-volume contribution

Under the OpenFOAM classification used above, incompressibility states that density is not pressure-dependent; the term `p / rho` nevertheless remains present in the enthalpy/internal-energy relation.

**Analytical implication:** Within this bounded comparison, a formulation should not justify replacing internal energy with enthalpy, or vice versa, merely by citing the OpenFOAM-style incompressible flag. Pressure scope and density behavior must still be declared.

## 5. Fixed and Undeformed Material Domains

COMSOL derives its general heat balance from the first law using internal-energy accumulation. It separately represents mechanical stress power, including pressure-volume work and viscous dissipation in fluids.

COMSOL also states that material and spatial frames coincide for immobile and undeformed materials.

**Evidence-supported conclusion:** Internal energy is the explicit accumulated thermodynamic energy in COMSOL's general first-law heat-balance derivation, while mechanical work remains a separate contribution to the balance.

**Analytical implication for a bounded ThermoCore formulation:** For a fixed, immobile, undeformed material domain that intentionally excludes pressure-volume work, deformation work, kinetic-energy evolution, and mass transport, internal energy provides a direct first-law accumulation coordinate. This does **not** prove that enthalpy is invalid; an enthalpy formulation remains possible if its pressure-volume contribution and reference convention are consistently defined.

Sources:

- COMSOL Multiphysics 6.4, *The Heat Balance Equation*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.005.html
- COMSOL Multiphysics 6.4, *Material and Spatial Frames*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.116.html

## 6. Phase-Change Evidence

### 6.1 Enthalpy is directly established for fixed-grid phase change

ANSYS Fluent's solidification/melting model computes material enthalpy as the sum of sensible enthalpy and latent heat content. The energy equation evolves this enthalpy, and temperature is coupled iteratively with liquid fraction.

Voller, Cross, and Markatos developed an enthalpy formulation for convection/diffusion phase change in which latent heat effects are isolated in a source term. The method covers latent heat evolution at a single isothermal transition temperature or over a temperature range.

Voller and Prakash developed a fixed-grid enthalpy methodology for convection-diffusion mushy-region phase-change problems, representing latent heat evolution and mushy-zone flow through source terms.

**Evidence-supported conclusion:** Enthalpy is a well-established and directly documented coordinate for fixed-grid solid/liquid phase-change formulations.

Sources:

- ANSYS Fluent Theory Guide, *Solidification and Melting — Energy Equation*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v242/en/flu_th/flu_th_sec_melt_theory_energy.html
- Voller, V. R., Cross, M., and Markatos, N. C. (1987), *An enthalpy method for convection/diffusion phase change*, DOI: 10.1002/nme.1620240119  
  https://doi.org/10.1002/nme.1620240119
- Voller, V. R., and Prakash, C. (1987), *A fixed grid numerical modelling methodology for convection-diffusion mushy region phase-change problems*, DOI: 10.1016/0017-9310(87)90317-6  
  https://doi.org/10.1016/0017-9310(87)90317-6

### 6.2 Phase-change precedent does not make enthalpy universally mandatory

The OpenFOAM thermophysical abstraction permits enthalpy or internal energy, while Fluent and the cited phase-change papers show strong precedent for enthalpy in a particular class of phase-change methods.

**Analytical implication:** The evidence supports enthalpy as a strong reference-formulation candidate for fixed-grid phase change, but it does not support elevating enthalpy into a universal Framework-level Thermodynamic State requirement.

## 7. Comparison by Assumption

| Assumption / use case | Internal-energy interpretation | Enthalpy interpretation | Evidence status |
|---|---|---|---|
| General continuum heat balance | Direct accumulated internal thermodynamic energy; mechanical work handled separately in COMSOL derivation | Related by pressure-volume contribution | Direct source support |
| Incompressible (OpenFOAM: `rho != f(p)`) | Valid candidate | Valid candidate; `p/rho` term remains | Direct relation + analytical implication |
| Isochoric (`rho = const`) | Valid candidate | Valid candidate; equivalence of changes additionally depends on pressure contribution | Direct relation + analytical implication |
| Fixed, immobile, undeformed material with excluded mechanical work | Directly aligned with first-law accumulation form | Usable if pressure-volume/reference treatment is consistently bounded | Source-supported interpretation + analytical implication |
| Fixed-grid solid/liquid phase change | Possible in principle but not established as the dominant formulation by the present source set | Strong direct precedent; latent heat naturally represented in enthalpy formulations | Strong direct source support for enthalpy |
| Compressible flow / significant pressure work | Requires full coupling to work and equation of state | Requires full coupling to pressure, density, work, and total-energy terms | Outside current bounded comparison |
| Density-changing phase change / mass transport | Requires explicit mass, volume, and work treatment | Requires explicit mass, volume, and work treatment | Evidence gap remains |

## 8. Preliminary Findings

### F-01 — The enthalpy/internal-energy distinction is physically meaningful

The relation between the two quantities contains the pressure-volume term `p / rho`.

Status: **Supported by primary/authoritative sources**

### F-02 — OpenFOAM's incompressible and isochoric classifications are distinct

OpenFOAM explicitly distinguishes pressure-independent density from constant density.

Status: **Supported by primary source**

### F-03 — Isochoric does not by itself imply equal enthalpy and internal-energy increments

With `rho = const`, pressure variation still contributes through `Delta p / rho`.

Status: **Derived analytical consequence of primary-source relations**

### F-04 — Internal energy is directly aligned with the general first-law accumulation form

COMSOL's continuum heat-balance derivation accumulates internal energy and treats stress power separately.

Status: **Supported by authoritative technical source**

### F-05 — Enthalpy has strong direct precedent for fixed-grid phase change

Fluent and the original Voller phase-change methods use enthalpy formulations to represent sensible and latent energy through phase change.

Status: **Supported by primary/authoritative sources**

### F-06 — No universal formulation winner is established

The current evidence does not justify a Framework-wide requirement that all conforming ThermoCore implementations use enthalpy or all use internal energy.

Status: **Research conclusion consistent with current Framework architecture**

## 9. Implication for ThermoCore Formulation Research

The present comparison narrows the formulation decision but does not freeze it.

For a possible bounded reference formulation, the next decision should explicitly declare:

1. whether the modeled control mass is fixed;
2. whether cell volume and density are constant, variable, or phase-dependent;
3. whether pressure is represented, assumed constant, or excluded from state evolution;
4. whether pressure-volume work, deformation work, viscous work, and kinetic energy are excluded;
5. whether mass can cross a cell or phase boundary;
6. whether the target is fixed-grid solid/liquid phase change;
7. whether the stored coordinate is `u`, `h`, or a volumetric/specific form of either;
8. how latent heat is included in the selected energy relation;
9. how temperature and phase fraction are recovered from the energy coordinate;
10. how reference-state offsets are normalized.

Only after these assumptions are fixed can an enthalpy-versus-internal-energy selection be judged for the reference formulation.

## 10. Remaining Evidence Gaps

This comparison closes the basic identity and assumption distinction but leaves the following unresolved:

1. specific versus volumetric energy choice in fixed-grid cells;
2. treatment of density change across solid/liquid phase change;
3. pressure and work assumptions for a simplified fixed-grid reference formulation;
4. source-term dimensional mapping from `[J]`, `[W]`, `[W/m^2]`, or `[W/m^3]` into the selected state coordinate;
5. benchmark evidence comparing internal-energy and enthalpy formulations under otherwise identical fixed-grid phase-change assumptions;
6. classification of Temperature and Phase Fraction as Persistent or Derived State under each bounded formulation.

## 11. Current Decision

**Do not modify the Framework Specification.**

**Do not yet freeze enthalpy or internal energy as the reference formulation.**

The strongest current result is conditional:

- internal energy is the direct accumulation quantity in a general first-law heat-balance description;
- enthalpy has strong and mature precedent for fixed-grid phase-change computation;
- neither OpenFOAM-style incompressibility nor isochoric behavior alone makes the two formulations physically identical;
- a reference-formulation choice requires explicit pressure, density, volume, work, mass-transport, and reference-state assumptions.

Proceed next to the fixed-grid **specific vs volumetric energy** comparison and dimensional source mapping before creating or authorizing `Thermodynamic_Formulation.md`.
