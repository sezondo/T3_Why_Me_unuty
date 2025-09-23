using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using System.Linq;

public class Matchmaker : MonoBehaviour, INetworkRunnerCallbacks
{
    //[Header("Settings")]
    [SerializeField] private int requiredPlayers = 2;        // 2인 매칭
    [SerializeField] private List<string> battleSceneNames;
   // [SerializeField] private string battleSceneName = "BattleScene";

    public event Action<MatchStatus> OnStatusChanged;

    private Fusion.NetworkRunner runner;
    private NetworkSceneManagerDefault sceneMgr;
    private int bossId;
    private bool started; // 중복 실행 방지

    public void SetBossId(int id) => bossId = id;

    public async void StartMatchmaking()
    {
        if (started) return;
        started = true;

        OnStatusChanged?.Invoke(MatchStatus.Searching);

        var go = new GameObject("Runner");
        DontDestroyOnLoad(go); // 씬 전환 시 Runner 유지

        runner   = go.AddComponent<Fusion.NetworkRunner>();
        sceneMgr = go.AddComponent<NetworkSceneManagerDefault>();
        runner.AddCallbacks(this);

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode     = GameMode.AutoHostOrClient,           // 없으면 Host, 있으면 Client
            SessionName  = $"T3_BossRaid_{bossId}",             // 보스별 매칭 분리 가능
            Scene        = SceneRef.None,                       // 로비 유지(팝업 방식)
            SceneManager = sceneMgr
        });

        if (!result.Ok)
        {
            Debug.LogError($"[Matchmaker] StartGame 실패: {result.ShutdownReason}");
            OnStatusChanged?.Invoke(MatchStatus.Failed);
            started = false;
            return;
        }

        OnStatusChanged?.Invoke(runner.IsServer ? MatchStatus.RoomCreated : MatchStatus.JoinedRoom);
    }

    // ==== INetworkRunnerCallbacks ====
    public async void OnPlayerJoined(NetworkRunner r, PlayerRef player)
    {
        OnStatusChanged?.Invoke(MatchStatus.PlayerJoined);

        if (r.IsServer && r.ActivePlayers.Count() >= requiredPlayers)
        {
            OnStatusChanged?.Invoke(MatchStatus.Starting);
            Debug.Log("string 제대로 들갔남?" + battleSceneNames[bossId]);
            await runner.LoadScene(battleSceneNames[bossId]);
        }
    }

    public void OnPlayerLeft(NetworkRunner r, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner r) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        OnStatusChanged?.Invoke(MatchStatus.Disconnected);
        started = false;
    }

    public void OnShutdown(NetworkRunner r, ShutdownReason reason)
    {
        Debug.Log($"[Matchmaker] Shutdown: {reason}");
        started = false;
    }

    // 지금은 미사용 콜백들
    public void OnInput(NetworkRunner r, NetworkInput input) { }
    public void OnConnectFailed(NetworkRunner r, NetAddress addr, NetConnectFailedReason reason)
    {
        OnStatusChanged?.Invoke(MatchStatus.Failed);
        started = false;
    }
    public void OnUserSimulationMessage(NetworkRunner r, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
    public void OnReliableDataProgress(NetworkRunner r, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner r) { }
    public void OnSceneLoadDone(NetworkRunner r) { }
    public void OnHostMigration(NetworkRunner r, HostMigrationToken token) { }
    public void OnObjectEnterAOI(NetworkRunner r, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner r, NetworkObject obj, PlayerRef player) { }

    // 선택: 매칭 취소/정리
    public async void CancelAndShutdown()
    {
        if (runner == null) return;
        try { await runner.Shutdown(); }
        finally { started = false; }
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        if (runner.ActivePlayers.Count() >= requiredPlayers)
        {
            request.Refuse();
        }
        else
        {
            request.Accept();
        }
    }
     public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
}
