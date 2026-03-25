using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Jet을 풀에서 선택하여 스폰하고 게임 상태에 따라 생성 타이밍을 제어하는 클래스
/// </summary>
public class JetSpawner : MonoBehaviour, IGameStateListener
{
    [SerializeField] private JetPool JetPool; // Jet 오브젝트 풀

    private bool isInGame = false; // 인게임 상태 여부
    private int mxIndex;           // 풀의 최대 인덱스

    void Awake()
    {
        // 방향과 색상 조합으로 최대 개수 계산
        mxIndex = JetPool.jetDir.Length * JetPool.jetColors.Length;
    }

    /// <summary>
    /// 풀에서 Jet을 선택하여 스폰하는 메서드
    /// </summary>
    public void SpawnJet()
    {
        int randIndex = Random.Range(0, mxIndex);

        // 이미 사용 중인 Jet이면 다시 선택
        while (JetPool.Pool[randIndex].curState == JetState.isSpawn)
        {
            randIndex = Random.Range(0, mxIndex);
        }

        // 선택된 Jet을 스폰 위치로 이동
        JetPool.Pool[randIndex].Go2SpawnPos();
    }

    /// <summary>
    /// 게임 상태에 따라 Jet 생성 루틴을 시작하는 메서드
    /// </summary>
    public void OnStateChanged(GameState state)
    {
        isInGame = state == GameState.InGame;

        if (isInGame)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    /// <summary>
    /// 일정 시간 후 Jet을 생성하는 메서드
    /// </summary>
    IEnumerator SpawnRoutine()
    {
        // 시작 딜레이
        yield return new WaitForSeconds(2f);

        SpawnJet();
    }
}