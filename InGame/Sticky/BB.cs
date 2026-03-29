using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 특정 스티키의 연출과 상태 반응을 담당하는 클래스
/// </summary>
public class BB : Sticky
{
    [SerializeField] private float wobbleStrength = 0.05f;  // 흔들림 강도
    [SerializeField] private float wobbleSpeed = 6f;        // 흔들림 속도
    [SerializeField] private Vector3 originPos;             // 초기 위치
    [SerializeField] private Vector3 originScale;           // 초기 스케일

    Animator anim; // 애니메이터
    Coroutine wobbleRoutine; // 흔들림 코루틴

    void Start()
    {
        // 등장 연출 시작
        Intro();

        // 이벤트 구독
        base.Subscribe();

        // 애니메이터 캐싱
        anim = GetComponent<Animator>();

        // 초기 값 저장
        originPos = transform.position;
        originScale = Vector3.one;    
    }

    /// <summary>
    /// 게임 상태에 따라 연출을 변경하는 메서드
    /// </summary>
    public override void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Intro();
                break;

            case GameState.GameOver:
                Outro();
                break; 
        }
    }

    // 등장 연출 시작
    void Intro()
    {
        gameObject.SetActive(true);

        StartCoroutine(IntroRoutine());

        // 흔들림 시작
        wobbleRoutine = StartCoroutine(WobbleMotion());
    }

    // 퇴장 연출 시작
    void Outro()
    {
        StartCoroutine(OutroRoutine());

        // 흔들림 종료
        StopCoroutine(wobbleRoutine);
    }

    /// <summary>
    /// 등장 애니메이션을 수행하는 메서드
    /// </summary>
    IEnumerator IntroRoutine()
    {
        yield return StartCoroutine(ScaleMotion(0f, 1.2f, 0.2f));
        yield return StartCoroutine(TiltMotion(0.2f, 10f));
        yield return StartCoroutine(ScaleMotion(1.2f, 1f, 0.1f));
    }

    /// <summary>
    /// 퇴장 애니메이션을 수행하는 메서드
    /// </summary>
    IEnumerator OutroRoutine()
    {
        yield return StartCoroutine(TiltMotion(0.2f, 10f));
        yield return StartCoroutine(ScaleMotion(1.2f, 0f, 0.1f));

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 스케일 변화를 적용하는 메서드
    /// </summary>
    IEnumerator ScaleMotion(float start, float end, float scaleDuration)
    {
        float time = 0;

        while(time < scaleDuration)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(start, end, time / scaleDuration);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one * end;
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
    /// 위치와 스케일에 흔들림을 적용하는 메서드
    /// </summary>
    IEnumerator WobbleMotion()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            yield return null;

            // Y축 위치 흔들림
            float offsetY = Mathf.Abs(Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength);

            // 스케일 변화
            float scaleOffset = 0.01f * Mathf.Abs(Mathf.Sin(Time.time * wobbleSpeed));

            transform.position = new Vector3(originPos.x, originPos.y + offsetY, originPos.z);

            transform.localScale = new Vector3(
                originScale.x + (scaleOffset * 2),
                originScale.y - scaleOffset,
                originScale.z
            );
        }
    }

    /// <summary>
    /// 성공 시 애니메이션을 실행하는 메서드
    /// </summary>
    public override void Fit()
    {
        anim.SetTrigger("Fit");
    }

    /// <summary>
    /// 실패 시 애니메이션을 실행하는 메서드
    /// </summary>
    public override void Unfit()
    {
        anim.SetTrigger("Unfit");
    }
}