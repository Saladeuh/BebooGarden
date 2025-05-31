using BebooGarden.Content;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using BebooGarden.UI;
using CrossSpeak;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BebooGarden;

public partial class Game1
{
  public static readonly string[] SUPPORTEDLANGUAGES = ["fr", "en", "pt-br", "pl", "vi", "zh-Hans", "de"];
  private Panel _escapeMenuPanel;
  private Panel _inventoryPanel;
  private Panel _teleportPanel;
  private Panel _bebooTPPanel;
  private Panel _languagesPanel;

  private void CreateEscapeMenu()
  {
    _escapeMenuPanel = new Panel();

    VerticalStackPanel mainGrid = new()
    {
      Spacing = 20,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    Label titleLabel = new()
    {
      Text = GameText.ui_mainmenu,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    mainGrid.Widgets.Add(titleLabel);
   
    // Espacement
    mainGrid.Widgets.Add(new Label { Text = "" });

    // Bouton Inventaire
    ConfirmButton inventoryButton = new(GameText.bag)
    {
      Id = "inventoryButton"
    };
    inventoryButton.Click += (_, _) =>
    {
      ShowInventoryMenu();
    };
    mainGrid.Widgets.Add(inventoryButton);

    // Bouton Trouver Beboo
    ConfirmButton findBebooButton = new(GameText.findbeboo)
    {
      Id = "findBebooButton"
    };
    findBebooButton.Click += (_, _) =>
    {
      ShowBebooTPMenu();
    };
    mainGrid.Widgets.Add(findBebooButton);

    // Bouton Téléportation
    ConfirmButton teleportButton = new(GameText.tp)
    {
      Id = "teleportButton"
    };
    teleportButton.Click += (_, _) =>
    {
      ShowTeleportMenu();
    };
    mainGrid.Widgets.Add(teleportButton);

    // Bouton Langues
    ConfirmButton languageButton = new(GameText.ui_language)
    {
      Id = "languageButton"
    };
    languageButton.Click += (_, _) =>
    {
      ShowLanguageMenu();
    };
    mainGrid.Widgets.Add(languageButton);

    // Bouton Retour/Fermer
    BackButton closeButton = new("Fermer")
    {
      Id = "closeButton"
    };
    closeButton.Click += (_, _) =>
    {
      CloseEscapeMenu();
    };
    mainGrid.Widgets.Add(closeButton);

    _escapeMenuPanel.Widgets.Add(mainGrid);
    _desktop.FocusedKeyboardWidget = inventoryButton;
  }

  private void ShowInventoryMenu()
  {
    _inventoryPanel = new Panel();

    VerticalStackPanel grid = new()
    {
      Spacing = 15,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    Label titleLabel = new()
    {
      Text = Game1.Instance.Inventory.Count > 0 ?
            GameText.ui_chooseitem :
            GameText.ui_emptyinventory,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    grid.Widgets.Add(titleLabel);

    if (Game1.Instance.Inventory.Count > 0)
    {
      Dictionary<string, Item> options = new Dictionary<string, Item>();

      foreach (Item item in Game1.Instance.Inventory)
      {
        int occurences = Game1.Instance.Inventory.FindAll(x => x.Name == item.Name).Count;
        string text = String.Format(GameText.inventory_item, item.Name, occurences);
        if (options.Keys.ToList().Find(x => x.Contains(item.Name)) == null && occurences == 1)
        {
          options.Add(item.Name, item);
        }
        else if (options.Keys.ToList().Find(x => x.Contains(text)) == null)
        {
          options.Add(text, item);
        }
      }

      foreach (var option in options)
      {
        ConfirmButton itemButton = new(option.Key)
        {
          Id = $"item_{option.Value.Name}"
        };
        itemButton.Click += (_, _) =>
        {
          OnItemSelected(option.Value);
        };
        grid.Widgets.Add(itemButton);
      }
    }

    BackButton backButton = new("Retour")
    {
      Id = "backToMainButton"
    };
    backButton.Click += (_, _) =>
    {
      ShowEscapeMenu();
    };
    grid.Widgets.Add(backButton);

    _inventoryPanel.Widgets.Add(grid);
    _desktop.Root = _inventoryPanel;
  }

  private void ShowTeleportMenu()
  {
    _teleportPanel = new Panel();

    VerticalStackPanel grid = new()
    {
      Spacing = 15,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    Dictionary<string, Item> tPOptions = new Dictionary<string, Item>();

    if (Game1.Instance.Map != null)
    {
      foreach (Item item in Game1.Instance.Map.Items)
      {
        List<Item> sameItems = Game1.Instance.Map.Items.FindAll(x => x.Name == item.Name);
        int occurences = sameItems.Count;

        if (tPOptions.Keys.ToList().Find(x => x.Contains(item.Name)) == null && occurences == 1)
        {
          tPOptions.Add(item.Name, item);
        }
        else if (tPOptions.Keys.ToList().Find(x => x.Contains(item.Name)) == null)
        {
          for (int i = 0; i < sameItems.Count; i++)
          {
            Item sameItem = sameItems[i];
            var text = String.Format(GameText.tp_item, item.Name, i + 1);
            tPOptions.Add(text, sameItem);
          }
        }
      }
    }

    Label titleLabel = new()
    {
      Text = tPOptions.Count > 0 ?
            GameText.ui_chooseitem :
            GameText.ui_emptymap,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    grid.Widgets.Add(titleLabel);

    foreach (var option in tPOptions)
    {
      ConfirmButton tpButton = new(option.Key)
      {
        Id = $"tp_{option.Value.Name}"
      };
      tpButton.Click += (_, _) =>
      {
        OnTeleportSelected(option.Value);
      };
      grid.Widgets.Add(tpButton);
    }

    BackButton backButton = new("Retour")
    {
      Id = "backToMainButton"
    };
    backButton.Click += (_, _) =>
    {
      ShowEscapeMenu();
    };
    grid.Widgets.Add(backButton);

    _teleportPanel.Widgets.Add(grid);
    _desktop.Root = _teleportPanel;
  }

  private void ShowBebooTPMenu()
  {
    _bebooTPPanel = new Panel();

    VerticalStackPanel grid = new()
    {
      Spacing = 15,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    Dictionary<string, Beboo> bebooTPOptions = new Dictionary<string, Beboo>();

    if (Game1.Instance.Map != null)
    {
      for (int i = 0; i < Game1.Instance.Map.Beboos.Count; i++)
      {
        bebooTPOptions.Add(Game1.Instance.Map.Beboos[i].Name, Game1.Instance.Map.Beboos[i]);
      }
    }

    Label titleLabel = new()
    {
      Text = bebooTPOptions.Count > 0 ?
            GameText.choosebeboo :
            GameText.nobeboo,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    grid.Widgets.Add(titleLabel);

    foreach (var option in bebooTPOptions)
    {
      ConfirmButton bebooButton = new(option.Key)
      {
        Id = $"beboo_{option.Value.Name}"
      };
      bebooButton.Click += (_, _) =>
      {
        OnBebooSelected(option.Value);
      };
      grid.Widgets.Add(bebooButton);
    }

    BackButton backButton = new("Retour")
    {
      Id = "backToMainButton"
    };
    backButton.Click += (_, _) =>
    {
      ShowEscapeMenu();
    };
    grid.Widgets.Add(backButton);

    _bebooTPPanel.Widgets.Add(grid);
    _desktop.Root = _bebooTPPanel;
  }

  private void ShowLanguageMenu()
  {
    _languagesPanel = new Panel();

    VerticalStackPanel grid = new()
    {
      Spacing = 15,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    Label titleLabel = new()
    {
      Text = GameText.ui_language,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    grid.Widgets.Add(titleLabel);

    Dictionary<string, string> languageOptions = new Dictionary<string, string>();
    foreach (string twoLetterLang in SUPPORTEDLANGUAGES)
    {
      languageOptions.Add(new CultureInfo(twoLetterLang).DisplayName, twoLetterLang);
    }

    foreach (var option in languageOptions)
    {
      ConfirmButton langButton = new(option.Key)
      {
        Id = $"lang_{option.Value}"
      };
      langButton.Click += (_, _) =>
      {
        OnLanguageSelected(option.Value);
      };
      grid.Widgets.Add(langButton);
    }

    BackButton backButton = new("Retour")
    {
      Id = "backToMainButton"
    };
    backButton.Click += (_, _) =>
    {
      ShowEscapeMenu();
    };
    grid.Widgets.Add(backButton);

    _languagesPanel.Widgets.Add(grid);
    _desktop.Root = _languagesPanel;
  }

  public void ShowEscapeMenu()
  {
    _desktop.Root = _escapeMenuPanel;
  }

  private void CloseEscapeMenu()
  {
    SwitchToScreen(GameScreen.game);
  }

  // Méthodes de callback à implémenter selon votre logique
  private void OnItemSelected(Item item)
  {
    // Logique de sélection d'objet
  }

  private void OnTeleportSelected(Item item)
  {
    // Logique de téléportation
  }

  private void OnBebooSelected(Beboo beboo)
  {
    // Logique de sélection de Beboo
  }

  private void OnLanguageSelected(string languageCode)
  {
    // Logique de changement de langue
  }
}