// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance;
    private int currentRecordedCommand = 0;
    private List<Command> recordedCommands = new List<Command>();
    private float playBackTimer = 0.0f;
    private bool isReplayPlaying = true;
    private bool isRewinding = false;

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

    public void StartReplay()
    {
        isReplayPlaying = true;
        StopAllCoroutines();
        StartCoroutine(PlayRecordedCommands());
    }

    public IEnumerator PlayRecordedCommands()
    {
        if (currentRecordedCommand >= recordedCommands.Count)
        {
            currentRecordedCommand = recordedCommands.Count - 1;
        }
        else if (currentRecordedCommand < 0)
        {
            currentRecordedCommand = 0;
        }

        if (currentRecordedCommand < recordedCommands.Count &&
            currentRecordedCommand >= 0)
        {
            playBackTimer = recordedCommands[currentRecordedCommand].timeStamp;
        }
        
        // Normal
        while (isReplayPlaying && 
            currentRecordedCommand <= recordedCommands.Count - 1 &&
            !isRewinding)
        {
            playBackTimer += Time.deltaTime;
            if (playBackTimer >= recordedCommands[currentRecordedCommand].timeStamp)
            {
                if (!recordedCommands[currentRecordedCommand].finished)
                {
                    recordedCommands[currentRecordedCommand].Execute();
                }
            }

            yield return null;
        }

        // Rewind
        while (isReplayPlaying &&
            currentRecordedCommand >= 0 &&
            isRewinding)
        {
            playBackTimer -= Time.deltaTime;
            if (playBackTimer <= recordedCommands[currentRecordedCommand].timeStamp)
            {
                if (!recordedCommands[currentRecordedCommand].finished)
                {
                    recordedCommands[currentRecordedCommand].Execute();
                }
            }

            yield return null;
        }

    }

    public bool GetIsReplayPlaying()
    {
        return isReplayPlaying;
    }

    public void SetIsReplayPlaying(bool _isReplayPlaying)
    {
        isReplayPlaying = _isReplayPlaying;
    }

    public bool GetIsRewinding()
    {
        return isRewinding;
    }

    public void SetIsRewinding(bool _isRewinding)
    {
        isRewinding = _isRewinding;
    }

    public void AddRecordedCommand(Command _command)
    {
        recordedCommands.Add(_command);
    }

    public void RestartReplay()
    {
        if (!isRewinding)
        {
            GameManager.Instance.SetPlayerStartingLocation(
            GameManager.Instance.GetPlayer().transform.position);
            currentRecordedCommand = 0;
        }
        else
        {
            GameManager.Instance.SetPlayerStartingLocation(
                new Vector3(((MoveCommand)recordedCommands[recordedCommands.Count - 1]).GetPlayerX(),
                ((MoveCommand)recordedCommands[recordedCommands.Count - 1]).GetPlayerY(),
                GameManager.Instance.GetPlayer().transform.position.z));
            currentRecordedCommand = recordedCommands.Count - 1;
        }
    }

    public void IncrementCurrentRecordedCommand()
    {
        currentRecordedCommand++;
    }

    public void DecrementCurrentRecordedCommand()
    {
        currentRecordedCommand--;
    }
}
