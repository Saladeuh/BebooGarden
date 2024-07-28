using System.Numerics;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

internal class Duck : Item
{
  protected override string _translateKeyName { get; set; } = "duck.name";
  protected override string _translateKeyDescription { get; set; } = "duck.description";
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = true;
  public override bool IsWaterProof {  get; set; } = true;
  public override Channel? Channel { get; set; }
  public override int Cost { get; set; } = 1;
  public override void Action()
  {
    PlaySound();
  }
  public override void PlaySound()
  {
    if (Position == null) return;
    Channel = Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.ItemDuckSound, (Vector3)Position);
  }
}
