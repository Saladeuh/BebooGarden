using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using FmodAudio;
using Newtonsoft.Json;

namespace BebooGarden.GameCore.Item;

public abstract class Item
{
  protected Item()
  {
    SoundLoopTimer = new TimedBehaviour<Item>(this, 3000, 3000, item => { item.PlaySound(); }, true);
  }

  public virtual string Name { get;  }
  public virtual string Description { get; } = string.Empty;
  public abstract Vector3? Position { get; set; } // position null = in inventory
  public virtual bool IsTakable { get; set; } = true;
  public virtual bool IsWaterProof { get; set; } = false;
  public virtual int Cost { get; set; } = 1;

  [JsonIgnore]
  public virtual Channel? Channel { get; set; }

  [JsonIgnore]
  public TimedBehaviour<Item> SoundLoopTimer { get; set; }

  public virtual void Action()
  {
  }

  public abstract void PlaySound();

  public virtual void Take()
  {
    Game1.Instance.Map?.Items.Remove(this);
    Position = null;
    Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.ItemTakeSound);
    Game1.Instance.Inventory.Add(this);
  }

  public virtual void Buy()
  {
    if (Game1.Instance.Tickets - Cost >= 0)
    {
      Game1.Instance.Tickets -= Cost;
      Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.ShopSound);
      Game1.Instance.Inventory?.Add(this);
    }
    else
    {
      Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.WarningSound);
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
