#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// 強制刷新所有資源並重新編譯
/// 使用：在 Unity 編輯器中右鍵點擊此腳本，選擇 "強制刷新專案"
/// </summary>
public class ForceRefresh
{
    [MenuItem("Tools/強制刷新專案")]
    public static void ForceRefreshProject()
    {
        Debug.Log("開始強制刷新專案...");
        
        // 刷新資源資料庫
        AssetDatabase.Refresh();
        
        // 重新編譯所有腳本
        AssetDatabase.ImportAsset("Assets", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
        
        Debug.Log("專案刷新完成！請檢查 Console 是否有錯誤。");
    }
    
    [MenuItem("Tools/清理所有 Missing Script")]
    public static void CleanMissingScripts()
    {
        Debug.Log("開始清理 Missing Script...");
        
        // 這個功能需要在 Unity 編輯器中手動執行
        // 或者使用 Unity 的內部 API（不推薦）
        Debug.Log("請在 Unity 編輯器中：");
        Debug.Log("1. 選擇場景中的所有 GameObject");
        Debug.Log("2. 在 Inspector 中移除 'Missing Script' 組件");
    }
}
#endif

