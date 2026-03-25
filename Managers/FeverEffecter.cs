using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 피버 상태에서 파티클 효과를 순차적으로 재생하는 클래스
/// </summary>
public class FeverEffecter : Singleton<FeverEffecter>
{
    [SerializeField] float fireInterval = 1.0f;     // 파티클 재생 간격
    [SerializeField] ParticleSystem[] particles;    // 자식 파티클 배열

    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화

        // 자식 오브젝트에서 파티클 자동 수집
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    /// <summary>
    /// 파티클 연출을 시작하는 메서드
    /// </summary>
    public void FireWork()
    {
        StartCoroutine(FireWorkMotion());
    }

    /// <summary>
    /// 파티클을 순차적으로 재생하는 메서드
    /// </summary>
    IEnumerator FireWorkMotion()
    {
        for(int i = 0; i < particles.Length; ++i)
        {
            // 파티클 하나씩 재생
            particles[i].Play();

            // 일정 시간 간격 대기
            yield return new WaitForSeconds(fireInterval);
        }
    }
}