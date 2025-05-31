using BebooGarden.Content;
using BebooGarden.GameCore.Item;
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
using System.Numerics;

namespace BebooGarden;

public partial class Game1
{
  private KeyboardState _previousKeyboardState;
  private MouseState _previousMouseState;

  public SaveParameters Save { get; private set; }

  private readonly object _keyLock = new();
  private readonly HashSet<Keys> _hookPressedKeys = new();
  private readonly HashSet<Keys> _keysToProcess = new(); // Nouvelles touches à traiter
  private bool _updateProcessed = false;
  public bool KeyboardSDFJKL = false;

  private void HandleKeyboardNavigation(KeyboardState currentKeyboardState)
  {
    Widget focused = _desktop.FocusedKeyboardWidget;
    /*
    // quit on escape key
    if (IsKeyPressed(currentKeyboardState, Keys.Escape))
    {
      if (_gameState.CurrentScreen != GameScreen.MainMenu)
      {
        SwitchToScreen(GameScreen.MainMenu);
      }
      else
      {
        Exit();
      }
    }
    // navigation keys (arrows, tab, entrance)
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
    */
    if (Race.IsARaceRunning || (DateTime.Now - LastPressedKeyTime).TotalMilliseconds < 150) return;
    LastPressedKeyTime = DateTime.Now;
    Item? itemUnderCursor = Map?.GetItemArroundPosition(PlayerPosition);
    Map?.IsInLake(PlayerPosition);
    if (currentKeyboardState.IsKeyDown(Keys.Left)
      || WASD && currentKeyboardState.IsKeyDown(Keys.A)
      || !WASD && currentKeyboardState.IsKeyDown( Keys.Q))
    {
      MoveOf(new Vector3(-1, 0, 0));
    }
    else if (currentKeyboardState.IsKeyDown( Keys.Right) || currentKeyboardState.IsKeyDown( Keys.D))
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
