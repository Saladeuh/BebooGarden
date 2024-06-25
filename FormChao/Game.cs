using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden;

internal class Game
{
  private SoundSystem SoundSystem { get; set; }
  private GlobalActions GlobalActions { get; set; }
  private DateTime LastPressedKeyTime { get; set; }
  static System.Windows.Forms.Timer tickTimer = new System.Windows.Forms.Timer();
  public Beboo beboo;
  public Game(Parameters parameters)
  {
    SoundSystem = new SoundSystem(parameters.Volume);
    GlobalActions = new GlobalActions(SoundSystem);
    SoundSystem.LoadMainScreen();
    LastPressedKeyTime = DateTime.Now;
    tickTimer.Tick += new EventHandler(Tick);
    beboo = new Beboo(SoundSystem);
  }
  public void KeyDownMapper(object sender, KeyEventArgs e)
  {
    if ((DateTime.Now - LastPressedKeyTime).TotalMilliseconds < 200) return;
    else LastPressedKeyTime = DateTime.Now;
    switch (e.KeyCode)
    {
      case Keys.Left:
      case Keys.Q:
        SoundSystem.MovePlayerOf(new Vector3(-1, 0, 0));
        break;
      case Keys.Right:
      case Keys.D:
        SoundSystem.MovePlayerOf(new Vector3(1, 0, 0));
        break;
      case Keys.Up:
      case Keys.Z:
        SoundSystem.MovePlayerOf(new Vector3(0, 1, 0));
        break;
      case Keys.Down:
      case Keys.S:
        SoundSystem.MovePlayerOf(new Vector3(0, -1, 0));
        break;
      case Keys.Space:
        beboo.DoCuteThing();
        break;
      default:
        GlobalActions.CheckGlobalActions(e.KeyCode);
        break;
    }
  }

  public void Tick(Object myObject, EventArgs myEventArgs)
  {
    SoundSystem.System.Update();
  }
}