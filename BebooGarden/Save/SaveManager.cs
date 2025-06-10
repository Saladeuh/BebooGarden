using BebooGarden.GameCore;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BebooGarden.Save;

public class SaveManager
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
    Game1.Instance.SoundSystem.LoadMenuSounds();
    SaveParameters parameters = LoadJson() ?? new SaveParameters();
    //if (parameters.Flags.NewGame) Welcome.BeforeGarden(parameters);
    return parameters;
  }

  private static SaveParameters? LoadJson()
  {
    if (File.Exists(DATAFILEPATH))
    {
      using StreamReader r = new(DATAFILEPATH);
      string json = r.ReadToEnd();
      try
      {
#if DEBUG
        json = StringCipher.Decrypt(json, Game1.Instance.Config["Save:Key"]);
#endif
      }
      catch (FormatException)
      {
      }


      SaveParameters parameters = JsonConvert.DeserializeObject<SaveParameters>(json, Settings);
      return parameters;
    }
#if DEBUG
    //ScreenReader.Output("Nouvelle save");
#endif
    return null;
  }


  public static void WriteSave(SaveParameters parameters)
  {
    string json = JsonConvert.SerializeObject(parameters, Settings);
#if !DEBUG
    json = StringCipher.Encrypt(json, Secrets.SAVEKEY);
#endif
    File.WriteAllText(DATAFILEPATH, json);
  }
}