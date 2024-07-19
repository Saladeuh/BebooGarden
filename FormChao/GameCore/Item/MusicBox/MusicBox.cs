
using FmodAudio;

namespace BebooGarden.GameCore.Item.MusicBox;

internal class MusicBox : IItem
{
  public static Roll[] AllRolls { get; private set; }
  private uint[] AvailableRolls { get; set; }
  static MusicBox()
  {
    var files = Directory.GetFiles(SoundSystem.CONTENTFOLDER + "music/musicbox/", "*.mp3", SearchOption.AllDirectories);
    AllRolls =new Roll [files.Length];
    var i = 0;
    foreach (var file in files)
    {
      if (File.Exists(file))
      {
        Sound musicStream = Game.SoundSystem.System.CreateStream(file, Mode.Loop_Normal);
        var splittedName = (new FileInfo(file).Name).Split('-');
      
        var title = splittedName[0];
        var source = splittedName[1];
        var isDanse = new FileInfo(file).DirectoryName == "danse";
        var isLullaby = new FileInfo(file).DirectoryName == "lullaby";
        var roll = new Roll(title, source, 0, 0, musicStream, isDanse, isLullaby);
        AllRolls[i] = roll;
        i++;
      }
    }
  }
}