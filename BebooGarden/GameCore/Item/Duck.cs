using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using FmodAudio;
using System.Numerics;

namespace BebooGarden.GameCore.Item;

internal class Duck : Item
{
  public override string Name { get; } = GameText.duck_name;
  public override string Description { get; } = GameText.duck_description;
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = true;
  public override bool IsWaterProof { get; set; } = true;
  public override Channel? Channel { get; set; }
  public override int Cost { get; set; } = 1;
  public override void Action()
  {
    PlaySound();
    if (Game1.Instance.Random.Next(101) == 1) Game1.Instance.GainTicket(1);
#if DEBUG
    Game1.Instance.GainTicket(1);
#endif
  }
  public override void BebooAction(Beboo beboo)
  {
    base.BebooAction(beboo);
    Action();
  }
  public override void PlaySound()
  {
    if (Position == null || !(Game1.Instance.Map?.Items.Contains(this) ?? false)) return;
    Channel = Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.ItemDuckSound, (Vector3)Position);
  }
}
