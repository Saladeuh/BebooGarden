using System;
using System.Windows.Forms;
using BebooGarden.GameCore;

namespace BebooGarden.Interface;

public partial class ChooseMenu<T> : Form
{
  private Label lblTitle;
  private Button[] btnOptions;
  private Dictionary<string, T> Choices { get; set; }
  public T? Result;
  public ChooseMenu(string title, Dictionary<string, T> choices)
  {
    Choices = choices;
    lblTitle = new Label();
    lblTitle.Text = title;
    lblTitle.AutoSize = true;
    Controls.Add(lblTitle);

    btnOptions = new Button[Choices.Keys.Count];
    for (int i = 0; i < Choices.Keys.Count; i++)
    {
      var choiceText = Choices.Keys.ElementAt(i);
      var btnOption = new Button();
      btnOption.Text = choiceText;
      btnOption.AccessibleDescription = (i + 1) + " of " + Choices.Keys.Count;
      btnOption.Click += btn_Click; // Attach click event handler
      btnOption.Enter += btn_enter;
      Controls.Add(btnOption);
      btnOptions[i] = btnOption;
    }
  }
  private void btn_enter(object? sender, EventArgs e)
  {
    Game.SoundSystem.Whistle();
  }

  void btn_Click(object sender, EventArgs e)
  {
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Game.GameWindow.Show();
    Close();
  }
}