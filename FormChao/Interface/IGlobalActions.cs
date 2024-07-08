using System.Globalization;
using System.Text;
using LocalizationCultureCore.StringLocalizer;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BebooGarden.Interface;

internal class IGlobalActions
{
  protected static IStringLocalizer Localizer { get; set; }
  private static readonly string[] SUPPORTEDLANGUAGES = { "fr", "en" };
  public IGlobalActions()
  {
    string twoLetterISOLanguageName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    if (!SUPPORTEDLANGUAGES.Contains(twoLetterISOLanguageName))
    {
      ChangeLanguage("en-US");
    }
    Func<string, LogLevel, bool> filterFunction = (category, logLevel) => logLevel >= LogLevel.Critical;
    ILogger logger = new Microsoft.Extensions.Logging.Console.ConsoleLogger("", filterFunction, false);
    Localizer = new JsonStringLocalizer("Content", "test", logger);
  }

  private static void ChangeLanguage(string language)
  {
    CultureInfo.CurrentUICulture = new CultureInfo(language);
  }

  public void CheckGlobalActions(Keys keyinfo)
  {
    switch (keyinfo)
    {
      case Keys.F2:
        GameCore.Game.SoundSystem.Volume -= 0.1f;
        break;
      case Keys.F3:
        GameCore.Game.SoundSystem.Volume += 0.1f;
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
  protected static string GetLocalizedString(string translationKey, params Object[] args)
  {
    return String.Format(Localizer.GetString(translationKey), args);
  }
  protected static void SayLocalizedString(string translationKey, params Object[] args, bool interrupt=false)
  {
    ScreenReader.Output(GetLocalizedString(translationKey, args), interrupt);
  }
}
