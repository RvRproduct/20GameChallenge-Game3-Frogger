// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using PoolTags;
using System;
using System.Collections;
using UnityEngine;

public class Entity : BasePoolObject
{

    [SerializeField] private bool isGoingLeft = false;
    [SerializeField] private float entitySpeed = 2.0f;
    [SerializeField] private EntityTypes entityType;
    private SpriteRenderer spriteRenderer;
    private Coroutine entityMoving;
    // For Commands
    private Command entityMoveCommand = null;
    private int entityIndex = -1;
    private bool isDoneMoving = false;

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

    public void MoveEntity(Command command)
    {
        if (entityMoving == null && gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            entityMoveCommand = (EntityMoveCommand)command;
            
            entityMoving = StartCoroutine(EntityMoving());  
        }
    }

    private IEnumerator EntityMoving()
    {
        long durationTicks = 0;
        isDoneMoving = false;

        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            float distance = Vector2.Distance(VectorConversions.ToUnity
                (((EntityMoveCommand)entityMoveCommand).GetStartPosition()),
                VectorConversions.ToUnity
                (((EntityMoveCommand)entityMoveCommand).GetEndPosition()));

            durationTicks = (long)Mathf.Max(1, Mathf.Round(distance *
                GameManager.Instance.GetTicksPerSecond() / entitySpeed));
        }
        else
        {
            durationTicks = Mathf.Max(1, entityMoveCommand.endTick - entityMoveCommand.startTick);
        }

        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            entityMoveCommand.endTick = (int)(entityMoveCommand.startTick + durationTicks);
        }

        while (!isDoneMoving)
        {
            float moveProgress = 0.0f;

            if (!ReplayManager.Instance.GetIsRewinding())
            {
                moveProgress = (GameManager.Instance.GetGlobalTick() - entityMoveCommand.startTick)
                    / (float)durationTicks;
            }
            else
            {
                moveProgress = ((GameManager.Instance.GetGlobalTick() - entityMoveCommand.endTick)
                    / (float)durationTicks) * -1;
            }

            moveProgress = Mathf.Clamp01(moveProgress);

            if (!ReplayManager.Instance.GetIsRewinding())
            {
                transform.position = Vector3.Lerp(
                 VectorConversions.ToUnity(((EntityMoveCommand)entityMoveCommand).GetStartPosition()),
                 VectorConversions.ToUnity(((EntityMoveCommand)entityMoveCommand).GetEndPosition()), 
                 moveProgress);

                if (moveProgress >= 1.0f) { isDoneMoving = true; }
            }
            else
            {
                transform.position = Vector3.Lerp(
                 VectorConversions.ToUnity(((EntityMoveCommand)entityMoveCommand).GetEndPosition())
                 ,VectorConversions.ToUnity(((EntityMoveCommand)entityMoveCommand).GetStartPosition()),
                 moveProgress);
                if (moveProgress >= 1.0f) { isDoneMoving = true; }
            }
            yield return new WaitForFixedUpdate();
        }

        SetEntityPosition(VectorConversions.ToUnity(((EntityMoveCommand)entityMoveCommand).GetStartPosition()),
            VectorConversions.ToUnity(((EntityMoveCommand)entityMoveCommand).GetEndPosition()));
  
        CleanUpEntityReplayMode();     
    }

    private void SetEntityPosition(Vector2 _startPosition, Vector2 _endPosition)
    {
        if (!ReplayManager.Instance.GetIsRewinding())
        {
            transform.position = _endPosition;
        }
        else
        {
            transform.position = _startPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Getting Rid of the base trigger event
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

    public void SetEntityIndex(int _entityIndex)
    {
        entityIndex = _entityIndex;
    }

    public int GetEntityIndex()
    {
        return entityIndex;
    }

    public EntityTypes GetEntityType()
    {
        return entityType;
    }


    public void SetIsDoneMoving(bool _isDoneMoving)
    {
        isDoneMoving = _isDoneMoving;
    }

    public bool GetIsDoneMoving()
    {
        return isDoneMoving;
    }

    public Coroutine GetEntityMoving()
    {
        return entityMoving;
    }

    private void CleanUpEntityReplayMode()
    {
        entityMoveCommand.finished = false;
        ((EntityMoveCommand)entityMoveCommand).SetEntity(null);
        entityMoving = null;
        gameObject.SetActive(false);
    }
}
