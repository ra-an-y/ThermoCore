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

The experiment will first verify this bounded case before attempting general geometry.

### H3 — Material definition and evolving memory remain separate

Material/configuration describes reusable physical properties such as conductivity, volumetric heat capacity, diffusivity, and effusivity.

Any persistent information required because a finite layer has prior physical evolution must be represented separately as experiment-local evolving state. It must not be stored inside the material definition merely because it is material-specific.

### H4 — Representation remains downstream

A renderer may consume a Gaussian-compatible field representation, but rendering parameters such as color and opacity do not become authoritative thermodynamic state and must not write Thermodynamic State.

## 3. Responsibility Boundary

```text
Material / Configuration
        |
        v
Experimental propagation computation <--- experiment-local evolving state
        |
        v
Gaussian-compatible field representation
        |
        +----> physical field query
        |
        +----> downstream visualization
```

The experiment may own mechanism-specific local state and numerical basis data. It must not:

- redefine Thermodynamic State identity;
- write Thermodynamic State outside Thermodynamic Computation;
- reassign Framework ownership;
- require Gaussian parameters to become mandatory Framework Core State;
- let Representation write or govern thermodynamic evolution;
- reinterpret implementation-specific Gaussian behavior as a Framework requirement;
- count the same energy contribution as both internal redistribution and external input.

## 4. Initial Bounded Scope

The first implementation is intentionally narrow:

- one spatial dimension;
- homogeneous material regions;
- perfect thermal contact at a planar interface;
- constant material properties;
- diffusion-only thermal propagation;
- Gaussian heat-kernel basis;
- no rendering dependency;
- no modification of existing `Framework/` implementation files.

Excluded from the first checkpoint:

- arbitrary 2D/3D geometry;
- curved interfaces;
- anisotropic conductivity;
- phase change;
- temperature-dependent material properties;
- finite-layer reduced-order memory;
- Gaussian merge/split heuristics;
- 3DGS opacity/compositing semantics.

## 5. First Verification Checkpoint

The first checkpoint is successful only if the branch-local prototype can demonstrate all of the following in the bounded 1D perfect-interface problem:

1. the Gaussian kernel evaluator reproduces its declared scalar field;
2. interface coefficients are computed from declared material properties rather than manually fitted constants;
3. the constructed piecewise field satisfies temperature continuity within numerical tolerance;
4. the constructed piecewise field satisfies heat-flux continuity within numerical tolerance;
5. material/configuration remains immutable during propagation;
6. no existing ThermoCore Framework source file is modified to make the experiment work.

A successful checkpoint does **not** establish a new solver, Framework feature, or research contribution. It only establishes feasibility for the bounded experiment.

## 6. Promotion Rule

Nothing in this directory is assumed to belong in ThermoCore `main`.

If the experiment later identifies a genuinely generic Framework-level need, that need must be reviewed independently against the normative specification. Gaussian-specific implementation code is not promoted merely because the experiment succeeds.
