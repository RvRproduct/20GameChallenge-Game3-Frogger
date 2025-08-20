// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] float playerMoveDistance;
    protected InputControls inputControls;

    protected virtual void Awake()
    {
        inputControls = new InputControls();
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

    protected Command HandleInput(Vector2 startPosition, Vector2 endPosition, Vector2 direction)
    {
        return new MoveCommand(player,
            VectorConversions.ToSystem(startPosition),
            VectorConversions.ToSystem(endPosition),
            VectorConversions.ToSystem(direction),
            GameManager.Instance.GetGlobalTick(),
            GameManager.Instance.GetCurrentCountDown(),
            true);
    }

    private void ApplyMovement(InputAction.CallbackContext context)
    {
        if (!player.GetInMiddleOfMoveCommand() && !GameManager.Instance.GetPlayer().GetIsDead())
        {
            player.SetInMiddleOfMoveCommand(true);
            if (inputControls != null && gameObject != null)
            {
                Vector2 playerMovement = context.ReadValue<Vector2>();
                Vector2 newPlayerLocation = new Vector2(
                    transform.position.x + (playerMovement.x * playerMoveDistance),
                    transform.position.y + (playerMovement.y * playerMoveDistance));
                Command currentCommand = HandleInput(transform.position, newPlayerLocation,
                    playerMovement);
                ReplayManager.Instance.AddRecordedCommand(CommandType.PlayerMoving, currentCommand);
                HandleCommand(currentCommand);
            }
        } 
    }
}
