// Game and Code By RvRproduct (Roberto Valentino Reynoso)
public class DeathCommand : ICommand
{
    public DeathCommand(Player player) { }

    public void Execute()
    {
        player.OnDeath();
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    private Player player;
}
