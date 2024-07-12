using BebooGarden.GameCore;

namespace BebooGarden.Interface;

public partial class TextForm : Form
{
  private Label lblTitle;
  private TextBox TextBox;
  private int MaxLength { get; }
  public string Result;
  private string _lastText = "";

  public TextForm(string title, string placeholderText, int maxLength)
  {
    lblTitle = new Label();
    Text = title;
    lblTitle.Text = title;
    lblTitle.AutoSize = true;
    Controls.Add(lblTitle);
    TextBox = new();
    MaxLength = maxLength;
    TextBox.PlaceholderText = placeholderText;
    TextBox.TextChanged += TextBox_modified;
    TextBox.KeyDown += TextBox_keydown;
    Controls.Add(TextBox);
  }

  private void TextBox_keydown(object? sender, KeyEventArgs e)
  {
    if (e.KeyCode == Keys.Enter)
    {
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
      Result = TextBox.Text;
      Close();
    }
  }

  private void TextBox_modified(object? sender, EventArgs e)
  {
    if (_lastText.Length == TextBox.TextLength + 1) // delete one letter
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeyDeleteSound);
    else if (TextBox.TextLength > MaxLength)
    {
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeyFullSound);
      TextBox.Text = _lastText;
      TextBox.SelectionStart = TextBox.Text.Length;
    }
    else Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuKeySound);
    _lastText = TextBox.Text;
  }
}
