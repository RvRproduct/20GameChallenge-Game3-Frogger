using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Replay Icons")]
    [SerializeField] private Image playButton;
    [SerializeField] private Image pauseButton;
    [SerializeField] private Image restartButton;

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
            ReplayManager.Instance.SetIsReplayPlaying(true);
            ReplayManager.Instance.StartReplay();
        }
        else
        {
            ReplayManager.Instance.SetIsReplayPlaying(false);
        }
    }

    public void RestartReplay()
    {
        ReplayManager.Instance.RestartReplay();
    }

    public void RewindReplay()
    {

    }
}
