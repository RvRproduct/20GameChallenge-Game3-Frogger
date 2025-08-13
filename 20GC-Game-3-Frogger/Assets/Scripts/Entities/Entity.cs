using System.Collections.Generic;
using UnityEngine;

public class Entity : BasePoolObject
{

    [SerializeField] private bool isGoingLeft = false;
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private EntityTypes entityType;

    protected override string ProvidePoolReturnTag()
    {
        return PoolTags.EntityReturnTags.EntityReturn;
    }

    protected override string ProvidePoolTag()
    {
        return EntitySelection();
    }

    private void Update()
    {
        if (isGoingLeft)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }

    private string EntitySelection()
    {
        switch (entityType)
        {
            case EntityTypes.SlowCar:
                return PoolTags.EntityTags.SlowCarEntity;
            case EntityTypes.FastCar:
                return PoolTags.EntityTags.FastCarEntity;
            case EntityTypes.SportsCar:
                return PoolTags.EntityTags.SportsCarEntity;
            case EntityTypes.Tractor:
                return PoolTags.EntityTags.TractorEntity;
            case EntityTypes.Truck:
                return PoolTags.EntityTags.TruckEntity;
            case EntityTypes.Croc:
                return PoolTags.EntityTags.CrocEntity;
            case EntityTypes.Turtle:
                return PoolTags.EntityTags.TurtleEntity;
            case EntityTypes.Log:
                return PoolTags.EntityTags.LogEntity;
            default:
                return PoolTags.EntityTags.SlowCarEntity;

        }
    }
}
