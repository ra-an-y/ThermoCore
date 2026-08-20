# Reference Formulation Caloric Validation v0.1

Status: Validation Result — COMPLETED — errors reported  
Physical PASS threshold: None adopted  
Framework Conformance: Not claimed

---

## 1. Validation Purpose

This record preserves the first executed Validation comparison for the bounded constant-heat-capacity ThermoCore reference implementation.

The validation purpose is limited to local caloric behavior for an ordinary H2O solid/liquid benchmark:

- solid sensible-enthalpy behavior;
- melting Temperature;
- latent enthalpy;
- liquid sensible-enthalpy behavior; and
- Temperature recovered from a supplied specific-enthalpy coordinate.

This record does not validate density change, shrinkage or expansion, mass transport, flow, conduction, free-surface behavior, pressure evolution, GPU execution, performance, complete Framework Validation, or Framework Conformance.

---

## 2. Evaluated and Evidentiary Versions

The implementation evaluated by this Validation is:

```text
Implementation baseline: d5ff233446f2415a8a2866176fbf1faa906b0ac6
Verification PR: #48
Verification result before Validation: 16/16 PASS
Reference formulation: Documentation/Thermodynamic_Formulation.md
```

The Validation plan was integrated by PR #49.

The successful Validation code/data snapshot is:

```text
Validation branch: validation/reference-caloric-benchmark-v0.1
Validation code/data commit: 3cfe3a1d8962a74f89c64064700d3ef97c1f9a83
GitHub Actions workflow: Reference Caloric Validation
Successful workflow run: 32385632109
Successful job: 96479300077
Workflow conclusion: success
```

No Framework or Material implementation source was changed between the evaluated implementation baseline and this Validation execution.

---

## 3. External Reference Basis

The benchmark uses primary IAPWS reference material:

1. IAPWS R10-06(2009), *Revised Release on the Equation of State 2006 for H2O Ice Ih*  
   https://www.iapws.org/relguide/Ice-2009.html
2. IAPWS R6-95(2018), *Revised Release on the IAPWS Formulation 1995 for the Thermodynamic Properties of Ordinary Water Substance for General and Scientific Use*  
   https://www.iapws.org/relguide/IAPWS-95.html
3. IAPWS R14-08(2011), *Revised Release on the Pressure along the Melting and Sublimation Curves of Ordinary Water Substance*  
   https://www.iapws.org/relguide/MeltSub.html
4. IAPWS SR6-08(2011), *Revised Supplementary Release on Properties of Liquid Water at 0.1 MPa*  
   https://www.iapws.org/relguide/LiquidWater.html

R10-06 supplies the Ice Ih caloric relation. R14-08 supplies the Ice-Ih melting condition. Liquid-water values at the fixed benchmark pressure are evaluated with the IAPWS 0.1 MPa supplementary correlation, which is an authoritative IAPWS representation for that condition and remains consistent with the broader IAPWS thermodynamic basis.

The stored CSV is the frozen numerical dataset used by this Validation run. This result record does not replace the IAPWS releases as the external physical/reference-model authority.

---

## 4. Benchmark Configuration

The executed benchmark uses:

```text
p_ref = 0.1 MPa
T_E_ref = 250 K
T_m,ref = 273.152617521 K
```

The common enthalpy datum is normalized to the Ice Ih reference enthalpy at `250 K`.

Constant heat-capacity parameters are calibrated from IAPWS enthalpy differences:

```text
solid calibration: 250 K, 270 K
liquid calibration: 280 K, 320 K

c_s,fit = 2000.0390434600001 J/(kg*K)
c_l,fit = 4183.862337324999 J/(kg*K)
L_ref   = 333426.89415089996 J/kg
```

The latent heat is obtained from the liquid-minus-Ice-Ih enthalpy difference at the selected coexistence condition.

The implementation requires a positive constant `rho_ref`; this benchmark supplies an Ice-Ih reference-density value for configuration completeness. Density is not a quantity evaluated by this caloric Validation.

---

## 5. Holdout States

Calibration points are not counted as independent holdout comparisons.

The executed holdout set contains 12 states:

```text
solid: 255, 260, 265, 268, 272, 273 K
liquid: 275, 285, 290, 300, 310, 330 K
```

For each holdout state, the frozen CSV preserves the IAPWS reference enthalpy, common-datum normalized enthalpy, ThermoCore forward enthalpy, ThermoCore recovered Temperature, and corresponding error quantities.

Evidence file:

`Validation/Data/reference_caloric_benchmark_v0.1.csv`

---

## 6. Execution Procedure

The executable comparison is:

`Validation/ThermoCore.ReferenceCaloricValidation.csproj`

with implementation in:

`Validation/Execution/Program.cs`

The runner:

1. reads the frozen benchmark CSV;
2. derives `c_s`, `c_l`, `T_m`, `L`, and the energy reference condition from declared calibration/coexistence rows;
3. constructs `ReferenceMaterialDefinition` and compiles it through the current ThermoCore implementation;
4. evaluates the current constant-Cp enthalpy branches;
5. supplies normalized IAPWS enthalpy states to the current `h -> T` recovery path;
6. compares the 12 holdout states;
7. reports aggregate Temperature and enthalpy errors;
8. checks latent interval width and melting Temperature against the calibrated coexistence parameters; and
9. repeats comparison semantics under a common additive enthalpy-datum shift.

The runner returns a nonzero exit code when the preserved dataset and executable comparison are internally inconsistent.

---

## 7. Preserved Execution Results

Successful workflow run `32385632109` reported:

```text
Evaluated holdouts: 12
p_ref [MPa]: 0.1
T_m,ref [K]: 273.152617521
c_s,fit [J/(kg*K)]: 2000.0390434600001
c_l,fit [J/(kg*K)]: 4183.862337324999
L_ref [J/kg]: 333426.89415089996

max |T error| [K]: 0.1833188082997026
mean |T error| [K]: 0.11670089323904402
max |h error| [J/kg]: 532.032172406849
mean |h error| [J/kg]: 355.0632066109268

latent-heat error [J/kg]: 0
melting-Temperature error [K]: 0
reference-datum shift invariance: PASS

Validation comparison: COMPLETED — errors reported.
```

The zero latent-heat error and zero melting-Temperature error are calibration identities for this benchmark configuration. They shall not be interpreted as independent predictive Validation of those two fitted quantities.

The nonzero holdout errors quantify the deviation introduced by the bounded constant-heat-capacity approximation over the selected benchmark ranges.

---

## 8. Coexistence Mapping Observation

At the selected coexistence condition, the normalized IAPWS phase enthalpies are not numerically identical to the ThermoCore constant-Cp transition thresholds even though the latent interval width is calibrated to the IAPWS latent heat.

The frozen dataset records an offset of approximately `268.148 J/kg` between the IAPWS coexistence enthalpies and the corresponding ThermoCore transition thresholds under the selected common datum and fitted sensible branch.

Consequently, supplying the normalized IAPWS liquid coexistence enthalpy directly to the ThermoCore recovery function produces approximately:

```text
T_recovered = 273.216708549 K
```

rather than exactly `T_m,ref`.

This is an observed consequence of fitting a bounded constant-`c_s` sensible branch while preserving one common energy datum. It is not hidden by the Validation record and it is not treated as an implementation defect by this result alone.

---

## 9. Invalid Predecessor Run

The first workflow execution is preserved as historical evidence:

```text
Workflow run: 32385437938
Job: 96478667366
Result: INVALID
```

The failure was caused by an evidence-annotation defect in the initial CSV: the liquid coexistence row incorrectly stored `T_m,ref` as the expected recovered Temperature even though its normalized IAPWS enthalpy lies above the ThermoCore liquid transition threshold.

The executable comparison detected the mismatch and terminated with:

```text
expected 273.152617521 K
actual   273.2167085491421 K
```

The CSV annotation was corrected in commit:

`3cfe3a1d8962a74f89c64064700d3ef97c1f9a83`

No Framework, reference-formulation, or implementation source change was required to obtain the subsequent successful run.

---

## 10. Validation Conclusion

Result status:

```text
COMPLETED — errors reported
```

The comparison procedure executed successfully against 12 independent sensible-branch holdout states, preserved the measured error metrics, and confirmed common enthalpy-datum shift invariance for the comparison.

No physical PASS/FAIL threshold was defined before execution. Therefore this record does **not** convert the measured errors into a physical `PASS` claim.

The result supports quantitative characterization of the bounded constant-Cp reference implementation within the declared local caloric benchmark. It does not establish validity for excluded density, transport, geometry, flow, conduction, GPU, or performance behavior, and it does not establish Framework Conformance by itself.

---

## 11. Known Limitations

- H2O is used only as a bounded caloric benchmark; its real solid/liquid density difference is outside the modeled reference scope.
- `c_s` and `c_l` are constant fitted values rather than full temperature-dependent IAPWS heat capacities.
- The melting Temperature and latent interval width are calibrated quantities in this run, not independent holdout predictions.
- The benchmark is fixed at `0.1 MPa` and does not validate pressure evolution.
- The holdout range is limited to the temperatures recorded in the frozen CSV.
- No conduction or spatial transient benchmark is included.
- No physical or application acceptance threshold has yet been adopted.

---

## 12. Evidence Integrity

This result is tied to the identified implementation baseline, Validation code/data commit, workflow run, and frozen CSV.

A materially different implementation, dataset, procedure, pressure, calibration interval, or conclusion shall be recorded as a new or versioned Validation result rather than silently replacing this evidence.
