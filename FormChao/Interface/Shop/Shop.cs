using BebooGarden.GameCore;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.ScriptedScene;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.Shop;

public class Shop
{
  //private  ItemsMenu ItemsMenu { get; set; }
  //private RollsMenu RollsMenu { get; set; }
  public ChooseMenu<Form> MainShopMenu { get; }

  public Shop()
  {
    var itemsList = new List<Item> { new Duck(), new MusicBox() };
    var itemsDictionary = itemsList.ToDictionary(
        item => IGlobalActions.GetLocalizedString("shop.item", item.Name, item.Cost),
        item => item
    );
    var ItemsMenu = new ChooseMenu<Item>("shop.items", itemsDictionary);
    var RollsMenu = new ChooseMenu<Roll>("shop.rolls", MusicBox.AllRolls.ToDictionary(roll => roll.Title, roll => roll));
    MainShopMenu = new ChooseMenu<Form>("shop.title", new Dictionary<string, Form>()
    {
      { "shop.items", ItemsMenu },
      { "shop.rolls", RollsMenu }
    });
  }
  public void Show()
  {
    Game.SoundSystem.MusicTransition(Game.SoundSystem.ShopMusicStream, 459264, 8156722, FmodAudio.TimeUnit.PCM);
    MainShopMenu.ShowDialog(Game.GameWindow);
    if (MainShopMenu.Result != null)
    {
      MainShopMenu.Result.ShowDialog(Game.GameWindow);
    }
  }
}
