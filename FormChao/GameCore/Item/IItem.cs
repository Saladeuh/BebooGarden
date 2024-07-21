using System.Numerics;
using BebooGarden.GameCore.World;

namespace BebooGarden.GameCore.Item;

public interface IItem
{
  public string TranslateKeyName { get; set; }
  public string TranslateKeyDescription { get; set; }
  public Vector3? Position { get; set; } // position null=in inventory
  public abstract void Action();
  public virtual void Take()
  {
    Game.Map.Items.Remove(this);
    Game.SayLocalizedString("ui.itemtake", Game.GetLocalizedString(TranslateKeyName));
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.ItemTakeSound);
    Game.Inventory.Add(this);
  }
}
