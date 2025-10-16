
## 2025년 10월 16일 - 클라이언트 연출 문제 디버깅

### 주요 진행 상황
- **생명주기 리팩토링 완료:** `RobHpNet`, `RobAttackNet`, `RobDetectorNet`, `RobMoveNet`, `RobShooterNet` 등 `DelayedStart` 코루틴을 사용하던 모든 스크립트를 Fusion의 표준 생명주기(`Spawned`, `FixedUpdateNetwork`)를 사용하도록 성공적으로 리팩토링함. 이를 통해 기존의 수많은 `NullReferenceException` 초기화 오류를 해결함.
- **RPC 오류 해결:** `BulletsNet`의 RPC 이름 규칙 오류를 해결함.

### 현재 진행중인 문제 및 다음 단계

현재 클라이언트 측에서 애니메이션과 이펙트가 정상적으로 보이지 않는 두 가지 연출 문제를 디버깅하고 있습니다.

**1. 클라이언트 애니메이션 미재생 문제**
- **문제 현상:** 유닛의 이동, 공격, 죽음 애니메이션이 클라이언트에서 보이지 않음.
- **현재 진단:** 코드 로직보다는 Unity 에디터 설정 문제일 가능성이 매우 높음. `NetworkTransform`이 위치를 제어하는 상황에서 `Animator`의 `Apply Root Motion` 설정이 충돌을 일으키는 것으로 추정.
- **내일 할 일:**
    1. 유닛 프리팹의 `Animator` 컴포넌트 확인.
    2. **`Apply Root Motion`** 체크 해제.
    3. **`Culling Mode`**를 `Always Animate`로 변경.

**2. 클라이언트 타격 이펙트 미재생 문제**
- **문제 현상:** 총알이 적에게 부딪혔을 때 타격 이펙트(`hitPrefab`)가 클라이언트에서 보이지 않음.
- **현재 진단:** 사용자가 `EffectManager`가 멀티플레이 씬에 존재함을 확인. 따라서 `EffectManager.instance`가 RPC 호출 시점에 `null`일 가능성을 조사해야 함. 스크립트 실행 순서 또는 `EffectManager` 오브젝트의 비활성화 상태가 원인일 수 있음.
- **내일 할 일:**
    1. `EffectManager.cs`의 `Awake()` 함수에 `Debug.Log`를 추가하여 `instance`가 언제 할당되는지 추적.
    2. `BulletsNet.cs`의 `RPC_DestroyBullet()` 함수에 `Debug.Log`를 추가하여, RPC 호출 시점의 `EffectManager.instance` 상태를 확인.
    3. 클라이언트의 콘솔 로그를 분석하여 문제의 원인 최종 확정.
