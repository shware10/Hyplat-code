using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 바 오브젝트의 충돌 판정과 레벨 상태를 관리하는 클래스
/// </summary>
public class Bar : MonoBehaviour
{
    private SpriteRenderer rdr;  // 렌더러
    private Color color;         // 바 색상

    [SerializeField] private int _level = 1; // 내부 레벨 값
    public int level
    {
        get { return _level; }
        set { _level = Mathf.Clamp(value, 1, 3); } // 레벨 범위 제한
    }

    public delegate void OnTrigger();   // 트리거 이벤트 델리게이트
    public event OnTrigger FitColor;    // 성공 이벤트
    public event OnTrigger UnfitColor;  // 실패 이벤트

    void Awake()
    {
        transform.localScale = Vector3.zero; // 초기 크기 숨김

        // 기본 이벤트 연결
        FitColor += AddBarLevel;
        UnfitColor += InitBarLevel;

        // 초기 충돌 비활성화
        GetComponent<Collider>().enabled = false;

        rdr = GetComponent<SpriteRenderer>();

        setGlowAmount(); // 초기 글로우 설정
    }

    /// <summary>
    /// 충돌 시 색상에 따라 성공 또는 실패를 처리하는 메서드
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        Jet jet = other.GetComponent<Jet>(); // Jet 가져오기

        if (jet.color == color)
        {
            FitColor?.Invoke(); // 성공 이벤트 호출

            jet.Fit(); // Jet 성공 처리

            AudioManager.Instance.PlaySFX(0); // 성공 사운드

            DebugX.Log($"성공 level: {level}");
        }
        else
        {
            UnfitColor?.Invoke(); // 실패 이벤트 호출

            jet.Unfit(); // Jet 실패 처리

            AudioManager.Instance.PlaySFX(1); // 실패 사운드

            DebugX.Log($"실패 level: {level}");
        }
    }

    /// <summary>
    /// 바 레벨을 증가시키는 메서드
    /// </summary>
    public void AddBarLevel()
    {
        ++level; // 레벨 증가

        setGlowAmount(); // 글로우 갱신
    }

    /// <summary>
    /// 바 레벨을 초기화하는 메서드
    /// </summary>
    public void InitBarLevel()
    {
        level = 1; // 레벨 초기화

        setGlowAmount(); // 글로우 갱신
    }

    /// <summary>
    /// 레벨에 따라 글로우 값을 설정하는 메서드
    /// </summary>
    public void setGlowAmount()
    {
        rdr.material.SetFloat("_GlowAmount", level); // 쉐이더 값 적용
    }
}