using System.Globalization;
using System.Security.Cryptography.Xml;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BebooGarden.Interface.EscapeMenu;

public class Languages(string title, Dictionary<string, string> choices) : ChooseMenu<string>(title, choices)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    var clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    CultureInfo.CurrentUICulture = new CultureInfo(Result);
    Close();
  }
  protected override void Back(object? sender, EventArgs e)
  {
    base.Back(sender, e);
  }
}
