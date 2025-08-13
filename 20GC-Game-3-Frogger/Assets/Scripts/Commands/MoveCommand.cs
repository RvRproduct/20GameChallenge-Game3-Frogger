// Game and Code By RvRproduct (Roberto Valentino Reynoso)
public class MoveCommand : Command
{
    public MoveCommand(Player _player, float _playerX, float _playerY, float _timeStamp, bool _finished) 
    {
        timeStamp = _timeStamp;
        finished = _finished;
        player = _player;        
        playerX = _playerX;
        playerY = _playerY;
    }
    public override void Execute()
    {
        finished = true;
        player.MovePlayer(playerX, playerY, this);
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }

    private Player player;
    private float playerX;
    private float playerY;
}
