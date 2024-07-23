using BebooGarden.GameCore;
using BebooGarden.Save;

namespace BebooGarden.Interface;

public partial class Form1 : Form
{
  internal Game Game { get; set; }
  public Form1()
  {
    WindowState = FormWindowState.Maximized;
    this.Text = "Jardin Bébous";
    Load += onLoad;
  }

  private void onLoad(object? sender, EventArgs e)
  {
    Game = new Game(this);
    this.KeyDown += Game.KeyDownMapper;
    this.KeyUp += Game.KeyUpMapper;
    this.FormClosing += Game.Close;
  }
  public void DisableInput()
  {
    Game.GameWindow.KeyDown -= Game.KeyDownMapper;
    Game.GameWindow.KeyUp -= Game.KeyUpMapper;
  }
  public void EnableInput()
  {
    Game.GameWindow.KeyDown += Game.KeyDownMapper;
    Game.GameWindow.KeyUp += Game.KeyUpMapper;
  }
}
