# VR 專案文件

## 專案概述
這是一個 VR 互動專案，使用 Unity XR Interaction Toolkit，包含手杖敲擊、物體發光、音效系統等功能。

## 專案結構

### 核心腳本
- `VRGameManager.cs` - 遊戲狀態管理
- `StartMenuUI.cs` - 開始選單 UI 控制
- `VRPauseMenu.cs` - 暫停選單 UI 控制
- `VRPauseInput.cs` - VR 控制器暫停輸入檢測
- `VRUISetup.cs` - VR UI 自動設定

### 互動系統
- `掛在手杖上的播放腳本.cs` - 手杖敲擊音效系統
- `掛在被敲擊物體上的設定.cs` - 物體音效材質設定
- `vrm.cs` - 物體發光系統（已刪除，功能整合到其他腳本）

### Shader
- `Assets/Shader/sca.shadergraph` - 自訂 Shader Graph
- `Assets/Shader/New Shader Graph.shadergraph` - 另一個 Shader Graph

## VR 設定

### XR Origin 配置
- 使用 XR Interaction Toolkit
- 已配置 Input Action Manager
- Continuous Turn 已啟用
- Tunneling Vignette 已關閉

### 輸入系統
- 使用 XRI Default Input Actions
- 支援 Continuous Turn（右手搖桿）
- 支援抓取和互動

## UI 系統

### 開始選單
- World Space Canvas
- 包含：開始遊戲、設定、退出按鈕
- 支援 VR 射線互動
- 音效支援

### 暫停選單
- 包含：繼續、重新開始、設定、主選單、退出
- 背景遮罩效果
- 淡入淡出動畫
- 控制器 Menu 按鈕觸發

## 遊戲功能

### 手杖敲擊系統
- 支援 Trigger 和 Collision 檢測
- 不同物體不同音效
- 速度影響音量
- 發光效果（碰到即亮，離開漸暗）

### 音效系統
- 支援多種音效格式
- 3D 空間音效
- 音量隨撞擊強度調整

## 設定檢查清單

### VR 基本設定
- [ ] XR Origin 已配置
- [ ] Input Action Manager 已啟用
- [ ] Continuous Turn 正常工作
- [ ] 控制器追蹤正常

### UI 設定
- [ ] 開始選單 Canvas 設為 World Space
- [ ] 暫停選單 Canvas 設為 World Space
- [ ] 所有按鈕都可正常互動
- [ ] VR 射線可點擊 UI

### 音效設定
- [ ] 手杖敲擊有聲音
- [ ] 不同物體音效不同
- [ ] 音量調整正常
- [ ] 3D 音效定位正確

### 發光效果
- [ ] 物體碰到會發光
- [ ] 離開時漸暗效果
- [ ] 多重接觸不會閃爍

## 常見問題

### Continuous Turn 不工作
1. 檢查 Input Action Manager 是否啟用
2. 確認 XRI Default Input Actions 已載入
3. 檢查 Continuous Turn Provider 是否啟用
4. 確認右手搖桿輸入動作正確

### UI 無法互動
1. 確認 Canvas 設為 World Space
2. 檢查 GraphicRaycaster 是否存在
3. 確認 XR UI Input Module 已配置
4. 檢查 EventSystem 設定

### 音效不播放
1. 確認 AudioSource 設定正確
2. 檢查音效檔案是否載入
3. 確認音量設定
4. 檢查 3D 音效設定

## 開發注意事項

### VR 最佳實踐
- UI 距離玩家 2-3 公尺
- 按鈕大小適合 VR 互動
- 避免快速移動造成不適
- 提供清晰的視覺回饋

### 效能優化
- 使用 LOD 系統
- 優化材質和貼圖
- 控制同時播放的音效數量
- 適當使用物件池

## 版本資訊
- Unity 版本：2022.3 LTS 或更新
- XR Interaction Toolkit：3.2.1
- 目標平台：PC VR (OpenXR)

## 聯絡資訊
如有問題，請參考此文件或重新建立對話。




