# ThermoCore v1.1.0 Final Metadata Preparation

Status: **DRAFT — awaiting reserved v1.1.0 Zenodo version DOI**  
Classification: **Non-Normative Release Engineering Record**

---

## 1. Purpose

This record governs final publication metadata preparation for ThermoCore v1.1.0 after completion of the release-candidate audit and bounded Reference Verification.

It does not authorize creation of the `v1.1.0` tag, GitHub Release, or Zenodo publication.

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

## 4. DOI Strategy

ThermoCore v1.1.0 shall use a **version-specific Zenodo DOI** linked as a new version of the existing ThermoCore Zenodo record.

The final repository metadata shall not invent or guess this DOI.

Because Zenodo's GitHub integration does not support pre-reserving a DOI before a GitHub Release, the selected preparation path is:

```text
existing ThermoCore v1.0.0 Zenodo record
        ↓
Create New Version draft
        ↓
Reserve version-specific DOI
        ↓
Insert reserved DOI into final repository metadata
        ↓
Review and merge final metadata commit
        ↓
Explicit publication authorization
        ↓
Create immutable v1.1.0 Git tag / GitHub Release
        ↓
Complete and publish the prepared Zenodo new-version record
```

The Zenodo Concept DOI, if present, is not a substitute for the v1.1.0 version-specific DOI in version-specific citation metadata.

The prepared Zenodo draft must not be deleted after its DOI is reserved, because deletion would invalidate the reservation.

If automatic GitHub-to-Zenodo ingestion is enabled for this repository, it shall be disabled before publishing the GitHub v1.1.0 Release while this manually prepared New Version draft is pending. Otherwise the GitHub Release may be ingested as a separate Zenodo version instead of completing the reserved-DOI draft. The prepared draft shall then be completed with the exact v1.1.0 release/tag archive and published manually; automatic integration may be re-enabled afterward for future releases if desired.

---

## 5. Metadata Changes to Apply After DOI Reservation

Only after the v1.1.0 version-specific DOI is reserved, update the following together:

### `CITATION.cff`

- `version`: `1.1.0`
- `doi`: reserved v1.1.0 version DOI
- `date-released`: actual publication date, set only when that date is fixed
- author, repository, title, type, and license remain unchanged unless independently justified

### `README.md`

- release badge / GitHub Release target → `v1.1.0`
- DOI badge / citation DOI → reserved v1.1.0 version DOI
- Repository Status wording → current stable release is v1.1.0
- Citation section → v1.1.0 version, DOI, GitHub Release
- preserve explicit bounded implementation / Validation / Conformance non-claims

### `README_zh-TW.md`

- mirror the same publication metadata changes
- preserve the same scope and non-claim semantics in Traditional Chinese

### Release engineering records

- replace remaining publication-time `TBD` values with the actual release identity where appropriate
- identify the final publication commit after metadata review
- preserve the RC baseline and verification run as historical traceability

---

## 6. Final Metadata Constraints

The final metadata update shall not:

- introduce new Framework semantics;
- modify production solver behavior;
- change RQ conclusions;
- rewrite historical Verification, Validation, or Performance evidence;
- claim complete four-component implementation;
- claim complete Framework Conformance;
- introduce a physical Validation PASS/FAIL conclusion;
- claim universal Performance or GPU capability;
- alter the frozen v1.0.0 archive.

If any non-metadata production or normative path changes after the verified RC baseline, the affected release gates shall be re-evaluated before publication.

---

## 7. Current Gate Status

```text
Final repository audit:             PASS
RC content/public-claim scan:       PASS
Production implementation delta:   PASS — no post-v1 production .cs change
Gallium reproducibility evidence:   PASS
Reference Verification on RC:      PASS
v1.1.0 version DOI reserved:        PENDING
Final README/CITATION metadata:     PENDING DOI
Final metadata diff review:         PENDING
Explicit publication authorization: NOT GIVEN
v1.1.0 tag:                         NOT CREATED
GitHub Release:                     NOT PUBLISHED
Zenodo v1.1.0 record:               NOT PUBLISHED
```

---

## 8. Next Required Input

Reserve the DOI in a Zenodo **New Version** draft derived from the existing ThermoCore v1.0.0 record, then provide the exact reserved DOI for insertion into the final repository metadata.

Do not publish or delete the Zenodo draft yet.
