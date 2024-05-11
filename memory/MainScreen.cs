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
        case ConsoleKey.LeftArrow:
          SoundSystem.MoveOf(new Vector3(-1, 0, 0));
          break;
        default:
          GlobalActions(keyinfo.Key);
          break;
      }
    } while (keyinfo.Key != ConsoleKey.Escape);
    return new Parameters { Volume = SoundSystem.Volume, Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName };
  }

}
