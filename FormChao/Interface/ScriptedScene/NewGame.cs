using BebooGarden.GameCore;
using BebooGarden.Interface.UI;
using BebooGarden.Save;

namespace BebooGarden.Interface.ScriptedScene;

internal class NewGame : IWindowManager
{
  public static void Run(SaveParameters parameters)
  {
    IWindowManager.ShowTalk("ui.welcome");
    var playerName = IWindowManager.ShowTextBox("ui.yourname", 12, true);
    IWindowManager.ShowTalk("ui.aboutyou", playerName);
    var color = IWindowManager.ShowChoice("ui.color", Util.Colors);
    var bebooName = IWindowManager.ShowTextBox("ui.bebooname", 12, true);
    if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(bebooName) || string.IsNullOrEmpty(color))
    {
      Game.GameWindow.Close();
    }
    else
    {
      parameters.FavoredColor=color;
      parameters.PlayerName = playerName;
      parameters.BebooName = bebooName;
    }
  }
}
