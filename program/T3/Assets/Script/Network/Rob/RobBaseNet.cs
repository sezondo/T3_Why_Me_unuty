using Fusion;
using UnityEngine;

// RobBaseNet은 RobBase를 상속받아 네트워크 기능을 확장합니다.
public class RobBaseNet : RobBase
{
    [Networked]
    public UnitState SyncedState { get; private set; }


    // 네트워크 속성의 변경을 감지하는 ChangeDetector
    //private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        base.Start();
    }

    // Fusion의 네트워크 업데이트 루프입니다.
    public override void FixedUpdateNetwork()
    {
        // 서버(호스트)는 유닛의 상태를 결정하고 동기화할 책임이 있습니다.
        if (Object.HasStateAuthority)
        {
            // RobBase의 currentState를 SyncedState로 복사하여 모든 클라이언트에 전파합니다.
            if (SyncedState != currentState)
            {
                SyncedState = currentState;
            }
        }

        switch (currentState)
        {
            case UnitState.Idle:

                break;
            case UnitState.Moving:

                break;
            case UnitState.Attacking:

                break;
            case UnitState.Dead:
                unitCollider.enabled = false; //나중에 고려해볼것 유닛 사망후 질질 끌려다니는거
                return;
            case UnitState.Turn:

                break;
        }

        animator.SetInteger("State", (int)currentState); // Attack 애니메이션은 트리거로 따로 관리
    }

    // Render는 시각적 업데이트를 처리하기에 가장 좋은 장소입니다.
    // FixedUpdateNetwork보다 더 자주 호출되어 부드러운 움직임을 보장합니다.
    public override void Render()
    {
        //클라에 전파
        if (!Object.HasStateAuthority)
        {
            if (currentState != SyncedState)
            {
                ChangeState(SyncedState);
            }
        }

    }

    //public override void Awake() { }
    public override void Update() { }
    public override void Start() { }
}