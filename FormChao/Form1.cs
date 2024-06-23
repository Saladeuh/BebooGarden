
using System.Numerics;
using System.Security.Cryptography.Xml;

namespace BebooGarden;

public partial class Form1 : Form
{
  private const string CONTENTFOLDER = "Content/sounds";
  private SoundSystem SoundSystem { get; set; }
  private GlobalActions GlobalActions { get; set; }
  private DateTime LastPressedKeyTime { get; set; }
  static System.Windows.Forms.Timer tickTimer = new System.Windows.Forms.Timer();

  public Form1(Parameters parameters)
  {
    InitializeComponent();
    SoundSystem = new SoundSystem(parameters.Volume);
    GlobalActions = new GlobalActions(SoundSystem);
    SoundSystem.LoadMainScreen();
    LastPressedKeyTime = DateTime.Now;
    var position = new Vector3(0, 0, 0);
    this.KeyDown += onKeyDown;
    tickTimer.Tick += new EventHandler(TimerEventProcessor);
  }

  private void onKeyDown(object sender, KeyEventArgs e)
  {
    if ((DateTime.Now - LastPressedKeyTime).TotalMilliseconds < 200) return;
    else LastPressedKeyTime = DateTime.Now;
    switch (e.KeyCode)
    {
      case Keys.Left:
      case Keys.Q:
        SoundSystem.MoveOf(new Vector3(-1, 0, 0));
        break;
      case Keys.Right:
      case Keys.D:
        SoundSystem.MoveOf(new Vector3(1, 0, 0));
        break;
      case Keys.Up:
      case Keys.Z:
        SoundSystem.MoveOf(new Vector3(0, 1, 0));
        break;
      case Keys.Down:
      case Keys.S:
        SoundSystem.MoveOf(new Vector3(0, -1, 0));
        break;
      case Keys.Space:
        SoundSystem.System.PlaySound(SoundSystem.AmbiSounds[0]);
        break;
        default:
        GlobalActions.CheckGlobalActions(e.KeyCode);
        break;
    }
    SoundSystem.System.Update();
    //ScreenReader.Output(e.KeyCode.ToString());
  }
  private static void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
  {
    ScreenReader.Output("wah");
  }
}
