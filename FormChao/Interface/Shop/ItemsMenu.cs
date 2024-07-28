
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.Shop;

public partial class ItemsMenu : Form
{
  public ItemsMenu()
  {
    var itemsList = new List<Item> { new Duck(), new MusicBox() };

    var itemsDictionary = itemsList.ToDictionary(
        item => IGlobalActions.GetLocalizedString("shop.item", item.Name, item.Cost),
        item => item
    );
    var itemsMenu = new ChooseMenu<Item>("shop.items", itemsDictionary);
    Controls.Add(itemsMenu);
  }
}
