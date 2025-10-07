using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class VRPauseMenu : MonoBehaviour
{
    [Header("UI 元素")]
    public GameObject pausePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI instructionText;

    [Header("按鈕")]
    public Button resumeButton;
    public Button restartButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("音效")]
    public AudioClip buttonClickSound;
    public AudioClip pauseSound;
    public AudioClip resumeSound;
    private AudioSource audioSource;

    [Header("視覺效果")]
    public bool dimBackground = true;
    public Color dimColor = new Color(0, 0, 0, 0.7f);
    public float fadeSpeed = 5f;

    private CanvasGroup canvasGroup;
    private Image backgroundDimImage;
    private bool isShowing = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 自動建立背景遮罩
        if (dimBackground && transform.Find("BackgroundDim") == null)
        {
            GameObject dimObj = new GameObject("BackgroundDim");
            dimObj.transform.SetParent(transform);
            dimObj.transform.SetSiblingIndex(0);
            RectTransform dimRect = dimObj.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.sizeDelta = Vector2.zero;
            backgroundDimImage = dimObj.AddComponent<Image>();
            backgroundDimImage.color = new Color(dimColor.r, dimColor.g, dimColor.b, 0);
        }
        else if (dimBackground)
        {
            backgroundDimImage = transform.Find("BackgroundDim")?.GetComponent<Image>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0;
        }

        SetupButtons();
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    void SetupButtons()
    {
        // 自動尋找按鈕
        if (resumeButton == null) resumeButton = transform.Find("Pause Panel/Resume Button")?.GetComponent<Button>();
        if (restartButton == null) restartButton = transform.Find("Pause Panel/Restart Button")?.GetComponent<Button>();
        if (settingsButton == null) settingsButton = transform.Find("Pause Panel/Settings Button")?.GetComponent<Button>();
        if (mainMenuButton == null) mainMenuButton = transform.Find("Pause Panel/Main Menu Button")?.GetComponent<Button>();
        if (quitButton == null) quitButton = transform.Find("Pause Panel/Quit Button")?.GetComponent<Button>();

        resumeButton?.onClick.AddListener(ResumeGame);
        restartButton?.onClick.AddListener(RestartGame);
        settingsButton?.onClick.AddListener(OpenSettings);
        mainMenuButton?.onClick.AddListener(GoToMainMenu);
        quitButton?.onClick.AddListener(QuitGame);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void ShowPauseMenu()
    {
        if (isShowing) return;
        isShowing = true;
        gameObject.SetActive(true);
        PlaySound(pauseSound);
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeSpeed));
        if (dimBackground && backgroundDimImage != null)
        {
            StartCoroutine(FadeImageAlpha(backgroundDimImage, 0, dimColor.a, fadeSpeed));
        }
    }

    public void HidePauseMenu()
    {
        if (!isShowing) return;
        isShowing = false;
        PlaySound(resumeSound);
        StartCoroutine(FadeCanvasGroup(canvasGroup, 1, 0, fadeSpeed, () => gameObject.SetActive(false)));
        if (dimBackground && backgroundDimImage != null)
        {
            StartCoroutine(FadeImageAlpha(backgroundDimImage, dimColor.a, 0, fadeSpeed));
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float speed, System.Action onComplete = null)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * speed;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, timer);
            yield return null;
        }
        group.alpha = endAlpha;
        onComplete?.Invoke();
    }

    IEnumerator FadeImageAlpha(Image image, float startAlpha, float endAlpha, float speed)
    {
        float timer = 0f;
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, endAlpha);
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * speed;
            image.color = Color.Lerp(startColor, endColor, timer);
            yield return null;
        }
        image.color = endColor;
    }

    public void ResumeGame()
    {
        PlaySound(buttonClickSound);
        Debug.Log("繼續遊戲！");
        VRGameManager.Instance?.TogglePause();
    }

    public void RestartGame()
    {
        PlaySound(buttonClickSound);
        Debug.Log("重新開始遊戲！");
        VRGameManager.Instance?.RestartGame();
    }

    public void OpenSettings()
    {
        PlaySound(buttonClickSound);
        Debug.Log("打開設定！");
    }

    public void GoToMainMenu()
    {
        PlaySound(buttonClickSound);
        Debug.Log("回到主選單！");
        VRGameManager.Instance?.ShowStartMenu();
    }

    public void QuitGame()
    {
        PlaySound(buttonClickSound);
        Debug.Log("退出遊戲！");
        VRGameManager.Instance?.QuitGame();
    }
}