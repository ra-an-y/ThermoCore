# Commit Convention

Version: 1.0  
Status: Repository Guideline

---

## 1. Purpose

This document defines the official commit message convention used by the ThermoCore repository.

It governs commit history only. It defines commit message format, supported commit types, repository scopes, description style, breaking change notation, and repository-specific examples.

It does not define Framework behavior, Framework architecture, Framework semantics, repository governance, branch naming, Pull Request workflow, or release procedure.

## 2. Normative Dependency

This document derives from `Repository_Governance.md`.

It refines commit procedures only. It shall not redefine or contradict the repository principles established by `Repository_Governance.md`.

## 3. Conventional Commits Baseline

ThermoCore adopts [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/) as the repository baseline for commit messages.

This document defines ThermoCore-specific usage of that convention.

## 4. Commit Message Format

The standard commit message format is:

```text
<type>(<scope>): <description>
```

The components are:

| Component | Requirement | Purpose |
|---|---|---|
| `type` | Required | Classifies the primary repository change. |
| `scope` | Recommended | Identifies the principal repository area affected by the change. |
| `description` | Required | Summarizes the repository change. |

A scope may be omitted when no supported scope accurately describes the change.

## 5. Commit Types

ThermoCore supports the following commit types:

| Type | Purpose |
|---|---|
| `feat` | Introduce new repository functionality. |
| `fix` | Correct incorrect behavior. |
| `docs` | Add or revise documentation. |
| `refactor` | Restructure content or code without changing intended semantics or behavior. |
| `style` | Change formatting only. |
| `test` | Add or revise Validation or testing material. |
| `build` | Change the build system or continuous integration configuration. |
| `chore` | Perform repository maintenance not covered by another supported type. |

The selected type shall describe the primary purpose of the commit.

Additional Conventional Commit types may be adopted later if necessary. Any adoption shall remain consistent with `Repository_Governance.md` and shall be documented before regular use.

## 6. ThermoCore Scopes

The currently supported ThermoCore scopes are:

| Scope | Repository Area |
|---|---|
| `core` | Framework Core implementation or core repository functionality. |
| `spec` | Framework Specifications and specification-system documents. |
| `validation` | Validation procedures, evidence, results, or validation support. |
| `demo` | Demo or Sandbox Reference Applications. |
| `example` | Focused examples that demonstrate repository usage. |
| `readme` | Root or component README documentation. |
| `cleanup` | Bounded documentation or repository cleanup. |
| `repo` | Repository operation, metadata, organization, or Repository Guidelines. |
| `ci` | Continuous integration configuration and automation. |

A commit should use the narrowest supported scope that identifies its primary repository area.

The scope list is intentionally limited to current repository needs. Additional scopes may be introduced later when a demonstrated need exists and the new scope is documented through the repository governance process.

## 7. Description Style

The description shall:

- use imperative mood;
- begin with a lowercase letter;
- avoid a trailing period; and
- briefly describe the repository change.

Examples:

```text
feat(spec): add framework conformance specification
docs(cleanup): add terminology audit report
refactor(spec): simplify governance references
test(validation): add V01 architecture conformance
chore(repo): reorganize repository guidelines
```

The description shall describe the committed change without claiming Framework Conformance, Validation, or completion that the commit does not establish.

## 8. Breaking Changes

A breaking change shall be explicitly declared.

The commit may place an exclamation mark before the colon:

```text
feat(core)!: revise state interface contract
```

It shall also include a `BREAKING CHANGE:` footer that explains the incompatible change:

```text
feat(core)!: revise state interface contract

BREAKING CHANGE: replace the previous state interface contract
```

Breaking change notation records the impact of a commit. It does not determine Framework authority or replace the applicable Framework Specification review.

## 9. Squash Merge

Repository history should preserve one meaningful commit per reviewed Pull Request whenever practical.

When a Pull Request is squash merged, the squash commit message should follow this convention and summarize the final reviewed change rather than an intermediate state.

This recommendation refines commit history only. Pull Request review and merge procedures remain governed by the applicable Repository Guideline documents.

## 10. Historical Compatibility

Conventional Commits applies to future repository changes. Earlier commits remain valid historical records and are not required to be rewritten.

Commit messages such as:

```text
Add framework conformance specification
```

remain part of the valid repository history even though they do not follow the convention defined here.

This preserves the traceability and integrity of existing history while establishing a consistent convention for future work.

## 11. Repository Examples

### Specification

```text
docs(spec): clarify normative dependencies
feat(spec): add extension boundary specification
refactor(spec): simplify governance references
```

### Validation

```text
test(validation): add V01 architecture conformance
docs(validation): record energy consistency procedure
```

### Cleanup

```text
docs(cleanup): add terminology audit report
style(cleanup): normalize markdown table formatting
```

### Repository Governance

```text
docs(repo): add commit convention
chore(repo): reorganize repository guidelines
```

### README

```text
docs(readme): add specification navigation
docs(readme): update validation status
```

## 12. Relationship to Framework Authority

Commit messages improve repository traceability.

They do not determine Framework authority, normative status, semantic ownership, Framework Conformance, or Validation status.

Framework semantics remain governed by the Framework Specifications. Repository governance remains governed by `Repository_Governance.md` and its applicable derived Repository Guideline documents.

## 13. Document Status

This document is a Repository Guideline.

It is not a Framework Specification.

It refines commit procedures derived from `Repository_Governance.md` and does not define Framework behavior.
