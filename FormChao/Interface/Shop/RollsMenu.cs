using System.Windows.Forms;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.Shop;

public partial class RollsMenu : Form
{
  public RollsMenu()
  {
    var rollsMenu = new ChooseMenu<Roll> ("shop.rolls", MusicBox.AllRolls.ToDictionary(roll => roll.Title, roll => roll));
    Controls.Add(rollsMenu);
  }
}
