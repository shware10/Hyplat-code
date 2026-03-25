using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Globalization;

/// <summary>
/// 텍스트 UI에 마우스 이벤트를 전달하는 클래스
/// </summary>
public class ClickableText : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TextMeshProUGUI textMesh; // 연결된 텍스트 컴포넌트

    // 포인터 이벤트 델리게이트
    public delegate void OnPointerEvent(TextMeshProUGUI clickedText);

    public event OnPointerEvent OnEnter; // 마우스 진입 이벤트
    public event OnPointerEvent OnExit;  // 마우스 이탈 이벤트
    public event OnPointerEvent OnClick; // 클릭 이벤트
    public event OnPointerEvent OnIdle;  // 초기 상태 이벤트

    void Awake()
    {
        // TextMeshPro 컴포넌트 자동 할당
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        // 활성화 시 초기 상태 이벤트 호출
        OnIdle?.Invoke(textMesh);
    }

    /// <summary>
    /// 클릭 시 이벤트를 호출하는 메서드
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(textMesh);
    }

    /// <summary>
    /// 마우스 진입 시 이벤트를 호출하는 메서드
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke(textMesh);
    }

    /// <summary>
    /// 마우스 이탈 시 이벤트를 호출하는 메서드
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke(textMesh);
    }
}