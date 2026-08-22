# ThermoCore

English | [繁體中文](README_zh-TW.md)

[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.22053832.svg)](https://doi.org/10.5281/zenodo.22053832)
[![Release](https://img.shields.io/badge/release-v1.0.0-blue)](https://github.com/ra-an-y/ThermoCore/releases/tag/v1.0.0)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

ThermoCore is an engine-agnostic framework for real-time thermodynamic simulation.

The framework decouples thermodynamic computation from material representation, allowing reusable state computation while keeping material models independent from the simulation core.

---

## Features

- Decoupled Thermodynamic Computation Layer
- Material Representation Layer
- Enthalpy-based State Update
- GPU-oriented Design
- Engine-agnostic Architecture

---

## Architecture

```text
Energy Input
      │
      ▼
Thermodynamic State Update
      │
      ▼
Material Representation
```

---

## Documentation

- [Specification Index](Documentation/Specification_Index.md)
- [Framework Vocabulary](Documentation/Framework_Vocabulary.md)
- [Repository Governance](Documentation/Repository_Guidelines/Repository_Governance.md)
- [Research Guide](Research/README.md)
- [Validation Evidence](Validation/README.md)
- [Performance Evaluation](Performance/README.md)

---

## Validation Evidence

ThermoCore currently publishes two independent bounded caloric Validation tracks for the reference formulation.

| Validation track | External basis | Repository-published result |
|---|---|---|
| [H2O caloric benchmark](Validation/Reference_Formulation_Caloric_Validation_v0.1.md) | IAPWS reference formulations | `COMPLETED — errors reported` |
| [Gallium caloric benchmark](Validation/Reference_Formulation_Gallium_Caloric_Validation_v0.1.md) | NIST Chemistry WebBook SRD 69 / NIST-JANAF | `COMPLETED — errors reported` |

These records preserve measured error against the declared external references. Neither track adopts a physical PASS/FAIL threshold, establishes complete Framework Validation, or implies Framework Conformance.

---

## Repository Status

The Framework Specification and repository governance baselines have been established.

A bounded thermodynamic reference formulation has been implemented and verified. Two independent caloric Validation tracks are published, and bounded CPU Performance Evaluation records are preserved under `Performance/`.

ThermoCore v1.0.0 is the first stable public repository publication baseline. Current work concerns post-v1.0 research, evidence consolidation, and additional bounded Validation or implementation work where independently justified.

Future releases may publish additional Validation Evidence and optional Reference Applications. Reference Applications are not mandatory release contents.

---

## Citation

ThermoCore v1.0.0 is archived on Zenodo as the first stable repository publication baseline.

- Version: `v1.0.0`
- DOI: [10.5281/zenodo.22053832](https://doi.org/10.5281/zenodo.22053832)
- GitHub Release: [ThermoCore v1.0.0](https://github.com/ra-an-y/ThermoCore/releases/tag/v1.0.0)

When citing this software release, use the citation metadata provided by the Zenodo record. The DOI identifies the archived v1.0.0 release; subsequent development on `main` does not alter that archived baseline.

---

## License

Licensed under the [Apache License 2.0](LICENSE).
