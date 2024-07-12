namespace BebooGarden.Save;
public class Flags
{
  public bool NewGame { get; set; } = true;
  public bool HasUnlockOnline { get; set; } = false;

  public Flags() { }

  public Flags(bool newGame, bool hasUnlockOnline)
  {
    NewGame = newGame;
    HasUnlockOnline = hasUnlockOnline;
  }
}
