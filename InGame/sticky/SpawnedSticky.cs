using System.Collections;
using UnityEngine;

/// <summary>
/// 생성된 스티키의 연출과 동작을 담당하는 클래스
/// </summary>
public class SpawnedSticky : Sticky
{
    [SerializeField] private float stickDuration = 0.2f;    // 스케일 연출 시간
    [SerializeField] private float rotateDuration = 0.3f;   // 회전 연출 시간
    [SerializeField] private float targetScale = 1.2f;      // 목표 스케일

    private LineRenderer LineRdr; // 라인 렌더러

    /// <summary>
    /// 게임 상태 변경 시 스티키를 정리하는 메서드
    /// </summary>
    public override void OnStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            if (gameObject.activeSelf)
            {
                // 연출 후 풀로 반환
                Unstick();
                base.Unsubscribe();
            }
        }
    }

    /// <summary>
    /// 스티키 초기 설정을 수행하는 메서드
    /// </summary>
    public override void Init(Material mat, float amount)
    {
        base.Init(mat, amount);

        // 자식 LineRenderer 가져오기
        LineRdr = transform.GetChild(0).GetComponent<LineRenderer>();

        // 머티리얼 적용
        LineRdr.material = mat;

        // 초기 alpha를 0으로 설정
        Color color = LineRdr.startColor;
        color.a = 0f;
        LineRdr.startColor = color;
        LineRdr.endColor = color;

        // 점수 배율 증가
        GameManager.Instance.AddScoreAmount(bonusScore);
    }

    /// <summary>
    /// 스티키를 활성화하고 위치를 설정하는 메서드
    /// </summary>
    public void Stick(Vector3 stickPos)
    {
        gameObject.SetActive(true);
        transform.position = stickPos;

        // 등장 연출 시작
        StartCoroutine(StickRoutine());

        // 이벤트 구독
        base.Subscribe();
    }

    /// <summary>
    /// 스티키를 제거하는 메서드
    /// </summary>
    public void Unstick()
    {
        FadeOut();
    }

    /// <summary>
    /// 사라지는 연출을 시작하는 메서드
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine(FadeRoutine());
    }

    /// <summary>
    /// 사라지는 전체 연출을 수행하는 메서드
    /// </summary>
    IEnumerator FadeRoutine()
    {
        // 라인 페이드 아웃
        yield return StartCoroutine(FadeMotion(0.2f, 1f, 0f));

        // 확대
        yield return StartCoroutine(ScaleMotion(0.1f, 1f, targetScale));

        // 흔들림
        yield return StartCoroutine(TiltMotion(0.2f, 5));

        // 축소
        yield return StartCoroutine(ScaleMotion(0.1f, targetScale, 0f));

        // 풀로 반환
        StickySpawner.Instance.spawnedStickyPool.Enqueue(gameObject);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 등장 연출을 수행하는 메서드
    /// </summary>
    IEnumerator StickRoutine()
    {
        yield return StartCoroutine(ScaleMotion(0.1f, 0, targetScale));
        yield return StartCoroutine(TiltMotion(0.2f, 5));
        yield return StartCoroutine(ScaleMotion(0.1f, targetScale, 1f));

        // 라인 페이드 인
        StartCoroutine(FadeMotion(0.5f, 0f, 1f));
    }

    /// <summary>
    /// 라인 alpha를 변화시키는 메서드
    /// </summary>
    IEnumerator FadeMotion(float fadeDuration, float start, float end)
    {
        float time = 0;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(start, end, time / fadeDuration);

            SetLineAlpha(alpha);

            time += Time.deltaTime;
            yield return null;
        }

        SetLineAlpha(end);
    }

    // 라인 렌더러 alpha 설정
    void SetLineAlpha(float alpha)
    {
        Gradient grad = LineRdr.colorGradient;
        GradientAlphaKey[] alphaKeys = grad.alphaKeys;

        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha = alpha;
        }

        grad.alphaKeys = alphaKeys;
        LineRdr.colorGradient = grad;
    }

    /// <summary>
    /// 스케일을 변화시키는 메서드
    /// </summary>
    IEnumerator ScaleMotion(float scaleDuration, float startScale, float endScale)
    {
        float time = 0;

        while (time < stickDuration)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, time / stickDuration);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one * endScale;
    }

    /// <summary>
    /// 회전 흔들림을 적용하는 메서드
    /// </summary>
    IEnumerator TiltMotion(float tiltDuration, float tiltAmount)
    {
        float time = 0;
        Vector3 originAngles = transform.localEulerAngles;

        while (time < tiltDuration)
        {
            float radian = Mathf.Lerp(0, 2 * Mathf.PI, time / tiltDuration);

            transform.localEulerAngles = new Vector3(
                originAngles.x,
                originAngles.y,
                originAngles.z + tiltAmount * Mathf.Sin(radian)
            );

            time += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = originAngles;
    }

    /// <summary>
    /// 180도 회전 연출을 수행하는 메서드
    /// </summary>
    IEnumerator RotateMotion()
    {
        float time = 0;

        while(time < rotateDuration)
        {
            float angle = Mathf.Lerp(0, 180, time / rotateDuration);

            transform.rotation = Quaternion.Euler(0, angle, 0);

            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 스케일 기반 연출을 수행하는 메서드
    /// </summary>
    IEnumerator ScaleRoutine()
    {
        yield return StartCoroutine(ScaleMotion(0.1f, 1f, targetScale));
        yield return StartCoroutine(TiltMotion(0.1f, 5));
        yield return StartCoroutine(ScaleMotion(0.1f, targetScale, 1f));
    }

    /// <summary>
    /// 성공 시 랜덤 연출을 수행하는 메서드
    /// </summary>
    public override void Fit()
    {
        int randInt = Random.Range(0, 2);

        switch (randInt)
        {
            case 0:
                StartCoroutine(RotateMotion());
                break;

            case 1:
                StartCoroutine(ScaleRoutine());
                break;
        }
    }

    /// <summary>
    /// 실패 시 호출되는 메서드
    /// </summary>
    public override void Unfit()
    {
        return;
    }
}