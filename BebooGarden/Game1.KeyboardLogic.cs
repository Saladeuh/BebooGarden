using BebooGarden.Content;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Minigame;
using BebooGarden.Save;
using BebooGarden.UI;
using CrossSpeak;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Myra.Graphics2D.UI;
using SharpHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace BebooGarden;

public partial class Game1
{
  public KeyboardState _previousKeyboardState;
  private MouseState _previousMouseState;

  public SaveParameters Save { get; private set; }

  private readonly object _keyLock = new();
  private readonly HashSet<Keys> _hookPressedKeys = new();
  private readonly HashSet<Keys> _keysToProcess = new(); // Nouvelles touches à traiter
  private bool _updateProcessed = false;

  private void HandleKeyboardNavigation(KeyboardState currentKeyboardState)
  {
    if (_gameState.CurrentScreen == GameScreen.game)
    {
      Item? itemUnderCursor = Map?.GetItemArroundPosition(PlayerPosition);
      Map?.IsInLake(PlayerPosition);
      if ((DateTime.Now - LastPressedKeyTime).TotalMilliseconds > 150)
      {
        if (currentKeyboardState.IsKeyDown(Keys.Left)
        || WASD && currentKeyboardState.IsKeyDown(Keys.A)
        || !WASD && currentKeyboardState.IsKeyDown(Keys.Q))
        {
          MoveOf(new Vector3(-1, 0, 0));
        }
        else if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D))
        {
          MoveOf(new Vector3(1, 0, 0));
        }
        else if (currentKeyboardState.IsKeyDown(Keys.Up)
          || WASD && IsKeyPressed(currentKeyboardState, Keys.W)
          || !WASD && currentKeyboardState.IsKeyDown(Keys.Z))
        {
          if (currentKeyboardState.IsKeyDown(Keys.Enter))
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
        else if (currentKeyboardState.IsKeyDown(Keys.Down)
          || WASD && currentKeyboardState.IsKeyDown(Keys.S)
          || !WASD && currentKeyboardState.IsKeyDown(Keys.S))
        {
          if (currentKeyboardState.IsKeyDown(Keys.Enter))
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
        LastPressedKeyTime = DateTime.Now;
      }
      if (IsKeyPressed(currentKeyboardState, Keys.F))
      {
        SayBebooState();
      }
      if (IsKeyPressed(currentKeyboardState, Keys.G))
      {
        SayBasketState();
      }
      if (IsKeyPressed(currentKeyboardState, Keys.T))
      {
        SayTickets();
      }
      if (IsKeyPressed(currentKeyboardState, Keys.Enter))
      {
        if (itemUnderCursor != null && itemUnderCursor.IsTakable) itemUnderCursor.Take();
        else if (!Race.IsARaceRunning && Save.Flags.UnlockShop && (Map?.IsArroundShop(PlayerPosition) ?? false))
        {
          ShowShop();
        }
        else if (!Race.IsARaceRunning && Save.Flags.UnlockSnowyMap && (Map?.IsArroundMapPath(PlayerPosition) ?? false))
          TravelBetwieen(MapPreset.garden, MapPreset.snowy);
        else if (!Race.IsARaceRunning && Save.Flags.UnlockUnderwaterMap && (Map?.IsArroundMapUnderWater(PlayerPosition) ?? false))
          TravelBetwieen(MapPreset.garden, MapPreset.underwater);
        else if (!Race.IsARaceRunning && Map?.Beboos.Count > 0 && (Map?.IsArroundRaceGate(PlayerPosition) ?? false))
        {
          ChooseBebooForRace();
        }
      }
      if (currentKeyboardState.GetPressedKeyCount() > 0)
      {
        int keyInt;
        var key = currentKeyboardState.GetPressedKeys()[0];
        if (Util.IsKeyDigit(key, out keyInt) && keyInt > 0)
        {
          if (Map != null && keyInt <= Map.Beboos.Count)
          {
            var sortedBeboos = new List<Beboo>(Map.Beboos);
            sortedBeboos.Sort(delegate (Beboo x, Beboo y) { return x.Age.CompareTo(y.Age); });
            var beboo = Map.Beboos[keyInt - 1];
            SoundSystem.Whistle(true, beboo.VoicePitch);
            CrossSpeakManager.Instance.Output(beboo.Name);
            beboo.Call(this, new EventArgs());
          }
        }
      }
      if (currentKeyboardState.IsKeyDown(Keys.Escape))
      {
        SwitchToScreen(GameScreen.MainMenu);
      }
      if (IsKeyPressed(currentKeyboardState, Keys.Space))
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
    }
    else if (_gameState.CurrentScreen == GameScreen.TalkDialog)
    {
      if (IsKeyPressed(currentKeyboardState, Keys.Escape, Keys.Space, Keys.Enter))
      {
        _talkDialog?.Next();
      }
      else if (currentKeyboardState.GetPressedKeyCount() > 0)
      {
        _talkDialog?.DisplayCurrentLine();
      }
    }
    else if (_aMenuShouldBeClosed &&
      _gameState.CurrentScreen == GameScreen.MainMenu || _gameState.CurrentScreen == GameScreen.Shop || _gameState.CurrentScreen == GameScreen.ChooseMenu)
    {
      SwitchToScreen(GameScreen.game);
      _aMenuShouldBeClosed = false;
    }
    else
    {
      HandleMenuNavigation(currentKeyboardState);
    }
  }

  private void HandleMenuNavigation(KeyboardState currentKeyboardState)
  {
    Widget focused = _desktop.FocusedKeyboardWidget;
    if (IsKeyPressed(currentKeyboardState, Keys.Down) || IsKeyPressed(currentKeyboardState, Keys.Right))
    {
      _desktop.FocusNext();
    }

    if (IsKeyPressed(currentKeyboardState, Keys.Up) || IsKeyPressed(currentKeyboardState, Keys.Left))
    {
      _desktop.FocusPrevious();
    }
    if (IsKeyPressed(currentKeyboardState, Keys.Enter))
    {
      if (focused is Button button)
      {
        button.DoClick();
      }
    }
  }

  public bool IsKeyPressed(KeyboardState currentKeyboardState, params Keys[] keys)
  {
    foreach (Keys key in keys)
    {
      if (currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key))
      {
        return true;
      }
    }
    return false;
  }

  private void OnKeyPressed(object sender, KeyboardHookEventArgs e)
  {
    if (!IsActive) return;
    Keys monogameKey = MonoUtil.ConvertKeyCodeToMonogameKey(e.Data.KeyCode);

    if (monogameKey != Keys.None)
    {
      lock (_keyLock)
      {
        _hookPressedKeys.Add(monogameKey);
        _keysToProcess.Add(monogameKey); // Ajouter aux touches à traiter
        _updateProcessed = false; // Réinitialiser le flag
      }
    }
  }

  private void OnKeyReleased(object sender, KeyboardHookEventArgs e)
  {
    if (!IsActive) return;
    Keys monogameKey = MonoUtil.ConvertKeyCodeToMonogameKey(e.Data.KeyCode);

    if (monogameKey != Keys.None)
    {
      lock (_keyLock)
      {
        _hookPressedKeys.Remove(monogameKey);
        // Do not withdraw from _KeStoprocess - These keys must be treated at least once      }
      }
    }
  }

}
