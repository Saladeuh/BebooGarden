using System.Globalization;
using AutoUpdaterDotNET;
using BebooGarden.GameCore;
using BebooGarden.Interface;
using BebooGarden.Save;
using Newtonsoft.Json;

namespace BebooGarden;

internal class Program
{
  private const string version = "0.5";
  [STAThread]
  static void Main()
  {
    ScreenReader.Load("wsh", "2");
    AutoUpdater.InstalledVersion = new Version(version);
#if !DEBUG
    AutoUpdate();
#endif
    var mainWindow = new Form1();
    Application.Run(mainWindow);
    var parameters = new SaveParameters(language: (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName ?? "en"),
      volume: Game.SoundSystem.Volume,
      bebooName: mainWindow.Game.Beboo.Name,
      mood: mainWindow.Game.Beboo.Mood,
      energy: mainWindow.Game.Beboo.Energy,
      age: mainWindow.Game.Beboo.Age,
      lastPayed: DateTime.Now,
      flags: mainWindow.Game.Flags,
      playerName: mainWindow.Game.PlayerName,
      fruitsBasket: mainWindow.Game.FruitsBasket
      );
    SaveManager.WriteJson(parameters);
  }

  private static void AutoUpdate()
  {
    AutoUpdater.Synchronous = true;
    AutoUpdater.ShowSkipButton = false;
    AutoUpdater.ShowRemindLaterButton = false;
    AutoUpdater.Start("https://raw.githubusercontent.com/Saladeuh/BebooGarden/main/AutoUpdater.xml");
  }
}