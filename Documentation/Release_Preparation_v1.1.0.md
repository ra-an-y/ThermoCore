# ThermoCore v1.1.0 Release Preparation

Status: **DRAFT — release preparation in progress**  
Classification: **Non-Normative Release Engineering Record**  
Tracking: GitHub Issue #168

---

## 1. Purpose

This document prepares the content and release gates for ThermoCore v1.1.0.

It is not a Framework Specification, Research result, Verification result, Validation conclusion, Performance conclusion, Git tag, GitHub Release, or archival publication.

Actual publication remains separately authorized.

---

## 2. Planned Release Identity

```text
Repository release: v1.1.0
Planned tag: v1.1.0
Release date: TBD
Final baseline commit: TBD
Archival DOI: TBD — do not invent before archival publication/reservation
```

Release-preparation branch:

```text
release/v1.1.0
base commit: 09690fdddf47c890eb94592ff3067697eeec2d44
```

Frozen predecessor:

```text
v1.0.0
SHA: 946af21d621369e3f19e4255bb64e506080fcaef
DOI: 10.5281/zenodo.22053832
```

v1.0.0 remains immutable.

---

## 3. Release Positioning

v1.1.0 is a backward-compatible minor repository release centered on specification refinement, completed research/evidence consolidation, user-facing extension guidance, and repository/publication maintenance.

It is **not** a new solver generation and does not claim complete implementation or complete Framework Conformance of all four normative Core responsibilities.

The post-v1 comparison contains no production `.cs` implementation delta under `Framework/` or `Materials/Definitions/` relative to v1.0.0. The bounded backend-independent C# reference implementation scope therefore remains materially unchanged.

---

## 4. Major Changes Since v1.0.0

### 4.1 Normative Framework refinement

The principal normative change is `Documentation/Framework_Specification/Extension_Boundary.md` v1.1.

It adds explicit **Ordinary Extension Admissibility** relative to the applicable thermodynamic formulation and claimed thermodynamic scope. It permits semantically honest communication enrichment when sufficiency is preserved and prohibits hiding required Thermodynamic State, Core responsibility, or ownership changes inside extension-local state, opaque communication, Configuration, Representation, or implementation indirection.

The refinement preserves:

- the four Core responsibilities;
- existing ownership assignments;
- Thermodynamic Computation as the Core State-evolution/write responsibility;
- implementation/backend/engine independence;
- Extension optionality.

`Framework_Principles.md` v1.1 also contains maintained Verification/Validation wording that removes stale dependence on an obsolete numbered Validation series without changing Core architecture.

`Specification_Index.md` and `Framework_Vocabulary.md` are updated to v1.1 informational baselines consistent with the current specification/evidence system.

### 4.2 Completed RQ-001 research package

The original RQ-001 architectural research-gap line is closed.

Two bounded research contributions are retained:

1. **RQ-EFM-001 — Formulation-Relative Thermodynamic Extension Admissibility Boundary**.
2. **RQ-ISO-001 — Fixed Semantic/Core-State Boundary under Ordinary Extension**.

Decision order:

```text
Formulation / claimed scope
        ↓
RQ-EFM admissibility
        ↓
if ordinary Extension is admissible
        ↓
RQ-ISO authority / non-promotion
```

Five investigated lines were closed/reclassified as engineering/conformance properties rather than promoted as independent contributions:

- Formulation Change Containment Property;
- Conservative Exchange Accounting Property;
- Aggregate Re-Admissibility Property;
- Configuration-Derivative Identity Property;
- Downstream Representation Non-Authority Property.

The final RQ-001 synthesis and representative external evidence backbone are preserved under `Research/04_Research_Gap/`.

### 4.3 User-facing extension guidance

`Documentation/Extension_Design_Guide.md` translates the completed research and current Framework rules into a practical two-stage extension decision process:

1. formulation-relative admissibility;
2. Core-State authority / non-promotion.

It also covers information classification, energy-bearing exchange accounting, downstream feedback, and aggregate re-admissibility for combined extensions.

The guide is non-normative; Framework Specifications remain authoritative.

### 4.4 Implementation/conformance scope clarification

`Documentation/Implementation_Conformance_Audit_v0.1.md` records that the current reference implementation is a bounded semantically consistent thermodynamic-computation/state/reference-formulation slice.

It explicitly does **not** establish:

- complete Material Representation implementation;
- complete Framework Interface responsibility across a complete implementation;
- complete Framework Conformance.

Top-level English and Traditional Chinese READMEs now state this limitation directly.

### 4.5 Validation / evidence navigation maintenance

The current repository publishes two independent bounded caloric Validation tracks:

- H2O against IAPWS reference formulations;
- Gallium against NIST Chemistry WebBook SRD 69 / NIST-JANAF condensed-phase thermochemistry.

Recorded status for both remains:

```text
COMPLETED — errors reported
```

Neither track adopts a physical PASS/FAIL threshold or establishes complete Framework Validation or Framework Conformance.

Current specification/navigation wording has been aligned with the fact that these Validation artifacts are already published rather than merely future work.

### 4.6 Performance evidence preservation

Existing bounded CPU Performance Evaluation, batch attribution, derived-state validation-cost attribution, corrected SIMD comparison, and multi-thread scaling records remain preserved.

No universal speedup, default worker-count, GPU, Unity, mobile, NUMA, or production-hardware claim is introduced by v1.1.0.

### 4.7 Repository and publication maintenance

Post-v1 maintenance includes:

- `CITATION.cff` for the published v1.0.0 baseline;
- DOI and release badges/navigation;
- repository directory guides and release/conformance reviews;
- RQ-001 external citation backbone;
- removal of obsolete placeholder artifacts;
- retirement of historical working branch refs;
- preservation of `Framework/Interfaces/` as an intentional structural placeholder for a normative responsibility not yet completely implemented/evidenced.

---

## 5. Production Implementation Delta

Current repository comparison against v1.0.0 shows no production `.cs` implementation change under:

- `Framework/`; or
- `Materials/Definitions/`.

Therefore v1.1.0 shall not be described as a new production solver implementation release.

The bounded implementation continues to provide:

- specific-enthalpy persistent state;
- derived Temperature / liquid phase fraction recovery;
- compiled material Configuration;
- Energy Input dimensional mapping;
- Thermodynamic Computation state evolution;
- semantics-preserving batch recovery for the bounded reference formulation specialization.

---

## 6. Known Limitations

v1.1.0 does not establish:

- complete four-component Framework implementation;
- complete Framework Conformance;
- production Material Representation implementation;
- complete Framework Interface implementation/conformance evidence;
- GPU execution;
- Unity/engine integration;
- general CFD or multiphysics capability;
- universal thermodynamic formulation coverage;
- a physical Validation PASS/FAIL threshold;
- universal Performance guarantees;
- a Reference Application requirement.

These are not hidden deficiencies. They define the bounded scope of the release.

---

## 7. Release-Gate Execution

### 7.1 Required before publication

- bounded `Reference Verification` shall execute against the intended release-candidate baseline;
- final repository tree and version/reference scan shall be clean;
- release notes shall identify the final tag and baseline commit;
- README/CITATION/release metadata shall be synchronized only after the publication baseline and DOI policy are fixed;
- the v1.1.0 tag shall identify exactly the intended final release commit;
- publication shall occur only after explicit authorization.

### 7.2 Validation reproducibility

The H2O caloric Validation workflow already supports manual dispatch.

The v1.1.0 release-preparation branch adds `workflow_dispatch` to the Gallium caloric Validation workflow without changing the Validation program, inputs, benchmark basis, or conclusion logic.

This makes a fresh release-candidate reproducibility run possible without fabricating an unrelated repository content change.

A fresh run does not retroactively rewrite historical Validation evidence. Any new preserved result must identify its actual evaluated commit.

### 7.3 Performance

Re-running historical Performance studies is not a publication gate for v1.1.0 because production implementation code is unchanged by the post-v1 release delta.

If any future release-preparation change alters production implementation behavior, this decision must be re-evaluated before publication.

---

## 8. Publication Metadata Sequence

Until final publication preparation is explicitly authorized:

```text
README release badge: v1.0.0
CITATION.cff version: 1.0.0
CITATION.cff DOI: 10.5281/zenodo.22053832
published tag: v1.0.0
```

Do not invent a v1.1.0 DOI.

For final publication, establish the archival metadata strategy first. If a version-specific DOI can be reserved before the final metadata commit, use the reserved v1.1.0 DOI in the final citation metadata. Otherwise use an archival workflow that preserves exact traceability among the final repository commit, tag, GitHub Release, and resulting archive record without rewriting v1.0.0.

The final GitHub Release record shall identify the actual release commit even if a repository-contained preparation document necessarily used `TBD` before that commit existed.

---

## 9. Draft GitHub Release Notes

### ThermoCore v1.1.0

ThermoCore v1.1.0 is a backward-compatible specification, research, and documentation evolution of the v1.0.0 public baseline.

The release formalizes formulation-relative ordinary-extension admissibility in `Extension_Boundary.md` v1.1, closes the original RQ-001 architectural research line with two bounded retained contributions, adds a user-facing Extension Design Guide, strengthens external evidence/citation traceability, clarifies the bounded reference-implementation/conformance scope, and aligns public Validation navigation with the two published caloric benchmark tracks.

The production C# reference implementation is not materially changed relative to v1.0.0. It remains a bounded thermodynamic-computation, Thermodynamic-State, material-configuration, and reference-formulation slice. Complete Material Representation implementation, complete Framework Interface responsibility, and complete Framework Conformance are not claimed.

Published H2O and Gallium caloric Validation tracks remain `COMPLETED — errors reported`; neither uses a physical PASS/FAIL threshold or establishes complete Framework Validation/Conformance. CPU Performance records remain environment-specific bounded engineering evidence.

#### Major changes

- `Extension_Boundary.md` v1.1 ordinary-extension admissibility refinement.
- Completed RQ-001 synthesis and external evidence backbone.
- Retained RQ-EFM-001 and RQ-ISO-001 bounded contribution statements.
- Preserved negative/reclassified RQ findings as engineering/conformance properties.
- Added Extension Design Guide.
- Added implementation/conformance and release-readiness audits.
- Aligned Validation references/navigation with published H2O and Gallium tracks.
- Added/maintained citation and repository-publication metadata.
- Cleaned obsolete placeholders and historical working branch refs.

#### Known limitations

- no complete four-component Framework implementation or complete Framework Conformance claim;
- no GPU/backend performance claim;
- no universal Performance guarantee;
- no physical Validation PASS/FAIL threshold;
- no mandatory Reference Application.

Final release date, tag commit, and archival DOI remain to be inserted at publication time.

---

## 10. Release Preparation Checklist

- [x] Final repository audit merged.
- [x] v1.1.0 version class confirmed.
- [x] `release/v1.1.0` created from audited baseline.
- [x] Draft release scope and notes prepared.
- [x] Gallium manual Validation dispatch enabled on release branch.
- [ ] Review release-preparation PR checks.
- [ ] Run bounded Reference Verification on intended RC baseline.
- [ ] Optionally run fresh H2O and Gallium reproducibility checks on RC.
- [ ] Confirm no production behavior change entered release branch.
- [ ] Perform final version/reference scan.
- [ ] Fix final release-candidate commit.
- [ ] Establish/reserve v1.1.0 archival DOI/metadata strategy.
- [ ] Update README/CITATION metadata to actual v1.1.0 values.
- [ ] Review final metadata diff.
- [ ] Merge/fix final release commit as appropriate.
- [ ] Create immutable `v1.1.0` tag only after explicit authorization.
- [ ] Publish GitHub Release only after explicit authorization.
- [ ] Publish/preserve archival record and DOI only after explicit authorization.

---

## 11. Current Release-Preparation Disposition

```text
Release branch prepared:
    YES

Release scope bounded:
    YES

Production implementation behavior changed:
    NO

Framework semantic change introduced by release preparation:
    NO

Ready for RC Verification / final metadata phase:
    YES, after PR review

Ready to publish immediately:
    NO
```

The v1.1.0 release-preparation phase is intentionally limited to release engineering. No additional feature, Research Question, solver rewrite, or complete-Conformance work should be pulled into this release without an independently justified scope change.
