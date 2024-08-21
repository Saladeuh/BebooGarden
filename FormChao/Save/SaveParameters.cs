using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.World;

namespace BebooGarden.Save;

public class SaveParameters
{
  public SaveParameters(string? language, float volume,
      DateTime lastPayed, Flags flags, string playerName, SortedDictionary<FruitSpecies, int> fruitsBasket, List<Item> inventory, int tickets, List<string> unlockedRolls, string favoredColor, Dictionary<MapPreset, MapInfo> mapInfos, MapPreset currentMap, Dictionary<RaceType, double> raceScores, int raceTodayTries, int raceTotalWin)
  {
    Volume = volume;
    Language = language;
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
    RaceScores = raceScores;
    RaceTodayTries = raceTodayTries;
    RaceTotalWin = raceTotalWin;
  }

  public SaveParameters()
  {
    Volume = 0.5f;
    Language = "en";
    LastPlayed = default;
    Flags = new Flags();
    PlayerName = string.Empty;
    FavoredColor = "none";
    FruitsBasket = [];
  }

  public float Volume { get; set; }
  public string? Language { get; set; }
  public DateTime LastPlayed { get; set; }
  public Flags Flags { get; set; }
  public string PlayerName { get; set; }
  public string FavoredColor { get; internal set; }
  public SortedDictionary<FruitSpecies, int> FruitsBasket { get; set; }
  public int Tickets { get; set; }
  public List<Item> Inventory { get; set; } = new();
  public Dictionary<MapPreset, MapInfo> MapInfos { get; set; }
  public List<string> UnlockedRolls { get; set; } = new();
  public MapPreset CurrentMap {  get; set; }
  public Dictionary<RaceType, double> RaceScores { get; set; }
  public string FreeTime { get; set; }
  public string Dessert { get; set; }
  public int RaceTodayTries { get; set; }
  public int RaceTotalWin { get; set; }
} 