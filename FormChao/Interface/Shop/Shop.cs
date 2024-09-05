using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;

namespace BebooGarden.Interface.Shop;

public class Shop
{
  private Inventory ItemsMenu { get; set; }
  private RollsMenu RollsMenu { get; set; }
  public MainMenu MainShopMenu { get; set; }

  public Shop()
  {
    List<Item> itemsList = new() { new Duck(), new MusicBox(), new BouncingBoots(), new Chest() };
    if (Game.Flags.UnlockEggInShop) itemsList.Add(new Egg("none"));
    Dictionary<string, Item> itemsDictionary = itemsList.ToDictionary(
        item => IGlobalActions.GetLocalizedString("shop.item", item.Name, item.Description, item.Cost),
        item => item
    );
    ItemsMenu = new(IGlobalActions.GetLocalizedString("shop.items", Game.Tickets), itemsDictionary);
    RollsMenu = new(IGlobalActions.GetLocalizedString("shop.rolls", Game.Tickets), MusicBox.AllRolls.ToDictionary(roll =>IGlobalActions.GetLocalizedString("shop.roll", roll.Title, roll.Source, roll.Cost), roll => roll));
    MainShopMenu = new(IGlobalActions.GetLocalizedString("shop.title", Game.Tickets), new Dictionary<string, Form>()
    {
      {IGlobalActions.GetLocalizedString( "shop.itemstitle"), ItemsMenu },
      { IGlobalActions.GetLocalizedString( "shop.rollstitle"), RollsMenu }
    });
  }


  public void Show()
  {
    Game.Pause();
    Game.SoundSystem.PlayShopMusic();
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicElevator, false);
    MainShopMenu.ShowDialog(Game.GameWindow);
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicElevator, false);
    Game.UpdateMapMusic();
    Game.Unpause();
  }
}
