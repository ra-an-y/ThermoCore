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

## Validation

The framework is evaluated through a staged validation series.

| Validation | Purpose | Status |
|---|---|---|
| V01 | Architecture Decoupling | In development |
| V02 | Continuous Phase Transition | Planned |
| V03 | Thermodynamic State | Planned |
| V04 | Energy Consistency | Planned |
| V05 | Performance | Planned |

---

## Repository Status

Current development focuses on establishing the framework architecture and validating the thermodynamic computation model.

Future releases will include additional validation reports and demonstration scenes.

---

## License

Licensed under the [Apache License 2.0](LICENSE).
