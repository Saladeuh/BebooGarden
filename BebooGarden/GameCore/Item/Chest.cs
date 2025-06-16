using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using FmodAudio;
using System.Numerics;

namespace BebooGarden.GameCore.Item;

internal class Chest : Item
{
  public override string Name { get; } = GameText.chest_name;
  public override string Description { get; } = GameText.chest_description;
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = true;
  public override bool IsWaterProof { get; set; } = true;
  public override Channel? Channel { get; set; }
  public override int Cost { get; set; } = 5;
  public override void Action()
  {
    Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.ItemChestOpenSound, (Vector3)Position);
    //var beboo = Game1.Instance.ChooseBeboo();
    Game1.Instance.CurrentPlayingMiniGame = new Minigame.memory.Memory(Game1.Instance.SoundSystem.Volume);
    Game1.Instance.CurrentPlayingMiniGame.Start();
  }
  public override void BebooAction(Beboo beboo)
  {
    base.BebooAction(beboo);
  }
  public override void PlaySound()
  {
    if (Position == null || !(Game1.Instance.Map?.Items.Contains(this) ?? false)) return;
    Channel = Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.ItemChestSound, (Vector3)Position);
  }
}
