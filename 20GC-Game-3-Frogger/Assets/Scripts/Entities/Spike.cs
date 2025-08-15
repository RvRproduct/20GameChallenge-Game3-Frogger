using UnityEngine;

public class Spike : MonoBehaviour
{
    private BoxCollider2D boxCollided2D;
    private Animator animator;
    [SerializeField] private float spikeHaterMaxCoolDown = 2.0f;
    [SerializeField] private bool isHater = false;
    private float currentSpikeHaterCoolDown = 0.0f;

    private void Awake()
    {
        boxCollided2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isHater)
        {
            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                ActivateSpike();
            }
        }
    }

    public void ResetSpikeHater()
    {
        currentSpikeHaterCoolDown = 0.0f;
    }

    private void ActivateSpike()
    {
        currentSpikeHaterCoolDown += Time.deltaTime;

        if (currentSpikeHaterCoolDown >= spikeHaterMaxCoolDown)
        {
            TriggerActivate();
            currentSpikeHaterCoolDown = 0;
        }
    }

    public bool GetIsHater()
    {
        return isHater;
    }

    public void ActivateCollision()
    {
        boxCollided2D.enabled = true;
    }

    public void DeactivateCollision()
    {
        boxCollided2D.enabled = false;
    }

    public void TriggerActivate()
    {
        animator.SetTrigger("Activate");
    }

    public void TriggerDeactivate()
    {
        animator.SetTrigger("Deactivate");
    }

    public Animator GetAnimator()
    {
        return animator;
    }
}
