// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private BoxCollider2D boxCollided2D;
    private Animator animator;
    [SerializeField] private float spikeHaterMaxCoolDown = 2.0f;
    [SerializeField] private bool isHater = false;
    private float currentSpikeHaterCoolDown = 0.0f;
    private List<Command> recordedSpikeCommands = new List<Command>();
    private int currentRecordedSpikeCommand = 0;
    private Command spikeCommand = null;
    private Coroutine spikeCoroutine = null;
    private bool isDonePlaying = false;
    private bool isAnimating = false;

    private void Awake()
    {
        boxCollided2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        isDonePlaying = false;
        animator.updateMode = AnimatorUpdateMode.Fixed;
    }

    private void FixedUpdate()
    {
        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            if (isHater)
            {
                if (ReplayManager.Instance.GetIsReplayPlaying())
                {
                    ActivateSpike();
                }
            }
        }
    }

    public void StartSpikeReplay()
    {
        //foreach (Command command in recordedSpikeCommands)
        //{
        //    Debug.Log($"Spike {GetInstanceID()} StartTick {command.startTick}");
        //}

        StopAllCoroutines();
        spikeCoroutine = StartCoroutine(PlaySpikeCommands());
    }

    private IEnumerator PlaySpikeCommands()
    {
        while (!isDonePlaying)
        {
            SpikeCommand currentSpikeCommand = (SpikeCommand)spikeCommand;

            if (!ReplayManager.Instance.GetIsRewinding())
            {
                while (GameManager.Instance.GetGlobalTick() <= currentSpikeCommand.startTick)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                while (GameManager.Instance.GetGlobalTick() >= currentSpikeCommand.endTick)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            
            currentSpikeCommand.Execute();

            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                if (!ReplayManager.Instance.GetIsRewinding())
                {
                    if (currentRecordedSpikeCommand < recordedSpikeCommands.Count - 1)
                    {
                        currentSpikeCommand.finished = false;
                        currentRecordedSpikeCommand++;
                        spikeCommand = (SpikeCommand)recordedSpikeCommands[currentRecordedSpikeCommand];
                    }
                    else
                    {
                        isDonePlaying = true;
                        spikeCoroutine = null;
                    }
                }
                else
                {
                    if (currentRecordedSpikeCommand > 0)
                    {
                        currentSpikeCommand.finished = false;
                        currentRecordedSpikeCommand--;
                        spikeCommand = (SpikeCommand)recordedSpikeCommands[currentRecordedSpikeCommand];
                    }
                    else
                    {
                        isDonePlaying = true;
                        spikeCoroutine = null;
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }  
    }

    public void PauseAnimator()
    {
        animator.speed = 0.0f;
        StopAllCoroutines();
        spikeCoroutine = null;
        spikeCommand.finished = true;
        animator.SetTrigger("Deactivate");

        isDonePlaying = true;
    }

    public void ReverseAnimator(bool reset = false)
    {
        animator.speed = 1.0f;
        StopAllCoroutines();
        spikeCoroutine = null;
        spikeCommand.finished = true;
        TriggerDeactivate();

        if (reset)
        {
            currentRecordedSpikeCommand = recordedSpikeCommands.Count - 1;
            spikeCommand = recordedSpikeCommands[currentRecordedSpikeCommand];
        }

        if (isDonePlaying)
        {
            isDonePlaying = false;
        }
    }

    public void ForwardAnimator(bool reset = false)
    {
        animator.speed = 1.0f;
        StopAllCoroutines();
        spikeCoroutine = null;
        spikeCommand.finished = true;
        TriggerDeactivate();

        if (reset)
        {
            currentRecordedSpikeCommand = 0;
            spikeCommand = recordedSpikeCommands[currentRecordedSpikeCommand];
        }

        if (isDonePlaying)
        {
            isDonePlaying = false;
        }
    }

    public void ResetSpikeHater()
    {
        currentSpikeHaterCoolDown = 0.0f;
    }

    private void ActivateSpike()
    {
        currentSpikeHaterCoolDown += Time.fixedDeltaTime;

        if (currentSpikeHaterCoolDown >= spikeHaterMaxCoolDown)
        {
            if (!isAnimating)
            {
                TriggerActivate();
                currentSpikeHaterCoolDown = 0;
            }  
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

    public bool TriggerActivate()
    {
        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            if (!isAnimating)
            {
                spikeCommand = null;
                isAnimating = true;
                spikeCommand = HandleCommand();
                recordedSpikeCommands.Add(spikeCommand);
                animator.SetTrigger("Activate");
                return true;
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                if (!ReplayManager.Instance.GetIsRewinding())
                {
                    animator.SetTrigger("Activate");
                }
                else
                {
                    animator.SetTrigger("ReverseActivate");
                    
                }
            } 
        }

        return false;
    }

    public void RewindEnd()
    {
        if (ReplayManager.Instance.GetIsRewinding())
        {
            TriggerDeactivate();
        }
    }

    public void ForwardEnd()
    {
        if (!ReplayManager.Instance.GetIsRewinding())
        {
            TriggerDeactivate();
        }
    }

    public void TriggerDeactivate()
    {
        if (!ReplayManager.Instance.GetIsInReplayMode())
        {
            animator.SetTrigger("Deactivate");

        }
        else
        {
            if (ReplayManager.Instance.GetIsReplayPlaying())
            {

                animator.SetTrigger("Deactivate");              
            }
        }
    }

    public void OnDeactivateAnimation()
    {
        if (ReplayManager.Instance.GetIsInReplayMode())
        {
            if (spikeCommand != null)
            {
                spikeCommand.finished = true;
            }
        }
        else
        {
            if (spikeCommand != null)
            {
                spikeCommand.endTick = GameManager.Instance.GetGlobalTick();
                isAnimating = false;
                spikeCommand.finished = false;
            }
        }
        
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    private SpikeCommand HandleCommand()
    {
        return new SpikeCommand(this, GameManager.Instance.GetGlobalTick(),
            true);
    }
}
