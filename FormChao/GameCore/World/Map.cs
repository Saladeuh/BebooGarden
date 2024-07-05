using System.Numerics;

namespace BebooGarden.GameCore.World;

internal class Map
{
  public int SizeX { get; set; }
  public int SizeY { get; set; }
  public List<TreeLine> TreeLines { get; }
  public Vector3 WaterPoint { get; }

  public Map(int sizeX, int sizeY, List<TreeLine> treeLines, Vector3 waterPoint)
  {
    SizeX = sizeX;
    SizeY = sizeY;
    TreeLines = treeLines;
    WaterPoint= waterPoint;
  }
  public Vector3 Clamp(Vector3 value)
  {
    float x = Math.Clamp(value.X, (SizeX / 2)*-1, SizeX / 2);
    float y = Math.Clamp(value.Y, (SizeY / 2) * -1, SizeY / 2);
    float z = value.Z;
    return new Vector3(x, y, z);
  }
  public TreeLine? GetTreeLineAtPosition(Vector3 position)
  {
    return TreeLines.FirstOrDefault(
      (TreeLine treeLine) => treeLine.isOnLine(position),
      null );
  }
}
