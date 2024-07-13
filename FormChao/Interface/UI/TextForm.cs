using BebooGarden.GameCore;

namespace BebooGarden.Interface.UI;

public partial class TextForm : Form
{
  private TextBox TextBox { get; set; }
  private int MaxLength { get; }
  private bool NameFormat { get; }

  public string Result;
  private string _lastText = "";
  private bool _lastWasIncorrect=false;

  public TextForm(string title, int maxLength, bool nameFormat)
  {
    WindowState = FormWindowState.Maximized;
    var lblTitle = new Label();
    Text = title;
    lblTitle.Text = title;
    lblTitle.AutoSize = true;
    Controls.Add(lblTitle);
    TextBox = new();
    MaxLength = maxLength;
    NameFormat = nameFormat;
    TextBox.TextChanged += TextBox_modified;
    TextBox.KeyDown += TextBox_keydown;
    Controls.Add(TextBox);
  }

  private void TextBox_keydown(object? sender, KeyEventArgs e)
  {
    if (e.KeyCode == Keys.Enter)
    {
      if (TextBox.Text.Length == 0) { 
        ScreenReader.Output("non vide");
        return;
      }
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
      Result = Char.ToUpper(TextBox.Text[0]) + TextBox.Text[1..];
      Close();
    }
  }

  private void TextBox_modified(object? sender, EventArgs e)
  {
    var lastSelectorPosition=TextBox.SelectionStart;
    if (_lastText.Length == TextBox.TextLength + 1) // letter deleted
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeyDeleteSound);
    else if (TextBox.TextLength > MaxLength || 
      (NameFormat && !System.Text.RegularExpressions.Regex.IsMatch(TextBox.Text, @"^[a-zA-Z0-9_-]+$") && TextBox.Text != string.Empty) )
    {
      _lastWasIncorrect = true;
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeyFullSound);
      TextBox.Text = _lastText;
      if (lastSelectorPosition != 0) lastSelectorPosition--;
      TextBox.SelectionStart = lastSelectorPosition;
    }
    else if (!_lastWasIncorrect) { Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeySound);
    }
    else _lastWasIncorrect = false;
    _lastText = TextBox.Text;
  }
}
