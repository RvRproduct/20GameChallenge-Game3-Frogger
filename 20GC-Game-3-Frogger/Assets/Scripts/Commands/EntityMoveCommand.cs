// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using PoolTags;
using System.Diagnostics;
using System.Numerics;

public class EntityMoveCommand : Command
{
    public EntityMoveCommand(Entity _entity,
        string _entityTag, Vector2 _startPosition, Vector2 _endPosition,
        int _startTick, bool _finished) 
    {
        entity = _entity;
        entityTag = _entityTag;
        startPosition = _startPosition;
        endPosition = _endPosition;
        startTick = _startTick;
        finished = _finished;

    }
    public override void Execute()
    {
        finished = true;
        entity.MoveEntity(this);
    }

    private Entity entity;
    private string entityTag;
    private Vector2 startPosition;
    private Vector2 endPosition;

    public Vector2 GetStartPosition()
    {
        return startPosition;
    }

    public Vector2 GetEndPosition()
    {
        return endPosition;
    }

    public string GetEntityTag()
    {
        return entityTag;
    }

    public void SetEntity(Entity _entity)
    {
        entity = _entity;
    }
    public Entity GetEntity()
    {
        return entity;
    }
}
