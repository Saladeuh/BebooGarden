using System.Globalization;
using BebooGarden.GameCore;

namespace BebooGarden.Interface.UI;

public class Languages(string title, Dictionary<string, string> choices, bool hasBack=true) : ChooseMenu<string>(title, choices, hasBack)
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
