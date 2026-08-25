# ThermoCore

English | [繁體中文](README_zh-TW.md)

[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.22096343.svg)](https://doi.org/10.5281/zenodo.22096343)
[![Release](https://img.shields.io/badge/release-v1.1.0-blue)](https://github.com/ra-an-y/ThermoCore/releases/tag/v1.1.0)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

ThermoCore is an engine-agnostic framework designed for real-time thermodynamic simulation.

The framework separates thermodynamic computation from material representation, enabling reusable thermodynamic-state computation while supplying material-specific configuration independently of computation.

This repository contains the normative Framework Specification, a bounded C# reference implementation, Verification and Validation evidence, Performance records, and supporting research.

---

## Framework Design

- Separation of Thermodynamic Computation and Material Representation
- Explicit Thermodynamic State ownership
- Engine-agnostic architecture
- GPU-oriented architectural design
- Explicit extension boundaries

### Current Reference Implementation

- Backend-independent C# reference implementation
- Enthalpy-based reference formulation
- Bounded Thermodynamic Computation and Thermodynamic State implementation slice
- Material Definition to compiled Configuration path

The reference implementation is intentionally bounded. It does not yet establish complete implementation or complete Framework Conformance of all four normative Core responsibilities.

---

## Conceptual Runtime Flow

```text
Energy Input
      │
      ▼
Thermodynamic Computation
      │
      ▼
Thermodynamic State
      │
      ▼
Material Representation
      │
      ▼
Representation Consumer
(outside Framework Core)
```

This simplified diagram shows the conceptual runtime dependency. Communication across these dependencies occurs through applicable Framework Interfaces. ThermoCore's normative Core Architecture consists of Thermodynamic Computation, Thermodynamic State, Material Representation, and Framework Interfaces.

The primary Framework Output is Thermodynamic State. Material Representation interprets state and applicable material information for downstream use by a Representation Consumer outside the Framework Core.

See the [Core Architecture specification](Documentation/Framework_Specification/Core_Architecture.md) for the authoritative architectural definition and the [Implementation Conformance Audit](Documentation/Implementation_Conformance_Audit_v0.1.md) for the current implementation scope.

---

## Extending ThermoCore

Adding a new physical mechanism is not decided by the mechanism name or coupling strength alone. ThermoCore first asks whether the selected thermodynamic formulation remains complete through semantically honest communication with the Core, then whether an accepted ordinary extension preserves authoritative Core-State semantics and ownership.

See the [Extension Design Guide](Documentation/Extension_Design_Guide.md) for a practical decision flow covering extension admissibility, state and information classification, energy-exchange accounting, feedback, and composition of multiple extensions.

---

## Documentation

- [Specification Index](Documentation/Specification_Index.md) — normative specification map and reading order
- [Core Architecture](Documentation/Framework_Specification/Core_Architecture.md) — authoritative Core responsibilities and boundaries
- [Extension Design Guide](Documentation/Extension_Design_Guide.md) — practical extension decision flow
- [Framework Vocabulary](Documentation/Framework_Vocabulary.md)
- [Implementation Conformance Audit](Documentation/Implementation_Conformance_Audit_v0.1.md)
- [Repository Governance](Documentation/Repository_Guidelines/Repository_Governance.md)
- [Research Guide](Research/README.md)
- [Validation Evidence](Validation/README.md)
- [Performance Evaluation](Performance/README.md)

---

## Validation Evidence

ThermoCore currently publishes two separate bounded caloric Validation tracks for the reference formulation.

| Validation track | External basis | Repository-published result |
|---|---|---|
| [H2O caloric benchmark](Validation/Reference_Formulation_Caloric_Validation_v0.1.md) | IAPWS reference formulations | `COMPLETED — errors reported` |
| [Gallium caloric benchmark](Validation/Reference_Formulation_Gallium_Caloric_Validation_v0.1.md) | NIST Chemistry WebBook SRD 69 / NIST-JANAF | `COMPLETED — errors reported` |

Here, `errors reported` refers to measured deviations from the declared external references, not execution failures. Neither track adopts a physical PASS/FAIL threshold, establishes complete Framework Validation, or implies Framework Conformance.

---

## Repository Status

The Framework Specification and repository governance baselines have been established.

A bounded thermodynamic reference formulation has been implemented and has Verification evidence within its stated scope. Two separate caloric Validation tracks are published, and bounded CPU Performance Evaluation records are preserved under `Performance/`.

ThermoCore v1.1.0 is the current stable public repository release. It refines the Framework specification, closes the bounded RQ-001 research package, and improves extension guidance and publication traceability while preserving the existing bounded reference-implementation scope.

The production C# reference implementation is not materially changed relative to v1.0.0, and v1.1.0 does not claim complete implementation or complete Framework Conformance of all four normative Core responsibilities.

Future releases may publish additional Validation Evidence and optional Reference Applications. Reference Applications are not mandatory release contents.

---

## Citation

ThermoCore v1.1.0 is archived on Zenodo as a version-specific software release.

- Version: `v1.1.0`
- DOI: [10.5281/zenodo.22096343](https://doi.org/10.5281/zenodo.22096343)
- GitHub Release: [ThermoCore v1.1.0](https://github.com/ra-an-y/ThermoCore/releases/tag/v1.1.0)

When citing this software release, use the version-specific Zenodo DOI above or the citation metadata in `CITATION.cff`. The earlier v1.0.0 archive remains immutable at DOI `10.5281/zenodo.22053832`.

---

## License

Licensed under the [Apache License 2.0](LICENSE).
