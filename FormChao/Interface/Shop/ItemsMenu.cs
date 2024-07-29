using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.Shop;

public class ItemsMenu(string title, Dictionary<string, Item> choices, bool closeWhenSelect = false)
  : ChooseMenu<Item>(title, choices, closeWhenSelect)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    var clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Result?.Take();
  }
}
