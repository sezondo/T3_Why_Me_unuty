using UnityEngine;
using Fusion;
using System;

public partial class UnitPlacementManager : NetworkBehaviour
{
    public static UnitPlacementManager Instance { get; private set; }

    [SerializeField] private LayerMask groundLayer; // 유닛을 배치할 수 있는 땅 레이어

    private NetworkObject _unitToPlacePrefab; // 배치할 유닛의 프리팹
    private GameObject _previewInstance; // 위치 지정을 위한 시각적 프리뷰 오브젝트

    public event Action<bool> OnPlacementModeChanged; // 배치 모드 변경 시 호출될 이벤트
    private RobReadyData _currentRobReadyData; // 현재 배치할 유닛의 RobReadyData

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_unitToPlacePrefab != null)
        {
            MovePreview();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceUnit();
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    // BeginPlacingUnit을 RobReadyData도 받도록 수정
    public void BeginPlacingUnit(NetworkObject unitPrefab, RobReadyData robReadyData)
    {
        if (PlayerCostManager.Instance.CurrentCost < robReadyData.cost)
        {
            Debug.Log("코스트가 부족합니다!");
            return;
        }

        _unitToPlacePrefab = unitPrefab;
        _currentRobReadyData = robReadyData; // RobReadyData 저장

        _previewInstance = Instantiate(unitPrefab.gameObject);
        if(_previewInstance.GetComponent<NetworkObject>())
           _previewInstance.GetComponent<NetworkObject>().enabled = false;

        OnPlacementModeChanged?.Invoke(true);
        Debug.Log($"{unitPrefab.name} 배치 시작.");
    }

    private void CancelPlacement()
    {
        Destroy(_previewInstance);
        _unitToPlacePrefab = null;
        _previewInstance = null;
        OnPlacementModeChanged?.Invoke(false);
        Debug.Log("배치 취소됨.");
    }

    private void MovePreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            _previewInstance.transform.position = hit.point;
        }
    }

    // TryPlaceUnit에서 RPC 호출 시 RobReadyData의 ID를 넘겨주도록 수정
    private void TryPlaceUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            // RPC 호출 시 프리팹 ID와 위치, RobReadyData의 경로를 넘겨줌
            RPC_SpawnPreviewUnit(_unitToPlacePrefab.Id, hit.point, _currentRobReadyData.name);

            Destroy(_previewInstance);
            _unitToPlacePrefab = null;
            _previewInstance = null;
            _currentRobReadyData = null;
            OnPlacementModeChanged?.Invoke(false);
        }
        else
        {
            Debug.Log("배치할 수 없는 위치입니다.");
        }
    }

    // RPC 이름을 RPC_SpawnPreviewUnit으로 변경하고, RobReadyData 이름을 받도록 함
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SpawnPreviewUnit(NetworkId previewPrefabId, Vector3 spawnPosition, string robReadyDataName)
    {
        var previewPrefab = Runner.FindObject(previewPrefabId);
        if (previewPrefab == null) return;

        // Resources 폴더에서 RobReadyData를 이름으로 로드
        var robReadyData = Resources.Load<RobReadyData>(robReadyDataName);
        if (robReadyData == null)
        {
            Debug.LogError($"{robReadyDataName} RobReadyData를 Resources 폴더에서 찾을 수 없습니다.");
            return;
        }

        if (PlayerCostManager.Instance.SpendCost(robReadyData.cost))
        {
            var previewInstance = Runner.Spawn(previewPrefab, spawnPosition, Quaternion.identity, Object.InputAuthority);
            // 스폰된 프리뷰에 RobReadyData 정보를 넘겨줌
            previewInstance.GetComponent<RobPreviewNet>().Initialize(robReadyData);
            Debug.Log($"{previewPrefab.name} 프리뷰를 {spawnPosition}에 스폰했습니다.");
        }
        else
        {
            Debug.Log("서버 확인 결과, 코스트가 부족하여 스폰에 실패했습니다.");
        }
    }

    public bool IsInPlacementMode()
    {
        return _unitToPlacePrefab != null;
    }
}