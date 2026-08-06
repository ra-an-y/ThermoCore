# Cross-Reference Audit v1.0

Version: 1.0  
Status: Documentation Audit  
Audit Phase: Documentation Cleanup v1.0 — Phase 1  
Audited Baseline Commit: `a5a63a8d096b5822a56dc3092dfd7215c723db93`  
Source Pull Request: [#11 — Add framework conformance specification](https://github.com/ra-an-y/ThermoCore/pull/11)  
Resulting Pull Request: [#12 — Add cross-reference audit report](https://github.com/ra-an-y/ThermoCore/pull/12)  
Resulting Commit: `6c7f6120976554cd0b5cccc554f3f577319f8ab6`  
Audit Date: 2026-08-06

---

> Historical baseline notice: This report records the result for the identified historical baseline. It does not certify later repository states.
>
> Resolution record: [`Integrity_Cleanup_v1.0.md`](Integrity_Cleanup_v1.0.md) records the bounded Phase 2A cleanup that addressed the findings reported here.

## 1. Audit Scope

This report audits the following normative Framework Specifications:

- `Framework_Principles.md`
- `Core_Architecture.md`
- `Data_Flow.md`
- `Thermodynamic_State.md`
- `Material_Representation.md`
- `Framework_Interfaces.md`
- `Extension_Boundary.md`
- `Framework_Conformance.md`
- `Specification_Governance.md`

The audit covers normative dependencies, internal cross-references, Single Source of Truth, governance duplication, and dependency direction.

This report does not modify, reinterpret, or replace any normative requirement.

## 2. Overall Result

**Needs Revision**

The normative dependency graph is acyclic and preserves parent-to-child authority. Every audited Framework Specification exists, and all declared parent filenames resolve.

However, three specifications contain five references to the nonexistent or obsolete filename `Verification_Specification.md`. In addition, `Specification_Governance.md` does not contain an explicit Normative Dependencies section even though its Document Status establishes a relationship to `Framework_Principles.md`.

These findings are documentation-integrity issues. This audit does not determine or propose a semantic change.

## 3. Dependency Matrix

The matrix below records explicit normative dependencies. The row order is topological rather than directory or publication order.

| Document | Explicit normative dependencies | Audit result |
|---|---|---|
| `Framework_Principles.md` | — | PASS — root normative specification |
| `Core_Architecture.md` | `Framework_Principles.md` | PASS |
| `Data_Flow.md` | `Framework_Principles.md`; `Core_Architecture.md` | PASS |
| `Thermodynamic_State.md` | `Framework_Principles.md`; `Core_Architecture.md`; `Data_Flow.md` | PASS |
| `Material_Representation.md` | `Framework_Principles.md`; `Core_Architecture.md`; `Data_Flow.md`; `Thermodynamic_State.md` | PASS |
| `Framework_Interfaces.md` | `Framework_Principles.md`; `Core_Architecture.md`; `Data_Flow.md`; `Thermodynamic_State.md`; `Material_Representation.md` | PASS |
| `Specification_Governance.md` | None explicitly declared | MINOR CLEANUP — Document Status identifies `Framework_Principles.md` as the root but no Normative Dependencies section is present |
| `Extension_Boundary.md` | `Framework_Principles.md`; `Core_Architecture.md`; `Data_Flow.md`; `Thermodynamic_State.md`; `Material_Representation.md`; `Framework_Interfaces.md`; `Specification_Governance.md` | PASS |
| `Framework_Conformance.md` | `Framework_Principles.md`; `Core_Architecture.md`; `Data_Flow.md`; `Thermodynamic_State.md`; `Material_Representation.md`; `Framework_Interfaces.md`; `Extension_Boundary.md`; `Specification_Governance.md` | PASS |

### 3.1 Dependency Direction

- No circular normative dependency was found.
- No child specification is declared as a normative dependency of its parent.
- Every declared dependency points from a refining child to an existing authoritative parent.
- Parent documents contain prospective references to later specifications in dedicated relationship or status sections. These references describe the refinement hierarchy; they are not normative dependencies and do not create reverse edges.
- `Specification_Governance.md` must precede `Extension_Boundary.md` and `Framework_Conformance.md` in a topological reading order because both explicitly depend on it.

## 4. Cross-Reference Verification

### 4.1 Broken or Obsolete References

| Referencing document | Occurrences | Referenced filename | Result |
|---|---:|---|---|
| `Material_Representation.md` | 2 | `Verification_Specification.md` | BROKEN — file does not exist in the audited Framework Specification set |
| `Framework_Interfaces.md` | 2 | `Verification_Specification.md` | BROKEN — file does not exist in the audited Framework Specification set |
| `Extension_Boundary.md` | 1 | `Verification_Specification.md` | BROKEN — file does not exist in the audited Framework Specification set |

The audit does not assume that `Framework_Conformance.md` is a mechanical replacement. Conformance semantics and Validation evidence have separate authority; the correct editorial target must be selected without changing that distinction.

### 4.2 Valid References

All references to the following filenames resolve within `Documentation/Framework_Specification/`:

- `Framework_Principles.md`
- `Core_Architecture.md`
- `Data_Flow.md`
- `Thermodynamic_State.md`
- `Material_Representation.md`
- `Framework_Interfaces.md`
- `Extension_Boundary.md`
- `Framework_Conformance.md`
- `Specification_Governance.md`

Self-references in Document Status or scope statements identify the current specification and do not create dependencies.

The future paths listed under `Framework_Conformance.md` are explicitly introduced as future Validation documents. They are roadmap references rather than claims that the files currently exist and are not classified as broken references by this audit.

### 4.3 Terminology

No filename spelling variation was found among the nine existing Framework Specifications. Existing terms used in cross-reference statements remain consistent with their authoritative document names.

## 5. Single Source of Truth Verification

| Normative concept | Authoritative specification | Result |
|---|---|---|
| Root Framework Principles | `Framework_Principles.md` | PASS |
| Core Architecture and core responsibility assignment | `Core_Architecture.md` | PASS |
| Information Flow | `Data_Flow.md` | PASS |
| Runtime State / Thermodynamic State semantics | `Thermodynamic_State.md` | PASS |
| Material Representation / Representation semantics | `Material_Representation.md` | PASS |
| Framework Interface and Communication semantics | `Framework_Interfaces.md` | PASS |
| Specification Governance | `Specification_Governance.md` | PASS |
| Extension Module semantics and boundaries | `Extension_Boundary.md` | PASS |
| Framework Conformance semantics | `Framework_Conformance.md` | PASS |

The authoritative ownership map remains distinguishable. Later specifications generally reference, constrain, or classify concepts within their assigned scope rather than claiming replacement authority.

### 5.1 Definition-Like Repetition Requiring Editorial Review

`Specification_Governance.md`, Section 7, gives short definition-like summaries of Runtime State, Configuration, and Representation. The section functions as a preservation rule, but its wording overlaps concepts authoritatively specified elsewhere. Phase 2 should determine whether these sentences are required governance summaries or can be converted to direct authoritative references. No change is made or prescribed by this audit.

No competing authoritative owner was found.

## 6. Governance Audit

The classifications below distinguish locally necessary boundary rules from governance text that substantially repeats `Specification_Governance.md`.

| Location | Statement or rule | Classification | Reason |
|---|---|---|---|
| `Framework_Interfaces.md` 7.1 | Communication Without Ownership | Required Local Rule | Applies the central Ownership Preservation and Interface Governance rules to the interface boundary |
| `Framework_Interfaces.md` 7.2 | Semantic Preservation | Required Local Rule | States the local communication obligation, although it overlaps the central Semantic Preservation Rule |
| `Framework_Interfaces.md` 7.3 | No Implementation Contract | Required Local Rule | Defines the interface-specific implementation-independence boundary |
| `Framework_Interfaces.md` 7.4 | Responsibility Preservation | Required Local Rule | Applies governance to responsibility boundaries connected by Framework Interfaces |
| `Framework_Interfaces.md` 7.5 | Extension Communication | Required Local Rule | Defines the applicable interface boundary for Extension Modules |
| `Extension_Boundary.md` 8.1 | Closed Core, Open Extension | Required Local Rule | Authoritative extension-boundary rule not replaced by general governance |
| `Extension_Boundary.md` 8.2 | Extend, Do Not Redefine | Required Local Rule | Defines extension-specific application of the Extension Governance Rule |
| `Extension_Boundary.md` 8.3 | Communicate, Do Not Bypass | Required Local Rule | Defines extension-specific interface compliance |
| `Extension_Boundary.md` 8.4 | Own Extension Information, Do Not Own Framework Information | Required Local Rule | Defines extension-specific ownership separation |
| `Extension_Boundary.md` 8.5 | Preserve Parent Specifications | Duplicate Governance | Substantially restates the Specification Dependency Rule without a distinct extension-only requirement |
| `Extension_Boundary.md` 8.6 | Preserve Implementation Independence | Required Local Rule | Prevents an extension implementation choice from becoming a Framework Core requirement |
| `Framework_Conformance.md` 8 | Named governance rules and preservation relationship | Required Local Rule | References central governance to define Conformance; it does not create alternative governance |
| `Specification_Governance.md` 7 | State Semantics summaries | Duplicate Governance candidate | Definition-like summaries overlap authoritative State and Representation specifications; editorial review is required before any change |

The presence of duplicated governance wording does not by itself establish a semantic conflict. Removal, consolidation, or conversion to a reference must be evaluated editorially and is outside this Phase 1 report.

## 7. Recommended Cleanup Items

All recommendations are editorial and must preserve normative semantics.

1. Resolve the five `Verification_Specification.md` references in `Material_Representation.md`, `Framework_Interfaces.md`, and `Extension_Boundary.md`.
   - Determine whether each sentence should reference `Framework_Conformance.md`, a future Validation document, or a non-file future specification description.
   - Do not mechanically rename the target where that would merge Conformance semantics with Validation evidence.
2. Normalize the dependency declaration of `Specification_Governance.md`.
   - If `Framework_Principles.md` is intended as its normative parent, state that relationship in an explicit Normative Dependencies section.
   - If governance is intentionally cross-cutting without a normative parent edge, state that explicitly.
3. Review `Extension_Boundary.md`, Governance Rule 5, for replacement by a direct reference to the Specification Dependency Rule while preserving its extension-specific applicability.
4. Review the definition-like summaries in `Specification_Governance.md`, Section 7, against the Single Source of Truth Rule. Prefer authoritative references if the summaries are not required locally.
5. Preserve prospective child references as non-dependency navigation. Do not add them to parent dependency lists.
6. Keep `Specification_Governance.md` before `Extension_Boundary.md` and `Framework_Conformance.md` in any published reading order or dependency visualization.

## 8. Phase 2 Readiness

Documentation Cleanup v1.0 may proceed to Phase 2 as an editorial cleanup phase.

Phase 2 should first resolve the broken or obsolete verification references and clarify the governance dependency declaration. These items can be addressed without changing normative architecture, ownership, semantics, or governance authority, provided that Conformance and Validation remain distinct.

---

## Audit Conclusion

The Framework Specification System has a coherent, acyclic normative dependency structure and a preserved authoritative owner for each core concept. The principal integrity defect is the remaining `Verification_Specification.md` filename, not a structural or semantic contradiction.

No Framework Specification was modified by this audit.
