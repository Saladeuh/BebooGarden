using BebooGarden.GameCore;

namespace BebooGarden.Interface;

public partial class Form1 : Form
{
    public Form1()
    {
        WindowState = FormWindowState.Maximized;
        Text = "Jardin Bébous";
        Load += onLoad;
    }

    internal Game Game { get; set; }

    private void onLoad(object? sender, EventArgs e)
    {
        Game = new Game(this);
        KeyDown += Game.KeyDownMapper;
        KeyUp += Game.KeyUpMapper;
        FormClosing += Game.Close;
    }

    public void DisableInput()
    {
        Game.GameWindow.KeyDown -= Game.KeyDownMapper;
        Game.GameWindow.KeyUp -= Game.KeyUpMapper;
    }

    public void EnableInput()
    {
        Game.GameWindow.KeyDown += Game.KeyDownMapper;
        Game.GameWindow.KeyUp += Game.KeyUpMapper;
    }
}