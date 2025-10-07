using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [Header("按鈕功能")]
    public string gameSceneName = "SampleScene";
    
    public void StartGame()
    {
        Debug.Log("開始遊戲！");
        // 使用 VRGameManager 開始遊戲
        var gameManager = FindObjectOfType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        else
        {
            // 備用：載入場景
            SceneManager.LoadScene(gameSceneName);
        }
    }
    
    public void QuitGame()
    {
        Debug.Log("退出遊戲！");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void ResumeGame()
    {
        Debug.Log("繼續遊戲！");
        var gameManager = FindObjectOfType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
    }
    
    public void RestartGame()
    {
        Debug.Log("重新開始遊戲！");
        var gameManager = FindObjectOfType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    public void GoToMainMenu()
    {
        Debug.Log("回到主選單！");
        var gameManager = FindObjectOfType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.ShowStartMenu();
        }
    }
}
