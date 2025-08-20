// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Numerics;

public class MoveCommand : Command
{
    public MoveCommand(Player _player,
        Vector2 _startPosition, Vector2 _endPosition, Vector2 _direction,
        int _startTick, int _CountDownTimeStart, bool _finished) 
    {
        player = _player;
        startPosition = _startPosition;
        endPosition = _endPosition;
        direction = _direction;
        startTick = _startTick;
        finished = _finished;
        CountDownTimeStart = _CountDownTimeStart;
    }
    public override void Execute()
    {
        finished = true;
        player.MovePlayer(this);
    }


    private Player player;
    private Vector2 startPosition; 
    private Vector2 endPosition;
    private Vector2 direction;
    private int CountDownTimeStart;
    private int CountDownTimeEnd = -1;

    public Vector2 GetStartPosition()
    {
        return startPosition;
    }

    public Vector2 GetEndPosition()
    {
        return endPosition;
    }

    public Vector2 GetDirection()
    {
        return direction;
    }
    public int GetCountDownTimeStart()
    {
        return CountDownTimeStart;
    }
    public int GetCountDownTimeEnd()
    {
        return CountDownTimeEnd;
    }

    public void SetCountDownTimeEnd(int _CountDownTimeEnd)
    {
        CountDownTimeEnd = _CountDownTimeEnd;
    }
}
