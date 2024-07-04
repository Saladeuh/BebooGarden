using System.Numerics;

namespace BebooGarden;

public class Util
{
  public static bool IsInSquare(Vector3 otherPoint, Vector3 center, int halfSideSize)
  {  
    bool isInX = Math.Abs(otherPoint.X - center.X) <= halfSideSize;
    bool isInY = Math.Abs(otherPoint.Y - center.Y) <= halfSideSize;
    return isInX && isInY;
  }
}
