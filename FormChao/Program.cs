using System.Globalization;
using BebooGarden.GameCore;
using BebooGarden.Interface;
using BebooGarden.Save;
using Newtonsoft.Json;

namespace BebooGarden;

internal static class Program
{
  private const string DATAFILEPATH = "save.dat";
  private const string version = "0.5";
  [STAThread]
  static void Main()
  {
    ScreenReader.Load("wsh", "2");
#if !DEBUG
    AutoUpdater.InstalledVersion = new Version(version); 
    AutoUpdater.Synchronous = true;
    AutoUpdater.ShowSkipButton = false;
    AutoUpdater.ShowRemindLaterButton = false;
    AutoUpdater.Start("https://raw.githubusercontent.com/Saladeuh/BebooGarden/main/AutoUpdater.xml");
#endif
    var parameters = (LoadJson() ?? new Parameters());
    SetConsoleParams((parameters.Language ?? "en"));
    var mainWindow = new Form1(parameters);
    Application.Run(mainWindow);
    parameters = new Parameters(language: (parameters.Language ?? "en"),
      volume: Game.SoundSystem.Volume,
      bebooName: mainWindow.Game.Beboo.Name,
      mood: mainWindow.Game.Beboo.Mood,
      energy: mainWindow.Game.Beboo.Energy,
      age: mainWindow.Game.Beboo.Age,
      lastPayed: DateTime.Now);
    WriteJson(parameters);
  }

  private static void SetConsoleParams(string language)
  {
    CultureInfo.CurrentUICulture = new CultureInfo(language);
  }

  private static Parameters? LoadJson()
  {
    if (File.Exists(DATAFILEPATH))
    {
      using StreamReader r = new(DATAFILEPATH);
      string json;
      try
      {
        json = StringCipher.Decrypt(r.ReadToEnd(), Secrets.SAVEKEY);
      }
      catch (FormatException)
      {
        json = r.ReadToEnd();
      }
      var parameters = JsonConvert.DeserializeObject<Parameters>(json, new JsonSerializerSettings
      { ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor, NullValueHandling = NullValueHandling.Ignore });
      return parameters;
    }
    else
    {
      return null;
    }
  }

  private static void WriteJson(Parameters parameters)
  {
    var json = JsonConvert.SerializeObject(parameters);
#if !DEBUG
    json = StringCipher.Encrypt(json, Secrets.SAVEKEY);
#endif
    File.WriteAllText(DATAFILEPATH, json);
  }
}