
using System.Numerics;
using System.Security.Cryptography.Xml;

namespace BebooGarden;

public partial class Form1 : Form
{
  private const string CONTENTFOLDER = "Content/sounds";
  public SoundSystem SoundSystem { get; set; }
  public GlobalActions GlobalActions { get; set; }
  public Form1(Parameters parameters)
  {
    InitializeComponent();
    SoundSystem = new SoundSystem(parameters.Volume);
    GlobalActions = new GlobalActions(SoundSystem);
    SoundSystem.LoadMainScreen();
    var position = new Vector3(0, 0, 0);
    this.KeyDown += onKeyDown;
  }
  private void onKeyDown(object sender, KeyEventArgs e)
  {
    SoundSystem.System.Update();
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
      default:
        GlobalActions.CheckGlobalActions(e.KeyCode);
        break;
    }
    //ScreenReader.Output(e.KeyCode.ToString());
  }

  private void Form1_Load(object sender, EventArgs e)
  {

  }
}
