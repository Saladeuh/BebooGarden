using FmodAudio;

namespace BebooGarden.GameCore.Item.MusicBox;

internal class Roll(string title, string source, uint startPCM, uint endPCM, Sound music, bool danse=false, bool lullaby=false)
{
  private string Title { get; set; } =title;
  private string Source { get; set; }=source;
  private uint StartPCM {  get; set; }=startPCM;
  private uint EndPCM { get; set; }=endPCM;
  public Sound Music { get; private set; } = music;
  public bool Danse { get; private set; } =danse;
  public bool Lullaby { get;private set; }=lullaby;
}
