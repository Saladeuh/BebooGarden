using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.World;
using BebooGarden.MiniGames;
using BebooGarden.Save;
using CrossSpeak;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BebooGarden;
public partial class Game1 : Game
{
  public readonly GameState _gameState;

  internal SoundSystem SoundSystem { get; }

  // Singleton
  public static Game1 Instance { get; private set; }
  public Random Random { get; set; }
  public IMiniGame? CurrentPlayingMiniGame { get; set; } = null;
  private bool _lastArrowWasUp;

  public Game1()
  {
    Config = new ConfigurationBuilder()
    .AddUserSecrets<Game1>()
    .Build();
    ScreenReader.Load();
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    Random = new Random();
    _gameState = new GameState();
    SoundSystem = new SoundSystem();
    Instance = this; // Set the static instance
  }

  private void OnExit(object sender, EventArgs e)
  {
    SaveManager.WriteSave(this.Save);
    CrossSpeakManager.Instance.Close();
  }
  public List<Item> Inventory { get; set; } = [];
  public Item? ItemInHand { get; set; }
  public IConfigurationRoot Config { get; private set; }

  public bool Wasd = false; // TODO InputLanguage.CurrentInputLanguage.Culture.TwoLetterISOLanguageName != "fr";
}