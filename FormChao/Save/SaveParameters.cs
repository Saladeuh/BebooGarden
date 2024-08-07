using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.World;

namespace BebooGarden.Save;

public class SaveParameters
{
  public SaveParameters(string? language, float volume, string bebooName, float age, float energy, int happiness,
      DateTime lastPayed, Flags flags, string playerName, SortedDictionary<FruitSpecies, int> fruitsBasket, List<Item> inventory, int tickets, List<string> unlockedRolls, string favoredColor, Dictionary<MapPreset, MapInfo> mapInfos, MapPreset currentMap)
  {
    Volume = volume;
    Language = language;
    Energy = energy;
    Happiness = happiness;
    BebooName = bebooName ?? "";
    Age = age;
    LastPlayed = lastPayed;
    Flags = flags;
    PlayerName = playerName;
    FruitsBasket = fruitsBasket;
    Inventory = inventory;
    Tickets = tickets;
    UnlockedRolls = unlockedRolls;
    FavoredColor = favoredColor;
    MapInfos = mapInfos;
    CurrentMap = currentMap;
  }

  public SaveParameters()
  {
    Volume = 0.5f;
    Language = "en";
    BebooName = string.Empty;
    Age = 0;
    LastPlayed = default;
    Energy = 5;
    Flags = new Flags();
    PlayerName = string.Empty;
    FavoredColor = "none";
    FruitsBasket = [];
  }

  public float Volume { get; set; }
  public string? Language { get; set; }
  public string BebooName { get; set; }
  public float Age { get; set; }
  public DateTime LastPlayed { get; set; }
  public float Energy { get; set; }
  public int Happiness { get; set; }
  public Flags Flags { get; set; }
  public string PlayerName { get; set; }
  public string FavoredColor { get; internal set; }
  public SortedDictionary<FruitSpecies, int> FruitsBasket { get; set; }
  public int Tickets { get; set; }
  public List<Item> Inventory { get; set; } = new();
  public Dictionary<MapPreset, MapInfo> MapInfos { get; set; }
  public List<string> UnlockedRolls { get; set; } = new();
  public MapPreset CurrentMap {  get; set; }
}