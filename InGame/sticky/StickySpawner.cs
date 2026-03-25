using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 스티키 오브젝트를 풀링으로 생성하고 랜덤 위치에 배치하는 클래스
/// </summary>
public class StickySpawner : MonoBehaviour
{
    public static StickySpawner Instance; // 싱글톤 인스턴스

    public Queue<GameObject> spawnedStickyPool = new Queue<GameObject>();       // 스티키 오브젝트 풀

    [SerializeField] private GameObject spawnedStickyPrefab;                    // 스티키 프리팹
    [SerializeField] private List<Material> stickyList = new List<Material>();  // 스티키 머티리얼 리스트

    [SerializeField] private RandomPosGenerator randomSpawnPos;                 // 랜덤 위치 생성기

    void Awake()
    {
        Instance = this;

        // 미리 오브젝트 생성하여 풀에 저장
        for (int i = 0; i < 100; ++i)
        {
            GameObject sticky = Instantiate(spawnedStickyPrefab, transform);

            spawnedStickyPool.Enqueue(sticky);

            // 초기에는 비활성화
            sticky.SetActive(false);
        }
    }

    /// <summary>
    /// 스티키를 생성하고 랜덤 위치에 배치하는 메서드
    /// </summary>
    public void SpawnSticky()
    {
        int randIdx = Random.Range(0, 10);

        Material selectedMat = null;
        float bonus = 0f;

        // 확률 기반 머티리얼 선택
        if (randIdx <= 1)
        {
            // 희귀 스티키
            selectedMat = stickyList[4];
            bonus = 0.25f;
        }
        else
        {
            // 일반 스티키
            selectedMat = stickyList[Random.Range(0, 4)];
            bonus = 0.1f;
        }

        if (spawnedStickyPool.Count != 0)
        {
            // 풀에서 오브젝트 하나 가져오기
            GameObject prefab = spawnedStickyPool.Dequeue();

            SpawnedSticky sticky = prefab.GetComponent<SpawnedSticky>();

            // 머티리얼과 보너스 초기화
            sticky.Init(selectedMat, bonus);

            // 랜덤 위치 생성
            Vector3 randPos = randomSpawnPos.GetRandomPos(prefab);

            // 위치 설정 및 활성화
            sticky.Stick(randPos);
        }
    }
}