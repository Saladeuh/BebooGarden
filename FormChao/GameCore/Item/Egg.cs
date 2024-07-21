using System.Numerics;
using BebooGarden.GameCore.Pet;

namespace BebooGarden.GameCore.Item;

public class Egg(string color) : IItem
{
  public string TranslateKeyName { get; set; } = "egg.name";
  public string TranslateKeyDescription { get; set; } = "egg.description";
  public Vector3? Position { get; set; } // position null=in inventory
  private string Color {  get; set; }=color;

  public void Action() {
    Hatch();
  }
  public void Hatch()
  {
    Game.Beboo=new Beboo("Gérard", 0, DateTime.MinValue);
    Game.Map.Items.Remove(this);
  }
}
