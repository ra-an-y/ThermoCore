# External Field Coupling Evidence Matrix v0.2

Status: **UNDER SURVEY — second taxonomy stress test**  
Research Question: **RQ-EFM-001**  
Date: **2026-08-23**  
Tracking: GitHub Issue #88  
Primary dependency: `External_Field_Coupling_Evidence_Matrix_v0.1.md`  
Research-question dependency: `Research/05_Research_Questions/RQ_EFM_001_Definition_v0.1.md`

---

## 1. Objective

This document extends the first RQ-EFM-001 evidence pass and tests three unresolved boundaries:

1. constitutive/material-property update versus hidden governing state;
2. caloric/order/history state versus ordinary extension-local state;
3. semantically separate cross-domain governing coupling versus actual Thermodynamic Core / governing-formulation revision.

The v0.1 survey already established that externally driven material response cannot be classified reliably by mechanism name alone. Joule heating can appear as a thermal source, yet a thermoelectric formulation can co-evolve thermal and electrical governing equations. Partitioned multiphysics also shows that strong governing interaction does not by itself require state merger.

v0.2 therefore asks a narrower question:

> **What semantic facts distinguish a current constitutive dependency, mechanism-local persistent state, separately governed cross-domain state, and a genuine change to the selected thermodynamic state-space or governing formulation?**

This document is non-normative. It does not modify the ThermoCore Framework Specification, production implementation, reference thermodynamic formulation, Validation, Performance, Framework Conformance, or the frozen v1.0.0 release baseline.

No Research Gap, novelty, or research-contribution claim is made here.

---

## 2. v0.1 Evidence Carried Forward

The v0.1 evidence remains part of the RQ-EFM-001 evidence chain and is not superseded or erased by this document.

| ID | Mechanism / architecture | v0.1 result retained in v0.2 |
|---|---|---|
| E-EFM-01 | COMSOL Joule / electromagnetic heating | Externally computed electrical loss may enter the heat equation as a source; temperature-dependent electrical properties can create bidirectional feedback. |
| E-EFM-02 | COMSOL optical / laser heating | Optical loss may be deposited as thermal power while electromagnetic state remains in the electromagnetic model. |
| E-EFM-03 | MOOSE Joule heating | Cross-module electromagnetic/electrical variables can produce a thermal source without becoming the thermal unknown. |
| E-EFM-04 | OpenFOAM heat source | Generic energy-source extensibility is established prior art and does not remove other governing-state requirements. |
| E-EFM-05 | COMSOL thermoelectric effect | Coupled heat and electric-current conservation with altered heat/current fluxes cannot be reduced to source-only coupling. |
| E-EFM-06 | preCICE partitioned multiphysics | Separate solvers can remain separate participants and interact through explicit or implicit data exchange. |
| E-EFM-07 | Magnetocaloric / multicaloric literature | Field-driven entropy/order response cannot be assumed to be a simple heat-source or coefficient update. |
| E-EFM-08 | Electrocaloric thermodynamics | Polarization/free-energy formulations can require explicit order-state treatment; mechanism name alone does not determine architecture. |

v0.1 therefore rejected the original four-class list as a complete single-axis taxonomy and introduced two refinement candidates:

- `C-EFM-4R` — cross-domain governing coupling with semantically separate physical-domain state;
- `C-EFM-5R` — Thermodynamic Core / governing-formulation revision.

v0.2 tests whether the resulting five labels can be used as mutually exclusive classes or whether they are actually different semantic dimensions that can coexist in one mechanism.

---

## 3. Evidence Questions for v0.2

For each new evidence record, the following questions are used where the source exposes enough information:

1. Which variables are governing, algebraically derived, prescribed, or historical?
2. Is the external field prescribed, solved elsewhere, or co-evolved?
3. Does the thermal side receive a source, flux, work term, constitutive dependency, or a changed state-space relation?
4. Does the mechanism require persistent internal/order state?
5. If persistent state exists, is it mechanism-local, separately governed cross-domain state, or part of the selected thermodynamic formulation?
6. Does the same mechanism admit both reduced and stateful formulations?
7. Can the thermal/thermodynamic Core remain semantically complete when the mechanism is absent?
8. Does a software object called `Material`, `Property`, `Coupling`, or `Interface` actually indicate semantic ownership, or only implementation placement?
9. Is physical coupling strength being confused with numerical coupling strategy?
10. What fact would force a `C-EFM-5R` classification rather than `C-EFM-4R`?

Unknown architecture semantics are recorded as unresolved rather than inferred from equations or class names alone.

---

## 4. New Evidence Records

### E-EFM-09 — MOOSE Current Material Properties vs Stateful Material Properties

**Source family:** MOOSE official documentation  
**Mechanism:** general constitutive / material-property evaluation  
**Primary sources:**

- https://mooseframework.inl.gov/releases/moose/2024-03-08/syntax/Materials/
- https://mooseframework.inl.gov/moose/getting_started/examples_and_tutorials/examples/ex09_stateful_materials.html
- https://mooseframework.inl.gov/user_workshop/index.html

**Observed architecture / semantics:**

MOOSE distinguishes two materially different uses of a `MaterialProperty`:

- ordinary material properties are computed on demand and may depend directly on current solution variables;
- when a consumer requests `old` or `older` values, the property becomes stateful and previous timestep values are stored.

The official material-system documentation gives temperature-dependent thermal conductivity as an example of a property coupled directly to a current solution variable. The stateful-material documentation explicitly notes that previous values are retained when history is required.

**Taxonomy pressure:**

This is strong architectural evidence that `property` and `state` are not mutually exclusive implementation labels.

A quantity may begin as a current constitutive mapping:

```text
k(t) = f(T(t), configuration)
```

and require no persistent material history.

A path-dependent model instead requires something like:

```text
x(t+dt) = G(x(t), current inputs)
property(t+dt) = f(current inputs, x(t+dt))
```

where `x` is persistent state regardless of whether the implementation stores it inside a Material object.

**Preliminary result:**

`C-EFM-2` and `C-EFM-3` are **not mutually exclusive**.

A mechanism may simultaneously contain:

- a constitutive/property mapping (`C-EFM-2`), and
- persistent mechanism-local history used by that mapping (`C-EFM-3`).

**Provisional semantic discriminator:**

> A property update is `C-EFM-2-only` only when the property is recoverable from current declared inputs and configuration without requiring an independently evolved or previously stored quantity. If previous history or an evolution law is required, persistent-state semantics are present in addition to the property mapping.

---

### E-EFM-10 — COMSOL Pyroelectric / Electrocaloric Reduced Coupling

**Source family:** COMSOL official documentation  
**Mechanism:** pyroelectric / electrocaloric coupling  
**Primary source:**

- https://doc.comsol.com/6.4/doc/com.comsol.help.mems/mems_ug_sme.08.3.html

**Observed architecture / physics:**

The COMSOL Pyroelectricity coupling offers separate selections for:

- direct pyroelectric effect;
- electrocaloric effect / inverse pyroelectric effect;
- fully coupled direct and inverse effects.

The coupling is parameterized using a total pyroelectric coefficient and can introduce a heat source associated with time variation of polarization without requiring the user-facing coupling feature to expose a separate polarization-evolution PDE as its own dependent-variable interface.

The public page therefore demonstrates a **reduced constitutive coupling representation** of a physical effect that other literature can formulate with explicit polarization dynamics.

**Taxonomy pressure:**

The same physical label `electrocaloric` does not imply one unique state architecture.

A reduced model can use coefficient-based constitutive coupling and source exchange, while a more detailed non-equilibrium ferroelectric model may solve polarization dynamics explicitly.

**Preliminary classification:**

`C-EFM-2 + thermal source/feedback form`, with explicit polarization-state ownership not exposed by this reduced coupling feature.

**Important limitation:**

This record does not prove that polarization is physically unnecessary. It establishes only that one accepted modeling interface can eliminate explicit polarization evolution from the selected reduced formulation.

---

### E-EFM-11 — Dynamic Electrocaloric Landau–Khalatnikov Formulation

**Source family:** electrocaloric thermodynamics / ferroelectric modeling literature  
**Mechanism:** dynamic electrocaloric response  
**Primary sources:**

- https://www.sciencedirect.com/science/article/pii/B9780128216477000025
- https://doi.org/10.1002/047134608X.W8244
- https://www.cpsjournals.cn/en/article/doi/10.1088/1674-1056/adc36c

**Observed physics:**

Dynamic ferroelectric/electrocaloric models may use polarization `P` as an order parameter governed by a Landau–Khalatnikov evolution equation derived from a Landau/Ginzburg/Devonshire free-energy model.

The reviewed thermodynamics literature explicitly treats time-varying polarization dynamics and thermal evolution as coupled: polarization evolves through its own kinetic relation while a heat equation is required for the temperature distribution.

**Taxonomy pressure:**

This directly falsifies any rule that `electrocaloric effect = material-property update`.

Compared with E-EFM-10, the mechanism name is unchanged while the state architecture changes materially:

```text
Reduced coefficient model:
current field / temperature -> constitutive coupling -> thermal response

Dynamic order model:
P(t+dt) = evolution(P(t), E, T, free energy)
P/T interaction -> thermal response
```

**Preliminary classification:**

- `C-EFM-2` remains present because free energy / constitutive relationships define response;
- `C-EFM-3` is present when polarization is retained as mechanism-local persistent order state;
- `C-EFM-4R` may apply when polarization/electric state is governed as a separate physical-domain responsibility;
- `C-EFM-5R` applies only if the selected Thermodynamic formulation itself declares that this additional coordinate is part of authoritative thermodynamic state or that the thermodynamic governing relation is incomplete without it.

This classification is therefore **formulation-relative**.

---

### E-EFM-12 — MOOSE Phase Field: Governing Order Parameters vs Material Free-Energy Objects

**Source family:** MOOSE official Phase Field documentation  
**Mechanism:** conserved/nonconserved order-state evolution  
**Primary sources:**

- https://mooseframework.inl.gov/moose/modules/phase_field/Phase_Field_Equations.html
- https://mooseframework.inl.gov/moose/modules/phase_field/FunctionMaterials/FreeEnergy.html

**Observed architecture / physics:**

The Phase Field module distinguishes:

- conserved variables evolved by Cahn–Hilliard equations;
- nonconserved order parameters evolved by Allen–Cahn equations;
- free-energy functions and derivatives supplied through the Material system.

The order parameter is therefore a governing solution variable even though free-energy functions used to evolve it are implemented as `Material` objects/properties.

**Taxonomy pressure:**

This is strong evidence against classifying semantic state from software-container names.

A `Material` can provide constitutive/free-energy data while a separate variable remains authoritative governing state.

**Preliminary result:**

`C-EFM-2` describes the constitutive/free-energy relationship, while an evolved order parameter belongs to a state category determined by its governing role (`C-EFM-3`, `C-EFM-4R`, or `C-EFM-5R` depending system boundary).

Therefore:

> **implementation placement of a free-energy or property object does not determine whether the information it depends on is Configuration, local history, cross-domain state, or Thermodynamic State.**

---

### E-EFM-13 — Elastocaloric / Shape-Memory Thermomechanical Internal Variables

**Source family:** shape-memory-alloy constitutive literature  
**Mechanism:** martensitic transformation / elastocaloric thermomechanical response  
**Primary sources:**

- https://doi.org/10.1177/1045389X9300400213
- https://www.sciencedirect.com/science/article/abs/pii/S0167663605001249
- https://www.sciencedirect.com/science/article/pii/S0022509622000977
- https://www.sciencedirect.com/science/article/am/pii/S0020768321003462

**Observed physics:**

Thermomechanical constitutive models for shape-memory alloys commonly introduce internal variables such as:

- martensite volume fraction;
- transformation strain;
- transformation entropy;
- residual martensitic fraction or related history variables.

The non-isothermal elastocaloric literature couples phase transformation, mechanics, and heat transfer. Internal heat generation may include latent heat, thermoelastic contribution, and dissipation while mechanical equilibrium and thermodynamic/thermal equations are solved together.

**Taxonomy pressure:**

This is a direct example where multiple RQ-EFM labels coexist:

- constitutive/free-energy mapping (`C-EFM-2`);
- persistent transformation/history state (`C-EFM-3`);
- mechanical governing state coupled to thermal response (`C-EFM-4R` when mechanics remains a separate physical-domain responsibility).

Whether martensite fraction or transformation entropy must become authoritative Thermodynamic State cannot be determined from the mechanism name. It depends on the selected thermodynamic-system boundary and formulation.

**Preliminary result:**

The five-label taxonomy cannot be treated as mutually exclusive categories.

A single physically correct model may require **C-EFM-2 + C-EFM-3 + C-EFM-4R simultaneously**.

---

### E-EFM-14 — preCICE Implicit Partitioned Coupling and Solver Checkpoint State

**Source family:** preCICE official documentation  
**Mechanism:** general partitioned multiphysics coupling  
**Primary sources:**

- https://precice.org/docs
- https://precice.org/configuration-coupling
- https://precice.org/couple-your-code-implicit-coupling

**Observed architecture / coupling:**

preCICE explicitly couples existing solvers as separate participants in partitioned multiphysics simulations.

Its coupling schemes may be:

- serial or parallel;
- explicit or implicit.

Implicit schemes repeat a coupling time window until interface data converge. The participant adapter must be able to checkpoint and restore enough solver information to reproduce an iteration when preCICE requests rollback.

**Taxonomy pressure:**

This establishes that:

- strong/iterative physical coupling can occur between separately governed solver states;
- numerical coupling strategy does not determine semantic state ownership;
- additional checkpoint state required by the coupling algorithm is not automatically physical-domain state.

**Preliminary classification:**

`C-EFM-4R` is best understood as a **governing-domain relationship**, not a thermal-side source/flux/property form and not a numerical solver strategy.

A partitioned implementation may exchange source, flux, temperature, displacement, force, or other data while the semantic classification of those quantities remains determined by the participating physical models.

---

### E-EFM-15 — COMSOL Thermoelectric Coupled Conservation Revisited

**Source family:** COMSOL official documentation  
**Mechanism:** Seebeck / Peltier / Thomson thermoelectric coupling  
**Primary sources:**

- https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_theory.07.087.html
- https://doc.comsol.com/6.4/doc/com.comsol.help.heat/heat_ug_multiphysics_features.12.14.html
- https://doc.comsol.com/6.4/doc/com.comsol.help.comsol/physics_builder_manual_examples.44.04.html

**Observed physics:**

The thermoelectric formulation simultaneously includes:

- heat-energy conservation;
- electric-current conservation;
- temperature `T`;
- electric potential `V`;
- modified conductive heat flux;
- modified electric current density;
- Joule, Peltier, Seebeck, and Thomson-related contributions.

The documentation treats `T` and `V` as different dependent variables in the coupled system.

**Taxonomy pressure:**

This strongly supports a distinction between:

- **thermal-side interaction form** — Peltier/Thomson terms can appear as heat-flux/source contributions;
- **cross-domain governing relationship** — electrical current conservation and thermal energy conservation are both active and mutually coupled.

The thermal-side use of a source or flux does not reduce the entire thermoelectric problem to `C-EFM-1`.

**Preliminary classification:**

`C-EFM-4R + flux/source/constitutive interaction forms`.

Nothing in this official formulation requires electric potential to be reclassified as Thermodynamic State merely because it is strongly coupled to temperature.

---

### E-EFM-16 — Magnetocaloric Equilibrium Free-Energy Formulations

**Source family:** peer-reviewed magnetocaloric thermodynamics literature  
**Mechanism:** magnetic-field-driven entropy / temperature response  
**Primary sources:**

- https://journals.aps.org/prb/abstract/10.1103/PhysRevB.64.144406
- https://journals.aps.org/prb/abstract/10.1103/PhysRevB.71.054410
- https://www.nature.com/articles/s41598-025-15896-8

**Observed physics:**

Equilibrium magnetocaloric formulations use thermodynamic potentials containing magnetic field and magnetization. In one reviewed formulation, Gibbs free energy contains a Zeeman term involving `H` and `M`, and equilibrium magnetization is obtained through free-energy minimization.

Other models couple magnetic and lattice entropy/energy contributions.

**Taxonomy pressure:**

This provides an important contrast to dynamic electrocaloric and shape-memory examples.

An order quantity can be:

- an equilibrium variable obtained from current field/temperature by minimization; or
- a persistent history-dependent variable in another formulation.

Therefore persistent-state classification cannot be inferred merely from the presence of `M`, `P`, phase fraction, or another order quantity in a free-energy expression.

**Preliminary result:**

The decisive question is whether the quantity requires independent temporal history/evolution under the selected model, not whether it is physically important.

---

### E-EFM-17 — Established Multiphysics Coupling Taxonomies

**Source family:** multiphysics modeling literature  
**Mechanism:** general coupling classification  
**Primary sources:**

- https://doi.org/10.1016/B978-0-12-407709-6.00002-X
- https://journals.sagepub.com/doi/10.1177/1094342012468181
- https://doi.org/10.1002/er.5111

**Observed prior art:**

Existing multiphysics literature already separates multiple coupling dimensions.

One established classification distinguishes physical coupling forms including production-term, natural-boundary-condition, constitutive-equation, and other coupling types.

The Keyes et al. review explicitly distinguishes physical strong/weak coupling from numerical tight/loose coupling and notes that these are not one-to-one.

A later multiscale thermal-hydraulic review classifies coupling using several independent dimensions such as architecture, operation mode, domain coupling, field mapping, and temporal coupling.

**Taxonomy pressure:**

This is strong prior-art pressure against treating the RQ-EFM-001 multi-axis observation itself as novel.

The current RQ-EFM contribution candidate, if any later survives, cannot be:

- `coupling has multiple dimensions`;
- `source coupling differs from constitutive coupling`;
- `partitioned differs from monolithic`; or
- `physical coupling strength differs from numerical coupling tightness`.

Those are already established.

The remaining RQ-EFM question must stay narrower and specifically concern **thermodynamic state authority / governing-state classification under external-field coupling**.

---

## 5. Cross-Evidence Matrix — v0.2 Additions

Legend:

- `None/current` — no additional retained history beyond current declared inputs in the reviewed formulation.
- `Local` — persistent mechanism-local/internal state.
- `Cross-domain` — separate governing physical-domain state.
- `Thermo-formulation` — candidate state-space/formulation impact, not automatically established by mechanism name.
- `U` — unresolved from public evidence.

| Evidence | Mechanism | Thermal-side form | Extra persistent/history state | Separate governing domain | Thermodynamic state-space revision required by reviewed formulation? | Main pressure |
|---|---|---|---|---|---|---|
| E-EFM-09 | MOOSE material properties | Constitutive/property | None/current or Local depending request | No | No | Separates current property mapping from stateful history |
| E-EFM-10 | Reduced pyroelectric/electrocaloric | Constitutive + heat-source coupling | Not exposed as independent polarization PDE | Electrical/structural coupling may exist | Not shown | Same mechanism can use reduced coefficient form |
| E-EFM-11 | Dynamic electrocaloric LGD/LK | Free-energy + thermal coupling | Polarization history/evolution | Electrical/order domain may be explicit | Formulation-dependent | Explicit order dynamics defeats property-only classification |
| E-EFM-12 | MOOSE phase field | Free-energy constitutive mapping | Governing order parameters | Phase-field/order equations | Depends on selected system boundary | Software Material object does not define state semantics |
| E-EFM-13 | Elastocaloric SMA | Work/latent heat + constitutive | Martensite fraction / transformation variables | Mechanics | Formulation-dependent | C2+C3+C4R can coexist |
| E-EFM-14 | preCICE implicit partitioned coupling | Any exchanged form | Solver checkpoints + domain state | Yes | No implication | Numerical coupling and semantic ownership are orthogonal |
| E-EFM-15 | Thermoelectric | Heat flux/source + current flux + constitutive | Electrical and thermal solution state | Yes | Not required merely by coupling | Strong cross-domain coupling need not merge state |
| E-EFM-16 | Magnetocaloric equilibrium models | Free-energy / entropy response | May be equilibrium-derived in reviewed models | Magnetic variables | Formulation-dependent | Order variable importance does not imply persistent history |
| E-EFM-17 | General multiphysics taxonomy | Multiple | N/A | Multiple | N/A | Multi-axis coupling taxonomy is established prior art |

---

## 6. Falsification Result — The Five Labels Are Not a Single-Axis Taxonomy

The central v0.2 falsification result is:

```text
FIVE-LABEL MUTUALLY-EXCLUSIVE TAXONOMY:
NOT SUPPORTED
```

The five labels remain useful research vocabulary, but they describe different semantic dimensions.

For example:

- a Joule-heating problem can be `source contribution + cross-domain governing coupling`;
- an elastocaloric model can be `constitutive coupling + local persistent state + cross-domain mechanical governing state`;
- a dynamic electrocaloric model can be `free-energy constitutive coupling + persistent polarization state`, with the ownership of polarization depending on the selected architecture;
- a thermoelectric model can use thermal source/flux terms while simultaneously solving a separate electrical conservation law.

Therefore a mechanism should not receive one exclusive `C-EFM-N` label.

---

## 7. Refined Multi-Axis Research Taxonomy — Under Survey

The evidence supports replacing the single-axis class interpretation with a multi-axis research description.

This is a research taxonomy only. It is not a Framework concept or Specification proposal.

### Axis A — Thermal / Thermodynamic Interaction Form

Record how the other mechanism enters the thermodynamic computation:

- `A1 Source / deposition` — additive energy or power contribution;
- `A2 Flux / work exchange` — modifies heat/energy flux or explicit work exchange;
- `A3 Constitutive dependency` — modifies a coefficient, free-energy relation, closure, transition condition, or material response;
- `A4 State-space / governing relation change` — the selected thermodynamic formulation itself requires a new coordinate, closure, or conservation responsibility.

A mechanism may occupy more than one A-form at once.

### Axis B — Driving / Internal State Role

Record what information is needed beyond current Thermodynamic State:

- `B0 Prescribed input` — externally specified field/load/power history;
- `B1 Current algebraic/equilibrium variable` — recoverable from current declared inputs without independent history;
- `B2 Mechanism-local persistent state` — history/evolution needed only for the optional mechanism;
- `B3 Separate cross-domain governing state` — another physical domain owns and evolves its own governing variables;
- `B4 Thermodynamic governing state` — the selected thermodynamic formulation itself requires the quantity as authoritative persistent thermodynamic information.

`B2`, `B3`, and `B4` are semantic roles, not memory-layout choices.

### Axis C — Coupling Direction / Numerical Relation

Record interaction structure without using it to infer ownership:

- one-way;
- bidirectional;
- explicit iterative;
- implicit iterative;
- monolithic;
- partitioned.

This axis is descriptive and largely established prior art.

### Axis D — Thermodynamic Authority Impact

Record whether the selected Thermodynamic Core/formulation remains semantically unchanged:

- `D0 No Thermodynamic Core/Formulation Revision`;
- `D1 Thermodynamic Core/Formulation Revision Required`.

This is the key RQ-EFM authority boundary.

---

## 8. Mapping the v0.1 Labels onto the Multi-Axis Taxonomy

The previous labels can now be interpreted more precisely:

| v0.1/v0.2 label | Refined meaning |
|---|---|
| C-EFM-1 | Usually Axis A1; may coexist with B3 cross-domain governing state and bidirectional coupling. |
| C-EFM-2 | Usually Axis A3; valid as `property-only` only when no additional history/governing state is hidden. |
| C-EFM-3 | Axis B2 mechanism-local persistent state. |
| C-EFM-4R | Primarily Axis B3 plus a coupled relationship on Axis C; does not imply D1. |
| C-EFM-5R | Axis D1 and often A4/B4; genuine Thermodynamic Core/formulation revision. |

This mapping resolves the principal ambiguity found in v0.1.

---

## 9. Provisional Decision Rules

The following rules are evidence-supported research discriminators, not normative Framework requirements.

### R-EFM-01 — Constitutive-Only Rule

A mechanism may be described as constitutive/property coupling without additional state only when the required property is recoverable from current declared inputs and configuration:

```text
property(t) = f(current inputs, current state, configuration)
```

and no independently evolved or previous-history quantity is required.

If the result depends on retained history or an evolution law, `B2`, `B3`, or `B4` state semantics must also be recorded.

### R-EFM-02 — Local-State Rule

Persistent mechanism state is `B2 mechanism-local` only when:

- it exists solely for the optional mechanism;
- removal of the mechanism does not make the selected Thermodynamic Core semantically incomplete;
- it is not actually another physical domain's governing state; and
- it is not required by the selected thermodynamic formulation as authoritative thermodynamic state.

### R-EFM-03 — Cross-Domain Governing Rule (`C-EFM-4R`)

A mechanism belongs to the cross-domain governing pattern when:

- another physical domain has its own governing variables/equations;
- those variables are co-evolved or solved independently rather than merely prescribed;
- the domains exchange source/flux/property/boundary information; and
- the selected thermodynamic state identity and governing thermodynamic formulation remain semantically valid without absorbing that other domain's state.

Partitioned versus monolithic implementation does not decide this classification.

### R-EFM-04 — Thermodynamic Core/Formulation Revision Rule (`C-EFM-5R`)

A Thermodynamic Core/formulation revision is required when correct evolution of the **selected thermodynamic model itself** becomes incomplete unless at least one of the following occurs:

- a new authoritative persistent thermodynamic coordinate is added;
- a new thermodynamic work/conservation term changes the governing thermodynamic formulation rather than acting as a supplied exchange term;
- a closure relation changes the semantic identity of the thermodynamic state-space;
- a responsibility previously external to Thermodynamic Computation becomes part of thermodynamic state evolution or completeness.

The mere existence of a second strongly coupled solver is not sufficient.

### R-EFM-05 — Formulation-Relative Classification Rule

Mechanism names do not determine classification.

`electrocaloric`, `magnetocaloric`, `thermoelectric`, `elastocaloric`, or `Joule heating` can map differently depending on whether the selected model is:

- prescribed;
- equilibrium/algebraic;
- history-dependent;
- separately co-evolved; or
- incorporated into an expanded thermodynamic formulation.

This rule is required to preserve contradictory but legitimate modeling choices in the evidence.

---

## 10. Hidden-State Audit Questions

For any future claim that a field response is only a material-property update, the following checks should be applied before classification:

1. Does the property depend only on current values, or on old/older history?
2. Is an order parameter minimized algebraically, or evolved dynamically?
3. Is a field value prescribed, or solved by a separate conservation/governing equation?
4. Does hysteresis require retained branch/phase/order information?
5. Would two systems with identical current Thermodynamic State and external field evolve differently because of hidden history?
6. If yes, where is that distinguishing state owned?
7. If the alleged property state is removed, does the thermodynamic evolution become semantically incomplete or only the optional mechanism disappear?
8. Is a software `MaterialProperty` or coefficient merely carrying an underlying state dependency from another variable?

If two systems with identical declared current inputs can evolve differently because of unrecorded history, a property-only classification is invalid.

---

## 11. C-EFM-4R vs C-EFM-5R Boundary Test

The v0.2 evidence supports the following provisional test.

### C-EFM-4R / D0 case

Use the cross-domain governing classification when the complete coupled system requires another governing domain, but the thermodynamic subsystem still has a coherent state/evolution definition of its own.

Examples from the current evidence include:

- thermoelectric coupling with separate temperature and electric-potential equations;
- partitioned multiphysics participants that exchange fields/fluxes;
- thermo-mechanical coupling where mechanics retains its own governing variables.

The thermal subsystem may be unable to predict the coupled problem alone, but that is different from saying its own state semantics have become invalid.

### C-EFM-5R / D1 case

Use the Thermodynamic Core/formulation-revision classification when the thermodynamic subsystem's **own selected state-space or conservation semantics** must change.

The decisive question is not:

> `Does another field affect temperature?`

It is:

> `After abstracting the other domain as declared external inputs/exchanges, is the existing thermodynamic state and evolution relation still a complete and semantically correct thermodynamic model for the selected scope?`

If yes, strong coupling may still be C-EFM-4R/D0.

If no, C-EFM-5R/D1 is required.

This boundary is intentionally consistent with the RQ-ISO-001 result that governing-physics changes must not be hidden inside ordinary extension state merely to preserve a zero-Core-change claim.

---

## 12. Prior-Art Exclusions Established by v0.2

The following broad claims are now removed from future ThermoCore-specific contribution candidates:

- source/deposition coupling is not novel;
- constitutive-equation coupling is not novel;
- stateful material/history variables are not novel;
- evolving phase/order parameters are not novel;
- partitioned multiphysics and implicit exchange are not novel;
- separate domain solvers participating in strong coupling are not novel;
- thermoelectric bidirectional heat/electric coupling is not novel;
- classifying multiphysics coupling along multiple independent dimensions is not novel;
- distinguishing physical coupling strength from numerical tightness is not novel.

Any later RQ-EFM Research Gap must be narrower than these established patterns.

---

## 13. Current Taxonomy Disposition

| Item | v0.2 classification |
|---|---|
| Original four-class taxonomy | Rejected as complete |
| Five-label mutually exclusive taxonomy | **Not supported** |
| C-EFM-1 source/deposition vocabulary | Supported as established prior-art pattern |
| C-EFM-2 constitutive/property vocabulary | Supported, but only with hidden-state audit |
| C-EFM-3 mechanism-local persistent state vocabulary | Supported as a semantic role, not an exclusive mechanism class |
| C-EFM-4R cross-domain governing coupling | Supported as distinct from simple source/property coupling and from Core revision |
| C-EFM-5R Thermodynamic Core/formulation revision | Supported as a necessary boundary category, but not yet a Research Gap |
| Multi-axis taxonomy | **Supported as the better research representation; generic multi-axis coupling classification itself is prior art** |
| Mechanism-name-based classification | Rejected |
| Research Gap | Not established |
| Research Contribution | Not established |
| Framework Specification change | Not authorized |

---

## 14. Remaining Falsification Targets

Three uncertainties remain before a Research Gap analysis is justified.

### F-EFM-07 — Existing State/Authority Taxonomy Prior Art

The v0.2 survey found broad prior art for multi-axis coupling taxonomies, but has not yet determined whether a prior framework or literature line explicitly distinguishes:

- current constitutive dependency;
- mechanism-local history;
- separately governed cross-domain state; and
- thermodynamic state-space/formulation revision

using an authority/completeness criterion comparable to the provisional R-EFM rules.

If such prior art exists, the surviving ThermoCore distinction must be narrowed to specialization/integration/evaluation.

### F-EFM-08 — Equilibrium vs Dynamic Order-State Boundary

Additional evidence is needed to test whether the provisional current-vs-history rule remains coherent across:

- equilibrium magnetization/polarization models;
- hysteretic first-order caloric transitions;
- phase-field order-parameter models; and
- rate-dependent kinetic formulations.

If realistic models require ad hoc exceptions, the taxonomy must be revised again.

### F-EFM-09 — Generalized Work-Term Boundary

The current evidence does not yet fully separate:

- externally supplied generalized work that can enter thermodynamic energy exchange without state-space revision; from
- thermodynamic formulations in which generalized coordinates/work pairs are themselves part of the authoritative thermodynamic state relation.

This is the most important remaining pressure on the C-EFM-4R/C-EFM-5R boundary.

---

## 15. Research-Gap Readiness Decision

The v0.2 evidence is **not yet sufficient** to open a Research Gap analysis.

Decision:

```text
RQ-EFM-001 RESEARCH-GAP READINESS:
NO-GO — ONE MORE BOUNDED EVIDENCE PASS REQUIRED
```

Reason:

The taxonomy changed materially during both v0.1 and v0.2:

- v0.1 split cross-domain governing coupling from Core revision;
- v0.2 showed that the five labels are not mutually exclusive and must be represented as orthogonal semantic axes.

Opening a Gap analysis now would risk treating a still-evolving research taxonomy as a stable missing architecture property.

The next pass should therefore be narrower rather than broader.

---

## 16. Recommended v0.3 Evidence Boundary

The next evidence pass should stop collecting generic Joule-heating, thermoelectric, and partitioned-coupling examples unless they directly test the remaining authority boundary.

Priority targets should be:

1. **Generalized thermodynamic coordinate/work-pair literature** — determine when field/mechanical variables are treated as thermodynamic state coordinates versus externally supplied work/exchange.
2. **Equilibrium vs hysteretic caloric formulations** — test whether current algebraic order variables and persistent order/history variables can be separated consistently.
3. **Framework-level state semantics** — search Modelica/MOOSE/other multiphysics abstractions for explicit rules separating constitutive dependency, internal state, coupled-domain state, and thermodynamic state-space expansion.
4. **Direct falsification target** — actively search for an existing architecture that already formalizes the full provisional R-EFM-01 through R-EFM-05 boundary logic.

If the v0.3 pass finds an existing direct equivalent, RQ-EFM-001 should be reframed around specialization or evidence-backed integration rather than a Research Gap.

If the boundary survives with stable definitions and no direct equivalent in the bounded evidence set, a dedicated Research Gap task may then be justified.

---

## 17. Conclusion

The v0.2 evidence resolves the main ambiguity left by the first survey.

The RQ-EFM coupling vocabulary does not describe five mutually exclusive mechanism classes. It describes different semantic dimensions that can coexist in one coupled model.

The most stable current interpretation is:

```text
Interaction form
    +
State role / ownership
    +
Coupling direction / numerical relation
    +
Thermodynamic authority impact
```

The strongest new discriminator is not whether a mechanism is `strongly coupled` or implemented in a separate solver.

It is whether the selected thermodynamic state-space and governing formulation remain semantically complete when the external mechanism is represented through declared inputs/exchanges, or whether correct thermodynamic evolution itself requires new authoritative thermodynamic state or governing responsibility.

That distinction is promising enough to continue RQ-EFM-001, but the current evidence also shows substantial prior art around multiphysics coupling taxonomies. Therefore no Research Gap claim is justified yet.

The correct next step is one additional, tightly bounded evidence pass focused on generalized thermodynamic work/state coordinates and explicit state-authority prior art.