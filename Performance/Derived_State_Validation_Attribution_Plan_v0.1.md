# Derived State Validation Attribution Plan v0.1

## Status

Performance Attribution Plan — no implementation promotion decision yet.

## Purpose

The first CPU batch-attribution study identified the validated `DerivedThermodynamicState` construction path as the dominant measured separation relative to an otherwise similar raw two-double struct. PR #59 then reduced part of that cost by making the still-fully-validating public constructor more JIT-friendly.

A measurable residual gap remains. This study narrows that residual by decomposing the valid-value constructor path into progressively stronger benchmark-local validation layers.

This is a Performance Evaluation artifact. It is not Verification, physical Validation, Framework Conformance, or a Framework Specification.

## Evaluated baseline

Repository baseline:

`9e358fd8c75529ba6528d30d68fedf2ea18a525a`

This is `main` after PR #59 (`Implementation: make Derived State validation JIT-friendly`).

No Framework implementation change is proposed by this plan.

## Question

For valid recovered values, how much of the remaining output-construction cost is associated with each progressively added validation layer, and is the actual public `DerivedThermodynamicState` constructor close to a benchmark-local mirror with the same validation semantics?

## Compared output paths

All timed paths consume the same precomputed valid Temperature and liquid-fraction arrays. Thermodynamic recovery arithmetic is excluded from the timed interval.

The first five paths all write the **same benchmark-local `LayeredOutput` two-double struct**. Only the validation applied by its aggressively-inlined static factory differs. This controls benchmark-local output type and layout while validation predicates are added progressively.

1. `raw_output`
   - assignment only;
   - no validation.

2. `temperature_finite_output`
   - finite Temperature check;
   - no liquid-fraction validation.

3. `both_finite_output`
   - finite Temperature check;
   - finite liquid-fraction check.

4. `finite_lower_bound_output`
   - both finite checks;
   - liquid fraction lower-bound check (`>= 0`).

5. `local_full_validation_output`
   - both finite checks;
   - lower- and upper-bound checks (`[0,1]`);
   - benchmark-local cold throw helpers and JIT hints mirroring the current public constructor shape.

6. `public_derived_output`
   - actual repository `DerivedThermodynamicState` constructor;
   - necessarily uses the real Derived State type rather than the benchmark-local control type.

The benchmark-local partial validators are deliberately incomplete outside the valid-value timing domain. They are measurement devices only and are **not implementation candidates**. No result from this study authorizes weakening the public Derived State invariant.

## Semantic and integrity gates

Before timing is accepted:

- source values are generated through the current reference formulation and are finite with liquid fraction in `[0,1]`;
- all six paths must reproduce exactly the same Temperature and liquid-fraction values for the deterministic 1,048,576-value gate;
- the actual public constructor and the local full-validation mirror must both reject non-finite Temperature, non-finite liquid fraction, liquid fraction below zero, and liquid fraction above one;
- checksums are computed outside the timed interval;
- arrays and delegates are allocated before timed samples;
- no Framework or Material source is modified by the attribution harness.

## Timing procedure

Working sets:

- 1,024 values
- 16,384 values
- 262,144 values
- 1,048,576 values

For each scenario:

- 3 warmup samples;
- 7 timed samples;
- target 8,388,608 value constructions per timed sample;
- median, minimum, maximum, nanoseconds/value, throughput, managed allocation, and checksum recorded.

The primary comparison is same-run relative cost. Absolute GitHub-hosted-runner timing remains environment-specific.

## Interpretation boundaries

The study may support statements such as:

- a particular added validation layer produces a measurable code-path increment in the tested environment;
- the full local validation mirror is or is not close to the actual public constructor;
- the residual gap is or is not explained by the explicit layered validation path alone.

Even with the common benchmark-local output type, pairwise differences remain JIT/code-generation observations rather than exact instruction-level costs of individual predicates.

The study shall not claim:

- a universal hardware-independent speedup factor;
- that any semantic invariant should be removed;
- that a benchmark-local partial validator is safe implementation machinery;
- GPU, Unity, engine, mobile, or production-hardware performance;
- Framework Conformance or physical Validation.

No post-hoc performance PASS threshold is adopted.

## Evidence disposition

A result record may be added only after execution logs are reviewed. If the decomposition is unstable across hosted runners, the result shall say so rather than promote a preferred explanation.

## Specification impact

- Framework Specification change: None
- Reference Formulation change: None
- Framework Freeze reopen: No
- Derived State invariant relaxation: None
- New Framework component / owner: None
