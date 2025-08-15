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

    [Header("Spike Friend Timing")]
    [SerializeField] private float timeBetweenSpikeFriends = 3.0f;
    private float currentTimeBetweenSpikeFriends = 0.0f;
    private bool activateOtherSpikeFriend = false;


    private void Update()
    {
        ActivateSpikes();
    }

    private void ActivateSpikes()
    {
        currentTimeBetweenSpikeFriends += Time.deltaTime;
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
}
