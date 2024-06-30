using System.Numerics;
using System.Security.Cryptography.Xml;
using BebooGarden.GameCore;
using BebooGarden.Save;
namespace BebooGarden;

public partial class Form1 : Form
{
  public Form1(Parameters parameters)
  {
    InitializeComponent();
    this.Text = "Jardin Bébous";
    var game = new Game(parameters);
    this.KeyDown += game.KeyDownMapper;
  }

}