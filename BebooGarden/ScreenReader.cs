using CrossSpeak;
using DavyKager;
using System;
using System.IO;

namespace BebooGarden;

internal class ScreenReader
{
  internal static bool Output(string text, bool interrupt = false)
  {
    if (text.Length == 0) return false;

    bool success = CrossSpeakManager.Instance.Output(text, interrupt);
    return success;
  }

  internal static void Load()
  {
    // Append accessibility deps (e.g. Tolk, NVDA drivers, etc.) to PATH
    string? path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
    //var accessibilityAssembliesDir = Path.Combine("%appdata%", "XIVLauncher", "installedPlugins", name, version);
    string accessibilityAssembliesDir = Path.Combine("lib", "screen-reader-libs", "windows");
    path += $";{accessibilityAssembliesDir}";
    string fmodAssembliesDir = Path.Combine("lib");
    path += $";{fmodAssembliesDir}";
    Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
    CrossSpeakManager.Instance.PreferSAPI(CrossSpeakManager.Instance.DetectScreenReader() == "");
    CrossSpeakManager.Instance.TrySAPI(true);
    CrossSpeakManager.Instance.Initialize();
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