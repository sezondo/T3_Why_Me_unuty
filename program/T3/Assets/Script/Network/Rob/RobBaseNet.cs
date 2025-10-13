using Fusion;
using UnityEngine;

// RobBaseNet은 RobBase를 상속받아 네트워크 기능을 확장합니다.
public class RobBaseNet : RobBase
{
    [Networked]
    public UnitState SyncedState { get; private set; }

    private bool IsServer => Object.HasStateAuthority;

    // 네트워크 속성의 변경을 감지하는 ChangeDetector
    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        // ChangeDetector를 초기화합니다.
        // Source를 SimulationState로 설정하여 FixedUpdateNetwork에서 변경된 값을 기준으로 감지합니다.
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    // Fusion의 네트워크 업데이트 루프입니다.
    public override void FixedUpdateNetwork()
    {
        // 서버(호스트)는 유닛의 상태를 결정하고 동기화할 책임이 있습니다.
        if (IsServer)
        {
            // RobBase의 currentState를 SyncedState로 복사하여 모든 클라이언트에 전파합니다.
            if (SyncedState != currentState)
            {
                SyncedState = currentState;
            }

        }

        //클라에 전파
        foreach (var propertyName in _changeDetector.DetectChanges(this))
        {
            // SyncedState 속성에 변경이 있었는지 확인합니다.
            if (propertyName == nameof(SyncedState))
            {
                ChangeState(SyncedState);
            }
        }
            
    }

    // Render는 시각적 업데이트를 처리하기에 가장 좋은 장소입니다.
    // FixedUpdateNetwork보다 더 자주 호출되어 부드러운 움직임을 보장합니다.
    public override void Render()
    {
        // 일단 대기
    }

   

    /// <summary>
    /// 외부에서 유닛의 상태 변경을 '요청'하는 공개 메소드입니다. 서버에서만 실행됩니다.
    /// </summary>
    /*
    public void RequestStateChange(UnitState newState)
    {
        if (IsServer)
        {
            // RobBase에 있는 기존 ChangeState 메소드를 호출합니다.
            // 이 변경은 FixedUpdateNetwork에서 감지되어 모든 클라이언트에 동기화됩니다.
            ChangeState(newState);
        }
    }
    */
}