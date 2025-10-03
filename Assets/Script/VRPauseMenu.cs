using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VRPauseMenu : MonoBehaviour
{
    [Header("暫停選單按鈕")]
    public Button resumeButton;
    public Button restartButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("UI 元素")]
    public GameObject pausePanel;
    public Text titleText;
    public Text instructionText;

    [Header("音效")]
    public AudioClip buttonClickSound;
    public AudioClip pauseSound;
    public AudioClip resumeSound;

    [Header("視覺效果")]
    public bool dimBackground = true;
    public Color dimColor = new Color(0, 0, 0, 0.5f);
    public float fadeSpeed = 2f;

    private VRGameManager gameManager;
    private AudioSource audioSource;
    private CanvasGroup canvasGroup;
    private Image backgroundDim;

    void Start()
    {
        Initialize();
        SetupButtons();
        SetupVisuals();
    }

    void Initialize()
    {
        // 取得 GameManager 引用
        gameManager = FindObjectOfType<VRGameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("找不到 VRGameManager！暫停選單可能無法正常工作。");
        }

        // 設定 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 設定 CanvasGroup 用於淡入淡出
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 初始隱藏
        gameObject.SetActive(false);
    }

    void SetupButtons()
    {
        // 繼續遊戲
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
            AddButtonSound(resumeButton);
        }

        // 重新開始
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            AddButtonSound(restartButton);
        }

        // 設定
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
            AddButtonSound(settingsButton);
        }

        // 主選單
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(BackToMainMenu);
            AddButtonSound(mainMenuButton);
        }

        // 退出遊戲
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
            AddButtonSound(quitButton);
        }
    }

    void SetupVisuals()
    {
        // 設定標題文字
        if (titleText != null)
        {
            titleText.text = "遊戲暫停";
        }

        // 設定說明文字
        if (instructionText != null)
        {
            instructionText.text = "使用控制器選擇選項";
        }

        // 建立背景遮罩
        if (dimBackground && backgroundDim == null)
        {
            CreateBackgroundDim();
        }
    }

    void CreateBackgroundDim()
    {
        // 建立全螢幕遮罩
        var dimObject = new GameObject("Background Dim");
        dimObject.transform.SetParent(transform, false);
        dimObject.transform.SetAsFirstSibling(); // 放在最底層

        backgroundDim = dimObject.AddComponent<Image>();
        backgroundDim.color = dimColor;
        backgroundDim.raycastTarget = false; // 不阻擋射線

        // 設定為全螢幕
        var rectTransform = backgroundDim.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    void AddButtonSound(Button button)
    {
        var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        // Hover 音效
        var hoverEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { PlaySound(buttonClickSound, 0.7f); });
        eventTrigger.triggers.Add(hoverEntry);
    }

    // === 按鈕功能 ===

    public void ResumeGame()
    {
        PlaySound(resumeSound);
        Debug.Log("繼續遊戲");

        if (gameManager != null)
        {
            gameManager.ResumeGame();
        }
        else
        {
            // 備用方案
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }

    public void RestartGame()
    {
        PlaySound(buttonClickSound);
        Debug.Log("重新開始遊戲");

        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        else
        {
            // 備用方案
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void OpenSettings()
    {
        PlaySound(buttonClickSound);
        Debug.Log("打開設定（功能待實作）");
        
        // TODO: 實作設定選單
        // 可以開啟子選單或切換到設定面板
    }

    public void BackToMainMenu()
    {
        PlaySound(buttonClickSound);
        Debug.Log("回到主選單");

        if (gameManager != null)
        {
            gameManager.ShowMainMenu();
        }
        else
        {
            // 備用方案
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
        PlaySound(buttonClickSound);
        Debug.Log("退出遊戲");

        if (gameManager != null)
        {
            gameManager.QuitGame();
        }
        else
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    // === 顯示/隱藏動畫 ===

    public void ShowPauseMenu()
    {
        gameObject.SetActive(true);
        PlaySound(pauseSound);
        
        if (canvasGroup != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void HidePauseMenu()
    {
        if (canvasGroup != null)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    // === 工具方法 ===

    void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    // 供外部呼叫的切換方法
    public void TogglePause()
    {
        if (gameManager != null)
        {
            if (gameManager.currentState == VRGameManager.GameState.Playing)
            {
                gameManager.PauseGame();
            }
            else if (gameManager.currentState == VRGameManager.GameState.Paused)
            {
                gameManager.ResumeGame();
            }
        }
    }
}

