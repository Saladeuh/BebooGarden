using System.Numerics;
using BebooGarden.GameCore.Pet;
using BebooGarden.Interface.ScriptedScene;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

public class Egg(string color) : IItem
{
  public override string TranslateKeyName { get; set; } = "egg.name";
  public override string TranslateKeyDescription { get; set; } = "egg.description";
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = false;
  public override Channel? Channel { get; set; }
  private string Color { get; set; } = color;

  public override void Action()
  {
    Hatch();
  }
  public void Hatch()
  {
    Game.Map.Items.Remove(this);
    SoundLoopTimer.Stop();
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicHatch);
    var name = NewBeboo.Run();
    Game.Beboo = new Beboo(name, 0, DateTime.MinValue);
  }
  public override void PlaySound()
  {
    if (Position == null) return;
    Channel = Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.EggKrakSounds, (Vector3)Position);
  }
}
