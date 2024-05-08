using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCultureCore.StringLocalizer;
using memoryGame;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BoomBox;

internal abstract class IGlobalConsoleActions
{
  public abstract SoundSystem SoundSystem { get; set; }
  protected IStringLocalizer? Localizer { get; set; }
  public static readonly string[] SUPPORTEDLANGUAGES = { "fr", "en" };
  public IGlobalConsoleActions()
  {
    string twoLetterISOLanguageName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    if (!SUPPORTEDLANGUAGES.Contains(twoLetterISOLanguageName))
    {
      ChangeLanguage("en-US");
    }
    Func<string, LogLevel, bool> filterFunction = (category, logLevel) => logLevel >= LogLevel.Critical;
    ILogger logger = new Microsoft.Extensions.Logging.Console.ConsoleLogger("", filterFunction, false);
    Localizer = (IStringLocalizer)new JsonStringLocalizer("Content", "test", logger);
  }

  private static void ChangeLanguage(string language)
  {
    CultureInfo.CurrentUICulture = new CultureInfo(language);
  }

  protected void GlobalActions(ConsoleKey keyinfo)
  {
    switch (keyinfo)
    {
      case ConsoleKey.F2:
        SoundSystem.Volume -= 0.1f;
        break;
      case ConsoleKey.F3:
        SoundSystem.Volume += 0.1f;
        break;
    }
  }
}
