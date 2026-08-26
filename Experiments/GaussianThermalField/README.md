# Gaussian Thermal Field Experiment

Status: **Experimental / Branch-Local**  
Branch: `exp/gaussian-thermal-field`  
Framework authority: **None**  

This directory contains an exploratory implementation of Gaussian-based thermal-field propagation under the existing ThermoCore architecture.

It does **not** modify, replace, or reinterpret ThermoCore's normative Framework Specification. The experiment must conform to the existing Framework boundaries; any conflict is treated as an experiment failure or a reason to narrow the experiment, not as permission to change ThermoCore.

## 1. Experimental Question

Can a thermal field be represented with Gaussian basis functions whose parameters evolve under material-conditioned propagation rules, while preserving ThermoCore's separation of material/configuration, evolving physical state, computation responsibility, interfaces, and downstream representation?

A secondary question is whether the same Gaussian-compatible field description can support low-cost downstream visualization without granting rendering or representation authority over thermodynamic evolution.

## 2. Current Hypotheses

### H1 — Gaussian field representation

A scalar thermal field may be reconstructed from Gaussian basis terms:

```text
Phi(x,t) = sum_i A_i(t) G_i(x; mu_i, Sigma_i)
```

The Gaussian parameters are an experimental numerical/field representation. They are not automatically Thermodynamic State.

### H2 — Material-conditioned interface propagation

For the bounded one-dimensional perfect-contact case, an incident thermal Gaussian can be represented by incident, image/reflection, and transmitted Gaussian kernels whose coefficients depend on material diffusivity and thermal effusivity.

### H3 — Material definition and evolving memory remain separate

Material/configuration describes reusable physical properties such as conductivity, volumetric heat capacity, diffusivity, and effusivity.

Persistent effects of prior evolution in a finite layer belong to separate experiment-local current state. The current reduced-state prototype stores a mean temperature perturbation and a bounded set of cosine diffusion-mode coefficients; it does not store the sequence of past events.

### H4 — Representation remains downstream

A renderer may consume a Gaussian-compatible field representation, but rendering parameters such as color and opacity do not become authoritative thermodynamic state and must not write Thermodynamic State.

### H5 — Finite-layer history may be reduced to present modal state

For a finite homogeneous one-dimensional layer, the diffusion field can be expanded in cosine eigenmodes. Retaining a finite number of coefficients provides a bounded reduced current state. Under piecewise-constant inward boundary heat fluxes, each retained mode is updated analytically over a timestep.

This hypothesis is currently a reduced-order numerical experiment, not a claim that a finite mode count is sufficient for arbitrary layered-media accuracy.

### H6 — Gaussian representation may bridge into and out of bounded current state

An incoming Gaussian field may be projected into the finite-layer reduced current state without making Gaussian parameters authoritative state. After state-driven evolution, the current field may be approximated again with a fixed-size signed Gaussian mixture for downstream field representation.

The recovery representation is allowed to be approximate. It may not write back into the current state merely to improve rendering or representation quality.

### H7 — Useful Gaussian representation may be much smaller than the reduced state

The non-trivial count floor for representing a non-zero same-sign region is one Gaussian term. A bounded sparse-recovery study should therefore begin at one term rather than from the previous nine-term recovery design.

For each retained Gaussian count, the selected amplitudes should satisfy the region-integrated scalar constraint directly. An extra energy-correction Gaussian must not be assumed when measuring the minimum useful representation size.

## 3. Responsibility Boundary

```text
Gaussian / external field representation
        |
        v
Projection
        |
        v
Experimental current reduced state
        |
        v
State evolution <--- Material / Configuration + Interfaces
        |
        v
Current reduced state
        |
        v
Field reconstruction / Gaussian-compatible recovery
        |
        +----> physical field query
        |
        +----> downstream visualization
```

The experiment may own mechanism-specific local state and numerical basis data. It must not:

- redefine Thermodynamic State identity;
- write Thermodynamic State outside Thermodynamic Computation;
- reassign Framework ownership;
- require Gaussian or modal coefficients to become mandatory Framework Core State;
- let Representation write or govern thermodynamic evolution;
- reinterpret implementation-specific Gaussian behavior as a Framework requirement;
- count the same energy contribution as both internal redistribution and external input.

## 4. Current Bounded Scope

The implementation remains intentionally narrow:

- one spatial dimension;
- homogeneous material regions;
- perfect thermal contact at planar interfaces;
- constant material properties;
- diffusion-only thermal propagation;
- Gaussian heat-kernel basis for the perfect-interface checkpoint;
- finite-layer cosine reduced state for physical memory;
- state-driven A-B-C interface coupling;
- Gaussian-to-state projection;
- fixed-size signed Gaussian field recovery;
- constrained sparse Gaussian representation study from one term per region;
- no rendering dependency;
- no modification of existing `Framework/` implementation files.

Still excluded:

- arbitrary 2D/3D geometry;
- curved interfaces;
- anisotropic conductivity;
- phase change;
- temperature-dependent material properties;
- arbitrary multilayer networks;
- adaptive Gaussian merge/split heuristics;
- proof of globally optimal Gaussian mixture parameters;
- 3DGS opacity/compositing semantics.

## 5. Checkpoint 1 — Perfect Interface

The first checkpoint verifies the bounded 1D analytic Gaussian interface case:

1. Gaussian field evaluation is explicit and signed;
2. interface coefficients are derived from material properties;
3. temperature continuity is satisfied within numerical tolerance;
4. conductive-gradient continuity is satisfied within numerical tolerance;
5. material/configuration remains immutable during propagation;
6. no existing ThermoCore Framework source file is modified.

## 6. Checkpoint 2 — Finite-Layer Reduced Current State

The second checkpoint separates reusable material definition from current state that summarizes prior finite-layer evolution.

```text
Current Reduced State
├── mean temperature perturbation
└── bounded cosine diffusion-mode coefficients
```

It verifies energy accounting, autonomous modal decay, symmetry under equal boundary forcing, and future evolution without an event-history list.

## 7. Checkpoint 3 — Reduced-State Dimension / Convergence

A 256-mode cosine solution is used only as an internal numerical reference. Candidate states with 4, 8, 16, and 32 retained modes must show monotonically decreasing relative field error.

Current result for the declared constant-flux case:

```text
4 modes   : 4.98293052e-2
8 modes   : 2.03643098e-2
16 modes  : 8.26089068e-3
32 modes  : 3.44733256e-3
```

The current declared 32-mode target is `5e-3` relative L2 error. This is an experiment checkpoint, not a Framework requirement.

## 8. Checkpoint 4 — Independent Finite-Volume Reference

To avoid comparing the reduced model only against the same modal formulation, an independent cell-centered finite-volume solver is used as a numerical reference.

Current declared results:

```text
32-mode constant-flux relative L2 error : 2.38601679e-3
4-mode pulse-history relative L2 error   : 6.37315446e-6
```

The pulse-history case applies heat for an initial interval and then removes the input. Subsequent evolution therefore depends on the retained current state rather than an event log.

## 9. Checkpoint 5 — State-Driven A-B-C Coupling

Three finite homogeneous regions are coupled through two perfect-contact interfaces:

```text
Gaussian initial field in A
        |
        v
Current State A
        |
      q_AB  <- solved from current boundary response
        |
        v
Current State B
        |
      q_BC  <- solved from current boundary response
        |
        v
Current State C
```

`q_AB` and `q_BC` are not prescribed. At every timestep they are solved simultaneously from the current reduced states by requiring end-of-step temperature continuity at both interfaces. Equal and opposite interface fluxes are applied to adjacent regions.

For the declared 0.6 s heterogeneous test with 32 modes per region:

```text
relative L2 error vs heterogeneous finite volume : 3.41684060e-3
maximum interface temperature jump               : 4.99600361e-16
reduced-state energy drift                       : -1.66533454e-16
reference energy drift                           : 5.55111512e-17
fixed retained state scalars                     : 99
```

The retained state size remains fixed during the test. No reflection/transmission event tree or growing list of Gaussian paths is stored. This supports bounded current-state feasibility for the declared 1D case only; it does not establish a general multilayer solver.

## 10. Checkpoint 6 — Gaussian / Current-State Bridge

The sixth checkpoint tests the full bounded bridge:

```text
initial Gaussian field
        |
        v
Gaussian -> 32-mode reduced-state projection
        |
        v
state-driven A-B-C evolution
        |
        v
current reduced states
        |
        v
fixed-size signed Gaussian recovery
```

Projection uses numerical cosine coefficients on each finite region. Recovery fits eight fixed Gaussian basis terms per region and adds one broad signed correction term whose only purpose is to preserve the region-integrated scalar quantity represented by the state's mean term. The recovered Gaussian mixture remains downstream representation and never writes the current state.

Current declared results after the same 0.6 s heterogeneous A-B-C evolution:

```text
initial Gaussian -> 32-mode state relative L2 error : 7.82044259e-4
recovered Gaussian vs current state relative L2      : 2.87965236e-3
recovered Gaussian vs heterogeneous finite volume    : 4.44475782e-3
recovered representation energy error                : 2.42107889e-10
maximum interface temperature jump                   : 4.99600361e-16
fixed recovered Gaussian terms                       : 27
```

The fixed recovered size is three regions times nine Gaussian terms per region. It does not grow with simulation time or the number of prior interface interactions in this declared test.

## 11. Checkpoint 7 — Minimum Gaussian Representation Study

The seventh checkpoint starts from the non-trivial representation count floor of one Gaussian per region and increases the count one term at a time through eight terms per region.

The implementation uses a bounded candidate dictionary of signed normalized Gaussians with candidate centers allowed both inside and outside a region and multiple candidate widths. Selection is greedy sparse approximation. At every retained count, all selected amplitudes are re-solved together with an equality constraint:

```text
integral(recovered Gaussian mixture)
=
integral(current reduced-state field)
```

No separate energy-correction Gaussian is appended.

Current results for the same final A-B-C state are:

```text
N/region   total N   global vs state   max region vs state   vs heterogeneous FV
1          3         1.78720182e-2     8.21719069e-2         1.45266520e-2
2          6         8.08563239e-3     3.26982487e-2         7.42204209e-3
3          9         3.30042739e-3     1.19026298e-2         4.06435372e-3
4          12        1.49252539e-3     6.54091605e-3         3.72455466e-3
5          15        9.46420593e-4     4.49467842e-3         3.49082833e-3
6          18        6.72994244e-4     3.69951094e-3         3.43452398e-3
7          21        5.28476910e-4     3.23772557e-3         3.41819237e-3
8          24        4.56221906e-4     3.03041410e-3         3.39677029e-3
```

The constrained regional integral error remains at floating-point scale (maximum observed approximately `1.39e-17`).

For the declared `0.5%` representation-error threshold:

```text
first N/region with global state error <= 0.5%       : 3
first N/region with every-region state error <= 0.5% : 5
```

This distinction matters because the global norm can hide a relatively large error in a low-energy region. In the declared test, three Gaussians per region are sufficient for the global criterion, while five are needed for the stricter every-region criterion.

The finite-volume comparison approaches a floor near the existing reduced-state solver error as Gaussian count increases. This indicates that beyond a certain representation size, further Gaussian terms primarily reduce representation error rather than the underlying reduced-model error.

These counts are empirical minima for this bounded candidate dictionary, state, and threshold. They are not a proof of the globally minimal Gaussian count for arbitrary fields.

This supports the current bounded division of responsibility:

```text
Gaussian representation  -> compact input/output field description
Reduced current state     -> physical memory
Interface coupling        -> material-to-material exchange
Finite-volume solve       -> independent numerical reference
```

## 12. Build and Verification

The experiment has an isolated `.NET 8` executable project:

```text
Experiments/GaussianThermalField/ThermoCore.GaussianThermalField.Experiment.csproj
```

A branch-local GitHub Actions workflow builds the project and runs all experiment checkpoints without modifying ThermoCore's existing reference-verification workflow.

## 13. Promotion Rule

Nothing in this directory is assumed to belong in ThermoCore `main`.

If the experiment later identifies a genuinely generic Framework-level need, that need must be reviewed independently against the normative specification. Gaussian-specific or reduced-order implementation code is not promoted merely because the experiment succeeds.
