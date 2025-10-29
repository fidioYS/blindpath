using UnityEngine;

namespace VRVisual
{
    /// <summary>
    /// 最終 VR Outline 腳本 - 完全避免所有 Shader 相容性問題
    /// 使用最簡單的材質替換方式實現邊框效果
    /// </summary>
    public class FinalVROutline : MonoBehaviour
    {
        [Header("邊框設定")]
        [Tooltip("邊框顏色")]
        public Color outlineColor = Color.yellow;
        
        [Tooltip("邊框強度 (0.1-5.0)")]
        [Range(0.1f, 5.0f)]
        public float outlineIntensity = 2.0f;
        
        [Header("動畫效果")]
        [Tooltip("是否使用脈衝效果")]
        public bool usePulse = true;
        
        [Tooltip("脈衝速度")]
        [Range(0.5f, 5.0f)]
        public float pulseSpeed = 2.0f;
        
        [Tooltip("脈衝強度")]
        [Range(0.1f, 2.0f)]
        public float pulseIntensity = 0.5f;
        
        [Header("顯示控制")]
        [Tooltip("是否顯示邊框")]
        public bool showOutline = true;
        
        [Tooltip("淡入時間")]
        [Range(0.1f, 2.0f)]
        public float fadeInTime = 0.3f;
        
        [Tooltip("淡出時間")]
        [Range(0.1f, 2.0f)]
        public float fadeOutTime = 0.5f;
        
        [Header("自動設定")]
        [Tooltip("自動設定邊框")]
        public bool autoSetup = true;
        
        [Tooltip("包含子物件")]
        public bool includeChildren = true;
        
        // 私有變數
        private Renderer[] targetRenderers;
        private Material[][] originalMaterials;
        private Material[] outlineMaterials;
        private bool isOutlineActive = false;
        private float currentIntensity = 1.0f;
        private float pulseTime = 0.0f;
        private Coroutine fadeCoroutine;
        
        private void Start()
        {
            if (autoSetup)
            {
                SetupOutline();
            }
            
            if (showOutline)
            {
                ShowOutline();
            }
        }
        
        private void Update()
        {
            if (isOutlineActive && usePulse)
            {
                UpdatePulse();
            }
        }
        
        private void OnDestroy()
        {
            RestoreOriginalMaterials();
        }
        
        /// <summary>
        /// 設定邊框
        /// </summary>
        [ContextMenu("設定邊框")]
        public void SetupOutline()
        {
            // 取得所有 Renderer
            if (includeChildren)
            {
                targetRenderers = GetComponentsInChildren<Renderer>();
            }
            else
            {
                var renderer = GetComponent<Renderer>();
                targetRenderers = renderer != null ? new Renderer[] { renderer } : new Renderer[0];
            }
            
            if (targetRenderers.Length == 0)
            {
                Debug.LogWarning($"[FinalVROutline] {gameObject.name} 沒有找到 Renderer 組件！");
                return;
            }
            
            // 儲存原始材質
            originalMaterials = new Material[targetRenderers.Length][];
            outlineMaterials = new Material[targetRenderers.Length];
            
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] == null) continue;
                
                // 儲存原始材質
                originalMaterials[i] = new Material[targetRenderers[i].materials.Length];
                for (int j = 0; j < targetRenderers[i].materials.Length; j++)
                {
                    originalMaterials[i][j] = targetRenderers[i].materials[j];
                }
                
                // 建立邊框材質
                CreateOutlineMaterial(i);
            }
            
            Debug.Log($"[FinalVROutline] 已設定 {targetRenderers.Length} 個 Renderer 的邊框");
        }
        
        /// <summary>
        /// 建立邊框材質 - 使用最簡單的材質，避免所有 Shader 問題
        /// </summary>
        private void CreateOutlineMaterial(int index)
        {
            // 使用最簡單的材質，避免所有 Shader 相容性問題
            Material outlineMaterial = new Material(Shader.Find("Sprites/Default"));
            if (outlineMaterial == null)
            {
                // 備用方案：使用最簡單的材質
                outlineMaterial = new Material(Shader.Find("Legacy Shaders/Diffuse"));
            }
            
            if (outlineMaterial == null)
            {
                // 最後備用方案：使用內建材質
                outlineMaterial = new Material(Shader.Find("Standard"));
            }
            
            if (outlineMaterial == null)
            {
                Debug.LogError("[FinalVROutline] 找不到可用的 Shader！");
                return;
            }
            
            // 設定材質屬性
            outlineMaterial.color = outlineColor * outlineIntensity;
            
            // 設定為發光材質
            if (outlineMaterial.HasProperty("_EmissionColor"))
            {
                outlineMaterial.SetColor("_EmissionColor", outlineColor * outlineIntensity);
                outlineMaterial.EnableKeyword("_EMISSION");
            }
            
            // 設定為不透明
            outlineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            outlineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            outlineMaterial.SetInt("_ZWrite", 1);
            outlineMaterial.renderQueue = 3000;
            
            outlineMaterials[index] = outlineMaterial;
        }
        
        /// <summary>
        /// 顯示邊框
        /// </summary>
        [ContextMenu("顯示邊框")]
        public void ShowOutline()
        {
            if (targetRenderers == null || targetRenderers.Length == 0)
            {
                SetupOutline();
            }
            
            if (targetRenderers == null || targetRenderers.Length == 0)
            {
                Debug.LogWarning("[FinalVROutline] 沒有找到可用的 Renderer 來顯示邊框");
                return;
            }
            
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            
            fadeCoroutine = StartCoroutine(ShowOutlineCoroutine());
        }
        
        /// <summary>
        /// 隱藏邊框
        /// </summary>
        [ContextMenu("隱藏邊框")]
        public void HideOutline()
        {
            if (!isOutlineActive) return;
            
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            
            fadeCoroutine = StartCoroutine(HideOutlineCoroutine());
        }
        
        /// <summary>
        /// 切換邊框顯示
        /// </summary>
        [ContextMenu("切換邊框")]
        public void ToggleOutline()
        {
            if (isOutlineActive)
            {
                HideOutline();
            }
            else
            {
                ShowOutline();
            }
        }
        
        /// <summary>
        /// 設定邊框顏色
        /// </summary>
        public void SetOutlineColor(Color color)
        {
            outlineColor = color;
            UpdateOutlineMaterials();
        }
        
        /// <summary>
        /// 設定邊框強度
        /// </summary>
        public void SetOutlineIntensity(float intensity)
        {
            outlineIntensity = Mathf.Clamp(intensity, 0.1f, 5.0f);
            UpdateOutlineMaterials();
        }
        
        /// <summary>
        /// 顯示邊框協程
        /// </summary>
        private System.Collections.IEnumerator ShowOutlineCoroutine()
        {
            isOutlineActive = true;
            
            // 淡入效果
            float elapsedTime = 0.0f;
            while (elapsedTime < fadeInTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / fadeInTime);
                currentIntensity = alpha;
                UpdateOutlineMaterials();
                yield return null;
            }
            
            currentIntensity = 1.0f;
            UpdateOutlineMaterials();
            fadeCoroutine = null;
        }
        
        /// <summary>
        /// 隱藏邊框協程
        /// </summary>
        private System.Collections.IEnumerator HideOutlineCoroutine()
        {
            // 淡出效果
            float elapsedTime = 0.0f;
            while (elapsedTime < fadeOutTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeOutTime);
                currentIntensity = alpha;
                UpdateOutlineMaterials();
                yield return null;
            }
            
            isOutlineActive = false;
            RestoreOriginalMaterials();
            fadeCoroutine = null;
        }
        
        /// <summary>
        /// 更新脈衝效果
        /// </summary>
        private void UpdatePulse()
        {
            pulseTime += Time.deltaTime * pulseSpeed;
            float pulseValue = (Mathf.Sin(pulseTime) + 1.0f) * 0.5f;
            float intensity = 1.0f + (pulseValue * pulseIntensity);
            currentIntensity = intensity;
            UpdateOutlineMaterials();
        }
        
        /// <summary>
        /// 更新邊框材質
        /// </summary>
        private void UpdateOutlineMaterials()
        {
            if (targetRenderers == null || outlineMaterials == null) return;
            
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] == null || outlineMaterials[i] == null) continue;
                
                if (isOutlineActive)
                {
                    // 設定邊框材質
                    Material[] materials = new Material[targetRenderers[i].materials.Length];
                    for (int j = 0; j < materials.Length; j++)
                    {
                        materials[j] = outlineMaterials[i];
                    }
                    targetRenderers[i].materials = materials;
                    
                    // 更新材質參數
                    Color finalColor = outlineColor * (outlineIntensity * currentIntensity);
                    outlineMaterials[i].color = finalColor;
                    
                    if (outlineMaterials[i].HasProperty("_EmissionColor"))
                    {
                        outlineMaterials[i].SetColor("_EmissionColor", finalColor);
                    }
                }
            }
        }
        
        /// <summary>
        /// 恢復原始材質
        /// </summary>
        private void RestoreOriginalMaterials()
        {
            if (targetRenderers == null || originalMaterials == null) return;
            
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] == null || originalMaterials[i] == null) continue;
                
                targetRenderers[i].materials = originalMaterials[i];
            }
        }
        
        /// <summary>
        /// 清除邊框設定
        /// </summary>
        [ContextMenu("清除邊框")]
        public void ClearOutline()
        {
            RestoreOriginalMaterials();
            
            // 清理材質
            if (outlineMaterials != null)
            {
                foreach (var material in outlineMaterials)
                {
                    if (material != null)
                    {
                        DestroyImmediate(material);
                    }
                }
            }
            
            targetRenderers = null;
            originalMaterials = null;
            outlineMaterials = null;
            isOutlineActive = false;
        }
        
        /// <summary>
        /// 檢查邊框是否啟用
        /// </summary>
        public bool IsOutlineActive()
        {
            return isOutlineActive;
        }
        
        /// <summary>
        /// 取得邊框設定
        /// </summary>
        public (Color color, float intensity) GetOutlineSettings()
        {
            return (outlineColor, outlineIntensity);
        }
    }
}




