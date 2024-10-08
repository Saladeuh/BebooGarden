﻿using System.Globalization;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface.ScriptedScene;
using BebooGarden.Interface.UI;

namespace BebooGarden.Interface.EscapeMenu;

public class EscapeMenu
{
  private Inventory Inventory { get; set; }
  private Teleport Teleport { get; set; }
  private BebooTP BebooTP { get; set; }
  private Languages Languages { get; set; }
  public MainMenu MainMenu { get; set; }

  public EscapeMenu()
  {
    Dictionary<string, Item> options = [];
    if (Game.Inventory.Count > 0)
    {
      foreach (Item item in Game.Inventory)
      {
        int occurences = Game.Inventory.FindAll(x => x.Name == item.Name).Count;
        string text = IGlobalActions.GetLocalizedString("inventory.item", item.Name, occurences);
        if (options.Keys.ToList().Find(x => x.Contains(item.Name)) == null && occurences == 1) options.Add(item.Name, item);
        else if (options.Keys.ToList().Find(x => x.Contains(text)) == null)
          options.Add(text, item);
      }
      Inventory = new("ui.chooseitem", options);
    }
    else Inventory = new("emptybag", options);
    Dictionary<string, Item> tPOptions = [];
    if (Game.Map != null)
    {
      foreach (Item item in Game.Map?.Items)
      {
        List<Item> sameItems = Game.Map.Items.FindAll(x => x.Name == item.Name);
        int occurences = sameItems.Count;
        if (tPOptions.Keys.ToList().Find(x => x.Contains(item.Name)) == null && occurences == 1) tPOptions.Add(item.Name, item);
        else if (tPOptions.Keys.ToList().Find(x => x.Contains(item.Name)) == null)
        {
          for (int i = 0; i < sameItems.Count; i++)
          {
            Item sameItem = sameItems[i];
            string text = IGlobalActions.GetLocalizedString("tp.item", item.Name, i + 1); options.Add(text, item);
            tPOptions.Add(text, sameItem);
          }
        }
      }
    }
    if (tPOptions.Count > 0)
      Teleport = new("ui.chooseitem", tPOptions);
    else Teleport = new("ui.emptymap", tPOptions);
    Dictionary<string, Beboo> bebooTPOptions = [];
    for (int i = 0; i < Game.Map.Beboos.Count; i++)
      bebooTPOptions.Add(Game.Map.Beboos[i].Name, Game.Map.Beboos[i]);
    if (bebooTPOptions.Count > 0)
      BebooTP = new("choosebeboo", bebooTPOptions);
    else BebooTP = new("nobeboo", bebooTPOptions);
    Dictionary<string, string> languageOptions = new();
    foreach (string twoLetterLang in IGlobalActions.SUPPORTEDLANGUAGES)
    {
      languageOptions.Add(new CultureInfo(twoLetterLang).DisplayName, twoLetterLang);
    }
    Languages = new("lang", languageOptions);
    MainMenu = new(IGlobalActions.GetLocalizedString("ui.mainmenu"), new Dictionary<string, Form>()
    {
      {IGlobalActions.GetLocalizedString("bag"), Inventory },
      {IGlobalActions.GetLocalizedString( "findbeboo"), BebooTP },
      {IGlobalActions.GetLocalizedString( "tp"), Teleport },
      {IGlobalActions.GetLocalizedString( "ui.language"), Languages },
    });
  }

  public void Show()
  {
    MainMenu.ShowDialog(Game.GameWindow);
  }
}
