using BebooGarden.GameCore;

namespace BebooGarden.Interface.ScriptedScene;

internal class UnlockSnowyMap : IWindowManager
{
  public static void Run()
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.JingleComplete);
    IWindowManager.ShowTalk("unlocksnowy");
  }
}
