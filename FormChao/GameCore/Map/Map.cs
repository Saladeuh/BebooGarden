using System.Numerics;

namespace BebooGarden.GameCore.Map;

internal class Map
{
  public int SizeX { get; set; }
  public int SizeY { get; set; }
  public List<TreeLine> TreeLines { get; set; }

  public Map(int sizeX, int sizeY, List<TreeLine> treeLines)
  {
    SizeX = sizeX;
    SizeY = sizeY;
    TreeLines = treeLines;
  }
}
