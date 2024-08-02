using BebooGarden.GameCore;

namespace BebooGarden.Interface.ScriptedScene;

public class RaceResult
{
  public static void Run(int third, int second, int first)
  {
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.RaceStopSound, false);
    Thread.Sleep(500);
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceBadSound);
    IGlobalActions.SayLocalizedString("race.third", Game.Beboos[third].Name);
    Thread.Sleep(1000);
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceGoodSound);
    IGlobalActions.SayLocalizedString("race.second", Game.Beboos[second].Name);
    Thread.Sleep(1500);
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceGoodSound);
    IGlobalActions.SayLocalizedString("race.first", Game.Beboos[first].Name);
    Thread.Sleep(500);
    if (first == 0) Game.GainTicket(2);
  }
}
