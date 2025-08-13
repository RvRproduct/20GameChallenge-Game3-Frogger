// Game and Code By RvRproduct (Roberto Valentino Reynoso)
public abstract class Command
{
    public float timeStamp;
    public bool finished;
    public abstract void Execute();

    public abstract void Undo();
}
