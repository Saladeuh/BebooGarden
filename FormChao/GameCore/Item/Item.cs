using System.Numerics;
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

    public abstract string TranslateKeyName { get; set; }
    public abstract string TranslateKeyDescription { get; set; }
    public abstract Vector3? Position { get; set; } // position null=in inventory
    public abstract bool IsTakable { get; set; }

    [JsonIgnore] public abstract Channel? Channel { get; set; }

    protected TimedBehaviour<Item> SoundLoopTimer { get; set; }

    public virtual void Action()
    {
    }

    public abstract void PlaySound();

    public virtual void Take()
    {
        Game.Map?.Items.Remove(this);
        IGlobalActions.SayLocalizedString("ui.itemtake", IGlobalActions.GetLocalizedString(TranslateKeyName));
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.ItemTakeSound);
        Game.Inventory.Add(this);
    }
}