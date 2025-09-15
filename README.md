**Managers**

* **GameManager.cs** : 게임 전체 흐름(상태 전환, 점수/목숨/피버 관리, UI 및 스폰 제어)을 총괄
* **AudioManager.cs** : 배경음악(BGM)·효과음(SFX) 재생을 관리하며 게임 상태 변화에 따라 자동으로 BGM 제어
* **FeverEffecter.cs** : 싱글턴으로 관리되는 파티클 효과 실행기, 일정 간격으로 불꽃놀이(Fever) 파티클을 순차 재생
* **RandomPosGenerator.cs** : 카메라 뷰포트 영역을 분할해 프리팹의 랜덤 위치를 계산·반환

**InGame**

* **Bar.cs** : Jet 충돌 시 색상 일치 여부를 판정해 바(Level) 증가·초기화 및 발광 강도/효과음 갱신
* **RectangleHandler.cs** : 4개의 Bar를 초기화/회전/확장·수축·페이드 처리, 입력(키보드·터치) 제어, Fever 조건 감지
* **Jet.cs** : 발사체 역할, 색상·방향·속도를 갖고 이동하며 충돌 시 효과 후 풀로 복귀, 상태별 속도 전환
* **JetPool.cs** : 색상×방향 조합 Jet을 미리 생성해 풀링, 재사용 관리
* **JetSpawner.cs** : JetPool에서 Jet을 랜덤 스폰, 게임 상태에 따라 자동 스폰 루틴 실행
* **Net.cs** : Jet 충돌 시 `Net()` 처리 후 효과음 실행, `OnNet` 이벤트 발행
* **NetHandler.cs** : 씬 내 모든 Net을 수집하고 OnNet 시 생명 차감 및 Bar 초기화 동작을 연결

* **Sticky.cs** : Sticky 기본 클래스, Bar/Net/GameState 이벤트 구독·해제 및 Fit/Unfit 동작 정의
* **BB.cs** : Sticky 파생 클래스, 인트로/아웃트로 연출과 Wobble 애니메이션, Fit/Unfit 시 애니메이션 트리거
* **SpawnedSticky.cs** : 플레이 중 랜덤 생성 Sticky, Stick/Unstick 애니메이션과 Fade/회전/스케일 효과, 풀 반환
* **StickySpawner.cs** : Sticky 풀링 관리, 보너스 확률·랜덤 위치 적용 후 Sticky 스폰
* **Tail.cs** : 라인렌더러 기반 꼬리 트레일, 목표 방향 따라 부드럽게 이어지고 흔들림 애니메이션 적용

**UI**

* **UIManager.cs** : 게임 상태별 UI 전환, 점수/루미나 표시, 텍스트 애니메이션(스케일/바운스) 관리
* **ButtonManager.cs** : 클릭 가능한 텍스트/이미지에 이벤트 연결, 깜빡임·흔들림 효과 부여
* **BlinkingText.cs** : 지정 텍스트를 일정 간격으로 깜빡이게 하며 필요 시 활성화
* **CanvasScaler.cs** : 카메라 뷰포트 크기에 맞게 UI 캔버스를 자동 스케일링
* **ChildsWobbler.cs** : 자식 오브젝트들의 원래 위치 기준 흔들림 애니메이션 적용
* **BoxHandler.cs** : 자식 오브젝트들을 위아래로 흔들리듯 움직이는 핸들러
* **ClickableImage.cs** : 이미지에 마우스 이벤트(Enter, Exit, Down, Up, Click) 제공
* **ClickableText.cs** : 텍스트에 마우스 이벤트(Enter, Exit, Click, Idle) 제공
* **LifeHandler.cs** : 플레이어 목숨 UI 관리, 초기화 시 모든 라이프 아이콘 활성화
* **IntroManager.cs** : 인트로 연출(CRT 셰이더 효과, 타이틀 애니메이션, 깜빡이는 텍스트 표시), 입력 시 씬 전환


**Interface & Utils**

* **IBarStateListener.cs** : Bar 상태 변화 시 `Fit()`·`Unfit()` 동작 강제하는 인터페이스
* **IGameStateListener.cs** : 게임 상태 변경 시 `OnStateChanged(GameState state)`를 호출받는 리스너 인터페이스
* **DebugX.cs** : 로그 메시지에 파일명·라인·메서드명 자동 포함, 디버깅 편의 제공
* **Singleton.cs** : 제네릭 싱글턴 패턴 구현, 씬 전환 간 파괴되지 않고 하나만 유지되는 MonoBehaviour 베이스

