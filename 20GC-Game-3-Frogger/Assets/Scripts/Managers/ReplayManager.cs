// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance;
    private int currentRecordedCommand = 0;
    private List<Command> recordedPlayMovingCommands = new List<Command>();
    private List<Command> recordedSpawningCommands = new List<Command>();
    private float playBackTimer = 0.0f;
    private bool isReplayPlaying = true;
    private bool isRewinding = false;
    private bool isInReplayMode = false;

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

    public void StartReplay()
    {
        isReplayPlaying = true;
        StopAllCoroutines();
        StartCoroutine(PlayRecordedCommands(currentRecordedCommand, recordedPlayMovingCommands,
            playBackTimer, isReplayPlaying, isRewinding));
    }

    public IEnumerator PlayRecordedCommands(int currentRecordedCommand, 
        List<Command> recordedCommands, float playBackTimer, bool isReplayPlaying, 
        bool isRewinding)
    {
        if (currentRecordedCommand >= recordedCommands.Count)
        {
            currentRecordedCommand = recordedCommands.Count - 1;
        }
        else if (currentRecordedCommand < 0)
        {
            currentRecordedCommand = 0;
        }

        if (currentRecordedCommand < recordedCommands.Count &&
            currentRecordedCommand > 0)
        {
            playBackTimer = recordedCommands[currentRecordedCommand].timeStamp;
        }
        else
        {
            playBackTimer = 0.0f;
        }

        // Normal
        while (isReplayPlaying && 
            currentRecordedCommand <= recordedCommands.Count - 1 &&
            !isRewinding)
        {
            playBackTimer += Time.deltaTime;
            if (playBackTimer >= recordedCommands[currentRecordedCommand].timeStamp)
            {
                if (!recordedCommands[currentRecordedCommand].finished)
                {
                    recordedCommands[currentRecordedCommand].Execute();
                }
            }

            yield return null;
        }

        // Rewind
        while (isReplayPlaying &&
            currentRecordedCommand >= 0 &&
            isRewinding)
        {
            playBackTimer -= Time.deltaTime;
            if (playBackTimer <= recordedCommands[currentRecordedCommand].timeStamp)
            {
                if (!recordedCommands[currentRecordedCommand].finished)
                {
                    recordedCommands[currentRecordedCommand].Execute();
                }
            }

            yield return null;
        }

    }

    public bool GetIsReplayPlaying()
    {
        return isReplayPlaying;
    }

    public void SetIsReplayPlaying(bool _isReplayPlaying)
    {
        isReplayPlaying = _isReplayPlaying;
    }

    public bool GetIsRewinding()
    {
        return isRewinding;
    }

    public void SetIsRewinding(bool _isRewinding)
    {
        isRewinding = _isRewinding;
    }

    public void AddRecordedPlayerMovingCommand(Command _command)
    {
        recordedPlayMovingCommands.Add(_command);
    }

    public void AddRecordedSpawningCommand(Command _command)
    {
        recordedSpawningCommands.Add(_command);
    }

    public void RestartReplay()
    {
        if (!isRewinding)
        {
            // This will need more logic soon
            GameManager.Instance.ResetCurrentCountDown();
            GameManager.Instance.SetPlayerStartingLocation(
            GameManager.Instance.GetPlayer().GetPlayerStartingLocation());
            currentRecordedCommand = 0;
        }
        else
        {
            GameManager.Instance.SetPlayerStartingLocation(
                new Vector3(((MoveCommand)recordedPlayMovingCommands[recordedPlayMovingCommands.Count - 1]).GetPlayerX(),
                ((MoveCommand)recordedPlayMovingCommands[recordedPlayMovingCommands.Count - 1]).GetPlayerY(),
                GameManager.Instance.GetPlayer().transform.position.z));
            currentRecordedCommand = recordedPlayMovingCommands.Count - 1;
        }
    }

    public void IncrementCurrentRecordedCommand()
    {
        currentRecordedCommand++;
    }

    public void DecrementCurrentRecordedCommand()
    {
        currentRecordedCommand--;
    }

    public bool GetIsInReplayMode()
    {
        return isInReplayMode;
    }

    public void SetIsInReplayMode(bool _isInReplayMode)
    {
        isInReplayMode = _isInReplayMode;
    }

    public List<Command> GetRecordedSpawningCommands()
    {
        return recordedSpawningCommands;
    }
}
