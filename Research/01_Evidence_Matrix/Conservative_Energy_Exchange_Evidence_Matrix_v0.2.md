# Conservative Energy Exchange Evidence Matrix

Version: 0.2  
Status: DIRECT-ANTECEDENT STRESS TEST — INDEPENDENT GAP CANDIDATE NOT SUPPORTED  
Research Question: RQ-CEX-001 — Conservative Cross-Boundary Energy Exchange  
Tracking Issue: #123  
Date: 2026-08-23

---

## 1. Objective

This second bounded evidence pass directly stress-tests the only candidate that survived v0.1:

> **Ownership-Preserving Single-Authority Energy Accounting Boundary**

The candidate asks whether ThermoCore adds an independently defensible architecture-level contribution by requiring each admitted energy-bearing interaction to have one unambiguous thermodynamic accounting role and conservation target while Thermodynamic State ownership remains separate and Thermodynamic Computation remains the only Framework Core writer of Thermodynamic State.

The purpose of v0.2 is not to collect additional examples of heat sources, heat fluxes, power variables, or conservative mappings. It tests whether established physical-network and port-based modeling semantics already provide the relevant conservation/accounting structure, such that the remaining ThermoCore-specific statement is only a composition of established conservation semantics with the already-supported RQ-ISO-001 and RQ-EFM-001 boundaries.

This document is non-normative. It does not establish novelty, priority, superiority, Framework Specification requirements, implementation requirements, or Validation status.

---

## 2. Stress-Test Question

The v0.1 candidate survives only if evidence supports a distinction stronger than the following already-known pattern:

```text
separate subsystem state
        +
physically typed port / bond / connection
        +
power or energy balance at the interaction
        +
external supply distinguished from internal conservative interconnection
        =
no need to merge subsystem state merely to conserve exchanged energy
```

The v0.2 test therefore asks:

1. Do established modeling systems already encode internal physical interactions as conserving connections rather than as independent source additions at both sides?
2. Do those systems preserve separate component/subsystem state while enforcing the interaction balance?
3. Do they distinguish energy storage/dissipation/source behavior from lossless interconnection behavior?
4. If yes, does ThermoCore's remaining 'single thermodynamic accounting authority' reduce to the application of those known semantics under RQ-ISO-001's already-supported state/write authority rule?

If the answer is yes, the independent RQ-CEX-001 gap candidate shall be closed rather than narrowed indefinitely.

---

## 3. Concepts Kept Separate

v0.2 distinguishes three concepts that must not be collapsed:

| Concept | Meaning | Research status after v0.2 |
|---|---|---|
| Physical interaction identity | One physical transfer is represented through a defined port/bond/connection relation and corresponding balance semantics | Strong prior art |
| State ownership | Which architectural responsibility governs and evolves subsystem state | ThermoCore-specific governance already addressed by RQ-ISO-001; not newly established by RQ-CEX |
| Accounting role | Whether the interaction is an external source/sink, internal transfer, boundary exchange, or cross-domain conversion | Strong general prior art; exact ThermoCore classification remains engineering/conformance usage |

The critical question is not whether these three can coexist. Prior art shows they can. The question is whether their composition within ThermoCore creates a new independent research gap. This pass finds insufficient support for that claim.

---

## 4. Direct-Antecedent Summary

| Source family | Conserving interaction | Separate component/subsystem state | External vs internal interaction distinction | Direct pressure on v0.1 candidate |
|---|---|---|---|---|
| Modelica flow + stream connectors | **Yes** — flow variables generate zero-sum connection equations; stream connection set is an infinitesimal control volume with mass/energy conservation | **Yes** — components retain their own models/state; connection equations couple only declared connector variables | **Yes / strong partial** — port interaction is separated from component storage/source equations | **Very strong** |
| Bond graphs | **Yes** — 0/1 junctions are power continuous and do not store/dissipate energy | **Yes** — storage, dissipation, sources, transformers and junctions are distinct elements | **Yes** — sources/storage/dissipation are explicitly different element classes from lossless junctions | **Very strong** |
| Port-Hamiltonian / Dirac structures | **Yes** — Dirac structures encode power-conserving interconnection; open/boundary ports carry energy exchange | **Yes** — subsystem Hamiltonians/state remain distinct and are interconnected through ports | **Yes** — internal interconnection and external/boundary power ports are structurally distinguished | **Very strong** |
| Simscape conserving ports | **Yes** — Through variables balance at branches; Across variables are compatible across connected ports | **Yes** — components remain separate while physical network equations enforce connection laws | **Yes / engineering precedent** — energy exchange occurs through conserving ports rather than signal ownership transfer | **Strong** |
| ECCO / power bonds | **Yes, with discrete residual accounting** — ideal power balance and residual energy over communication intervals | **Yes** — independently stepped simulators remain separate | **Yes / coupling-level** — residual measures energetic mismatch across coupling | **Strong temporal complement** |

---

## 5. Evidence Record CEX2-01 — Modelica Flow and Stream Connection Semantics

### 5.1 Primary sources

- Modelica Language Specification, Chapter 9 — Connectors and Connections:  
  https://specification.modelica.org/master/connectors-and-connections.html
- Modelica Language Specification, Chapter 15 — Stream Connectors:  
  https://specification.modelica.org/master/stream-connectors.html
- Modelica Language Specification, Appendix — Derivation of Stream Equations:  
  https://specification.modelica.org/maint/3.6/derivation-of-stream-equations.html

### 5.2 Findings

Modelica connection semantics generate zero-sum equations for variables declared with the `flow` prefix. In a connection set, potential variables become equal and flow variables sum to zero with signs determined by inside/outside connector orientation.

This is already a language-level conservation structure: connected components do not each independently invent a separate contribution for the same physical connection. The connection set itself generates the coupling balance.

Modelica stream connectors go further for fluid energy transport. A stream connector associates a specific transported quantity such as specific enthalpy with a mass-flow variable. The Language Specification explicitly states that the connection set may be viewed as an infinitesimally small control volume and that the generated stream connection equations are equivalent to conservation equations for mass and energy.

The standard physical energy-balance usage is of the form:

```text
mass flow × actual transported specific enthalpy
```

which yields an energy-flow contribution for the component balance while preserving direction-dependent transport semantics.

### 5.3 Direct pressure on the surviving candidate

This is stronger than the v0.1 evidence based only on `HeatPort`.

It demonstrates all of the following simultaneously:

- component models and their internal states remain separate;
- one physical connection is represented through one connection set;
- the connection semantics generate a conservation relation;
- transported energy is tied to the associated physical flow;
- internal transfer is not represented as two unrelated external heat-source additions;
- a component may include the resulting port contribution in its own energy balance without owning the other component's state.

Therefore, the general idea:

> 'one physical transfer needs one coherent accounting relation while states remain separately governed'

already has a strong direct thermodynamic-domain antecedent.

### 5.4 Remaining difference from ThermoCore

Modelica does not define ThermoCore's exact rule that Thermodynamic Computation exclusively writes a shared authoritative Thermodynamic State. However, that rule is already part of ThermoCore's RQ-ISO-001 / Framework architecture history. It is not newly discovered by RQ-CEX-001.

Accordingly, combining Modelica-style conserving connection semantics with ThermoCore's existing write-authority rule does not, by itself, establish a new independent research contribution.

**Classification:** `DIRECT ANTECEDENT — VERY STRONG`.

---

## 6. Evidence Record CEX2-02 — Bond-Graph 0/1 Junctions

### 6.1 Sources

- 20-sim Bond Graph tutorial, 0 and 1 junctions:  
  https://20sim.com/webhelp/modeling_tutorial_bond_graphs_zeroandonejunctions.php
- Broenink, *Bond Graphs: A Unifying Framework for Modeling Physical Systems*, University of Twente author-hosted material:  
  https://ris.utwente.nl/ws/portalfiles/portal/250822857/Broenink_2020_Bond_graphs_a_unifying_framework_fo.pdf
- van der Schaft / collaborators, port-based modeling text, University of Twente repository:  
  https://ris.utwente.nl/ws/portalfiles/portal/185509417/Duindam09modeling.pdf

### 6.2 Findings

Bond graphs model physical interaction through power bonds whose effort and flow variables define power.

The canonical 0- and 1-junctions are **power-continuous** elements:

- a 0-junction equalizes effort and imposes a signed zero-sum relation on flows;
- a 1-junction equalizes flow and imposes a signed zero-sum relation on efforts.

The junction itself does not store or dissipate energy. Storage elements (`I`, `C`), dissipation (`R`), sources (`Se`, `Sf`), transformers/gyrators, and junctions have distinct physical roles.

This is important for RQ-CEX because an internal connection is not modeled as a new energy source. It is modeled as a power-conserving relation between elements. Energy creation, storage, and dissipation belong to different explicit model elements.

### 6.3 Direct pressure on the surviving candidate

Bond graphs already encode the principle that:

> a physical interaction should be represented by one power-continuous interconnection relation, while source/storage/dissipation roles remain structurally distinct.

That principle substantially overlaps the proposed RQ-CEX requirement that one physical energy contribution not be semantically counted once as internal transfer and again as a new external source.

The remaining ThermoCore-specific aspect is not the conservation principle. It is the existing governance choice that Thermodynamic Computation alone has Framework Core state-write responsibility.

Again, this makes the v0.1 candidate look like a composition of known energy-interconnection semantics with RQ-ISO-001 rather than a new standalone energy-exchange research gap.

**Classification:** `DIRECT POWER-ACCOUNTING ANTECEDENT — VERY STRONG`.

---

## 7. Evidence Record CEX2-03 — Port-Hamiltonian / Dirac-Structure Interconnection

### 7.1 Sources

- Duindam et al., *Modeling and Control of Complex Physical Systems: The Port-Hamiltonian Approach*, University of Twente repository:  
  https://ris.utwente.nl/ws/portalfiles/portal/185509417/Duindam09modeling.pdf
- Port-Hamiltonian continuum mechanics example, *Journal of Nonlinear Science*:  
  https://link.springer.com/article/10.1007/s00332-025-10130-1

### 7.2 Findings

Port-Hamiltonian systems generalize bond-graph power-interconnection semantics.

A key construct is the Dirac structure, which encodes power-conserving relations among effort and flow variables. In the simplest junction examples, the sum of powers is zero. For open systems, boundary or interaction ports allow nonzero energy exchange with other subsystems or the environment while preserving an explicit power balance.

The architecture therefore separates:

- subsystem stored energy represented by Hamiltonians/state;
- power-conserving internal interconnection;
- dissipative relations where applicable; and
- external or boundary power exchange.

### 7.3 Direct pressure on the surviving candidate

This is especially damaging to the idea that an ownership-preserving energy accounting boundary is new merely because subsystems retain separate state.

Port-Hamiltonian modeling explicitly supports open interconnected subsystems whose internal state remains in the subsystem while their energetic coupling is represented through ports and power-balance relations.

The important architectural distinction is already mature:

```text
state storage / dynamics
        !=
lossless interconnection
        !=
external boundary power
        !=
dissipation
```

ThermoCore's separate state authority is therefore compatible with established port-based physics rather than an independent RQ-CEX novelty source.

**Classification:** `DIRECT ARCHITECTURE-LEVEL ANTECEDENT — VERY STRONG`.

---

## 8. Evidence Record CEX2-04 — Simscape Conserving Ports

### 8.1 Primary sources

- MathWorks, *Through and Across Variables*:  
  https://www.mathworks.com/help/simscape/ug/through-and-across-variables.html
- MathWorks, *Essential Physical Modeling Techniques*:  
  https://www.mathworks.com/help/simscape/ug/essential-physical-modeling-techniques.html
- MathWorks, *How Simscape Simulation Works*:  
  https://www.mathworks.com/help/simscape/ug/how-simscape-simulation-works.html

### 8.2 Findings

Simscape physical networks use conserving ports. Domain-specific Through and Across variables represent physical exchange. MathWorks documentation states that energy flow is commonly characterized by paired variables whose product is power.

For directly connected conserving ports:

- Across variables are constrained to compatible/equal values; and
- Through variables obey branch conservation, with the sum entering a branch matching the sum leaving it.

Components remain distinct; the physical network constructs the system equations from the conserving interconnection.

### 8.3 Direct pressure on the surviving candidate

Simscape independently demonstrates the same engineering pattern outside Modelica and academic bond-graph literature:

> component separation is compatible with conservation-aware physical-network connections, and the connection semantics — not duplicated source bookkeeping — govern the physical transfer.

Simscape does not provide ThermoCore's normative state-ownership vocabulary. But that vocabulary is not enough to make the underlying energy-accounting rule a separate research contribution.

**Classification:** `INDEPENDENT ENGINEERING ANTECEDENT — STRONG`.

---

## 9. Relationship to ECCO / Discrete Co-Simulation

ECCO remains relevant but no longer carries the main v0.2 burden.

Continuous physical-network formalisms such as Modelica, bond graphs, port-Hamiltonian systems, and Simscape already establish conserving connection semantics. ECCO adds the discrete co-simulation observation that independently stepped subsystems may violate ideal power balance numerically during communication intervals, producing residual power and integrated residual energy.

This confirms the earlier v0.1 distinction:

```text
semantic identity and conservation target of an interaction
        !=
actual numerical conservation achieved by a coupling algorithm
```

Therefore a future ThermoCore engineering/conformance property may state the semantic accounting role without promising that every discretization, mapper, or time-coupling algorithm will conserve energy numerically.

---

## 10. Stress Test of the Three Candidate Components

### 10.1 One physical contribution requires one coherent interaction identity

**Prior art status: ESTABLISHED.**

Modelica connection sets, bond-graph bonds/junctions, port-Hamiltonian ports, and Simscape conserving connections all associate a physical interaction with a defined connection relation rather than independent source injection at every participant.

No independent RQ-CEX gap is supported here.

### 10.2 Internal transfer must not be reintroduced as external source

**Prior art status: ESTABLISHED AS A GENERAL PHYSICAL-MODELING PRINCIPLE.**

Bond graphs explicitly distinguish sources from power-continuous junctions. Port-Hamiltonian formulations distinguish boundary/external ports from lossless interconnection and storage. Modelica connection equations account internal exchange through the connection balance.

The exact wording 'do not double-count an internal transfer as a source' may not appear verbatim in each source, but it follows directly from the established balance structures. Treating the same interaction as both an internal conserving transfer and an additional independent source would violate the defined system energy balance.

No independent gap is supported merely by restating this consequence.

### 10.3 One Framework Core writer of Thermodynamic State

**Prior art status for this exact ThermoCore rule: NOT THE SUBJECT OF CEX PRIOR ART.**

However, this rule is already established internally by ThermoCore's architecture and RQ-ISO-001. It is not a new CEX-specific finding.

The composition:

```text
known conserving energy-port semantics
        +
known ThermoCore exclusive state-write authority
```

is useful engineering architecture, but the bounded evidence does not establish that the composition itself is a new independent research gap.

---

## 11. Candidate Survival Decision

### 11.1 v0.1 candidate

**Ownership-Preserving Single-Authority Energy Accounting Boundary**

### 11.2 v0.2 result

**NOT SUPPORTED AS AN INDEPENDENT RESEARCH GAP WITHIN THE BOUNDED REVIEW.**

Reason:

1. Physical-network and port-based formalisms already establish conserving internal interconnection under separate subsystem state.
2. They already distinguish source/storage/dissipation/external-boundary roles from lossless internal interconnection.
3. Modelica stream connectors provide a particularly direct thermodynamic antecedent in which mass and energy conservation are generated for a connection set carrying specific enthalpy.
4. Simscape independently confirms the same engineering pattern.
5. The remaining 'single thermodynamic accounting authority' aspect derives from ThermoCore's already-supported RQ-ISO-001 ownership/write rule.
6. RQ-EFM-001 already governs cases where richer exchange semantics are insufficient because the selected thermodynamic formulation itself is incomplete.
7. No independent consequence or semantic structure has yet been identified that cannot be explained by those established antecedents plus the existing ThermoCore RQs.

The correct research response is therefore **closure/reclassification**, not further narrowing for project symmetry.

---

## 12. Recommended Reclassification

Recommended property name:

> **Conservative Exchange Accounting Property**

Recommended classification:

> **ENGINEERING / CONFORMANCE PROPERTY — NOT AN INDEPENDENT RESEARCH CONTRIBUTION**

### 12.1 Suggested property meaning

For a compatible fixed-scope ThermoCore formulation:

- every energy-bearing interaction used by Thermodynamic Computation should have a semantically declared accounting role and conservation target;
- an internal transfer should be represented as redistribution/conserving exchange rather than independently reintroduced as a new external source;
- an external source/sink should be distinguishable from internal redistribution and cross-domain conversion;
- communicated quantity, basis, sign, and temporal support should be sufficient for unambiguous thermodynamic accounting;
- Thermodynamic Computation remains the Framework Core writer of Thermodynamic State;
- extension/cross-domain state remains outside Thermodynamic State unless RQ-EFM-001 requires formulation/Core revision;
- numerical conservation remains a separate Verification/Validation/algorithm concern.

This property is useful and testable, but its conceptual ingredients are not established as novel by this review.

### 12.2 Candidate future evidence

A future engineering/conformance verification could use matched scenarios such as:

- external injection: one declared source increases total thermodynamic energy by the declared amount;
- pairwise redistribution: one side loses exactly the amount the other side gains at semantic accounting level;
- cross-domain conversion: source-domain loss and thermal-domain gain are associated with the same declared physical conversion without shared state ownership;
- temporal stress: identical numeric values under power vs interval-energy semantics produce intentionally different accounting, demonstrating need for explicit temporal support;
- negative case: duplicate treatment of one internal transfer as a new external source is detected as accounting inconsistency;
- formulation boundary: if the required conservation relation cannot be expressed without new governing thermodynamic state, route to RQ-EFM-001.

These would verify an engineering property. They would not retroactively create a research contribution.

---

## 13. Relationship to Prior ThermoCore RQs

The evidence now supports a clean responsibility split:

```text
RQ-EFM-001
Is the selected thermodynamic formulation complete enough for the mechanism/coupling?
        ↓ if ordinary/Core-preserving participation is admissible
RQ-ISO-001
Which state remains outside Core, and who owns/writes authoritative Thermodynamic State?
        ↓
CEX engineering property
Is each admitted energy-bearing interaction semantically accounted as source, transfer,
conversion, or boundary exchange without duplicate thermodynamic accounting?
```

RQ-CEX-001 does not add a third independent research layer under the bounded evidence currently available.

---

## 14. Explicit Non-Claims

This evidence pass does **not** establish that:

- Modelica, bond graphs, port-Hamiltonian systems, Simscape, ECCO, or preCICE are full ThermoCore equivalents;
- all numerical couplings are energy-conservative;
- one universal energy-port representation is sufficient for every thermodynamic mechanism;
- every cross-domain exchange can be reduced to a source term;
- the ThermoCore v1.0.0 reference formulation already implements all proposed exchange roles;
- conservative mapping alone guarantees thermodynamic conservation;
- RQ-CEX-001 has global novelty or priority;
- the recommended engineering property is itself novel;
- any Framework Specification change is authorized by this artifact.

---

## 15. Disposition

| Item | v0.2 disposition |
|---|---|
| Generic energy conservation | `ESTABLISHED PRIOR ART` |
| Port/bond power-continuity semantics | `ESTABLISHED PRIOR ART` |
| Flow zero-sum / sign semantics | `ESTABLISHED PRIOR ART` |
| Stream-carried enthalpy with connection-level mass/energy balance | `DIRECT THERMODYNAMIC ANTECEDENT ESTABLISHED` |
| External/source vs lossless internal interconnection distinction | `ESTABLISHED PRIOR ART` |
| Separate subsystem state with conserving interconnection | `ESTABLISHED PRIOR ART` |
| ThermoCore exclusive Thermodynamic State writer | `EXISTING THERMOCORE RULE / RQ-ISO-001 — NOT NEW CEX CONTRIBUTION` |
| Ownership-Preserving Single-Authority Energy Accounting candidate | `NOT SUPPORTED AS INDEPENDENT RESEARCH GAP WITHIN BOUNDED REVIEW` |
| Surviving value | `CONSERVATIVE EXCHANGE ACCOUNTING PROPERTY` |
| Classification | `ENGINEERING / CONFORMANCE PROPERTY` |
| Research Gap Analysis readiness | `NO-GO` |
| Novelty / priority | `NOT ESTABLISHED` |
| Framework Specification impact | `NONE` |

---

## 16. Recommended Next Action

After review of this matrix:

1. merge v0.2 if accepted;
2. formally close and reclassify RQ-CEX-001 as an engineering/conformance property;
3. do **not** open an RQ-CEX-001 Research Gap Analysis;
4. preserve the negative result and direct antecedent evidence;
5. optionally add a future Verification/Conformance task for the Conservative Exchange Accounting Property;
6. select a new Research Question only if it begins from a distinct unresolved problem rather than re-packaging conservation-port prior art.

---

## 17. References

### Modelica

- Modelica Association, *Modelica Language Specification — Connectors and Connections*.  
  https://specification.modelica.org/master/connectors-and-connections.html
- Modelica Association, *Modelica Language Specification — Stream Connectors*.  
  https://specification.modelica.org/master/stream-connectors.html
- Modelica Association, *Derivation of Stream Equations*.  
  https://specification.modelica.org/maint/3.6/derivation-of-stream-equations.html

### Bond graphs / port-Hamiltonian

- 20-sim, *0 and 1 junctions*.  
  https://20sim.com/webhelp/modeling_tutorial_bond_graphs_zeroandonejunctions.php
- Broenink, *Bond Graphs: A Unifying Framework for Modeling Physical Systems*.  
  https://ris.utwente.nl/ws/portalfiles/portal/250822857/Broenink_2020_Bond_graphs_a_unifying_framework_fo.pdf
- Duindam et al., *Modeling and Control of Complex Physical Systems: The Port-Hamiltonian Approach*.  
  https://ris.utwente.nl/ws/portalfiles/portal/185509417/Duindam09modeling.pdf
- *The Port-Hamiltonian Structure of Continuum Mechanics*, Journal of Nonlinear Science.  
  https://link.springer.com/article/10.1007/s00332-025-10130-1

### Simscape

- MathWorks, *Through and Across Variables*.  
  https://www.mathworks.com/help/simscape/ug/through-and-across-variables.html
- MathWorks, *Essential Physical Modeling Techniques*.  
  https://www.mathworks.com/help/simscape/ug/essential-physical-modeling-techniques.html
- MathWorks, *How Simscape Simulation Works*.  
  https://www.mathworks.com/help/simscape/ug/how-simscape-simulation-works.html

### Prior v0.1 context

- Sadjina et al., *Energy conservation and power bonds in co-simulations*.  
  https://www.sintef.no/en/publications/publication/0198cc88d6f9-e165dba7-9e72-4d1a-b51b-d3f21422620f/
- FMI 3.0.2 Specification.  
  https://fmi-standard.org/docs/3.0.2/
- preCICE Mapping configuration.  
  https://precice.org/configuration-mapping

---

## 18. Final v0.2 Statement

Within the bounded reviewed evidence, the v0.1 **Ownership-Preserving Single-Authority Energy Accounting Boundary** does not survive as a defensible independent Research Gap. Modelica stream-connection semantics, bond-graph and port-Hamiltonian power-conserving interconnection, and Simscape conserving-port semantics already establish that physically meaningful energy interaction can be represented through conservation-aware connections while subsystem state remains separate and source/storage/interconnection roles remain distinct. ThermoCore's remaining single-writer/state-authority rule is an existing RQ-ISO-001 result, while formulation incompleteness remains governed by RQ-EFM-001. The surviving value is therefore best retained as a **Conservative Exchange Accounting Property** for future engineering/conformance verification, not as a new research contribution.