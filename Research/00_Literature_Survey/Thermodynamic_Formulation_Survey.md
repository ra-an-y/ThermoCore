# Thermodynamic Formulation Survey

Status: Preliminary Survey  
Scope: Research only — non-normative  
Target: Thermodynamic formulation used by a conforming ThermoCore implementation

---

## 1. Research Question

Which established energy-state formulation is appropriate for a conforming ThermoCore thermodynamic implementation, and how should runtime `Energy Input` map into that formulation while preserving dimensional consistency, reference-state semantics, phase-change behavior, and energy balance?

This survey does **not** modify the ThermoCore Framework Specification. In particular, it does not prescribe a universal set of Thermodynamic State variables.

## 2. Background

The ThermoCore Framework Specification intentionally defines Thermodynamic State semantically rather than prescribing specific physical quantities or solver mathematics. `Thermodynamic_State.md` leaves the concrete Persistent/Derived State quantities to the thermodynamic formulation used by a conforming ThermoCore implementation, while `Data_Flow.md` defines `Energy Input` only as runtime information supplied to Thermodynamic Computation.

A reference or conforming implementation therefore requires a lower-level thermodynamic formulation that makes the physical semantics explicit without redefining the framework architecture.

The immediate question arose from reviewing whether an enthalpy-based implementation is physically justified when ThermoCore does not itself prescribe pressure evolution, mechanical deformation, or a particular solver.

## 3. Survey Scope

The survey compares established formulations relevant to ThermoCore's current thermal and phase-change use case:

1. enthalpy-based formulations;
2. internal-energy-based formulations;
3. enthalpy decompositions into sensible and latent contributions;
4. apparent/effective heat-capacity formulations as an alternative computational representation of phase-change energy;
5. abstraction patterns that allow a thermodynamic implementation to select enthalpy or internal energy without changing the framework architecture.

The following are outside the present survey unless required to establish formulation boundaries:

- full compressible-flow thermodynamics;
- chemical-potential formulations;
- Gibbs/Helmholtz free-energy minimization;
- complete multiphysics coupling;
- backend-specific optimization.

## 4. Preliminary Evidence

### 4.1 Enthalpy is an established phase-change formulation

ANSYS Fluent's solidification/melting energy formulation defines material enthalpy as the sum of sensible enthalpy and latent heat content. The sensible contribution uses a reference enthalpy, reference temperature, and constant-pressure specific heat. Liquid fraction determines latent heat content and is coupled to the energy equation.

**Implication:** An enthalpy-based thermal/phase-change formulation is established practice; lack of an explicit framework-level `pV` solver does not by itself invalidate the use of enthalpy in a suitably bounded implementation.

Source: ANSYS Fluent Theory Guide, *Solidification and Melting — Energy Equation*, Release 2025 R1.  
https://ansyshelp.ansys.com/public/Views/Secured/corp/v251/en/flu_th/flu_th_sec_melt_theory_energy.html

### 4.2 Specific enthalpy and latent-energy treatment are used in commercial phase-change models

COMSOL's Apparent Heat Capacity Method expresses density and **specific enthalpy** for phase-change material and derives an apparent heat capacity containing both equivalent sensible contribution and distributed latent-heat contribution. The formulation also distinguishes material-frame density requirements for solid phase change and notes the need for mass and energy conservation when density varies.

**Implication:** The distinction among total, specific, and volumetric energy quantities is a formulation-level requirement. A phase-change computational representation may distribute latent heat over a transition interval without changing the underlying need for an energy-consistent formulation.

Source: COMSOL Multiphysics 6.4, *Apparent Heat Capacity Method*.  
https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.024.html

### 4.3 Enthalpy/internal-energy choice can be abstracted

OpenFOAM's `heThermo` abstraction stores an `he` energy field described as **Enthalpy/Internal energy [J/kg]** and provides temperature recovery from the selected energy variable together with pressure and an initial temperature estimate. The thermophysical implementation can therefore use an enthalpy- or internal-energy-based energy variable without requiring the architecture to treat one as universally mandatory.

**Implication:** ThermoCore should not claim that enthalpy is the universally correct thermal energy state. A thermodynamic formulation used by a conforming ThermoCore implementation can select an established energy variable while preserving framework-level State semantics.

Source: OpenFOAM API, `heThermo.H`.  
https://api.openfoam.com/2606/heThermo_8H_source.html

### 4.4 Temperature–enthalpy coupling requires explicit formulation

Published numerical studies of solid/liquid phase change treat the temperature–enthalpy relationship as strongly nonlinear and evaluate corrected and uncorrected enthalpy methods for solving that coupling.

**Implication:** `Energy Input -> Enthalpy -> Temperature/Phase` cannot remain an informal implementation assumption. A thermodynamic formulation used by a conforming ThermoCore implementation must explicitly define the energy variable, its reference convention, and the material relation used to recover temperature and phase information.

Source: *Comparison of Corrected and Uncorrected Enthalpy Methods for Solving Conduction-Driven Solid/Liquid Phase Change Problems*, Energies 16(1), 449 (2023).  
https://www.mdpi.com/1996-1073/16/1/449

## 5. Preliminary Comparison

| Formulation | Primary solved / represented quantity | Phase-change treatment | Current relevance to ThermoCore | Preliminary status |
|---|---|---|---|---|
| Enthalpy formulation | Enthalpy / specific enthalpy | Latent contribution embedded in enthalpy relation | Strong | Supported candidate |
| Internal-energy formulation | Internal energy / specific internal energy | Requires consistent phase-energy relation | Moderate | Requires deeper survey |
| Enthalpy decomposition — sensible + latent | Sensible enthalpy plus latent content | Liquid/phase fraction contributes latent energy | Strong | Supported candidate |
| Apparent heat capacity | Temperature with effective/apparent heat capacity | Latent heat distributed through transition interval | Strong as computational representation | Supported alternative representation |
| Generic `Energy State` | Unspecified | Unspecified | Too semantically broad as a physical formulation name | Not preferred without stronger definition |

This table is preliminary evidence only and does not establish a Framework decision.

## 6. Required Semantic Questions

Before adopting a ThermoCore thermodynamic formulation, the following must be resolved explicitly:

1. **Energy quantity** — Is the evolving quantity total enthalpy `[J]`, specific enthalpy `[J/kg]`, volumetric enthalpy `[J/m^3]`, internal energy, or another established quantity?
2. **Reference convention** — What reference temperature/state and reference energy convention are used, and how are external material datasets normalized?
3. **Energy Input semantics** — Is runtime input energy `[J]`, power `[W]`, surface flux `[W/m^2]`, volumetric source `[W/m^3]`, specific source `[W/kg]`, or a normalized net contribution?
4. **Geometry/mass mapping** — Where do density, cell volume, mass, boundary area, and timestep enter the mapping from supplied input to the evolving energy quantity?
5. **Sensible relation** — Is `cp` constant, temperature-dependent, phase-dependent, or represented through a precompiled `h(T)` relation?
6. **Latent relation** — How are latent heat and phase fraction represented and coupled to the energy state?
7. **Transition interval** — Which transition widths represent physical solidus/liquidus behavior and which are numerical regularization parameters?
8. **Pressure/work scope** — Which assumptions make the selected energy formulation valid, and which pressure, compressibility, mechanical-work, or mass-transport effects are explicitly outside the formulation?
9. **Derived quantities** — Which quantities, such as temperature or phase fraction, are uniquely derivable from Persistent State and material information under the selected formulation?
10. **Conservation equation** — What governing balance determines evolution of the selected energy quantity?

## 7. Preliminary Findings

### F-01 — Enthalpy naming is provisionally supported

Existing phase-change formulations provide direct precedent for enthalpy-based energy evolution and temperature/phase recovery. Therefore the name `Enthalpy` should **not** be rejected merely because the Framework Specification does not prescribe pressure evolution or mechanical work.

Status: **Provisionally Supported**

### F-02 — `Energy State` is too broad as a replacement term

A generic `Energy State` name does not identify whether the quantity is internal energy, enthalpy, total energy, specific energy, or volumetric energy. It should not replace an established thermodynamic quantity solely to avoid formulation assumptions.

Status: **Preliminary Finding**

### F-03 — The framework correctly leaves the concrete energy variable open

The current Framework Specification does not enumerate physical Thermodynamic State variables. This is compatible with evidence that established thermophysical systems may select enthalpy or internal energy at the formulation level.

Status: **Consistent with current Framework architecture**

### F-04 — `Energy Input` requires formulation-level physical semantics

Framework-level information flow is intentionally insufficient to define a solver update such as `h_new = h_old + EnergyInput`. A thermodynamic formulation used by a conforming ThermoCore implementation must perform dimensional and physical normalization before state evolution.

Status: **Research requirement**

### F-05 — Numerical transition width must not be promoted to physical material evidence

A smoothed/apparent heat-capacity phase transition can distribute latent heat over a finite temperature interval for numerical purposes. Such a numerical interval is not evidence that the real material possesses an equal physical mushy-zone width.

Status: **Preliminary distinction — further evidence required**

## 8. Relationship to Existing ThermoCore Specifications

No current Framework Specification change is recommended from this preliminary survey.

In particular:

- `Framework_Principles.md` should remain implementation- and numerical-method-agnostic.
- `Thermodynamic_State.md` should continue to define State semantics without enumerating universal physical variables.
- `Data_Flow.md` should continue to define `Energy Input` at the information-flow level rather than hard-coding units or one solver equation.
- A later thermodynamic-formulation document may define physical quantity semantics for a specific conforming formulation, provided it does not redefine Framework architecture or ownership.

## 9. Evidence Gaps / Next Research

The following evidence is still required before a formulation decision can be frozen:

1. primary-source comparison of enthalpy and internal-energy formulations under incompressible/isochoric and phase-change assumptions;
2. explicit treatment of specific versus volumetric enthalpy in fixed-grid/cell-based solvers;
3. reference-state conventions and normalization practices across thermophysical frameworks;
4. energy-source dimensional mapping for conduction, boundary flux, volumetric generation, and external source terms;
5. phase-change benchmarks comparing enthalpy and apparent-heat-capacity representations;
6. limits introduced by density change, compressibility, pressure work, mass transport, and irreversible material changes;
7. criteria for classifying Temperature and Phase Fraction as Persistent or Derived State for each candidate formulation.

## 10. Candidate Downstream Artifact

If evidence supports a single reference formulation, a later non-framework reference-formulation specification may be considered, tentatively named:

`Thermodynamic_Formulation.md`

It should define formulation-level physical semantics only and must not redefine Framework State ownership, information flow, extension boundaries, or implementation independence. It would not become part of the Framework Specification hierarchy unless separately authorized through the applicable governance process.

Creation of that document is **not** authorized by this Preliminary Survey alone.

## 11. Current Decision

**Do not modify the Framework Specification.**

Continue Research -> Evidence before adopting or renaming any thermodynamic energy variable in a normative formulation.
