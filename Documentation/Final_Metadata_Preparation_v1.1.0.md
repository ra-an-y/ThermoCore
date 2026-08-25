# ThermoCore v1.1.0 Final Metadata Preparation

Status: **READY — final publication metadata fixed for 2026-08-25**  
Classification: **Non-Normative Release Engineering Record**

---

## 1. Purpose

This record governs final publication metadata preparation for ThermoCore v1.1.0 after completion of the release-candidate audit and bounded Reference Verification.

Publication has been explicitly authorized for 2026-08-25. This record does not itself create the `v1.1.0` tag, GitHub Release, or Zenodo publication; those remain separate publication actions.

---

## 2. Verified Release-Candidate Baseline

```text
RC baseline commit:
f9f63cb28634b9112c43fc7a745ab5345baf3ad3
```

This commit is the integrated `main` baseline produced by merging PR #169.

Required bounded Reference Verification was executed manually through `.github/workflows/reference-verification.yml` against this exact commit:

```text
Workflow: Reference Verification
Run: #19
Run ID: 32820298789
Event: workflow_dispatch
Branch: main
Evaluated SHA: f9f63cb28634b9112c43fc7a745ab5345baf3ad3
Conclusion: success
```

The `verify-reference-formulation` job and the `Run bounded reference verification` step both completed successfully.

The final PR-head Gallium caloric Validation run also completed successfully on a repository tree content-identical to the merged RC baseline.

---

## 3. Frozen Predecessor

ThermoCore v1.0.0 remains immutable:

```text
Version: v1.0.0
SHA: 946af21d621369e3f19e4255bb64e506080fcaef
DOI: 10.5281/zenodo.22053832
```

No v1.0.0 tag, archive, DOI metadata, or historical evidence shall be rewritten during v1.1.0 publication preparation.

---

## 4. Reserved v1.1.0 Archive Identity

A Zenodo **New Version** draft derived from the existing ThermoCore record has been created and saved.

The reserved version-specific DOI is:

```text
10.5281/zenodo.22096343
```

This DOI is reserved for the v1.1.0 version record and has been inserted into the repository's final publication metadata on the metadata-preparation branch.

The Zenodo draft remains unpublished and shall not be deleted before publication.

The repository owner has disabled automatic GitHub-to-Zenodo ingestion for ThermoCore before creation of the GitHub v1.1.0 Release. This prevents a GitHub Release event from creating a duplicate Zenodo version while the manually prepared New Version draft is pending.

Selected publication path:

```text
verified RC baseline
        ↓
Zenodo New Version draft
        ↓
reserve DOI 10.5281/zenodo.22096343
        ↓
insert DOI into README / CITATION metadata
        ↓
fix publication date 2026-08-25
        ↓
review final metadata diff
        ↓
fix publication commit
        ↓
create immutable v1.1.0 tag and GitHub Release
        ↓
complete prepared Zenodo draft with exact v1.1.0 archive
        ↓
publish Zenodo v1.1.0 record
```

The Zenodo Concept DOI, if present, is not a substitute for this v1.1.0 version-specific DOI in version-specific citation metadata.

---

## 5. Repository Metadata State

### `CITATION.cff`

Final prepared values:

```text
version: 1.1.0
doi: 10.5281/zenodo.22096343
date-released: 2026-08-25
```

Author, repository, title, type, and license remain unchanged.

### `README.md`

Prepared for the published v1.1.0 state:

- release badge and target → `v1.1.0`;
- DOI badge and citation DOI → `10.5281/zenodo.22096343`;
- Repository Status → v1.1.0 current stable public release;
- Citation section → v1.1.0 version-specific archive identity;
- bounded implementation, Validation, and Conformance non-claims preserved.

### `README_zh-TW.md`

The same publication metadata is mirrored in Traditional Chinese while preserving the same scope and non-claim semantics.

---

## 6. Metadata-Only Change Boundary

Changes after the verified RC baseline are restricted to publication metadata and non-normative release-engineering records.

They shall not:

- introduce new Framework semantics;
- modify production solver behavior;
- change RQ conclusions;
- rewrite historical Verification, Validation, or Performance evidence;
- claim complete four-component implementation;
- claim complete Framework Conformance;
- introduce a physical Validation PASS/FAIL conclusion;
- claim universal Performance or GPU capability;
- alter the frozen v1.0.0 archive.

If any production, test, Validation-program, research-result, or normative specification path changes after the verified RC baseline, the affected release gates shall be re-evaluated before publication.

---

## 7. Current Gate Status

```text
Final repository audit:             PASS
RC content/public-claim scan:       PASS
Production implementation delta:   PASS — no post-v1 production .cs change
Gallium reproducibility evidence:   PASS
Reference Verification on RC:      PASS
v1.1.0 version DOI reserved:        PASS — 10.5281/zenodo.22096343
GitHub-to-Zenodo auto-ingest:        DISABLED for v1.1.0 publication
README/CITATION version + DOI:       PASS
Actual publication date:            PASS — 2026-08-25
Final metadata diff review:         PASS
Final publication commit:           PENDING PR #170 merge
Explicit publication authorization: GIVEN — publish on 2026-08-25
v1.1.0 tag:                         NOT CREATED
GitHub Release:                     NOT PUBLISHED
Zenodo v1.1.0 record:               DRAFT — NOT PUBLISHED
```

---

## 8. Remaining Sequence

1. Merge PR #170 and fix the exact publication commit.
2. Create immutable `v1.1.0` tag from that exact final publication commit.
3. Publish the GitHub Release for `v1.1.0`.
4. Attach/import the exact v1.1.0 release archive into the prepared Zenodo New Version draft and verify its metadata.
5. Publish the Zenodo v1.1.0 record using DOI `10.5281/zenodo.22096343`.
6. Re-enable GitHub-to-Zenodo integration afterward if desired for future releases.
