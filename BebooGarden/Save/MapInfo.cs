using BebooGarden.GameCore.Item;
using System.Collections.Generic;

namespace BebooGarden.Save;

public class MapInfo
{
  public MapInfo(List<Item> items, int remainingFruits, List<BebooInfo> bebooInfos)
  {
    Items = items;
    RemainingFruits = remainingFruits;
    BebooInfos = bebooInfos;
  }

  public List<Item> Items { get; set; } = new();
  public List<BebooInfo> BebooInfos { get; set; } = new();
  public int RemainingFruits { get; set; }
}
