using System.Numerics;
using BebooGarden.Interface.ScriptedScene;
using FmodAudio;

namespace BebooGarden.GameCore.Item.MusicBox;

internal class MusicBox : IItem
{
  public string TranslateKeyName { get; set; } = "musicbox.name";
  public string TranslateKeyDescription { get; set; } = "musicbox.description";
  public Vector3? Position { get; set; } = null;
  public static Roll[] AllRolls { get; private set; }
  public static List<string> AvailableRolls { get; set; } = new();
  static MusicBox()
  {
    var files = Directory.GetFiles(SoundSystem.CONTENTFOLDER + "music/musicbox/", "*.mp3", SearchOption.AllDirectories);
    AllRolls = new Roll[files.Length];
    var i = 0;
    foreach (var file in files)
    {
      if (File.Exists(file))
      {
        Sound musicStream = Game.SoundSystem.System.CreateStream(file, Mode.Loop_Normal);
        FileInfo fileInfo = new FileInfo(file);
        var splittedName = fileInfo.Name.Split('-');

        var title = splittedName[0].Trim();
        var source = splittedName[1].Trim().Replace(".mp3", "");
        var lastDirectoryName = new Uri(fileInfo.DirectoryName).Segments.Last();
        var isDanse = lastDirectoryName == "danse";
        var isLullaby = lastDirectoryName == "lullaby";
        GetLoopValues(fileInfo.FullName.Replace(".mp3", ".txt"), out uint startLoop, out uint endLoop);
        var roll = new Roll(title, source, startLoop, endLoop, musicStream, isDanse, isLullaby);
        AllRolls[i] = roll;
        i++;
      }
    }
  }
  static void GetLoopValues(string filePath, out uint loopStart, out uint loopEnd)
  {
    loopStart = 0;
    loopEnd = 0;
    if (File.Exists(filePath))
    {
      try
      {
        foreach (var line in File.ReadLines(filePath))
        {
          if (line.StartsWith("LOOP_START:"))
          {
            loopStart = uint.Parse(line.Split(':')[1].Trim());
          }
          else if (line.StartsWith("LOOP_END:"))
          {
            loopEnd = uint.Parse(line.Split(':')[1].Trim());
          }
        }
      }
      catch { }
    }
  }
  public void Action()
  {
    if (AvailableRolls.Count > 0)
    {
      Dictionary<string, Roll> rollDictionary = new();
      foreach (var rollName in MusicBox.AvailableRolls)
      {
        var roll = Array.Find<Roll>(AllRolls, roll => roll.Title + roll.Source == rollName);
        rollDictionary.Add(roll.Title, roll);
      }
      var choosedRoll = IWindowManager.ShowChoice<Roll>(Game.GetLocalizedString("ui.chooseroll"), rollDictionary);
      choosedRoll.Play();
    }
    else
    {
      Game.SayLocalizedString("musicbox.noroll");
    }
  }
}