// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Coroutine playerMoving;
    [SerializeField] private float playerDuration = 0.15f;
    Vector3 playerLocation;

    private void Awake()
    {
        playerLocation = transform.position;
    }
    public void MovePlayer(float _playerX, float _playerY)
    {
        if (playerMoving == null)
        {
            playerMoving = StartCoroutine(PlayerMoving(new Vector3(_playerX, _playerY, 0.0f)));
        }
    }

    private IEnumerator PlayerMoving(Vector3 _newPlayerLocation)
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
}
