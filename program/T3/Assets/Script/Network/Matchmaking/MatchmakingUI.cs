using UnityEngine;
using System.Collections; // Coroutine을 위해 추가

public class MatchmakingUI : MonoBehaviour
{
    [SerializeField] private GameObject matchmakingOverlay;
    [SerializeField] private GameObject matchmakingPopup;
    [SerializeField] private TMPro.TextMeshProUGUI statusText; // TMP 사용 가정

    private Matchmaker matchmaker;
    private Coroutine _animationCoroutine; // 코루틴 핸들러

    [SerializeField] private GameObject selectedStage;
    private Carousel carousel;
    private int carouseSelet;

    void Start()
    {
        carousel = selectedStage.GetComponent<Carousel>();
    }

    void Awake()
    {
        matchmaker = GetComponent<Matchmaker>();
        matchmaker.OnStatusChanged += HandleStatus;
    }

    void OnDestroy()
    {
        matchmaker.OnStatusChanged -= HandleStatus;
    }

    public void OnSelectBoss(int bossId) => matchmaker.SetBossId(bossId);

    public void OnClickMatchStart()
    {
        matchmaker.SetBossId(carousel.currentSelected);

        matchmakingPopup.SetActive(true);
        matchmakingOverlay.SetActive(true);
        matchmaker.StartMatchmaking();
    }

    public void OnClickCancel()
    {
        StopTextAnimation();
        matchmakingOverlay.SetActive(false);
        matchmaker.CancelAndShutdown();
        matchmakingPopup.SetActive(false);
        // 필요하다면 보스 선택 창이나 메인 화면으로 돌아가는 로직 추가
    }

    void HandleStatus(MatchStatus status)
    {
        // 새로운 상태가 오면, 일단 이전 애니메이션 코루틴은 중지
        StopTextAnimation();

        switch (status)
        {
            case MatchStatus.Searching:
                _animationCoroutine = StartCoroutine(AnimateEllipsis("Waiting"));
                break;
            case MatchStatus.RoomCreated:
                _animationCoroutine = StartCoroutine(AnimateEllipsis("Searching"));
                break;
            case MatchStatus.JoinedRoom:   statusText.text = "Joining..."; break;
            case MatchStatus.PlayerJoined:
                _animationCoroutine = StartCoroutine(AnimateEllipsis("Searching"));
                break;
            case MatchStatus.Starting:     statusText.text = "Starting..."; break;
            case MatchStatus.Failed:       statusText.text = "Failed"; break;
            case MatchStatus.Disconnected: statusText.text = "Disconnected"; break;
        }
    }

    private void StopTextAnimation()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }

    private IEnumerator AnimateEllipsis(string baseText)
    {
        int dotCount = 1;
        while (true)
        {
            statusText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount % 3) + 1; // 1, 2, 3 반복
            yield return new WaitForSeconds(1.0f);
        }
    }
}
