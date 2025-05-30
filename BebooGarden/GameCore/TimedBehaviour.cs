using System;
using System.Timers;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Minigame;
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
    ActionTimer = minMS < maxMS ? new Timer(Game1.Instance.Random.Next(minMS, maxMS)) : new Timer(MinMS);
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
    try
    {
      if (ActionParameter is Beboo)
      {
        var beboo = ActionParameter as Beboo;
        if (beboo.Racer && Race.IsARaceRunning) Action(ActionParameter);
        else if (!beboo.Racer) Action(ActionParameter);
      }
      else Action(ActionParameter);
      ActionTimer.Dispose();
      int ms = MinMS != MaxMS ? Game1.Instance.Random.Next(MinMS, MaxMS) : MinMS;
      ActionTimer = new Timer(ms);
      ActionTimer.Elapsed += onTimer;
      ActionTimer.Enabled = Enabled;
    }
    catch { }
  }
  public void Restart()
  {
    ActionTimer.Dispose();
    int ms = MinMS != MaxMS ? Game1.Instance.Random.Next(MinMS, MaxMS) : MinMS;
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
      try
      {
        Timer delayTimer = new(delayMS);
        delayTimer.Elapsed += (_, _) => ActionTimer.Enabled = true;
        delayTimer.Enabled = true;
        Enabled = true;
      }
      catch { }
    }
  }

  public void Stop()
  {
    if (!Enabled) return;
    ActionTimer.Stop();
    Enabled = false;
  }
}