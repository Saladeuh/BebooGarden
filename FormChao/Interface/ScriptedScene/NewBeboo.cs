namespace BebooGarden.Interface.ScriptedScene;

internal class NewBeboo : IWindowManager
{
  public static string Run()
  {
    return IWindowManager.ShowTextBox("ui.bebooname", 12, true);
  }
}