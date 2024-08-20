using BebooGarden.GameCore.World;

namespace BebooGarden.Interface.ScriptedScene;

internal class NewBeboo : IWindowManager
{
  public static string Run()
  {
    IWindowManager.ShowTalk("ui.letsname");
    var alreadnExistingName = false;
    var name = "";
    do
    {
      alreadnExistingName = false;
      name = IWindowManager.ShowTextBox("ui.bebooname", 12, true);
      foreach (var map in Map.Maps)
      {
        foreach (var beboo in map.Value.Beboos)
        {
          alreadnExistingName = alreadnExistingName || beboo.Name == name;
        }
      }
    } while (alreadnExistingName);
    if (GameCore.Game.Flags.NewGame) IWindowManager.ShowTalk("ui.quicktips", false, name);
    return name;
  }
}