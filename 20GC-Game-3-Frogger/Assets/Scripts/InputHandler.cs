using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputControls inputControls;
    private Vector2 playerLocation;

    private void Awake()
    {
        inputControls = new InputControls();
        inputControls.Enable();
    }

    private void OnEnable()
    {
        inputControls.Player.Move.started += MoveTo;
    }

    private void OnDisable()
    {
        inputControls.Player.Move.canceled -= MoveTo;
    }

    private void MoveTo(InputAction.CallbackContext context)
    {
        Vector2 playerMovement = context.ReadValue<Vector2>();
        SetPlayerLocation(playerLocation);
        MovePlayer(GetPlayerLocation().x, GetPlayerLocation().y);
    }

    public void MovePlayer(float _playerX, float _playerY)
    {

    }

    // Getters and Setters
    protected Vector2 GetPlayerLocation()
    {
        return playerLocation;
    }

    protected void SetPlayerLocation(Vector2 _newPlayerLocation)
    {
        playerLocation = _newPlayerLocation;
    }
}
