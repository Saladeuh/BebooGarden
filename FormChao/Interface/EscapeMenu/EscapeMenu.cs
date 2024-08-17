using BebooGarden.GameCore;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Item;
using BebooGarden.Interface.UI;
using FmodAudio;
using BebooGarden.Interface.ScriptedScene;
using System.Diagnostics;
using System.Globalization;

namespace BebooGarden.Interface.EscapeMenu;

public class EscapeMenu
{
  private Inventory Inventory { get; set; }
  private Languages Languages { get; set; }
  public MainMenu MainMenu { get; set; }

  public EscapeMenu()
  {
    Dictionary<string, Item> options = [];
    foreach (var item in Game.Inventory)
    {
      int occurences = Game.Inventory.FindAll(x => x.Name == item.Name).Count;
      string text = IGlobalActions.GetLocalizedString("inventory.item", item.Name, occurences);
      if (options.Keys.ToList().Find(x => x.Contains(item.Name)) == null && occurences == 1) options.Add(item.Name, item);
      else if (options.Keys.ToList().Find(x => x.Contains(text)) == null)
        options.Add(text, item);
    }
    Inventory = new("ui.chooseitem", options);
    var languageOptions = new Dictionary<string, string>();
    foreach(var twoLetterLang in IGlobalActions.SUPPORTEDLANGUAGES)
    {
      languageOptions.Add(new CultureInfo(twoLetterLang).DisplayName, twoLetterLang);
    }
    Languages=new("lang",  languageOptions);
    MainMenu = new(IGlobalActions.GetLocalizedString("mainmenu"), new Dictionary<string, Form>()
    {
      {IGlobalActions.GetLocalizedString( "bag"), Inventory },
    //  {IGlobalActions.GetLocalizedString( "languages"), Languages },
    });
  }


  public void Show()
  {
    MainMenu.ShowDialog(Game.GameWindow);
  }
}
