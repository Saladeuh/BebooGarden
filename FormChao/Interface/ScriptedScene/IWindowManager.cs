using System.Linq;
using BebooGarden.GameCore;
using BebooGarden.Interface.UI;
using Newtonsoft.Json.Linq;

namespace BebooGarden.Interface.ScriptedScene;

public interface IWindowManager
{
  protected static void ShowTalk(string translateKey)
  {
    var talk = new Talk(Game.GetLocalizedString(translateKey));
    talk.ShowDialog(Game.GameWindow);
  }
  protected static void ShowTalk(string translateKey, params Object[] args)
  {
    var talk = new Talk(Game.GetLocalizedString(translateKey, args));
    talk.ShowDialog(Game.GameWindow);
  }

  protected static string ShowTextBox(string title, int maxLength, bool nameFormat)
  {
    var texbox = new TextForm(Game.GetLocalizedString(title), maxLength, nameFormat);
    texbox.ShowDialog(Game.GameWindow);
    return texbox.Result;
  }
  protected static string ShowChoice(string title, string[] choices)
  {
    var localizedChoices = new Dictionary<string, string>(choices.Select(value => new KeyValuePair<string, string>(Game.GetLocalizedString(value), value)));
    var choiceMenu = new ChooseMenu<string>(title, localizedChoices);
     choiceMenu.ShowDialog(Game.GameWindow);
    return choiceMenu.Result;
  }
  public static T ShowChoice<T>(string title, Dictionary<string, T> choices)
  {
    var choiceMenu = new ChooseMenu<T>(Game.GetLocalizedString(title), choices);
    choiceMenu.ShowDialog(Game.GameWindow);
    return choiceMenu.Result;
  }
}
