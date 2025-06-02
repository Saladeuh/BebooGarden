using BebooGarden.Content;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Minigame;
using BebooGarden.Save;
using BebooGarden.UI;
using CrossSpeak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
    CrossSpeakManager.Instance.Output(String.Format(GameText.tickets, Save.Tickets));
  }
  private void SayBasketState()
  {
    var fruits = 0;
    foreach (var fruistCount in Save.FruitsBasket.Values) fruits += fruistCount;
    if (Save.FruitsBasket != null) CrossSpeakManager.Instance.Output(String.Format(GameText.ui_basket, fruits));
  }
  private void TryPutItemInHand()
  {
    if (ItemInHand == null) return;
    bool waterProof = ItemInHand?.IsWaterProof ?? false;
    bool inWater = Map?.IsInLake(PlayerPosition) ?? false;
    if (inWater && !waterProof)
    {
      SoundSystem.System.PlaySound(SoundSystem.WarningSound);
      CrossSpeakManager.Instance.Output(GameText.ui_warningwater);
    }
    else
    {
      if (ItemInHand != null)
      {
        Map?.AddItem(ItemInHand, PlayerPosition);
        CrossSpeakManager.Instance.Output(String.Format(GameText.ui_itemput, ItemInHand.Name));
        if (inWater) SoundSystem.System.PlaySound(SoundSystem.ItemPutWaterSound);
        else SoundSystem.System.PlaySound(SoundSystem.ItemPutSound);
        Inventory.Remove(ItemInHand);
      }
      ItemInHand = null;
    }
  }
  private void Whistle()
  {
    SoundSystem.System.Get3DListenerAttributes(0, out Vector3 currentPosition, out _, out _, out _);
    SoundSystem.Whistle();
    foreach (Beboo beboo in Map?.Beboos)
    {
      if (Map?.Beboos.Count <= 1 || Random.Next(2) == 1)
      {
        Task.Run(async () =>
        {
          await Task.Delay(Random.Next(1000, 2000));
          beboo.WakeUp();
        });
        beboo.GoalPosition = currentPosition;
      }
    }
  }
  public void GainTicket(int amount)
  {
    if (amount > 0)
    {
      Save.Tickets += amount;
      CrossSpeakManager.Instance.Output(String.Format(GameText.gainticket, amount));
      SoundSystem.System.PlaySound(SoundSystem.MenuOk2Sound);
      if (!Save.Flags.UnlockShop && Map.Preset != MapPreset.snowyrace && Map.Preset != MapPreset.basicrace)
      {
        Save.Flags.UnlockShop = true;
        SoundSystem.System.PlaySound(SoundSystem.JingleComplete);
        new TalkDialog(GameText.shopunlock)
          .Show();
      }
    }
  }
}
