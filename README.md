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

Current work concerns evidence consolidation, release readiness, and additional bounded Validation or implementation work where independently justified.

Future releases may publish additional Validation Evidence and optional Reference Applications. Reference Applications are not mandatory release contents.

---

## License

Licensed under the [Apache License 2.0](LICENSE).
