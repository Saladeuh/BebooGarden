using BebooGarden.GameCore;

namespace BebooGarden.Interface.ScriptedScene;

public class RaceResult
{
  public static void Run((int,double) third, (int,double) second, (int,double) first)
  {
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.RaceStopSound, false);
    Thread.Sleep(500);
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceBadSound);
    IGlobalActions.SayLocalizedString("race.third", Game.Beboos[third.Item1].Name, third.Item2);
    Thread.Sleep(1000);
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceGoodSound);
    IGlobalActions.SayLocalizedString("race.second", Game.Beboos[second.Item1].Name, second.Item2);
    Thread.Sleep(1500);
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.RaceGoodSound);
    IGlobalActions.SayLocalizedString("race.first", Game.Beboos[first.Item1].Name, first.Item2);
    Thread.Sleep(500);
    if (first.Item1 == 0) Game.GainTicket(2);
  }
}
