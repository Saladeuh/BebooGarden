using System.Globalization;
using BebooGarden.GameCore;
using BebooGarden.Interface.EscapeMenu;
using BebooGarden.Save;

namespace BebooGarden.Interface;

public partial class Form1 : Form
{
  public Form1()
  {
    WindowState = FormWindowState.Maximized;
    Text = "Beboo Garden";
    Load += onLoad;
  }

  internal Game Game { get; set; }

  private void onLoad(object? sender, EventArgs e)
  {
    SaveParameters p = SaveManager.LoadSave();
    CultureInfo.CurrentUICulture = new CultureInfo(p.Language);
    Game = new Game(this, p);
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