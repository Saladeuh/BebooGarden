namespace BebooGarden.Save;

public class BebooInfo(string name, float age, int happiness, float energy)
{
  public string Name { get; set; } = name;
  public float Age { get; set; } = age;
  public int Happiness { get; set; } = happiness;
  public float Energy { get; set; } = energy;
}
