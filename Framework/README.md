# Purpose

Framework exists to contain the implementation of the ThermoCore framework.

It provides the repository area in which Framework behavior is realized without making implementation artifacts a substitute for the specifications that define their intended semantics.

---

# Scope

This directory is responsible for implementation artifacts that realize the ThermoCore Framework and its defined interfaces.

Research records, material-definition artifacts, Framework Validation evidence, example applications, and repository governance do not belong here.

---

# Relationship

Framework implementation is guided by the applicable Framework Specifications in `Documentation/`.

It may consume material-related artifacts from `Materials/`, and its implementation correctness may be examined by `Tests/`. Framework Validation remains a separate responsibility from implementation and testing.

The bounded thermodynamic reference implementation is additionally guided by `Documentation/Thermodynamic_Formulation.md`, which is an authoritative non-Framework reference-formulation specification.

---

# Current Status

Active — bounded reference implementation established.

The current implementation profile is backend-independent C# with no Unity-specific runtime dependency. It realizes persistent specific-enthalpy state, compiled thermodynamic parameters, `h -> T` / `h -> phi` recovery, Energy Input dimensional mapping, Thermodynamic Computation state evolution, Material Definition compilation, and the semantics-preserving batch recovery API for a constant-positive-heat-capacity specialization of the reference formulation.

Implementation correctness is exercised by the bounded reference Verification under `Tests/`. Independent H2O and Gallium caloric Validation evidence is preserved under `Validation/`, and bounded CPU Performance Evaluation evidence is preserved under `Performance/`.

The current Material Definition compiler normalizes the zero-enthalpy datum from `T_E_ref` onto the solid sensible branch and therefore supports `T_E_ref < T_m` in this first bounded implementation profile. Broader reference-datum placement, variable heat-capacity inversion, GPU execution, engine integration, and Reference Applications remain outside the current bounded profile.

---

# Notes

The presence of an implementation artifact does not by itself establish Framework Conformance or Validation success.

---

# Document Status

This document is a repository directory guide.

It provides repository navigation only.

It is not a Framework Specification.

It does not define Framework semantics or Repository Governance.
