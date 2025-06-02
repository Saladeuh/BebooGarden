using BebooGarden.Content;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.MiniGames;
using BebooGarden.Save;
using BebooGarden.UI;
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
        SoundSystem.System.PlaySound(SoundSystem.JingleComplete);
        _talkDialog = new TalkDialog(GameText.unlocksnowy);
        _talkDialog?.Show();
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
    SetPreviousKeyboardStates(currentKeyboardState, currentMouseState);
    SoundSystem.System.Update();
    base.Update(gameTime);
  }

  private void SetPreviousKeyboardStates(KeyboardState currentKeyboardState, MouseState currentMouseState)
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
    if ((_gameState.CurrentScreen == GameScreen.ChoicePractice || _gameState.CurrentScreen == GameScreen.WordPractice)
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
  public void Pause()
  {
    foreach (Beboo beboo in Map?.Beboos) beboo.Pause();
    SoundSystem.DisableAmbiTimer();
    if (Map == null) return;
    SoundSystem.Pause(Map);
    SoundSystem.Music?.Paused = true;
  }
  public void Unpause()
  {
    foreach (Beboo beboo in Map?.Beboos) beboo.Unpause();
    SoundSystem.EnableAmbiTimer();
    if (Map == null) return;
    SoundSystem.Unpause(Map);
    SoundSystem.Music.Paused = false;
  }

}
