using BebooGarden.GameCore;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.ScriptedScene;

public interface IWindowManager
{
  protected static void ShowTalk(string translateKey, bool blocking = true)
  {
    Talk talk = new(IGlobalActions.GetLocalizedString(translateKey));
    if (blocking) talk.ShowDialog(Game.GameWindow);
    else
    {
      talk.Show();
      talk.Activate();
    }
  }

  protected static void ShowTalk(string translateKey, bool blocking, params object[] args)
  {
    Talk talk = new(IGlobalActions.GetLocalizedString(translateKey, args));
    if (blocking) talk.ShowDialog(Game.GameWindow);
    else
    {
      talk.Show();
      talk.Activate();
    }
  }

  protected static string ShowTextBox(string title, int maxLength, bool nameFormat)
  {
    TextForm texbox = new(IGlobalActions.GetLocalizedString(title), maxLength, nameFormat);
    texbox.ShowDialog(Game.GameWindow);
    return texbox.Result;
  }

  protected static string? ShowChoice(string title, string[] choices, bool back = true)
  {
    Dictionary<string, string> localizedChoices = new(choices.Select(value =>
        new KeyValuePair<string, string>(IGlobalActions.GetLocalizedString(value), value)));
    ChooseMenu<string> choiceMenu = new(title, localizedChoices, back);
    choiceMenu.ShowDialog(Game.GameWindow);
    return choiceMenu.Result;
  }

  public static T? ShowChoice<T>(string title, Dictionary<string, T> choices)
  {
    ChooseMenu<T> choiceMenu = new(IGlobalActions.GetLocalizedString(title), choices);
    choiceMenu.ShowDialog(Game.GameWindow);
    return choiceMenu.Result;
  }
}