using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// UI 전환 점수/텍스트 연출을 담당하는 매니저 클래스
/// </summary>
public class UIManager : Singleton<UIManager>, IGameStateListener, IBarStateListener
{
    public TextMeshProUGUI scoreAmountText; // 점수 증가량 표시 텍스트
    public TextMeshProUGUI bestScoreText;   // 최고 점수 텍스트
    public TextMeshProUGUI scoreText;       // 현재 점수 텍스트
    public TextMeshProUGUI feverText;       // 피버 상태 텍스트
    
    [SerializeField] private CanvasGroup MainMenuUIGroup;   // 메인 메뉴 UI 그룹
    [SerializeField] private CanvasGroup InGameUIGroup;     // 인게임 UI 그룹
    [SerializeField] private CanvasGroup GameOverUIGroup;   // 게임오버 UI 그룹
    
    [SerializeField] private float vanishDuration = 1f;     // UI 사라지는 시간
    [SerializeField] private float appearDuration = 1f;     // UI 나타나는 시간
    [SerializeField] private float bounceDuration = 0.2f;   // 텍스트 애니메이션 시간
    [SerializeField] private float targetScale = 1.2f;      // 텍스트 확대 목표 크기

    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화
    }

    void Fade(CanvasGroup canvasGroup, bool isCurGroup)
    {
        // 이미 활성 상태면 불필요한 실행 방지
        if (canvasGroup.gameObject.activeSelf && isCurGroup) return;

        StartCoroutine(FadeRoutine(canvasGroup, isCurGroup));
    }

    IEnumerator FadeRoutine(CanvasGroup canvasGroup, bool isCurGroup)
    {
        // 새로운 UI 등장 전에 기존 UI 사라질 시간 대기
        if (isCurGroup)
            yield return new WaitForSeconds(vanishDuration);

        float time = 0f;

        if (!isCurGroup)
        {
            canvasGroup.blocksRaycasts = false; // 입력 차단

            while (time <= vanishDuration)
            {
                // alpha 감소 → fade out
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / vanishDuration);

                time += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(false); // 완전히 숨김
        }
        else
        {
            canvasGroup.gameObject.SetActive(true);

            while (time <= appearDuration)
            {
                // alpha 증가 > 페이드 인
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / appearDuration);

                time += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true; // 입력 허용
        }
    }

    /// <summary>
    /// 게임 상태에 따라 UI 전환
    /// </summary>
    public void OnStateChanged(GameState state)
    {
        Fade(MainMenuUIGroup, state == GameState.MainMenu);
        Fade(InGameUIGroup, state == GameState.InGame);
        Fade(GameOverUIGroup, state == GameState.GameOver);
    }

    /// <summary>
    /// 초기 UI 설정
    /// </summary>
    public void Init()
    {
        Fade(MainMenuUIGroup, true);
    }

    /// <summary>
    /// 텍스트 확대 → 복귀 연출
    /// </summary>
    public void ScoreScale(TextMeshProUGUI text, Color targetColor, Color endColor)
    {
        StartCoroutine(ScaleRoutine(text, targetColor, endColor));
    }

    IEnumerator ScaleRoutine(TextMeshProUGUI text, Color targetColor, Color endColor)
    {
        yield return ScaleMotion(text, 1f, targetScale, targetColor); // 확대
        yield return new WaitForSeconds(1f);                          // 유지
        yield return ScaleMotion(text, targetScale, 1f, endColor);    // 복귀
    }

    IEnumerator ScaleMotion(TextMeshProUGUI text, float originScale, float targetScale, Color targetColor)
    {
        float time = 0f;
        float duration = bounceDuration / 2;

        Color startColor = text.color;

        while (time < duration)
        {
            float t = time / duration;

            // 크기 + 색상 보간
            text.transform.localScale = Vector3.one * Mathf.Lerp(originScale, targetScale, t);
            text.color = Vector4.Lerp(startColor, targetColor, t);

            time += Time.deltaTime;
            yield return null;
        }

        text.transform.localScale = Vector3.one * targetScale;
        text.color = targetColor;
    }

    IEnumerator BounceMotion(TextMeshProUGUI text, float duration, Color targetColor = default)
    {
        if (targetColor == default) targetColor = Color.white;

        float time = 0;
        Color originColor = text.color;

        while (time < duration)
        {
            float t = time / duration;

            // sin 기반 바운스 효과
            float pingPong = Mathf.Sin(t * Mathf.PI);

            text.transform.localScale = Vector3.one * Mathf.Lerp(1f, targetScale, pingPong);
            text.color = Vector4.Lerp(originColor, targetColor, pingPong);

            time += Time.deltaTime;
            yield return null;
        }

        // 원상 복구
        text.transform.localScale = Vector3.one;
        text.color = originColor;
    }

    /// <summary>
    /// 텍스트 바운스 연출
    /// </summary>
    public void ScoreBounce(TextMeshProUGUI text, float duration, Color targetColor = default)
    {
        StartCoroutine(BounceMotion(text, duration, targetColor));
    }

    /// <summary>
    /// 정수 텍스트 출력
    /// </summary>
    public void ToText(TextMeshProUGUI text, int i)
    {
        text.SetText("{0}", i);
    }

    /// <summary>
    /// 실수 텍스트 출력
    /// </summary>
    public void ToText(TextMeshProUGUI text, float f)
    {
        text.SetText("{0:+0.00}", f);
    }

    /// <summary>
    /// 게임오버 UI 비활성화
    /// </summary>
    public void OffGameOverUIGroup()
    {
        GameOverUIGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// 점수 갱신 연출
    /// </summary>
    public void UpdateBounce()
    {
        ScoreScale(scoreText, Color.white, Color.white);
        ScoreScale(bestScoreText, Color.yellow, Color.grey);
    }

    /// <summary>
    /// 피버/이벤트 연출
    /// </summary>
    public void Fit()
    {
        ScoreBounce(scoreText, bounceDuration);
        ScoreBounce(feverText, bounceDuration);
        ScoreBounce(scoreAmountText, bounceDuration, Color.green);
    }

    /// <summary>
    /// 인터페이스 대응 
    /// </summary>
    public void Unfit() { return; }
}