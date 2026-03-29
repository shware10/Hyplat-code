# Hyplat-Code

Unity 기반 캐주얼 액션 게임 프로젝트로, UI 연출, 오브젝트 상호작용, 게임 상태 관리 등을 중심으로 구성된 코드 구조입니다.

---

## Folder Structure

### Utils

공통적으로 재사용되는 유틸리티

* **Singleton<T>** : MonoBehaviour 기반 제네릭 싱글톤 패턴 구현

---

### Managers

게임 전반의 상태, 오디오, 이펙트 및 생성 로직 관리

* **GameManager** : 게임 상태, 점수, 생명 및 전체 흐름 관리
* **AudioManager** : BGM 및 SFX 재생과 상태 기반 오디오 제어
* **FeverEffecter** : 피버 상태 파티클 연출 처리
* **RandomPosGenerator** : 화면 영역 기반 랜덤 위치 생성

---

### Interface

게임 상태 및 판정 이벤트 전달 인터페이스

* **IGameStateListener** : 게임 상태 변화 이벤트
* **IBarStateListener** : 성공/실패(Fit/Unfit) 판정 이벤트

---

### UI

UI 연출 및 인터랙션 처리

* **UIManager** : UI 전환 및 텍스트 연출 관리

* **ButtonManager** : 버튼 인터랙션 및 텍스트 애니메이션 처리

* **ClickableText** : 텍스트 UI 이벤트 처리

* **ClickableImage** : 이미지 UI 이벤트 처리

* **BlinkingText** : 텍스트 깜빡임 연출

* **IntroManager** : 인트로 연출 및 씬 전환

* **BoxHandler** : 자식 오브젝트 흔들림 연출

* **ChildsWobbler** : 자식 오브젝트 sin 기반 흔들림

* **CanvasScaler** : 카메라 기준 Canvas 크기 조정

* **LifeHandler** : 플레이어 체력(UI) 관리

---

### InGame

#### Sticky

스티키 오브젝트 생성 및 연출

* **Sticky** : 스티키 공통 동작 및 이벤트 처리

* **SpawnedSticky** : 생성된 스티키 연출 및 상호작용

* **StickySpawner** : 스티키 풀링 및 생성

* **BB** : 특정 스티키 연출 처리

* **Tail** : 꼬리(LineRenderer) 트레일 생성

#### Net

충돌 기반 이벤트 처리

* **Net** : 충돌 시 이벤트 발생 및 제트 처리
* **NetHandler** : Net 이벤트를 게임 로직에 연결

#### Jet

제트 오브젝트 생성 및 이동

* **Jet** : 이동, 충돌, 연출 및 상태 관리
* **JetSpawner** : 제트 생성 및 타이밍 제어
* **JetPool** : 제트 오브젝트 풀링 관리

#### Bar

바 오브젝트 및 판정 시스템

* **Bar** : 충돌 판정 및 레벨 관리
* **RectangleHandler** : 바 전체 관리 및 입력/연출 처리

---

## 특징

* 이벤트 기반 구조 (Listener 인터페이스 활용)
* 상태 기반 게임 흐름 관리 (GameState)
* 오브젝트 풀링을 활용한 성능 최적화
* UI와 로직 분리 구조 (Manager / View)
* 다양한 연출 시스템 (페이드, 바운스, 흔들림, 파티클)

---
