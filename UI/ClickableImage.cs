using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// 이미지 UI에 포인터 이벤트를 전달하는 클래스
/// </summary>
public class ClickableImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public delegate void OnPointerEvent(CanvasGroup cg); // CanvasGroup을 전달하는 이벤트 델리게이트

    public event OnPointerEvent OnEnter; // 마우스 진입 이벤트
    public event OnPointerEvent OnExit;  // 마우스 이탈 이벤트
    public event OnPointerEvent OnUp;    // 버튼 해제 이벤트
    public event OnPointerEvent OnDown;  // 버튼 누름 이벤트
    public event OnPointerEvent OnClick; // 클릭 이벤트

    public delegate void OnHoldEvent(bool isHold); // 홀드 상태 이벤트 델리게이트

    private CanvasGroup cg; // 대상 CanvasGroup

    void Awake()
    {
        // CanvasGroup 컴포넌트 자동 할당
        cg = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 마우스 진입 시 이벤트를 호출하는 메서드
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke(cg);
    }

    /// <summary>
    /// 마우스 이탈 시 이벤트를 호출하는 메서드
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke(cg);
    }

    /// <summary>
    /// 버튼 누름 시 이벤트를 호출하는 메서드
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDown?.Invoke(cg);
    }

    /// <summary>
    /// 버튼 해제 및 클릭 이벤트를 호출하는 메서드
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        OnUp?.Invoke(cg);

        // 클릭으로 간주하여 함께 호출
        OnClick?.Invoke(cg);
    }
}