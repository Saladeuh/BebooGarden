namespace BebooGarden.Interface.ScriptedScene;

public class ShopUnlock : IWindowManager
{
  public static void Run()
  {
    IWindowManager.ShowTalk("shopunlock");
  }
}
