using System.Numerics;

namespace BebooGarden;

public static class Util
{
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

    using (System.IO.StringReader reader = new System.IO.StringReader(input))
    {
      string line;
      while ((line = reader.ReadLine()) != null)
      {
        yield return line;
      }
    }
  }
}
