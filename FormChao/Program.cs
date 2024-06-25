using BebooGarden;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json;

namespace BebooGarden;

internal static class Program
{
  private const string DATAFILEPATH = "data.json";
  [STAThread]
  static void Main()
  {
    ScreenReader.Load("wsh", "2");
    var parameters = LoadJson();
    SetConsoleParams(parameters.Language);
    ApplicationConfiguration.Initialize();
    Application.Run(new Form1(parameters));
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
      string json = r.ReadToEnd();
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
    File.WriteAllText(DATAFILEPATH, json);
  }
}