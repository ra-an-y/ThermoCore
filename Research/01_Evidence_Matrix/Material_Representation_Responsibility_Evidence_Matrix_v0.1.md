# Material Representation Responsibility Evidence Matrix

Version: 0.1  
Status: Bounded Direct-Antecedent Survey — Non-Normative Research Record  
Research Question: RQ-MRR-001 — Material Representation Responsibility Boundary

---

## 1. Objective

This Evidence Matrix performs the first bounded direct-antecedent survey for **RQ-MRR-001 — Material Representation Responsibility Boundary**.

The research question asks whether ThermoCore requires an independent architectural criterion for distinguishing legitimate Material Representation from responsibilities that belong instead to Thermodynamic Computation, authoritative Thermodynamic State, reusable Material Definition, an Extension Module, or an external/application responsibility.

The purpose of this document is not to defend the current normative `Material_Representation.md` design. That design already exists and is not admissible as novelty evidence for this survey.

The survey instead tests the strongest null explanation:

> Established simulation, post-processing, visualization, constitutive, and coupling systems already distinguish solved/evolving state from derived or downstream data. A value may be generated, cached, persisted, rendered, exported, or transformed without thereby becoming authoritative simulation state. Conversely, if an apparently downstream value is used to determine governing evolution, closure, constitutive response, or feedback, then its role must be treated as part of the governing/coupling/input path rather than as mere presentation. ThermoCore may therefore be formalizing a useful conformance boundary rather than establishing a new research contribution.

The result of this matrix is intentionally allowed to be negative.

---

## 2. Relationship to the Original RQ-001 Gap

The original `Research_Gap_Analysis_v0.1.md` identified four unresolved architectural boundaries:

1. ownership of evolving simulation state;
2. responsibility of Material Representation;
3. runtime material abstraction;
4. extension coupling boundary.

Three of those lines have since been independently processed:

- **RQ-ISO-001** established a bounded state-authority / non-promotion contribution;
- **RQ-EFM-001** established a bounded formulation-relative extension-admissibility contribution;
- **RQ-RMA-001** was closed and reclassified as a Configuration-Derivative Identity engineering/conformance property.

RQ-MRR-001 is therefore the final unresolved line inherited from the original RQ-001 architectural gap analysis.

This survey shall not preserve that status merely for symmetry. If the responsibility distinction is already explained by prior art and completed ThermoCore boundaries, RQ-MRR-001 shall be narrowed or closed.

---

## 3. ThermoCore Baseline Being Tested

The current normative baseline states, in summary, that Material Representation:

- interprets Thermodynamic State and applicable Material Definition;
- produces Representation for downstream consumption;
- owns Representation;
- does not own or modify Thermodynamic State;
- does not perform State Evolution;
- does not replace Material Definition;
- does not make a Representation Consumer part of the Framework Core;
- may maintain Persistent Representation without converting it into Runtime State.

This survey treats those statements only as the **candidate boundary to be externally stress-tested**.

No normative conclusion follows from the specification merely because it is internally clear.

---

## 4. Candidate Dimensions Under Survey

RQ-MRR-001 defined eight candidate semantic dimensions:

1. interpretation versus State Evolution;
2. source dependence versus independent physical authority;
3. Representation ownership versus source-information ownership;
4. state-dependent interpretation versus closure-critical computation;
5. downstream consumption versus feedback/control into thermodynamic evolution;
6. Persistent Representation versus Runtime State;
7. application-facing/consumer-specific outputs versus reusable Material Definition;
8. extension-specific Representation versus extension state or governing responsibility.

All eight dimensions remain research questions in this document until evaluated against evidence.

---

## 5. Evidence Method

The evidence search is bounded to framework, standards, and author/project documentation that directly exposes one or more of the following distinctions:

- solved/evolving variables versus auxiliary/derived variables;
- runtime/post-processing outputs versus governing unknowns;
- output data versus input/feedback roles;
- material properties versus constitutive state evolution;
- data-pipeline transformation versus source-data authority;
- visualization/output persistence versus physical-state persistence.

Priority is given to official or project-maintained documentation.

The survey does **not** treat terminology as proof. In particular:

- `output` does not automatically mean Representation;
- `auxiliary` does not automatically mean non-authoritative;
- `postprocessor` does not automatically mean downstream-only;
- `state` does not automatically mean ThermoCore Thermodynamic State;
- `persistent` does not automatically mean Runtime State;
- `material` does not automatically mean Material Representation.

The actual responsibility and dependency role is the relevant evidence.

---

# 6. Direct Antecedent Evidence

## 6.1 MOOSE — Auxiliary Variables, Postprocessors, and Feedback

### Evidence

MOOSE documents `AuxVariable` as an auxiliary field variable used to compute or store intermediate quantities that are **not the main variables being solved for** by the equation system.

Source:

- https://mooseframework.inl.gov/source/variables/AuxVariable.html

MOOSE `AuxKernel` documentation states that auxiliary values are often used for visualization and output, but may also be **coupled back into other calculations, including Kernels**, or supplied to Postprocessors.

Source:

- https://mooseframework.inl.gov/source/auxkernels/AuxKernel.html
- https://mooseframework.inl.gov/moose/syntax/AuxKernels/

MOOSE Postprocessors compute scalar values from the solution, integrals, averages, extrema, or other quantities. Their results may be used purely for output, but MOOSE also permits other objects to retrieve those values. The documentation includes an example in which a Postprocessor value is used by a Neumann boundary condition and therefore contributes to the residual calculation.

Source:

- https://mooseframework.inl.gov/syntax/Postprocessors/
- https://mooseframework.inl.gov/source/interfaces/PostprocessorInterface.html

MOOSE Controls may modify controllable simulation parameters at runtime. A postprocessor can therefore participate in a feedback/control path when explicitly connected into that responsibility.

Source:

- https://mooseframework.inl.gov/syntax/Controls/index.html

### Finding

MOOSE provides a strong direct antecedent for the distinction between:

```text
solution / governing variables
        ↓
auxiliary or postprocessed value
        ↓
output-only use
```

and:

```text
solution / governing variables
        ↓
auxiliary or postprocessed value
        ↓
explicit coupling back into Kernel / BC / Control
        ↓
governing calculation
```

The same **kind of computed value** may be downstream-only in one use and governingly relevant in another use.

Therefore:

> A producer category or data label alone is not sufficient to establish non-authoritative Representation semantics.

This directly pressures any RQ-MRR claim based merely on “derived output versus solver data.”

### RQ-MRR relevance

- interpretation versus State Evolution: **DIRECT ANTECEDENT**;
- downstream consumption versus feedback/control: **DIRECT ANTECEDENT**;
- source dependence versus authority: **STRONG PARTIAL ANTECEDENT**;
- persistent/cached output versus state: **PARTIAL ANTECEDENT**;
- exact ThermoCore ownership vocabulary: **NOT ESTABLISHED AS DIRECT ANTECEDENT**.

---

## 6.2 OpenFOAM — Function Objects and Derived Runtime/Post-Processing Data

### Evidence

OpenFOAM function objects generate user-requested data during runtime and post-processing. Output may include log data, text, images, or fields. Function-object outputs can be retained in the mesh database and chained into other function objects and applications.

Source:

- https://doc.openfoam.com/2212/tools/post-processing/function-objects/

OpenFOAM `derivedFields` explicitly computes derived fields such as `rhoU` and total pressure from input fields.

Source:

- https://doc.openfoam.com/2312/tools/post-processing/function-objects/field/derivedFields/

OpenFOAM runtime post-processing generates images by assembling and compositing scene objects, including outputs produced by other function objects.

Source:

- https://doc.openfoam.com/2606/tools/post-processing/function-objects/graphics/runtime-postprocessing/

The standalone `postProcess` utility may execute function objects on already-produced time-directory data after solver execution.

Source:

- https://doc.openfoam.com/2606/tools/post-processing/utilities/postProcess/

### Finding

OpenFOAM establishes mature separation among:

- governing simulation fields;
- derived fields;
- chained post-processing data;
- runtime visualization products;
- post-simulation analysis.

The framework can produce and persist derived fields without treating every derived field as a governing unknown.

It also allows derived output to be chained into additional downstream computations without implying that those downstream computations own the original governing fields.

### RQ-MRR relevance

- downstream interpretation / derived output: **DIRECT ANTECEDENT**;
- consumer-side transformation: **DIRECT ANTECEDENT**;
- persistence of derived output: **DIRECT ENGINEERING PRECEDENT**;
- visualization as a distinct downstream responsibility: **DIRECT ANTECEDENT**;
- exact unique ownership semantics used by ThermoCore: **NOT DIRECTLY ESTABLISHED**.

---

## 6.3 ParaView — Pipeline Transformation without Source-Role Identity

### Evidence

ParaView describes filters as pipeline modules with inputs and outputs. Filters accept input data, transform it, and produce resulting output data. Filters may have multiple input and output ports with different roles.

Source:

- https://docs.paraview.org/_/downloads/en/latest/pdf/

### Finding

The filter/pipeline model provides a direct general precedent for downstream transformation:

```text
source data
    ↓
filter / interpretation / transformation
    ↓
derived output
```

A downstream pipeline product has its own output identity without becoming the original source dataset.

This is not thermodynamic evidence by itself, and it does not establish ThermoCore’s exact Representation ownership model. It does, however, strongly antecede the generic claim that a derived downstream representation may remain semantically distinct from its source information.

### RQ-MRR relevance

- source dependence versus source identity: **STRONG PARTIAL ANTECEDENT**;
- downstream consumer transformation: **DIRECT ANTECEDENT**;
- independent output object without source replacement: **DIRECT ENGINEERING PRECEDENT**;
- thermodynamic authority boundary: **NOT ADDRESSED**.

---

## 6.4 FMI 3.0.2 — Output Causality Does Not Imply Non-State or Non-Feedback

### Evidence

FMI 3.0.2 defines:

- input variables as values defined outside the model;
- output variables as values computed in the FMU and designed for use outside the FMU;
- local variables as internal variables not intended for FMU connections;
- continuous and discrete states as distinct state categories.

The FMI specification also permits an output of one FMU to be forwarded into an input of another FMU.

Variable dependency information is explicitly used to detect and classify algebraic loops across connected inputs and outputs.

Critically, FMI permits a continuous-time state to also have output causality. Therefore “output” and “state” are not mutually exclusive classifications in FMI.

Source:

- https://fmi-standard.org/docs/3.0.2/

### Finding

FMI establishes two important negative controls for RQ-MRR:

1. **Output-ness alone does not imply Representation-like non-authority.**
2. **A value designed for outside use may later participate in a feedback/coupling loop when connected to another input.**

Therefore a ThermoCore Representation boundary cannot be justified by saying merely that a quantity is “an output” or “consumer-facing.”

The meaningful distinction must concern semantic responsibility in the claimed architecture.

### RQ-MRR relevance

- downstream output versus feedback: **DIRECT ANTECEDENT**;
- output versus state classification: **DIRECT COUNTER-ANTECEDENT TO NAME-BASED CLASSIFICATION**;
- feedback dependency loops: **DIRECT ANTECEDENT**;
- ThermoCore-specific Representation ownership: **NOT ESTABLISHED**.

---

## 6.5 MFront / MGIS — Material-Related Quantities that Participate in Governing Evolution

### Evidence

MFront documentation distinguishes simple material properties from material behaviours. Material properties may be functions of the current thermodynamic state, while behaviours describe evolution and often require integration of state-variable evolution equations.

Source:

- https://thelfer.github.io/tfel/web/material-properties.html
- https://thelfer.github.io/MFrontGallery/web/index.html

In an MFront/FEniCS coupling, the solver handles gradients, force assembly, and stiffness assembly, while the MFront behaviour computes thermodynamic forces, updates internal state variables, and supplies tangent information at integration points.

Source:

- https://thelfer.github.io/mgis/web/FEniCSBindings.html

MFront also explicitly distinguishes material properties, persistent variables, state variables, auxiliary state variables, external state variables, and local variables.

Source:

- https://thelfer.github.io/tfel/web/mfront-python.html

MFront mechanical behaviour documentation shows constitutive response and internal-variable evolution participating directly in the iterative governing solution.

Source:

- https://thelfer.github.io/tfel/web/behaviours.html

### Finding

This evidence provides the strongest counter-boundary in the survey:

> A quantity does not become “Representation” merely because it is material-related, derived from current state, or produced by a material-facing subsystem.

Stress, thermodynamic force, consistent tangent, and internal state may be material-facing quantities, yet they participate directly in governing constitutive response and state evolution.

For ThermoCore, a quantity that is required for formulation closure, state update, or governing response must therefore be routed to Thermodynamic Computation, RQ-EFM, RQ-ISO, or an admitted Extension responsibility rather than hidden in Material Representation.

### RQ-MRR relevance

- state-dependent interpretation versus closure-critical computation: **DIRECT ANTECEDENT**;
- material-facing output versus governing response: **DIRECT ANTECEDENT**;
- internal history state versus derived display data: **DIRECT ANTECEDENT**;
- exact ThermoCore architectural allocation: **NOT DIRECTLY ESTABLISHED**.

---

## 6.6 DOLFINx — Supporting Derived-Function Evidence

### Evidence

DOLFINx exposes interpolation operations that evaluate an expression or function and place the result into another finite-element Function.

Source:

- https://docs.fenicsproject.org/dolfinx/main/cpp/doxygen/d8/dbf/namespacedolfinx_1_1fem.html
- https://docs.fenicsproject.org/dolfinx/main/cpp/doxygen/d7/d76/classdolfinx_1_1fem_1_1Function.html

### Finding

This provides supporting evidence that simulation frameworks routinely produce additional derived field representations from existing functions or expressions.

The documentation does not itself establish a thermodynamic authority boundary, so this source is supporting rather than decisive.

### RQ-MRR relevance

- derived field construction: **SUPPORTING ANTECEDENT**;
- ownership / authority semantics: **NOT ESTABLISHED**.

---

# 7. Cross-Evidence Findings

## 7.1 Finding MRR-F01 — Derived / Auxiliary / Postprocessed Output Is Established Prior Art

Simulation frameworks commonly compute quantities that are not the main governing unknowns and expose them for:

- visualization;
- post-processing;
- logging;
- downstream analysis;
- interpolation;
- field derivation;
- application consumption.

This is established practice.

ThermoCore cannot claim novelty for the mere existence of a separate Material Representation or downstream derived-output stage.

**Status:** `ESTABLISHED PRIOR ART`

---

## 7.2 Finding MRR-F02 — Output Labels Do Not Determine Physical Authority

MOOSE auxiliary and postprocessor values may be coupled into Kernels, boundary conditions, or Controls.

FMI output variables may be connected to other inputs, and a state variable may itself be exposed as an output.

Therefore:

```text
name / API category / output flag
        ≠
semantic proof of non-authority
```

**Status:** `DIRECT COUNTER-ANTECEDENT TO NAME-BASED CLASSIFICATION`

---

## 7.3 Finding MRR-F03 — Downstream-to-Governing Re-entry Is an Established Coupling Pattern

A value may originate as a derived or output quantity and later participate in governing computation through an explicit coupling path.

The relevant architecture therefore has at least two roles:

```text
upstream solution
      ↓
derived/output value
```

and, if feedback is introduced:

```text
derived/output value
      ↓
explicit input / BC / control / coupling role
      ↓
governing calculation
```

This strongly antecedents the ThermoCore idea that Representation feedback cannot bypass normal input/coupling semantics.

**Status:** `ESTABLISHED PRIOR ART / ENGINEERING PRECEDENT`

---

## 7.4 Finding MRR-F04 — Material-Facing Computation Is Not Necessarily Representation

MFront/MGIS demonstrates material-facing responsibilities that compute thermodynamic forces, update internal variables, and supply tangent operators required by the governing solve.

Therefore the boundary cannot be:

```text
material-related
    =>
Representation
```

The deciding issue is whether the responsibility is downstream interpretation or governing evolution/closure.

**Status:** `DIRECT THERMOMECHANICAL / CONSTITUTIVE ANTECEDENT`

---

## 7.5 Finding MRR-F05 — Persistence Does Not Decide State versus Representation

The reviewed systems use persistent data for many reasons:

- state history;
- auxiliary variables;
- post-processing continuity;
- output files;
- derived-field chaining;
- visualization products.

Conversely, FMI permits a state to also be an output.

Therefore neither persistence nor output status alone determines semantic category.

**Status:** `STRONGLY SUPPORTED DISTINCTION — NOT NOVELTY`

---

## 7.6 Finding MRR-F06 — Downstream Transformation without Source Replacement Is Mature

OpenFOAM function-object chaining, ParaView filters, and derived-field systems show that downstream data can be transformed into new outputs without replacing the source data’s own semantic role.

This directly pressures any claim that ThermoCore is novel merely because Representation is distinct from Thermodynamic State and Material Definition.

**Status:** `ESTABLISHED ENGINEERING PATTERN`

---

# 8. Evaluation of the Eight Candidate Dimensions

| ID | Candidate dimension | Evidence result | Current disposition |
|---|---|---|---|
| MRR-D1 | Interpretation vs State Evolution | Strongly separated in post-processing/auxiliary systems and constitutive evolution systems | `DIRECTLY ANTECEDED` |
| MRR-D2 | Source dependence vs independent physical authority | Derived outputs depend on source data; outputs may also be fed back; role matters more than dependency alone | `STRONG PARTIAL ANTECEDENT` |
| MRR-D3 | Representation ownership vs source ownership | Pipeline/output separation is established; exact unique ThermoCore ownership vocabulary is not directly mirrored | `UNDER SURVEY — POSSIBLE CONFORMANCE FORMALIZATION` |
| MRR-D4 | State-dependent interpretation vs closure-critical computation | MFront/MGIS directly distinguishes property evaluation from behaviour/state integration | `DIRECTLY ANTECEDED` |
| MRR-D5 | Downstream consumption vs feedback/control | MOOSE and FMI directly show output-to-governing re-entry | `DIRECTLY ANTECEDED` |
| MRR-D6 | Persistent Representation vs Runtime State | Persistence/output status does not determine state semantics | `STRONG PARTIAL ANTECEDENT` |
| MRR-D7 | Consumer-specific output vs Material Definition | Derived/post-processing/output systems strongly separate reusable model input from consumer output | `ESTABLISHED PATTERN` |
| MRR-D8 | Extension-specific Representation vs extension governing responsibility | Meaningful state/closure cases route to existing ThermoCore RQ-ISO/RQ-EFM boundaries; output-only cases resemble established derived-output patterns | `NO INDEPENDENT CATEGORY YET` |

### Interim result

Of the eight candidate dimensions:

- six are directly or strongly anteceded;
- one exact ownership phrasing remains ThermoCore-specific in wording but has not shown independent research decision power;
- one extension-specific distinction currently collapses into existing RQ-ISO / RQ-EFM / output-role classification.

No new independent architecture predicate has yet been demonstrated.

---

# 9. Preliminary Matched-Scenario Classification

These scenarios are not yet a pre-registered consequence test. They are bounded classification probes used to determine whether a focused v0.2 stress test is justified.

## MRR-S0 — Temperature-to-colour mapping

### Setup

Current Thermodynamic State provides temperature. Material Definition provides any applicable mapping configuration. A downstream component maps temperature to colour for visualization.

### Classification

```text
Thermodynamic State
        ↓ read
Material Representation
        ↓
colour Representation
        ↓
consumer
```

No State Evolution or closure authority is introduced.

### Preliminary result

`REPRESENTATION — CURRENT RULES SUFFICIENT`

No independent RQ-MRR predicate required.

---

## MRR-S1 — Derived phase label or phase fraction for application display

### Setup

A current state and material model are sufficient to derive a phase fraction or user-facing label. The result is consumed downstream and is not required as a persistent governing coordinate.

### Preliminary result

`DERIVED REPRESENTATION / DERIVED OUTPUT — CURRENT RULES SUFFICIENT`

If the quantity becomes required to determine the next thermodynamic state under the selected formulation, the case changes and routes to RQ-EFM.

---

## MRR-S2 — Expensive Representation cached across steps

### Setup

An expensive downstream interpretation is cached and reused until its source information changes.

### Preliminary result

Caching alone does not create state authority.

`PERSISTENT/CACHED REPRESENTATION — CURRENT RULES SUFFICIENT`

This parallels the broader RQ-RMA finding that storage lifecycle is not sufficient to change semantic category.

---

## MRR-S3 — Persistent consumer-continuity representation

### Setup

A consumer requires a representation to remain available across framework operation for continuity, streaming, UI, or temporal rendering reasons.

### Preliminary result

Persistence alone does not make the artifact Thermodynamic State.

`PERSISTENT REPRESENTATION — CURRENT RULES SUFFICIENT`

---

## MRR-S4 — “Representation” required to determine next Thermodynamic State

### Setup

A quantity is initially described as a representation output, but two otherwise identical current thermodynamic states produce different next thermodynamic states depending on the retained value of that quantity.

### Preliminary result

The quantity carries governing closure/evolution significance.

`NOT MERE REPRESENTATION`

Route:

- RQ-EFM for formulation/state-space sufficiency;
- RQ-ISO for state authority/non-promotion where applicable.

No new RQ-MRR rule required so far.

---

## MRR-S5 — Representation used in feedback

### Setup

A consumer transforms a displayed or derived representation into a control signal, energy source, boundary value, or property update that affects later thermodynamic evolution.

### Preliminary result

The original output may remain a representation of its source, but the **feedback path has a new semantic role**.

It must re-enter through an explicit input/coupling/source/control responsibility.

If energy-bearing, conservative exchange/accounting semantics also apply.

`REPRESENTATION + SEPARATE RE-ENTRY ROLE`

This pattern is directly anteceded by MOOSE and FMI.

---

## MRR-S6 — Extension-specific visualization from extension-local state

### Setup

An ordinary extension owns local state and derives a visualization or application-facing output from that state.

### Preliminary result

The extension may own its local state while extension-specific representation remains a downstream output responsibility.

No promotion of extension state into Thermodynamic State is implied.

`EXTENSION-LOCAL STATE + DOWNSTREAM REPRESENTATION`

RQ-ISO remains the authority boundary.

---

## MRR-S7 — Constitutive/material response affecting governing closure

### Setup

A material-facing subsystem computes stress, force, tangent, reaction progress, or another quantity required by the governing solve or thermodynamic update.

### Preliminary result

This is not Representation merely because the result is material-facing or state-dependent.

`GOVERNING / CONSTITUTIVE RESPONSIBILITY`

Route to applicable computation/extension/formulation rules, especially RQ-EFM.

MFront/MGIS is a direct antecedent for this distinction.

---

## MRR-S8 — Consumer stores/transforms/re-renders Representation

### Setup

A downstream consumer caches, serializes, re-colours, resamples, or otherwise transforms Representation.

### Preliminary result

Consumer-side transformation does not transfer Framework ownership of source Thermodynamic State or Material Definition.

`CONSUMER-SIDE TRANSFORMATION — NO CORE OWNERSHIP TRANSFER`

This is strongly anteceded by pipeline systems such as ParaView and OpenFOAM post-processing chains.

---

# 10. Routing Against Existing ThermoCore Research Results

The current evidence suggests that RQ-MRR meaningful failure cases already route through established boundaries.

## 10.1 RQ-ISO-001

Use RQ-ISO when a representation-side or extension-side value attempts to acquire:

- authoritative Thermodynamic State membership;
- Thermodynamic State write authority;
- state ownership through access or persistence;
- promotion into mandatory Core State without justified Core change.

Representation naming does not override RQ-ISO.

---

## 10.2 RQ-EFM-001

Use RQ-EFM when a supposedly downstream value is actually required for:

- thermodynamic closure;
- update sufficiency;
- governing response;
- a formulation-specific state-space coordinate;
- Core/formulation revision.

Representation naming does not permit closure-critical information to remain outside the governing formulation when the formulation is incomplete without it.

---

## 10.3 Conservative Exchange Accounting Property

Use the CEX-derived engineering property when a feedback/re-entry path carries energy-bearing contributions and the accounting role must be distinguished among:

- external source/sink;
- internal redistribution;
- cross-domain conversion;
- boundary exchange.

Representation status does not exempt a feedback value from physical accounting semantics.

---

## 10.4 RQ-RMA-001

RQ-RMA already established that compilation, caching, persistence, layout, or backend specialization alone do not change semantic information category.

That result applies analogously here:

```text
persistent / cached / GPU-resident / serialized
        ≠
authoritative Runtime State by itself
```

---

# 11. Surviving ThermoCore-Specific Candidate

After the direct-antecedent pass, the strongest remaining ThermoCore-specific formulation is:

## Downstream Representation Non-Authority Property

> A framework artifact may be treated as Representation while its framework role is downstream interpretation or consumption of authoritative source information and it does not acquire independent authority over thermodynamic closure, State Evolution, Thermodynamic State ownership, or Material Definition. Persistence, caching, transport, storage format, rendering, or consumer transformation do not by themselves change this classification. If a derived/representation value is used to influence later thermodynamic evolution, that influence must re-enter through an explicitly classified input, coupling, extension, source, or governing role rather than granting the Representation itself hidden thermodynamic authority.

This candidate is useful because it unifies several ThermoCore constraints into a concise conformance test.

However, the current evidence does **not** establish it as an independent research contribution.

Why:

1. state/output/post-processing separation is mature prior art;
2. output feedback/re-entry is mature prior art;
3. constitutive versus downstream-output distinction is mature prior art;
4. persistence not determining state identity is already supported by both external evidence and RQ-RMA;
5. state authority is already addressed by RQ-ISO;
6. closure/governing responsibility is already addressed by RQ-EFM;
7. energy-bearing feedback accounting is already addressed by the CEX-derived property.

The surviving candidate currently appears to be a **composition of established distinctions and existing ThermoCore rules**.

---

# 12. Falsification Assessment

## F-MRR-1 — Equivalent state/output/post-processing separation exists

**Result:** `TRIGGERED`

MOOSE, OpenFOAM, ParaView, FMI, and DOLFINx provide strong antecedents for separating solved/source information from derived/output/consumer data.

This falsifies any broad claim that ThermoCore is first to separate simulation state from representation/output.

---

## F-MRR-2 — Material-related governing response is already distinguished from presentation

**Result:** `TRIGGERED`

MFront/MGIS directly distinguishes material properties, internal state variables, thermodynamic forces, tangent operators, and governing constitutive integration.

This falsifies any broad claim that “material-facing interpretation versus governing response” is absent from prior systems.

---

## F-MRR-3 — Feedback from outputs is an established system concept

**Result:** `TRIGGERED`

MOOSE allows auxiliary/postprocessor values to participate in Kernels, BCs, and Controls. FMI permits output-to-input connections and explicitly models algebraic dependencies and loops.

Therefore an RQ-MRR contribution cannot be based merely on recognizing that output may later affect a solver.

---

## F-MRR-4 — Existing ThermoCore boundaries already classify failure cases

**Result:** `STRONGLY SUPPORTED — FINAL STRESS TEST STILL REQUIRED`

Preliminary matched cases route to:

- RQ-ISO for state authority;
- RQ-EFM for closure/governing significance;
- CEX property for energy accounting;
- RQ-RMA for storage/cache/backend non-reclassification;
- existing Material Representation / Conformance rules for downstream non-authority.

No independent RQ-MRR architecture category has yet survived.

---

# 13. Evidence Status Summary

| Claim / distinction | Status |
|---|---|
| Derived/output/post-processing systems separate from main solved variables | `ESTABLISHED PRIOR ART` |
| Runtime and post-simulation derived fields | `ESTABLISHED PRIOR ART` |
| Visualization pipeline / transformed downstream outputs | `ESTABLISHED PRIOR ART` |
| Auxiliary or output values may feed back into governing calculation | `DIRECT ANTECEDENT ESTABLISHED` |
| Output label does not imply non-state identity | `DIRECT COUNTER-ANTECEDENT ESTABLISHED` |
| Material-facing constitutive response may be governing, not representational | `DIRECT ANTECEDENT ESTABLISHED` |
| Persistence/storage form alone does not determine state authority | `STRONGLY SUPPORTED DISTINCTION` |
| Exact ThermoCore unique Representation ownership wording | `NOT DIRECTLY ESTABLISHED AS A DISTINCT CONTRIBUTION` |
| Downstream Representation Non-Authority Property | `SURVIVING ENGINEERING / CONFORMANCE CANDIDATE` |
| Independent RQ-MRR Research Gap | `NOT ESTABLISHED` |
| Novelty / priority | `NOT ESTABLISHED` |
| Research Gap Analysis readiness | `NO-GO` |
| Framework Specification impact | `NONE` |

---

# 14. Current Disposition

The first direct-antecedent survey places **strong negative pressure** on RQ-MRR-001 as an independent research contribution.

The broad concepts required by the candidate boundary are already strongly represented in prior systems:

- solved state versus auxiliary/derived output;
- post-processing and visualization pipelines;
- output-to-input feedback;
- governing constitutive state versus downstream results;
- persistent derived data versus state identity;
- downstream transformation without source replacement.

The exact ThermoCore phrase “Material Representation owns Representation while source information retains separate ownership” is not directly mirrored in the reviewed evidence as a named universal principle. But wording uniqueness is not sufficient for a research gap.

The current evidence therefore supports only a narrower surviving engineering candidate:

> **Downstream Representation Non-Authority Property**

### Research disposition

- **Independent Research Gap:** `NOT ESTABLISHED`
- **Research contribution claim:** `UNDER STRONG NEGATIVE PRESSURE`
- **Surviving value:** `Downstream Representation Non-Authority Property`
- **Likely classification:** `ENGINEERING / CONFORMANCE PROPERTY`
- **Research Gap Analysis:** `NO-GO`
- **Novelty / priority:** `NOT ESTABLISHED`
- **Framework Specification impact:** `NONE`

---

# 15. Required Next Step

Do **not** open a Research Gap Analysis from v0.1.

The next justified step is a focused **v0.2 matched-scenario stress test**.

That test shall determine whether the Downstream Representation Non-Authority Property adds any decision power beyond existing ThermoCore rules.

At minimum, v0.2 should test:

1. pure temperature-to-colour interpretation;
2. derived phase/application label;
3. cached expensive representation;
4. persistent consumer-continuity representation;
5. a hidden closure-critical quantity mislabelled as Representation;
6. output feedback re-entering as source/control/property update;
7. extension-local state producing extension-specific representation;
8. material/constitutive response required by governing closure;
9. consumer-side persistence/transformation without Core ownership transfer.

For each scenario, compare:

```text
existing ThermoCore rules
        versus
hypothetical independent RQ-MRR rule
```

If the independent rule changes no classification, RQ-MRR-001 shall be closed and reclassified rather than narrowed indefinitely.

---

# 16. Guardrails

This document does not establish that:

- ThermoCore is the first framework to separate state from representation;
- Material Representation is globally novel;
- downstream-output architecture is superior;
- every postprocessed variable is Representation;
- every persistent output is non-state;
- every material-facing quantity is Representation;
- every displayed quantity is non-authoritative;
- an output may feed back into Thermodynamic Computation without an explicit coupling/input role;
- the reviewed external systems are complete ThermoCore equivalents;
- current v1.0.0 implements every scenario considered here.

No Framework Specification, production implementation, Verification, Validation, or Performance change is authorized by this document.

---

# 17. Review Checklist

- [x] MOOSE auxiliary/postprocessor distinction reviewed.
- [x] MOOSE feedback into Kernel/BC/Control reviewed.
- [x] OpenFOAM runtime/post-processing derived-field precedent reviewed.
- [x] OpenFOAM visualization/post-processing chain reviewed.
- [x] ParaView pipeline transformation precedent reviewed.
- [x] FMI output/input/state-causality distinction reviewed.
- [x] FMI feedback/algebraic-loop precedent reviewed.
- [x] MFront/MGIS constitutive state/governing response reviewed.
- [x] DOLFINx derived-function support reviewed.
- [x] All eight candidate dimensions evaluated.
- [x] Persistence/storage separated from authority.
- [x] Output naming separated from semantic role.
- [x] RQ-ISO / RQ-EFM / CEX / RQ-RMA routing explicit.
- [x] Negative outcome retained as valid.
- [x] Research Gap Analysis remains NO-GO.
- [x] No Framework or implementation changes introduced.

---

## Document Status

This file is a non-normative bounded research artifact.

It records the first direct-antecedent survey for RQ-MRR-001 and does not modify the authoritative Framework Specification.

The current recommended next step is focused v0.2 matched-scenario stress testing. If no independent decision rule survives, RQ-MRR-001 should be closed and reclassified as an engineering/conformance result.
