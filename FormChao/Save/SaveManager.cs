using BebooGarden.GameCore;
using BebooGarden.Interface;
using BebooGarden.Interface.ScriptedScene;
using Newtonsoft.Json;

namespace BebooGarden.Save;

internal class SaveManager
{
  private const string DATAFILEPATH = "save.dat";

  private static readonly JsonSerializerSettings Settings = new()
  {
    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
    NullValueHandling = NullValueHandling.Ignore,
    DefaultValueHandling = DefaultValueHandling.Populate,
    TypeNameHandling = TypeNameHandling.All
  };

  internal static SaveParameters LoadSave()
  {
    Game.SoundSystem.LoadMenuSounds();
    var parameters = LoadJson() ?? new SaveParameters();
    if (parameters.Flags.NewGame) NewGame.Run(parameters);
    IGlobalActions.SetAppLanguage(parameters.Language ?? "en");
    return parameters;
  }

  private static SaveParameters? LoadJson()
  {
    if (File.Exists(DATAFILEPATH))
    {
      using StreamReader r = new(DATAFILEPATH);
      var json = r.ReadToEnd();
      try
      {
        json = StringCipher.Decrypt(json, Secrets.SAVEKEY);
      }
      catch (FormatException)
      {
      }

      var parameters = JsonConvert.DeserializeObject<SaveParameters>(json, Settings);
      return parameters;
    }
#if DEBUG
    ScreenReader.Output("Nouvelle save");
#endif
    return null;
  }


  public static void WriteJson(SaveParameters parameters)
  {
    parameters.Flags.NewGame = false;
    var json = JsonConvert.SerializeObject(parameters, Settings);
#if !DEBUG
    json = StringCipher.Encrypt(json, Secrets.SAVEKEY);
#endif
    File.WriteAllText(DATAFILEPATH, json);
  }
}