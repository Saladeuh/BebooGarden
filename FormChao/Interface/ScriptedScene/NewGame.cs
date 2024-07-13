using BebooGarden.GameCore;
using BebooGarden.Interface.UI;
using BebooGarden.Save;

namespace BebooGarden.Interface.ScriptedScene;

internal class NewGame
{
  public static void Run(SaveParameters parameters)
  {
    Dictionary<string, int> languages = new();
    for (var i = 0; i < Game.SUPPORTEDLANGUAGES.Length; i++)
    {
      languages[Game.SUPPORTEDLANGUAGES[i]] = i;
    }
    var menuLang = new ChooseMenu<int>("Choose your language", languages);
    menuLang.ShowDialog(Game.GameWindow);
    string language = Game.SUPPORTEDLANGUAGES[menuLang.Result];
    Game.SetAppLanguage(language);
    var welcome = new Talk(Game.GetLocalizedString("ui.welcome"));
    welcome.ShowDialog(Game.GameWindow);
    var textPlayerMenu = new TextForm(Game.GetLocalizedString("ui.yourname"), 12, true);
    textPlayerMenu.ShowDialog(Game.GameWindow);
    string playerName = textPlayerMenu.Result;
    var textBebooMenu = new TextForm(Game.GetLocalizedString("ui.bebooname"), 12, true);
    textBebooMenu.ShowDialog(Game.GameWindow);
    string bebooName = textBebooMenu.Result;
    if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(bebooName) || string.IsNullOrEmpty(playerName))
    {
      Game.GameWindow.Close();
    }
    else
    {
      parameters.Language = language;
      parameters.PlayerName = playerName;
      parameters.BebooName = bebooName;
      parameters.Flags.NewGame=false;
    }
  }
}
