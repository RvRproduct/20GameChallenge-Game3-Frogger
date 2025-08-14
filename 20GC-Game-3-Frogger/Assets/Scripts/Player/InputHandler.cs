using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] float playerMoveDistance;
    protected InputControls inputControls;
    private Command move;

    protected virtual void Awake()
    {
        inputControls = new InputControls();
        move = null;
    }

    private void OnEnable()
    {
        inputControls.Enable();
        inputControls.Player.Move.started += ApplyMovement;
    }

    private void OnDisable()
    {
        inputControls.Player.Move.started -= ApplyMovement;
        inputControls.Disable();
    }

    private void OnDestroy()
    {
        inputControls?.Dispose();
    }

    private void HandleCommand(Command _move)
    {
        _move.Execute();
    }

    protected Command HandleInput(float newPlayerX, float newPlayerY, float directionX, float directionY)
    {
        move = null;
        return move = new MoveCommand(player,
            newPlayerX,
            newPlayerY,
            directionX,
            directionY,
            GameManager.Instance.GetGameTimer(),
            true);
    }

    private void ApplyMovement(InputAction.CallbackContext context)
    {
        if (inputControls != null && gameObject != null)
        {
            Vector2 playerMovement = context.ReadValue<Vector2>();
            Vector2 newPlayerLocation = new Vector2(
                transform.position.x + (playerMovement.x * playerMoveDistance),
                transform.position.y + (playerMovement.y * playerMoveDistance));
            Command currentCommand = HandleInput(newPlayerLocation.x, newPlayerLocation.y,
                playerMovement.x, playerMovement.y);
            ReplayManager.Instance.AddRecordedCommand(currentCommand);
            HandleCommand(currentCommand);
        }     
    }
}
