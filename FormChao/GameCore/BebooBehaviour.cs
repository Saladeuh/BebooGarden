using System.Timers;

namespace BebooGarden.GameCore;
internal class BebooBehaviour
{
  private Beboo Beboo { get; set; }
  private int minSecond { get; set; }
  private int maxSecond { get; set; }
  private Action<Beboo> Action { get; set; }
  private System.Timers.Timer ActionTimer { get; set; }

  public BebooBehaviour(Beboo beboo, int minSecond, int maxSecond, Action<Beboo> action)
  {
    this.Beboo=beboo;
    this.minSecond = minSecond;
    this.maxSecond = maxSecond;
    this.Action = action;
    ActionTimer = new System.Timers.Timer(minSecond);
    ActionTimer.Elapsed += onTimer;
  }
  public void onTimer(object? sender, ElapsedEventArgs e)
  {
    Action(Beboo);
    var rnd = new Random();
    ActionTimer.Dispose();
    ActionTimer = new System.Timers.Timer(rnd.Next(minSecond, maxSecond));
    ActionTimer.Enabled = true;
    ActionTimer.Elapsed += onTimer;
    ActionTimer.Enabled = true;
  }
  public void Start() {
    ActionTimer.Enabled = true;
  }
  public void Stop()
  {
    ActionTimer.Enabled = false;
  }
}
