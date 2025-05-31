using BebooGarden.Content;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.World;
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
    GetKeyStates(out KeyboardState currentKeyboardState, out MouseState currentMouseState);
    _desktop.UpdateInput();
    foreach (GameCore.Pet.Beboo beboo in Map?.Beboos)
    {
      if (Map.IsLullabyPlaying) beboo.GoAsleep();
      else if (Map.IsDansePlaying) beboo.WakeUp();
      if (Map.Preset != MapPreset.basicrace && Map.Preset != MapPreset.snowyrace && beboo.Racer) beboo.Pause();
      if (beboo.Age >= 2 && !Save.Flags.VoiceRecoPopupPrinted)
      {
        Save.Flags.VoiceRecoPopupPrinted = true;
        beboo.KnowItsName = true;
      }
      else if (beboo.Age >= 3 && !Save.Flags.UnlockSnowyMap)
      {
        Save.Flags.UnlockSnowyMap = true;
        //UnlockSnowyMap.Run();
        Map.Maps[MapPreset.snowy].AddItem(new Egg("none"), new(0, 0, 0));
        Save.Flags.UnlockEggInShop = true;
      }
      if (beboo.SwimLevel >= 10 && !Save.Flags.UnlockPerfectSwimming)
      {
        Save.Flags.UnlockPerfectSwimming = true;
        //UnlockSwimming.Run(beboo.Name);
        Save.Flags.UnlockUnderwaterMap = true;
        Map.Maps[MapPreset.underwater].AddItem(new Egg("blue"), new(0, 0, 0));
      }
    }
    HandleKeyboardNavigation(currentKeyboardState);
    UpdateMinigames(gameTime, currentKeyboardState);
    UpdateUIState();
    SetPReviousKeyboardStates(currentKeyboardState, currentMouseState);
    base.Update(gameTime);
  }

  private void SetPReviousKeyboardStates(KeyboardState currentKeyboardState, MouseState currentMouseState)
  {
    _previousKeyboardState = currentKeyboardState;
    _previousMouseState = currentMouseState;
    lock (_keyLock)
    {
      _updateProcessed = true;
      _keysToProcess.Clear(); // Vider les touches à traiter puisqu'elles ont été traitées
    }
  }

  private void UpdateMinigames(GameTime gameTime, KeyboardState currentKeyboardState)
  {
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
        CurrentPlayingMiniGame = null;
        SaveManager.WriteSave(Save);
      }
    }
  }

  private void GetKeyStates(out KeyboardState currentKeyboardState, out MouseState currentMouseState)
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
    currentKeyboardState = new(allPressedKeys.ToArray());
    currentMouseState = Mouse.GetState();
  }
}
