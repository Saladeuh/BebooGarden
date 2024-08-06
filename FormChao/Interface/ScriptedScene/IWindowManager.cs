using BebooGarden.GameCore;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.ScriptedScene;

public interface IWindowManager
{
  protected static void ShowTalk(string translateKey, bool blocking=true)
  {
    var talk = new Talk(IGlobalActions.GetLocalizedString(translateKey));
    if (blocking) talk.ShowDialog(Game.GameWindow);
    else { talk.Show();
      talk.Activate();
    }
    Game.ResetKeyState();
  }

  protected static void ShowTalk(string translateKey, bool blocking, params object[] args)
  {
    var talk = new Talk(IGlobalActions.GetLocalizedString(translateKey, args));
    if (blocking) talk.ShowDialog(Game.GameWindow);
    else { talk.Show();
      talk.Activate();
    }
    Game.ResetKeyState();
  }

  protected static string ShowTextBox(string title, int maxLength, bool nameFormat)
  {
    var texbox = new TextForm(IGlobalActions.GetLocalizedString(title), maxLength, nameFormat);
    texbox.ShowDialog(Game.GameWindow);
    return texbox.Result;
    Game.ResetKeyState();
  }

  protected static string? ShowChoice(string title, string[] choices)
  {
    var localizedChoices = new Dictionary<string, string>(choices.Select(value =>
        new KeyValuePair<string, string>(IGlobalActions.GetLocalizedString(value), value)));
    var choiceMenu = new ChooseMenu<string>(title, localizedChoices);
    choiceMenu.ShowDialog(Game.GameWindow);
    return choiceMenu.Result;
    Game.ResetKeyState();
  }

  public static T? ShowChoice<T>(string title, Dictionary<string, T> choices)
  {
    var choiceMenu = new ChooseMenu<T>(IGlobalActions.GetLocalizedString(title), choices);
    choiceMenu.ShowDialog(Game.GameWindow);
    return choiceMenu.Result;
    Game.ResetKeyState();
  }
}