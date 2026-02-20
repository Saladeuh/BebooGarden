using System.Numerics;

namespace BebooGarden.GameCore.World;

public class MapConnexion(Vector3 position, MapPreset mapPreset, string name)
{

  public Vector3 Position { get; set; } = position;
  public MapPreset MapPreset = mapPreset;
  public string Nme = name;
  public Map Map => Map.Maps[MapPreset];
}
