# Purpose

Tests exists to verify the correctness of ThermoCore implementation artifacts.

It supports repeatable detection of implementation defects without treating test execution as proof of Framework Conformance.

---

# Scope

This directory is responsible for checks whose primary purpose is to verify implementation behavior and correctness.

Framework Specifications, research investigation, material definitions, example applications, and Framework Validation evidence do not belong here.

---

# Relationship

Tests examines implementation artifacts in `Framework/` and may use supporting artifacts from `Materials/`.

Tests and Framework Validation have different responsibilities. Tests verify implementation correctness; Framework Validation provides evidence supporting Framework Conformance. Passing tests does not by itself establish Framework Validation success.

---

# Current Status

Active — bounded reference-formulation Verification in progress.

`ThermoCore.ReferenceVerification.csproj` builds the current backend-independent C# reference implementation together with deterministic verification cases in `Verification/Program.cs`.

The current verification slice checks material/reference-state compilation, `h -> T` and `h -> phi` recovery, phase-boundary invariants, dimensional Energy Input mapping, latent-energy consistency, immutable state evolution, and selected invalid-input guards.

Execution is automated by `.github/workflows/reference-verification.yml`. A passing run verifies only the tested implementation behavior and does not by itself establish Framework Conformance or physical Validation.

---

# Notes

Test results should be interpreted within the scope of the implementation behavior they examine.

---

# Document Status

This document is a repository directory guide.

It provides repository navigation only.

It is not a Framework Specification.

It does not define Framework semantics or Repository Governance.
