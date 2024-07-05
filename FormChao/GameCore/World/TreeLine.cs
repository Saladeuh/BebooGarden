using System.Numerics;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace BebooGarden.GameCore.World;

internal class TreeLine
{
  public Vector2 X { get; }
  public Vector2 Y { get; }
  public int FruitPerHour { get; set; }
  private int Fruits { get; set; }
  public List<FruitSpecies> AvailableFruitSpecies { get; set; }
  private TimedBehaviour<TreeLine> RegenerateBehaviour { get; set; }
  public TreeLine(Vector2 x, Vector2 y, int fruitPerHour = 5, List<FruitSpecies>? availableFruitSpecies = null)
  {
    X = x;
    Y = y;
    FruitPerHour = fruitPerHour;
    Fruits = fruitPerHour;
    AvailableFruitSpecies = availableFruitSpecies ?? [FruitSpecies.Normal];
    RegenerateBehaviour = new(this, 3600000 / fruitPerHour, 60 / fruitPerHour, (treeLine) =>
    {
      treeLine.Regenerate();
    });
    RegenerateBehaviour.Start();
  }
  private void Regenerate() => Fruits++;
  private DateTime lastShaked = DateTime.MinValue;
  public FruitSpecies? Shake()
  {
    if((DateTime.Now-lastShaked).TotalSeconds <1) return null;
    else lastShaked = DateTime.Now;
    Game.SoundSystem.ShakeTrees();
    var rnd = new Random();
    if (Fruits > 0 && rnd.Next(6) == 5)
    {
      Fruits--;
      var droppedFruit= AvailableFruitSpecies[rnd.Next(AvailableFruitSpecies.Count)];
      Game.SoundSystem.DropFruitSound(droppedFruit);
      return droppedFruit;
        }
    else return null;
  }
  public bool isOnLine(Vector3 point)
  {
    var dxc = point.X - X.X;
    var dyc = point.Y - X.Y;
    var dxl = Y.X - X.X;
    var dyl = Y.Y - X.Y;
    var cross = dxc * dyl - dyc * dxl;
    if (cross != 0)
    {
      return false;
    }
    if (Math.Abs(dxl) >= Math.Abs(dyl))
      return dxl > 0 ?
        X.X <= point.X && point.X <= Y.X :
        Y.X <= point.X && point.X <= X.X;
    else
      return dyl > 0 ?
        X.Y <= point.Y && point.Y <= Y.Y :
        Y.Y <= point.Y && point.Y <= X.Y;
  }
}