using System.Numerics;
using BebooGarden.GameCore.Pet;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

public class Egg(string color) : IItem
{
  public override string TranslateKeyName { get; set; } = "egg.name";
  public override string TranslateKeyDescription { get; set; } = "egg.description";
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable {  get; set; }=false;
  public override Channel? Channel { get; set; }
  private string Color {  get; set; }=color;

  public override void Action() {
    Hatch();
  }
  public void Hatch()
  {
    Game.Beboo=new Beboo("Gérard", 0, DateTime.MinValue);
    Game.Map.Items.Remove(this);
    SoundLoopTimer.Stop();
  }
  public override void PlaySound()
  {
    if(Position==null) return;
    Channel = Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.EggKrakSounds, (Vector3)Position);
  }
}
