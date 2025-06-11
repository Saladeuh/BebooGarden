using BebooGarden.GameCore;
using BebooGarden.Content;
using BebooGarden.UI;
using CrossSpeak;
using Microsoft.Xna.Framework.Input;
using Myra.Events;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BebooGarden.Interface.UI;

public class ChooseMenu<T>
{
  private Panel _menuPanel;
  private Action<T?> _onItemSelected;
  private string _title;
  private Dictionary<string, T> _choices;
  private bool _allowCancel;
  private bool _firstClick = true;

  public ChooseMenu(string title, Dictionary<string, T> choices, Action<T?> onItemSelected, bool allowCancel = true)
  {
    _title = title;
    _choices = choices ?? new Dictionary<string, T>();
    _onItemSelected = onItemSelected;
    _allowCancel = allowCancel;
    CreateMenuPanel();
  }

  private void CreateMenuPanel()
  {
    _menuPanel = new Panel();

    VerticalStackPanel grid = new()
    {
      Spacing = 15,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    // Titre du menu
    Label titleLabel = new()
    {
      Text = _title,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    grid.Widgets.Add(titleLabel);

    // Vérifier s'il y a des choix disponibles
    if (_choices.Count > 0)
    {
      // Créer les boutons pour chaque choix
      foreach (var choice in _choices)
      {
        ConfirmButton choiceButton = new(choice.Key)
        {
          Id = $"choice_{choice.Key.Replace(" ", "_")}"
        };

        // Capturer la valeur dans une variable locale pour éviter les problèmes de closure
        var selectedValue = choice.Value;
        choiceButton.Click += (_, _) =>
        {
          OnItemSelected(selectedValue);
        };
        grid.Widgets.Add(choiceButton);
      }
    }
    // Bouton de retour/annulation (optionnel)
    if (_allowCancel)
    {
      BackButton cancelButton = new(GameText.ui_back)
      {
        Id = "cancelButton"
      };
      cancelButton.Click += (_, _) =>
      {
        OnCancel();
      };
      grid.Widgets.Add(cancelButton);
    }

    _menuPanel.Widgets.Add(grid);
  }

  private void OnEscapePressed(object? sender, GenericEventArgs<Keys> e)
  {
    if (e.Data == Keys.Escape && _allowCancel)
    {
      OnCancel();
    }
  }

  private void OnItemSelected(T item)
  {
    Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);
    Close();
    _onItemSelected?.Invoke(item);
  }

  private void OnCancel()
  {
    Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuBackSound);
    Close();
    _onItemSelected?.Invoke(default(T)); // Passer la valeur par défaut pour indiquer l'annulation
  }

  public void Show()
  {
    Game1.Instance.SwitchToScreen(GameScreen.ChooseMenu);
    if (_choices.Count == 0)
    {
      if (_allowCancel)
      {
        OnCancel();
      }
      return;
    }
    Game1.Instance._desktop.Root = _menuPanel;
    var firstChoiceButton = _menuPanel.Widgets.First()?.GetChildren()?.FirstOrDefault(w => w.Id?.StartsWith("choice_") == true);
    if (firstChoiceButton != null)
    {
      Game1.Instance._desktop.FocusedKeyboardWidget = firstChoiceButton;
    }
  }

  private void Close()
  {
    Game1.Instance._aMenuShouldBeClosed = true;
  }
}