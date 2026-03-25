using UnityEngine;

/// <summary>
/// LineRenderer를 이용해 꼬리 형태의 트레일을 생성하는 클래스
/// </summary>
public class Tail : MonoBehaviour
{
    private int length;              // 꼬리 포인트 개수
    private LineRenderer lineRdr;    // 라인 렌더러
    private Vector3[] points;        // 각 포인트 위치 배열
    private Vector3[] pointVelocity; // SmoothDamp용 속도 배열
    
    private int randInt; // 랜덤값

    [SerializeField] private Transform targetDir;   // 기준 위치
    [SerializeField] private float targetDist;      // 포인트 간 거리
    [SerializeField] private float smoothSpeed;     // 따라가는 속도
    [SerializeField] private float trailSpeed;      // 뒤쪽 지연 속도

    [SerializeField] private float wiggleSpeed;     // 흔들림 속도
    [SerializeField] private float wiggleMagnitude; // 흔들림 크기
    [SerializeField] private Transform wiggleDir;   // 흔들림 방향 기준

    void Start()
    {
        // 라인 포인트 개수 설정
        lineRdr.positionCount = length;

        // 포인트 배열 초기화
        points = new Vector3[length];
        pointVelocity = new Vector3[length];

        // 랜덤 값 생성
        randInt = Random.Range(0, 1000);
    }

    /// <summary>
    /// 꼬리 위치를 갱신하고 흔들림을 적용하는 메서드
    /// </summary>
    void Update()
    {
        // sin 기반 회전으로 흔들림 생성
        wiggleDir.localRotation = Quaternion.Euler(
            0,
            0,
            Mathf.Sin((Time.time + randInt) * wiggleSpeed) * wiggleMagnitude
        );

        // 첫 포인트를 기준 위치로 설정
        points[0] = targetDir.position;

        for(int i = 1; i < points.Length; ++i)
        {
            // 앞 포인트를 따라가며 부드럽게 이동
            points[i] = Vector3.SmoothDamp(
                points[i],
                points[i - 1] + targetDir.right * targetDist,
                ref pointVelocity[i],
                smoothSpeed + i / trailSpeed
            );
        }

        // 라인 렌더러에 위치 적용
        lineRdr.SetPositions(points);
    }
}