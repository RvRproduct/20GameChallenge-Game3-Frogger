using PoolTags;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovingManager : MonoBehaviour
{
    static public EntityMovingManager Instance;

    //private Dictionary<string, List<int>> entitiesIndex = new Dictionary<string, List<int>>();
    private Dictionary<EntityTypes, Coroutine> entityCoroutines = new Dictionary<EntityTypes, Coroutine>();

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

        entityCoroutines.Add(EntityTypes.Bat, null);
        entityCoroutines.Add(EntityTypes.Skeleton, null);
        entityCoroutines.Add(EntityTypes.SlimeB, null);
        entityCoroutines.Add(EntityTypes.SlimeG, null);
        entityCoroutines.Add(EntityTypes.SlimeR, null);
    }

    public void ReplayFromEntityManager(EntityTypes _entityType, Entity entity)
    {
        ReplayManager.Instance.SetupEntity(CommandType.EntityMoving, _entityType, entity);

        switch (_entityType)
        {   
            case EntityTypes.Bat:
                if (entityCoroutines[EntityTypes.Bat] == null)
                {
                    entityCoroutines[EntityTypes.Bat] = 
                        StartCoroutine(ReplayManager.Instance.PlayRecordedCommands(
                            CommandType.EntityMoving, _entityType));
                }
                break;
            case EntityTypes.Skeleton:
                if (entityCoroutines[EntityTypes.Skeleton] == null)
                {
                    entityCoroutines[EntityTypes.Skeleton] =
                        StartCoroutine(ReplayManager.Instance.PlayRecordedCommands(
                            CommandType.EntityMoving, _entityType));
                }
                break;
            case EntityTypes.SlimeB:
                if (entityCoroutines[EntityTypes.SlimeB] == null)
                {
                    entityCoroutines[EntityTypes.SlimeB] =
                        StartCoroutine(ReplayManager.Instance.PlayRecordedCommands(
                            CommandType.EntityMoving, _entityType));
                }
                break;
            case EntityTypes.SlimeG:
                if (entityCoroutines[EntityTypes.SlimeG] == null)
                {
                    entityCoroutines[EntityTypes.SlimeG] =
                        StartCoroutine(ReplayManager.Instance.PlayRecordedCommands(
                            CommandType.EntityMoving, _entityType));
                }
                break;
            case EntityTypes.SlimeR:
                if (entityCoroutines[EntityTypes.SlimeR] == null)
                {
                    entityCoroutines[EntityTypes.SlimeR] =
                        StartCoroutine(ReplayManager.Instance.PlayRecordedCommands(
                            CommandType.EntityMoving, _entityType));
                }
                break;
            default:
                //// This Makes sure that we Set the correct spawn Entity Index whenever we rewind
                //// or go forward
                if (ReplayManager.Instance.GetIsInReplayMode() &&
                    ReplayManager.Instance.GetIsReplayPlaying())
                {
                    if (!ReplayManager.Instance.GetIsRewinding())
                    {
                        ReplayManager.Instance.IncrementCurrentRecordedCommand(
                            CommandType.EntityMoving, entity.GetEntityType());
                    }
                    else
                    {
                        ReplayManager.Instance.DecrementCurrentRecordedCommand(
                            CommandType.EntityMoving, entity.GetEntityType());
                    }
                }
                break;
        }
    }

    public void RefreshReplayFromEntityManager()
    {
        StopAllCoroutines();
    }

    // I dont like this
    public void EntityMoveCommander(Entity _entity, 
        string _entityTag, Vector2 _spawnPoint)
    {
       
        EntityTypes entityType = _entity.GetEntityType();

        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            StartCoroutine(MovingEntity(_entity, _entityTag,
                entityType, _spawnPoint));
        }
    }

    private IEnumerator MoveEntity(Entity entity, string _entityTag,
        EntityTypes _entityType, Vector2 _spawnPoint, int _currentEntityIndex)
    {
        // We could use an event here probably
        while (!entity.GetHasReachedDestination())
        {
            if (!ReplayManager.Instance.GetIsInReplayMode())
            {
                Command entityCommand = HandleEntityMove(entity, _entityTag, _spawnPoint);
                ReplayManager.Instance.AddRecordedCommand(CommandType.EntityMoving,
                       entityCommand, _entityType);

                entityCommand.Execute();
            }
            
            yield return new WaitUntil(() => entity.GetEntityMoving() == null);
        }
    }

    private IEnumerator MovingEntity(Entity entity, string _entityTag,
        EntityTypes _entityType, Vector2 _spawnPoint, int _currentEntityIndex = -1)
    {
        yield return StartCoroutine(MoveEntity(entity, _entityTag,
            _entityType ,_spawnPoint, _currentEntityIndex));

        entity.CleanUpEntity();
    }

    private Command HandleEntityMove(Entity _entity, string _entityTag,
        Vector2 _spawnPoint)
    {
        // Our spawn points are just the opposites on the x axis (start and end point),
        // so we can do this.
        Vector2 endPosition = new Vector2((_spawnPoint.x * -1), _spawnPoint.y);

        return new EntityMoveCommand(_entity, _entityTag, 
            VectorConversions.ToSystem(_spawnPoint), VectorConversions.ToSystem(endPosition),
            GameManager.Instance.GetGlobalTick(), true);
    }
}
