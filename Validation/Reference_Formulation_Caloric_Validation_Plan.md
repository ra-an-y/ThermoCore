# Reference Formulation Caloric Validation Plan

Version: 0.1  
Status: Validation Plan — no Validation conclusion yet

---

## 1. Validation Purpose

This Validation track evaluates whether the bounded constant-heat-capacity ThermoCore reference implementation reproduces the caloric behavior expected for a real solid/liquid material within the physical scope actually represented by `Documentation/Thermodynamic_Formulation.md`.

The first benchmark material is ordinary H2O using authoritative IAPWS thermodynamic reference formulations.

The purpose is deliberately limited to specific-enthalpy / Temperature / phase-transition caloric behavior. It does not validate density-jump effects, shrinkage or expansion, mass transport, fluid flow, moving boundaries, conduction, free-surface motion, pressure evolution, GPU execution, or Framework Conformance.

---

## 2. Evaluated Baseline

Initial Validation baseline:

```text
Repository commit: d5ff233446f2415a8a2866176fbf1faa906b0ac6
Verification PR: #48
Verification result before Validation entry: 16/16 PASS
Reference formulation: Documentation/Thermodynamic_Formulation.md
Implementation profile: bounded constant-positive-Cp C# reference implementation
```

The Framework Specification v1.0 freeze baseline remains separately governed and is not changed by this Validation activity.

Any later Validation run against a different implementation commit shall record that commit separately rather than silently replacing evidence for this baseline.

---

## 3. External Reference Basis

The benchmark shall use primary authoritative IAPWS sources:

1. IAPWS R10-06(2009), *Revised Release on the Equation of State 2006 for H2O Ice Ih*  
   https://www.iapws.org/relguide/Ice-2009.html
2. IAPWS R6-95(2018), *Revised Release on the IAPWS Formulation 1995 for the Thermodynamic Properties of Ordinary Water Substance for General and Scientific Use*  
   https://www.iapws.org/relguide/IAPWS-95.html
3. IAPWS R14-08(2011), *Revised Release on the Pressure along the Melting and Sublimation Curves of Ordinary Water Substance*  
   https://www.iapws.org/relguide/MeltSub.html

These sources provide the reference thermodynamic property relations for Ice Ih, fluid water, and the melting boundary.

No value copied from a secondary engineering table shall override the selected IAPWS reference basis without an explicit Validation-plan revision.

---

## 4. Why H2O Is Used Only as a Bounded Caloric Benchmark

Real water and ice do not satisfy the reference formulation's equal-density physical simplification.

Therefore this Validation shall not compare or claim validity for density, volume change, buoyancy, shrinkage/expansion, or mass redistribution.

The benchmark uses H2O only for quantities that remain meaningful in the current local caloric scope:

- melting-boundary Temperature at the declared pressure;
- specific enthalpy differences in the solid branch;
- latent enthalpy difference across melting;
- specific enthalpy differences in the liquid branch;
- Temperature recovered from a supplied specific-enthalpy coordinate.

This bounded use prevents a successful caloric comparison from being misrepresented as validation of the excluded density and transport physics.

---

## 5. Reference Condition

The initial benchmark pressure shall be declared explicitly and held fixed for generation of the IAPWS reference dataset.

Candidate initial pressure:

```text
p_ref = 0.1 MPa
```

The exact melting Temperature `T_m,ref` shall be obtained from the IAPWS melting-curve reference rather than assumed from an informal rounded value.

The ThermoCore energy datum shall remain arbitrary but explicit. The benchmark comparison shall use enthalpy differences or a common normalized datum so that a constant reference offset cannot affect the physical comparison.

---

## 6. Constant-Cp Approximation and Calibration Boundary

The current implementation does not reproduce the full temperature-dependent heat-capacity relations of IAPWS. It intentionally uses one constant positive `c_s` and one constant positive `c_l`.

Accordingly, Validation shall distinguish parameter calibration from holdout evaluation.

For each sensible branch, a constant heat capacity may be determined from a declared calibration interval using an IAPWS enthalpy difference:

```text
c_s,fit = [h_ice(T_s,2, p_ref) - h_ice(T_s,1, p_ref)] / (T_s,2 - T_s,1)

c_l,fit = [h_liq(T_l,2, p_ref) - h_liq(T_l,1, p_ref)] / (T_l,2 - T_l,1)
```

The latent heat parameter shall be obtained from the IAPWS enthalpy difference between the coexisting liquid and Ice Ih states at the selected melting condition:

```text
L_ref = h_liq(T_m,ref, p_ref) - h_ice(T_m,ref, p_ref)
```

Temperatures used to determine these fitted parameters shall be recorded as calibration points and shall not also be counted as independent holdout comparisons.

---

## 7. Holdout Validation Quantities

After calibration, the implementation shall be evaluated at additional IAPWS reference states that were not used to determine `c_s`, `c_l`, or the common energy datum.

For each holdout point, preserve at least:

```text
phase
pressure
reference Temperature
reference specific enthalpy after common-datum normalization
ThermoCore predicted / recovered Temperature
ThermoCore corresponding specific enthalpy
absolute Temperature error
specific-enthalpy error
relative error where numerically meaningful
```

The latent transition shall additionally record:

```text
T_m,ref
L_ref
implemented latent interval width
Temperature across the represented latent interval
```

---

## 8. Required Comparisons

The first executed Validation shall include all of the following categories:

### 8.1 Solid sensible branch

Compare the constant-`c_s` ThermoCore branch against held-out Ice Ih reference states below melting.

### 8.2 Melting transition

Compare the implemented transition Temperature and latent enthalpy width against the IAPWS coexistence condition used by the benchmark.

### 8.3 Liquid sensible branch

Compare the constant-`c_l` ThermoCore branch against held-out liquid-water reference states above melting and within the selected benchmark range.

### 8.4 Reference-datum invariance

Repeat at least one comparison after applying a common additive enthalpy shift. Physical errors shall remain unchanged apart from floating-point roundoff.

---

## 9. Metrics

The first run shall report measured error rather than silently choose a convenient pass threshold after seeing the data.

Required metrics include:

```text
max absolute Temperature error [K]
mean absolute Temperature error [K]
max absolute specific-enthalpy error [J/kg]
mean absolute specific-enthalpy error [J/kg]
latent-heat error [J/kg and %]
melting-Temperature error [K]
```

If a formal PASS/FAIL acceptance threshold is later adopted, its physical or application justification shall be recorded before that threshold is used as a release or Conformance claim.

Until such a threshold is justified, the conclusion vocabulary for this track shall be descriptive, for example:

```text
COMPLETED — errors reported
INCOMPLETE — evidence missing
INVALID — procedure or reference mismatch
```

rather than an unsupported physical `PASS`.

---

## 10. Evidence Artifacts

The executed Validation shall preserve versioned evidence rather than editing this plan into a result report.

Planned artifacts:

```text
Validation/
  README.md
  Reference_Formulation_Caloric_Validation_Plan.md
  Reference_Formulation_Caloric_Validation_v0.1.md
  Data/
    reference_caloric_benchmark_v0.1.csv
```

If executable benchmark-generation or comparison code is added, its repository location and exact commit shall be recorded by the result report.

---

## 11. Validation Boundaries

A successful result in this track shall support only the stated local caloric purpose.

It shall not establish:

- correctness of unimplemented conduction or convection;
- correctness of variable-density phase change;
- conservation under mass transport;
- free-surface or geometry evolution;
- physical validity of the equal-density simplification for water;
- GPU equivalence;
- performance suitability;
- complete Framework Validation; or
- Framework Conformance by itself.

---

## 12. Entry Criterion Status

At creation of this plan:

```text
Reference formulation specification: available
Bounded implementation: available
Material Definition compilation: available
Implementation Verification: 16/16 PASS
External authoritative reference basis: selected
Executed physical/reference-model comparison: not yet performed
Validation conclusion: none
```

The Validation stage is therefore active, but no physical Validation result is claimed yet.
