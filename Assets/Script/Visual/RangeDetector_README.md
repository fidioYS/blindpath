# RangeDetector 使用說明

## 概述
RangeDetector 是一個高兼容性的範圍檢測器，專門設計用於與 FinalVROutline 配合使用，實現粒子探測器觸發範圍內物體顯現外框的功能。

## 主要特性
- ✅ **完全兼容** - 與 FinalVROutline 完美配合
- ✅ **高性能** - 使用快取避免 GC 分配
- ✅ **易於使用** - 簡單的參數設定
- ✅ **調試友好** - 完整的 Gizmos 和調試信息
- ✅ **靈活配置** - 支援多種檢測參數

## 快速開始

### 1. 基本設定
```csharp
// 將 RangeDetector 掛到你的粒子系統物件上
RangeDetector detector = gameObject.AddComponent<RangeDetector>();

// 設定基本參數
detector.detectionRadius = 3f;        // 檢測半徑
detector.detectionHeight = 2f;        // 檢測高度
detector.detectableLayers = -1;      // 可檢測圖層
```

### 2. 粒子效果設定
```csharp
// 設定粒子系統
detector.detectionParticles = yourParticleSystem;
detector.followParticles = true;     // 跟隨粒子效果
detector.useParticleColorChange = true; // 粒子顏色變化
```

### 3. 外框設定
```csharp
// 設定外框參數
detector.outlineColor = Color.cyan;      // 外框顏色
detector.outlineIntensity = 2f;          // 外框強度
detector.outlineDuration = 3f;           // 外框持續時間
```

## 詳細參數說明

### 檢測設定
- **detectionRadius**: 檢測半徑 (0.5-10)
- **detectionHeight**: 檢測高度範圍 (0.5-5)
- **detectableLayers**: 可檢測的圖層
- **detectionInterval**: 檢測間隔 (0.01-0.5秒)

### 粒子效果
- **detectionParticles**: 檢測粒子系統
- **followParticles**: 是否跟隨粒子效果
- **useParticleColorChange**: 是否使用粒子顏色變化

### 外框設定
- **outlineColor**: 外框顏色
- **outlineIntensity**: 外框強度 (0.1-5)
- **outlineDuration**: 外框持續時間 (0=永久)

### 調試選項
- **showGizmos**: 顯示檢測範圍
- **showDebugInfo**: 顯示調試信息

## 使用方法

### 方法一：自動檢測
```csharp
// 掛載腳本後自動開始檢測
// 無需額外設定
```

### 方法二：手動控制
```csharp
// 手動觸發檢測
detector.ManualDetection();

// 清除所有外框
detector.ClearAllOutlines();

// 檢查物體是否在範圍內
bool inRange = detector.IsObjectInRange(targetObject);
```

### 方法三：動態參數調整
```csharp
// 動態調整檢測參數
detector.SetDetectionParameters(newRadius, newHeight, newLayers);

// 獲取檢測統計
var (current, previous) = detector.GetDetectionStats();
```

## 測試腳本

使用 `RangeDetectorTester` 進行測試：

```csharp
// 在 Inspector 中右鍵選擇：
// - "創建測試物體" - 創建測試用立方體
// - "運行測試" - 執行檢測測試
// - "兼容性測試" - 檢查兼容性
// - "清理測試" - 清理測試物體
```

## 與 FinalVROutline 的配合

RangeDetector 會自動：
1. 檢測範圍內的物體
2. 為物體添加 FinalVROutline 組件
3. 設定外框參數
4. 顯示/隱藏外框
5. 管理外框生命週期

## 性能優化

- 使用 `OverlapSphereNonAlloc` 避免 GC 分配
- 快取 Collider 陣列
- 按間隔檢測而非每幀檢測
- 智能的物體追蹤

## 常見問題

### Q: 外框不顯示？
A: 檢查物體是否有 Renderer 組件，以及 FinalVROutline 是否正確設定

### Q: 檢測不準確？
A: 調整 detectionRadius 和 detectionHeight 參數

### Q: 性能問題？
A: 增加 detectionInterval 值，減少檢測頻率

### Q: 粒子效果不跟隨？
A: 確保 followParticles 為 true，並正確設定 detectionParticles

## 最佳實踐

1. **合理設定檢測範圍** - 不要設定過大的檢測半徑
2. **使用圖層過濾** - 只檢測需要的物體
3. **調整檢測間隔** - 根據需求調整檢測頻率
4. **使用調試工具** - 開啟 Gizmos 查看檢測範圍
5. **測試兼容性** - 使用測試腳本驗證功能

## 範例場景

```csharp
// 盲杖探測器範例
public class CaneDetector : MonoBehaviour
{
    private RangeDetector detector;
    
    void Start()
    {
        detector = GetComponent<RangeDetector>();
        
        // 設定盲杖專用參數
        detector.detectionRadius = 2f;
        detector.detectionHeight = 1.5f;
        detector.outlineColor = Color.yellow;
        detector.outlineDuration = 2f;
    }
    
    void Update()
    {
        // 盲杖移動時自動檢測
        // RangeDetector 會自動處理
    }
}
```

## 技術支援

如有問題，請檢查：
1. Console 中的錯誤信息
2. 使用 RangeDetectorTester 進行診斷
3. 確認 FinalVROutline 組件正常
4. 檢查物體圖層設定

