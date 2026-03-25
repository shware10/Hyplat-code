using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 사각형 바의 상태, 연출, 입력 제어를 관리하는 클래스
/// </summary>
public class RectangleHandler : MonoBehaviour, IGameStateListener, IBarStateListener
{
    public List<Bar> barList = new List<Bar>(); // 바 리스트
    private List<Collider> colList = new List<Collider>(); // 바 콜라이더 리스트

    Color[] BarColors = { Color.red, Color.green, Color.blue, Color.white }; // 바 색상 배열

    [Header("Touch")]
    private Vector2 swipeStart;             // 터치 시작 위치
    private float inchesThreshold = 0.2f;   // 스와이프 기준 거리
    private float dpi;                      // 화면 DPI
    private float minSwipeDist;             // 최소 스와이프 거리
    private float halfWidth;                // 화면 절반 기준

    [Header("확대 축소 연출")]
    [SerializeField] private float introDuration = 0.5f;    // 인트로 시간
    [SerializeField] private float targetScale = 4f;        // 목표 스케일
    [SerializeField] private float targetAngle = 180f;      // 목표 회전 각도
    [SerializeField] private float barTargetScale = 0.04f;  // 바 스케일 목표
    [SerializeField] private float barTargetPos = 0.3f;     // 바 위치 목표
    private float[] barDir = { 1, 1, -1, -1 }; // 바 방향

    [Header("회전 연출")]
    [SerializeField] private float rotateZDuration = 0.2f; // Z 회전 시간
    [SerializeField] private float rotateXDuration = 0.2f; // X 회전 시간

    [Header("바운스 연출")]
    [SerializeField] private float bounceDuration = 0.2f;  // 바운스 시간
    [SerializeField] private float bounceScale = 4.5f;     // 바운스 스케일

    [Header("페이드 연출")]
    [SerializeField] private float fadeDuration = 1f; // 페이드 시간

    private bool isRotating = false;    // 회전 중 여부
    private bool isInGame = false;      // 인게임 상태 여부
    private bool isBack = false;        // 뒤집힘 상태

    /// <summary>
    /// 게임 상태에 따라 연출을 처리하는 메서드
    /// </summary>
    public void OnStateChanged(GameState state)
    {
        isInGame = state == GameState.InGame; // 상태 갱신

        switch (state)
        {
            case GameState.InGame:
                StartCoroutine(Intro()); // 인트로 시작
                break;

            case GameState.GameOver:
                StartCoroutine(Outro()); // 아웃트로 시작
                break;

            case GameState.MainMenu:
                FadeIn(); // 페이드 인
                break;
        }
    }

    void Start()
    {
        int idx = 0;

        // 모든 리스너 수집
        var listeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                        .OfType<IBarStateListener>();

        foreach (Transform barTransform in transform)
        {
            Bar bar = barTransform.GetComponent<Bar>();

            bar.color = BarColors[idx++]; // 색상 설정

            // 이벤트 연결
            foreach (var listener in listeners)
            {
                bar.FitColor += listener.Fit;
                bar.UnfitColor += listener.Unfit;
            }

            barList.Add(bar);
            colList.Add(barTransform.GetComponent<Collider>());
        }

        // 터치 입력 기준값 설정
        dpi = Screen.dpi;
        minSwipeDist = inchesThreshold * dpi;
        halfWidth = Screen.width * 0.5f;
    }

    /// <summary>
    /// 확장 연출을 수행하는 메서드
    /// </summary>
    IEnumerator Expand()
    {
        float time = 0;
        float barOriginScale = barList[0].transform.localScale.x;

        while (time < introDuration)
        {
            float t = time / introDuration;

            // 전체 확대
            transform.localScale = Vector3.one * Mathf.Lerp(1f, targetScale, t);

            // 회전
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, targetAngle, t));

            float barScale = Mathf.Lerp(barOriginScale, barTargetScale, t);
            float barPos = Mathf.Lerp(0, barTargetPos, t);

            // 바 위치 및 크기 조정
            for (int i = 0; i < 4; ++i)
            {
                if (i % 2 == 0)
                {
                    barList[i].transform.localScale = new Vector3(barOriginScale, barScale, barOriginScale);
                    barList[i].transform.localPosition = new Vector3(0, barDir[i] * barPos, 0);
                }
                else
                {
                    barList[i].transform.localScale = new Vector3(barScale, barOriginScale, barOriginScale);
                    barList[i].transform.localPosition = new Vector3(barDir[i] * barPos, 0, 0);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 인트로 연출을 수행하는 메서드
    /// </summary>
    IEnumerator Intro()
    {
        yield return StartCoroutine(Expand()); // 확장 연출

        EnableCollider(isInGame); // 충돌 활성화

        yield return StartCoroutine(ActivateControll()); // 입력 시작
    }

    /// <summary>
    /// 입력을 받아 회전시키는 메서드
    /// </summary>
    IEnumerator ActivateControll()
    {
        isBack = false;

        while (isInGame)
        {
            if (!isRotating)
            {
                // 키 입력 처리
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    StartCoroutine(RotateY(transform.localEulerAngles.y, transform.localEulerAngles.y + 180));

                if (Input.GetKeyDown(KeyCode.RightArrow))
                    StartCoroutine(RotateY(transform.localEulerAngles.y, transform.localEulerAngles.y - 180));
            }

            yield return null;
        }
    }

    /// <summary>
    /// Y축 회전을 수행하는 메서드
    /// </summary>
    IEnumerator RotateY(float start, float end)
    {
        isBack = !isBack; // 방향 반전
        float time = 0;
        isRotating = true;

        float originX = transform.localEulerAngles.x;
        float originZ = transform.localEulerAngles.z;

        while (time <= rotateZDuration)
        {
            float angle = Mathf.Lerp(start, end, time / rotateZDuration);

            // 회전 적용
            transform.rotation = Quaternion.Euler(originX, angle, originZ);

            time += Time.deltaTime;
            yield return null;
        }

        isRotating = false;
    }

    /// <summary>
    /// 바운스 연출을 수행하는 메서드
    /// </summary>
    IEnumerator Bounce()
    {
        float time = 0;

        while (time < bounceDuration)
        {
            float t = time / bounceDuration;

            // sin 기반 스케일 변화
            float scale = Mathf.Lerp(targetScale, bounceScale, Mathf.Sin(t * Mathf.PI));

            transform.localScale = Vector3.one * scale;

            time += Time.deltaTime;
            yield return null;
        }
    }

    public void RectangleBounce()
    {
        StartCoroutine(Bounce()); // 바운스 실행
    }

    // 콜라이더 활성화 제어
    void EnableCollider(bool isInGame)
    {
        foreach (Collider col in colList)
        {
            col.enabled = isInGame; // 상태에 따라 on/off
        }
    }

    /// <summary>
    /// 피버 상태를 체크하는 메서드
    /// </summary>
    public void FeverCheck()
    {
        if (GameManager.Instance.isFever) return; // 이미 피버면 종료

        bool isFever = true;

        // 모든 바가 최대 레벨인지 검사
        for (int i = 0; i < barList.Count; ++i)
        {
            if (barList[i].level != 3) isFever = false;
        }

        if (isFever)
        {
            GameManager.Instance.isFever = true; // 피버 활성화

            ButtonManager.Instance.FeverBonusText.gameObject.SetActive(true); // UI 표시

            FeverEffecter.Instance.FireWork(); // 연출 실행
        }
    }
}