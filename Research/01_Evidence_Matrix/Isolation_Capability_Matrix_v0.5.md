# Isolation Capability Matrix v0.5 — Direct-Equivalence Falsification Extension

Status: Under Survey  
Research Question: RQ-ISO-001  
Date: 2026-08-22  
Dependency: `Isolation_Capability_Matrix_v0.4.md`

---

## 1. Purpose of This Revision

This revision continues the falsification-oriented search requested by v0.4, with emphasis on architectures that explicitly separate:

- information production from information consumption;
- update privilege from observation;
- data semantics from runtime producers; and
- extensibility from central communication infrastructure.

The new comparison targets are:

1. IEEE High Level Architecture (HLA), including current HLA 4 / IEEE 1516-2025 material where publicly available;
2. OMG Data Distribution Service (DDS) exclusive ownership; and
3. Azure IoT Plug and Play / DTDL read-only and writable property conventions, with Azure Digital Twins checked separately where enforcement semantics differ.

The purpose is to test the surviving v0.4 hypothesis of `Restricted State-Authority Isolation` rather than to accumulate generic modularity evidence.

This revision does not establish novelty or a Research Gap.

---

## 2. Focused Capability Comparison

The capability definitions C1–C12 remain unchanged from v0.4.

| Capability | HLA | DDS | IoT Plug and Play / DTDL | ThermoCore | Revised Interpretation |
|---|---:|---:|---:|---:|---|
| C1 Authoritative State | P | P | P | Y | HLA provides exclusive ownership per object-instance attribute rather than one domain-wide state. DDS provides exclusive ownership per data instance. IoT Plug and Play distinguishes reported and desired property state but is not a thermodynamic state architecture. |
| C2 Explicit State Owner | Y | Y | P | Y | HLA explicitly assigns each owned instance attribute to at most one federate at a time. DDS exclusive ownership selects one DataWriter per instance. IoT Plug and Play assigns reporting responsibility to the device for read-only properties, but not a general architectural state owner. |
| C3 Read Does Not Confer Ownership | Y | Y | Y | Y | HLA subscription/reflection does not itself confer attribute ownership. DDS DataReaders do not become the exclusive DataWriter by consuming data. IoT Plug and Play read-only properties are reported by the device rather than set by the back end. |
| C4 Representation Cannot Redefine State | P | N/A | P | Y | HLA can support pure observing federates, but the architecture does not impose a permanent role-level prohibition on a federate acquiring update ownership where the object model permits it. DDS has no representation-role concept. IoT Plug and Play separates read-only and writable properties, but this is property-access semantics rather than a simulation Representation boundary. |
| C5 Extension-owned State | Y | P | Y | Y | HLA federates and IoT components may maintain local/internal state. DDS applications may also maintain local state, but DDS itself does not define extension-local simulation state semantics. |
| C6 Core Complete Without Extensions | P | Y | Y | Y | DDS and DTDL tooling do not require a particular optional application component. HLA supports modular and extensible federations, but federation completeness is defined by the chosen federation/object model rather than by a fixed domain core whose completeness is invariant under all ordinary extensions. |
| C7 Interface-governed Communication | Y | Y | Y | Y | HLA requires FOM data exchange through RTI services. DDS uses standardized DataWriter/DataReader communication and QoS. IoT Plug and Play uses declared DTDL capabilities and twin conventions. |
| C8 Ownership Preservation Across Communication | Y | Y | P | Y | HLA is a strong prior-art example: reflecting/subscribing does not transfer attribute ownership, and ownership transfer uses separate Ownership Management services. DDS exclusive ownership similarly separates receiving data from owning the writable instance. IoT Plug and Play preserves the device/back-end distinction for read-only properties, but Azure Digital Twins itself does not enforce DTDL `writable`. |
| C9 State Growth Control | P | N/A | P | Y | HLA FOM modules, including HLA 4 extensions, can extend the shared federation object model. DDS does not define a simulation-core state boundary. DTDL models can be extended with additional properties/components. None of these sources establishes ThermoCore's candidate prohibition against ordinary extensions automatically expanding mandatory Core State. |
| C10 Validation Boundary | P | N/A | U | Y | HLA has a mature federation engineering and conformance ecosystem, but the current evidence does not show a validation-scope rule equivalent to ThermoCore's candidate state-authority boundary. DDS ownership is a communication QoS rule, not a validation architecture. |
| C11 Core Change Isolation | Y | Y | Y | Y | HLA federates/FOM modules, DDS publishers/subscribers, and DTDL interfaces are all designed for extensible composition. Generic core-change isolation is established prior art. |
| C12 Normative Governance Constraints | Y | Y | Y | Y | HLA and DDS are formal standards; DTDL/IoT Plug and Play conventions also define explicit role and property rules. Normative ownership/access constraints are not distinctive by themselves. |

A `Y` does not imply semantic equivalence with ThermoCore.

---

## 3. New Evidence Records

### ISO-E11 — HLA Attribute Ownership, Reflection, and Object-Model Semantics

**Evidence status:** Verified for exclusive attribute ownership, update/reflection separation, interface-governed exchange, dynamic ownership transfer, and object-model semantics. Under Survey for equivalence to ThermoCore semantic authority and core-completeness rules.

IEEE 1516-2025 remains the active HLA Framework and Rules standard and defines responsibilities of HLA federates and federations. The HLA family provides an RTI interface for coordinated information exchange among interacting simulations.

Public HLA material and implementations consistently preserve the long-standing ownership model:

- an object-instance attribute is owned by at most one federate at a time;
- the owner has the responsibility/privilege to update that attribute;
- subscribing federates receive reflected attribute values without thereby becoming owners;
- ownership may be transferred dynamically through dedicated Ownership Management services; and
- FOM/SOM definitions document exchanged object classes, attributes, datatypes, update policies, and semantics.

This is a strong counterexample to any ThermoCore claim based merely on:

- having one writer/owner for shared state information;
- separating producer/update privilege from read/subscription;
- preserving ownership across ordinary communication; or
- using a normative information model plus governed interfaces.

HLA therefore materially narrows the surviving ThermoCore hypothesis.

However, the current evidence also identifies a boundary difference. HLA ownership is **update ownership of object-instance attributes**, and that ownership is intentionally transferable. HLA object models can also be extended; HLA Evolved introduced modular FOMs, and HLA 4 extends FOM/SOM merging capabilities. HLA therefore does not currently establish a general rule that an ordinary extension or representation role may never acquire semantic authority over a fixed domain state or expand the mandatory shared state model.

**Supported capabilities:** C2 yes, C3 yes, C7 yes, C8 yes, C11 yes, C12 yes; C1/C4/C6/C9/C10 partial.

**Primary / high-authority sources:**

- IEEE 1516-2025 Framework and Rules overview: https://standards.ieee.org/ieee/1516/6687
- IEEE 1516.1-2025 Federate Interface Specification overview: https://standards.ieee.org/ieee/1516.1/6688/
- IEEE 1516.2-2025 Object Model Template overview: https://standards.ieee.org/ieee/1516.2/6689/
- MAK RTI current HLA 1.3 / 1516-2000 / 1516-2010 / 1516-2025 capability description: https://www.mak.com/mak-one/tools/mak-rti/capabilities
- HLA 4 introduction tutorial, Ownership Management: https://antelius.github.io/papers/Tutorial_24T53_Introduction_to_HLA4.pdf
- Public HLA ownership-management synopsis (historical HLA rule/interface material): https://www.cs.cmu.edu/afs/cs/academic/class/15413-s99/www/hla/doc/rti_synopsis/09-Ownership_Management/Ownership_Management.html
- Public HLA framework rules synopsis: https://www.cs.cmu.edu/afs/cs.cmu.edu/academic/class/15413-s99/www/hla/doc/rti_synopsis/01-Introduction_to_HLA/Introduction_to_HLA.html
- Public HLA OMT semantics / lexicon material: https://www.cs.cmu.edu/afs/cs/academic/class/15413-s99/www/hla/doc/omt/components/omt_FSlexicon.html

**Interpretation:**

HLA falsifies the idea that exclusive update authority plus read-only observation is itself a distinctive architectural property. The remaining ThermoCore candidate must be about **non-transferable semantic authority and fixed Core-State/Core-completeness boundaries**, not about single-writer ownership.

---

### ISO-E12 — DDS Exclusive Ownership and Publisher/Subscriber Decoupling

**Evidence status:** Verified for exclusive per-instance DataWriter ownership and DataReader consumption without ownership transfer.

OMG DDS defines standardized data-centric publish/subscribe communication. Its OWNERSHIP QoS may be `SHARED` or `EXCLUSIVE`.

Under exclusive ownership:

- one DataWriter is selected as owner of a data instance at a time;
- only the owner's modifications are visible to matching DataReaders;
- DataReaders may consume the instance without becoming its owner; and
- owner selection can change according to ownership strength, liveliness, and related QoS conditions.

The DDS specification and current RTI documentation explicitly describe this as a mechanism for decoupling publishers and subscribers while controlling which writer supplies the visible state of an instance.

This is direct prior art for communication-level ownership preservation and single-writer arbitration. It further eliminates any broad claim that ThermoCore is distinctive because consumers cannot obtain write authority merely by receiving data.

DDS is not semantically equivalent to ThermoCore because it does not define:

- a Thermodynamic State information category;
- a framework-level owner of thermodynamic evolution;
- a Representation role;
- extension-local versus Core State; or
- Core completeness/conformance under optional mechanisms.

**Supported capabilities:** C2 yes, C3 yes, C7 yes, C8 yes, C11 yes, C12 yes; C1 partial.

**Primary / high-authority sources:**

- OMG Data Distribution Service 1.4 specification index and normative document: https://www.omg.org/spec/DDS/
- OMG DDS issue record documenting OWNERSHIP on Topic/DataReader/DataWriter: https://issues.omg.org/issues/DDS11-72
- RTI Connext current OWNERSHIP QoS reference: https://community.rti.com/static/documentation/connext-dds/current/doc/api/connext_dds/api_c/group__DDSOwnershipQosModule.html
- RTI Connext current OWNERSHIP policy behavior: https://community.rti.com/static/documentation/connext_dds_professional/users_manual/users_manual/OWNERSHIP_QosPolicy.htm

**Interpretation:**

DDS confirms that update ownership can be preserved independently from data consumption in a mature standardized architecture. ThermoCore's surviving research question must therefore concern semantic and compositional authority, not generic publisher/subscriber ownership.

---

### ISO-E13 — IoT Plug and Play / DTDL Reported-vs-Writable State Roles

**Evidence status:** Verified for device-reported read-only properties and back-end-set writable properties in IoT Plug and Play conventions. Verified that Azure Digital Twins does not enforce DTDL `writable` in the same way.

IoT Plug and Play uses DTDL models to define device capabilities, including properties representing device/entity state.

Its conventions distinguish:

- **read-only properties**, whose values are set/reported by the device to the back end; and
- **writable properties**, which a back-end application may set and send to the device, with device acknowledgment and reporting of the actual resulting value.

This is relevant prior art because it separates state-reporting authority from consuming/controlling applications at the property level. A cloud application does not directly become the reporter of a read-only device property merely because it observes that property.

The comparison also reveals an important implementation-specific caveat: Azure Digital Twins accepts DTDL models but currently does not enforce the `writable` attribute; clients with general write permission can write twin properties. Therefore DTDL modeling semantics and service enforcement must not be conflated.

**Supported capabilities:** C3 yes, C5 yes, C7 yes, C11 yes, C12 yes; C1/C2/C4/C8/C9 partial.

**Primary sources:**

- Azure IoT Plug and Play conventions — read-only and writable properties: https://learn.microsoft.com/en-us/azure/iot/concepts-convention
- Azure IoT device development / DTDL capability model: https://learn.microsoft.com/en-us/azure/iot/iot-overview-device-development
- Azure Digital Twins DTDL model semantics and service-specific `writable` limitation: https://learn.microsoft.com/en-us/azure/digital-twins/concepts-models

**Interpretation:**

Role-specific control over state reporting is established prior art in digital-twin/IoT systems. ThermoCore cannot rely on a broad `producer owns state, consumer only reads` claim. The remaining candidate must include the stronger prohibition against ordinary extensions or representations changing the semantic definition and Core membership of authoritative Thermodynamic State.

---

## 4. Revised Findings

### F-ISO-15 — Exclusive update authority is established simulation prior art

HLA explicitly separates attribute ownership from subscription/reflection and permits only the current owner to provide authoritative updates for an owned instance attribute.

ThermoCore shall not claim novelty for single-writer or owner-based state update discipline.

### F-ISO-16 — Ownership preservation across communication is established prior art

HLA and DDS both show that receiving information does not inherently transfer update ownership. HLA requires dedicated ownership-management operations for ownership transfer; DDS exclusive ownership similarly preserves one selected writer while many readers may consume.

ThermoCore shall not claim novelty for the statement that `Read` or `Consume` does not itself confer write ownership.

### F-ISO-17 — Producer-vs-consumer state roles are established digital-twin prior art

IoT Plug and Play distinguishes device-reported read-only state from properties writable by a back-end application.

ThermoCore shall not claim novelty merely because one role reports/evolves a value while another role observes or interprets it.

### F-ISO-18 — The surviving candidate is semantic/compositional, not access-control ownership

After HLA, DDS, and IoT Plug and Play, the remaining ThermoCore hypothesis is narrower than `Restricted State-Authority Isolation` initially suggested.

The still-unfalsified combination is approximately:

> A Framework defines one authoritative Thermodynamic State information category and assigns its evolution responsibility to Thermodynamic Computation; ordinary Representation Consumers and Extension Modules may consume that State or contribute through declared boundaries, but their participation cannot by itself transfer authority to redefine the State's semantics, owner, mandatory Core membership, or the completeness of the Framework Core.

The strongest potentially distinctive terms are therefore:

1. **semantic authority** rather than update privilege;
2. **non-promotion of extension-local state into mandatory Core State**;
3. **Core completeness invariant under absence of ordinary extensions**; and
4. **role restrictions that remain true even when an implementation's communication mechanism would technically permit broader writes.**

This remains Under Survey.

---

## 5. Updated Falsification Conditions

The remaining candidate shall be rejected, narrowed, or reclassified as prior-art integration if an existing architecture is found that jointly and explicitly requires all of the following:

1. a shared physical-domain state whose semantics are normatively defined independently of any optional representation/extension;
2. one architectural responsibility for evolving that state;
3. consumers/representations that may observe or derive from the state without gaining update or semantic authority;
4. optional mechanisms that may own persistent mechanism-specific state;
5. a prohibition against ordinary optional mechanisms promoting their local state into mandatory Core State;
6. a prohibition against ordinary optional mechanisms redefining the semantic identity or owner of the authoritative state;
7. Core completeness/conformance that remains valid when every ordinary optional mechanism is absent; and
8. a bounded change/validation impact that follows from the above boundaries rather than merely from folder/module separation.

The following are now explicitly insufficient to establish equivalence on their own:

- single-writer or exclusive ownership;
- publish/subscribe decoupling;
- read-only vs writable properties;
- dynamic ownership transfer;
- a central object/schema definition;
- plugin or modular-FOM extensibility;
- module-specific V&V; or
- interface-level access control.

---

## 6. Current Classification

| Item | Classification |
|---|---|
| Single-writer / exclusive update ownership | Verified prior art |
| Read/subscribe does not confer update ownership | Verified prior art |
| Explicit ownership transfer mechanism | Verified prior art |
| Governed shared schema / object-model semantics | Verified prior art |
| Device-reported vs back-end-writable state roles | Verified prior art |
| Interface-governed publish/subscribe communication | Verified prior art |
| Modular extension of shared simulation/object models | Verified prior art |
| Representation non-authority over Thermodynamic State semantics | Under Survey |
| Non-promotion of extension-local state into mandatory Core State | Under Survey |
| Core completeness invariant under ordinary extension absence | Under Survey |
| Reduced mandatory state growth caused by these rules | Unverified hypothesis |
| Reduced revalidation impact caused by these rules | Unverified hypothesis |
| Research Gap | Not established |

---

## 7. Consequence for RQ-ISO-001

HLA provides the strongest falsification pressure found so far because it already combines:

- normative simulation architecture;
- semantics documentation;
- publish/subscribe communication;
- exclusive per-attribute ownership;
- read/reflection without ownership transfer; and
- modular extensibility.

Therefore the RQ-ISO-001 candidate shall no longer be framed as a broad `state authority` or `ownership isolation` contribution.

The research should continue only around the stricter question:

> **Does prior art explicitly preserve a fixed semantic/Core boundary such that ordinary extensions and representations can use authoritative physical state but are structurally unable to redefine its semantic identity, owner, mandatory state membership, or Core completeness?**

If direct prior art is found for that stronger combination, the candidate Research Gap should be rejected or reframed as integration/formalization.

If direct prior art is not found after bounded search, the next justified step is an evidence-backed Research Gap candidate plus experiments measuring state-growth and revalidation impact.

---

## 8. Next Search Boundary

The next bounded search should prioritize architectures with stronger semantic-governance claims than generic simulation middleware:

1. digital-twin reference architectures and standards that distinguish authoritative physical state from derived/representation state;
2. safety-critical component frameworks that separate owned state from observers while preserving fixed platform semantics;
3. simulation standards with explicit immutable/shared state schemas plus optional participant-local state; and
4. architecture-description or model-based frameworks that normatively prohibit extensions from redefining core information semantics.

The search should stop expanding breadth if new candidates only repeat already-falsified properties such as single-writer ownership, publish/subscribe isolation, or plugin modularity.
