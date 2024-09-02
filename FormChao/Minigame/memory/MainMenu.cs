using BebooGarden.GameCore;
using BebooGarden.Interface;
using BebooGarden.Interface.ScriptedScene;
using FmodAudio;

namespace BebooGarden.Minigame.memory;

internal class MainMenu : IGlobalActions
{
  private static bool AlreadyPlaied = false;
  public FmodSystem System { get; }
  public SoundSystem SoundSystem { get; set; }
  private int MaxScore { get; set; }
  private readonly string CONTENTFOLDER = "Content/boombox/sounds/";

  public MainMenu(float volume)
  {
    SoundSystem = new SoundSystem(volume-0.3f);
  }

  public int PlayGame()
  {
    SoundSystem.FreeRessources();
    Game.GameWindow?.DisableInput();
    Game.Pause();
    if (!AlreadyPlaied)
    {
      IWindowManager.ShowTalk("welcome");
      IWindowManager.ShowTalk("goal");
      AlreadyPlaied = true;
    }
    SoundSystem.LoadMenu();
    var random = new Random();
    var groups = Directory.GetDirectories(CONTENTFOLDER).ToList();
    groups.Insert(0, BebooGarden.SoundSystem.CONTENTFOLDER + "sounds/beboo");
    int score = -1;
    do
    {
      score++;
      var group1 = groups[random.Next(groups.Count)];
      string? group2 = null;
      int nbSounds = 4;
      int maxRetry = 4;
      if (score % 3 == 0 && score != 0)
      {
        do
        {
          group2 = groups[random.Next(groups.Count)];
        } while (group1 == group2);
      }
      else if (score % 4 == 0 || score == 0)
      {
        nbSounds = 3;
        group1 = groups[0];
      }
      if (score < 4)
      {
        maxRetry = 3;
      }
      Level level = new(SoundSystem, nbSounds, maxRetry, group1, group2);
      level.ShowDialog(Game.GameWindow);
    } while (Level.Win);
    SayLocalizedString("score", score);
    if (MaxScore < score) MaxScore = score;
    Game.GameWindow?.EnableInput();
    Game.Unpause();
    SoundSystem.System.Release();
    return score;
  }
}