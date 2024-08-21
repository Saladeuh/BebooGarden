using System.Numerics;
using BebooGarden.GameCore.Pet;
using BebooGarden.Interface;
using FmodAudio;
using Newtonsoft.Json;

namespace BebooGarden.GameCore.Item;

public abstract class Item
{
  protected Item()
  {
    SoundLoopTimer = new TimedBehaviour<Item>(this, 3000, 3000, item => { item.PlaySound(); }, true);
  }

  protected abstract string _translateKeyName { get; }
  public virtual string Name { get => Game.GetLocalizedString(_translateKeyName); }
  protected abstract string _translateKeyDescription { get; }
  public string Description { get => Game.GetLocalizedString(_translateKeyDescription); }
  public abstract Vector3? Position { get; set; } // poition null=in inventory
  public virtual bool IsTakable { get; set; } = true;
  public virtual bool IsWaterProof { get; set; } = false;
  public virtual int Cost { get; set; } = 1;
  [JsonIgnore] public virtual Channel? Channel { get; set; }
  [JsonIgnore]
  public TimedBehaviour<Item> SoundLoopTimer { get; set; }

  public virtual void Action()
  {
  }

  public abstract void PlaySound();

  public virtual void Take()
  {
    Game.Map?.Items.Remove(this);
    Position = null;
    IGlobalActions.SayLocalizedString("ui.itemtake", IGlobalActions.GetLocalizedString(_translateKeyName));
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.ItemTakeSound);
    Game.Inventory.Add(this);
  }
  public virtual void Buy()
  {
    if (Game.Tickets - Cost >= 0)
    {
      Game.Tickets -= Cost;
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.ShopSound);
      IGlobalActions.SayLocalizedString("shop.buy");
      Game.Inventory?.Add(this);
    }
    else
    {
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.WarningSound);
      IGlobalActions.SayLocalizedString("shop.notickets");
    }
  }
  public virtual void BebooAction(Beboo beboo)
  {

  }
  public virtual void Pause()
  {
    SoundLoopTimer?.Stop();
  }
  public virtual void Unpause()
  {
    SoundLoopTimer?.Start();
  }
}