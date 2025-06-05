using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden;

public partial class Game1
{
  private Map? _backedMap;
  private void TravelBetwieen(MapPreset a, MapPreset b)
  {
    Map richedMap = Map;
    if (Map.Preset == a) richedMap = Map.Maps[b];
    else if (Map.Preset == b) richedMap = Map.Maps[a];
    List<Beboo> beboosHere = BeboosUnderCursor(2);
    foreach (Beboo transferedBeboo in beboosHere)
    {
      Map?.Beboos.Remove(transferedBeboo);
      transferedBeboo.Position = new(0, 0, 0);
      richedMap.Beboos.Add(transferedBeboo);
    }
    ChangeMap(richedMap);
    ChangeMapMusic();
    PlayerPosition = new(0, 0, 0);
  }
  public void ChangeMapMusic()
  {
    if (Map != null) SoundSystem.PlayMapMusic(Map);
  }

  internal void ChangeMap(Map map, bool backup = true)
  {
    if (Map != null) SoundSystem.Pause(Map);
    if (backup) _backedMap = Map;
    Map = map;
    SoundSystem.LoadMap(map);
    foreach (var otherMap in Map.Maps.Values)
      if (otherMap != map) SoundSystem.Pause(otherMap);
  }
  internal void LoadBackedMap()
  {
    if (_backedMap == null) return;
    SoundSystem.Pause(Map);
    Map = _backedMap;
    _backedMap = null;
    SoundSystem.Unpause(Map);
  }

}
