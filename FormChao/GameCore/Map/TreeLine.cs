using System.Numerics;

namespace BebooGarden.GameCore.Map;

internal class TreeLine
{
  public Vector2 X { get; }
  public Vector2 Y { get; }
  public int FruitPerHour { get; set; }
  private int Fruits { get; set; }
  public List<FruitSpecies> AvailableFruitSpecies { get; set; }

  public TreeLine(Vector2 x, Vector2 y, int fruitPerHour = 5, List<FruitSpecies>? availableFruitSpecies = null)
  {
    X = x;
    Y = y;
    FruitPerHour = fruitPerHour;
    Fruits = fruitPerHour;
    AvailableFruitSpecies = (availableFruitSpecies ?? [FruitSpecies.Normal]);
  }
  public FruitSpecies? Shake()
  {
    var rnd= new Random();
    if (rnd.Next(11) == 10) return AvailableFruitSpecies[rnd.Next(AvailableFruitSpecies.Count)];
    else return null;
  }
}
