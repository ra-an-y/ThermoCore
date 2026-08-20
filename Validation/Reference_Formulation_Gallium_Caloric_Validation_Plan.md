# Reference Formulation Gallium Caloric Validation Plan

Version: 0.1  
Status: Validation Plan — no Validation conclusion yet

---

## 1. Validation Purpose

This Validation track evaluates whether the bounded constant-heat-capacity ThermoCore reference implementation preserves useful caloric behavior for a second material whose thermodynamic scale and phase-transition characteristics differ materially from the first H2O benchmark.

The benchmark material is elemental gallium using NIST Chemistry WebBook SRD 69 / NIST-JANAF condensed-phase thermochemistry data.

This track is intentionally limited to specific-enthalpy / Temperature / solid-liquid phase-transition caloric behavior. It does not validate density change, volume change, transport, conduction, fluid flow, free surfaces, pressure evolution, GPU execution, performance, or Framework Conformance.

---

## 2. Evaluated Baseline

Initial evaluated implementation baseline:

```text
Repository commit: 88964c7462bef4c83b0e02b30c63bef6d5e4d0bf
Prior Validation PR: #50
Reference formulation: Documentation/Thermodynamic_Formulation.md
Implementation profile: bounded constant-positive-Cp C# reference implementation
```

No Framework Specification or reference-formulation semantic change is introduced by this Validation track.

---

## 3. External Reference Basis

Primary reference source:

- NIST Chemistry WebBook, SRD 69, Gallium (CAS 7440-55-3)
  - condensed-phase Shomate relations from Chase, 1998;
  - fusion Temperature from NIST/TRC phase-change data.

Reference URLs:

```text
https://webbook.nist.gov/cgi/cbook.cgi?ID=C7440553&Mask=32
https://webbook.nist.gov/cgi/cbook.cgi?ID=C7440553&Mask=4
```

The NIST condensed-phase page reports:

```text
Molar mass: 69.723 g/mol
Solid Shomate validity: 298.00 K to 302.92 K
Liquid Shomate validity: 302.92 K to 2476.57 K
T_fus: 302.92 K
```

The NIST page identifies `T_fus = 302.92 K` as a recommended calibration-standard value and gives the solid and liquid Shomate coefficients used by this benchmark.

---

## 4. Reference Enthalpy Construction

NIST publishes the condensed-phase Shomate relation

```text
H° - H°_298.15
  = A t + B t^2/2 + C t^3/3 + D t^4/4 - E/t + F - H

t = T / 1000
```

with enthalpy in kJ/mol.

This Validation uses the NIST-JANAF phase tables with one common benchmark datum. The solid standard-state offset is zero for elemental gallium. For the liquid branch, the Shomate table parameter `H = 5.577983 kJ/mol` is used as the liquid standard-state offset so the benchmark retains the internal precision of the published Shomate parameter set rather than the separately rounded `5.58 kJ/mol` display value.

The common molar reference enthalpies are therefore constructed as:

```text
H_ref,solid(T)  = H_Sh,solid(T)
H_ref,liquid(T) = H_Sh,liquid(T) + 5.577983 kJ/mol
```

They are converted to J/kg using the NIST molar mass `69.723 g/mol`.

A common additive normalization is then applied:

```text
h_norm(T, phase) = h_ref(T, phase) - h_ref(298.5 K, solid)
```

Therefore the ThermoCore energy datum for this benchmark is:

```text
T_E_ref = 298.5 K
h = 0 J/kg at 298.5 K on the solid branch
```

Only enthalpy differences and common-datum-normalized values are physically compared.

---

## 5. Shomate Coefficients

### 5.1 Solid gallium

Validity:

```text
298.00 K <= T <= 302.92 K
```

Coefficients:

```text
A = 102.3394
B = -347.5134
C = 603.3621
D = -360.7047
E = -1.490304
F = -24.68472
G = 236.2780
H = 0.000000
```

### 5.2 Liquid gallium

Validity:

```text
302.92 K <= T <= 2476.57 K
```

Coefficients:

```text
A = 24.62138
B = 2.701388
C = -1.272134
D = 0.196526
E = 0.286145
F = -0.908736
G = 89.90830
H = 5.577983
```

No benchmark point shall be evaluated outside the corresponding NIST validity range.

---

## 6. Constant-Cp Calibration Boundary

The reference implementation intentionally uses one constant positive `c_s` and one constant positive `c_l`.

Calibration points are fixed before holdout comparison:

```text
Solid calibration:
  298.5 K
  301.0 K

Liquid calibration:
  350.0 K
  800.0 K
```

The fitted values are defined by enthalpy slopes:

```text
c_s,fit = [h_ref(301.0 K, solid) - h_ref(298.5 K, solid)] / 2.5 K

c_l,fit = [h_ref(800.0 K, liquid) - h_ref(350.0 K, liquid)] / 450 K
```

The phase-transition Temperature is fixed from NIST/TRC:

```text
T_m = 302.92 K
```

The latent heat is obtained from the common-datum NIST-JANAF branch difference at `T_m`:

```text
L_ref = h_ref(T_m, liquid) - h_ref(T_m, solid)
```

Calibration points shall not be counted as independent holdout evidence.

---

## 7. Holdout States

Solid holdouts:

```text
299.0 K
299.5 K
300.0 K
300.5 K
301.5 K
302.5 K
```

Liquid holdouts:

```text
303 K
320 K
450 K
600 K
1000 K
1500 K
```

All holdout points lie within the validity interval of the applicable NIST Shomate branch.

---

## 8. Required Comparisons

For every holdout state, preserve and compare:

```text
phase
pressure / standard-state declaration
reference Temperature
NIST-JANAF specific enthalpy
common-datum-normalized specific enthalpy
ThermoCore forward specific enthalpy
ThermoCore recovered Temperature
absolute Temperature error
specific-enthalpy error
relative enthalpy error where meaningful
```

The executed benchmark shall also verify:

```text
T_m parameter identity against the declared NIST fusion Temperature
implemented latent interval width against L_ref
reference-datum shift invariance
reproduction of the frozen CSV reference values from the recorded Shomate coefficients
```

---

## 9. Density Boundary

The current implementation requires a positive `rho_ref` in Material Definition even though this Validation does not exercise mass or volumetric mapping.

The benchmark therefore uses a deliberately non-physical positive placeholder:

```text
rho_ref = 1.0 kg/m^3
```

This value exists only to satisfy Configuration construction. It is not a gallium density claim, is not used in any reported caloric metric, and shall not be interpreted as physical Validation of density behavior.

---

## 10. Metrics and Conclusion Vocabulary

Required aggregate metrics:

```text
max absolute Temperature error [K]
mean absolute Temperature error [K]
max absolute specific-enthalpy error [J/kg]
mean absolute specific-enthalpy error [J/kg]
latent-heat parameter error [J/kg]
melting-Temperature parameter error [K]
```

As with the first caloric benchmark, no physical PASS/FAIL threshold shall be invented after observing the result.

Until an acceptance threshold is justified independently, the result vocabulary remains:

```text
COMPLETED — errors reported
INCOMPLETE — evidence missing
INVALID — procedure or reference mismatch
```

---

## 11. Evidence Artifacts

Planned artifacts:

```text
Validation/
  Reference_Formulation_Gallium_Caloric_Validation_Plan.md
  Reference_Formulation_Gallium_Caloric_Validation_v0.1.md
  ThermoCore.GalliumCaloricValidation.csproj
  Data/
    gallium_caloric_benchmark_v0.1.csv
  Execution/
    GalliumProgram.cs
```

The executed result report shall identify the exact workflow run and evaluated implementation baseline.

---

## 12. Validation Boundary

A completed result supports only the declared local caloric purpose for this bounded reference implementation.

It does not establish:

- gallium density accuracy;
- shrinkage or expansion behavior;
- transport or conduction correctness;
- pressure-dependent material behavior;
- moving-boundary behavior;
- GPU equivalence;
- performance suitability;
- complete Framework Validation; or
- Framework Conformance.

---

## 13. Entry Criterion Status

At creation of this plan:

```text
Reference formulation specification: available
Bounded implementation: available
Implementation Verification: available
First H2O caloric benchmark: COMPLETED — errors reported
Second external reference basis: selected
Gallium benchmark execution: not yet performed
Validation conclusion: none
```
