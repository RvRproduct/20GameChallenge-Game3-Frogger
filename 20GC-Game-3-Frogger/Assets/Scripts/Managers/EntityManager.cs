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
        SpawnEntitiesInWorld();
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
                        foreach (Transform spawnPoint in entityLocation.entitySpawnPoint)
                        {
                            SpawnEntity(PoolTags.EntityTags.BatEntity,
                                spawnPoint.position);
                        }
                    }
                    break;
                case EntityTypes.Skeleton:
                    if (CanSpawnEntity(entityLocation))
                    {
                        foreach (Transform spawnPoint in entityLocation.entitySpawnPoint)
                        {
                            SpawnEntity(PoolTags.EntityTags.SkeletonEntity,
                                spawnPoint.position);
                        }
                    }
                    break;
                case EntityTypes.SlimeB:
                    if (CanSpawnEntity(entityLocation))
                    {
                        foreach (Transform spawnPoint in entityLocation.entitySpawnPoint)
                        {
                            SpawnEntity(PoolTags.EntityTags.SlimeBEntity,
                                spawnPoint.position);
                        }
                    }
                    break;
                case EntityTypes.SlimeG:
                    if (CanSpawnEntity(entityLocation))
                    {
                        foreach (Transform spawnPoint in entityLocation.entitySpawnPoint)
                        {
                            SpawnEntity(PoolTags.EntityTags.SlimeGEntity,
                                spawnPoint.position);
                        }
                    }
                    break;
                case EntityTypes.SlimeR:
                    if (CanSpawnEntity(entityLocation))
                    {
                        foreach (Transform spawnPoint in entityLocation.entitySpawnPoint)
                        {
                            SpawnEntity(PoolTags.EntityTags.SlimeREntity,
                                spawnPoint.position);
                        }
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
}
