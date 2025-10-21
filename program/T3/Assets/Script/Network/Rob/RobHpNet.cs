using UnityEngine;
using System.Collections;
using Fusion;

public class RobHpNet : RobHp
{
    private bool _deathAnimationTriggered; //클라에서 죽음 애니메이션/sfx한번만 사용하게
    [Networked] private TickTimer Tick { get; set; } // 시간계산용 네트워크 타이머
    private bool _timerArmed; // 타이머 한번만 사용 플래그
    private bool _deathFallActive; // 가라앉는중인지 표시

    private float _deathElapsed; //경과 시간
    private const float DeathDuration = 5f; // 가라앉는데 걸리는 시간
    private const float DeathSpeed = 3f; // 떨어지는 거리
    private Vector3 _deathStartPosition; // 시작 (위치)
    private Vector3 _deathEndPosition; // 끝 (위치)

    public override void Spawned()
    {
        base.Start();
        ResetDeathState();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        if (currentHp <= 0 && robBase.currentState != UnitState.Dead)
        {
            robBase.ChangeState(UnitState.Dead);
        }

        if (robBase.currentState == UnitState.Dead && !_deathFallActive)
        {
            BeginDeathFall();
        }

        if (!_deathFallActive)
        {
            return;
        }

        if (!_timerArmed && !Tick.IsRunning)
        {
            Tick = TickTimer.CreateFromSeconds(Runner, 5f);
        }

        if (_timerArmed || Tick.Expired(Runner))
        {
            _timerArmed = true; // 자기유지

            transform.position = Vector3.Lerp(
                _deathStartPosition,
                _deathEndPosition,
                _deathElapsed / DeathDuration);

            _deathElapsed += Runner.DeltaTime;

            if (_deathElapsed >= DeathDuration)
            {
                Destroy();
            }
        }
    }

    public override IEnumerator TakeDie()
    {
        return base.TakeDie();
    }

    public override void Render()
    {
        if (robBase.currentState != UnitState.Dead || _deathAnimationTriggered)
        {
            return;
        }

        _deathAnimationTriggered = true;
        animator.SetTrigger("Death");
        SoundManager.instance.PlaySFX(robBase.data.dieAudioClip, transform);
    }

    public override void Destroy()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    public override void Start() { }
    public override void Update() { }

    private void BeginDeathFall()
    {
        _deathFallActive = true;
        _timerArmed = false;
        _deathElapsed = 0f;
        Tick = TickTimer.None;
        _deathStartPosition = transform.position; //시작위치
        _deathEndPosition = _deathStartPosition + Vector3.down * DeathSpeed; //목표위치
    }

    private void ResetDeathState() // 초기화
    {
        _deathAnimationTriggered = false;
        _deathFallActive = false;
        _timerArmed = false;
        _deathElapsed = 0f;
        Tick = TickTimer.None;
    }
}
