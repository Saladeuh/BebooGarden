using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FmodAudio;

namespace BebooGarden.GameCore.World;

public class WaterRectangle
{
  public Vector3 TopLeftPoint { get; set; }
  public WaterPreset Preset { get; set; }
  public int Length1 { get; set; }
  public int Length2 { get; set; }
  public Channel? Channel { get; set; }

  public WaterRectangle(WaterPreset preset, Vector3 topLeftPoint, int length1, int length2)
  {
    TopLeftPoint = topLeftPoint;
    Preset = preset;
    Length1 = length1;
    Length2 = length2;
  }
  public bool IsInRectangle(Vector3 position)
  {
    bool withinX = IsWithinX(position);
    bool withinY = IsWithinY(position);
    return withinX && withinY;
  }

  public bool IsWithinY(Vector3 position)
  {
    return position.Y <= TopLeftPoint.Y && position.Y >= TopLeftPoint.Y - Length2;
  }

  public bool IsWithinX(Vector3 position)
  {
    return position.X >= TopLeftPoint.X && position.X <= TopLeftPoint.X + Length1;
  }
  public Vector3 GetMiddlePoint()
  {
    return new Vector3(
      TopLeftPoint.X + (Length1 / 2),
      TopLeftPoint.Y - (Length2 / 2),
      TopLeftPoint.Z);
  }
}
public enum WaterPreset
{
  Lagoon,
  Sea,
}