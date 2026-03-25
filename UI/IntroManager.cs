using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// 인트로 연출과 씬 전환을 관리하는 클래스
/// </summary>
public class IntroManager : MonoBehaviour
{
    [SerializeField] private Material CRTShader; // CRT 효과를 적용할 머티리얼
    private float originValue; // 초기 노이즈 값
    [SerializeField] private float targetValue = 0.5f;    // 목표 노이즈 값
    [SerializeField] private float introDuration = 1f;    // 인트로 전체 연출 시간
    [SerializeField] private float noiseDuration = 0.5f;  // 노이즈 유지 시간

    public BlinkingText btext;  // 깜빡이는 텍스트
    public Transform title;     // 타이틀 Transform

    private bool isReady = false; // 입력 가능 상태 여부

    void Awake()
    {
        // 초기 노이즈 값 저장
        originValue = CRTShader.GetFloat("_Blur_Offset");

        // 인트로 연출 시작
        StartCoroutine(Intro());
    }

    /// <summary>
    /// 인트로 연출을 진행하는 메서드
    /// </summary>
    IEnumerator Intro()
    {
        float time = 0;

        // CRT 효과 활성화
        RenderFeatureToggler.Instance.ToggleFeature(true);

        float expandDuration = introDuration * 0.6f;
        float contractDuration = introDuration * 0.4f;

        while (time < expandDuration)
        {
            float percent = time / expandDuration;

            // 타이틀 확대
            title.localScale = Vector3.one * Mathf.Lerp(1f, 1.2f, percent);

            // 노이즈 증가
            float value = Mathf.Lerp(originValue, targetValue, percent);
            CRTShader.SetFloat("_Blur_Offset", value);

            time += Time.deltaTime;
            yield return null;
        }

        // 확대 상태 유지
        title.localScale = Vector3.one * 1.2f;
        CRTShader.SetFloat("_Blur_Offset", targetValue);

        time = 0;

        while (time < contractDuration)
        {
            float percent = time / contractDuration;

            // 타이틀 축소
            title.localScale = Vector3.one * Mathf.Lerp(1.2f, 1f, percent);

            // 노이즈 감소
            float value = Mathf.Lerp(targetValue, originValue, percent);
            CRTShader.SetFloat("_Blur_Offset", value);

            time += Time.deltaTime;
            yield return null;
        }

        // 원래 상태 복구
        title.localScale = Vector3.one;
        CRTShader.SetFloat("_Blur_Offset", originValue);

        // 노이즈 유지
        yield return new WaitForSeconds(noiseDuration);

        // CRT 효과 비활성화
        RenderFeatureToggler.Instance.ToggleFeature(false);

        // 텍스트 표시
        btext.TurnOn();

        // 입력 허용
        isReady = true;
    }

    /// <summary>
    /// 입력을 감지하여 다음 씬으로 이동하는 메서드
    /// </summary>
    void Update()
    {
        if ((isReady && Input.touchCount > 0) || Input.GetKeyDown(KeyCode.Space))
        {
            // 씬 전환
            SceneManager.LoadScene(1);
        }
    }
}