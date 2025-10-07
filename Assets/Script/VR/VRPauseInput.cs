using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
using System.Linq;

public class VRPauseInput : MonoBehaviour
{
    [Header("輸入動作")]
    public InputActionProperty leftMenuAction;
    public InputActionProperty rightMenuAction;
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("防誤觸設定")]
    public bool requireDoublePress = true;
    public float doublePressTime = 0.3f;

    private float lastPressTime = -999f;
    private bool inputEnabled = true;

    void OnEnable()
    {
        if (leftMenuAction.action != null)
        {
            leftMenuAction.action.Enable();
            leftMenuAction.action.performed += OnMenuButtonPressed;
        }
        
        if (rightMenuAction.action != null)
        {
            rightMenuAction.action.Enable();
            rightMenuAction.action.performed += OnMenuButtonPressed;
        }
    }

    void OnDisable()
    {
        if (leftMenuAction.action != null)
        {
            leftMenuAction.action.performed -= OnMenuButtonPressed;
            leftMenuAction.action.Disable();
        }
        
        if (rightMenuAction.action != null)
        {
            rightMenuAction.action.performed -= OnMenuButtonPressed;
            rightMenuAction.action.Disable();
        }
    }

    void Start()
    {
        // 自動尋找輸入動作
        SetupInputActions();
    }

    void SetupInputActions()
    {
        // 尋找 XRI Default Input Actions
        var inputActionAsset = Resources.FindObjectsOfTypeAll<InputActionAsset>()
            .FirstOrDefault(asset => asset.name.Contains("XRI Default Input Actions"));
        
        if (inputActionAsset != null)
        {
            // 設定左手選單按鈕
            var leftMenuAction = inputActionAsset.FindAction("XRI LeftHand/Menu");
            if (leftMenuAction != null)
            {
                this.leftMenuAction = new InputActionProperty(leftMenuAction);
            }
            
            // 設定右手選單按鈕
            var rightMenuAction = inputActionAsset.FindAction("XRI RightHand/Menu");
            if (rightMenuAction != null)
            {
                this.rightMenuAction = new InputActionProperty(rightMenuAction);
            }
            
            Debug.Log("輸入動作已自動設定完成");
        }
        else
        {
            Debug.LogWarning("找不到 XRI Default Input Actions，請手動設定輸入動作");
        }
    }

    void Update()
    {
        if (!inputEnabled) return;

        // 鍵盤 ESC 暫停 (主要用於編輯器測試)
        if (Keyboard.current != null && Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            HandlePauseInput();
        }
    }

    void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return;
        HandlePauseInput();
    }

    void HandlePauseInput()
    {
        if (requireDoublePress)
        {
            if (Time.unscaledTime - lastPressTime < doublePressTime)
            {
                // 雙擊成功
                TogglePauseState();
                lastPressTime = -999f;
            }
            else
            {
                // 第一次點擊，記錄時間
                lastPressTime = Time.unscaledTime;
            }
        }
        else
        {
            // 不需要雙擊，直接暫停
            TogglePauseState();
        }
    }

    void TogglePauseState()
    {
        if (VRGameManager.Instance != null)
        {
            VRGameManager.Instance.TogglePause();
        }
        else
        {
            Debug.LogWarning("VRGameManager 未找到，無法切換暫停狀態。");
        }
    }

    // 外部呼叫，強制暫停
    public void ForcePause()
    {
        if (VRGameManager.Instance != null && !VRGameManager.Instance.IsGamePaused())
        {
            VRGameManager.Instance.TogglePause();
        }
    }

    // 外部呼叫，強制恢復
    public void ForceResume()
    {
        if (VRGameManager.Instance != null && VRGameManager.Instance.IsGamePaused())
        {
            VRGameManager.Instance.TogglePause();
        }
    }

    // 啟用/停用暫停輸入
    public void SetInputEnabled(bool enable)
    {
        inputEnabled = enable;
        if (!enable)
        {
            lastPressTime = -999f;
        }
    }
}