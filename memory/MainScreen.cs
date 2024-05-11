using System.Globalization;
using System.Numerics;
using System.Text;
using FFXIVAccess;
using FmodAudio;
using memoryGame;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Localization;

namespace BoomBox;

internal class MainScreen : IGlobalConsoleActions
{
  private const string CONTENTFOLDER = "Content/sounds";
  public FmodSystem System { get; }
  public override SoundSystem SoundSystem { get; set; }
  public MainScreen(Parameters parameters)
  {
    SoundSystem = new SoundSystem(parameters.Volume);
  }
  public Parameters Run()
  {
    ConsoleKeyInfo keyinfo;
    SoundSystem.LoadMainScreen();
    SoundSystem.LoadAmbiSounds();
    var position = new Vector3(0, 0, 0);
    do
    {
      //Console.WriteLine(Localizer.GetString("welcome"));
      SoundSystem.System.Update();
      keyinfo = Console.ReadKey(true);
      switch (keyinfo.Key)
      {
        case ConsoleKey.Enter:
        case ConsoleKey.Spacebar:
          Console.Clear();
          break;
        case ConsoleKey.F1:
        case ConsoleKey.H:
          //Console.WriteLine("Aide");
          break;
        case ConsoleKey.L:
        case ConsoleKey.F5:
          bool changed = ChangeLanguageMenu();
          if (changed) Console.WriteLine("Language changed, please restart");
          break;
        case ConsoleKey.LeftArrow:
          position += new Vector3(-1, 0, 0);
          SoundSystem.System.Set3DListenerAttributes(0, position, default, in SoundSystem.Forward, in SoundSystem.Up);
          break;
        default:
          GlobalActions(keyinfo.Key);
          break;
      }
    } while (keyinfo.Key != ConsoleKey.Escape);
    return new Parameters { Volume = SoundSystem.Volume, Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName };
  }

  private bool ChangeLanguageMenu()  // bool to indicate if a n^ew language is choosed
  {
    Console.WriteLine(this.Localizer.GetString("changeLang"));
    for (int i = 0; i < SUPPORTEDLANGUAGES.Length; i++)
    {
      Console.WriteLine($"{i}: {SUPPORTEDLANGUAGES[i]}");
    }
    ConsoleKeyInfo keyinfo;
    do
    {
      keyinfo = Console.ReadKey();
      if (char.IsDigit(keyinfo.KeyChar) && int.Parse(keyinfo.KeyChar.ToString()) < SUPPORTEDLANGUAGES.Length)
      {
        CultureInfo.CurrentUICulture = new CultureInfo(SUPPORTEDLANGUAGES[Int32.Parse(keyinfo.KeyChar.ToString())]);
        return true;
      }
    } while (keyinfo.Key != ConsoleKey.Escape);
    return false;
  }
}
