using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden;

public partial class Game1
{
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
    UpdateMapMusic();
    PlayerPosition = new(0, 0, 0);
  }
  public void UpdateMapMusic()
  {
    if (Map != null) SoundSystem.PlayMapMusic(Map);
  }
}
