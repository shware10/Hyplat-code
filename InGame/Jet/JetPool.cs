using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jet 오브젝트를 미리 생성하여 풀로 관리하는 클래스
/// </summary>
public class JetPool : MonoBehaviour
{
    private List<Jet> Pool = new List<Jet>(); // Jet 객체 풀

    private Vector3[] jetDir = { Vector3.up, Vector3.down, Vector3.left, Vector3.right }; // 이동 방향
    private Color[] jetColors = { Color.red, Color.green, Color.blue, Color.white };      // 색상 종류
    private int[] jetAngle = { 0, 180, 90, 270 }; // 회전 각도

    [SerializeField] private Transform rectanglePos; // 스폰 위치 기준
    [SerializeField] private GameObject[] jetPrefabs = new GameObject[4]; // 색상별 프리팹

    void Awake()
    {
        // 색상과 방향 조합으로 Jet을 미리 생성
        for (int i = 0; i < jetColors.Length;  ++i)
        {
            for (int j = 0; j < jetDir.Length; ++j)
            {
                // 프리팹 생성
                GameObject obj = Instantiate(jetPrefabs[i], transform);

                Jet jet = obj.GetComponent<Jet>();

                // 색상 설정
                jet.color = jetColors[i];

                // 방향과 각도 설정
                jet.dir = jetDir[j];
                jet.angle = jetAngle[j];

                // 풀 위치와 스폰 위치 설정
                jet.poolPos = transform.position;
                jet.spawnPos = rectanglePos.position;

                // 풀에 추가
                Pool.Add(jet);
            }
        }
    }
}