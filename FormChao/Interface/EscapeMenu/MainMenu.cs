using System.Diagnostics;
using System.Globalization;
using BebooGarden.GameCore;

namespace BebooGarden.Interface.EscapeMenu;

public class MainMenu : Form
{
  public Form? Result;
  public MainMenu(string title, Dictionary<string, Form> choices)
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
      btnOption.Click += btn_Click;
      btnOption.Enter += btn_enter;
      btnOption.KeyDown += KeyHandle;
      Controls.Add(btnOption);
    }
    Button commandsButton = new()
    {
      Text = IGlobalActions.GetLocalizedString("ui.commands")
    };
    commandsButton.Click += OpenCommands;
    commandsButton.Enter += btn_enter;
    commandsButton.KeyDown += KeyHandle;
    Controls.Add(commandsButton);
    Button discordButton = new()
    {
      Text = IGlobalActions.GetLocalizedString("ui.discord")
    };
    discordButton.Click += InviteDiscord;
    discordButton.Enter += btn_enter;
    discordButton.KeyDown += KeyHandle;
    Controls.Add(discordButton);
    Button creditsButton = new()
    {
      Text = IGlobalActions.GetLocalizedString("ui.credits")
    };
    creditsButton.Click += OpenCredits;
    creditsButton.Enter += btn_enter;
    creditsButton.KeyDown += KeyHandle;
    Controls.Add(creditsButton);
    Button quitButton = new()
    {
      Text = IGlobalActions.GetLocalizedString("ui.quit")
    };
    quitButton.Click += Quit;
    quitButton.Enter += btn_enter;
    quitButton.KeyDown += KeyHandle;
    Controls.Add(quitButton);
    Button back = new();
    back.Text = IGlobalActions.GetLocalizedString("ui.back");
    back.AccessibleDescription = Choices.Keys.Count + 1 + "/" + (Choices.Keys.Count + 1);
    back.Click += Back;
    back.Enter += btn_enter;
    back.KeyDown += KeyHandle;
    Controls.Add(back);
  }

  private void Quit(object? sender, EventArgs e)
  {
    Game.GameWindow.Close();
    Back(sender, e);
  }

  private void OpenCommands(object? sender, EventArgs e)
  {
    var twoLetterLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    var langFile = Path.Combine(SoundSystem.CONTENTFOLDER, "doc", $"commands_{twoLetterLang}.html");
    var file = Path.Combine(SoundSystem.CONTENTFOLDER, "doc", "commands.html");
    if (File.Exists(langFile))
      Process.Start(new ProcessStartInfo(langFile) { UseShellExecute = true });
    else if (File.Exists(file))
      Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
  }
  private void InviteDiscord(object? sender, EventArgs e)
  {
    Process.Start(new ProcessStartInfo(Secrets.DISCORDINVITE) { UseShellExecute = true });
  }

  protected void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Result.ShowDialog(this);
    if (Result is Inventory or Teleport or Languages) Close();
  }
  private void OpenCredits(object? sender, EventArgs e)
  {
    var twoLetterLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    var langFile = Path.Combine(SoundSystem.CONTENTFOLDER, "doc", $"credits_{twoLetterLang}.html");
    var file = Path.Combine(SoundSystem.CONTENTFOLDER, "doc", "credits.html");
    if (File.Exists(langFile))
      Process.Start(new ProcessStartInfo(langFile) { UseShellExecute = true });
    else if (File.Exists(file))
      Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
  }
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
    Result = default(Form);
    Close();
  }

  protected Dictionary<string, Form> Choices { get; }

  private void btn_enter(object? sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuBipSound);
  }

}
