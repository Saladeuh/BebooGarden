using System.Numerics;
using BebooGarden.GameCore.Item;

namespace BebooGarden.GameCore.World;

public class Map(string name, int sizeX, int sizeY, List<TreeLine> treeLines, Vector3 waterPoint)
{
  public string Name { get; } = name;
  public int SizeX { get; set; } = sizeX;
  public int SizeY { get; set; } = sizeY;
  public List<TreeLine> TreeLines { get; } = treeLines;
  public Vector3 WaterPoint { get; } = waterPoint;
  public List<IItem> Items { get; set; } = new();
  public bool IsLullabyPlaying { get; set; } = false;
  public bool IsDansePlaying { get; set; } = false;
  public Vector3 Clamp(Vector3 value)
  {
    float x = Math.Clamp(value.X, SizeX / 2 * -1, SizeX / 2);
    float y = Math.Clamp(value.Y, (SizeY / 2) * -1, SizeY / 2);
    float z = value.Z;
    return new Vector3(x, y, z);
  }
  public TreeLine? GetTreeLineAtPosition(Vector3 position)
  {
    return TreeLines.FirstOrDefault(
      treeLine => treeLine.IsOnLine(position),
      null);
  }
  public bool AddItem(IItem item, Vector3 position)
  {
    if (GetTreeLineAtPosition(position) != null) return false;
    Items.Add(item);
    item.Position = position;
    return true;
  }
  public IItem? GetItemArroundPosition(Vector3 position)
  {
    if (Items.Count == 0) return null;
    return Items.FirstOrDefault(
     item => Util.IsInSquare((Vector3)item.Position, position, 1),
     null);
  }
}
