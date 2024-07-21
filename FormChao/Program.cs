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
    ScreenReader.Load();
    AutoUpdater.InstalledVersion = new Version(version);
#if !DEBUG
    AutoUpdate();
#endif
    var mainWindow = new Form1();
    Application.Run(mainWindow);
  }

  private static void AutoUpdate()
  {
    AutoUpdater.Synchronous = true;
    AutoUpdater.ShowSkipButton = false;
    AutoUpdater.ShowRemindLaterButton = false;
    AutoUpdater.Start("https://raw.githubusercontent.com/Saladeuh/BebooGarden/main/AutoUpdater.xml");
  }
}