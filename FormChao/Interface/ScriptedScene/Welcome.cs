using System.Globalization;
using BebooGarden.GameCore;
using BebooGarden.Interface.UI;
using BebooGarden.Save;

namespace BebooGarden.Interface.ScriptedScene;

internal class Welcome : IWindowManager
{
  public static void BeforeGarden(SaveParameters parameters)
  {
    Game.SoundSystem.PlayNWelcomeMusic();
    Dictionary<string, string> languageOptions = new();
    foreach (string twoLetterLang in IGlobalActions.SUPPORTEDLANGUAGES)
    {
      languageOptions.Add(new CultureInfo(twoLetterLang).DisplayName, twoLetterLang);
    }
    var languages = new Languages("language", languageOptions, false);
    languages.ShowDialog(Game.GameWindow);
    IWindowManager.ShowTalk("ui.welcome");
    string playerName = IWindowManager.ShowTextBox("ui.yourname", 12, true);
    IWindowManager.ShowTalk("ui.aboutyou", true, playerName);
    string? color = IWindowManager.ShowChoice("ui.color", Util.Colors, false);
    IWindowManager.ShowTalk("ui.freetime", true);
    string freetime = IWindowManager.ShowTextBox("ui.freetimequick", 300, false);
    string? dessert = IWindowManager.ShowChoice("ui.dessert", ["chocolatekake", "icecream", "fruitsalad", "coffee"], false);
    if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(color) || string.IsNullOrEmpty(dessert) || string.IsNullOrEmpty(freetime))
    {
      Game.GameWindow?.Close();
    }
    else
    {
      parameters.FavoredColor = color;
      parameters.PlayerName = playerName;
      parameters.FreeTime = freetime;
      parameters.Dessert = dessert;
      Game.SoundSystem.MusicFadeOut();
    }
  }
  public static void AfterGarden()
  {
    IWindowManager.ShowTalk("ui.welcome2");
  }
}