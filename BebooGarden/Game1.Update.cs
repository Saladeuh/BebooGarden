using BebooGarden.Content;
using BebooGarden.MiniGames;
using BebooGarden.Save;
using CrossSpeak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace BebooGarden;

public partial class Game1
{
  private bool _firstScreenTipsSayed = false;

  protected override void Update(GameTime gameTime)
  {
    KeyboardState nativeKeyboardState = Keyboard.GetState();
    List<Keys> allPressedKeys = nativeKeyboardState.GetPressedKeys().ToList();
    lock (_keyLock)
    {
      allPressedKeys.AddRange(_hookPressedKeys);
      if (!_updateProcessed)
      {
        allPressedKeys.AddRange(_keysToProcess);
        allPressedKeys = allPressedKeys.Distinct().ToList();
      }
    }
    KeyboardState currentKeyboardState = new(allPressedKeys.ToArray());
    MouseState currentMouseState = Mouse.GetState();

    if (!_firstScreenTipsSayed && gameTime.TotalGameTime.Seconds >= 2 && Save.Flags.NewGame)
    {
      _firstScreenTipsSayed = true;
      //CrossSpeakManager.Instance.Output(GameText.Tips);
    }
    _desktop.UpdateInput();
    HandleKeyboardNavigation(currentKeyboardState);
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
    if ((_gameState.CurrentScreen == GameScreen.BasicPractice || _gameState.CurrentScreen == GameScreen.ChoicePractice || _gameState.CurrentScreen == GameScreen.WordPractice)
      && !_gameState.IsPaused)
    {
      if (CurrentPlayingMiniGame.IsRunning)
      {
        CurrentPlayingMiniGame.Update(gameTime, currentKeyboardState);
      }
      else
      {
        SwitchToScreen(GameScreen.MainMenu);
        //CrossSpeakManager.Instance.Output(string.Format(GameText.Score, CurrentPlayingMiniGame..Score));
        CurrentPlayingMiniGame = null;
        SaveManager.WriteSave(Save);
      }
    }
    UpdateUIState();

    _previousKeyboardState = currentKeyboardState;
    _previousMouseState = currentMouseState;
    lock (_keyLock)
    {
      _updateProcessed = true;
      _keysToProcess.Clear(); // Vider les touches à traiter puisqu'elles ont été traitées
    }
    base.Update(gameTime);
  }
}
