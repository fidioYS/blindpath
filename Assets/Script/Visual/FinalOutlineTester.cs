using UnityEngine;
using VRVisual;

namespace VRVisual
{
    /// <summary>
    /// 最終 Outline 測試腳本
    /// </summary>
    public class FinalOutlineTester : MonoBehaviour
    {
        [Header("測試設定")]
        [Tooltip("測試物件")]
        public GameObject testObject;
        
        [Tooltip("自動建立測試物件")]
        public bool createTestObject = true;
        
        [Tooltip("測試顏色")]
        public Color[] testColors = { Color.yellow, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta };
        
        [Tooltip("測試強度")]
        public float[] testIntensities = { 1.0f, 2.0f, 3.0f, 5.0f };
        
        [Header("控制")]
        [Tooltip("當前測試索引")]
        public int currentColorIndex = 0;
        public int currentIntensityIndex = 0;
        
        private FinalVROutline outlineScript;
        
        private void Start()
        {
            SetupTest();
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        /// <summary>
        /// 設定測試
        /// </summary>
        private void SetupTest()
        {
            if (createTestObject && testObject == null)
            {
                CreateTestObject();
            }
            
            if (testObject != null)
            {
                outlineScript = testObject.GetComponent<FinalVROutline>();
                if (outlineScript == null)
                {
                    outlineScript = testObject.AddComponent<FinalVROutline>();
                }
            }
        }
        
        /// <summary>
        /// 建立測試物件
        /// </summary>
        private void CreateTestObject()
        {
            // 建立一個簡單的立方體
            testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.name = "FinalOutlineTestObject";
            testObject.transform.position = transform.position + Vector3.forward * 2;
            testObject.transform.localScale = Vector3.one * 2;
            
            // 添加顏色
            var renderer = testObject.GetComponent<Renderer>();
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = Color.white;
            renderer.material = material;
            
            Debug.Log("[FinalOutlineTester] 已建立測試物件");
        }
        
        /// <summary>
        /// 處理輸入
        /// </summary>
        private void HandleInput()
        {
            if (outlineScript == null) return;
            
            // 切換邊框顯示
            if (Input.GetKeyDown(KeyCode.O))
            {
                outlineScript.ToggleOutline();
                Debug.Log($"[FinalOutlineTester] 切換邊框顯示: {outlineScript.IsOutlineActive()}");
            }
            
            // 切換顏色
            if (Input.GetKeyDown(KeyCode.C))
            {
                currentColorIndex = (currentColorIndex + 1) % testColors.Length;
                outlineScript.SetOutlineColor(testColors[currentColorIndex]);
                Debug.Log($"[FinalOutlineTester] 切換顏色: {testColors[currentColorIndex]}");
            }
            
            // 切換強度
            if (Input.GetKeyDown(KeyCode.I))
            {
                currentIntensityIndex = (currentIntensityIndex + 1) % testIntensities.Length;
                outlineScript.SetOutlineIntensity(testIntensities[currentIntensityIndex]);
                Debug.Log($"[FinalOutlineTester] 切換強度: {testIntensities[currentIntensityIndex]}");
            }
            
            // 重新設定
            if (Input.GetKeyDown(KeyCode.R))
            {
                outlineScript.ClearOutline();
                outlineScript.SetupOutline();
                Debug.Log("[FinalOutlineTester] 重新設定邊框");
            }
        }
        
        /// <summary>
        /// 顯示控制說明
        /// </summary>
        private void OnGUI()
        {
            if (outlineScript == null) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("Final Outline 測試控制:");
            GUILayout.Label("O - 切換邊框顯示");
            GUILayout.Label("C - 切換顏色");
            GUILayout.Label("I - 切換強度");
            GUILayout.Label("R - 重新設定");
            GUILayout.Space(10);
            GUILayout.Label($"當前狀態: {(outlineScript.IsOutlineActive() ? "啟用" : "停用")}");
            GUILayout.Label($"當前顏色: {testColors[currentColorIndex]}");
            GUILayout.Label($"當前強度: {testIntensities[currentIntensityIndex]}");
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// 測試所有設定
        /// </summary>
        [ContextMenu("測試所有設定")]
        public void TestAllSettings()
        {
            if (outlineScript == null) return;
            
            Debug.Log("[FinalOutlineTester] 開始測試所有設定...");
            
            // 測試顏色
            foreach (var color in testColors)
            {
                outlineScript.SetOutlineColor(color);
                Debug.Log($"[FinalOutlineTester] 測試顏色: {color}");
            }
            
            // 測試強度
            foreach (var intensity in testIntensities)
            {
                outlineScript.SetOutlineIntensity(intensity);
                Debug.Log($"[FinalOutlineTester] 測試強度: {intensity}");
            }
            
            Debug.Log("[FinalOutlineTester] 測試完成！");
        }
        
        /// <summary>
        /// 設定測試物件
        /// </summary>
        public void SetTestObject(GameObject obj)
        {
            testObject = obj;
            if (obj != null)
            {
                outlineScript = obj.GetComponent<FinalVROutline>();
                if (outlineScript == null)
                {
                    outlineScript = obj.AddComponent<FinalVROutline>();
                }
            }
        }
    }
}




