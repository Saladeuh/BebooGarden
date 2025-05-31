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
    /*
    Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.ItemChestOpenSound, (Vector3)Position);
    var beboo = Game1.Instance.ChooseBeboo();
    if (beboo != null)
    {
      var score = new MiniGame1.Instance.memory.MainMenu(Game1.Instance.SoundSystem.Volume).PlayGame();
      Game1.Instance.GainTicket((int)score / 5);
      beboo.Age += ((int)score / 5) * 0.1f;
      if (score > 0) beboo.Happiness++;
      else beboo.Happiness--;
    }
    Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.ItemChestCloseSound, (Vector3)Position);
  */
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
