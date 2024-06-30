using System.Globalization;
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
    var parameters = LoadJson();
    SetConsoleParams(parameters.Language);
    ApplicationConfiguration.Initialize();
    var mainWindow = new Form1(parameters);
    Application.Run(mainWindow);
    parameters = new Parameters
    {
      Language = parameters.Language,
      Volume = mainWindow.Game.SoundSystem.Volume,
      BebooName = mainWindow.Game.beboo.Name,
      Mood = mainWindow.Game.beboo.Mood,
      Age = mainWindow.Game.beboo.Age,
      LastPayed= DateTime.Now,
    };
    WriteJson(parameters);
  }
  public static void SetConsoleParams(string language)
  {
    if (language != null) CultureInfo.CurrentUICulture = new CultureInfo(language);
  }
  public static Parameters LoadJson()
  {
    if (File.Exists(DATAFILEPATH))
    {
      using StreamReader r = new(DATAFILEPATH);
      string json = StringCipher.Decrypt(r.ReadToEnd(), Secrets.SAVEKEY);
      Parameters parameters = JsonConvert.DeserializeObject<Parameters>(json);
      return parameters;
    }
    else
    {
      return new Parameters { Volume = 0.5f };
    }
  }
  public static void WriteJson(Parameters parameters)
  {
    var json = JsonConvert.SerializeObject(parameters);
    File.WriteAllText(DATAFILEPATH, StringCipher.Encrypt(json, Secrets.SAVEKEY));
  }
}