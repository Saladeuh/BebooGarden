using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.EscapeMenu;

public class MainMenu(string title, Dictionary<string, Form> choices, bool closeWhenSelect = false)
  : ChooseMenu<Form>(title, choices, closeWhenSelect)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    var clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Result.ShowDialog(this);
    if (Result is Inventory) Close();
  }
}
