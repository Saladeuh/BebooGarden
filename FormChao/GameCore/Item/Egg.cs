using System.Numerics;

namespace BebooGarden.GameCore.Item;

public class Egg : IItem
{
  public string TranslateKeyName { get; set; } = "egg.name";
  public string TranslateKeyDescription { get; set; } = "egg.description";
  public Vector3? Position { get; set; } // position null=in inventory
  public void Action() { }
}
