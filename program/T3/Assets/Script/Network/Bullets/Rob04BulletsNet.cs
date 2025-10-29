using UnityEngine;
using DG.Tweening;
using Fusion;


public class Rob04BulletsNet : BulletsNet
{
    [SerializeField] private AudioClip destAudioClip;
    [Networked] private Vector3 TargetPos { get; set; }
    [Networked] private NetworkBool ArcPrepared { get; set; } //곡선 시작 플래그
    [Networked] private Vector3 SpawnPos{ get; set; }
    private Tweener arcTween; // 현재 진행중인 DOT윈
    private bool arcStarted; // 곡물 구동 중복 방지 체크
    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        base.Spawned();

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (ArcPrepared) //네트워크값이 들어왔으면 바로 시작
            StartArc();
    }

    public override void FixedUpdateNetwork()
    {
        foreach (var changedName in _changeDetector.DetectChanges(this))
        {
            if (changedName == nameof(ArcPrepared) && ArcPrepared)
                StartArc();
        }

        if (pendingDespawn && DespawnTimer.Expired(Runner) && Object.HasStateAuthority)
            Runner.Despawn(Object);
    }

    private void StartArc()
    {
        if (arcStarted || !ArcPrepared) //값 들어왔나 체크 및 중복 체크
            return;

        arcStarted = true;
        PlayArc();
    }

    private void PlayArc()
    {
        Vector3 startPos = Object.HasStateAuthority ? transform.position : SpawnPos;

        if (startPos == Vector3.zero)
            startPos = transform.position;
        

        Vector3 midPoint = (startPos + TargetPos) * 0.5f
                           + Vector3.up * bulletData.arcHeight;

        Vector3[] path = { startPos, midPoint, TargetPos };
        float distance = Vector3.Distance(startPos, TargetPos);
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
    public void RPC_SetTarget(Vector3 startPos, Vector3 targetPos)
    {
        SpawnPos = startPos;
        transform.position = startPos;
        TargetPos = targetPos;
        ArcPrepared = true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayImpact(Vector3 hitPos)
    {
        SoundManager.instance.PlaySFX(destAudioClip, transform);

        if (EffectManager.instance != null && hitPrefab != null)
        {
            EffectManager.instance.PlayEffect(hitPrefab, hitPos);
        }
    }

    public override void Update()
    {
        // intentionally left blank
    }

    public override void OnTriggerEnter(Collider other)
    {
        // intentionally left blank
    }
}
