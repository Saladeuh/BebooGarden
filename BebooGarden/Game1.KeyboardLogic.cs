using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using SharpHook;
using System.Collections.Generic;
using BebooGarden.UI;
using CrossSpeak;
using Microsoft.Xna.Framework.Media;
using BebooGarden.Save;
using System;
using BebooGarden.Content;

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
    
    if (IsKeyPressed(currentKeyboardState, Keys.F1))
    {
      if (CurrentPlayingMiniGame != null)
      {
        //CrossSpeakManager.Instance.Output(CurrentPlayingMiniGame.Tips);
      }
      else
      {
        //CrossSpeakManager.Instance.Output(GameText.Tips);
      }
    }
    if (IsKeyPressed(currentKeyboardState, Keys.F2))
    {
      MediaPlayer.Volume -= 0.1f;
    }
    if (IsKeyPressed(currentKeyboardState, Keys.F3))
    {
      MediaPlayer.Volume += 0.1f;
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
