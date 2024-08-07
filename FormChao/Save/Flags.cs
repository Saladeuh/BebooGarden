namespace BebooGarden.Save;

public class Flags
{
  public Flags()
  {
  }

  public Flags(bool newGame, bool hasUnlockOnline, bool unlockShop, bool voiceRecoPopupPrinted)
  {
    NewGame = newGame;
    HasUnlockOnline = hasUnlockOnline;
    UnlockShop = unlockShop;
    VoiceRecoPopupPrinted = voiceRecoPopupPrinted;
  }

  public bool NewGame { get; set; } = true;
  public bool HasUnlockOnline { get; set; }
  public bool UnlockShop { get; set; }
  public bool VoiceRecoPopupPrinted { get; set; }
  public bool UnlockSnowyMap { get; set; }
}