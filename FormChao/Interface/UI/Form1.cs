using BebooGarden.GameCore;

namespace BebooGarden.Interface;

public partial class Form1 : Form
{
  public Form1()
  {
    WindowState = FormWindowState.Maximized;
    Text = IGlobalActions.GetLocalizedString("beboogarden");
    Load += onLoad;
  }

  internal Game Game { get; set; }

  private void onLoad(object? sender, EventArgs e)
  {
    Game = new Game(this);
    KeyDown += Game.KeyDownMapper;
    FormClosing += Game.Close;
  }

  public void DisableInput()
  {
    Game.GameWindow.KeyDown -= Game.KeyDownMapper;
  }

  public void EnableInput()
  {
    Game.GameWindow.KeyDown += Game.KeyDownMapper;
  }
}