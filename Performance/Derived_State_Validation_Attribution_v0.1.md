# Derived State Validation Attribution v0.1

## Status

**COMPLETED — fine-grained attribution measurements reported**

Performance acceptance threshold: None.

Framework Conformance: Not claimed.

Physical Validation: Not claimed.

## Evaluated baseline

Repository implementation baseline:

`9e358fd8c75529ba6528d30d68fedf2ea18a525a`

This is `main` after PR #59 (`Implementation: make Derived State validation JIT-friendly`).

The final attribution harness is on PR #60 head lineage and does not modify `Framework/**` or `Materials/**`.

## Purpose

PR #56 established that validated `DerivedThermodynamicState` construction was the dominant measured cost layer in the tested recovery/output path. PR #59 reduced part of that layer without weakening the public invariant.

This evaluation asks a narrower question: for already-valid recovered values, how does the remaining output-construction cost change as benchmark-local validation predicates are added progressively?

It does **not** ask whether any invariant should be removed.

## Final controlled method

The first five timed scenarios write the same benchmark-local two-double `LayeredOutput` value type. Only the validation performed by the aggressively-inlined factory differs:

1. `raw_output` — assignment only.
2. `temperature_finite_output` — finite Temperature check.
3. `both_finite_output` — finite Temperature + finite liquid fraction.
4. `finite_lower_bound_output` — both finite + liquid fraction >= 0.
5. `local_full_validation_output` — both finite + liquid fraction in [0,1].
6. `public_derived_output` — actual repository `DerivedThermodynamicState` constructor.

The first five paths therefore control benchmark-local output type and layout while validation code is layered. The public path necessarily uses the real Derived State type.

All benchmark-local partial validators are measurement devices only. They are intentionally incomplete outside the valid-value timing domain and are not implementation candidates.

Timing procedure:

- value counts: 1,024; 16,384; 262,144; 1,048,576;
- 3 warmup samples;
- 7 timed samples;
- target 8,388,608 value constructions per sample;
- median/min/max timing;
- throughput and managed allocation recorded;
- checksum traversal outside the timed interval.

## Evidence gates

Both final controlled executions reported:

```text
public_and_local_full_invariant_sanity_gate: PASS
semantic_gate_max_temperature_error: 0
semantic_gate_max_liquid_fraction_error: 0
valid_domain_semantic_equivalence_gate: PASS
```

The sanity gate verifies that the real public constructor and the local full-validation mirror both reject non-finite Temperature, non-finite liquid fraction, liquid fraction below zero, and liquid fraction above one.

All timed scenarios reported median managed allocation of `0` bytes.

## Final controlled execution A

GitHub Actions workflow: `Derived State Validation Attribution`

Run: `32497864418`

Job: `96820475330`

Environment:

- .NET 8.0.30
- Ubuntu 24.04.4 LTS
- X64
- 2 logical processors visible
- Intel(R) Xeon(R) 6973P-C
- Server GC: false

At 1,048,576 values:

| Scenario | Median ms | Relative to preceding layer |
| --- | ---: | ---: |
| raw output | 9.379890 | — |
| Temperature finite | 13.414167 | 1.430x |
| both finite | 14.189975 | 1.058x |
| finite + lower bound | 14.501561 | 1.022x |
| local full validation | 15.350561 | 1.059x |
| public Derived | 14.998465 | 0.977x vs local full |

Same-run observations:

- raw -> Temperature finite: `+4.034277 ms`;
- Temperature finite -> both finite: `+0.775808 ms`;
- both finite -> lower bound: `+0.311586 ms`;
- lower bound -> full validation: `+0.849000 ms`;
- raw -> local full: `1.637x`;
- raw -> public Derived: `1.599x`;
- public Derived was about `2.3%` faster than the local full mirror in this execution.

## Final controlled execution B

The same workflow/job was re-run without source changes.

Run: `32497864418` (re-run attempt)

Job: `96821080019`

Environment:

- .NET 8.0.30
- Ubuntu 24.04.4 LTS
- X64
- 2 logical processors visible
- Intel(R) Xeon(R) Platinum 8573C
- Server GC: false

At 1,048,576 values:

| Scenario | Median ms | Relative to preceding layer |
| --- | ---: | ---: |
| raw output | 8.402374 | — |
| Temperature finite | 16.523278 | 1.967x |
| both finite | 16.275954 | 0.985x |
| finite + lower bound | 16.348650 | 1.004x |
| local full validation | 16.893929 | 1.033x |
| public Derived | 16.914598 | 1.001x vs local full |

Same-run observations:

- raw -> Temperature finite: `+8.120904 ms`;
- Temperature finite -> both finite: `-0.247324 ms` at the largest working set, showing that small incremental layers can be obscured by JIT / hosted-runner variation;
- both finite -> lower bound: `+0.072696 ms`;
- lower bound -> full validation: `+0.545279 ms`;
- raw -> local full: `2.011x`;
- raw -> public Derived: `2.013x`;
- public Derived and the local full mirror differed by about `0.1%` in this execution.

At smaller working sets in this same execution, adding the second finite check generally increased measured time. The slight 1,048,576-value reversal is therefore treated as environment/code-generation noise rather than evidence that an additional check has negative intrinsic cost.

## Attribution result

The two final controlled executions support the following bounded interpretation:

```text
First finite-value gate (Temperature):
LARGEST STABLE ADDED MEASURED LAYER

Additional finite / range predicates:
SMALLER AND MORE JIT / ENVIRONMENT SENSITIVE

Local full-validation mirror vs public Derived constructor:
CLOSE AT THE LARGE WORKING SET IN BOTH FINAL RUNS

Invariant removal:
NOT AUTHORIZED
```

The strongest repeatable result is not an exact cost for an individual `IsFinite` instruction. It is that introducing the first finite-value guard creates the largest measured separation from raw two-double output in both final controlled environments. Later validation layers are smaller and sufficiently sensitive to JIT/code generation that they should not be assigned precise instruction-level costs from this benchmark.

The close large-working-set agreement between the full local mirror and the real public constructor also indicates that the remaining public-constructor overhead is broadly represented by the explicit validating code path; there is no evidence here for a large hidden type-layout penalty beyond that path.

## Invalid and superseded predecessor executions

The first PR #60 workflow execution (`32497353292`, job `96818811441`) failed to compile because the initial benchmark-only throw helpers used invalid parameter-name expressions. It produced no performance evidence. The harness was corrected without changing Framework implementation.

A later successful intermediate harness used a different benchmark-local struct type for each validation layer. Cross-run behavior exposed output-type/JIT code-generation confounding, so those measurements were not adopted as the final attribution basis. The methodology was tightened to the common `LayeredOutput` type before the final controlled executions above.

Historical workflow results are not silently rewritten; these predecessor states explain the refinement path but do not support the final layer attribution.

## Limitations

- GitHub-hosted runner absolute timings are environment-specific.
- The benchmark evaluates valid-value output construction only; invalid-input exception cost is not timed.
- Static factory and constructor code shape may be inlined or optimized differently by different JIT/runtime/hardware combinations.
- Pairwise layer differences are code-path observations, not exact instruction-level predicate costs.
- No multi-thread CPU, GPU, Unity, engine, mobile, or production-hardware claim is made.
- No physical accuracy or Framework Conformance conclusion is implied.

## Engineering disposition

PR #59 already preserves all Derived State invariants while improving JIT-friendliness. This attribution does not provide evidence strong enough to justify a more complex trusted/unchecked construction path.

The next high-value Performance Evaluation should therefore move away from further single-thread constructor micro-optimization and evaluate **multi-thread CPU batch-recovery scaling** using the current semantics-preserving implementation as the baseline.

## Specification impact

- Framework Specification change: None
- Reference Formulation change: None
- Framework Freeze reopen: No
- Derived State invariant relaxation: None
- New Framework component / owner: None
