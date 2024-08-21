using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.Shop;

public class Inventory(string title, Dictionary<string, Item> choices, bool closeWhenSelect = false)
  : ChooseMenu<Item>(title, choices)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Result?.Buy();
  }
  protected override void Back(object? sender, EventArgs e)
  {
    base.Back(sender, e);
  }
}
