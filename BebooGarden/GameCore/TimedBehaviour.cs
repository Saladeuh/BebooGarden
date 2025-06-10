using System;
using System.Timers;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Minigame;
using Timer = System.Timers.Timer;

namespace BebooGarden.GameCore;

public class TimedBehaviour
{
  public TimedBehaviour(int minMS, int maxMS, bool startAtInit)
  {
    _defaultMinMS = minMS;
    _defaultMaxMS = maxMS;
    MinMS = minMS;
    MaxMS = maxMS;
    Enabled = startAtInit;
  }

  public int MinMS { get; set; }
  public int MaxMS { get; set; }
  private bool Enabled { get; set; }
  private DateTime _timer = DateTime.Now;
  private int _defaultMinMS;
  private int _defaultMaxMS;

  public bool ItsTime()
  {
    var elapsedms = (DateTime.Now - _timer).TotalMilliseconds;
    return Enabled && elapsedms >= Game1.Instance.Random.Next(MinMS, MaxMS);
  }
  public void Start() => Enabled = true;
  public void Stop() => Enabled = false;

  public void Start(int delayMS)
  {
    _timer = (DateTime.Now - TimeSpan.FromMilliseconds(delayMS));
    Enabled = true;
  }

  public void Restart()
  {
    MinMS = _defaultMinMS;
    MaxMS = _defaultMaxMS;
    Start();
  }
  public void Done() => _timer = DateTime.Now;
}