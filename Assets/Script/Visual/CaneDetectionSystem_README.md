# 手杖精確接觸檢測系統使用說明

## 概述
這是一個完整的手杖精確接觸檢測系統，實現了"手杖接觸物件樓梯邊緣時能顯現出這階被碰到邊緣外框的效果，不是整個碰到就亮，而是有碰到的點才有亮"的需求。

## 系統組件

### 1. CanePreciseDetector.cs
**主要功能：**
- 精確檢測手杖接觸點
- 識別樓梯邊緣
- 管理接觸點數據
- 觸發局部外框顯示

### 2. LocalOutlineManager.cs
**主要功能：**
- 管理局部外框顯示
- 外框物件池管理
- 動畫效果控制
- 性能優化

### 3. CaneDetectionTester.cs
**主要功能：**
- 測試系統功能
- 模擬手杖移動
- 兼容性檢查
- 調試工具

## 快速開始

### 1. 基本設定
```csharp
// 將 CanePreciseDetector 掛到你的手杖物件上
CanePreciseDetector detector = gameObject.AddComponent<CanePreciseDetector>();

// 設定手杖尖端
detector.caneTip = yourCaneTipTransform;

// 設定檢測參數
detector.caneTipRadius = 0.02f;        // 手杖尖端半徑
detector.detectionPrecision = 0.005f;  // 檢測精度
detector.edgeDetectionDistance = 0.3f; // 邊緣檢測距離
```

### 2. 外框管理器設定
```csharp
// 將 LocalOutlineManager 掛到同一個物件上
LocalOutlineManager outlineManager = gameObject.AddComponent<LocalOutlineManager>();

// 設定外框參數
outlineManager.outlineColor = Color.yellow;     // 外框顏色
outlineManager.outlineIntensity = 2f;          // 外框強度
outlineManager.outlineSize = 0.2f;             // 外框大小
outlineManager.outlineDuration = 3f;           // 外框持續時間
```

### 3. 樓梯物件設定
```csharp
// 為樓梯物件設定標籤
stairObject.tag = "Stair";

// 或者使用包含特定名稱的物件
// 物件名稱包含 "Stair"、"Step" 等關鍵字
```

## 詳細參數說明

### CanePreciseDetector 參數
- **caneTip**: 手杖尖端 Transform
- **caneTipRadius**: 手杖尖端半徑 (0.01-0.1)
- **detectionPrecision**: 檢測精度 (0.001-0.01)
- **edgeDetectionDistance**: 邊緣檢測距離 (0.1-1)
- **edgeAngleThreshold**: 邊緣角度閾值 (0-90度)
- **detectableLayers**: 可檢測的圖層
- **outlineColor**: 外框顏色
- **outlineIntensity**: 外框強度 (0.1-5)
- **outlineDuration**: 外框持續時間 (0.5-10)
- **outlineSize**: 外框大小 (0.1-2)

### LocalOutlineManager 參數
- **outlineMaterial**: 外框材質
- **outlineColor**: 外框顏色
- **outlineIntensity**: 外框強度 (0.1-5)
- **outlineSize**: 外框大小 (0.05-1)
- **outlineDuration**: 外框持續時間 (0.5-10)
- **fadeOutTime**: 淡出時間 (0.1-2)
- **usePulse**: 脈衝效果
- **pulseSpeed**: 脈衝速度 (0.5-5)
- **pulseIntensity**: 脈衝強度 (0.1-1)

## 使用方法

### 方法一：自動檢測
```csharp
// 掛載腳本後自動開始檢測
// 手杖移動時自動檢測接觸點
// 自動顯示局部外框
```

### 方法二：手動控制
```csharp
// 手動設定手杖尖端
detector.SetCaneTip(caneTipTransform);

// 清除所有接觸點
detector.ClearAllContacts();

// 獲取接觸統計
int contactCount = detector.GetContactCount();
```

### 方法三：外框管理
```csharp
// 手動顯示外框
GameObject outline = outlineManager.ShowLocalOutline(targetObject, contactPosition, contactNormal);

// 手動隱藏外框
outlineManager.HideLocalOutline(targetObject);

// 清除所有外框
outlineManager.ClearAllOutlines();

// 設定外框參數
outlineManager.SetOutlineParameters(color, intensity, size);
```

## 測試腳本使用

### 基本測試
```csharp
// 在 Inspector 中右鍵選擇：
// - "創建模擬手杖尖端" - 創建測試用手杖
// - "創建測試物體" - 創建測試用立方體
// - "創建樓梯測試物體" - 創建測試用樓梯
// - "運行測試" - 執行檢測測試
// - "兼容性測試" - 檢查兼容性
```

### 進階測試
```csharp
// - "測試手動接觸" - 測試手動外框創建
// - "測試清除外框" - 測試外框清除
// - "測試參數調整" - 測試參數動態調整
// - "清理測試" - 清理所有測試物體
```

## 核心特性

### 1. 精確接觸檢測
- 多方向射線檢測
- 接觸點去重
- 邊緣識別
- 樓梯邊緣特殊處理

### 2. 局部外框顯示
- 只顯示被碰到的點
- 動態外框生成
- 物件池優化
- 自動淡出效果

### 3. 樓梯邊緣識別
- 標籤識別 (tag = "Stair")
- 名稱識別 (包含 "Stair"、"Step")
- 法向量角度檢測
- 特殊外框效果

### 4. 性能優化
- 射線快取
- 物件池管理
- 智能更新
- 自動清理

## 最佳實踐

### 1. 手杖設定
- 確保手杖尖端 Transform 正確設定
- 調整檢測半徑適合你的手杖大小
- 設定合適的檢測精度

### 2. 樓梯物件設定
- 為樓梯物件設定 "Stair" 標籤
- 確保樓梯有正確的 Collider
- 設定合適的圖層

### 3. 性能優化
- 使用圖層過濾減少檢測範圍
- 調整檢測間隔平衡性能和精度
- 使用物件池避免頻繁創建銷毀

### 4. 調試建議
- 開啟 showDebugInfo 查看檢測信息
- 開啟 showDebugLines 查看檢測線
- 使用測試腳本驗證功能

## 常見問題

### Q: 外框不顯示？
A: 檢查手杖尖端是否正確設定，以及檢測參數是否合適

### Q: 檢測不準確？
A: 調整 detectionPrecision 和 caneTipRadius 參數

### Q: 樓梯邊緣不識別？
A: 確保樓梯物件有正確的標籤或名稱

### Q: 性能問題？
A: 減少檢測範圍，使用圖層過濾，調整檢測間隔

### Q: 外框位置不正確？
A: 檢查接觸點計算和法向量方向

## 技術支援

如有問題，請檢查：
1. Console 中的錯誤信息
2. 使用 CaneDetectionTester 進行診斷
3. 確認手杖尖端設定正確
4. 檢查樓梯物件標籤和 Collider
5. 驗證檢測參數設定

## 範例場景

```csharp
// 盲人下樓梯 VR 體驗範例
public class BlindStairExperience : MonoBehaviour
{
    private CanePreciseDetector caneDetector;
    private LocalOutlineManager outlineManager;
    
    void Start()
    {
        // 設定手杖檢測
        caneDetector = GetComponent<CanePreciseDetector>();
        caneDetector.caneTip = transform.Find("CaneTip");
        caneDetector.outlineColor = Color.yellow;
        caneDetector.outlineDuration = 2f;
        
        // 設定外框管理器
        outlineManager = GetComponent<LocalOutlineManager>();
        outlineManager.outlineColor = Color.yellow;
        outlineManager.usePulse = true;
        outlineManager.pulseSpeed = 2f;
    }
    
    void Update()
    {
        // 手杖移動時自動檢測
        // 系統會自動處理接觸檢測和外框顯示
    }
}
```

