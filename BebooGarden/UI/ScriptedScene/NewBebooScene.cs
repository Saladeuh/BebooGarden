using BebooGarden.Content;
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

public class NewBebooScene : IScriptedScene
{
  private Beboo _beboo;
  private TalkDialog _letsNameDialog;
  private bool _letsNameDialogShowed;
  private string _bebooName = "";
  private TalkDialog _quickTipsDialog;
  private Dialog _nameTextFieldDialog;
  private FancyTextField _textField;
  private bool _tipsDialogShowed;

  public NewBebooScene(Beboo beboo)
  {
    _beboo = beboo;
    _letsNameDialog = new TalkDialog(GameText.ui_letsname, GameScreen.ScriptedScene);
    _nameTextFieldDialog = new Dialog
    {
      Title = GameText.ui_bebooname
    };
    var stackPanel = new HorizontalStackPanel
    {
      Spacing = 8
    };
    _textField = new FancyTextField(12);
    StackPanel.SetProportionType(_textField, ProportionType.Fill);
    stackPanel.Widgets.Add(_textField);
    _nameTextFieldDialog.Content = stackPanel;
    _textField.KeyDown += (s, a) =>
    {
      if ((Keys)a.Data == Keys.Enter)
      {
        bool alreadyExistingName = false;
        var name = _textField.Text;
        foreach (var map in Map.Maps)
        {
          foreach (Beboo beboo in map.Value.Beboos)
          {
            alreadyExistingName = alreadyExistingName || beboo.Name.ToLower() == name.ToLower();
          }
        }
        if (alreadyExistingName)
        {
          _nameTextFieldDialog.ShowModal(Game1.Instance._desktop);
        }
        else
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);
          _bebooName = name;
          _beboo.Name = _bebooName;
        }
      }
    };
  }
  public void Update(GameTime gameTime)
  {
    if (Game1.Instance._gameState.CurrentScreen != GameScreen.ScriptedScene) return;
    if (!_letsNameDialogShowed)
    {
      _letsNameDialog.Show();
      _letsNameDialogShowed = true;
    }
    else if (_bebooName == String.Empty)
    {
      _nameTextFieldDialog.ShowModal(Game1.Instance._desktop);
      Game1.Instance._desktop.FocusedKeyboardWidget = _textField;
    }
    else if (Game1.Instance.Save.Flags.NewGame)
    {
      _quickTipsDialog = new TalkDialog(String.Format(GameText.ui_quicktips, _bebooName), GameScreen.ScriptedScene);
      _quickTipsDialog.Show();
      _tipsDialogShowed = true;
      Game1.Instance.Save.Flags.NewGame = false;
    }
    else if (Game1.Instance.Save.Flags.NewGame && _tipsDialogShowed || _bebooName != String.Empty)
    {
      Close();
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
  }
}
