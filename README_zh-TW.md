# ThermoCore

[English](README.md) | 繁體中文

[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.22053832.svg)](https://doi.org/10.5281/zenodo.22053832)
[![Release](https://img.shields.io/badge/release-v1.0.0-blue)](https://github.com/ra-an-y/ThermoCore/releases/tag/v1.0.0)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

ThermoCore 是一套用於即時熱力學模擬、與引擎無關的框架。

本框架將熱力學計算與材質表現解耦，使熱力學狀態計算得以重複使用，同時讓材質模型保持獨立，不與模擬核心綁定。

---

## 功能特色

- 解耦式熱力學計算層
- 材質表現層
- 基於焓的狀態更新
- GPU 導向設計
- 與引擎無關的架構

---

## 架構

```text
能量輸入
   │
   ▼
熱力學狀態更新
   │
   ▼
材質表現
```

---

## 擴充 ThermoCore

新增物理機制時，不應只依機制名稱、所屬物理領域或耦合強度判斷它是否能成為 Extension。ThermoCore 會先判斷選定的熱力學公式在語意誠實的通訊下是否仍然完整，再判斷已接受的普通 Extension 是否能維持 Core-State 的權威語意與所有權。

請參閱 [Extension Design Guide](Documentation/Extension_Design_Guide.md)。該指南以實際決策流程說明 Extension admissibility、狀態與資訊分類、能量交換 accounting、下游 feedback，以及多 Extension 組合後的重新判定方式。

---

## 文件導覽

- [Extension Design Guide](Documentation/Extension_Design_Guide.md)
- [規格索引](Documentation/Specification_Index.md)
- [框架詞彙表](Documentation/Framework_Vocabulary.md)
- [倉庫治理](Documentation/Repository_Guidelines/Repository_Governance.md)
- [研究導覽](Research/README.md)
- [驗證證據](Validation/README.md)
- [效能評估](Performance/README.md)

---

## 驗證證據

ThermoCore 目前已發布兩條彼此獨立、範圍受限的參考公式 caloric Validation 路線。

| 驗證路線 | 外部依據 | 倉庫已發布結果 |
|---|---|---|
| [H2O caloric benchmark](Validation/Reference_Formulation_Caloric_Validation_v0.1.md) | IAPWS 參考公式 | `COMPLETED — errors reported` |
| [Gallium caloric benchmark](Validation/Reference_Formulation_Gallium_Caloric_Validation_v0.1.md) | NIST Chemistry WebBook SRD 69 / NIST-JANAF | `COMPLETED — errors reported` |

這些紀錄保存相對於已宣告外部參考基準的量測誤差。兩條路線皆未採用物理 PASS/FAIL 門檻，也不代表完整 Framework Validation 或 Framework Conformance。

---

## 倉庫狀態

框架規格與倉庫治理基線已建立。

一套範圍受限的熱力學參考公式已完成實作與 Verification；目前亦已發布兩條獨立 caloric Validation 路線，並於 `Performance/` 保存範圍受限的 CPU Performance Evaluation 紀錄。

ThermoCore v1.0.0 是第一個穩定公開的倉庫發布基線。目前工作重點為 v1.0 之後的研究、證據整合，以及在具有獨立依據時進行額外的範圍受限 Validation 或實作工作。

未來版本可能發布更多驗證證據與可選的參考應用；參考應用不是版本發布的必要內容。

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
