using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowOnTrigger : MonoBehaviour
{
    [Header("Glow Settings")]
    public Color glowColor = Color.cyan; // 發光顏色
    public float intensity = 2f;         // 發光強度
    public float fadeOutDuration = 0.25f; // 離開時漸暗秒數

    // 追蹤每個被碰到 GameObject 的重疊次數，避免多重接觸時誤熄滅
    private readonly Dictionary<GameObject, int> _contactCountByObject = new Dictionary<GameObject, int>();
    // 針對各個 Renderer 管理各自的淡出協程
    private readonly Dictionary<Renderer, Coroutine> _fadeOutByRenderer = new Dictionary<Renderer, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        HandleEnter(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleExit(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleEnter(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        HandleExit(collision.gameObject);
    }

    private void HandleEnter(GameObject target)
    {
        if (target == null) return;

        if (_contactCountByObject.TryGetValue(target, out var count))
            _contactCountByObject[target] = count + 1;
        else
            _contactCountByObject[target] = 1;

        var renderers = target.GetComponentsInChildren<Renderer>(includeInactive: false);
        if (renderers == null || renderers.Length == 0) return;

        // 立即點亮，並取消進行中的淡出
        foreach (var rend in renderers)
        {
            if (rend == null) continue;
            if (_fadeOutByRenderer.TryGetValue(rend, out var co) && co != null)
            {
                StopCoroutine(co);
                _fadeOutByRenderer[rend] = null;
            }

            var mat = rend.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", glowColor * intensity);
        }
    }

    private void HandleExit(GameObject target)
    {
        if (target == null) return;
        if (!_contactCountByObject.TryGetValue(target, out var count)) return;

        count -= 1;
        if (count > 0)
        {
            _contactCountByObject[target] = count;
            return;
        }

        _contactCountByObject.Remove(target);

        var renderers = target.GetComponentsInChildren<Renderer>(includeInactive: false);
        if (renderers == null || renderers.Length == 0) return;

        foreach (var rend in renderers)
        {
            if (rend == null) continue;
            // 啟動漸變到黑
            if (_fadeOutByRenderer.TryGetValue(rend, out var running) && running != null)
                StopCoroutine(running);
            var co = StartCoroutine(FadeEmissionToBlack(rend));
            _fadeOutByRenderer[rend] = co;
        }
    }

    private IEnumerator FadeEmissionToBlack(Renderer rend)
    {
        if (rend == null) yield break;
        var mat = rend.material;
        mat.EnableKeyword("_EMISSION");

        Color startColor = mat.GetColor("_EmissionColor");
        Color endColor = Color.black;

        float duration = Mathf.Max(0.01f, fadeOutDuration);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            mat.SetColor("_EmissionColor", Color.Lerp(startColor, endColor, k));
            yield return null;
        }
        mat.SetColor("_EmissionColor", endColor);

        // 可選：若完全熄滅可關閉關鍵字
        // mat.DisableKeyword("_EMISSION");

        _fadeOutByRenderer[rend] = null;
    }
}
