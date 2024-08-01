namespace BebooGarden.Save;

public class Flags
{
  public Flags()
  {
  }

  public Flags(bool newGame, bool hasUnlockOnline, bool unlockShop)
  {
    NewGame = newGame;
    HasUnlockOnline = hasUnlockOnline;
    UnlockShop = unlockShop;
  }

  public bool NewGame { get; set; } = true;
  public bool HasUnlockOnline { get; set; }
  public bool UnlockShop { get; set; }
}