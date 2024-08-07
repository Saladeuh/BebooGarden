using System.Numerics;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

internal class BouncingBoots : Item
{
  protected override string _translateKeyName { get; } = "boots.name";
  protected override string _translateKeyDescription { get; } = "boots.description";
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = true;
  public override bool IsWaterProof {  get; set; } = false;
  public override Channel? Channel { get; set; }
  public override int Cost { get; set; } = 8;
  public override void Action()
  {
  }
  public override void BebooAction()
  {
    base.BebooAction();
    Game.Beboos[0].BootsSlippedOn = true;
  }
  public override void PlaySound()
  {
  }
}
