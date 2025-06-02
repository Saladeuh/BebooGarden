using BebooGarden.Content;
using BebooGarden.GameCore;
using CrossSpeak;
using Microsoft.Xna.Framework.Input;
using Myra.Events;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BebooGarden.UI;

public class TalkDialog
{
  private Panel _talkPanel;
  private Label _textLabel;
  private int CurrentLine { get; set; }
  private string[] FullText { get; }

  public TalkDialog(string text)
  {
    FullText = text.SplitToLines().ToArray();
    CurrentLine = 0;
    CreateTalkPanel();
    DisplayCurrentLine();
  }

  private void CreateTalkPanel()
  {
    _talkPanel = new Panel();

    VerticalStackPanel mainGrid = new()
    {
      Spacing = 20,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    };

    _textLabel = new Label()
    {
      Text = "",
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center,
      Wrap = true
    };

    mainGrid.Widgets.Add(_textLabel);

    _talkPanel.Widgets.Add(mainGrid);
  }

  public void Next()
  {
    Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuBipSound);
    if (CurrentLine < FullText.Length - 1)
    {
      CurrentLine++;
      DisplayCurrentLine();
    }
    else
    {
      Close();
    }
  }

  public void DisplayCurrentLine()
  {
    if (CurrentLine < FullText.Length)
    {
      _textLabel.Text = FullText[CurrentLine];
      CrossSpeakManager.Instance.Output(_textLabel.Text);
    }
  }

  public void Show()
  {
    Game1.Instance._desktop.Root = _talkPanel;
    Game1.Instance._desktop.FocusedKeyboardWidget = _textLabel;
    Game1.Instance.SwitchToScreen(GameScreen.Talkdialog);
  }

  private void Close()
  {
    Game1.Instance.SwitchToScreen(GameScreen.game);
    Game1.Instance._talkDialog = null;
  }
}