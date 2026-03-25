using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 자식 오브젝트들을 흔들리는 애니메이션으로 움직이게 하는 클래스
/// </summary>
public class ChildsWobbler : MonoBehaviour
{
    List<Transform> childs = new List<Transform>(); // 자식 Transform 리스트
    List<Vector3> originPos = new List<Vector3>();  // 자식들의 초기 위치

    [SerializeField] private float wobblingSpeed = 10f;     // 흔들림 속도
    [SerializeField] private float wobblingStrength = 2f;   // 흔들림 강도

    void Awake()
    {
        foreach (Transform child in transform)
        {
            // 자식 Transform 저장
            childs.Add(child);

            // 초기 위치 저장
            originPos.Add(child.localPosition);
        }
    }

    void Update()
    {
        // 매 프레임 흔들림 적용
        Wobbling();
    }

    /// <summary>
    /// 자식 오브젝트에 sin 기반 흔들림을 적용하는 메서드
    /// </summary>
    void Wobbling()
    {
        for (int i = 0; i < childs.Count; ++i)
        {
            // 시간 기반 sin 파형으로 Y축 이동 값 계산
            Vector3 offset = new Vector3(
                0,
                Mathf.Sin(Time.time * wobblingSpeed + i) * wobblingStrength,
                0
            );

            // 초기 위치에 오프셋을 더해 위치 갱신
            childs[i].localPosition = originPos[i] + offset;
        }
    }
}