using System.Globalization;
using BebooGarden.GameCore;
using BebooGarden.Interface.UI;
using LocalizationCultureCore.StringLocalizer;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BebooGarden.Interface;

internal class IGlobalActions
{
  private static IStringLocalizer Localizer { get; set; }
  public static readonly string[] SUPPORTEDLANGUAGES = ["fr", "en"];

 static IGlobalActions()
  {
    CultureInfo.CurrentUICulture = CultureInfo.InstalledUICulture;
    string twoLetterISOLanguageName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    if (!SUPPORTEDLANGUAGES.Contains(twoLetterISOLanguageName))
    {
      ChangeLanguage("en-US");
    }

    bool FilterFunction(string _, LogLevel logLevel) => logLevel >= LogLevel.Critical;
    ILogger logger = new Microsoft.Extensions.Logging.Console.ConsoleLogger("", FilterFunction, false);
    Localizer = new JsonStringLocalizer("Content", "test", logger);

  }
  public string LanguageMenu()
  {
    Dictionary<string, int> languages = new();
    for (var i = 0; i < SUPPORTEDLANGUAGES.Length; i++)
    {
      languages[SUPPORTEDLANGUAGES[i]] = i;
    }
    var menuLang = new ChooseMenu<int>("Choose your language", languages);
    menuLang.ShowDialog(Game.GameWindow);
    string language = SUPPORTEDLANGUAGES[menuLang.Result];
    return language;
  }
  private static void ChangeLanguage(string language)
  {
  }

  public void CheckGlobalActions(Keys keyinfo)
  {
    switch (keyinfo)
    {
      case Keys.F2:
        Game.SoundSystem.Volume -= 0.1f;
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.DownSound);
        break;
      case Keys.F3:
        GameCore.Game.SoundSystem.Volume += 0.1f;
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.UpSound);
        break;
        case Keys.F4:
          Game.SoundSystem.Music.Mute=!Game.SoundSystem.Music.Mute;
        break;
      case Keys.F1:
      case Keys.H:
        //Console.WriteLine("Aide");
        break;
      case Keys.L:
      case Keys.F5:
        bool changed = ChangeLanguageMenu();
        if (changed) ScreenReader.Output("Language changed, please restart");
        break;
    }
  }
  private bool ChangeLanguageMenu()  // bool to indicate if a n^ew language is choosed
  {
    /*
    ScreenReader.Output(this.Localizer.GetString("changeLang"));
    for (int i = 0; i < SUPPORTEDLANGUAGES.Length; i++)
    {
      ScreenReader.Output($"{i}: {SUPPORTEDLANGUAGES[i]}");
    }
    ConsoleKeyInfo keyinfo;
    do
    {
      if (char.IsDigit(keyinfo.KeyChar) && int.Parse(keyinfo.KeyChar.ToString()) < SUPPORTEDLANGUAGES.Length)
      {
        CultureInfo.CurrentUICulture = new CultureInfo(SUPPORTEDLANGUAGES[Int32.Parse(keyinfo.KeyChar.ToString())]);
        return true;
      }
    } while (keyinfo.Key != ConsoleKey.Escape);
    */
    return false;
  }
  public static string GetLocalizedString(string translationKey, params Object[] args)
  {
    return String.Format(Localizer.GetString(translationKey), args);
  }
  public static string GetLocalizedString(string translationKey)
  {
    return Localizer.GetString(translationKey);
  }
  public static void SayLocalizedString(string translationKey, params Object[] args)
  {
    ScreenReader.Output(GetLocalizedString(translationKey, args));
  }
  public static void SayLocalizedString(string translationKey)
  {
    ScreenReader.Output(GetLocalizedString(translationKey));
  }
  public static void SetAppLanguage(string language)
  {
    CultureInfo.CurrentUICulture = new CultureInfo(language);
  }
}