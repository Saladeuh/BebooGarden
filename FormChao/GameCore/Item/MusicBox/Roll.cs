using System.Numerics;
using BebooGarden.GameCore.Pet;
using BebooGarden.Interface;
using FmodAudio;

namespace BebooGarden.GameCore.Item.MusicBox;

internal class Roll(
    string title,
    string source,
    uint startPCM,
    uint endPCM,
    Sound music,
    bool danse = false,
    bool lullaby = false) : Item
{
  protected override string _translateKeyName { get; } = title;
  protected override string _translateKeyDescription { get; } = source;
  public override Vector3? Position { get; set; } // position null=in inventory
  public override Channel? Channel { get; set; }
  public override bool IsTakable { get; set; } = true;
  public string Title { get; } = title;
  public string Source { get; set; } = source;
  private uint StartPCM { get; } = startPCM;
  private uint EndPCM { get; } = endPCM;
  private Sound Music { get; } = music;
  public bool Danse { get; } = danse;
  public bool Lullaby { get; } = lullaby;

  public void Play()
  {
    Game.SoundSystem.MusicTransition(Music, StartPCM, EndPCM, TimeUnit.PCM);
    if (Game.Map != null)
    {
      Game.Map.IsLullabyPlaying = Lullaby;
      Game.Map.IsDansePlaying = Danse;
    }
  }

  public override void Take()
  {
    Game.Map?.Items.Remove(this);
    if (!MusicBox.AvailableRolls.Contains(Title + Source))
    {
      IGlobalActions.SayLocalizedString("ui.rolltake", Title, Source);
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.JingleStar2);
      MusicBox.AvailableRolls.Add(Title + Source);
    }
  }

  public override void PlaySound()
  {
  }
  public override void Buy()
  {
    if (!MusicBox.AvailableRolls.Contains(Title + Source))
    {
      if (Game.Tickets - Cost >= 0)
      {
        Game.Tickets -= Cost;
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.ShopSound);
        IGlobalActions.SayLocalizedString("shop.buy", Name);
        MusicBox.AvailableRolls.Add(Title + Source);
      }
      else
      {
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.WarningSound);
        IGlobalActions.SayLocalizedString("shop.notickets");
      }
    }
    else
    {
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.WarningSound);
      IGlobalActions.SayLocalizedString("shop.alreadyroll");
    }
  }
  public override void Action() => Take();
  public override void BebooAction(Beboo beboo) => Take();

}