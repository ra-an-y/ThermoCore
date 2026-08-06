# ThermoCore

English | [繁體中文](README_zh-TW.md)

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

---

## Validation Evidence

The table reports Validation Evidence currently published in this repository. It does not represent assumed local progress or unpublished work.

| Validation | Purpose | Repository-published Evidence |
|---|---|---|
| V01 | Architecture Decoupling | Not published |
| V02 | Continuous Phase Transition | Not published |
| V03 | Thermodynamic State | Not published |
| V04 | Energy Consistency | Not published |

A status in this table indicates repository publication only. Framework Conformance and Validation conclusions remain governed by their applicable authoritative artifacts.

---

## Repository Status

The Framework Specification and repository governance baselines have been established.

Current work concerns implementation preparation, Framework Validation preparation, evidence development, and supporting documentation.

Future releases may publish additional Validation Evidence and optional Reference Applications. Reference Applications are not mandatory release contents.

---

## License

Licensed under the [Apache License 2.0](LICENSE).
