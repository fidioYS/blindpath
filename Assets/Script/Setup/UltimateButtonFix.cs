using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UltimateButtonFix : MonoBehaviour
{
    [Header("終極按鈕修復")]
    public bool autoFix = true;
    
    void Start()
    {
        if (autoFix)
        {
            FixAllButtons();
        }
    }
    
    [ContextMenu("修復所有按鈕")]
    public void FixAllButtons()
    {
        Debug.Log("開始終極按鈕修復...");
        
        // 1. 修復所有按鈕
        var buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            FixSingleButton(button);
        }
        
        Debug.Log("終極按鈕修復完成！");
    }
    
    void FixSingleButton(Button button)
    {
        Debug.Log($"修復按鈕: {button.name}");
        
        // 1. 確保按鈕有正確的 Image
        var image = button.GetComponent<Image>();
        if (image == null)
        {
            image = button.gameObject.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.8f);
        }
        
        // 2. 確保按鈕有正確的 RectTransform
        var rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // 調整按鈕大小（VR 需要更大的按鈕）
            rectTransform.sizeDelta = new Vector2(200f, 50f);
        }
        
        // 3. 清除所有現有的事件
        button.onClick.RemoveAllListeners();
        
        // 4. 根據按鈕名稱添加正確的事件
        string buttonName = button.name.ToLower();
        
        if (buttonName.Contains("start") || buttonName.Contains("開始"))
        {
            button.onClick.AddListener(() => {
                Debug.Log("開始遊戲按鈕被點擊！");
                StartGame();
            });
        }
        else if (buttonName.Contains("quit") || buttonName.Contains("退出"))
        {
            button.onClick.AddListener(() => {
                Debug.Log("退出遊戲按鈕被點擊！");
                QuitGame();
            });
        }
        else if (buttonName.Contains("resume") || buttonName.Contains("繼續"))
        {
            button.onClick.AddListener(() => {
                Debug.Log("繼續遊戲按鈕被點擊！");
                ResumeGame();
            });
        }
        else if (buttonName.Contains("restart") || buttonName.Contains("重新"))
        {
            button.onClick.AddListener(() => {
                Debug.Log("重新開始按鈕被點擊！");
                RestartGame();
            });
        }
        else if (buttonName.Contains("menu") || buttonName.Contains("選單"))
        {
            button.onClick.AddListener(() => {
                Debug.Log("主選單按鈕被點擊！");
                GoToMainMenu();
            });
        }
        else
        {
            // 預設按鈕功能
            button.onClick.AddListener(() => {
                Debug.Log($"按鈕 {button.name} 被點擊！");
                TestButtonClick();
            });
        }
        
        // 5. 確保按鈕可互動
        button.interactable = true;
        
        Debug.Log($"按鈕 {button.name} 修復完成");
    }
    
    // 按鈕功能方法
    public void StartGame()
    {
        Debug.Log("🎮 開始遊戲！");
        var gameManager = FindFirstObjectByType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        else
        {
            Debug.LogWarning("找不到 VRGameManager！");
        }
    }
    
    public void QuitGame()
    {
        Debug.Log("🚪 退出遊戲！");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void ResumeGame()
    {
        Debug.Log("▶️ 繼續遊戲！");
        var gameManager = FindFirstObjectByType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
    }
    
    public void RestartGame()
    {
        Debug.Log("🔄 重新開始遊戲！");
        var gameManager = FindFirstObjectByType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    public void GoToMainMenu()
    {
        Debug.Log("🏠 回到主選單！");
        var gameManager = FindFirstObjectByType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.ShowStartMenu();
        }
    }
    
    public void TestButtonClick()
    {
        Debug.Log("✅ 按鈕測試成功！");
    }
}







