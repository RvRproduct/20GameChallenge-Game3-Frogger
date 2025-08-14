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
        public Transform entitySpawnPoint;
    }

    [SerializeField] private List<EntityLocationPlacement> entityLocationPlacements;
    [SerializeField] private float maxSpawnRateTime = 2.0f;
    private float currentSpawnRate = 0.0f;

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
        currentSpawnRate += Time.deltaTime;

        if (currentSpawnRate >= maxSpawnRateTime)
        {
            SpawnEntitiesInWorld();
            currentSpawnRate = 0;
        }
    }

    private void SpawnEntitiesInWorld()
    {
        foreach (EntityLocationPlacement entityLocation in entityLocationPlacements)
        {
            switch (entityLocation.entityType)
            {
                case EntityTypes.Bat:
                    SpawnEntity(PoolTags.EntityTags.BatEntity, 
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.Skeleton:
                    SpawnEntity(PoolTags.EntityTags.SkeletonEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.SlimeB:
                    SpawnEntity(PoolTags.EntityTags.SlimeBEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.SlimeG:
                    SpawnEntity(PoolTags.EntityTags.SlimeGEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.SlimeR:
                    SpawnEntity(PoolTags.EntityTags.SlimeREntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
            }
        }
    }

    private void SpawnEntity(string entityTag, Vector3 spawnPoint)
    {
        GameObject validObject = GetValidObjectInPool(entityTag);
        validObject.transform.position = spawnPoint;
    }
}
