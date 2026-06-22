using BebooGarden.Content;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden.UI.ScriptedScene;

public class WelcomeScene : IScriptedScene
{
  private Panel _welcomePanel;
  private TalkDialog _letsNameYouDialog;
  private TalkDialog _aboutYouDialog;
  private bool _letsNameYouDialogShowed;
  private TalkDialog _quickTipsDialog;
  private Dialog _nameTextFieldDialog;
  private string _yourName;
  private FancyTextField _textField;
  private bool _yourNameTextBoxShowed = false;
  private string? _favoredColor = null;
  private string? _freeTime;
  private Panel _colorChoicePanel;
  private Dialog _freeTimeTextFieldDialog;
  private FancyTextField _freeTimeTextField;
  private bool _colorChoiceDialogShowed = false;
  private bool _freeTimeTextFieldDialogShowed = false;

  public WelcomeScene()
  {
    _welcomePanel = new Panel();
    _letsNameYouDialog = new TalkDialog(BebooText.ui_welcome, GameScreen.ScriptedScene);
    _aboutYouDialog = new TalkDialog(BebooText.ui_aboutyou, GameScreen.ScriptedScene);
    CreateNameTextBox();
    CreateColorChoice();
    CreateFreeTimeTextBox();
  }

  private void CreateNameTextBox()
  {
    _nameTextFieldDialog = new Dialog
    {
      Title = BebooText.ui_yourname
    };
    var stackPanel = new HorizontalStackPanel
    {
      Spacing = 8
    };
    _textField = new FancyTextField(12, true);
    StackPanel.SetProportionType(_textField, ProportionType.Fill);
    stackPanel.Widgets.Add(_textField);
    _nameTextFieldDialog.Content = stackPanel;
    _textField.KeyDown += (s, a) =>
    {
      if ((Keys)a.Data == Keys.Enter)
      {
        bool alreadyExistingName = false;
        var name = ((FancyTextField)s).Text;
        if (name.Length > 0)
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);
          _yourName = name;
        }
      }
    };
  }

  private void CreateFreeTimeTextBox()
  {
    _freeTimeTextFieldDialog = new Dialog
    {
      Title = BebooText.ui_freetime
    };
    var stackPanel = new HorizontalStackPanel
    {
      Spacing = 8
    };
    _freeTimeTextField = new FancyTextField(200);
    StackPanel.SetProportionType(_freeTimeTextField, ProportionType.Fill);
    stackPanel.Widgets.Add(_freeTimeTextField);
    _freeTimeTextFieldDialog.Content = stackPanel;
    _freeTimeTextField.KeyDown += (s, a) =>
    {
      if ((Keys)a.Data == Keys.Enter)
      {
        bool alreadyExistingName = false;
        var freeTime = ((FancyTextField)s).Text;
        if (freeTime.Length > 0)
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);
          _freeTime = freeTime;
        }
      }
    };
  }
  public void Update(GameTime gameTime)
  {
    if (Game1.Instance._currentScreen != GameScreen.ScriptedScene) return;
    if (!_letsNameYouDialogShowed)
    {
      _letsNameYouDialog.Show();
      _letsNameYouDialogShowed = true;
    }
    else if (!_yourNameTextBoxShowed && _letsNameYouDialog.Closed)
    {
      _nameTextFieldDialog.ShowModal(Game1.Instance._desktop);
      Game1.Instance._desktop.FocusedKeyboardWidget = _textField;
      _yourNameTextBoxShowed = true;
    }
    else if (!_colorChoiceDialogShowed && (_yourName != null && _yourName != String.Empty))
    {
      _nameTextFieldDialog.Close();
      Game1.Instance._desktop.Root = _colorChoicePanel;
      //_colorChoicePanel.ShowModal(Game1.Instance._desktop);
      _colorChoiceDialogShowed = true;
      var firstButton = _colorChoicePanel.FindWidgetById("color_blue");
      if (firstButton != null)
      {
        Game1.Instance._desktop.FocusedKeyboardWidget = firstButton;
      }
    }
    else if (!_freeTimeTextFieldDialogShowed && _favoredColor != null && _favoredColor != String.Empty)
    {
      _freeTimeTextFieldDialog.ShowModal(Game1.Instance._desktop);
      Game1.Instance._desktop.FocusedKeyboardWidget = _freeTimeTextField;
      _freeTimeTextFieldDialogShowed = true;
    }
  }
  private void CreateColorChoice()
  {
    _colorChoicePanel = new Panel();
      VerticalStackPanel grid = new()
      {
        Spacing = 15,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
      };
    _colorChoicePanel.Widgets.Add(grid);
    Label titleLabel = new()
    {
      Text = BebooText.ui_color,
      HorizontalAlignment = HorizontalAlignment.Center
    };
    grid.Widgets.Add(titleLabel);

    foreach (var color in Util.Colors)
    {
      ConfirmButton colorButton = new(color)
      {
        Id = $"color_{color}"
      };
      colorButton.Click += (_, _) =>
      {
        _favoredColor = color;
      };
      grid.Widgets.Add(colorButton);
    }
  }

  private void Close()
  {
    Game1.Instance.SwitchToScreen(GameScreen.game);
    Game1.Instance._scriptedScene = null;
  }
  public void Show()
  {
    Game1.Instance.SwitchToScreen(GameScreen.ScriptedScene);
    Game1.Instance._scriptedScene = this;
    Game1.Instance.SoundSystem.PlayNWelcomeMusic();
    Game1.Instance.ShowLanguageMenu(_welcomePanel, false);
  }
}
