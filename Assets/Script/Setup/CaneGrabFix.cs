using UnityEngine;


public class CaneGrabFix : MonoBehaviour
{
    [Header("手杖抓取修復")]
    public bool autoFix = true;
    public string caneName = "Cane";
    
    void Start()
    {
        if (autoFix)
        {
            FixCaneGrab();
        }
    }
    
    [ContextMenu("修復手杖抓取")]
    public void FixCaneGrab()
    {
        Debug.Log("開始修復手杖抓取...");
        
        // 1. 找到手杖物件
        GameObject cane = GameObject.Find(caneName);
        if (cane == null)
        {
            Debug.LogError($"找不到名為 '{caneName}' 的手杖物件！");
            return;
        }
        
        Debug.Log($"找到手杖: {cane.name}");
        
        // 2. 檢查並添加必要的組件
        FixCaneComponents(cane);
        
        Debug.Log("手杖抓取修復完成！");
    }
    
    void FixCaneComponents(GameObject cane)
    {
        // 1. 確保有 Rigidbody
        var rigidbody = cane.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = cane.AddComponent<Rigidbody>();
            rigidbody.mass = 0.5f; // 手杖重量
            rigidbody.linearDamping = 0.5f;
            rigidbody.angularDamping = 0.5f;
            Debug.Log("添加 Rigidbody 到手杖");
        }
        
        // 2. 確保有 Collider
        var collider = cane.GetComponent<Collider>();
        if (collider == null)
        {
            // 嘗試添加 CapsuleCollider（適合手杖形狀）
            var capsuleCollider = cane.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 1.0f;
            capsuleCollider.radius = 0.02f;
            capsuleCollider.direction = 1; // Y 軸方向
            Debug.Log("添加 CapsuleCollider 到手杖");
        }
        
        // 3. 添加 XRGrabInteractable
        var grabInteractable = cane.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = cane.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            
            // 設定抓取參數
            grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable.MovementType.VelocityTracking;
            grabInteractable.trackPosition = true;
            grabInteractable.trackRotation = true;
            grabInteractable.throwOnDetach = true;
            grabInteractable.throwSmoothingDuration = 0.1f;
            grabInteractable.throwVelocityScale = 1.5f;
            grabInteractable.throwAngularVelocityScale = 1.5f;
            
            Debug.Log("添加 XRGrabInteractable 到手杖");
        }
        
        // 4. 確保有 Attach Transform
        Transform attachTransform = cane.transform.Find("Attach");
        if (attachTransform == null)
        {
            // 建立 Attach Transform
            GameObject attachObj = new GameObject("Attach");
            attachObj.transform.SetParent(cane.transform);
            attachObj.transform.localPosition = new Vector3(0, 0.5f, 0); // 手杖中間位置
            attachObj.transform.localRotation = Quaternion.identity;
            
            // 設定到 XRGrabInteractable
            grabInteractable.attachTransform = attachObj.transform;
            
            Debug.Log("建立 Attach Transform 到手杖");
        }
        
        // 5. 確保手杖可以與其他物件互動
        var interactable = cane.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogWarning("手杖沒有 XRBaseInteractable 組件");
        }
        
        // 6. 設定手杖層級（避免與其他物件衝突）
        cane.layer = LayerMask.NameToLayer("Default");
        
        Debug.Log($"手杖 {cane.name} 修復完成！");
        Debug.Log($"- Rigidbody: {(rigidbody != null ? "✓" : "✗")}");
        Debug.Log($"- Collider: {(collider != null ? "✓" : "✗")}");
        Debug.Log($"- XRGrabInteractable: {(grabInteractable != null ? "✓" : "✗")}");
        Debug.Log($"- Attach Transform: {(attachTransform != null ? "✓" : "✗")}");
    }
    
    [ContextMenu("檢查手杖狀態")]
    public void CheckCaneStatus()
    {
        GameObject cane = GameObject.Find(caneName);
        if (cane == null)
        {
            Debug.LogError($"找不到手杖物件: {caneName}");
            return;
        }
        
        Debug.Log($"=== 手杖 {cane.name} 狀態檢查 ===");
        Debug.Log($"Rigidbody: {cane.GetComponent<Rigidbody>() != null}");
        Debug.Log($"Collider: {cane.GetComponent<Collider>() != null}");
        Debug.Log($"XRGrabInteractable: {cane.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() != null}");
        Debug.Log($"Attach Transform: {cane.transform.Find("Attach") != null}");
        Debug.Log($"Layer: {LayerMask.LayerToName(cane.layer)}");
    }
}






