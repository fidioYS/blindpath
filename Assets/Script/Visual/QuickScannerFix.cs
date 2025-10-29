using UnityEngine;

/// <summary>
/// 快速修復 Scanner Prefab 問題
/// </summary>
public class QuickScannerFix : MonoBehaviour
{
    [Header("快速修復")]
    [SerializeField] private bool fixOnStart = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixScannerIssue();
        }
    }
    
    [ContextMenu("修復 Scanner 問題")]
    public void FixScannerIssue()
    {
        Debug.Log("開始修復 Scanner Prefab 問題...");
        
        // 方法1: 重新載入場景
        Debug.Log("建議解決方案:");
        Debug.Log("1. 關閉 Unity");
        Debug.Log("2. 刪除 Library 資料夾");
        Debug.Log("3. 重新開啟 Unity");
        Debug.Log("4. 等待 Unity 重新生成所有檔案");
        
        // 方法2: 在 Unity 編輯器中執行
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.AssetDatabase.ImportAsset("Assets/GamE ObJecT/prefab/scanner.prefab", UnityEditor.ImportAssetOptions.ForceUpdate);
        Debug.Log("已強制重新匯入 Scanner Prefab");
        #endif
    }
    
    [ContextMenu("檢查 Scanner 狀態")]
    public void CheckScannerStatus()
    {
        // 檢查 scanner prefab 是否存在
        GameObject scannerPrefab = Resources.Load<GameObject>("scanner");
        if (scannerPrefab == null)
        {
            Debug.Log("在 Resources 中找不到 scanner prefab");
        }
        else
        {
            Debug.Log($"找到 scanner prefab: {scannerPrefab.name}");
        }
        
        // 檢查場景中的 scanner 物件
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int scannerCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("scanner"))
            {
                scannerCount++;
                Debug.Log($"場景中找到 scanner 物件: {obj.name}");
            }
        }
        
        Debug.Log($"總共找到 {scannerCount} 個 scanner 物件");
    }
}

