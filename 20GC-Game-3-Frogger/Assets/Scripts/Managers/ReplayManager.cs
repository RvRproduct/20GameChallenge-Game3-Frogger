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
    private bool isRecordingPlaying = false;
    private bool isRewinding = false;
    private InputControls inputControls;

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

        inputControls = new InputControls();
    }

    private void OnEnable()
    {
        inputControls.Enable();
        inputControls.TestRecording.PlayRecording.started += StartRecording;
    } 

    private void OnDisable()
    {
        inputControls.Disable();
        inputControls.TestRecording.PlayRecording.started -= StartRecording;
    }

    private void StartRecording(InputAction.CallbackContext context)
    {
        GameManager.Instance.SetPlayerToStartingLocation();
        isRecordingPlaying = true;
        StartCoroutine(PlayRecordedCommands());
    }

    public IEnumerator PlayRecordedCommands()
    {
        currentRecordedCommand = 0;
        while (isRecordingPlaying && currentRecordedCommand < recordedCommands.Count)
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

    public bool GetIsPlayingRecording()
    {
        return isRecordingPlaying;
    }

    public bool GetIsRewinding()
    {
        return isRewinding;
    }

    public void AddRecordedCommand(Command _command)
    {
        recordedCommands.Add(_command);
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
