using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Rob08BulletsNet : NetworkBehaviour
{
    [Header("발사 연출")]
    [SerializeField] private ParticleSystem fireFX;
    [SerializeField] private ParticleSystem smokeFX;
    [SerializeField] private ParticleSystem bitsFX;
    [SerializeField] private AudioSource audioSource;

    [Header("데미지 데이터")]
    [SerializeField] private BulletData bulletData;
    [SerializeField] private Collider damageCollider;

    [Networked] private float attackInterval { get; set; }

    private readonly HashSet<GameObject> damagedTargets = new();
    private Coroutine stopRoutine;

    private void Awake()
    {
        if (damageCollider == null)
            damageCollider = GetComponent<Collider>();

        if (damageCollider != null)
            damageCollider.isTrigger = true;

        if (audioSource != null) {
            audioSource.spatialBlend = 1f;  // 3D
            audioSource.minDistance = 5f;   // 근거리 최대 볼륨
            audioSource.maxDistance = 30f;  // 이 거리 이후로 거의 안 들림 (필요에 맞게 조정)
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        damagedTargets.Clear();
        stopRoutine = null;

        if (damageCollider != null)
            damageCollider.enabled = false;

        PlayFx(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetFire(bool enable, float interval)
    {
        if (enable)
        {
            attackInterval = Mathf.Max(0.05f, interval);

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (stopRoutine != null)
            {
                StopCoroutine(stopRoutine);
                stopRoutine = null;
            }

            damagedTargets.Clear();
            ToggleDamage(true);
            PlayFx(true);
        }
        else
        {
            ToggleDamage(false);
            PlayFx(false);

            if (stopRoutine != null)
                StopCoroutine(stopRoutine);

            stopRoutine = StartCoroutine(DelayedCleanup());
        }
    }

    private void ToggleDamage(bool enable)
    {
        if (damageCollider != null)
            damageCollider.enabled = enable;
    }

    private void PlayFx(bool enable)
    {
        ToggleParticle(fireFX, enable);
        ToggleParticle(smokeFX, enable);
        ToggleParticle(bitsFX, enable);

        if (audioSource == null)
            return;

        if (enable)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private static void ToggleParticle(ParticleSystem ps, bool enable)
    {
        if (ps == null)
            return;

        if (enable)
            ps.Play();
        else
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (other.gameObject.layer == gameObject.layer)
            return;

        if (damagedTargets.Contains(other.gameObject))
            return;

        if (!other.TryGetComponent(out RobHp hp))
            return;

        hp.TakeDamage(bulletData.bulletDamage);
        damagedTargets.Add(other.gameObject);

        StartCoroutine(RemoveAfterDelay(other.gameObject, attackInterval));
    }

    private IEnumerator RemoveAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        damagedTargets.Remove(target);
    }

    private IEnumerator DelayedCleanup()
    {
        yield return new WaitForSeconds(2.5f);
        damagedTargets.Clear();
        stopRoutine = null;

        if (Object != null && Object.IsValid && Object.HasStateAuthority)
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }
}
