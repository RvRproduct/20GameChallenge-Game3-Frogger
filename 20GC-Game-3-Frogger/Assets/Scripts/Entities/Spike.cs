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
    private bool isPaused = false;

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
        if (spikeCoroutine == null)
        {
            StopAllCoroutines();
            spikeCoroutine = StartCoroutine(PlaySpikeCommands());
        }
    }

    private IEnumerator PlaySpikeCommands()
    {
        while (!isDonePlaying)
        {
            SpikeCommand currentSpikeCommand = (SpikeCommand)spikeCommand;

            if (!ReplayManager.Instance.GetIsRewinding())
            {
                if (!currentSpikeCommand.finished)
                {
                    while (GameManager.Instance.GetGlobalTick() < currentSpikeCommand.startTick)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
            else
            {
                if (!currentSpikeCommand.finished)
                {
                    while (GameManager.Instance.GetGlobalTick() > currentSpikeCommand.endTick)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                } 
            }

            if (!currentSpikeCommand.finished)
            {
                currentSpikeCommand.Execute();
            }

            yield return new WaitUntil(() => !currentSpikeCommand.finished);

            if (ReplayManager.Instance.GetIsReplayPlaying())
            {
                if (!ReplayManager.Instance.GetIsRewinding())
                {
                    if (currentRecordedSpikeCommand < recordedSpikeCommands.Count - 1)
                    {
                        if (!currentSpikeCommand.finished)
                        {
                            currentRecordedSpikeCommand++;
                            spikeCommand = (SpikeCommand)recordedSpikeCommands[currentRecordedSpikeCommand];
                        }
                        
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
                        if (!currentSpikeCommand.finished)
                        {
                            currentRecordedSpikeCommand--;
                            spikeCommand = (SpikeCommand)recordedSpikeCommands[currentRecordedSpikeCommand];
                        }
                        
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
        isPaused = true;
        //spikeCoroutine = null;
        //spikeCommand.finished = false;
        //animator.SetTrigger("Deactivate");

        isDonePlaying = true;
    }

    public void ReverseAnimator(bool reset = false)
    {
        animator.speed = 1.0f;
        if (!isPaused)
        {
            spikeCoroutine = null;
            spikeCommand.finished = false;
            TriggerDeactivate();
        }
        else
        {
            isPaused = false;
        }


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
        if (!isPaused)
        {
            spikeCoroutine = null;
            spikeCommand.finished = false;
            TriggerDeactivate();
        }
        else
        {
            isPaused = false;
        }

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
                spikeCommand.finished = false;
            }
        }
        else
        {
            if (spikeCommand != null)
            {
                spikeCommand.endTick = GameManager.Instance.GetGlobalTick();
                spikeCommand.finished = false;
                isAnimating = false;
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

    public void CleanUpOutlierCommands()
    {
        for (int commandIndex = recordedSpikeCommands.Count - 1; commandIndex > 0; commandIndex--)
        {
            if (recordedSpikeCommands[commandIndex].endTick <= -1)
            {
                recordedSpikeCommands.RemoveAt(commandIndex);
            }
            else
            {
                break;
            }
        }
    }
}
