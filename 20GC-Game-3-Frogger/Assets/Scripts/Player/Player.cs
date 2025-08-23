// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool isHitByKillingEntity = false;
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
        long durationTicks = 0;

        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            durationTicks = (long)Mathf.Max(1, Mathf.Round(playerDuration *
            GameManager.Instance.GetTicksPerSecond()));
        }
        else
        {
            durationTicks = Mathf.Max(1, moveCommand.endTick - moveCommand.startTick);            
        }


        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            moveCommand.endTick = (int)(moveCommand.startTick + durationTicks);
        }
        
        bool isDoneMoving = false;

        // :)
        if (moveCommand != null)
        {
            if (GameManager.Instance.GetCurrentCountDown() != 
                ((MoveCommand)moveCommand).GetCountDownTimeStart())
            {
                GameManager.Instance.SetCurrentCountDown(
                    ((MoveCommand)moveCommand).GetCountDownTimeStart());
            }
        }

        // :)
        if (Treasure.Instance.GetIsOpen())
        {
            if (!((MoveCommand)moveCommand).GetHitTreasure())
            {
                Treasure.Instance.TriggerClose();
            }
        }

        // :)
        if (moveCommand != null)
        {
            if (currentPlayerLives != ((MoveCommand)moveCommand).GetPlayerLives())
            {
                currentPlayerLives = ((MoveCommand)moveCommand).GetPlayerLives();
                UIManager.Instance.TakeALifeAway(currentPlayerLives);
            }
        }


        while (!isDoneMoving)
        {
            yield return new WaitUntil(() => ReplayManager.Instance.GetIsReplayPlaying());

            if (!HitBlock(transform.position, 
                VectorConversions.ToUnity(((MoveCommand)moveCommand).GetDirection())))
            {
                float moveProgress = moveProgress = (GameManager.Instance.GetGlobalTick() - moveCommand.startTick) / (float)durationTicks;

                transform.position = Vector3.Lerp(
                                VectorConversions.ToUnity(((MoveCommand)moveCommand).GetStartPosition()),
                                VectorConversions.ToUnity(((MoveCommand)moveCommand).GetEndPosition()),
                            moveProgress);

                moveProgress = Mathf.Clamp01(moveProgress);

                if ((!ReplayManager.Instance.GetIsRewinding() && GameManager.Instance.GetGlobalTick() >= moveCommand.endTick) ||
                    ReplayManager.Instance.GetIsRewinding() && GameManager.Instance.GetGlobalTick() <= moveCommand.startTick)
                {
                    isDoneMoving = true;
                }

                yield return new WaitForFixedUpdate();
            }
            else
            {
                hitBlockReaction = true;
                break;
            }
        }


        // If we hit a wall
        if (ReplayManager.Instance.GetIsInReplayMode())
        {
            if (!isDoneMoving)
            {
                moveCommand.endTick += (int)durationTicks;
            }
        }
        

        while (!isDoneMoving)
        {
            yield return new WaitUntil(() => ReplayManager.Instance.GetIsReplayPlaying());

            float moveProgress = (GameManager.Instance.GetGlobalTick() - moveCommand.startTick)
                        / (float)durationTicks;

            moveProgress = Mathf.Clamp01(moveProgress);

            transform.position = Vector3.Lerp(
                        transform.position,
                        VectorConversions.ToUnity(((MoveCommand)moveCommand).GetStartPosition()),
                    moveProgress);

            if ((!ReplayManager.Instance.GetIsRewinding() && GameManager.Instance.GetGlobalTick() >= moveCommand.endTick) ||
                    ReplayManager.Instance.GetIsRewinding() && GameManager.Instance.GetGlobalTick() <= moveCommand.startTick)
            {
                isDoneMoving = true;
            }

            yield return new WaitForFixedUpdate();
        }

        if (hitBlockReaction)
        {
            hitBlockReaction = false;
            SetPlayerLocation(VectorConversions.ToUnity(((MoveCommand)moveCommand).GetStartPosition()));
        }
        else
        {
            if (!ReplayManager.Instance.GetIsRewinding())
            {
                SetPlayerLocation(VectorConversions.ToUnity(((MoveCommand)moveCommand).GetEndPosition()));
            }
            else
            {
                SetPlayerLocation(VectorConversions.ToUnity(((MoveCommand)moveCommand).GetStartPosition()));
            }

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
                if (ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.PlayerMoving) <
                    ReplayManager.Instance.GetRecordedCommands(CommandType.PlayerMoving).Count)
                {
                    ReplayManager.Instance.IncrementCurrentRecordedCommand(CommandType.PlayerMoving);
                }
                
            }
            else
            {
                if (ReplayManager.Instance.GetCurrentRecordedCommand(CommandType.PlayerMoving) > 0)
                {
                    ReplayManager.Instance.DecrementCurrentRecordedCommand(CommandType.PlayerMoving);
                }   
            }
        }

        moveCommand.finished = false;
        inMiddleOfMoveCommand = false;
        playerMoving = null;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "KillingEntity")
        {
            if (!isHitByKillingEntity)
            {
                isHitByKillingEntity = true;
                PlayerDie();
            }  
        }
    }

    public void PlayerDie()
    {
        if (currentPlayerLives >= 1)
        {
            if (!ReplayManager.Instance.GetIsInReplayMode())
            {
                if (moveCommand != null)
                {
                    ((MoveCommand)moveCommand).SetPlayerLives(currentPlayerLives);
                }
            }

            currentPlayerLives--;
            UIManager.Instance.TakeALifeAway(currentPlayerLives);

            // Doing this since the death animation would have to 
            // be added as a command other wise, which could be done
            // but for the purposes of this game it's cool
            if (!ReplayManager.Instance.GetIsRewinding())
            {
                boxCollider2D.enabled = false;
                isDead = true;
                SoundManager.Instance.PlayDieSound();
                SetTriggerDeath(); 
            }
            else
            {
                if (isHitByKillingEntity)
                {
                    isHitByKillingEntity = false;
                }
            }
            
        }
    }


    public void OnDeath()
    {
        if (currentPlayerLives >= 1)
        {
            isDead = false;
            transform.position = playerStartingLocation;
            GameManager.Instance.ResetCurrentCountDown();
            boxCollider2D.enabled = true;
            isHitByKillingEntity = false;
            SetTriggerIdle();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
       
    }

    public void RefreshPlayersLifeState()
    {
        isDead = false;
        isHitByKillingEntity = false;
        currentPlayerLives = maxPlayerLives;
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
                if (hit.collider.gameObject.tag == "Treasure" &&
                    !Treasure.Instance.GetIsOpen())
                {
                    if (moveCommand != null)
                    {
                        ((MoveCommand)moveCommand).SetHitTreasure(true);
                    }
                    SoundManager.Instance.PlayWinSound();
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

    public void ResetToStartingLocation()
    {
        transform.position = playerStartingLocation;
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

    public int GetCurrentPlayerLives()
    {
        return currentPlayerLives;
    }

    public void SetCurrentPlayerlives(int _currentLives)
    {
        currentPlayerLives = _currentLives;
    }

    public int GetMaxPlayerLives()
    {
        return maxPlayerLives;
    }

    public void StopCoroutineOnReset()
    {
        StopAllCoroutines();
        playerMoving = null;
        moveCommand = null;

        foreach (Command command in ReplayManager.Instance.GetRecordedCommands(CommandType.PlayerMoving))
        {
            command.finished = false;
        }
    }

    // Animation
    public void SetTriggerIdle()
    {
        animator.SetTrigger("Idle");
    }

    public void SetTriggerWalk()
    {
        animator.SetTrigger("Walk");
    }

    private void SetTriggerDeath()
    {
        animator.SetTrigger("Death");
    }
}
