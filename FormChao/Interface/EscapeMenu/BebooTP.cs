using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.EscapeMenu;

public class BebooTP(string title, Dictionary<string, Beboo?> choices, bool closeWhenSelect = false)
  : ChooseMenu<Beboo?>(title, choices)
{
  protected override void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    if (Result.Position != null) Game.MoveOf(Result.Position - Game.PlayerPosition);
    Close();
  }
}
