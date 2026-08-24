# v1 Maintenance Release Readiness Review v0.1

Status: **COMPLETED — release-scope and third-party-usability review**  
Classification: **Non-Normative Release Readiness / Maintenance Review**  
Date: **2026-08-24**  
Tracking: GitHub Issue #159

---

## 1. Purpose

This review determines how the current post-v1.0.0 ThermoCore `main` baseline should be presented and versioned if another v1 release is prepared.

It does not create a release, tag, DOI, Framework requirement, or research claim.

The frozen v1.0.0 release remains unchanged.

---

## 2. Current Baseline

Frozen publication baseline:

```text
v1.0.0
SHA: 946af21d621369e3f19e4255bb64e506080fcaef
DOI: 10.5281/zenodo.22053832
```

At the time of this review, current `main` is 154 commits ahead of v1.0.0 and zero commits behind.

The compare result shows that the post-v1 change set is dominated by:

- completed RQ-001 research/evidence artifacts;
- research execution harnesses;
- citation and research-navigation additions;
- the user-facing Extension Design Guide;
- specification-index / vocabulary maintenance;
- a normative refinement of `Extension_Boundary.md` from document version 1.0 to 1.1.

No production file under `Framework/` or `Materials/Definitions/` appears in the current `v1.0.0...main` file-diff list.

Therefore the implementation profile is materially different from the normative/documentation baseline only in the sense that the repository now contains more explicit specification and guidance, not a new production thermodynamic solver implementation.

---

## 3. Third-Party Usability Path

The intended user path is now coherent:

```text
README
   ↓
Extension Design Guide
   ↓
Framework Specifications / Vocabulary
   ↓
Bounded reference implementation
   ↓
Tests / Validation / Performance evidence
```

### 3.1 README

The README now provides a visible `Extending ThermoCore` entry and links directly to `Documentation/Extension_Design_Guide.md`.

This lets a user start from a practical question — whether and how to add a new mechanism — without reading the RQ research history first.

### 3.2 Extension Design Guide

The guide translates the completed RQ-001 results into practical decisions:

1. formulation-relative admissibility first;
2. Core-State authority/non-promotion second;
3. information-category guidance;
4. energy-accounting guidance;
5. composition re-evaluation;
6. practical examples and a pre-integration checklist.

It explicitly leaves normative authority with the Framework Specifications.

### 3.3 Evidence navigation

`Framework/README.md`, `Tests/README.md`, `Validation/README.md`, and `Performance/README.md` each preserve their separate responsibilities and do not use one evidence layer as a substitute for another.

Third-party documentation-path disposition:

> **PASS — coherent enough for a bounded framework/reference-implementation repository, provided implementation scope is stated explicitly in the top-level README.**

---

## 4. Implementation and Conformance Wording

`Implementation_Conformance_Audit_v0.1.md` found:

```text
Implemented thermodynamic-computation slice: CONSISTENT
Persistent / Derived State treatment: CONSISTENT
Material Definition -> compiled Configuration: CONSISTENT
Energy Input dimensional mapping: CONSISTENT
Direct normative contradiction identified: NONE
Complete four-component Framework implementation: NOT ESTABLISHED
Complete Framework Conformance: NOT ESTABLISHED
```

The most important public-facing distinction is therefore:

> ThermoCore defines a four-responsibility Framework architecture, while the current backend-independent C# reference implementation realizes a bounded thermodynamic-computation/state/reference-formulation slice. It shall not be presented as a complete implementation or complete Conformance demonstration of every normative Core responsibility.

The top-level README should state this explicitly so that the architectural `Material Representation` feature is not mistaken for a currently complete production implementation of that responsibility.

---

## 5. Normative Delta from v1.0.0

The key versioning fact is not the number of research documents added after v1.0.0. It is the normative specification delta.

### 5.1 v1.0.0 baseline

Frozen v1.0.0 contains `Extension_Boundary.md` document version 1.0.

That version already required Extensions to:

- remain optional;
- preserve Framework Core completeness;
- preserve parent specifications;
- avoid redefining Runtime State or Core responsibilities;
- communicate through applicable Framework Interfaces;
- preserve ownership and semantics.

### 5.2 Current baseline

Current `main` contains `Extension_Boundary.md` document version 1.1.

It adds explicit normative treatment of **Ordinary Extension Admissibility**, including:

- admissibility relative to the applicable thermodynamic formulation and claimed scope;
- permitted semantically honest communication enrichment;
- prohibition on hiding required Thermodynamic State or Core responsibility in opaque communication, extension-local state, Configuration, Representation, or implementation indirection;
- explicit routing to formulation/Core revision or scope narrowing when the existing formulation is insufficient;
- governance rule `Admit by Formulation Completeness, Not Participation`;
- additional numbered normative Extension constraints.

These additions are consistent with and refine the earlier closed-Core/ownership rules, and they do not change the four Core components or assign a new Core owner.

However, they are more than editorial wording: they create an explicit normative admissibility criterion and additional conformance obligations for implementations that include Extensions.

Normative-delta classification:

> **BACKWARD-COMPATIBLE NORMATIVE REFINEMENT / MINOR SPECIFICATION EVOLUTION**

No evidence in this review requires classifying the change as a breaking Framework generation, but it is too substantive to describe the entire current baseline as a patch-only documentation correction.

---

## 6. Version Decision

### 6.1 v1.0.1

Disposition:

> **NO-GO for the current `main` baseline.**

Reason:

A patch version would imply that the public Framework contract remains at the v1.0.0 semantic level except for compatible fixes/clarifications. Current `main` includes a deliberate normative advancement of `Extension_Boundary.md` from 1.0 to 1.1 with a new explicit admissibility section, additional normative constraints, and a new governance rule.

Calling the whole baseline `v1.0.1` would understate that normative change.

This decision corrects the earlier conditional assumption that v1.0.1 would be appropriate **if** all post-v1 changes proved to be documentation/clarification only. The repository comparison shows that condition is not satisfied.

### 6.2 Lowest defensible next version class

Disposition:

> **v1.1.0 is the lowest defensible next release class for the current baseline.**

Why not v2.0.0:

- no four-component Core architecture change;
- no ownership reassignment;
- no production API break identified in the compare;
- no production `Framework/` implementation change identified in the current file-diff set;
- the Extension admissibility refinement operationalizes existing Core-preservation constraints rather than replacing the v1 architecture.

A later breaking API, state-semantic, ownership, or Core-architecture change would justify v2.0.0 independently.

---

## 7. Citation and Release Metadata

Until a new release is explicitly authorized and published:

- top-level release badge should remain `v1.0.0`;
- `CITATION.cff` should remain version `1.0.0` and DOI `10.5281/zenodo.22053832`;
- the existing Zenodo DOI must continue to identify the frozen v1.0.0 archive;
- `main` shall not be described as if it altered that archived artifact.

A future v1.1.0 publication should update release metadata only as part of the actual release preparation/publication sequence.

---

## 8. Remaining Release-Readiness Work

No new research or production-solver work is required merely to prepare a bounded v1.1.0 documentation/specification/research baseline.

Before actual publication, the remaining tasks are release engineering:

1. preserve explicit bounded reference-implementation wording in both READMEs;
2. prepare release notes that separate normative refinement, research/evidence additions, user guidance, and unchanged production implementation scope;
3. update version/citation metadata only on the release-preparation branch or final release commit;
4. run the applicable existing Verification/CI checks against the intended release commit;
5. create the tag/release/Zenodo record only after explicit publication authorization.

These are release tasks, not new Research Questions.

---

## 9. GO / NO-GO Summary

```text
Third-party documentation path:
    PASS

Current bounded implementation wording:
    PASS WITH TOP-LEVEL SCOPE CLARIFICATION

Complete Framework Conformance claim:
    NO-GO

v1.0.1 for current main:
    NO-GO

Lowest defensible next version class:
    v1.1.0

v2.0.0 required by current changes:
    NO

Prepare v1.1.0 release notes / metadata plan:
    GO

Create tag / GitHub Release / Zenodo record now:
    NOT AUTHORIZED BY THIS REVIEW
```

---

## 10. Final Release-Readiness Disposition

The current repository has reached a coherent bounded v1 maintenance/evolution baseline:

- the RQ-001 research line is closed;
- its useful decisions are translated into a user-facing Extension Design Guide;
- the current reference implementation has been audited against the refined specification without finding a direct contradiction;
- incomplete four-component implementation/conformance is explicitly identified rather than hidden;
- the post-v1 normative delta is understood.

If the project chooses to stop further framework evolution at this point, the appropriate release candidate is **v1.1.0**, not v1.0.1, because the repository now contains a real but non-breaking normative specification refinement.

This review authorizes preparation of release notes and release metadata planning only. Actual publication remains a separate explicit decision.