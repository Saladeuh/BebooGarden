﻿using System.Numerics;
using BebooGarden.GameCore.Pet;
using BebooGarden.Interface.ScriptedScene;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

public class Egg(string color) : Item
{
  protected override string _translateKeyName { get; } = "egg.name";
  public override string Name { get {
      var textColor = " ";
      if (Color !="none") textColor= Color;
      return Game.GetLocalizedString(_translateKeyName, Game.GetLocalizedString(textColor)); } }
  protected override string _translateKeyDescription { get; } = "egg.description";
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
    Game.Map?.Items.Remove(this);
    SoundLoopTimer.Stop();
    BebooType bebooType = Color != "none" ? Util.GetBebooTypeByColor(Color) : Util.GetRandomBebooType();
    Sound cinematic;
    if (!Game.SoundSystem.CinematicsHatch.TryGetValue(bebooType, out cinematic)) cinematic = Game.SoundSystem.CinematicsHatch[BebooType.Base];
    Game.SoundSystem.PlayCinematic(cinematic);
    string name = NewBeboo.Run();
    int swimLevel = (Game.Map?.IsInLake(Position ?? new(0, 0, 0)) ?? false) ? 10 : 0;
    Game.Map?.Beboos.Add(new Beboo(name, bebooType, 1, DateTime.MinValue, 3, 3, swimLevel, false, 1 + (Game.Random.Next(4) / 10)) { Position = this.Position ?? new(0, 0, 0) });
    Game.Flags.NewGame = false;
    Game.UpdateMapMusic();
  }

  public override void PlaySound()
  {
    if (Position == null) return;
    Channel = Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.EggKrakSounds, (Vector3)Position);
  }
}