using UnityEngine;
using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public PlayerSide playerSide { get; private set; }
    [Networked] public int Cost { get; private set; }

    [SerializeField]
    private int costGenerationRate = 1; // 1초당 코스트 획득량
    [SerializeField]
    private int maxCost = 10; // 1초당 코스트 획득량

    private TickTimer costGenerationTimer;

    public static NetworkPlayer Local { get; private set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority) // 자신이 로컬이라는것을 확인
        {
            Local = this;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 서버에서만 코스트 생성 로직을 실행하여 권위를 가집니다.
        if (Runner.IsServer)
        {
            // 타이머가 만료될 때마다 코스트를 증가시킵니다.
            if (costGenerationTimer.ExpiredOrNotRunning(Runner))
            {
                // 타이머를 1초로 재설정합니다.
                costGenerationTimer = TickTimer.CreateFromSeconds(Runner, 1.0f);
                // 코스트를 증가시킵니다.
                Cost += costGenerationRate;
            }
            if (Cost >= maxCost)
            {
                Cost = maxCost;
            }
        }
    }
    //초기에 호스트/클라이언트 타입찾을때
    public void SetPlayerSide(PlayerSide inPlayerSide)
    {
        if (inPlayerSide == PlayerSide.Host)
        {
            playerSide = PlayerSide.Host;
        }
        else if(inPlayerSide == PlayerSide.Client)
        {
            playerSide = PlayerSide.Client;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SpendCost(int amount)
    {
        if (Cost >= amount)
        {
            Cost -= amount;
        }
        else
        {
            Debug.LogWarning($"[Server] Cost is not sufficient for player {playerSide}. Required: {amount}, Has: {Cost}");
        }
    }

    public void SetCost(int amount)
    {
        Cost = amount;
    }
}
