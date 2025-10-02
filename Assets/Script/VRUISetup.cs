using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class VRUISetup : MonoBehaviour
{
    [Header("VR UI 設定")]
    [Tooltip("UI 距離玩家的距離（公尺）")]
    public float distanceFromPlayer = 3f;
    
    [Tooltip("UI 面板的大小")]
    public Vector2 canvasSize = new Vector2(4f, 3f);
    
    [Tooltip("是否跟隨玩家視線")]
    public bool followPlayerGaze = false;
    
    [Tooltip("跟隨速度（當 followPlayerGaze 為 true 時）")]
    public float followSpeed = 2f;

    private Canvas canvas;
    private Transform playerCamera;
    private Vector3 initialPosition;

    void Start()
    {
        SetupVRCanvas();
        FindPlayerCamera();
        PositionUI();
    }

    void SetupVRCanvas()
    {
        canvas = GetComponent<Canvas>();
        
        // 設定為 World Space Canvas（VR 必需）
        canvas.renderMode = RenderMode.WorldSpace;
        
        // 設定 Canvas 大小
        var rectTransform = canvas.GetComponent<RectTransform>();
        rectTransform.sizeDelta = canvasSize;
        
        // 設定適當的縮放
        transform.localScale = Vector3.one * 0.001f; // 1 unit = 1 meter
        
        // 確保有 GraphicRaycaster 用於 VR 互動
        if (GetComponent<GraphicRaycaster>() == null)
        {
            var raycaster = gameObject.AddComponent<GraphicRaycaster>();
            raycaster.ignoreReversedGraphics = false; // 允許背面互動
        }

        // 加入 Canvas Group 用於淡入淡出
        if (GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }
    }

    void FindPlayerCamera()
    {
        // 尋找 XR Origin 的攝影機
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin != null)
        {
            playerCamera = xrOrigin.Camera.transform;
        }
        else
        {
            // 備用：尋找主攝影機
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                playerCamera = mainCamera.transform;
            }
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("找不到玩家攝影機，UI 位置可能不正確");
        }
    }

    void PositionUI()
    {
        if (playerCamera == null) return;

        // 將 UI 放在玩家前方
        Vector3 forward = playerCamera.forward;
        forward.y = 0; // 保持水平
        forward = forward.normalized;

        transform.position = playerCamera.position + forward * distanceFromPlayer;
        transform.LookAt(playerCamera.position);
        transform.Rotate(0, 180, 0); // 翻轉面向玩家

        initialPosition = transform.position;
    }

    void Update()
    {
        if (followPlayerGaze && playerCamera != null)
        {
            FollowPlayerGaze();
        }
    }

    void FollowPlayerGaze()
    {
        // 計算目標位置
        Vector3 forward = playerCamera.forward;
        forward.y = 0;
        forward = forward.normalized;
        
        Vector3 targetPosition = playerCamera.position + forward * distanceFromPlayer;
        
        // 平滑移動
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        
        // 面向玩家
        Vector3 lookDirection = playerCamera.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
    }

    // 重新定位 UI 到玩家前方
    public void ResetPosition()
    {
        PositionUI();
    }

    // 設定 UI 可見性
    public void SetVisible(bool visible)
    {
        canvas.enabled = visible;
    }

    // 在 Scene View 中顯示 UI 範圍
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(canvasSize.x * 0.001f, canvasSize.y * 0.001f, 0.1f));
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(playerCamera.position, transform.position);
        }
    }
}
