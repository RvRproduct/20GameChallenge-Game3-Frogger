using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player player;

    private float gameTimer = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
    }

    public float GetGameTimer()
    {
        return gameTimer;
    }

    public void SetPlayerToStartingLocation()
    {
        player.transform.position = player.GetPlayerStartingLocation();
    }
}
