using UnityEngine;
using Unity.VisualScripting;

public class RobHpNet : RobHp
{
    private bool dieCheck;

    public override void Spawned()
    {
        dieCheck = false;
        if (Object.HasStateAuthority)
        {
            base.Start();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 서버는 유닛의 상태를 Dead로 변경할 책임만 가집니다.
        if (Object.HasStateAuthority)
        {
            if (currentHp <= 0 && robBase.currentState != UnitState.Dead)
            {
                robBase.ChangeState(UnitState.Dead);
            }
        }
        // 죽음에 대한 시각적 처리는 Render 함수로 이전되었습니다.
    }

    // Render 함수를 추가하여 죽음 관련 시각 효과를 처리합니다.
    public override void Render()
    {
        // 유닛의 상태가 Dead이고, 아직 죽음 처리를 하지 않았다면,
        if (robBase.currentState == UnitState.Dead && !dieCheck)
        {
            // 중복 실행을 막기 위해 플래그를 true로 설정하고,
            dieCheck = true;
            // 죽음 관련 시각 효과를 처리하는 함수를 호출합니다.
            DeadDisposal();
        }
    }

    public override void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        base.TakeDamage(damage);
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
}