using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 카메라 화면 영역을 분할하여 랜덤 위치를 생성하는 클래스
/// </summary>
public class RandomPosGenerator : MonoBehaviour
{
    struct ViewportArea
    {
        public Vector2 min; // Viewport 최소 좌표
        public Vector2 max; // Viewport 최대 좌표
    }

    [SerializeField] private Camera targetCamera;       // 기준 카메라
    [SerializeField] private float horizontalOffset;    // 좌우 분할 간격
    [SerializeField] private float verticalOffset;      // 상하 분할 간격

    private List<ViewportArea> viewportAreas = new List<ViewportArea>(); // 분할된 영역 리스트

    void Awake()
    {
        // 카메라가 없으면 메인 카메라 사용
        if (targetCamera == null)
            targetCamera = Camera.main;

        // 화면 비율 기반 오프셋 계산
        Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRight = targetCamera.ViewportToWorldPoint(new Vector3(1, 1, 10f));

        horizontalOffset = 0.14f;

        // 화면 비율에 맞게 세로 오프셋 보정
        verticalOffset = 0.14f * ((topRight.x - bottomLeft.x) / (topRight.y - bottomLeft.y));

        // 영역 초기화
        InitViewportAreas();
    }

    /// <summary>
    /// 화면을 4개의 영역으로 분할하는 메서드
    /// </summary>
    void InitViewportAreas()
    {
        viewportAreas.Clear();

        // 왼쪽 영역
        viewportAreas.Add(new ViewportArea
        {
            min = new Vector2(0f, 0f),
            max = new Vector2(0.5f - horizontalOffset, 1f)
        });

        // 오른쪽 영역
        viewportAreas.Add(new ViewportArea
        {
            min = new Vector2(0.5f + horizontalOffset, 0f),
            max = new Vector2(1f, 1f)
        });

        // 아래 영역
        viewportAreas.Add(new ViewportArea
        {
            min = new Vector2(0f, 0f),
            max = new Vector2(1f, 0.5f - verticalOffset)
        });

        // 위 영역
        viewportAreas.Add(new ViewportArea
        {
            min = new Vector2(0f, 0.5f + verticalOffset),
            max = new Vector2(1f, 1f)
        });
    }

    /// <summary>
    /// 분할된 영역 중 하나에서 랜덤 위치를 반환하는 메서드
    /// </summary>
    public Vector3 GetRandomPos(GameObject prefab)
    {
        // 랜덤 영역 선택
        int randIdx = Random.Range(0, viewportAreas.Count);
        ViewportArea selected = viewportAreas[randIdx];

        // 프리팹 크기 확인
        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        Vector3 size = sr.bounds.size;

        // Viewport 좌표에서 랜덤 위치 생성
        Vector2 randomViewportPos = new Vector2(
            Random.Range(selected.min.x, selected.max.x),
            Random.Range(selected.min.y, selected.max.y)
        );

        // Viewport → World 좌표 변환
        Vector3 worldPos = targetCamera.ViewportToWorldPoint(
            new Vector3(randomViewportPos.x, randomViewportPos.y, 10f)
        );

        worldPos.z = 0f;

        return worldPos;
    }

    /// <summary>
    /// 분할된 영역을 Gizmos로 시각화하는 메서드
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || targetCamera == null) return;

        Gizmos.color = Color.green;

        foreach (var area in viewportAreas)
        {
            // Viewport 영역을 World 좌표로 변환
            Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(
                new Vector3(area.min.x, area.min.y, 10f)
            );

            Vector3 topRight = targetCamera.ViewportToWorldPoint(
                new Vector3(area.max.x, area.max.y, 10f)
            );

            // 사각형 라인으로 표시
            Gizmos.DrawLine(bottomLeft, new Vector3(topRight.x, bottomLeft.y, 0));
            Gizmos.DrawLine(bottomLeft, new Vector3(bottomLeft.x, topRight.y, 0));
            Gizmos.DrawLine(topRight, new Vector3(topRight.x, bottomLeft.y, 0));
            Gizmos.DrawLine(topRight, new Vector3(bottomLeft.x, topRight.y, 0));
        }
    }
}