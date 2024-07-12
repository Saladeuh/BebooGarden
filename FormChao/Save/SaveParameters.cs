
using BebooGarden.GameCore.Pet;

namespace BebooGarden.Save;

public class SaveParameters
{
  public float Volume { get; set; }
  public string? Language { get; set; }
  public Mood Mood { get; set; }
  public string BebooName { get; set; }
  public int Age { get; set; }
  public DateTime LastPayed { get; set; }
  public float Energy { get; set; } 
  public Flags Flags { get; set; }
  public string PlayerName { get; set; }
  public SaveParameters(string? language, float volume, BebooGarden.GameCore.Pet.Mood mood, string bebooName, int age, float energy, DateTime lastPayed, Flags flags, string playerName)
  {
    Volume = volume;
    Language = language;
    Mood = mood;
    Energy = energy;
    BebooName = (bebooName ?? "");
    Age = age;
    LastPayed = lastPayed;
    Flags = flags;
    PlayerName = playerName;
  }

  public SaveParameters()
  {
    Volume = 0.5f;
    Language = "en";
    Mood = Mood.None;
    BebooName = string.Empty;
    Age = 0;
    LastPayed = default(DateTime);
    Energy = 5;
    Flags = new();
    PlayerName = string.Empty;
  }
}
