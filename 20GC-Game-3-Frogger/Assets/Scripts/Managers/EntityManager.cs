// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum EntityTypes
{
    Bat,
    Skeleton,
    SlimeB,
    SlimeG,
    SlimeR
}

public class EntityManager : ObjectPool
{
    public static EntityManager Instance;

    [System.Serializable]
    public class EntityLocationPlacement
    {
        public EntityTypes entityType;
        public List<Transform> entitySpawnPoint;
        public float maxSpawnRateTime = 2.0f;
        [HideInInspector] public float currentSpawnRateTime = 0.0f;
    }

    [SerializeField] private List<EntityLocationPlacement> entityLocationPlacements;

    // For Play Back Purposes
    private Coroutine spawning;

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        base.Awake();

        ResetSpawnTimes();
    }

    private void Start()
    {
        SetUpObjectPool();
    }

    public void SpawnEntities()
    {
        if (spawning == null)
        {
            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                spawning = StartCoroutine(SpawnEntitiesInWorld());
            }
        }
    }

    private void FixedUpdate()
    {
        if (spawning == null &&
            !ReplayManager.Instance.GetIsInReplayMode() &&
            ReplayManager.Instance.GetIsReplayPlaying())
        {
            SpawnTimer();
        } 
    }

    private void SpawnTimer()
    {

        if (spawning == null)
        {
            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                spawning = StartCoroutine(SpawnEntitiesInWorld());
            }
        }
    }

    private void HandleCommand(Command _spawn)
    {
        _spawn.Execute();
    }

    private Command HandleSpawn(string _entityTag, Vector3 _spawnPoint)
    {
        return new SpawnerCommand(this,
            _entityTag,
            VectorConversions.ToSystem(_spawnPoint),
            GameManager.Instance.GetGlobalTick(),
            GameManager.Instance.GetGlobalTick(),
            true);
    }

    public void StartReplay()
    {
        StopAllCoroutines();
        StartCoroutine(ReplayManager.Instance.PlayRecordedCommands(CommandType.Spawning));
    }

    private IEnumerator SpawnEntitiesInWorld()
    {
        foreach (EntityLocationPlacement entityLocation in entityLocationPlacements)
        {
            entityLocation.currentSpawnRateTime += Time.fixedDeltaTime;

            switch (entityLocation.entityType)
            {
                case EntityTypes.Bat:
                    if (CanSpawnEntity(entityLocation))
                    {
                        CheckReplaySpawnPoint(PoolTags.EntityTags.BatEntity, entityLocation);
                    }
                    break;
                case EntityTypes.Skeleton:
                    if (CanSpawnEntity(entityLocation))
                    {
                        CheckReplaySpawnPoint(PoolTags.EntityTags.SkeletonEntity, entityLocation);
                    }
                    break;
                case EntityTypes.SlimeB:
                    if (CanSpawnEntity(entityLocation))
                    {
                        CheckReplaySpawnPoint(PoolTags.EntityTags.SlimeBEntity, entityLocation);
                    }
                    break;
                case EntityTypes.SlimeG:
                    if (CanSpawnEntity(entityLocation))
                    {
                        CheckReplaySpawnPoint(PoolTags.EntityTags.SlimeGEntity, entityLocation);
                    }
                    break;
                case EntityTypes.SlimeR:
                    if (CanSpawnEntity(entityLocation))
                    {
                        CheckReplaySpawnPoint(PoolTags.EntityTags.SlimeREntity, entityLocation);
                    }
                    break;
            }

            spawning = null;

            yield return null;
        }
    }

    private bool CanSpawnEntity(EntityLocationPlacement entityLocation)
    {
        if (entityLocation.currentSpawnRateTime >= entityLocation.maxSpawnRateTime)
        {
            entityLocation.currentSpawnRateTime = 0;
            return true;
        }

        return false;
    }

    // For Command on Execute
    public void SpawnEntity(Command _spawnCommand)
    {
        GameObject validObject = GetValidObjectInPool(((SpawnerCommand)_spawnCommand).GetEntityTag());

        Vector3 spawnPoint = VectorConversions.ToUnity(((SpawnerCommand)_spawnCommand).GetSpawnPoint());
        if (!ReplayManager.Instance.GetIsRewinding())
        {
            validObject.transform.position = spawnPoint;
            _spawnCommand.finished = false;
        }
        else
        {
            float reflectSpawnPointXAxis = -1.0f;
            spawnPoint = new Vector3(spawnPoint.x * reflectSpawnPointXAxis,
                spawnPoint.y, spawnPoint.z);


            validObject.transform.position = spawnPoint;
            _spawnCommand.finished = false;
        }

        // Why not... Another Manager. In such a great spot
        EntityMovingManager.Instance.EntityMoveCommander(validObject,
                ((SpawnerCommand)_spawnCommand).GetEntityTag(), spawnPoint);

        if (ReplayManager.Instance.GetIsReplayPlaying() &&
                ReplayManager.Instance.GetIsInReplayMode())
        {
            if (!ReplayManager.Instance.GetIsRewinding())
            {
                ReplayManager.Instance.IncrementCurrentRecordedCommand(CommandType.Spawning);
            }
            else
            {
                ReplayManager.Instance.DecrementCurrentRecordedCommand(CommandType.Spawning);
            }
        }
    }

    // For Setting up the Command... Ya it could be named something else, but eh overload haha
    public void SpawnEntity(string entityTag, Vector3 spawnPoint)
    {
        Command spawnCommand = HandleSpawn(entityTag, spawnPoint);
        ReplayManager.Instance.AddRecordedCommand(CommandType.Spawning, spawnCommand);
        HandleCommand(spawnCommand);
        spawnCommand.Execute();
    }

    private void CheckReplaySpawnPoint(string entityTag, EntityLocationPlacement entityLocation)
    {
        float reflectSpawnPointXAxis = -1.0f;
        if (!ReplayManager.Instance.GetIsRewinding())
        {
            foreach (Transform spawnPoint in entityLocation.entitySpawnPoint)
            {
                SpawnEntity(entityTag, spawnPoint.position);
            }
        }
        else
        {
            foreach (Transform spawnPoint in entityLocation.entitySpawnPoint)
            {
                Vector3 reflectedSpawnPoint = new Vector3(
                    spawnPoint.position.x * reflectSpawnPointXAxis,
                    spawnPoint.position.y, 
                    spawnPoint.position.z);
                SpawnEntity(entityTag, reflectedSpawnPoint);
            }
        }
    }

    public void ResetAllEntities()
    {
        StopAllCoroutines();
        ResetSpawnTimes();

        int countDestroy = 0;

        foreach (string poolObjectKey in objectPool.Keys)
        {
            foreach (GameObject poolObject in objectPool[poolObjectKey])
            {
                countDestroy++;
                poolObject.SetActive(false);
            }
        }
    }

    private void ResetSpawnTimes()
    {
        foreach (EntityLocationPlacement entityLocation in entityLocationPlacements)
        {
            entityLocation.currentSpawnRateTime = entityLocation.maxSpawnRateTime;
        }
    }


    public void SetPoolTagForReplay()
    {
        foreach (string poolObjectKey in objectPool.Keys)
        {
            foreach (GameObject poolObject in objectPool[poolObjectKey])
            {
                // Only During the Replay Functionality are we Calling this,
                // so it's not horrible I suppose, but not the best. The thing here
                // is it proves the concept.
                poolObject.GetComponent<BasePoolObject>().SetPoolReturnTagForReplay();
            }
        }
    }
}
