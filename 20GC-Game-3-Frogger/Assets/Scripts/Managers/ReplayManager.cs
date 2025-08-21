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
    private Dictionary<EntityTypes , int> 
        currentRecordedEntityCommand = new Dictionary<EntityTypes , int>();
    private List<Command> recordedPlayerMovingCommands = new List<Command>();
    private List<Command> recordedSpawningCommands = new List<Command>();
    private Dictionary<EntityTypes, List<Command>> 
        recordedEntityCommands = new Dictionary<EntityTypes, List<Command>>();
    // End of For Commands
    private bool isReplayPlaying = true;
    private bool isRewinding = false;
    private bool isInReplayMode = false;
    private bool isAtEndReplay = false;
    private bool isStartingFromBack = false;
    // Coroutine
    private Coroutine playerCoroutine = null;
    // Capture Replay End Tick
    private int endReplayTick = 0;
    [Header("Extra UnNeeded Commands")]
    [SerializeField] private int maxNumberOfCuts = 5;
    private int numberOfCuts;

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

        currentRecordedEntityCommand.Add(EntityTypes.Bat, 0);
        currentRecordedEntityCommand.Add(EntityTypes.Skeleton, 0);
        currentRecordedEntityCommand.Add(EntityTypes.SlimeB, 0);
        currentRecordedEntityCommand.Add(EntityTypes.SlimeG, 0);
        currentRecordedEntityCommand.Add(EntityTypes.SlimeR, 0);
        numberOfCuts = maxNumberOfCuts;
    }

    public void StartReplay()
    {
        StopAllCoroutines();
        playerCoroutine = StartCoroutine(PlayRecordedCommands(CommandType.PlayerMoving));
    }

    public IEnumerator PlayRecordedCommands(CommandType commandType)
    {
        if (isAtEndReplay) { isAtEndReplay = false; }

        // Normal
        while (isReplayPlaying &&
            GetCurrentRecordedCommand(commandType) <=
            GetRecordedCommands(commandType).Count - 1 &&
            !isRewinding)
        {
            if (GameManager.Instance.GetGlobalTick() >= 
                GetRecordedCommands(commandType)[GetCurrentRecordedCommand
                (commandType)].startTick)
            {
                if (!GetRecordedCommands(commandType)[GetCurrentRecordedCommand(
                    commandType)].finished)
                {
                    GetRecordedCommands(commandType)[GetCurrentRecordedCommand(
                        commandType)].Execute();
                }
            }

            if (GetCurrentRecordedCommand(commandType) >= GetRecordedCommands(
                commandType).Count - 1)
            {
                if (commandType == CommandType.PlayerMoving)
                {
                    if (isAtEndReplay)
                    {
                        StopCoroutine(playerCoroutine);
                        playerCoroutine = null;
                        //EntityManager.Instance.ResetAllEntities();
                    }
                }   
            }

            yield return new WaitForFixedUpdate();
        }

        // Rewind
        while (isReplayPlaying &&
            GetCurrentRecordedCommand(commandType) >= 0 &&
            isRewinding)
        {
            if (GameManager.Instance.GetGlobalTick() <= 
                GetRecordedCommands(commandType)[GetCurrentRecordedCommand(
                    commandType)].endTick)
            {
                if (!GetRecordedCommands(commandType)[GetCurrentRecordedCommand(
                    commandType)].finished)
                {
                    GetRecordedCommands(commandType)[GetCurrentRecordedCommand(
                        commandType)].Execute();
                }
            }

            if (GetCurrentRecordedCommand(commandType) <= 0)
            {

                if (isAtEndReplay)
                {
                    StopCoroutine(playerCoroutine);
                    playerCoroutine = null;
                    //EntityManager.Instance.ResetAllEntities();
                }
            }

            yield return new WaitForFixedUpdate();
        }

    }

    public void RefreshCurrentRecordedEntity()
    {
        currentRecordedEntityCommand[EntityTypes.Bat] = 0;
        currentRecordedEntityCommand[EntityTypes.Skeleton] = 0;
        currentRecordedEntityCommand[EntityTypes.SlimeB] = 0;
        currentRecordedEntityCommand[EntityTypes.SlimeG] = 0;
        currentRecordedEntityCommand[EntityTypes.SlimeR] = 0;
    }

    public List<Command> GetRecordedCommands(CommandType commandType,
        EntityTypes entityType = EntityTypes.None)
    {
        switch (commandType)
        {
            case CommandType.PlayerMoving:
                return recordedPlayerMovingCommands;
            case CommandType.Spawning:
                return recordedSpawningCommands;
            case CommandType.EntityMoving:
                return recordedEntityCommands[entityType];
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

    public void SetCurrentRecordedCommand(CommandType commandType, int _currentCommand,
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
        EntityTypes entityType = EntityTypes.None)
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
                if (!recordedEntityCommands.ContainsKey(entityType))
                {
                    recordedEntityCommands.Add(entityType, new List<Command>());
                }
                recordedEntityCommands[entityType].Add(_command);
                break;                
        }
    }

    public void RestartReplay()
    {
        // This will need more logic soon
        GameManager.Instance.ResetCurrentCountDown();
        GameManager.Instance.SetPlayerStartingLocation(
        GameManager.Instance.GetPlayer().GetPlayerStartingLocation());
        currentRecordedPlayerMovingCommand = 0;
        currentRecordedSpawningCommand = 0;
        RefreshCurrentRecordedEntity();
    }

    public void SetEndReplayTick(int _endReplayTick)
    {
        endReplayTick = _endReplayTick;
    }

    public int GetEndReplayTick()
    {
        return endReplayTick;
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

    public void SetIsAtEndReplay(bool _isAtEndReplay)
    {
        isAtEndReplay = _isAtEndReplay;
    }

    public void ResetForRewind()
    {
        GameManager.Instance.SetReplayDirection(ReplayDirection.Pause);
        isStartingFromBack = true;
        int commandIndex = GetRecordedCommands(CommandType.PlayerMoving).Count - 2;
        MoveCommand currentMoveCommand = (MoveCommand)GetRecordedCommands(CommandType.PlayerMoving)
            [GetRecordedCommands(CommandType.PlayerMoving).Count - 1];

        GameManager.Instance.SetGlobalTick(endReplayTick);

        SetCurrentRecordedCommand(CommandType.PlayerMoving, commandIndex);

        //SetCurrentRecordedCommand(CommandType.Spawning, GetRecordedCommands(
        //CommandType.Spawning).Count - 1);

        SetCurrentRecordedCommand(CommandType.EntityMoving, GetRecordedCommands(
            CommandType.EntityMoving, EntityTypes.Bat).Count - 1, EntityTypes.Bat);

        SetCurrentRecordedCommand(CommandType.EntityMoving, GetRecordedCommands(
            CommandType.EntityMoving, EntityTypes.Skeleton).Count - 1, EntityTypes.Skeleton);

        SetCurrentRecordedCommand(CommandType.EntityMoving, GetRecordedCommands(
            CommandType.EntityMoving, EntityTypes.SlimeG).Count - 1, EntityTypes.SlimeG);

        SetCurrentRecordedCommand(CommandType.EntityMoving, GetRecordedCommands(
            CommandType.EntityMoving, EntityTypes.SlimeB).Count - 1, EntityTypes.SlimeB);

        SetCurrentRecordedCommand(CommandType.EntityMoving, GetRecordedCommands(
            CommandType.EntityMoving, EntityTypes.SlimeR).Count - 1, EntityTypes.SlimeR);
    }

    public void ResetForForward()
    {
        GameManager.Instance.SetReplayDirection(ReplayDirection.Pause);
        isStartingFromBack = false;
        GameManager.Instance.SetGlobalTick(0);

        SetCurrentRecordedCommand(CommandType.PlayerMoving, 0);

        // SetCurrentRecordedCommand(CommandType.Spawning, 0);

        SetCurrentRecordedCommand(CommandType.EntityMoving, 0, EntityTypes.Bat);

        SetCurrentRecordedCommand(CommandType.EntityMoving, 0, EntityTypes.Skeleton);

        SetCurrentRecordedCommand(CommandType.EntityMoving, 0, EntityTypes.SlimeG);

        SetCurrentRecordedCommand(CommandType.EntityMoving, 0, EntityTypes.SlimeB);

        SetCurrentRecordedCommand(CommandType.EntityMoving, 0, EntityTypes.SlimeR);
    }

    public void CleanUpCommands()
    {
        //for (int currentSpawnCommand = GetRecordedCommands(CommandType.Spawning).Count - 1;
        //    currentSpawnCommand > 0; currentSpawnCommand--)
        //{
        //    if (GetRecordedCommands(CommandType.Spawning)[currentSpawnCommand].endTick >=
        //        GetRecordedCommands(CommandType.PlayerMoving)[
        //            GetRecordedCommands(CommandType.PlayerMoving).Count - 1].endTick)
        //    {
        //        GetRecordedCommands(CommandType.Spawning).RemoveAt(currentSpawnCommand);
        //    }
        //    else
        //    {
        //        break;
        //    }
        //}

        CleanUpEntityCommands(EntityTypes.Bat);
        CleanUpEntityCommands(EntityTypes.Skeleton);
        CleanUpEntityCommands(EntityTypes.SlimeB);
        CleanUpEntityCommands(EntityTypes.SlimeG);
        CleanUpEntityCommands(EntityTypes.SlimeR);
    }

    private void CleanUpEntityCommands(EntityTypes entityType)
    {
        for (int currentEntityCommand = GetRecordedCommands(CommandType.EntityMoving, entityType).Count - 1;
            currentEntityCommand > 0; currentEntityCommand--)
        {
            if (GetRecordedCommands(CommandType.EntityMoving, entityType)[currentEntityCommand].endTick >=
                GetRecordedCommands(CommandType.PlayerMoving)[
                    GetRecordedCommands(CommandType.PlayerMoving).Count - 1].endTick &&
                    numberOfCuts > 0)
            {
                if (endReplayTick < GetRecordedCommands(CommandType.EntityMoving, entityType)[currentEntityCommand].endTick)
                {
                    endReplayTick = GetRecordedCommands(CommandType.EntityMoving, entityType)[currentEntityCommand].endTick;
                }
                GetRecordedCommands(CommandType.EntityMoving, entityType).RemoveAt(currentEntityCommand);
                numberOfCuts--;
            }
            else
            {
                numberOfCuts = maxNumberOfCuts;
                break;
            }
        }
    }

    public void NullAllEntitiesToCommands(bool resetMidWay)
    {
        NullEntitiesToCommands(EntityTypes.Bat, resetMidWay);
        NullEntitiesToCommands(EntityTypes.Skeleton, resetMidWay);
        NullEntitiesToCommands(EntityTypes.SlimeB, resetMidWay);
        NullEntitiesToCommands(EntityTypes.SlimeG, resetMidWay);
        NullEntitiesToCommands(EntityTypes.SlimeR, resetMidWay);
    }

    private void NullEntitiesToCommands(EntityTypes entityType, bool resetMidWay)
    {
        foreach (Command currentEntityCommand in GetRecordedCommands(CommandType.EntityMoving, entityType))
        {
            if (!resetMidWay)
            {
                ((EntityMoveCommand)currentEntityCommand).SetEntity(null);
            }
            else
            {
                if (((EntityMoveCommand)currentEntityCommand).GetEntity() != null &&
                    !((EntityMoveCommand)currentEntityCommand).GetEntity().isActiveAndEnabled)
                {
                    ((EntityMoveCommand)currentEntityCommand).SetEntity(null);
                }
            }
            
        }
    }

    public void SetIsStartingFromBack(bool _isStartingFromBack)
    {
        isStartingFromBack = _isStartingFromBack;
    }

    public bool GetIsStartingFromBack()
    {
        return isStartingFromBack;
    }
}
