using System.Numerics;
using BebooGarden.GameCore.Pet;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

internal class Chest : Item
{
  protected override string _translateKeyName { get; } = "chest.name";
  protected override string _translateKeyDescription { get; } = "chest.description";
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = true;
  public override bool IsWaterProof { get; set; } = true;
  public override Channel? Channel { get; set; }
  public override int Cost { get; set; } = 5;
  public override void Action()
  {
    Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.ItemChestOpenSound, (Vector3)Position);
    var score = new Minigame.memory.MainMenu(Game.SoundSystem.Volume).PlayGame();
    Game.GainTicket((int)score / 5);
    Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.ItemChestCloseSound, (Vector3)Position);
  }
  public override void BebooAction(Beboo beboo)
  {
    base.BebooAction(beboo);
  }
  public override void PlaySound()
  {
    if (Position == null || !(Game.Map?.Items.Contains(this) ?? false)) return;
    Channel = Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.ItemChestSound, (Vector3)Position);
  }
}
