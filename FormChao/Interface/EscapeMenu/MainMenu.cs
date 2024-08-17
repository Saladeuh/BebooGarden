using System.Diagnostics;
using BebooGarden.GameCore;

namespace BebooGarden.Interface.EscapeMenu;

public class MainMenu : Form
{
  public Form? Result;
  public MainMenu(string title, Dictionary<string, Form> choices)
  {
    WindowState = FormWindowState.Maximized;
    Choices = choices;
    var lblTitle = new Label();
    Text = IGlobalActions.GetLocalizedString(title);
    lblTitle.Text = IGlobalActions.GetLocalizedString(title);
    lblTitle.AutoSize = true;
    Controls.Add(lblTitle);

    for (var i = 0; i < Choices.Keys.Count; i++)
    {
      var choiceText = Choices.Keys.ElementAt(i);
      var btnOption = new Button();
      btnOption.Text = choiceText;
      btnOption.Click += btn_Click;
      btnOption.Enter += btn_enter;
      btnOption.KeyDown += KeyHandle;
      Controls.Add(btnOption);
    }
    var websiteButton = new Button
    {
      Text = "website"
    };
    websiteButton.Click += OpenWiebsite;
    websiteButton.Enter += btn_enter;
    websiteButton.KeyDown += KeyHandle;
    Controls.Add(websiteButton);
    var discordButton = new Button
    {
      Text = "Discord"
    };
    discordButton.Click += InviteDiscord;
    discordButton.Enter += btn_enter;
    discordButton.KeyDown += KeyHandle;
    Controls.Add(discordButton);
    var creditsButton = new Button
    {
      Text = "credits"
    };
    creditsButton.Click += OpenCredits;
    creditsButton.Enter += btn_enter;
    creditsButton.KeyDown += KeyHandle;
    Controls.Add(creditsButton);
    var back = new Button();
    back.Text = IGlobalActions.GetLocalizedString("ui.back");
    back.AccessibleDescription = Choices.Keys.Count + 1 + "/" + (Choices.Keys.Count + 1);
    back.Click += Back;
    back.Enter += btn_enter;
    back.KeyDown += KeyHandle;
    Controls.Add(back);
    Game.ResetKeyState();
  }

  private void OpenCredits(object? sender, EventArgs e)
  {
    Process.Start(new ProcessStartInfo("save.txt") { UseShellExecute = true });
  }
  private void InviteDiscord(object? sender, EventArgs e)
  {
    Process.Start(new ProcessStartInfo(Secrets.DISCORDINVITE) { UseShellExecute = true });
  }

  protected void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    var clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Result.ShowDialog(this);
    if (Result is Inventory) Close();
  }
  private void OpenWiebsite(object? sender, EventArgs e)
  {
    Process.Start(new ProcessStartInfo("https://www.example.com") { UseShellExecute = true });
  }
  private void KeyHandle(object? sender, KeyEventArgs e)
  {
    switch (e.KeyCode)
    {
      case Keys.Escape:
      case Keys.Back:
        Back(sender, EventArgs.Empty);
        break;
      case Keys.F4:
        if (e.Modifiers == Keys.Alt) Back(sender, EventArgs.Empty);
        break;
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
