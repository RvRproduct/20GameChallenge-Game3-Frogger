// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using System.Numerics;

public class SpikeCommand : Command
{
    public SpikeCommand(Spike _spike, int _startTick, bool _finished) 
    {
        spike = _spike;
        startTick = _startTick;
        endTick = -1;
        finished = _finished;
        
    }

    private Spike spike;

    public override void Execute()
    {
        finished = true;
        spike.TriggerActivate();
    }
}
