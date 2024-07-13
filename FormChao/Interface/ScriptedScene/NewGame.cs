using BebooGarden.GameCore;
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
    var menuLang = new ChooseMenu<int>("test", languages);
    menuLang.ShowDialog(Game.GameWindow);
    string language = Game.SUPPORTEDLANGUAGES[menuLang.Result];
    var textPlayerMenu = new TextForm("Votre pseudo", "Pseudo", 12);
    textPlayerMenu.ShowDialog(Game.GameWindow);
    string playerName = textPlayerMenu.Result;
    var textBebooMenu = new TextForm("Nom du bébou", "", 12);
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
