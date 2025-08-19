// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

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
    private Dictionary<EntityTypes ,List<int>> 
        currentRecordedEntityCommand = new Dictionary<EntityTypes ,List<int>>();
    private List<Command> recordedPlayerMovingCommands = new List<Command>();
    private List<Command> recordedSpawningCommands = new List<Command>();
    private Dictionary<string, List<List<Command>>> 
        recordedEntityCommands = new Dictionary<string, List<List<Command>>>();
    // End of For Commands
    private bool isReplayPlaying = true;
    private bool isRewinding = false;
    private bool isInReplayMode = false;
    private bool isAtEndReplay = false;

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
        StopAllCoroutines();
        StartCoroutine(PlayRecordedCommands(CommandType.PlayerMoving));
    }

    public IEnumerator PlayRecordedCommands(CommandType commandType,
        string entityTag = "", EntityTypes entityType = EntityTypes.None)
    {
        if (isAtEndReplay) { isAtEndReplay = false; }


        if (GetCurrentRecordedCommand(commandType, entityType) >= 
            GetRecordedCommands(commandType, entityTag, entityType).Count)
        {

            /// hmmmmmm
            if (commandType == CommandType.PlayerMoving)
            {
                int commandIndex = GetRecordedCommands(commandType).Count - 1;
                GameManager.Instance.SetGlobalTick(
                    (GetRecordedCommands(commandType))[commandIndex].endTick);
            }
            else
            {
                SetCurrentRecordedCommand(commandType, GetRecordedCommands(commandType,
                entityTag, entityType).Count - 2, entityType);
            }
            
        }
        else if (GetCurrentRecordedCommand(commandType, entityType) < 0)
        {
            
            if (commandType == CommandType.PlayerMoving)
            {
                SetCurrentRecordedCommand(commandType, 0);
                GameManager.Instance.SetGlobalTick(0);
            }
            else
            {
                SetCurrentRecordedCommand(commandType, 0, entityType);
            }
        }

        // Normal
        while (isReplayPlaying &&
            GetCurrentRecordedCommand(commandType, entityType) <= GetRecordedCommands(commandType, entityTag, entityType).Count - 1 &&
            !isRewinding)
        {
            if (GameManager.Instance.GetGlobalTick() >= 
                GetRecordedCommands(commandType, entityTag, entityType)[GetCurrentRecordedCommand
                (commandType, entityType)].startTick)
            {
                if (!GetRecordedCommands(commandType, entityTag, entityType)[GetCurrentRecordedCommand(
                    commandType, entityType)].finished)
                {
                    GetRecordedCommands(commandType, entityTag, entityType)[GetCurrentRecordedCommand(
                        commandType, entityType)].Execute();
                }
            }

            if (GetCurrentRecordedCommand(commandType, entityType) >= GetRecordedCommands(
                commandType, entityTag, entityType).Count - 1)
            {
                isAtEndReplay = true;
            }

            yield return new WaitForFixedUpdate();
        }

        // Rewind
        while (isReplayPlaying &&
            GetCurrentRecordedCommand(commandType, entityType) >= 0 &&
            isRewinding)
        {
            if (GameManager.Instance.GetGlobalTick() <= 
                GetRecordedCommands(commandType, entityTag, entityType)[GetCurrentRecordedCommand(
                    commandType, entityType)].startTick)
            {
                if (!GetRecordedCommands(commandType, entityTag, entityType)[GetCurrentRecordedCommand(
                    commandType, entityType)].finished)
                {
                    GetRecordedCommands(commandType, entityTag, entityType)[GetCurrentRecordedCommand(
                        commandType, entityType)].Execute();
                }
            }

            if (GetCurrentRecordedCommand(commandType, entityType) <= 0)
            {
                isAtEndReplay = true;
            }

            yield return new WaitForFixedUpdate();
        }

    }

    private List<Command> GetRecordedCommands(CommandType commandType,
        string entityTag = "", EntityTypes entityType = EntityTypes.None)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                return recordedPlayerMovingCommands;
            case CommandType.Spawning:
                return recordedSpawningCommands;
            case CommandType.EntityMoving:
                return recordedEntityCommands[entityTag]
                    [currentRecordedEntityCommand[entityType]];
            default:
                return null;
        }
    }

    public int GetCurrentRecordedCommand(CommandType commandType, 
        EntityTypes entityType = EntityTypes.None)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                return currentRecordedPlayerMovingCommand;
            case CommandType.Spawning:
                return currentRecordedSpawningCommand;
            case CommandType.EntityMoving:
                return currentRecordedEntityCommand[entityType];
            default:
                return 0;
        }
    }

    public void SetCurrentRecordedCommand(CommandType commandType ,int _currentCommand,
        EntityTypes entityType = EntityTypes.None)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                currentRecordedPlayerMovingCommand = _currentCommand;
                break;
            case CommandType.Spawning:
                currentRecordedSpawningCommand = _currentCommand;
                break;
            case CommandType.EntityMoving:
                currentRecordedEntityCommand[entityType] = _currentCommand;
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

    public void AddRecordedCommand(CommandType commandType, Command _command, 
        int _entityIndex = 0)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                recordedPlayerMovingCommands.Add(_command);
                break;
            case CommandType.Spawning:
                recordedSpawningCommands.Add(_command);
                break;
            case CommandType.EntityMoving:
                string entityTag = ((EntityMoveCommand)_command).GetEntityTag();

                if (!recordedEntityCommands.ContainsKey(entityTag))
                {
                    recordedEntityCommands.Add(entityTag, new List<List<Command>>());
                }

                if (_entityIndex >= 0 && _entityIndex < recordedEntityCommands[entityTag].Count)
                {
                    recordedEntityCommands[entityTag][_entityIndex].Add(_command);
                }
                else
                {
                    recordedEntityCommands[entityTag].Add(new List<Command>());
                    recordedEntityCommands[entityTag][_entityIndex].Add(_command);
                }
                
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
            Vector2 startPosition = VectorConversions.ToUnity(((MoveCommand)
                recordedPlayerMovingCommands[recordedPlayerMovingCommands.Count - 1]).GetStartPosition());

            GameManager.Instance.SetPlayerStartingLocation(
                new Vector3(startPosition.x, startPosition.y,
                GameManager.Instance.GetPlayer().transform.position.z));
            currentRecordedPlayerMovingCommand = recordedPlayerMovingCommands.Count - 1;
        }
    }

    public void IncrementCurrentRecordedCommand(CommandType commandType,
        EntityTypes entityType = EntityTypes.None)
    {
        switch(commandType)
        {
            case CommandType.PlayerMoving:
                currentRecordedPlayerMovingCommand++;
                break;
            case CommandType.Spawning:
                currentRecordedSpawningCommand++;
                break;
            case CommandType.EntityMoving:
                currentRecordedEntityCommand[entityType]++;
                break;
        }
    }

    public void DecrementCurrentRecordedCommand(CommandType commandType,
        EntityTypes entityType = EntityTypes.None)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                currentRecordedPlayerMovingCommand--;
                break;
            case CommandType.Spawning:
                currentRecordedSpawningCommand--;
                break;
            case CommandType.EntityMoving:
                currentRecordedEntityCommand[entityType]--;
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

    public bool GetIsAtEndReplay()
    {
        return isAtEndReplay;
    }
}
