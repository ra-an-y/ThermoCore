# Reference CPU SIMD Evaluation v0.1 — Erratum

Status: Performance Evidence Correction Notice  
Superseding result for implementation decisions: `Reference_CPU_SIMD_Evaluation_v0.2.md`

---

## Correction

`Reference_CPU_SIMD_Evaluation_v0.1.md` remains preserved as the historical record of its executed method and observations.

After PR #53 was merged, review identified a fairness limitation in the v0.1 scalar-reference timing path: the benchmark reconstructed a new `ThermodynamicState` from raw specific enthalpy inside every timed cell recovery.

The established scalar CPU recovery baseline instead operates on already-existing `ThermodynamicState` values. Therefore the v0.1 scalar-reference versus batch ratios include repeated input-state construction and finite-value validation that are not representative of the existing stored-state recovery path.

No Framework or reference-formulation defect was involved.

---

## Evidence Disposition

The following v0.1 evidence remains valid for its declared execution:

- the recorded hosted-run environment metadata;
- the recorded raw timing observations for the method actually executed;
- the semantic-equivalence gate results;
- the observation that hosted-run absolute timings vary materially; and
- the observation that incremental `Vector<double>` SIMD benefit was unstable across v0.1 runs.

The following v0.1 interpretation is superseded for implementation decisions:

```text
scalar-reference versus scalar-batch speedup ratios
```

The corrected comparison is recorded in:

- `Reference_CPU_SIMD_Evaluation_Plan_v0.2.md`
- `Reference_CPU_SIMD_Evaluation_v0.2.md`

v0.2 constructs and validates `ThermodynamicState[]` before timing and performs the current scalar recovery directly from that pre-existing state buffer.

---

## Repository Integrity

The erratum does not rewrite or delete the historical v0.1 result.

```text
Framework Specification change: None
Reference Formulation change: None
Framework implementation correction: None
Framework Freeze reopen: No
```
