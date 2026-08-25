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

## 3. Responsibility Boundary

```text
Material / Configuration
        |
        v
Experimental state evolution <--- experiment-local current reduced state
        |
        v
Field reconstruction / Gaussian-compatible representation
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
- finite-layer cosine reduced state for the state-memory checkpoint;
- no rendering dependency;
- no modification of existing `Framework/` implementation files.

Still excluded:

- arbitrary 2D/3D geometry;
- curved interfaces;
- anisotropic conductivity;
- phase change;
- temperature-dependent material properties;
- general finite multilayer transfer operators;
- Gaussian merge/split heuristics;
- 3DGS opacity/compositing semantics.

## 5. Checkpoint 1 — Perfect Interface

The first checkpoint is successful only if the branch-local prototype can demonstrate all of the following in the bounded 1D perfect-interface problem:

1. the Gaussian kernel evaluator reproduces its declared scalar field;
2. interface coefficients are computed from declared material properties rather than manually fitted constants;
3. the constructed piecewise field satisfies temperature continuity within numerical tolerance;
4. the constructed piecewise field satisfies heat-flux continuity within numerical tolerance;
5. material/configuration remains immutable during propagation;
6. no existing ThermoCore Framework source file is modified to make the experiment work.

## 6. Checkpoint 2 — Finite-Layer Reduced Current State

The second checkpoint separates reusable material definition from the current state that summarizes prior finite-layer evolution.

The current reduced model uses:

```text
Current Reduced State
├── mean temperature perturbation
└── bounded cosine diffusion-mode coefficients
```

For constant inward heat flux over one timestep, the implementation verifies:

1. integrated energy change matches the net boundary heat input;
2. retained non-constant modes decay exponentially when both boundary inputs are zero;
3. equal inward heat flux at both boundaries does not excite odd cosine modes;
4. the reconstructed field preserves the corresponding mirror symmetry;
5. no event-history list is required to advance the current state;
6. material properties remain separate Configuration.

Passing this checkpoint does **not** establish that a small fixed mode count reproduces the full finite-layer Green function for all times or material contrasts. That accuracy question remains a later checkpoint.

## 7. Build and Verification

The experiment has an isolated `.NET 8` executable project:

```text
Experiments/GaussianThermalField/ThermoCore.GaussianThermalField.Experiment.csproj
```

A branch-local GitHub Actions workflow builds the project and runs the experiment checkpoints without modifying ThermoCore's existing reference-verification workflow.

## 8. Promotion Rule

Nothing in this directory is assumed to belong in ThermoCore `main`.

If the experiment later identifies a genuinely generic Framework-level need, that need must be reviewed independently against the normative specification. Gaussian-specific or reduced-order implementation code is not promoted merely because the experiment succeeds.
