using BebooGarden.GameCore;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;
using FmodAudio;
using BebooGarden.Interface.ScriptedScene;

namespace BebooGarden.Interface.EscapeMenu;

public class EscapeMenu
{
  private Inventory Inventory { get; set; }
  public MainMenu MainMenu { get; set; }

  public EscapeMenu()
  {
    Dictionary<string, Item> options = [];
    foreach (var item in Game.Inventory)
    {
      int occurences = Game.Inventory.FindAll(x => x.Name == item.Name).Count;
      string text =IGlobalActions.GetLocalizedString("inventory.item", item.Name, occurences);
      if (options.Keys.ToList().Find(x => x.Contains(item.Name)) == null && occurences == 1) options.Add(item.Name, item);
      else if (options.Keys.ToList().Find(x => x.Contains(text)) == null)
        options.Add(text, item);
    }
    Inventory=new("ui.chooseitem", options);
    MainMenu = new(IGlobalActions.GetLocalizedString("shop.title", Game.Tickets), new Dictionary<string, Form>()
    {
      {IGlobalActions.GetLocalizedString( "bag"), Inventory },
    });
  }

  public void Show()
  {
    MainMenu.ShowDialog(Game.GameWindow);
  }
}
