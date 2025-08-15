using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player player;
    [SerializeField] private Treasure treasure;

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

    public Player GetPlayer()
    {
        return player;
    }

    public void SetPlayerStartingLocation(Vector3 _startingLocation)
    {
        player.transform.position = _startingLocation;
    }

    public Treasure GetTreasure()
    {
        return treasure;
    }
}
