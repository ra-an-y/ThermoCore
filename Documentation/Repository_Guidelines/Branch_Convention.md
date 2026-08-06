# Branch Convention

Version: 1.0  
Status: Repository Guideline

---

## 1. Normative Dependency

This document derives from `Repository_Governance.md`.

It refines branch procedures only. It shall not redefine or contradict the root repository governance principles established by `Repository_Governance.md`.

`Commit_Convention.md` is a related Repository Guideline governing commit messages and commit history. It is not a normative parent of this document.

## 2. Purpose

This document defines the official branch convention used by the ThermoCore repository.

Repository branches:

- isolate proposed work;
- preserve reviewable scope;
- protect the integrated repository baseline; and
- support traceable Pull Request integration.

A branch is a repository workflow mechanism.

A branch does not determine Framework authority, normative status, Framework Conformance, or Validation status. It does not define Framework architecture, semantics, ownership, or implementation behavior.

## 3. Branch Model

ThermoCore currently uses a lightweight main-based workflow:

```text
main
  │
  └── working branch
          │
          ▼
     Pull Request
          │
          ▼
        main
```

Working branches are created from the intended integration baseline and return changes to `main` through Pull Requests.

ThermoCore does not use a permanent `develop` branch.

## 4. Branch Categories

ThermoCore supports the following branch categories:

| Category | Purpose |
|---|---|
| `agent/*` | Task-focused work performed through the current agent-assisted workflow. |
| `docs/*` | Documentation-only work. |
| `research/*` | Research records, surveys, evidence, or research-gap work. |
| `validation/*` | Validation documents, procedures, evidence, or related implementation. |
| `feature/*` | Repository functionality or implementation capability. |
| `fix/*` | Corrections to incorrect behavior or content. |
| `hotfix/*` | Urgent correction against an integrated or released baseline. |
| `release/*` | Temporary release-preparation work. |

These categories organize work only. They do not assign normative authority, Framework ownership, Conformance, Validation status, or other semantic meaning.

The `agent/*` category represents the current agent-assisted workflow. It is a supported category, not the only valid branch category. The other categories support clear use by current and future human contributors.

## 5. Branch Naming Format

Branch names use:

```text
<category>/<short-purpose>
```

Examples:

```text
agent/framework-conformance
agent/documentation-cleanup-phase2a
docs/specification-index
research/rq-002-evidence
validation/v01-architecture-conformance
feature/enthalpy-update
fix/extension-ownership-wording
hotfix/broken-release-reference
release/v1.0
```

A branch name shall:

- use lowercase ASCII;
- use hyphens between words;
- remain concise;
- describe the primary task;
- avoid spaces and underscores;
- avoid dates that do not communicate a relevant purpose;
- avoid personal names and vague terms; and
- avoid status terms such as `final`, `done`, or `approved`.

A branch category shall be selected according to the primary purpose of the proposed work.

## 6. Branch Scope

Each working branch shall:

- contain one bounded task or closely related change set;
- avoid unrelated specification, implementation, research, Validation, and repository-governance changes;
- preserve the authority boundaries of affected documents; and
- remain reviewable as one Pull Request.

If two changes require independent review or are governed by different authorities, they should use separate branches.

A branch shall not use a broad name or category to justify unrelated changes.

## 7. Branch Creation

A working branch shall begin from the intended current integration baseline.

The normal baseline is:

```text
main
```

Before substantive work begins, confirm:

- the target branch;
- the baseline commit;
- the proposed scope; and
- the affected repository area.

A branch created from an outdated baseline shall be updated before merge when required to preserve a correct and reviewable diff.

Branch creation does not approve the proposed work or grant authority to its contents.

## 8. Branch Lifecycle

The conceptual working-branch lifecycle is:

```text
Create from main
      ↓
Implement bounded change
      ↓
Open Draft Pull Request
      ↓
Review and revise
      ↓
Ready for Review
      ↓
Merge
      ↓
Delete branch
```

This lifecycle is a repository workflow. It does not define execution order for Framework behavior.

Draft status indicates proposed work only. It does not indicate approval, normative status, Framework Conformance, or Validation success.

Branch contents are not authoritative until integrated into the applicable repository baseline. Integration does not replace any additional authority or evidence required by the affected artifact.

## 9. Branch Synchronization

Before merge, verify that:

- the target branch is correct;
- the working branch is not unexpectedly behind;
- mergeability is known;
- conflicts are resolved; and
- the final diff remains within the declared scope.

Updating a working branch from `main` shall not introduce unrelated content.

Conflict resolution shall preserve the intended authority, semantics, and traceability of both the working branch and the current baseline.

This document does not prescribe one mandatory synchronization mechanism. Merge, rebase, or other methods permitted by repository policy may be used, provided traceability and reviewability are preserved.

## 10. Branch Protection and Integration

`main` is the current integrated repository baseline.

Changes should enter `main` through Pull Requests.

Direct changes to `main` should be limited to exceptional repository administration or recovery, with the reason preserved in repository history.

Normative Framework changes require explicit normative review before integration.

A successful merge does not by itself prove Framework Conformance or Validation success.

This document does not define detailed GitHub branch-protection settings. Such settings may be documented later as repository configuration guidance.

## 11. Branch Lifetime

Working branches should be short-lived.

After merge:

- delete the branch when it is no longer needed;
- preserve history through the merged Pull Request and integrated commit; and
- do not retain the merged branch as an alternative authoritative baseline.

A long-lived branch requires an explicit purpose, owner, and maintenance policy.

Long-lived status does not grant a branch authority equivalent to `main` or to a published release.

## 12. Abandoned and Superseded Branches

A branch that is abandoned, superseded, or no longer intended for merge should be closed or deleted after preserving any necessary research, evidence, decisions, or discussion in an appropriate repository artifact.

Branch deletion shall not be used to conceal important decisions or evidence.

Unmerged branch content shall not be cited as an authoritative repository baseline.

If material from an abandoned branch remains useful, it should be transferred to an appropriate maintained artifact with its status and origin preserved.

## 13. Relationship to Other Repository Guidelines

Repository Guideline responsibilities are separated as follows:

- `Commit_Convention.md` governs commit messages and commit history.
- `Pull_Request_Convention.md` will govern review and merge procedures.
- `Release_Convention.md` will govern release branches, tags, and publication.
- This document governs branches only.

A branch name shall not replace a clear commit message, Pull Request description, release record, or authoritative document status.

This document does not define commit message syntax, detailed Pull Request review procedure, a universal merge strategy, or release versioning.

## 14. Historical Compatibility

This convention applies to future branch creation and maintenance.

Earlier branch names remain valid historical records and do not need to be recreated or rewritten.

Existing merged branches do not require retrospective normalization.

Historical branch names do not establish new naming categories or override this convention for future work.

## 15. Document Status

This document is a Repository Guideline.

It derives from `Repository_Governance.md`.

It is not a Framework Specification.

It does not define Framework architecture, semantics, ownership, Framework Conformance, Validation results, commit message syntax, detailed Pull Request procedure, universal merge strategy, release versioning, or implementation behavior.
