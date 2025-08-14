// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Numerics;

public class MoveCommand : Command
{
    public MoveCommand(Player _player,
        float _playerX, float _playerY, float _directionX, float _directionY,
        float _timeStamp, bool _finished) 
    {
        timeStamp = _timeStamp;
        finished = _finished;
        directionX = _directionX;
        directionY = _directionY;
        player = _player;        
        playerX = _playerX;
        playerY = _playerY;
    }
    public override void Execute()
    {
        finished = true;
        player.MovePlayer(playerX, playerY, directionX, directionY, this);
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }

    private Player player;
    private float playerX;
    private float playerY;
    private float directionX;
    private float directionY;
}
