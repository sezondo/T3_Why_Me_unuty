using UnityEngine;
using Fusion;

// 플레이어의 코스트(엘릭서)를 관리하는 스크립트입니다.
// 플레이어 캐릭터나 세션 관리 오브젝트에 붙여주세요.
public class PlayerCostManager : NetworkBehaviour
{
    // 싱글톤 인스턴스
    public static PlayerCostManager Instance { get; private set; }

    [SerializeField] private float maxCost = 10f; // 최대 코스트
    [SerializeField] private float costGenerationRate = 1f; // 초당 코스트 생성량

    // [Networked] 속성은 이 변수가 네트워크를 통해 모든 클라이언트에게 자동으로 동기화되게 합니다.
    // OnChanged 콜백을 통해 값이 변경될 때마다 특정 함수(OnCostChanged)를 호출할 수 있습니다.
    [Networked(OnChanged = nameof(OnCostChanged))]
    public float CurrentCost { get; set; }

    public override void Spawned()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 이미 인스턴스가 있다면 이 오브젝트는 파괴
            Destroy(gameObject);
            return;
        }

        // 서버에서만 코스트 초기화
        if (Object.HasStateAuthority)
        {
            CurrentCost = 3f; // 시작 코스트
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 이 로직은 서버 또는 호스트에서만 실행되어야 합니다.
        // HasStateAuthority는 이 클라이언트가 이 오브젝트의 상태를 제어할 권한이 있는지(즉, 서버인지) 확인합니다.
        if (Object.HasStateAuthority)
        {
            if (CurrentCost < maxCost)
            {
                CurrentCost += costGenerationRate * Runner.DeltaTime;
                if (CurrentCost > maxCost)
                {
                    CurrentCost = maxCost;
                }
            }
        }
    }

    // 서버에서 호출될 코스트 차감 함수
    public bool SpendCost(float amount)
    {
        if (CurrentCost >= amount)
        {
            CurrentCost -= amount;
            return true;
        }
        return false;
    }

    // CurrentCost 값이 변경될 때 모든 클라이언트에서 호출될 콜백 함수
    private static void OnCostChanged(Changed<PlayerCostManager> changed)
    {
        // 여기서 UI 텍스트 업데이트 등의 시각적 처리를 할 수 있습니다.
        // 예: UIManager.Instance.UpdateCostText(changed.Behaviour.CurrentCost);
        Debug.Log($"Cost updated to: {changed.Behaviour.CurrentCost}");
    }
}