// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundAudioSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip winClip;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartMusic();
    }

    private void StartMusic()
    {
        musicAudioSource.loop = true;
        musicAudioSource.clip = musicClip;
        musicAudioSource.Play();
    }

    public void PlayDieSound()
    {
        soundAudioSource.clip = dieClip;
        soundAudioSource.Play();
    }

    public void PlayWinSound()
    {
        soundAudioSource.clip = winClip;
        soundAudioSource.Play();
    }
}
