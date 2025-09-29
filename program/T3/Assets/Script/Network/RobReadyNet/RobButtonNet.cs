using UnityEngine;
using UnityEngine.UI;
using Fusion;

// UI 버튼에 붙여서 사용합니다.
[RequireComponent(typeof(Button))]
public class RobButtonNet : MonoBehaviour
{
    [SerializeField] private RobReadyData robReadyData; // 이 버튼이 사용할 RobReadyData (Cost, 프리팹 정보)

    private Button _button;
    private float _cost;

    void Start()
    {
        _button = GetComponent<Button>();
        _cost = robReadyData.cost;

        _button.onClick.AddListener(OnButtonClicked);

        if (UnitPlacementManager.Instance != null)
        {
            UnitPlacementManager.Instance.OnPlacementModeChanged += HandlePlacementModeChange;
        }
    }

    void OnDestroy()
    {
        if (UnitPlacementManager.Instance != null)
        {
            UnitPlacementManager.Instance.OnPlacementModeChanged -= HandlePlacementModeChange;
        }
    }

    void Update()
    {
        if (UnitPlacementManager.Instance != null && UnitPlacementManager.Instance.IsInPlacementMode())
        {
            return;
        }
        
        if (PlayerCostManager.Instance != null)
        {
            _button.interactable = PlayerCostManager.Instance.CurrentCost >= _cost;
        }
    }

    private void OnButtonClicked()
    {
        // RobReadyData에 있는 프리뷰 프리팹을 배치하도록 요청
        
        if (robReadyData != null && robReadyData.RobRedayPrefab != null)
        {
            var previewPrefab = robReadyData.RobRedayPrefab.GetComponent<NetworkObject>();
            if (previewPrefab != null)
            {
                UnitPlacementManager.Instance.BeginPlacingUnit(previewPrefab, robReadyData);
            }
            else
            {
                Debug.LogError("RobReadyData의 RobRedayPrefab에 NetworkObject 컴포넌트가 없습니다.");
            }
        }
    }

    private void HandlePlacementModeChange(bool isInPlacementMode)
    {
        if (isInPlacementMode)
        {
            _button.interactable = false;
        }
    }
}
