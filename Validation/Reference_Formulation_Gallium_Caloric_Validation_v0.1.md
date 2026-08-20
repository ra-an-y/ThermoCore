# Reference Formulation Gallium Caloric Validation v0.1

Status: Validation Result — COMPLETED — errors reported  
Physical PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Validation Purpose

This record preserves the second independent executed caloric Validation comparison for the bounded constant-heat-capacity ThermoCore reference implementation.

The benchmark material is elemental gallium and the validation purpose is limited to:

- solid sensible-enthalpy behavior near melting;
- the declared solid/liquid transition Temperature;
- the latent enthalpy interval;
- liquid sensible-enthalpy behavior over a broader temperature range; and
- Temperature recovered from a supplied specific-enthalpy coordinate.

This record does not validate density, shrinkage or expansion, mass transport, conduction, flow, free surfaces, pressure evolution, GPU execution, performance, complete Framework Validation, or Framework Conformance.

---

## 2. Evaluated and Evidentiary Versions

The implementation baseline evaluated by this Validation is:

```text
Implementation baseline: 88964c7462bef4c83b0e02b30c63bef6d5e4d0bf
Prior H2O Validation PR: #50
Reference formulation: Documentation/Thermodynamic_Formulation.md
Implementation profile: bounded constant-positive-Cp C# reference implementation
```

The successful Gallium Validation code/data snapshot is:

```text
Validation branch: validation/gallium-caloric-benchmark-v0.1
Validation code/data commit: 0845fbe039af3cfc091eadb6cf3f7deaf845ca86
GitHub Actions workflow: Gallium Caloric Validation
Successful workflow run: 32399146795
Successful job: 96522916902
Workflow conclusion: success
```

The pre-existing H2O caloric Validation workflow was also triggered by the Validation-directory change and completed successfully in run `32399146729`, job `96522916906`.

No Framework or Material implementation source was changed by this Validation branch.

---

## 3. External Reference Basis

The benchmark uses NIST Chemistry WebBook SRD 69 / NIST-JANAF gallium data:

1. NIST Chemistry WebBook, Gallium condensed-phase thermochemistry  
   https://webbook.nist.gov/cgi/cbook.cgi?ID=C7440553&Mask=32
2. NIST Chemistry WebBook, Gallium phase-change data  
   https://webbook.nist.gov/cgi/cbook.cgi?ID=C7440553&Mask=4
3. Chase, M.W., Jr., *NIST-JANAF Thermochemical Tables, Fourth Edition*, J. Phys. Chem. Ref. Data, Monograph 9, 1998.

The NIST data provide:

```text
Molar mass: 69.723 g/mol
Solid Shomate validity: 298.00 K to 302.92 K
Liquid Shomate validity: 302.92 K to 2476.57 K
T_fus: 302.92 K
```

The benchmark runner reconstructs the frozen CSV reference enthalpies directly from the recorded NIST-JANAF Shomate coefficients before any ThermoCore comparison is accepted.

---

## 4. Benchmark Configuration

The executed benchmark uses:

```text
reference standard pressure: 1 bar
T_E_ref = 298.5 K
T_m,ref = 302.92 K
```

The common enthalpy datum is normalized to solid gallium at `298.5 K`.

The NIST-JANAF Shomate phase relations are converted from kJ/mol to J/kg using the NIST molar mass `69.723 g/mol`.

For the liquid branch, the Shomate table parameter `H = 5.577983 kJ/mol` is retained as the standard-state phase offset in the common benchmark datum. This preserves the internal precision of the published coefficient set.

Constant heat capacities are calibrated from fixed pre-declared intervals:

```text
solid calibration: 298.5 K, 301.0 K
liquid calibration: 350 K, 800 K

c_s,fit = 374.0942286128 J/(kg*K)
c_l,fit = 384.3443361244444 J/(kg*K)
L_ref   = 80241.96657261255 J/kg
```

The implementation requires a positive `rho_ref` in Material Definition. This benchmark supplies `rho_ref = 1.0 kg/m^3` as an explicitly non-physical placeholder because density is excluded from all reported metrics.

---

## 5. Holdout States

Calibration points are not counted as independent holdout comparisons.

The executed holdout set contains 12 states:

```text
solid: 299.0, 299.5, 300.0, 300.5, 301.5, 302.5 K
liquid: 303, 320, 450, 600, 1000, 1500 K
```

Every holdout lies inside the validity interval of the corresponding NIST Shomate branch.

Evidence file:

`Validation/Data/gallium_caloric_benchmark_v0.1.csv`

---

## 6. Execution Procedure

The executable comparison is:

`Validation/ThermoCore.GalliumCaloricValidation.csproj`

with implementation in:

`Validation/Execution/GalliumProgram.cs`

The runner:

1. reads the frozen Gallium CSV;
2. reconstructs every stored NIST reference enthalpy from the recorded Shomate coefficients;
3. verifies the common-datum normalization;
4. derives constant `c_s` and `c_l` from the declared calibration rows;
5. derives `L_ref` from the NIST-JANAF solid/liquid branch difference at `302.92 K`;
6. constructs and compiles `ReferenceMaterialDefinition` through the current ThermoCore implementation;
7. evaluates the current constant-Cp forward enthalpy branches;
8. supplies normalized reference enthalpy states to the current `h -> T` recovery path;
9. compares 12 independent holdout states;
10. reports aggregate Temperature and enthalpy errors;
11. checks the calibrated latent interval and melting-Temperature parameters; and
12. verifies common additive enthalpy-datum shift invariance.

The executable returns a nonzero exit code if the frozen evidence, reconstructed NIST reference values, or current comparison path are internally inconsistent.

---

## 7. Preserved Execution Results

Workflow run `32399146795` reported:

```text
Gallium caloric validation execution
Evaluated holdouts: 12
reference pressure [bar]: 1
T_m,ref [K]: 302.92
c_s,fit [J/(kg*K)]: 374.0942286128
c_l,fit [J/(kg*K)]: 384.3443361244444
L_ref [J/kg]: 80241.96657261255

max |T error| [K]: 4.22040809054721
mean |T error| [K]: 1.1222973855630538
max |h error| [J/kg]: 1622.0899457356136
mean |h error| [J/kg]: 431.34304175170155

latent-heat parameter error [J/kg]: 0
melting-Temperature parameter error [K]: 0
reference-datum shift invariance: PASS

Validation comparison: COMPLETED — errors reported.
```

The zero latent-heat parameter error and zero melting-Temperature parameter error are calibration identities for the declared benchmark configuration. They are not independent predictive evidence for those two parameters.

The nonzero holdout errors quantify the deviation introduced by replacing the temperature-dependent NIST-JANAF caloric relations with one constant `c_s` and one constant `c_l`.

---

## 8. Observed Error Structure

The solid holdouts remain close to the constant-`c_s` approximation over the narrow NIST solid validity interval. The largest solid holdout Temperature error is only a few millikelvin.

The dominant error occurs on the wider liquid branch. The largest observed Temperature deviation occurs at the `450 K` liquid holdout:

```text
|T error| ≈ 4.220408 K
```

The largest absolute specific-enthalpy error is also on the liquid branch:

```text
|h error| ≈ 1622.090 J/kg
```

This pattern is expected for a bounded constant-heat-capacity profile compared with a temperature-dependent reference relation over a much larger liquid-temperature interval. The Validation record reports that discrepancy rather than treating it as an implementation failure or hiding it through a fitted post-hoc threshold.

---

## 9. Comparison with the First H2O Benchmark

The two completed caloric Validation tracks exercise the same reference implementation against materially different external reference systems:

```text
H2O:
  IAPWS reference formulations
  T_m ≈ 273.153 K
  L ≈ 333.4 kJ/kg
  max holdout |T error| ≈ 0.183 K

Gallium:
  NIST-JANAF / NIST Chemistry WebBook
  T_m = 302.92 K
  L ≈ 80.24 kJ/kg
  max holdout |T error| ≈ 4.220 K
```

These results shall not be ranked as if the benchmark ranges, calibration intervals, or material behavior were identical. Their value is that the same ThermoCore formulation and implementation path are now quantitatively characterized against two independent materials and two independent authoritative reference families.

---

## 10. Validation Conclusion

Result status:

```text
COMPLETED — errors reported
```

The second benchmark executed successfully, reconstructed its frozen external-reference enthalpies from the recorded NIST-JANAF relations, compared 12 independent holdout states, and confirmed common enthalpy-datum shift invariance.

No physical PASS/FAIL threshold was defined before execution. Therefore this record does **not** convert the measured errors into a physical `PASS` claim.

The result strengthens evidence that the bounded reference implementation can be applied consistently to more than one material/reference family while also exposing the quantitative approximation error introduced by constant heat capacity over wider ranges.

It does not establish complete physical Validation or Framework Conformance.

---

## 11. Known Limitations

- The solid gallium Shomate validity range is only `298.00–302.92 K`, so the independent solid holdout range is necessarily narrow.
- The constant-`c_l` approximation is evaluated over a much broader liquid interval, producing larger deviations than the first H2O benchmark.
- `T_m` and latent interval width are calibrated quantities, not independent holdout predictions.
- Density is explicitly excluded; the required `rho_ref` is a non-physical positive placeholder.
- No pressure evolution is evaluated.
- No conduction, transport, geometry evolution, or transient spatial benchmark is included.
- No GPU or performance evidence is produced by this track.
- No physical or application acceptance threshold has been adopted.

---

## 12. Evidence Integrity

This result is tied to the identified implementation baseline, Validation branch, code/data commit, workflow run, frozen CSV, and recorded external reference equations.

A materially different implementation, reference coefficient set, calibration interval, holdout range, or conclusion shall be recorded as new or versioned Validation evidence rather than silently replacing this record.
