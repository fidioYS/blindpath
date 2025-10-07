using UnityEngine;

public class GlowOnContact : MonoBehaviour
{
    public string targetTag = "Block";
    public Color glowColor = Color.cyan;
    public float intensity = 2f;

    // 可選：把原本的發光顏色記住，離開時還原
    private void SetGlow(Renderer rend, bool enable)
    {
        if (rend == null) return;

        var mat = rend.material;
        if (enable)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", glowColor * intensity);
        }
        else
        {
            // 關閉/還原 emission
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    // 物理碰撞（非 Trigger）
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            var rend = collision.gameObject.GetComponent<Renderer>();
            SetGlow(rend, true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            var rend = collision.gameObject.GetComponent<Renderer>();
            SetGlow(rend, false);
        }
    }

    // 觸發器
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            var rend = other.GetComponent<Renderer>();
            SetGlow(rend, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            var rend = other.GetComponent<Renderer>();
            SetGlow(rend, false);
        }
    }
}
