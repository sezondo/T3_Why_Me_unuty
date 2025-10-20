using UnityEngine;
using System.Collections;
using Fusion;

public class RobAttackNet : RobAttack
{
    // RPC 대신 네트워크로 동기화될 Tick 변수를 사용합니다.
    [Networked]
    public int AttackTick { get; private set; }

    // 클라이언트에서 마지막으로 확인한 Tick 값을 저장하기 위한 로컬 변수입니다.
    private int _lastAttackTick;

    public override void Spawned()
    {
        base.Start();
        // 스폰 시점에 로컬 Tick 값을 네트워크 Tick 값과 동기화합니다.
        _lastAttackTick = AttackTick;
    }

    public override void Start() { }
    public override void Update() { }

    public override void FixedUpdateNetwork()
    {
        if (robBase.currentState == UnitState.Dead)
        {
            return;
        }

        switch (robBase.currentState)
        {
            case UnitState.Idle:
                break;

            case UnitState.Attacking:
                if (!CoroutineCheck)
                {
                    CoroutineCheck = true;
                    StartCoroutine(Attacking());
                }
                break;

            case UnitState.Moving:
                break;

            case UnitState.Dead:
                break;

            case UnitState.Turn:
                break;
        }
    }

    // Render 함수에서 시각적 표현을 처리합니다.
    public override void Render()
    {
        // 네트워크로 동기화된 AttackTick 값이 로컬에 저장된 값보다 크면,
        // 새로운 공격이 발생했다는 의미입니다.
        if (AttackTick > _lastAttackTick)
        {
            // 공격 애니메이션과 사운드를 재생합니다.
            animator.SetTrigger("Attack");
            
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySFX(robBase.data.attackAudioClip, this.transform);
            }

            // 로컬 Tick 값을 업데이트하여 다음 프레임에서 중복 실행되는 것을 방지합니다.
            _lastAttackTick = AttackTick;
        }
    }

    public override IEnumerator Attacking()
    {
        while (true)
        {
            if (robBase.currentState == UnitState.Dead || robBase.currentState != UnitState.Attacking)
            {
                break;
            }

            Fire();

            yield return new WaitForSeconds(robBase.data.attackSpeed);
        }
        CoroutineCheck = false;
    }

    public override void Fire()
    {
        // 서버 권한이 있을 때만 발사 로직과 Tick 값 증가를 실행합니다.
        if (Object.HasStateAuthority)
        {
            // 1. 실제 총알 발사
            foreach (var fp in shooter)
            {
                fp.Shoot();
            }

            // 2. 네트워크 Tick 값을 증가시켜 클라이언트에게 공격이 발생했음을 알립니다.
            AttackTick++;
        }
    }
}
