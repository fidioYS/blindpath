using UnityEngine;
using UnityEngine.InputSystem;

public class VRPauseInput : MonoBehaviour
{
    [Header("暫停輸入設定")]
    [Tooltip("左手控制器的 Menu 按鈕")]
    public InputActionReference leftMenuAction;
    
    [Tooltip("右手控制器的 Menu 按鈕")]
    public InputActionReference rightMenuAction;
    
    [Tooltip("鍵盤暫停鍵（測試用）")]
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("防誤觸設定")]
    public float doublePressTime = 0.3f; // 雙擊時間窗口
    public bool requireDoublePress = false; // 是否需要雙擊才暫停

    private VRGameManager gameManager;
    private VRPauseMenu pauseMenu;
    private float lastPressTime = -1f;
    private bool inputEnabled = true;

    void Start()
    {
        // 找到相關元件
        gameManager = FindObjectOfType<VRGameManager>();
        pauseMenu = FindObjectOfType<VRPauseMenu>();

        if (gameManager == null)
        {
            Debug.LogWarning("找不到 VRGameManager！暫停輸入可能無法正常工作。");
        }

        if (pauseMenu == null)
        {
            Debug.LogWarning("找不到 VRPauseMenu！暫停輸入可能無法正常工作。");
        }

        // 啟用輸入動作
        EnableInputActions();
    }

    void OnEnable()
    {
        EnableInputActions();
    }

    void OnDisable()
    {
        DisableInputActions();
    }

    void EnableInputActions()
    {
        if (leftMenuAction != null && leftMenuAction.action != null)
        {
            leftMenuAction.action.Enable();
            leftMenuAction.action.performed += OnLeftMenuPressed;
        }

        if (rightMenuAction != null && rightMenuAction.action != null)
        {
            rightMenuAction.action.Enable();
            rightMenuAction.action.performed += OnRightMenuPressed;
        }
    }

    void DisableInputActions()
    {
        if (leftMenuAction != null && leftMenuAction.action != null)
        {
            leftMenuAction.action.performed -= OnLeftMenuPressed;
            leftMenuAction.action.Disable();
        }

        if (rightMenuAction != null && rightMenuAction.action != null)
        {
            rightMenuAction.action.performed -= OnRightMenuPressed;
            rightMenuAction.action.Disable();
        }
    }

    void Update()
    {
        // 鍵盤輸入（主要用於編輯器測試）
        if (Input.GetKeyDown(pauseKey))
        {
            HandlePauseInput();
        }
    }

    void OnLeftMenuPressed(InputAction.CallbackContext context)
    {
        if (inputEnabled)
        {
            HandlePauseInput();
        }
    }

    void OnRightMenuPressed(InputAction.CallbackContext context)
    {
        if (inputEnabled)
        {
            HandlePauseInput();
        }
    }

    void HandlePauseInput()
    {
        if (gameManager == null) return;

        // 只在遊戲進行中或暫停時允許暫停輸入
        if (gameManager.currentState != VRGameManager.GameState.Playing && 
            gameManager.currentState != VRGameManager.GameState.Paused)
        {
            return;
        }

        // 雙擊檢測
        if (requireDoublePress)
        {
            float currentTime = Time.unscaledTime;
            
            if (lastPressTime > 0 && currentTime - lastPressTime <= doublePressTime)
            {
                // 雙擊確認，執行暫停
                ExecutePause();
                lastPressTime = -1f; // 重置
            }
            else
            {
                // 第一次按下
                lastPressTime = currentTime;
            }
        }
        else
        {
            // 單擊暫停
            ExecutePause();
        }
    }

    void ExecutePause()
    {
        if (gameManager == null) return;

        if (gameManager.currentState == VRGameManager.GameState.Playing)
        {
            // 暫停遊戲
            gameManager.PauseGame();
            
            // 顯示暫停選單
            if (pauseMenu != null)
            {
                pauseMenu.ShowPauseMenu();
            }
        }
        else if (gameManager.currentState == VRGameManager.GameState.Paused)
        {
            // 繼續遊戲
            gameManager.ResumeGame();
            
            // 隱藏暫停選單
            if (pauseMenu != null)
            {
                pauseMenu.HidePauseMenu();
            }
        }
    }

    // 外部控制輸入啟用/停用
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    // 強制暫停（供其他腳本呼叫）
    public void ForcePause()
    {
        if (gameManager != null && gameManager.currentState == VRGameManager.GameState.Playing)
        {
            ExecutePause();
        }
    }

    // 強制繼續（供其他腳本呼叫）
    public void ForceResume()
    {
        if (gameManager != null && gameManager.currentState == VRGameManager.GameState.Paused)
        {
            ExecutePause();
        }
    }

    void OnDestroy()
    {
        DisableInputActions();
    }
}

