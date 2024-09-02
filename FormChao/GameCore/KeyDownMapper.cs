using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Interface.EscapeMenu;
using BebooGarden.Interface.Shop;
using BebooGarden.Minigame;
using System.Numerics;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace BebooGarden.GameCore;

internal partial class Game
{
  public void KeyDownMapper(object sender, KeyEventArgs e)
  {
    if (Race.IsARaceRunning || (DateTime.Now - LastPressedKeyTime).TotalMilliseconds < 150) return;
    LastPressedKeyTime = DateTime.Now;
    Item.Item? itemUnderCursor = Map?.GetItemArroundPosition(PlayerPosition);
    Map?.IsInLake(PlayerPosition);
    Keys key = e.KeyCode;
    if (key is Keys.Left
      || WASD && key is Keys.A
      || !WASD && key is Keys.Q)
    {
      MoveOf(new Vector3(-1, 0, 0));
    }
    else if (key is Keys.Right or Keys.D)
    {
      MoveOf(new Vector3(1, 0, 0));
    }
    else if (key is Keys.Up
      || WASD && key is Keys.W
      || !WASD && key is Keys.Z)
    {
      if ((Keyboard.GetKeyStates(Key.Enter) & KeyStates.Down) > 0)
      {
        if (!_lastArrowWasUp)
        {
          ShakeOrPetAtPlayerPosition();
          _lastArrowWasUp = true;
        }
      }
      else
      {
        MoveOf(new Vector3(0, 1, 0));
      }
    }
    else if (key is Keys.Down
      || WASD && key is Keys.S
      || !WASD && key is Keys.S)
    {
      if ((Keyboard.GetKeyStates(Key.Enter) & KeyStates.Down) > 0)
      {
        if (_lastArrowWasUp)
        {
          ShakeOrPetAtPlayerPosition();
          _lastArrowWasUp = false;
        }
      }
      else
      {
        MoveOf(new Vector3(0, -1, 0));
      }
    }
    else if (key == Keys.F)
    {
      SayBebooState();
    }
    else if (key == Keys.G)
    {
      SayBasketState();
    }
    else if (key == Keys.T)
    {
      SayTickets();
    } else if (key == Keys.Enter)
    {
      if (itemUnderCursor != null && itemUnderCursor.IsTakable) itemUnderCursor.Take();
      else if (!Race.IsARaceRunning && Flags.UnlockShop && (Map?.IsArroundShop(PlayerPosition) ?? false)) new Shop().Show();
      else if (!Race.IsARaceRunning && Flags.UnlockSnowyMap && (Map?.IsArroundMapPath(PlayerPosition) ?? false))
        TravelBetwieen(MapPreset.garden, MapPreset.snowy);
      else if (!Race.IsARaceRunning && Flags.UnlockUnderwaterMap && (Map?.IsArroundMapUnderWater(PlayerPosition) ?? false))
        TravelBetwieen(MapPreset.garden, MapPreset.underwater);
      else if (!Race.IsARaceRunning && Map?.Beboos.Count > 0 && (Map?.IsArroundRaceGate(PlayerPosition) ?? false))
        StartRace();
    }
    else
    {
      int keyInt;
      if (Util.IsKeyDigit(key, out keyInt) && keyInt > 0)
      {
        if (Map != null && keyInt <= Map.Beboos.Count)
        {
          var sortedBeboos = new List<Beboo>(Map.Beboos);
          sortedBeboos.Sort(delegate (Beboo x, Beboo y) { return x.Age.CompareTo(y.Age); });
          var beboo = Map.Beboos[keyInt - 1];
          SoundSystem.Whistle(true, beboo.VoicePitch);
          ScreenReader.Output(beboo.Name);
          beboo.Call(sender, e);
        }
      }
      else if (key == Keys.Escape)
      {
        new EscapeMenu().Show();
      }
      else if (key == Keys.Space)
      {
        if (ItemInHand != null)
        {
          TryPutItemInHand();
        }
        else
        {
          Beboo? bebooUnderCursor = BebooUnderCursor();
          if (bebooUnderCursor != null)
          {
            if (bebooUnderCursor.Sleeping) Whistle();
            else FeedBeboo();
          }
          else if (Map?.GetTreeLineAtPosition(PlayerPosition) != null)
          {
          }
          else if (itemUnderCursor != null)
          {
            itemUnderCursor.Action();
          }
          else
          {
            Whistle();
          }
        }
      }
      else
      {
        CheckGlobalActions(key);
      }
    }
  }
}
