// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Player Lives")]
    [SerializeField] private GameObject lifeOne;
    [SerializeField] private GameObject lifeTwo;
    [SerializeField] private GameObject lifeThree;

    [Header("Count Down Timer")]
    [SerializeField] private TextMeshProUGUI countDownText;

    [Header("Replay Mode Prompt")]
    [SerializeField] GameObject replayModePrompt;

    [Header("ReplayMode")]
    [SerializeField] GameObject replayMode;
    [SerializeField] GameObject LeaveReplayMode;


    private Color selectedColor = new Color(1.0f, 1.0f, 0.0f);
    private Color unSelectedColor = Color.black;
    private ColorBlock selectedColorBlock;
    private ColorBlock unSelectedColorBlock;

    private bool isForward = false;
    private bool isRewind = false;
    private bool isPause = false;
    private bool isFirstTime = true;

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
            if (!ReplayManager.Instance.GetIsRewinding())
            {
                GameManager.Instance.SetReplayDirection(ReplayDirection.Forward);
            }
            else
            {
                GameManager.Instance.SetReplayDirection(ReplayDirection.Rewind);
            }

            pauseButton.colors = unSelectedColorBlock;
            isPause = false;
            if (isForward)
            {
                SpikeManager.Instance.ForwardAllSpikes();
            }
            else
            {
                SpikeManager.Instance.ReverseAllSpikes();
            }

            if (!ReplayManager.Instance.GetIsReplayPlaying())
            {
                ReplayManager.Instance.SetIsReplayPlaying(true);
                if (isForward || isRewind)
                {
                    ReplayManager.Instance.StartReplay();
                    SpikeManager.Instance.StartReplayForAllSpikes();
                    // EntityManager.Instance.StartReplay();
                }
            }
        }

        else
        {
            pauseButton.colors = selectedColorBlock;
            isPause = true;
            SpikeManager.Instance.PauseAllSpikes();
            GameManager.Instance.SetReplayDirection(ReplayDirection.Pause);
            ReplayManager.Instance.SetIsReplayPlaying(false);
        }
    }

    public void RestartReplay()
    {
        // resetting Global Tick
        GameManager.Instance.SetGlobalTick(0);
        ReplayManager.Instance.RestartReplay();
        EntityManager.Instance.ResetAllEntities();
        SpikeManager.Instance.ResetAllSpikes();

        if (ReplayManager.Instance.GetIsReplayPlaying())
        {
            ReplayManager.Instance.StartReplay();
            SpikeManager.Instance.StartReplayForAllSpikes();
            //EntityManager.Instance.StartReplay();
        }
    }

    public void RewindReplay()
    {
        if (!isRewind)
        {
            if (ReplayManager.Instance.GetIsAtEndReplay())
            {
                ReplayManager.Instance.ResetForRewind();
                SpikeManager.Instance.ReverseAllSpikes(true);
            }
            else if (ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.PlayerMoving) >=
                ReplayManager.Instance.GetRecordedCommands(CommandType.PlayerMoving).Count - 1)
            {
                ReplayManager.Instance.SetIsAtEndReplay(true);
                EntityManager.Instance.ResetAllEntities();
                ReplayManager.Instance.NullAllEntitiesToCommands(false);

                ReplayManager.Instance.ResetForRewind();
                SpikeManager.Instance.ReverseAllSpikes(true);
            }
            else
            {
                ReplayManager.Instance.NullAllEntitiesToCommands(true);
                SpikeManager.Instance.ReverseAllSpikes(false);
            }

            MoveCommand currentMoveCommand = (MoveCommand)ReplayManager.Instance.GetRecordedCommands(
                CommandType.PlayerMoving)[ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.PlayerMoving)];

            // For visual Clarity
            rewindButton.colors = selectedColorBlock;
            isRewind = true;
            isForward = false;
            forwardButton.colors = unSelectedColorBlock;
            GameManager.Instance.SetReplayDirection(ReplayDirection.Rewind);
            GameManager.Instance.SetCurrentCountDown(currentMoveCommand.GetCountDownTimeEnd());

            if (!ReplayManager.Instance.GetIsRewinding())
            {
                EntityManager.Instance.SetPoolTagForReplay();     
                ReplayManager.Instance.SetIsRewinding(true);
            }

            if (isFirstTime)
            {
                ReplayManager.Instance.SetIsReplayPlaying(true);
                SpikeManager.Instance.ReverseAllSpikes(true);
                isFirstTime = false;
            }

            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                ReplayManager.Instance.StartReplay();
                SpikeManager.Instance.StartReplayForAllSpikes();
                //EntityManager.Instance.StartReplay();
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
            if (ReplayManager.Instance.GetIsAtEndReplay())
            {
                ReplayManager.Instance.ResetForForward();
                SpikeManager.Instance.ForwardAllSpikes(true);
            }
            else if (ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.PlayerMoving) <= 0 &&
                !isFirstTime)
            {
                ReplayManager.Instance.SetIsAtEndReplay(true);
                EntityManager.Instance.ResetAllEntities();
                ReplayManager.Instance.NullAllEntitiesToCommands(false);

                ReplayManager.Instance.ResetForForward();
                SpikeManager.Instance.ForwardAllSpikes(true);
            }
            else
            {
                if (ReplayManager.Instance.GetIsStartingFromBack())
                {
                    // Ya.... this is a guard if you are starting from back and 
                    // try to switch directions
                    ReplayManager.Instance.SetIsAtEndReplay(true);
                    EntityManager.Instance.ResetAllEntities();
                    ReplayManager.Instance.NullAllEntitiesToCommands(false);
                    GameManager.Instance.GetPlayer().ResetToStartingLocation();
                    ReplayManager.Instance.ResetForForward();
                    SpikeManager.Instance.ForwardAllSpikes(true);
                }
                else
                {
                    //ReplayManager.Instance.NullAllEntitiesToCommands(true);
                    SpikeManager.Instance.ForwardAllSpikes(false);
                } 
            }

            MoveCommand currentMoveCommand = (MoveCommand)ReplayManager.Instance.GetRecordedCommands(
            CommandType.PlayerMoving)[ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.PlayerMoving)];

            // For visual Clarity
            forwardButton.colors = selectedColorBlock;
            isForward = true;
            isRewind = false;
            rewindButton.colors = unSelectedColorBlock;
            GameManager.Instance.SetReplayDirection(ReplayDirection.Forward);
            GameManager.Instance.SetCurrentCountDown(currentMoveCommand.GetCountDownTimeStart());

            if (ReplayManager.Instance.GetIsRewinding())
            {
                EntityManager.Instance.SetPoolTagForReplay();
                ReplayManager.Instance.SetIsRewinding(false);
            }

            if (isFirstTime)
            {
                ReplayManager.Instance.SetIsReplayPlaying(true);
                SpikeManager.Instance.ForwardAllSpikes(true);
                isFirstTime = false;
            }

            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                ReplayManager.Instance.StartReplay();
                SpikeManager.Instance.StartReplayForAllSpikes();
              // EntityManager.Instance.StartReplay();
            }

        }
        else
        {
            // For visual Clarity
            forwardButton.colors = unSelectedColorBlock;
            isForward = false;
        }
    }

    public void TakeALifeAway()
    {
        if (lifeOne.activeInHierarchy)
        {
            lifeOne.SetActive(false);
        }
        else if (lifeTwo.activeInHierarchy)
        {
            lifeTwo.SetActive(false);
        }
        else if (lifeThree.activeInHierarchy)
        {
            lifeThree.SetActive(false);
        }
    }

    public void UpdateCountDownTimer(int currentCountDown)
    {
        int mins = currentCountDown / 60;
        int seconds = currentCountDown % 60;

        countDownText.text = $"{mins:0}:{seconds:00}";
    }

    // Replay Mode and Buttons
    public void PromptForReplay()
    {
        replayModePrompt.SetActive(true);
    }

    public void EnterReplayMode()
    {
        GameManager.Instance.SetReplayDirection(ReplayDirection.Pause);
        replayModePrompt.SetActive(false);
        ReplayManager.Instance.SetIsReplayPlaying(false);
        ReplayManager.Instance.SetIsInReplayMode(true);
        ReplayManager.Instance.CleanUpCommands();
        RestartReplay();
        replayMode.SetActive(true);
    }

    public void IgnoreReplayMode()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
