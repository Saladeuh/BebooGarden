using AutoUpdaterDotNET;
using BebooGarden.Interface;

namespace BebooGarden;

internal class Program
{
  private const string version = "1.3" +
    "" +
    ".0.0";

  [STAThread]
  private static void Main()
  {
#if !DEBUG
    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    Application.ThreadException += HandleThreadException;
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
    logException((Exception) e.ExceptionObject);
  }

  private static void HandleThreadException(object sender, ThreadExceptionEventArgs e)
  {
    logException(e.Exception);
  }

  
  private static void logException(Exception ex)
  {
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
    AutoUpdater.ShowSkipButton = true;
    AutoUpdater.ShowRemindLaterButton = false;
    AutoUpdater.Start("https://raw.githubusercontent.com/Saladeuh/BebooGarden/main/AutoUpdater.xml");
  }
}