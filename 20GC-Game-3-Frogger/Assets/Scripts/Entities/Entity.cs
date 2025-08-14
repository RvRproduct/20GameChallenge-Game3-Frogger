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
            case EntityTypes.Bat:
                return PoolTags.EntityTags.BatEntity;
            case EntityTypes.Skeleton:
                return PoolTags.EntityTags.SkeletonEntity;
            case EntityTypes.SlimeB:
                return PoolTags.EntityTags.SlimeBEntity;
            case EntityTypes.SlimeG:
                return PoolTags.EntityTags.SlimeGEntity;
            case EntityTypes.SlimeR:
                return PoolTags.EntityTags.SlimeREntity;
            default:
                return PoolTags.EntityTags.BatEntity;

        }
    }
}
