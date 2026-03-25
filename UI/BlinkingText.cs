using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;

/// <summary>
/// 텍스트를 일정 간격으로 깜빡이게 하는 클래스
/// </summary>
public class BlinkingText : MonoBehaviour
{
    TextMeshProUGUI text;   // 대상 텍스트
    float interval = 0.5f;  // 깜빡임 간격
    WaitForSeconds wts;     // 재사용 대기 객체
    Vector4 color;          // 텍스트 색상 값

    void Awake()
    {
        // TextMeshPro 컴포넌트 캐싱
        text = GetComponent<TextMeshProUGUI>();

        // 대기 시간 캐싱
        wts = new WaitForSeconds(interval);

        // 초기 색상 저장
        color = text.color;

        // 시작 시 비활성화
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 텍스트를 반복적으로 깜빡이게 하는 메서드
    /// </summary>
    IEnumerator Start()
    {
        while(true)
        {
            // alpha를 0으로 설정하여 숨김
            color.w = 0;
            text.color = color;

            yield return wts;

            // alpha를 1로 설정하여 표시
            color.w = 1;
            text.color = color;

            yield return wts;
        }
    }

    /// <summary>
    /// 텍스트를 활성화하는 메서드
    /// </summary>
    public void TurnOn()
    {
        gameObject.SetActive(true);
    }
}