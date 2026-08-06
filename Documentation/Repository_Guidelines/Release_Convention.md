# Release Convention

Version: 1.0  
Status: Repository Guideline

---

## 1. Normative Dependency

This document derives from `Repository_Governance.md`.

It governs repository release only. It refines release procedures within the authority assigned by the root repository governance document and shall not redefine or contradict the repository principles established there.

Related Repository Guidelines are:

- `Commit_Convention.md`, which governs commit messages and commit history;
- `Branch_Convention.md`, which governs working branches; and
- `Pull_Request_Convention.md`, which governs repository review and integration.

These related guidelines are not normative parents of this document.

## 2. Purpose

This document defines the official release convention used by the ThermoCore repository.

Repository releases:

- identify a stable repository baseline;
- preserve repository history;
- support reproducibility; and
- provide reference versions for documentation and research.

A repository release is a publication mechanism.

A repository release does not determine:

- Framework authority;
- normative status;
- Framework Conformance;
- Validation success; or
- scientific correctness.

## 3. Release Lifecycle

The conceptual release lifecycle is:

```text
Integrated Repository Baseline
            │
            ▼
Release Preparation
            │
            ▼
Release Review
            │
            ▼
Tag
            │
            ▼
Release Publication
            │
            ▼
Archived Release
```

This figure represents repository publication only. It does not define Framework execution order, Framework governance, Validation procedure, or archival-service behavior.

`Archived Release` represents a published release retained as repository history. It may also be preserved through a persistent archival service when long-term reference is required.

## 4. Release Criteria

Before creating a repository release, verify:

- the intended repository scope;
- completion of the applicable repository review;
- identification of the repository baseline;
- synchronization of relevant documentation; and
- preparation of release notes.

A release shall identify the repository state being published.

The release scope shall distinguish included artifacts from excluded, incomplete, or deferred work when that distinction is necessary to understand the baseline.

Meeting these release criteria establishes repository publication readiness only. It does not establish Framework Conformance, Validation success, scientific correctness, or implementation quality.

## 5. Release Readiness

Before repository release, ThermoCore should confirm that:

- applicable specifications are synchronized with the intended repository baseline;
- relevant documentation is updated;
- when the release claims completion of a Validation activity, corresponding Validation Evidence exists and identifies the evaluated baseline; and
- release notes are complete and consistent with the content being published.

Release Readiness is a ThermoCore repository practice. It is not a hosting-platform feature and does not replace the applicable specification, review, or Framework Validation process.

The presence of Validation Evidence is conditional on the claims made by the release. A repository release may publish incomplete or ongoing Validation work when its status and limitations are stated accurately.

## 6. Versioning

ThermoCore follows Semantic Versioning for repository releases.

Repository release versions use the form:

```text
vMAJOR.MINOR.PATCH
```

Examples include:

```text
v1.0.0
v1.0.1
v1.1.0
v2.0.0
```

The version components describe repository publication changes:

- **Major** — identifies a repository release containing incompatible or substantially reorganized published repository content.
- **Minor** — identifies a backward-compatible repository release adding material repository content or capability.
- **Patch** — identifies a backward-compatible repository release containing corrections, maintenance, or limited refinements.

The `v` prefix is part of the repository release and tag convention.

Version classification shall be based on the published repository baseline and described in the release notes.

This section defines repository versioning only. It does not define Framework version semantics, normative document version rules, Framework compatibility, Framework Conformance levels, or Validation status.

## 7. Release Contents

Release notes should include:

| Item | Purpose |
|---|---|
| Repository version | Identifies the published repository release. |
| Release date | Records when the release was published. |
| Associated tag | Connects the release record to its Git tag. |
| Baseline commit | Identifies the exact repository state being published. |
| Summary | Explains the purpose and overall content of the release. |
| Major changes | Describes material changes since the preceding relevant baseline. |
| Known limitations | Records incomplete work, constraints, or unresolved matters. |
| Related documentation | Identifies documents needed to understand the release. |
| Validation status, if applicable | Summarizes relevant Validation status without replacing its evidence. |

Release notes may also distinguish normative, implementation, Validation, research, documentation, Demo, and example changes when useful for traceability.

Release notes summarize repository content. They do not redefine repository artifacts, Framework Specifications, Validation Evidence, or authoritative document status.

If release notes and an authoritative repository artifact appear to differ, the artifact retains the authority assigned by its own governance and status.

## 8. Release Tags

Each repository release should have one corresponding Git tag.

A tag identifies a specific repository state. The associated release record shall identify the same baseline commit.

Release tags shall remain immutable after publication whenever practical.

A published tag shall not be moved to conceal or replace an earlier repository state. A correction should be recorded through a new commit and, when appropriate, a new repository version and tag.

A tag is a repository reference. It does not by itself establish Framework authority, Framework Conformance, Validation success, or scientific correctness.

## 9. Relationship to Validation

Validation may support a repository release.

Validation is not created by the release itself.

When release notes state a Validation status, the statement shall remain traceable to the corresponding Validation artifacts and evaluated repository baseline.

A tag, release record, publication date, or persistent identifier shall not be treated as Validation Evidence merely because it identifies a fixed repository state.

Validation remains governed by the Framework Validation process. This document does not define Validation procedures, acceptance criteria, evidence requirements, or conclusions.

## 10. Relationship to Framework Specifications

Repository releases publish repository artifacts.

Framework Specifications remain authoritative according to their own governance, Normative Dependencies, version status, and declared scope.

Publishing a repository release shall not change normative authority.

A Framework Specification does not gain, lose, or transfer authority solely because it is included in a release, identified by a tag, or preserved by an archive.

This document does not define Framework versions or the version semantics of individual Framework Specifications.

## 11. Relationship to Persistent Archives

A repository release may be archived through a persistent archival service capable of issuing persistent identifiers, such as a DOI.

A persistent identifier references a released repository baseline. The archived artifact shall correspond to the identified repository version, tag, and baseline commit.

Archival publication shall preserve repository traceability between:

- the persistent identifier;
- the repository release;
- the associated tag; and
- the baseline commit.

An archived release shall not be silently replaced with a different repository state under the same release record.

This convention does not prescribe a specific archival provider, DOI workflow implementation, metadata interface, or repository-hosting integration.

## 12. Historical Compatibility

This convention applies to future repository releases and to maintenance of current release records when practical.

Earlier releases remain valid repository history.

They shall not require renumbering, retagging, reconstruction, or retrospective modification solely to satisfy this convention.

Earlier version labels and release-note structures do not override this convention for future releases.

## 13. Future Release Branches

Release branches may be used when repository workflow requires temporary stabilization or preparation of a specific repository release.

The current lightweight workflow does not require permanent release branches.

When used, a release branch remains a working branch and is governed by `Branch_Convention.md`. This document governs only its release-preparation purpose and relationship to publication.

Release branch usage may evolve without changing `Repository_Governance.md`, provided the evolved procedure remains consistent with the root repository governance principles.

A release branch does not become an alternative authoritative baseline merely because it is intended for publication.

## 14. Relationship to Other Repository Guidelines

Repository Guideline responsibilities are separated as follows:

| Repository Guideline | Responsibility |
|---|---|
| `Commit_Convention.md` | Governs commit messages and commit history. |
| `Branch_Convention.md` | Governs working branches, including branch naming, scope, lifecycle, and synchronization. |
| `Pull_Request_Convention.md` | Governs repository review, revision, approval, and integration. |
| `Release_Convention.md` | Governs repository versioning, tags, release readiness, release records, and publication. |

This document does not redefine commit syntax, branch naming, Pull Request review, merge procedure, or root repository governance.

A release version or tag shall not replace a clear commit history, branch record, Pull Request review, Validation artifact, or authoritative document status.

## 15. Document Status

This document is a Repository Guideline.

It derives from `Repository_Governance.md`.

It governs repository release only.

It is not a Framework Specification.

It does not define Framework architecture, semantics, ownership, Framework Conformance, Validation procedures, Validation results, Framework version semantics, commit syntax, branch naming, Pull Request review procedure, DOI workflow implementation, or repository-hosting platform behavior.
