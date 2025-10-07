using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [Header("UI 按鈕")]
    public Button playButton;
    public Button quitButton;
    public Button settingsButton;

    [Header("場景設定")]
    public string gameSceneName = "SampleScene";
    public string settingsSceneName = "Settings";

    [Header("音效 (可選)")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;

    private AudioSource audioSource;

    void Start()
    {
        // 取得或建立 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 綁定按鈕事件
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
            AddButtonSounds(playButton);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
            AddButtonSounds(quitButton);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
            AddButtonSounds(settingsButton);
        }

        // 確保游標在 VR 中不會干擾
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    void AddButtonSounds(Button button)
    {
        // 加入 hover 音效
        var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        // Hover 進入音效
        var hoverEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { PlaySound(buttonHoverSound); });
        eventTrigger.triggers.Add(hoverEntry);
    }

    public void StartGame()
    {
        PlaySound(buttonClickSound);
        Debug.Log("開始遊戲！");
        
        // 優先使用 VRGameManager（同場景管理）
        var gameManager = FindObjectOfType<VRGameManager>();
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        else if (!string.IsNullOrEmpty(gameSceneName))
        {
            // 備用：載入指定場景
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            // 最後備用：直接隱藏 UI
            gameObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
        PlaySound(buttonClickSound);
        Debug.Log("退出遊戲！");
        
        // 優先使用 VRGameManager
        var gameManager = FindObjectOfType<VRGameManager>();
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

    public void OpenSettings()
    {
        PlaySound(buttonClickSound);
        Debug.Log("打開設定！");
        
        if (!string.IsNullOrEmpty(settingsSceneName))
        {
            SceneManager.LoadScene(settingsSceneName);
        }
        else
        {
            Debug.LogWarning("未設定 Settings 場景名稱");
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // 可選：淡入淡出效果
    public void FadeInUI(float duration = 1f)
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        StartCoroutine(FadeCoroutine(canvasGroup, 0f, 1f, duration));
    }

    public void FadeOutUI(float duration = 1f)
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        StartCoroutine(FadeCoroutine(canvasGroup, 1f, 0f, duration));
    }

    System.Collections.IEnumerator FadeCoroutine(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
}
