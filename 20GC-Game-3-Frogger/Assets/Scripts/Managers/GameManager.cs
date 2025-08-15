// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player player;
    [SerializeField] private Treasure treasure;
    private float gameTimer = 0;
    [SerializeField] private int maxCountDownTimer = 80;
    private float countingCountDownSeconds = 0.0f;
    private int currentCountDown;

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
        currentCountDown = maxCountDownTimer;
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
        UpdateCountDownUI();
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

    private void UpdateCountDownUI()
    {
        if (currentCountDown > 0)
        {
            countingCountDownSeconds += Time.deltaTime;

            if (countingCountDownSeconds >= 1.0f)
            {
                currentCountDown--;
                UIManager.Instance.UpdateCountDownTimer(currentCountDown);
                countingCountDownSeconds = 0.0f;
            }
        }
        else if (!player.GetIsDead())
        {
            player.PlayerDie();
        }
    }

    public int GetCurrentCountDown()
    {
        return currentCountDown;
    }

    public void ResetCurrentCountDown()
    {
        currentCountDown = maxCountDownTimer;
    }
}
