// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using UnityEngine;

public class Entity : BasePoolObject
{

    [SerializeField] private bool isGoingLeft = false;
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private EntityTypes entityType;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer)
        {
            if (isGoingLeft)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

        }
    }

    protected override string ProvidePoolReturnTag()
    {
        if (isGoingLeft)
        {
            if (!ReplayManager.Instance.GetIsRewinding())
            {
                return PoolTags.EntityReturnTags.EntityLeftReturn;
            }
            else
            {
                return PoolTags.EntityReturnTags.EntityRightReturn;  
            }
            
        }
        else
        {
            if (!ReplayManager.Instance.GetIsRewinding())
            {
                return PoolTags.EntityReturnTags.EntityRightReturn;
            }
            else
            {
                return PoolTags.EntityReturnTags.EntityLeftReturn; 
            }     
        }      
    }

    protected override string ProvidePoolTag()
    {
        return EntitySelection();
    }

    private void FixedUpdate()
    {
        CheckForReplayBeforeMove();
    }

    private void CheckForReplayBeforeMove()
    {
        if (ReplayManager.Instance.GetIsReplayPlaying() &&
            !ReplayManager.Instance.GetIsAtEndReplay())
        {
            if (isGoingLeft)
            {
                if (!ReplayManager.Instance.GetIsRewinding())
                {
                    transform.position += Vector3.left * speed * Time.fixedDeltaTime;
                }
                else
                {
                    transform.position += Vector3.right * speed * Time.fixedDeltaTime;
                }

            }
            else
            {
                if (!ReplayManager.Instance.GetIsRewinding())
                {
                    transform.position += Vector3.right * speed * Time.fixedDeltaTime;
                }
                else
                {
                    transform.position += Vector3.left * speed * Time.fixedDeltaTime;
                }
            }
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
