using System;
using System.Numerics;
using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

public class Egg(string color) : Item
{
  public override string Name { get
    {
      var textColor = " ";
      return textColor;
           if (Color !="none") textColor= Color;
            return String.Format(GameText.egg_name, textColor); 
    } }
  public override string Description { get; } = GameText.egg_description;
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = false;
  public override int Cost { get; set; } = 20;
  public override bool IsWaterProof { get; set; } = true;
  public override Channel? Channel { get; set; }
  public string Color { get; } = color;

  public override void Action() => Hatch();
  public override void Take() => Hatch();
  public override void BebooAction(Beboo beboo) => Hatch();
  private void Hatch()
  {
    Game1.Instance.Map?.Items.Remove(this);
    SoundLoopTimer.Stop();
    BebooType bebooType = Color != "none" ? Util.GetBebooTypeByColor(Color) : Util.GetRandomBebooType();
    Sound cinematic;
    if (!Game1.Instance.SoundSystem.CinematicsHatch.TryGetValue(bebooType, out cinematic)) cinematic = Game1.Instance.SoundSystem.CinematicsHatch[BebooType.Base];
    Game1.Instance.SoundSystem.PlayCinematic(cinematic);
    string name = "";// NewBeboo.Run();
    int swimLevel = (Game1.Instance.Map?.IsInLake(Position ?? new(0, 0, 0)) ?? false) ? 10 : 0;
    Game1.Instance.Map?.Beboos.Add(new Beboo(name, bebooType, 1, DateTime.MinValue, 3, 3, swimLevel, false, 1 + (Game1.Instance.Random.Next(4) / 10)) { Position = this.Position ?? new(0, 0, 0) });
    Game1.Instance.Save.Flags.NewGame = false;
    Game1.Instance.UpdateMapMusic();
  }

  public override void PlaySound()
  {
    if (Position == null) return;
    Channel = Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.EggKrakSounds, (Vector3)Position);
  }
}