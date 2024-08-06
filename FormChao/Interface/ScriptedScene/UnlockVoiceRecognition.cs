using BebooGarden.GameCore;

namespace BebooGarden.Interface.ScriptedScene;

internal class UnlockVoiceRecognition : IWindowManager
{
  public static void Run(string bebooName)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.JingleComplete);
    IWindowManager.ShowTalk("unlockvoice", true, bebooName);
  }
}
