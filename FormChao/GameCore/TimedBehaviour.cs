using System.Timers;

namespace BebooGarden.GameCore;
internal class TimedBehaviour<T>
{
    private T ActionParameter { get; set; }
    public int MinMS { get; set; }
    public int MaxMS { get; set; }
    private Action<T> Action { get; set; }
    private System.Timers.Timer ActionTimer { get; set; }

    public TimedBehaviour(T actionParameter, int minSecond, int maxSecond, Action<T> action)
    {
        ActionParameter = actionParameter;
        MinMS = minSecond;
        MaxMS = maxSecond;
        Action = action;
        ActionTimer = new System.Timers.Timer(minSecond);
        ActionTimer.Elapsed += onTimer;
    }
    public void onTimer(object? sender, ElapsedEventArgs e)
    {
        Action(ActionParameter);
        ActionTimer.Dispose();
        int ms;
        if (MinMS != MaxMS)
        {
            var rnd = new Random();
            ms = rnd.Next(MinMS, MaxMS);
        }
        else ms = MinMS;
        ActionTimer = new(ms);
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
