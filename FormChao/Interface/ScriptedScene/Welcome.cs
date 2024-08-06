using BebooGarden.GameCore;
using BebooGarden.Save;

namespace BebooGarden.Interface.ScriptedScene;

internal class Welcome : IWindowManager
{
  public static void BeforeGarden(SaveParameters parameters)
  {
    IWindowManager.ShowTalk("ui.welcome");
    var playerName = IWindowManager.ShowTextBox("ui.yourname", 12, true);
    IWindowManager.ShowTalk("ui.aboutyou", true, true, playerName);
    var color = IWindowManager.ShowChoice("ui.color", Util.Colors);
    if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(color))
    {
      Game.GameWindow?.Close();
    }
    else
    {
      parameters.FavoredColor = color;
      parameters.PlayerName = playerName;
    }
  }
  public static void AfterGarden()
  {
    IWindowManager.ShowTalk("ui.welcome2");
  }
}