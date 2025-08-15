// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Xml.Serialization;

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
    }

    private void Start()
    {
        SetUpObjectPool();
        SpawnEntitiesInWorld();
    }

    private void Update()
    {
        SpawnTimer();
    }

    private void SpawnTimer()
    {
        if (ReplayManager.Instance.GetIsReplayPlaying())
        {
            SpawnEntitiesInWorld();
        }
    }

    private void SpawnEntitiesInWorld()
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
        foreach (string poolObjectKey in objectPool.Keys)
        {
            foreach (GameObject poolObject in objectPool[poolObjectKey])
            {
                poolObject.SetActive(false);
            }
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
