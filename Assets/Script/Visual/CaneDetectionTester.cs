using UnityEngine;
using VRCaneDetection;
using VRVisual;

/// <summary>
/// 手杖精確檢測測試腳本
/// 測試手杖接觸檢測和局部外框顯示功能
/// </summary>
public class CaneDetectionTester : MonoBehaviour
{
    [Header("測試設定")]
    [SerializeField] private CanePreciseDetector caneDetector;
    [SerializeField] private LocalOutlineManager outlineManager;
    [SerializeField] private bool autoTest = true;
    [SerializeField] private float testInterval = 1f;
    
    [Header("測試物體")]
    [SerializeField] private GameObject[] testObjects;
    [SerializeField] private bool createTestObjects = true;
    [SerializeField] private bool createStairObjects = true;
    
    [Header("測試參數")]
    [SerializeField] private float testRadius = 2f;
    [SerializeField] private float testHeight = 1f;
    [SerializeField] private LayerMask testLayers = -1;
    
    [Header("手杖模擬")]
    [SerializeField] private Transform simulatedCaneTip;
    [SerializeField] private bool simulateCaneMovement = true;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float movementRadius = 3f;
    
    private float lastTestTime = 0f;
    private int testCount = 0;
    private float movementAngle = 0f;
    
    private void Start()
    {
        // 自動獲取組件
        if (caneDetector == null)
        {
            caneDetector = GetComponent<CanePreciseDetector>();
        }
        
        if (outlineManager == null)
        {
            outlineManager = GetComponent<LocalOutlineManager>();
        }
        
        if (caneDetector == null)
        {
            caneDetector = FindObjectOfType<CanePreciseDetector>();
        }
        
        if (outlineManager == null)
        {
            outlineManager = FindObjectOfType<LocalOutlineManager>();
        }
        
        // 創建測試物體
        if (createTestObjects)
        {
            CreateTestObjects();
        }
        
        if (createStairObjects)
        {
            CreateStairObjects();
        }
        
        // 設定手杖尖端
        if (simulatedCaneTip == null)
        {
            CreateSimulatedCaneTip();
        }
        
        if (caneDetector != null && simulatedCaneTip != null)
        {
            caneDetector.SetCaneTip(simulatedCaneTip);
        }
        
        Debug.Log("[CaneDetectionTester] 測試初始化完成");
    }
    
    private void Update()
    {
        // 模擬手杖移動
        if (simulateCaneMovement && simulatedCaneTip != null)
        {
            SimulateCaneMovement();
        }
        
        // 自動測試
        if (autoTest && Time.time - lastTestTime >= testInterval)
        {
            RunTest();
            lastTestTime = Time.time;
        }
    }
    
    /// <summary>
    /// 創建模擬手杖尖端
    /// </summary>
    [ContextMenu("創建模擬手杖尖端")]
    public void CreateSimulatedCaneTip()
    {
        if (simulatedCaneTip != null) return;
        
        GameObject caneTipObj = new GameObject("SimulatedCaneTip");
        caneTipObj.transform.parent = transform;
        caneTipObj.transform.localPosition = Vector3.forward * 2f;
        
        // 添加視覺指示器
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicator.transform.parent = caneTipObj.transform;
        indicator.transform.localPosition = Vector3.zero;
        indicator.transform.localScale = Vector3.one * 0.1f;
        
        // 設定顏色
        Renderer renderer = indicator.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.green;
        }
        
        simulatedCaneTip = caneTipObj.transform;
        
        Debug.Log("[CaneDetectionTester] 創建模擬手杖尖端");
    }
    
    /// <summary>
    /// 模擬手杖移動
    /// </summary>
    private void SimulateCaneMovement()
    {
        if (simulatedCaneTip == null) return;
        
        movementAngle += Time.deltaTime * movementSpeed;
        
        Vector3 offset = new Vector3(
            Mathf.Sin(movementAngle) * movementRadius,
            Mathf.Sin(movementAngle * 0.5f) * 0.5f,
            Mathf.Cos(movementAngle) * movementRadius
        );
        
        simulatedCaneTip.position = transform.position + offset;
        simulatedCaneTip.LookAt(transform.position);
    }
    
    /// <summary>
    /// 創建測試物體
    /// </summary>
    [ContextMenu("創建測試物體")]
    public void CreateTestObjects()
    {
        if (testObjects != null && testObjects.Length > 0)
        {
            foreach (GameObject obj in testObjects)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
        }
        
        // 創建測試物體
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
            testObj.transform.localScale = Vector3.one * 0.8f;
            
            // 添加隨機顏色
            Renderer renderer = testObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(
                    Random.Range(0.3f, 1f),
                    Random.Range(0.3f, 1f),
                    Random.Range(0.3f, 1f)
                );
            }
            
            testObjects[i] = testObj;
        }
        
        Debug.Log($"[CaneDetectionTester] 創建了 {testObjects.Length} 個測試物體");
    }
    
    /// <summary>
    /// 創建樓梯測試物體
    /// </summary>
    [ContextMenu("創建樓梯測試物體")]
    public void CreateStairObjects()
    {
        // 創建樓梯
        GameObject stair = new GameObject("TestStair");
        stair.transform.position = new Vector3(0, 0, 5);
        stair.tag = "Stair";
        
        // 創建樓梯階層
        for (int i = 0; i < 3; i++)
        {
            GameObject step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.transform.parent = stair.transform;
            step.transform.localPosition = new Vector3(0, i * 0.2f, i * 0.3f);
            step.transform.localScale = new Vector3(2, 0.2f, 0.3f);
            step.name = $"Step_{i}";
            
            // 設定顏色
            Renderer renderer = step.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.gray;
            }
        }
        
        Debug.Log("[CaneDetectionTester] 創建了樓梯測試物體");
    }
    
    /// <summary>
    /// 運行測試
    /// </summary>
    [ContextMenu("運行測試")]
    public void RunTest()
    {
        if (caneDetector == null)
        {
            Debug.LogError("[CaneDetectionTester] CanePreciseDetector 為空！");
            return;
        }
        
        testCount++;
        
        // 獲取接觸統計
        int contactCount = caneDetector.GetContactCount();
        
        Debug.Log($"[CaneDetectionTester] 測試 #{testCount}");
        Debug.Log($"  - 接觸點數量: {contactCount}");
        Debug.Log($"  - 手杖尖端位置: {(simulatedCaneTip != null ? simulatedCaneTip.position.ToString() : "未設定")}");
        
        // 測試外框管理器
        if (outlineManager != null)
        {
            int outlineCount = outlineManager.GetActiveOutlineCount();
            Debug.Log($"  - 活躍外框數量: {outlineCount}");
        }
        
        // 測試個別物體
        if (testObjects != null)
        {
            for (int i = 0; i < testObjects.Length; i++)
            {
                if (testObjects[i] != null)
                {
                    bool hasOutline = outlineManager != null && outlineManager.HasOutline(testObjects[i]);
                    Debug.Log($"  - 測試物體 {i}: {(hasOutline ? "有外框" : "無外框")}");
                }
            }
        }
    }
    
    /// <summary>
    /// 測試手動接觸
    /// </summary>
    [ContextMenu("測試手動接觸")]
    public void TestManualContact()
    {
        if (outlineManager == null)
        {
            Debug.LogError("[CaneDetectionTester] LocalOutlineManager 為空！");
            return;
        }
        
        if (testObjects == null || testObjects.Length == 0)
        {
            Debug.LogError("[CaneDetectionTester] 沒有測試物體！");
            return;
        }
        
        // 為第一個測試物體創建外框
        GameObject testObj = testObjects[0];
        Vector3 contactPos = testObj.transform.position + Vector3.up * 0.5f;
        Vector3 contactNormal = Vector3.up;
        
        GameObject outline = outlineManager.ShowLocalOutline(testObj, contactPos, contactNormal);
        
        if (outline != null)
        {
            Debug.Log($"[CaneDetectionTester] 成功創建手動接觸外框: {testObj.name}");
        }
        else
        {
            Debug.LogError("[CaneDetectionTester] 創建手動接觸外框失敗！");
        }
    }
    
    /// <summary>
    /// 測試清除外框
    /// </summary>
    [ContextMenu("測試清除外框")]
    public void TestClearOutlines()
    {
        if (caneDetector == null)
        {
            Debug.LogError("[CaneDetectionTester] CanePreciseDetector 為空！");
            return;
        }
        
        if (outlineManager == null)
        {
            Debug.LogError("[CaneDetectionTester] LocalOutlineManager 為空！");
            return;
        }
        
        Debug.Log("[CaneDetectionTester] 清除所有外框...");
        
        caneDetector.ClearAllContacts();
        outlineManager.ClearAllOutlines();
        
        Debug.Log("[CaneDetectionTester] 外框清除完成");
    }
    
    /// <summary>
    /// 測試參數調整
    /// </summary>
    [ContextMenu("測試參數調整")]
    public void TestParameterAdjustment()
    {
        if (outlineManager == null)
        {
            Debug.LogError("[CaneDetectionTester] LocalOutlineManager 為空！");
            return;
        }
        
        // 隨機調整參數
        Color newColor = new Color(
            Random.Range(0.2f, 1f),
            Random.Range(0.2f, 1f),
            Random.Range(0.2f, 1f)
        );
        float newIntensity = Random.Range(1f, 3f);
        float newSize = Random.Range(0.1f, 0.5f);
        
        Debug.Log($"[CaneDetectionTester] 調整參數: 顏色 {newColor}, 強度 {newIntensity}, 大小 {newSize}");
        outlineManager.SetOutlineParameters(newColor, newIntensity, newSize);
    }
    
    /// <summary>
    /// 兼容性測試
    /// </summary>
    [ContextMenu("兼容性測試")]
    public void TestCompatibility()
    {
        Debug.Log("[CaneDetectionTester] 開始兼容性測試...");
        
        // 檢查 CanePreciseDetector
        if (caneDetector == null)
        {
            Debug.LogError("❌ CanePreciseDetector 組件缺失");
        }
        else
        {
            Debug.Log("✅ CanePreciseDetector 組件正常");
        }
        
        // 檢查 LocalOutlineManager
        if (outlineManager == null)
        {
            Debug.LogError("❌ LocalOutlineManager 組件缺失");
        }
        else
        {
            Debug.Log("✅ LocalOutlineManager 組件正常");
        }
        
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
        
        // 檢查手杖尖端
        if (simulatedCaneTip == null)
        {
            Debug.LogError("❌ 手杖尖端未設定");
        }
        else
        {
            Debug.Log("✅ 手杖尖端已設定");
        }
        
        Debug.Log("[CaneDetectionTester] 兼容性測試完成");
    }
    
    /// <summary>
    /// 清理測試
    /// </summary>
    [ContextMenu("清理測試")]
    public void CleanupTest()
    {
        // 清理測試物體
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
        
        // 清理樓梯物體
        GameObject[] stairs = GameObject.FindGameObjectsWithTag("Stair");
        foreach (GameObject stair in stairs)
        {
            if (stair.name.Contains("TestStair"))
            {
                DestroyImmediate(stair);
            }
        }
        
        // 清理手杖尖端
        if (simulatedCaneTip != null)
        {
            DestroyImmediate(simulatedCaneTip.gameObject);
            simulatedCaneTip = null;
        }
        
        // 清除外框
        if (caneDetector != null)
        {
            caneDetector.ClearAllContacts();
        }
        
        if (outlineManager != null)
        {
            outlineManager.ClearAllOutlines();
        }
        
        Debug.Log("[CaneDetectionTester] 測試清理完成");
    }
    
    private void OnDestroy()
    {
        CleanupTest();
    }
}

