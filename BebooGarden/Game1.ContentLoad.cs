﻿using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.World;
using BebooGarden.Minigame;
using BebooGarden.Save;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Linq;
using System.Numerics;

namespace BebooGarden;

public partial class Game1
{
  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    Save = SaveManager.LoadSave();
    if (Save.RaceScores != null) Race.RaceScores = Save.RaceScores;
    Race.TotalWin = Save.RaceTotalWin;
    Race.TodayTries = Save.LastPlayed.Day == DateTime.Now.Day ? Save.RaceTodayTries : 0;
    Save.Flags.UnlockEggInShop = Save.Flags.UnlockUnderwaterMap || Save.Flags.UnlockSnowyMap || Save.Flags.UnlockEggInShop;
    try
    {
      if (Save.CurrentMap != MapPreset.basicrace && Save.CurrentMap != MapPreset.snowyrace)
        Map = Map.Maps[Save.CurrentMap];
      else
        Map = Map.Maps[MapPreset.garden];
    }
    catch (Exception) { Map = Map.Maps[MapPreset.garden]; }
    MusicBox.AvailableRolls = Save.UnlockedRolls ?? [];
    SoundSystem.Volume = Save.Volume;
    SoundSystem.LoadMainScreen();
    if (!Save.Flags.NewGame)
    {
    }
    else
    {
      PlayerPosition = new Vector3(-2, 0, 0);
      Map.AddItem(new Egg(Save.FavoredColor), new(2, 0, 0));
    }
    SoundSystem.Music?.Volume = Save.MusicVolume;
    LastPressedKeyTime = DateTime.Now;
    if (Save.FruitsBasket == null || Save.FruitsBasket.Count == 0)
    {
      Save.FruitsBasket = [];
      foreach (FruitSpecies fruitSpecies in Enum.GetValues(typeof(FruitSpecies))) Save.FruitsBasket[fruitSpecies] = 0;
    }
    Inventory = Save.Inventory;
    MediaPlayer.Volume = 0.3f;
    SoundEffect.MasterVolume = 1f;
    MyraEnvironment.Game = this;
    _desktop = new Desktop
    {
      HasExternalTextInput = true
    };
    Window.TextInput += (s, a) =>
    {
      _desktop.OnChar(a.Character);
    };
    ChangeMapMusic();
    Inventory.Add(new MusicBox());
    Inventory.Add(new Duck());
    Save.Tickets = 20;
    Save.Flags.UnlockShop = true;
    Save.Flags.UnlockSnowyMap = true;
    SwitchToScreen(GameScreen.game);
    //SwitchToScreen(Save.Flags.EmptySave ? GameScreen.First : GameScreen.MainMenu);
  }
}