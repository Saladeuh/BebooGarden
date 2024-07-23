namespace BebooGarden.Save;

public class Flags
{
    public Flags()
    {
    }

    public Flags(bool newGame, bool hasUnlockOnline)
    {
        NewGame = newGame;
        HasUnlockOnline = hasUnlockOnline;
    }

    public bool NewGame { get; set; } = true;
    public bool HasUnlockOnline { get; set; }
}