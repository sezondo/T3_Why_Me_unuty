using UnityEngine;
using DG.Tweening;
using Fusion;

public class Rob04BulletsNet : BulletsNet
{
    [SerializeField] private AudioClip destAudioClip;
    [Networked] private Vector3 TargetPos { get; set; }
    //[Networked] private NetworkBool HasTarget { get; set; }
    [Networked] private NetworkBool ArcPrepared { get; set; }
    private Tweener arcTween;
    private bool arcStarted;

    public override void Spawned()
    {

        base.Spawned();

        
        StartArc();
        

    }

    public override void FixedUpdateNetwork()
    {
        if (!arcStarted && ArcPrepared)
        StartArc();

        if (pendingDespawn && DespawnTimer.Expired(Runner) && Object.HasStateAuthority)
            Runner.Despawn(Object);
    
    }


    private void StartArc()
    {
        if (arcStarted || !ArcPrepared)    // TargetPos 대신 ArcPrepared 플래그만 사용
            return;

        arcStarted = true;
        PlayArc();
    }

    private void PlayArc()
    {
        Vector3 midPoint = (transform.position + TargetPos) * 0.5f
                         + Vector3.up * bulletData.arcHeight;

        Vector3[] path = { transform.position, midPoint, TargetPos };
        float distance = Vector3.Distance(transform.position, TargetPos);
        float duration = Mathf.Max(distance / bulletData.bulletSpeed, 0.01f);

        arcTween = transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .SetLookAt(0.01f)
            .OnComplete(() =>
            {
                if (Object.HasStateAuthority)
                    DestroyBullet();
            });
    }


    public override void DestroyBullet()
    {
        if (!Object.HasStateAuthority) return;

        Vector3 hitPos = transform.position;

        foreach (var hit in Physics.OverlapSphere(hitPos, bulletData.explosionRadius))
        {
            if (hit.TryGetComponent(out RobHp hp))
            {
                hp.TakeDamage(bulletData.bulletDamage);
            }
        }

        pendingDespawn = true;
        DespawnTimer = TickTimer.CreateFromSeconds(Runner, 0.05f);

        arcTween?.Kill();

        RPC_PlayImpact(hitPos);

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetTarget(Vector3 targetPos)
    {
        TargetPos = targetPos;
        ArcPrepared = true;

        if (!arcStarted)
            StartArc();

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayImpact(Vector3 hitPos)
    {
        SoundManager.instance.PlaySFX(destAudioClip, this.transform);

        if (EffectManager.instance != null && hitPrefab != null)
        {
            EffectManager.instance.PlayEffect(hitPrefab, hitPos);
        }
    }

    

    public override void Update()
    {
        //트랜스폼 이동 삭제
    }
    public override void OnTriggerEnter(Collider other)
    {
        //이놈도 삭제
    }
    

    
}
