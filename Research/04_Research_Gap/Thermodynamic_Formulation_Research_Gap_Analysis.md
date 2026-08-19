# Thermodynamic Formulation Research Gap Analysis

Version: 0.2  
Status: Completed Formulation-Gap Closure Analysis — Non-Normative  
Research line: Bounded fixed-grid thermodynamic reference formulation

---

## 1. Objective

This document analyzes and closes the formulation gaps carried forward by `Thermodynamic_Formulation_Evidence_Matrix.md` for the bounded fixed-grid reference-formulation branch.

The purpose is to determine, for each gap, whether it:

- can be closed by an explicit bounded modeling decision supported by the evidence;
- requires a focused closure study;
- belongs to downstream Verification / Validation rather than pre-specification research; or
- remains outside the minimal reference-formulation scope.

This document does **not** modify the ThermoCore Framework Specification and does not itself define an authoritative thermodynamic formulation.

## 2. Evidence Baseline

The analysis depends on:

- `Research/01_Evidence_Matrix/Thermodynamic_Formulation_Evidence_Matrix.md`;
- the seven source survey/comparison/closure artifacts traced by that matrix;
- `Research/00_Literature_Survey/Enthalpy_Temperature_Phase_Closure_Study.md` for closure of `TF-G06` and `TF-G07`;
- the frozen Framework semantics in `Documentation/Framework_Specification/Thermodynamic_State.md` and related Framework documents.

The governing research constraint remains:

> formulation decisions may narrow implementation physics without redefining Framework architecture, ownership, State semantics, Representation, or Interface semantics.

## 3. Gap Classification

| Classification | Meaning |
|---|---|
| **Closed by bounded decision** | Existing evidence is sufficient to select a reference-formulation convention without claiming universal superiority |
| **Closed by focused closure study** | A previously blocking relation has been explicitly defined and its bounded validity conditions established |
| **Downstream Verification / Validation** | The issue must be tested after the formulation and implementation exist; it is not a pre-specification blocker |
| **Deferred broader formulation** | Valid but outside the minimal reference formulation |

## 4. TF-G01 — Enthalpy vs Internal Energy

### Evidence position

The evidence establishes that enthalpy and internal energy are physically distinct, that internal energy aligns directly with a general first-law accumulation description, and that enthalpy has strong fixed-grid phase-change precedent. The Framework itself can remain neutral to either choice.

### Bounded decision

The minimal reference formulation is intended primarily as a fixed-grid solid/liquid phase-change path. For that bounded purpose, enthalpy provides the stronger direct precedent because sensible and latent energy can be represented in one thermodynamic coordinate.

### Disposition

**Closed by bounded decision.**

```text
reference energy-coordinate family = enthalpy
```

This is not a Framework requirement and does not prohibit internal-energy formulations in other conforming implementations.

## 5. TF-G02 — Specific vs Volumetric Energy Coordinate

### Evidence position

The evidence establishes that a specific thermodynamic coordinate `[J/kg]` can coexist with conservative volumetric accumulation and volumetric source terms. Under constant `rho_ref`, specific and volumetric forms are related by one fixed conversion factor.

### Bounded decision

Using a specific coordinate aligns the persistent thermodynamic state with common specific heat-capacity and latent-heat material data while keeping geometry/source normalization at the formulation boundary.

### Disposition

**Closed by bounded decision.**

```text
persistent energy basis = specific enthalpy h [J/kg]
```

Volumetric bookkeeping remains permitted as an equation- or implementation-level representation derived through `rho_ref`.

## 6. TF-G03 — Reference-Density Convention

### Evidence position

The bounded fixed-volume, fixed-mass branch requires one density across solid/liquid phase change unless an additional mass/volume conservation mechanism is introduced. Reference density also requires explicit provenance and a declared reference condition.

### Bounded decision

Standardize the semantic contract rather than one universal numerical density value:

```text
rho_ref = one constant material reference density [kg/m^3]
same rho_ref across the modeled solid/liquid transition
explicit provenance
explicit T_rho_ref
m_cell = rho_ref * V_cell
```

A solid-phase density at a declared reference condition may be used as a default material-data convention, but is not claimed as universally mandatory.

### Disposition

**Closed by bounded decision.**

## 7. TF-G04 — Energy Datum Convention

### Evidence position

Reference temperature and reference-energy offset are explicit thermophysical semantics in established software. A common additive shift preserves nonreacting energy differences, while inconsistent phase offsets can corrupt latent-heat relations.

### Bounded decision

For the minimal reference formulation, use a canonical zero reference:

```text
h_ref = 0 J/kg at declared T_E_ref
```

External datasets using another datum must be normalized consistently before use. If the selected enthalpy definition includes a constant pressure-volume contribution, it is handled consistently in this common datum; no evolving pressure state is introduced.

### Disposition

**Closed by bounded decision.**

## 8. TF-G05 — Density and Energy Reference Temperatures

### Evidence position

`T_rho_ref` identifies the density/reference-geometry condition while `T_E_ref` identifies the thermodynamic-energy datum. Their semantics are distinct even if the same numerical temperature is convenient.

### Bounded decision

```text
retain T_rho_ref and T_E_ref as distinct semantics
permit the same numerical value when explicitly chosen
```

### Disposition

**Closed by bounded decision.**

## 9. TF-G06 — Temperature-Recovery Uniqueness

### Previous blocker

The earlier gap analysis required an explicit enthalpy-to-temperature closure with a single-valued inversion before a reference-formulation specification could be authorized.

### Focused closure result

`Enthalpy_Temperature_Phase_Closure_Study.md` selected an isothermal enthalpy-jump closure for the bounded pure-substance-like reference branch.

Define:

```text
h_s_star = h_s(T_m)
h_l_star = h_s_star + L
```

with validity conditions:

```text
c_s(T) > 0
c_l(T) > 0
L > 0
```

and Temperature recovery:

```text
if h < h_s_star:
    T = inverse_h_s(h)

if h_s_star <= h <= h_l_star:
    T = T_m

if h > h_l_star:
    T = inverse_h_l(h)
```

Positive sensible heat-capacity branches make the solid and liquid sensible enthalpy relations monotonic; the latent enthalpy interval maps to one Temperature `T_m`.

### Disposition

**Closed by focused closure study.**

The formulation-level uniqueness gap is closed. Numerical inversion accuracy, convergence, and performance remain downstream Verification concerns.

## 10. TF-G07 — Phase-Fraction Recovery Uniqueness

### Previous blocker

The earlier gap analysis required one explicit, single-valued phase-transition relation that did not silently conflate physical transition width with numerical regularization.

### Focused closure result

For the selected isothermal enthalpy interval:

```text
if h < h_s_star:
    phi = 0

if h_s_star <= h <= h_l_star:
    phi = (h - h_s_star) / L

if h > h_l_star:
    phi = 1
```

The relation is bounded, continuous in `h`, single-valued, and history-independent within the selected equilibrium-like reference branch.

No finite numerical transition-temperature width is part of the physical reference closure. A physical solidus/liquidus interval belongs to another formulation profile; an implementation smoothing width remains an implementation approximation subject to Verification.

### Disposition

**Closed by focused closure study.**

History-dependent, hysteretic, kinetic, metastable, and physical mushy-zone formulations remain outside this minimal closure and may require additional Persistent State or different phase relations.

## 11. TF-G08 — Benchmark Energy and Recovery Consistency

### Evidence position

Energy conservation and Temperature/Phase recovery must ultimately be demonstrated, but those results require a defined formulation and implementation.

### Disposition

**Downstream Verification / Validation.**

TF-G08 is not a pre-specification blocker. It is carried forward as a traceable obligation for later:

```text
formulation invariant verification
implementation verification
physical / benchmark validation
```

The reference-formulation specification should expose the invariants to be tested without claiming that Verification or Validation has already occurred.

## 12. Final Gap Disposition Summary

| Gap | Final disposition | Result |
|---|---|---|
| TF-G01 | Closed by bounded decision | Enthalpy family selected for the minimal reference formulation |
| TF-G02 | Closed by bounded decision | Specific enthalpy `[J/kg]` selected as persistent energy basis |
| TF-G03 | Closed by bounded decision | One constant material `rho_ref` with explicit provenance/reference condition |
| TF-G04 | Closed by bounded decision | Reference enthalpy normalized to zero at `T_E_ref` |
| TF-G05 | Closed by bounded decision | Density and energy reference temperatures remain semantically distinct |
| TF-G06 | Closed by focused closure study | Unique piecewise `h -> T` relation under declared validity conditions |
| TF-G07 | Closed by focused closure study | Explicit single-valued `h -> phi` isothermal latent closure |
| TF-G08 | Downstream Verification / Validation | Carry forward as test and benchmark obligation |

```text
Open pre-specification formulation gaps: 0
```

## 13. Bounded Reference-Formulation Profile After Gap Closure

The completed Research → Evidence → Research Gap chain supports the following bounded profile for transfer into a later non-Framework reference-formulation specification:

```text
Geometry / mass:
  fixed cell volume
  fixed per-cell mass
  no mass transport
  no shrinkage / expansion

Density:
  rho_ref constant per material
  one rho_ref across solid/liquid phase change
  explicit T_rho_ref and provenance

Persistent Thermodynamic State:
  specific enthalpy h [J/kg]

Energy datum:
  h_ref = 0 J/kg at T_E_ref
  common reference-compatible datum across phase branches

Pressure / work:
  no evolving runtime pressure state
  no mechanical / pressure-volume work evolution
  constant datum contribution handled consistently by the enthalpy reference convention

Phase change:
  isothermal transition at T_m
  h_s_star = h_s(T_m)
  h_l_star = h_s_star + L
  L > 0

Temperature recovery:
  solid inverse below h_s_star
  T = T_m through the latent interval
  liquid inverse above h_l_star

Liquid Phase Fraction:
  0 below h_s_star
  (h - h_s_star) / L in the latent interval
  1 above h_l_star

Validity conditions:
  c_s(T) > 0
  c_l(T) > 0
  history-independent equilibrium-like transition
```

The profile remains bounded and non-universal. Physical mushy zones, variable-density shrinkage/expansion, pressure/compressibility evolution, mass transport, and history-dependent phase behavior remain outside its scope.

## 14. Framework Impact

The completed gap analysis finds no reason to modify the Framework Specification.

The bounded profile remains compatible with the frozen Framework because:

- Framework-level Thermodynamic State remains variable-neutral;
- Persistent/Derived classification remains formulation-relative;
- Material Definition remains Configuration;
- Energy Input remains Framework-level Runtime Information without universal physical units;
- source-unit conversion remains formulation-level;
- Thermodynamic Computation remains the only Core responsibility that evolves/writes Thermodynamic State;
- no new Framework Core component or Interface semantic is introduced.

```text
Framework Specification change: None
Framework Freeze reopen: No
```

## 15. Specification Authorization Assessment

### Research-gap status

```text
Pre-specification blockers: 0
```

### Reference-formulation status

```text
Thermodynamic_Formulation.md research readiness: READY FOR AUTHORIZATION
```

The Research → Evidence → Research Gap sequence has now produced explicit bounded decisions for the formulation's energy coordinate, basis, density/reference semantics, Persistent/Derived profile, and enthalpy–temperature–phase closure.

TF-G08 remains downstream and therefore does not block specification authorization.

**This document does not itself make `Thermodynamic_Formulation.md` part of the frozen Framework Specification hierarchy.** Any authorized formulation document must remain a non-Framework reference-formulation specification unless a separate governance process explicitly changes that status.

## 16. Downstream Obligations

Once a reference-formulation specification is authorized, the next stages should be:

```text
Specification
    -> Implementation
    -> Verification
    -> Validation
    -> Performance Evaluation
```

At minimum, downstream Verification should cover:

```text
h -> T recovery
h -> phi recovery
0 <= phi <= 1
h_l_star - h_s_star = L
continuity/endpoint consistency of phi(h)
monotonic sensible branches over supported material range
source-unit dimensional mapping
reference-datum invariance
latent-energy conservation
```

Validation should then compare the implemented formulation against representative physical/benchmark cases appropriate to the declared bounded scope.

## 17. Current Decision

**All pre-specification thermodynamic-formulation research gaps identified as TF-G01 through TF-G07 are closed for the bounded fixed-grid reference branch.**

**TF-G08 is retained as a downstream Verification / Validation obligation.**

**Do not modify or reopen the Framework Specification.**

**The bounded non-Framework reference-formulation specification is ready for authorization review.**