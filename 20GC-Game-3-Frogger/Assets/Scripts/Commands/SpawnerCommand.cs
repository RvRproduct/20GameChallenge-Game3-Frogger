// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Numerics;

public class SpawnerCommand : Command
{
    public SpawnerCommand(EntityManager _entityManager,
        string _entityTag, Vector3 _spawnPoint,
        int _startTick, int _endTick ,bool _finished) 
    {
        entityManager = _entityManager;
        entityTag = _entityTag;
        spawnPoint = _spawnPoint;
        startTick = _startTick;
        endTick = _endTick;
        finished = _finished;
    }
    public override void Execute()
    {
        finished = true;
        entityManager.SpawnEntity(this);
    }

    private EntityManager entityManager;
    private string entityTag;
    private Vector3 spawnPoint;

    public string GetEntityTag()
    {
        return entityTag;
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

}
