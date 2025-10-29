using UnityEngine;
using VRVisual;

/// <summary>
/// RangeDetector 測試腳本
/// 用於測試和驗證 RangeDetector 的功能
/// </summary>
public class RangeDetectorTester : MonoBehaviour
{
    [Header("測試設定")]
    [SerializeField] private RangeDetector rangeDetector;
    [SerializeField] private bool autoTest = true;
    [SerializeField] private float testInterval = 2f;
    
    [Header("測試物體")]
    [SerializeField] private GameObject[] testObjects;
    [SerializeField] private bool createTestObjects = true;
    
    [Header("測試參數")]
    [SerializeField] private float testRadius = 5f;
    [SerializeField] private float testHeight = 3f;
    [SerializeField] private LayerMask testLayers = -1;
    
    private float lastTestTime = 0f;
    private int testCount = 0;
    
    private void Start()
    {
        // 自動獲取 RangeDetector
        if (rangeDetector == null)
        {
            rangeDetector = GetComponent<RangeDetector>();
        }
        
        if (rangeDetector == null)
        {
            rangeDetector = FindObjectOfType<RangeDetector>();
        }
        
        if (rangeDetector == null)
        {
            Debug.LogError("[RangeDetectorTester] 找不到 RangeDetector 組件！");
            return;
        }
        
        // 創建測試物體
        if (createTestObjects)
        {
            CreateTestObjects();
        }
        
        // 設定測試參數
        rangeDetector.SetDetectionParameters(testRadius, testHeight, testLayers);
        
        Debug.Log("[RangeDetectorTester] 測試初始化完成");
    }
    
    private void Update()
    {
        if (autoTest && Time.time - lastTestTime >= testInterval)
        {
            RunTest();
            lastTestTime = Time.time;
        }
    }
        
    /// <summary>
    /// 創建測試物體
    /// </summary>
    [ContextMenu("創建測試物體")]
    public void CreateTestObjects()
    {
        if (testObjects != null && testObjects.Length > 0)
        {
            // 清除現有測試物體
            foreach (GameObject obj in testObjects)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
        }
        
        // 創建新的測試物體
        testObjects = new GameObject[5];
        Vector3[] positions = {
            new Vector3(2, 0, 0),
            new Vector3(-2, 0, 0),
            new Vector3(0, 0, 2),
            new Vector3(0, 0, -2),
            new Vector3(1, 1, 1)
        };
        
        for (int i = 0; i < testObjects.Length; i++)
        {
            GameObject testObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObj.name = $"TestObject_{i}";
            testObj.transform.position = positions[i];
            testObj.transform.localScale = Vector3.one * 0.5f;
            
            // 添加隨機顏色
            Renderer renderer = testObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(
                    Random.Range(0.2f, 1f),
                    Random.Range(0.2f, 1f),
                    Random.Range(0.2f, 1f)
                );
            }
            
            testObjects[i] = testObj;
        }
        
        Debug.Log($"[RangeDetectorTester] 創建了 {testObjects.Length} 個測試物體");
    }
    
    /// <summary>
    /// 運行測試
    /// </summary>
    [ContextMenu("運行測試")]
    public void RunTest()
    {
        if (rangeDetector == null)
        {
            Debug.LogError("[RangeDetectorTester] RangeDetector 為空！");
            return;
        }
        
        testCount++;
        
        // 獲取檢測統計
        var (currentCount, previousCount) = rangeDetector.GetDetectionStats();
        
        Debug.Log($"[RangeDetectorTester] 測試 #{testCount}");
        Debug.Log($"  - 當前檢測到: {currentCount} 個物體");
        Debug.Log($"  - 之前檢測到: {previousCount} 個物體");
        Debug.Log($"  - 檢測半徑: {rangeDetector.detectionRadius}");
        Debug.Log($"  - 檢測高度: {rangeDetector.detectionHeight}");
        
        // 測試個別物體
        if (testObjects != null)
        {
            for (int i = 0; i < testObjects.Length; i++)
            {
                if (testObjects[i] != null)
                {
                    bool inRange = rangeDetector.IsObjectInRange(testObjects[i]);
                    Debug.Log($"  - 測試物體 {i}: {(inRange ? "在範圍內" : "在範圍外")}");
                }
            }
        }
    }
    
    /// <summary>
    /// 測試手動檢測
    /// </summary>
    [ContextMenu("手動檢測測試")]
    public void TestManualDetection()
    {
        if (rangeDetector == null)
        {
            Debug.LogError("[RangeDetectorTester] RangeDetector 為空！");
            return;
        }
        
        Debug.Log("[RangeDetectorTester] 執行手動檢測...");
        rangeDetector.ManualDetection();
        
        var (currentCount, previousCount) = rangeDetector.GetDetectionStats();
        Debug.Log($"手動檢測結果: 當前 {currentCount} 個, 之前 {previousCount} 個");
    }
    
    /// <summary>
    /// 測試清除外框
    /// </summary>
    [ContextMenu("清除外框測試")]
    public void TestClearOutlines()
    {
        if (rangeDetector == null)
        {
            Debug.LogError("[RangeDetectorTester] RangeDetector 為空！");
            return;
        }
        
        Debug.Log("[RangeDetectorTester] 清除所有外框...");
        rangeDetector.ClearAllOutlines();
        
        var (currentCount, previousCount) = rangeDetector.GetDetectionStats();
        Debug.Log($"清除後結果: 當前 {currentCount} 個, 之前 {previousCount} 個");
    }
    
    /// <summary>
    /// 測試參數調整
    /// </summary>
    [ContextMenu("調整測試參數")]
    public void TestParameterAdjustment()
    {
        if (rangeDetector == null)
        {
            Debug.LogError("[RangeDetectorTester] RangeDetector 為空！");
            return;
        }
        
        // 隨機調整參數
        float newRadius = Random.Range(1f, 8f);
        float newHeight = Random.Range(1f, 5f);
        
        Debug.Log($"[RangeDetectorTester] 調整參數: 半徑 {newRadius}, 高度 {newHeight}");
        rangeDetector.SetDetectionParameters(newRadius, newHeight, testLayers);
        
        // 運行檢測
        rangeDetector.ManualDetection();
    }
    
    /// <summary>
    /// 測試兼容性
    /// </summary>
    [ContextMenu("兼容性測試")]
    public void TestCompatibility()
    {
        Debug.Log("[RangeDetectorTester] 開始兼容性測試...");
        
        // 檢查 RangeDetector
        if (rangeDetector == null)
        {
            Debug.LogError("❌ RangeDetector 組件缺失");
            return;
        }
        Debug.Log("✅ RangeDetector 組件正常");
        
        // 檢查 FinalVROutline 命名空間
        try
        {
            var outline = FindObjectOfType<FinalVROutline>();
            if (outline != null)
            {
                Debug.Log("✅ FinalVROutline 組件可用");
            }
            else
            {
                Debug.Log("⚠️ 場景中沒有 FinalVROutline 組件");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ FinalVROutline 兼容性問題: {e.Message}");
        }
        
        // 檢查粒子系統
        if (rangeDetector.detectionParticles != null)
        {
            Debug.Log("✅ 粒子系統已設定");
        }
        else
        {
            Debug.Log("⚠️ 粒子系統未設定");
        }
        
        Debug.Log("[RangeDetectorTester] 兼容性測試完成");
    }
    
    /// <summary>
    /// 清理測試
    /// </summary>
    [ContextMenu("清理測試")]
    public void CleanupTest()
    {
        if (testObjects != null)
        {
            foreach (GameObject obj in testObjects)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
            testObjects = null;
        }
        
        if (rangeDetector != null)
        {
            rangeDetector.ClearAllOutlines();
        }
        
        Debug.Log("[RangeDetectorTester] 測試清理完成");
    }
    
    private void OnDestroy()
    {
        CleanupTest();
    }
}

