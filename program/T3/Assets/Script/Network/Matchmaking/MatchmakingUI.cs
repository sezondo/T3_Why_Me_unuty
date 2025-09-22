using UnityEngine;

public class MatchmakingUI : MonoBehaviour
{
    public GameObject bossSelectPopup;
    public GameObject matchmakingPopup;
    public TMPro.TextMeshProUGUI statusText; // TMP 사용 가정

    [SerializeField] private Matchmaker matchmaker;

    void Awake()
    {
        matchmaker.OnStatusChanged += HandleStatus;
    }

    void OnDestroy()
    {
        matchmaker.OnStatusChanged -= HandleStatus;
    }

    public void OnClickMultiplayer() => bossSelectPopup.SetActive(true);

    public void OnSelectBoss(int bossId) => matchmaker.SetBossId(bossId);

    public void OnClickMatchStart()
    {
        bossSelectPopup.SetActive(false);
        matchmakingPopup.SetActive(true);
        statusText.text = "팀원 찾는 중…";
        matchmaker.StartMatchmaking();
    }

    void HandleStatus(MatchStatus status)
    {
        switch (status)
        {
            case MatchStatus.Searching:    statusText.text = "팀원 찾는 중…"; break;
            case MatchStatus.RoomCreated:  statusText.text = "방 생성됨, 팀원 대기 중…"; break;
            case MatchStatus.JoinedRoom:   statusText.text = "참가 완료! 팀원 대기 중…"; break;
            case MatchStatus.PlayerJoined: statusText.text = "팀원 서칭 완료"; break;
            case MatchStatus.Starting:     statusText.text = "게임을 시작합니다"; break;
            case MatchStatus.Failed:       statusText.text = "매칭 실패"; break;
            case MatchStatus.Disconnected: statusText.text = "연결이 끊겼습니다"; break;
        }
    }
}
