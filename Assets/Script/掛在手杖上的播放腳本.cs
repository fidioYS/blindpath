using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CaneImpactPlayer : MonoBehaviour
{
    [Header("播放方式")]
    public bool useAttachedAudioSource = true; // 勾選：用手杖上的 AudioSource；否則在碰撞點建立暫時音源
    public float spatialBlend = 1f;            // 1 是 3D 聲音

    private AudioSource audioSource;
    private Rigidbody caneRigidbody;

    [Header("Trigger 支援")]
    public bool enableTriggerImpact = true;    // 若使用 Trigger Collider，啟用後也能出聲
    public float minTriggerImpactSpeed = 0.3f; // 估算速度低於此值不出聲
    public bool enableDebugLogs = false;       // 偵錯：印出觸發與速度

    private Vector3 lastPosition;

    void Awake()
    {
        if (useAttachedAudioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
            audioSource.spatialBlend = spatialBlend;
        }

        caneRigidbody = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayImpact(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        // 若需要拖拉摩擦也偶爾出聲，保留；若不需要可移除
        // PlayImpact(collision);
    }

    private void PlayImpact(Collision collision)
    {
        var target = collision.collider.GetComponentInParent<ImpactMaterial>();
        if (target == null) return;

        float relativeSpeed = collision.relativeVelocity.magnitude;
        if (!target.TryGetPlayableClip(out var clip, out var volume, out var pitch, relativeSpeed)) return;

        if (useAttachedAudioSource && audioSource != null)
        {
            var contactPoint = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
            audioSource.transform.position = contactPoint; // 讓聲音更貼近碰撞點
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            Vector3 point = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
            var temp = new GameObject("CaneImpactAudio");
            temp.transform.position = point;
            var src = temp.AddComponent<AudioSource>();
            src.spatialBlend = spatialBlend;
            src.pitch = pitch;
            src.PlayOneShot(clip, volume);
            Object.Destroy(temp, clip.length + 0.1f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enableTriggerImpact) return;

        var target = other.GetComponentInParent<ImpactMaterial>();
        if (target == null) return;

        // 以手杖剛體速度估算撞擊速度
        float estimatedSpeed = 0f;
        if (caneRigidbody != null)
            estimatedSpeed = caneRigidbody.linearVelocity.magnitude; // 注意：使用 Rigidbody.velocity

        // 若剛體速度過低（XR 常見 isKinematic，由追蹤器驅動），改用 Transform 位移估算
        if (estimatedSpeed < 0.01f)
        {
            float dt = Mathf.Max(Time.deltaTime, 1e-4f);
            float distance = Vector3.Distance(transform.position, lastPosition);
            float transformSpeed = distance / dt;
            estimatedSpeed = transformSpeed;
        }

        if (enableDebugLogs)
            Debug.Log($"[CaneImpact] Trigger hit {other.name}, speed={estimatedSpeed:F2}");
        if (estimatedSpeed < minTriggerImpactSpeed) return;

        if (!target.TryGetPlayableClip(out var clip, out var volume, out var pitch, estimatedSpeed)) return;

        Vector3 point = other.ClosestPoint(transform.position);
        if (useAttachedAudioSource && audioSource != null)
        {
            audioSource.transform.position = point;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            var temp = new GameObject("CaneImpactAudio");
            temp.transform.position = point;
            var src = temp.AddComponent<AudioSource>();
            src.spatialBlend = spatialBlend;
            src.pitch = pitch;
            src.PlayOneShot(clip, volume);
            Object.Destroy(temp, clip.length + 0.1f);
        }
    }
}