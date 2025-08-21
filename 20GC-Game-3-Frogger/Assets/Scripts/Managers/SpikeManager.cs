// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeManager : MonoBehaviour
{
    public static SpikeManager Instance;

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

    [Header("Spikes")]
    [SerializeField] private List<Spike> spikeFriendsOne;
    [SerializeField] private List<Spike> spikeFriendsTwo;
    [SerializeField] private List<Spike> spikeHaters;

    [Header("Spike Friend Timing")]
    [SerializeField] private float timeBetweenSpikeFriends = 3.0f;
    private float currentTimeBetweenSpikeFriends = 0.0f;
    private bool initialOtherSpikeFriend = false;
    private bool activateOtherSpikeFriend = false;
    //private Coroutine spikeCoroutine = null;


    private void FixedUpdate()
    {
        if (ReplayManager.Instance.GetIsReplayPlaying() &&
            !ReplayManager.Instance.GetIsInReplayMode())
        {
            ActivateSpikes();
        }
    }

    public void StartReplayForAllSpikes()
    {
        foreach (Spike spike in spikeFriendsOne)
        {
            spike.StartSpikeReplay();
        }

        foreach (Spike spike in spikeFriendsTwo)
        {
            spike.StartSpikeReplay();
        }

        foreach (Spike spike in spikeHaters)
        {
            spike.StartSpikeReplay();
        }
    }

    public void PauseAllSpikes()
    {
        foreach (Spike spike in spikeFriendsOne)
        {
            spike.PauseAnimator();
        }

        foreach (Spike spike in spikeFriendsTwo)
        {
            spike.PauseAnimator();
        }

        foreach (Spike spike in spikeHaters)
        {
            spike.PauseAnimator();
        }
    }

    public void ReverseAllSpikes(bool _reset = false)
    {
        foreach (Spike spike in spikeFriendsOne)
        {
            spike.ReverseAnimator(_reset);
        }

        foreach (Spike spike in spikeFriendsTwo)
        {
            spike.ReverseAnimator(_reset);
        }

        foreach (Spike spike in spikeHaters)
        {
            spike.ReverseAnimator(_reset);
        }
    }

    public void ForwardAllSpikes(bool _reset = false)
    {
        foreach (Spike spike in spikeFriendsOne)
        {
            spike.ForwardAnimator(_reset);
        }

        foreach (Spike spike in spikeFriendsTwo)
        {
            spike.ForwardAnimator(_reset);
        }

        foreach (Spike spike in spikeHaters)
        {
            spike.ForwardAnimator(_reset);
        }
    }

    public void ResetAllSpikes()
    {
        ForwardAllSpikes(true);
    }

    public void ResetSpikes()
    {
        currentTimeBetweenSpikeFriends = 0.0f;
        ReturnToBaseAnimation();
        activateOtherSpikeFriend = initialOtherSpikeFriend;

        foreach (Spike spike in spikeHaters)
        {
            spike.ResetSpikeHater();
        }
    }

    private void ActivateSpikes()
    {
        currentTimeBetweenSpikeFriends += Time.fixedDeltaTime;
        if (currentTimeBetweenSpikeFriends >= timeBetweenSpikeFriends)
        {
            if (!activateOtherSpikeFriend)
            {
                foreach (Spike spike in spikeFriendsOne)
                {
                    if (!spike.GetIsHater())
                    {
                        spike.TriggerActivate();
                    }
                }
                activateOtherSpikeFriend = true;
                currentTimeBetweenSpikeFriends = 0.0f;
            }
            else
            {
                foreach (Spike spike in spikeFriendsTwo)
                {
                    if (!spike.GetIsHater())
                    {
                        spike.TriggerActivate();
                    }
                }
                activateOtherSpikeFriend = false;
                currentTimeBetweenSpikeFriends = 0.0f;
            }
        }

    }

    private void ReturnToBaseAnimation()
    {
        foreach (Spike spike in spikeFriendsOne)
        {
            spike.GetAnimator().Play("Deactivate");
        }

        foreach (Spike spike in spikeFriendsTwo)
        {
            spike.GetAnimator().Play("Deactivate");
        }

        foreach (Spike spike in spikeHaters)
        {
            spike.GetAnimator().Play("Deactivate");
        }
    }
}
