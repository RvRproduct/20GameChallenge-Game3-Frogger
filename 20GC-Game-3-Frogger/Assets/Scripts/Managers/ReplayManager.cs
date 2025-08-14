using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance;
    private int currentRecordedCommand = 0;
    private List<Command> recordedCommands = new List<Command>();
    private float playBackTimer = 0.0f;
    private bool isReplayPlaying = false;
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
        StartCoroutine(PlayRecordedCommands());
    }

    public IEnumerator PlayRecordedCommands()
    {
        while (isReplayPlaying && currentRecordedCommand < recordedCommands.Count)
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

    public void AddRecordedCommand(Command _command)
    {
        recordedCommands.Add(_command);
    }

    public void RestartReplay()
    {
        GameManager.Instance.SetPlayerToStartingLocation();
        currentRecordedCommand = 0;
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
