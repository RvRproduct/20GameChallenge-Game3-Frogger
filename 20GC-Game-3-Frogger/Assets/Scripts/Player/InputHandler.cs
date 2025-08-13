using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] float playerMoveDistance;
    protected InputControls inputControls;
    private MoveCommand move;

    protected virtual void Awake()
    {
        inputControls = new InputControls();
        inputControls.Enable();
        move = null;
    }

    private void OnEnable()
    {
        inputControls.Player.Move.started += ApplyMovement;
    }

    private void OnDisable()
    {
        inputControls.Player.Move.canceled -= ApplyMovement;
    }

    private void HandleCommand(MoveCommand _move)
    {
        _move.Execute();
    }

    protected MoveCommand HandleInput(float newPlayerX, float newPlayerY)
    {
        move = null;
        return move = new MoveCommand(player, newPlayerX, newPlayerY);
    }

    private void ApplyMovement(InputAction.CallbackContext context)
    {
        Vector2 playerMovement = context.ReadValue<Vector2>();
        Vector2 newPlayerLocation = new Vector2(
            transform.position.x + (playerMovement.x * playerMoveDistance),
            transform.position.y + (playerMovement.y * playerMoveDistance));
        HandleCommand(HandleInput(newPlayerLocation.x, newPlayerLocation.y));
    }
}
