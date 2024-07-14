using System.Numerics;

namespace BebooGarden;

public static class Util
{
  public static readonly string[] Colors = new string[] { "pink", "red", "orange", "yellow", "green", "blue", "indigo", "violet", "none" }; 
  public static bool IsInSquare(Vector3 otherPoint, Vector3 center, int halfSideSize)
  {  
    bool isInX = Math.Abs(otherPoint.X - center.X) <= halfSideSize;
    bool isInY = Math.Abs(otherPoint.Y - center.Y) <= halfSideSize;
    return isInX && isInY;
  }
  public static IEnumerable<string> SplitToLines(this string input)
  {
    if (input == null)
    {
      yield break;
    }

    using (var reader = new StringReader(input))
    {
      while (reader.ReadLine() is { } line)
      {
        yield return line;
      }
    }
  }
}
