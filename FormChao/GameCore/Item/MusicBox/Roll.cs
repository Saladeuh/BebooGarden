using System.Numerics;
using FmodAudio;

namespace BebooGarden.GameCore.Item.MusicBox;

internal class Roll(string title, string source, uint startPCM, uint endPCM, Sound music, bool danse = false, bool lullaby = false) : IItem
{
  public string TranslateKeyName { get; set; } = "roll.name";
  public string TranslateKeyDescription { get; set; } = "roll.description";
  public Vector3? Position { get; set; } // position null=in inventory
  public string Title { get; private set; } = title;
  public string Source { get; set; } = source;
  private uint StartPCM { get; set; } = startPCM;
  private uint EndPCM { get; set; } = endPCM;
  public Sound Music { get; private set; } = music;
  public bool Danse { get; private set; } = danse;
  public bool Lullaby { get; private set; } = lullaby;

  public void Play()
  {
    Game.SoundSystem.MusicTransition(Music, StartPCM, EndPCM, TimeUnit.PCM);
    Game.Map.IsLullabyPlaying = Lullaby;
    Game.Map.IsDansePlaying = Danse;
  }
  public void Take()
  {
    Game.Map.Items.Remove(this);
    if (!MusicBox.AvailableRolls.Contains(Title + Source))
    {
      Game.SayLocalizedString("ui.rolltake", Title, Source);
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.JingleStar2);
      MusicBox.AvailableRolls.Add(Title + Source);
    }
  }
}
