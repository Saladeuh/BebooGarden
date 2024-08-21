using AutoUpdaterDotNET;
using BebooGarden.Interface;

namespace BebooGarden;

internal class Program
{
  private const string version = "0.5";

  [STAThread]
  private static void Main()
  {
#if !DEBUG
    AppDomain.CurrentDomain.UnhandledException += HandleUnhandleException;
#endif
    ScreenReader.Load();
    AutoUpdater.InstalledVersion = new Version(version);
#if !DEBUG
    AutoUpdate();
#endif
    Form1 mainWindow = new();
    Application.Run(mainWindow);
  }

  private static void HandleUnhandleException(object sender, UnhandledExceptionEventArgs e)
  {
    Exception ex = (Exception)e.ExceptionObject;
    string filePath = @"error.log";
    using (StreamWriter writer = new(filePath, true))
    {
      writer.WriteLine("-----------------------------------------------------------------------------");
      writer.WriteLine("Date : " + DateTime.Now.ToString());
      writer.WriteLine();
      while (ex != null)
      {
        writer.WriteLine(ex.GetType().FullName);
        writer.WriteLine("Message : " + ex.Message);
        writer.WriteLine("StackTrace : " + ex.StackTrace);
        ex = ex.InnerException;
      }
    }
  }
  private static void AutoUpdate()
  {
    AutoUpdater.Synchronous = true;
    AutoUpdater.ShowSkipButton = false;
    AutoUpdater.ShowRemindLaterButton = false;
    AutoUpdater.Start("https://raw.githubusercontent.com/Saladeuh/BebooGarden/main/AutoUpdater.xml");
  }
}