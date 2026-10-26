# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

Unity 6 (`6000.3.22f1`, URP 17.3) 기반 3D FPS 학습용 게임 프로젝트. 씬은 `Assets/01.Scenes/GameScene.unity` 하나이며, 직접 작성한 게임 코드는 모두 `Assets/02.Scripts/` 아래에 있다. 그 외 `Assets/` 하위 폴더(`BloodDecalsAndEffects`, `Plugins`, `TextMesh Pro`, `Workshop`, `TutorialInfo` 등)는 외부 에셋/템플릿이므로 수정하지 않는다.

## 빌드 / 실행 / 테스트

- 빌드·실행은 Unity 에디터에서 한다. CLI 빌드 스크립트, 린터, 작성된 테스트는 없다. (`com.unity.test-framework`는 패키지만 설치되어 있음)
- 루트의 `*.csproj` / `3DFPSGame.sln`은 Unity가 자동 생성하는 파일이므로 직접 편집하지 않는다.
- 에디터에서 실행하지 않고는 동작을 확인할 수 없으므로, 실행해 보지 않았다면 실행했다고 말하지 않는다.

## 아키텍처

### 게임 흐름과 싱글톤
- `GameManager`(싱글톤)가 `GameState`(`Ready` → `Playing` → `GameOver`)를 관리한다. 시작 시 코루틴으로 "준비.. → 시작!" 연출 후 `Playing`으로 전환한다.
- 게임플레이 스크립트(`PlayerManager`, `PlayerGunFire`, `Zombie` 등)는 `Update()` 첫 줄에서 `GameManager.Instance.GameState != GameState.Playing`이면 return 하는 패턴을 따른다. 새 게임플레이 로직도 이 패턴을 지킨다.
- `PlayerManager`(싱글톤)가 플레이어의 체력/스태미나 데이터를 소유하고, 체력이 0이 되면 `GameManager.GameOver()`를 호출한다. `Player` 컴포넌트는 피격 처리만 하고 실제 수치 변경은 `PlayerManager`에 위임한다.

### 대미지 시스템
- 피해를 받을 수 있는 모든 오브젝트는 `IDamageable.TakeDamage(DamageInfo)`를 구현한다 (`Player`, `Zombie`, `Box`, `Drum` 등).
- `DamageInfo`는 피해량과 피격 지점/법선을 담는 struct로, 피 튀김·이펙트 방향 계산에 쓰인다.
- 공격 측은 대상 타입을 모른 채 `collider.GetComponent<IDamageable>()?.TakeDamage(...)`로 호출한다 (총: Raycast, 폭탄: `Physics.OverlapSphere`).

### 적 AI
- `Zombie`는 `ZombieState` enum 기반 유한 상태 머신(Idle/Roaming/Trace/Comeback/Attack/Hit/Death)이다. 상태 전환은 `ChangeState()`를 거치며, 배회·피격·사망은 코루틴으로 처리하고 핸들을 필드에 보관해 중복 실행을 막는다.
- 이동은 NavMesh가 아닌 `CharacterController`로 직접 처리한다.

### 오브젝트 풀링
- 총알 피격 이펙트, 폭탄, 폭발 이펙트 등 반복 생성되는 오브젝트는 Lean Pool(`Lean.Pool.LeanPool.Spawn/Despawn`)을 사용한다. 풀링 대상에 `Instantiate`/`Destroy`를 섞어 쓰지 않도록 주의한다.

### 카메라 / UI
- 카메라는 `CameraFollow`(시점 모드 `CameraViewMode`)와 `CameraViewSwitcher`(V키로 1인칭 → 3인칭 → 쿼터뷰 → 숄더뷰 → 탑다운 순환), `CameraRotate`로 구성된다. 미니맵은 별도 `MinimapCamera` + `UI_MiniMap`.
- UI 스크립트는 `UI_` 접두사를 사용한다 (`UI_PlayerStats`, `UI_ScreenEffect` 등).

### 입력
- Player Settings의 Active Input Handling이 "Both"이지만, 현재 코드는 레거시 `Input` API(`Input.GetButton`, `Input.GetKeyDown`)만 사용한다. 새 코드도 주변 코드와 일관되게 레거시 API를 사용한다.

### 기타 폴더
- `StudyScripts/`, `Test/`는 수업/실습용 스크립트로 실제 게임 로직과 무관하다.

## 코드 컨벤션

- private 필드는 `_camelCase`, Inspector 노출은 public 필드 대신 `[SerializeField] private`를 우선한다.
- 코루틴 메서드명은 `xxx_Coroutine` 형식으로 쓴다.
- 주석과 답변은 한국어로 작성한다.

## 작업 지침

- 코드 리뷰 시 `Docs/CodeReviewGuide.md`의 기준(중요도 분류 Critical/High/Medium/Low, 매직 넘버, 네이밍 등)을 적용한다.
- 요청받지 않았다면 직접 코드를 변경하거나 커밋하지 않는다. 작업과 관련 없는 파일은 수정하지 않는다.
- 변경된 코드의 의도와 주변 코드를 함께 확인한다.
- 커밋 메시지는 `feat:`, `fix:`, `asset:` 접두사 + 한국어 설명 형식을 따른다.
