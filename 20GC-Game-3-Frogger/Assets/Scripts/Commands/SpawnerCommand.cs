// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Numerics;

public class SpawnerCommand : Command
{
    public SpawnerCommand(EntityManager _entityManager,
        ReplayManager _replayManager,
        float _timeStamp, bool _finished) 
    {
        finished = _finished;
        timeStamp = _timeStamp;
        entityManager = _entityManager;
        replayManager = _replayManager;
    }
    public override void Execute()
    {
        finished = true;
        entityManager.SpawnEntities();
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }

    private EntityManager entityManager;
    private ReplayManager replayManager;

}
