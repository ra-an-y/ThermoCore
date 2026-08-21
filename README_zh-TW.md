# ThermoCore

[English](README.md) | 繁體中文

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

## 文件導覽

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

目前工作重點為證據整合、版本發布準備，以及在具有獨立依據時進行額外的範圍受限 Validation 或實作工作。

未來版本可能發布更多驗證證據與可選的參考應用；參考應用不是版本發布的必要內容。

---

## 授權

本專案採用 [Apache License 2.0](LICENSE)。
