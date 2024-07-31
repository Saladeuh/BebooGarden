using System.Timers;
using Timer = System.Timers.Timer;

namespace BebooGarden.GameCore;

public class TimedBehaviour<T>
{
  public TimedBehaviour(T actionParameter, int minMS, int maxMS, Action<T> action, bool startAtInit)
  {
    ActionParameter = actionParameter;
    MinMS = minMS;
    MaxMS = maxMS;
    Action = action;
    ActionTimer = new Timer(minMS);
    ActionTimer.Elapsed += onTimer;
    ActionTimer.Enabled = startAtInit;
    Enabled = startAtInit;
  }

  private T ActionParameter { get; }
  public int MinMS { get; set; }
  public int MaxMS { get; set; }
  private Action<T> Action { get; }
  private Timer ActionTimer { get; set; }
  private bool Enabled { get; set; }

  public void onTimer(object? sender, ElapsedEventArgs e)
  {
    Action(ActionParameter);
    ActionTimer.Dispose();
    int ms;
    if (MinMS != MaxMS)
    {
      ms = Game.Random.Next(MinMS, MaxMS);
    }
    else
    {
      ms = MinMS;
    }

    ActionTimer = new Timer(ms);
    ActionTimer.Elapsed += onTimer;
    ActionTimer.Enabled = Enabled;
  }
  public void Restart()
  {
    ActionTimer.Dispose();
    int ms;
    if (MinMS != MaxMS)
    {
      ms = Game.Random.Next(MinMS, MaxMS);
    }
    else
    {
      ms = MinMS;
    }

    ActionTimer = new Timer(ms);
    ActionTimer.Elapsed += onTimer;
    ActionTimer.Enabled = Enabled;
  }
  public void Start(float delayMS = 0)
  {
    if (Enabled) return;
    if (delayMS == 0)
    {
      ActionTimer.Enabled = true;
      Enabled = true;
    }
    else
    {
      var delayTimer = new Timer(delayMS);
      delayTimer.Elapsed += (_, _) => ActionTimer.Enabled = true;
      delayTimer.Enabled = true;
      Enabled = true;
    }
  }

  public void Stop()
  {
    if (!Enabled) return;
    ActionTimer.Stop();
    Enabled = false;
  }
}