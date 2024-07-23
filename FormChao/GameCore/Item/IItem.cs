using System.Numerics;
using BebooGarden.GameCore.World;
using Newtonsoft.Json;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

public abstract class IItem
{
  public abstract string TranslateKeyName { get; set; }
  public abstract string TranslateKeyDescription { get; set; }
  public abstract Vector3? Position { get; set; } // position null=in inventory
  public abstract bool IsTakable { get; set; }
  [JsonIgnore]
  public abstract Channel? Channel { get; set; }
  public TimedBehaviour<IItem> SoundLoopTimer {  get; set; }
  public IItem()
  {
     SoundLoopTimer = new(this, 3000, 3000, item =>
     {
       item.PlaySound();
     }, true);
  }
  public virtual void Action() { }
  public abstract void PlaySound();
  public virtual void Take()
  {
    Game.Map.Items.Remove(this);
    Game.SayLocalizedString("ui.itemtake", Game.GetLocalizedString(TranslateKeyName));
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.ItemTakeSound);
    Game.Inventory.Add(this);
  }
}
