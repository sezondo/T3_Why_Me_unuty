using UnityEngine;
using Fusion;

// 네트워크 프리뷰 유닛에 붙는 스크립트
public class RobPreviewNet : NetworkBehaviour
{
    [Networked] private TickTimer SwapTimer { get; set; } // 유닛 교체를 위한 네트워크 타이머
    private RobReadyData _robReadyData;
    private bool _isInitialized = false;

    // 서버에서 호출하여 RobReadyData를 설정하는 함수
    public void Initialize(RobReadyData data)
    {
        _robReadyData = data;
        _isInitialized = true;
    }

    public override void Spawned()
    {
        // 서버에서만 타이머 시작
        if (Object.HasStateAuthority)
        {
            SwapTimer = TickTimer.CreateFromSeconds(Runner, 1.5f); // 1.5초 타이머 설정
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 서버이고, 초기화가 완료되었고, 타이머가 만료되었을 때
        if (Object.HasStateAuthority && _isInitialized && SwapTimer.Expired(Runner))
        {
            // 실제 유닛 프리팹이 있는지 확인
            if (_robReadyData.RobPrefab != null)
            {
                // 실제 유닛을 현재 위치에 스폰
                Runner.Spawn(_robReadyData.RobPrefab, transform.position, transform.rotation, Object.InputAuthority);
            }
            else
            {
                Debug.LogError("RobReadyData에 실제 유닛 프리팹(RobPrefab)이 지정되지 않았습니다.");
            }

            // 자신(프리뷰)을 파괴
            Runner.Despawn(Object);

            // 이 로직이 두 번 실행되지 않도록 타이머를 초기화하고 비활성화
            SwapTimer = TickTimer.None;
            _isInitialized = false;
        }
    }
}