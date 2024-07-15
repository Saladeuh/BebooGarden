
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;

namespace BebooGarden.Save;

public class SaveParameters
{
  public float Volume { get; set; }
  public string? Language { get; set; }
  public string BebooName { get; set; }
  public int Age { get; set; }
  public DateTime LastPayed { get; set; }
  public float Energy { get; set; } 
  public Flags Flags { get; set; }
  public string PlayerName { get; set; }
  public string FavoredColor { get; internal set; }
  public SortedDictionary<FruitSpecies, int> FruitsBasket { get; set; }

  public SaveParameters(string? language, float volume, string bebooName, int age, float energy, DateTime lastPayed, Flags flags, string playerName, SortedDictionary<FruitSpecies,int> fruitsBasket)
  {
    Volume = volume;
    Language = language;
    Energy = energy;
    BebooName = (bebooName ?? "");
    Age = age;
    LastPayed = lastPayed;
    Flags = flags;
    PlayerName = playerName;
    FruitsBasket = fruitsBasket;
  }

  public SaveParameters()
  {
    Volume = 0.5f;
    Language = "en";
    BebooName = string.Empty;
    Age = 0;
    LastPayed = default(DateTime);
    Energy = 5;
    Flags = new();
    PlayerName = string.Empty;
    FavoredColor = "none";
    FruitsBasket = [];
  }
}
