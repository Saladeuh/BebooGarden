using System.Globalization;
using BebooGarden.GameCore;
using BebooGarden.Interface;
using BebooGarden.Interface.ScriptedScene;
using Newtonsoft.Json;

namespace BebooGarden.Save;

internal class SaveManager
{
  private const string DATAFILEPATH = "save.dat";
  internal static SaveParameters LoadSave()
  {
    Game.SoundSystem.LoadMenuSounds();
    var parameters = (SaveManager.LoadJson() ?? new SaveParameters());
    if (parameters.Flags.NewGame)
    {
      NewGame.Run(parameters);
    }
    Game.SetAppLanguage((parameters.Language ?? "en"));
    return parameters;
  }

  private static SaveParameters? LoadJson()
  {
    if (File.Exists(DATAFILEPATH))
    {
      using (StreamReader r = new(DATAFILEPATH))
      {
        string json = r.ReadToEnd();
        try
        {
          json = StringCipher.Decrypt(json, Secrets.SAVEKEY);
        }
        catch (FormatException)
        {
        }
        var parameters = JsonConvert.DeserializeObject<SaveParameters>(json, new JsonSerializerSettings
        {
          ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
          NullValueHandling = NullValueHandling.Ignore,
          DefaultValueHandling = DefaultValueHandling.Populate
        });
        return parameters;
      }
    }
    else
    {
#if DEBUG
      ScreenReader.Output("Nouvelle save");
#endif
      return null;
    }
  }

  
  public static void WriteJson(SaveParameters parameters)
  {
    var json = JsonConvert.SerializeObject(parameters);
#if !DEBUG
    json = StringCipher.Encrypt(json, Secrets.SAVEKEY);
#endif
    File.WriteAllText(DATAFILEPATH, json);
  }

}
