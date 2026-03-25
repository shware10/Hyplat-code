using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 버튼 텍스트와 이미지의 이벤트 및 연출을 관리하는 클래스
/// </summary>
public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance; // 싱글톤 인스턴스

    [Header("Clikable Text")]
    [Header("MainMenu")]
    public ClickableText startButton;   // 시작 버튼
    public ClickableText exitButton;    // 종료 버튼

    [Header("InGame Text")]
    public ClickableText FeverBonusText; // 피버 텍스트

    [Header("GameOver")]
    public ClickableText freePresentButton; // 보상 버튼
    public ClickableText yesButton;         // 예 버튼
    public ClickableText noButton;          // 아니오 버튼

    [Header("회전 흔들림")]
    public float rotWobbleStrength = 5f;    // 회전 흔들림 강도
    public float rotWobbleSpeed = 5f;       // 회전 흔들림 속도

    [Header("상하 흔들림")]
    public float tdWobbleStrength = 2f;     // 상하 흔들림 강도
    public float tdWobbleSpeed = 10f;       // 상하 흔들림 속도

    [Header("깜빡임")]
    [SerializeField] private float blinkDuration = 0.25f; // 깜빡임 시간

    void Awake()
    {
        Instance = this;

        // 메인 메뉴 버튼 이벤트 연결
        startButton.OnEnter += DarkenText;
        startButton.OnExit += LightenText;
        startButton.OnClick += BlinkText;
        startButton.OnIdle += WobbleTextR;

        exitButton.OnEnter += DarkenText;
        exitButton.OnExit += LightenText;
        exitButton.OnIdle += WobbleTextTD;

        // 인게임 텍스트
        FeverBonusText.OnIdle += WobbleTextTD;

        // 게임오버 버튼
        yesButton.OnEnter += DarkenText;
        yesButton.OnExit += LightenText;
        yesButton.OnIdle += WobbleTextR;

        noButton.OnEnter += DarkenText;
        noButton.OnExit += LightenText;
        noButton.OnIdle += WobbleTextR;

        freePresentButton.OnIdle += WobbleTextTD;
    }

    // 텍스트 밝기 복구
    void LightenText(TextMeshProUGUI text)
    {
        Color color = text.color;
        color.a = 1f;
        text.color = color;
    }

    // 텍스트 어둡게
    void DarkenText(TextMeshProUGUI text)
    {
        Color color = text.color;
        color.a = 0.7f;
        text.color = color;
    }

    // 텍스트 깜빡임 시작
    void BlinkText(TextMeshProUGUI text)
    {
        StartCoroutine(Blink(text));
    }

    // 텍스트 깜빡임 애니메이션
    IEnumerator Blink(TextMeshProUGUI text)
    {
        CanvasGroup canvasGroup = text.GetComponent<CanvasGroup>();
        float time = 0f;

        while (time < blinkDuration)
        {
            float radian = Mathf.Lerp(0, Mathf.PI * 2, time / blinkDuration);

            // sin 기반 alpha 변화
            float pingpong = 1 - Mathf.Abs(Mathf.Sin(radian));
            canvasGroup.alpha = pingpong;

            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    // 회전 흔들림 시작
    void WobbleTextR(TextMeshProUGUI text)
    {
        StartCoroutine(RotateWobbling(text));
    }

    // 상하 흔들림 시작
    void WobbleTextTD(TextMeshProUGUI text)
    {
        StartCoroutine(TopDownWobbling(text));
    }

    /// <summary>
    /// 텍스트를 원형으로 흔들리게 하는 메서드
    /// </summary>
    IEnumerator RotateWobbling(TextMeshProUGUI text)
    {
        while (true)
        {
            text.ForceMeshUpdate(); // 최신 메쉬 정보 갱신

            TMP_TextInfo textInfo = text.textInfo;

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                int materialIndex = charInfo.materialReferenceIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // 원형 흔들림 오프셋
                Vector3 offset = new Vector3(
                    Mathf.Sin(Time.time * rotWobbleSpeed + i) * rotWobbleStrength,
                    Mathf.Cos(Time.time * rotWobbleSpeed + i) * rotWobbleStrength,
                    0
                );

                // 문자 4개 버텍스 이동
                for (int j = 0; j < 4; ++j)
                {
                    vertices[vertexIndex + j] += (j % 2 == 0) ? offset : offset / 2;
                }
            }

            // 메쉬 적용
            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                text.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }

    /// <summary>
    /// 텍스트를 위아래로 흔들리게 하는 메서드
    /// </summary>
    IEnumerator TopDownWobbling(TextMeshProUGUI text)
    {
        while (true)
        {
            text.ForceMeshUpdate();

            TMP_TextInfo textInfo = text.textInfo;

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                int materialIndex = charInfo.materialReferenceIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // Y축 흔들림 오프셋
                Vector3 offset = new Vector3(
                    0,
                    Mathf.Sin(Time.time * tdWobbleSpeed + i) * tdWobbleStrength,
                    0
                );

                for (int j = 0; j < 4; ++j)
                {
                    vertices[vertexIndex + j] += offset;
                }
            }

            // 메쉬 적용
            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                text.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }

    // 이미지 어둡게
    void DarkenImage(CanvasGroup cg)
    {
        cg.alpha = 0.7f;
    }

    // 이미지 밝게
    void LightenImage(CanvasGroup cg)
    {
        cg.alpha = 1f;
    }

    // 이미지 깜빡임 시작
    void BlinkImage(CanvasGroup cg)
    {
        StartCoroutine(Blink(cg));
    }

    // 이미지 깜빡임 애니메이션
    IEnumerator Blink(CanvasGroup cg)
    {
        float time = 0f;

        while (time < blinkDuration)
        {
            float radian = Mathf.Lerp(0, Mathf.PI * 2, time / blinkDuration);
            //sin 기반 핑퐁값
            float pingpong = 1 - Mathf.Abs(Mathf.Sin(radian));
            cg.alpha = pingpong;

            time += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 1f;
    }
}