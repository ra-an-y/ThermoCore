# Enthalpy–Temperature–Phase Closure Study

Status: Focused Closure Study  
Scope: Research only — non-normative  
Target: `TF-G06` and `TF-G07` from `Thermodynamic_Formulation_Research_Gap_Analysis.md`

---

## 1. Research Question

For the bounded fixed-grid, fixed-mass, equal-density reference-formulation candidate, what enthalpy–temperature–phase relation can provide:

- a single persistent specific-enthalpy coordinate `h [J/kg]`;
- unique recovery of Temperature `T` from `h`;
- unique recovery of Phase Fraction `phi` from `h`;
- explicit latent-heat accounting;
- no silent promotion of a numerical smoothing width into physical material evidence?

This study does **not** modify the ThermoCore Framework Specification and does not define implementation algorithms, memory layouts, GPU kernels, or Validation results.

## 2. Evidence Basis

The closure study relies on established phase-change evidence already recorded by the research line.

### 2.1 Enthalpy supports latent heat at an isothermal temperature or over a range

Voller, Cross, and Markatos describe an enthalpy formulation applicable both when latent heat evolves at one isothermal transition temperature and when it evolves over a temperature range.

Source:

- Voller, V. R., Cross, M., and Markatos, N. C. (1987), *An enthalpy method for convection/diffusion phase change*, DOI `10.1002/nme.1620240119`.

### 2.2 Fluent couples sensible enthalpy, latent heat, and liquid fraction

Ansys Fluent defines material enthalpy as sensible enthalpy plus latent heat content. Liquid fraction determines the latent contribution, and the solidification/melting energy equation evolves enthalpy.

Fluent also distinguishes the general solidus/liquidus interval case from the pure-metal case where solidus and liquidus temperatures coincide.

Source:

- Ansys Fluent Theory Guide, *Solidification and Melting — Energy Equation*:  
  https://ansyshelp.ansys.com/public/Views/Secured/corp/v242/en/flu_th/flu_th_sec_melt_theory_energy.html

### 2.3 COMSOL identifies an ideal enthalpy jump for a pure substance

COMSOL's Apparent Heat Capacity Method states that, in the ideal Heaviside limit, latent heat becomes an enthalpy jump `L` at the phase-change temperature `T_pc` for a pure substance.

COMSOL then regularizes this ideal behavior over a finite transition interval for its apparent-heat-capacity formulation and supports either a smooth Heaviside or a linear phase-transition function.

Source:

- COMSOL Multiphysics 6.4, *Apparent Heat Capacity Method*:  
  https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.024.html

### 2.4 Fixed-grid enthalpy methods also support mushy-region formulations

Voller and Prakash provide a fixed-grid enthalpy methodology for convection-diffusion mushy-region phase-change problems.

Source:

- Voller, V. R., and Prakash, C. (1987), *A fixed grid numerical modelling methodology for convection-diffusion mushy region phase-change problems*, DOI `10.1016/0017-9310(87)90317-6`.

**Evidence conclusion:** Both isothermal and finite-temperature-range phase-change closures are established. The research question is therefore not whether one is universally valid, but which is the cleanest bounded reference closure for the current energy-coordinate branch.

## 3. Closure Options

### 3.1 Option A — Finite transition interval with `phi(T)`

A finite interval may define:

```text
T_s < T_l
```

and a monotonic phase relation:

```text
phi = Phi(T)
```

with:

```text
phi = 0  below T_s
0 < phi < 1  between T_s and T_l
phi = 1  above T_l
```

The enthalpy relation then includes both sensible enthalpy and latent contribution through `phi(T)`.

This option is compatible with apparent-heat-capacity and mushy-zone formulations.

**Strength:** smooth or continuous Temperature evolution can be obtained over a finite interval.

**Boundary:** the interval must be semantically classified. It may represent a real physical solidus/liquidus interval, or it may be a numerical regularization. Those meanings must not be conflated.

### 3.2 Option B — Isothermal enthalpy jump with `phi(h)`

For a pure-substance-like bounded reference formulation, phase change may occur at one configured phase-change temperature:

```text
T_m
```

with a latent-enthalpy interval of width:

```text
L [J/kg]
```

in the persistent enthalpy coordinate.

During that enthalpy interval:

```text
T = T_m
```

while Phase Fraction changes continuously from 0 to 1 as latent enthalpy is accumulated.

**Strength:** no artificial temperature-width parameter is required to distinguish different latent-energy states.

**Boundary:** this models an isothermal solid/liquid transition and is not a universal model for alloys or materials with a physical mushy interval.

## 4. Selection Criterion

The current ThermoCore reference-formulation branch is intended to be:

```text
minimal
energy-coordinate based
fixed-grid
fixed-mass
nonreacting
solid/liquid phase change
implementation-independent
```

The reference closure should therefore minimize additional physical assumptions while preserving thermodynamic distinguishability.

A finite artificial transition interval would introduce another formulation parameter whose physical or numerical meaning must be established for every material.

An isothermal enthalpy-jump closure instead uses the persistent enthalpy coordinate itself to distinguish latent-energy progress while Temperature remains at the configured transition temperature.

**Research selection:** Option B is the stronger minimal reference closure for the current bounded branch.

Finite physical mushy-zone formulations remain valid later alternatives.

## 5. Selected Bounded Closure — Definitions

Let:

```text
h      = specific enthalpy [J/kg]
T      = Temperature [K]
phi    = liquid Phase Fraction [0,1]
T_m    = configured phase-change temperature [K]
L      = latent heat of fusion [J/kg], L > 0
c_s(T) = solid sensible heat capacity [J/(kg*K)]
c_l(T) = liquid sensible heat capacity [J/(kg*K)]
```

The current bounded equal-density convention remains:

```text
rho = rho_ref = constant
```

and does not enter the local `h -> (T, phi)` inversion except when mapping between specific and volumetric quantities.

## 6. Common Energy Datum

The prior gap analysis selected a common reference-enthalpy convention.

To keep the phase branches reference-compatible, define a raw enthalpy relation first and apply one common additive offset afterward.

Let the solid sensible enthalpy branch be:

```text
h_s(T)
```

and define its value at phase change as:

```text
h_s_star = h_s(T_m)
```

The fully liquid enthalpy immediately after completion of phase change is:

```text
h_l_star = h_s_star + L
```

The liquid branch is then anchored to `h_l_star`, preserving exactly one latent-heat jump `L` between the phase branches.

A common additive offset `C` may then normalize the entire relation so that:

```text
h = 0 J/kg
```

at the declared energy-reference condition.

**Constraint:** the same offset must be applied to every branch. Independent solid/liquid offsets are not permitted in the selected candidate because they would alter the latent-energy difference.

## 7. Sensible-Enthalpy Branches

A general bounded relation may use positive heat-capacity functions.

For the solid branch:

```text
h_s(T2) - h_s(T1)
= integral(T1 -> T2) c_s(theta) dtheta
```

For the liquid branch above `T_m`:

```text
h_l(T)
= h_l_star + integral(T_m -> T) c_l(theta) dtheta
```

The closure requires, over the supported material range:

```text
c_s(T) > 0
c_l(T) > 0
```

These are formulation validity conditions for monotonic sensible enthalpy, not universal claims about every possible thermodynamic system.

Under these conditions, each sensible branch is strictly increasing in Temperature and therefore admits at most one Temperature for a given branch enthalpy.

## 8. Exact `h -> T` Recovery

Define the two phase-change thresholds:

```text
h_s_star
h_l_star = h_s_star + L
```

The selected Temperature closure is:

```text
if h < h_s_star:
    T = inverse_h_s(h)

if h_s_star <= h <= h_l_star:
    T = T_m

if h > h_l_star:
    T = inverse_h_l(h)
```

where `inverse_h_s` and `inverse_h_l` denote inversion of the monotonic sensible-enthalpy branches.

### 8.1 Uniqueness

If:

```text
c_s(T) > 0
c_l(T) > 0
L > 0
```

then:

- the solid branch is monotonic;
- the liquid branch is monotonic;
- the latent interval is disjoint in enthalpy from both sensible branches except at its endpoints;
- every supported enthalpy value maps to exactly one Temperature.

**Result:** `TF-G06` can be closed at the formulation-research level for this bounded relation.

Numerical inversion accuracy and performance remain downstream implementation/Verification concerns.

## 9. Exact `h -> phi` Recovery

The selected liquid Phase Fraction closure is:

```text
if h < h_s_star:
    phi = 0

if h_s_star <= h <= h_l_star:
    phi = (h - h_s_star) / L

if h > h_l_star:
    phi = 1
```

This relation is:

```text
bounded:   0 <= phi <= 1
continuous in h
single-valued
history-independent
```

within the selected equilibrium-like reference formulation.

At the latent interval boundaries:

```text
phi(h_s_star) = 0
phi(h_l_star) = 1
```

**Result:** `TF-G07` can be closed at the formulation-research level for this bounded relation.

## 10. Why Phase Fraction Is Derived Rather Than Persistent Here

The persistent quantity is:

```text
h
```

and both derived quantities are unique functions of that enthalpy plus Material Definition:

```text
T   = T(h; material)
phi = Phi(h; material)
```

No history variable is required by the selected closure.

Therefore the current Framework-aligned state candidate remains:

```text
Persistent Thermodynamic State:
  specific enthalpy h

Derived Thermodynamic State:
  Temperature T
  liquid Phase Fraction phi
```

An implementation may cache `T` or `phi` without changing this semantic classification.

## 11. Physical Transition Width vs Numerical Regularization

The selected reference closure intentionally contains **no finite numerical transition-temperature width** for the isothermal branch.

This resolves the earlier ambiguity between:

```text
physical solidus/liquidus interval
```

and

```text
numerical smoothing interval
```

for the minimal reference formulation.

### 11.1 Physical finite-width transition

If a material has a physically supported transition range:

```text
T_s < T_l
```

that range belongs to a different or extended formulation profile and may define a physical `phi(T)` relation.

### 11.2 Numerical smoothing

If an implementation smooths the isothermal enthalpy jump for numerical reasons, that smoothing is an implementation approximation.

It must not silently redefine:

```text
T_m
L
physical phase interval
```

or be presented as material evidence.

Its numerical error and convergence behavior belong to Verification.

## 12. Comparison with Apparent-Heat-Capacity Regularization

COMSOL's apparent-heat-capacity formulation is an established alternative that spreads latent heat over a finite temperature interval and provides smooth or linear transition functions.

That formulation is valid and useful, particularly for temperature-primary solving and materials with mushy behavior.

The present selection does not reject apparent heat capacity. It selects a different reference closure because ThermoCore's current bounded branch has already selected a persistent energy coordinate and needs a formulation that preserves latent-energy progress without requiring a finite transition-width parameter.

## 13. Boundary Cases

### 13.1 Zero latent heat

The selected phase-change closure requires:

```text
L > 0
```

If `L = 0`, the latent interval disappears and the material should be handled as a no-latent-transition material relation rather than by the selected phase-change closure.

### 13.2 Nonpositive sensible heat capacity

If a configured branch violates:

```text
c_s(T) > 0
```

or

```text
c_l(T) > 0
```

unique branch inversion is not guaranteed by this study.

Such a material relation is outside the validity domain of the selected reference closure unless additional evidence/formulation rules are provided.

### 13.3 Hysteresis and kinetic phase change

The selected relation is history-independent.

Supercooling, superheating hysteresis, metastability, kinetic phase evolution, and irreversible transformation history remain outside this minimal profile and may require additional Persistent State.

### 13.4 Physical mushy zones

Alloys or materials whose phase equilibrium spans a real solidus/liquidus interval remain valid thermodynamic cases, but they are not represented by the isothermal reference closure.

They should be treated by a finite-range formulation with explicit physical evidence for the transition interval and phase relation.

## 14. Formulation Invariants Exposed for Later Verification

The selected relation makes several later tests possible without performing those tests in Research.

For every supported cell state:

```text
0 <= phi <= 1
```

For latent states:

```text
T = T_m
h_s_star <= h <= h_l_star
phi = (h - h_s_star) / L
```

For phase boundaries:

```text
phi(h_s_star) = 0
phi(h_l_star) = 1
h_l_star - h_s_star = L
```

For sensible branches:

```text
dh/dT > 0
```

within the supported material range.

For a common reference shift `C`:

```text
Delta h
```

and the latent interval width `L` remain unchanged.

These are candidate formulation invariants to be transferred into a later authoritative formulation and then verified by implementation tests.

## 15. Gap Closure Assessment

### TF-G06 — Temperature recovery

```text
Status: CLOSED BY BOUNDED CLOSURE SELECTION
```

Reason:

- specific enthalpy is the independent coordinate;
- positive sensible heat capacities make the solid/liquid branches monotonic;
- the latent enthalpy interval maps uniquely to `T_m`;
- every supported `h` therefore maps to one `T`.

### TF-G07 — Phase Fraction recovery

```text
Status: CLOSED BY BOUNDED CLOSURE SELECTION
```

Reason:

- `phi` is explicitly defined from persistent enthalpy;
- the relation is bounded and single-valued;
- no unresolved transition-temperature smoothing width is required;
- history-dependent phase behavior is explicitly outside scope.

## 16. Remaining Research vs Downstream Work

With TF-G06 and TF-G07 closed, the remaining work identified by this research line is no longer a pre-specification formulation-selection gap.

The unresolved activities are downstream:

```text
write the authorized reference-formulation specification
implement the selected relation
verify h -> T recovery
verify h -> phi recovery
verify latent-energy conservation
validate against representative phase-change benchmarks
measure performance
```

The exact numerical inversion technique for variable `c_s(T)` / `c_l(T)` is an implementation choice as long as it preserves the selected formulation relation.

## 17. Framework Impact

No Framework change is indicated.

The selected closure preserves:

- Framework-level variable neutrality;
- formulation-relative Persistent/Derived classification;
- Material Definition as Configuration;
- Thermodynamic Computation ownership of State evolution;
- Framework Interface semantics;
- separation of Research, Specification, Implementation, Verification, and Validation.

```text
Framework Specification change: None
Framework Freeze reopen: No
```

## 18. Current Decision

The focused closure study supports the following bounded research decision:

```text
Reference energy coordinate:
  specific enthalpy h [J/kg]

Reference phase-change closure:
  isothermal transition at T_m
  latent enthalpy interval width L

Persistent Thermodynamic State candidate:
  h

Derived Thermodynamic State candidates:
  T
  phi

Temperature recovery:
  unique piecewise h -> T relation

Phase recovery:
  phi = (h - h_s_star) / L within the latent interval

Numerical transition-temperature smoothing width:
  not part of the physical reference closure
```

**TF-G06 and TF-G07 are closed for this bounded reference-formulation branch.**

The next research-governance step is to update the Evidence Matrix and Research Gap Analysis with this closure evidence. If those updates pass review without new gaps, `Thermodynamic_Formulation.md` may then be considered for authorization as a non-Framework reference-formulation specification.