using UnityEngine;

public class VRGameManager : MonoBehaviour
{
    [Header("UI 管理")]
    public GameObject startMenuUI;
    public GameObject gameplayUI;
    public GameObject pauseMenuUI;
    
    [Header("遊戲物件")]
    public GameObject[] gameObjects; // 遊戲中的互動物件
    
    [Header("XR 設定")]
    public GameObject xrOrigin;
    
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused
    }
    
    public GameState currentState = GameState.MainMenu;
    
    void Start()
    {
        ShowMainMenu();
    }
    
    public void ShowMainMenu()
    {
        currentState = GameState.MainMenu;
        
        // 顯示開始選單
        if (startMenuUI != null) startMenuUI.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        
        // 隱藏遊戲物件（可選）
        SetGameObjectsActive(false);
        
        // 重置玩家位置到起始點
        ResetPlayerPosition();
    }
    
    public void StartGame()
    {
        currentState = GameState.Playing;
        
        // 隱藏選單，顯示遊戲 UI
        if (startMenuUI != null) startMenuUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        
        // 啟用遊戲物件
        SetGameObjectsActive(true);
        
        Debug.Log("遊戲開始！");
    }
    
    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;
        
        currentState = GameState.Paused;
        Time.timeScale = 0f; // 暫停時間
        
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }
    
    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;
        
        currentState = GameState.Playing;
        Time.timeScale = 1f; // 恢復時間
        
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f; // 確保時間正常
        ShowMainMenu();
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void SetGameObjectsActive(bool active)
    {
        if (gameObjects == null) return;
        
        foreach (var obj in gameObjects)
        {
            if (obj != null) obj.SetActive(active);
        }
    }
    
    void ResetPlayerPosition()
    {
        if (xrOrigin != null)
        {
            // 重置到起始位置，你可以設定具體座標
            xrOrigin.transform.position = Vector3.zero;
            xrOrigin.transform.rotation = Quaternion.identity;
        }
    }
    
    void Update()
    {
        // VR 中可以用控制器按鈕暫停
        if (currentState == GameState.Playing)
        {
            // 這裡可以加入暫停的輸入檢測
            // 例如：按 Menu 鈕暫停
        }
    }
}
