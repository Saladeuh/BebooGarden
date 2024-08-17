using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.EscapeMenu;

public class Teleport(string title, Dictionary<string, Item> choices, bool closeWhenSelect = false)
  : ChooseMenu<Item>(title, choices)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    var clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    if (Result.Position != null) Game.MoveOf(Result.Position.Value - Game.PlayerPosition);
    Close();
  }
  protected override void Back(object? sender, EventArgs e)
  {
    base.Back(sender, e);
  }
}
