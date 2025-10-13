using UnityEngine;

[DisallowMultipleComponent]
public class ImpactMaterial : MonoBehaviour
{
    [Header("敲擊音效集合（可隨機）")]
    public AudioClip[] clips;

    [Header("音量/音高")]
    [Range(0f, 1f)] public float baseVolume = 0.9f;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    [Header("速度音量映射")]
    public float minImpactVelocity = 0.2f;  // 低於此速度不發聲
    public float maxImpactVelocity = 6f;    // 對應最大音量

    [Header("冷卻(避免連續觸發)")]
    public float playCooldown = 0.05f;

    private float lastPlayTime = -999f;

    public bool TryGetPlayableClip(out AudioClip clip, out float volume, out float pitch, float relativeSpeed)
    {
        clip = null;
        volume = 0f;
        pitch = 1f;

        if (clips == null || clips.Length == 0) return false;
        if (relativeSpeed < minImpactVelocity) return false;
        if (Time.time - lastPlayTime < playCooldown) return false;

        lastPlayTime = Time.time;

        float t = Mathf.InverseLerp(minImpactVelocity, maxImpactVelocity, relativeSpeed);
        volume = baseVolume * Mathf.Clamp01(t);
        pitch = Random.Range(minPitch, maxPitch);
        clip = clips[Random.Range(0, clips.Length)];
        return true;
    }
}