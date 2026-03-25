using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 충돌 시 이벤트를 발생시키고 제트와 상호작용하는 클래스
/// </summary>
public class Net : MonoBehaviour
{
    public delegate void OnTrigger();   // 트리거 이벤트 델리게이트
    public event OnTrigger OnNet;       // Net 충돌 이벤트

    /// <summary>
    /// 충돌 시 제트 처리와 이벤트를 실행하는 메서드
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트에서 Jet 컴포넌트 가져오기
        Jet jet = other.GetComponent<Jet>();

        // Jet 처리 실행
        jet.Net();

        // 효과음 재생
        AudioManager.Instance.PlaySFX(1);

        // 이벤트 호출
        OnNet?.Invoke();
    }
}