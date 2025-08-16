// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    private Coroutine playerMoving;
    [SerializeField] private float playerDuration = 0.05f;
    private Vector3 playerLocation;
    private Vector3 playerStartingLocation;
    private Animator animator;
    [SerializeField] private float rayDistance = 5;
    [SerializeField] LayerMask blockLayer;
    private bool inMiddleOfMoveCommand = false;
    private BoxCollider2D boxCollider2D;
    private int maxPlayerLives = 3;
    private int currentPlayerLives;
    private bool isDead = false;
    Command moveCommand = null;

    private void Awake()
    {
        playerLocation = transform.position;
        playerStartingLocation = transform.position;
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        currentPlayerLives = maxPlayerLives;
    }
    public void MovePlayer(Command command)
    {
        if (playerMoving == null && gameObject.activeInHierarchy && !isDead)
        {
            moveCommand = (MoveCommand)command;
            SetTriggerWalk();
            playerMoving = StartCoroutine(PlayerMoving());
        }
    }

    private IEnumerator PlayerMoving()
    {
        bool hitBlockReaction = false;
        long durationTicks = (long)Mathf.Max(1, Mathf.Round(playerDuration *
            GameManager.Instance.GetTicksPerSecond()));
        moveCommand.endTick = (int)(moveCommand.startTick + durationTicks);
        bool isDoneMoving = false;

        while (!isDoneMoving)
        {
            if (!HitBlock(transform.position, 
                Vector2Conversions.ToUnity(((MoveCommand)moveCommand).GetDirection())))
            {
                float rawMoveProgress = (GameManager.Instance.GetGlobalTick() - moveCommand.startTick)
                / (float)(moveCommand.endTick - moveCommand.startTick);

                float moveProgress = Mathf.Clamp01(rawMoveProgress);

                transform.position = Vector3.Lerp(
                     Vector2Conversions.ToUnity(((MoveCommand)moveCommand).GetStartPosition()),
                     Vector2Conversions.ToUnity(((MoveCommand)moveCommand).GetEndPosition()),
                    moveProgress);

                if (GameManager.Instance.GetReplayDirection() == ReplayDirection.Forward)
                {
                    if (rawMoveProgress >= 1.0f) { isDoneMoving = true; }
                }
                else if (GameManager.Instance.GetReplayDirection() == ReplayDirection.Rewind)
                {
                    if (rawMoveProgress <= 0.0f) { isDoneMoving = true; }
                }
                yield return null;
            }
            else
            {
                hitBlockReaction = true;
                break;
            }     
        }

        // If we hit a wall
        if (!isDoneMoving)
        {
            moveCommand.endTick += (int)durationTicks;
        }

        while (!isDoneMoving)
        {
            float rawMoveProgress = (GameManager.Instance.GetGlobalTick() - moveCommand.startTick)
                / (float)(moveCommand.endTick - moveCommand.startTick);

            float moveProgress = Mathf.Clamp01(rawMoveProgress);

            transform.position = Vector3.Lerp(
                transform.position,
                Vector2Conversions.ToUnity(((MoveCommand)moveCommand).GetStartPosition()),
                moveProgress);

            if (GameManager.Instance.GetReplayDirection() == ReplayDirection.Forward)
            {
                if (rawMoveProgress >= 1.0f) { isDoneMoving = true; }
            }
            else if (GameManager.Instance.GetReplayDirection() == ReplayDirection.Rewind)
            {
                if (rawMoveProgress <= 0.0f) { isDoneMoving = true; }
            }

            yield return null;
        }

        if (hitBlockReaction)
        {
            hitBlockReaction = false;
            SetPlayerLocation(Vector2Conversions.ToUnity(((MoveCommand)moveCommand).GetStartPosition()));
        }
        else
        {
            SetPlayerLocation(Vector2Conversions.ToUnity(((MoveCommand)moveCommand).GetEndPosition()));
        }
        if (!isDead)
        {
            SetTriggerIdle();
        }
        
        if (ReplayManager.Instance.GetIsReplayPlaying() &&
            ReplayManager.Instance.GetIsInReplayMode())
        {
            if (!ReplayManager.Instance.GetIsRewinding())
            {
                ReplayManager.Instance.IncrementCurrentRecordedCommand(CommandType.PlayerMoving);
            }
            else
            {
                ReplayManager.Instance.DecrementCurrentRecordedCommand(CommandType.PlayerMoving);
            }
        }

        moveCommand.finished = false;
        inMiddleOfMoveCommand = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "KillingEntity")
        {
            PlayerDie();
        }
    }

    public void PlayerDie()
    {
        if (currentPlayerLives >= 1)
        {
            boxCollider2D.enabled = false;
            isDead = true;
            UIManager.Instance.TakeALifeAway();
            SetTriggerDeath();
        }
    }


    public void OnDeath()
    {
        currentPlayerLives--;

        if (currentPlayerLives >= 1)
        {
            isDead = false;
            transform.position = playerStartingLocation;
            GameManager.Instance.ResetCurrentCountDown();
            boxCollider2D.enabled = true;
            SetTriggerIdle();
        }
       
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    private bool HitBlock(Vector2 origin ,Vector2 direction)
    {

        Debug.DrawRay(origin, direction * rayDistance, Color.red);
        List<Vector2> origins = new List<Vector2>();
        if (direction.x == 0)
        {
            origins.Add(origin);
            origins.Add(new Vector2(origin.x + 0.25f, origin.y));
            origins.Add(new Vector2(origin.x - 0.25f, origin.y));
        }
        else
        {
            origins.Add(origin);
            origins.Add(new Vector2(origin.x, origin.y + 0.25f));
            origins.Add(new Vector2(origin.x, origin.y - 0.25f));
        }

        foreach (Vector2 _origin in origins)
        {
            RaycastHit2D hit = Physics2D.Raycast(_origin, direction, rayDistance, blockLayer);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Treasure")
                {
                    GameManager.Instance.GetTreasure().TriggerOpen();
                }

                return true;
            }
        }
        return false;
    }

    // Getters and Setters
    private Vector2 GetPlayerLocation()
    {
        return playerLocation;
    }

    private void SetPlayerLocation(Vector2 _newPlayerLocation)
    {
        playerLocation = _newPlayerLocation;
        transform.position = playerLocation;
        playerMoving = null;
    }

    public Vector3 GetPlayerStartingLocation()
    {
        return playerStartingLocation;
    }

    public void SetInMiddleOfMoveCommand(bool _inMiddleOfMoveCommand)
    {
        inMiddleOfMoveCommand = _inMiddleOfMoveCommand;
    }

    public bool GetInMiddleOfMoveCommand()
    {
        return inMiddleOfMoveCommand;
    }

    // Animation
    private void SetTriggerIdle()
    {
        animator.SetTrigger("Idle");
    }

    private void SetTriggerWalk()
    {
        animator.SetTrigger("Walk");
    }

    private void SetTriggerDeath()
    {
        animator.SetTrigger("Death");
    }
}
