using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using FmodAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden.GameCore.Item;

internal class RubberRing : Item
{
  public override string Name { get; } = BebooText.duck_description;
  public override string Description { get; } = BebooText.duck_description;
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = true;
  public override bool IsWaterProof { get; set; } = true;
  public override Channel? Channel { get; set; }
  public override int Cost { get; set; } = 3;
  public override void Action()
  { }
  public override void BebooAction(Beboo beboo)
  {
    base.BebooAction(beboo);
    beboo.RubberRingSlippedOn = true;
  }
  public override void PlaySound()
  {
    if (Position == null || !(Game1.Instance.Map?.Items.Contains(this) ?? false)) return;
    Channel = Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.ItemRubberRingSound, (Vector3)Position);
  }
}
