using System.Globalization;
using AutoUpdaterDotNET;
using BebooGarden.GameCore;
using BebooGarden.Interface;
using BebooGarden.Save;
using Newtonsoft.Json;

namespace BebooGarden;

internal class Program
{
  private const string DATAFILEPATH = "save.dat";
  private const string version = "0.5";
  [STAThread]
  static void Main()
  {
    ScreenReader.Load("wsh", "2");
    AutoUpdater.InstalledVersion = new Version(version);
#if !DEBUG
    AutoUpdate();
#endif
    var parameters = (LoadJson() ?? new SaveParameters());
    SetConsoleParams((parameters.Language ?? "en"));
    var mainWindow = new Form1(parameters);
    Application.Run(mainWindow);
    parameters = new SaveParameters(language: (parameters.Language ?? "en"),
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

  private static SaveParameters? LoadJson()
  {
    if (File.Exists(DATAFILEPATH))
    {
      using (StreamReader r = new(DATAFILEPATH))
      {
        string json=r.ReadToEnd();
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
          DefaultValueHandling = DefaultValueHandling.Ignore
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

  private static void WriteJson(SaveParameters parameters)
  {
    var json = JsonConvert.SerializeObject(parameters);
#if !DEBUG
    json = StringCipher.Encrypt(json, Secrets.SAVEKEY);
#endif
    File.WriteAllText(DATAFILEPATH, json);
  }
  private void AutoUpdate()
  {
    AutoUpdater.Synchronous = true;
    AutoUpdater.ShowSkipButton = false;
    AutoUpdater.ShowRemindLaterButton = false;
    AutoUpdater.Start("https://raw.githubusercontent.com/Saladeuh/BebooGarden/main/AutoUpdater.xml");
  }
}