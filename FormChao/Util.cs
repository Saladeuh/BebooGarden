using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;

namespace BebooGarden;

public static class Util
{
  public static readonly Vector3[] DIRECTIONS = [new(0, 1, 0), new(1, 0, 0), new(-1, 0, 0), new(0, -1, 0)];
  public static readonly string[] Colors =
      { "pink", "red", "orange", "yellow", "green", "blue", "indigo", "violet", "none" };

  public static bool IsInSquare(Vector3 otherPoint, Vector3 center, int halfSideSize)
  {
    bool isInX = Math.Abs(otherPoint.X - center.X) <= halfSideSize;
    bool isInY = Math.Abs(otherPoint.Y - center.Y) <= halfSideSize;
    return isInX && isInY;
  }

  public static IEnumerable<string> SplitToLines(this string input)
  {
    if (input == null) yield break;

    using (StringReader reader = new(input))
    {
      while (reader.ReadLine() is { } line) yield return line;
    }
  }
  public static bool IsKeyDigit(Keys key, out int keyInt)
  {
    return int.TryParse(key.ToString().Replace("NumPad", "").Replace("D", ""), out keyInt);
  }
  public static BebooType GetRandomBebooType()
  {
    var bebooTypes = Enum.GetValues(typeof(BebooType));
    return (BebooType)bebooTypes.GetValue(Game.Random.Next(1, bebooTypes.Length));
  }

  public static BebooType GetBebooTypeByColor(string color)
  {
    switch (color)
    {
      case "none": return BebooType.Base;
      case "pink": return BebooType.Pink;
      case "red": return BebooType.Red;
      case "orange": return BebooType.Orange;
      case "yellow": return BebooType.Yellow;
      case "green": return BebooType.Green;
      case "blue": return BebooType.Blue;
      case "indigo": return BebooType.Indigo;
      case "violet": return BebooType.Violet;
      default: return BebooType.Base;
    }
  }
}