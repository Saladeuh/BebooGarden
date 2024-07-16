using BebooGarden.GameCore;
using BebooGarden.Interface.UI;
using BebooGarden.Save;

namespace BebooGarden.Interface.ScriptedScene;

internal class NewGame : IScriptedScene
{
  public static void Run(SaveParameters parameters)
  {
    IScriptedScene.ShowTalk("ui.welcome");
    var playerName = IScriptedScene.ShowTextBox("ui.yourname", 12, true);
    IScriptedScene.ShowTalk("ui.aboutyou", playerName);
    var color = IScriptedScene.ShowChoice("ui.color", Util.Colors);
    var bebooName = IScriptedScene.ShowTextBox("ui.bebooname", 12, true);
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
