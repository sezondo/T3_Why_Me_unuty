# T3_Why_Me_unuty 

<img width="1780" height="980" alt="image" src="https://github.com/user-attachments/assets/757ea4d1-d76e-434c-8f42-b016cc2416d7" />



유튜브 링크 : https://youtu.be/a92dWzTW6Yc?si=XboiPHjLudrlJBKZ

----------------------------------------------------------------------------------------------------------------

# 프로그램 설명서

개요

T3 프로젝트는 전술 전략 요소와 액션이 결합된 Unity 기반 게임으로, 플레이어는 다양한 **로봇(유닛)**을 배치하고, 적과 전투를 벌여 승리 조건을 달성하는 것을 목표로 합니다. 본 설명서는 프로젝트 구조와 주요 클래스 및 시스템의 역할을 개괄합니다.

핵심 데이터 정의 (Enums & ScriptableObjects)

데이터 및 상태를 정의하는 구성요소들은 다음과 같습니다.

열거형 (Enums)

BattleState – 전투의 결과 상태를 정의합니다: inJudgment, win, loss.

FactionType – 유닛의 소속을 정의합니다: Ally, Enemy.

ReadyManagerState – 게임 시작 전 준비 상태: Ready, Start.

ReadyUnitState – 배치 준비 중인 유닛의 상태: Readying, Readyed.

UnitState – 각 유닛의 행동 상태: Idle, Moving, Attacking, Dead, Turn, Hurt.

스크립터블 오브젝트 (ScriptableObjects)

BulletData – 총알 속도/수명/피해량/경로 등의 기본 데이터를 담습니다.

RobData – 개별 로봇의 스탯(체력, 이동 속도, 공격 속도, 회전 속도 등) 및 소속과 발사체 프리팹을 정의합니다.

LevelData – 스테이지의 비용 및 레벨 정보를 보유합니다.

RobReadyData – 준비 상태에서 사용할 프리팹과 배치 비용을 지정합니다.

핵심 유닛 로직 (Core Unit)

전투와 이동 등을 담당하는 기본 클래스들입니다.

RobBase – 각 유닛의 기본 스크립트로, RobData를 보유하고 UnitState를 관리합니다. 유닛의 레이어 설정(Ally/Enemy)도 담당합니다.

RobHp – 유닛의 체력과 피해 처리를 담당합니다. 체력이 0 이하가 되면 UnitState.Dead로 전환하고 소멸시킵니다.

RobMove – 네비메시를 활용해 가장 가까운 적 유닛을 추적·회전시키는 로직을 담당합니다. currentTarget을 공개하고 추적 및 이동/정지 상태를 관리합니다.

RobDetector – 레이캐스트로 적 감지 및 공격 개시 여부를 판별합니다. robBase와 robMove를 참조하며, 감지 플래그와 레이어 마스크를 관리합니다.

Rob04Detector – RobDetector를 상속하며, 벽을 무시하고 오직 적을 감지하도록 레이어 마스크를 설정합니다.

RobBaseReady – 준비 상태에서 유닛 프리뷰를 나타내는 컴포넌트로, ReadyUnitState와 RobReadyData를 보유합니다. 시작 상태에서 사라집니다.
RobReadyData 안에는 RobData가 존재합니다.

공격 시스템 (Attack)

공격 동작과 공격 타입을 정의합니다.

RobAttack – 기본 공격 클래스. IAttack 인터페이스를 구현하며, RobBase, Animator, RobShooter[], CoroutineCheck(공개) 및 공격 속도에 따른 코루틴을 통해 공격을 수행합니다.

Rob04Attack – 공격 타이밍을 오버라이드하여 애니메이션과 발사 타이밍을 별도로 조절합니다. (미사일 유닛)

Rob08Attack – 지속 발사형 공격을 구현한 클래스. Rob08Stooter[]를 사용해 매 프레임 Fire() 호출, 사망/상태 변경 시 발사 중단을 처리합니다. (화염 방사 유닛)

발사체와 발사기 (Shooter & Bullets)

발사기

RobShooter – IShooter 인터페이스를 구현하는 기본 발사기. 부모의 RobData를 통해 프리팹을 생성합니다.
여기서 부모는 RobAttack을 의미하며 RobAttack가 가진 RobBase를 참조합니다.

Rob04Shooter – 타겟 좌표를 설정하고 포물선 형태의 Rob04Bullets를 발사합니다.

Rob08Stooter – 자식의 Rob08Bullets를 사용해 화염/지속 공격을 수행하고, 발사 시작/정지를 제어합니다.

발사체

Bullets – 기본 총알 스크립트. 속도·수명·피해량 등은 전부 비공개 필드로 설정되었으며, 충돌 시 RobHp.TakeDamage()를 호출하고 EffectManager로 이펙트를 재생합니다.

Rob04Bullets – 곡사 발사체로, 목표 지점까지 이동 후 폭발해 주위 적에게 피해를 줍니다. SoundManager를 통해 SFX 재생을 추가합니다.

Rob08Bullets – 화염/연기 파티클과 지속 피해를 제공하는 발사체. attackTime에 따라 활성화되며, 여러 대상에게 연속적으로 피해를 줍니다.

준비 및 배치 시스템 (Ready & Placement)

배틀 시작 전 유닛을 배치하는 로직입니다.

ReadyManager – 싱글톤으로 배치 로직과 UI를 관리합니다. 현재 배치 중인 프리뷰(currentPreviews), useButton 플래그, 스테이지 levelData, 현재 비용 currentCost 등이 공개 필드로 제공됩니다. 유닛들을 GameStart() 시 본체로 인스턴스화하며, 비용 초과 및 겹침 여부를 체크해 팝업을 띄웁니다.

RobButton – 다양한 프리뷰 프리팹을 보유하고 있으며, 버튼 클릭 시 RobReadyData에 정의된 프리뷰를 인스턴스화합니다. ReadyManager.useButton 플래그를 통해 중복 드래그를 방지합니다.

RobDragAndDrop – 드래그 중인 프리뷰를 Ground 위로 이동시키고, 마우스 업 시 코스트 및 겹침 체크 후 배치를 확정(ReadyUnitState.Readyed)하거나 취소합니다. ReadyManager와 RobReadyData.cost를 사용합니다.

RobReadyEnable – 프리뷰의 ReadyUnitState에 따라 렌더러를 활성/비활성화하여 배치 전 시각 효과를 지원합니다.

관리자 컴포넌트 (Managers)

ReadyManager – 앞서 설명한 준비 시스템의 핵심. 배치/코스트/팝업 UI/사운드 등을 통합 관리합니다.

PlayerManager – 현재 최고 클리어 스테이지를 기록하고 업데이트하는 싱글톤입니다.

SoundManager – SFX 및 UI 사운드를 재생합니다. 공격/폭발/클릭 등 모든 사운드 요청을 처리합니다.

EffectManager – 모든 이펙트 프리팹을 재생하며 위치를 지정해 줍니다.

BattleManager – 전투 결과(BattleState)를 판정하는 싱글톤으로, ReadyManager 상태가 Start일 때 주기적으로 승패 여부를 확인해 GameEndPopup을 호출합니다.

UI & 기타 컴포넌트

UIButtonSound – UI 버튼 클릭 시 사운드를 재생합니다.

StartButton – UI 텍스트와 페이드 애니메이션을 관리합니다.

UIControl – 스테이지 선택 화면과 로봇 배치 UI를 관리하며, StageStart() 호출로 전투를 시작합니다.

NextStage – 다음 스테이지 씬을 로드합니다.

MemuRob – 메뉴에서 로봇을 표시하는 오브젝트 그룹입니다.

LevelDestroy – 준비 상태가 Start로 바뀌면 특정 오브젝트를 삭제합니다.

ReadyButtonDestroy – 준비 버튼을 DOTween으로 이동시키고 일정 시간 후 제거합니다.

ButtonMove – 버튼을 드래그 시 부드럽게 이동시키고 배치 완료 시 원위치로 이동시킵니다.

TouchCameraController – 터치/마우스를 통해 카메라 이동 및 줌을 제어합니다.


----------------------------------------------------------------------------------------------------------------

# 작업 내역
## 25.06.19
게임 기획 및 설계

## 25.06.25
클래스 설계 및 아이디어 구상

## 25.06.28
작업 설정 업데이트

## 25.06.30
작업 환경 이전 테스트

## 25.06.30
프로그램 작성 시작 및 FSM 설정

## 25.07.01
유닛 HP 설계 및 테스트

## 25.07.02
유닛 네비게이션 작성 및 테스트

## 25.07.03
유닛 공격 구현 및 네비게이션 코드 수정

## 25.07.04
유닛 식별 레이어마스크 실험 및 유닛 AI 구성

## 25.07.06
유닛 FSM 상태 추가 및 공격 개선

## 25.07.08
유닛 사망 판정
카메라 이동 개선
맵 추가

## 25.07.10
코드 정리

## 25.07.14
Git 정리 및 커밋 충돌 테스트

## 25.07.15
유닛 배치 시스템 초안 완성

## 25.07.16
UI 수정

## 25.07.17
게임 스타트 버튼 구현

## 25.07.22
배치 초기화 및 코스트 시스템 도입
게임 클리어 UI 제작 및 전투 종료 조건 확인

## 25.07.23
Rob07 유닛 완성

## 25.07.24
Rob 05, 02 완성

## 25.07.25
모든 유닛 완성 완료

## 25.08.03
레벨 디자인 및 메인 화면 구성

## 25.08.04
스테이지 제작

## 25.08.08
베타 버전 완성

## 25.08.11
영상 작업 및 완성
