using System.Numerics;

namespace BebooGarden;

public static class Util
{
  public static readonly Vector3[] DIRECTIONS = [new(0, 1, 0), new(1, 0, 0), new(-1, 0, 0), new(0, -1, 0)];
  public static readonly string[] Colors =
      { "pink", "red", "orange", "yellow", "green", "blue", "indigo", "violet", "none" };

  public static bool IsInSquare(Vector3 otherPoint, Vector3 center, int halfSideSize)
  {
    var isInX = Math.Abs(otherPoint.X - center.X) <= halfSideSize;
    var isInY = Math.Abs(otherPoint.Y - center.Y) <= halfSideSize;
    return isInX && isInY;
  }

  public static IEnumerable<string> SplitToLines(this string input)
  {
    if (input == null) yield break;

    using (var reader = new StringReader(input))
    {
      while (reader.ReadLine() is { } line) yield return line;
    }
  }
}