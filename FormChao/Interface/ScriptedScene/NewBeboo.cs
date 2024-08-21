using BebooGarden.GameCore.World;

namespace BebooGarden.Interface.ScriptedScene;

internal class NewBeboo : IWindowManager
{
  public static string Run()
  {
    IWindowManager.ShowTalk("ui.letsname");
    bool alreadnExistingName = false;
    string name = "";
    do
    {
      alreadnExistingName = false;
      name = IWindowManager.ShowTextBox("ui.bebooname", 12, true);
      foreach (KeyValuePair<MapPreset, Map> map in Map.Maps)
      {
        foreach (GameCore.Pet.Beboo beboo in map.Value.Beboos)
        {
          alreadnExistingName = alreadnExistingName || beboo.Name == name;
        }
      }
    } while (alreadnExistingName);
    if (GameCore.Game.Flags.NewGame) IWindowManager.ShowTalk("ui.quicktips", false, name);
    return name;
  }
}