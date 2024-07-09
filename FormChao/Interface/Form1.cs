using BebooGarden.GameCore;
using BebooGarden.Save;

namespace BebooGarden.Interface;

public partial class Form1 : Form
{
  internal Game Game { get; }
  public Form1(Parameters parameters)
  {
    InitializeComponent();
    this.Text = "Jardin Bébous";
    Game = new Game(parameters);
    this.KeyDown += Game.KeyDownMapper;
    this.KeyUp += Game.KeyUpMapper;
  }

}