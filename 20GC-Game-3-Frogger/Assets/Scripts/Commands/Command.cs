// Game and Code By RvRproduct (Roberto Valentino Reynoso)
public abstract class Command
{
    public int startTick;
    public int endTick;
    public bool finished;
    public abstract void Execute();
}
