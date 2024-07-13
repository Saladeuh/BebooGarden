using BebooGarden.GameCore;

namespace BebooGarden.Interface.UI;

public partial class Talk : Form
{
  private int CurrentLine { get; set; }=0;
  private string[] FullText { get; set; }
  public Talk(string text)
  {
    WindowState = FormWindowState.Maximized;
    FullText = text.SplitToLines().ToArray();
    Text = FullText[CurrentLine];
    ScreenReader.Output(Text);
    CurrentLine++;
    KeyDown += KeyHandle;
  }

  private void KeyHandle(object? sender, KeyEventArgs e)
  {
    switch (e.KeyCode)
    {
      case Keys.Escape:
      case Keys.Enter:
      case Keys.Space:
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuBipSound);
        if (CurrentLine < FullText.Count())
        {
          Text=FullText[CurrentLine];
          ScreenReader.Output(Text);
          CurrentLine++;
        }
        else Close();
        break;
      default:
        ScreenReader.Output(Text);
        break;
    }
  }
}
