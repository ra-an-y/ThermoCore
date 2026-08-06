# Repository Governance

Version: 1.0  
Status: Repository Governance

---

## 1. Purpose

This document establishes the root governance principles for operation and maintenance of the ThermoCore Git repository.

It defines how repository changes are proposed, recorded, reviewed, integrated, maintained, documented, and released.

This document does not define Framework architecture, Framework behavior, thermodynamic semantics, implementation behavior, or Framework Conformance.

## 2. Scope

This document governs repository-level practices for:

- contributions;
- repository maintenance;
- Commit Convention;
- Branch Convention;
- Pull Request Convention;
- documentation workflow;
- Validation Evidence preservation;
- Release Convention; and
- future contributor guidance.

These practices apply to Framework Specifications, research records, implementation artifacts, Validation documents, Reference Applications, repository metadata, and other maintained repository content.

Where a change affects a Framework Specification, the content of that change remains governed by the applicable Framework Specifications and `Specification_Governance.md`.

### 2.1 Derived Repository Convention Documents

The repository convention documents `Commit_Convention.md`, `Branch_Convention.md`, `Pull_Request_Convention.md`, and `Release_Convention.md` derive from this document.

They may refine repository procedures within their assigned scope, but shall not redefine or contradict the repository principles established here.

This establishes the following governance relationship:

```text
Repository_Governance.md
          │
          ├── Commit_Convention.md
          ├── Branch_Convention.md
          ├── Pull_Request_Convention.md
          └── Release_Convention.md
```

## 3. Governance Relationship

The ThermoCore governance system separates semantic authority from repository operation.

| Level | Responsibility |
|---|---|
| Framework Specifications | Define normative Framework requirements and authoritative Framework semantics. |
| `Specification_Governance.md` | Governs dependency, preservation, and separation of concerns across Framework Specifications. |
| Repository Governance | Governs how changes are contributed, reviewed, recorded, maintained, and released in the repository. |
| Git Workflow | Operationalizes Repository Governance through commits, branches, pull requests, merges, tags, and releases. |

The two governance axes are:

```text
Framework meaning:                 Repository operation:

Framework Specifications           Repository Governance
          ↓                                  ↓
Specification Governance           Git Workflow
```

These axes identify distinct governance responsibilities. Repository Governance is not a child of Specification Governance, is not a Framework Specification, and has no authority to define Framework semantics.

When a repository change affects Framework Specifications, both axes apply. The Framework meaning axis governs the proposed normative meaning, dependencies, ownership, and semantic preservation. The repository operation axis governs how the change is proposed, reviewed, recorded, integrated, and released.

Repository Governance may require a Framework Specification change to receive appropriate review and preserve traceability. It shall not determine the architectural meaning of that change.

## 4. Repository Principles

ThermoCore repository operation shall preserve the following principles:

1. **Traceable History**  
   Repository history shall preserve the origin, purpose, scope, and integration of material changes.

2. **Reviewable Changes**  
   Repository changes shall remain sufficiently focused and documented for meaningful review.

3. **Explicit Authority**  
   A repository artifact shall not acquire normative authority solely because it is committed, merged, tagged, or released.

4. **Normative Review**  
   Changes to Framework Specifications shall be reviewed against their declared Normative Dependencies, authoritative terminology, ownership, architecture, and applicable governance before merge.

5. **Evidence Preservation**  
   Validation activity shall preserve the evidence required to understand the evaluated version, procedure, configuration, result, and conclusion.

6. **Terminology Preservation**  
   Documentation shall preserve the authoritative terminology recorded by the Framework Specification System and `Framework_Vocabulary.md`.

7. **Governance Separation**  
   Repository governance shall remain independent of Framework semantics and shall not redefine Framework requirements.

8. **Authoritative Integration**  
   The default branch represents the current integrated repository state. Unmerged branches and Draft Pull Requests are proposed changes and are not authoritative repository baselines.

## 5. Contribution Principles

A contribution shall:

- have a defined purpose and bounded scope;
- identify the repository area it changes;
- explain why the change is necessary;
- separate unrelated concerns into independent changes;
- preserve applicable authoritative documents and terminology;
- distinguish normative, informational, implementation, research, and Validation content;
- include relevant checks, evidence, or review notes; and
- comply with the repository license and applicable attribution requirements.

A contribution shall not:

- use an implementation artifact to redefine a Framework Specification;
- present candidate research as an adopted Framework requirement;
- present a Reference Application as the Framework itself;
- convert Validation Evidence into a normative definition; or
- bypass the authoritative document that owns an affected concept.

Future contributors shall identify the applicable authority before changing a normative concept. If that authority is unclear, the contribution shall remain proposed until the ownership and dependency relationships are resolved.

## 6. Commit Convention

Each commit shall represent one coherent and reviewable change.

A commit message shall:

- state the primary change directly;
- describe the actual committed scope;
- remain concise and distinguishable in repository history; and
- avoid implying completion, Conformance, or Validation not supported by the commit.

A commit should not combine unrelated specification, implementation, cleanup, and evidence changes when those changes can be reviewed independently.

Files shall be staged intentionally. Unrelated local or contributor changes shall not be included silently.

Published shared history shall remain traceable. Corrections to integrated or shared work should be recorded through subsequent commits or pull requests rather than concealed through history rewriting.

## 7. Branch Convention

A working branch shall:

- begin from the intended current integration baseline;
- contain one bounded contribution or closely related change set;
- use a concise name that identifies its purpose;
- remain temporary rather than becoming an alternative authoritative baseline; and
- be checked for divergence from the target branch before merge.

The recommended branch form is:

```text
<category>/<short-purpose>
```

Examples of categories include `agent`, `docs`, `research`, `validation`, `feature`, and `fix`.

A branch name organizes work only. It does not determine the authority, status, or semantic meaning of its contents.

Changes shall enter the default branch through a reviewable integration process. Direct changes to the default branch should be limited to repository administration or exceptional recovery where the reason remains documented.

## 8. Pull Request Convention

A Pull Request shall provide:

- a clear title;
- a concise purpose and rationale;
- an explicit change scope;
- a list or summary of affected artifacts;
- relevant verification or review checks;
- disclosure of known limitations, unresolved matters, or deferred work; and
- confirmation that unrelated changes are excluded.

Draft Pull Requests may be used for incomplete or preliminary review. Draft status shall not be treated as approval or as an authoritative repository baseline.

Before merge:

- the complete diff shall be reviewed;
- the target branch and contribution scope shall be confirmed;
- applicable checks shall be completed or their absence explained;
- outdated dependencies and broken references introduced by the change shall be resolved;
- review findings shall be addressed or explicitly documented; and
- the Pull Request description shall accurately represent the final diff.

A Framework Specification change shall receive explicit normative review before merge. The review shall confirm preservation of applicable dependencies, semantics, ownership, architecture, terminology, and governance.

A Pull Request that includes Validation Evidence shall identify the evaluated version and preserve the relationship between the evidence and its conclusion.

## 9. Documentation Workflow

ThermoCore uses the following project documentation progression:

```text
Research
   ↓
Evidence
   ↓
Specification
   ↓
Implementation
   ↓
Verification
   ↓
Validation
```

This progression identifies distinct responsibilities:

- **Research** investigates questions and candidate ideas.
- **Evidence** records the basis for a framework decision.
- **Specification** defines adopted normative requirements.
- **Implementation** realizes applicable requirements without redefining them.
- **Verification** checks whether specified requirements are implemented.
- **Validation** evaluates the result for a stated validation purpose and preserves supporting evidence.

Repository placement, filenames, commits, or releases shall not collapse these responsibilities into one another.

Normative documentation changes shall follow `Specification_Governance.md`. Later or dependent documents shall reference authoritative definitions rather than duplicate them.

Informational documents may summarize or navigate authoritative material, but they shall state their non-normative status and shall not create competing definitions.

Editorial cleanup shall be distinguished from semantic change. If a documentation revision changes normative meaning, responsibility, ownership, dependency, or scope, it shall be reviewed as a normative change rather than represented as editorial cleanup.

## 10. Validation Evidence Preservation

Validation records shall preserve enough context to determine:

- the evaluated repository version or commit;
- the applicable Framework Specification baseline;
- the validation purpose;
- the procedure and relevant configuration;
- the observed result;
- the evaluation conclusion; and
- known limitations or deviations.

Historical Validation Evidence shall not be silently modified to represent a later run or different result.

When a procedure, configuration, implementation, or conclusion changes materially, the updated evaluation shall be recorded as new or versioned evidence with a traceable relationship to the earlier record.

A failed, incomplete, or superseded result may remain valuable evidence. Its status shall be stated explicitly rather than obscured or presented as a current passing result.

Validation Evidence supports Framework Conformance assessment. It does not define Framework Conformance or replace the applicable normative requirements.

## 11. Repository Maintenance

Repository maintenance shall preserve navigability, traceability, and authoritative boundaries.

Maintenance includes:

- correcting broken references;
- identifying obsolete or superseded material;
- preserving consistent directory and naming conventions;
- keeping navigation and status documents aligned with authoritative content;
- reviewing stale branches and incomplete Pull Requests;
- recording documentation cleanup separately from semantic revision when practical; and
- ensuring release metadata identifies the correct source state.

Removal, relocation, or replacement of an authoritative or evidentiary artifact shall preserve a traceable explanation and update affected references.

Repository cleanup shall not erase material decision history merely to simplify the current directory structure.

## 12. Release Convention

A ThermoCore release shall identify a fixed repository state.

Before release, the release scope shall be reviewed for:

- intended included artifacts;
- applicable Framework Specification versions;
- known Validation status;
- unresolved limitations;
- documentation consistency; and
- correspondence between the release contents and release notes.

A release shall:

- use a unique version or tag;
- identify the source commit;
- summarize material changes;
- distinguish normative changes from implementation, Validation, documentation, and Reference Application changes;
- state known limitations and incomplete Validation honestly; and
- avoid claiming Framework Conformance without applicable requirements and supporting evidence.

A published release tag shall be treated as an immutable reference. Corrections after publication shall be issued through a new commit and, when applicable, a new release.

When a release is intended for academic citation or long-term reference, it should be archived as a fixed artifact with a persistent identifier. The archived artifact shall correspond to the identified release state.

## 13. Future Contributor Guidance

A future contributor should begin by determining whether the proposed work concerns:

- Framework semantics;
- repository governance;
- research or evidence;
- implementation;
- Verification or Validation;
- a Reference Application; or
- an Extension Module.

The contributor shall then use the authoritative documents for that concern and keep the change within its declared scope.

If a proposed capability can be expressed within an existing Extension Boundary, its repository contribution shall preserve the Framework Core and applicable Framework Interfaces. Any proposal to change the Framework Core remains a Framework Specification matter and shall follow the applicable research, evidence, specification, and governance processes.

Questions about repository procedure shall be resolved through this document or later repository guidance derived from it. Questions about Framework meaning shall be resolved through the authoritative Framework Specification, not through branch names, commit messages, Pull Request discussions, implementation behavior, or repository convention.

## 14. Document Status

This document is the root governance document for ThermoCore repository operation.

It is authoritative for repository-level contribution, maintenance, Git workflow, documentation workflow, evidence preservation, and release practices.

It is not a Framework Specification.

It does not define or modify Framework architecture, Framework semantics, Framework Conformance, thermodynamic behavior, ownership, Information Flow, Runtime State, Material Representation, Framework Interfaces, or Extension Boundaries.

If repository procedure and Framework semantics appear to conflict, the applicable Framework Specification remains authoritative for Framework meaning, while this document remains authoritative only for the repository process used to propose, review, record, integrate, and release changes.
