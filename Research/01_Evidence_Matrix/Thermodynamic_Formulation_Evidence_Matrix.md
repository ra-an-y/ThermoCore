# Thermodynamic Formulation Evidence Matrix

Status: Evidence Matrix  
Scope: Research only — non-normative  
Research line: Bounded fixed-grid thermodynamic reference-formulation investigation

---

## 1. Purpose

This document synthesizes the completed thermodynamic-formulation survey chain into an evidence layer.

It does not create Framework requirements, does not authorize a concrete `Thermodynamic_Formulation.md`, and does not reopen the frozen Framework Specification.

The matrix separates:

- directly supported source evidence;
- analytical consequences derived from that evidence;
- bounded formulation candidates;
- unresolved research gaps.

## 2. Source Research Artifacts

The matrix is derived from the following completed survey/comparison artifacts:

1. `Research/00_Literature_Survey/Thermodynamic_Formulation_Survey.md`
2. `Research/00_Literature_Survey/Enthalpy_Internal_Energy_Primary_Source_Comparison.md`
3. `Research/00_Literature_Survey/Specific_Volumetric_Energy_Source_Mapping_Comparison.md`
4. `Research/00_Literature_Survey/Fixed_Grid_Density_Mass_Volume_Phase_Change_Comparison.md`
5. `Research/00_Literature_Survey/Reference_Density_Energy_Reference_State_Comparison.md`
6. `Research/00_Literature_Survey/Persistent_Derived_State_Classification_Comparison.md`

The underlying source set includes official OpenFOAM, COMSOL, Ansys Fluent, and Ansys CFX documentation together with the primary phase-change literature recorded in the source artifacts.

## 3. Evidence Status Vocabulary

| Status | Meaning |
|---|---|
| **Verified** | Directly supported by a primary or authoritative technical source recorded in the survey chain |
| **Supported inference** | Analytical consequence derived from Verified evidence with explicit scope assumptions |
| **Bounded candidate** | A formulation choice that is defensible under the current bounded model but is not yet frozen |
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
| TF-E25 | For the bounded energy-coordinate branch, one independent thermodynamic energy coordinate is the strongest Persistent-State candidate. | Bounded candidate | Framework minimal-persistence rule + TF-E05 + TF-E23 | Energy kind/basis still unresolved | Candidate state profile |
| TF-E26 | For the bounded energy-coordinate branch, Temperature may be Derived State when uniquely recoverable from Persistent State and Configuration. | Bounded candidate | TF-E23 + Framework Derived-State semantics | Requires unique constitutive inversion | Candidate state profile |
| TF-E27 | For the bounded equilibrium-like energy-coordinate branch, Phase Fraction may be Derived State when uniquely recoverable from energy/Temperature and the configured phase law. | Bounded candidate | TF-E24 + Framework Derived-State semantics | Does not extend to history-dependent phase behavior | Candidate state profile |
| TF-E28 | Caching, buffering, or iteratively solving a quantity does not by itself make it Framework Persistent State. | Supported inference | ThermoCore `Thermodynamic_State.md` | Semantic classification, not memory-layout rule | Implementation boundary |
| TF-E29 | Hysteresis, kinetic phase evolution, metastability, or other history dependence may require additional Persistent State. | Supported inference | Framework minimal-persistence criterion | Outside current bounded equilibrium-like candidate | Extension/future formulation boundary |
| TF-E30 | Temperature is a valid primary numerical variable in established apparent-heat-capacity formulations, so ThermoCore cannot universally require Temperature to be Derived. | Verified | COMSOL Heat Transfer interface + Apparent Heat Capacity Method | Alternative formulation precedent | Protects formulation neutrality |

## 5. Consolidated Bounded Candidate

The current evidence supports the following **research candidate**, not a frozen formulation:

```text
Geometry / mass:
  fixed cell geometry
  fixed per-cell mass
  no mass transport
  no shrinkage / expansion

Density:
  one constant rho_ref across solid/liquid phase change
  explicit rho_ref provenance and reference condition

Energy reference:
  explicit T_E_ref
  explicit energy reference value
  common reference-compatible normalization across phase relations

State branch:
  one independent energy coordinate epsilon as Persistent-State candidate
  Temperature as Derived-State candidate
  Phase Fraction as Derived-State candidate
```

Derived-State classification remains conditional on unique reconstruction from Persistent State and Configuration.

## 6. Decisions Supported vs Not Yet Supported

### 6.1 Sufficiently supported boundaries

The evidence is strong enough to retain the following boundaries in subsequent research:

1. Framework-level Thermodynamic State should remain variable-neutral.
2. Concrete physical units and source mappings belong to a formulation-level artifact, not to Framework-level `Energy Input` semantics.
3. The minimal bounded reference formulation should not silently include variable-density shrinkage/expansion physics.
4. A single equal-density/reference-density treatment is a defensible candidate for the simplified fixed-grid branch.
5. Reference density and thermodynamic energy datum must be explicit and traceable.
6. Persistent/Derived classification must be evaluated relative to the selected formulation rather than assigned universally by variable name.

These are research-supported boundaries. They are not new Framework Specification requirements.

### 6.2 Decisions not yet supported for freeze

The evidence matrix does **not** yet justify freezing:

```text
enthalpy vs internal energy
specific vs volumetric persistent coordinate
exact numerical/provenance rule for rho_ref
zero-reference vs another documented energy offset
one shared vs separate density/energy reference temperatures
exact phase-transition regularization
benchmark acceptance criteria
```

## 7. Research Gaps Carried Forward

| Gap ID | Open question | Why unresolved | Required next evidence / decision |
|---|---|---|---|
| TF-G01 | Enthalpy or internal energy for the bounded reference formulation? | Both are physically valid; enthalpy has stronger fixed-grid phase-change precedent while internal energy aligns directly with first-law accumulation | Explicit bounded-assumption comparison and selection criterion |
| TF-G02 | Specific `[J/kg]` or volumetric `[J/m^3]` persistent coordinate? | Both can be mapped conservatively under constant `rho_ref` | Selection criterion based on material data, source mapping, conservation, and implementation independence |
| TF-G03 | What exact `rho_ref` convention should the simplified profile use? | Solid-density and reference-temperature conventions both have precedent | Documented selection rule and representative material evidence |
| TF-G04 | What energy datum convention should be standardized? | Zero-reference and arbitrary documented offsets are both valid if consistent | Decide interoperability/normalization policy |
| TF-G05 | Should `T_rho_ref` and `T_E_ref` be numerically unified? | Semantically distinct but may be unified for simplicity | Evaluate simplification benefit vs provenance clarity |
| TF-G06 | Is Temperature recovery unique and stable over the intended material range? | Depends on selected energy relation and transition model | Constitutive inversion analysis + benchmark |
| TF-G07 | Is Phase Fraction recovery unique over the selected phase relation? | Depends on transition regularization/history assumptions | Explicit phase law + counterexample testing |
| TF-G08 | Does the bounded profile conserve energy and recover Temperature/Phase consistently in representative tests? | No formulation benchmark has yet frozen the candidate | Reference benchmarks and validation criteria |

## 8. Readiness Assessment

### Framework Specification

```text
Change required: No
Freeze reopen: No
```

The completed survey/evidence line identifies no Framework-level defect requiring modification.

### Reference-formulation specification

```text
Authorization status: NOT YET READY
```

Reason:

- architecture and formulation boundaries are now well supported;
- the candidate state/density/reference semantics are narrowed;
- however the independent energy coordinate, coordinate basis, exact reference conventions, and benchmark evidence remain unresolved.

A concrete `Thermodynamic_Formulation.md` should therefore not yet be authored as an authoritative formulation specification.

## 9. Next Research Step

Proceed to a focused Research Gap / Decision Analysis for `TF-G01` through `TF-G08`.

The next artifact should determine which gaps require additional external evidence and which can be closed by an explicit bounded modeling decision supported by the present matrix.

Only after that gap analysis should the project consider authorizing a reference-formulation specification.

## 10. Current Decision

**The thermodynamic-formulation Preliminary Survey chain is complete enough to transition into the Evidence / Research-Gap stage for this bounded research question.**

**Do not modify the Framework Specification.**

**Do not yet authorize `Thermodynamic_Formulation.md`.**