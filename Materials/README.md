# Purpose

Materials exists to contain material definitions and related repository artifacts used by ThermoCore.

It keeps material-specific configuration artifacts separate from Framework implementation, research investigation, and authoritative specification text.

---

# Scope

This directory is responsible for material definitions, material-related profiles, and supporting artifacts that carry material-specific configuration information.

Framework solver implementation, general Framework semantics, research evidence, test logic, and Validation conclusions do not belong here.

---

# Relationship

Materials provides material-related artifacts that may be consumed by `Framework/`, exercised by `Tests/`, or used by Validation-related repository work.

The meaning and ownership of Material Representation remain defined by the applicable Framework Specifications in `Documentation/`; this directory guide does not define them.

---

# Current Status

Active — bounded reference material definition in progress.

`Definitions/ReferenceMaterialDefinition.cs` currently provides the reusable Configuration required by the constant-heat-capacity reference implementation profile, including `rho_ref`, `T_rho_ref`, `T_E_ref`, `T_m`, `L`, `c_s`, `c_l`, material identity, and provenance.

It does not contain evolving per-cell Thermodynamic State. Conversion from this reusable Material Definition into computation-ready Configuration is performed by Framework implementation logic.

---

# Notes

Material artifacts should remain distinguishable from implementation behavior and from evidence about Framework Conformance.

---

# Document Status

This document is a repository directory guide.

It provides repository navigation only.

It is not a Framework Specification.

It does not define Framework semantics or Repository Governance.
