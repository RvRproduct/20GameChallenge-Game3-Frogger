using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Replay Icons")]
    [SerializeField] private Sprite playButton;
    [SerializeField] private Sprite pauseButton;
    [SerializeField] private Sprite rewindButton;
    [SerializeField] private Sprite doubleRewindButton;

    [Header("Replay Button Images")]
    [SerializeField] private Image playImage;
    [SerializeField] private Image rewindImage;

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
    }

    public void PlayOrPauseReplay()
    {
        if (!ReplayManager.Instance.GetIsReplayPlaying())
        {
            playImage.sprite = pauseButton;
            ReplayManager.Instance.SetIsReplayPlaying(true);
            ReplayManager.Instance.StartReplay();
        }
        else
        {
            playImage.sprite = playButton;
            ReplayManager.Instance.SetIsReplayPlaying(false);
        }
    }

    public void RestartReplay()
    {
        ReplayManager.Instance.RestartReplay();
        if (ReplayManager.Instance.GetIsReplayPlaying())
        {
            ReplayManager.Instance.StartReplay();
        }
    }

    public void RewindReplay()
    {

    }
}
