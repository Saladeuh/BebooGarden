using System.Numerics;

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
    RegenerateBehaviour = new(this, 60 / fruitPerHour, 60 / fruitPerHour, (treeLine) =>
    {
      treeLine.Regenerate();
    });
  }
  private void Regenerate() => Fruits++;
  public FruitSpecies? Shake()
  {
    var rnd = new Random();
    if (Fruits > 0 && rnd.Next(11) == 10)
    {
      Fruits--;
      return AvailableFruitSpecies[rnd.Next(AvailableFruitSpecies.Count)];
    }
    else return null;
  }
}
