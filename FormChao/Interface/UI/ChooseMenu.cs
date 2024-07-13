using System;
using System.Windows.Forms;
using BebooGarden.GameCore;

namespace BebooGarden.Interface.UI;

public partial class ChooseMenu<T> : Form
{
  private Label lblTitle;
  private Dictionary<string, T> Choices { get; set; }
  public T? Result;
  public ChooseMenu(string title, Dictionary<string, T> choices)
  {
    WindowState = FormWindowState.Maximized;
    Choices = choices;
    lblTitle = new Label();
    Text = title;
    lblTitle.Text = title;
    lblTitle.AutoSize = true;
    Controls.Add(lblTitle);

    for (int i = 0; i < Choices.Keys.Count; i++)
    {
      var choiceText = Choices.Keys.ElementAt(i);
      var btnOption = new Button();
      btnOption.Text = choiceText;
      btnOption.AccessibleDescription = (i + 1) + " of " + Choices.Keys.Count;
      btnOption.Click += btn_Click; // Attach click event handler
      btnOption.Enter += btn_enter;
      Controls.Add(btnOption);
    }
  }
  private void btn_enter(object? sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuBipSound);
  }

  void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    //Game.GameWindow.Show();
    Close();
  }
}