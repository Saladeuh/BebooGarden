using BebooGarden.GameCore;
using BebooGarden.Save;

namespace BebooGarden.Interface;

public partial class Form1 : Form
{
  internal Game Game { get; set; }
  public Form1()
  {
    InitializeComponent();
    this.Text = "Jardin Bébous";
    Load += onLoad;
  }

  private void onLoad(object? sender, EventArgs e)
  {
    Game = new Game(this);
    this.KeyDown += Game.KeyDownMapper;
    this.KeyUp += Game.KeyUpMapper;
  }
}
