using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Replay Sprites")]
    [SerializeField] private Sprite forwardSprite;
    [SerializeField] private Sprite doubleForwardSprite;
    [SerializeField] private Sprite rewindSprite;
    [SerializeField] private Sprite doubleRewindSprite;

    [Header("Replay Buttons")]
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button rewindButton;
    [SerializeField] private Button pauseButton;

    //[Header("Replay Images")]
    //[SerializeField] private Image forwardImage;
    //[SerializeField] private Image rewindImage;
    //[SerializeField] private Image pauseImage;


    private Color selectedColor = new Color(1.0f, 1.0f, 0.0f);
    private Color unSelectedColor = Color.black;
    private ColorBlock selectedColorBlock;
    private ColorBlock unSelectedColorBlock;

    private bool isForward = false;
    private bool isRewind = false;
    private bool isPause = false;

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

        // Neat
        selectedColorBlock = pauseButton.colors;
        selectedColorBlock.normalColor = selectedColor;
        selectedColorBlock.selectedColor = selectedColor;
        unSelectedColorBlock = pauseButton.colors;
        unSelectedColorBlock.normalColor = unSelectedColor;
        unSelectedColorBlock.selectedColor = unSelectedColor;
    }

    public void PlayOrPauseReplay()
    {
        if (isPause)
        {
            pauseButton.colors = unSelectedColorBlock;
            isPause = false;
            if (!ReplayManager.Instance.GetIsReplayPlaying())
            {    
                ReplayManager.Instance.SetIsReplayPlaying(true);
                if (isForward || isRewind)
                {
                    ReplayManager.Instance.StartReplay();
                }
            }
        }
        
        else
        {
            pauseButton.colors = selectedColorBlock;
            isPause = true;
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

        if (!isRewind)
        {
            // For visual Clarity
            rewindButton.colors = selectedColorBlock;
            isRewind = true;
            isForward = false;
            forwardButton.colors = unSelectedColorBlock;

            if (!ReplayManager.Instance.GetIsRewinding())
            {
                ReplayManager.Instance.SetIsRewinding(true);
            }

            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                ReplayManager.Instance.StartReplay();
            }
            
        }
        else
        {
            // For visual Clarity
            rewindButton.colors = selectedColorBlock;
            isRewind = false;
        }
    }

    public void ForwardReplay()
    {

        if (!isForward)
        {
            // For visual Clarity
            forwardButton.colors = selectedColorBlock;
            isForward = true;
            isRewind = false;
            rewindButton.colors = unSelectedColorBlock;

            if (ReplayManager.Instance.GetIsRewinding())
            {
                ReplayManager.Instance.SetIsRewinding(false);
            }

            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                ReplayManager.Instance.StartReplay();
            }
            
        }
        else
        {
            // For visual Clarity
            forwardButton.colors = unSelectedColorBlock;
            isForward = false;
        }
    }
}
