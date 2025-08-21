// Game and Code By RvRproduct (Roberto Valentino Reynoso)
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


    private void FixedUpdate()
    {
        if (ReplayManager.Instance.GetIsReplayPlaying())
        {
            ActivateSpikes();
        }
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
