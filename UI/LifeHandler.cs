using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 하트를 관리하는 클래스
/// </summary>
public class LifeHandler : MonoBehaviour
{
    // 하트 UI Transform 리스트
    public List<Transform> lifeList = new List<Transform>();  

    public void Awake()
    {
        // 현재 오브젝트의 모든 자식들을 순회
        foreach(Transform child in transform)
        {
            // 자식들을 하트 리스트에 저장
            lifeList.Add(child);
        }
    }

    /// <summary>
    /// 모든 하트 UI를 활성화하여 초기 상태로 되돌리는 메서드
    /// </summary>
    public void InitLife()
    {
        for (int i = 0; i < lifeList.Count; ++i)
        {
            // 각 하트 오브젝트를 활성화
            lifeList[i].gameObject.SetActive(true);
        }
    }
}