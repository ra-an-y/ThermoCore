# Thermodynamic Formulation Evidence Matrix

Status: Evidence Matrix  
Scope: Research only — non-normative  
Research line: Bounded fixed-grid thermodynamic reference-formulation investigation

---

## 1. Purpose

This document synthesizes the completed thermodynamic-formulation survey and closure chain into an evidence layer.

It does not create Framework requirements and does not reopen the frozen Framework Specification. Its role is to establish whether the bounded reference-formulation research is sufficiently supported for a later non-Framework formulation specification to be authorized.

The matrix separates:

- directly supported source evidence;
- analytical consequences derived from that evidence;
- bounded formulation candidates and decisions;
- research gaps and downstream verification/validation obligations.

## 2. Source Research Artifacts

The matrix is derived from the following completed survey/comparison/closure artifacts:

1. `Research/00_Literature_Survey/Thermodynamic_Formulation_Survey.md`
2. `Research/00_Literature_Survey/Enthalpy_Internal_Energy_Primary_Source_Comparison.md`
3. `Research/00_Literature_Survey/Specific_Volumetric_Energy_Source_Mapping_Comparison.md`
4. `Research/00_Literature_Survey/Fixed_Grid_Density_Mass_Volume_Phase_Change_Comparison.md`
5. `Research/00_Literature_Survey/Reference_Density_Energy_Reference_State_Comparison.md`
6. `Research/00_Literature_Survey/Persistent_Derived_State_Classification_Comparison.md`
7. `Research/00_Literature_Survey/Enthalpy_Temperature_Phase_Closure_Study.md`

The underlying source set includes official OpenFOAM, COMSOL, Ansys Fluent, and Ansys CFX documentation together with the primary phase-change literature recorded in the source artifacts.

## 3. Evidence Status Vocabulary

| Status | Meaning |
|---|---|
| **Verified** | Directly supported by a primary or authoritative technical source recorded in the survey chain |
| **Supported inference** | Analytical consequence derived from Verified evidence with explicit scope assumptions |
| **Bounded candidate** | A formulation choice defensible under the current bounded model; it remains non-universal even if selected for a later reference formulation |
| **Research gap** | Evidence or decision remains insufficient for specification authorization |

These statuses are research classifications only.

## 4. Evidence Matrix

| ID | Evidence statement | Status | Primary support | Scope / limit | Downstream relevance |
|---|---|---|---|---|---|
| TF-E01 | Enthalpy and internal energy are distinct thermodynamic quantities related through a pressure-volume contribution. | Verified | OpenFOAM `HtoEthermo` / `EtoHthermo`; COMSOL thermodynamic heat-transfer theory | Does not select either coordinate for ThermoCore | Energy-coordinate selection |
| TF-E02 | OpenFOAM distinguishes incompressible (`rho != f(p)`) from isochoric (`rho = const`) thermophysical assumptions. | Verified | OpenFOAM `heThermo` | OpenFOAM taxonomy; not asserted as a universal terminology definition | Pressure/density assumption declaration |
| TF-E03 | Constant density alone does not remove the enthalpy/internal-energy pressure contribution if pressure varies. | Supported inference | TF-E01 + TF-E02 | Bounded algebraic consequence | Prevents invalid `isochoric => h=u` shortcut |
| TF-E04 | Internal energy is a direct accumulation quantity in a general first-law heat-balance description while mechanical work can be represented separately. | Verified | COMSOL heat-balance theory | General continuum formulation; not a universal solver prescription | Internal-energy candidate support |
| TF-E05 | Enthalpy has strong direct precedent for fixed-grid solid/liquid phase-change computation. | Verified | Ansys Fluent solidification/melting; Voller et al. phase-change methods | Strong precedent, not universal Framework requirement | Enthalpy candidate support |
| TF-E06 | Established thermophysical software can support enthalpy/internal-energy selection below a stable higher-level architecture. | Verified | OpenFOAM `heThermo` abstraction | Software precedent rather than ThermoCore requirement | Supports keeping Framework variable-neutral |
| TF-E07 | A mass-specific thermodynamic energy coordinate `[J/kg]` can coexist with a conservative accumulation involving `rho * energy`. | Verified | OpenFOAM `he` + energy-equation structure | Specific implementation precedent | Specific vs volumetric decision |
| TF-E08 | Domain heat sources are commonly volumetric `[W/m^3]`, while boundary heat contributions may be area-based `[W/m^2]`. | Verified | COMSOL Heat Source / Heat Flux; Ansys Fluent cell-zone sources | Source-unit semantics do not determine storage basis | Energy Input dimensional mapping |
| TF-E09 | Source-unit choice and persistent energy-coordinate basis are separate formulation decisions. | Supported inference | TF-E07 + TF-E08 | Requires explicit density/geometry mappings | Prevents source/storage conflation |
| TF-E10 | Mapping total energy, power, surface flux, or volumetric source into a cell balance requires applicable mass, area, volume, density, distribution, and timestep semantics. | Supported inference | Source-unit evidence + dimensional identities | Distribution rules remain formulation-specific | Energy Input conversion contract |
| TF-E11 | Fixed Eulerian grid geometry does not imply fixed material mass per cell. | Supported inference | Continuum mass relation; COMSOL frame semantics | Fixed-grid statement only | Density/mass model boundary |
| TF-E12 | A physical solid/liquid density jump cannot be inserted into a fixed-volume, fixed-mass, no-transport cell model without an additional mass/volume conservation mechanism. | Supported inference | COMSOL conservation guidance; variable-density phase-change literature | Applies to the bounded no-transport model | Density simplification decision |
| TF-E13 | Unequal phase densities can be represented on a fixed Eulerian grid when continuity, velocity/mass transport, and phase-volume/free-surface dynamics are coupled consistently. | Verified | Thirumalaisamy & Bhalla variable-density enthalpy methods | Broader multiphysics formulation | Defines out-of-scope variable-density path |
| TF-E14 | A single density across phases is directly supported for undeformed material-frame phase change when mass conservation is to be preserved without deformation. | Verified | COMSOL Phase Change Material / Apparent Heat Capacity Method | Material-frame bounded case | Equal-density candidate support |
| TF-E15 | Equal-density solid/liquid phase change is a defensible simplification for the current fixed-volume, fixed-mass, no-transport candidate. | Bounded candidate | TF-E12 + TF-E14 | Explicitly excludes physical shrinkage/expansion | Candidate reference profile |
| TF-E16 | Reference density is tied to a declared reference condition/configuration rather than being an arbitrary phase-dependent scalar. | Verified | COMSOL material-frame reference-density semantics | Does not determine one universal numerical value | `rho_ref` semantics |
| TF-E17 | Using solid density as `rho_ref` is an established implementation convention. | Verified | OpenFOAM `solidificationMeltingSource` | Precedent only; not universal | Candidate `rho_ref` provenance |
| TF-E18 | Thermodynamic energy relations require explicit reference temperature and reference-energy semantics. | Verified | OpenFOAM `hConstThermo` / `eConstThermo`; Fluent; COMSOL | Exact datum convention remains selectable | Dataset normalization |
| TF-E19 | A common additive energy offset preserves nonreacting energy differences when all coupled relations use the same datum. | Supported inference | TF-E18 + thermodynamic difference identity | Requires reference-compatible material relations | Canonical normalization rule |
| TF-E20 | Independent phase-energy offsets can corrupt latent-heat differences or phase-energy consistency. | Verified | Ansys CFX phase-change guidance; Fluent phase-change enthalpy | Applies where phase enthalpies/latent heat are coupled | Cross-phase normalization |
| TF-E21 | Density-reference temperature and energy-reference temperature have distinct semantics even if a formulation chooses the same numerical value. | Supported inference | COMSOL reference-density and reference-enthalpy semantics | May later be intentionally unified | Reference-state parameterization |
| TF-E22 | Persistent/Derived Thermodynamic State classification is formulation-relative under the existing ThermoCore Framework. | Verified | ThermoCore `Thermodynamic_State.md`; OpenFOAM energy-primary and COMSOL temperature-primary precedents | Framework semantics remain unchanged | State-profile selection |
| TF-E23 | OpenFOAM provides direct precedent for recovering Temperature from enthalpy/internal energy. | Verified | OpenFOAM `THE(he,p,T0,...)` | Recovery depends on complete thermophysical relation | Temperature derivation support |
| TF-E24 | COMSOL apparent-heat-capacity phase fractions are defined through a configured phase-transition function of Temperature. | Verified | COMSOL Apparent Heat Capacity Method | Equilibrium-like / configured transition law | Phase-fraction derivation support |
| TF-E25 | For the bounded energy-coordinate branch, one independent thermodynamic energy coordinate is the strongest Persistent-State candidate. | Bounded candidate | Framework minimal-persistence rule + TF-E05 + TF-E23 | Energy kind/basis selected only for bounded reference branch | Candidate state profile |
| TF-E26 | For the bounded energy-coordinate branch, Temperature may be Derived State when uniquely recoverable from Persistent State and Configuration. | Bounded candidate | TF-E23 + Framework Derived-State semantics | Requires unique constitutive inversion | Candidate state profile |
| TF-E27 | For the bounded equilibrium-like energy-coordinate branch, Phase Fraction may be Derived State when uniquely recoverable from energy/Temperature and the configured phase law. | Bounded candidate | TF-E24 + Framework Derived-State semantics | Does not extend to history-dependent phase behavior | Candidate state profile |
| TF-E28 | Caching, buffering, or iteratively solving a quantity does not by itself make it Framework Persistent State. | Supported inference | ThermoCore `Thermodynamic_State.md` | Semantic classification, not memory-layout rule | Implementation boundary |
| TF-E29 | Hysteresis, kinetic phase evolution, metastability, or other history dependence may require additional Persistent State. | Supported inference | Framework minimal-persistence criterion | Outside current bounded equilibrium-like candidate | Extension/future formulation boundary |
| TF-E30 | Temperature is a valid primary numerical variable in established apparent-heat-capacity formulations, so ThermoCore cannot universally require Temperature to be Derived. | Verified | COMSOL Heat Transfer interface + Apparent Heat Capacity Method | Alternative formulation precedent | Protects formulation neutrality |
| TF-E31 | Established enthalpy methods support both isothermal latent-heat evolution and finite-temperature-range/mushy phase-change formulations. | Verified | Voller et al.; Voller & Prakash; Fluent; COMSOL | Does not make either closure universal | Phase-closure selection |
| TF-E32 | For a pure-substance-like isothermal closure, latent-energy progress can be represented by a finite interval in enthalpy while Temperature remains at one configured transition temperature. | Bounded candidate | TF-E05 + TF-E31 + closure study | Minimal reference branch only; not alloys/physical mushy ranges | `h -> T` / `h -> phi` closure |
| TF-E33 | If `c_s(T) > 0`, `c_l(T) > 0`, and `L > 0`, the selected piecewise sensible/latent enthalpy relation provides a single-valued `h -> T` mapping over its declared material range. | Supported inference | Monotonic enthalpy branches + closure study | Validity conditions explicit; numerical inversion remains Verification | Closes TF-G06 at research level |
| TF-E34 | Within the selected latent enthalpy interval, `phi = (h - h_s*) / L` is bounded, continuous in `h`, single-valued, and history-independent. | Supported inference | Isothermal latent-enthalpy closure | Does not extend to hysteretic/kinetic phase behavior | Closes TF-G07 at research level |
| TF-E35 | A numerical transition-temperature smoothing width is not required by the selected physical isothermal closure and, if introduced by an implementation, must remain an implementation approximation rather than material evidence. | Supported inference | COMSOL regularization distinction + closure selection | Physical finite-width transitions remain separate valid formulations | Verification boundary |
| TF-E36 | The bounded enthalpy closure introduces no evolving runtime pressure state; any constant pressure-volume contribution consistent with its enthalpy definition can be absorbed into the common enthalpy datum. | Bounded candidate | TF-E01 + prior pressure/work exclusion + closure study | No claim about variable-pressure formulations | Pressure-scope consistency |

## 5. Consolidated Bounded Candidate

The completed evidence and gap-closure chain supports the following **bounded research candidate** for a later non-Framework reference formulation:

```text
Geometry / mass:
  fixed cell geometry
  fixed per-cell mass
  no mass transport
  no shrinkage / expansion

Density:
  one constant rho_ref across solid/liquid phase change
  explicit rho_ref provenance and T_rho_ref

Persistent Thermodynamic State candidate:
  specific enthalpy h [J/kg]

Energy reference:
  explicit T_E_ref
  h_ref = 0 J/kg at T_E_ref
  common reference-compatible normalization across phase relations

Pressure / work:
  no evolving runtime pressure state
  no pressure-volume or mechanical-work evolution
  any constant p/rho datum contribution handled consistently by the enthalpy reference convention

Phase closure:
  isothermal transition at configured T_m
  latent interval h_s* <= h <= h_l*
  h_l* - h_s* = L > 0

Derived Thermodynamic State candidates:
  T = inverse sensible branch outside latent interval; T = T_m inside it
  phi = 0 below h_s*
  phi = (h - h_s*) / L inside latent interval
  phi = 1 above h_l*
```

Derived-State classification remains conditional on the declared validity conditions and history-independent closure.

## 6. Research Decisions and Boundaries

### 6.1 Bounded decisions supported for specification transfer

The research chain now supports transferring the following **bounded** decisions into a reference-formulation specification:

1. Select the enthalpy family for the minimal fixed-grid solid/liquid reference formulation.
2. Use specific enthalpy `[J/kg]` as the persistent energy basis.
3. Use one constant material `rho_ref` across the modeled solid/liquid transition, with explicit provenance and reference condition.
4. Normalize the reference enthalpy to `0 J/kg` at a declared `T_E_ref` using one compatible datum across phase relations.
5. Keep `T_rho_ref` and `T_E_ref` semantically distinct even when numerically equal.
6. Use the isothermal enthalpy-jump closure for the minimal pure-substance-like reference branch.
7. Keep Temperature and liquid Phase Fraction as Derived-State candidates under the selected single-valued closure.
8. Keep physical finite-width/mushy transitions and history-dependent phase behavior outside this minimal reference profile.
9. Treat any numerical smoothing of the isothermal transition as an implementation approximation subject to Verification.

These are not Framework-wide requirements and do not invalidate other formulations used by conforming ThermoCore implementations.

### 6.2 Items intentionally not standardized by the research line

The bounded reference-formulation research does **not** establish a universal rule for:

```text
all ThermoCore energy coordinates
all material density models
all phase-transition models
all pressure/compressibility formulations
all history-dependent transformations
numerical inversion algorithms
GPU/data layout
benchmark pass thresholds
```

Those remain formulation-, implementation-, or Validation-specific as applicable.

## 7. Gap Disposition Status

| Gap ID | Final disposition | Evidence / decision result |
|---|---|---|
| TF-G01 | Closed by bounded decision | Enthalpy family selected for the minimal reference formulation |
| TF-G02 | Closed by bounded decision | Specific enthalpy `[J/kg]` selected as persistent energy basis |
| TF-G03 | Closed by bounded decision | One constant material `rho_ref` with explicit provenance/reference condition |
| TF-G04 | Closed by bounded decision | `h_ref = 0 J/kg` at declared `T_E_ref` with common phase-compatible datum |
| TF-G05 | Closed by bounded decision | `T_rho_ref` and `T_E_ref` remain semantically distinct |
| TF-G06 | Closed by focused closure study | Piecewise monotonic/isothermal `h -> T` relation is single-valued under declared validity conditions |
| TF-G07 | Closed by focused closure study | `h -> phi` relation is explicit, bounded, single-valued, and history-independent |
| TF-G08 | Downstream Verification / Validation | Energy/recovery invariants and benchmarks must be tested after specification and implementation |

No pre-specification research gap remains open for the bounded reference-formulation branch.

## 8. Readiness Assessment

### Framework Specification

```text
Change required: No
Freeze reopen: No
```

The completed research identifies no Framework-level defect requiring modification.

### Reference-formulation specification

```text
Research readiness: READY FOR AUTHORIZATION
```

Reason:

- energy-coordinate family and basis have bounded decisions;
- density/reference-state semantics have bounded decisions;
- Persistent/Derived State candidates are defined relative to the selected formulation;
- `h -> T` and `h -> phi` closures are explicit and single-valued under declared validity conditions;
- remaining benchmark work is correctly downstream Verification / Validation rather than missing pre-specification evidence.

This Evidence Matrix does not itself create the authoritative formulation specification. It establishes that the bounded research line no longer contains an unresolved pre-specification evidence gap.

## 9. Next Governance Step

Update and close the corresponding Research Gap Analysis using the focused closure study.

If the Research Gap review confirms that no pre-specification blocker remains, the project may authorize a **non-Framework reference-formulation specification** named `Thermodynamic_Formulation.md` (or repository-equivalent path) whose scope is subordinate to the frozen Framework semantics without becoming part of the Framework Specification hierarchy.

The specification must preserve the bounded/non-universal status of the selected formulation and must carry forward TF-G08 as later Verification / Validation obligations.

## 10. Current Decision

**The thermodynamic-formulation Research → Evidence chain is complete for the bounded fixed-grid reference-formulation question.**

**All pre-specification formulation gaps represented by TF-G01 through TF-G07 are closed for this bounded branch.**

**TF-G08 remains a downstream Verification / Validation obligation.**

**Do not modify or reopen the Framework Specification.**

**The reference-formulation research is ready for authorization review.**