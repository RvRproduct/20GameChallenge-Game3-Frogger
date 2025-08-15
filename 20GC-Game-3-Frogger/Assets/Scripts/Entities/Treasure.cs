// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using UnityEngine;

public class Treasure : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;
    private float timeWhenHit = float.PositiveInfinity;

    private void Awake()
    {
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

    public void SetTimeWhenHit(float _timeWhenHit)
    {
        timeWhenHit = _timeWhenHit;
    }

    public float GetTimeWhenHit()
    {
        return timeWhenHit;
    }
}
