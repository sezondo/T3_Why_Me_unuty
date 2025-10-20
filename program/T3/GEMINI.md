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

---

## 2025년 10월 20일 - 클라이언트 연출 문제 해결 및 리팩토링

### 주요 해결 내용
- **`Render` 루프 기반 애니메이션 처리 패턴 확립:** `FixedUpdateNetwork`에서 시각 효과를 처리할 때 발생하는 문제를 진단하고, 모든 애니메이션 및 시각 효과는 `Render` 함수에서 처리하는 것으로 패턴을 확립함.
- **`RobBaseNet` 수정:** `Render` 함수에서 `currentState`를 기반으로 `animator.SetInteger`를 호출하여 이동/정지 등 지속적인 상태 변화 애니메이션을 클라이언트에서 부드럽게 재생하도록 수정함.
- **`RobAttackNet` 리팩토링:** 공격 애니메이션처럼 단발성 이벤트의 경우, RPC와 `Render`의 충돌 문제를 발견함. 이를 해결하기 위해 `[Networked]` Tick 변수(`AttackTick`)를 도입. 서버가 공격 시 Tick을 증가시키면, 클라이언트의 `Render`가 변화를 감지하고 애니메이션을 딱 한 번만 재생하는 안정적인 방식으로 리팩토링 완료.
- **`RobHpNet` 리팩토링:** 죽음 연출(`DeadDisposal`)이 `FixedUpdateNetwork`에서 호출되던 문제를 수정. `Render` 함수에서 `Dead` 상태를 감지하여 죽음 연출이 클라이언트에서 정상적으로 한 번만 재생되도록 수정함.

### 다음 단계
- **패턴 일관성 적용:** 새로 확립한 `Render` 기반의 시각 처리 패턴을 아직 확인하지 않은 다른 네트워크 스크립트(예: `RobShooterNet`의 총구 섬광 효과)에도 일관적으로 적용할 예정.
- **타격 이펙트 문제 디버깅 재개:** 애니메이션 문제를 해결하는 동안 잠시 보류했던 '총알 타격 이펙트' 문제의 디버깅을 계속 진행할 예정.