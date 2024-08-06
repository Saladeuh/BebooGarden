using System.Numerics;
using BebooGarden.Interface;
using BebooGarden.Interface.ScriptedScene;

using FmodAudio;

namespace BebooGarden.GameCore.Item.MusicBox;

internal class MusicBox : Item
{
  static MusicBox()
  {
    var files = Directory.GetFiles(SoundSystem.CONTENTFOLDER + "music/musicbox/", "*.mp3",
        SearchOption.AllDirectories);
    AllRolls = new Roll[files.Length];
    var i = 0;
    foreach (var file in files)
      if (File.Exists(file))
      {
        Sound musicStream = Game.SoundSystem.System.CreateStream(file, Mode.Loop_Normal);
        var fileInfo = new FileInfo(file);
        var splittedName = fileInfo.Name.Split('-');

        var title = splittedName[0].Trim();
        var source = splittedName[1].Trim().Replace(".mp3", "");
        if (fileInfo.DirectoryName != null)
        {
          var lastDirectoryName = new Uri(fileInfo.DirectoryName).Segments.Last();
          var isDanse = lastDirectoryName == "danse";
          var isLullaby = lastDirectoryName == "lullaby";
          GetLoopValues(fileInfo.FullName.Replace(".mp3", ".txt"), out var startLoop, out var endLoop);
          var roll = new Roll(title, source, startLoop, endLoop, musicStream, isDanse, isLullaby);
          AllRolls[i] = roll;
        }

        i++;
      }
  }

  protected override string _translateKeyName { get; } = "musicbox.name";
  protected override string _translateKeyDescription { get; } = "musicbox.description";
  public override Vector3? Position { get; set; } = null;
  public override Channel? Channel { get; set; }
  public override bool IsTakable { get; set; } = true;
  public override int Cost { get; set; } = 3;
  public static Roll[] AllRolls { get; }
  public static List<string> AvailableRolls { get; set; } = []; //title+source

  private static void GetLoopValues(string filePath, out uint loopStart, out uint loopEnd)
  {
    loopStart = 0;
    loopEnd = 0;
    if (File.Exists(filePath))
      try
      {
        foreach (var line in File.ReadLines(filePath))
          if (line.StartsWith("LOOP_START:"))
            loopStart = uint.Parse(line.Split(':')[1].Trim());
          else if (line.StartsWith("LOOP_END:")) loopEnd = uint.Parse(line.Split(':')[1].Trim());
      }
      catch
      {
      }
  }

  public override void Action()
  {
    if (AvailableRolls.Count > 0)
    {
      Dictionary<string, Roll> rollDictionary = [];
      foreach (var rollName in AvailableRolls)
      {
        var roll = Array.Find(AllRolls, roll => roll.Title + roll.Source == rollName);
        if (roll != null) rollDictionary.Add(roll.Title, roll);
      }

      var choosedRoll =
          IWindowManager.ShowChoice(IGlobalActions.GetLocalizedString("ui.chooseroll"), rollDictionary);
      if (choosedRoll != null) choosedRoll.Play();
      else Game.UpdateMapPusic();
    }
    else
    {
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.WarningSound);
      IGlobalActions.SayLocalizedString("musicbox.noroll");
    }
  }
  public override void BebooAction()
  {
    base.BebooAction();
    var rollName = AvailableRolls[Game.Random.Next(AvailableRolls.Count)];
    var roll = Array.Find(AllRolls, roll => roll.Title + roll.Source == rollName);
    roll?.Play();
  }
  public override void PlaySound()
  {
  }
}