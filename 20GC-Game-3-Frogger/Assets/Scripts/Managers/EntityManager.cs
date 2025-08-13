using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum EntityTypes
{
    SlowCar,
    FastCar,
    SportsCar,
    Tractor,
    Truck,
    Croc,
    Turtle,
    Log
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
        currentSpawnRate = Time.deltaTime;

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
                case EntityTypes.SlowCar:
                    SpawnEntity(PoolTags.EntityTags.SlowCarEntity, 
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.FastCar:
                    SpawnEntity(PoolTags.EntityTags.FastCarEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.SportsCar:
                    SpawnEntity(PoolTags.EntityTags.SportsCarEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.Tractor:
                    SpawnEntity(PoolTags.EntityTags.TractorEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.Truck:
                    SpawnEntity(PoolTags.EntityTags.TruckEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.Croc:
                    SpawnEntity(PoolTags.EntityTags.CrocEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.Turtle:
                    SpawnEntity(PoolTags.EntityTags.TurtleEntity,
                        entityLocation.entitySpawnPoint.position);
                    break;
                case EntityTypes.Log:
                    SpawnEntity(PoolTags.EntityTags.LogEntity,
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
