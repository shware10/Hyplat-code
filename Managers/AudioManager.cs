using UnityEngine;

// BGM 상태를 구분하기 위한 enum
enum BGMState
{ 
    InGameBGM // 인게임 배경음
}

/// <summary>
/// BGM과 SFX를 관리하고 게임 상태에 따라 오디오를 제어하는 클래스
/// </summary>
public class AudioManager : Singleton<AudioManager>, IGameStateListener
{
    [SerializeField] private AudioSource bgmSource; // 배경음 AudioSource
    [SerializeField] private AudioSource sfxSource; // 효과음 AudioSource

    [SerializeField] private AudioClip[] bgmClips; // 배경음 클립 배열
    [SerializeField] private AudioClip[] sfxClips; // 효과음 클립 배열
    [SerializeField] private AudioClip[] uiClips;  // UI 사운드 클립 배열

    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화
    }

    /// <summary>
    /// 효과음을 재생하는 메서드
    /// </summary>
    public void PlaySFX(int index)
    {
        // 클립 설정 후 한 번 재생
        sfxSource.clip = sfxClips[index];
        sfxSource.loop = false;
        sfxSource.Play();
    }

    /// <summary>
    /// 배경음을 재생하는 메서드
    /// </summary>
    public void PlayBGM(int index)
    {
        // 클립 설정 후 반복 재생
        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// 배경음을 정지하는 메서드
    /// </summary>
    public void StopBGM() => bgmSource.Stop();

    /// <summary>
    /// 게임 상태에 따라 BGM을 변경하는 메서드
    /// </summary>
    public void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.InGame:
                // 인게임 상태에서 BGM 재생
                PlayBGM((int)BGMState.InGameBGM);
                break;

            default:
                // 그 외 상태에서는 BGM 정지
                StopBGM();
                break;
        }
    }
}