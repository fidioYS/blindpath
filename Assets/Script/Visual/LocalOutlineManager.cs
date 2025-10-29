using UnityEngine;
using System.Collections.Generic;
using VRVisual;

namespace VRCaneDetection
{
    /// <summary>
    /// 局部外框管理器
    /// 管理手杖接觸點的局部外框顯示
    /// </summary>
    public class LocalOutlineManager : MonoBehaviour
    {
        [Header("外框設定")]
        [Tooltip("外框材質")]
        public Material outlineMaterial;
        
        [Tooltip("外框顏色")]
        public Color outlineColor = Color.yellow;
        
        [Tooltip("外框強度")]
        [Range(0.1f, 5f)]
        public float outlineIntensity = 2f;
        
        [Tooltip("外框大小")]
        [Range(0.05f, 1f)]
        public float outlineSize = 0.2f;
        
        [Tooltip("外框持續時間")]
        [Range(0.5f, 10f)]
        public float outlineDuration = 3f;
        
        [Tooltip("淡出時間")]
        [Range(0.1f, 2f)]
        public float fadeOutTime = 0.5f;
        
        [Header("動畫效果")]
        [Tooltip("脈衝效果")]
        public bool usePulse = true;
        
        [Tooltip("脈衝速度")]
        [Range(0.5f, 5f)]
        public float pulseSpeed = 2f;
        
        [Tooltip("脈衝強度")]
        [Range(0.1f, 1f)]
        public float pulseIntensity = 0.3f;
        
        [Header("調試")]
        [Tooltip("顯示調試信息")]
        public bool showDebugInfo = false;
        
        // 私有變數
        private Dictionary<GameObject, LocalOutlineData> activeOutlines = new Dictionary<GameObject, LocalOutlineData>();
        private Queue<GameObject> outlinePool = new Queue<GameObject>();
        
        // 局部外框數據結構
        [System.Serializable]
        public struct LocalOutlineData
        {
            public GameObject outlineObject;
            public Renderer renderer;
            public Material originalMaterial;
            public Material outlineMaterial;
            public float startTime;
            public Vector3 contactPosition;
            public Vector3 contactNormal;
            public Coroutine fadeCoroutine;
            
            public LocalOutlineData(GameObject obj, Renderer rend, Material origMat, Material outMat, Vector3 pos, Vector3 norm)
            {
                outlineObject = obj;
                renderer = rend;
                originalMaterial = origMat;
                outlineMaterial = outMat;
                startTime = Time.time;
                contactPosition = pos;
                contactNormal = norm;
                fadeCoroutine = null;
            }
        }
        
        private void Start()
        {
            // 創建外框材質
            if (outlineMaterial == null)
            {
                CreateOutlineMaterial();
            }
            
            // 預創建外框物件池
            CreateOutlinePool();
        }
        
        private void Update()
        {
            // 更新脈衝效果
            if (usePulse)
            {
                UpdatePulseEffect();
            }
            
            // 清理過期的外框
            CleanupExpiredOutlines();
        }
        
        /// <summary>
        /// 創建外框材質
        /// </summary>
        private void CreateOutlineMaterial()
        {
            outlineMaterial = new Material(Shader.Find("Sprites/Default"));
            if (outlineMaterial == null)
            {
                outlineMaterial = new Material(Shader.Find("Legacy Shaders/Diffuse"));
            }
            
            if (outlineMaterial != null)
            {
                outlineMaterial.color = outlineColor * outlineIntensity;
                
                // 設定發光效果
                if (outlineMaterial.HasProperty("_EmissionColor"))
                {
                    outlineMaterial.SetColor("_EmissionColor", outlineColor * outlineIntensity);
                    outlineMaterial.EnableKeyword("_EMISSION");
                }
            }
        }
        
        /// <summary>
        /// 創建外框物件池
        /// </summary>
        private void CreateOutlinePool()
        {
            for (int i = 0; i < 20; i++)
            {
                GameObject outlineObj = CreateOutlineObject();
                outlineObj.SetActive(false);
                outlinePool.Enqueue(outlineObj);
            }
        }
        
        /// <summary>
        /// 從池中獲取外框物件
        /// </summary>
        private GameObject GetOutlineFromPool()
        {
            if (outlinePool.Count > 0)
            {
                GameObject outlineObj = outlinePool.Dequeue();
                outlineObj.SetActive(true);
                return outlineObj;
            }
            return null;
        }
        
        /// <summary>
        /// 創建外框物件
        /// </summary>
        private GameObject CreateOutlineObject()
        {
            GameObject outlineObj = new GameObject("LocalOutline");
            
            // 創建立方體
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
            
            // 獲取 Renderer
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 創建外框材質副本
                Material outlineMat = new Material(outlineMaterial);
                renderer.material = outlineMat;
            }
            
            return outlineObj;
        }
        
        /// <summary>
        /// 顯示局部外框
        /// </summary>
        public GameObject ShowLocalOutline(GameObject targetObject, Vector3 contactPosition, Vector3 contactNormal)
        {
            if (targetObject == null) return null;
            
            // 檢查是否已存在外框
            if (activeOutlines.ContainsKey(targetObject))
            {
                // 更新現有外框
                UpdateExistingOutline(targetObject, contactPosition, contactNormal);
                return activeOutlines[targetObject].outlineObject;
            }
            
            // 從池中獲取外框物件
            GameObject outlineObj = GetOutlineFromPool();
            if (outlineObj == null)
            {
                outlineObj = CreateOutlineObject();
            }
            
            // 設定外框位置
            outlineObj.transform.position = contactPosition;
            outlineObj.transform.parent = targetObject.transform;
            outlineObj.SetActive(true);
            
            // 獲取 Renderer
            Renderer renderer = outlineObj.GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("[LocalOutlineManager] 找不到 Renderer 組件！");
                return null;
            }
            
            // 創建外框材質
            Material outlineMat = new Material(outlineMaterial);
            Material originalMat = renderer.material;
            
            // 設定外框材質
            renderer.material = outlineMat;
            
            // 創建外框數據
            LocalOutlineData outlineData = new LocalOutlineData(
                outlineObj, 
                renderer, 
                originalMat, 
                outlineMat, 
                contactPosition, 
                contactNormal
            );
            
            // 添加到活躍外框列表
            activeOutlines[targetObject] = outlineData;
            
            // 設定自動隱藏
            outlineData.fadeCoroutine = StartCoroutine(FadeOutAfterDelay(targetObject, outlineDuration));
            activeOutlines[targetObject] = outlineData;
            
            if (showDebugInfo)
            {
                Debug.Log($"[LocalOutlineManager] 顯示局部外框: {targetObject.name} at {contactPosition}");
            }
            
            return outlineObj;
        }
        
        /// <summary>
        /// 更新現有外框
        /// </summary>
        private void UpdateExistingOutline(GameObject targetObject, Vector3 contactPosition, Vector3 contactNormal)
        {
            if (!activeOutlines.ContainsKey(targetObject)) return;
            
            LocalOutlineData data = activeOutlines[targetObject];
            
            // 更新位置
            data.outlineObject.transform.position = contactPosition;
            
            // 重置時間
            data.startTime = Time.time;
            
            // 重新設定淡出協程
            if (data.fadeCoroutine != null)
            {
                StopCoroutine(data.fadeCoroutine);
            }
            
            data.fadeCoroutine = StartCoroutine(FadeOutAfterDelay(targetObject, outlineDuration));
            activeOutlines[targetObject] = data;
        }
        
        /// <summary>
        /// 隱藏局部外框
        /// </summary>
        public void HideLocalOutline(GameObject targetObject)
        {
            if (!activeOutlines.ContainsKey(targetObject)) return;
            
            LocalOutlineData data = activeOutlines[targetObject];
            
            // 停止淡出協程
            if (data.fadeCoroutine != null)
            {
                StopCoroutine(data.fadeCoroutine);
            }
            
            // 開始淡出
            StartCoroutine(FadeOutCoroutine(targetObject));
        }
        
        /// <summary>
        /// 延遲淡出
        /// </summary>
        private System.Collections.IEnumerator FadeOutAfterDelay(GameObject targetObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return StartCoroutine(FadeOutCoroutine(targetObject));
        }
        
        /// <summary>
        /// 淡出協程
        /// </summary>
        private System.Collections.IEnumerator FadeOutCoroutine(GameObject targetObject)
        {
            if (!activeOutlines.ContainsKey(targetObject)) yield break;
            
            LocalOutlineData data = activeOutlines[targetObject];
            
            // 淡出效果
            float elapsedTime = 0f;
            Color startColor = data.outlineMaterial.color;
            
            while (elapsedTime < fadeOutTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutTime);
                
                Color newColor = startColor;
                newColor.a = alpha;
                data.outlineMaterial.color = newColor;
                
                yield return null;
            }
            
            // 隱藏外框
            HideOutline(targetObject);
        }
        
        /// <summary>
        /// 隱藏外框
        /// </summary>
        private void HideOutline(GameObject targetObject)
        {
            if (!activeOutlines.ContainsKey(targetObject)) return;
            
            LocalOutlineData data = activeOutlines[targetObject];
            
            // 停用外框物件
            data.outlineObject.SetActive(false);
            
            // 回收到池中
            outlinePool.Enqueue(data.outlineObject);
            
            // 從活躍列表中移除
            activeOutlines.Remove(targetObject);
            
            if (showDebugInfo)
            {
                Debug.Log($"[LocalOutlineManager] 隱藏局部外框: {targetObject.name}");
            }
        }
        
        /// <summary>
        /// 更新脈衝效果
        /// </summary>
        private void UpdatePulseEffect()
        {
            if (!usePulse) return;
            
            float pulseValue = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            float intensity = 1f + (pulseValue * pulseIntensity);
            
            foreach (var data in activeOutlines.Values)
            {
                if (data.outlineMaterial != null)
                {
                    Color pulseColor = outlineColor * (outlineIntensity * intensity);
                    data.outlineMaterial.color = pulseColor;
                    
                    if (data.outlineMaterial.HasProperty("_EmissionColor"))
                    {
                        data.outlineMaterial.SetColor("_EmissionColor", pulseColor);
                    }
                }
            }
        }
        
        /// <summary>
        /// 清理過期的外框
        /// </summary>
        private void CleanupExpiredOutlines()
        {
            List<GameObject> toRemove = new List<GameObject>();
            
            foreach (var kvp in activeOutlines)
            {
                LocalOutlineData data = kvp.Value;
                
                // 檢查是否過期
                if (Time.time - data.startTime > outlineDuration + fadeOutTime + 1f)
                {
                    toRemove.Add(kvp.Key);
                }
            }
            
            foreach (GameObject key in toRemove)
            {
                HideOutline(key);
            }
        }
        
        /// <summary>
        /// 清除所有外框
        /// </summary>
        [ContextMenu("清除所有外框")]
        public void ClearAllOutlines()
        {
            List<GameObject> keys = new List<GameObject>(activeOutlines.Keys);
            
            foreach (GameObject key in keys)
            {
                HideOutline(key);
            }
            
            if (showDebugInfo)
            {
                Debug.Log("[LocalOutlineManager] 已清除所有外框");
            }
        }
        
        /// <summary>
        /// 設定外框參數
        /// </summary>
        public void SetOutlineParameters(Color color, float intensity, float size)
        {
            outlineColor = color;
            outlineIntensity = intensity;
            outlineSize = size;
            
            // 更新現有外框
            foreach (var data in activeOutlines.Values)
            {
                if (data.outlineMaterial != null)
                {
                    data.outlineMaterial.color = outlineColor * outlineIntensity;
                    
                    if (data.outlineMaterial.HasProperty("_EmissionColor"))
                    {
                        data.outlineMaterial.SetColor("_EmissionColor", outlineColor * outlineIntensity);
                    }
                }
            }
        }
        
        /// <summary>
        /// 取得活躍外框數量
        /// </summary>
        public int GetActiveOutlineCount()
        {
            return activeOutlines.Count;
        }
        
        /// <summary>
        /// 檢查物件是否有外框
        /// </summary>
        public bool HasOutline(GameObject obj)
        {
            return activeOutlines.ContainsKey(obj);
        }
        
        private void OnDestroy()
        {
            ClearAllOutlines();
        }
    }
}
