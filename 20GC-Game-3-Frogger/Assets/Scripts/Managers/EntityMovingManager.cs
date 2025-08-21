// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using PoolTags;
using UnityEngine;

public class EntityMovingManager : MonoBehaviour
{
    static public EntityMovingManager Instance;

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

    public void ReplayFromEntityManager(EntityTypes _entityType, Entity entity)
    {
        if (ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.EntityMoving, _entityType) <
            ReplayManager.Instance.GetRecordedCommands(
            CommandType.EntityMoving, _entityType).Count &&
            (ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.EntityMoving, _entityType) >= 0))
        {
            EntityMoveCommand currentEntityMoveCommand = (EntityMoveCommand)ReplayManager.Instance.GetRecordedCommands(
            CommandType.EntityMoving, _entityType)[
            ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.EntityMoving, _entityType)];

            if (currentEntityMoveCommand.GetEntity() == null)
            {
                currentEntityMoveCommand.SetEntity(entity);
            }
            else
            {
                entity.CleanUpOutlier();
            }    
            
            currentEntityMoveCommand.Execute();

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
        } 
    }

    // I dont like this
    public void EntityMoveCommander(Entity _entity, 
        string _entityTag, Vector2 _spawnPoint)
    {
       
        EntityTypes entityType = _entity.GetEntityType();

        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            Command entityCommand = HandleEntityMove(_entity, _entityTag, _spawnPoint);
            ReplayManager.Instance.AddRecordedCommand(CommandType.EntityMoving,
                   entityCommand, _entity.GetEntityType());

            entityCommand.Execute();
        }
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
