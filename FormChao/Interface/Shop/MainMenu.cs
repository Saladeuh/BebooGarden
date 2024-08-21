using BebooGarden.GameCore;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.Shop;

public class MainMenu(string title, Dictionary<string, Form> choices, bool closeWhenSelect = false)
  : ChooseMenu<Form>(title, choices)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Result.ShowDialog(this);
  }
}
