using System.Timers;

namespace BebooGarden.GameCore.Beboo;
internal class BebooBehaviour
{
    private Beboo Beboo { get; set; }
    private int MinMS { get; set; }
    private int MaxMS { get; set; }
    private Action<Beboo> Action { get; set; }
    private System.Timers.Timer ActionTimer { get; set; }

    public BebooBehaviour(Beboo beboo, int minSecond, int maxSecond, Action<Beboo> action)
    {
        Beboo = beboo;
        MinMS = minSecond;
        MaxMS = maxSecond;
        Action = action;
        ActionTimer = new System.Timers.Timer(minSecond);
        ActionTimer.Elapsed += onTimer;
    }
    public void onTimer(object? sender, ElapsedEventArgs e)
    {
        Action(Beboo);
        var rnd = new Random();
        ActionTimer.Dispose();
        ActionTimer = new(rnd.Next(MinMS, MaxMS))
        {
            Enabled = true
        };
        ActionTimer.Elapsed += onTimer;
        ActionTimer.Enabled = true;
    }
    public void Start(float delayMS = 0)
    {
        if (delayMS == 0) ActionTimer.Enabled = true;
        else
        {
            var delayTimer = new System.Timers.Timer(delayMS);
            delayTimer.Elapsed += (_, _) => ActionTimer.Enabled = true;
            delayTimer.Enabled = true;
        }
    }
    public void Stop()
    {
        ActionTimer.Enabled = false;
    }
}
