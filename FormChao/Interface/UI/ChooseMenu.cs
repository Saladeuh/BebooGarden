using BebooGarden.GameCore;

namespace BebooGarden.Interface.UI;

public partial class ChooseMenu<T> : Form
{
    public T? Result;

    public ChooseMenu(string title, Dictionary<string, T> choices)
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
            btnOption.AccessibleDescription = i + 1 + "/" + Choices.Keys.Count;
            btnOption.Click += btn_Click; // Attach click event handler
            btnOption.Enter += btn_enter;
            Controls.Add(btnOption);
        }
    }

    private Dictionary<string, T> Choices { get; }

    private void btn_enter(object? sender, EventArgs e)
    {
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuBipSound);
    }

    private void btn_Click(object sender, EventArgs e)
    {
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
        var clickedButton = (Button)sender;
        Result = Choices[clickedButton.Text];
        //Game.GameWindow.Show();
        Close();
    }
}