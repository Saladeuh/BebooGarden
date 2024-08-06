namespace BebooGarden.Interface.ScriptedScene;

internal class NewBeboo : IWindowManager
{
  public static string Run()
  {
    IWindowManager.ShowTalk("ui.letsname");
    var name = IWindowManager.ShowTextBox("ui.bebooname", 12, true);
    IWindowManager.ShowTalk("ui.quicktips", false, name);
    return name;
  }
}