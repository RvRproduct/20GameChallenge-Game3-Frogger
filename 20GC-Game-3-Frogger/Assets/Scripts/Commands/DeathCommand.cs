// Game and Code By RvRproduct (Roberto Valentino Reynoso)
public class DeathCommand : Command
{
    public DeathCommand(Player player) { }

    public override void Execute()
    {
        player.OnDeath();
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }

    private Player player;
}
