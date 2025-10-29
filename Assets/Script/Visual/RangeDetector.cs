using UnityEngine;
using System.Collections.Generic;

namespace VRVisual
{
    /// <summary>
    /// 範圍探測器 - 檢測範圍內的物體並顯示外框
    /// 與 FinalVROutline 完美配合，兼容性最佳
    /// </summary>
    public class RangeDetector : MonoBehaviour
    {
        [Header("探測設定")]
        [Tooltip("檢測半徑")]
        [Range(0.5f, 10f)]
        public float detectionRadius = 3f;
        
        [Tooltip("檢測高度範圍")]
        [Range(0.5f, 5f)]
        public float detectionHeight = 2f;
        
        [Tooltip("可檢測的圖層")]
        public LayerMask detectableLayers = -1;
        
        [Tooltip("檢測間隔 (秒)")]
        [Range(0.01f, 0.5f)]
        public float detectionInterval = 0.1f;
        
        [Header("粒子效果")]
        [Tooltip("檢測粒子效果")]
        public ParticleSystem detectionParticles;
        
        [Tooltip("跟隨粒子效果")]
        public bool followParticles = true;
        
        [Tooltip("粒子顏色變化")]
        public bool useParticleColorChange = true;
        
        [Header("外框設定")]
        [Tooltip("外框顏色")]
        public Color outlineColor = Color.cyan;
        
        [Tooltip("外框強度")]
        [Range(0.1f, 5f)]
        public float outlineIntensity = 2f;
        
        [Tooltip("外框持續時間 (0 = 永久)")]
        [Range(0f, 10f)]
        public float outlineDuration = 3f;
        
        [Header("調試")]
        [Tooltip("顯示檢測範圍")]
        public bool showGizmos = true;
        
        [Tooltip("顯示檢測信息")]
        public bool showDebugInfo = false;
        
        // 私有變數
        private List<GameObject> currentDetectedObjects = new List<GameObject>();
        private List<GameObject> previousDetectedObjects = new List<GameObject>();
        private Dictionary<GameObject, Coroutine> outlineCoroutines = new Dictionary<GameObject, Coroutine>();
        private float lastDetectionTime = 0f;
        
        // 快取變數
        private Collider[] colliderCache = new Collider[50]; // 避免 GC 分配
        
        private void Start()
        {
            // 初始化檢測
            InitializeDetection();
        }
        
        private void Update()
        {
            // 按間隔檢測
            if (Time.time - lastDetectionTime >= detectionInterval)
            {
                DetectObjectsInRange();
                lastDetectionTime = Time.time;
            }
            
            // 更新粒子效果
            if (detectionParticles != null && followParticles)
            {
                UpdateParticleEffect();
            }
        }
        
        /// <summary>
        /// 初始化檢測
        /// </summary>
        private void InitializeDetection()
        {
            // 確保有 Collider 用於檢測
            if (GetComponent<Collider>() == null)
            {
                SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
                sphereCollider.radius = detectionRadius;
                sphereCollider.isTrigger = true;
            }
            
            // 初始化粒子效果
            if (detectionParticles != null)
            {
                var main = detectionParticles.main;
                main.startColor = Color.blue;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[RangeDetector] 初始化完成 - 檢測半徑: {detectionRadius}, 高度: {detectionHeight}");
            }
        }
        
        /// <summary>
        /// 檢測範圍內的物體
        /// </summary>
        private void DetectObjectsInRange()
        {
            // 清空當前檢測到的物體
            currentDetectedObjects.Clear();
            
            // 使用快取避免 GC 分配
            int objectCount = Physics.OverlapSphereNonAlloc(
                transform.position, 
                detectionRadius, 
                colliderCache, 
                detectableLayers
            );
            
            // 檢測範圍內的物體
            for (int i = 0; i < objectCount; i++)
            {
                Collider obj = colliderCache[i];
                if (obj == null) continue;
                
                // 檢查高度範圍
                float heightDiff = Mathf.Abs(obj.transform.position.y - transform.position.y);
                if (heightDiff <= detectionHeight)
                {
                    currentDetectedObjects.Add(obj.gameObject);
                }
            }
            
            // 處理新檢測到的物體
            foreach (GameObject obj in currentDetectedObjects)
            {
                if (!previousDetectedObjects.Contains(obj))
                {
                    ShowObjectOutline(obj);
                }
            }
            
            // 處理不再檢測到的物體
            foreach (GameObject obj in previousDetectedObjects)
            {
                if (!currentDetectedObjects.Contains(obj))
                {
                    HideObjectOutline(obj);
                }
            }
            
            // 更新列表
            previousDetectedObjects.Clear();
            previousDetectedObjects.AddRange(currentDetectedObjects);
            
            if (showDebugInfo && currentDetectedObjects.Count > 0)
            {
                Debug.Log($"[RangeDetector] 檢測到 {currentDetectedObjects.Count} 個物體");
            }
        }
        
        /// <summary>
        /// 顯示物體外框
        /// </summary>
        private void ShowObjectOutline(GameObject obj)
        {
            if (obj == null) return;
            
            // 使用 FinalVROutline 組件
            FinalVROutline outline = obj.GetComponent<FinalVROutline>();
            if (outline == null)
            {
                outline = obj.AddComponent<FinalVROutline>();
            }
            
            // 設定外框參數
            outline.SetOutlineColor(outlineColor);
            outline.SetOutlineIntensity(outlineIntensity);
            outline.ShowOutline();
            
            // 如果有持續時間，設定自動隱藏
            if (outlineDuration > 0f)
            {
                if (outlineCoroutines.ContainsKey(obj))
                {
                    if (outlineCoroutines[obj] != null)
                    {
                        StopCoroutine(outlineCoroutines[obj]);
                    }
                }
                
                outlineCoroutines[obj] = StartCoroutine(HideOutlineAfterDelay(obj, outlineDuration));
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[RangeDetector] 顯示 {obj.name} 的外框");
            }
        }
        
        /// <summary>
        /// 隱藏物體外框
        /// </summary>
        private void HideObjectOutline(GameObject obj)
        {
            if (obj == null) return;
            
            FinalVROutline outline = obj.GetComponent<FinalVROutline>();
            if (outline != null)
            {
                outline.HideOutline();
            }
            
            // 停止相關的協程
            if (outlineCoroutines.ContainsKey(obj))
            {
                if (outlineCoroutines[obj] != null)
                {
                    StopCoroutine(outlineCoroutines[obj]);
                }
                outlineCoroutines.Remove(obj);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[RangeDetector] 隱藏 {obj.name} 的外框");
            }
        }
        
        /// <summary>
        /// 延遲隱藏外框
        /// </summary>
        private System.Collections.IEnumerator HideOutlineAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            HideObjectOutline(obj);
        }
        
        /// <summary>
        /// 更新粒子效果
        /// </summary>
        private void UpdateParticleEffect()
        {
            if (detectionParticles == null) return;
            
            var main = detectionParticles.main;
            var emission = detectionParticles.emission;
            
            // 根據檢測到的物體數量調整粒子效果
            if (currentDetectedObjects.Count > 0)
            {
                // 有物體時增加粒子發射率
                emission.rateOverTime = currentDetectedObjects.Count * 20f;
                
                if (useParticleColorChange)
                {
                    // 根據物體數量改變顏色
                    if (currentDetectedObjects.Count == 1)
                    {
                        main.startColor = Color.green;
                    }
                    else if (currentDetectedObjects.Count <= 3)
                    {
                        main.startColor = Color.yellow;
                    }
                    else
                    {
                        main.startColor = Color.red;
                    }
                }
            }
            else
            {
                // 沒有物體時減少粒子發射率
                emission.rateOverTime = 5f;
                main.startColor = Color.blue;
            }
        }
        
        /// <summary>
        /// 手動觸發檢測
        /// </summary>
        [ContextMenu("手動檢測")]
        public void ManualDetection()
        {
            DetectObjectsInRange();
        }
        
        /// <summary>
        /// 清除所有外框
        /// </summary>
        [ContextMenu("清除所有外框")]
        public void ClearAllOutlines()
        {
            foreach (GameObject obj in currentDetectedObjects)
            {
                HideObjectOutline(obj);
            }
            
            currentDetectedObjects.Clear();
            previousDetectedObjects.Clear();
            
            // 停止所有協程
            foreach (var coroutine in outlineCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            outlineCoroutines.Clear();
            
            if (showDebugInfo)
            {
                Debug.Log("[RangeDetector] 已清除所有外框");
            }
        }
        
        /// <summary>
        /// 設定檢測參數
        /// </summary>
        public void SetDetectionParameters(float radius, float height, LayerMask layers)
        {
            detectionRadius = radius;
            detectionHeight = height;
            detectableLayers = layers;
            
            // 更新 Collider
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                sphereCollider.radius = detectionRadius;
            }
        }
        
        /// <summary>
        /// 取得檢測統計
        /// </summary>
        public (int currentCount, int previousCount) GetDetectionStats()
        {
            return (currentDetectedObjects.Count, previousDetectedObjects.Count);
        }
        
        /// <summary>
        /// 檢查物體是否在檢測範圍內
        /// </summary>
        public bool IsObjectInRange(GameObject obj)
        {
            return currentDetectedObjects.Contains(obj);
        }
        
        /// <summary>
        /// 在 Scene 視圖中顯示檢測範圍
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;
            
            // 顯示檢測範圍
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            // 顯示高度範圍
            Gizmos.color = Color.cyan;
            Vector3 top = transform.position + Vector3.up * detectionHeight;
            Vector3 bottom = transform.position - Vector3.up * detectionHeight;
            Gizmos.DrawLine(top, bottom);
            
            // 顯示檢測到的物體
            Gizmos.color = Color.green;
            foreach (GameObject obj in currentDetectedObjects)
            {
                if (obj != null)
                {
                    Gizmos.DrawLine(transform.position, obj.transform.position);
                }
            }
        }
        
        /// <summary>
        /// 清理資源
        /// </summary>
        private void OnDestroy()
        {
            ClearAllOutlines();
        }
    }
}

