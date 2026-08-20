# Reference CPU Performance Evaluation Plan

Version: 0.1  
Status: Performance Plan — no performance conclusion yet

---

## 1. Purpose

This Performance Evaluation quantifies execution cost and scaling of the bounded C# ThermoCore reference implementation without changing its thermodynamic semantics.

The initial track is intentionally CPU-only and backend-independent. It does not evaluate GPU execution, Unity, Unreal, WebGL, mobile hardware, conduction, transport, rendering, or a complete application pipeline.

---

## 2. Evaluated Baseline

Initial evaluated repository baseline:

```text
Repository commit: 11be054721ca3334932e7db20b7fbb53aed894ea
Implementation profile: bounded constant-positive-Cp C# reference implementation
Persistent state: specific enthalpy h [J/kg]
Derived state: Temperature and liquid phase fraction
```

This commit is `main` after the second independent caloric Validation benchmark (PR #51).

No Framework Specification change is required by this evaluation.

---

## 3. Performance Questions

The first CPU track asks three bounded questions:

1. What is the measured per-cell cost of persistent-state enthalpy update?
2. What is the measured per-cell cost of recovering Temperature and liquid fraction from enthalpy?
3. What is the measured cost of performing update plus recovery in one per-cell loop?

The benchmark also observes how these costs change as the working set increases from cache-resident to larger arrays.

---

## 4. Benchmark Scenarios

### 4.1 State update

For each cell:

```text
ThermodynamicComputation.ApplySpecificEnthalpyIncrement
```

The updated `ThermodynamicState` is written back into the state array.

### 4.2 State recovery

For each cell:

```text
ReferenceThermodynamicFormulation.Recover
```

The input population cycles through solid, latent, and liquid enthalpy regions so all three bounded recovery branches are exercised. Recovered Temperature and liquid fraction contribute to a checksum consumed after timing.

### 4.3 Combined update + recovery

For each cell:

```text
ApplySpecificEnthalpyIncrement
then
Recover
```

The updated state is written back and recovered values contribute to the checksum.

---

## 5. Workload Sizes

The first run shall evaluate:

```text
1,024 cells
16,384 cells
262,144 cells
1,048,576 cells
```

Each timed sample shall process at least `1,048,576` cell operations by repeating smaller arrays enough times to reach that target. This avoids interpreting sub-millisecond timer noise as meaningful scaling.

The per-cell state array is allocated before timing. Allocation and initial population are not part of the reported operation timing.

---

## 6. Benchmark Configuration

A synthetic, fixed, valid reference material configuration shall be used only to keep benchmark semantics stable:

```text
rho_ref = 1000 kg/m^3
T_rho_ref = 300 K
T_E_ref = 250 K
T_m = 300 K
L = 250000 J/kg
c_s = 2000 J/(kg*K)
c_l = 4000 J/(kg*K)
```

This configuration is not a physical Validation material. Its values are not evidence about any real substance.

The benchmark state population cycles across representative solid, latent, and liquid enthalpy values under this configuration.

---

## 7. Timing Procedure

For each scenario and cell count:

- execute two untimed warmup samples;
- execute five timed samples;
- report the median elapsed time;
- also preserve minimum and maximum sample times to show runner noise;
- derive median nanoseconds per cell operation and cells per second;
- preserve an output checksum so benchmarked computation has an observable result.

The benchmark shall use .NET `Stopwatch` and Release configuration.

The execution harness shall print the available runtime environment information, including:

- .NET runtime description;
- operating-system description;
- process architecture;
- logical processor count visible to the process; and
- server-GC state.

---

## 8. Interpretation Boundary

GitHub-hosted runners are shared cloud benchmark environments. Absolute timings from one run shall therefore be recorded as environment-specific observations, not universal ThermoCore performance guarantees.

The first result may support statements such as:

- the measured cost on the identified runner;
- relative cost between the three benchmark scenarios in the same run;
- observed scaling across the four working-set sizes in the same run.

It shall not support claims about:

- production desktop CPUs generally;
- phones or tablets;
- GPU throughput;
- Unity or Unreal frame time;
- application-level thermodynamic simulation throughput;
- conduction / transport solver performance; or
- real-time suitability for a particular application without an application-specific requirement.

---

## 9. No Post-Hoc Performance PASS Threshold

No performance `PASS` threshold is adopted before the first measurements exist.

The initial result vocabulary shall therefore be descriptive:

```text
COMPLETED — measurements reported
INCOMPLETE — required evidence missing
INVALID — benchmark procedure or execution inconsistent
```

A later performance requirement may define an acceptance threshold only if the target workload, hardware class, and application requirement are stated before that threshold is used as a release claim.

---

## 10. Planned Evidence

```text
Performance/
  README.md
  Reference_CPU_Performance_Evaluation_Plan.md
  Reference_CPU_Performance_Evaluation_v0.1.md
  ThermoCore.ReferencePerformance.csproj
  Execution/
    Program.cs
```

A dedicated GitHub Actions workflow will execute the benchmark in Release configuration.

---

## 11. Stage Boundary

This activity changes no Framework architecture, ownership, state semantics, material semantics, or reference-formulation equations.

It is measurement of the current implementation only.
