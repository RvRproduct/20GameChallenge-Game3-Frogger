// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Data;

public enum CommandType
{
    None,
    PlayerMoving,
    Spawning,
    EntityMoving,
    Spikes
}

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance;
    // For Commands
    private int currentRecordedPlayerMovingCommand = 0;
    private int currentRecordedSpawningCommand = 0;
    private List<Command> recordedPlayerMovingCommands = new List<Command>();
    private List<Command> recordedSpawningCommands = new List<Command>();
    // End of For Commands
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
        StartCoroutine(PlayRecordedCommands(CommandType.PlayerMoving));
    }

    public IEnumerator PlayRecordedCommands(CommandType commandType)
    {

        if (GetCurrentRecordedCommand(commandType) >= GetRecordedCommands(commandType).Count)
        {
            SetCurrentRecordedCommand(commandType, GetRecordedCommands(commandType).Count - 1);
        }
        else if (GetCurrentRecordedCommand(commandType) < 0)
        {
            SetCurrentRecordedCommand(commandType, 0);
        }

        // Normal
        while (isReplayPlaying &&
            GetCurrentRecordedCommand(commandType) <= GetRecordedCommands(commandType).Count - 1 &&
            !isRewinding)
        {
            if (GameManager.Instance.GetGlobalTick() >= 
                GetRecordedCommands(commandType)[GetCurrentRecordedCommand(commandType)].startTick)
            {
                //Debug.Log("Playing Next Command on execute");
                

                if (!GetRecordedCommands(commandType)[GetCurrentRecordedCommand(commandType)].finished)
                {
                    Debug.Log("Command on execute");
                    Debug.Log($"Current Recorded Command {GetCurrentRecordedCommand(commandType)}");
                    GetRecordedCommands(commandType)[GetCurrentRecordedCommand(commandType)].Execute();
                }
            }

            yield return null;
        }

        // Rewind
        while (isReplayPlaying &&
            GetCurrentRecordedCommand(commandType) >= 0 &&
            isRewinding)
        {
            if (GameManager.Instance.GetGlobalTick() <= 
                GetRecordedCommands(commandType)[GetCurrentRecordedCommand(commandType)].startTick)
            {
                if (!GetRecordedCommands(commandType)[GetCurrentRecordedCommand(commandType)].finished)
                {
                    GetRecordedCommands(commandType)[GetCurrentRecordedCommand(commandType)].Execute();
                }
            }

            yield return null;
        }

    }

    private List<Command> GetRecordedCommands(CommandType commandType)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                return recordedPlayerMovingCommands;
            case CommandType.Spawning:
                return recordedSpawningCommands;
            default:
                return null;
        }
    }

    public int GetCurrentRecordedCommand(CommandType commandType)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                return currentRecordedPlayerMovingCommand;
            case CommandType.Spawning:
                return currentRecordedSpawningCommand;
            default:
                return 0;
        }
    }

    public void SetCurrentRecordedCommand(CommandType commandType ,int _currentCommand)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                currentRecordedPlayerMovingCommand = _currentCommand;
                break;
            case CommandType.Spawning:
                currentRecordedSpawningCommand = _currentCommand;
                break;
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

    public void AddRecordedCommand(CommandType commandType, Command _command)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                recordedPlayerMovingCommands.Add(_command);
                break;
            case CommandType.Spawning:
                recordedSpawningCommands.Add(_command);
                break;
        }
    }

    public void RestartReplay()
    {
        if (!isRewinding)
        {
            // This will need more logic soon
            GameManager.Instance.ResetCurrentCountDown();
            GameManager.Instance.SetPlayerStartingLocation(
            GameManager.Instance.GetPlayer().GetPlayerStartingLocation());
            currentRecordedPlayerMovingCommand = 0;
            currentRecordedSpawningCommand = 0;
        }
        else
        {
            // NEEDS More logic for other edge cases
            Vector2 startPosition = Vector2Conversions.ToUnity(((MoveCommand)
                recordedPlayerMovingCommands[recordedPlayerMovingCommands.Count - 1]).GetStartPosition());

            GameManager.Instance.SetPlayerStartingLocation(
                new Vector3(startPosition.x, startPosition.y,
                GameManager.Instance.GetPlayer().transform.position.z));
            currentRecordedPlayerMovingCommand = recordedPlayerMovingCommands.Count - 1;
        }
    }

    public void IncrementCurrentRecordedCommand(CommandType commandType)
    {
        switch(commandType)
        {
            case CommandType.PlayerMoving:
                currentRecordedPlayerMovingCommand++;
                break;
            case CommandType.Spawning:
                currentRecordedSpawningCommand++;
                break;
        }
    }

    public void DecrementCurrentRecordedCommand(CommandType commandType)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                currentRecordedPlayerMovingCommand--;
                break;
            case CommandType.Spawning:
                currentRecordedSpawningCommand--;
                break;
        }
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
