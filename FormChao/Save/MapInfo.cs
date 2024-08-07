using BebooGarden.GameCore.Item;

namespace BebooGarden.Save;

public class MapInfo
{
  public MapInfo(List<Item> items, int remainingFruits)
  {
    Items = items;
    RemainingFruits = remainingFruits;
  }

  public List<Item> Items { get; set; } = new();
  public int RemainingFruits { get; set; }
}
