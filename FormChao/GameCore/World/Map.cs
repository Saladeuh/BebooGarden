using System.Numerics;
using FmodAudio;
using Newtonsoft.Json;

namespace BebooGarden.GameCore.World;

public class Map(string name, int sizeX, int sizeY, List<TreeLine> treeLines, Vector3 waterPoint)
{
  public string Name { get; } = name;
  public int SizeX { get; set; } = sizeX;
  public int SizeY { get; set; } = sizeY;
  public List<TreeLine> TreeLines { get; } = treeLines;
  public Vector3 WaterPoint { get; } = waterPoint;
  public List<Item.Item> Items { get; set; } = new();
  public bool IsLullabyPlaying { get; set; } = false;
  public bool IsDansePlaying { get; set; } = false;
  [JsonIgnore]
  public List<Channel> WaterChannels { get; set; } = new();
  [JsonIgnore]
  public List<Channel> TreesChannels { get; set; } = new();
  [JsonIgnore]
  public Channel? BackgroundChannel { get; set; } 
  public Vector3 Clamp(Vector3 value)
  {
    var x = Math.Clamp(value.X, SizeX / 2 * -1, SizeX / 2);
    var y = Math.Clamp(value.Y, SizeY / 2 * -1, SizeY / 2);
    var z = value.Z;
    var newPos = new Vector3(x, y, z);
    return newPos;
  }
  public bool isInLake(Vector3 position) => Util.IsInSquare(position, WaterPoint, 5);
  public TreeLine? GetTreeLineAtPosition(Vector3 position)
  {
    return TreeLines.FirstOrDefault(
        treeLine => treeLine != null && treeLine.IsOnLine(position),
        null);
  }

  public bool AddItem(Item.Item item, Vector3 position)
  {
    if (GetTreeLineAtPosition(position) != null) return false;
    Items.Add(item);
    item.Position = position;
    return true;
  }

  public Item.Item? GetItemArroundPosition(Vector3 position)
  {
    if (Items == null || Items.Count == 0) return null;
    return Items.FirstOrDefault(item => item != null && item.Position != null && Util.IsInSquare(item.Position.Value, position, 1),
            null);
  }
}