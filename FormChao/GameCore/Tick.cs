using System.Timers;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.World;
using BebooGarden.Interface.ScriptedScene;
using BebooGarden.Minigame;

namespace BebooGarden.GameCore;

internal partial class Game
{
  private void Tick(object? _, EventArgs __)
  {
    foreach (var beboo in Map?.Beboos)
    {
      if (Map.IsLullabyPlaying) beboo.GoAsleep();
      if (beboo.Age >= 2 && !Flags.VoiceRecoPopupPrinted)
      {
        Flags.VoiceRecoPopupPrinted = true;
        UnlockVoiceRecognition.Run(beboo.Name);
      }
      else if (beboo.Age >= 3 && !Flags.UnlockSnowyMap)
      {
        Flags.UnlockSnowyMap = true;
        UnlockSnowyMap.Run();
        Map.Maps[MapPreset.snowy].AddItem(new Egg("none"), new(0, 0, 0));
        Flags.UnlockEggInShop = true;
      }
      if (beboo.SwimLevel >= 10 && !Flags.UnlockPerfectSwimming)
      {
        Flags.UnlockPerfectSwimming = true;
        UnlockSwimming.Run(beboo.Name);
        Flags.UnlockUnderwaterMap = true;
        Map.Maps[MapPreset.underwater].AddItem(new Egg("blue"), new(0, 0, 0));
      }
    }
    SoundSystem.System.Update();
  }
  private static void SetTimerForMidnight()
  {
    // Calculer le temps jusqu'à minuit
    DateTime now = DateTime.Now;
    DateTime midnight = now.Date.AddDays(1); // minuit du prochain jour
    TimeSpan timeUntilMidnight = midnight - now;

    // Créer un timer qui se déclenchera à minuit
    var timer = new System.Timers.Timer(timeUntilMidnight.TotalMilliseconds);
    timer.Elapsed += OnMidnightReached;
    timer.AutoReset = false; // ne pas réinitialiser automatiquement le timer
    timer.Start();
  }

  private static void OnMidnightReached(object? sender, ElapsedEventArgs e)
  {
    Race.TodayTries = 0;
  }
}
