// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor.Build;

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

    private void Awake()
    {
        playerLocation = transform.position;
        playerStartingLocation = transform.position;
        animator = GetComponent<Animator>();
    }
    public void MovePlayer(float _playerX, float _playerY, float directionX, float directionY, Command command)
    {
        if (playerMoving == null && gameObject.activeInHierarchy)
        {
            SetTriggerWalk();
            playerMoving = StartCoroutine(PlayerMoving(new Vector3(_playerX, _playerY, 0.0f),
                new Vector2(directionX, directionY),command));
        }
    }

    private IEnumerator PlayerMoving(Vector3 _newPlayerLocation, 
        Vector2 playerDirection, Command command)
    {
        float elapsedTime = 0.0f;
        Vector3 previousPosition = transform.position;
        bool hitBlockReaction = false;

        while (elapsedTime < playerDuration)
        {
            if (!HitBlock(transform.position, playerDirection))
            {
                elapsedTime += Time.deltaTime;
                float time = elapsedTime / playerDuration;
                transform.position = Vector3.Lerp(
                    transform.position,
                    _newPlayerLocation,
                    time);
                yield return null;
            }
            else
            {
                hitBlockReaction = true;
                elapsedTime = 0.0f;
                break;
            }     
        }

        while (elapsedTime < playerDuration)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / playerDuration;
            transform.position = Vector3.Lerp(
                transform.position,
                previousPosition,
                time);
            yield return null;
        }

        if (hitBlockReaction)
        {
            hitBlockReaction = false;
            SetPlayerLocation(previousPosition);
        }
        else
        {
            SetPlayerLocation(_newPlayerLocation);
        }
   
        SetTriggerIdle();
        if (ReplayManager.Instance.GetIsReplayPlaying())
        {
            if (!ReplayManager.Instance.GetIsRewinding())
            {
                ReplayManager.Instance.IncrementCurrentRecordedCommand();
            }
            else
            {
                ReplayManager.Instance.DecrementCurrentRecordedCommand();
            }
        }
        command.finished = false;
        inMiddleOfMoveCommand = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.tag == "KillingEntity")
        //{
        //    //gameObject.SetActive(false);
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}
    }


    public void OnDeath()
    {

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
                    Debug.Log("IHASJofha");
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
}
