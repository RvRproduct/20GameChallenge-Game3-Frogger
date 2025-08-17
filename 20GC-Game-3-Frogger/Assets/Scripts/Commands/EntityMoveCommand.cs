// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using PoolTags;
using System.Numerics;

public class EntityMoveCommand : Command
{
    public EntityMoveCommand(Entity _entity,
        EntityTags _entityTag, Vector2 _startPosition, Vector2 _endPosition,
        int _startTick, int _endTick, bool _finished) 
    {
        entity = _entity;
        entityTag = _entityTag;
        startPosition = _startPosition;
        endPosition = _endPosition;
        startTick = _startTick;
        endTick = _endTick;
        finished = _finished;

    }
    public override void Execute()
    {
        finished = true;
    }

    private Entity entity;
    private EntityTags entityTag;
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
}
