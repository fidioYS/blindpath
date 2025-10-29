# 腳本功能清單

## 核心遊戲管理

### VRGameManager.cs
**功能**：遊戲狀態管理
```csharp
// 主要方法
ShowMainMenu()     // 顯示開始選單
StartGame()        // 開始遊戲
PauseGame()        // 暫停遊戲
ResumeGame()       // 繼續遊戲
RestartGame()      // 重新開始
QuitGame()         // 退出遊戲

// 狀態
GameState.MainMenu // 主選單狀態
GameState.Playing  // 遊戲中狀態
GameState.Paused   // 暫停狀態
```

## UI 系統

### StartMenuUI.cs
**功能**：開始選單控制
```csharp
// 按鈕功能
StartGame()        // 開始遊戲
QuitGame()         // 退出遊戲
OpenSettings()     // 打開設定

// 視覺效果
FadeInUI()         // 淡入效果
FadeOutUI()        // 淡出效果

// 音效
PlaySound()        // 播放音效
AddButtonSounds()  // 加入按鈕音效
```

### VRPauseMenu.cs
**功能**：暫停選單控制
```csharp
// 按鈕功能
ResumeGame()       // 繼續遊戲
RestartGame()      // 重新開始
OpenSettings()     // 打開設定
BackToMainMenu()   // 回到主選單
QuitGame()         // 退出遊戲

// 顯示控制
ShowPauseMenu()    // 顯示暫停選單
HidePauseMenu()    // 隱藏暫停選單
TogglePause()      // 切換暫停狀態

// 視覺效果
FadeIn()           // 淡入動畫
FadeOut()          // 淡出動畫
```

### VRPauseInput.cs
**功能**：VR 暫停輸入檢測
```csharp
// 輸入檢測
HandlePauseInput() // 處理暫停輸入
ExecutePause()     // 執行暫停/恢復

// 外部控制
ForcePause()       // 強制暫停
ForceResume()      // 強制恢復
SetInputEnabled()  // 設定輸入啟用狀態
```

### VRUISetup.cs
**功能**：VR UI 自動設定
```csharp
// 設定方法
SetupVRCanvas()    // 設定 VR Canvas
FindPlayerCamera() // 尋找玩家攝影機
PositionUI()       // 定位 UI

// 跟隨功能
FollowPlayerGaze() // 跟隨玩家視線

// 工具方法
ResetPosition()    // 重置位置
SetVisible()       // 設定可見性
```

## 互動系統

### 掛在手杖上的播放腳本.cs (CaneImpactPlayer)
**功能**：手杖敲擊音效播放
```csharp
// 碰撞檢測
OnCollisionEnter() // 物理碰撞進入
OnTriggerEnter()   // 觸發器進入

// 音效播放
PlayImpact()       // 播放撞擊音效

// 設定
useAttachedAudioSource // 使用附加音源
enableTriggerImpact    // 啟用觸發器撞擊
minTriggerImpactSpeed  // 最小觸發速度
enableDebugLogs        // 啟用偵錯日誌
```

### 掛在被敲擊物體上的設定.cs (ImpactMaterial)
**功能**：物體音效材質設定
```csharp
// 音效設定
clips[]            // 音效片段陣列
baseVolume         // 基礎音量
minPitch           // 最小音高
maxPitch           // 最大音高

// 速度設定
minImpactVelocity  // 最小撞擊速度
maxImpactVelocity  // 最大撞擊速度

// 冷卻設定
playCooldown       // 播放冷卻時間

// 主要方法
TryGetPlayableClip() // 取得可播放音效
```

## 發光系統

### GlowOnTrigger (已整合)
**功能**：物體發光效果
```csharp
// 發光設定
glowColor          // 發光顏色
intensity          // 發光強度
fadeOutDuration    // 淡出時間

// 碰撞處理
HandleEnter()      // 處理進入
HandleExit()       // 處理離開

// 視覺效果
FadeEmissionToBlack() // 淡出發光到黑色
```

## 設定參數說明

### VRGameManager 參數
```csharp
startMenuUI        // 開始選單 UI
gameplayUI         // 遊戲 UI
pauseMenuUI        // 暫停選單 UI
gameObjects[]      // 遊戲物件陣列
xrOrigin          // XR Origin 引用
```

### StartMenuUI 參數
```csharp
playButton         // 開始按鈕
quitButton         // 退出按鈕
settingsButton     // 設定按鈕
gameSceneName      // 遊戲場景名稱
buttonClickSound   // 按鈕點擊音效
buttonHoverSound   // 按鈕懸停音效
```

### VRPauseMenu 參數
```csharp
resumeButton       // 繼續按鈕
restartButton      // 重新開始按鈕
settingsButton     // 設定按鈕
mainMenuButton     // 主選單按鈕
quitButton         // 退出按鈕
pausePanel         // 暫停面板
titleText          // 標題文字
instructionText    // 說明文字
```

### VRPauseInput 參數
```csharp
leftMenuAction     // 左手選單動作
rightMenuAction    // 右手選單動作
pauseKey           // 暫停按鍵
doublePressTime    // 雙擊時間
requireDoublePress // 需要雙擊
```

### VRUISetup 參數
```csharp
distanceFromPlayer // 距離玩家距離
canvasSize         // Canvas 大小
followPlayerGaze   // 跟隨玩家視線
followSpeed        // 跟隨速度
```

### CaneImpactPlayer 參數
```csharp
useAttachedAudioSource // 使用附加音源
spatialBlend           // 空間混合
enableTriggerImpact    // 啟用觸發器撞擊
minTriggerImpactSpeed  // 最小觸發速度
enableDebugLogs        // 啟用偵錯日誌
```

### ImpactMaterial 參數
```csharp
clips[]            // 音效片段
baseVolume         // 基礎音量
minPitch           // 最小音高
maxPitch           // 最大音高
minImpactVelocity  // 最小撞擊速度
maxImpactVelocity  // 最大撞擊速度
playCooldown       // 播放冷卻
```

## 使用範例

### 開始遊戲
```csharp
var gameManager = FindObjectOfType<VRGameManager>();
gameManager.StartGame();
```

### 暫停遊戲
```csharp
var pauseInput = FindObjectOfType<VRPauseInput>();
pauseInput.ForcePause();
```

### 播放音效
```csharp
var impactPlayer = FindObjectOfType<CaneImpactPlayer>();
// 音效會自動播放
```

### 控制 UI 顯示
```csharp
var pauseMenu = FindObjectOfType<VRPauseMenu>();
pauseMenu.ShowPauseMenu();
```

## 注意事項

### 相依性
- VRGameManager 需要 StartMenuUI 和 VRPauseMenu
- VRPauseInput 需要 VRGameManager
- CaneImpactPlayer 需要 ImpactMaterial
- 所有 UI 腳本需要 VRUISetup

### 設定順序
1. 先設定 VRGameManager
2. 再設定 UI 腳本
3. 最後設定互動腳本

### 常見錯誤
- 忘記設定 UI 引用
- 輸入動作引用錯誤
- Canvas 未設為 World Space
- 音效檔案路徑錯誤









