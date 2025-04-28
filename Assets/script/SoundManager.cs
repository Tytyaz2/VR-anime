using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    public AudioClip backgroundMusicClip;
    public AudioClip alarmMusicClip;
    public AudioClip bossDanceMusicClip;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true; // Musiques en boucle
    }

    public void PlayBackgroundMusic()
    {
        PlayMusic(backgroundMusicClip, 0.03f);
    }

    public void PlayAlarmMusic()
    {
        PlayMusic(alarmMusicClip);
    }

    public void PlayBossDanceMusic()
    {
        PlayMusic(bossDanceMusicClip);
    }

    private void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
