# Pull Request Convention

Version: 1.0  
Status: Repository Guideline

---

## 1. Normative Dependency

This document derives from `Repository_Governance.md`.

It governs Pull Request workflow only. It refines Pull Request procedures within the authority assigned by the root repository governance document and shall not redefine or contradict the repository principles established there.

Related Repository Guidelines are:

- `Commit_Convention.md`, which governs commit messages and commit history; and
- `Branch_Convention.md`, which governs branch procedures.

These related guidelines are not normative parents of this document.

## 2. Purpose

This document defines the official Pull Request convention used by the ThermoCore repository.

Pull Requests:

- provide a reviewable integration unit;
- preserve repository traceability;
- separate proposal from integration; and
- document review history.

A Pull Request is a repository review mechanism.

A Pull Request does not determine:

- Framework authority;
- normative status;
- Framework Conformance;
- Validation success; or
- repository release status.

## 3. Pull Request Lifecycle

The conceptual Pull Request lifecycle is:

```text
Working Branch
      │
      ▼
Draft Pull Request
      │
      ▼
Author Completion
      │
      ▼
Ready for Review
      │
      ▼
Technical Review
      │
      ├────────────── Request Changes
      │                       │
      │                       ▼
      │                    Revision
      │                       │
      │                       ▼
      │                Ready for Review
      │                       │
      │                       └──────► Technical Review
      │
      └────────────── Approved
                              │
                              ▼
                         Final Review
                              │
                              ▼
                            Merge
                              │
                              ▼
                        Delete Branch
```

This figure represents repository workflow only. It does not define Framework execution order, architecture, semantics, Conformance, or Validation procedure.

`Author Completion` indicates that the author has completed the proposed scope, updated the Pull Request description, and performed the checks expected before formal review.

`Ready for Review` indicates that the author believes the proposed repository change is complete and ready for formal review. It does not by itself imply approval, correctness, Framework Conformance, Validation success, or merge readiness.

Technical Review is performed by one or more reviewers after a Pull Request is marked Ready for Review.

When Technical Review requests changes, the Pull Request returns to revision. After the author completes the revision, the Pull Request is again presented as Ready for Review and returns to Technical Review.

Approval indicates that the applicable review has accepted the reviewed repository change. Final Review still verifies the current integration conditions before merge.

## 4. Draft Pull Requests

A Draft Pull Request represents proposed work that is incomplete or not yet ready for formal review.

Draft Pull Requests support:

- early discussion;
- work-in-progress visibility; and
- review before integration.

A Draft Pull Request may receive preliminary Editorial or Technical feedback. Such feedback does not replace the formal review required after the Pull Request becomes Ready for Review.

Draft Pull Requests shall not be treated as repository baselines, approved changes, Framework authority, Framework Conformance, or Validation Evidence.

## 5. Review Scope

Every Pull Request shall clearly describe:

- its purpose;
- its repository scope;
- the affected artifacts; and
- the expected outcome.

Review shall remain bounded by the declared change and the authority of the affected artifacts.

A Pull Request shall avoid unrelated repository changes. If changes require independent review, concern different authorities, or cannot be evaluated as one coherent integration unit, they should be separated.

The Pull Request description and final diff shall remain consistent throughout review.

## 6. Review Principles

Repository review evaluates proposed repository changes according to the following principles:

- **Correctness** — the change accurately performs or states its declared purpose.
- **Traceability** — the origin, rationale, affected artifacts, review findings, and outcome remain understandable.
- **Scope Consistency** — the final diff remains within the declared and reviewable scope.
- **Authority Preservation** — the change respects the authority and ownership boundaries of affected artifacts.
- **Documentation Consistency** — terminology, dependencies, references, and document status remain consistent.
- **Governance Compliance** — the change follows applicable Repository Guidelines and, when relevant, Framework Specification governance.

Review evaluates repository changes. It does not independently redefine Framework semantics.

When a Pull Request proposes a normative Framework change, reviewers evaluate that proposal against the applicable Framework Specifications and governance. The Pull Request discussion and review outcome do not replace those authoritative documents.

## 7. Review Levels

ThermoCore uses three Repository Review levels:

| Review Level | Purpose |
|---|---|
| **Editorial Review** | Evaluates formatting, wording, naming, readability, references, and consistency without changing intended semantics. |
| **Technical Review** | Evaluates technical content, architecture, governance, responsibility boundaries, authoritative terminology, and repository scope. |
| **Final Review** | Confirms before merge that applicable requirements, reviews, revisions, checks, scope, and integration conditions have been completed. |

These levels classify repository review responsibilities. They are not part of the Framework Specification System and do not establish Framework authority, Conformance, or Validation success.

Review levels may occur iteratively and do not require three different reviewers.

The applicable review level depends on the change:

- Editorial Review applies to presentation and consistency concerns.
- Technical Review is required when a change affects technical content, architecture, governance, responsibility boundaries, normative documentation, Validation interpretation, or other semantic content.
- Final Review applies before merge.

A change identified as editorial shall be escalated to Technical Review if it alters or may alter meaning, authority, ownership, requirements, or responsibility boundaries.

## 8. Technical Review

Technical Review is performed by one or more reviewers after a Pull Request is marked Ready for Review.

Technical Review evaluates:

- repository consistency;
- affected documentation and artifacts;
- governance compliance;
- terminology consistency;
- authority and responsibility boundaries;
- technical correctness within the declared scope; and
- repository scope.

Technical Review may result in:

- `Approved`; or
- `Request Changes`.

An approval applies to the reviewed state of the Pull Request. Material changes after approval shall receive renewed review when necessary.

Technical Review does not itself establish Framework Conformance, Validation success, scientific correctness, or release status.

Reviewers shall use the applicable Framework Specifications as authoritative sources when evaluating Framework-related content. Review comments shall not silently create replacement Framework definitions.

## 9. Revision

If review identifies issues:

- revise the existing Pull Request whenever practical;
- preserve review history;
- respond to or otherwise account for review findings;
- update the Pull Request description when its scope or outcome changes; and
- avoid creating a replacement Pull Request unless necessary.

A revised Pull Request shall be marked Ready for Review again when the author believes the requested work is complete.

A replacement Pull Request may be appropriate when the original scope becomes invalid, the change is superseded, the integration baseline changes materially, or preserving a coherent review is no longer practical. The relationship between the Pull Requests should then be documented.

Revision shall not erase important review decisions, unresolved limitations, or necessary evidence.

## 10. Merge Criteria

Before merge, Final Review shall verify that:

- applicable review is completed;
- requested revisions are addressed or explicitly resolved;
- the Pull Request description represents the final diff;
- repository scope is preserved;
- the target branch is correct;
- the working branch is appropriately synchronized;
- mergeability is confirmed;
- applicable checks are completed or their absence is explained;
- repository guidelines are satisfied; and
- known limitations and future work are recorded when relevant.

If the final state differs materially from the approved state, the Pull Request shall return to the applicable review before merge.

A successful merge means that the proposed repository change becomes part of the integrated repository baseline.

A successful merge does not by itself imply:

- Framework correctness;
- Framework Conformance;
- Validation success;
- scientific correctness;
- implementation quality; or
- release status.

## 11. Pull Request Description

A Pull Request description should include:

### Purpose

Explain why the change is proposed.

### Scope

Define the bounded repository change and identify excluded concerns when useful.

### Affected Documents

List or summarize the documents, implementation artifacts, evidence, or repository areas affected.

### Review Notes

Identify important review questions, completed checks, dependencies, or decisions.

### Known Limitations

State unresolved limitations, incomplete checks, or constraints honestly.

### Future Work

Record deferred work that is outside the current Pull Request scope.

The description should enable future repository readers to understand the change, its reason, its reviewed scope, and its known limitations without reconstructing the entire working process.

The description shall be updated when revision materially changes the final diff or expected outcome.

## 12. Relationship to Validation

Validation documents may reference Pull Requests to preserve traceability between a repository change and related evaluation activity.

Pull Requests themselves are not Validation Evidence.

A Pull Request discussion, approval, or merge shall not be presented as proof of Framework Conformance or Validation success.

Validation Evidence shall be interpreted and preserved according to the applicable Framework Specifications, Framework Conformance requirements, repository evidence-preservation rules in `Repository_Governance.md`, and applicable Validation documentation. It shall preserve the evaluated version, procedure, configuration, observations, results, and applicable conclusions.

## 13. Relationship to Other Repository Guidelines

Repository Guideline responsibilities are separated as follows:

- `Commit_Convention.md` governs commit messages and commit history.
- `Branch_Convention.md` governs branch naming, scope, lifecycle, synchronization, and maintenance.
- `Release_Convention.md` governs releases, tags, versioning, and publication.
- This document governs Pull Request proposal, review, revision, approval, and merge procedures only.

A Pull Request title, description, status, approval, or merge shall not replace a clear commit history, branch record, release record, or authoritative document status.

This document does not define commit syntax, branch naming, release workflow, Framework governance, or implementation behavior.

## 14. Historical Compatibility

This convention applies to future Pull Requests and to the continued maintenance of open Pull Requests when practical.

Earlier Pull Requests remain valid historical records.

They do not require retrospective modification, renaming, description changes, renewed review, or reconstruction of their original workflow.

Historical Pull Request practices do not override this convention for future repository work.

## 15. Document Status

This document is a Repository Guideline.

It derives from `Repository_Governance.md`.

It governs Pull Request workflow only.

It is not a Framework Specification.

It does not define Framework architecture, semantics, ownership, Framework Conformance, Validation results, commit message syntax, branch naming, release workflow, or implementation behavior.
