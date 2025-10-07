# VR 專案快速設定指南

## 5 分鐘快速檢查清單

### 1. VR 基本設定 ✅
```
1. 確認 XR Origin (XR Rig) 在場景中
2. 檢查 Input Action Manager 已加入並啟用
3. 確認 XRI Default Input Actions 已載入
4. 測試 Continuous Turn（右手搖桿左右）
```

### 2. 開始選單設定 ✅
```
1. Start Menu Canvas 存在且啟用
2. 掛有 StartMenuUI 腳本
3. 按鈕引用正確設定
4. Canvas 設為 World Space
```

### 3. 暫停選單設定 ✅
```
1. Pause Menu Canvas 存在
2. 掛有 VRPauseMenu 腳本
3. 掛有 VRPauseInput 腳本
4. Menu 按鈕輸入動作已設定
```

### 4. 遊戲管理設定 ✅
```
1. GameManager 物件存在
2. 掛有 VRGameManager 腳本
3. UI 引用正確設定
4. 遊戲物件引用設定
```

### 5. 手杖敲擊系統 ✅
```
1. 手杖物件掛有 CaneImpactPlayer 腳本
2. 被敲物體掛有 ImpactMaterial 腳本
3. 音效檔案已指定
4. 發光效果正常
```

## 快速測試步驟

### 測試 VR 移動
1. 進入 Play 模式
2. 用右手搖桿左右推動
3. 應該可以平滑轉向

### 測試開始選單
1. 看到開始選單 UI
2. 用控制器射線點擊「開始遊戲」
3. 選單應該隱藏，遊戲開始

### 測試暫停功能
1. 在遊戲中按控制器 Menu 按鈕
2. 暫停選單應該出現
3. 點擊「繼續遊戲」應該恢復

### 測試手杖敲擊
1. 抓取手杖
2. 敲擊不同物體
3. 應該有不同音效和發光效果

## 常見問題快速修復

### 轉向不工作
```csharp
// 檢查 Input Action Manager
// 確認 Action Assets 包含 XRI Default Input Actions
// 確認 Continuous Turn Provider 啟用
```

### UI 無法點擊
```csharp
// 確認 Canvas Render Mode = World Space
// 確認有 GraphicRaycaster 元件
// 確認 EventSystem 存在且啟用 XR Input
```

### 音效不播放
```csharp
// 確認 AudioSource 設定正確
// 確認音效檔案已指定
// 確認音量 > 0
```

### 發光效果異常
```csharp
// 確認物體有 Renderer 元件
// 確認材質支援 Emission
// 確認 GlowOnTrigger 腳本正確掛載
```

## 檔案結構檢查

```
Assets/
├── Script/
│   ├── VRGameManager.cs
│   ├── StartMenuUI.cs
│   ├── VRPauseMenu.cs
│   ├── VRPauseInput.cs
│   ├── VRUISetup.cs
│   ├── 掛在手杖上的播放腳本.cs
│   └── 掛在被敲擊物體上的設定.cs
├── Shader/
│   ├── sca.shadergraph
│   └── New Shader Graph.shadergraph
└── Documentation/
    ├── VR_Project_Documentation.md
    └── Quick_Setup_Guide.md
```

## 緊急修復命令

### 重新設定 Input Action Manager
1. 刪除現有 Input Action Manager
2. 重新建立並加入 XRI Default Input Actions
3. 確認 Action Assets 啟用

### 重新設定 UI
1. 確認所有 Canvas 設為 World Space
2. 確認有 GraphicRaycaster
3. 確認 EventSystem 啟用 XR Input

### 重新設定 VR Origin
1. 確認 XR Origin 有正確的 Locomotion System
2. 確認 Continuous Turn Provider 啟用
3. 確認 Turn Action 指向正確輸入動作

## 備份建議

### 重要檔案
- 場景檔案 (.unity)
- 腳本檔案 (.cs)
- 設定檔案 (.asset)
- Shader 檔案 (.shadergraph)

### 版本控制
```bash
git add .
git commit -m "VR project setup complete"
git push
```

## 聯絡支援

如果遇到問題：
1. 檢查此快速指南
2. 查看完整專案文件
3. 重新建立對話並提供錯誤訊息
4. 提供 Unity Console 錯誤日誌




