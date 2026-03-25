using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 스티키 오브젝트의 공통 동작과 이벤트 구독을 관리하는 클래스
/// </summary>
public class Sticky : MonoBehaviour, IBarStateListener, IGameStateListener
{
    public SpriteRenderer rdr;      // 스프라이트 렌더러
    public StickySO stickyData;     // 스티키 데이터
    protected float bonusScore;     // 보너스 점수 값
    protected Transform poolPos;    // 풀 위치

    List<Bar> barList; // 바 리스트
    List<Net> netList; // 네트 리스트

    protected virtual void Awake()
    {
        // RectangleHandler에서 바 리스트 가져오기
        barList = GameObject.FindGameObjectWithTag("RectangleHandler")
                 .GetComponent<RectangleHandler>().barList;

        // NetHandler에서 네트 리스트 가져오기
        netList = GameObject.FindGameObjectWithTag("NetHandler")
                 .GetComponent<NetHandler>().netList;

        // 풀 위치 가져오기
        poolPos = GameObject.FindGameObjectWithTag("StickyPool").GetComponent<Transform>();
        
        // SpriteRenderer 캐싱
        rdr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 스티키의 머티리얼과 보너스 값을 초기화하는 메서드
    /// </summary>
    public virtual void Init(Material mat, float amount)
    {
        rdr.material = mat;
        bonusScore = amount;
    }

    /// <summary>
    /// 게임 상태 및 바와 네트 이벤트를 구독하는 메서드
    /// </summary>
    protected void Subscribe()
    {
        // 게임 상태 이벤트 구독
        GameManager.Instance.OnStateChanged += OnStateChanged;

        for (int i = 0; i < barList.Count; ++i)
        {
            // 바 이벤트 구독
            barList[i].FitColor += Fit;
            barList[i].UnfitColor += Unfit;

            // 네트 이벤트 구독
            netList[i].OnNet += Unfit;
        }
    }

    /// <summary>
    /// 게임 상태 및 바와 네트 이벤트 구독을 해제하는 메서드
    /// </summary>
    protected void Unsubscribe()
    {
        // 게임 상태 이벤트 해제
        GameManager.Instance.OnStateChanged -= OnStateChanged;

        for (int i = 0; i < barList.Count; ++i)
        {
            // 바 이벤트 해제
            barList[i].FitColor -= Fit;
            barList[i].UnfitColor -= Unfit;

            // 네트 이벤트 해제
            netList[i].OnNet -= Unfit;
        }
    } 

    /// <summary>
    /// 성공 판정 시 호출되는 메서드
    /// </summary>
    public virtual void Fit()
    {
    }

    /// <summary>
    /// 실패 판정 시 호출되는 메서드
    /// </summary>
    public virtual void Unfit()
    {
    }

    /// <summary>
    /// 게임 상태 변경 시 호출되는 메서드
    /// </summary>
    public virtual void OnStateChanged(GameState state) { }
}