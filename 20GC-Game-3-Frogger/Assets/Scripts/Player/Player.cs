// Game and Code By RvRproduct (Roberto Valentino Reynoso)
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

    private void Awake()
    {
        playerLocation = transform.position;
        playerStartingLocation = transform.position;
        animator = GetComponent<Animator>();
    }
    public void MovePlayer(float _playerX, float _playerY, Command command)
    {
        if (playerMoving == null && gameObject.activeInHierarchy)
        {
            SetTriggerWalk();
            playerMoving = StartCoroutine(PlayerMoving(new Vector3(_playerX, _playerY, 0.0f), command));
        }
    }

    private IEnumerator PlayerMoving(Vector3 _newPlayerLocation, Command command)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < playerDuration)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / playerDuration;
            transform.position = Vector3.Lerp(
                transform.position,
                _newPlayerLocation,
                time);
            yield return null;
        }

        SetPlayerLocation(_newPlayerLocation);
        SetTriggerIdle();
        if (ReplayManager.Instance.GetIsPlayingRecording())
        {
            ReplayManager.Instance.IncrementCurrentRecordedCommand();
        }
        command.finished = false;
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
