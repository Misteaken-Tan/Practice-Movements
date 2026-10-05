using UnityEngine;

public class GameResultAudio : MonoBehaviour
{
    public static GameResultAudio Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip winSound;
    public AudioClip gameOverSound;

    void Awake()
    {
        Instance = this;
    }

    public void TriggerWin()
    {
        if (bgmSource) bgmSource.Stop();
        if (sfxSource && winSound) sfxSource.PlayOneShot(winSound);
    }

    public void TriggerLoss()
    {
        if (bgmSource) bgmSource.Stop();
        if (sfxSource && gameOverSound) sfxSource.PlayOneShot(gameOverSound);
    }
}