using BebooGarden.GameCore;

namespace BebooGarden.Interface.UI;

public class ChooseMenu<T> : Form
{
  public T? Result;

  public ChooseMenu(string title, Dictionary<string, T> choices, bool hasBack = true)
  {
    WindowState = FormWindowState.Maximized;
    Choices = choices;
    Label lblTitle = new();
    Text = IGlobalActions.GetLocalizedString(title);
    lblTitle.Text = IGlobalActions.GetLocalizedString(title);
    lblTitle.AutoSize = true;
    Controls.Add(lblTitle);

    for (int i = 0; i < Choices.Keys.Count; i++)
    {
      string choiceText = Choices.Keys.ElementAt(i);
      Button btnOption = new();
      btnOption.Text = choiceText;
      int max = Choices.Keys.Count;
      if (hasBack) max += 1;
      btnOption.AccessibleDescription = i + 1 + "/" + max;
      btnOption.Click += btn_Click;
      btnOption.Enter += btn_enter;
      btnOption.KeyDown += KeyHandle;
      Controls.Add(btnOption);
    }
    if (hasBack)
    {
      Button back = new();
      back.Text = IGlobalActions.GetLocalizedString("ui.back");
      back.AccessibleDescription = Choices.Keys.Count + 1 + "/" + (Choices.Keys.Count + 1);
      back.Click += Back;
      back.Enter += btn_enter;
      back.KeyDown += KeyHandle;
      Controls.Add(back);
    }
    Game.ResetKeyState();
  }
  // copy paste in MainMenu
  private void KeyHandle(object? sender, KeyEventArgs e)
  {
    Button button = sender as Button;
    var key = e.KeyCode;
    if (key == Keys.Escape || key == Keys.Back)
      Back(sender, EventArgs.Empty);
    else if (key == Keys.F4 && e.Modifiers == Keys.Alt) Back(sender, EventArgs.Empty);
    else if (Game.WASD && (key == Keys.W || key == Keys.A)
      || !Game.WASD && (key == Keys.Z || key == Keys.Q))
    {
      if (button.TabIndex > 1) Controls[button.TabIndex - 1].Focus();
      else Controls[Controls.Count - 1].Focus();
    }
    else if (Game.WASD && (key == Keys.S || key == Keys.D)
      || !Game.WASD && (key == Keys.S || key == Keys.D))
    {
      if (button.TabIndex < Controls.Count - 1) Controls[button.TabIndex + 1].Focus();
      else Controls[1].Focus();
    }
  }
  protected virtual void Back(object? sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuReturnSound);
    Result = default(T);
    Close();
  }

  protected Dictionary<string, T> Choices { get; }
  public bool CloseWhenSelect { get; }

  private void btn_enter(object? sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuBipSound);
  }

  protected virtual void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Close();
  }
}