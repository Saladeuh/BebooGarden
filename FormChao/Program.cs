using System.Globalization;
using BebooGarden.GameCore;
using BebooGarden.Interface;
using BebooGarden.Save;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BebooGarden;

internal static class Program
{
  private const string DATAFILEPATH = "save.dat";
  [STAThread]
  static void Main()
  {
    ScreenReader.Load("wsh", "2");
    var parameters = (LoadJson() ?? new Parameters());
    SetConsoleParams((parameters.Language ?? "en"));
    var mainWindow = new Form1(parameters);
    Application.Run(mainWindow);
    parameters = new Parameters(language: (parameters.Language ?? "en"),
  volume: Game.SoundSystem.Volume,
      bebooName: mainWindow.Game.beboo.Name,
      mood: mainWindow.Game.beboo.Mood,
      age: mainWindow.Game.beboo.Age,
      lastPayed: DateTime.Now);
    WriteJson(parameters);
  }
  public static void SetConsoleParams(string language)
  {
    if (language != null) CultureInfo.CurrentUICulture = new CultureInfo(language);
  }
  public static Parameters? LoadJson()
  {
    if (File.Exists(DATAFILEPATH))
    {
      using StreamReader r = new(DATAFILEPATH);
      string json = string.Empty;
      try
      {
        json = StringCipher.Decrypt(r.ReadToEnd(), Secrets.SAVEKEY);
      }
      catch (FormatException)
      {
        json = r.ReadToEnd();
      }
      var parameters = JsonConvert.DeserializeObject<Parameters>(json, new JsonSerializerSettings
      {ConstructorHandling=ConstructorHandling.AllowNonPublicDefaultConstructor, NullValueHandling=NullValueHandling.Ignore });
      return parameters;
    }
    else
    {
      return null;
    }
  }
  public static void WriteJson(Parameters parameters)
  {
    var json = JsonConvert.SerializeObject(parameters);
#if !DEBUG
    json = StringCipher.Encrypt(json, Secrets.SAVEKEY);
#endif
    File.WriteAllText(DATAFILEPATH, json);
  }
}