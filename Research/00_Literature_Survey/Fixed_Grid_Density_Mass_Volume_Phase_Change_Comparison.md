# Fixed-Grid Density, Mass, and Volume Handling in Phase Change — Primary-Source Comparison

Status: Primary-Source Comparison  
Scope: Research only — non-normative  
Target: Density-change evidence gap from `Specific_Volumetric_Energy_Source_Mapping_Comparison.md`

---

## 1. Research Question

For a fixed-grid, cell-based thermodynamic formulation, how can solid/liquid density change be represented without violating mass conservation, and what assumptions are required when cell geometry, material motion, mass transport, or phase volume change are excluded?

This comparison does **not** modify the ThermoCore Framework Specification. It does not require a variable-density model, a moving mesh, a free-surface solver, or any particular stored Thermodynamic State variable.

## 2. Source Set

This comparison uses sources that directly define mass-conservation requirements or implement density-changing phase-change models:

1. COMSOL Multiphysics 6.4 Heat Transfer documentation:
   - *Apparent Heat Capacity Method*;
   - *Phase Change Material*;
   - *Consistency with Mass and Momentum Conservation Laws*;
   - *Material Density in Features Defined in the Material Frame*;
   - *Conversion Between Material and Spatial Frames*.
2. OpenFOAM official source documentation:
   - `solidificationMeltingSource.H`.
3. Ansys official documentation:
   - Boussinesq/reference-density treatment for buoyancy.
4. Thirumalaisamy and Bhalla (2023), *A low Mach enthalpy method to model non-isothermal gas–liquid–solid flows with melting and solidification*.
5. Thirumalaisamy and Bhalla (2025), *A consistent, volume preserving, and adaptive mesh refinement-based framework for modeling non-isothermal gas–liquid–solid flows with phase change*.

The comparison separates direct source statements from dimensional or architectural implications derived from them.

## 3. Fixed Grid Does Not Mean Fixed Material Mass

Let a geometric cell have fixed volume:

```text
V_cell = constant
```

and instantaneous density:

```text
rho = rho(t)
```

Then the mass represented inside that geometric cell is:

```text
m_cell = rho * V_cell
```

If `rho` changes while `V_cell` stays fixed, `m_cell` also changes unless mass enters or leaves the cell.

Therefore three independent ideas must not be collapsed:

```text
fixed geometric cell volume
fixed material mass
variable density
```

A formulation can combine any two only if the third is treated consistently through mass flux, material deformation, or another declared conservation mechanism.

**Dimensional consequence:** A fixed Eulerian grid does not imply that each geometric cell is a closed material control mass.

## 4. Material-Frame Solid Phase Change

COMSOL explicitly distinguishes density defined in the material frame from density defined in the spatial frame.

For solid phase change represented on the material frame, COMSOL states that a **single density should be defined for the different phases to ensure mass conservation on the material frame**.

COMSOL further explains that a nonconstant density specified directly in a material-frame feature implies addition or removal of matter unless deformation is represented consistently.

**Evidence-supported conclusion:** In a fixed, undeformed material-frame phase-change model, assigning different solid and liquid densities as if they were independently switchable material properties is not mass-conservative by itself.

Sources:

- COMSOL Multiphysics 6.4, *Apparent Heat Capacity Method*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.024.html
- COMSOL Multiphysics 6.4, *Phase Change Material*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_ht_features.09.036.html
- COMSOL Multiphysics 6.4, *Material Density in Features Defined in the Material Frame*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_modeling.06.25.html

## 5. Variable Density Requires a Mass-Conservation Mechanism

COMSOL states that when fluid density changes with time, the transport velocity field and density must be defined so that mass is conserved locally. It also notes that deformation may be represented with a moving mesh when volume change must be captured.

The general mass-conservation context is maintained even when the Heat Transfer interface itself solves only the energy equation.

**Evidence-supported conclusion:** Density evolution cannot be treated as a purely local thermodynamic property update when the corresponding mass or volume change is physically retained. The formulation must also account for local mass conservation.

Sources:

- COMSOL Multiphysics 6.4, *Apparent Heat Capacity Method*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.024.html
- COMSOL Multiphysics 6.4, *Consistency with Mass and Momentum Conservation Laws*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.006.html

## 6. Material-Frame and Spatial-Frame Volume Are Not the Same Quantity

COMSOL's frame-conversion documentation preserves total mass when transforming between material and spatial representations. Density changes associated with deformation are therefore linked to the Jacobian of the geometric transformation rather than treated as an arbitrary per-cell scalar replacement.

**Evidence-supported conclusion:** If a formulation models actual expansion or shrinkage, density and volume must be coupled through a mass-conserving geometric or transport relation.

Source:

- COMSOL Multiphysics 6.4, *Conversion Between Material and Spatial Frames*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.117.html

## 7. Reference-Density and Boussinesq-Type Approximations

Ansys documents Boussinesq-type buoyancy models in which a constant reference density is retained for the governing equations while temperature-dependent density variation enters only through the buoyancy source approximation.

This is useful when density variation is small and the objective is buoyancy rather than true shrinkage or expansion.

**Evidence-supported conclusion:** A reference-density/Boussinesq approximation is not equivalent to a variable-density mass-conserving phase-change model. It intentionally suppresses most density-change effects while retaining selected buoyancy behavior.

Source:

- Ansys CFX-Solver Theory Guide, *Boussinesq Model*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v252/en/cfx_thry/i1299782.html

## 8. OpenFOAM `solidificationMeltingSource` Is a Bounded Source Model

OpenFOAM's `solidificationMeltingSource` exposes:

```text
L       latent heat of fusion [J/kg]
rhoRef  reference density, typically solid density
beta    thermal expansion coefficient [1/K]
rho     optional density field name
alpha1  phase-fraction field
```

The model is explicitly a source model for solidification and melting and cites the Voller fixed-grid enthalpy literature.

**Evidence-supported conclusion:** OpenFOAM provides a practical fixed-grid phase-change source model with reference-density and phase-fraction concepts.

**Boundary on interpretation:** The presence of `rhoRef`, `beta`, and an optional density field is not, by itself, evidence that this source model captures arbitrary shrinkage/expansion or free-surface motion caused by a solid/liquid density jump. Those effects require examination of the complete coupled flow and mass-conservation formulation.

Source:

- OpenFOAM official source, `solidificationMeltingSource.H`:  
  https://api.openfoam.com/2506/solidificationMeltingSource_8H_source.html

## 9. Fixed Eulerian Grids Can Represent Density-Change-Induced Volume Change — With Additional Physics

Thirumalaisamy and Bhalla (2023) developed a fixed-grid low-Mach enthalpy method that explicitly permits variable thermophysical properties including density. The method couples a solid/liquid phase-change material to a gas phase so that free-surface dynamics and density-change-induced volume change can be represented.

The paper validates the method against Stefan problems with a density jump and reports conservation of phase-change-material mass.

Their later 2025 work improves consistency across the mass, momentum, and enthalpy equations and demonstrates conservation of mass, momentum, enthalpy, and phase composition in closed tests.

**Evidence-supported conclusion:** Unequal phase densities are compatible with a fixed Eulerian grid, but only when the fixed grid is used as a spatial discretization while mass transport, velocity, phase motion, and free-surface or volume redistribution are solved consistently.

Sources:

- Thirumalaisamy, R., and Bhalla, A. P. S. (2023), *A low Mach enthalpy method to model non-isothermal gas–liquid–solid flows with melting and solidification*, International Journal of Multiphase Flow 169, 104605.  
  https://doi.org/10.1016/j.ijmultiphaseflow.2023.104605
- Thirumalaisamy, R., and Bhalla, A. P. S. (2025), *A consistent, volume preserving, and adaptive mesh refinement-based framework for modeling non-isothermal gas–liquid–solid flows with phase change*, International Journal of Multiphase Flow 183, 105060.  
  https://doi.org/10.1016/j.ijmultiphaseflow.2024.105060

## 10. Three Distinct Modeling Regimes

The source evidence supports separating at least three regimes.

### 10.1 Regime A — Fixed geometry, fixed material mass, no shrinkage/expansion

Assumptions:

```text
cell geometry fixed
material domain undeformed
no mass transport
no free-surface motion
no density-jump-induced flow
```

A conservative simplification is to use one reference/effective density across the phase transition.

This is consistent with COMSOL's material-frame requirement that one density be used for solid phase change when the material frame itself is not changing.

**Interpretation:** Density difference between physical solid and liquid phases is intentionally outside the model.

### 10.2 Regime B — Fixed Eulerian mesh, variable density, mass redistribution allowed

Assumptions:

```text
mesh geometry fixed
material may move through cells
rho may vary
continuity is solved
velocity / mass flux participates
phase boundary or free surface may move
```

This is the regime demonstrated by the low-Mach enthalpy literature.

**Interpretation:** The mesh is fixed, but the material represented by an individual cell is not a fixed control mass.

### 10.3 Regime C — Deforming material domain

Assumptions:

```text
material mass tracked
rho may vary
physical volume changes
geometry / mesh deformation participates
```

COMSOL's material/spatial-frame treatment and moving-mesh guidance are examples of this class.

**Interpretation:** Density change is represented through deformation rather than being hidden inside an undeformed fixed material volume.

## 11. Per-Cell Mass Consequence

For a fixed geometric cell:

```text
m_cell(t) = rho(t) * V_cell
```

Taking a time derivative:

```text
dm_cell/dt = V_cell * drho/dt
```

if `V_cell` is constant.

If the cell is also assumed closed to mass flux, mass conservation requires:

```text
dm_cell/dt = 0
```

which implies:

```text
drho/dt = 0
```

for that closed fixed-volume cell.

**Analytical conclusion:** A model cannot simultaneously require all three of the following without an additional conservation mechanism:

1. fixed cell volume;
2. fixed mass in each cell;
3. phase-dependent density that changes during phase transition.

At least one of those assumptions must be relaxed or reinterpreted.

## 12. Consequence for Specific vs Volumetric Energy

The previous comparison established:

```text
e_v = rho * e_s
```

Density behavior now determines whether this mapping is simple or dynamically coupled.

### Equal-density bounded formulation

If:

```text
rho = rho_ref = constant
```

then specific and volumetric energy are related by one fixed conversion factor:

```text
e_v = rho_ref * e_s
```

This makes fixed-cell conservation bookkeeping comparatively simple.

### Variable-density formulation

If:

```text
rho = rho(T, phase, ...)
```

then:

```text
e_v = rho * e_s
```

changes with both thermodynamic energy and density. The energy update cannot be specified independently of the density/mass update when a conservative volumetric balance is required.

**Analytical conclusion:** Density treatment must be frozen before choosing between specific and volumetric energy as the reference storage coordinate.

## 13. Preliminary Findings

### F-01 — Fixed-grid geometry does not imply fixed cell mass

A fixed Eulerian cell may exchange material with neighboring cells while retaining constant geometric volume.

Status: **Dimensional and continuum-mechanics consequence**

### F-02 — Fixed, undeformed material-frame phase change favors a single density

COMSOL explicitly requires one density across phases for material-frame solid phase change to preserve mass conservation.

Status: **Supported by authoritative technical source**

### F-03 — Variable density requires continuity, deformation, or another mass-conservation mechanism

Time-varying density cannot be introduced as an isolated local property update if corresponding volume/mass effects are intended to remain physical.

Status: **Supported by authoritative technical sources**

### F-04 — Boussinesq/reference-density treatment is an approximation, not shrinkage/expansion modeling

It retains selected buoyancy effects while using a constant reference density elsewhere.

Status: **Supported by authoritative technical source**

### F-05 — Fixed-grid variable-density phase-change methods exist

Peer-reviewed low-Mach enthalpy methods demonstrate that density-change-induced motion and volume change can be modeled on a fixed Eulerian grid when mass, momentum, enthalpy, and phase/free-surface dynamics are coupled.

Status: **Supported by primary research literature**

### F-06 — A simple cell-local thermal formulation cannot retain a physical solid/liquid density jump for free

If the intended reference formulation excludes mass transport, deformation, velocity evolution, and free-surface motion, then retaining distinct phase densities would create an unresolved mass/volume conservation problem.

Status: **Analytical conclusion supported by the source set**

### F-07 — Equal-density phase change is a defensible bounded simplification, not a universal physical claim

A fixed reference density can preserve a closed, undeformed, fixed-volume thermal formulation while explicitly excluding shrinkage/expansion caused by phase-density differences.

Status: **Candidate bounded modeling assumption — not yet frozen**

## 14. Implication for ThermoCore Reference-Formulation Research

The evidence now separates two viable directions rather than leaving density behavior undefined.

### Candidate simplified reference formulation

A minimal fixed-grid thermal/phase-change reference formulation may declare:

```text
cell volume fixed
per-cell mass fixed
one reference density across solid/liquid phase change
no mass transport
no density-jump-induced flow
no shrinkage/expansion
no moving mesh or free surface
```

This would intentionally model thermal energy and phase transition while excluding mechanical/transport consequences of unequal phase density.

### Variable-density formulation

A formulation that retains real phase-density differences must additionally define at least:

```text
mass conservation / continuity
velocity or material transport
phase-volume change
boundary or free-surface motion, or an equivalent redistribution rule
consistent energy transport with changing rho
```

That is a substantially broader physical formulation and should not be silently introduced into a minimal thermal reference implementation.

Neither direction changes the Framework Core architecture. The distinction belongs to formulation scope and later implementation/extension decisions.

## 15. Remaining Evidence Gaps

After this comparison, the principal unresolved questions are:

1. whether the simplified reference formulation should explicitly adopt equal solid/liquid density;
2. which reference-density convention should be used if that simplification is selected;
3. how reference-state energy offsets are normalized;
4. whether Temperature and Phase Fraction are Persistent or Derived State under the bounded formulation;
5. whether a later variable-density formulation belongs as a separate reference formulation or extension-oriented implementation profile;
6. benchmark evidence for the equal-density simplified formulation before any formulation freeze.

## 16. Current Decision

**Do not modify the Framework Specification.**

**Do not introduce phase-dependent density into a fixed-volume, fixed-mass cell model without an explicit mass/volume conservation mechanism.**

The strongest current candidate for a minimal fixed-grid thermal reference formulation is therefore a deliberately bounded equal-density assumption across solid/liquid phase change. This is a research candidate, not a normative or frozen formulation decision.

Proceed next to reference-density convention, reference-state normalization, and Persistent/Derived State classification before authorizing `Thermodynamic_Formulation.md`.
