using BebooGarden.Content;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.UI;
using Microsoft.Xna.Framework.Input;
using Myra.Events;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BebooGarden;

public partial class Game1
{
  private Panel _mainShopPanel;
  private Panel _itemsSopPanel;
  private Panel _rollsShopPanel;

  private void CreateShopMenus()
  {
    CreateMainShopMenu();
    CreateItemsMenu();
    CreateRollsMenu();
  }

  private void CreateMainShopMenu()
  {
    _mainShopPanel = new Panel();

    VerticalStackPanel mainGrid = new()
    {
      Spacing = 20,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    Label titleLabel = new()
    {
      Text = GameText.shop_title,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    mainGrid.Widgets.Add(titleLabel);

    // Espacement
    mainGrid.Widgets.Add(new Label { Text = "" });

    ConfirmButton itemsButton = new(String.Format(GameText.shop_items, Save.Tickets))
    {
      Id = "itemsButton"
    };
    itemsButton.Click += (_, _) =>
    {
      ShowItemsMenu();
    };
    mainGrid.Widgets.Add(itemsButton);

    ConfirmButton rollsButton = new(String.Format(GameText.shop_rolls, Save.Tickets))
    {
      Id = "rollsButton"
    };
    rollsButton.Click += (_, _) =>
    {
      ShowRollsMenu();
    };
    mainGrid.Widgets.Add(rollsButton);

    BackButton closeButton = new(GameText.ui_back)
    {
      Id = "closeButton"
    };
    closeButton.Click += (_, _) =>
    {
      CloseShop();
    };
    mainGrid.Widgets.Add(closeButton);

    _mainShopPanel.Widgets.Add(mainGrid);
  }

  private void CreateItemsMenu()
  {
    _itemsSopPanel = new Panel();

    VerticalStackPanel grid = new()
    {
      Spacing = 15,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    Label titleLabel = new()
    {
      Text = GameText.shop_itemstitle,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    grid.Widgets.Add(titleLabel);

    List<Item> itemsList = new() { new Duck(), new MusicBox(), new BouncingBoots(), new Chest() };
    if (Save.Flags.UnlockEggInShop)
      itemsList.Add(new Egg("none"));

    Dictionary<string, Item> itemsOptions = itemsList.ToDictionary(
        item => string.Format(GameText.shop_item, item.Name, item.Description, item.Cost),
        item => item
    );

    if (itemsOptions.Count > 0)
    {
      foreach (var option in itemsOptions)
      {
        ConfirmButton itemButton = new(option.Key)
        {
          //Id = $"item_{option.Value.Name}"
        };
        itemButton.Click += (_, _) =>
        {
          OnItemShopSelected(option.Value);
        };
        grid.Widgets.Add(itemButton);
      }
    }
    else
    {
      Label emptyLabel = new()
      {
        Text = "y a r",
        HorizontalAlignment = HorizontalAlignment.Center
      };
      grid.Widgets.Add(emptyLabel);
    }

    BackButton backButton = new(GameText.ui_back)
    {
      Id = "backToMainButton"
    };
    backButton.Click += (_, _) =>
    {
      ShowMainShopMenu();
    };
    grid.Widgets.Add(backButton);

    _itemsSopPanel.Widgets.Add(grid);

  }

  private void CreateRollsMenu()
  {
    _rollsShopPanel = new Panel();

    VerticalStackPanel grid = new()
    {
      Spacing = 15,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    Label titleLabel = new()
    {
      Text = GameText.shop_rollstitle,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    grid.Widgets.Add(titleLabel);

    var availableRolls = MusicBox.AllRolls.Where((roll, index) =>
        !MusicBox.AvailableRolls.Contains(roll.Name + roll.Source)).ToList();

    Dictionary<string, Roll> rollsOptions = availableRolls.ToDictionary(
        roll => string.Format(GameText.shop_roll, roll.Title, roll.Source, roll.Cost),
        roll => roll
    );

    if (rollsOptions.Count > 0)
    {
      foreach (var option in rollsOptions)
      {
        ConfirmButton rollButton = new(option.Key)
        {
          //Id = $"roll_{option.Value.Name}"
        };
        rollButton.Click += (_, _) =>
        {
          OnRollSelected(option.Value);
        };
        grid.Widgets.Add(rollButton);
      }
    }
    else
    {
      Label emptyLabel = new()
      {
        Text = "ya a r",
        HorizontalAlignment = HorizontalAlignment.Center
      };
      grid.Widgets.Add(emptyLabel);
    }

    BackButton backButton = new(GameText.ui_back)
    {
      Id = "backToMainButton"
    };
    backButton.Click += (_, _) =>
    {
      ShowMainShopMenu();
    };
    grid.Widgets.Add(backButton);

    _rollsShopPanel.Widgets.Add(grid);
  }

  private void OnEscapePressed(object? sender, GenericEventArgs<Keys> e)
  {
    if (e.Data == Keys.Escape)
    {
      CloseShop();
    }
  }

  public void ShowShop()
  {
    SwitchToScreen(GameScreen.Shop);
    CreateShopMenus();
    Pause();
    SoundSystem.PlayShopMusic();
    SoundSystem.PlayCinematic(SoundSystem.CinematicElevator, false);

    ShowMainShopMenu();
  }

  private void ShowMainShopMenu()
  {
    _desktop.Root = _mainShopPanel;

    var itemsButton = _mainShopPanel.FindWidgetById("itemsButton");
    if (itemsButton != null)
    {
      _desktop.FocusedKeyboardWidget = itemsButton;
    }
  }

  private void ShowItemsMenu()
  {
    SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);

    // Recréer le menu des objets pour actualiser les données
    CreateItemsMenu();
    _desktop.Root = _itemsSopPanel;

    // Définir le focus sur le premier élément disponible
    var firstButton = _itemsSopPanel.Widgets.First()?.GetChildren()?.FirstOrDefault(w => w.Id?.StartsWith("item_") == true);
    if (firstButton != null)
    {
      _desktop.FocusedKeyboardWidget = firstButton;
    }
  }

  private void ShowRollsMenu()
  {
    SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);

    // Recréer le menu des rouleaux pour actualiser les données
   CreateRollsMenu();
    _desktop.Root = _rollsShopPanel;

    // Définir le focus sur le premier élément disponible
    var firstButton = _rollsShopPanel.Widgets.First()?.GetChildren()?.FirstOrDefault(w => w.Id?.StartsWith("roll_") == true);
    if (firstButton != null)
    {
      _desktop.FocusedKeyboardWidget = firstButton;
    }
  }

  private void OnItemShopSelected(Item item)
  {
    SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);
    item?.Buy();

    ShowItemsMenu();
  }

  private void OnRollSelected(Roll roll)
  {
    SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);
    roll?.Buy();

    // Actualiser le menu des rouleaux après l'achat
    ShowRollsMenu();
  }

  private void CloseShop()
  {
    _aMenuShouldBeClosed = true;
    SoundSystem.PlayCinematic(SoundSystem.CinematicElevator, false);
    Game1.Instance.ChangeMapMusic();
    Game1.Instance.Unpause();
    SwitchToScreen(GameScreen.game);
  }
}
