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
    // 어디서든 접근 가능한 Runner 속성
    public static NetworkRunner Runner { get; private set; }

    // --- Inspector-assigned Fields ---
    [SerializeField]
    private List<SceneRef> battleScenes; // 인스펙터에서 할당할 보스 전투 씬 목록

    [SerializeField]
    private int requiredPlayers = 2; // 게임 시작에 필요한 플레이어 수

    [SerializeField]
    private NetworkObject playerPrefab; // 유니티 에디터에서 NetworkPlayer 프리팹을 할당해야 합니다.

    // --- Public Events ---
    public event Action<MatchStatus> OnStatusChanged;

    // --- Private State ---
    private NetworkRunner _runner; // 현재 활성 네트워크 러너 인스턴스
    private bool _started; // 매치메이킹 시작 여부 (중복 시작 방지)
    private int _bossId; // 선택된 보스 ID

    private void Awake()
    {
        // 에디터나 개발 빌드가 아닐 경우, 성능을 위해 모든 디버그 로그를 비활성화합니다.
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        Debug.unityLogger.logEnabled = false;
#endif
    }

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
            Debug.LogWarning("[매치메이커] 이미 매치메이킹 프로세스가 시작되었습니다. 중복 실행을 방지합니다.");
            return;
        }
        _started = true;

        // --- 초기 유효성 검사 ---
        if (battleScenes == null || battleScenes.Count == 0)
        {
            Debug.LogError("[매치메이커] 인스펙터에서 'Battle Scenes' 리스트가 설정되지 않았습니다. 매치메이킹을 진행할 수 없습니다.");
            OnStatusChanged?.Invoke(MatchStatus.Failed);
            _started = false;
            return;
        }

        // --- Runner GameObject 생성 및 설정 ---
        var runnerGo = new GameObject("Runner");
        _runner = runnerGo.AddComponent<NetworkRunner>();

        // 생성된 _runner를 정적 속성에 할당
        Runner = _runner;

        _runner.AddCallbacks(this);

        // Matchmaker와 Runner가 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(runnerGo);

        OnStatusChanged?.Invoke(MatchStatus.Searching);
        Debug.Log("[매치메이커] 상대 플레이어를 찾기 위해 게임 세션 탐색을 시작합니다.");

        // --- Fusion StartGame 호출 ---
        // 공식 문서에 따르면, 이미 로드된 씬에서 시작할 때는 SceneRef.None 대신
        // 현재 씬의 참조를 명시적으로 전달하는 것이 올바른 방법입니다.
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = null, // 고정된 세션 이름 대신 null을 사용하여 랜덤 매칭 활성화
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = requiredPlayers,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                // 'b'라는 키로 bossId를 세션 속성에 추가하여
                // 같은 보스를 선택한 플레이어들끼리 매칭되도록 합니다.
                { "b", _bossId }
            }
        };

        var result = await _runner.StartGame(startGameArgs); //네트워크 러너가 이걸로 게임 시작함

        // --- 결과 처리 ---
        if (!result.Ok) //뭐 이상한 이유로 게임 시작 실패하면 이거임
        {
            Debug.LogError($"[매치메이커] 게임 시작에 실패했습니다. 사유: {result.ShutdownReason}");
            OnStatusChanged?.Invoke(MatchStatus.Failed);
            await _runner.Shutdown(); // 실패 시 즉시 셧다운
            _started = false;
            return;
        }

        Debug.Log($"[매치메이커] 게임 세션 시작 성공. 현재 역할: {(_runner.IsServer ? "서버(호스트)" : "클라이언트")}");
        OnStatusChanged?.Invoke(_runner.IsServer ? MatchStatus.RoomCreated : MatchStatus.JoinedRoom);

        // --- 호스트 자체 참가 처리 (Race Condition 방지) ---
        // StartGame 성공 후 호스트의 LocalPlayer가 ActivePlayers 목록에 즉시 나타나지 않을 수 있음
        await Task.Delay(200); // 짧은 지연으로 목록이 업데이트될 시간을 줌

        if (_runner != null && _runner.IsServer)
        {
            // 호스트 자신의 NetworkPlayer 스폰
            Debug.Log("[매치메이커] 호스트의 NetworkPlayer를 스폰합니다.");
            NetworkObject hostObject = _runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, _runner.LocalPlayer);
            NetworkPlayer networkPlayerHost = hostObject.GetComponent<NetworkPlayer>();

            if (networkPlayerHost != null)
            {
                networkPlayerHost.SetPlayerSide(PlayerSide.Host);
            }

            var localPlayerExists = _runner.ActivePlayers.Any(p => p == _runner.LocalPlayer);
            Debug.Log($"[매치메이커] 호스트 자신의 플레이어 존재 여부 확인: {localPlayerExists}");

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
            Debug.Log("[매치메이커] 사용자가 매치메이킹을 취소하여 네트워크 세션을 종료합니다.");
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
        Debug.Log($"[매치메이커] 게임 시작 조건 확인. 현재 플레이어: {activeCount}명 / 필요 플레이어: {requiredPlayers}명");

        if (activeCount >= requiredPlayers)
        {
            // --- 보스 ID 및 씬 유효성 검사 ---
            if (_bossId < 0 || _bossId >= battleScenes.Count)
            {
                Debug.LogError($"[매치메이커] 잘못된 BossId({_bossId})가 전달되어 게임을 시작할 수 없습니다.");
                return;
            }

            SceneRef sceneRef = battleScenes[_bossId];
            if (!sceneRef.IsValid)
            {
                Debug.LogError($"[매치메이커] BossId({_bossId})에 해당하는 씬이 유효하지 않습니다.");
                return;
            }

                    Debug.Log($"[매치메이커] 게임 시작 조건을 충족했습니다. BossId: {_bossId}, Scene: {sceneRef} (으)로 전투 씬 로드를 시작합니다.");
                    OnStatusChanged?.Invoke(MatchStatus.Starting);
            
                    // 게임이 시작되면 세션을 보이지 않게 하고 닫아서 추가 입장을 막습니다.
                    // 이렇게 하면 다음 매치메이킹 시 이전 세션에 참가하려는 시도를 방지할 수 있습니다.
                    runner.SessionInfo.IsVisible = false;
                    runner.SessionInfo.IsOpen = false;
            
                    // 모든 클라이언트에게 씬 로드를 지시
                    try
                    {
                        await runner.LoadScene(sceneRef);
                    }            catch (Exception e)
            {
                Debug.LogError($"[매치메이커] 씬 로드 중 예외 발생: {e.Message}");
            }
        }
    }

    // --- INetworkRunnerCallbacks Implementation ---

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) //이미 세션이 존재하는곳에 참가할때 호출
    {
        int activeCount = runner.ActivePlayers.Count();
        Debug.Log($"[매치메이커] 새로운 플레이어 참가. 플레이어 ID: {player}, 현재 플레이어: {activeCount}명");

        if (runner.IsServer)//클라이언트용 근데 서버가 만듬
        {
            // 새로 참가한 플레이어를 위한 NetworkPlayer 스폰
            Debug.Log($"[매치메이커] 새로 참가한 플레이어({player})의 NetworkPlayer를 스폰합니다.");
            NetworkObject clientObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

            NetworkPlayer networkPlayerClient = clientObject.GetComponent<NetworkPlayer>();

            if (networkPlayerClient != null)
            {
                networkPlayerClient.SetPlayerSide(PlayerSide.Client);
            }

            OnStatusChanged?.Invoke(MatchStatus.PlayerJoined);
            // 새 플레이어가 참가했으므로 게임 시작 조건 확인
            _ = TryStartGameAsync(runner);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[매치메이커] 플레이어 나감. 플레이어 ID: {player}");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // 세션이 가득 차지 않았으면 연결을 수락
        if (runner.ActivePlayers.Count() < requiredPlayers)
        {
            request.Accept();
            Debug.Log("[매치메이커] 새로운 클라이언트의 연결 요청을 수락했습니다.");
        }
        else
        {
            request.Refuse();
            Debug.LogWarning("[매치메이커] 세션이 가득 차서 새로운 클라이언트의 연결 요청을 거절했습니다.");
        }
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("[매치메이커] 서버로부터 연결이 끊어졌습니다.");
        OnStatusChanged?.Invoke(MatchStatus.Disconnected);
        // OnShutdown에서 최종 정리가 수행됨
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"[매치메이커] 네트워크 세션이 종료되었습니다. 사유: {shutdownReason}");
        OnStatusChanged?.Invoke(MatchStatus.Disconnected);
        _started = false;

        // Runner GameObject 정리
        if (runner != null && runner.gameObject != null)
        {
            Destroy(runner.gameObject);
        }
        _runner = null;

        // 정적 참조도 정리
        if (runner == Runner)
        {
            Runner = null;
        }

        // 사용자가 직접 정상 종료한 경우가 아니라면, 자동 재매치를 시도합니다.
        if (shutdownReason != ShutdownReason.Ok)
        {
            Debug.Log("[매치메이커] 3초 후 자동으로 재매치를 시도합니다.");
            // RematchAsync를 비동기적으로 호출하고 기다리지 않습니다.
            _ = RematchAsync();
        }
    }

    private async Task RematchAsync()
    {
        // 3초 대기
        await Task.Delay(3000);

        // 씬 전환 등으로 이 컴포넌트가 파괴되지 않았을 경우에만 재매치 시작
        if (this != null)
        {
            Debug.Log("[매치메이커] 재매치를 시작합니다.");
            await StartMatchmaking();
        }
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
        Debug.Log("[매치메이커] 전투 씬 로드가 완료되었습니다.");
    }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

}