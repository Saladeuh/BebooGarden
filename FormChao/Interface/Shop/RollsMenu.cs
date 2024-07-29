using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.Shop;

internal class RollsMenu(string title, Dictionary<string, Roll> choices, bool closeWhenSelect = false)
  : ChooseMenu<Roll>(title, choices, closeWhenSelect)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    var clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Result?.Take();
  }
}
