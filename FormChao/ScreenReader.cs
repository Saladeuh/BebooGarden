using DavyKager;
using System;
using System.IO;

namespace BebooGarden;

internal class ScreenReader
{
  internal static bool Output(string text, bool interrupt = false)
  {
    if (text.Length == 0)
    {
      return false;
    }

    var success = Tolk.Output(text, interrupt);
    return success;
  }

  internal static void Load(string name, string version)
  {
    /*
    // Append accessibility deps (e.g. Tolk, NVDA drivers, etc.) to PATH
    var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
    var accessibilityAssembliesDir = Path.Combine("%appdata%", "XIVLauncher", "installedPlugins", name, version);
    path += $";{accessibilityAssembliesDir}";
    Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
    */
    // Load Tolk
    Tolk.TrySAPI(true);
    Tolk.Load();
  }

  internal static void Unload()
  {
    Tolk.Unload();
  }

  internal static bool IsUsingSAPI()
  {
    return Tolk.DetectScreenReader().Equals("SAPI");
  }

  internal static void Interrupt()
  {
    Tolk.Output("", true);
  }
}
