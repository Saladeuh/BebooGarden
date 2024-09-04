using System.Globalization;
using BebooGarden.GameCore;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.EscapeMenu;

public class Languages(string title, Dictionary<string, string> choices) : ChooseMenu<string>(title, choices)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    CultureInfo.CurrentUICulture = new CultureInfo(Result);
    IGlobalActions.UpdateLocalizer();
    IGlobalActions.SayLocalizedString("ui.languagechanged");
    Close();
  }
  protected override void Back(object? sender, EventArgs e)
  {
    base.Back(sender, e);
  }
}
