using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// 게임 상태 정의 열겨형
public enum GameState
{
    MainMenu,
    InGame,
    GameOver
}

/// <summary>
/// 게임 상태와 점수 및 생명 등 전체 게임 흐름을 관리하는 클래스
/// </summary>
public class GameManager : Singleton<GameManager>, IBarStateListener
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu; // 현재 게임 상태

    [SerializeField] private float exchangeDuration = 1f; // 점수 교환 연출 시간

    public RectangleHandler rectangleHandler;   // 바 관련 처리
    public LifeHandler lifeHandler;             // 생명 관리
    public JetSpawner jetSpawner;               // 제트 생성
    public StickySpawner stickySpawner;         // 스티키 생성

    public bool isFever = false;        // 피버 상태 여부
    private int bestScore = 0;          // 최고 점수
    private float scoreAmount = 1.0f;   // 점수 배율

    private float _score = 0;   // 내부 점수 값
    public int score            // 외부 노출 점수
    {
        get { return (int)_score; } 
        set { _score = value; }
    } 

    private int _luminar = 1000;    // 내부 자원 값
    public int luminar              // 외부 노출 자원 값
    { 
        get { return _luminar; }
        set { _luminar = Math.Max(value,0); } // 음수 방지
    }

    private int life = 3; // 현재 생명

    public delegate void GameStateChanged(GameState newState); // 상태 변경 이벤트
    public event GameStateChanged OnStateChanged;

    /// <summary>
    /// 게임 상태를 변경하는 메서드
    /// </summary>
    public void SetState(GameState state)
    {
        CurrentState = state;
        OnStateChanged?.Invoke(CurrentState); // 모든 리스너에 전파
    }

    protected override void Awake()
    {
        base.Awake();
        Init(); // 리스너 등록
        PostInit(); // 초기 설정
    }

    // 리스너 자동 등록
    void Init()
    {
        var listeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (var listener in listeners)
        {
            OnStateChanged += listener.OnStateChanged;
        }
    }

    // 초기 UI 및 버튼 설정
    void PostInit()
    {
        StartCoroutine(PostInitRoutine());
    }

    IEnumerator PostInitRoutine()
    {
        yield return null;

        // 버튼 이벤트 연결
        ButtonManager.Instance.startButton.OnClick += SetInGame;
        ButtonManager.Instance.exitButton.OnClick += QuitGame;
        ButtonManager.Instance.noButton.OnClick += SetMainMenu;

        // 초기 연출
        yield return StartCoroutine(rectangleHandler.Fade(true));

        UIManager.Instance.Init();
    }

    // 버튼 이벤트용 상태 전환
    void SetInGame(TextMeshProUGUI text)
    {
        SetState(GameState.InGame);
    }

    void SetMainMenu(TextMeshProUGUI text)
    {
        SetState(GameState.MainMenu);
    }

    /// <summary>
    /// 인게임 종료 후 초기화를 진행하는 메서드
    /// </summary>
    void InitInGame()
    {
        StartCoroutine(InitInGameRoutine());
    }

    IEnumerator InitInGameRoutine()
    {
        UpdateBestScore();

        // 점수 감소 연출
        yield return StartCoroutine(ExchangeMotion());

        yield return new WaitForSeconds(1f);

        // 게임 오버 상태로 전환
        SetState(GameState.GameOver);

        yield return new WaitForSeconds(2f);

        // 상태 초기화
        life = 3;
        lifeHandler.InitLife();
        scoreAmount = 1.0f;

        UIManager.Instance.ToText(UIManager.Instance.scoreAmountText, scoreAmount);
    }

    /// <summary>
    /// 성공 판정 처리 메서드
    /// </summary>
    public void Fit()
    {
        rectangleHandler.FeverCheck(); // 피버 체크
        AddScore(); // 점수 증가
    }

    /// <summary>
    /// 실패 판정 처리 메서드
    /// </summary>
    public void Unfit()
    {
        LoseLife(); // 생명 감소
        isFever = false;

        ButtonManager.Instance.FeverBonusText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 최고 점수를 갱신하는 메서드
    /// </summary>
    public void UpdateBestScore()
    {
        if(score > bestScore)
        {
            bestScore = score;

            UIManager.Instance.ToText(UIManager.Instance.bestScoreText, bestScore);
            UIManager.Instance.UpdateBounce();
        }
    }

    /// <summary>
    /// 점수 배율을 증가시키는 메서드
    /// </summary>
    public void AddScoreAmount(float amount)
    {
        scoreAmount += amount;

        UIManager.Instance.ToText(UIManager.Instance.scoreAmountText, scoreAmount);
    }

    /// <summary>
    /// 점수를 증가시키는 메서드
    /// </summary>
    public void AddScore()
    {
        // 피버 상태면 2배
        _score += isFever ? scoreAmount * 2 : scoreAmount;

        UIManager.Instance.ToText(UIManager.Instance.scoreText, score);

        // 추가 오브젝트 생성
        stickySpawner.SpawnSticky();
        jetSpawner.SpawnJet();
    }

    /// <summary>
    /// 생명을 감소시키는 메서드
    /// </summary>
    public void LoseLife()
    {
        --life;

        // UI에서 생명 하나 비활성화
        lifeHandler.lifeList[life].gameObject.SetActive(false);

        if (life == 0)
        {
            // 바 초기화
            for(int i = 0; i < 4; ++i)
                rectangleHandler.barList[i].InitBarLevel();

            InitInGame();
        }
        else
        {
            // 생명이 남아있으면 제트 생성
            jetSpawner.SpawnJet();
        }
    }

    /// <summary>
    /// 점수를 0으로 감소시키는 연출 메서드
    /// </summary>
    IEnumerator ExchangeMotion()
    {
        yield return new WaitForSeconds(1.5f);

        float time = 0;
        float startS = score;

        _score = 0f;

        while (time < exchangeDuration)
        {
            float percent = time / exchangeDuration;

            float scoreTmp = Mathf.Lerp(startS, 0f, percent);

            UIManager.Instance.ToText(UIManager.Instance.scoreText, (int)scoreTmp);

            time += Time.deltaTime;
            yield return null;
        }

        UIManager.Instance.ToText(UIManager.Instance.scoreText, 0);
    }

    /// <summary>
    /// 게임을 종료하는 메서드
    /// </summary>
    public void QuitGame(TextMeshProUGUI text)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}