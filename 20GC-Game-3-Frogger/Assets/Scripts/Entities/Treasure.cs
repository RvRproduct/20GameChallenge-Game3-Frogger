// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using UnityEngine;

public class Treasure : MonoBehaviour
{
    static public Treasure Instance;
    private Animator animator;
    private bool isOpen = false;
    private float timeWhenHit = float.PositiveInfinity;

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

        animator = GetComponent<Animator>();
    }

    public void TriggerOpen()
    {
        if (!isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
        }
    }

    public void TriggerClose()
    {
        if (isOpen)
        {
            animator.SetTrigger("Close");
            isOpen = false;
        }
    }

    public void AfterOpen()
    {
        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            UIManager.Instance.PromptForReplay();
        } 
    }

    public void SetTimeWhenHit(float _timeWhenHit)
    {
        timeWhenHit = _timeWhenHit;
    }

    public float GetTimeWhenHit()
    {
        return timeWhenHit;
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }
}
