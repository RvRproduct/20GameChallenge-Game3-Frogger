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
    private Command spawn;
    private float playBackTimer;
    private Coroutine spawning;
    private bool isReplayPlaying = false;
    private int currentRecordedCommand = 0;

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
        spawn = null;

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

    private void Update()
    {
        if (spawning == null &&
            !ReplayManager.Instance.GetIsInReplayMode())
        {
            SpawnTimer();
        } 
    }

    private void SpawnTimer()
    {
        Command command = HandleSpawn();
        ReplayManager.Instance.AddRecordedSpawningCommand(command);
        HandleCommand(command);

    }

    private void HandleCommand(Command _spawn)
    {
        _spawn.Execute();
    }

    private Command HandleSpawn()
    {
        spawn = null;
        return spawn = new SpawnerCommand(this,
            ReplayManager.Instance,
            GameManager.Instance.GetGameTimer(),
            true);
    }

    public void StartReplay()
    {
        isReplayPlaying = true;
        StopAllCoroutines();
        StartCoroutine(ReplayManager.Instance.PlayRecordedCommands(
            currentRecordedCommand, ReplayManager.Instance.GetRecordedSpawningCommands(),
            playBackTimer, isReplayPlaying, ReplayManager.Instance.GetIsRewinding()));
    }

    private IEnumerator SpawnEntitiesInWorld()
    {
        foreach (EntityLocationPlacement entityLocation in entityLocationPlacements)
        {
            entityLocation.currentSpawnRateTime += Time.deltaTime;

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

    private void SpawnEntity(string entityTag, Vector3 spawnPoint)
    {
        GameObject validObject = GetValidObjectInPool(entityTag);
        validObject.transform.position = spawnPoint;
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
        ResetSpawnTimes();

        foreach (string poolObjectKey in objectPool.Keys)
        {
            foreach (GameObject poolObject in objectPool[poolObjectKey])
            {
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

    public void IncrementCurrentRecordedCommand()
    {
        currentRecordedCommand++;
    }

    public void DecrementCurrentRecordedCommand()
    {
        currentRecordedCommand--;
    }
}
