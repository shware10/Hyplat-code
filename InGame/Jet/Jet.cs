using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// Jet 상태 정의
public enum JetState
{ 
    isSpawn, // 활성 상태
    isPool   // 풀 상태
}

/// <summary>
/// Jet의 이동, 연출, 상태 전환을 관리하는 클래스
/// </summary>
public class Jet : MonoBehaviour, IGameStateListener
{
    public Color color; // 색상
    public Vector3 dir; // 이동 방향
    public float angle; // 초기 회전 각도

    [SerializeField] private float spinDuration = 1f;       // 회전 연출 시간
    [SerializeField] private float minSpinDuration = 1.2f;  // 최소 회전 시간
    [SerializeField] private float maxSpinDuration = 1f;    // 최대 회전 시간
    [SerializeField] private float readyTime = 0f;          // 이동 전 대기 시간
    [SerializeField] private float moveSpeed = 2.5f;        // 이동 속도
    [SerializeField] private float minSpeed = 2.5f;         // 최소 속도
    [SerializeField] private float maxSpeed = 4f;           // 최대 속도
    [SerializeField] private float transDuration = 120f;    // 속도 변화 시간

    public Vector3 spawnPos;    // 스폰 위치
    public Vector3 poolPos;     // 풀 위치

    public JetState curState = JetState.isPool; // 현재 상태

    private Collider col;       // 콜라이더
    private SpriteRenderer rdr; // 렌더러

    ParticleSystem fitParticle;     // 성공 파티클
    ParticleSystem unfitParticle;   // 실패 파티클
    ParticleSystem netParticle;     // 네트 파티클

    Coroutine moveRoutine;  // 이동 코루틴
    Coroutine transRoutine; // 속도 변화 코루틴

    void Awake()
    {
        // 컴포넌트 캐싱
        rdr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider>();

        // 파티클 캐싱
        fitParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        unfitParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        netParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    void Start()
    {
        // 초기 회전 설정
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// 성공 처리 메서드
    /// </summary>
    public void Fit()
    {
        StartCoroutine(FitRoutine());
    }

    /// <summary>
    /// 실패 처리 메서드
    /// </summary>
    public void Unfit()
    {
        StartCoroutine(UnfitRoutine());
    }

    /// <summary>
    /// 네트 충돌 처리 메서드
    /// </summary>
    public void Net()
    {
        StartCoroutine(NetRoutine());
    }

    IEnumerator FitRoutine()
    {
        if(moveRoutine != null) StopCoroutine(moveRoutine);

        // 효과 실행
        FitEffect();

        // 비활성화 처리
        rdr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(fitParticle.main.duration);

        Go2PoolPos();
    }

    IEnumerator UnfitRoutine()
    {
        if(moveRoutine != null) StopCoroutine(moveRoutine);

        UnfitEffect();

        rdr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(unfitParticle.main.duration);

        Go2PoolPos();
    }

    IEnumerator NetRoutine()
    {
        if(moveRoutine != null) StopCoroutine(moveRoutine);

        NetEffect();

        rdr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(unfitParticle.main.duration);

        Go2PoolPos();
    }

    // 파티클 재생
    public void FitEffect() => fitParticle.Play();
    public void UnfitEffect() => unfitParticle.Play();
    public void NetEffect() => netParticle.Play();

    /// <summary>
    /// 스폰 위치로 이동시키고 활성화하는 메서드
    /// </summary>
    public void Go2SpawnPos()
    {
        rdr.enabled = true;
        col.enabled = true;

        transform.position = spawnPos;
        curState = JetState.isSpawn;

        moveRoutine = StartCoroutine(MoveRoutine());
    }

    /// <summary>
    /// 풀 위치로 되돌리는 메서드
    /// </summary>
    public void Go2PoolPos()
    {
        curState = JetState.isPool;

        transform.position = poolPos;
        transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// 이동을 수행하는 메서드
    /// </summary>
    IEnumerator MoveRoutine()
    {
        // 등장 연출
        yield return StartCoroutine(SpinMotion());

        yield return new WaitForSeconds(readyTime);

        while (curState == JetState.isSpawn)
        {
            // 방향으로 이동
            transform.position += dir * moveSpeed * Time.deltaTime;

            yield return null;
        }
    }

    /// <summary>
    /// 등장 회전 연출을 수행하는 메서드
    /// </summary>
    IEnumerator SpinMotion()
    {
        float time = 0;

        float originZ = transform.localEulerAngles.z;
        float targetAngle = 360f;
        float targetScale = 0.5f;

        // 방향에 따라 회전 축 변경
        if ((this.angle / 90) % 2 == 0)
        {
            while (time < spinDuration)
            {
                float scale = Mathf.Lerp(0, targetScale, time / spinDuration);
                float angle = Mathf.Lerp(0, targetAngle, time / spinDuration);

                transform.rotation = Quaternion.Euler(0, angle, originZ);
                transform.localScale = Vector3.one * scale;

                time += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (time < spinDuration)
            {
                float scale = Mathf.Lerp(0, targetScale, time / spinDuration);
                float angle = Mathf.Lerp(0, targetAngle, time / spinDuration);

                transform.rotation = Quaternion.Euler(angle, 0, originZ);
                transform.localScale = Vector3.one * scale;

                time += Time.deltaTime;
                yield return null;
            }
        }

        transform.localScale = Vector3.one * targetScale;
    }

    /// <summary>
    /// 게임 상태에 따라 속도 변화를 처리하는 메서드
    /// </summary>
    public void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.InGame:
                transRoutine = StartCoroutine(SpeedTransition());
                break;

            case GameState.GameOver:
                if(transRoutine != null) StopCoroutine(transRoutine);
                InitSpeed();
                break;
        }
    }

    /// <summary>
    /// 시간에 따라 속도를 증가시키는 메서드
    /// </summary>
    IEnumerator SpeedTransition()
    {
        float time = 0;

        while(time < transDuration)
        {
            float percent = time / transDuration;

            moveSpeed = Mathf.Lerp(minSpeed, maxSpeed, percent);
            spinDuration = Mathf.Lerp(minSpinDuration, maxSpinDuration, percent);

            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = maxSpeed;
        spinDuration = maxSpinDuration;
    }

    // 속도 초기화
    void InitSpeed()
    {
        moveSpeed = minSpeed;
        spinDuration = minSpinDuration;
    }
}