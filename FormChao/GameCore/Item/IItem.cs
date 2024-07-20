using System.Numerics;

namespace BebooGarden.GameCore.Item;

public interface IItem
{
  public string TranslateKeyName { get; set; }
  public string TranslateKeyDescription { get; set; }
  public Vector3? Position { get; set; } // position null=in inventory
  public abstract void Action();
}
