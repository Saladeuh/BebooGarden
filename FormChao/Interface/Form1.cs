using System.Numerics;
using System.Security.Cryptography.Xml;
using BebooGarden.GameCore;
namespace BebooGarden;

public partial class Form1 : Form
{
  public Form1(Parameters parameters)
  {
    InitializeComponent();
    var game = new Game(parameters);
    this.KeyDown += game.KeyDownMapper;
  }

}