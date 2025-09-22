using Fusion;
using Fusion.Sockets; // 🔑 StartGame 확장메서드가 여기 들어있음
using UnityEngine;

public class T3FusionBootstrap : MonoBehaviour
{
    private Fusion.NetworkRunner runner;

    private async void Start()
    {
        
        runner = gameObject.AddComponent<Fusion.NetworkRunner>();
        
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode    = GameMode.AutoHostOrClient,
            SessionName = "TestRoom"
        });

        if (result.Ok)
            Debug.Log("✅ Fusion 연결/세션 시작 성공");
        else
            Debug.LogError($"❌ Fusion 시작 실패: {result.ShutdownReason}");
    }
}
