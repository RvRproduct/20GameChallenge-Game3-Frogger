// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using UnityEngine;

public enum ReplayDirection
{
    Rewind = -1,
    Pause = 0,
    Forward = 1,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player player;
    [SerializeField] private Treasure treasure;
    private const int TICKS_PER_SECOND = 60;
    private int globalTick = 0;
    [SerializeField] private int maxCountDownTimer = 80;
    private float countingCountDownSeconds = 0.0f;
    private int currentCountDown;
    private ReplayDirection replayDirection = ReplayDirection.Forward;

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

        Time.fixedDeltaTime = 1.0f / 60.0f;
    }

    private void FixedUpdate()
    {
        switch (replayDirection)
        {
            case ReplayDirection.Forward:
                globalTick++;

                if (ReplayManager.Instance.GetIsInReplayMode() &&
                    !ReplayManager.Instance.GetIsAtEndReplay())
                {
                    if (globalTick >= ReplayManager.Instance.GetEndReplayTick())
                    {
                        ReplayManager.Instance.SetIsAtEndReplay(true);
                        EntityManager.Instance.ResetAllEntities();
                        ReplayManager.Instance.NullAllEntitiesToCommands(false);
                    }
                }
                break;
            case ReplayDirection.Pause:
                break;
            case ReplayDirection.Rewind:
                if (globalTick > 0)
                {
                    globalTick--;

                    if (ReplayManager.Instance.GetIsInReplayMode() &&
                        !ReplayManager.Instance.GetIsAtEndReplay())
                    {
                        if (globalTick <= 0)
                        {
                            ReplayManager.Instance.SetIsAtEndReplay(true);
                            EntityManager.Instance.ResetAllEntities();
                            ReplayManager.Instance.NullAllEntitiesToCommands(false);
                        }
                    }
                }
                break;
        }

        if (replayDirection != ReplayDirection.Pause)
        {
            UpdateCountDownUI();
        }
    }

    public int GetGlobalTick()
    {
        return globalTick;
    }

    public void SetGlobalTick(int _globalTick)
    {
        globalTick = _globalTick;
    }

    public int GetTicksPerSecond()
    {
        return TICKS_PER_SECOND;
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
            countingCountDownSeconds += Time.fixedDeltaTime;

            if (countingCountDownSeconds >= 1.0f)
            {
                if (!ReplayManager.Instance.GetIsRewinding())
                {
                    currentCountDown--;
                    UIManager.Instance.UpdateCountDownTimer(currentCountDown);
                    countingCountDownSeconds = 0.0f;
                }
                else
                {
                    if (currentCountDown < maxCountDownTimer)
                    {
                        currentCountDown++;
                        UIManager.Instance.UpdateCountDownTimer(currentCountDown);
                        countingCountDownSeconds = 0.0f;
                    }
                }
                
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

    public void SetCurrentCountDown(int _currentCountDown)
    {

    }

    public void ResetCurrentCountDown()
    {
        currentCountDown = maxCountDownTimer;
    }

    public ReplayDirection GetReplayDirection()
    {
        return replayDirection;
    }

    public void SetReplayDirection(ReplayDirection _replayDirection)
    {
        replayDirection = _replayDirection;
    }
}
