using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FmodAudio;

namespace BebooGarden.GameCore.World;

public class WaterPoint
{
  public Vector3 Position {  get; set; }
  public WaterPreset Preset { get; set; }
  public int Radius { get; set; }
  public Channel? Channel { get; set; }
  public WaterPoint(Vector3 position, WaterPreset preset=WaterPreset.Lagoon, int radius=5)
  {
    Position = position;
    Preset = preset;
    Radius=radius;
  }
}

public enum WaterPreset
{
  Lagoon,
  Sea,
}