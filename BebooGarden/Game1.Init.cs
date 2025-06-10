using BebooGarden.Save;
using CrossSpeak;
using Microsoft.Xna.Framework.Input;
using SharpHook;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.World;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using BebooGarden.Content;

namespace BebooGarden;

public partial class Game1
{
  private Map map;

  public Map? Map { get => map; private set => map = value; }
  public System.Numerics.Vector3 PlayerPosition { get; private set; }
  public DateTime LastPressedKeyTime { get; private set; }

  protected override void Initialize()
  {
    base.Initialize();
    this.Exiting += OnExit;
    CrossSpeakManager.Instance.Initialize();
    // create hook to get keyboard and simulated keyboard (e.g. screen readers inputs) 
    TaskPoolGlobalHook hook = new();
    hook.KeyPressed += OnKeyPressed;
    hook.KeyReleased += OnKeyReleased;
    Task.Run(() => hook.Run());
    _previousKeyboardState = Keyboard.GetState();
    _previousMouseState = Mouse.GetState();
    Save=SaveManager.LoadSave();
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
    SoundSystem.LoadMainScreen();
    /*
`
    if (!Save.Flags.NewGame)
    {
      CultureInfo.CurrentUICulture = new CultureInfo(parameters.Language);
      PlayerPosition = new Vector3(0, 0, 0);
      foreach (Map map in Map.Maps.Values)
      {
        if (map.TreeLines.Count > 0) map.TreeLines[0].SetFruitsAfterAWhile(Save.LastPlayed, Save.MapInfos[map.Preset].RemainingFruits);
        try
        {
          map.Items = Save.MapInfos[map.Preset].Items;
        }
        catch
        {
          map.Items = new();
        }
        try
        {
          foreach (BebooInfo bebooInfo in Save.MapInfos[map.Preset].BebooInfos)
          {
            if (bebooInfo.Name != "bob" && bebooInfo.Name != "boby")
            {
              BebooType bebootype;
              if (bebooInfo.BebooType != null && bebooInfo.BebooType != BebooType.Base) bebootype = bebooInfo.BebooType;
              else
              {
                bebootype = Random.Next(2) == 1 && Save.FavoredColor != "none"
                  ? Util.GetBebooTypeByColor(Save.FavoredColor)
                  : Util.GetRandomBebooType();
              }
              Beboo beboo = new(bebooInfo.Name, bebootype, bebooInfo.Age, Save.LastPlayed, bebooInfo.Happiness, bebooInfo.Energy, bebooInfo.SwimLevel, false, bebooInfo.Voice)
              {
                KnowItsName = bebooInfo.KnowItsName || bebooInfo.Age >= 2
              };
              map.Beboos.Add(beboo);
              if (map != Map) beboo.Pause();
            }
          }
        }
        catch (KeyNotFoundException _) { }
        if (map != Map) SoundSystem.Pause(map);
      }
    }
    else
    {
      PlayerPosition = new Vector3(-2, 0, 0);
      Map.AddItem(new Egg(Parameters.FavoredColor), new(2, 0, 0));
    }
    SoundSystem.LoadMap(Map);
    if (Save.Flags.NewGame) Welcome.AfterGarden();
    else UpdateMapMusic();
    SoundSystem.Music.Volume = Save.MusicVolume;
    LastPressedKeyTime = DateTime.Now;
    if (Save.FruitsBasket == null || Save.FruitsBasket.Count == 0)
    {
      Save.FruitsBasket = [];
      foreach (FruitSpecies fruitSpecies in Enum.GetValues(typeof(FruitSpecies))) Save.FruitsBasket[fruitSpecies] = 0;
    }
    */
  }

}
