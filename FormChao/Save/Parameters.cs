using BebooGarden.GameCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BebooGarden.Save;

public class Parameters
{
  private string name;
  private DateTime now;
  private object value;

  public float Volume { get; set; }
  public string Language { get; set; }
  public Mood Mood { get; set; }
  public string BebooName { get; set; }
  public int Age { get; set; }
  public DateTime LastPayed { get; set; }

  public Parameters(string language, float volume, Mood mood, string bebooName, int age, DateTime lastPayed)
  {
    Volume = volume;
    Language = (language ?? "en");
    Mood = mood;
    BebooName = (bebooName ?? "");
    Age = age;
    LastPayed = lastPayed;
  }

  public Parameters()
  {
    Volume = 0.5f;
    Language = "en";
    Mood = Mood.None;
    BebooName = string.Empty;
    Age = 0;
    LastPayed = default(DateTime);
  }
}
