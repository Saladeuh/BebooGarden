using System.Timers;

namespace BebooGarden.GameCore;
public class TimedBehaviour<T>
{
  private T ActionParameter { get; set; }
  public int MinMS { get; set; }
  public int MaxMS { get; set; }
  private Action<T> Action { get; set; }
  private System.Timers.Timer ActionTimer { get; set; }
  private bool Enabled { get; set; }
  public TimedBehaviour(T actionParameter, int minSecond, int maxSecond, Action<T> action, bool startAtInit)
  {
    ActionParameter = actionParameter;
    MinMS = minSecond;
    MaxMS = maxSecond;
    Action = action;
    ActionTimer = new System.Timers.Timer(minSecond);
    ActionTimer.Elapsed += onTimer;
    ActionTimer.Enabled = startAtInit;
    Enabled = startAtInit;
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
    ActionTimer.Enabled = Enabled;
  }
  public void Start(float delayMS = 0)
  {
    if(Enabled) return;
    if (delayMS == 0) { ActionTimer.Enabled = true;
      Enabled = true;
    }
    else
    {
      var delayTimer = new System.Timers.Timer(delayMS);
      delayTimer.Elapsed += (_, _) => ActionTimer.Enabled = true;
      delayTimer.Enabled = true;
      Enabled = true;
    }
  }
  public void Stop()
  {
    if(!Enabled) return;
    ActionTimer.Stop();
    Enabled = false;
  }
}
