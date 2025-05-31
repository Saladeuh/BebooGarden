using BebooGarden.Content;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Minigame;
using BebooGarden.Save;
using CrossSpeak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace BebooGarden;

public partial class Game1
{
  public void MoveOf(Vector3 movement)
  {
    if (Map == null) return;
    System.Numerics.Vector3 newPos = Map.Clamp(PlayerPosition + movement);
    if (newPos != PlayerPosition + movement) SoundSystem.System.PlaySound(SoundSystem.WallSound);
    else SoundSystem.PlayCursorSound();
    PlayerPosition = newPos;
    SoundSystem.MovePlayerTo(newPos);
    if (Map.IsInLake(newPos) && Map.Preset != MapPreset.underwater) CrossSpeak.CrossSpeakManager.Instance.Output(GameText.water);
    if (Save.Flags.UnlockShop && (Map?.IsArroundShop(PlayerPosition) ?? false)) CrossSpeakManager.Instance.Output(GameText.shop);
    else if (Save.Flags.UnlockSnowyMap && (Map?.IsArroundMapPath(PlayerPosition) ?? false)) CrossSpeakManager.Instance.Output(GameText.path);
    else if (Map?.IsArroundRaceGate(PlayerPosition) ?? false) 
      CrossSpeakManager.Instance.Output(String.Format(GameText.race_gate, Race.GetRemainingTriesToday()));
    else if (Save.Flags.UnlockUnderwaterMap && (Map?.IsArroundMapUnderWater(PlayerPosition) ?? false)) CrossSpeakManager.Instance.Output(GameText.underwater);
    SpeakObjectUnderCursor();
  }
  private void SpeakObjectUnderCursor()
  {
    TreeLine? treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    Item? item = Map?.GetItemArroundPosition(PlayerPosition);
    var beboosUnderCursor = BeboosUnderCursor(1);
    if (beboosUnderCursor.Count > 0)
      foreach (var beboo in beboosUnderCursor) ScreenReader.Output(beboo.Name);
    if (treeLine != null)
    {
      if (treeLine.Fruits == treeLine.FruitPerHour)
        CrossSpeakManager.Instance.Output(GameText.trees_full);
      else if (treeLine.Fruits == 0)
        CrossSpeakManager.Instance.Output(GameText.trees_empty);
      else if (treeLine.Fruits <= treeLine.FruitPerHour / 2)
        CrossSpeakManager.Instance.Output(GameText.trees_soonempty);
      else if (treeLine.Fruits >= treeLine.FruitPerHour / 2)
        CrossSpeakManager.Instance.Output(GameText.trees_soonfull);
    }
    else if (item != null) CrossSpeakManager.Instance.Output(item.Name);
  }

  private void ShakeOrPetAtPlayerPosition()
  {
    TreeLine? treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    Beboo? bebooUnderCursor = BebooUnderCursor();
    if (treeLine != null)
    {
      FruitSpecies? dropped = treeLine.Shake();
      if (dropped != null)
        if (Save.FruitsBasket != null)
          if (Save.FruitsBasket.TryGetValue(dropped.Value, out _)) Save.FruitsBasket[dropped.Value]++;
          else Save.FruitsBasket[dropped.Value] = 1;
    }
    else if (bebooUnderCursor != null)
    {
      bebooUnderCursor.GetPetted();
    }
  }
  public Beboo? BebooUnderCursor()
  {
    foreach (Beboo beboo in Map.Beboos)
    {
      if (Util.IsInSquare(beboo.Position, PlayerPosition, 1)) return beboo;
    }
    return null;
  }
  public List<Beboo> BeboosUnderCursor(int squareSide = 1)
  {
    List<Beboo> beboos = new();
    foreach (Beboo beboo in Map.Beboos)
    {
      if (Util.IsInSquare(beboo.Position, PlayerPosition, squareSide)) beboos.Add(beboo);
    }
    return beboos;
  }
  private void SayTickets()
  {
   CrossSpeakManager.Instance.Output(String.Format(GameText.tickets, Tickets));
  }

}
