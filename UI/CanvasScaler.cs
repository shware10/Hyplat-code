using UnityEngine;

/// <summary>
/// 카메라 뷰 영역에 맞게 Canvas 크기와 위치를 조정하는 클래스
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class CanvasScaler : MonoBehaviour
{
    public Camera targetCamera;     // 기준이 되는 카메라
    public float camDistance = 10f; // 카메라로부터의 거리

    private RectTransform rectTransform; // Canvas의 RectTransform

    private void Awake()
    {
        // 카메라가 없으면 메인 카메라 사용
        if (targetCamera == null)
            targetCamera = Camera.main;

        // RectTransform 캐싱
        rectTransform = GetComponent<RectTransform>();

        // 카메라 기준으로 Canvas 크기 조정
        ScaleCanvasToCameraView();
    }

    /// <summary>
    /// 카메라 뷰 크기에 맞게 Canvas 위치와 크기를 설정하는 메서드
    /// </summary>
    void ScaleCanvasToCameraView()
    {
        // Viewport 좌표를 월드 좌표로 변환
        Vector3 worldBottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector3 worldTopRight = targetCamera.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        // 월드 공간에서의 크기와 중심 계산
        Vector3 worldSize = worldTopRight - worldBottomLeft;
        Vector3 worldCenter = (worldTopRight + worldBottomLeft) * 0.5f;

        // Canvas 위치를 중앙으로 이동
        transform.position = worldCenter;

        // Canvas 크기를 카메라 뷰 크기에 맞게 설정
        rectTransform.sizeDelta = new Vector2(worldSize.x, worldSize.y) * 100;
    }
}