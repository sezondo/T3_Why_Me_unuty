using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Matchmaker와 UI 간의 상태 통신을 위한 열거형
public enum MatchStatus
{
    Idle,
    Searching,
    RoomCreated,
    JoinedRoom,
    PlayerJoined,
    Starting,
    InGame,
    Disconnected,
    Failed
}

/// <summary>
/// Fusion을 사용한 2인 보스 레이드 매치메이킹을 처리합니다.
/// GameMode.AutoHostOrClient를 사용하여 호스트 또는 클라이언트로 자동 시작하고,
/// 지정된 bossId에 따라 세션을 생성하고 참가합니다.
/// </summary>
public class Matchmaker : MonoBehaviour, INetworkRunnerCallbacks
{
    // --- Inspector-assigned Fields ---
    [SerializeField]
    private List<SceneRef> battleScenes; // 인스펙터에서 할당할 보스 전투 씬 목록

    [SerializeField]
    private int requiredPlayers = 2; // 게임 시작에 필요한 플레이어 수

    // --- Public Events ---
    public event Action<MatchStatus> OnStatusChanged;

    // --- Private State ---
    private NetworkRunner _runner; // 현재 활성 네트워크 러너 인스턴스
    private bool _started; // 매치메이킹 시작 여부 (중복 시작 방지)
    private int _bossId; // 선택된 보스 ID

    /// <summary>
    /// 매치메이킹을 위한 보스 ID를 설정합니다.
    /// </summary>
    public void SetBossId(int bossId)
    {
        _bossId = bossId;
    }

    /// <summary>
    /// 매치메이킹 프로세스를 시작합니다.
    /// </summary>
    public async Task StartMatchmaking()
    {
        if (_started)
        {
            Debug.LogWarning("[Matchmaker] Already starting or started.");
            return;
        }
        _started = true;

        // --- 초기 유효성 검사 ---
        if (battleScenes == null || battleScenes.Count == 0)
        {
            Debug.LogError("[Matchmaker] Battle Scenes list is not assigned in the inspector.");
            OnStatusChanged?.Invoke(MatchStatus.Failed);
            _started = false;
            return;
        }

        // --- Runner GameObject 생성 및 설정 ---
        var runnerGo = new GameObject("Runner");
        _runner = runnerGo.AddComponent<NetworkRunner>();
        _runner.AddCallbacks(this);

        // Matchmaker와 Runner가 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(runnerGo);

        OnStatusChanged?.Invoke(MatchStatus.Searching);
        Debug.Log("[Matchmaker] Searching for game...");

        // --- Fusion StartGame 호출 ---
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = $"T3_BossRaid_{_bossId}",
            Scene = SceneRef.None, // 씬은 수동으로 로드
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(), // 기본 씬 매니저 사용
            PlayerCount = requiredPlayers,
        };

        var result = await _runner.StartGame(startGameArgs);

        // --- 결과 처리 ---
        if (!result.Ok)
        {
            Debug.LogError($"[Matchmaker] Failed to start game: {result.ShutdownReason}");
            OnStatusChanged?.Invoke(MatchStatus.Failed);
            await _runner.Shutdown(); // 실패 시 즉시 셧다운
            _started = false;
            return;
        }

        Debug.Log($"[Matchmaker] StartGame successful. IsServer: {_runner.IsServer}, IsClient: {_runner.IsClient}");
        OnStatusChanged?.Invoke(_runner.IsServer ? MatchStatus.RoomCreated : MatchStatus.JoinedRoom);

        // --- 호스트 자체 참가 처리 (Race Condition 방지) ---
        // StartGame 성공 후 호스트의 LocalPlayer가 ActivePlayers 목록에 즉시 나타나지 않을 수 있음
        await Task.Delay(200); // 짧은 지연으로 목록이 업데이트될 시간을 줌

        if (_runner != null && _runner.IsServer)
        {
            var localPlayerExists = _runner.ActivePlayers.Any(p => p == _runner.LocalPlayer);
            Debug.Log($"[Matchmaker] Host self-check. LocalPlayer in ActivePlayers: {localPlayerExists}");

            if (!localPlayerExists)
            {
                // UI에 호스트가 '참가'했음을 알림
                OnStatusChanged?.Invoke(MatchStatus.PlayerJoined);
            }
            // 게임 시작 조건 확인
            await TryStartGameAsync(_runner);
        }
    }

    /// <summary>
    /// 매치메이킹을 취소하고 네트워크 세션을 종료합니다.
    /// </summary>
    public async void CancelAndShutdown()
    {
        if (_runner != null && !_runner.IsShutdown)
        {
            Debug.Log("[Matchmaker] Shutdown initiated by user.");
            await _runner.Shutdown();
        }
        // OnShutdown 콜백에서 _started 플래그가 재설정됨
    }

    /// <summary>
    /// 게임 시작 조건을 확인하고 충족 시 씬을 로드합니다. 서버에서만 실행됩니다.
    /// </summary>
    private async Task TryStartGameAsync(NetworkRunner runner)
    {
        // 서버가 아니거나 러너가 없으면 중단
        if (runner == null || !runner.IsServer) return;

        int activeCount = runner.ActivePlayers.Count();
        Debug.Log($"[Matchmaker] TryStartGameAsync check. Active players: {activeCount}/{requiredPlayers}");

        if (activeCount >= requiredPlayers)
        {
            // --- 보스 ID 및 씬 유효성 검사 ---
            if (_bossId < 0 || _bossId >= battleScenes.Count)
            {
                Debug.LogError($"[Matchmaker] Invalid BossId ({_bossId}). Cannot start game.");
                return;
            }

            SceneRef sceneRef = battleScenes[_bossId];
            if (!sceneRef.IsValid)
            {
                Debug.LogError($"[Matchmaker] Invalid SceneRef for BossId ({_bossId}).");
                return;
            }

            Debug.Log($"[Matchmaker] Starting game with BossId: {_bossId}, Scene: {sceneRef}. Loading scene...");
            OnStatusChanged?.Invoke(MatchStatus.Starting);

            // 모든 클라이언트에게 씬 로드를 지시
            try
            {
                await runner.LoadScene(sceneRef);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Matchmaker] Exception while loading scene: {e.Message}");
            }
        }
    }

    // --- INetworkRunnerCallbacks Implementation ---

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        int activeCount = runner.ActivePlayers.Count();
        Debug.Log($"[Matchmaker] OnPlayerJoined. Player: {player}, IsServer: {runner.IsServer}, ActivePlayers: {activeCount}");

        if (runner.IsServer)
        {
            OnStatusChanged?.Invoke(MatchStatus.PlayerJoined);
            // 새 플레이어가 참가했으므로 게임 시작 조건 확인
            _ = TryStartGameAsync(runner);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Matchmaker] Player {player} left.");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // 세션이 가득 차지 않았으면 연결을 수락
        if (runner.ActivePlayers.Count() < requiredPlayers)
        {
            request.Accept();
            Debug.Log($"[Matchmaker] Accepted connect request.");
        }
        else
        {
            request.Refuse();
            Debug.LogWarning($"[Matchmaker] Refused connect request, session is full.");
        }
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("[Matchmaker] Disconnected from server.");
        OnStatusChanged?.Invoke(MatchStatus.Disconnected);
        // OnShutdown에서 최종 정리가 수행됨
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"[Matchmaker] Runner shutdown. Reason: {shutdownReason}");
        OnStatusChanged?.Invoke(MatchStatus.Disconnected);
        _started = false;

        // Runner GameObject 정리
        if (runner != null && runner.gameObject != null)
        {
            Destroy(runner.gameObject);
        }
        _runner = null;
    }

    // --- Unused Callbacks ---
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) 
    {
        OnStatusChanged?.Invoke(MatchStatus.InGame);
        Debug.Log($"[Matchmaker] Scene load done.");
    }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

}