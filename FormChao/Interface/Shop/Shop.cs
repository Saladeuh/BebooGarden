using BebooGarden.GameCore;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;
using FmodAudio;

namespace BebooGarden.Interface.Shop;

public class Shop
{
  private ItemsMenu ItemsMenu { get; set; }
  private RollsMenu RollsMenu { get; set; }
  public MainMenu MainShopMenu { get; set; }

  public Shop()
  {
    var itemsList = new List<Item> { new Duck(), new MusicBox(), new BouncingBoots() };
    var itemsDictionary = itemsList.ToDictionary(
        item => IGlobalActions.GetLocalizedString("shop.item", item.Name, item.Cost),
        item => item
    );
    ItemsMenu = new(IGlobalActions.GetLocalizedString("shop.items", Game.Tickets), itemsDictionary);
    RollsMenu = new(IGlobalActions.GetLocalizedString("shop.rolls", Game.Tickets), MusicBox.AllRolls.ToDictionary(roll => roll.Title, roll => roll));
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
    Game.UpdateMapPusic();
    Game.Unpause();
  }
}
