// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Numerics;

public class SpawnerCommand : Command
{
    public SpawnerCommand(EntityManager _entityManager,
        ReplayManager _replayManager, int _startTick, 
        int _endTick ,bool _finished) 
    {
        entityManager = _entityManager;
        replayManager = _replayManager;
        startTick = _startTick;
        endTick = _endTick;
        finished = _finished;
    }
    public override void Execute()
    {
        finished = true;
        entityManager.SpawnEntities();
    }

    private EntityManager entityManager;
    private ReplayManager replayManager;

}
