using System.Text.RegularExpressions;
using BebooGarden.GameCore;

namespace BebooGarden.Interface.UI;

public partial class TextForm : Form
{
  private string _lastText = "";
  private bool _lastWasIncorrect;
  public string Result;

  public TextForm(string title, int maxLength, bool nameFormat)
  {
    WindowState = FormWindowState.Maximized;
    Text = title;
    TextBox = new TextBox();
    MaxLength = maxLength;
    NameFormat = nameFormat;
    TextBox.TextChanged += TextBox_modified;
    TextBox.KeyDown += TextBox_keydown;
    Controls.Add(TextBox);
  }

  private TextBox TextBox { get; }
  private int MaxLength { get; }
  private bool NameFormat { get; }

  private void TextBox_keydown(object? sender, KeyEventArgs e)
  {
    if (e.KeyCode == Keys.Enter)
    {
      if (TextBox.Text.Length == 0)
      {
        IGlobalActions.SayLocalizedString("ui.empty");
        return;
      }

      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
      Result = char.ToUpper(TextBox.Text[0]) + TextBox.Text[1..];
      Close();
    }
  }

  private void TextBox_modified(object? sender, EventArgs e)
  {
    int lastSelectorPosition = TextBox.SelectionStart;
    if (_lastText.Length == TextBox.TextLength + 1) // letter deleted
    {
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeyDeleteSound);
    }
    else if (TextBox.TextLength > MaxLength ||
             (NameFormat && !Regex.IsMatch(TextBox.Text, @"^[a-zA-Z0-9\p{L}\p{M}_-]+$") &&
              TextBox.Text != string.Empty))
    {
      _lastWasIncorrect = true;
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeyFullSound);
      TextBox.Text = _lastText;
      if (lastSelectorPosition != 0) lastSelectorPosition--;
      TextBox.SelectionStart = lastSelectorPosition;
    }
    else if (!_lastWasIncorrect)
    {
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeySound);
    }
    else
    {
      _lastWasIncorrect = false;
    }

    _lastText = TextBox.Text;
  }
}