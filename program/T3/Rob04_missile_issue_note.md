# Rob04 네트워크 미사일 이슈 메모

## 현상
- 호스트에서는 Rob04 미사일이 정상 발사, 곡선 궤적/폭발 적용이 보이지만
  클라이언트(권한 없는 플레이어) 화면에는 미사일 오브젝트가 보이지 않고,
  폭발 RPC만 도착해 이펙트·사운드만 재생된다.

## 현재 코드 스냅샷
- `Rob04ShooterNet.Shoot()`에서 `Runner.Spawn()` 뒤 바로 `RPC_SetTarget(targetPos)` 호출.
- `Rob04BulletsNet`은 `[Networked] Vector3 TargetPos`, `[Networked] NetworkBool ArcPrepared`
  를 가지고 있으며, `RPC_SetTarget`에서 `TargetPos` 할당 후 `StartArc()` 호출.
- `StartArc()` 는 `arcStarted`와 `TargetPos == default`로 가드하고,
  DOTween `DOPath`로 곡선을 구성한 뒤 State Authority에서만 피해를 계산.
- 부모 `BulletsNet.Spawned()`는 비권한 객체에서 early return이 있어
  기본 `DelayedStart` 코루틴이 중단됨.

## 가능성 높은 원인
- 비권한 인스턴스에서 `BulletsNet.Spawned()`가 즉시 반환되면서
  `RunDelayedStart`/레이어 초기화 등 기본 셋업이 이루어지지 않아 DOTween이 정상 실행되었는지 불확실.
- `StartArc()`의 `TargetPos == default` 체크가 여전히 남아 있어,
  RPC 수신 전에 호출될 경우 arc가 영구적으로 막힐 수 있음.
- 네트워크 프리팹 등록 여부 재확인 필요 (`robBase.data.bulletPrefab`이 `NetworkProjectConfig`에 등록됐는지).
- RPC/Networked 값이 설정될 때까지 DOTween 실행을 지연시키는 로직이 클라 측에서 정상 작동하는지
  (디버그 로그로 `ArcPrepared`, `StartArc()` 호출 여부 확인 요망).

## 다음 액션 아이디어
1. `StartArc()` 가드 조건을 `if (arcStarted || !ArcPrepared)` 형태로 조정.
2. 클라에서 `RPC_SetTarget` 실행 여부, `TargetPos` 값 체크용 로그 임시 추가.
3. `BulletsNet.Spawned()`가 비권한 객체에서 즉시 리턴하는 부분을 재검토
   (프록시에서도 `base.Start()`가 호출되도록 수정할지 결정).
4. 위 조정 후 멀티 플레이 세션에서 재테스트.
