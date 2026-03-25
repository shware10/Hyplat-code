using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Net 오브젝트들을 관리하고 이벤트를 연결하는 클래스
/// </summary>
public class NetHandler : MonoBehaviour
{
    public List<Net> netList = new List<Net>(); // Net 리스트
    private RectangleHandler rectangleHandler;  // 바 관리 객체

    void Awake()
    {
        // 자식 오브젝트에서 Net 컴포넌트 수집
        foreach (Transform netTransform in transform)
        {
            Net net = netTransform.GetComponent<Net>();
            netList.Add(net);
        }

        // RectangleHandler 가져오기
        rectangleHandler = GameObject.FindGameObjectWithTag("RectangleHandler")
            .GetComponent<RectangleHandler>();
    }

    /// <summary>
    /// Net 이벤트를 게임 로직에 연결하는 메서드
    /// </summary>
    void Start()
    {
        for(int i = 0; i < netList.Count; ++i)
        {
            // Net 충돌 시 하트 감소
            netList[i].OnNet += GameManager.Instance.LoseLife;

            // Net 충돌 시 바 초기화
            netList[i].OnNet += rectangleHandler.InitBarLevel;
        }
    }
}