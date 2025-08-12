// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Numerics;

public class MoveCommand : ICommand
{
    public MoveCommand(Player _player, float _playerX, float _playerY) 
    {
        player = _player;
        playerX = _playerX;
        playerY = _playerY;
    }
    public void Execute()
    {
        player.MovePlayer(playerX, playerY);
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    private Player player;
    private float playerX;
    private float playerY;
}
