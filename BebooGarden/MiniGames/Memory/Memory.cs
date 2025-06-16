using BebooGarden.Content;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.Interface;
using BebooGarden.MiniGames;
using CrossSpeak;
using FmodAudio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace BebooGarden.Minigame.memory;

internal class Memory : IMiniGame
{
  private static bool AlreadyPlaied = false;
  private Random _random;
  private List<string> _groups;
  private Level? _level;

  public FmodSystem System { get; }
  public SoundSystem SoundSystem { get; set; }
  private int MaxScore { get; set; }

  public bool IsRunning { get; set; }

  public string Tips => "";

  public int Score { get; set; }

  private readonly string CONTENTFOLDER = "Content/boombox/sounds/";

  public Memory(float volume)
  {
    SoundSystem = new SoundSystem(volume - 0.3f);
  }

  public void Start()
  {
    IsRunning = true;
    SoundSystem.FreeRessources();
    Game1.Instance.Pause();
    if (!AlreadyPlaied)
    {
      //IWindowManager.ShowTalk("welcome");
      //IWindowManager.ShowTalk("goal");
      AlreadyPlaied = true;
    }
    SoundSystem.LoadMenu();
    _random = new Random();
    _groups = Directory.GetDirectories(Path.Combine(BebooGarden.SoundSystem.CONTENTFOLDER, BebooGarden.SoundSystem.BEBOOSOUNDSFOLDER)).ToList();
    _groups.Concat(Directory.GetDirectories(CONTENTFOLDER).ToList());
    Score = 0;
    StartNewLevel();
  }

  public void Update(GameTime gameTime, KeyboardState currentKeyboardState)
  {
    _level?.Update(gameTime, currentKeyboardState);
    if ((_level?.Ended??false) && (_level?.Win ?? false))
    {
      Score++;
      _level = null;
      StartNewLevel();
    } else if((_level?.Ended??false) && !(_level?.Win??false))
    {
      _level = null;
      CrossSpeakManager.Instance.Output(String.Format(GameText.score, Score));
      End();
    }
  }

  private void End()
  {
    if (MaxScore < Score) MaxScore = Score;
    Game1.Instance.Unpause();
    SoundSystem.System.Release();
    IsRunning = false;
/*
    Game1.Instance.GainTicket((int)score / 4);
    beboo.Age += ((int)score / 5) * 0.1f;
    if (score > 0) beboo.Happiness++;
    else beboo.Happiness--;
  */
    //Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.ItemChestCloseSound, (Vector3)Position);
    Game1.Instance.CurrentPlayingMiniGame = null;
  }

  private void StartNewLevel()
  {
    var group1 = _groups[_random.Next(_groups.Count)];
    string? group2 = null;
    int nbSounds = 4;
    int maxRetry = 4;
    if (Score % 3 == 0 && Score != 0)
    {
      do
      {
        group2 = _groups[_random.Next(_groups.Count)];
      } while (group1 == group2);
    }
    else if (Score % 4 == 0 || Score == 0)
    {
      nbSounds = 3;
      group1 = _groups[0];
    }
    if (Score < 4)
    {
      maxRetry = 3;
    }
    _level = new Level(SoundSystem, nbSounds, maxRetry, group1, group2);
  }
}