using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using FmodAudio;
using System.Numerics;

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
  public override string Name { get; } =title;
  public override string Description { get; } = source;

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
    Game1.Instance.SoundSystem.MusicTransition(Music, StartPCM, EndPCM, TimeUnit.PCM);
    if (Game1.Instance.Map != null)
    {
      Game1.Instance.Map.IsLullabyPlaying = Lullaby;
      Game1.Instance.Map.IsDansePlaying = Danse;
    }
  }

  public override void Take()
  {
    Game1.Instance.Map?.Items.Remove(this);
    if (!MusicBox.AvailableRolls.Contains(Title + Source))
    {
      //IGlobalActions.SayLocalizedString("ui.rolltake", Title, Source);
      Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.JingleStar2);
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
      if (Game1.Instance.Tickets - Cost >= 0)
      {
        Game1.Instance.Tickets -= Cost;
        Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.ShopSound);
        //IGlobalActions.SayLocalizedString("shop.buy", Name);
        MusicBox.AvailableRolls.Add(Title + Source);
      }
      else
      {
        Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.WarningSound);
        //IGlobalActions.SayLocalizedString("shop.notickets");
      }
    }
    else
    {
      Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.WarningSound);
      //IGlobalActions.SayLocalizedString("shop.alreadyroll");
    }
  }
  public override void Action() => Take();
  public override void BebooAction(Beboo beboo) => Take();

}