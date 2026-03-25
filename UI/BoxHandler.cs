using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 자식 박스 오브젝트에 흔들림을 적용하는 클래스
/// </summary>
public class BoxHandler : MonoBehaviour
{
    List<Transform> boxChilds = new List<Transform>(); // 자식 박스 Transform 리스트

    [SerializeField] private float wobblingSpeed = 1f;      // 흔들림 속도
    [SerializeField] private float wobblingStrength = 1f;   // 흔들림 강도

    void Awake()
    {
        // 자식 오브젝트들을 리스트에 저장
        foreach(Transform child in transform)
        {
            boxChilds.Add(child);
        }
    }

    void Update()
    {
        // 매 프레임 흔들림 적용
        Wobbling();
    }

    /// <summary>
    /// 자식 오브젝트 위치에 sin 기반 흔들림을 적용하는 메서드
    /// </summary>
    void Wobbling()
    {
        for (int i = 0; i < boxChilds.Count; ++i)
        {
            // Y축 방향 흔들림 값 계산
            Vector3 offset = new Vector3(
                0,
                Mathf.Sin(Time.time * wobblingSpeed + i) * wobblingStrength,
                0
            );

            // 현재 위치에 오프셋을 더해 흔들림 적용
            boxChilds[i].localPosition += offset;
        }
    }
}