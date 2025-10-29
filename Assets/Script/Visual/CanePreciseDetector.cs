using UnityEngine;
using System.Collections.Generic;
using VRVisual;

namespace VRCaneDetection
{
    /// <summary>
    /// 手杖精確接觸檢測系統
    /// 只顯示被碰到的邊緣部分的外框效果
    /// </summary>
    public class CanePreciseDetector : MonoBehaviour
    {
        [Header("手杖設定")]
        [Tooltip("手杖尖端位置")]
        public Transform caneTip;
        
        [Tooltip("手杖尖端半徑")]
        [Range(0.01f, 0.1f)]
        public float caneTipRadius = 0.02f;
        
        [Tooltip("檢測精度")]
        [Range(0.001f, 0.01f)]
        public float detectionPrecision = 0.005f;
        
        [Header("邊緣檢測")]
        [Tooltip("邊緣檢測距離")]
        [Range(0.1f, 1f)]
        public float edgeDetectionDistance = 0.3f;
        
        [Tooltip("邊緣角度閾值")]
        [Range(0f, 90f)]
        public float edgeAngleThreshold = 45f;
        
        [Tooltip("可檢測的圖層")]
        public LayerMask detectableLayers = -1;
        
        [Header("外框設定")]
        [Tooltip("外框顏色")]
        public Color outlineColor = Color.yellow;
        
        [Tooltip("外框強度")]
        [Range(0.1f, 5f)]
        public float outlineIntensity = 2f;
        
        [Tooltip("外框持續時間")]
        [Range(0.5f, 10f)]
        public float outlineDuration = 3f;
        
        [Tooltip("外框大小")]
        [Range(0.1f, 2f)]
        public float outlineSize = 0.5f;
        
        [Header("調試")]
        [Tooltip("顯示調試信息")]
        public bool showDebugInfo = false;
        
        [Tooltip("顯示檢測線")]
        public bool showDebugLines = true;
        
        // 私有變數
        private Dictionary<GameObject, List<ContactPoint>> contactPoints = new Dictionary<GameObject, List<ContactPoint>>();
        private Dictionary<GameObject, Coroutine> outlineCoroutines = new Dictionary<GameObject, Coroutine>();
        private RaycastHit[] hitCache = new RaycastHit[10];
        
        // 接觸點結構
        [System.Serializable]
        public struct ContactPoint
        {
            public Vector3 position;
            public Vector3 normal;
            public float timestamp;
            public GameObject targetObject;
            
            public ContactPoint(Vector3 pos, Vector3 norm, GameObject obj)
            {
                position = pos;
                normal = norm;
                timestamp = Time.time;
                targetObject = obj;
            }
        }
        
        private void Start()
        {
            // 自動獲取手杖尖端
            if (caneTip == null)
            {
                caneTip = transform;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[CanePreciseDetector] 初始化完成 - 尖端半徑: {caneTipRadius}");
            }
        }
        
        private void Update()
        {
            DetectPreciseContact();
        }
        
        /// <summary>
        /// 精確接觸檢測
        /// </summary>
        private void DetectPreciseContact()
        {
            if (caneTip == null) return;
            
            Vector3 tipPosition = caneTip.position;
            Vector3 tipDirection = caneTip.forward;
            
            // 多方向射線檢測
            DetectContactPoints(tipPosition, tipDirection);
            
            // 檢測邊緣
            DetectEdges(tipPosition);
        }
        
        /// <summary>
        /// 檢測接觸點
        /// </summary>
        private void DetectContactPoints(Vector3 position, Vector3 direction)
        {
            // 使用多個射線檢測不同方向
            Vector3[] directions = {
                direction,
                direction + Vector3.up * 0.1f,
                direction + Vector3.down * 0.1f,
                direction + Vector3.left * 0.1f,
                direction + Vector3.right * 0.1f
            };
            
            foreach (Vector3 dir in directions)
            {
                int hitCount = Physics.RaycastNonAlloc(
                    position, 
                    dir, 
                    hitCache, 
                    caneTipRadius * 2f, 
                    detectableLayers
                );
                
                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit hit = hitCache[i];
                    ProcessContactPoint(hit);
                }
            }
        }
        
        /// <summary>
        /// 檢測邊緣
        /// </summary>
        private void DetectEdges(Vector3 position)
        {
            // 檢測周圍的邊緣
            for (float angle = 0; angle < 360; angle += 10)
            {
                Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
                
                if (Physics.Raycast(position, direction, out RaycastHit hit, edgeDetectionDistance, detectableLayers))
                {
                    // 檢查是否為邊緣
                    if (IsEdge(hit))
                    {
                        ProcessEdgeContact(hit);
                    }
                }
            }
        }
        
        /// <summary>
        /// 處理接觸點
        /// </summary>
        private void ProcessContactPoint(RaycastHit hit)
        {
            GameObject targetObject = hit.collider.gameObject;
            Vector3 contactPosition = hit.point;
            Vector3 contactNormal = hit.normal;
            
            // 檢查是否為新的接觸點
            if (!IsExistingContact(targetObject, contactPosition))
            {
                ContactPoint newContact = new ContactPoint(contactPosition, contactNormal, targetObject);
                
                // 添加到接觸點列表
                if (!contactPoints.ContainsKey(targetObject))
                {
                    contactPoints[targetObject] = new List<ContactPoint>();
                }
                
                contactPoints[targetObject].Add(newContact);
                
                // 顯示局部外框
                ShowLocalOutline(targetObject, contactPosition, contactNormal);
                
                if (showDebugInfo)
                {
                    Debug.Log($"[CanePreciseDetector] 檢測到接觸點: {targetObject.name} at {contactPosition}");
                }
            }
        }
        
        /// <summary>
        /// 處理邊緣接觸
        /// </summary>
        private void ProcessEdgeContact(RaycastHit hit)
        {
            GameObject targetObject = hit.collider.gameObject;
            Vector3 edgePosition = hit.point;
            Vector3 edgeNormal = hit.normal;
            
            // 檢查是否為樓梯邊緣
            if (IsStairEdge(targetObject, edgePosition, edgeNormal))
            {
                ShowStairEdgeOutline(targetObject, edgePosition, edgeNormal);
                
                if (showDebugInfo)
                {
                    Debug.Log($"[CanePreciseDetector] 檢測到樓梯邊緣: {targetObject.name} at {edgePosition}");
                }
            }
        }
        
        /// <summary>
        /// 檢查是否為邊緣
        /// </summary>
        private bool IsEdge(RaycastHit hit)
        {
            // 檢查法向量角度
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > edgeAngleThreshold;
        }
        
        /// <summary>
        /// 檢查是否為樓梯邊緣
        /// </summary>
        private bool IsStairEdge(GameObject obj, Vector3 position, Vector3 normal)
        {
            // 檢查物件標籤或名稱
            if (obj.CompareTag("Stair") || obj.name.Contains("Stair") || obj.name.Contains("Step"))
            {
                return true;
            }
            
            // 檢查法向量（樓梯邊緣通常是垂直的）
            float verticalAngle = Vector3.Angle(normal, Vector3.up);
            return verticalAngle > 60f && verticalAngle < 120f;
        }
        
        /// <summary>
        /// 檢查是否為現有接觸
        /// </summary>
        private bool IsExistingContact(GameObject obj, Vector3 position)
        {
            if (!contactPoints.ContainsKey(obj)) return false;
            
            foreach (ContactPoint contact in contactPoints[obj])
            {
                if (Vector3.Distance(contact.position, position) < detectionPrecision)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// 顯示局部外框
        /// </summary>
        private void ShowLocalOutline(GameObject obj, Vector3 position, Vector3 normal)
        {
            // 創建局部外框物件
            GameObject outlineObj = CreateLocalOutlineObject(obj, position, normal);
            
            // 設定外框參數
            FinalVROutline outline = outlineObj.GetComponent<FinalVROutline>();
            if (outline == null)
            {
                outline = outlineObj.AddComponent<FinalVROutline>();
            }
            
            outline.SetOutlineColor(outlineColor);
            outline.SetOutlineIntensity(outlineIntensity);
            outline.ShowOutline();
            
            // 設定自動隱藏
            if (outlineCoroutines.ContainsKey(outlineObj))
            {
                if (outlineCoroutines[outlineObj] != null)
                {
                    StopCoroutine(outlineCoroutines[outlineObj]);
                }
            }
            
            outlineCoroutines[outlineObj] = StartCoroutine(HideOutlineAfterDelay(outlineObj, outlineDuration));
        }
        
        /// <summary>
        /// 顯示樓梯邊緣外框
        /// </summary>
        private void ShowStairEdgeOutline(GameObject obj, Vector3 position, Vector3 normal)
        {
            // 創建樓梯邊緣外框
            GameObject edgeOutline = CreateStairEdgeOutline(obj, position, normal);
            
            // 設定外框參數
            FinalVROutline outline = edgeOutline.GetComponent<FinalVROutline>();
            if (outline == null)
            {
                outline = edgeOutline.AddComponent<FinalVROutline>();
            }
            
            outline.SetOutlineColor(Color.red); // 樓梯邊緣用紅色
            outline.SetOutlineIntensity(outlineIntensity * 1.5f); // 更強的外框
            outline.ShowOutline();
            
            // 設定自動隱藏
            if (outlineCoroutines.ContainsKey(edgeOutline))
            {
                if (outlineCoroutines[edgeOutline] != null)
                {
                    StopCoroutine(outlineCoroutines[edgeOutline]);
                }
            }
            
            outlineCoroutines[edgeOutline] = StartCoroutine(HideOutlineAfterDelay(edgeOutline, outlineDuration));
        }
        
        /// <summary>
        /// 創建局部外框物件
        /// </summary>
        private GameObject CreateLocalOutlineObject(GameObject parent, Vector3 position, Vector3 normal)
        {
            GameObject outlineObj = new GameObject($"LocalOutline_{parent.name}_{Time.time:F3}");
            outlineObj.transform.position = position;
            outlineObj.transform.parent = parent.transform;
            
            // 創建小的立方體作為外框
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = outlineObj.transform;
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one * outlineSize;
            
            // 移除碰撞器
            Collider collider = cube.GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }
            
            return outlineObj;
        }
        
        /// <summary>
        /// 創建樓梯邊緣外框
        /// </summary>
        private GameObject CreateStairEdgeOutline(GameObject parent, Vector3 position, Vector3 normal)
        {
            GameObject edgeOutline = new GameObject($"StairEdgeOutline_{parent.name}_{Time.time:F3}");
            edgeOutline.transform.position = position;
            edgeOutline.transform.parent = parent.transform;
            
            // 創建邊緣線條
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.transform.parent = edgeOutline.transform;
            line.transform.localPosition = Vector3.zero;
            line.transform.localScale = new Vector3(outlineSize * 2f, outlineSize * 0.2f, outlineSize * 0.2f);
            
            // 移除碰撞器
            Collider collider = line.GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }
            
            return edgeOutline;
        }
        
        /// <summary>
        /// 延遲隱藏外框
        /// </summary>
        private System.Collections.IEnumerator HideOutlineAfterDelay(GameObject outlineObj, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (outlineObj != null)
            {
                FinalVROutline outline = outlineObj.GetComponent<FinalVROutline>();
                if (outline != null)
                {
                    outline.HideOutline();
                }
                
                // 延遲銷毀物件
                yield return new WaitForSeconds(1f);
                if (outlineObj != null)
                {
                    DestroyImmediate(outlineObj);
                }
            }
            
            // 從協程字典中移除
            if (outlineCoroutines.ContainsKey(outlineObj))
            {
                outlineCoroutines.Remove(outlineObj);
            }
        }
        
        /// <summary>
        /// 清除所有接觸點
        /// </summary>
        [ContextMenu("清除所有接觸點")]
        public void ClearAllContacts()
        {
            // 停止所有協程
            foreach (var coroutine in outlineCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            outlineCoroutines.Clear();
            
            // 清除接觸點
            contactPoints.Clear();
            
            if (showDebugInfo)
            {
                Debug.Log("[CanePreciseDetector] 已清除所有接觸點");
            }
        }
        
        /// <summary>
        /// 設定手杖尖端
        /// </summary>
        public void SetCaneTip(Transform tip)
        {
            caneTip = tip;
        }
        
        /// <summary>
        /// 取得接觸統計
        /// </summary>
        public int GetContactCount()
        {
            int totalContacts = 0;
            foreach (var contacts in contactPoints.Values)
            {
                totalContacts += contacts.Count;
            }
            return totalContacts;
        }
        
        /// <summary>
        /// 在 Scene 視圖中顯示調試信息
        /// </summary>
        void OnDrawGizmos()
        {
            if (!showDebugLines) return;
            
            if (caneTip != null)
            {
                // 顯示手杖尖端
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(caneTip.position, caneTipRadius);
                
                // 顯示檢測方向
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(caneTip.position, caneTip.forward * caneTipRadius * 2f);
            }
            
            // 顯示接觸點
            Gizmos.color = Color.yellow;
            foreach (var contacts in contactPoints.Values)
            {
                foreach (ContactPoint contact in contacts)
                {
                    Gizmos.DrawWireSphere(contact.position, 0.05f);
                }
            }
        }
        
        private void OnDestroy()
        {
            ClearAllContacts();
        }
    }
}

