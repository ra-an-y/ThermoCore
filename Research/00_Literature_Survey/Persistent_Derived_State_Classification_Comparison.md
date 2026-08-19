# Persistent vs Derived Thermodynamic State — Formulation Classification

Status: Primary-Source / Framework-Alignment Comparison  
Scope: Research only — non-normative  
Target: Persistent/Derived State evidence gap from `Reference_Density_Energy_Reference_State_Comparison.md`

---

## 1. Research Question

Under the deliberately bounded fixed-grid thermal/phase-change candidate developed by the preceding research chain, which thermodynamic quantities must be classified as **Persistent State** and which may be **Derived State** without losing the thermodynamic condition required for subsequent state evolution?

The quantities examined here are:

- a thermodynamic energy coordinate `epsilon`;
- Temperature `T`;
- Phase Fraction `phi`.

This comparison does **not** modify the ThermoCore Framework Specification. It does not freeze enthalpy versus internal energy, specific versus volumetric storage, or a concrete implementation layout.

## 2. Governing Framework Semantics

The authoritative ThermoCore `Thermodynamic_State.md` defines:

- **Persistent State** as information maintained as part of evolving Runtime State and required to preserve the conforming thermodynamic condition across state evolution;
- **Derived State** as information derived from Persistent State when required;
- the specific assignment of quantities to either classification as dependent on the thermodynamic formulation and later applicable specifications.

The Framework specification also makes this distinction semantic rather than a prescription of storage duration, storage location, memory layout, or implementation structure.

**Framework-alignment consequence:** A quantity does not become Persistent State merely because an implementation caches, buffers, iterates, or numerically solves it. Persistent classification requires semantic necessity for preserving the thermodynamic condition across evolution.

Internal source:

- `Documentation/Framework_Specification/Thermodynamic_State.md`

## 3. Bounded Candidate Assumptions

This comparison inherits the current research candidate, without freezing it:

```text
fixed cell geometry
fixed per-cell mass
one constant rho_ref across solid/liquid phase change
no mass transport
no shrinkage/expansion
no mechanical work state
no free-surface or moving-mesh state
explicit thermodynamic energy datum
nonreacting thermal/phase-change scope
```

The present comparison intentionally keeps the energy coordinate abstract:

```text
epsilon = selected thermodynamic energy coordinate
```

It may later become enthalpy or internal energy, and may later be represented on a specific or volumetric basis.

## 4. Two Established Numerical Formulation Patterns

The source set demonstrates that thermodynamic software need not choose the same primary numerical variable.

### 4.1 Energy-coordinate pattern

OpenFOAM `heThermo` exposes a thermodynamic field `he` described as enthalpy/internal energy `[J/kg]` and provides `THE(he, p, T0, ...)` to obtain temperature from enthalpy/internal energy.

This directly demonstrates a formulation pattern in which an energy quantity is available as the thermodynamic coordinate and temperature is recoverable from it through the thermophysical relation.

Source:

- OpenFOAM official source, `heThermo.H`:  
  https://api.openfoam.com/2606/heThermo_8H_source.html

### 4.2 Temperature-primary apparent-heat-capacity pattern

COMSOL Heat Transfer interfaces use Temperature `T` as the dependent variable. In its Apparent Heat Capacity Method, phase change is captured by one heat-transfer equation whose heat capacity is modified to include latent heat.

COMSOL also defines phase volume fractions through a phase-transition function `alpha_1->2(T)` over the transition interval.

**Evidence-supported conclusion:** Temperature can be the maintained solved quantity in one valid formulation, while an energy quantity can be the maintained thermodynamic coordinate in another valid formulation.

Sources:

- COMSOL Multiphysics 6.4, *Settings for the Heat Transfer Interface*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_interfaces.08.26.html
- COMSOL Multiphysics 6.4, *Apparent Heat Capacity Method*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.024.html

**Framework implication:** Persistent/Derived classification is formulation-relative. The external evidence does not support a universal rule that Temperature is always Persistent or always Derived.

## 5. Energy Coordinate as Persistent-State Candidate

For the energy-coordinate branch of the bounded ThermoCore candidate, let:

```text
epsilon_n
```

represent the thermodynamic energy condition before an update, and let an accepted energy increment produce:

```text
epsilon_(n+1) = F(epsilon_n, Energy Input, Material Definition, ...)
```

If the next thermodynamic condition depends on the accumulated value of `epsilon`, then removing that value between updates loses information required to continue evolution.

**Analytical classification:** In an energy-coordinate formulation, the independent energy coordinate is a **Persistent State candidate** because it preserves accumulated thermodynamic condition across state evolution.

This conclusion does not yet decide whether the coordinate is:

- enthalpy or internal energy;
- specific `[J/kg]` or volumetric `[J/m^3]`;
- stored directly or represented through another implementation mechanism.

### 5.1 Latent-energy precedent strengthens the energy-coordinate candidate

Ansys Fluent defines material enthalpy as sensible enthalpy plus latent heat content and writes the solidification/melting energy equation in terms of enthalpy. Liquid fraction participates in the latent contribution, and temperature is obtained through iteration between the energy equation and the liquid-fraction relation.

**Evidence-supported conclusion:** For an enthalpy-based phase-change formulation, accumulated energy contains thermodynamic information that is not equivalent to storing temperature alone.

Source:

- Ansys Fluent Theory Guide, *Solidification and Melting — Energy Equation*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v242/en/flu_th/flu_th_sec_melt_theory_energy.html

## 6. Temperature as Derived-State Candidate in an Energy-Coordinate Formulation

OpenFOAM explicitly provides a mapping from enthalpy/internal energy to temperature:

```text
T = THE(he, p, T0, ...)
```

For the bounded ThermoCore candidate, pressure evolution is currently outside scope. If the selected constitutive relation and material configuration provide a deterministic temperature recovery from the persistent energy coordinate, then:

```text
T = T(epsilon; Material Definition, reference state)
```

**Analytical classification:** Temperature may be **Derived State** when it is uniquely recoverable from Persistent State and Configuration.

### 6.1 Numerical iteration does not automatically make Temperature persistent

OpenFOAM's temperature-recovery function accepts `T0` as a starting temperature for inversion. A numerical initial guess, cached temperature, predictor value, or convergence aid may need to survive an implementation step for numerical reasons.

That alone does not establish Framework-level Persistent State semantics.

**Framework-alignment rule:** Numerical convenience state must not be promoted to Persistent State unless the thermodynamic condition itself cannot be preserved or reconstructed without it.

### 6.2 Temperature-primary formulations remain valid alternatives

COMSOL demonstrates a valid formulation where Temperature `T` is the dependent variable of the heat-transfer equation.

Therefore this comparison does **not** conclude that Temperature is universally Derived State. It concludes only that Temperature is a Derived-State candidate for the energy-coordinate formulation branch when the inversion is complete and single-valued.

## 7. Phase Fraction as Derived-State Candidate

COMSOL's apparent-heat-capacity method represents phase fractions through a phase-transition function of temperature. In that formulation:

```text
phi = alpha(T)
```

and the phase fractions are therefore algebraically determined by the temperature and the configured transition law.

Source:

- COMSOL Multiphysics 6.4, *Apparent Heat Capacity Method*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.024.html

Fluent similarly defines liquid fraction through a phase-change relation and couples its update to the enthalpy/temperature solution.

Source:

- Ansys Fluent Theory Guide, *Solidification and Melting — Energy Equation*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v242/en/flu_th/flu_th_sec_melt_theory_energy.html

**Analytical classification:** Phase Fraction may be **Derived State** when the selected phase relation makes it a unique function of the persistent thermodynamic coordinate, directly or through derived Temperature.

Possible bounded forms are:

```text
phi = Phi(T)
```

or

```text
phi = Phi(epsilon)
```

provided the mapping is uniquely defined by the formulation and Material Definition.

## 8. Why Temperature Alone Is Not a Sufficient Universal Persistent Coordinate

Phase-change formulations expose an important distinction between sensible temperature and stored latent energy.

Fluent's material enthalpy contains both sensible enthalpy and latent heat content. The liquid fraction controls the latent contribution, and the temperature solution is coupled to that energy relation.

COMSOL's apparent-heat-capacity theory likewise identifies the ideal pure-substance case as an enthalpy jump at the phase-change temperature and approximates it over a finite interval for numerical treatment.

**Analytical consequence:** A single temperature value does not universally identify how much latent energy has been accumulated through a phase transition. Whether Temperature alone is sufficient depends on the selected formulation and phase-transition regularization.

Sources:

- Ansys Fluent Theory Guide, *Solidification and Melting — Energy Equation*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v242/en/flu_th/flu_th_sec_melt_theory_energy.html
- COMSOL Multiphysics 6.4, *Apparent Heat Capacity Method*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.024.html

This is one reason the current ThermoCore research should distinguish an energy-primary reference formulation from a temperature-primary apparent-heat-capacity formulation rather than mixing their state semantics.

## 9. Uniqueness Is the Governing Criterion for Derived State

A quantity can safely remain Derived State only if the persistent condition plus applicable Configuration determines it without unresolved history.

For a candidate derived quantity `y`:

```text
y = G(Persistent State, Configuration)
```

must be sufficiently defined for the selected formulation that two physically distinct admissible thermodynamic conditions do not map to the same persistent state while requiring different `y` values.

### 9.1 When Phase Fraction would need Persistent classification

The current bounded research assumes equilibrium-like phase relations without explicit hysteresis or kinetic history.

If a future formulation introduces any mechanism where the same energy/temperature condition can correspond to different phase fractions because of history, such as:

```text
hysteresis
supercooling / superheating history
metastability
kinetic phase evolution
irreversible transformation history
```

then Phase Fraction or another history variable may become semantically necessary Persistent State.

**Analytical boundary:** The present Derived-State candidate for Phase Fraction does not extend automatically to history-dependent or nonequilibrium phase-change formulations.

### 9.2 When Temperature would need additional state support

If temperature recovery from the selected energy coordinate is not unique without another thermodynamic variable, then the missing independent quantity must be represented by Persistent State or by an explicitly supplied non-state condition that makes the mapping complete.

The current bounded candidate avoids pressure, composition, and mechanical-state evolution, so this comparison does not resolve those broader formulations.

## 10. Semantic Persistence vs Implementation Storage

The following implementation choices do **not** by themselves change Framework classification:

```text
caching derived Temperature
buffering Phase Fraction on the GPU
storing both old and new Temperature for a time integrator
keeping an iterative liquid-fraction estimate
memoizing a constitutive inversion
maintaining lookup-table indices
```

These may be useful or necessary implementation data.

They become Framework Persistent State only if the information is thermodynamically necessary to preserve the evolving condition rather than recomputable from Persistent State and Configuration.

**Research conclusion:** Minimal Persistent State is a semantic minimality rule, not a command to minimize every allocated runtime buffer.

## 11. Classification Matrix

| Quantity | Energy-coordinate formulation | Temperature-primary apparent-`Cp` formulation | Classification condition |
|---|---|---|---|
| Thermodynamic energy coordinate `epsilon` | Persistent candidate | May be derived/integrated quantity | Persistent when it is the independent accumulated thermodynamic coordinate required for continued evolution |
| Temperature `T` | Derived candidate | Persistent candidate | Derived when uniquely recoverable from independent persistent thermodynamic state |
| Phase Fraction `phi` | Derived candidate | Derived candidate under configured `alpha(T)` | Derived only when uniquely determined by persistent state + configuration |
| Numerical inversion seed / cached `T` | Implementation auxiliary | Implementation auxiliary or solver value | Not Persistent solely because it is stored |
| History variable for hysteresis/kinetics | Outside current candidate | Outside current candidate | Would become Persistent if required to distinguish admissible thermodynamic conditions |

The matrix compares formulation semantics; it does not prescribe storage layout.

## 12. Minimality Test for the Bounded Energy-Coordinate Candidate

For each candidate state quantity, apply the following test:

### Test A — Remove the energy coordinate

If `epsilon` is removed and only Temperature/Phase Fraction remain, can the full accumulated thermodynamic condition required for the next energy update always be reconstructed?

For the enthalpy-style phase-change precedent, not universally. Latent energy is part of the energy coordinate.

**Result:** Energy coordinate remains a Persistent-State candidate.

### Test B — Remove stored Temperature

If `epsilon` and Material Definition remain, can Temperature be recovered through the constitutive inversion?

OpenFOAM provides direct precedent for temperature recovery from enthalpy/internal energy.

**Result:** Temperature may remain Derived State in the energy-coordinate candidate.

### Test C — Remove stored Phase Fraction

If `epsilon`, derived `T`, and the configured phase relation remain, can Phase Fraction be recovered uniquely?

COMSOL and Fluent provide phase relations tying phase fraction to the thermal/energy solution.

**Result:** Phase Fraction may remain Derived State for the bounded equilibrium-like candidate, subject to the uniqueness criterion.

## 13. Preliminary Findings

### F-01 — Persistent/Derived classification is formulation-relative

Temperature-primary and energy-primary formulations both exist in established software.

Status: **Supported by authoritative/primary technical sources and current Framework semantics**

### F-02 — One independent energy coordinate is the strongest Persistent-State candidate for the bounded energy-primary branch

It preserves accumulated thermodynamic condition, including latent-energy effects in enthalpy-style phase-change formulations.

Status: **Supported research candidate — energy kind/basis not yet frozen**

### F-03 — Temperature can be Derived State in an energy-coordinate formulation

OpenFOAM provides an explicit temperature-from-enthalpy/internal-energy recovery interface.

Status: **Supported implementation precedent + formulation inference**

### F-04 — Phase Fraction can be Derived State when the phase relation is single-valued

COMSOL explicitly defines phase fractions through a configured phase-transition function; Fluent couples liquid fraction to the phase-change energy solution.

Status: **Supported bounded candidate**

### F-05 — Caching or numerically solving a quantity does not by itself make it Persistent State

Framework Persistent State is defined by semantic necessity across evolution, not implementation storage.

Status: **Direct Framework-alignment conclusion**

### F-06 — History dependence is the principal boundary on minimal derived phase state

If phase behavior is hysteretic, kinetic, metastable, or otherwise history-dependent, an additional persistent history/phase variable may be required.

Status: **Analytical boundary — outside current bounded candidate**

### F-07 — Temperature alone is not a universal substitute for an energy coordinate through phase change

Latent-energy content can distinguish thermodynamic conditions that a temperature-only description may not distinguish without a formulation-specific regularization or auxiliary relation.

Status: **Supported by phase-change enthalpy/apparent-heat-capacity evidence + analytical consequence**

## 14. Candidate State Profile

For the bounded **energy-coordinate** reference-formulation branch, the strongest current candidate is:

```text
Persistent Thermodynamic State:
  epsilon

Derived Thermodynamic State:
  Temperature T
  Phase Fraction phi
```

subject to:

```text
T = uniquely recoverable from epsilon + Material Definition + reference semantics
phi = uniquely recoverable from epsilon/T + configured phase relation
```

The profile intentionally does **not** state whether `epsilon` is:

```text
enthalpy or internal energy
specific or volumetric
```

Those remain separate formulation decisions.

The profile also does not prohibit implementations from caching `T` or `phi`.

## 15. Framework Alignment

This candidate preserves the frozen Framework rules:

- Thermodynamic State remains Runtime State;
- Persistent State remains the maintained basis of evolving Runtime State;
- Derived State remains semantically dependent on Persistent State;
- Material Definition remains Configuration;
- Thermodynamic Computation remains the only Core responsibility that evolves/writes Thermodynamic State;
- no new Framework Core component or Interface semantic is introduced.

No Framework Specification change is indicated by this comparison.

## 16. Remaining Evidence Gaps

After this comparison, the research line is substantially narrower. Before authorizing a concrete `Thermodynamic_Formulation.md`, the remaining decisions/evidence needs are:

1. choose the reference energy coordinate: enthalpy or internal energy;
2. choose the coordinate basis: specific or volumetric;
3. choose the exact `rho_ref` provenance/reference condition for the simplified equal-density profile;
4. decide whether the minimal profile standardizes `epsilon_ref = 0` at `T_E_ref` or permits another documented datum;
5. demonstrate that the selected constitutive relation provides stable and unique Temperature recovery over the intended material range;
6. demonstrate that Phase Fraction recovery is unique over the selected transition model;
7. validate energy conservation and recovery consistency with representative fixed-grid phase-change benchmarks.

These are formulation/evidence decisions, not reasons to reopen the Framework Specification.

## 17. Current Decision

**Do not modify the Framework Specification.**

**Do not classify Temperature or Phase Fraction as universally Persistent or universally Derived across all ThermoCore formulations.**

For the current bounded **energy-coordinate candidate**, adopt the following research position:

```text
energy coordinate epsilon -> Persistent-State candidate
Temperature T            -> Derived-State candidate
Phase Fraction phi       -> Derived-State candidate
```

with Derived classification conditional on unique reconstruction from Persistent State and Configuration.

The next repository step should synthesize the completed formulation-survey chain into the Evidence layer and determine which remaining formulation choices are sufficiently supported to become an authorized reference-formulation specification.