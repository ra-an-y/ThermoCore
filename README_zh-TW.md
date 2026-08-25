# ThermoCore

[English](README.md) | 繁體中文

[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.22053832.svg)](https://doi.org/10.5281/zenodo.22053832)
[![Release](https://img.shields.io/badge/release-v1.0.0-blue)](https://github.com/ra-an-y/ThermoCore/releases/tag/v1.0.0)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

ThermoCore 是一套為即時熱力學模擬而設計、與引擎無關的框架。

本框架將 Thermodynamic Computation 與 Material Representation 分離，使 Thermodynamic State 的計算可重複使用；計算所需的材質資訊則以獨立的 Configuration 提供，而不是把材質特定邏輯嵌入 Thermodynamic Computation。

本倉庫包含 normative Framework Specification、範圍受限的 C# reference implementation、Verification 與 Validation evidence、Performance 紀錄，以及相關研究資料。

---

## Framework 設計

- Thermodynamic Computation 與 Material Representation 分離
- 明確的 Thermodynamic State 所有權
- 與引擎無關的架構
- GPU 導向的架構設計
- 明確的 Extension 邊界

### 目前的 Reference Implementation

- 與後端無關的 C# reference implementation
- 基於焓的 reference formulation
- 範圍受限的 Thermodynamic Computation 與 Thermodynamic State 實作切片
- Material Definition 到 compiled Configuration 的供應路徑

目前的 reference implementation 刻意維持在範圍受限的實作切片；它尚未建立四個 normative Core responsibilities 的完整實作，也尚未建立完整 Framework Conformance。

---

## 概念性 Runtime Flow

```text
Energy Input
      │
      ▼
Thermodynamic Computation
      │
      ▼
Thermodynamic State
      │
      ▼
Material Representation
      │
      ▼
Representation Consumer
（位於 Framework Core 之外）
```

此簡化圖描述的是概念性 runtime dependency。這些 dependency 之間的通訊透過適用的 Framework Interfaces 進行。ThermoCore 的 normative Core Architecture 由 Thermodynamic Computation、Thermodynamic State、Material Representation 與 Framework Interfaces 四項責任組成。

Thermodynamic State 是 Framework 的主要輸出。Material Representation 會解讀 Thermodynamic State 與適用的材質資訊，再提供給位於 Framework Core 之外的 Representation Consumer 使用。

正式架構定義請參閱 [Core Architecture specification](Documentation/Framework_Specification/Core_Architecture.md)；目前實作範圍請參閱 [Implementation Conformance Audit](Documentation/Implementation_Conformance_Audit_v0.1.md)。

---

## 擴充 ThermoCore

新增物理機制時，不應只依機制名稱、所屬物理領域或耦合強度判斷它是否能成為 Extension。ThermoCore 會先判斷：在與 Core 進行語意誠實且符合既定邊界的通訊時，選定的熱力學公式是否仍然完整；若可作為普通 Extension，再判斷它是否維持 Core-State 的權威語意與所有權。

請參閱 [Extension Design Guide](Documentation/Extension_Design_Guide.md)。該指南以實際決策流程說明 Extension admissibility、狀態與資訊分類、能量交換 accounting、下游 feedback，以及多 Extension 組合後的重新判定方式。

---

## 文件導覽

- [規格索引](Documentation/Specification_Index.md) — normative specifications 的閱讀順序與關係
- [Core Architecture](Documentation/Framework_Specification/Core_Architecture.md) — Core responsibilities 與邊界的正式定義
- [Extension Design Guide](Documentation/Extension_Design_Guide.md) — Extension 的實務判定流程
- [框架詞彙表](Documentation/Framework_Vocabulary.md)
- [Implementation Conformance Audit](Documentation/Implementation_Conformance_Audit_v0.1.md)
- [倉庫治理](Documentation/Repository_Guidelines/Repository_Governance.md)
- [研究導覽](Research/README.md)
- [驗證證據](Validation/README.md)
- [效能評估](Performance/README.md)

---

## 驗證證據

ThermoCore 目前已發布兩條不同、範圍受限的 reference formulation caloric Validation 路線。

| 驗證路線 | 外部依據 | 倉庫已發布結果 |
|---|---|---|
| [H2O caloric benchmark](Validation/Reference_Formulation_Caloric_Validation_v0.1.md) | IAPWS 參考公式 | `COMPLETED — errors reported` |
| [Gallium caloric benchmark](Validation/Reference_Formulation_Gallium_Caloric_Validation_v0.1.md) | NIST Chemistry WebBook SRD 69 / NIST-JANAF | `COMPLETED — errors reported` |

此處的 `errors reported` 指相對於已宣告外部參考基準所量測的偏差，而不是程式執行失敗。兩條路線皆未採用物理 PASS/FAIL 門檻，也不代表完整 Framework Validation 或 Framework Conformance。

---

## 倉庫狀態

Framework Specification 與倉庫治理基線已建立。

一套範圍受限的熱力學 reference formulation 已完成實作，並在其宣告範圍內具有 Verification evidence。目前亦已發布兩條不同的 caloric Validation 路線，並於 `Performance/` 保存範圍受限的 CPU Performance Evaluation 紀錄。

ThermoCore v1.0.0 是第一個穩定公開的倉庫發布基線。目前工作重點為 v1.0 之後的研究、證據整合，以及在具有獨立依據時進行額外的範圍受限 Validation 或實作工作。

未來版本可能發布更多 Validation Evidence 與可選的 Reference Applications；Reference Applications 不是版本發布的必要內容。

---

## 引用

ThermoCore v1.0.0 已作為第一個穩定的倉庫發布基線封存於 Zenodo。

- 版本：`v1.0.0`
- DOI：[10.5281/zenodo.22053832](https://doi.org/10.5281/zenodo.22053832)
- GitHub Release：[ThermoCore v1.0.0](https://github.com/ra-an-y/ThermoCore/releases/tag/v1.0.0)

引用此軟體版本時，請使用 Zenodo 紀錄所提供的 citation metadata。此 DOI 對應已封存的 v1.0.0；之後 `main` 的持續開發不會改變該封存基線。

---

## 授權

本專案採用 [Apache License 2.0](LICENSE)。
