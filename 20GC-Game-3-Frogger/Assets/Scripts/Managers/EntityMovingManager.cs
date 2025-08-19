using PoolTags;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovingManager : MonoBehaviour
{
    static public EntityMovingManager Instance;

    private Dictionary<string, List<int>> entitiesIndex = new Dictionary<string, List<int>>();

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

    public void StartReplayFromEntityManager(string _entityTag,
        EntityTypes _entityType)
    {
        StopAllCoroutines();
        ReplayManager.Instance.PlayRecordedCommands(CommandType.EntityMoving,
                    _entityTag, _entityType);
    }

    // I dont like this
    public void EntityMoveCommander(Entity _entity, 
        string _entityTag, Vector2 _spawnPoint)
    {
       
        EntityTypes entityType = _entity.GetEntityType();

        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            if (!entitiesIndex.ContainsKey(_entityTag))
            {
                entitiesIndex.Add(_entityTag, new List<int>());
            }

            int currentEntityIndex = entitiesIndex[_entityTag].Count;
            entitiesIndex[_entityTag].Add(currentEntityIndex);

            StartCoroutine(MovingEntity(_entity, _entityTag,
                entityType, _spawnPoint, currentEntityIndex));
        }
        else
        {
            int currentEntityIndex = ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.EntityMoving,
                entityType);

            StartCoroutine(MovingEntity(_entity, _entityTag,
                entityType, _spawnPoint, currentEntityIndex));
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
                       entityCommand, _currentEntityIndex);

                entityCommand.Execute();
            }
            else
            {

            }
            
            yield return new WaitUntil(() => entity.GetEntityMoving() == null);
        }
    }

    private IEnumerator MovingEntity(Entity entity, string _entityTag,
        EntityTypes _entityType, Vector2 _spawnPoint, int _currentEntityIndex)
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
