using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;
using System.Collections.Generic;

public class VRGameManager : MonoBehaviour
{
    [Header("場景管理")]
    public string startMenuSceneName = "StartMenu";
    public string gameplaySceneName = "Gameplay";
    public string pauseMenuSceneName = "PauseMenu";

    [Header("UI 管理")]
    public GameObject startMenuUI;
    public GameObject gameplayUI;
    public GameObject pauseMenuUI;

    [Header("遊戲物件")]
    public List<GameObject> gameObjectsToManage;

    [Header("XR 設定")]
    public XROrigin xrOrigin;

    public static VRGameManager Instance { get; private set; }

    private bool isGamePaused = false;
    private bool isGameStarted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        ShowStartMenu();
    }

    public void ShowStartMenu()
    {
        isGameStarted = false;
        isGamePaused = false;

        if (startMenuUI != null) startMenuUI.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        SetGameObjectsActive(false);
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        isGameStarted = true;
        isGamePaused = false;

        // 載入遊戲場景
        if (!string.IsNullOrEmpty(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            // 備用：使用 UI 切換
            if (startMenuUI != null) startMenuUI.SetActive(false);
            if (gameplayUI != null) gameplayUI.SetActive(true);
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

            SetGameObjectsActive(true);
            Time.timeScale = 1f;
            ResetPlayerPosition();
        }
    }

    public void TogglePause()
    {
        if (!isGameStarted) return;

        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;
            if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
            if (gameplayUI != null) gameplayUI.SetActive(false);
            SetGameObjectsActive(false);
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            if (gameplayUI != null) gameplayUI.SetActive(true);
            SetGameObjectsActive(true);
        }
    }

    public void RestartGame()
    {
        // 重新載入當前場景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToStartMenu()
    {
        // 回到開始選單
        if (!string.IsNullOrEmpty(startMenuSceneName))
        {
            SceneManager.LoadScene(startMenuSceneName);
        }
        else
        {
            ShowStartMenu();
        }
    }

    public void GoToPauseMenu()
    {
        // 載入暫停選單場景
        if (!string.IsNullOrEmpty(pauseMenuSceneName))
        {
            SceneManager.LoadScene(pauseMenuSceneName);
        }
        else
        {
            TogglePause();
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

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public bool IsGameStarted()
    {
        return isGameStarted;
    }

    void SetGameObjectsActive(bool active)
    {
        if (gameObjectsToManage == null)
        {
            Debug.LogWarning("gameObjectsToManage 列表為 null，跳過設定");
            return;
        }

        foreach (var obj in gameObjectsToManage)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
            else
            {
                Debug.LogWarning("gameObjectsToManage 列表中有 null 物件，已跳過");
            }
        }
    }

    void ResetPlayerPosition()
    {
        if (xrOrigin != null)
        {
            // 重置 XR Origin 位置到原點
            xrOrigin.transform.position = Vector3.zero;
            xrOrigin.transform.rotation = Quaternion.identity;
            
            // 重置攝影機位置
            Camera xrCamera = xrOrigin.GetComponentInChildren<Camera>();
            if (xrCamera != null)
            {
                xrCamera.transform.localPosition = Vector3.zero;
                xrCamera.transform.localRotation = Quaternion.identity;
            }
        }
    }
}